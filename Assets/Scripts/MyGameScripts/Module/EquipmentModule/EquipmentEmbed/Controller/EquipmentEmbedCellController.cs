// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EquipmentEmbedCellController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;

public partial class EquipmentEmbedCellController
{
    //宝石用的~
    List<HoleItemLittleController> embedHoleItems = new List<HoleItemLittleController>();
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

    public void UpdateView(EquipmentEmbedCellVo vo, EquipmentDto equipment,bool isChoice)
    {
        
        View.OnSelectIcon_UISprite.gameObject.SetActive(isChoice);
        View.EqNameLbl_UILabel.text = vo.name;
        if(equipment == null)
        {
            SetIcon(vo.part);
            UIHelper.SetCommonIcon(View.IconBg_UISprite, "equipment_bg");
        }
        else
        {
            UIHelper.SetItemIcon(View.EqIcon_UISprite, equipment.equip.icon);
            UIHelper.SetItemQualityIcon(View.IconBg_UISprite, equipment.property.quality);
            View.EqNameLbl_UILabel.text = View.EqNameLbl_UILabel.text.WrapColor(ItemHelper.GetItemNameColorByRank(equipment.property.quality));
        }
       
        if (embedHoleItems.Count == 0)
        {
            for (int i = 0; i < View.EmbedNumberShow.transform.childCount; i++)
            {
                var ctrl = AddController<HoleItemLittleController, HoleItemLittle>(View.EmbedNumberShow.transform.GetChild(i).gameObject);
                embedHoleItems.Add(ctrl);
            }
        }

        embedHoleItems.ForEachI((x, i) =>
        {
            if (vo.EmbedHoleVoList[i].embedid == -1)
                x.SetEmbedHole(false);
            else
                x.SetEmbedHole(true);
        });
        
    }

    private void SetIcon(Equipment.PartType part)
    {
        string spriteName = "equipment_";
        switch(part)
        {
            case Equipment.PartType.Weapon:
                spriteName += "weapons";
                break;
            case Equipment.PartType.Shoe:
                spriteName += "shouse";
                break;
            case Equipment.PartType.Glove:
                spriteName += "gloves";
                break;
            case Equipment.PartType.Clothes:
                spriteName += "coat";
                break;
            case Equipment.PartType.AccOne:
            case Equipment.PartType.AccTwo:
                spriteName += "ornament";
                break;
        }
        View.EqIcon_UISprite.spriteName = spriteName;
    }

}
