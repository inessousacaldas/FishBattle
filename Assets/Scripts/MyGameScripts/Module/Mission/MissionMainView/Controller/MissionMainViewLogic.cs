// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 9/5/2017 11:21:50 AM
// **********************************************************************

using UniRx;
using AppDto;

public sealed partial class MissionDataMgr
{
    
    public static partial class MissionMainViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
	    var layer = UILayerType.DefaultModule;
            var ctrl = MissionMainViewController.Show<MissionMainViewController>(
                MissionMainView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }

        private static void InitReactiveEvents(IMissionMainViewController ctrl, MisViewTab tabType = MisViewTab.Accepted)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
            TestAcceptMission();
            _disposable.Add(ctrl.TabMgr.Stream.Subscribe(e =>
            {
                ctrl.OnTabBtnClick((MisViewTab)e, DataMgr._data);
            }
             ));
            _disposable.Add(ctrl.CloseEvt.Subscribe(_ => Dispose()));
            _disposable.Add(ctrl.Onclose_UIButtonClick.Subscribe(_ => close_UIButtonClick()));
            _disposable.Add(ctrl.OnDropBtn_UIButtonClick.Subscribe(_ => DropMission(ctrl.CurMission)));
            _disposable.Add(ctrl.OnGoonBtn_UIButtonClick.Subscribe(_ => AccessMission(ctrl.CurMission)));
            _disposable.Add(ctrl.OnCanAccessBtn_UIButtonClick.Subscribe(_ => CanAccessMission(ctrl.CurMission)));
            _disposable.Add(ctrl.SubItemClick.Subscribe(e => ctrl.OnSubItemClick(e)));
            _disposable.Add(ctrl.RecordAreaItemEvt.Subscribe(e => { ctrl.OnAreaClick(DataMgr._data,e); }));
            _disposable.Add(ctrl.OnRecordBtn_UIButtonClick.Subscribe(e => { ctrl.OnRecordDetailBtnClick(DataMgr._data); }));
            _disposable.Add(ctrl.EvtItemClick.Subscribe(e => { ctrl.OnEvtItemClick(e, DataMgr._data); }));
            _disposable.Add(ctrl.OnBlackbgBtn_UIButtonClick.Subscribe(e => { ctrl.ShowOrHideWindow(false); }));
            _disposable.Add(ctrl.OnRecordDetailItem_UIButtonClick.Subscribe(e => { ctrl.OnRecordDetailItemClick(e); }));
            _disposable.Add(ctrl.OnRecordStateBtnClick.Subscribe(e => { ctrl.OnStateBtnClick(e,DataMgr._data); }));
            _disposable.Add(ctrl.MisTypeItemClick.Subscribe(e => { ctrl.OnMisTypeItemClick(e); }));
            ctrl.OnTabBtnClick(tabType, DataMgr._data);
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
        private static void close_UIButtonClick()
        {
            UIModuleManager.Instance.CloseModule(MissionMainView.NAME);
        }

        private static void TestAcceptMission()
        {
            //MissionNetMsg.ReqAcceptMission(1);
            //MissionNetMsg.ReqAcceptMission(2);
            //MissionNetMsg.ReqAcceptMission(3);
            //MissionNetMsg.ReqAcceptMission(4);
            //MissionNetMsg.ReqAcceptMission(7);

        }

        private static void DropMission(Mission mission)
        {
            if (mission == null)
            {
                TipManager.AddTip("当前任务为空");
                return;
            }
            DataMgr._data.DropMission(mission);
            //if (MissionHelper.IsMainMissionType(mission))
            //{
            //    TipManager.AddTip("主线不能放弃~~~");
            //}
            //else
            //{
            //    string des = "确认是否放弃该任务";
            //    string title = "";
            //    var ctrl = ProxyBaseWinModule.Open();
            //    BaseTipData data = BaseTipData.Create(title, des, 0, delegate
            //    {
            //        DataMgr._data.DropMission(mission);
            //        //MissionNetMsg.ReqDropMission(mission.id);
            //    }, null);
            //    ctrl.InitView(data);
            //}
        }

        private static void AccessMission(Mission mission)
        {
            if (TowerDataMgr.DataMgr.IsInTowerAndCheckLeave(delegate
            {
                //DataMgr._data.FindToMissionNpcByMission(mission, true);
                DataMgr._data.FindToMissionNpcByMission(mission,true);
                close_UIButtonClick();
            })) return;
            DataMgr._data.FindToMissionNpcByMission(mission, true);
            close_UIButtonClick();
        }

        private static void CanAccessMission(Mission mission)
        {
            if (TowerDataMgr.DataMgr.IsInTowerAndCheckLeave(delegate
            {
                DataMgr._data.FindToMissionNpcByMission(mission, false);
                close_UIButtonClick();
            })) return;
            DataMgr._data.FindToMissionNpcByMission(mission, false);
            close_UIButtonClick();
        }
        
    }
}

