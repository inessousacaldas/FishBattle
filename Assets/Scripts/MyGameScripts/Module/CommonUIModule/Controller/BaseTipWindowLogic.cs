// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 7/2/2017 2:23:38 PM
// **********************************************************************

using UniRx;

public static partial class BaseTipWindowLogic
{
    private static CompositeDisposable _disposable;

    public static void Open()
    {
        // open的参数根据需求自己调整
        var ctrl = BaseTipWindowController.Show<BaseTipWindowController>(
            BaseTipWindow.NAME
            , UILayerType.BaseModule
            , true
            , true);
        InitReactiveEvents(ctrl);
    }

    private static void InitReactiveEvents(IBaseTipWindowController ctrl)
    {
        if (ctrl == null) return;
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
        {
            _disposable.Clear();
        }

        _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_=>CloseBtn_UIButtonClick()));

    }

	private static void Dispose()
    {
        _disposable = _disposable.CloseOnceNull();
        OnDispose();
    }

    // 如果有自定义的内容需要清理，在此实现
    private static void OnDispose()
    {

    }

    private static void CloseBtn_UIButtonClick()
    {
        Dispose();
    }
}

