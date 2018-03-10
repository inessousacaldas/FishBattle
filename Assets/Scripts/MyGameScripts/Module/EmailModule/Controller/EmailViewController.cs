// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EmailViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;
using AppDto;
using AppServices;
using UniRx;
using EmailNetMsg = EmailDataMgr.EmailNetMsg;

public partial class EmailViewController
{

    private CompositeDisposable _disposable;
    private PlayerMailDto _mailDto;
    private bool _outTime = false;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
        InitAttachItem();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        _disposable.Add(OnGetBtn_UIButtonClick.Subscribe(_ => GetOrDelBtn_UIButtonClickHandler()));
        _disposable.Add(EmailDataMgr.Stream.Subscribe(data => UpdateView(data)));
        //EventDelegate.Set(_view.ContentRegion_UIEventTrigger.onClick, OnEmailContentClick);
        CloseBtn_UIButtonEvt.Subscribe(_ =>
        {
            UIModuleManager.Instance.CloseModule(EmailView.NAME);
        });
    }

    protected override void OnDispose()
    {
        _attatchmentCtlList = null;

        if (_disposable != null)
            _disposable.Dispose();
        _disposable = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        //EventDelegate.Remove(_view.ContentRegion_UIEventTrigger.onClick, OnEmailContentClick);
    }


    //用特定接口类型数据刷新界面
    public void UpdateView(IEmailData data)
    {
        if (data == null || data.CurMailDto == null) return;
        _mailDto = data.CurMailDto;
        _outTime = data.IsOutTime;

        _view.BtnLabel_UILabel.text = _mailDto.hasAttachments ? "领取" : "删除";
        _view.InBg_2_UISprite.gameObject.SetActive(_mailDto.hasAttachments);
        _view.ContentLabel_UILabel.text = _mailDto.content;
        _view.TitleLabel_UILabel.text = _mailDto.title;

        //邮件过期
        if (_outTime)
        {
            SetBtnEnable(false);
        }

        if (_mailDto.mailTypeId == 5)
        {
            //帮派公告，不显示时效和寄件人
            _view.TimeLabel_UILabel.enabled = false;
            _view.SenderLabel_UILabel.enabled = false;
        }
        else
        {
            _view.TimeLabel_UILabel.text = DateUtil.GetDateStr(_mailDto.sendTime);
            _view.SenderLabel_UILabel.text = "系统";
        }

        //添加附件物品到Table下面                
        for (int i = 0; i < EmailDataMgr.EmailData.ATTACHMENT_MAXCOUNT; i++)
        {
            if (i < _mailDto.attachments.Count)
            {
                _attatchmentCtlList[i].UpdateView(_mailDto.attachments[i]);                     //更新附件显示
                _attatchmentCtlList[i].Show();
            }
            else
            {
                _attatchmentCtlList[i].Hide();
            }
        }
        View.Grid_UIGrid.Reposition();
    }

    private void GetOrDelBtn_UIButtonClickHandler()
    {
        if (_mailDto.hasAttachments)
        {
            if (_outTime)
            {
                TipManager.AddTip("邮件过期");
                return;
            }
            //像服务器发送领取附件请求
            EmailNetMsg.ReqExtract(_mailDto.id, OnExtractSuccess, OnExtractFail);
            SetBtnEnable(false);
        }
        else
        {
            //删除邮件
            EmailNetMsg.ReqDelMail(_mailDto.id, () => HideEmailView());
        }
    }


    private List<EmailAttatchItemController> _attatchmentCtlList;
    //初始化邮件附件界面
    private void InitAttachItem()
    {
        if (_attatchmentCtlList == null)
        {
            _attatchmentCtlList = new List<EmailAttatchItemController>();
            for (int i = 0; i < EmailDataMgr.EmailData.ATTACHMENT_MAXCOUNT; i++)
            {
                var ctrl = AddChild<EmailAttatchItemController, EmailAttatchItem>(
                    View.Grid_UIGrid.gameObject, EmailAttatchItem.NAME
                    );
                _attatchmentCtlList.Add(ctrl);
                ctrl.Hide();
            }
        }
    }


    //开启邮件面板时，初始化数据显示
    public void SetData(PlayerMailDto dto)
    {
        _mailDto = dto;
    }

    private void SetBtnEnable(bool b)
    {
        _view.GetBtn_UIButton.enabled = b;
        _view.GetBtn_UIButton.sprite.isGrey = !b;
    }

    #region 回调

    //提取附件回调
    private void OnExtractSuccess()
    {
        //TipManager.AddTip("领取成功");
        SetBtnEnable(true);         //使Btn可以点击

        if (_mailDto.mailType.readDelete)
        {
            EmailNetMsg.ReqDelMail(_mailDto.id);
        }
        else
        {
            _view.BtnLabel_UILabel.text = "删除";
        }
    }
    
    private void OnExtractFail(ErrorResponse obj)
    {
        SetBtnEnable(true);
        TipManager.AddTip("包裹不足，领取失败");
    }

    private void HideEmailView()
    {
        this.Hide();
    }

    #endregion

    #region 超链接
    //点击邮件内容超链接
    private void OnEmailContentClick()
    {
        string urlStr = _view.ContentLabel_UILabel.GetUrlAtPosition(UICamera.lastWorldPosition);
        if (!string.IsNullOrEmpty(urlStr))
        {
            //  超链接  TODO      聊天模块中    TODO
            // ModelManager.Chat.DecodeUrlMsg(urlStr);
        }
    }
    #endregion

}
