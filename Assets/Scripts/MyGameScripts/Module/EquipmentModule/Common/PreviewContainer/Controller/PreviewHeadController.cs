// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PreviewHeadController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;

public partial class PreviewHeadController
{

    public enum HeadLabel
    {
        None,//无标签
        Equip,//已经装备
        Recommend,//推荐
    }
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

    private void HideALLWiget()
    {
        View.EquipmentLvlbl_UILabel.gameObject.SetActive(false);
        View.EquipmentNamelbl_UILabel.gameObject.SetActive(false);
        View.EquipmentPower_UILabel.gameObject.SetActive(false);
        View.EquipmentTypelbl_UILabel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 打造物品后的装备展示显示战力
    /// </summary>
    /// <param name="data"></param>
    /// <param name="powerChange"></param>
    /// <param name="label"></param>
    public void MakeEquipmentSmithShow(EquipmentDto data, int powerChange = 0,HeadLabel label = HeadLabel.None)
    {
        View.EquipmentPower_UILabel.gameObject.SetActive(true);

        View.EquipmentNamelbl_UILabel.text = data.equip.name;
        View.EquipmentPower_UILabel.text = "战斗力 :" + data.property.power;
        var equipmentInfo = data.equip as Equipment;
        View.EquipmentLvlbl_UILabel.text = equipmentInfo.grade +"级";
        var eq_type = DataCache.getDtoByCls<EquipmentType>(equipmentInfo.equipType);
        View.EquipmentTypelbl_UILabel.text = eq_type.name;

        var headIconCtrl = AddController<EquipmentItemCellController, EquipmentItemCell>(View.EquipmentItemCell);
        headIconCtrl.UpdateViewData(data.equip as Equipment);

        //TODO:推荐，上升下降标示
        ShowPowerChange(powerChange);
        //Todo:根据label加载标签
        ShowHeadLabel(label);
    }

    /// <summary>
    /// 打造装备前的装备预览
    /// </summary>
    /// <param name="eq"></param>
    public void MakePreviewEquipment(Equipment eq)
    {
        View.EquipmentPower_UILabel.gameObject.SetActive(false);

        View.EquipmentNamelbl_UILabel.text = eq.name;
        View.EquipmentLvlbl_UILabel.text = eq.grade + "级";
        var eq_type = DataCache.getDtoByCls<EquipmentType>(eq.equipType);
        View.EquipmentTypelbl_UILabel.text = eq_type.name;
        var headIconCtrl = AddController<EquipmentItemCellController, EquipmentItemCell>(View.EquipmentItemCell);
        headIconCtrl.UpdateViewData(eq);
    }
    //根据战斗力的浮动显示变化
    private void ShowPowerChange(int powerChange)
    {
        //GameDebuger.Log("Todo:推荐，上升下降标示");
        if (powerChange > 0)
        {

        }
        else if (powerChange < 0)
        {

        }
    }

    /// <summary>
    /// 展示右上角的标签
    /// </summary>
    private void ShowHeadLabel(HeadLabel label)
    {
        GameDebuger.Log("Todo:根据label加载标签");
    }
}
