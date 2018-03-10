// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ScheduleMainViewController.cs
// Author   : xjd
// Created  : 1/19/2018 5:35:39 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;

public partial interface IScheduleMainViewController
{
    event Action<ScheduleRightViewTab, IMonolessViewController> OnChildCtrlAdd;
    TabbtnManager TabbtnMgr { get; }
    UniRx.IObservable<ScheduleRewardBackStruct> ScheduleRewardBackItemDisapear { get; }
}
public partial class ScheduleMainViewController
{
    private ScheduleDailyViewController _dailyViewCtrl = null;
    private ScheduleDailyViewController _limitViewCtrl = null;
    private ScheduleRewardBackViewController _rewardBackViewCtrl = null;
    IMonolessViewController curCtrl;
    public event Action<ScheduleRightViewTab, IMonolessViewController> OnChildCtrlAdd;

    TabbtnManager tabbtnMgr;
    public TabbtnManager TabbtnMgr { get { return tabbtnMgr; } }
    private Func<int, ITabBtnController> func;

    private Subject<ScheduleRewardBackStruct> scheduleRewardBackItemEvt = new Subject<ScheduleRewardBackStruct>();
    public UniRx.IObservable<ScheduleRewardBackStruct> ScheduleRewardBackItemDisapear { get { return scheduleRewardBackItemEvt; } }
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
        View.TabBtnTable_UIGrid.gameObject
        , TabbtnPrefabPath.TabBtnWidget.ToString()
        , "Tabbtn_" + i);
        tabbtnMgr = TabbtnManager.Create(ScheduleMainViewDataMgr.DataMgr.RightViewTabInfos, func);
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
        OnChildCtrlAdd = null;
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IScheduleMainViewData data)
    {
        var lastCtrl = curCtrl;
        switch (data.CurRightTab)
        {
            case ScheduleRightViewTab.DaliyActView:
                if (_dailyViewCtrl == null)
                {
                    _dailyViewCtrl = AddChild<ScheduleDailyViewController, ScheduleDailyView>(View.MainContent_Transform.gameObject, ScheduleDailyView.NAME);
                    OnChildCtrlAdd(ScheduleRightViewTab.DaliyActView, _dailyViewCtrl);
                }
                _dailyViewCtrl.UpdateView(data, ScheduleRightViewTab.DaliyActView);
                curCtrl = _dailyViewCtrl;
                break;
            case ScheduleRightViewTab.LimitActView:
                if (_limitViewCtrl == null)
                {
                    _limitViewCtrl = AddChild<ScheduleDailyViewController, ScheduleDailyView>(View.MainContent_Transform.gameObject, ScheduleDailyView.NAME);
                    OnChildCtrlAdd(ScheduleRightViewTab.LimitActView, _limitViewCtrl);
                }
                _limitViewCtrl.UpdateView(data, ScheduleRightViewTab.LimitActView);
                curCtrl = _limitViewCtrl;
                break;
            case ScheduleRightViewTab.RewardBackView:
                if (_rewardBackViewCtrl == null)
                {
                    _rewardBackViewCtrl = AddChild<ScheduleRewardBackViewController, ScheduleRewardBackView>(View.MainContent_Transform.gameObject, ScheduleRewardBackView.NAME);
                    //OnChildCtrlAdd(ScheduleRightViewTab.DaliyActView, _rewardBackViewCtrl);
                    _disposable.Add(_rewardBackViewCtrl.ScheduleRewardBackItemDisapear.Subscribe(e => scheduleRewardBackItemEvt.OnNext(e)));
                }
                _rewardBackViewCtrl.UpdateView(data);
                curCtrl = _rewardBackViewCtrl;
                break;
        }

        if (lastCtrl == curCtrl)
            return;
        else
        {
            if (lastCtrl != null)
                lastCtrl.Hide();
            curCtrl.Show();
        }
    }
}
