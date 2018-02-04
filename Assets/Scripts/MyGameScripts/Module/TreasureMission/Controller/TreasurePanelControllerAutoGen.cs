// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TreasurePanelControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface ITreasurePanelController : ICloseView
{
     UniRx.IObservable<Unit> OnTreasureBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnOnClose_UIButtonClick{get;}

}

public partial class TreasurePanelController:FRPBaseController<
    TreasurePanelController
    , TreasurePanel
    , ITreasurePanelController
    , ITreasureMissionData>
    , ITreasurePanelController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    TreasureBtn_UIButtonEvt = View.TreasureBtn_UIButton.AsObservable();
    OnClose_UIButtonEvt = View.OnClose_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        TreasureBtn_UIButtonEvt = TreasureBtn_UIButtonEvt.CloseOnceNull();
        OnClose_UIButtonEvt = OnClose_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> TreasureBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnTreasureBtn_UIButtonClick{
        get {return TreasureBtn_UIButtonEvt;}
    }

    private Subject<Unit> OnClose_UIButtonEvt;
    public UniRx.IObservable<Unit> OnOnClose_UIButtonClick{
        get {return OnClose_UIButtonEvt;}
    }


    }
