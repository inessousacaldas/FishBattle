// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamSetItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;

public partial class TeamSetItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Add(_view.Button_UIButton.onClick, OnSliderClick);
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private void OnSliderClick()
    {
        if (_view.Slider_UISlider.value == 1)
            _view.Slider_UISlider.value = 0;
        else
            _view.Slider_UISlider.value = 1;
    }

    public void UpdateInfo(string name, string desc)
    {
        _view.NameLb_UILabel.text = name;
        _view.DescLb_UILabel.text = desc;
    }
}
