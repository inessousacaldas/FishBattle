﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed class GuildRequester : BaseView
{
    public const string NAME ="GuildRequester";

    #region Element Bindings

    /// bind gameobject
    public UILabel rNameLabel_UILabel;
    public UILabel rLvLabel_UILabel;
    public UILabel rGenderLabel_UILabel;
    public UILabel rRefererLabel_UILabel;
    public UILabel rOperateLabel_UILabel;
    public GameObject rOperateLabel;
    public UIButton rOperateBtn_UIButton;
    public GameObject rOperateBtn;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        rNameLabel_UILabel = root.FindScript<UILabel>("rNameLabel");
        rLvLabel_UILabel = root.FindScript<UILabel>("rLvLabel");
        rGenderLabel_UILabel = root.FindScript<UILabel>("rGenderLabel");
        rRefererLabel_UILabel = root.FindScript<UILabel>("rRefererLabel");
        rOperateLabel_UILabel = root.FindScript<UILabel>("rOperateLabel");
        rOperateLabel = root.FindGameObject("rOperateLabel");
        rOperateBtn_UIButton = root.FindScript<UIButton>("rOperateBtn");
        rOperateBtn = root.FindGameObject("rOperateBtn");

    }
    #endregion
}
