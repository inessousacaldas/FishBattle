﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed class HornTypeBtn : BaseView
{
    public const string NAME ="HornTypeBtn";

    #region Element Bindings

    /// bind gameobject
    public UISprite HornTypeBtn_UISprite;
    public UIButton HornTypeBtn_UIButton;
    public UILabel hornLabel_UILabel;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        HornTypeBtn_UISprite = root.FindScript<UISprite>("");
        HornTypeBtn_UIButton = root.FindScript<UIButton>("");
        hornLabel_UILabel = root.FindScript<UILabel>("hornLabel");

    }
    #endregion
}
