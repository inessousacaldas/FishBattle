// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  DelegateMissionEffectPanelController.cs
// Author   : DM-PC092
// Created  : 11/21/2017 8:32:56 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;

public partial interface IDelegateMissionEffectPanelController
{

}
public partial class DelegateMissionEffectPanelController    {

	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        View.Notice_TweenAlpha.PlayForward();
        JSTimer.Instance.CancelCd("CloseEffect");
        JSTimer.Instance.SetupCoolDown("CloseEffect",3,null,CloseEffect);
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IEveryDayMissionData data){

    }

    public void CloseEffect() {
        DelegateMissionEffectDataMgr.DelegateMissionEffectPanelLogic.Close();
    }

}
