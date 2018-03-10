﻿// **********************************************************************
// Copyright (c) 2016 Baoyugame. All rights reserved.
// Author : shuai.yan
// Created : 6/20/2016 4:32:21 PM
// Desc	: Auto generated by MarsZ. update this if need.
// **********************************************************************

using UnityEngine;
using System.Collections.Generic;
using AppDto;
using AppServices;
using System.Collections;

 /// <summary>
 /// This is the controller class for module OperateBtnItem, use this to control the ui or view , such as it's init , update or dispose.
 /// @shuai.yan in 6/20/2016 4:32:21 PM
 /// </summary>
public class OperateBtnItemController : MonolessViewController<OperateBtnItem>
{    

    #region property and field
    #region const
    #endregion

    #region ui object
   
    #endregion

    public MainUIExpandContentViewController.OperateTeamType _itemType = MainUIExpandContentViewController.OperateTeamType.NONE;
    public string _test = "";
    public TeamMemberDto playerDto;
    public TeamMemberDto onClickDto;

    private System.Action<MainUIExpandContentViewController.OperateTeamType, TeamMemberDto, TeamMemberDto> _onSelectCallBack;
    #endregion

    #region interface functions
    public void Open(TeamMemberDto playerDto, TeamMemberDto onClickDto, System.Action<MainUIExpandContentViewController.OperateTeamType, TeamMemberDto, TeamMemberDto> callBack)
	{
        this.playerDto = playerDto;
        this.onClickDto = onClickDto;
        _onSelectCallBack = callBack;
	}

    protected override void InitView ()
    {
        if (null == _view)
        {
           base.InitView ();
            
        }
    }

    protected override void RegistCustomEvent ()
	{
	    EventDelegate.Set(_view.OperateBtnItem_UIButton.onClick, OperateBtnItemEvent);
    }

    public void OperateBtnItemEvent()
    {
        if(_onSelectCallBack != null)
            _onSelectCallBack(_itemType, playerDto, onClickDto);
    }

    public void SetLabel(string test)
    {
        _test = test;
        _view.Label_UILabel.text = _test;
		_view.Label_UILabel.GetComponent<ButtonLabelSpacingAdjust>().ReAdjust();
    }

    public void SetTypeItem(MainUIExpandContentViewController.OperateTeamType itemType)
    {
        _itemType = itemType;
    }


    #endregion
}
