// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ScheduleRewardTypeBtnController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppDto;

public partial class ScheduleRewardTypeBtnController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        OnScheduleRewardTypeBtn_UIButtonClick.Subscribe(_ =>
        {
            clickItemStream.OnNext(_idx);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private int _idx = 0;
    public void UpdateView(ScheduleActivity.ControlTypeEnum type, string name, bool isChose)
    {
        _idx = (int)type;
        View.Label_UILabel.text = name;
        View.Sprite_UISprite.enabled = isChose;
    }

    readonly UniRx.Subject<int> clickItemStream = new UniRx.Subject<int>();
    public UniRx.IObservable<int> OnClickItemStream
    {
        get { return clickItemStream; }
    }
}
