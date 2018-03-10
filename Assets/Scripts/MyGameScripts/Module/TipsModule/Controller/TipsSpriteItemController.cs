// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TipsSpriteItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;

public partial class TipsSpriteItemController
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

   public void UpdateView(string name, int quality)
    {
        UIHelper.SetItemIcon(View.ItemIcon_UISprite, name);
        UIHelper.SetItemQualityIcon(View.BgIcon_UISprite, quality);
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

}
