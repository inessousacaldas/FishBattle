// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MartialViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IMartialViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnTipsBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnFirstWarBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnFirstWinBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnToggleBtn_UIButtonClick{get;}

}

public partial class MartialViewController:FRPBaseController<
    MartialViewController
    , MartialView
    , IMartialViewController
    , IMartialData>
    , IMartialViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    TipsBtn_UIButtonEvt = View.TipsBtn_UIButton.AsObservable();
    FirstWarBtn_UIButtonEvt = View.FirstWarBtn_UIButton.AsObservable();
    FirstWinBtn_UIButtonEvt = View.FirstWinBtn_UIButton.AsObservable();
    ToggleBtn_UIButtonEvt = View.ToggleBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        TipsBtn_UIButtonEvt = TipsBtn_UIButtonEvt.CloseOnceNull();
        FirstWarBtn_UIButtonEvt = FirstWarBtn_UIButtonEvt.CloseOnceNull();
        FirstWinBtn_UIButtonEvt = FirstWinBtn_UIButtonEvt.CloseOnceNull();
        ToggleBtn_UIButtonEvt = ToggleBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> TipsBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnTipsBtn_UIButtonClick{
        get {return TipsBtn_UIButtonEvt;}
    }

    private Subject<Unit> FirstWarBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnFirstWarBtn_UIButtonClick{
        get {return FirstWarBtn_UIButtonEvt;}
    }

    private Subject<Unit> FirstWinBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnFirstWinBtn_UIButtonClick{
        get {return FirstWinBtn_UIButtonEvt;}
    }

    private Subject<Unit> ToggleBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnToggleBtn_UIButtonClick{
        get {return ToggleBtn_UIButtonEvt;}
    }


    }
