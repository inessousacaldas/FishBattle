// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 10/18/2017 3:02:07 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class PrivateMsgDataMgr
{
    public static class PrivateMsgNetMsg
    {
        //向好友发私信
        public static void ReqTalkFriend(long toPlayerId, string content, Action onSuccess = null, Action<ErrorResponse> onFail = null)
        {
            Action<GeneralResponse> respHandler = null;
            GameUtil.GeneralReq(Services.Friends_Talk(toPlayerId, content), respHandler, onSuccess, onFail);
        }

        //读取好友离线私信
        public static void ReqLoadMessage(Action completeAction)
        {
            GameUtil.GeneralReq(Services.Friends_LoadMessage(), resp =>
            {
                var dto = resp as FriendLoadMsgDto;
                dto.chatNotifies.ForEach(x =>
                {
                    DataMgr._data.AddChatNotify(x);
                });

                GameUtil.SafeRun(completeAction);

                FireData();
            });
        }
    }
}
