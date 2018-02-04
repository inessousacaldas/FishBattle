// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 12/19/2017 11:25:11 AM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class GuideMainViewDataMgr
{
    public static class GuideMainViewNetMsg
    {
        //引导信息
        public static void ReqGuideInfo()
        {
            GameUtil.GeneralReq(Services.Guide_Info(), resp =>
            {
                var dto = resp as GuideListDto;
                DataMgr._data.UpdateData(dto);
                FireData();
            });
        }

        //引导领取奖励
        public static void ReqGetReward(int guideId)
        {
            GameUtil.GeneralReq(Services.Guide_Reward(guideId), resp =>
            {
                DataMgr._data.SetGetId(guideId);
                FireData();
            });
        }
    }
}
