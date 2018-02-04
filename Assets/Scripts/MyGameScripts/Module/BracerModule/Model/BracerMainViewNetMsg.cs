// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 11/9/2017 3:18:17 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class BracerMainViewDataMgr
{
    public static class BracerMainViewNetMsg
    {
        //请求任务
        public static void ReqEnterBracer()
        {
            GameUtil.GeneralReq(Services.Bracer_Enter(), resp =>
            {
                var dto = resp as BracerEnterDto;
                DataMgr._data.UpdateData(dto);

                FireData();
            });
        }

        //升级
        public static void ReqRankUp()
        {
            GameUtil.GeneralReq(Services.Bracer_RankUp(), resp =>
            {
                var dto = resp as BracerMissionListDto;
                DataMgr._data.UpdateBracerRankAndMissionList(dto);

                BracerLvUpViewController.Show<BracerLvUpViewController>(BracerLvUpView.NAME, UILayerType.ThreeModule, false, false);

                FireData();
            });
        }
    }
}
