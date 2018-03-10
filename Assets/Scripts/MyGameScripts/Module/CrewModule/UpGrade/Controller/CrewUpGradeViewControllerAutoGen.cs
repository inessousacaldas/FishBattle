// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewUpGradeViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface ICrewUpGradeViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnUpGradeBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnUseAllBtn_UIButtonClick{get;}

}

public partial class CrewUpGradeViewController:FRPBaseController<
    CrewUpGradeViewController
    , CrewUpGradeView
    , ICrewUpGradeViewController
    , ICrewViewData>
    , ICrewUpGradeViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    UpGradeBtn_UIButtonEvt = View.UpGradeBtn_UIButton.AsObservable();
    UseAllBtn_UIButtonEvt = View.UseAllBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        UpGradeBtn_UIButtonEvt = UpGradeBtn_UIButtonEvt.CloseOnceNull();
        UseAllBtn_UIButtonEvt = UseAllBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> UpGradeBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnUpGradeBtn_UIButtonClick{
        get {return UpGradeBtn_UIButtonEvt;}
    }

    private Subject<Unit> UseAllBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnUseAllBtn_UIButtonClick{
        get {return UseAllBtn_UIButtonEvt;}
    }


    }
