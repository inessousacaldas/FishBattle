using System;
using AppDto;
using UniRx;
using ChatChannelEnum = AppDto.ChatChannel.ChatChannelEnum;

namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner chatDataMgr = new StaticDispose.StaticDelegateRunner(
            () => 
            {
                var mgr = ChatDataMgr.DataMgr;
            });
    }
}

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeChatDataMgr = new StaticDelegateRunner(
            ChatDataMgr.ExecuteDispose);
    }
}

public sealed partial class ChatDataMgr
{
    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<ChatNotify>(HandleChatNotify));
        _disposable.Add(FullServiceAnnouncementModel.Stream.Subscribe(HandleMarqueeNoticeNotify));
        #region 模拟消息推送
        _disposable.Add(NotifyListenerRegister.RegistListener<ItemTipsNotify>(MakeChatNotify));//奖励
        _disposable.Add(NotifyListenerRegister.RegistListener<FriendOnlineNotify>(MakeChatNotify));//好友上线
        _disposable.Add(NotifyListenerRegister.RegistListener<TeamShoutNotify>(MakeChatNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<FriendFlowersNotify>(MakeChatNotify)); //收到鲜花
        #endregion
        //InitVoiceEvent();
    }



    public void OnDispose(){
       
    }
    //处理聊天信息
    private void HandleChatNotify(ChatNotify notify)
    {
        notify.content = ChatHelper.ReplaceAddrToUrl(notify.content);

        _data.AddChatNotify(notify);
        GameEventCenter.SendEvent(GameEvent.CHAT_MSG_NOTIFY, notify);
        FireData();
    }
    //模拟添加消息
    private void AddChatNotifySim(ChatNotify notify)
    {
        _data.AddChatNotify(notify);
        FireData();
    }

    //其他系统直接调用发送系统消息
    public void SendSystemMessage(ChatNotify notify)
    {
        _data.AddChatNotify(notify);
        FireData();
    }

    /// <summary>
    /// 走马灯的消息处理
    /// </summary>
    /// <param name="marNotify"></param>
    private void HandleMarqueeNoticeNotify(MarqueeNoticeNotify marNotify)
    {
       var chatNotify = new ChatNotify(){
           channelId = (int)ChatChannel.ChatChannelEnum.System,
           content = marNotify.title + marNotify.content,
           lableType = (int)ChatChannel.LableTypeEnum.System
       };
       _data.AddChatNotify(chatNotify);
        GameEventCenter.SendEvent(GameEvent.CHAT_MSG_NOTIFY, chatNotify);
        FireData();
    }


    #region Check 检查
    /// <summary>
    /// 检查是否接受
    /// </summary>
    /// <param name="notifyChannelId"></param>
    /// <returns></returns>
    private bool CheckReceive(ChatChannelEnum notifyChannelId)
    {
        bool value;
        DataMgr._data.chatSettingData.channelShowDic.TryGetValue(notifyChannelId.ToString(), out value);
        return value;
    }

    /// <summary>
    /// 检查是否自动播放
    /// </summary>
    /// <param name="channelId"></param>
    /// <returns></returns>
    private bool CheckAutoPlay(ChatChannelEnum channelId)
    {
        switch (channelId)
        {
            case ChatChannelEnum.Team:
                return SystemSettingDataMgr.Instance.AutoPlayTeamVoice;
            default: return false;
        }
    }
    #endregion


    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
        _ins = null;
    }
}
