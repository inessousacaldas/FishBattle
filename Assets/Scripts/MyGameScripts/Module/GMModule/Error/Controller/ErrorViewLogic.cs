// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : fmd
// Created  : 7/28/2017 3:42:37 PM
// **********************************************************************

using Assets.Scripts.MyGameScripts.Utils;
using UniRx;

public static partial class ErrorViewLogic
{
    private static CompositeDisposable _disposable;
    private static IErrorViewController ctrl;

    public static void ShowError(string s,string detail = "",bool needLog = true)
    {
        return;
            ctrl = ErrorViewController.Show<ErrorViewController>(
                ErrorView.NAME
                ,UILayerType.BaseModule
                ,true
                ,true);
        InitReactiveEvents(ctrl);
        ctrl.ShowError(s,detail,needLog);
    }

    public static void RegisterErrorLog()
    {
//        UnityEngine.Application.logMessageReceived += logMessageReceived;
    }

    private static void logMessageReceived(string msg,string stackTrace,UnityEngine.LogType type)
    {
        if(type == UnityEngine.LogType.Error)
        {
            ShowError(msg + "\n" + stackTrace);
        }
    }

    private static void InitReactiveEvents(IErrorViewController ctrl)
    {
        if(ctrl == null) return;
        if(_disposable == null)
            _disposable = new CompositeDisposable();
        else
        {
            _disposable.Clear();
        }

        _disposable.Add(ctrl.OnbtnCopy_UIButtonClick.Subscribe(_ => btnCopy_UIButtonClick(ctrl)));
        _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => CloseBtn_UIButtonClick()));

        _disposable.Add(ctrl.PageTurnCtrl.Stream.Subscribe(
            pageIdx =>
            {
                ctrl.OnPageChange(pageIdx);
            }
            ));
    }

    private static void btnCopy_UIButtonClick(IErrorViewController ctrl)
    {
        StringUtil.CopyString(ctrl.ErrorMsg);
    }

    private static void CloseBtn_UIButtonClick()
    {
        UIModuleManager.Instance.CloseModule(ErrorView.NAME);
        _disposable = _disposable.CloseOnceNull();
    }

    // 请务必注意在关闭窗口的时候调用 _disposable ＝_disposable.CloseOnceNull();
}

