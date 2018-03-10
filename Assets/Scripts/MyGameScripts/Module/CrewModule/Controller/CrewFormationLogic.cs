// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 7/24/2017 5:37:14 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using AppDto;

public sealed partial class TeamFormationDataMgr
{
    
    public static partial class CrewFormationLogic
    {
        private static CompositeDisposable _disposable;
        
        public static void Open(TeamFormationData.FormationType formationType = TeamFormationData.FormationType.CrewFormation)
        {
            // open的参数根据需求自己调整
            var ctrl = CrewFormationController.Show<CrewFormationController>(
                CrewFormation.NAME
                , UILayerType.DefaultModule
                , true
                , true
                , Stream);
            DataMgr._data.CurFormationType = formationType;
            InitReactiveEvents(ctrl);
        }

        private static void InitReactiveEvents(ICrewFormationController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }

            _disposable.Add(ctrl.CloseEvt.Subscribe(_ => Dispose()));
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => OnCloseBtnHandler()));
            _disposable.Add(ctrl.GetToggleFormationHandler.Subscribe(data => { ChangeCase(data); }));                //选择阵法
            _disposable.Add(ctrl.GetOptionClickHandler.Subscribe(tuple => { OnOptionClickHandler(tuple.p1, tuple.p2); }));        //设置伙伴状态
            
            _disposable.Add(ctrl.GetFormationCaseHandler.Subscribe(data =>
            {
                DataMgr._data.SetCurCaseIdx(data.GetCaseId);
                ctrl.UpdatePosGroup(DataMgr._data.GetCrewFormationList.Find(d=>d.caseId == data.GetCaseId));        //RefreshPos
            }));

            var caseId = DataMgr._data.CurFormationType ==
                         TeamFormationData.FormationType.ArenaFormation
                ? DataMgr._data.CrewFormationData.GetArenaCaseId
                : DataMgr._data.CrewFormationData.GetUseCaseIdx;
            ctrl.InitData(caseId);
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

        private static void OnCloseBtnHandler()
        {
            UIModuleManager.Instance.CloseModule(CrewFormation.NAME);
        }

        private static void OnOptionClickHandler(CrewFormationController.Option option, long id)
        {
            switch (option)
            {
                case CrewFormationController.Option.MainWar:
                    TeamFormationNetMsg.Formation_ChangeMainCrew(DataMgr._data.GetCurCaseIdx, id);
                    break;
                case CrewFormationController.Option.Up:
                    TeamFormationNetMsg.Formation_AddPosition(DataMgr._data.GetCurCaseIdx, id);
                    break;
                case CrewFormationController.Option.Down:
                    TeamFormationNetMsg.Formation_RemovePosition(DataMgr._data.GetCurCaseIdx, id);
                    break;
            }
        }

        private static void ChangeCase(IFormationCaseData data)
        {
            TeamFormationNetMsg.Formation_ChangeCase(data.GetCaseId);
        }
    }
}

