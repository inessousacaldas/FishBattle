// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 12/14/2017 2:45:59 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class GarandArenaMainViewDataMgr
{
    public static class GarandArenaMainViewNetMsg
    {
        //竞技场信息
        public static void ReqArenaInfo()
        {
            GameUtil.GeneralReq(Services.Arena_Info(), resp =>
            {
                var dto = resp as ArenaInfoDto;
                //RankInfoDto的list按排行顺序
                DataMgr._data.UpdateData(dto);
                FireData();
            });
        }

        //挑战 
        public static void ReqChallenge(long targetPlayerId)
        {
            GameUtil.GeneralReq(Services.Arena_Challenge(targetPlayerId), resp =>
            {
                ProxyGarandArenaMainView.Close();
            });
        }

        //刷新对手
        public static void ReqRefresh()
        {
            GameUtil.GeneralReq(Services.Arena_Refresh(), resp =>
            {
                var list = resp as DataList;
                var dtoList = new List<OpponentInfoDto>();
                list.items.ForEach(d =>
                {
                    dtoList.Add(d as OpponentInfoDto);
                });
                DataMgr._data.RefreshRivalList(dtoList);
                TipManager.AddTip("刷新成功");
                FireData();
            });
        }
    }
}
