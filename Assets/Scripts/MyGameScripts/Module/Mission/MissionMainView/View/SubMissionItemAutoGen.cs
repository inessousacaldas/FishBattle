﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed partial class SubMissionItem : BaseView
{
    public const string NAME ="SubMissionItem";

    #region Element Bindings

    /// bind gameobject
    public UILabel lbltype_UILabel;
    public UISprite bg_UISprite;
    public UIButton SubMissionItem_UIButton;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        lbltype_UILabel = root.FindScript<UILabel>("lbltype");
        SubMissionItem_UIButton = root.FindScript<UIButton>("");
        bg_UISprite = root.FindScript<UISprite>("");
    }
    #endregion
}
