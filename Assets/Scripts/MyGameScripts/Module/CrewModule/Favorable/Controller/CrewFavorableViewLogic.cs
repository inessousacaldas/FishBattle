// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 9/15/2017 5:08:08 PM
// **********************************************************************

using System.Linq;
using UniRx;

public sealed partial class CrewViewDataMgr
{
    
    public static partial class CrewFavorableViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
	    var layer = UILayerType.DefaultModule;
            var ctrl = CrewFavorableViewController.Show<CrewFavorableViewController>(
                CrewFavorableView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(ICrewFavorableViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnPrefixBtn_UIButtonClick.Subscribe(_ => { TipManager.AddTip("敬请期待"); }));
            _disposable.Add(ctrl.OnDressBtn_UIButtonClick.Subscribe(_ => { TipManager.AddTip("敬请期待"); }));
            _disposable.Add(ctrl.OnBiographyBtn_UIButtonClick.Subscribe(_ => { TipManager.AddTip("敬请期待"); }));
            _disposable.Add(ctrl.OnRewardBtn_UIButtonClick.Subscribe(_ =>
            {
                ctrl.RewardPanelTween(true);
                ctrl.ShowRewardList();
                ctrl.ShowSecondPanel(true);
            }));
            _disposable.Add(ctrl.OnHistoryBtn_UIButtonClick.Subscribe(_ =>
            {
                ctrl.HistoryPanelTween(true);
                ctrl.ShowHistoryPanel();
                ctrl.ShowSecondPanel(true);

            }));
            _disposable.Add(ctrl.OnRecordBtn_UIButtonClick.Subscribe(_ =>
            {
                ctrl.RecordPanelTween(true);
                ctrl.ShowRecordPanel();
                ctrl.ShowSecondPanel(true);

            }));
            _disposable.Add(ctrl.OnNextBtn_UIButtonClick.Subscribe(_ =>
            {
                if (DataMgr._data.GetCurFavorIdx < DataMgr._data.GetSelfCrew().Count() - 1)
                {
                    if (DataMgr._data.GetCurFavorIdx + 1 == DataMgr._data.GetSelfCrew().Count() - 1)
                        ctrl.GetRightPageBtn.gameObject.SetActive(false);
                    else
                        ctrl.GetRightPageBtn.gameObject.SetActive(true);

                    ctrl.GetLeftPageBtn.gameObject.SetActive(true);
                    DataMgr._data.UpdateCurFavorableIdx(DataMgr._data.GetCurFavorIdx + 1);
                    ctrl.UpdateCrewInfo(DataMgr._data.GetSelfCrew().TryGetValue(DataMgr._data.GetCurFavorIdx));
                }
            }));
            _disposable.Add(ctrl.OnLastBtn_UIButtonClick.Subscribe(_ =>
            {
                if (DataMgr._data.GetCurFavorIdx > 0)
                {
                    ctrl.GetRightPageBtn.gameObject.SetActive(true);
                    ctrl.GetLeftPageBtn.gameObject.SetActive(DataMgr._data.GetCurFavorIdx > 1);
                    DataMgr._data.UpdateCurFavorableIdx(DataMgr._data.GetCurFavorIdx - 1);
                    ctrl.UpdateCrewInfo(DataMgr._data.GetSelfCrew().TryGetValue(DataMgr._data.GetCurFavorIdx));
                }
            }));

            _disposable.Add(ctrl.GetModelClick.Subscribe(_ => OnModelClick()));
            _disposable.Add(ctrl.GetRewardItemClick.Subscribe(tid => OnRewardItemClick(tid)));
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => OnCloseBtnClick()));
            _disposable.Add(ctrl.OnTexture_UIButtonClick.Subscribe(_ => OnModelClick()));
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

        private static void OnRewardItemClick(int itemId)
        {
            var crewDto = DataMgr._data.GetSelfCrew().TryGetValue(DataMgr._data.GetCurFavorIdx);
            CrewViewNetMsg.CrewReward(crewDto.id, itemId);
        }

        private static void OnModelClick()
        {
            var crewDto = DataMgr._data.GetSelfCrew().TryGetValue(DataMgr._data.GetCurFavorIdx);
            CrewViewNetMsg.CrewClickModel(crewDto.id);
        }

        private static void OnCloseBtnClick()
        {
            UIModuleManager.Instance.CloseModule(CrewFavorableView.NAME);
        }
    }
}

