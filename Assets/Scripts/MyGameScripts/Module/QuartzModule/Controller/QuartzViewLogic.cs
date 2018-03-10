// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 8/25/2017 6:05:18 PM
// **********************************************************************

using System.Linq;
using UniRx;
using ZXing.Datamatrix;

public sealed partial class QuartzDataMgr
{
    
    public static partial class QuartzViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open(TabEnum tab = TabEnum.Info)
        {
        // open的参数根据需求自己调整
            var ctrl = QuartzViewController.Show<QuartzViewController>(
                QuartzView.NAME
                , UILayerType.DefaultModule
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl, tab);
        }
        
        private static void InitReactiveEvents(IQuartzViewController ctrl, TabEnum tab = TabEnum.Info)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_=>CloseBtn_UIButtonClick()));
            
            _disposable.Add(ctrl.GetClickHandler.Subscribe(data =>
            {
                DataMgr._data.SetCurOrbmentData(data);
                DataMgr._data.IsBagGroup = false;
                DataMgr._data.CurQuartzPos = 1;
                DataMgr._data.CurBagPos = -1;
                FireData();
            })); 

            _disposable.Add(ctrl.IGreIconScrollEvt.Subscribe(data =>
            {
                ctrl.GetCrewInfoIdx(data);
                FireData();
            }));

            _disposable.Add(ctrl.GetChangeTabHandler.Subscribe(idx =>
            {
                DataMgr._data.CurTabPage = idx;
                FireData();
            }));
            _disposable.Add(ctrl.GetSelectOrbmentHandler.Subscribe(data =>
            {
                DataMgr._data.SelectOrbment = data;
                if (data.GetTab == TabEnum.Strength)
                {
                    DataMgr._data.IsBagGroup = true;
                    DataMgr._data.CurQuartzPos = -1;
                    var index = BackpackDataMgr.DataMgr.GetQuartzItems()
                            .FindElementIdx(d => d.bagId == data.GetBagId && d.itemId == data.GetItemId);

                    DataMgr._data.CurBagPos = index == -1 ? 0 : index;
                }
                    
            }));

            #region Info
            _disposable.Add(ctrl.InfoCtrl.GetCellClick.Subscribe(idx =>
            {
                DataMgr._data.CurBagPos = -1;
                DataMgr._data.CurQuartzPos = idx;
            }));
            #endregion

            #region strength
            _disposable.Add(ctrl.StrengthCrtl.GetCellClick.Subscribe(idx =>
            {
                DataMgr._data.CurBagPos = idx;
                DataMgr._data.CurQuartzPos = -1;
            }));
            _disposable.Add(ctrl.StrengthCrtl.GetQuartzCellClick.Subscribe(idx =>
            {
                DataMgr._data.CurBagPos = -1;
                DataMgr._data.CurQuartzPos = idx;
            }));
            _disposable.Add(ctrl.StrengthCrtl.GetChangeGroup.Subscribe(b =>
            {
                DataMgr._data.IsBagGroup = b;
                DataMgr._data.CurBagPos = b ? 0 : -1;
                DataMgr._data.CurQuartzPos = b ? -1 : 1;    
                FireData();
            }));
            _disposable.Add(ctrl.StrengthCrtl.GetSelectOrbmentHandler.Subscribe(data =>
            {
                DataMgr._data.SelectOrbment = null; //清空缓存
            }));

            if (tab != TabEnum.Info)
            {
                DataMgr._data.CurTabPage = tab;
                FireData();
            }
            #endregion

            #region forge
            _disposable.Add(ctrl.InitForgeHandler.Subscribe(_ => { InitForgeCtrl(ctrl); }));
            
            #endregion
        }

        private static void InitForgeCtrl(IQuartzViewController ctrl)
        {
            _disposable.Add(ctrl.ForgeCtrl.CommonBtnHandler.Subscribe(grade =>
            {
                QuartzNetMsg.Quartz_Smith(grade, false);
            }));
            _disposable.Add(ctrl.ForgeCtrl.StrengthHandler.Subscribe(grade =>
            {
                QuartzNetMsg.Quartz_Smith(grade, false);
            }));
        }

        private static void Dispose()
        {
            _disposable = _disposable.CloseOnceNull();
            DataMgr._data.CurQuartzPos = 1;
            DataMgr._data.SelectOrbment = null;
            OnDispose();    
        }
        
        // 如果有自定义的内容需要清理，在此实现
        private static void OnDispose()
        {
            
        }
        private static void CloseBtn_UIButtonClick()
        {
            ProxyQuartz.CloseQuartzMainView();
        }
    }
}

