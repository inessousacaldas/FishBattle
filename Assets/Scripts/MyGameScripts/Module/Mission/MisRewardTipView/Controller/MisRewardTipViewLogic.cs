// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 12/12/2017 11:42:28 AM
// **********************************************************************

using UniRx;


public sealed partial class MisRewardTipDataMgr
{
    
    public static partial class MisRewardTipViewLogic
    {
        private static CompositeDisposable _disposable;

        public const string CheckItemTipsInBattle = "CheckItemTipsInBattle";
        public const string UpdateItemTipsInBattle = "UpdateItemTipsInBattle";
        public const string DelayItemTipsInBattle = "DelayItemTipsInBattle";
        public const string DelayCommonTipsInBattle = "DelayCommonTipsInBattle";
        public static void Open()
        {
        // open的参数根据需求自己调整
	    var layer = UILayerType.SubModule;
            var ctrl = MisRewardTipViewController.Show<MisRewardTipViewController>(
                MisRewardTipView.NAME
                , layer
                , false
                , false
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IMisRewardTipViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnwidgtBox_UIButtonClick.Subscribe(_=>widgtBox_UIButtonClick()));
            FireData();
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
        private static void widgtBox_UIButtonClick()
        {
            UIModuleManager.Instance.CloseModule(MisRewardTipView.NAME);
        }
    }
}

