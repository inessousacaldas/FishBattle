// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 1/16/2018 2:27:53 PM
// **********************************************************************

using UniRx;

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
            ProxyRank.CloseTradeView();
        }

    
    }
}

