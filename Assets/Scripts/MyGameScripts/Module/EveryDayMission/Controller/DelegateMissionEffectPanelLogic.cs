// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 11/21/2017 8:32:56 PM
// **********************************************************************

using UniRx;

public sealed partial class DelegateMissionEffectDataMgr
{
    
    public static partial class DelegateMissionEffectPanelLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
            // open的参数根据需求自己调整
            var ctrl = DelegateMissionEffectPanelController.Show<DelegateMissionEffectPanelController>(
                DelegateMissionEffectPanel.NAME
                , UILayerType.FloatTip
                , true
                , true
                , null);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IDelegateMissionEffectPanelController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
           
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

        public static void Close() {
            UIModuleManager.Instance.CloseModule(DelegateMissionEffectPanel.NAME);
        }
    }
}

