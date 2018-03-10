﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed class CrewPassiveSkillView : BaseView
{
    public const string NAME ="CrewPassiveSkillView";

    #region Element Bindings

    /// bind gameobject
    public UIButton ForgetBtn_UIButton;
    public Transform SkillIconGroup_Transform;
    public UILabel SkillNameLbl_UILabel;
    public GameObject SelectSkillGroup;
    public UISprite MaterialIcon_UISprite;
    public UIButton UseBtn_UIButton;
    public UIButton UpBtn_UIButton;
    public UISlider ExpSlider_UISlider;
    public UILabel LvLbl_UILabel;
    public UILabel SkillTypeLbl_UILabel;
    public UILabel SkillEffectLbl_UILabel;
    public UILabel PercentLb_UILabel;
    public UIGrid SkillGrid_UIGrid;
    public GameObject ToggleBtnGroup;
    public GameObject SkillDescriptGroup;
    public UILabel ConsumeLbl_UILabel;
    public Transform pool_Transform;
    public UIScrollView SkillScroll_UIScrollView;
    public UIButton EquipBtn_UIButton;
    public UILabel EquipLbl_UILabel;
    public UILabel lblForgetName_UILabel;
    public UILabel lblForgetLevel_UILabel;
    public GameObject ForgetWindow;
    public UIButton btnWindowForget_UIButton;
    public UIButton btnCancel_UIButton;
    public UIButton BlackBG_UIButton;
    public UILabel SkillEffLbl_UILabel;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        ForgetBtn_UIButton = root.FindScript<UIButton>("Content/SkillDescriptGroup/ForgetBtn");
        SkillIconGroup_Transform = root.FindTrans("Content/SkillIconGroup");
        SkillNameLbl_UILabel = root.FindScript<UILabel>("Content/SkillNameLbl");
        SelectSkillGroup = root.FindGameObject("Content/SelectSkillGroup");
        MaterialIcon_UISprite = root.FindScript<UISprite>("Content/SkillDescriptGroup/ConsumeItem/Material/MaterialIcon");
        UseBtn_UIButton = root.FindScript<UIButton>("Content/SkillDescriptGroup/UseBtn");
        UpBtn_UIButton = root.FindScript<UIButton>("Content/SkillDescriptGroup/UpBtn");
        ExpSlider_UISlider = root.FindScript<UISlider>("Content/SkillDescriptGroup/SkillExpSlider/Slider/ExpSlider");
        LvLbl_UILabel = root.FindScript<UILabel>("Content/SkillDescriptGroup/MaterialConsume/LvLbl");
        SkillTypeLbl_UILabel = root.FindScript<UILabel>("Content/SkillDescriptGroup/SkillDescripeGroup/SkillType/SkillTypeLbl");
        SkillEffectLbl_UILabel = root.FindScript<UILabel>("Content/SkillDescriptGroup/SkillDescripeGroup/SkillEffect/SkillEffectLbl");
        PercentLb_UILabel = root.FindScript<UILabel>("Content/SkillDescriptGroup/SkillExpSlider/Slider/PercentLb");
        SkillGrid_UIGrid = root.FindScript<UIGrid>("Content/SelectSkillGroup/SkillScroll/SkillGrid");
        ToggleBtnGroup = root.FindGameObject("Content/SelectSkillGroup/ToggleBtnGroup");
        SkillDescriptGroup = root.FindGameObject("Content/SkillDescriptGroup");
        ConsumeLbl_UILabel = root.FindScript<UILabel>("Content/SkillDescriptGroup/ConsumeItem/Material/ConsumeLbl");
        pool_Transform = root.FindTrans("Content/SelectSkillGroup/pool");
        SkillScroll_UIScrollView = root.FindScript<UIScrollView>("Content/SelectSkillGroup/SkillScroll");
        EquipBtn_UIButton = root.FindScript<UIButton>("Content/SelectSkillGroup/EquipBtn");
        EquipLbl_UILabel = root.FindScript<UILabel>("Content/SelectSkillGroup/EquipBtn/EquipLbl");
        lblForgetName_UILabel = root.FindScript<UILabel>("ForgetWindow/lblForgetName");
        lblForgetLevel_UILabel = root.FindScript<UILabel>("ForgetWindow/lblForgetLevel");
        ForgetWindow = root.FindGameObject("ForgetWindow");
        btnWindowForget_UIButton = root.FindScript<UIButton>("ForgetWindow/btnWindowForget");
        btnCancel_UIButton = root.FindScript<UIButton>("ForgetWindow/btnCancel");
        BlackBG_UIButton = root.FindScript<UIButton>("ForgetWindow/BlackBG");
        SkillEffLbl_UILabel = root.FindScript<UILabel>("Content/SelectSkillGroup/SkillEffect/SkillEffLbl");

    }
    #endregion
}
