// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 12/13/2017 11:28:04 AM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class TowerDataMgr
{
    public static class TowerNetMsg
    {
        public static void EnterTower()
        {
            GameUtil.GeneralReq(Services.Tower_Enter(DataMgr.CurTowerId),e=> { UpdateData(); } );
        }
        private static void UpdateData()
        {
            DataMgr.UpdateData();
        }
        public static void ResetTower()
        {
            GameUtil.GeneralReq(Services.Tower_Reset(),e=> { DataMgr.UpdateResetCount(); });
        }

        public static void TowerRank()
        {
        }

        public static void TowerChallenge(int towerId,int monsterId)
        {
            GameUtil.GeneralReq(Services.Tower_Challenge(towerId, monsterId));
        }
    }
}
