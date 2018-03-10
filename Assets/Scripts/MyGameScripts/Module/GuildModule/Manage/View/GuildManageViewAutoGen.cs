﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed class GuildManageView : BaseView
{
    public const string NAME ="GuildManageView";

    #region Element Bindings

    /// bind gameobject
    public UIButton NoticeModificationBtn_UIButton;
    public UILabel NoticdContentLabel_UILabel;
    public UILabel ManifestoContentLabel_UILabel;
    public UIButton ManifestoModificationBtn_UIButton;
    public UIButton MoreMessageLabel_UIButton;
    public UIScrollView ScrollView_UIScrollView;
    public UIRecycledList GuildMessageContent_UIRecycledList;
    public UISprite GuildIcon_UISprite;
    public UILabel GuildNameLbl_UILabel;
    public UILabel GuildBossLbl_UILabel;
    public UILabel MemberCountLabel_UILabel;
    public UILabel GuildGradeLabel_UILabel;
    public UILabel GuildIdLabel_UILabel;
    public UILabel GuildAssetsLabel_UILabel;
    public UILabel MaintainCostLabel_UILabel;
    public UILabel ProsperityLabel_UILabel;
    public UILabel PopularityLabel_UILabel;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        NoticeModificationBtn_UIButton = root.FindScript<UIButton>("GuildNoticeGroup/GuildNotice/NoticeModificationBtn");
        NoticdContentLabel_UILabel = root.FindScript<UILabel>("GuildNoticeGroup/GuildNotice/NoticdContentLabel");
        ManifestoContentLabel_UILabel = root.FindScript<UILabel>("GuildNoticeGroup/GuildManifesto/ManifestoContentLabel");
        ManifestoModificationBtn_UIButton = root.FindScript<UIButton>("GuildNoticeGroup/GuildManifesto/ManifestoModificationBtn");
        MoreMessageLabel_UIButton = root.FindScript<UIButton>("GuildEventGroup/Title/MoreMessageLabel");
        ScrollView_UIScrollView = root.FindScript<UIScrollView>("GuildEventGroup/ScrollView");
        GuildMessageContent_UIRecycledList = root.FindScript<UIRecycledList>("GuildEventGroup/ScrollView/GuildMessageContent");
        GuildIcon_UISprite = root.FindScript<UISprite>("GuildInfoGroup/GuildIcon");
        GuildNameLbl_UILabel = root.FindScript<UILabel>("GuildInfoGroup/Table1/Name/GuildNameLbl");
        GuildBossLbl_UILabel = root.FindScript<UILabel>("GuildInfoGroup/Table1/Boss/GuildBossLbl");
        MemberCountLabel_UILabel = root.FindScript<UILabel>("GuildInfoGroup/Table1/MemberCount/MemberCountLabel");
        GuildGradeLabel_UILabel = root.FindScript<UILabel>("GuildInfoGroup/Table1/Grade/GuildGradeLabel");
        GuildIdLabel_UILabel = root.FindScript<UILabel>("GuildInfoGroup/Table1/ID/GuildIdLabel");
        GuildAssetsLabel_UILabel = root.FindScript<UILabel>("GuildInfoGroup/Table2/Assets/GuildAssetsLabel");
        MaintainCostLabel_UILabel = root.FindScript<UILabel>("GuildInfoGroup/Table2/MaintainCost/MaintainCostLabel");
        ProsperityLabel_UILabel = root.FindScript<UILabel>("GuildInfoGroup/Table2/Prosperity/ProsperityLabel");
        PopularityLabel_UILabel = root.FindScript<UILabel>("GuildInfoGroup/Table2/Popularity/PopularityLabel");

    }
    #endregion
}
