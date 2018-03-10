﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed class TeamEasyGroupInfoItem : BaseView
{
    public const string NAME ="TeamEasyGroupInfoItem";

    #region Element Bindings

    /// bind gameobject
    public UISprite IconSprite_UISprite;
    public UILabel targetName_UILabel;
    public UILabel sceneLb_UILabel;
    public UISprite factionInfo_UISprite;
    public Transform memberGrid_Transform;
    public UILabel leaderName_UILabel;
    public UIButton applyBtn_UIButton;
    public UILabel BtnLb_UILabel;
    public UIGrid MagicGrid_UIGrid;
    public UIGrid LvGrid_UIGrid;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        IconSprite_UISprite = root.FindScript<UISprite>("IconSprite");
        targetName_UILabel = root.FindScript<UILabel>("targetName");
        sceneLb_UILabel = root.FindScript<UILabel>("factionInfo/sceneLb");
        factionInfo_UISprite = root.FindScript<UISprite>("factionInfo");
        memberGrid_Transform = root.FindTrans("memberGrid");
        leaderName_UILabel = root.FindScript<UILabel>("leaderName");
        applyBtn_UIButton = root.FindScript<UIButton>("applyBtn");
        BtnLb_UILabel = root.FindScript<UILabel>("applyBtn/BtnLb");
        MagicGrid_UIGrid = root.FindScript<UIGrid>("MagicGrid");
        LvGrid_UIGrid = root.FindScript<UIGrid>("LvGrid");

    }
    #endregion
}
