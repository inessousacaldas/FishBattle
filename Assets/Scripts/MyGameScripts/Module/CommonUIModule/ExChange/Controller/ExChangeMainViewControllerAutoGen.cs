// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ExChangeMainViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IExChangeMainViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnExchangeButton_UIButtonClick { get; }
     UniRx.IObservable<Unit> OnTipsButton_UIButtonClick { get; }

}

public partial class ExChangeMainViewController:FRPBaseController<
    ExChangeMainViewController
    , ExChangeMainView
    , IExChangeMainViewController
    , IExChangeMainData>
    , IExChangeMainViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    ExchangeButton_UIButtonEvt = View.ExchangeButton_UIButton.AsObservable();
        TipsButton_UIButtonEvt = View.TipsButton_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        ExchangeButton_UIButtonEvt = ExchangeButton_UIButtonEvt.CloseOnceNull();
        TipsButton_UIButtonEvt = TipsButton_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> ExchangeButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnExchangeButton_UIButtonClick{
        get {return ExchangeButton_UIButtonEvt;}
    }
    
    private Subject<Unit> TipsButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnTipsButton_UIButtonClick
    {
        get { return TipsButton_UIButtonEvt; }
    }

}
