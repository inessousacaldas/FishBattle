// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistLearnViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UnityEngine;
using AssetPipeline;

public partial class AssistLearnViewController
{
    TabbtnManager tabbtnMgr;
    public TabbtnManager TabbtnMgr { get { return tabbtnMgr; } }
    private Func<int, ITabBtnController> func;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
                View.TabBtn_UITable.gameObject
                , TabbtnPrefabPath.TabBtnWidget_H1.ToString()
                , "Tabbtn_" + i);

        tabbtnMgr = TabbtnManager.Create(AssistSkillMainDataMgr.AssistSkillMainData._TabInfos, func);
        tabbtnMgr.SetBtnLblFont(20, "2e2e2e", 18, "bdbdbd");
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

    public void UpdateView(IAssistSkillMainData data)
    {
        GameLog.Log_Formation(data.SkillId.ToString());

        int skill = data.SkillId == 0 ? data.ChosedSkillId : data.SkillId;
        switch (skill)
        {
            case (int)AssistSkill.AssistSkillEnum.CarryCook:
                View.Icon_1_UITexture.gameObject.SetActive(true);
                View.Icon_2_UITexture.gameObject.SetActive(false);
                View.Icon_3_UITexture.gameObject.SetActive(false);
                break;
            case (int)AssistSkill.AssistSkillEnum.GrailCook:
                View.Icon_1_UITexture.gameObject.SetActive(false);
                View.Icon_2_UITexture.gameObject.SetActive(true);
                View.Icon_3_UITexture.gameObject.SetActive(false);
                break;
            case (int)AssistSkill.AssistSkillEnum.LeadForceSkill:
                View.Icon_1_UITexture.gameObject.SetActive(false);
                View.Icon_2_UITexture.gameObject.SetActive(false);
                View.Icon_3_UITexture.gameObject.SetActive(true);
                break;
        }

        tabbtnMgr.SetTabBtn(data.CurTab == AssistViewTab.CookUpGradeView || data.CurTab == AssistViewTab.ForceUpGradeView ? 0 : 1);
    }

    public void SetIsShowTab(bool isShow)
    {
        View.TabBtn_UITable.gameObject.SetActive(isShow);
    }

}
