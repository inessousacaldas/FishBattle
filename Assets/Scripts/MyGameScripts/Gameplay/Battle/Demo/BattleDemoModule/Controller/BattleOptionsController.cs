// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BattleOptionsController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;

public partial interface IBattleOptionsController
{
    UniRx.IObservable<string> BtnClickEvt { get; }
}

public partial class BattleOptionsController
{
    private CompositeDisposable _disposable;

    private Subject<string> _currClickEvt;
    public UniRx.IObservable<string> BtnClickEvt
    {
        get { return _currClickEvt; }
    }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
        _currClickEvt = new Subject<string>();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        _disposable.Add(AttackButton_UIButtonEvt.Subscribe(_ => { _currClickEvt.OnNext(_view.AttackButton_UIButton.name); }));
        _disposable.Add(MagicButton_UIButtonEvt.Subscribe(_ => { _currClickEvt.OnNext(_view.MagicButton_UIButton.name);}));
        _disposable.Add(EscapeButton_UIButtonEvt.Subscribe(_ => { _currClickEvt.OnNext(_view.EscapeButton_UIButton.name);}));
        _disposable.Add(UserItemButton_UIButtonEvt.Subscribe(_ => { _currClickEvt.OnNext(_view.UserItemButton_UIButton.name);}));
        _disposable.Add(SkillButton_UIButtonEvt.Subscribe(_ => { _currClickEvt.OnNext(_view.SkillButton_UIButton.name);}));
        EventDelegate.Add(_view.CallButton_UIButton.onClick, () =>
        {
            _currClickEvt.OnNext(_view.CallButton_UIButton.name);
        });
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

}
