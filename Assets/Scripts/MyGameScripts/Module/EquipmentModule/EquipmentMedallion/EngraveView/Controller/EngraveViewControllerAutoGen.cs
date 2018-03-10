// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EngraveViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IEngraveViewController : ICloseView
{
     UniRx.IObservable<Unit> OnSaleBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnEngraveBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnTipBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}

}

public partial class EngraveViewController:FRPBaseController<
    EngraveViewController
    , EngraveView
    , IEngraveViewController
    , IEngraveData>
    , IEngraveViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    SaleBtn_UIButtonEvt = View.SaleBtn_UIButton.AsObservable();
    EngraveBtn_UIButtonEvt = View.EngraveBtn_UIButton.AsObservable();
    TipBtn_UIButtonEvt = View.TipBtn_UIButton.AsObservable();
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        SaleBtn_UIButtonEvt = SaleBtn_UIButtonEvt.CloseOnceNull();
        EngraveBtn_UIButtonEvt = EngraveBtn_UIButtonEvt.CloseOnceNull();
        TipBtn_UIButtonEvt = TipBtn_UIButtonEvt.CloseOnceNull();
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> SaleBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSaleBtn_UIButtonClick{
        get {return SaleBtn_UIButtonEvt;}
    }

    private Subject<Unit> EngraveBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnEngraveBtn_UIButtonClick{
        get {return EngraveBtn_UIButtonEvt;}
    }

    private Subject<Unit> TipBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnTipBtn_UIButtonClick{
        get {return TipBtn_UIButtonEvt;}
    }

    private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }


    }
