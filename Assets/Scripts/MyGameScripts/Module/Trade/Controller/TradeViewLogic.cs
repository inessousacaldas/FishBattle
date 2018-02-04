// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : CL-PC007
// Created  : 7/1/2017 10:17:33 AM
// **********************************************************************

using AppDto;
using UniRx;
using UnityEngine;

public sealed partial class TradeDataMgr
{
    
    public static partial class TradeViewLogic
    {
        private static CompositeDisposable _disposable;
        private static bool _canRefresh = false;
        public static void Open()
        {
        // open的参数根据需求自己调整
	    var layer = UILayerType.DefaultModule;
            var ctrl = TradeViewController.Show<TradeViewController>(
                TradeView.NAME
                , layer
                , true
                , true
                , Stream);
            DataMgr._data.CurTab = TradeTab.Cmomerce;
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(ITradeViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => ProxyTrade.CloseTradeView()));
            _disposable.Add(ctrl.GetTabMgr.Stream.Subscribe(page =>
            {
                DataMgr._data.CurTab = (TradeTab) page;
                ctrl.UpdateTabPage((TradeTab)page);
                if ((TradeTab) page == TradeTab.Cmomerce)
                {
                    TradeNetMsg.StallExit();
                    TradeNetMsg.OpenTradeView(null);
                }
                if ((TradeTab) page == TradeTab.Pitch)
                {
                    TradeNetMsg.TradeExit();
                    TradeNetMsg.OpenPitchView(() =>
                    {
                        RefreshStellLb(ctrl.GetPitchCtrl);
                    });
                }
                TradeNetMsg.StallMenu(2000);    //请求第一项的数据
            }));

            #region 摆摊

            _disposable.Add(ctrl.GetPitchCtrl.OnAddCashBtn_UIButtonClick.Subscribe(_=>OnAddCashBtnClick()));
            _disposable.Add(ctrl.GetPitchCtrl.GetBuyGoodHandler.Subscribe(dto => BuyPitchItemClick(dto)));
            _disposable.Add(ctrl.GetPitchCtrl.OnLastBtn_UIButtonClick.Subscribe(_=>ctrl.GetPitchCtrl.ChangeItemListPage(0)));
            _disposable.Add(ctrl.GetPitchCtrl.OnNextBtn_UIButtonClick.Subscribe(_=>ctrl.GetPitchCtrl.ChangeItemListPage(1)));
            _disposable.Add(ctrl.GetPitchCtrl.OnOneKeyGetCashBtn_UIButtonClick.Subscribe(_=>OneKeyGetCashClick()));
            _disposable.Add(ctrl.GetPitchCtrl.OnOneKeySellBtn_UIButtonClick.Subscribe(_=>OneKeySellClick()));
            _disposable.Add(ctrl.GetPitchCtrl.GetLockClick.Subscribe(idx => OnLockBtnClick()));
            _disposable.Add(ctrl.GetPitchCtrl.OnSellTipBtn_UIButtonClick.Subscribe(_ => ShowSellTip()));
            _disposable.Add(ctrl.GetPitchCtrl.GetPutawayClick.Subscribe(id => { TradeNetMsg.StallOneItemCash(id);}));
            _disposable.Add(ctrl.GetPitchCtrl.RefreshHandler.Subscribe(menu =>
            {
                DataMgr._data.CurStellMenu = menu;
                ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.GOLD, 500, () =>
                {
                    TradeNetMsg.StallRefresh(menu.id);
                });

            }));
            _disposable.Add(ctrl.GetPitchCtrl.GetRefreshMenuHander.Subscribe(menuId =>
            {
                TradeNetMsg.StallMenu(menuId);
            }));
            _disposable.Add(ctrl.GetPitchCtrl.CDTimeHandler.Subscribe(menuId =>
            {
                TradeNetMsg.StallRefresh(menuId);
            }));
            _disposable.Add(ctrl.GetPitchCtrl.GetRefreshCDTimeHandler.Subscribe(_ => { RefreshStellLb(ctrl.GetPitchCtrl); }));

            #endregion
            #region 商会
            _disposable.Add(ctrl.GetCmomerceCtrl.GetOnBuyHandler.Subscribe(buyData => { OnBuyCmomerceItem(buyData); }));
            _disposable.Add(ctrl.GetCmomerceCtrl.GetSellHandler.Subscribe(buyData => { OnSellCmomerceItem(buyData); }));
            #endregion
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

        #region 摆摊

        private static void OnAddCashBtnClick()
        {
            
        }

        private static void BuyPitchItemClick(IPitchItemData itemdata)
        {
            if (ModelManager.Player.GetPlayerWealthSilver() < itemdata.GetDto.price * itemdata.GetNum)
                ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.SILVER, itemdata.GetDto.price, () =>
                {
                    TradeNetMsg.BuyPitchItem(itemdata);
                });
            else
                TradeNetMsg.BuyPitchItem(itemdata);
        }

        private static void OneKeyGetCashClick()
        {
            TradeNetMsg.StallCash();
        }

        private static void OneKeySellClick()
        {
            TradeNetMsg.StallReupAll();
        }

        private static void OnLockBtnClick()
        {
            TradeHelper.OpenLockPitchitem(DataMgr._data.Capability, () => { TradeNetMsg.OpenLockPitchitem(); });
        }

        private static void ShowSellTip()
        {
            ProxyTips.OpenTextTips(23, new Vector3(300, 164, 0));
        }

        private static void RefreshStellLb(IPitchViewController ctrl)
        {
            var time = DataMgr._data.PitchCDTime;
            JSTimer.Instance.SetupCoolDown("StellView", time, e =>
            {
                DataMgr._data.PitchCDTime -= 1;
                ctrl.UpdateRefreshLb(DataMgr._data.PitchCDTime);
            }, () =>
            {
                DataMgr._data.PitchCDTime = 0;
                ctrl.UpdateRefreshLb(0);
                _canRefresh = true;
                var menuId = DataMgr._data.CurStellMenu.id;
                if(DataMgr._data.CurTab == TradeTab.Pitch && 
                    ctrl.GetCurTab == PitchViewController.PageType.BuyPage)
                    TradeNetMsg.StallRefresh(menuId);
                JSTimer.Instance.CancelCd("StellView");
            }, 1f);
        }

        #endregion

        #region 商会

        private static void OnBuyCmomerceItem(ICmomerItemData itemData)
        {
            if (itemData.GetItemId == 0)
            {
                TipManager.AddTip("请选择物品");
                return;
            }

            if (itemData.GetItemNum == 0)
            {
                TipManager.AddTip("请选择数量");
                return;
            }
            TradeNetMsg.TradeBuy(itemData.GetItemId, itemData.GetItemNum, itemData.GetPrice);
        }

        private static void OnSellCmomerceItem(ICmomerItemData itemData)
        {
            if (itemData.GetItemNum == 0)
            {
                TipManager.AddTip("请选择数量");
                return;
            }
            TradeNetMsg.TradeSell(itemData.GetItemId, itemData.GetItemNum);
        }
        #endregion
    }
}

