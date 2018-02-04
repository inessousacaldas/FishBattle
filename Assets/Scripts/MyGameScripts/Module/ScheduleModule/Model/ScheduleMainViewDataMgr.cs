// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 1/19/2018 5:35:39 PM
// **********************************************************************

using UniRx;
using AppDto;
using System.Collections.Generic;
using Asyn;
using System;

namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner scheduleDataMgr = new StaticDispose.StaticDelegateRunner(
            () => { var mgr = ScheduleMainViewDataMgr.DataMgr; });
    }
}

public sealed partial class ScheduleMainViewDataMgr : AbstractAsynInit
{
    public override void StartAsynInit(Action<IAsynInit> onComplete)
    {
        Action act = delegate ()
        {
            onComplete(this);
        };

        if (FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_38))
            ScheduleMainViewNetMsg.ReqScheduleInfo(act);
        else
            GameUtil.SafeRun(act);
    }

    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<ActiveNotify>(HandleActiveNotify));
    }
    
    private void OnDispose(){
            
    }

    //完成某活动后活跃度变化
    private void HandleActiveNotify(ActiveNotify notify)
    {
        DataMgr._data.AddActivityValue(notify.addActive);
        FireData();
    }

    public IEnumerable<int> GetCancleList()
    {
        return DataMgr._data.CancelNotifyIdList;
    }

    public void UpdateCancelList()
    {
        DataMgr._data.UpdateCancelList();
    }

    public ActiveDto GetActivityDtoById(int id)
    {
        return DataMgr._data.GetActivityDtoById(id);
    }

    public int GetActivityState(ActiveDto dto, ScheduleActivity itemData)
    {
        return DataMgr._data.GetActivityState(dto, itemData);
    }

    public string GetActivityStartTime(ActiveDto dto, ScheduleActivity itemData)
    {
        return DataMgr._data.GetActivityStartTime(dto, itemData);
    }
}
