﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed partial class GoodsItemTipsMainView : BaseView
{
    public const string NAME ="GoodsItemTipsMainView";

    #region Element Bindings

    /// bind gameobject
    public UITable Table_UITable;
    public UISprite Bg_UISprite;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        Table_UITable = root.FindScript<UITable>("Table");
        Bg_UISprite = root.FindScript<UISprite>("Bg");
    }
    #endregion
}
