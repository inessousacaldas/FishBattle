// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 10/18/2017 8:38:28 PM
// **********************************************************************

using AppDto;
using UniRx;
using Asyn;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using AssetPipeline;
using ChatChannelEnum = AppDto.ChatChannel.ChatChannelEnum;

namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner privateMsgDataMgr = new StaticDispose.StaticDelegateRunner(
            () => { var mgr = PrivateMsgDataMgr.DataMgr; });
    }
}

public sealed partial class PrivateMsgDataMgr : AbstractAsynInit
{
    public override void StartAsynInit(Action<IAsynInit> onComplete)
    {
        Action act = delegate ()
        {
            onComplete(this);
        };

        //先获取本地聊天记录
        LoadChatRecord();

        //再获取离线消息
        PrivateMsgNetMsg.ReqLoadMessage(act);
    }

    // 初始化
    private void LateInit()
    {
        //消息
        _disposable.Add(NotifyListenerRegister.RegistListener<ChatNotify>(HandleChatNotify));
        //好友度
        _disposable.Add(NotifyListenerRegister.RegistListener<FriendDegreeNotify>(HandleFriendDegreeNotify));
    }

    //处理好友私信
    private void HandleChatNotify(ChatNotify notify)
    {
        notify.content = ChatHelper.ReplaceAddrToUrl(notify.content);

        if (notify.channelId == (int)ChatChannel.ChatChannelEnum.Friend)
            _data.AddChatNotify(notify);

        FireData();
    }

    //其他系统不打开界面 直接发送消息 比如送花系统送花成功发私信 需要传入name 显示陌生人数据
    public void SendMessageWithoutView(long id, string name, string str)
    {
        PrivateMsgNetMsg.ReqTalkFriend(id, str, () =>
        {
            ChatNotify tempNotify = new ChatNotify();
            ShortPlayerDto tempDto = new ShortPlayerDto();
            tempDto.id = ModelManager.Player.GetPlayerId();
            tempNotify.channelId = (int)ChatChannelEnum.Friend;
            tempNotify.content = str;
            tempNotify.fromPlayer = tempDto;
            tempNotify.fromTime = SystemTimeManager.Instance.GetUTCTimeStamp();
            tempNotify.toPlayerId = id;
            tempNotify.itemId = 0;
            tempNotify.lableType = 0;
            tempNotify.actionType = 0;

            DataMgr._data.AddChatNotify(tempNotify);

            var infoDto = new FriendInfoDto();
            infoDto.friendId = id;
            infoDto.name = name;
            SetCurFriendDto(infoDto);
        });
     }

    //好友度
    private void HandleFriendDegreeNotify(FriendDegreeNotify notify)
    {
        FireData();
    }

    //自己发送消息 设置当前聊天对象 陌生人情况缓存陌生人数据
    public void SetCurFriendDto(FriendInfoDto dto)
    {
        _data.SetCurFriendId(dto.friendId);
        _data.AddFriendInfoDto(dto);
        _data.AddChatFriendQueueId(dto.friendId);
    }

    public void AddFriendInfoDto(FriendInfoDto dto)
    {
        _data.AddFriendInfoDto(dto);
    }

    public void SetCurFriendId(TeamMemberDto dto)
    {
        _data.SetCurFriendId(dto.id);
        var friendDto = new FriendInfoDto();
        friendDto.friendId = dto.id;
        friendDto.name = dto.nickname;
        friendDto.grade = dto.grade;
        friendDto.factionId = dto.factionId;
        friendDto.countryId = 0;
        friendDto.degree = 0;
        friendDto.online = true;
        _data.AddFriendInfoDto(friendDto);
    }

  
#if UNITY_EDITOR || UNITY_STANDALONE
    private string path = GameResPath.persistentDataPath + "/staticConfig/";
#elif UNITY_IPHONE
    private string path = GameResPath.persistentDataPath + "Raw/";
#elif UNITY_ANDROID
    private string path = GameResPath.persistentDataPath + "/staticConfig/";
#endif


    public static void SaveChatRecord()
    {
        if (_ins == null)
            return;
        _ins.SaveRecord();
    }
    
    private void SaveRecord(){
    XmlDocument xml = new XmlDocument();
        string xmlPath = path + "chatRecord_" + ModelManager.Player.GetPlayerId() + ".xml";
        //TipManager.AddTip(xmlPath);
        if (Directory.Exists(xmlPath))
            File.Delete(xmlPath);

        //基本信息
        XmlElement root = xml.CreateElement("ChatRecord");
        root.SetAttribute("Name", "FriendChatRecord");
        root.SetAttribute("Version", "1.0.0");

        //TipManager.AddTip("@@@@create XML~~~~~~~~");
        #region 聊天数据
        XmlElement elementRecord = xml.CreateElement("Record");
        _data.FriendMsgDic.ForEach(item =>
        {
            XmlElement element = xml.CreateElement("Friends");
            element.SetAttribute("Id", item.Key.ToString());

            item.Value.ForEach(chatNotify =>
            {
                XmlElement childElement = xml.CreateElement("MsgDetail");
                childElement.SetAttribute("fromPlayerId", chatNotify.fromPlayer.id.ToString());
                childElement.SetAttribute("toPlayerId", chatNotify.toPlayerId.ToString());
                childElement.SetAttribute("fromTime", chatNotify.fromTime.ToString());
                childElement.SetAttribute("content", chatNotify.content.ToString());
                element.AppendChild(childElement);
            });
            elementRecord.AppendChild(element);
        });
        root.AppendChild(elementRecord);

        #endregion
        //TipManager.AddTip("@@@@聊天数据 XML~~~~~~~~");
        #region 聊天列表信息
        //聊天好友顺序 最多20个
        XmlElement elementSort = xml.CreateElement("Sort");
        string sortText = string.Empty;
        _data.ChatFriendQueue.ForEach(id =>
        {
            elementSort.InnerText += string.Format("{0};", id);
        });
        root.AppendChild(elementSort);

        //未读消息保存id
        XmlElement elementNoRead = xml.CreateElement("NoRead");
        string NoReadText = string.Empty;
        _data.NoReadQueue.ForEach(id =>
        {
            elementNoRead.InnerText += string.Format("{0};", id);
        });
        root.AppendChild(elementNoRead);
        #endregion
        //TipManager.AddTip("@@@@聊天列表 XML~~~~~~~~");
        #region 陌生人数据
        XmlElement elementStrangerData = xml.CreateElement("StrangerData");
        _data.StrangerInfoDtoList.ForEach(item =>
        {
            XmlElement elementStranger = xml.CreateElement("Stranger");
            elementStranger.SetAttribute("Id", item.friendId.ToString());
            elementStranger.SetAttribute("Name", item.name);
            //头像 todo
            elementStranger.SetAttribute("HeadId", item.charactorId.ToString());

            elementStrangerData.AppendChild(elementStranger);
        });
        root.AppendChild(elementStrangerData);
        #endregion
        //TipManager.AddTip("@@@@陌生人数据 XML~~~~~~~~");
        xml.AppendChild(root);
        xml.Save(xmlPath);
        //TipManager.AddTip("@@@@保存保存！！ XML~~~~~~~~");
    }

    public void LoadChatRecord()
    {
        XmlDocument xml = new XmlDocument();
        string xmlPath = path + "chatRecord_" + ModelManager.Player.GetPlayerId() + ".xml";
        //TipManager.AddTip("@@@@LoadChatRecord！"+ xmlPath);
        if (!File.Exists(xmlPath))
            return;

        //TipManager.AddTip("##@找到文件！！！！！");
        xml.Load(xmlPath);
        XmlNode rootNode = xml.SelectSingleNode("ChatRecord");
        XmlNode xmlNodeRecord = rootNode.SelectSingleNode("Record");
        if (xmlNodeRecord == null || xmlNodeRecord.ChildNodes==null)
            return;

        //TipManager.AddTip("@@#@文件不为空！！！！！！");
        Dictionary<int, GeneralCharactor> charactorDic = DataCache.getDicByCls<GeneralCharactor>();
        #region 优先处理陌生人信息 以便后面聊天记录对notify中的friendDto赋值
        XmlNode xmlNodeStrangerData = rootNode.SelectSingleNode("StrangerData");
        if (xmlNodeStrangerData == null)
            return;
        XmlNodeList xmlNodeStrangerList = xmlNodeStrangerData.ChildNodes;
        foreach (XmlElement strangerElement in xmlNodeStrangerList)
        {
            var infoDto = new FriendInfoDto();
            if (string.IsNullOrEmpty(strangerElement.GetAttribute("Id")) || string.IsNullOrEmpty(strangerElement.GetAttribute("Name"))
                || string.IsNullOrEmpty(strangerElement.GetAttribute("HeadId")))
                continue;
            infoDto.friendId = long.Parse(strangerElement.GetAttribute("Id"));
            infoDto.name = strangerElement.GetAttribute("Name");
            infoDto.charactorId = StringHelper.ToInt(strangerElement.GetAttribute("HeadId"));
            if (charactorDic.ContainsKey(infoDto.charactorId))
                infoDto.charactor = charactorDic[infoDto.charactorId];
            _data.AddFriendInfoDto(infoDto);
        }
        #endregion

        #region 聊天记录
        XmlNodeList xmlNodeList = xmlNodeRecord.ChildNodes;
        int index = 0;
        foreach(XmlElement itemElement in xmlNodeList)
        {
            long id = long.Parse(itemElement.GetAttribute("Id"));
            var queue = new Queue<ChatNotify>();
            //TipManager.AddTip("@@#@有个人记录~~~~~~~  id是" + id);

            foreach (XmlElement childElement in itemElement.ChildNodes)
            {
                var itemChatNotify = new ChatNotify();
                ShortPlayerDto tempDto = new ShortPlayerDto();
                tempDto.id = long.Parse(childElement.GetAttribute("fromPlayerId"));
                itemChatNotify.channelId = (int)ChatChannelEnum.Friend;
                itemChatNotify.content = childElement.GetAttribute("content");
                itemChatNotify.fromPlayer = tempDto;
                itemChatNotify.fromTime = long.Parse(childElement.GetAttribute("fromTime"));
                itemChatNotify.toPlayerId = long.Parse(childElement.GetAttribute("toPlayerId"));

                queue.Enqueue(itemChatNotify);

                //TipManager.AddTip("@@#@此人有个聊天记录~~~~~~~  fromId是" + tempDto.id);
            }

            
            _data.FriendMsgDic.AddOrReplace(id, queue);
            index++;
        }
        #endregion
        //TipManager.AddTip("@@#@缓存一个聊天记录！！！！！");
        #region 聊天好友顺序 未读消息
        XmlNode xmlSort = rootNode.SelectSingleNode("Sort");
        if(xmlSort != null)
        {
            var strSort = xmlSort.InnerText;
            if(_data == null)
            {
                GameDebuger.LogError("~~~~~~~~privateData been release OR not Init??!!@@@@###"+ this.GetHashCode());
                return;
            }
            _data.ResetChatFriendQueue(strSort);
        }

        //未读消息
        XmlNode xmlNoRead = rootNode.SelectSingleNode("NoRead");
        if(xmlNoRead != null)
        {
            var strNoRead = xmlNoRead.InnerText;
            _data.ResetNoReadQueue(strNoRead);
        }
        #endregion
    }

    //清空好友聊天记录
    public void ClearRecordById(long friendId)
    {
        _data.ClearRecordById(friendId);

        FireData();
    }

    public void OnReadFriendMsg(long id)
    {
        _data.OnReadFriendMsg(id);
    }

    public long GetCurChatLastTime(int idx)
    {
        if (idx <= 0)
            return 0;

        var chatList = _data.CurChatList.ToList();
        //间隔大于10分钟
        if (chatList.Count > idx && chatList[idx].fromTime - chatList[idx-1].fromTime > 10*60*1000)
        {
            return chatList[idx].fromTime;
        }

        return 0;
    }

    public FriendInfoDto GetInfoDtoById(long id)
    {
        return _data.GetInfoDtoById(id);
    }

    public void OnDispose()
    {

    }
}
