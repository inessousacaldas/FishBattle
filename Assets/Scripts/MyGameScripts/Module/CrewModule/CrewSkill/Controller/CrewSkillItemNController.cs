// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewSkillItemNController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using UnityEngine;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule;

public interface ICrewSkillItemNController
{
    UniRx.IObservable<UniRx.Unit> OnbtnUp_UIButtonClick { get; }

    UniRx.IObservable<UniRx.Unit> OnbtnAdd_UIButtonClick { get; }
    
    ICrewSkillVO SkillVO { get; }
    
    PsvItemData PsvItemData { get; }
    CrewSkillItemNController.MagicSkillState SkillState { get; }
}

public partial class CrewSkillItemNController: ICrewSkillItemNController
{

    public enum MagicSkillState
    {
        None,
        Locked,     //还未解锁
        NoEquiped,   //只是开放，并没有装备技能
        Equiped  //装备了技能
    }
    private MagicSkillState magicSkillState = MagicSkillState.None;
    private RoleSkillRangeController rangeCtrl;

    private ICrewSkillVO skillVO;
    // 因为前面技巧数据结构设置的就是取于dto,所以不太好去继承ICreSkillVO
    private CrewPassiveSkill psvDto;
    private PsvItemData psvItemData = new PsvItemData();
    private PassiveState state = PassiveState.Lock;

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
        base.OnDispose();
        magicSkillState = MagicSkillState.None;
        rangeCtrl = null;
        skillVO = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public ICrewSkillVO SkillVO
    {
        get { return skillVO; }
    }
    

    public PsvItemData PsvItemData
    {
        get { return psvItemData; }
    }
    //174181
    //可能所有的技能都会有这些属性
    public void UpdateView(ICrewSkillVO data)
    {
        skillVO = data;
        //UIHelper.SetSkillIcon(View.itemIcon_UISprite, data.Icon);
        UIHelper.SetUITexture(View.itemIcon_UISprite, data.Icon, false);
        View.lblGrade_UILabel.text = data.Grade.ToString();
        View.lblName_UILabel.text = data.Name;
        View.lblTip_UILabel.text = "[272020]技能效果:[-][174181]  " + RoleSkillUtils.Formula(data.SkillDes,skillVO.Grade);
        View.lblType_UILabel.text = "[353535]技能类型：[-][174181]战技[-]";
        //string.Format("{0}", "战技".WrapColor(ColorConstantV3.Color_Red_Str));
        UpdateViewDef(data);
    }

    //更新不同的技能（战技，魔法，技巧）
    public void UpdateViewDef(ICrewSkillVO data)
    {
        CrewSkillCraftsVO craftsVo = data as CrewSkillCraftsVO;
        if (craftsVo != null)
        {
            UpdateCraftsView(craftsVo);
        }
    }

    /// <summary>
    /// 更新战技
    /// </summary>
    /// <param name="craftsVo"></param>
    private void UpdateCraftsView(CrewSkillCraftsVO craftsVo)
    {
        if (craftsVo == null || craftsVo.cfgVO == null) return;
        View.IsSIcon_Transform.gameObject.SetActive(craftsVo.IsSuperCrafts);
        UIHelper.SetSkillQualityIcon(View.bg_UISprite, craftsVo.cfgVO.quality);
        UpdateRightRange(craftsVo.cfgVO.scopeId);
    }

    private void UpdateRightRange(int scopeId)
    {
        CrewSkillCraftsData tmp = CrewSkillHelper.GetCraftsData();
        var scopeVO = tmp.GetScopeByID(scopeId);
        var type = tmp.GetScopeTarType(scopeId);
        if (rangeCtrl == null)
        {
            rangeCtrl = RoleSkillRangeController.Show(View.rangeTrans_Transform.gameObject, scopeVO.scopeIndex, type);
            rangeCtrl.transform.localPosition = new UnityEngine.Vector3(0, -17);
            rangeCtrl.transform.localScale = UnityEngine.Vector3.one;
        }
        else
        {
            rangeCtrl.Show(scopeVO.scopeIndex, type);
        }
    }

    #region 技巧

    //有数据
    public void UpdatePsvView(CrewPassiveSkill data,PassiveSkillDto dto)
    {
        psvItemData.psvVO = data;
        psvItemData.psvDto = dto;
        psvItemData.state = PassiveState.HaveItem;

        View.lblName_UILabel.text = data.name;
        View.lblType_UILabel.text = "[353535]技能类型：[-][174181]技巧[-]";
        View.lblGrade_UILabel.text = dto.grade.ToString();
        View.lblTip_UILabel.text = "[272020]技能效果:[-]  [174181]" + RoleSkillUtils.Formula(data.shortDescription,dto.grade) +"[-]";
        View.Label_UILabel.text = "升 级";
        UIHelper.SetUITexture(View.itemIcon_UISprite, data.icon, false);
        //UIHelper.SetSkillIcon(View.itemIcon_UISprite, data.icon);
        View.addIcon_Transform.gameObject.SetActive(false);
        View.itemIcon_UISprite.gameObject.SetActive(true);
        View.defaltIcon_Trans.gameObject.SetActive(false);
        View.lockIcon_Transform.gameObject.SetActive(false);
        View.btnUp_UIButton.transform.GetComponent<UISprite>().isGrey = false;
        View.btnUp_UIButton.transform.GetComponent<BoxCollider>().enabled = true;

    }

    public void UpdatePsvView(int unLockNum,int idx)
    {
        View.itemIcon_UISprite.gameObject.SetActive(false);
        View.defaltIcon_Trans.gameObject.SetActive(true);
        if (unLockNum > idx)
        {
            //格子解锁
            psvItemData.state = PassiveState.NeedItem;

            View.lblName_UILabel.text = "";
            View.lblTip_UILabel.text = "";
            View.lblType_UILabel.text = "";
            View.lblGrade_UILabel.text = "";
            View.Label_UILabel.text = "学 习";
            View.addIcon_Transform.gameObject.SetActive(true);
            View.lockIcon_Transform.gameObject.SetActive(false);
            View.btnUp_UIButton.transform.GetComponent<UISprite>().isGrey = false;
            View.btnUp_UIButton.transform.GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            //格子未解锁
            psvItemData.state = PassiveState.Lock;

            View.lblName_UILabel.text = LockDefine(idx);
            View.lblTip_UILabel.text = "此技能未解锁";
            View.lblType_UILabel.text = "";
            View.lblGrade_UILabel.text = "";
            View.Label_UILabel.text = "未解锁";
            View.addIcon_Transform.gameObject.SetActive(false);
            View.lockIcon_Transform.gameObject.SetActive(true);
            View.btnUp_UIButton.transform.GetComponent<UISprite>().isGrey = true;
            View.btnUp_UIButton.transform.GetComponent<BoxCollider>().enabled = false;
        }
    }

    private string LockDefine(int i)
    {
        switch (i)
        {
            case 0:
                return "蓝色品质解锁";
            case 1:
                return "紫色品质解锁";
            case 2:
                return "橙色品质解锁";
            case 3:
                return "红色品质解锁";
        }
        return "";
    }

    #endregion

    #region 魔法

    public void UpdateMagicView(bool isOpen,int openLv,CrewSkillMagicVO magicVO = null)
    {
        skillVO = magicVO;
        View.lockIcon_Transform.gameObject.SetActive(!isOpen);
        View.addIcon_Transform.gameObject.SetActive(magicVO == null && isOpen);
        View.defaltIcon_Trans.gameObject.SetActive(magicVO == null);
        View.rangeTrans_Transform.gameObject.SetActive(magicVO != null);
        if (magicVO != null)
        {
            UIHelper.SetUITexture(View.itemIcon_UISprite, magicVO.magicVO.icon,false);
            //UIHelper.SetSkillIcon(View.itemIcon_UISprite, magicVO.magicVO.icon);
            UpdateRightRange(magicVO.magicVO.scopeId);
        }
        else
        {
            View.itemIcon_UISprite.mainTexture = null;
        }
        magicSkillState = isOpen ?
                                       magicVO == null ? MagicSkillState.NoEquiped : MagicSkillState.Equiped
                                       : MagicSkillState.Locked;
        View.Label_UILabel.text = isOpen ?
                                            magicVO == null ? "导力器" : "详情"
                                            : "未解锁";
        View.lblType_UILabel.text = magicVO == null ? "" : "魔法";

        View.lblName_UILabel.text = isOpen ?
                                            magicVO == null ? "" : magicVO.magicVO.name
                                            : openLv + "级解锁";
        View.lblTip_UILabel.text = isOpen ?
                                            magicVO == null ? "快去导力器系统配置魔法技能吧!" : RoleSkillUtils.Formula(magicVO.magicVO.shortDescription,magicVO.Grade)
                                            : "伙伴的这个技能格还没有解锁哦。";
        View.btnUp_UIButton.transform.GetComponent<UISprite>().isGrey = magicSkillState == MagicSkillState.Locked;
        View.btnUp_UIButton.transform.GetComponent<BoxCollider>().enabled = magicSkillState != MagicSkillState.Locked;
    }

    public MagicSkillState SkillState { get { return magicSkillState; } }
    #endregion

}
