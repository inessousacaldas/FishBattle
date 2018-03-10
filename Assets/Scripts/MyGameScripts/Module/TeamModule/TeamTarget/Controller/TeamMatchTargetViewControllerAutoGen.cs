// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamMatchTargetViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface ITeamMatchTargetViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnConfirmBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnChangeLvBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCancelBtn_UIButtonClick{get;}

}

public partial class TeamMatchTargetViewController:FRPBaseController<
    TeamMatchTargetViewController
    , TeamMatchTargetView
    , ITeamMatchTargetViewController
    , ITeamData>
    , ITeamMatchTargetViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    ConfirmBtn_UIButtonEvt = View.ConfirmBtn_UIButton.AsObservable();
    ChangeLvBtn_UIButtonEvt = View.ChangeLvBtn_UIButton.AsObservable();
    CancelBtn_UIButtonEvt = View.CancelBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        ConfirmBtn_UIButtonEvt = ConfirmBtn_UIButtonEvt.CloseOnceNull();
        ChangeLvBtn_UIButtonEvt = ChangeLvBtn_UIButtonEvt.CloseOnceNull();
        CancelBtn_UIButtonEvt = CancelBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> ConfirmBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnConfirmBtn_UIButtonClick{
        get {return ConfirmBtn_UIButtonEvt;}
    }

    private Subject<Unit> ChangeLvBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnChangeLvBtn_UIButtonClick{
        get {return ChangeLvBtn_UIButtonEvt;}
    }

    private Subject<Unit> CancelBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCancelBtn_UIButtonClick{
        get {return CancelBtn_UIButtonEvt;}
    }


    }
