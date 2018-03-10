// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ShopMainViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IShopMainViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnBuyButton_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnResetButton_UIButtonClick{get;}

}

public partial class ShopMainViewController:FRPBaseController<
    ShopMainViewController
    , ShopMainView
    , IShopMainViewController
    , IShopData>
    , IShopMainViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    BuyButton_UIButtonEvt = View.BuyButton_UIButton.AsObservable();
    ResetButton_UIButtonEvt = View.ResetButton_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        BuyButton_UIButtonEvt = BuyButton_UIButtonEvt.CloseOnceNull();
        ResetButton_UIButtonEvt = ResetButton_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> BuyButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnBuyButton_UIButtonClick{
        get {return BuyButton_UIButtonEvt;}
    }

    private Subject<Unit> ResetButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnResetButton_UIButtonClick{
        get {return ResetButton_UIButtonEvt;}
    }


    }
