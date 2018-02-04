// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 12/19/2017 11:25:11 AM
// **********************************************************************

using UniRx;
using AppDto;

public sealed partial class GuideMainViewDataMgr
{
    
    public static partial class GuideMainViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
            if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_59)) return;

            // open的参数根据需求自己调整
            var layer = UILayerType.DefaultModule;
            var ctrl = GuideMainViewController.Show<GuideMainViewController>(
                GuideMainView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IGuideMainViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => OnCloseBtn_UIButtonClick()));

            GuideMainViewNetMsg.ReqGuideInfo();
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

        private static void OnCloseBtn_UIButtonClick()
        {
            ProxyGuideMainView.Close();
        }
    }
}

