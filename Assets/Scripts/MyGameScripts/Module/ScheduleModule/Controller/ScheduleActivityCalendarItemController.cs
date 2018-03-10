// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ScheduleActivityCalendarItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;

public partial class ScheduleActivityCalendarItemController
{
    private List<UIButton> _buttonList = new List<UIButton>();
    private List<UILabel> _labelList = new List<UILabel>();
    private List<ScheduleActivity> _activityLlist = new List<ScheduleActivity>(7);

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _buttonList.Add(View.Btn_1_UIButton);
        _buttonList.Add(View.Btn_2_UIButton);
        _buttonList.Add(View.Btn_3_UIButton);
        _buttonList.Add(View.Btn_4_UIButton);
        _buttonList.Add(View.Btn_5_UIButton);
        _buttonList.Add(View.Btn_6_UIButton);
        _buttonList.Add(View.Btn_7_UIButton);

        _labelList.Add(View.Label_1_UILabel);
        _labelList.Add(View.Label_2_UILabel);
        _labelList.Add(View.Label_3_UILabel);
        _labelList.Add(View.Label_4_UILabel);
        _labelList.Add(View.Label_5_UILabel);
        _labelList.Add(View.Label_6_UILabel);
        _labelList.Add(View.Label_7_UILabel);
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        Btn_1_UIButtonEvt.Subscribe(_ =>
        {
            if (_activityLlist[0] == null) return;
            OpenActivityTipsView(_activityLlist[0]);
        });

        Btn_2_UIButtonEvt.Subscribe(_ =>
        {
            if (_activityLlist[1] == null) return;
            OpenActivityTipsView(_activityLlist[1]);
        });

        Btn_3_UIButtonEvt.Subscribe(_ =>
        {
            if (_activityLlist[2] == null) return;
            OpenActivityTipsView(_activityLlist[2]);
        });

        Btn_4_UIButtonEvt.Subscribe(_ =>
        {
            if (_activityLlist[3] == null) return;
            OpenActivityTipsView(_activityLlist[3]);
        });

        Btn_5_UIButtonEvt.Subscribe(_ =>
        {
            if (_activityLlist[4] == null) return;
            OpenActivityTipsView(_activityLlist[4]);
        });

        Btn_6_UIButtonEvt.Subscribe(_ =>
        {
            if (_activityLlist[5] == null) return;
            OpenActivityTipsView(_activityLlist[5]);
        });

        Btn_7_UIButtonEvt.Subscribe(_ =>
        {
            if (_activityLlist[6] == null) return;
            OpenActivityTipsView(_activityLlist[6]);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
    }

    private void OpenActivityTipsView(ScheduleActivity data)
    {
        var ctrl = ScheduleActivityTipsController.Show<ScheduleActivityTipsController>(ScheduleActivityTips.NAME, UILayerType.ThreeModule, true);
        ctrl.UpdateView(ScheduleMainViewDataMgr.DataMgr.GetActivityDtoById(data.id), data);
    }
    public void UpdateView(string timeStr, List<ScheduleActivity> list)
    {
        if (_buttonList.Count != 7 || _labelList.Count != 7 || list.Count != 7) return;

        View.Time_UILabel.text = timeStr;
        _activityLlist.Clear();
        list.ForEachI((itemData,index) =>
        {
            _activityLlist.Add(itemData);
            _labelList[index].text = itemData != null ? itemData.name : "";
        });
    }
}
