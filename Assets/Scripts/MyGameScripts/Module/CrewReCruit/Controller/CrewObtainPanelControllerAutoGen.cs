// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewObtainPanelControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface ICrewObtainPanelController : ICloseView
{
     UniRx.IObservable<Unit> OnOneBuy_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
        
    UniRx.IObservable<Unit> OnSkillButton1_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnSkillButton2_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnSkillButton3_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnSkillButton4_UIButtonClick { get; }

}

public partial class CrewObtainPanelController:FRPBaseController<
    CrewObtainPanelController
    , CrewObtainPanel
    , ICrewObtainPanelController
    , ICrewReCruitData>
    , ICrewObtainPanelController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
        OneBuy_UIButtonEvt = View.OneBuy_UIButton.AsObservable();
        CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
        OnSkillButton1_UIButtonEvt = View.SkillIcon_1_UIButton.AsObservable();
        OnSkillButton2_UIButtonEvt = View.SkillIcon_2_UIButton.AsObservable();
        OnSkillButton3_UIButtonEvt = View.SkillIcon_3_UIButton.AsObservable();
        OnSkillButton4_UIButtonEvt = View.SkillIcon_4_UIButton.AsObservable();
        }
        
        protected override void RemoveEvent()
        {
        OneBuy_UIButtonEvt = OneBuy_UIButtonEvt.CloseOnceNull();
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        OnSkillButton1_UIButtonEvt = OnSkillButton1_UIButtonEvt.CloseOnceNull();
        OnSkillButton2_UIButtonEvt = OnSkillButton2_UIButtonEvt.CloseOnceNull();
        OnSkillButton3_UIButtonEvt = OnSkillButton3_UIButtonEvt.CloseOnceNull();
        OnSkillButton4_UIButtonEvt = OnSkillButton3_UIButtonEvt.CloseOnceNull();
        }
        
            private Subject<Unit> OneBuy_UIButtonEvt;
    public UniRx.IObservable<Unit> OnOneBuy_UIButtonClick{
        get {return OneBuy_UIButtonEvt;}
    }

    private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> OnSkillButton1_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSkillButton1_UIButtonClick {
        get { return OnSkillButton1_UIButtonEvt; }
    }

    private Subject<Unit> OnSkillButton2_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSkillButton2_UIButtonClick {
        get { return OnSkillButton2_UIButtonEvt; }
    }

    private Subject<Unit> OnSkillButton3_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSkillButton3_UIButtonClick {
        get { return OnSkillButton3_UIButtonEvt; }
    }

    private Subject<Unit> OnSkillButton4_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSkillButton4_UIButtonClick {
        get { return OnSkillButton4_UIButtonEvt; }
    }

}
