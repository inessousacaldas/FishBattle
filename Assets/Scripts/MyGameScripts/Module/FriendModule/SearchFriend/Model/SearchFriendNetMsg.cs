// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 9/20/2017 7:57:58 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class SearchFriendDataMgr
{
    public static class SearchFriendNetMsg
    {
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

        //获取推荐好友列表
        public static void ReqRecommendFriendList()
        {
            GameUtil.GeneralReq(Services.Friends_Recommend(), resp =>
            {
                var dto = resp as FriendListDto;
                DataMgr._data.UpdateRecommend(dto);
                FireData();
            });
        }

        //搜索玩家
        public static void ReqSearchFriend(string str)
        {
            GameUtil.GeneralReq(Services.Friends_Search(str), resp =>
            {
                var dto = resp as FriendListDto;
                DataMgr._data.UpdateSearch(dto);
                FireData();
            });
        }

        //换一批
        public static void ReqChange()
        {
            GameUtil.GeneralReq(Services.Friends_Recommend(), resp =>
            {
                var dto = resp as FriendListDto;
                DataMgr._data.UpdateChange(dto);
                FireData();
            });
        }
    }
}
