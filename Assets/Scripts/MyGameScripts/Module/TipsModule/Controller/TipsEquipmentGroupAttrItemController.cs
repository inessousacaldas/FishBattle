// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TipsEquipmentGroupAttrItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;

public partial class TipsEquipmentGroupAttrItemController
{
    public Equipment.PartType part;
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
    public void InitData(Equipment.PartType part)
    {
        this.part = part;
    }
    public void UpdateView(string name,bool isContain)
    {
        var temppart = this.part;
        if (temppart == Equipment.PartType.AccTwo)
            temppart = Equipment.PartType.AccOne;
        var partName = DataCache.getDtoByCls<EquipmentPart>((int)temppart);
        View.AttrLabel_UILabel.text = string.Format("【{0}】{1}", name, partName.name);
        var greyColor = "A8A8B0FF";
        var normalColor = "68c4ff";
        var color = normalColor;
        if (isContain)
        {
            color = normalColor;
            View.AttrLabel_UILabel.text = View.AttrLabel_UILabel.text.WrapColor(normalColor);

        }
        else
        {
            color = greyColor;
            View.AttrLabel_UILabel.text = View.AttrLabel_UILabel.text.WrapColor(greyColor);
        }
            


        var spike = View.gameObject.FindScript<UILabel>("Label");
        if (spike != null)
        {
            spike.text = spike.text.WrapColor(color);
        }
    }
}
