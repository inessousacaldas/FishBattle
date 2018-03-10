// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  SpecialCopyMissionPanelControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface ISpecialCopyMissionPanelController : ICloseView
{
     UniRx.IObservable<Unit> OnOnClose_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnGoToNpc_UIButtonClick{get;}

}

public partial class SpecialCopyMissionPanelController:FRPBaseController<
    SpecialCopyMissionPanelController
    , SpecialCopyMissionPanel
    , ISpecialCopyMissionPanelController
    , ISpecialCopyData>
    , ISpecialCopyMissionPanelController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    OnClose_UIButtonEvt = View.OnClose_UIButton.AsObservable();
    GoToNpc_UIButtonEvt = View.GoToNpc_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        OnClose_UIButtonEvt = OnClose_UIButtonEvt.CloseOnceNull();
        GoToNpc_UIButtonEvt = GoToNpc_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> OnClose_UIButtonEvt;
    public UniRx.IObservable<Unit> OnOnClose_UIButtonClick{
        get {return OnClose_UIButtonEvt;}
    }

    private Subject<Unit> GoToNpc_UIButtonEvt;
    public UniRx.IObservable<Unit> OnGoToNpc_UIButtonClick{
        get {return GoToNpc_UIButtonEvt;}
    }


    }
