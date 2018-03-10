﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed class BracerMainView : BaseView
{
    public const string NAME ="BracerMainView";

    #region Element Bindings

    /// bind gameobject
    public UISprite Icon_UISprite;
    public UILabel LvName_UILabel;
    public UISprite GradeIcon_UISprite;
    public UILabel ExpLabel_UILabel;
    public UISlider Slider_UISlider;
    public UIButton RewardBtn_UIButton;
    public UIButton NPC_UIButton;
    public UISprite Bubble_UISprite;
    public UILabel Talk_UILabel;
    public UIButton Btn_UIButton;
    public UIScrollView ScrollView_UIScrollView;
    public UIGrid Grid_UIGrid;
    public UIButton CloseBtn_UIButton;
    public UIButton TipsBtn_UIButton;
    public UIButton LvViewBtn_UUIButton;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        Icon_UISprite = root.FindScript<UISprite>("PlayerInfoBg/Icon");
        LvName_UILabel = root.FindScript<UILabel>("PlayerInfoBg/LvName");
        GradeIcon_UISprite = root.FindScript<UISprite>("PlayerInfoBg/GradeIcon");
        ExpLabel_UILabel = root.FindScript<UILabel>("PlayerInfoBg/ExpSlider/ExpLabel");
        Slider_UISlider = root.FindScript<UISlider>("PlayerInfoBg/ExpSlider/Slider");
        RewardBtn_UIButton = root.FindScript<UIButton>("PlayerInfoBg/RewardBtn");
        NPC_UIButton = root.FindScript<UIButton>("NPCPanel/NPC");
        Bubble_UISprite = root.FindScript<UISprite>("NPCPanel/Bubble");
        Talk_UILabel = root.FindScript<UILabel>("NPCPanel/Bubble/Talk");
        Btn_UIButton = root.FindScript<UIButton>("NPCPanel/Bubble/Btn");
        ScrollView_UIScrollView = root.FindScript<UIScrollView>("MissionPanel/ScrollView");
        Grid_UIGrid = root.FindScript<UIGrid>("MissionPanel/ScrollView/Grid");
        CloseBtn_UIButton = root.FindScript<UIButton>("CloseBtn");
        TipsBtn_UIButton = root.FindScript<UIButton>("PlayerInfoBg/TipsBtn");
        LvViewBtn_UUIButton = root.FindScript<UIButton>("LvViewBtn");

    }
    #endregion
}
