﻿// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ScheduleRewardBackItemController.cs
// the file is generated by tools
// **********************************************************************


using UniRx;

public partial interface IScheduleRewardBackItemController
{
     UniRx.IObservable<Unit> OnPrefectBackBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnNormalBackBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnFinishedBtn_UIButtonClick { get; }

}

public partial class ScheduleRewardBackItemController:MonolessViewController<ScheduleRewardBackItem>, IScheduleRewardBackItemController
{
    //机器自动生成的事件绑定
    protected override void InitReactiveEvents(){
        PrefectBackBtn_UIButtonEvt = View.PrefectBackBtn_UIButton.AsObservable();
        NormalBackBtn_UIButtonEvt = View.NormalBackBtn_UIButton.AsObservable();
        FinishedBtn_UIButtonEvt = View.finishedBtn_UIButton.AsObservable();

    }

    protected override void ClearReactiveEvents(){
        PrefectBackBtn_UIButtonEvt = PrefectBackBtn_UIButtonEvt.CloseOnceNull();
        NormalBackBtn_UIButtonEvt = NormalBackBtn_UIButtonEvt.CloseOnceNull();
        FinishedBtn_UIButtonEvt = FinishedBtn_UIButtonEvt.CloseOnceNull();
    }

    private Subject<Unit> PrefectBackBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPrefectBackBtn_UIButtonClick{
        get {return PrefectBackBtn_UIButtonEvt;}
    }

    private Subject<Unit> NormalBackBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnNormalBackBtn_UIButtonClick{
        get {return NormalBackBtn_UIButtonEvt;}
    }

    private Subject<Unit> FinishedBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnFinishedBtn_UIButtonClick
    {
        get { return FinishedBtn_UIButtonEvt; }
    }

}
