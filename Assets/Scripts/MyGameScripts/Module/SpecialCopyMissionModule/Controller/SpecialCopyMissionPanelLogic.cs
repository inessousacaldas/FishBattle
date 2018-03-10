// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 2/8/2018 3:10:11 PM
// **********************************************************************

using AppDto;
using UniRx;
using System.Collections.Generic;

public sealed partial class SpecialCopyDataMgr
{
    
    public static partial class SpecialCopyMissionPanelLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open(int MissionID)
        {
        // open的参数根据需求自己调整
            var ctrl = SpecialCopyMissionPanelController.Show<SpecialCopyMissionPanelController>(
                SpecialCopyMissionPanel.NAME
                , UILayerType.FloatTip
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
            ctrl.SetData(MissionID);
            DataMgr._data.GetMission = MissionID;
        }
        
        private static void InitReactiveEvents(ISpecialCopyMissionPanelController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnOnClose_UIButtonClick.Subscribe(_=>OnClose_UIButtonClick()));
            _disposable.Add(ctrl.OnGoToNpc_UIButtonClick.Subscribe(_=>GoToNpc_UIButtonClick()));
           
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
        private static void OnClose_UIButtonClick()
        {
            Close();
        }
        private static void GoToNpc_UIButtonClick()
        {
            Close();
            if(!TeamDataMgr.DataMgr.IsLeader())
            {
                if(TeamDataMgr.DataMgr.HasTeam())
                {
                    var val = WorldManager.Instance.GetModel().GetPlayerDto(ModelManager.Player.GetPlayerId());
                    if(val != null)
                    {
                        if(val.teamStatus != (int)TeamMemberDto.TeamMemberStatus.Away)
                        {
                            TipManager.AddTip("正在组队跟随状态中，不能进行此操作！");
                            return;
                        }
                    }
                }
            }

            List<Mission> tMission = MissionDataMgr.DataMgr.GetExistSubMissionMenuList();
            for(int i = 0;i < tMission.Count;i++)
            {
                if(tMission[i].id == DataMgr._data.GetMission)
                {
                    NpcInfoDto acceptNpc = MissionDataMgr.DataMgr.GetCompletionConditionNpc(tMission[i]);
                    Npc npc = MissionHelper.GetNpcByNpcInfoDto(acceptNpc);
                    WorldManager.Instance.FlyToByNpc(npc,0,null);
                    break;
                }
            }
        }

        public static void Close() {
            DataMgr._data.GetMission = 0;
            UIModuleManager.Instance.CloseModule(SpecialCopyMissionPanel.NAME);
        }
    
    }
}

