// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillSpecialityViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UniRx;

public interface IRoleSkillSpecialityViewController
{
    UniRx.IObservable<Unit> OnbtnReset_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnbtnCancel_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnbtnRecommend_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnbtnConform_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnbtnUp_UIButtonClick { get; }
}
public partial class RoleSkillSpecialityViewController
{
    private Dictionary<int,RoleSkillSpecialityItemController> itemList = new Dictionary<int, RoleSkillSpecialityItemController>();
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        CreateItem();
    }

    private void CreateItem()
    {
        for(var i = 1;i<=3;i++)
        {
            var item = AddChild<RoleSkillSpecialityItemController,RoleSkillSpecialityItem>(View.Container_UIWidget.gameObject,RoleSkillSpecialityItem.NAME,RoleSkillSpecialityItem.NAME);
            itemList[i] = item;
        }
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
        data.CheckClearAddPoint(AppDto.Speciality.SpecialityLayerEnum.Second);
        data.CheckClearAddPoint(AppDto.Speciality.SpecialityLayerEnum.Third);
        UpdateItem(data);
        View.lblHas_UILabel.text = data.GetShowPoint().ToString();
        View.lblLevel_UILabel.text = data.GetCurGrade().ToString();
    }

    private void UpdateItem(IRoleSkillSpecialityData data)
    {
        var curType = data.GetCurType();
        View.goEdit_Transform.gameObject.SetActive(curType == RoleSkillSpecItemSingleType.Edit);
        View.goNormal_Transform.gameObject.SetActive(curType == RoleSkillSpecItemSingleType.Normal);
        foreach(var tempVO in data.TempList.Values)
        {
            var itemCtrl = itemList[tempVO.cfgVO.layer];
            itemCtrl.curLayer = (AppDto.Speciality.SpecialityLayerEnum)tempVO.cfgVO.layer;
            itemCtrl.UpdateItem(data,tempVO,curType);
        }

        foreach(var itemCtrl in itemList.Values)
        {
            itemCtrl.UpdateItemPos();
            itemCtrl.UpdateView(data);
        }

        var height = 0; 
        foreach(var itemCtrl in itemList.Values)
        {
            itemCtrl.View.transform.localPosition = new UnityEngine.Vector3(0,height);
            height += itemCtrl.Height;
        }
        //View.Container_UIWidget.height = Math.Abs(height);
    }
}
