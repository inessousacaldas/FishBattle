// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 11/9/2017 3:18:17 PM
// **********************************************************************

using UniRx;
using AppDto;

public sealed partial class BracerMainViewDataMgr
{
    
    public static partial class BracerMainViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
            //功能是否开启
            if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_27, true)) return;

            // open的参数根据需求自己调整
            var layer = UILayerType.DefaultModule;
            var ctrl = BracerMainViewController.Show<BracerMainViewController>(
                BracerMainView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IBracerMainViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnRewardBtn_UIButtonClick.Subscribe(_=>RewardBtn_UIButtonClick())); 
            _disposable.Add(ctrl.OnBtn_UIButtonClick.Subscribe(_=>Btn_UIButtonClick()));
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => CloseBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnTipsBtn_UIButtonClick.Subscribe(_ => TipsBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnLvViewBtn_UIWidgetClick.Subscribe(_=> LvViewBtn_UIButtonClick()));

            BracerMainViewNetMsg.ReqEnterBracer();
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
        private static void RewardBtn_UIButtonClick()
        {
            TipManager.AddTip("领取奖励");
        }

        private static void Btn_UIButtonClick()
        {
            TipManager.AddTip("领取特殊任务");
        }
        private static void CloseBtn_UIButtonClick()
        {
            ProxyBracerMainView.Close();
        }
        private static void TipsBtn_UIButtonClick()
        {
            var rewardCtrl = BracerRewardViewController.Show<BracerRewardViewController>(BracerRewardView.NAME, UILayerType.ThreeModule, false, true);
            rewardCtrl.UpdateView();
        }
        private static void LvViewBtn_UIButtonClick()
        {
            BracerLvViewController.Show<BracerLvViewController>(BracerLvView.NAME, UILayerType.ThreeModule, true, false);
        }
    }
}

