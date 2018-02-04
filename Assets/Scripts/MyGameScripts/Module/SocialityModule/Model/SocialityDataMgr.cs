using System.Collections.Generic;
using AppDto;
using UniRx;

public sealed partial class SocialityDataMgr
{
    public void LateInit(){
        
        // 四个分页的数据源
        if (ChatDataMgr.Stream.LastValue != null)
        {
            _data._IChatData = ChatDataMgr.Stream.LastValue.ChatInfoViewData;
        }
            

        // 手工merge数据流
        _disposable.Add(ChatDataMgr.Stream.Subscribe(
            chatData =>
            {
                _data._IChatData = chatData != null ? chatData.ChatInfoViewData : null;
                if (_data.CurPageTab == ChatPageTab.Chat)
                    FireData();
            }));

        if (EmailDataMgr.Stream.LastValue != null)
            _data._IEmailData = EmailDataMgr.Stream.LastValue;
        _disposable.Add(EmailDataMgr.Stream.Subscribe(
            emailData =>
            {
                _data._IEmailData = emailData;
                if (_data.CurPageTab == ChatPageTab.Email)
                    FireData();
            }));

        if (PrivateMsgDataMgr.Stream.LastValue != null)
            _data._PrivateMsgData = PrivateMsgDataMgr.Stream.LastValue;
        _disposable.Add(PrivateMsgDataMgr.Stream.Subscribe(
            privateMsgData =>
            {
                _data._PrivateMsgData = privateMsgData;
                if (_data.CurPageTab == ChatPageTab.PrivateMsg)
                    FireData();
            }));

        if (FriendDataMgr.Stream.LastValue != null)
            _data._FriendData = FriendDataMgr.Stream.LastValue;
        _disposable.Add(FriendDataMgr.Stream.Subscribe(
            friendData =>
            {
                _data._FriendData = friendData;
                if (_data.CurPageTab == ChatPageTab.Friend)
                    FireData();
            }));
    }
    private void OnDispose()
    {
        _data.Dispose();
	    _data = null;
	    stream = stream.CloseOnceNull();
        _disposable.Dispose();
        _disposable = null;
    }

 
}
