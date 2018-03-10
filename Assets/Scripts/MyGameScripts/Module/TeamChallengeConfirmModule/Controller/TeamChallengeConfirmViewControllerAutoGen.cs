// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamChallengeConfirmViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface ITeamChallengeConfirmViewController : ICloseView
{
     UniRx.IObservable<Unit> OnSurelBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCanelBtn_UIButtonClick{get;}

}

public partial class TeamChallengeConfirmViewController:FRPBaseController<
    TeamChallengeConfirmViewController
    , TeamChallengeConfirmView
    , ITeamChallengeConfirmViewController
    , ITremChallengeConfirmData>
    , ITeamChallengeConfirmViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    SurelBtn_UIButtonEvt = View.SurelBtn_UIButton.AsObservable();
    CanelBtn_UIButtonEvt = View.CanelBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        SurelBtn_UIButtonEvt = SurelBtn_UIButtonEvt.CloseOnceNull();
        CanelBtn_UIButtonEvt = CanelBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> SurelBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSurelBtn_UIButtonClick{
        get {return SurelBtn_UIButtonEvt;}
    }

    private Subject<Unit> CanelBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCanelBtn_UIButtonClick{
        get {return CanelBtn_UIButtonEvt;}
    }


    }
