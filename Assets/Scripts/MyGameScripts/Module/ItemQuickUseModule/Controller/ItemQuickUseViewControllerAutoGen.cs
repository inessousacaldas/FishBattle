// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ItemQuickUseViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IItemQuickUseViewController : ICloseView
{
     UniRx.IObservable<Unit> OnUseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}

}

public partial class ItemQuickUseViewController:FRPBaseController<
    ItemQuickUseViewController
    , ItemQuickUseView
    , IItemQuickUseViewController
    , IItemQuickUseData>
    , IItemQuickUseViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    UseBtn_UIButtonEvt = View.UseBtn_UIButton.AsObservable();
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        UseBtn_UIButtonEvt = UseBtn_UIButtonEvt.CloseOnceNull();
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> UseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnUseBtn_UIButtonClick{
        get {return UseBtn_UIButtonEvt;}
    }

    private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }


    }
