// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewMainViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface ICrewMainViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnShowTypeBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnPullBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnTypeBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnFormationBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnFollowBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnAddExpBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnOtherAddExpBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnPullbackBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnChangeNameBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCrewAddBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnpageSprite_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnpageSprite_1_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnExtendCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnPageInfoBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnFavorableBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCrewNameBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCrewTalkBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCrewVoiceBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnMoreOperationBtn_UIButtonClick{get;}

}

public partial class CrewMainViewController:FRPBaseController<
    CrewMainViewController
    , CrewMainView
    , ICrewMainViewController
    , ICrewViewData>
    , ICrewMainViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    ShowTypeBtn_UIButtonEvt = View.ShowTypeBtn_UIButton.AsObservable();
    PullBtn_UIButtonEvt = View.PullBtn_UIButton.AsObservable();
    TypeBtn_UIButtonEvt = View.TypeBtn_UIButton.AsObservable();
    FormationBtn_UIButtonEvt = View.FormationBtn_UIButton.AsObservable();
    FollowBtn_UIButtonEvt = View.FollowBtn_UIButton.AsObservable();
    AddExpBtn_UIButtonEvt = View.AddExpBtn_UIButton.AsObservable();
    OtherAddExpBtn_UIButtonEvt = View.OtherAddExpBtn_UIButton.AsObservable();
    PullbackBtn_UIButtonEvt = View.PullbackBtn_UIButton.AsObservable();
    ChangeNameBtn_UIButtonEvt = View.ChangeNameBtn_UIButton.AsObservable();
    CrewAddBtn_UIButtonEvt = View.CrewAddBtn_UIButton.AsObservable();
    pageSprite_UIButtonEvt = View.pageSprite_UIButton.AsObservable();
    pageSprite_1_UIButtonEvt = View.pageSprite_1_UIButton.AsObservable();
    ExtendCloseBtn_UIButtonEvt = View.ExtendCloseBtn_UIButton.AsObservable();
    PageInfoBtn_UIButtonEvt = View.PageInfoBtn_UIButton.AsObservable();
    FavorableBtn_UIButtonEvt = View.FavorableBtn_UIButton.AsObservable();
    CrewNameBtn_UIButtonEvt = View.CrewNameBtn_UIButton.AsObservable();
    CrewTalkBtn_UIButtonEvt = View.CrewTalkBtn_UIButton.AsObservable();
    CrewVoiceBtn_UIButtonEvt = View.CrewVoiceBtn_UIButton.AsObservable();
    MoreOperationBtn_UIButtonEvt = View.MoreOperationBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        ShowTypeBtn_UIButtonEvt = ShowTypeBtn_UIButtonEvt.CloseOnceNull();
        PullBtn_UIButtonEvt = PullBtn_UIButtonEvt.CloseOnceNull();
        TypeBtn_UIButtonEvt = TypeBtn_UIButtonEvt.CloseOnceNull();
        FormationBtn_UIButtonEvt = FormationBtn_UIButtonEvt.CloseOnceNull();
        FollowBtn_UIButtonEvt = FollowBtn_UIButtonEvt.CloseOnceNull();
        AddExpBtn_UIButtonEvt = AddExpBtn_UIButtonEvt.CloseOnceNull();
        OtherAddExpBtn_UIButtonEvt = OtherAddExpBtn_UIButtonEvt.CloseOnceNull();
        PullbackBtn_UIButtonEvt = PullbackBtn_UIButtonEvt.CloseOnceNull();
        ChangeNameBtn_UIButtonEvt = ChangeNameBtn_UIButtonEvt.CloseOnceNull();
        CrewAddBtn_UIButtonEvt = CrewAddBtn_UIButtonEvt.CloseOnceNull();
        pageSprite_UIButtonEvt = pageSprite_UIButtonEvt.CloseOnceNull();
        pageSprite_1_UIButtonEvt = pageSprite_1_UIButtonEvt.CloseOnceNull();
        ExtendCloseBtn_UIButtonEvt = ExtendCloseBtn_UIButtonEvt.CloseOnceNull();
        PageInfoBtn_UIButtonEvt = PageInfoBtn_UIButtonEvt.CloseOnceNull();
        FavorableBtn_UIButtonEvt = FavorableBtn_UIButtonEvt.CloseOnceNull();
        CrewNameBtn_UIButtonEvt = CrewNameBtn_UIButtonEvt.CloseOnceNull();
        CrewTalkBtn_UIButtonEvt = CrewTalkBtn_UIButtonEvt.CloseOnceNull();
        CrewVoiceBtn_UIButtonEvt = CrewVoiceBtn_UIButtonEvt.CloseOnceNull();
        MoreOperationBtn_UIButtonEvt = MoreOperationBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> ShowTypeBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnShowTypeBtn_UIButtonClick{
        get {return ShowTypeBtn_UIButtonEvt;}
    }

    private Subject<Unit> PullBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPullBtn_UIButtonClick{
        get {return PullBtn_UIButtonEvt;}
    }

    private Subject<Unit> TypeBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnTypeBtn_UIButtonClick{
        get {return TypeBtn_UIButtonEvt;}
    }

    private Subject<Unit> FormationBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnFormationBtn_UIButtonClick{
        get {return FormationBtn_UIButtonEvt;}
    }

    private Subject<Unit> FollowBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnFollowBtn_UIButtonClick{
        get {return FollowBtn_UIButtonEvt;}
    }

    private Subject<Unit> AddExpBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnAddExpBtn_UIButtonClick{
        get {return AddExpBtn_UIButtonEvt;}
    }

    private Subject<Unit> OtherAddExpBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnOtherAddExpBtn_UIButtonClick{
        get {return OtherAddExpBtn_UIButtonEvt;}
    }

    private Subject<Unit> PullbackBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPullbackBtn_UIButtonClick{
        get {return PullbackBtn_UIButtonEvt;}
    }

    private Subject<Unit> ChangeNameBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnChangeNameBtn_UIButtonClick{
        get {return ChangeNameBtn_UIButtonEvt;}
    }

    private Subject<Unit> CrewAddBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCrewAddBtn_UIButtonClick{
        get {return CrewAddBtn_UIButtonEvt;}
    }

    private Subject<Unit> pageSprite_UIButtonEvt;
    public UniRx.IObservable<Unit> OnpageSprite_UIButtonClick{
        get {return pageSprite_UIButtonEvt;}
    }

    private Subject<Unit> pageSprite_1_UIButtonEvt;
    public UniRx.IObservable<Unit> OnpageSprite_1_UIButtonClick{
        get {return pageSprite_1_UIButtonEvt;}
    }

    private Subject<Unit> ExtendCloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnExtendCloseBtn_UIButtonClick{
        get {return ExtendCloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> PageInfoBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPageInfoBtn_UIButtonClick{
        get {return PageInfoBtn_UIButtonEvt;}
    }

    private Subject<Unit> FavorableBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnFavorableBtn_UIButtonClick{
        get {return FavorableBtn_UIButtonEvt;}
    }

    private Subject<Unit> CrewNameBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCrewNameBtn_UIButtonClick{
        get {return CrewNameBtn_UIButtonEvt;}
    }

    private Subject<Unit> CrewTalkBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCrewTalkBtn_UIButtonClick{
        get {return CrewTalkBtn_UIButtonEvt;}
    }

    private Subject<Unit> CrewVoiceBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCrewVoiceBtn_UIButtonClick{
        get {return CrewVoiceBtn_UIButtonEvt;}
    }

    private Subject<Unit> MoreOperationBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnMoreOperationBtn_UIButtonClick{
        get {return MoreOperationBtn_UIButtonEvt;}
    }


    }
