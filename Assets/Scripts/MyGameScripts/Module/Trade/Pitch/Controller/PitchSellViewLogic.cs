// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 7/1/2017 6:01:01 PM
// **********************************************************************

using UniRx;

public sealed partial class TradeDataMgr
{
    
    public static partial class PitchSellViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
	    var layer = UILayerType.SubModule;
            var ctrl = PitchSellViewController.Show<PitchSellViewController>(
                PitchSellView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IPitchSellViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnAddBtn_UIButtonClick.Subscribe(_=>AddBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnPriceAddBtn_UIButtonClick.Subscribe(_=>ctrl.AddPriceLb(true)));
            _disposable.Add(ctrl.OnPriceReduceBtn_UIButtonClick.Subscribe(_=> ctrl.AddPriceLb(false)));
            _disposable.Add(ctrl.OnAddNumBtn_UIButtonClick.Subscribe(_=>ctrl.UpdateCountLb(true)));
            _disposable.Add(ctrl.OnReduceNumBtn_UIButtonClick.Subscribe(_=> ctrl.UpdateCountLb(false)));
            _disposable.Add(ctrl.GetSellHandler.Subscribe(str=>SellBtn_UIButtonClick(str)));
           _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_=>CloseBtn_UIButtonClick()));
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
        
        private static void AddBtn_UIButtonClick()
        {
            TradeHelper.OpenLockPitchitem(DataMgr._data.Capability, () => { TradeNetMsg.OpenLockPitchitem(); });
        }

        private static void SellBtn_UIButtonClick(string str)
        {
            TradeNetMsg.StallUp(str);
        }

        private static void CloseBtn_UIButtonClick()
        {
            ProxyTrade.ClosePitchSellView();
        }
    }
}

