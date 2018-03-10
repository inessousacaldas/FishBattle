// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BattlePVEFailViewController.cs
// Author   : xush
// Created  : 12/20/2017 4:38:09 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using UnityEngine;

public partial interface IBattlePVEFailViewController
{

}
public partial class BattlePVEFailViewController
{
    private CompositeDisposable _disposable;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
            _disposable = new CompositeDisposable();
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        _disposable.Add(OnEquipmentBtn_UIButtonClick.Subscribe(_ => { }));
        _disposable.Add(OnCrewBtn_UIButtonClick.Subscribe(_ => { }));
        _disposable.Add(OnUpSkillGradeBtn_UIButtonClick.Subscribe(_ => { }));
        _disposable.Add(OnUpGradeBtn_UIButtonClick.Subscribe(_ => { }));
	    _disposable.Add(CloseWidget_UIButtonEvt.Subscribe(_ => { ProxyBattleDemoModule.ClosePVEFailView(); }));
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        base.OnDispose();
    }
}
