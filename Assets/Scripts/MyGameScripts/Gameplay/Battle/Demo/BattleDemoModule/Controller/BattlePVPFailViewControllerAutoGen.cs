// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BattlePVPFailViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface IBattlePVPFailViewController
{
     UniRx.IObservable<Unit> OnEquipmentBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCrewBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnUpSkillGradeBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnUpGradeBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnBackHomeBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnResurgenceBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnPerfectBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCloseWidget_UIButtonClick{get;}

}

public partial class BattlePVPFailViewController:MonoViewController<BattlePVPFailView>
    , IBattlePVPFailViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    EquipmentBtn_UIButtonEvt = View.EquipmentBtn_UIButton.AsObservable();
    CrewBtn_UIButtonEvt = View.CrewBtn_UIButton.AsObservable();
    UpSkillGradeBtn_UIButtonEvt = View.UpSkillGradeBtn_UIButton.AsObservable();
    UpGradeBtn_UIButtonEvt = View.UpGradeBtn_UIButton.AsObservable();
    BackHomeBtn_UIButtonEvt = View.BackHomeBtn_UIButton.AsObservable();
    ResurgenceBtn_UIButtonEvt = View.ResurgenceBtn_UIButton.AsObservable();
    PerfectBtn_UIButtonEvt = View.PerfectBtn_UIButton.AsObservable();
    CloseWidget_UIButtonEvt = View.CloseWidget_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        EquipmentBtn_UIButtonEvt = EquipmentBtn_UIButtonEvt.CloseOnceNull();
        CrewBtn_UIButtonEvt = CrewBtn_UIButtonEvt.CloseOnceNull();
        UpSkillGradeBtn_UIButtonEvt = UpSkillGradeBtn_UIButtonEvt.CloseOnceNull();
        UpGradeBtn_UIButtonEvt = UpGradeBtn_UIButtonEvt.CloseOnceNull();
        BackHomeBtn_UIButtonEvt = BackHomeBtn_UIButtonEvt.CloseOnceNull();
        ResurgenceBtn_UIButtonEvt = ResurgenceBtn_UIButtonEvt.CloseOnceNull();
        PerfectBtn_UIButtonEvt = PerfectBtn_UIButtonEvt.CloseOnceNull();
        CloseWidget_UIButtonEvt = CloseWidget_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> EquipmentBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnEquipmentBtn_UIButtonClick{
        get {return EquipmentBtn_UIButtonEvt;}
    }

    private Subject<Unit> CrewBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCrewBtn_UIButtonClick{
        get {return CrewBtn_UIButtonEvt;}
    }

    private Subject<Unit> UpSkillGradeBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnUpSkillGradeBtn_UIButtonClick{
        get {return UpSkillGradeBtn_UIButtonEvt;}
    }

    private Subject<Unit> UpGradeBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnUpGradeBtn_UIButtonClick{
        get {return UpGradeBtn_UIButtonEvt;}
    }

    private Subject<Unit> BackHomeBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnBackHomeBtn_UIButtonClick{
        get {return BackHomeBtn_UIButtonEvt;}
    }

    private Subject<Unit> ResurgenceBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnResurgenceBtn_UIButtonClick{
        get {return ResurgenceBtn_UIButtonEvt;}
    }

    private Subject<Unit> PerfectBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPerfectBtn_UIButtonClick{
        get {return PerfectBtn_UIButtonEvt;}
    }

    private Subject<Unit> CloseWidget_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseWidget_UIButtonClick{
        get {return CloseWidget_UIButtonEvt;}
    }


    }
