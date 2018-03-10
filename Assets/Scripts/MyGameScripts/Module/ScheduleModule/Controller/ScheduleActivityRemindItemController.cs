// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ScheduleActivityRemindItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppDto;

public partial class ScheduleActivityRemindItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        Btn_UIButtonEvt.Subscribe(_ =>
        {
            _isChose = !_isChose;
            View.SwitchOn_UISprite.enabled = _isChose;
            View.SwitchOff_UISprite.enabled = !_isChose;
            View.Btn_UIButton.transform.localPosition = _isChose ? new UnityEngine.Vector3(224, -2) : new UnityEngine.Vector3(191, -2);
            clickItemStream.OnNext(_isChose);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private bool _isChose = false;
    public void UpdateView(ScheduleActivity dto, bool isCancel)
    {
        if (dto == null) return;

        _isChose = !isCancel;
        View.Name_UILabel.text = dto.name;
        View.Date_UILabel.text = dto.openWeeksDesc;
        View.Time_UILabel.text = dto.activityTime;
        View.SwitchOn_UISprite.enabled = _isChose;
        View.SwitchOff_UISprite.enabled = !_isChose;
        View.Btn_UIButton.transform.localPosition = _isChose ? new UnityEngine.Vector3(224, -2) : new UnityEngine.Vector3(191, -2);
    }

    readonly UniRx.Subject<bool> clickItemStream = new UniRx.Subject<bool>();
    public UniRx.IObservable<bool> OnClickItemStream
    {
        get { return clickItemStream; }
    }
}
