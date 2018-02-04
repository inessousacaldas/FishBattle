// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BaseTipWindowController.cs
// Author   : xush
// Created  : 7/2/2017 2:23:38 PM
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public partial class BaseTipData
{
    private string title;
    private string _content;
    private string _leftBtnLb;
    private string _rightBtnLb;
    private int time;
    private Action _enterAction;
    private Action _cancelAction;

    public string GetTitle { get { return title; } }
    public string GetContent { get { return _content; } }
    public int GetTime { get { return time; } }
    public Action GetEnterAction { get { return _enterAction; } }
    public Action GetCancelAction { get { return _cancelAction;} }
    public string GetLeftBtnLb { get { return _leftBtnLb; } }
    public string GetRightBtnLb { get { return _rightBtnLb; } }
    public static BaseTipData Create(string txt,
        string content,
        int time,
        Action enterAction,
        Action cancelAction,
        string leftBtnLb = "取消",
        string rightBtnLb = "确定")
    {
        BaseTipData data = new BaseTipData();
        data.title = txt;
        data.time = time;
        data._enterAction = enterAction;
        data._cancelAction = cancelAction;
        data._leftBtnLb = leftBtnLb;
        data._rightBtnLb = rightBtnLb;
        data._content = content;
        return data;
    }
}

public partial interface IBaseTipWindowController
{
	void InitView(TeamInvitationNotify notify, bool rank = false);
    void InitView(TeamRequestNotify notify, bool rank = false);
    void InitView(CallMemberNotify notify, bool rank = false);
    void InitShopConfirmTip(long updateTime, int price, Action onComfirm);
    IBaseTipContentController InitView(BaseTipData data, bool rank = false,Action closeUIAction = null);
    void InitAssistForget(string str, int constId, int cost, bool isFirst=false, Action enterCallback=null, Action cancelCallback = null);
}

public partial class BaseTipWindowController
{
	public static IBaseTipWindowController Show<T>(
		  string moduleName
		  , UILayerType layerType
		  , bool addBgMask
		  , bool bgMaskClose = false)
		  where T : MonoController, IBaseTipWindowController
	{
		var controller = UIModuleManager.Instance.OpenFunModule<T>(
				moduleName
				, layerType
				, addBgMask
				, bgMaskClose) as IBaseTipWindowController;
		
		return controller;        
	}

    private CompositeDisposable _disposable;

    private static bool OpeningWin;
    public static bool SetWinState { set { OpeningWin = value; } }

    public static ArrayList TipWinList = new ArrayList();


    //点击叉叉，关闭通用tip的时候执行的回调函数
    private Action mCloseUIAction;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
	{
        _disposable = new CompositeDisposable();
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
	{
        _disposable.Add(_view.CloseBtn_UIButton.AsObservable().Subscribe(_ => {
            if(mCloseUIAction != null) {
                mCloseUIAction();
                mCloseUIAction = null;
            }
            ProxyBaseWinModule.Close();
        }));
	}

	protected override void RemoveCustomEvent ()
	{
	}
	
	protected override void OnDispose()
	{
        mCloseUIAction = null;
        _disposable = _disposable.CloseOnceNull();
		base.OnDispose();
	}

	//在打开界面之前，初始化数据
	protected override void InitData()
	{
		
	}
	
	//普通的,只有一行文字,然后两个按钮
	public IBaseTipContentController InitView(BaseTipData data, bool rank = false,Action closeUIAction = null)
	{
        if (rank == false && OpeningWin)
        {
            TipWinList.Add(data);
            return null;
        }
        var controller = AddChild<BaseTipContentController, CommonTipWin>(
			_view.Anchor
			, CommonTipWin.NAME);

		controller.SetMainContent(data.GetTitle, 
            data.GetContent, 
            data.GetTime, 
            data.GetEnterAction, 
            data.GetCancelAction,
            data.GetLeftBtnLb,
            data.GetRightBtnLb);
        SetWinState = true;
        mCloseUIAction = closeUIAction;
        return controller;
	}

    #region 组队系统
    //邀请入队
    public void InitView(TeamInvitationNotify notify, bool rank = false)
    {
        if (rank == false && OpeningWin)
        {
            TipWinList.Add(notify);
            return;
        }
        var controller = AddChild<TeamBeInviteController, CommonTipWin>(
            _view.Anchor
            , CommonTipWin.NAME);

        controller.SetMainContent(notify);
        SetWinState = true;
    }

    //申请入队
    public void InitView(TeamRequestNotify notify, bool rank = false)
    {
        if (rank == false && OpeningWin)
        {
            TipWinList.Add(notify);
            return;
        }
        var controller = AddChild<TeamApplyController, CommonTipWin>(
            _view.Anchor
            , CommonTipWin.NAME);

        controller.SetMainContent(notify);
        SetWinState = true;
    }

    //召唤队员归队
    public void InitView(CallMemberNotify notify = null, bool rank = false)
    {
        if (rank == false && OpeningWin)
        {
            TipWinList.Add(notify);
            return;
        }
        var controller = AddChild<CallMemberController, CommonTipWin>(
            _view.Anchor
            , CommonTipWin.NAME);

        controller.SetMainContent();
        SetWinState = true;
    }
    #endregion

    #region 商城
    public void InitShopConfirmTip(long updateTime,int price,Action onComfirm)
    {
        var controller = AddChild<ShopConfirmTipViewController, CommonTipWin>(_view.Anchor, CommonTipWin.NAME);
        controller.InitView(updateTime,price, onComfirm);
    }
    #endregion

    #region 生活技能 遗忘
    public void InitAssistForget(string str, int constId, int cost, bool isFirst=false, Action enterCallback=null, Action cancelCallback=null)
    {
        var ctrl = AddChild<AssistSkillForgetViewController, AssistSkillForgetView>(_view.Anchor, AssistSkillForgetView.NAME);
        ctrl.UpdateView(str, constId, cost, isFirst, enterCallback, cancelCallback);
    }
    #endregion
}
