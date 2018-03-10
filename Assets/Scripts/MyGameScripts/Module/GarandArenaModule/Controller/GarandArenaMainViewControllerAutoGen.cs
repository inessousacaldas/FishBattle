// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ArenaMainViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IGarandArenaMainViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnExchangeBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnReportBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnEmbattleBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnTipsBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnRefreshBtn_UIButtonClick{get;}

}

public partial class GarandArenaMainViewController:FRPBaseController<
    GarandArenaMainViewController
    , GarandArenaMainView
    , IGarandArenaMainViewController
    , IGarandArenaMainViewData>
    , IGarandArenaMainViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    ExchangeBtn_UIButtonEvt = View.ExchangeBtn_UIButton.AsObservable();
    ReportBtn_UIButtonEvt = View.ReportBtn_UIButton.AsObservable();
    EmbattleBtn_UIButtonEvt = View.EmbattleBtn_UIButton.AsObservable();
    TipsBtn_UIButtonEvt = View.TipsBtn_UIButton.AsObservable();
    RefreshBtn_UIButtonEvt = View.RefreshBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        ExchangeBtn_UIButtonEvt = ExchangeBtn_UIButtonEvt.CloseOnceNull();
        ReportBtn_UIButtonEvt = ReportBtn_UIButtonEvt.CloseOnceNull();
        EmbattleBtn_UIButtonEvt = EmbattleBtn_UIButtonEvt.CloseOnceNull();
        TipsBtn_UIButtonEvt = TipsBtn_UIButtonEvt.CloseOnceNull();
        RefreshBtn_UIButtonEvt = RefreshBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> ExchangeBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnExchangeBtn_UIButtonClick{
        get {return ExchangeBtn_UIButtonEvt;}
    }

    private Subject<Unit> ReportBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnReportBtn_UIButtonClick{
        get {return ReportBtn_UIButtonEvt;}
    }

    private Subject<Unit> EmbattleBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnEmbattleBtn_UIButtonClick{
        get {return EmbattleBtn_UIButtonEvt;}
    }

    private Subject<Unit> TipsBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnTipsBtn_UIButtonClick{
        get {return TipsBtn_UIButtonEvt;}
    }

    private Subject<Unit> RefreshBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnRefreshBtn_UIButtonClick{
        get {return RefreshBtn_UIButtonEvt;}
    }


    }
