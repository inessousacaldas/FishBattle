using System;
using System.Collections.Generic;
using AppDto;
using UniRx;

using AbstractAsynInit = Asyn.AbstractAsynInit;
using IAsynInit = Asyn.IAsynInit;

namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner emailDataMgr = new StaticDispose.StaticDelegateRunner(
            ()=> { var mgr = EmailDataMgr.DataMgr; } );
    }
}

public sealed partial class EmailDataMgr :AbstractAsynInit
{
    public override void StartAsynInit(Action<IAsynInit> onComplete){
        Action act = delegate ()
        {
            onComplete(this);
        };
        EmailNetMsg.ReqMailList(act, act);
    }

    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<MailChangeIdsNotify>(HandleNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<PlayerMailDto>(AddMailDto));
    }
    
    private void OnDispose(){
            
    }

    //登录时获取邮件列表信息
    public void AcquireEmailDtosSuccess(GeneralResponse e)
    {
        MailsDto dataList = e as MailsDto;
        if (dataList != null)
        {
            var mailDtoList = dataList.mails;
            _data._mailList = mailDtoList;
        }
    }

    //DTO监听，收到增加的邮件DTO就走增加逻辑
    public void AddMailDto(PlayerMailDto dto)
    {
        if (dto == null) return;

        //这里可以判断背包是否满了，然后给提示说背包满了，通过邮件接收        TODO
        _data._mailList.Insert(0, dto);       //客户端缓存
//        _data._mailList.Sort(CustomSort);

        //刷新界面
        FireData();
    }

    //处理服务端传回的邮件idList                       TODO
    public void HandleNotify(MailChangeIdsNotify notify)
    {
        if (notify != null)
        {
            //领取附件成功
            if (notify.extractmailIds != null)
            {
                notify.extractmailIds.ForEach<long>(id =>
                {
                    var dto = _data.MailDtoList.Find(mail => mail.id == id);
                    if (dto != null)
                    {
                        dto.hasAttachments = false;
                        dto.read = true;
                        dto.attachments.Clear();
                    }
                });
            }

            //删除超出存储范围的邮件
            if (notify.deletemailIds != null)
            {
                notify.deletemailIds.ForEach<long>(id =>
                {
                    var dto = _data.MailDtoList.Find(mail => mail.id == id);
                    if (dto != null)
                    {
                        _data._mailList.Remove(dto);
                        _data.deleteMailIds.Add(id);
                    }
                });
            }
            FireData();
        }     
    }
    
    //删除所有已读邮件数据
    public void RemoveAllReadedMail()
    {
        List<PlayerMailDto> tempList = new List<PlayerMailDto>();
        _data.MailDtoList.ForEach(dto =>
        {
            if (dto.read && !dto.hasAttachments)
                tempList.Add(dto);
        });

        tempList.ForEach(dto =>
        {
            _data._mailList.Remove(dto);
        });

        tempList.Clear();
        _data.isDeleteAll = true;

        MailSort(_data._mailList);
        FireData();
    }

    //删除当前邮件
    public void RemoveCurrentMail()
    {
        if (_data.CurMailDto == null) return;

        _data._mailList.Remove(_data.CurMailDto);
        if (!_data._mailList.IsNullOrEmpty())
        {
            if (_data.CurMailIdx >= _data._mailList.Count)
            {
                _data.CurMailIdx = -1;
                _data.CurMailDto = null;
            }
            else
            {
                PlayerMailDto dto;
                _data._mailList.TryGetValue(_data.CurMailIdx, out dto);
                _data.CurMailDto = dto;
            }
        }
        else
        {
            _data.CurMailIdx = -1;
            _data.CurMailDto = null;
        }

        FireData();
    }

    //获取附件成功
    public void GetAttachmentSuccess()
    {
        foreach (var tempDto in _data.MailDtoList)
        {
            if (tempDto == _data.CurMailDto)
            {
                tempDto.read = true;
                tempDto.hasAttachments = false;
                tempDto.attachments.Clear();
                break;
            }
        }

        //FireData();
    }

    //设置当前选中邮件及索引
    public PlayerMailDto SetAndGetCurMail( int idx)
    {
        PlayerMailDto dto = null;
        _data._mailList.TryGetValue(idx, out dto);
        _data.CurMailDto = dto;
        _data.CurMailIdx = idx;
        _data.isOutTime = CheckEmailTimeOut(dto);
        return dto;
    }

    //将当前选中邮件设为已读
    public void ReadMail()
    {
        var dto = _data.MailDtoList.Find(i => i == _data.CurMailDto);   //待处理
        if (dto != null)
        {
            dto.read = true;
            FireData();
        }
    }

    //检查邮件是否过期
    private bool CheckEmailTimeOut(PlayerMailDto dto)
    {
        DateTime curTime = SystemTimeManager.Instance.GetServerTime();
        DateTime expirationTime = DateUtil.UnixTimeStampToDateTime(dto.sendTime);
        expirationTime = expirationTime.AddDays(dto.mailType.saveDate);
        return DateTime.Compare(curTime, expirationTime) > 0;
    }

    // dodo fish: 需要问策划这个刷新时间点，打开界面还是切分页
    public void RemoveOutTimeMail()
    {
        //如果邮件过期则删除
        _data.MailDtoList.ForEach(dto =>
        {
            if (CheckEmailTimeOut(dto) || (dto.mailType.readDelete && dto.read))
            {
                _data._mailList.Remove(dto);
            }
        });
        FireData();
    }

    public int GetNoReadCount()
    {
        var noReadCount = 0;
        _data.MailDtoList.ForEach(dto =>
        {
            if (!dto.read)
                noReadCount++;
        });

        return noReadCount;
    }

    //邮件排序
    private static Comparison<PlayerMailDto> _comparison = null;
    private static List<PlayerMailDto> MailSort(List<PlayerMailDto> list)
    {
        for (var i = 0; i < list.Count; i++)
        {
            for (var j = i; j < list.Count; j++)
            {
                if (list[i].read == list[j].read)
                {
                    if (list[i].sendTime < list[j].sendTime ||
                        list[i].sendTime == list[j].sendTime && list[i].hasAttachments != list[j].hasAttachments && list[j].hasAttachments)
                    {
                        var temp = list[i];
                        list[i] = list[j];
                        list[j] = temp;
                    }
                }
                else if (list[i].read)
                {
                    var temp = list[i];
                    list[i] = list[j];
                    list[j] = temp;
                }
            }
        }

        return list;
    }

    public void SetSortEmail()
    {
        MailSort(_data._mailList);
    }

}
