// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Zijian
// Created  : 8/24/2017 2:54:17 PM
// **********************************************************************

using System;
using UniRx;
using AppDto;

public sealed partial class ExChangeMainDataMgr
{
    
    public static partial class ExChangeMainViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open(int expectId)
        {
        // open的参数根据需求自己调整
            var ctrl = ExChangeMainViewController.Show<ExChangeMainViewController>(
                ExChangeMainView.NAME
                , UILayerType.FiveModule
                , true
                , false
                , Stream);

            //兑换货币种类
            DataMgr._data.SetExpectId(expectId);
            ctrl.InitDataBefore(expectId);

            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IExChangeMainViewController ctrl)
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
            _disposable.Add(ctrl.OnExchangeButton_UIButtonClick.Subscribe(_ => OnExChangeButtonClick()));
            _disposable.Add(ctrl.OnTipsButton_UIButtonClick.Subscribe(_ => OnTipsButtonClick()));
            _disposable.Add(PlayerModel.Stream.Subscribe(_ => { FireData(); }));

            if (ctrl.TabMgr != null)
            _disposable.Add(ctrl.TabMgr.Stream.Subscribe(i=> {
                DataMgr._data.SetCurTab((ExChangeUseTabType)i);
                FireData();
            }));
            _disposable.Add(ctrl.OnSelectCountStream.Subscribe(i => {
                DataMgr._data.SetSelectCount(i);
                FireData();
            }));

            //=======默认优先使用米拉======
            if (DataMgr._data.ExchangeId == (int)AppVirtualItem.VirtualItemEnum.MIRA)
            {
                DataMgr._data.SetCurTab(ExChangeUseTabType.UserOnlyDiamond);
            }
            else
            {
                DataMgr._data.SetCurTab(ExChangeUseTabType.UseDiamondAndBindDiamond);
            }
            DataMgr._data.SetSelectCount(1);
        }

        private static void OnExChangeButtonClick()
        {
            long owernMoney = DataMgr._data.GetWealth(DataMgr._data.CurTab);
            if (DataMgr._data.CurSelectCount > owernMoney)
            {
                //如果是钻石则弹出充值界面 todo xjd
                if (DataMgr._data.CurTab == ExChangeUseTabType.UseMira && DataMgr.GetRemainMiraCount() <= 0)
                {
                    TipManager.AddTip("兑换数量已达上限");
                    return;
                }
                    
                TipManager.AddTopTip("货币不足");
            }
            else
                ExChangeMainNetMsg.ReqCurrencyConvert(DataMgr._data.CurUseExChangeId, DataMgr._data.ExchangeId, DataMgr._data.CurSelectCount, false);
        }

        private static void OnTipsButtonClick()
        {
            ProxyTips.OpenTextTips(16, new UnityEngine.Vector3(-85, -133), true);
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
        private static void CloseBtn_UIButtonClick()
        {
            UIModuleManager.Instance.CloseModule(ExChangeMainView.NAME);
        }
    }
}

