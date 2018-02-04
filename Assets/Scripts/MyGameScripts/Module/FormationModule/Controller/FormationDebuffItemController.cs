// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FormationDebuffItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UnityEngine;

public partial class FormationDebuffItemController
{

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    //用特定接口类型数据刷新界面
    public void UpdateView(IFormationData data)
    {

    }

    public void SetNameColor(Color color)
    {
        View.NameLable_UILabel.color = color;
        View.NameLable_UILabel.MarkAsChanged();
    }

    public void SetName(string name)
    {
        View.NameLable_UILabel.text = name;
    }

    public void SetEffect(string effect)
    {
        View.EffectLable_UILabel.text = effect;
    }

    public void SetScroll(UIScrollView scroll)
    {
        var s = View.gameObject.GetMissingComponent<UIDragScrollView>();
        s.scrollView = scroll;
    }
}
