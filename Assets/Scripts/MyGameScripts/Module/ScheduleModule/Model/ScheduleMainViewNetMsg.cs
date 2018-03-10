// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 1/19/2018 5:35:39 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class ScheduleMainViewDataMgr
{
    public static class ScheduleMainViewNetMsg
    {
        //请求当前日程信息
        public static void ReqScheduleInfo(Action completeAction=null)
        {
            GameUtil.GeneralReq(Services.Schedule_Info(), resp =>
            {
                var dto = resp as ScheduleDto;
                DataMgr._data.UpdateActivityData(dto);
                GameUtil.SafeRun(completeAction);

                FireData();
            });
        }

        //领取活跃度奖励
        public static void ReqActivityReward(int activeRewardId)
        {
            GameUtil.GeneralReq(Services.Schedule_ActiveReward(activeRewardId), resp =>
            {
                DataMgr._data.AddActivityReward(activeRewardId);
                FireData();
            });
        }

        //开启/关闭活动通知
        public static void ReqActivityNotify(int scheduleActivityId, bool notify)
        {
            GameUtil.GeneralReq(Services.Schedule_Notify(scheduleActivityId, notify), resp =>
            {
                FireData();
            });
        }

        //一键完美找回
        public static void ReqRewardBack(int regainType, long regainId)
        {
            GameUtil.GeneralReq(Services.Schedule_Regain(regainType, regainId), resp =>
            {
                DataMgr._data.UpdateRewardBack(regainType, regainId);
                FireData();
            });
        }
    }
}
