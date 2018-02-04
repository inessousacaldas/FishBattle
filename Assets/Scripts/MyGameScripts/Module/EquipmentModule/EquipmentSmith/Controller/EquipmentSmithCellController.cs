// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EquipmentSmithCellController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;

public partial class EquipmentSmithCellController
{
    public enum ShowType
    {
        One,//打造专用，只有一行文字
        Two,//洗练，纹章，精炼，进阶使用~
        Three,//宝石专用
    }

    //Index方便后面服用使用ReclyList
    public int index;

    //宝石用的~
    List<HoleItemLittleController> embedHoleItems = new List<HoleItemLittleController>();

    ItemCellController itemCellCtrl;
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
    private void HideOther(ShowType type)
    {
        switch(type)
        {
            case ShowType.One:
                View.LvLbl_UILabel.text = "";
                View.EqNameLbl_1_UILabel.text = "";
                View.typeLbl_UILabel.text = "";
                View.EmbedNumberShow.gameObject.SetActive(false);
                break;
            case ShowType.Two:
                View.EqNameLbl_UILabel.text = "";
                View.EmbedNumberShow.gameObject.SetActive(false);
                break;
            case ShowType.Three:
                View.EmbedNumberShow.gameObject.SetActive(true);
                View.LvLbl_UILabel.text = "";
                View.EqNameLbl_1_UILabel.text = "";
                View.typeLbl_UILabel.text = "";
                View.EqNameLbl_UILabel.text = "";
                break;
        }
    }
    public void UpdateViewData(int index,Equipment equipment, bool isChoice,ShowType type = ShowType.One,int quality = 0)
    {
        this.index = index;

        //UIHelper.SetItemIcon(View.EqIcon_UISprite, equipment.icon);
        if(itemCellCtrl == null)
        {
            itemCellCtrl = AddChild<ItemCellController, ItemCell>(View.IconAnchor,ItemCell.Prefab_BagItemCell);
        }

        itemCellCtrl.UpdateView(equipment);
        View.OnSelectIcon_UISprite.gameObject.SetActive(isChoice);
        var equipmentType = DataCache.getDtoByCls<EquipmentType>(equipment.equipType);

        HideOther(type);
        if (type == ShowType.One)
        {
            
            View.EqNameLbl_UILabel.text = equipment.name;
        }
        else if(type == ShowType.Two)
        {
            View.LvLbl_UILabel.text = equipment.grade+"级";
            View.EqNameLbl_1_UILabel.text = equipment.name;
            View.typeLbl_UILabel.text = equipmentType.name;

            if (quality > 0)
            {
                UIHelper.SetItemQualityIcon(itemCellCtrl.Border, quality);
                View.EqNameLbl_1_UILabel.text = View.EqNameLbl_1_UILabel.text.WrapColor(ItemHelper.GetItemNameColorByRank(quality));
            }
        }
    }


    /// <summary>
    /// 宝石专用
    /// </summary>
    public void UpdateEmbedViewData(int index,EquipmentEmbedCellVo vo,bool isChoice)
    {
        HideOther(ShowType.Three);
        //View.OnSelectIcon_UISprite.gameObject.SetActive(isChoice);
        //View.EqNameLbl_UILabel.text = vo.name;


        if(embedHoleItems.Count == 0)
        {
            for (int i = 0; i < View.EmbedNumberShow.transform.childCount; i++)
            {
                var ctrl = AddController<HoleItemLittleController, HoleItemLittle>(View.EmbedNumberShow.transform.GetChild(i).gameObject);
                embedHoleItems.Add(ctrl);
            }
        }

        embedHoleItems.ForEachI((x, i) =>
        {
            if(vo.EmbedHoleVoList[i].embedid == -1)
                x.SetEmbedHole(false);
            else
                x.SetEmbedHole(true);
        });

    }
}
