// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ScheduleActivityCalendarViewController.cs
// Author   : xjd
// Created  : 1/19/2018 5:47:00 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;
using AppDto;

public partial interface IScheduleActivityCalendarViewController
{

}
public partial class ScheduleActivityCalendarViewController    {

    public static IScheduleActivityCalendarViewController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IScheduleActivityCalendarViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IScheduleActivityCalendarViewController;
            
        return controller;        
    }         
        
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        UpdateView();
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        CloseBtn_UIButtonEvt.Subscribe(_ =>
        {
            UIModuleManager.Instance.CloseModule(ScheduleActivityCalendarView.NAME);
        });
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    private List<ScheduleActivityCalendarItemController> _calendarItemCtrlList = new List<ScheduleActivityCalendarItemController>();
    private Dictionary<int, WeekCalendar> _weekCalenderDic = DataCache.getDicByCls<WeekCalendar>();
    private void UpdateView()
    {
        _calendarItemCtrlList.ForEach(item => { item.Hide(); });
        _weekCalenderDic.ForEachI((itemData, index) =>
        {
            var ctrl = AddCalendarItemIfNotExist(index);
            var list = new List<ScheduleActivity>();
            list.Add(itemData.Value.monday);
            list.Add(itemData.Value.tuesday);
            list.Add(itemData.Value.wednesday);
            list.Add(itemData.Value.thursday);
            list.Add(itemData.Value.friday);
            list.Add(itemData.Value.saturday);
            list.Add(itemData.Value.sunday);
            ctrl.UpdateView(itemData.Value.startTime, list);
            ctrl.Show();
        });

        View.Grid_UIGrid.Reposition();
    }

    private ScheduleActivityCalendarItemController AddCalendarItemIfNotExist(int idx)
    {
        ScheduleActivityCalendarItemController ctrl = null;
        _calendarItemCtrlList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<ScheduleActivityCalendarItemController, ScheduleActivityCalendarItem>(View.Grid_UIGrid.gameObject, ScheduleActivityCalendarItem.NAME);
            _calendarItemCtrlList.Add(ctrl);
        }

        return ctrl;
    }
}
