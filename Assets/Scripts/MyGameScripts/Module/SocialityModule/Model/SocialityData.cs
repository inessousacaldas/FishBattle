using System.Collections.Generic;
using ChatChannelEnum = AppDto.ChatChannel.ChatChannelEnum;

public interface ISocialityData
{
    IEmailData EmailData { get; }
    IChatInfoViewData ChatData { get; }
    IPrivateMsgData PrivateMsgData { get; }
    IFriendData FriendData { get; }
    ChatPageTab CurPageTab { get; }
}

public enum ChatPageTab{
    Chat  // 聊天
    , PrivateMsg  // 私信
    , Friend      // 好友
    , Email       // 邮箱
}

public sealed partial class SocialityDataMgr
{
    public sealed partial class SocialityData
        :ISocialityData
    {
        public IChatInfoViewData _IChatData;
        public IEmailData _IEmailData;
        public IPrivateMsgData _PrivateMsgData;
        public IFriendData _FriendData;

        public SocialityData()
        {

        }

        #region ClientData

        public bool delayShowMsg = false;
        
        private ChatPageTab _curPageTab;
        public ChatPageTab CurPageTab { 
            get{ return _curPageTab;}
            set{ _curPageTab = value;} 
        }
        #endregion

        #region Static Data
        

        #endregion

        public void InitData()
        {
            CurPageTab = ChatPageTab.Chat;
        }

        public void Dispose()
        {

        }

        public IChatInfoViewData ChatData {
            get { return _IChatData; }
        }
        
        public IEmailData EmailData {
            get { return _IEmailData; }
        }

        public IPrivateMsgData PrivateMsgData
        { get { return _PrivateMsgData; } }

        public IFriendData FriendData
        { get { return _FriendData; } }
    }

}
