// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BattlePVPFailViewController.cs
// Author   : xush
// Created  : 12/20/2017 4:45:34 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using UnityEngine;

public partial interface IBattlePVPFailViewController
{

}
public partial class BattlePVPFailViewController
{
    private CompositeDisposable _disposable;

    private BattlePlayerItemController _playerCtrl;
    private BattlePlayerItemController _enemyCtrl;
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
        _playerCtrl = AddController<BattlePlayerItemController, BattlePlayerItem>(_view.Player);
        _enemyCtrl = AddController<BattlePlayerItemController, BattlePlayerItem>(_view.Enemy);
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
	{
        _disposable.Add(OnEquipmentBtn_UIButtonClick.Subscribe(_ => { }));
        _disposable.Add(OnCrewBtn_UIButtonClick.Subscribe(_ => { }));
        _disposable.Add(OnUpSkillGradeBtn_UIButtonClick.Subscribe(_ => { }));
        _disposable.Add(OnUpGradeBtn_UIButtonClick.Subscribe(_ => { }));
        _disposable.Add(OnBackHomeBtn_UIButtonClick.Subscribe(_ => { }));
        _disposable.Add(OnPerfectBtn_UIButtonClick.Subscribe(_ => { }));
        _disposable.Add(OnResurgenceBtn_UIButtonClick.Subscribe(_ => { }));
        _disposable.Add(OnCloseWidget_UIButtonClick.Subscribe(_ => { }));
        _disposable.Add(CloseWidget_UIButtonEvt.Subscribe(_ => { ProxyBattleDemoModule.ClosePVPFailView(); }));
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        base.OnDispose();
    }

    // 业务逻辑数据刷新
    public void SetViewInfo()
    {
        _playerCtrl.SetPlayerInfo();
        _enemyCtrl.SetPlayerInfo();
        _view.PerfectDescLb_UILabel.text = string.Format("+{0}%{1}({2}分钟)", 20, "伤害", 10);
        _view.PerfectLb_UILabel.text = string.Format("消耗[ff2727]{0}钻石[-]", 20);
        _view.ResurgenceLb_UILabel.text = String.Format("剩余免费[7EE830]{0}次[-]", 3);
    }
}
