// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ShopItemTipsViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface IShopItemTipsViewController
{
     UniRx.IObservable<Unit> OnCloseMask_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnLeftBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnRightBtn_UIButtonClick{get;}

}

public partial class ShopItemTipsViewController:MonoViewController<ShopItemTipsView>
    , IShopItemTipsViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseMask_UIButtonEvt = View.CloseMask_UIButton.AsObservable();
    LeftBtn_UIButtonEvt = View.LeftBtn_UIButton.AsObservable();
    RightBtn_UIButtonEvt = View.RightBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseMask_UIButtonEvt = CloseMask_UIButtonEvt.CloseOnceNull();
        LeftBtn_UIButtonEvt = LeftBtn_UIButtonEvt.CloseOnceNull();
        RightBtn_UIButtonEvt = RightBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseMask_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseMask_UIButtonClick{
        get {return CloseMask_UIButtonEvt;}
    }

    private Subject<Unit> LeftBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnLeftBtn_UIButtonClick{
        get {return LeftBtn_UIButtonEvt;}
    }

    private Subject<Unit> RightBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnRightBtn_UIButtonClick{
        get {return RightBtn_UIButtonEvt;}
    }


    }
