// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MiniWorldMapViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IMiniWorldMapViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCurrMapBtn_UIButtonClick{get;}

}

public partial class MiniWorldMapViewController:FRPBaseController<
    MiniWorldMapViewController
    , MiniWorldMapView
    , IMiniWorldMapViewController
    , IWorldMapData>
    , IMiniWorldMapViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    CurrMapBtn_UIButtonEvt = View.CurrMapBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        CurrMapBtn_UIButtonEvt = CurrMapBtn_UIButtonEvt.CloseOnceNull();

        }

    private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> CurrMapBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCurrMapBtn_UIButtonClick{
        get {return CurrMapBtn_UIButtonEvt;}
    }


    }
