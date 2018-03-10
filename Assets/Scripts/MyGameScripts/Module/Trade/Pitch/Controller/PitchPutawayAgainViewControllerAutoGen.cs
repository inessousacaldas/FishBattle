// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PitchPutawayAgainViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface IPitchPutawayAgainViewController
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnPriceAddBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnPriceReduceBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnSoldOutBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnPutawayBtn_UIButtonClick{get;}

}

public partial class PitchPutawayAgainViewController:MonoViewController<PitchPutawayAgainView>
    , IPitchPutawayAgainViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    PriceAddBtn_UIButtonEvt = View.PriceAddBtn_UIButton.AsObservable();
    PriceReduceBtn_UIButtonEvt = View.PriceReduceBtn_UIButton.AsObservable();
    SoldOutBtn_UIButtonEvt = View.SoldOutBtn_UIButton.AsObservable();
    PutawayBtn_UIButtonEvt = View.PutawayBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        PriceAddBtn_UIButtonEvt = PriceAddBtn_UIButtonEvt.CloseOnceNull();
        PriceReduceBtn_UIButtonEvt = PriceReduceBtn_UIButtonEvt.CloseOnceNull();
        SoldOutBtn_UIButtonEvt = SoldOutBtn_UIButtonEvt.CloseOnceNull();
        PutawayBtn_UIButtonEvt = PutawayBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> PriceAddBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPriceAddBtn_UIButtonClick{
        get {return PriceAddBtn_UIButtonEvt;}
    }

    private Subject<Unit> PriceReduceBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPriceReduceBtn_UIButtonClick{
        get {return PriceReduceBtn_UIButtonEvt;}
    }

    private Subject<Unit> SoldOutBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSoldOutBtn_UIButtonClick{
        get {return SoldOutBtn_UIButtonEvt;}
    }

    private Subject<Unit> PutawayBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPutawayBtn_UIButtonClick{
        get {return PutawayBtn_UIButtonEvt;}
    }


    }
