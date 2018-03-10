// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 1/17/2018 5:10:21 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class MartialDataMgr
{
    public static class MartialNetMsg
    {
        //进入场景
        //无返回
        public static void EnterKungfu()
        {
            GameUtil.GeneralReq(Services.Kungfu_Enter());
        }

        public static void OpenKungFu(Action callback)
        {
            GameUtil.GeneralReq(Services.Kungfu_Info(), response =>
            {
                KungfuInfoDto dto = response as KungfuInfoDto;
                if (dto == null)
                {
                    GameDebuger.LogError("Kungfu_Info返回的数据异常,请检查");
                    return;
                }
                DataMgr._data.KungFuInfo = dto;
                DataMgr._data.WinRankInfo = dto.winRankInfoDto;
                DataMgr._data.LoserRankInfo = dto.loseRankInfoDto;
                if (callback != null)
                    callback();
            });
        }

        public static void GetReward(int rewardType)
        {
             GameUtil.GeneralReq(Services.Kungfu_Reward(rewardType), response =>
             {
                 if (rewardType == (int)KungfuReward.KungfuRewardType.FirstWin)
                     DataMgr._data.KungFuInfo.battleWinGiftState = (int) KungfuInfoDto.GiftState.Received;
                 if (rewardType == (int)KungfuReward.KungfuRewardType.FirstBattle)
                     DataMgr._data.KungFuInfo.joinBattleGiftState = (int) KungfuInfoDto.GiftState.Received;

                 FireData();
             });   
        }

        public static void Exit()
        {
            GameUtil.GeneralReq(Services.Kungfu_Exit());
        }
    }
}
