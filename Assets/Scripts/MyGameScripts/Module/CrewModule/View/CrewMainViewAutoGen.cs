﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed class CrewMainView : BaseView
{
    public const string NAME ="CrewMainView";

    #region Element Bindings

    /// bind gameobject
    public UIButton CloseBtn_UIButton;
    public GameObject PartnerListGroup;
    public UIButton ShowTypeBtn_UIButton;
    public UIPageGrid TiledIconGrid_UIPageGrid;
    public UIButton PullBtn_UIButton;
    public UIButton TypeBtn_UIButton;
    public UIScrollView TiledScrollView_UIScrollView;
    public GameObject ExtendPanel;
    public GameObject PartnerModelGroup;
    public GameObject ModelAnchor;
    public UILabel NameLb_UILabel;
    public UILabel StrengthLevelLbl;
    public GameObject Score;
    public UILabel AttLb_UILabel;
    public UILabel FavorableLb_UILabel;
    public GameObject HadPartner;
    public GameObject NoHadPartner;
    public UIButton FormationBtn_UIButton;
    public UIButton FollowBtn_UIButton;
    public UISlider ExpSlider_UISlider;
    public UILabel ExpLb_UILabel;
    public UILabel LvLb_UILabel;
    public UIButton AddExpBtn_UIButton;
    public UISlider OtherExpSlider_UISlider;
    public UILabel OtherExpLb_UILabel;
    public UISprite MagicSprite_UISprite;
    public UIButton OtherAddExpBtn_UIButton;
    public UIButton PullbackBtn_UIButton;
    public UITable TabGroup_UITable;
    public GameObject PartnerInfoGroup;
    public GameObject PartnerSkillGroup;
    public GameObject PartnerCultivateGroup;
    public GameObject PartnerFetterGroup;
    public UIScrollView FetterScrollView;
    public UITable FetterGrid;
    public GameObject PartnerFavorableGroup;
    public UIButton ChangeNameBtn_UIButton;
    public UIButton CrewAddBtn_UIButton;
    public GameObject Slider;
    public UISprite OtherIcon_UISprite;
    public Transform CrewInfoContent_Transform;
    public UIPageGrid PageGrid;
    public PageScrollView CrewInfoContent_PageScrollView;
    public UIButton pageSprite_UIButton;
    public UIButton pageSprite_1_UIButton;
    public UILabel TypeBtnLabel_UILabel;
    public Transform ListBtnGroup_Transform;
    public UIPanel BtnGroup_UIPanel;
    public UILabel InfoTabLb_UILabel;
    public UILabel FetterTabLb_UILabel;
    public Transform BtnGroup_Transform;
    public UIButton ExtendCloseBtn_UIButton;
    public UIButton PageInfoBtn_UIButton;
    public UIGrid TiledIconGrid_UIGrid;
    public UIButton FavorableBtn_UIButton;
    public UIButton CrewNameBtn_UIButton;
    public UIButton CrewTalkBtn_UIButton;
    public UIButton CrewVoiceBtn_UIButton;
    public UIButton MoreOperationBtn_UIButton;
    public GameObject HideBtnList;
    public GameObject BackGround;
    public UISprite FactionSprite_UISprite;
    public UIGrid PartnerGrid_UIGrid;
    public UIScrollView IconScrollView_UIScrollView;
    public UIPanel FilterBtnGroup_UIPanel;
    public Transform FilterBtnGroup_Transform;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        CloseBtn_UIButton = root.FindScript<UIButton>("BaseMainWindow/CloseBtn");
        PartnerListGroup = root.FindGameObject("MainContent/PartnerListGroup");
        ShowTypeBtn_UIButton = root.FindScript<UIButton>("MainContent/PartnerListGroup/ShowTypeBtn");
        TiledIconGrid_UIPageGrid = root.FindScript<UIPageGrid>("MainContent/ExtendPanel/TiledScrollView/TiledIconGrid");
        PullBtn_UIButton = root.FindScript<UIButton>("MainContent/PartnerListGroup/PullBtn");
        TypeBtn_UIButton = root.FindScript<UIButton>("MainContent/ExtendPanel/TypeBtn");
        TiledScrollView_UIScrollView = root.FindScript<UIScrollView>("MainContent/ExtendPanel/TiledScrollView");
        ExtendPanel = root.FindGameObject("MainContent/ExtendPanel");
        PartnerModelGroup = root.FindGameObject("MainContent/PartnerModelGroup");
        ModelAnchor = root.FindGameObject("MainContent/PartnerModelGroup/ModelAnchor");
        NameLb_UILabel = root.FindScript<UILabel>("MainContent/PartnerModelGroup/NameLb");
        StrengthLevelLbl = root.FindScript<UILabel>("MainContent/PartnerModelGroup/NameLb/StrengthLevelLbl");
        Score = root.FindGameObject("MainContent/PartnerModelGroup/Score");
        AttLb_UILabel = root.FindScript<UILabel>("MainContent/PartnerModelGroup/Score/AttLb");
        FavorableLb_UILabel = root.FindScript<UILabel>("MainContent/PartnerModelGroup/PopupBtnList/FavorableBtn/FavorableLb");
        HadPartner = root.FindGameObject("MainContent/PartnerModelGroup/HadPartner");
        NoHadPartner = root.FindGameObject("MainContent/PartnerModelGroup/NoHadPartner");
        FormationBtn_UIButton = root.FindScript<UIButton>("MainContent/PartnerListGroup/FormationBtn");
        FollowBtn_UIButton = root.FindScript<UIButton>("MainContent/PartnerModelGroup/HadPartner/FollowBtn");
        ExpSlider_UISlider = root.FindScript<UISlider>("MainContent/PartnerModelGroup/HadPartner/Slider/ExpSlider");
        ExpLb_UILabel = root.FindScript<UILabel>("MainContent/PartnerModelGroup/HadPartner/Slider/ExpLb");
        LvLb_UILabel = root.FindScript<UILabel>("MainContent/PartnerModelGroup/HadPartner/Slider/LvLb");
        AddExpBtn_UIButton = root.FindScript<UIButton>("MainContent/PartnerModelGroup/HadPartner/Slider/AddExpBtn");
        OtherExpSlider_UISlider = root.FindScript<UISlider>("MainContent/PartnerModelGroup/NoHadPartner/Slider/OtherExpSlider");
        OtherExpLb_UILabel = root.FindScript<UILabel>("MainContent/PartnerModelGroup/NoHadPartner/Slider/OtherExpLb");
        MagicSprite_UISprite = root.FindScript<UISprite>("MainContent/PartnerModelGroup/NameLb/MagicSprite");
        OtherAddExpBtn_UIButton = root.FindScript<UIButton>("MainContent/PartnerModelGroup/NoHadPartner/Slider/OtherAddExpBtn");
        PullbackBtn_UIButton = root.FindScript<UIButton>("MainContent/ExtendPanel/PullbackBtn");
        TabGroup_UITable = root.FindScript<UITable>("BaseMainWindow/TabGroup");
        PartnerInfoGroup = root.FindGameObject("MainContent/PartnerInfoGroup");
        PartnerSkillGroup = root.FindGameObject("MainContent/PartnerSkillGroup");
        PartnerCultivateGroup = root.FindGameObject("MainContent/PartnerCultivateGroup");
        PartnerFetterGroup = root.FindGameObject("MainContent/PartnerFetterGroup");
        FetterScrollView = root.FindScript<UIScrollView>("MainContent/PartnerFetterGroup/FetterScrollView");
        FetterGrid = root.FindScript<UITable>("MainContent/PartnerFetterGroup/FetterScrollView/FetterGrid");
        PartnerFavorableGroup = root.FindGameObject("MainContent/PartnerFavorableGroup");
        ChangeNameBtn_UIButton = root.FindScript<UIButton>("MainContent/PartnerModelGroup/NameLb/ChangeNameBtn");
        CrewAddBtn_UIButton = root.FindScript<UIButton>("MainContent/PartnerModelGroup/NoHadPartner/CrewAddBtn");
        Slider = root.FindGameObject("MainContent/PartnerModelGroup/NoHadPartner/Slider");
        OtherIcon_UISprite = root.FindScript<UISprite>("MainContent/PartnerModelGroup/NoHadPartner/Slider/OtherIcon");
        CrewInfoContent_Transform = root.FindTrans("MainContent/PartnerInfoGroup/CrewInfoGroup/CrewInfoScrollView/CrewInfoContent");
        PageGrid = root.FindScript<UIPageGrid>("MainContent/PartnerInfoGroup/CrewInfoGroup/CrewInfoScrollView/CrewInfoContent");
        CrewInfoContent_PageScrollView = root.FindScript<PageScrollView>("MainContent/PartnerInfoGroup/CrewInfoGroup/CrewInfoScrollView/CrewInfoContent");
        pageSprite_UIButton = root.FindScript<UIButton>("MainContent/PartnerInfoGroup/CrewInfoGroup/pageFlagGrid/pageSprite");
        pageSprite_1_UIButton = root.FindScript<UIButton>("MainContent/PartnerInfoGroup/CrewInfoGroup/pageFlagGrid/pageSprite_1");
        TypeBtnLabel_UILabel = root.FindScript<UILabel>("MainContent/ExtendPanel/TypeBtn/TypeBtnLabel");
        ListBtnGroup_Transform = root.FindTrans("MainContent/PartnerListGroup/ShowTypeBtn/ListBtnGroup");
        BtnGroup_UIPanel = root.FindScript<UIPanel>("MainContent/ExtendPanel/BtnGroup");
        InfoTabLb_UILabel = root.FindScript<UILabel>("MainContent/PartnerInfoGroup/CrewInfoGroup/pageFlagGrid/pageSprite/InfoTabLb");
        FetterTabLb_UILabel = root.FindScript<UILabel>("MainContent/PartnerInfoGroup/CrewInfoGroup/pageFlagGrid/pageSprite_1/FetterTabLb");
        BtnGroup_Transform = root.FindTrans("MainContent/ExtendPanel/BtnGroup");
        ExtendCloseBtn_UIButton = root.FindScript<UIButton>("MainContent/ExtendPanel/ExtendCloseBtn");
        PageInfoBtn_UIButton = root.FindScript<UIButton>("MainContent/PartnerInfoGroup/CrewInfoGroup/PageInfoBtn");
        TiledIconGrid_UIGrid = root.FindScript<UIGrid>("MainContent/ExtendPanel/TiledScrollView/TiledIconGrid");
        FavorableBtn_UIButton = root.FindScript<UIButton>("MainContent/PartnerModelGroup/PopupBtnList/FavorableBtn");
        CrewNameBtn_UIButton = root.FindScript<UIButton>("MainContent/PartnerModelGroup/PopupBtnList/HideBtnList/CrewNameBtn");
        CrewTalkBtn_UIButton = root.FindScript<UIButton>("MainContent/PartnerModelGroup/PopupBtnList/HideBtnList/CrewTalkBtn");
        CrewVoiceBtn_UIButton = root.FindScript<UIButton>("MainContent/PartnerModelGroup/PopupBtnList/HideBtnList/CrewVoiceBtn");
        MoreOperationBtn_UIButton = root.FindScript<UIButton>("MainContent/PartnerModelGroup/PopupBtnList/MoreOperationBtn");
        HideBtnList = root.FindGameObject("MainContent/PartnerModelGroup/PopupBtnList/HideBtnList");
        BackGround = root.FindGameObject("MainContent/BackGround");
        FactionSprite_UISprite = root.FindScript<UISprite>("MainContent/PartnerModelGroup/NameLb/FactionSprite");
        PartnerGrid_UIGrid = root.FindScript<UIGrid>("MainContent/PartnerListGroup/IconScrollView/PartnerGrid");
        IconScrollView_UIScrollView = root.FindScript<UIScrollView>("MainContent/PartnerListGroup/IconScrollView");
        FilterBtnGroup_UIPanel = root.FindScript<UIPanel>("MainContent/PartnerListGroup/FilterBtnGroup");
        FilterBtnGroup_Transform = root.FindTrans("MainContent/PartnerListGroup/FilterBtnGroup");

    }
    #endregion
}
