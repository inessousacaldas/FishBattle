// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillTipsController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using Assets.Scripts.MyGameScripts.Manager;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class RoleSkillTipsController
{
    private RoleSkillRangeController rangeCtrl;//技能范围
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
        rangeCtrl = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public static RoleSkillTipsController Show(GameObject pParent,RoleSkillCraftsVO vo,Vector3 pos = default(Vector3))
    {
        List<IViewController> mCachedUIControllerList = null;
        List<GameObject> mUIList = null;
        var ctrl = AutoCacherHelper.AddChild<RoleSkillTipsController, RoleSkillTips>(
            pParent
            , RoleSkillTips.NAME
            , ref mUIList
            , ref mCachedUIControllerList);
        ctrl.gameObject.name = RoleSkillTips.NAME;
        ctrl.UpdateView(vo);
        ctrl.View.transform.localPosition = pos;
        UIComponentMgr.AddBgMask(ctrl.View,ctrl.View.transform.parent,true);
        return ctrl;
    }

    public void UpdateView(RoleSkillCraftsVO vo)
    {
        View.lblName_UILabel.text = vo.Name + "\n" + RoleSkillUtils.GetSkillEnumName(Skill.SkillEnum.Crafts);
        View.lblDesc_UILabel.text = RoleSkillDataMgr.DataMgr.MainData.GetCraftsDesc(vo.id);
        UpdateRange(RoleSkillDataMgr.DataMgr.MainData,vo);
    }

    private void UpdateRange(IRoleSkillMainData data,RoleSkillCraftsVO itemVO)
    {
        var scopeVO = DataCache.getDtoByCls<SkillScope>(itemVO.cfgVO.scopeId);
        var type = data.GetScopeTarType(itemVO.cfgVO.scopeId);
        if(rangeCtrl == null)
        {
            rangeCtrl = RoleSkillRangeController.Show(View.Content_Transform.gameObject,scopeVO.scopeIndex,type);
            rangeCtrl.transform.localPosition = new Vector3(268,117);
        }
        else
        {
            rangeCtrl.Show(scopeVO.scopeIndex,type);
        }
    }

}
