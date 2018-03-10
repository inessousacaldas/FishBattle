// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : CL-PC080
// Created  : 07/04/2017 20:21:34
// **********************************************************************

using AppDto;
using UniRx;

public sealed partial class WorldMapDataMgr
{
    
    public static partial class MiniWorldMapViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
	    var layer = UILayerType.ThreeModule;
            var ctrl = MiniWorldMapViewController.Show<MiniWorldMapViewController>(
                MiniWorldMapView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IMiniWorldMapViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => CloseMiniMapView()));
            _disposable.Add(ctrl.OnCurrMapBtn_UIButtonClick.Subscribe(_ => OnCurrMapBtn()));
            _disposable.Add(ctrl.SceneBtnClickEvt.Subscribe(id => MapBtnClickHandler(id)));

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

        private static void MapBtnClickHandler(int mapID)
        {
            var scene = DataCache.getDtoByCls<SceneMap>(mapID);
            if (TowerDataMgr.DataMgr.IsInTowerAndCheckLeave(delegate { EnterMap(mapID); }))
                return;
            if (MartialDataMgr.DataMgr.IsInMartialScene())
            {
                TipManager.AddTip(string.Format("{0}场景禁止飞入", scene.name));
                return;
            }
            if (!TeamDataMgr.DataMgr.IsCanFlyScene())
            {
                TipManager.AddTip("正在组队跟随状态中，不能进行此操作!");
                return;
            }
            EnterMap(mapID);
        }

        private static void EnterMap(int mapID)
        {
            WorldManager.Instance.Enter(mapID, false);
            ProxyNpcDialogueView.Close();
            CloseMiniMapView();
        }

        private static void OnCurrMapBtn()
        {
            CloseMiniMapView();
            ProxyWorldMapModule.OpenMiniMap();
        }

        private static void CloseMiniMapView()
        {
            ProxyWorldMapModule.CloseMiniWorldMap();
        }
    }
}

