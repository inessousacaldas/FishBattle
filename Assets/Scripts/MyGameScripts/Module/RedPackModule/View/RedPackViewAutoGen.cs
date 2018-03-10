﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed class RedPackView : BaseView
{
    public const string NAME ="RedPackView";

    #region Element Bindings

    /// bind gameobject
    public UIRecycledList RedPackGrid_UIGrid;
    public UIButton button_key_UIButton;
    public UIButton button_common_UIButton;
    public UIButton Page_guild_UIButton;
    public UIButton Page_world_UIButton;
    public UIGrid TabBtn_UIGrid;
    public UIButton CloseBtn_UIButton;

    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        RedPackGrid_UIGrid = root.FindScript<UIRecycledList>("Content/RedPackGrid");
        button_key_UIButton = root.FindScript<UIButton>("RedPacket/button_key");
        button_common_UIButton = root.FindScript<UIButton>("RedPacket/button_common");
        Page_guild_UIButton = root.FindScript<UIButton>("Content/Page_guild");
        Page_world_UIButton = root.FindScript<UIButton>("Content/Page_world");
        CloseBtn_UIButton = root.FindScript<UIButton>("RedPacket/CloseBtn");
        TabBtn_UIGrid = root.FindScript<UIGrid>("Content/TabBtn");
}
    #endregion
}
