// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillTalentGradeViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using Assets.Scripts.MyGameScripts.Manager;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule.Model;
using Assets.Scripts.MyGameScripts.UI;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule;
using System;

public partial class RoleSkillTalentGradeViewController
{
    private RoleSkillTalentSingleItemController itemCtrl;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        itemCtrl = AddChild<RoleSkillTalentSingleItemController,RoleSkillTalentSingleItem>(View.gameObject,RoleSkillTalentSingleItem.NAME,RoleSkillTalentSingleItem.NAME);
        UIComponentMgr.ResetDepth(itemCtrl.gameObject,15);
        itemCtrl.transform.localPosition = new UnityEngine.Vector3(36,-36);
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

    public void UpdateView(RoleSkillTalentSingleItemVO itemVO)
    {
        itemCtrl.UpdateReCommendView(itemVO.cfgVO,true);
        View.lblName_UILabel.text = itemVO.Name;
        View.lblType_UILabel.text = itemVO.cfgVO.talentType;
        int lv = 0;
        if (itemVO.gradeDto == null)
            lv = 0;
        else
            lv = itemVO.gradeDto.grade;
        View.lblDesc_UILabel.text = RoleSkillUtils.Formula(itemVO.cfgVO.description, lv);
        var curTalent = ModelManager.Player.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.TALENTPOINT);
        View.lblPoint_UILabel.text = string.Format("剩余天赋点数：{0}\n{1}/{2}",HtmlUtil.Font2(curTalent.ToString(),GameColor.MONEY_0),itemVO.Grade,itemVO.cfgVO.maxGrade);
        View.btnUp_UIButton.isEnabled = itemVO.Grade < itemVO.cfgVO.maxGrade;
        View.btnUp_UIButton.GetComponentInChildren<UILabel>().text = itemVO.Grade < itemVO.cfgVO.maxGrade ? "升级(需要" + itemVO.cfgVO.needPoint + "点)" : "升级";
    }

}
