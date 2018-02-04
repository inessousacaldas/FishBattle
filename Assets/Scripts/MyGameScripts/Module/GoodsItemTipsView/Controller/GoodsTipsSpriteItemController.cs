// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GoodsTipsSpriteItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;

public partial class GoodsTipsSpriteItemController
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

   public void UpdateView(string icon="")
    {
        if(icon.Equals(string.Empty))
        {
            View.Bg_UISprite.gameObject.SetActive(true);
        }
        else
        {
            View.Bg_UISprite.gameObject.SetActive(false);
        }
        //todo
        UIHelper.SetItemIcon(View.ItemIcon_UISprite, icon);
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

}
