// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EmojiItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;

public partial class EmojiItemController
{
    private string _prefix;
    public string Prefix{get{ return _prefix;}}
    private Subject<Unit> clickEvt;

    public UniRx.IObservable<Unit> ClickHandler {
        get { return clickEvt; }
    }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        clickEvt = _view.gameObject.OnClickAsObservable();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        
    }

    protected override void OnDispose()
    {
        clickEvt = clickEvt.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    //用特定接口类型数据刷新界面
    public void UpdateView(string prefix)
    {
        _prefix = prefix;
//帧数大于1的只取第一帧
        if (EmojiDataUtil.GetEmotionMaxFrameCount(prefix) >= 1)
        {
            _view._spriteAnimator.sprite.spriteName = prefix + "_00";
            _view._spriteAnimator.namePrefix = prefix + "_";
        }
        else
        {
            _view._spriteAnimator.sprite.spriteName = prefix;
        }
    }
}
