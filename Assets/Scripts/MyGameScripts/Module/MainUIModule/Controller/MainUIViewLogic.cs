// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : CL-PC080
// Created  : 07/15/2017 18:59:40
// **********************************************************************

using System.Collections.Generic;
using UniRx;
using UnityEngine;
using AppDto;
using Assets.Scripts.MyGameScripts.Module.SkillModule;

public sealed partial class MainUIDataMgr
{
    
    public static partial class MainUIViewLogic
    {
        private static CompositeDisposable _disposable;
        private static IMainUIViewController mainCtrl;
        public static void Open()
        {
            
        // open的参数根据需求自己调整
            var ctrl = MainUIViewController.Show<MainUIViewController>(
                MainUIView.NAME
                , UILayerType.BaseModule
                , false
                , dataUpdator:Stream);
            InitReactiveEvents(ctrl);
        }

        //打开采集等任务的物品版
        public static void OpenMissionProps(AppMissionItem missionItem = null,System.Action callback = null,bool callbackState = false)
        {
            if (mainCtrl != null)
                mainCtrl.OnClickPopupUseMissionPropsBtn(missionItem,callback,callbackState);
        }
        private static void InitReactiveEvents(IMainUIViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
            if (mainCtrl == null) mainCtrl = ctrl;
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));

            _disposable.Add(UIModuleManager.Instance.TopModuleEvt.Subscribe(topNmae =>
            {
                if (topNmae == BattleDemoView.NAME
                    ||topNmae == MainUIView.NAME)
                    ctrl.Show();
                else
                    ctrl.Hide();
                
            }));
            _disposable.Add(NotifyListenerRegister.RegistListener<MainCrewInfoNotify>(ctrl.SetCrewInfo));
            _disposable.Add(MartialDataMgr.Stream.SubscribeAndFire(data => { ctrl.UpdateKungFuInfo(data);}));
            _disposable.Add(LayerManager.Stream.SubscribeAndFire(mode =>
            {
                DataMgr._data._showState.allBtnPanelHide = false;
                ctrl.UpdateByLayerChange(mode, DataMgr._data._showState);
            }));
            _disposable.Add(ctrl.OnButton_GmTest_UIButtonClick.Subscribe(_=>Button_GmTest_UIButtonClick()));
            _disposable.Add(ctrl.OnWorldMapBtn_UIButtonClick.Subscribe(_=>WorldMapBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnMiniMapBtn_UIButtonClick.Subscribe(_=>MiniMapBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnButton_PlayerInfo_UIButtonClick.Subscribe(_=>ctrl.OnPlayerInfoBtnClick(DataMgr._data)));
            _disposable.Add(ctrl.OnButton_Pack_UIButtonClick.Subscribe(_=>Button_Pack_UIButtonClick()));
            _disposable.Add(ctrl.OnButton_Partner_UIButtonClick.Subscribe(_=>Button_Partner_UIButtonClick()));
            _disposable.Add(ctrl.OnTempBtn_UIButtonClick.Subscribe(_=>TempBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnButton_Skill_UIButtonClick.Subscribe(_=>Button_Skill_UIButtonClick()));
            _disposable.Add(ctrl.OnButton_Guide_UIButtonClick.Subscribe(_=>Button_Guide_UIButtonClick()));
            _disposable.Add(ctrl.OnButton_Upgrade_UIButtonClick.Subscribe(_=>Button_Upgrade_UIButtonClick()));
            _disposable.Add(ctrl.OnLeftShrinkBtn_UIButtonClick.Subscribe(_=>LeftShrinkBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnButton_Daily_UIButtonClick.Subscribe(_=>Button_Daily_UIButtonClick()));
            _disposable.Add(ctrl.OnButton_Store_UIButtonClick.Subscribe(_=>Button_Store_UIButtonClick()));
            _disposable.Add(ctrl.OnButton_Trade_UIButtonClick.Subscribe(_=>Button_Trade_UIButtonClick()));
            _disposable.Add(ctrl.OnButton_Ranking_UIButtonClick.Subscribe(_=>Button_Ranking_UIButtonClick()));
            _disposable.Add(ctrl.OnButton_Reward_UIButtonClick.Subscribe(_=>Button_Reward_UIButtonClick()));
            _disposable.Add(ctrl.OnButton_Equipment_UIButtonClick.Subscribe(_=> Button_Equipment_UIButtonClick()));
            _disposable.Add(ctrl.OnWeatherBtn_UIButtonClick.Subscribe(_=>WeatherBtn_UIButtonClick(ctrl.WeatherBtn_GO)));
            _disposable.Add(ctrl.PlayerInfo.OnIconBtnClick.Subscribe(_ => OpenPlayerPropView()));
            _disposable.Add(ctrl.OnButton_ModelTest_UIButtonClick.Subscribe(_ => OnModelTestBtnClick()));
            _disposable.Add(ctrl.OnButton_lifeskill_UIButtonClick.Subscribe(_ => Button_lifeskill_UIButtonClick()));
            _disposable.Add(ctrl.OnButton_quartz_UIButtonClick.Subscribe(_ => Button_quartz_UIButtonClick()));
            _disposable.Add(ctrl.OnButton_Friend_UIButtonClick.Subscribe(_ => Button_Friend_UIButtonClick()));
            _disposable.Add(ctrl.OnButton_Question_UIButtonClick.Subscribe(_=> Button_Question_UIButtonClick()));
            _disposable.Add(ctrl.OnGuideBtn_UIButtonClick.Subscribe(_ => Button_Guide_UIButtonClick()));
            _disposable.Add(ctrl.OnScheduleBtn_UIButtonClick.Subscribe(_ => Button_Schedule_UIButtonClick()));
            _disposable.Add(ctrl.OnButton_Crew_UIButtonClick.Subscribe(_ => { Button_Partner_UIButtonClick(); }));
            _disposable.Add(ctrl.OnButton_Recruit_UIButtonClick.Subscribe(_ => { ProxyCrewReCruit.Open(); }));
            _disposable.Add(ctrl.OnButton_Guild_UIButtonClick.Subscribe(_ => Button_guild_UIButtonClick()));
            _disposable.Add(ctrl.PlayerInfo.OnBuffBtnClick.Subscribe(_ =>
            {
                List<int> list = new List<int>{1,2,3,4};        //用于展示 --xush
                ctrl.UpdateBuffShow(list);
            }));
            _disposable.Add(ctrl.ExpandBtnGroupCtrl.RefreshPos.Subscribe(e =>
            {
                DataMgr._data._showState.allBtnPanelHide = !e;
                ctrl.UpdateAllBtnGroup(DataMgr._data._showState.allBtnPanelHide, DataMgr._data._showState);
            }));

            _disposable.Add(ctrl.OnBottomShrinkBtn_UIButtonClick.Subscribe(_ =>
            {
                DataMgr._data._showState.allBtnPanelHide = !DataMgr._data._showState.allBtnPanelHide;
                ctrl.UpdateAllBtnGroup(DataMgr._data._showState.allBtnPanelHide, DataMgr._data._showState);
            }));

            _disposable.Add(ctrl.ExpandCtrl.GetTeamBtnClick.Subscribe(b =>
            {
                DataMgr._data._showState.expandPanelShow = b;
            }));
            
            

            //初始化ChatBox的界面
            ChatDataMgr.ChatBoxViewLogic.InitChatBoxEvent(ctrl.ChatBoxCtrl);
            #region 抢答
            _disposable.Add(QuestionDataMgr.Stream.SubscribeAndFire(data =>
            {
                ctrl.ChatBoxCtrl.SetQuizariumState(data.QuizariumData.QuizariumState);
                ctrl.ChatBoxCtrl.SetQuizraiumTime(data.QuizariumData.GetQuizariumTime);
            }));
            #endregion
            #region ExpandPanel

            _disposable.Add(ctrl.ExpandCtrl.OncallOrAwayBtn_UIButtonClick.Subscribe(_ =>
            {
                if (TeamDataMgr.DataMgr.HasTeam())
                {
                    if (TeamDataMgr.DataMgr.IsLeader())
                        TeamDataMgr.TeamNetMsg.SummonAwayTeamMembers();
                    else
                    {
                        var statue = TeamDataMgr.DataMgr.GetTeamberStatueById(ModelManager.Player.GetPlayerId());
                        if(statue == TeamMemberDto.TeamMemberStatus.Away)
                        {
                            //当前是否在四轮之塔，里面做了确定/取消判断，
                            if (TowerDataMgr.DataMgr.IsInTowerAndCheckLeave(delegate 
                            {
                                TeamDataMgr.TeamNetMsg.BackTeam();
                            })) return;
                            TeamDataMgr.TeamNetMsg.BackTeam();
                        }
                        else if(statue == TeamMemberDto.TeamMemberStatus.Member)
                            TeamDataMgr.TeamNetMsg.AwayTeam();
                    }
                }
                else
                {
                    OnCreateTeamBtnClick();
                }
            }));
            _disposable.Add(ctrl.ExpandCtrl.OnjoinOrLeaveBtn_UIButtonClick.Subscribe(_ =>
            {
                if (TeamDataMgr.DataMgr.HasTeam())
                    OnLeaveTeamBtnClick();
                else
                {
                    var funcOpen = FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_13, true);
                    if(funcOpen)
                        OnEasyCreateTeamBtnClick();
                }
            }));
            _disposable.Add(ctrl.ExpandCtrl.OnTeamRequestBtnClick.Subscribe(_=>
            {
                OnTeamRequestBtnClick();
                ctrl.ExpandCtrl.HideTeamRequestBtn();
            }));
            _disposable.Add(ctrl.OnUsePropsBtn_UIButtonClick.Subscribe(_ => ctrl.OnUsePropsBtnClick()));
            _disposable.Add(ctrl.OnMissionUseClose_UIButtonClick.Subscribe(_ => ctrl.OnClickPopupUseMissionPropsClose()));
            #endregion

            InitMissionCell(ctrl);

            _disposable.Add(WorldManager.WorkdModelStream.Subscribe(e =>
            {
                var sceneMap = WorldManager.Instance.GetModel().GetSceneDto().sceneMap;
                ctrl.SetActivityPollGo(false);
                switch (sceneMap.type)
                {
                    case (int)SceneMap.SceneType.SinglePlayer:
                        switch (sceneMap.sceneFunctionType)
                        {
                            case (int)SceneMap.SceneFunctionType.Tower:
                                var tower = TowerDataMgr.DataMgr.ITowerData();
                                ctrl.UpdateTowerGuildUI(tower);
                                if (tower.ShowTowerGuildUI())
                                    DataMgr._data.CurActivityType = ActivityPollType.FourTower;
                                else
                                    DataMgr._data.CurActivityType = ActivityPollType.None;
                                break;
                        }
                        break;
                    case (int)SceneMap.SceneType.Kungfu:
                        DataMgr._data.CurActivityType = ActivityPollType.Kungfu;
                        var data = MartialDataMgr.DataMgr.GetMartialData();
                        ctrl.UpdateKungFuInfo(data);
                        ctrl.SetActivityPollGo(true);
                        break;
                }
            }));

            _disposable.Add(ctrl.OnActivityPoll_UIButtonClick.Subscribe(e => OnActivityPollClick(DataMgr._data)));

        }

        private static void OnActivityPollClick(IMainUIData data)
        {
            switch (data.CurActivityType)
            {
                case ActivityPollType.FourTower:
                    TowerDataMgr.DataMgr.FindToNpc();
                    break;
                case ActivityPollType.Kungfu:
                    ProxyMartial.OpenMartialView();
                    break;
                default:
                    break;
            }
        }

        private static void Dispose()
        {
            OnDispose();
        }
        
        // 如果有自定义的内容需要清理，在此实现
        private static void OnDispose()
        {
            ChatDataMgr.ChatBoxViewLogic.Dispose();
            _disposable = _disposable.CloseOnceNull();
            mainCtrl = null;
        }


        private static void Button_guild_UIButtonClick()
        {
            ProxyGuildMain.OpenPanel();
        }
        private static void Button_GmTest_UIButtonClick()
        {
            ProxyGM.OpenPanel();
        }
        private static void WorldMapBtn_UIButtonClick()
        {
            ProxyWorldMapModule.OpenMiniWorldMap();
        }
        private static void MiniMapBtn_UIButtonClick()
        {
            if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_5, true)) return;

            ProxyWorldMapModule.OpenMiniMap();
        }

        private static void Button_Pack_UIButtonClick()
        {
            ProxyBackpack.OpenBackpack();
        }
        private static void Button_Partner_UIButtonClick()
        {
            CrewProxy.OpenCrewMainView();
        }
        private static void TempBtn_UIButtonClick()
        {
            ProxyBackpack.OpenTempBackpack();
        }
        private static void Button_Skill_UIButtonClick()
        {
            ProxyRoleSkill.OpenPanel();
        }
        private static void Button_Guide_UIButtonClick()
        {
            ProxyGuideMainView.Open();
        }
        private static void Button_Schedule_UIButtonClick()
        {
            ProxyScheduleMainView.Open();
        }
        private static void Button_Upgrade_UIButtonClick()
        {

        }
        private static void LeftShrinkBtn_UIButtonClick()
        {
            DataMgr._data._showState.topBtnPanelShow = !DataMgr._data._showState.topBtnPanelShow;
            FireData();
        }
        private static void Button_Daily_UIButtonClick()
        {
            //TipManager.AddTip("===敬请期待===");
            ProxyBracerMainView.Open();
        }

        private static void OnModelTestBtnClick()
        {
            ProxyGMTestModule.Close();
            ProxyAnimatorTestModule.Open();
        }
        private static void Button_Store_UIButtonClick()
        {
            //TipManager.AddTip("===敬请期待===");
            //return;
            ProxyShop.OpenShop();
            //ProxyShop.OpenShopByType(103,38,5);
        }
        private static void Button_Trade_UIButtonClick()
        {
            //TipManager.AddTip("===敬请期待===");
            //return;
            ProxyTrade.OpenTradeView();
            //ExChangeHelper.ConsumeBefore(AppVirtualItem.VirtualItemEnum.Gold, 18000000, () =>
            //{
            //    GameDebuger.Log("发送购买协议");
            //}, null);
        }
        private static void Button_Ranking_UIButtonClick()
        {
            ProxyRank.OpenRankView();
        }
        private static void Button_Reward_UIButtonClick()
        {
            //ProxyAssistSkillMain.OpenAssistSkillModule();
            //TipManager.AddTip("===敬请期待===");
            //return;
        }

        private static void Button_lifeskill_UIButtonClick()
        {
            ProxyAssistSkillMain.OpenAssistSkillModule();
        }
        private static void Button_quartz_UIButtonClick()
        {
            ProxyQuartz.OpenQuartzMainView();
        }

        private static void Button_Friend_UIButtonClick()
        {
            ProxySociality.OpenChatMainView(ChatPageTab.Friend);
        }

        private static void Button_Question_UIButtonClick()
        {
            ProxyQuestion.OpenQuestionView();
        }

        private static void Button_Equipment_UIButtonClick()
        {
            ProxyEquipmentMain.Open();   
        }
        private static void WeatherBtn_UIButtonClick(GameObject go)
        {
            bool night = SystemTimeManager.Instance.night;

            int hourMinute = SystemTimeManager.Instance.GetServerTime().Minute;
            int modMinute = hourMinute % 15;
            modMinute = 15 - modMinute;

            int hourSecond = SystemTimeManager.Instance.GetServerTime().Second;

            int leftSecond = modMinute * 60 + (60 - hourSecond);

            string nowWeather = night ? "黑夜" : "白天";
            string timeTip = DateUtil.GetVipTime(leftSecond, false);
            string nextWeather = !night ? "黑夜" : "白天";
            string hint = string.Format("现在为{0}，将在{1}后转为{2}\n夜间物理攻击和防御降低，带有夜战效果的则不受影响", nowWeather, timeTip,
                nextWeather);
            GameHintManager.Open(go, hint);
        }

        //人物属性面板入口
        private static void OpenPlayerPropView()
        {
            GameDebuger.TODO(@"if (ModelManager.BridalSedan.IsMe())
        {
            TipManager.AddTip('你正在乘坐花轿，不能到处乱跑哦！');
            return;
        }");
            ProxyPlayerProperty.OpenPlayerPropertyModule();
        }

        private static void OnEasyCreateTeamBtnClick()
        {
            ProxyTeamModule.OpenMainView(TeamMainViewTab.CreateTeam);
        }

        private static void OnLeaveTeamBtnClick()
        {
            var controller = ProxyBaseWinModule.Open();
            var title = "是否离队";
            var txt = "是否确定离队";
            BaseTipData data = BaseTipData.Create(title, txt, 0, () => { TeamDataMgr.TeamNetMsg.LeaveTeam(); }, null);
            controller.InitView(data);
        }

        private static void OnCreateTeamBtnClick()
        {
            var target = TeamDataMgr.DataMgr.GetCurTeamMatchTargetData;
            if (target == null)
            {
                var targetId = 0;
                var maxLv = 100;
                var minLv = 1;
                target = TeamMatchTargetData.Create(targetId, maxLv, minLv, true);
            }
                
            TeamDataMgr.TeamNetMsg.CreateTeam(target, () => ProxyTeamModule.OpenMainView());
        }
        
        private static void OnTeamRequestBtnClick()
        {
            ProxyTeamModule.OpenApplicationView();
        }
    }
}
public sealed partial class ChatDataMgr
{
    public class ChatBoxViewLogic
    {
        static CompositeDisposable _disposable =new CompositeDisposable();
        public static void InitChatBoxEvent(ChatBoxController ctrl)
        {
            _disposable.Clear();

            _disposable.Add(ctrl.OnSetUpBtn_UIButtonClick.Subscribe(_ =>
                ProxySociality.OpenChatBoxSetting()
            ));
            _disposable.Add(ctrl.ChatItemClickEvt.Subscribe(data =>
            {
                if (data == null)
                {
                    ProxySociality.OpenChatMainView(ChatPageTab.Chat);
                }
                else
                {
                    if (!string.IsNullOrEmpty(data.UrlStr))
                        ChatHelper.DecodeUrlMsg(data.UrlStr, UICamera.current.gameObject);
                    else
                        ProxySociality.OpenChatMainView(ChatPageTab.Chat, data.ChannelID);
                }
            }));

            var chatBoxData = DataMgr._data.ChatBoxData;
            //消息刷新
            _disposable.Add(ChatDataMgr.Stream.SubscribeAndFire
                (
                    data =>
                    {
                        ctrl.UpdateHornView(data);
                        ctrl.UpdateChatBoxUnRead(chatBoxData);
                        if(chatBoxData.LockState)
                            UpdateChatBoxNewMsg(ctrl);
                    }
                ));
            _disposable.Add(ctrl.TeamVoicePopUpListContrllor.OnChoiceIndexStream.Subscribe(idx=> {
                DataMgr._data._chatBoxData.CurTeamVoiceMode = (TeamVoiceMode)idx.EnumValue;
                FireData();
            }));
            _disposable.Add(ctrl.OnInteractionBtn_UIButtonClick.Subscribe(_ => {
                ProxyShortCutMessageView.Show();
            }));
            //拖动
            _disposable.Add(ctrl.OnScrollStopDrag.Subscribe(isBottom =>
            {
                if (chatBoxData.LockState != isBottom && isBottom)
                {
                    ResetBoxData();
                    FireData();
                }
                chatBoxData.LockState = isBottom;
            }));
            //未读消息点击
            _disposable.Add(ctrl.OnUnReadBtn_UIButtonClick.Subscribe(e => 
            {
                ResetBoxData();
                FireData();
            }));
            //改变size的按钮点击
            _disposable.Add(ctrl.OnUpBtn_UIButtonClick.Subscribe(_ =>
            {
                ResetBoxData();
                ctrl.IsUp = !ctrl.IsUp;
                ctrl.OnClickChatBoxExpandBtn(ctrl.IsUp);
                FireData();
            }));
        }

        private static void UpdateChatBoxNewMsg(ChatBoxController ctrl)
        {
            ResetBoxData();
            ctrl.UpdateView(DataMgr._data);
        }

        private static void ResetBoxData()
        {
            var boxData = DataMgr._data.ChatBoxData;
            boxData.LockState = true;
            boxData.ChatBoxNewMsgCnt[0] = 0;
        }

        public static void Dispose()
        {
            _disposable.Clear();
        }
    }

}
