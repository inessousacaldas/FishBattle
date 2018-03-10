﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed class AssistDelegateMainView : BaseView
{
    public const string NAME ="AssistDelegateMainView";

    #region Element Bindings

    /// bind gameobject
    public UIGrid MissionGrid_UIGrid;
    public UIButton UpdateMissionBtn_UIButton;
    public UIGrid RewardGrid_UIGrid;
    public UILabel LeftTime_UILabel;
    public UILabel MissionDesLabel_UILabel;
    public UILabel DemandDesLabel_UILabel;
    public UILabel ProNum_1_UILabel;
    public UILabel ProNum_2_UILabel;
    public UILabel ProNum_3_UILabel;
    public UIButton CrewItem_1_UIButton;
    public UISprite Add_UISprite_1;
    public UIButton CrewItem_2_UIButton;
    public UISprite Add_UISprite_2;
    public UIButton CrewItem_3_UIButton;
    public UISprite Add_UISprite_3;
    public UIButton CrewItem_4_UIButton;
    public UISprite Add_UISprite_4;
    public UIButton CrewItem_5_UIButton;
    public UISprite FriendIcon_UISprite;
    public UISprite FriendLabel_UILabel;
    public Transform CancelOrFast_Transform;
    public UIButton CancelBtn_UIButton;
    public UIButton FastBtn_UIButton;
    public UIButton AcceptBtn_UIButton;
    public UIButton GetRewardBtn_UIButton;
    public UIButton TipsBtn_UIButton;
    public UILabel MissionProgressLabel_UILabel;
    public UITexture Npc_UITexture;
    public GameObject NoMissionTips;
    public Transform MissionTra;
    public Transform DetailBg;
    public GameObject NumBg;
    
    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        MissionGrid_UIGrid = root.FindScript<UIGrid>("Mission/MissionGrid");
        UpdateMissionBtn_UIButton = root.FindScript<UIButton>("Mission/UpdateMissionBtn");
        RewardGrid_UIGrid = root.FindScript<UIGrid>("DetailBg/Rewrad/RewardGrid");
        LeftTime_UILabel = root.FindScript<UILabel>("DetailBg/Rewrad/LeftTime");
        MissionDesLabel_UILabel = root.FindScript<UILabel>("DetailBg/Middle/MissionDesLabel");
        DemandDesLabel_UILabel = root.FindScript<UILabel>("DetailBg/Demand/DemandDesLabel");
        ProNum_1_UILabel = root.FindScript<UILabel>("DetailBg/Demand/Probability_1/ProNum_1");
        ProNum_2_UILabel = root.FindScript<UILabel>("DetailBg/Demand/Probability_2/ProNum_2");
        ProNum_3_UILabel = root.FindScript<UILabel>("DetailBg/Demand/Probability_3/ProNum_3");
        CrewItem_1_UIButton = root.FindScript<UIButton>("DetailBg/Demand/CrewGrid/CrewItem_1");
        Add_UISprite_1 = root.FindScript<UISprite>("DetailBg/Demand/CrewGrid/CrewItem_1/Add");
        CrewItem_2_UIButton = root.FindScript<UIButton>("DetailBg/Demand/CrewGrid/CrewItem_2");
        Add_UISprite_2 = root.FindScript<UISprite>("DetailBg/Demand/CrewGrid/CrewItem_2/Add");
        CrewItem_3_UIButton = root.FindScript<UIButton>("DetailBg/Demand/CrewGrid/CrewItem_3");
        Add_UISprite_3 = root.FindScript<UISprite>("DetailBg/Demand/CrewGrid/CrewItem_3/Add");
        CrewItem_4_UIButton = root.FindScript<UIButton>("DetailBg/Demand/CrewGrid/CrewItem_4");
        Add_UISprite_4 = root.FindScript<UISprite>("DetailBg/Demand/CrewGrid/CrewItem_4/Add");
        CrewItem_5_UIButton = root.FindScript<UIButton>("DetailBg/Demand/CrewGrid/CrewItem_5");
        FriendIcon_UISprite = root.FindScript<UISprite>("DetailBg/Demand/CrewGrid/CrewItem_5/FriendIcon");
        FriendLabel_UILabel = root.FindScript<UISprite>("DetailBg/Demand/CrewGrid/CrewItem_5/FriendLabel");
        CancelOrFast_Transform = root.FindTrans("DetailBg/Demand/CancelOrFast");
        CancelBtn_UIButton = root.FindScript<UIButton>("DetailBg/Demand/CancelOrFast/CancelBtn");
        FastBtn_UIButton = root.FindScript<UIButton>("DetailBg/Demand/CancelOrFast/FastBtn");
        AcceptBtn_UIButton = root.FindScript<UIButton>("DetailBg/Demand/AcceptBtn");
        GetRewardBtn_UIButton = root.FindScript<UIButton>("DetailBg/Demand/GetRewardBtn");
        MissionProgressLabel_UILabel = root.FindScript<UILabel>("NumBg/MissionProgressLabel");
        TipsBtn_UIButton = root.FindScript<UIButton>("DetailBg/Demand/TipsBtn");
        Npc_UITexture = root.FindScript<UITexture>("NoMissionTips/Texture");
        NoMissionTips = root.FindGameObject("NoMissionTips");
        MissionTra = root.FindTrans("Mission");
        DetailBg = root.FindTrans("DetailBg");
        NumBg = root.FindGameObject("NumBg");

    }
    #endregion
}
