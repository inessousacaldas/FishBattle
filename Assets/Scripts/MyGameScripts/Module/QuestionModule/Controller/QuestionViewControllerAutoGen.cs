// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  QuestionViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IQuestionViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnRewardBoxBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnHelpBtn_UIButtonClick{get;}

}

public partial class QuestionViewController:FRPBaseController<
    QuestionViewController
    , QuestionView
    , IQuestionViewController
    , IQuestionData>
    , IQuestionViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    RewardBoxBtn_UIButtonEvt = View.RewardBoxBtn_UIButton.AsObservable();
    HelpBtn_UIButtonEvt = View.HelpBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        RewardBoxBtn_UIButtonEvt = RewardBoxBtn_UIButtonEvt.CloseOnceNull();
        HelpBtn_UIButtonEvt = HelpBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> RewardBoxBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnRewardBoxBtn_UIButtonClick{
        get {return RewardBoxBtn_UIButtonEvt;}
    }

    private Subject<Unit> HelpBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnHelpBtn_UIButtonClick{
        get {return HelpBtn_UIButtonEvt;}
    }


    }
