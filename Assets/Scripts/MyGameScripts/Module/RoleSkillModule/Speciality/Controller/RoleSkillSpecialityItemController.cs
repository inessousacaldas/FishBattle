// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillSpecialityItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
using UnityEngine;


public partial class RoleSkillSpecialityItemController
{
    private static string[] layerNameList = new string[] {"", "一层专精","二层专精","三层专精"};

    public Speciality.SpecialityLayerEnum curLayer;//当前是哪一层
    public Dictionary<int,RoleSkillSpecialityItemSingleController> itemList = new Dictionary<int, RoleSkillSpecialityItemSingleController>();

    private int height;
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

    public void UpdateView(IRoleSkillSpecialityData data)
    {
        View.lblName_little_UILabel.text = string.Format("（需要前置点数{0}点）", data.GetNeedPoint(curLayer));
        //View.lblName_UILabel.text = layerNameList[(int)curLayer] + string.Format("（需要前置点数{0}点）",data.GetNeedPoint(curLayer));
        View.lblName_UILabel.text = layerNameList[(int)curLayer].ToString();
    }

    public void UpdateItem(IRoleSkillSpecialityData data,RoleSkillSpecialityTempVO dataVO,RoleSkillSpecItemSingleType type)
    {
        RoleSkillSpecialityItemSingleController itemCtrl;
        if(itemList.ContainsKey(dataVO.id) == false)
        {
            itemCtrl = AddChild<RoleSkillSpecialityItemSingleController,RoleSkillSpecialityItemSingle>(View.Content_Transform.gameObject,RoleSkillSpecialityItemSingle.NAME,RoleSkillSpecialityItemSingle.NAME);
            itemCtrl.View.gameObject.name = RoleSkillSpecialityItemSingle.NAME + dataVO.id;
            itemList[dataVO.id] = itemCtrl;
        } else
        {
            itemCtrl = itemList[dataVO.id];
        }
        View.Content_Transform.Reposition();
        itemCtrl.UpdateView(data.CheckCanAdd(curLayer),dataVO,type);
    }

    public void UpdateItemPos()
    {
        RoleSkillSpecialityItemSingleController lastItem = null;
        itemList.ForEach(e =>
        {
            lastItem = e.Value;
        });
        height = (int)lastItem.View.transform.localPosition.y - 93;
    }

    public int Height
    {
        get { return height; }
    }
}
