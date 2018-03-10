// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Wangsimin
// Created  : 2/27/2018 4:22:54 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AppDto;
using AppServices;

public sealed partial class RedPackDataMgr
{
    public static class RedPackNetMsg
    {
        //红包列表
        public static void ReqRedPackInfo(long guildID, Action callback)
        {
            GameUtil.GeneralReq(Services.RedPack_Infos(DataMgr._data.index, guildID), resp =>
            {
                var res = resp as RedPackInfoDto;
                if (res == null) return;
                var channel = guildID > 0 ? RedPackChannelType.Guild : RedPackChannelType.World;
                DataMgr._data.SetRedPackList(DataMgr._data.index, channel, res);
                FireData();
            }
                , onSuccess:callback);
        }
        //发送红包
        public static void SendRedPack(int total,int count,string word, int type, long guildId)
        {
            GameUtil.GeneralReq(Services.RedPack_Send(total, count, word, type, guildId), resp =>
            {
                TipManager.AddTip("发送成功");
                FireData();
            });
        }
        //领红包
        public static void ReceiveRedPack(long Id)
        {
            GameUtil.GeneralReq(Services.RedPack_Receive(Id), resp =>
            {
                TipManager.AddTip("领取成功");
                FireData();
            });
        }
        //红包详情
        public static void RedPackDetail(long Id)
        {
            GameUtil.GeneralReq(Services.RedPack_Detail(Id), resp =>
            {
                var res = resp as RedPackDetailDto;
                if (res == null)
                {
                    GameDebuger.LogError("=====RedPack_Detail数据返回不对=======");
                    return;
                }

                DataMgr._data.RedPackDetailFunc(res.detail);
                FireData();
            });
        }
    }
}
