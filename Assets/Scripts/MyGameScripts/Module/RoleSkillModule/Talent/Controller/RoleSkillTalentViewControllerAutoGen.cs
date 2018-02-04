// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillTalentViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface IRoleSkillTalentViewController
{
     UniRx.IObservable<Unit> OnbtnReset_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnbtnAdd_UIButtonClick{get;}

}

public partial class RoleSkillTalentViewController:MonolessViewController<RoleSkillTalentView>, IRoleSkillTalentViewController
{
	    //机器自动生成的事件订阅
        protected override void InitReactiveEvents()
        {
            base.InitReactiveEvents();
    btnReset_UIButtonEvt = View.btnReset_UIButton.AsObservable();
    btnAdd_UIButtonEvt = View.btnAdd_UIButton.AsObservable();

        }
        
        protected override void ClearReactiveEvents()
        {
        btnReset_UIButtonEvt = btnReset_UIButtonEvt.CloseOnceNull();
        btnAdd_UIButtonEvt = btnAdd_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> btnReset_UIButtonEvt;
    public UniRx.IObservable<Unit> OnbtnReset_UIButtonClick{
        get {return btnReset_UIButtonEvt;}
    }

    private Subject<Unit> btnAdd_UIButtonEvt;
    public UniRx.IObservable<Unit> OnbtnAdd_UIButtonClick{
        get {return btnAdd_UIButtonEvt;}
    }


    }
