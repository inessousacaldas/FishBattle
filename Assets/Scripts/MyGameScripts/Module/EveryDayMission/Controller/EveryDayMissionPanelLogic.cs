// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 11/10/2017 4:36:55 PM
// **********************************************************************

using UniRx;

public sealed partial class EveryDayMissionDataMgr
{
    
    public static partial class EveryDayMissionPanelLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
	    //var layer = //UILayerType.DefaultModule;
            var ctrl = EveryDayMissionPanelController.Show<EveryDayMissionPanelController>(
                EveryDayMissionPanel.NAME
                , UILayerType.DefaultModule
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
            ProxyMainUI.Hide();
        }
        
        private static void InitReactiveEvents(IEveryDayMissionPanelController ctrl)
        {
            if (ctrl == null) return;
            EveryDayMissionNetMsg.GetAllEveryDayMission();
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.IGetOptionClickEvt.Subscribe(missionID => {
                if(missionID != 0)
                    EveryDayMissionNetMsg.AcceptFaction(missionID);
            }));
           
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

        public static void Close()
        {
            ProxyMainUI.Show();
            UIModuleManager.Instance.CloseModule(EveryDayMissionPanel.NAME);
        }


    }
}

