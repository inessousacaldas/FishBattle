// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 1/20/2018 11:55:37 AM
// **********************************************************************

using AppDto;
using UniRx;

public sealed partial class CopyPanelDataMgr
{
    
    public static partial class CopyMissionViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
            var ctrl = CopyMissionViewController.Show<CopyMissionViewController>(
                CopyMissionView.NAME
                ,UILayerType.DefaultModule
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(ICopyMissionViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnEnter_UIButtonClick.Subscribe(_=>Enter_UIButtonClick()));
            _disposable.Add(ctrl.OnClose_UIButtonClick.Subscribe(_ => OnClose()));
            _disposable.Add(ctrl.GetIconClickEvt.Subscribe(data =>
            {
                DataMgr._data.CopyID = data.mCopyID;
                DataMgr._data.EnterCopy = data.mCopy.id;
                FireData();
            }));

            _disposable.Add(ctrl.OnNormalButton_ButtonClick.Subscribe(_ => {
                DataMgr._data.CopyType = 0;
                DataMgr._data.CopyID = 0;
                DataMgr._data.EnterCopy = ctrl.GetdefaultNormal;
                FireData();
            }));
            _disposable.Add(ctrl.OnEliteButton_ButtonClick.Subscribe(_ => {
                DataMgr._data.CopyType = 1;
                DataMgr._data.CopyID = 0;
                DataMgr._data.EnterCopy = ctrl.GetdefaultElite;
                FireData();
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
        private static void Enter_UIButtonClick()
        {
            if(!TeamDataMgr.DataMgr.IsLeader())
            {
                if(TeamDataMgr.DataMgr.HasTeam())
                {
                    TipManager.AddTip("你不是队长");
                    return;
                }
            }
            CopyPanelNetMsg.EnterCopy(DataMgr._data.EnterCopy);
        }

        public static void OnClose()
        {
            DataMgr._data.OnClose();
            UIModuleManager.Instance.CloseModule(CopyMissionView.NAME);
        }

    }
}

