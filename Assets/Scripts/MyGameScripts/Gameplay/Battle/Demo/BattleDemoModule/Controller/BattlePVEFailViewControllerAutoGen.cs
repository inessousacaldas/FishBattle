// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BattlePVEFailViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface IBattlePVEFailViewController
{
     UniRx.IObservable<Unit> OnUpGradeBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnUpSkillGradeBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCrewBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnEquipmentBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCloseWidget_UIButtonClick{get;}

}

public partial class BattlePVEFailViewController:MonoViewController<BattlePVEFailView>
    , IBattlePVEFailViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    UpGradeBtn_UIButtonEvt = View.UpGradeBtn_UIButton.AsObservable();
    UpSkillGradeBtn_UIButtonEvt = View.UpSkillGradeBtn_UIButton.AsObservable();
    CrewBtn_UIButtonEvt = View.CrewBtn_UIButton.AsObservable();
    EquipmentBtn_UIButtonEvt = View.EquipmentBtn_UIButton.AsObservable();
    CloseWidget_UIButtonEvt = View.CloseWidget_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        UpGradeBtn_UIButtonEvt = UpGradeBtn_UIButtonEvt.CloseOnceNull();
        UpSkillGradeBtn_UIButtonEvt = UpSkillGradeBtn_UIButtonEvt.CloseOnceNull();
        CrewBtn_UIButtonEvt = CrewBtn_UIButtonEvt.CloseOnceNull();
        EquipmentBtn_UIButtonEvt = EquipmentBtn_UIButtonEvt.CloseOnceNull();
        CloseWidget_UIButtonEvt = CloseWidget_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> UpGradeBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnUpGradeBtn_UIButtonClick{
        get {return UpGradeBtn_UIButtonEvt;}
    }

    private Subject<Unit> UpSkillGradeBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnUpSkillGradeBtn_UIButtonClick{
        get {return UpSkillGradeBtn_UIButtonEvt;}
    }

    private Subject<Unit> CrewBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCrewBtn_UIButtonClick{
        get {return CrewBtn_UIButtonEvt;}
    }

    private Subject<Unit> EquipmentBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnEquipmentBtn_UIButtonClick{
        get {return EquipmentBtn_UIButtonEvt;}
    }

    private Subject<Unit> CloseWidget_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseWidget_UIButtonClick{
        get {return CloseWidget_UIButtonEvt;}
    }


    }
