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
                TipManager.AddTip("领取成功");
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

        //奖励普通找回
        public static void ReqRewardBackNormal(int activeRewardId)
        {
            GameUtil.GeneralReq(Services.Schedule_ActiveReward(activeRewardId), resp =>
            {
                TipManager.AddTip("领取成功");

                FireData();
            });
        }

        //奖励普通找回
        public static void ReqRewardBackPrefect(int activeRewardId)
        {
            GameUtil.GeneralReq(Services.Schedule_ActiveReward(activeRewardId), resp =>
            {
                TipManager.AddTip("领取成功");

                FireData();
            });
        }

        //一键找回
        public static void ReqAllRewardBackNormal(int activeRewardId)
        {
            GameUtil.GeneralReq(Services.Schedule_ActiveReward(activeRewardId), resp =>
            {
                TipManager.AddTip("领取成功");

                FireData();
            });
        }

        //一键完美找回
        public static void ReqAllRewardBackPrefect()
        {
            GameUtil.GeneralReq(Services.Schedule_ActiveReward(21), resp =>
            {
                TipManager.AddTip("领取成功");

                FireData();
            });
        }
    }
}
