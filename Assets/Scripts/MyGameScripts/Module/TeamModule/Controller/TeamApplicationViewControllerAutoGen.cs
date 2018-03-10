// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamApplicationViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface ITeamApplicationViewController:ICloseView
{
     UniRx.IObservable<Unit> OnIgnoreBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnRefreshBtn_UIButtonClick{get;}

}

public partial class TeamApplicationViewController:FRPBaseController<
    TeamApplicationViewController
    , TeamApplicationView
    , ITeamApplicationViewController
    , ITeamData>
    , ITeamApplicationViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
            base.RegistEvent();
    IgnoreBtn_UIButtonEvt = View.IgnoreBtn_UIButton.AsObservable();
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    RefreshBtn_UIButtonEvt = View.RefreshBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        IgnoreBtn_UIButtonEvt = IgnoreBtn_UIButtonEvt.CloseOnceNull();
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        RefreshBtn_UIButtonEvt = RefreshBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> IgnoreBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnIgnoreBtn_UIButtonClick{
        get {return IgnoreBtn_UIButtonEvt;}
    }

    private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> RefreshBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnRefreshBtn_UIButtonClick{
        get {return RefreshBtn_UIButtonEvt;}
    }


    }
