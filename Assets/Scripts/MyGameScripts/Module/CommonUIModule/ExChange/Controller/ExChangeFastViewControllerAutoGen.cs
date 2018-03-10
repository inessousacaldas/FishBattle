// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ExChangeFastViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface IExChangeFastViewController
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnMilaButton_UIButtonClick{get;}
     UniRx.IObservable<Unit> OndiamondButton_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnTipsBtn_UIButtonClick {get;}

}

public partial class ExChangeFastViewController:MonoViewController<ExChangeFastView>
    , IExChangeFastViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    MilaButton_UIButtonEvt = View.MiraButton_UIButton.AsObservable();
    diamondButton_UIButtonEvt = View.DiamondButton_UIButton.AsObservable();
    TipsBtn_UIButtonEvt = View.TipsBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        MilaButton_UIButtonEvt = MilaButton_UIButtonEvt.CloseOnceNull();
        diamondButton_UIButtonEvt = diamondButton_UIButtonEvt.CloseOnceNull();
        TipsBtn_UIButtonEvt = TipsBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> MilaButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnMilaButton_UIButtonClick{
        get {return MilaButton_UIButtonEvt;}
    }

    private Subject<Unit> diamondButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OndiamondButton_UIButtonClick{
        get {return diamondButton_UIButtonEvt;}
    }
    
    private Subject<Unit> TipsBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnTipsBtn_UIButtonClick
    {
        get { return TipsBtn_UIButtonEvt; }
    }

}
