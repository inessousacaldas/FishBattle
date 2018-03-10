﻿//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

/// <summary>
/// Generates a safe wrapper for AnnouncementView.
/// </summary>
public partial class AnnouncementView : BaseView
{
	public const string NAME ="AnnouncementView";
	public UIButton CloseButton_UIButton;
	public UIButton OkButton_UIButton;
	public UITable Table_UITable;
	public UIEventTrigger dragRegion_UIEventTrigger;

	protected override void InitElementBinding ()
    {
        var root = this.gameObject.transform;
		CloseButton_UIButton = root.Find("BaseWindow/CloseBtn").GetComponent<UIButton>();
		OkButton_UIButton = root.Find("BgSprite/OkButton").GetComponent<UIButton>();
		Table_UITable = root.Find("BgSprite/ContentGroup/Scroll View/Table").GetComponent<UITable>();
		dragRegion_UIEventTrigger = root.Find("BgSprite/ContentGroup/dragRegion").GetComponent<UIEventTrigger>();
	}
}
