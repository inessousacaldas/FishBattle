﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed class SearchFriendItem : BaseView
{
    public const string NAME ="SearchFriendItem";

    #region Element Bindings

    /// bind gameobject
    public UIButton FriendIcon_UIButton;
    public UILabel FactionLvLabel_UILabel;
    public UILabel FactionNameLabel_UILabel;
    public UILabel NameLabel_UILabel;
    public UISprite CampIcon_UISprite;
    public UILabel RelationshipLabel_UILabel;
    public UILabel DistanceLabel_UILabel;
    public UIButton AddBtn_UIButton;
    public UILabel AddedLabel_UILabel;
    public UILabel DistanceLabel_2_UILabel;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        FriendIcon_UIButton = root.FindScript<UIButton>("IconBG/FriendIcon");
        FactionLvLabel_UILabel = root.FindScript<UILabel>("FactionLvLabel");
        FactionNameLabel_UILabel = root.FindScript<UILabel>("FactionNameLabel");
        NameLabel_UILabel = root.FindScript<UILabel>("NameLabel");
        CampIcon_UISprite = root.FindScript<UISprite>("CampIcon");
        RelationshipLabel_UILabel = root.FindScript<UILabel>("RelationshipLabel");
        DistanceLabel_UILabel = root.FindScript<UILabel>("DistanceLabel");
        AddBtn_UIButton = root.FindScript<UIButton>("AddBtn");
        AddedLabel_UILabel = root.FindScript<UILabel>("AddedLabel");
        DistanceLabel_2_UILabel = root.FindScript<UILabel>("DistanceLabel_2");

    }
    #endregion
}
