// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 1/16/2018 2:27:53 PM
// **********************************************************************

using AppDto;
using AppServices;
using UniRx;
using UnityEngine;

public sealed partial class RankDataMgr
{
    
    public static partial class RankingViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
	    var layer = UILayerType.DefaultModule;
            var ctrl = RankingViewController.Show<RankingViewController>(
                RankingView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IRankingViewController ctrl)
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
           _disposable.Add(ctrl.RankingPageCtrl.OnMenuClickHandler.Subscribe(rankid =>
           {
               var index = DataMgr._data.GetAllRankData.Keys.FindElementIdx(d => d == rankid);
               if(index < 0)
                   RankNetMsg.RankingsInfo(rankid);
           }));
            _disposable.Add(ctrl.RankingPageCtrl.OnPlayerClickHandler.Subscribe(data =>
            {
                if (data.Getuid == ModelManager.Player.GetPlayerId()) return;
                switch (data.GetRankings.rankAlertType)
                {
                    //玩家综合实力
                    case 1:
                        TipManager.AddTip("玩家实力榜");
                        break;
                    //伙伴查看
                    case 2:
                        TipManager.AddTip("伙伴战力榜");
                        break;
                    //玩家交互
                    case 3:
                        GameUtil.GeneralReq(Services.Player_TipInfo(data.Getuid), resp =>
                        {
                            var tipsDto = resp as PlayerTipDto;
                            var win = FriendDetailViewController.Show<FriendDetailViewController>(FriendDetailView.NAME,
                            UILayerType.ThreeModule, false);
                            win.UpdateRankView(tipsDto);
                            win.SetPosition(new Vector3(225, 197, 0));
                        });
                        break;
                    default:
                        break;
                }
                
            }));
        }
            
        private static void Dispose()
        {
            _disposable = _disposable.CloseOnceNull();
            DataMgr._data.Dispose();
            OnDispose();    
        }
        
        // 如果有自定义的内容需要清理，在此实现
        private static void OnDispose()
        {
            
        }
        private static void CloseBtn_UIButtonClick()
        {
            ProxyRank.CloseRankView();
        }

    
    }
}

