﻿//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

/// <summary>
/// Generates a safe wrapper for GameCenterView.
/// </summary>
public partial class SdkGameCenterView : BaseView
{
	public const string NAME ="SdkGameCenterView";
	public UIButton BindBtn;
	public UIButton ModifyPasswordBtn;
	public UIButton ChangeAccountBtn;
	public UIButton ModifyNameBtn;
	public UISprite AvatarIcon;
	public UILabel NameLabel;
	public UILabel UIDLabel;

	protected override void InitElementBinding ()
    {
        var root = this.gameObject.transform;
		BindBtn = root.Find("BindBtn").GetComponent<UIButton>();
		ModifyPasswordBtn = root.Find("ModifyPasswordBtn").GetComponent<UIButton>();
		ChangeAccountBtn = root.Find("ChangeAccountBtn").GetComponent<UIButton>();
		ModifyNameBtn = root.Find("ModifyNameBtn").GetComponent<UIButton>();
		AvatarIcon = root.Find("AccountInfo/AvatarIcon").GetComponent<UISprite>();
		NameLabel = root.Find("AccountInfo/NameLabel").GetComponent<UILabel>();
		UIDLabel = root.Find("AccountInfo/UIDLabel").GetComponent<UILabel>();
	}
}
