// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 12/17/2017 2:57:47 PM
// **********************************************************************

using AppDto;
using UniRx;

public sealed partial class ContestDataMgr
{
    
    public static partial class ContestPanelLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open(object bagitem)
        {
        // open的参数根据需求自己调整
            var ctrl = ContestPanelController.Show<ContestPanelController>(
                ContestPanel.NAME
                ,UILayerType.DefaultModule
                , true
                , true
                , Stream);
            DataMgr._data.SetCurBagItem(bagitem as BagItemDto);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IContestPanelController ctrl)
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

        public static void OnClose() {
            UIModuleManager.Instance.CloseModule(ContestPanel.NAME);
        }

    
    }
}

