// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillSpecialityAddViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface IRoleSkillSpecialityAddViewController:ICloseView
{
     UniRx.IObservable<Unit> OnbtnItem_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnbtnTrain1_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnbtnTrain10_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick { get;}
}

public partial class RoleSkillSpecialityAddViewController:FRPBaseController<
    RoleSkillSpecialityAddViewController
    ,RoleSkillSpecialityAddView
    ,IRoleSkillSpecialityAddViewController
    ,IRoleSkillData>
    , IRoleSkillSpecialityAddViewController
{
    //机器自动生成的事件订阅
    protected override void RegistEvent()
    {
        base.RegistEvent();
        btnItem_UIButtonEvt = View.btnItem_UIButton.AsObservable();
        btnTrain1_UIButtonEvt = View.btnTrain1_UIButton.AsObservable();
        btnTrain10_UIButtonEvt = View.btnTrain10_UIButton.AsObservable();
        CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
        
        }

    protected override void RemoveEvent()
    {
        btnItem_UIButtonEvt = btnItem_UIButtonEvt.CloseOnceNull();
        btnTrain1_UIButtonEvt = btnTrain1_UIButtonEvt.CloseOnceNull();
        btnTrain10_UIButtonEvt = btnTrain10_UIButtonEvt.CloseOnceNull();
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();

    }

    private Subject<Unit> btnItem_UIButtonEvt;
    public UniRx.IObservable<Unit> OnbtnItem_UIButtonClick {
        get { return btnItem_UIButtonEvt; }
    }

    private Subject<Unit> btnTrain1_UIButtonEvt;
    public UniRx.IObservable<Unit> OnbtnTrain1_UIButtonClick {
        get { return btnTrain1_UIButtonEvt; }
    }

    private Subject<Unit> btnTrain10_UIButtonEvt;
    public UniRx.IObservable<Unit> OnbtnTrain10_UIButtonClick {
        get { return btnTrain10_UIButtonEvt; }
    }

    private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick
    {
        get { return CloseBtn_UIButtonEvt; }
    }


}
