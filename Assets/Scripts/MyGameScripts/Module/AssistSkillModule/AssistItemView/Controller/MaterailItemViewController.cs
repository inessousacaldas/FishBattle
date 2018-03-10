// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MaterailItemViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;


public partial class MaterailItemViewController
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

    public void UpdateView(int id, long count, bool isHave=false)
    {
        this.gameObject.SetActive(true);
        View.Label_UILabel.text = isHave ? "拥有数量" : "单次消耗";
        UIHelper.SetAppVirtualItemIcon(View.Icon_UISprite, (AppVirtualItem.VirtualItemEnum)id);
        View.Num_UILabel.text = count.ToString();
        if (!isHave && ModelManager.Player.GetPlayerWealthById(id) < count)
            View.Num_UILabel.text = View.Num_UILabel.text.WrapColor(ColorConstantV3.Color_Red);
    }
}
