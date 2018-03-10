// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RedPackViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;
using System;
public partial interface IRedPackViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> Onbutton_key_UIButtonClick{get;}
     UniRx.IObservable<Unit> Onbutton_common_UIButtonClick{get;}
}

public partial class RedPackViewController : FRPBaseController<
    RedPackViewController
    , RedPackView
    , IRedPackViewController
    , IRedPackData>
    , IRedPackViewController
{
    //机器自动生成的事件订阅
    protected override void RegistEvent()
    {
        CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
        button_key_UIButtonEvt = View.button_key_UIButton.AsObservable();
        button_common_UIButtonEvt = View.button_common_UIButton.AsObservable();
        Page_guild_UIButtonEvt = View.Page_guild_UIButton.AsObservable();
        Page_world_UIButtonEvt = View.Page_world_UIButton.AsObservable();

    }

    protected override void RemoveEvent()
    {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        button_key_UIButtonEvt = button_key_UIButtonEvt.CloseOnceNull();
        button_common_UIButtonEvt = button_common_UIButtonEvt.CloseOnceNull();
        Page_guild_UIButtonEvt = Page_guild_UIButtonEvt.CloseOnceNull();
        Page_world_UIButtonEvt = Page_world_UIButtonEvt.CloseOnceNull();
    }

    private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick
    {
        get { return CloseBtn_UIButtonEvt; }
    }

    private Subject<Unit> button_key_UIButtonEvt;
    public UniRx.IObservable<Unit> Onbutton_key_UIButtonClick
    {
        get { return button_key_UIButtonEvt; }
    }

    private Subject<Unit> button_common_UIButtonEvt;
    public UniRx.IObservable<Unit> Onbutton_common_UIButtonClick
    {
        get { return button_common_UIButtonEvt; }
    }

    private Subject<Unit> Page_guild_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPageGuildUiButtonClick
    {
        get { return Page_guild_UIButtonEvt; }
    }

    private Subject<Unit> Page_world_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPageWorldUiButtonClick
    {
        get { return Page_world_UIButtonEvt; }
    }

}
