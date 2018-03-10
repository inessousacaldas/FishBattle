// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 9/20/2017 7:57:58 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class FriendDataMgr
{
    public static class FriendNetMsg
    {
        //请求好友系统数据
        public static void ReqFriendInfo(Action completeAction)
        {
            GameUtil.GeneralReq(Services.Friends_Check(), resp =>
            {
                var friendsDtoTmp = resp as FriendsDto;
                DataMgr._data.UpdateData(friendsDtoTmp);
                GameUtil.SafeRun(completeAction);

                FireData();
            });
        }

        //添加好友(多处用到 这里统一判断是否已拉黑)
        public static void ReqAddFriend(long id)
        {
            if (FriendDataMgr.DataMgr.isMyBlack(id))
                BuiltInDialogueViewController.OpenView("你已将该玩家拉进黑名单，是否将该玩家从黑名单中删除并加为好友？",
                    null, (() =>
                    {
                        GameUtil.GeneralReq(Services.Friends_Add(id), resp =>
                        {
                            var dto = resp as FriendInfoDto;
                            FriendDataMgr.DataMgr.AddFriend(dto);

                            TipManager.AddTip("添加好友成功");
                            FireData();
                        });
                    }),
                    UIWidget.Pivot.Left, "取消", "确定");
            else
                GameUtil.GeneralReq(Services.Friends_Add(id), resp =>
                {
                    var dto = resp as FriendInfoDto;
                    FriendDataMgr.DataMgr.AddFriend(dto);

                    TipManager.AddTip("添加好友成功");
                    FireData();
                });
        }

        //添加黑名单
        public static void ReqAddBlack(long id)
        {
            GameUtil.GeneralReq(Services.Friends_BlackList(id), resp =>
            {
                var dto = resp as FriendInfoDto;
                DataMgr.AddBlack(dto);

                TipManager.AddTip("添加黑名单成功");
                FireData();
            });
        }

        //删除好友
        public static void ReqDeleteFriend(long id)
        {
            GameUtil.GeneralReq(Services.Friends_Delete(id), resp =>
            {
                DataMgr.DeleteFriend(id);

                TipManager.AddTip("删除好友成功");
                FireData();
            });
        }

        //删除黑名单
        public static void ReqDeleteBlack(long id)
        {
            GameUtil.GeneralReq(Services.Friends_DeleteBlackList(id), resp =>
            {
                DataMgr.DeleteBlack(id);

                TipManager.AddTip("删除黑名单成功");
                FireData();
            });
        }

        //好友互动
        public static void ReqActionFriend(int actionId, long playerId)
        {
            GameUtil.GeneralReq(Services.Friends_Action(actionId, playerId), resp =>
            {
                TipManager.AddTip("发送好友互动");
            });
        }
    }
}
