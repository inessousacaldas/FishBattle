// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ItemQuickBuyViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IItemQuickBuyViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnReduceBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnAddBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnMaxBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnBuyBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnLastBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnNextBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnSimpleBtn_UIButtonClick{get;}

}

public partial class ItemQuickBuyViewController:FRPBaseController<
    ItemQuickBuyViewController
    , ItemQuickBuyView
    , IItemQuickBuyViewController
    , IItemQuickBuyData>
    , IItemQuickBuyViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    ReduceBtn_UIButtonEvt = View.ReduceBtn_UIButton.AsObservable();
    AddBtn_UIButtonEvt = View.AddBtn_UIButton.AsObservable();
    MaxBtn_UIButtonEvt = View.MaxBtn_UIButton.AsObservable();
    BuyBtn_UIButtonEvt = View.BuyBtn_UIButton.AsObservable();
    LastBtn_UIButtonEvt = View.LastBtn_UIButton.AsObservable();
    NextBtn_UIButtonEvt = View.NextBtn_UIButton.AsObservable();
    SimpleBtn_UIButtonEvt = View.SimpleBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        ReduceBtn_UIButtonEvt = ReduceBtn_UIButtonEvt.CloseOnceNull();
        AddBtn_UIButtonEvt = AddBtn_UIButtonEvt.CloseOnceNull();
        MaxBtn_UIButtonEvt = MaxBtn_UIButtonEvt.CloseOnceNull();
        BuyBtn_UIButtonEvt = BuyBtn_UIButtonEvt.CloseOnceNull();
        LastBtn_UIButtonEvt = LastBtn_UIButtonEvt.CloseOnceNull();
        NextBtn_UIButtonEvt = NextBtn_UIButtonEvt.CloseOnceNull();
        SimpleBtn_UIButtonEvt = SimpleBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> ReduceBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnReduceBtn_UIButtonClick{
        get {return ReduceBtn_UIButtonEvt;}
    }

    private Subject<Unit> AddBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnAddBtn_UIButtonClick{
        get {return AddBtn_UIButtonEvt;}
    }

    private Subject<Unit> MaxBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnMaxBtn_UIButtonClick{
        get {return MaxBtn_UIButtonEvt;}
    }

    private Subject<Unit> BuyBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnBuyBtn_UIButtonClick{
        get {return BuyBtn_UIButtonEvt;}
    }

    private Subject<Unit> LastBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnLastBtn_UIButtonClick{
        get {return LastBtn_UIButtonEvt;}
    }

    private Subject<Unit> NextBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnNextBtn_UIButtonClick{
        get {return NextBtn_UIButtonEvt;}
    }

    private Subject<Unit> SimpleBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSimpleBtn_UIButtonClick{
        get {return SimpleBtn_UIButtonEvt;}
    }


    }
