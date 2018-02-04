// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ScheduleRewardBackViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;

public partial class ScheduleRewardBackViewController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        WeekBtn_UIButtonEvt.Subscribe(_ =>
        {
            ScheduleActivityCalendarViewController.Show<ScheduleActivityCalendarViewController>(ScheduleActivityCalendarView.NAME, UILayerType.ThreeModule, true, false);
        });

        PushBtn_UIButtonEvt.Subscribe(_ =>
        {
            ScheduleActivityRemindViewController.Show<ScheduleActivityRemindViewController>(ScheduleActivityRemindView.NAME, UILayerType.ThreeModule, true, false);
        });

        TipsBtn_UIButtonEvt.Subscribe(_ =>
        {
            ProxyTips.OpenTextTips(26, new Vector3(171, 163));
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private List<ScheduleRewardBackItemController> _itemCtrlList = new List<ScheduleRewardBackItemController>();
    public void UpdateView(IScheduleMainViewData data)
    {
        _itemCtrlList.ForEach(item => { item.Hide(); });
        data.RewardBackList.ForEachI((btnItem, index) =>
        {
            var ctrl = AddActivityItemIfNotExist(index);
            ctrl.UpdateView();
            ctrl.Show();
        });

        View.Grid_UIGrid.Reposition();
    }

    private ScheduleRewardBackItemController AddActivityItemIfNotExist(int idx)
    {
        ScheduleRewardBackItemController ctrl = null;
        _itemCtrlList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<ScheduleRewardBackItemController, ScheduleRewardBackItem>(View.Grid_UIGrid.gameObject, ScheduleRewardBackItem.NAME);
            _itemCtrlList.Add(ctrl);
        }

        return ctrl;
    }
}
