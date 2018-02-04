// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 10/18/2017 7:45:04 PM
// **********************************************************************
using System;
using UnityEngine;

public class ProxyProgressBar
{
    private static string _timeName = string.Empty;
    private static Action _onFinished = null;

    /// <summary>
    /// 是否移动才打断进度条(用于一些特殊需求情况，要求在整理包裹道具等其他操作时不打断进度条)
    /// </summary>
    /// <param name="pIsMoveClose"></param>
    public static void Close(bool pIsMoveClose = false)
    {
        if(!pIsMoveClose)
        {
            JSTimer.Instance.CancelCd(_timeName);
            _onFinished = null;
            UIModuleManager.Instance.CloseModule(ProgressBarView.NAME);
        }
    }

    public static void ShowProgressBar(
        string iconStr =""
       ,string labelStr = ""
       ,Action callback = null
       ,string taskName = ""
       ,Action onFinished = null
       ,float totalTime = 1f
       ,float updateFrequence = 0f
       ,bool timeScale = false,bool pIsMoveClose = false)
    {

        _timeName = taskName;
        _onFinished = onFinished;
        GameObject module = UIModuleManager.Instance.OpenFunModule(ProgressBarView.NAME, UILayerType.Dialogue, false);
        var controller = module.GetMissingComponent<ProgressBarViewController>();
        //采集了中自动停止寻路
        ModelManager.Player.StopAutoRun();
        controller.SetMissionUsePropsProgress(true,iconStr,labelStr,callback,pIsMoveClose);
        JSTimer.Instance.SetupCoolDown(_timeName,totalTime,
            (remainTime) => { controller.SetMissionUsePropsProgress(1 - remainTime / totalTime); },
            () =>
            {
                UIModuleManager.Instance.CloseModule(ProgressBarView.NAME);
                GameUtil.SafeRun(onFinished);
            },updateFrequence);

    }
}

