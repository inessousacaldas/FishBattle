// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RecipeTipsItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;

public partial class RecipeTipsItemController
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

    public void UpdateView(int itemId)
    {
        if (ItemHelper.GetGeneralItemByItemId(itemId) == null)
            return;

        var itemData = ItemHelper.GetGeneralItemByItemId(itemId);
        UIHelper.SetItemIcon(View.Icon_UISprite, itemData.icon);
        View.Name_UILabel.text = itemData.name;
        View.Des_UILabel.text = itemData.description;
    }
}
