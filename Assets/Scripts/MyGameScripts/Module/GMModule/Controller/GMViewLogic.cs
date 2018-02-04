// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 7/29/2017 10:35:14 AM
// **********************************************************************

using UniRx;

public sealed partial class GMDataMgr
{
    
    public static partial class GMViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
            var ctrl = GMViewController.Show<GMViewController>(
                GMView.NAME
                , UILayerType.BaseModule
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IGMViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
            ctrl.TabMgr.Stream.Subscribe(e =>
            {
                ctrl.OnTabChange((GMDataTab)e,DataMgr._data);
                DataMgr._data.mainTab = (GMDataTab)e;
            });
            ctrl.TabMgr.SetTabBtn((int)DataMgr._data.mainTab);
            ctrl.OnTabChange(DataMgr._data.mainTab,DataMgr._data);
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => CloseBtn_UIButtonClick()));
            FireData();
        }

        
        private static void CloseBtn_UIButtonClick()
        {
            CloseView();
        }

        public static void CloseView()
        {
            UIModuleManager.Instance.CloseModule(GMView.NAME);
            DisposeModule();
        }

        public static void DisposeModule()
        {
            Dispose();
            GMCodeViewLogic.Dispose();
            GMDtoConnViewLogic.Dispose();
        }

        private static void Dispose()
        {
            DataMgr._data.mainTab = GMDataTab.Code;
            if(_disposable != null)
            {
                _disposable = _disposable.CloseOnceNull();
            }
        }
        // 请务必注意在关闭窗口的时候调用 _disposable ＝_disposable.CloseOnceNull();
    }
}

