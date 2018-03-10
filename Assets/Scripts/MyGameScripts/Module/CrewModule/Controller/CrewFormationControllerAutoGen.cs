// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewFormationControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface ICrewFormationController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnTiledBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnTreatBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnMagicBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnControlBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnPowerBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnAllTypeBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnTypeBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnListBtn_UIButtonClick{get;}

}

public partial class CrewFormationController:FRPBaseController<
    CrewFormationController
    , CrewFormation
    , ICrewFormationController
    , ITeamFormationData>
    , ICrewFormationController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    TiledBtn_UIButtonEvt = View.TiledBtn_UIButton.AsObservable();
    TreatBtn_UIButtonEvt = View.TreatBtn_UIButton.AsObservable();
    MagicBtn_UIButtonEvt = View.MagicBtn_UIButton.AsObservable();
    ControlBtn_UIButtonEvt = View.ControlBtn_UIButton.AsObservable();
    PowerBtn_UIButtonEvt = View.PowerBtn_UIButton.AsObservable();
    AllTypeBtn_UIButtonEvt = View.AllTypeBtn_UIButton.AsObservable();
    TypeBtn_UIButtonEvt = View.TypeBtn_UIButton.AsObservable();
    ListBtn_UIButtonEvt = View.ListBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        TiledBtn_UIButtonEvt = TiledBtn_UIButtonEvt.CloseOnceNull();
        TreatBtn_UIButtonEvt = TreatBtn_UIButtonEvt.CloseOnceNull();
        MagicBtn_UIButtonEvt = MagicBtn_UIButtonEvt.CloseOnceNull();
        ControlBtn_UIButtonEvt = ControlBtn_UIButtonEvt.CloseOnceNull();
        PowerBtn_UIButtonEvt = PowerBtn_UIButtonEvt.CloseOnceNull();
        AllTypeBtn_UIButtonEvt = AllTypeBtn_UIButtonEvt.CloseOnceNull();
        TypeBtn_UIButtonEvt = TypeBtn_UIButtonEvt.CloseOnceNull();
        ListBtn_UIButtonEvt = ListBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> TiledBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnTiledBtn_UIButtonClick{
        get {return TiledBtn_UIButtonEvt;}
    }

    private Subject<Unit> TreatBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnTreatBtn_UIButtonClick{
        get {return TreatBtn_UIButtonEvt;}
    }

    private Subject<Unit> MagicBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnMagicBtn_UIButtonClick{
        get {return MagicBtn_UIButtonEvt;}
    }

    private Subject<Unit> ControlBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnControlBtn_UIButtonClick{
        get {return ControlBtn_UIButtonEvt;}
    }

    private Subject<Unit> PowerBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPowerBtn_UIButtonClick{
        get {return PowerBtn_UIButtonEvt;}
    }

    private Subject<Unit> AllTypeBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnAllTypeBtn_UIButtonClick{
        get {return AllTypeBtn_UIButtonEvt;}
    }

    private Subject<Unit> TypeBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnTypeBtn_UIButtonClick{
        get {return TypeBtn_UIButtonEvt;}
    }

    private Subject<Unit> ListBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnListBtn_UIButtonClick{
        get {return ListBtn_UIButtonEvt;}
    }


    }
