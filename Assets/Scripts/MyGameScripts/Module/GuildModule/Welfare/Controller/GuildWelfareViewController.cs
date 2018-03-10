// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildWelfareViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public partial interface IGuildWelfareViewController
{
    void UpdateView(IGuildMainData data);
    UniRx.IObservable<GuildBuildItemController> OncheckBtn_UIButtonClick { get; }
}

public partial class GuildWelfareViewController
{
    private CompositeDisposable _disposable;
    private List<GuildBuildItemController> _itemList = new List<GuildBuildItemController>();
    private Subject<GuildBuildItemController> checkBtn_UIButtonEvt = new Subject<GuildBuildItemController>();
    
    public UniRx.IObservable<GuildBuildItemController> OncheckBtn_UIButtonClick
    {
        get { return checkBtn_UIButtonEvt; }
    }
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
        {
            _disposable.Clear();
        }
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        _itemList.Clear();
        base.OnDispose();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
    }

    public void UpdateView(IGuildMainData data)
    {
        var list = data.GuildWelfareList;
        if (list == null) return;
        if(_itemList.Count == 0)
        {
            list.ForEach(e =>
            {
                var ctrl = AddChild<GuildBuildItemController, GuildBuildItem>(View.UIGrid_UIGrid.gameObject, GuildBuildItem.NAME);
                _disposable.Add(ctrl.OncheckBtn_UIButtonClick.Subscribe(f => checkBtn_UIButtonEvt.OnNext(ctrl)));
                ctrl.UpdateWelfareView(e);
                _itemList.Add(ctrl);
            });
        }
    }
    
}
