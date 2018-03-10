// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 1/16/2018 12:11:22 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class RankDataMgr
{
    public static class RankNetMsg
    {
        public static void RankingsInfo(int Menuid)
        {
            GameUtil.GeneralReq(Services.Rankings_Info(Menuid), res =>
            {
                RankInfoDto dto = res as RankInfoDto;
                if (dto == null)
                {
                    GameDebuger.LogError("服务器返回的数据有问题");
                    return;
                }
                DataMgr._data.CurRankId = Menuid;
                DataMgr._data.UpdateRankData(dto, Menuid);
                FireData();
            });
        }
    }
}
