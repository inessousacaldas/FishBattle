﻿//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

/// <summary>
/// Generates a safe wrapper for _ModuleName_View.
/// </summary>
public partial class TabBtnWidget : BaseView
{
    public const string NAME ="TabBtnWidget";

    #region Element Bindings

    /// bind gameobject
    public GameObject redFlag;
    public UILabel btnLbl_UILabel;
    public UISprite TabBtnWidget_UISprite;
    public BoxCollider TabBtnWidget_BoxCollider;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        redFlag = root.FindGameObject("redFlag");
        btnLbl_UILabel = root.FindScript<UILabel>("btnLbl");
        TabBtnWidget_UISprite = root.FindScript<UISprite>("");
        TabBtnWidget_BoxCollider = root.FindScript<BoxCollider>("");

    }

    #endregion
}

