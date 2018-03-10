// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillTalentItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using Assets.Scripts.MyGameScripts.Module.RoleSkillModule.Model;
using Assets.Scripts.MyGameScripts.UI;
using System;
using System.Collections.Generic;

public partial class RoleSkillTalentItemController
{
    public RoleSkillTalentItemVO curVO;
    private List<RoleSkillTalentSingleItemController> listSingle = new List<RoleSkillTalentSingleItemController>();
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
    }

    private void CreateItem(RoleSkillTalentItemVO vo,Action<int,RoleSkillTalentSingleItemController> AddToDic)
    {
        if(listSingle.Count > 0) return;
        var list = vo.singleList;
        int max = list.Count;
        for(var i = 0;i < max; i++)
        {
            var itemCtrl = AddChild<RoleSkillTalentSingleItemController,RoleSkillTalentSingleItem>(View.gameObject,RoleSkillTalentSingleItem.NAME,RoleSkillTalentSingleItem.NAME);
            AddToDic.Invoke(list[i].id, itemCtrl);
            listSingle.Add(itemCtrl);
            int idx = list[i].cfgVO.postIndex - 1;
            itemCtrl.View.transform.localPosition = new UnityEngine.Vector3(-61 + 101 * idx, 11);
            EventUtil.AddClick(itemCtrl.View.gameObject,RoleSkillDataMgr.RoleSkillTalentViewLogic.OnSelectItem,itemCtrl);
        }
    }

    // 客户端自定义事件chm
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

    public void UpdateView(RoleSkillTalentItemVO vo,Action<int, RoleSkillTalentSingleItemController> AddToDic,int idx)
    {
        curVO = vo;
        CreateItem(vo,AddToDic);
        for(var i = 0;i < listSingle.Count;i++)
        {
            listSingle[i].UpdateView(vo.singleList[i],idx);
        }
        View.lblLevel_UILabel.text = vo.level + "级";
    }

    public void UpdatePreArrow()
    {

    }
}
