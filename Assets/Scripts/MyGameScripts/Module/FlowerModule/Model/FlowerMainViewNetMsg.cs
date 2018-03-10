// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 1/13/2018 10:13:30 AM
// **********************************************************************

using System;
using AppDto;
using AppServices;
using ChatChannelEnum = AppDto.ChatChannel.ChatChannelEnum;

public sealed partial class FlowerMainViewDataMgr
{
    public static class FlowerMainViewNetMsg
    {
        public static void ReqGiveFlower(long friendId, string friendName, int itemId, int itemCount, string content)
        {
            GameUtil.GeneralReq(Services.Friends_GiftFlowers(friendId, itemId, itemCount, content), resp =>
            {
                TipManager.AddTip("送花成功");
                FireData();
                var itemData = ItemHelper.GetGeneralItemByItemId(itemId);
                if (itemData == null) return;
                PrivateMsgDataMgr.DataMgr.SendMessageWithoutView(friendId, friendName, string.Format("赠你{0}朵{1}，{2}", itemCount, itemData.name, content));
                ChatNotify chatNotify = new ChatNotify() { channelId = (int)ChatChannelEnum.System, content = 
                    string.Format("{0}向{1}赠送{2}朵{3}，并对TA说：{4}", ModelManager.Player.GetPlayerName(), friendName, itemCount, itemData.name, content), lableType = 1 };
                ChatDataMgr.DataMgr.SendSystemMessage(chatNotify);
            });
        }

        public static void ReqSearchFriend(string str)
        {
            GameUtil.GeneralReq(Services.Friends_Search(str), resp =>
            {
                var dto = resp as FriendListDto;
                DataMgr._data.UpdateSearchList(dto);
                FireData();
            });
        }
    }
}
