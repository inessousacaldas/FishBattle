using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class ChatDataMgr
{
    public static class ChatNetMsg
    {
//        Chat_Talk 			发送聊天信息
        public static void ReqSendChatMsg(
            ChatChannel.ChatChannelEnum chatChannelId
            , long toPlayerId
            , bool talkLimit
            , string content
            , Action onSuccess = null
            , Action<ErrorResponse> onFail = null)
        {
            Action<GeneralResponse> respHandler = null;

            GameUtil.GeneralReq(
                Services.Chat_Talk((int)chatChannelId, toPlayerId, talkLimit, content)
            , respHandler
            , onSuccess
            , onFail
                );
        }

        
        public static void ReqSendChatMsgWithProps(
            int chatChannelId
            , int itemId
            , string content
            , Action onSuccess = null
            , Action<ErrorResponse> onFail = null)
        {
            Action<GeneralResponse> respHandler = null;

            GameUtil.GeneralReq(
                Services.Chat_TalkUseProps((int)chatChannelId, itemId, content)
                , respHandler
                , onSuccess
                , onFail
            );
        }

        public static void ReqSendVoiceMsg(
            int chatChannelId
            , long toPlayerId
            , string content
            , Action onSuccess = null
            , Action<ErrorResponse> onFail = null)
        {
            Action<GeneralResponse> respHandler = null;
            content = ChatHelper.ReplaceUrlToAddr(content);
            GameUtil.GeneralReq(
                Services.Chat_VoiceTalk((int)chatChannelId, toPlayerId, content)
                , respHandler
                , onSuccess
                , onFail
            );
        }
    }
}
