﻿//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

/// <summary>
/// Generates a safe wrapper for BattleDemoConfigView.
/// </summary>
public partial class BattleDemoConfigView : BaseView
{
	public const string NAME ="BattleDemoConfigView";
	public UIButton CloseButton_UIButton;
	public UIButton BattleButton_UIButton;
	public UIButton WatchButton_UIButton;
	public UIButton ResumeButton_UIButton;
	public UIInput SceneIdInput_UIInput;
	public UIInput SceneCameraInput_UIInput;
	public UIScrollView ScrollViewEnemy_UIScrollView;
	public UIGrid GridEnemy_UIGrid;
	public UIScrollView ScrollViewFriend_UIScrollView;
	public UIGrid GridFriend_UIGrid;
	public UIButton BtnReloadConfig_UIButton;
	public UIButton BtnAddEnemy_UIButton;
	public UIButton BtnDeleteEnemy_UIButton;
	public UIButton BtnCopyEnemy_UIButton;
	public UIButton BtnCopyFriend_UIButton;
	public UIButton BtnDeleteFriend_UIButton;
	public UIButton BtnAddFriend_UIButton;
	public UIInput LabelEnemyMonsterId_UIInput;
	public UIInput LabelFriendMonsterId_UIInput;

	protected override void InitElementBinding ()
	{
		var root = this.gameObject.transform;
		CloseButton_UIButton = root.Find("CloseButton").GetComponent<UIButton>();
		BattleButton_UIButton = root.Find("CntrOptionBtns/BattleButton").GetComponent<UIButton>();
		WatchButton_UIButton = root.Find("CntrOptionBtns/WatchButton").GetComponent<UIButton>();
		ResumeButton_UIButton = root.Find("CntrOptionBtns/ResumeButton").GetComponent<UIButton>();
		SceneIdInput_UIInput = root.Find("CntrLabels/SceneIdLabel/SceneIdInput").GetComponent<UIInput>();
		SceneCameraInput_UIInput = root.Find("CntrLabels/SceneCameraLabel/SceneCameraInput").GetComponent<UIInput>();
		ScrollViewEnemy_UIScrollView = root.Find("CntrListEnemy/ScrollViewEnemy").GetComponent<UIScrollView>();
		GridEnemy_UIGrid = root.Find("CntrListEnemy/ScrollViewEnemy/GridEnemy").GetComponent<UIGrid>();
		ScrollViewFriend_UIScrollView = root.Find("CntrListFriend/ScrollViewFriend").GetComponent<UIScrollView>();
		GridFriend_UIGrid = root.Find("CntrListFriend/ScrollViewFriend/GridFriend").GetComponent<UIGrid>();
		BtnReloadConfig_UIButton = root.Find("CntrOptionBtns/BtnReloadConfig").GetComponent<UIButton>();
		BtnAddEnemy_UIButton = root.Find("CntrEnemy/CntrFighterOptionBtns/BtnAddEnemy").GetComponent<UIButton>();
		BtnDeleteEnemy_UIButton = root.Find("CntrEnemy/CntrFighterOptionBtns/BtnDeleteEnemy").GetComponent<UIButton>();
		BtnCopyEnemy_UIButton = root.Find("CntrEnemy/CntrFighterOptionBtns/BtnCopyEnemy").GetComponent<UIButton>();
		BtnCopyFriend_UIButton = root.Find("CntrFriend/CntrFighterOptionBtns/BtnCopyFriend").GetComponent<UIButton>();
		BtnDeleteFriend_UIButton = root.Find("CntrFriend/CntrFighterOptionBtns/BtnDeleteFriend").GetComponent<UIButton>();
		BtnAddFriend_UIButton = root.Find("CntrFriend/CntrFighterOptionBtns/BtnAddFriend").GetComponent<UIButton>();
		LabelEnemyMonsterId_UIInput = root.Find("CntrEnemy/LabelEnemyMonsterId").GetComponent<UIInput>();
		LabelFriendMonsterId_UIInput = root.Find("CntrFriend/LabelFriendMonsterId").GetComponent<UIInput>();
	}
}
