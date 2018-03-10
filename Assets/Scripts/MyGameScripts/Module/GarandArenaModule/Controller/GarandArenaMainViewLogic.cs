// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 12/14/2017 2:45:59 PM
// **********************************************************************

using UniRx;
using AppDto;

public sealed partial class GarandArenaMainViewDataMgr
{
    
    public static partial class GarandArenaMainViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
            //有队伍 且非暂离 不可进入竞技场
            if (TeamDataMgr.DataMgr.HasTeam() 
                && TeamDataMgr.DataMgr.TeamState(ModelManager.Player.GetPlayerId()) != (int)TeamMemberDto.TeamMemberStatus.Away)
            {
                TipManager.AddTip("格兰竞技场为个人玩法，请退队或者暂离再来");
                return;
            }

            // open的参数根据需求自己调整
            var layer = UILayerType.DefaultModule;
            var ctrl = GarandArenaMainViewController.Show<GarandArenaMainViewController>(
                GarandArenaMainView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IGarandArenaMainViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_=>CloseBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnExchangeBtn_UIButtonClick.Subscribe(_=>ExchangeBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnReportBtn_UIButtonClick.Subscribe(_=>ReportBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnEmbattleBtn_UIButtonClick.Subscribe(_=>EmbattleBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnTipsBtn_UIButtonClick.Subscribe(_=>TipsBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnRefreshBtn_UIButtonClick.Subscribe(_=>RefreshBtn_UIButtonClick()));

            GarandArenaMainViewNetMsg.ReqArenaInfo();
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

        private static void CloseBtn_UIButtonClick()
        {
            ProxyGarandArenaMainView.Close();
        }

        //积分商城
        private static void ExchangeBtn_UIButtonClick()
        {
            ProxyShop.OpenShop(ShopTypeTab.ArenaShop, ShopTypeTab.ArenaScroeShopId);
        }

        private static void ReportBtn_UIButtonClick()
        {
            GarandArenaReportViewController.Show<GarandArenaReportViewController>(GarandArenaReportView.NAME, UILayerType.ThreeModule, true, false);
        }

        //布阵
        private static void EmbattleBtn_UIButtonClick()
        {
            TeamFormationDataMgr.TeamFormationNetMsg.ReqGarandArenaFormation(() =>
            {
                TeamFormationDataMgr.CrewFormationLogic.Open(TeamFormationDataMgr.TeamFormationData.FormationType.ArenaFormation);
            });
        }

        private static void TipsBtn_UIButtonClick()
        {
            ProxyTips.OpenTextTips(15, new UnityEngine.Vector3(38, -230), true);
        }

        private static void RefreshBtn_UIButtonClick()
        {
            if (DataMgr._data.CanRefresh)
            {
                GarandArenaMainViewNetMsg.ReqRefresh();
            }
            else
            {
                //60秒CD刷
                int cdTime = 60 - (int)((SystemTimeManager.Instance.GetUTCTimeStamp() - DataMgr._data.RefreshCdAt) / 1000);
                cdTime = cdTime > 1 ? cdTime : 1;
                TipManager.AddTip(string.Format("寻找下一批对手还需要{0}秒", cdTime));
            }
        }
    }
}

