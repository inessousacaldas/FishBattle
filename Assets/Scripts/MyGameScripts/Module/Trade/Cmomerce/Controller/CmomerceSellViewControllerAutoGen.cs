// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CmomerceSellViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface ICmomerceSellViewController
{
     UniRx.IObservable<Unit> OnReduceBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnAddBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnSellBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}

}

public partial class CmomerceSellViewController:MonoViewController<CmomerceSellView>
    , ICmomerceSellViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    ReduceBtn_UIButtonEvt = View.ReduceBtn_UIButton.AsObservable();
    AddBtn_UIButtonEvt = View.AddBtn_UIButton.AsObservable();
    SellBtn_UIButtonEvt = View.SellBtn_UIButton.AsObservable();
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        ReduceBtn_UIButtonEvt = ReduceBtn_UIButtonEvt.CloseOnceNull();
        AddBtn_UIButtonEvt = AddBtn_UIButtonEvt.CloseOnceNull();
        SellBtn_UIButtonEvt = SellBtn_UIButtonEvt.CloseOnceNull();
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> ReduceBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnReduceBtn_UIButtonClick{
        get {return ReduceBtn_UIButtonEvt;}
    }

    private Subject<Unit> AddBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnAddBtn_UIButtonClick{
        get {return AddBtn_UIButtonEvt;}
    }

    private Subject<Unit> SellBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSellBtn_UIButtonClick{
        get {return SellBtn_UIButtonEvt;}
    }

    private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }


    }
