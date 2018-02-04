// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 1/17/2018 5:10:21 PM
// **********************************************************************

using AppDto;
using UniRx;

public sealed partial class MartialDataMgr
{
    
    public static partial class MartialViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
	    var layer = UILayerType.DefaultModule;
            var ctrl = MartialViewController.Show<MartialViewController>(
                MartialView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IMartialViewController ctrl)
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
            _disposable.Add(ctrl.OnTipsBtn_UIButtonClick.Subscribe(_=>TipsBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnFirstWarBtn_UIButtonClick.Subscribe(_=>FirstWarBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnFirstWinBtn_UIButtonClick.Subscribe(_=>FirstWinBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnToggleBtn_UIButtonClick.Subscribe(_=>ToggleBtn_UIButtonClick()));
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
            ProxyMartial.CloseMartialView();
        }
        private static void TipsBtn_UIButtonClick()
        {
        }
        private static void FirstWarBtn_UIButtonClick()
        {
            switch (DataMgr._data.KungFuInfo.joinBattleGiftState)
            {
                case (int)KungfuInfoDto.GiftState.UnAble:
                    TipManager.AddTip("首次战斗后可领取");
                    break;
                case (int)KungfuInfoDto.GiftState.Received:
                    TipManager.AddTip("你已经领取奖励");
                    break;
                case (int)KungfuInfoDto.GiftState.Able:
                    MartialNetMsg.GetReward((int)KungfuReward.KungfuRewardType.FirstBattle);
                    break;
            }
        }
        private static void FirstWinBtn_UIButtonClick()
        {
            switch (DataMgr._data.KungFuInfo.battleWinGiftState)
            {
                case (int)KungfuInfoDto.GiftState.Able:
                    MartialNetMsg.GetReward((int)KungfuReward.KungfuRewardType.FirstWin);
                    break;
                case (int)KungfuInfoDto.GiftState.UnAble:
                    TipManager.AddTip("首次战斗胜利后可领取");
                    break;
                case (int)KungfuInfoDto.GiftState.Received:
                    TipManager.AddTip("你已经领取奖励");
                    break;
            }
        }

        private static void ToggleBtn_UIButtonClick()
        {
        }

    
    }
}

