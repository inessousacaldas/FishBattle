// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EquipmentItemCellController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;

public partial class EquipmentItemCellController
{
    Equipment data;
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
    public void UpdateViewData(Equipment data,int quality = -1)
    {
        this.data = data;
        if (data == null) return;
        
        UIHelper.SetItemIcon(View.Eq_Icon_UISprite,data.icon);
        if(quality != -1)
            UIHelper.SetItemQualityIcon(View.Quality_Border_UISprite, quality);
    }
}
