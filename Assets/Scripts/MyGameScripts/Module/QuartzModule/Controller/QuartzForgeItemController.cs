// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  QuartzForgeItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;

public partial interface IQuartzForgeItemController
{
    UniRx.IObservable<Unit> OnClickHandler { get; }
}

public partial class QuartzForgeItemController
{
    private Subject<Unit> _onclickEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnClickHandler { get { return _onclickEvt; } }  
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Add(_view.QuartzForgeItem_UIButton.onClick, () => { _onclickEvt.OnNext(new Unit());});
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void SetItemInfo(string name)
    {
        _view.Label_UILabel.text = name;
    }

    public bool IsSelect
    {
        get { return _view.Select; }
        set { _view.Select.SetActive(value);}
    }

    public bool IsLock
    {
        get { return _view.Icon_UISprite; }
        set
        {
            _view.Icon_UISprite.isGrey = value; 
            _view.Lock.SetActive(value);
        }
    }

}
