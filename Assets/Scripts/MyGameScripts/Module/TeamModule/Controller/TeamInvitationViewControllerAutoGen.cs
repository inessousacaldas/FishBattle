// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamInvitationViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface ITeamInvitationViewController : ICloseView
{
    UniRx.IObservable<Unit> OnRefreshBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnClearBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick { get; }

}

public partial class TeamInvitationViewController : FRPBaseController<
    TeamInvitationViewController
    , TeamInvitationView
    , ITeamInvitationViewController
    , ITeamData>
    , ITeamInvitationViewController
{
    //机器自动生成的事件订阅
    protected override void RegistEvent()
    {
        RefreshBtn_UIButtonEvt = View.RefreshBtn_UIButton.AsObservable();
        ClearBtn_UIButtonEvt = View.ClearBtn_UIButton.AsObservable();
        CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();

    }

    protected override void RemoveEvent()
    {
        RefreshBtn_UIButtonEvt = RefreshBtn_UIButtonEvt.CloseOnceNull();
        ClearBtn_UIButtonEvt = ClearBtn_UIButtonEvt.CloseOnceNull();
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();

    }

    private Subject<Unit> RefreshBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnRefreshBtn_UIButtonClick
    {
        get { return RefreshBtn_UIButtonEvt; }
    }

    private Subject<Unit> ClearBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnClearBtn_UIButtonClick
    {
        get { return ClearBtn_UIButtonEvt; }
    }

    private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick
    {
        get { return CloseBtn_UIButtonEvt; }
    }
}
