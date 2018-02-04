// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MainUIViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IMainUIViewController : ICloseView
{
     UniRx.IObservable<Unit> OnButton_GmTest_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnWorldMapBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnMiniMapBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_PlayerInfo_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_Pack_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnTempBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_Skill_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_Guide_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_Upgrade_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnLeftShrinkBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_Daily_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_Store_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_Trade_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_Reward_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_Equipment_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnBottomShrinkBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnWeatherBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_Ranking_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_Partner_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_ModelTest_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnUsePropsBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnMissionUseClose_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_lifeskill_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_quartz_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_Friend_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_Question_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnQuizariumBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnGuideBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnScheduleBtn_UIButtonClick {get;}
     UniRx.IObservable<Unit> OnActivityPoll_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_Crew_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_Recruit_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnButton_Guild_UIButtonClick { get; }

}

public partial class MainUIViewController:FRPBaseController<
    MainUIViewController
    , MainUIView
    , IMainUIViewController
    , IMainUIData>
    , IMainUIViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    Button_GmTest_UIButtonEvt = View.Button_GmTest_UIButton.AsObservable();
    WorldMapBtn_UIButtonEvt = View.WorldMapBtn_UIButton.AsObservable();
    MiniMapBtn_UIButtonEvt = View.MiniMapBtn_UIButton.AsObservable();
    Button_PlayerInfo_UIButtonEvt = View.Button_PlayerInfo_UIButton.AsObservable();
    Button_Pack_UIButtonEvt = View.Button_Pack_UIButton.AsObservable();
    TempBtn_UIButtonEvt = View.TempBtn_UIButton.AsObservable();
    Button_Skill_UIButtonEvt = View.Button_Skill_UIButton.AsObservable();
    Button_Guide_UIButtonEvt = View.Button_Guide_UIButton.AsObservable();
    Button_Upgrade_UIButtonEvt = View.Button_Upgrade_UIButton.AsObservable();
    LeftShrinkBtn_UIButtonEvt = View.LeftShrinkBtn_UIButton.AsObservable();
    Button_Daily_UIButtonEvt = View.Button_Daily_UIButton.AsObservable();
    Button_Store_UIButtonEvt = View.Button_Store_UIButton.AsObservable();
    Button_Trade_UIButtonEvt = View.Button_Trade_UIButton.AsObservable();
    Button_Reward_UIButtonEvt = View.Button_Reward_UIButton.AsObservable();
    Button_Equipment_UIButtonEvt = View.Button_Equipment_UIButton.AsObservable();
    BottomShrinkBtn_UIButtonEvt = View.BottomShrinkBtn_UIButton.AsObservable();
    WeatherBtn_UIButtonEvt = View.WeatherBtn_UIButton.AsObservable();
    Button_Ranking_UIButtonEvt = View.Button_Ranking_UIButton.AsObservable();
    Button_Partner_UIButtonEvt = View.Button_Partner_UIButton.AsObservable();
    Button_ModelTest_UIButtonEvt = View.Button_ModelTest_UIButton.AsObservable();
    UsePropsBtn_UIButtonEvt = View.UsePropsBtn_UIButton.AsObservable();
    MissionUseClose_UIButtonEvt = View.MissionUseClose_UIButton.AsObservable();
    Button_lifeskill_UIButtonEvt = View.Button_lifeskill_UIButton.AsObservable();
    Button_quartz_UIButtonEvt = View.Button_quartz_UIButton.AsObservable();
    Button_Friend_UIButtonEvt = View.Button_Friend_UIButton.AsObservable();
    Button_Question_UIButtonEvt = View.Button_Question_UIButton.AsObservable();
    QuizariumBtn_UIButtonEvt = View.QuizariumBtn_UIButton.AsObservable();
    GuideBtn_UIButtonEvt = View.GuideBtn_UIButton.AsObservable();
    ScheduleBtn_UIButtonEvt = View.ScheduleBtn_UIButton.AsObservable();
    ActivityPoll_UIButtonEvt = View.ActivityPoll_UIButton.AsObservable();
    Button_Crew_UIButtonEvt = View.Button_Crew_UIButton.AsObservable();
    Button_Recruit_UIButtonEvt = View.Button_Recruit_UIButton.AsObservable();
    Button_Guild_UIButtonEvt = View.Button_guild_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        Button_GmTest_UIButtonEvt = Button_GmTest_UIButtonEvt.CloseOnceNull();
        WorldMapBtn_UIButtonEvt = WorldMapBtn_UIButtonEvt.CloseOnceNull();
        MiniMapBtn_UIButtonEvt = MiniMapBtn_UIButtonEvt.CloseOnceNull();
        Button_PlayerInfo_UIButtonEvt = Button_PlayerInfo_UIButtonEvt.CloseOnceNull();
        Button_Pack_UIButtonEvt = Button_Pack_UIButtonEvt.CloseOnceNull();
        TempBtn_UIButtonEvt = TempBtn_UIButtonEvt.CloseOnceNull();
        Button_Skill_UIButtonEvt = Button_Skill_UIButtonEvt.CloseOnceNull();
        Button_Guide_UIButtonEvt = Button_Guide_UIButtonEvt.CloseOnceNull();
        Button_Upgrade_UIButtonEvt = Button_Upgrade_UIButtonEvt.CloseOnceNull();
        LeftShrinkBtn_UIButtonEvt = LeftShrinkBtn_UIButtonEvt.CloseOnceNull();
        Button_Daily_UIButtonEvt = Button_Daily_UIButtonEvt.CloseOnceNull();
        Button_Store_UIButtonEvt = Button_Store_UIButtonEvt.CloseOnceNull();
        Button_Trade_UIButtonEvt = Button_Trade_UIButtonEvt.CloseOnceNull();
        Button_Reward_UIButtonEvt = Button_Reward_UIButtonEvt.CloseOnceNull();
        Button_Equipment_UIButtonEvt = Button_Equipment_UIButtonEvt.CloseOnceNull();
        BottomShrinkBtn_UIButtonEvt = BottomShrinkBtn_UIButtonEvt.CloseOnceNull();
        WeatherBtn_UIButtonEvt = WeatherBtn_UIButtonEvt.CloseOnceNull();
        Button_Ranking_UIButtonEvt = Button_Ranking_UIButtonEvt.CloseOnceNull();
        Button_Partner_UIButtonEvt = Button_Partner_UIButtonEvt.CloseOnceNull();
        Button_ModelTest_UIButtonEvt = Button_ModelTest_UIButtonEvt.CloseOnceNull();
        UsePropsBtn_UIButtonEvt = UsePropsBtn_UIButtonEvt.CloseOnceNull();
        MissionUseClose_UIButtonEvt = MissionUseClose_UIButtonEvt.CloseOnceNull();
        Button_lifeskill_UIButtonEvt = Button_lifeskill_UIButtonEvt.CloseOnceNull();
        Button_quartz_UIButtonEvt = Button_quartz_UIButtonEvt.CloseOnceNull();
        Button_Friend_UIButtonEvt = Button_Friend_UIButtonEvt.CloseOnceNull();
        Button_Question_UIButtonEvt = Button_Question_UIButtonEvt.CloseOnceNull();
        QuizariumBtn_UIButtonEvt = QuizariumBtn_UIButtonEvt.CloseOnceNull();
        GuideBtn_UIButtonEvt = GuideBtn_UIButtonEvt.CloseOnceNull();
        ScheduleBtn_UIButtonEvt = ScheduleBtn_UIButtonEvt.CloseOnceNull();
        ActivityPoll_UIButtonEvt = ActivityPoll_UIButtonEvt.CloseOnceNull();
        Button_Crew_UIButtonEvt = Button_Crew_UIButtonEvt.CloseOnceNull();
        Button_Recruit_UIButtonEvt = Button_Recruit_UIButtonEvt.CloseOnceNull();
        Button_Guild_UIButtonEvt = Button_Guild_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> Button_GmTest_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_GmTest_UIButtonClick{
        get {return Button_GmTest_UIButtonEvt;}
    }

    private Subject<Unit> WorldMapBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnWorldMapBtn_UIButtonClick{
        get {return WorldMapBtn_UIButtonEvt;}
    }

    private Subject<Unit> MiniMapBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnMiniMapBtn_UIButtonClick{
        get {return MiniMapBtn_UIButtonEvt;}
    }

    private Subject<Unit> Button_PlayerInfo_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_PlayerInfo_UIButtonClick{
        get {return Button_PlayerInfo_UIButtonEvt;}
    }

    private Subject<Unit> Button_Pack_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_Pack_UIButtonClick{
        get {return Button_Pack_UIButtonEvt;}
    }

    private Subject<Unit> TempBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnTempBtn_UIButtonClick{
        get {return TempBtn_UIButtonEvt;}
    }

    private Subject<Unit> Button_Skill_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_Skill_UIButtonClick{
        get {return Button_Skill_UIButtonEvt;}
    }

    private Subject<Unit> Button_Guide_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_Guide_UIButtonClick{
        get {return Button_Guide_UIButtonEvt;}
    }

    private Subject<Unit> Button_Upgrade_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_Upgrade_UIButtonClick{
        get {return Button_Upgrade_UIButtonEvt;}
    }

    private Subject<Unit> LeftShrinkBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnLeftShrinkBtn_UIButtonClick{
        get {return LeftShrinkBtn_UIButtonEvt;}
    }

    private Subject<Unit> Button_Daily_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_Daily_UIButtonClick{
        get {return Button_Daily_UIButtonEvt;}
    }

    private Subject<Unit> Button_Store_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_Store_UIButtonClick{
        get {return Button_Store_UIButtonEvt;}
    }

    private Subject<Unit> Button_Trade_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_Trade_UIButtonClick{
        get {return Button_Trade_UIButtonEvt;}
    }

    private Subject<Unit> Button_Reward_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_Reward_UIButtonClick{
        get {return Button_Reward_UIButtonEvt;}
    }

    private Subject<Unit> Button_Equipment_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_Equipment_UIButtonClick{
        get {return Button_Equipment_UIButtonEvt;}
    }

    private Subject<Unit> BottomShrinkBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnBottomShrinkBtn_UIButtonClick{
        get {return BottomShrinkBtn_UIButtonEvt;}
    }

    private Subject<Unit> WeatherBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnWeatherBtn_UIButtonClick{
        get {return WeatherBtn_UIButtonEvt;}
    }

    private Subject<Unit> Button_Ranking_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_Ranking_UIButtonClick{
        get {return Button_Ranking_UIButtonEvt;}
    }

    private Subject<Unit> Button_Partner_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_Partner_UIButtonClick{
        get {return Button_Partner_UIButtonEvt;}
    }

    private Subject<Unit> Button_ModelTest_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_ModelTest_UIButtonClick{
        get {return Button_ModelTest_UIButtonEvt;}
    }

    private Subject<Unit> UsePropsBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnUsePropsBtn_UIButtonClick{
        get {return UsePropsBtn_UIButtonEvt;}
    }

    private Subject<Unit> MissionUseClose_UIButtonEvt;
    public UniRx.IObservable<Unit> OnMissionUseClose_UIButtonClick{
        get {return MissionUseClose_UIButtonEvt;}
    }

    private Subject<Unit> Button_lifeskill_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_lifeskill_UIButtonClick{
        get {return Button_lifeskill_UIButtonEvt;}
    }

    private Subject<Unit> Button_quartz_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_quartz_UIButtonClick{
        get {return Button_quartz_UIButtonEvt;}
    }

    private Subject<Unit> Button_Friend_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_Friend_UIButtonClick{
        get {return Button_Friend_UIButtonEvt;}
    }

    private Subject<Unit> Button_Question_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_Question_UIButtonClick{
        get {return Button_Question_UIButtonEvt;}
    }

    private Subject<Unit> QuizariumBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnQuizariumBtn_UIButtonClick{
        get {return QuizariumBtn_UIButtonEvt;}
    }

    private Subject<Unit> GuideBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnGuideBtn_UIButtonClick{
        get {return GuideBtn_UIButtonEvt;}
    }
    
    private Subject<Unit> ScheduleBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnScheduleBtn_UIButtonClick
    {
        get { return ScheduleBtn_UIButtonEvt; }
    }
    private Subject<Unit> ActivityPoll_UIButtonEvt;
    public UniRx.IObservable<Unit> OnActivityPoll_UIButtonClick{
        get {return ActivityPoll_UIButtonEvt;}
    }

    private Subject<Unit> Button_Crew_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_Crew_UIButtonClick{
        get {return Button_Crew_UIButtonEvt;}
    }

    private Subject<Unit> Button_Recruit_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_Recruit_UIButtonClick{
        get {return Button_Recruit_UIButtonEvt;}
    }

    private Subject<Unit> Button_Guild_UIButtonEvt;
    public UniRx.IObservable<Unit> OnButton_Guild_UIButtonClick {
        get { return Button_Guild_UIButtonEvt; }
    }

}
