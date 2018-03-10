// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamMainViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Diagnostics;
using AppDto;
using UniRx;
using UnityEngine;
using System.Collections.Generic;
using AppStaticConfigs = AppProtobuf.AppStaticConfigs;

public sealed partial class TeamDataMgr
{
    public partial class TeamMainViewController:FRPBaseController_V1<TeamMainView, ITeamMainView>
    {
        private CompositeDisposable _disposable = new CompositeDisposable();
        public static void Open(TeamMainViewTab tab = TeamMainViewTab.Team)
        {
            if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_6, true)) return;

            DataMgr._data.curTab = tab;
            var controller = UIModuleManager.Instance.OpenFunModule<TeamMainViewController>(TeamMainView.NAME, UILayerType.DefaultModule,
                true, false);
            if (tab != TeamMainViewTab.Team)
            {
                controller.OnTabClick(tab);
                controller.GeTabbtnManager.SetTabBtn((int)tab);
            }
        }

        public TabbtnManager GeTabbtnManager { get { return tabMgr; } }
        private TabbtnManager tabMgr = null;

        private TeamInfoTabContentViewController _teamInfoController;
        private CreateTeamViewController _createTeamController;
        private RecommendTeamViewController _recommendTeamController;

        private readonly int _targetId = 0; //默认组队目标
        private readonly int _maxLv = 0;  //默认组队最大等级
        private readonly int _minLv = 0;    //默认组队最小等级

        private ITeamMatchTargetData _teamTarget;

        private bool _canCreateTeamRefresh = true;
        // 用数据层中的数据初始化界面
        protected override void InitViewWithStream()
        {
            stream.OnNext(DataMgr._data);
        }


	    // 界面初始化完成之后的一些后续初始化工作
        protected override void AfterInitView ()
        {
            CreateTabInfo();
            
            _disposable.Add(stream.Subscribe(data =>
            {
                View.UpdateView(data);
            }));

            if (DataMgr._data.teamDto != null)
            {
                var teamDto = DataMgr._data.teamDto;
                _teamTarget = TeamMatchTargetData.Create(teamDto.actionTargetId, teamDto.maxGrade, teamDto.minGrade, true);
                DataMgr.GetCurTeamMatchTargetData = _teamTarget;
            }
            else
            {
                _teamTarget = DataMgr.GetCurTeamMatchTargetData;
                if (_teamTarget == null)
                {
                    _teamTarget = TeamMatchTargetData.Create(_targetId, _maxLv, _minLv, true);
                }       
            }

            CreateTeamInfo();
            OnTabClick(DataMgr._data.curTab);
            tabMgr.SetTabBtn((int)DataMgr._data.curTab);
        }

	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
            View.OnCloseBtn_UIButtonClick.Subscribe(_=>CloseBtn_UIButtonClickHandler());
        }

        protected override void OnDispose()
        {
            tabMgr.Dispose();
            _disposable.Dispose();
            base.OnDispose();
        }

	    //在打开界面之前，初始化数据
	    protected override void InitData()
    	{
    	}

        private void CreateTabInfo()
        {
            Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
                _view.WinTabGroup_UIGrid.gameObject
                , TabbtnPrefabPath.TabBtnWidget.ToString()
                , "Tabbtn_" + i);

            List<ITabInfo> TeamTabInfos = new List<ITabInfo>(3)
            {
                TabInfoData.Create((int) TeamMainViewTab.Team, "我的队伍"),
                TabInfoData.Create((int)TeamMainViewTab.CreateTeam, "组队平台"),
                TabInfoData.Create((int) TeamMainViewTab.RecommendTeam, "推荐队伍")
            };

            tabMgr = TabbtnManager.Create(TeamTabInfos, func);
            if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_13))
                tabMgr.SetBtnHide((int)TeamMainViewTab.CreateTeam);

            tabMgr.Stream.Subscribe(pageIdx =>
            {
                DataMgr._data.curTab = (TeamMainViewTab)pageIdx;
                OnTabClick((TeamMainViewTab)pageIdx);
                FireData();
            });
        }

        public void OnTabClick(TeamMainViewTab tab, int page = -1)
        {
            switch (tab)
            {
                case TeamMainViewTab.Team:
                    CreateTeamInfo();
                    break;
                case TeamMainViewTab.CreateTeam:
                    TeamNetMsg.TeamPanel();
                    CreateTeamBuildView();
                    break;
                case TeamMainViewTab.RecommendTeam:
                    CreateRecommendTeam(page);
                    break;
            }
        }

        #region 我的队伍
        private void CreateTeamInfo()
        {
            if (_teamInfoController != null) return;
            _teamInfoController = AddChild<TeamInfoTabContentViewController, TeamInfoTabContentView>(
                View.Get_TabContentRoot()
                , TeamInfoTabContentView.NAME);

            _disposable.Add(stream.SubscribeAndFire(data => {
                if(_teamInfoController.gameObject.activeSelf)
                    _teamInfoController.UpdateView(data);
            }));

            _teamInfoController.SetCurFormatiom(DataMgr._data);

            _disposable.Add(_teamInfoController.OnCreateTeamBtn_UIButtonClick.Subscribe(_ => TeamNetMsg.CreateTeam(_teamTarget)));
            _disposable.Add(_teamInfoController.OnLeaveBtn_UIButtonClick.Subscribe(_ => TeamNetMsg.LeaveTeam()));
            _disposable.Add(_teamInfoController.OnBackTeamUiButtonClick.Subscribe(_ => TeamNetMsg.BackTeam()));
            _disposable.Add(_teamInfoController.OnAwayBtn_UIButtonClick.Subscribe(_ => TeamNetMsg.AwayTeam()));

            _disposable.Add(_teamInfoController.OnApplicationBtn_UIButtonClick.Subscribe(_ => OnApplicationBtnClickHandler()));
            _disposable.Add(_teamInfoController.OnSpeechBtn_UIButtonClick.Subscribe(_ => OnSpeechBtnClickHandler()));
            _disposable.Add(_teamInfoController.OnAutoMatchBtn_UIButtonClick.Subscribe(_ => OnAutoMatchBtnClickHandler()));
            _disposable.Add(_teamInfoController.OnGotoMissionBtn_UIButtonClick.Subscribe(_ => OnGotoMissionBtnHandler()));
            _disposable.Add(_teamInfoController.OnCancelBtn_UIButtonClick.Subscribe(_=> OnCancelAutoBtnClickHandler()));            

            _disposable.Add(_teamInfoController.OnArrayBtn_UIButtonClick.Subscribe(_ => OnArrayBtnClickHandler()));
            _disposable.Add(_teamInfoController.OnSettingBtn_UIButtonClick.Subscribe(_ => OnSettingBtnClickHandler(_teamInfoController.View.SecondPanel.gameObject)));
            _disposable.Add(_teamInfoController.OnCaptainBtn_UIButtonClick.Subscribe(_ => OnCaptainBtnClickHandler()));
            _disposable.Add(_teamInfoController.OnpartnerArrayBtn_UIButtonClick.Subscribe(_ => OnpartnerArrayBtnHandler()));
            _disposable.Add(_teamInfoController.OnBattleSettingBtn_UIButtonClick.Subscribe(_ =>
            {
                OnBattleSettingBtnClickHandler();
                _teamInfoController.View.SecondPanel.gameObject.SetActive(false);
            }));
            _disposable.Add(_teamInfoController.OnTeamSettingBtn_UIButtonClick.Subscribe(_ =>
            {
                OnTeamSettingClickHandler();
                _teamInfoController.View.SecondPanel.gameObject.SetActive(false);
            }));
            _disposable.Add(_teamInfoController.OnInviteBtn_UIButtonClick.Subscribe(_ => { OnInviteBtnClickHandler();}));

            _disposable.Add(_teamInfoController.OnTeamTargetBtn_UIButtonClick.Subscribe(_ =>
            {
                OnTeamTargetBtnHandler();
            }));

            _disposable.Add(TeamFormationDataMgr.Stream.SubscribeAndFire(_data =>
            {
                var tuple = _data.GetActivatedFormationNameAndLev();
                if (tuple != null)
                {
                    var show = _data.ActiveFormationId != (int)Formation.FormationType.Regular;
                    var str = FormationHelper.GetFormationNameAndLevel(tuple.p1, tuple.p2, show);
                    _teamInfoController.UpdateFomatinInfo(str);
                }
                else
                {
                    var str = FormationHelper.GetFormationNameAndLevel("无", 0, true);
                    _teamInfoController.UpdateFomatinInfo(str);
                }

                if (_data.CrewFormationData.GetCrewFormationList.Count <=1 ) return; //1代表只有一个防御阵

                var caseId = _data.CrewFormationData.GetUseCaseIdx;
                var data = _data.CrewFormationData.GetCrewFormationList.Find(d=>d.caseId == caseId);
                var f = _data.acquiredFormation.Find(d => d.formationId == data.formationId);

                ActiveCaseInfoDto dto = new ActiveCaseInfoDto();
                dto.formationId = data.formationId;
                dto.level = f == null ? 0 : f.level;
                dto.crewInfoDtos = data.casePositions;
                DataMgr._data.CurFormationInfo = dto;

                _teamInfoController.UpdateView(DataMgr._data);
            }));

            _disposable.Add(_teamInfoController.AddPlayerHandler.Subscribe(_ =>
            {
                tabMgr.SetTabBtn((int)TeamMainViewTab.RecommendTeam);
                DataMgr._data.curTab = TeamMainViewTab.RecommendTeam;
                OnTabClick(TeamMainViewTab.RecommendTeam, (int)RecommendViewTab.MyFriend);
                FireData();
            }));
        }
        #endregion

        #region 组队平台
        private void CreateTeamBuildView()
        {
            if (_createTeamController != null) return;

            _createTeamController = AddChild<CreateTeamViewController, CreateTeamView>(
                View.Get_CreateTabRoot()
                ,CreateTeamView.NAME);

            _disposable.Add(stream.SubscribeAndFire(data =>
            {
                if(_createTeamController.gameObject.activeSelf)
                    _createTeamController.UpdateView(data);
            }));

            _disposable.Add(_createTeamController.OnBackTeamBtn_UIButtonClick.Subscribe(_ => TeamNetMsg.BackTeam()));
            _disposable.Add(_createTeamController.OnAwayBtn_UIButtonClick.Subscribe(_ => TeamNetMsg.AwayTeam()));
            _disposable.Add(_createTeamController.OnCreateTeamBtn_UIButtonClick.Subscribe(_ => TeamNetMsg.CreateTeam(_teamTarget)));
            _disposable.Add(_createTeamController.OnLeaveBtn_UIButtonClick.Subscribe(_ => TeamNetMsg.LeaveTeam()));
            
            _disposable.Add(_createTeamController.OnAutoMatchBtn_UIButtonClick.Subscribe(_ => OnAutoMatchBtnClickHandler()));            
            _disposable.Add(_createTeamController.OnGotoMissionBtn_UIButtonClick.Subscribe(_ => OnGotoMissionBtnHandler()));
            _disposable.Add(_createTeamController.OnApplicationBtn_UIButtonClick.Subscribe(_ => OnInviteBtnClickHandler()));
            _disposable.Add(_createTeamController.OnSpeechBtn_UIButtonClick.Subscribe(_ => OnSpeechBtnClickHandler()));
            _disposable.Add(_createTeamController.OnApplyBtn_UIButtonClick.Subscribe(_ => OnApplicationBtnClickHandler()));
            _disposable.Add(_createTeamController.OnCancelBtn_UIButtonClick.Subscribe(_=> OnCancelAutoBtnClickHandler()));
            _disposable.Add(_createTeamController.OnCaptainBtn_UIButtonClick.Subscribe(_ => OnCaptainBtnClickHandler()));
            
            _disposable.Add(_createTeamController.OnTeamTargetBtn_UIButtonClick.Subscribe(_ =>
            {
                OnTeamTargetBtnHandler();
            }));
            
            _disposable.Add(_createTeamController.OnRefreshBtn_UIButtonClick.Subscribe(_ =>
            {
                if (!_canCreateTeamRefresh)
                {
                    var remainTime = Mathf.Floor(JSTimer.Instance.GetRemainTime("CreateTeamRefresh"));
                    if (remainTime > 0)
                        TipManager.AddTip(string.Format("刷新太急了，请等{0}秒后再试试吧。", remainTime));
                    return;
                }

                _canCreateTeamRefresh = false;
                TipManager.AddTip("刷新列表");

                JSTimer.Instance.SetupCoolDown("CreateTeamRefresh", 30f, null, () =>
                {
                    _canCreateTeamRefresh = true;
                });

                TeamNetMsg.TeamPanel();
            }));
        }
        #endregion

        #region 推荐队伍
        private void CreateRecommendTeam(int page = -1)
        {
            if (_recommendTeamController != null)
            {
                if (page != -1)
                {
                    _recommendTeamController.TurnPageView((RecommendViewTab)page);
                    _recommendTeamController.UpdateTabBtn(page);
                }
                return;
            }

            _recommendTeamController = AddChild<RecommendTeamViewController, RecommendTeamView>(
                View.Get_RecommendTeamRoot()
                , RecommendTeamView.NAME);

            _disposable.Add(stream.SubscribeAndFire(data =>
            {
                if(_recommendTeamController.gameObject.activeSelf)
                    _recommendTeamController.UpdateView(data);
            }));

            _recommendTeamController.TurnPageView(RecommendViewTab.MyFriend);
            _recommendTeamController.UpdateTabBtn(page);
        }
        #endregion

        // 客户端自定义代码
        protected override void RegistCustomEvent ()
        {
            
        }

        private void CloseBtn_UIButtonClickHandler(){
            UIModuleManager.Instance.CloseModule(TeamMainView.NAME);
        }

        #region TabInfoController
        private void OnApplicationBtnClickHandler()
        {
            ProxyTeamModule.OpenApplicationView();
        }

        private void OnArrayBtnClickHandler()
        {
            //TipManager.AddTip("====敬请期待=====");
            //return;
            ProxyFormation.OpenFormationView(FormationPosController.FormationType.Team);
        }

        private void OnAutoMatchBtnClickHandler()
        {
            if (DataMgr._data.GetMatchTargetData == null)
            {
                TipManager.AddTip("请选择组对目标");
                return;
            }

            TeamNetMsg.AutoMatchTeam(DataMgr._data.GetMatchTargetData, true);
        }

        private void OnCancelAutoBtnClickHandler()
        {
            TeamNetMsg.AutoMatchTeam(DataMgr._data.GetMatchTargetData);
        }

        private void OnSettingBtnClickHandler(GameObject go)
        {
            go.SetActive(!go.gameObject.activeSelf);
        }

        private void OnSpeechBtnClickHandler()
        {
            if (!DataMgr._data.IsLeader())
            {
                TipManager.AddTip("只有队长才能喊话");
                return;
            }

            if (DataMgr._data.GetMatchTargetData.GetActiveId == 0 ||
                DataMgr._data.teamDto.actionTargetId == 0) //0代表组对目标为全部
            {
                TipManager.AddTip("请选择具体组队目标");
                return;
            }
            ProxySpeakViewModule.Open();
        }

        private void OnCaptainBtnClickHandler()
        {
            TipManager.AddTip("===敬请期待===");
        }

        private void OnGotoMissionBtnHandler()
        {
            var target = DataMgr._data.GetMatchTargetData;
            var actionTarget = DataCache.getDtoByCls<TeamActionTarget>(target.GetActiveId);
            if (actionTarget == null)
            {
                TipManager.AddTip("请选择具体的行动目标");
                return;
            }

            var smartGuide = DataCache.getDtoByCls<SmartGuide>(actionTarget.smartGuideId);
            switch (smartGuide.type)
            {
                case (int)SmartGuide.SmartGuideType.SmartGuideType_3:
                    var npc = DataCache.getDtoByCls<Npc>(StringHelper.ToInt(smartGuide.param));
                    WorldManager.Instance.FlyToByNpc(npc);
                    CloseBtn_UIButtonClickHandler();
                    break;
                case (int)SmartGuide.SmartGuideType.SmartGuideType_4:
                    TipManager.AddTip(smartGuide.param);
                    break;
            }
        }

        private void OnpartnerArrayBtnHandler()
        {
            //TipManager.AddTip("====敬请期待=====");
            //return;
            ProxyFormation.OpenCrewFormation();
        }   

        private void OnBattleSettingBtnClickHandler()
        {
            TipManager.AddTip("===打开战斗指令界面===");
        }

        private void OnTeamSettingClickHandler()
        {
            ProxyWindowModule.OpenTeamSettingWindow();
        }

        private void OnInviteBtnClickHandler()
        {
            ProxyTeamModule.OpenInvitationView();
        }
        #endregion

        private void OnTeamTargetBtnHandler()
        {
            TeamMatchTargetViewLogic.Open();
        }
    }
}
