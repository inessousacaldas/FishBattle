// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewReCruitPanelControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface ICrewReCruitPanelController : ICloseView
{
     UniRx.IObservable<Unit> OnSeniorOneButton_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnNormalOneButton_UIButtonClick{get;}
    UniRx.IObservable<Unit> OnCloseButton_UIButtonClick { get; }
}

public partial class CrewReCruitPanelController:FRPBaseController<
    CrewReCruitPanelController
    , CrewReCruitPanel
    , ICrewReCruitPanelController
    , ICrewReCruitData>
    , ICrewReCruitPanelController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    SeniorOneButton_UIButtonEvt = View.SeniorOneButton_UIButton.AsObservable();
    NormalOneButton_UIButtonEvt = View.NormalOneButton_UIButton.AsObservable();
        OnCloseButton_UIButtonEvt = View.CloseButton_UIButton.AsObservable();
        }
        
        protected override void RemoveEvent()
        {
        SeniorOneButton_UIButtonEvt = SeniorOneButton_UIButtonEvt.CloseOnceNull();
        NormalOneButton_UIButtonEvt = NormalOneButton_UIButtonEvt.CloseOnceNull();
        OnCloseButton_UIButtonEvt = OnCloseButton_UIButtonEvt.CloseOnceNull();
    }
        
            private Subject<Unit> SeniorOneButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSeniorOneButton_UIButtonClick{
        get {return SeniorOneButton_UIButtonEvt;}
    }


    private Subject<Unit> NormalOneButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnNormalOneButton_UIButtonClick{
        get {return NormalOneButton_UIButtonEvt;}
    }

    private Subject<Unit> OnCloseButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseButton_UIButtonClick {
        get { return OnCloseButton_UIButtonEvt; }
    }

    }
