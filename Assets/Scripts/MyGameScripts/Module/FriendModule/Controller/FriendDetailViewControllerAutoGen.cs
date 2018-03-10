// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FriendDetailViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface IFriendDetailViewController
{
     UniRx.IObservable<Unit> OnInviteTeamBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnFightBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnReportBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnJoinFactionBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnJoinTeamBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnDeleteBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnWatchBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnFlowerBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnZoneBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnBlackBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnChatBtn_UIButtonClick{get;}

}

public partial class FriendDetailViewController:MonoViewController<FriendDetailView>
    , IFriendDetailViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    InviteTeamBtn_UIButtonEvt = View.InviteTeamBtn_UIButton.AsObservable();
    FightBtn_UIButtonEvt = View.FightBtn_UIButton.AsObservable();
    ReportBtn_UIButtonEvt = View.ReportBtn_UIButton.AsObservable();
    JoinFactionBtn_UIButtonEvt = View.JoinFactionBtn_UIButton.AsObservable();
    JoinTeamBtn_UIButtonEvt = View.JoinTeamBtn_UIButton.AsObservable();
    DeleteBtn_UIButtonEvt = View.DeleteBtn_UIButton.AsObservable();
    WatchBtn_UIButtonEvt = View.WatchBtn_UIButton.AsObservable();
    FlowerBtn_UIButtonEvt = View.FlowerBtn_UIButton.AsObservable();
    ZoneBtn_UIButtonEvt = View.ZoneBtn_UIButton.AsObservable();
    BlackBtn_UIButtonEvt = View.BlackBtn_UIButton.AsObservable();
    ChatBtn_UIButtonEvt = View.ChatBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        InviteTeamBtn_UIButtonEvt = InviteTeamBtn_UIButtonEvt.CloseOnceNull();
        FightBtn_UIButtonEvt = FightBtn_UIButtonEvt.CloseOnceNull();
        ReportBtn_UIButtonEvt = ReportBtn_UIButtonEvt.CloseOnceNull();
        JoinFactionBtn_UIButtonEvt = JoinFactionBtn_UIButtonEvt.CloseOnceNull();
        JoinTeamBtn_UIButtonEvt = JoinTeamBtn_UIButtonEvt.CloseOnceNull();
        DeleteBtn_UIButtonEvt = DeleteBtn_UIButtonEvt.CloseOnceNull();
        WatchBtn_UIButtonEvt = WatchBtn_UIButtonEvt.CloseOnceNull();
        FlowerBtn_UIButtonEvt = FlowerBtn_UIButtonEvt.CloseOnceNull();
        ZoneBtn_UIButtonEvt = ZoneBtn_UIButtonEvt.CloseOnceNull();
        BlackBtn_UIButtonEvt = BlackBtn_UIButtonEvt.CloseOnceNull();
        ChatBtn_UIButtonEvt = ChatBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> InviteTeamBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnInviteTeamBtn_UIButtonClick{
        get {return InviteTeamBtn_UIButtonEvt;}
    }

    private Subject<Unit> FightBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnFightBtn_UIButtonClick{
        get {return FightBtn_UIButtonEvt;}
    }

    private Subject<Unit> ReportBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnReportBtn_UIButtonClick{
        get {return ReportBtn_UIButtonEvt;}
    }

    private Subject<Unit> JoinFactionBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnJoinFactionBtn_UIButtonClick{
        get {return JoinFactionBtn_UIButtonEvt;}
    }

    private Subject<Unit> JoinTeamBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnJoinTeamBtn_UIButtonClick{
        get {return JoinTeamBtn_UIButtonEvt;}
    }

    private Subject<Unit> DeleteBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnDeleteBtn_UIButtonClick{
        get {return DeleteBtn_UIButtonEvt;}
    }

    private Subject<Unit> WatchBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnWatchBtn_UIButtonClick{
        get {return WatchBtn_UIButtonEvt;}
    }

    private Subject<Unit> FlowerBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnFlowerBtn_UIButtonClick{
        get {return FlowerBtn_UIButtonEvt;}
    }

    private Subject<Unit> ZoneBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnZoneBtn_UIButtonClick{
        get {return ZoneBtn_UIButtonEvt;}
    }

    private Subject<Unit> BlackBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnBlackBtn_UIButtonClick{
        get {return BlackBtn_UIButtonEvt;}
    }

    private Subject<Unit> ChatBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnChatBtn_UIButtonClick{
        get {return ChatBtn_UIButtonEvt;}
    }


    }
