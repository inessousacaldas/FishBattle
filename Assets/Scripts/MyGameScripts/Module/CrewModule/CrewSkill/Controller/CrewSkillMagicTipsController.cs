// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewSkillMagicTipsController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using Assets.Scripts.MyGameScripts.Module.RoleSkillModule;

public partial class CrewSkillMagicTipsController
{

    private RoleSkillRangeController rangeCtrl;
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

    public void UpdateView(ICrewSkillVO data)
    {
        CrewSkillMagicVO magic = data as CrewSkillMagicVO;
        if (magic != null)
        {
            //   UIHelper.SetSkillIcon(View.itemIcon_UISprite, magic.Icon);
            UIHelper.SetUITexture(View.itemIcon_UISprite, magic.Icon, false);
            View.lblGrade_UILabel.text = magic.Grade.ToString();
            View.lblName_UILabel.text = magic.Name;
            View.lblType_UILabel.text = "魔法";
            View.lblAfter_UILabel.text = magic.SkillTimeAfter;
            View.lblBefor_UILabel.text = magic.SkillTimeBefore;
            View.lblCp_UILabel.text = magic.magicVO.consume + "CP";
            View.lblType2_UILabel.text = "[ff0000]" + magic.SkillType + "[-]";
            View.lblScope_UILabel.text = magic.Scope;
            View.lblEff_UILabel.text = "[272020]技能效果: [-][174181]" + RoleSkillUtils.Formula(magic.SkillDes,magic.Grade) + "[-]";
            UpdateRightRange(magic);
        }
    }

    private void UpdateRightRange(CrewSkillMagicVO magicVo)
    {
        CrewSkillCraftsData tmp = CrewSkillHelper.GetCraftsData();
        var scopeVO = tmp.GetScopeByID(magicVo.magicVO.scopeId);
        var type = tmp.GetScopeTarType(magicVo.magicVO.scopeId);
        if (rangeCtrl == null)
        {
            rangeCtrl = RoleSkillRangeController.Show(View.rangeTrans_Transform.gameObject, scopeVO.scopeIndex, type);
            rangeCtrl.transform.localPosition = new UnityEngine.Vector3(0, 0);
            rangeCtrl.transform.localScale = UnityEngine.Vector3.one;
        }
        else
        {
            rangeCtrl.Show(scopeVO.scopeIndex, type);
        }
    }

}
