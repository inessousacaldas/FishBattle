// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ItemUseTipsViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using AppDto;
using UniRx;

public partial class ItemUseTipsViewController:FRPBaseController_V1<ItemUseTipsView, IItemUseTipsView>
{
    private IDisposable _disposable = null;

    //机器自动生成的事件订阅
    protected override void RegistEvent ()
    {
        _disposable = _disposable.CombineRelease(View.OnOptBtn_UIButtonClick.Subscribe(_ => OptBtn_UIButtonClickHandler()));
        _disposable = _disposable.CombineRelease(View.OnCloseBtn_UIButtonClick.Subscribe(_ => CloseBtn_UIButtonClickHandler()));

    }
}
