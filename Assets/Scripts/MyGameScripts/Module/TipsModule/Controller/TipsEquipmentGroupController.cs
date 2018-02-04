// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TipsEquipmentGroupController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
public partial class TipsEquipmentGroupController
{
    //Dictionary<Equipment.PartType, TipsEquipmentGroupAttrItemController> attrCtrlsDic = new Dictionary<Equipment.PartType, TipsEquipmentGroupAttrItemController>();
    List<TipsEquipmentGroupAttrItemController> attrCtrls = new List<TipsEquipmentGroupAttrItemController>();
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        
        for (int i=0;i<6;i++)
        {
            
            var go = View.GroupContent_UIGrid.transform.GetChild(i).gameObject;
            var ctrl = AddController<TipsEquipmentGroupAttrItemController, TipsEquipmentGroupAttrItem>(go);
            attrCtrls.Add(ctrl);
        }
        attrCtrls[0].InitData(Equipment.PartType.Weapon);
        attrCtrls[1].InitData(Equipment.PartType.Clothes);
        attrCtrls[2].InitData(Equipment.PartType.Glove);
        attrCtrls[3].InitData(Equipment.PartType.Shoe);
        attrCtrls[4].InitData(Equipment.PartType.AccOne);
        attrCtrls[5].InitData(Equipment.PartType.AccTwo);
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

    public void UpdateView(int groupId)
    {
        var groupConfig = DataCache.getDtoByCls<EquipmentGroup>(groupId);
        var curGroup = EquipmentMainDataMgr.DataMgr.GetEquipmentGroup(groupId);
        //固定6条
        View.GroupTitle_UILabel.text = string.Format("【{0}套装】({1}/{2})", groupConfig.name,curGroup.Count,6);
        attrCtrls.ForEachI((x, i) => {
            bool isContain = curGroup.Find(g => g == attrCtrls[i].part) !=  Equipment.PartType.Unknown;
            attrCtrls[i].UpdateView(groupConfig.name,isContain);
        });
       
        View.Des_1_UILabel.text = string.Format("2件套:{0}", ParseEquipmentGroupFun(groupConfig.function[0]));
        View.Des_2_UILabel.text = string.Format("4件套:{0}", ParseEquipmentGroupFun(groupConfig.function[1]));
        View.Des_3_UILabel.text = string.Format("6件套:{0}", ParseEquipmentGroupFun(groupConfig.function[2]));

        var greyColor = "A8A8B0FF";
        var normalColor = "68c4ff";
        if (curGroup.Count >= 2)
            View.Des_1_UILabel.text = View.Des_1_UILabel.text.WrapColor(normalColor);
        else
            View.Des_1_UILabel.text = View.Des_1_UILabel.text.WrapColor(greyColor);
        if (curGroup.Count >= 4)
            View.Des_2_UILabel.text = View.Des_2_UILabel.text.WrapColor(normalColor);
        else
            View.Des_2_UILabel.text = View.Des_2_UILabel.text.WrapColor(greyColor);
        if (curGroup.Count >= 6)
            View.Des_3_UILabel.text = View.Des_3_UILabel.text.WrapColor(normalColor);
        else
            View.Des_3_UILabel.text = View.Des_3_UILabel.text.WrapColor(greyColor);
        View.GroupContent_UIGrid.Reposition();
        View.GroupDes_UITable.Reposition();
    }

    private string ParseEquipmentGroupFun(string str)
    {
        var desRawStr = str.Split(':');
        var desType = StringHelper.ToInt(desRawStr[0]);
        var id = StringHelper.ToInt(desRawStr[1]);
        var value = 0.0f;
        var Des = "";
        if (desRawStr.Length > 2)
        {
            value = float.Parse(desRawStr[2]);
        }

        if (desType == 1)
        {
            var cab = DataCache.getDtoByCls<CharacterAbility>(id);
            Des = string.Format("{0}+{1}", cab.name, value);
        }
        else if (desType == 2)
        {
            var skill = DataCache.getDtoByCls<AssistSkill>(id);
            Des = skill.description;
        }
        return Des;
    }
}
