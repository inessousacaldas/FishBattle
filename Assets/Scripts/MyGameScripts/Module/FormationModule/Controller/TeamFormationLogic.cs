// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// **********************************************************************

using System;
using AppDto;
using UniRx;

public sealed partial class TeamFormationDataMgr
{
    
    public static partial class TeamFormationTabContentViewLogic
    {
        private static CompositeDisposable _disposable;
        private static FormationPosController.FormationType _type;
        public static void Open(FormationPosController.FormationType type)
        {
            // open的参数根据需求自己调整
            var ctrl = TeamFormationController.Show<TeamFormationController>(
                TeamFormationTabContentView.NAME
                , UILayerType.DefaultModule
                , true
                , true
                , Stream);

            _type = type;
            InitData(type);
            InitReactiveEvents(ctrl);
        }

        private static void InitData(FormationPosController.FormationType type)
        {
            switch (type)
            {
                case FormationPosController.FormationType.Team:
                    if (!TeamDataMgr.DataMgr.HasTeam())
                    {
                        var caseIdx = DataMgr._data.AllCaseInfoDto.activeAttackFormationCaseIndex;
                        DataMgr._data.SetCurCaseIdx(caseIdx);
                    }
                    break;
                case FormationPosController.FormationType.Crew:
                    DataMgr._data.FormationListSort();
                    var crewCaseIdx = DataMgr._data.GetCurCaseIdx;
                    var caseData = DataMgr._data.CrewFormationData.GetCrewFormationList.Find(d=>d.caseId == crewCaseIdx);
                    var curFormation = DataCache.getDtoByCls<Formation>(caseData.formationId);
                    DataMgr._data.SetCurCaseIdx(crewCaseIdx);
                    DataMgr._data.SetCurFormationID(curFormation.id);
                    break;
            }
        }

        private static void InitReactiveEvents(ITeamFormationController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }

            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => CloseBtn_UIButtonClick()));
            _disposable.Add(ctrl.GetLearnFormationHandler.Subscribe(id => LearnBtn_UIButtonClick(id)));
            _disposable.Add(ctrl.GetUpGradeHandler.Subscribe(id => UpGradeBtn_UIButtonClick(id)));
            _disposable.Add(ctrl.GetNoUseFormationHandler.Subscribe(_ => NoUseBtn_UIButtonClick()));
            _disposable.Add(ctrl.GetUseBtnHandler.Subscribe(id => UseBtn_UIButtonClick(id)));
            
            ctrl.InitFormationList(DataMgr._data, _type);
        }

        private static void CloseBtn_UIButtonClick()
        {
            UIModuleManager.Instance.CloseModule(TeamFormationTabContentView.NAME);
            _disposable = _disposable.CloseOnceNull();
        }

        private static void LearnBtn_UIButtonClick(int formationId)
        {
            TeamFormationNetMsg.Formation_UpgradeOrLearn(formationId);
        }

        private static void NoUseBtn_UIButtonClick()
        {
            if (!IsLeader())
            {
                TipManager.AddTip("只有队长才能关闭队形");
                return;
            }

            TeamFormationNetMsg.Formastion_ChangeCaseFormation(DataMgr._data.GetCurCaseIdx, (int) Formation.FormationType.Regular);
        }

        private static void UseBtn_UIButtonClick(int formationId)
        {
            if (!IsLeader())
            {
                TipManager.AddTip("只有队长才能开启队形");
                return;
            }

            TeamFormationNetMsg.Formastion_ChangeCaseFormation(DataMgr._data.GetCurCaseIdx, formationId);
        }

        private static void UpGradeBtn_UIButtonClick(int formationId)
        {
            //默认选中使用中的
            DataMgr._data.CurUpFradeFormation = DataMgr._data.GetFormationById(formationId);
            ProxyFormation.OpenUpdateView();
        }

        private static bool IsLeader()
        {
            if (_type == FormationPosController.FormationType.Team)
            {
                if (!TeamDataMgr.DataMgr.HasTeam())
                    return true;
                return TeamDataMgr.DataMgr.IsLeader();
            }
            return true;
        }
    }
}

