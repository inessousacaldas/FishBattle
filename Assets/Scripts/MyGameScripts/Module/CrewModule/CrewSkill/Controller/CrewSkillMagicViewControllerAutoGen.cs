﻿// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewSkillMagicViewController.cs
// the file is generated by tools
// **********************************************************************

using System;
using UniRx;

public partial class CrewSkillMagicViewController:MonolessViewController<CrewSkillMagicView>, ICrewSkillMagicViewController
{

    //机器自动生成的事件绑定
    protected override void InitReactiveEvents(){
        btnDLQ_UIButtonEvt = View.btnDLQ_UIButton.AsObservable();

    }

    protected override void ClearReactiveEvents(){
        btnDLQ_UIButtonEvt = btnDLQ_UIButtonEvt.CloseOnceNull();

    }

    private Subject<Unit> btnDLQ_UIButtonEvt;
    public UniRx.IObservable<Unit> OnbtnDLQ_UIButtonClick{
        get {return btnDLQ_UIButtonEvt;}
    }


}
