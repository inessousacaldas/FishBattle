// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  QuartzViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IQuartzViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnShowTypeBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnPullbackBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnTypeBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCloseExtendBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnPullBtn_UIButtonClick{get;}

}

public partial class QuartzViewController:FRPBaseController<
    QuartzViewController
    , QuartzView
    , IQuartzViewController
    , IQuartzData>
    , IQuartzViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    ShowTypeBtn_UIButtonEvt = View.ShowTypeBtn_UIButton.AsObservable();
    PullbackBtn_UIButtonEvt = View.PullbackBtn_UIButton.AsObservable();
    TypeBtn_UIButtonEvt = View.TypeBtn_UIButton.AsObservable();
    CloseExtendBtn_UIButtonEvt = View.CloseExtendBtn_UIButton.AsObservable();
    PullBtn_UIButtonEvt = View.PullBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        ShowTypeBtn_UIButtonEvt = ShowTypeBtn_UIButtonEvt.CloseOnceNull();
        PullbackBtn_UIButtonEvt = PullbackBtn_UIButtonEvt.CloseOnceNull();
        TypeBtn_UIButtonEvt = TypeBtn_UIButtonEvt.CloseOnceNull();
        CloseExtendBtn_UIButtonEvt = CloseExtendBtn_UIButtonEvt.CloseOnceNull();
        PullBtn_UIButtonEvt = PullBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> ShowTypeBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnShowTypeBtn_UIButtonClick{
        get {return ShowTypeBtn_UIButtonEvt;}
    }

    private Subject<Unit> PullbackBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPullbackBtn_UIButtonClick{
        get {return PullbackBtn_UIButtonEvt;}
    }

    private Subject<Unit> TypeBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnTypeBtn_UIButtonClick{
        get {return TypeBtn_UIButtonEvt;}
    }

    private Subject<Unit> CloseExtendBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseExtendBtn_UIButtonClick{
        get {return CloseExtendBtn_UIButtonEvt;}
    }

    private Subject<Unit> PullBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPullBtn_UIButtonClick{
        get {return PullBtn_UIButtonEvt;}
    }


    }
