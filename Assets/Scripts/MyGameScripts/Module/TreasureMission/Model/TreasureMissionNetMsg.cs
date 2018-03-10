// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 12/13/2017 4:15:20 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class TreasureMissionDataMgr
{
    public static class TreasureMissionNetMsg
    {
        public static void InitTreasureMission()
        {
            GameUtil.GeneralReq<HighTreasuryInfoDto>(Services.Items_HighTreasuryInfo(),delegate (HighTreasuryInfoDto e)
            {
                DataMgr._data.SetInitData(e);
            },
            (e) =>
            {
                TipManager.AddTip(e.message);
            });
        }


        /// <summary>
        /// 关闭宝图的时候需要发协议给服务器
        /// </summary>
        public static void CloseTreasury() {
            GameUtil.GeneralReq(Services.Items_CloseHighTreasury());
        }

    }
}
