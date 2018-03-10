// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillTalentSingleItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule.Model;
using System;

public partial class RoleSkillTalentSingleItemController
{
    public RoleSkillTalentSingleItemVO vo;
    public bool isCanSelect;
    private int curIndex;

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
    private bool setFirst = false;
    public void UpdateView(RoleSkillTalentSingleItemVO _vo,int idx)
    {
        if (!setFirst)
        {
            //View.lblName_UILabel.effectStyle = UILabel.Effect.None;
            //View.lblName_UILabel.color = new UnityEngine.Color(51f / 255f, 51f / 255f, 51f / 255f);
            View.lblName_UILabel.transform.localPosition = new UnityEngine.Vector3(-0.4f, -33.8f, 0f);
            setFirst = true;
        }
        
        vo = _vo;
        vo.index = idx;
        if(vo != null)
        {
            UIHelper.SetSkillIcon(View.spIcon_UISprite, vo.cfgVO.icon);
            var roleLV = ModelManager.Player.GetPlayerLevel();
            var limitLV = vo.limitLv;
            isCanSelect = roleLV >= limitLV;
            View.spIcon_UISprite.isGrey = !isCanSelect;
            View.lblName_UILabel.text = vo.cfgVO.name;
            View.lblNum_UILabel.text = vo.Grade + "/" + vo.cfgVO.maxGrade;
        }
    }

    public void UpdateReCommendView(Talent cfgVO,bool isTalent = false)
    {
        if (cfgVO == null)
        {
            GameDebuger.Log("请策划查看faction对应的天赋技能id是否存在于Talent表中");
            return;
        }
        UIHelper.SetSkillIcon(View.spIcon_UISprite, cfgVO.icon);
        if(!isTalent)
            View.lblName_UILabel.text = cfgVO.name;
        else
            View.lblName_UILabel.text = "";
        View.lblNum_UILabel.text = "";
    }

    public void SetSelected(bool sel)
    {
        View.spSelected_UISprite.gameObject.SetActive(sel);
    }

    public void UpdateArrow(int nowIdx)
    {
        int rate = vo.index - nowIdx;
        View.arrow_UISprite.height = 44 * rate;
        View.arrow_UISprite.gameObject.SetActive(true);
    }

}
