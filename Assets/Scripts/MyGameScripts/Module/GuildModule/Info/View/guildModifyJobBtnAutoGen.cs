﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed class guildModifyJobBtn : BaseView
{
    public const string NAME ="guildModifyJobBtn";

    #region Element Bindings

    /// bind gameobject
    public UIButton guildModifyJobBtn_UIButton;
    public UISprite Background_UISprite;
    public UILabel Label_UILabel;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        guildModifyJobBtn_UIButton = root.FindScript<UIButton>("");
        Background_UISprite = root.FindScript<UISprite>("Background");
        Label_UILabel = root.FindScript<UILabel>("Label");

    }
    #endregion
}
