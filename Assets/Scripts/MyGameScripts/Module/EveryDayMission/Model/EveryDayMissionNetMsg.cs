// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 11/10/2017 4:36:56 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class EveryDayMissionDataMgr
{
    public static class EveryDayMissionNetMsg
    {
        /// <summary>
        /// 获取当前所有日常委托任务信息
        /// </summary>
        public static void GetAllEveryDayMission()
        {
            GameUtil.GeneralReq(Services.Mission_RefreshFaction(),response =>
            {
                FactionMissionIdDto factionMissionIdDto = response as FactionMissionIdDto;
                //if(factionMissionIdDto.dailyFinishCount > AppStaticConfigs.FACTION_MISSION_DAILY_FINISH_REWARD_PARAM)
                //{
                //    ProxyWindowModule.OpenConfirmWindow("今日已经做满4次游击士任务，继续完成只能获得少量奖励，是否继续？","每日委托",SureOpenPanel,ClickDialogueBtn,UIWidget.Pivot.Left,null,null,30);
                //    return;
                //}
                if(factionMissionIdDto.factionMissionIds.Count > 0)
                {
                    DataMgr._data.InitMissionList(factionMissionIdDto);
                }
            });
        }

        /// <summary>
        /// 接受委托
        /// </summary>
        /// <param name="missionID">委托的任务ID</param>
        public static void AcceptFaction(int factionID)
        {
            GameUtil.GeneralReq(Services.Mission_AcceptFaction(factionID),response => {
                EveryDayMissionPanelLogic.Close();
            });
        }
    }
}
