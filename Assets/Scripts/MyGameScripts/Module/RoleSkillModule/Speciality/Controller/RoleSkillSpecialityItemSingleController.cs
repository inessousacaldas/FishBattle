// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillSpecialityItemSingleController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using Assets.Scripts.MyGameScripts.UI;
using System;

public partial class RoleSkillSpecialityItemSingleController
{
    public RoleSkillSpecialityTempVO vo;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        RoleSkillDataMgr.RoleSkillSpecialityViewLogic.InitItemSingleReactiveEvents(this);
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(bool checkCanAdd,RoleSkillSpecialityTempVO _vo,RoleSkillSpecItemSingleType type)
    {
        vo = _vo;
        if(vo.gradeDto != null)
        {
            View.lblName_UILabel.text = vo.cfgVO.name;
            View.lblEff_UILabel.text = string.Format("{0}",HtmlUtil.Font2(vo.ShowEffPer,vo.addGrade >0 ? GameColor.GREEN : GameColor.BLACK));

            var gradeStr = HtmlUtil.Font2(vo.ShowGrade,vo.addGrade >0 ? GameColor.GREEN : GameColor.WHITE);
            View.lblEditNum_UILabel.text = gradeStr;
            View.lblNormalNum_UILabel.text = gradeStr;
        }
        View.goEdit_Transform.gameObject.SetActive(type == RoleSkillSpecItemSingleType.Edit);
        View.goNormal_Transform.gameObject.SetActive(type == RoleSkillSpecItemSingleType.Normal);
        View.btnAdd_UIButton.isEnabled = checkCanAdd;
        View.btnSub_UIButton.isEnabled = checkCanAdd;
        View.btnInput_UIButton.isEnabled = checkCanAdd;
    }

    public int Width
    {
        get { return View.RoleSkillSpecialityItemSingle_UIWidget.width; }
    }

    public int Height
    {
        get { return View.RoleSkillSpecialityItemSingle_UIWidget.height; }
    }
    

}
