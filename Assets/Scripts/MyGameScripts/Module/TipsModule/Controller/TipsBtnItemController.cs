// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TipsBtnItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;

public partial class TipsBtnItemController
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

    public void UpdateView(string str, Action click)
    {
        View.Label_UILabel.text = str;
    }

    public int GetHeight()
    {
        return this.gameObject.GetComponent<UISprite>().height;
    }

}
