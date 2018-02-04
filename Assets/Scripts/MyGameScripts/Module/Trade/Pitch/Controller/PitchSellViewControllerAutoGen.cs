// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PitchSellViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IPitchSellViewController : ICloseView
{
     UniRx.IObservable<Unit> OnAddBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnAddNumBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnReduceNumBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnPriceReduceBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnPriceAddBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnSellBtn_UIButtonClick{get;}

}

public partial class PitchSellViewController:FRPBaseController<
    PitchSellViewController
    , PitchSellView
    , IPitchSellViewController
    , ITradeData>
    , IPitchSellViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    AddBtn_UIButtonEvt = View.AddBtn_UIButton.AsObservable();
    AddNumBtn_UIButtonEvt = View.AddNumBtn_UIButton.AsObservable();
    ReduceNumBtn_UIButtonEvt = View.ReduceNumBtn_UIButton.AsObservable();
    PriceReduceBtn_UIButtonEvt = View.PriceReduceBtn_UIButton.AsObservable();
    PriceAddBtn_UIButtonEvt = View.PriceAddBtn_UIButton.AsObservable();
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    SellBtn_UIButtonEvt = View.SellBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        AddBtn_UIButtonEvt = AddBtn_UIButtonEvt.CloseOnceNull();
        AddNumBtn_UIButtonEvt = AddNumBtn_UIButtonEvt.CloseOnceNull();
        ReduceNumBtn_UIButtonEvt = ReduceNumBtn_UIButtonEvt.CloseOnceNull();
        PriceReduceBtn_UIButtonEvt = PriceReduceBtn_UIButtonEvt.CloseOnceNull();
        PriceAddBtn_UIButtonEvt = PriceAddBtn_UIButtonEvt.CloseOnceNull();
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        SellBtn_UIButtonEvt = SellBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> AddBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnAddBtn_UIButtonClick{
        get {return AddBtn_UIButtonEvt;}
    }

    private Subject<Unit> AddNumBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnAddNumBtn_UIButtonClick{
        get {return AddNumBtn_UIButtonEvt;}
    }

    private Subject<Unit> ReduceNumBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnReduceNumBtn_UIButtonClick{
        get {return ReduceNumBtn_UIButtonEvt;}
    }

    private Subject<Unit> PriceReduceBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPriceReduceBtn_UIButtonClick{
        get {return PriceReduceBtn_UIButtonEvt;}
    }

    private Subject<Unit> PriceAddBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPriceAddBtn_UIButtonClick{
        get {return PriceAddBtn_UIButtonEvt;}
    }

    private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> SellBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSellBtn_UIButtonClick{
        get {return SellBtn_UIButtonEvt;}
    }


    }
