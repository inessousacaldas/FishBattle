﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed class QuartzForgeItem : BaseView
{
    public const string NAME ="QuartzForgeItem";

    #region Element Bindings

    /// bind gameobject
    public UIButton QuartzForgeItem_UIButton;
    public UILabel Label_UILabel;
    public UISprite Icon_UISprite;
    public GameObject Lock;
    public GameObject Select;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        QuartzForgeItem_UIButton = root.FindScript<UIButton>("");
        Label_UILabel = root.FindScript<UILabel>("Label");
        Icon_UISprite = root.FindScript<UISprite>("Icon");
        Lock = root.FindGameObject("Lock");
        Select = root.FindGameObject("Select");

    }
    #endregion
}
