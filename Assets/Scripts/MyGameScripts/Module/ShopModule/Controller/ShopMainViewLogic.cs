// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Zijian
// Created  : 8/17/2017 2:13:45 PM
// **********************************************************************

using UniRx;
using System.Linq;
using Debug = GameDebuger;
using System;
using AppDto;

public sealed partial class ShopDataMgr
{
    
    public static partial class ShopMainViewLogic
    {
        private static CompositeDisposable _disposable;
        /// <summary>
        /// 打开游戏商城
        /// </summary>
        public static void Open(ShopTypeTab tab, 
            ShopTypeTab shopId = ShopTypeTab.LimitShopId, 
            int selectShopGoodId = -1)
        {
            ShopNetMsg.ReqGetAllShopInfo(tab, shopId, ()=> 
            {
                var ctrl = ShopMainViewController.Show<ShopMainViewController>(
               ShopMainView.NAME
               , UILayerType.DefaultModule
               , true
               , true
               , Stream);
                InitReactiveEvents(ctrl);

                ctrl.SetShopTab(tab);
                if (selectShopGoodId >= 0)
                {
                    DataMgr._data.SelectGoodsId = selectShopGoodId;
                    SelectShopItem(selectShopGoodId);
                }
            });
        } 

        /// <summary>
        /// 打开系统商城
        /// </summary>
        /// <param name="ShopType">系统商城的Type</param>
        /// <param name="selectShopGoodId"> 打开后默认选中的物品 </param>
        /// <param name="selectCount">打开后默认选中的数量 </param>
        public static void OpenSystemShop(int ShopType, int selectShopGoodId = -1, int selectCount = 1)
        {
            var ctrl = ShopMainViewController.Show<ShopMainViewController>(
               ShopMainView.NAME
               , UILayerType.DefaultModule
               , true
               , true
               , Stream);

            DataMgr._data.UpdateSystemShopData(ShopType, selectShopGoodId, selectCount);
            InitReactiveEvents(ctrl);
        }
        private static void InitReactiveEvents(IShopMainViewController ctrl)
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
            _disposable.Add(ctrl.ShopTabMgr.Stream.Subscribe(i =>
            {
                DataMgr._data.CurSelectCount = 0;
                DataMgr._data.SelectGoodsId = -1;
                int shopType = ShopData.tabInfoList[i].EnumValue;
                DataMgr._data.SetCurShopTab(shopType);
                DataMgr._data.SetCurShopId(DataMgr._data.ShopTypeIdMap[shopType][0].EnumValue);
                ctrl.UpdateScrollViewPos(DataMgr._data.CurShopInfo.ShopItems);
                if (DataMgr._data.CurShopInfo != null && DataMgr._data.CurShopInfo.ShopItems.Count() > 0)
                    DataMgr._data.SetCurSelectShopVo(DataMgr._data.CurShopInfo.ShopItems.First());
                else
                    DataMgr._data.SetCurSelectShopVo(null);
                
                try
                {
                    FireData();
                }catch(Exception e)
                {
                    GameDebuger.Log(e.Message + e.StackTrace);
                }
            }));

            _disposable.Add(ctrl.OnShopIdTabStream.Subscribe(i =>
            {
                DataMgr._data.SelectGoodsId = -1;
                int res = DataMgr._data.ShopTypeIdMap[(int)DataMgr._data.CurShopTypeTab][i].EnumValue;
                DataMgr._data.SetCurShopId(res);
                ctrl.UpdateScrollViewPos(DataMgr._data.CurShopInfo.ShopItems);
                if (DataMgr._data.CurShopInfo !=null && DataMgr._data.CurShopInfo.ShopItems.Count() > 0)
                {
                    DataMgr._data.SetCurSelectShopVo(DataMgr._data.CurShopInfo.ShopItems.First());
                }
                else{
                    DataMgr._data.SetCurSelectShopVo(null);
                }
                try
                {
                    FireData();
                }
                catch (Exception e)
                {
                    GameDebuger.Log(e.Message + e.StackTrace);
                }
            }));

            _disposable.Add(ctrl.OnClickItemStream.Subscribe(item =>
            {
                if(item.clickType== ShopItemViewController.ClickType.ClickItem)
                {
                    DataMgr._data.SetCurSelectShopVo(item.vo);
                    FireData();
                }          
            }));

            _disposable.Add(ctrl.SelectCountStream.Subscribe(i => 
            {
                DataMgr._data.CurSelectCount = i;
                FireData();
            }));

            _disposable.Add(ctrl.OnBuyButton_UIButtonClick.Subscribe(_ => {
                var data = DataMgr._data;
                //1.获取消费金币
                //2.获取消费总额
                var ownerMoney = data.OwenerMoney;
                var totalPrice = data.CurTotalPrice;

                //================ 判断背包的位置是否足够=====
                if (BackpackDataMgr.DataMgr.IsBagFull())
                {
                    TipManager.AddTopTip("背包已满，无法购买");
                    return;
                }
                if(data.CurSelectShopVo.RemainNumber == 0)
                {
                    TipManager.AddTopTip("该物品限购数量为0,下次再来哦~");
                    return;
                }
                if (data.CurSelectCount == 0)
                {
                    TipManager.AddTopTip("购买数量不能为0");
                    return;
                }

                if (ownerMoney >= totalPrice)
                    ShopNetMsg.ReqBuyShop(data.CurShopInfo, data.CurSelectShopVo.ShopGood, data.CurSelectCount);
                else
                {
                    //TipManager.AddTip(string.Format("{0}不足,无法购买", GetMoneyName(data.CurSelectShopVo.ExpendItemId)));
                    ExChangeHelper.CheckIsNeedExchange((AppVirtualItem.VirtualItemEnum)data.CurSelectShopVo.ExpendItemId, totalPrice, () => {
                        ShopNetMsg.ReqBuyShop(data.CurShopInfo, data.CurSelectShopVo.ShopGood, data.CurSelectCount);
                    });
                }  
            }));

            _disposable.Add( ctrl.OnResetButton_UIButtonClick.Subscribe(_ => {
                ShopNetMsg.ReqShopReset(DataMgr._data.CurShopInfo.ShopId);
            }));

            //监听玩家自身的财富
            _disposable.Add(PlayerModel.Stream.SubscribeAndFire(x=> {
                Debug.Log("Shop 刷新财富");
                DataMgr._data.SetCurPlayerModel(x);
                FireData();
            }));
            DataMgr._data.SetCurPlayerModel(ModelManager.IPlayer);

            JSTimer.Instance.SetupTimer("ShopRemainTimer",()=> {
                if (DataMgr._data.CurShopInfo == null )
                    return;
                if (DataMgr._data.CurShopInfo.ShopInfo.resetRule == (int)AppDto.Shop.ResetRule.None)
                    return;
                if(DataMgr._data.CurShopInfo.RemainTime <= 0 )
                {
                    Debug.Log("发送协议商城刷新....");
                    ShopNetMsg.ReqGetShopInfo(DataMgr._data.CurShopInfo.ShopInfo.id);
                }
                else
                {
                    ctrl.UpdatePlayerInfo(DataMgr._data);
                }
            },1);
            FireData();
        }
        private static void Dispose()
        {
            _disposable = _disposable.CloseOnceNull();
            DataMgr._data.SelectGoodsId = -1;
            JSTimer.Instance.CancelTimer("ShopRemainTimer");
            OnDispose();
            Debug.Log("Shop Dispose");
        }
        
        // 如果有自定义的内容需要清理，在此实现
        private static void OnDispose()
        {
            
        }
        private static void CloseBtn_UIButtonClick()
        {
            UIModuleManager.Instance.CloseModule(ShopMainView.NAME);
        }

        private static void SelectShopItem(int goodsId)
        {
            var goods = DataCache.getDtoByCls<ShopGoods>(goodsId);
            var itemVo = new ShopItemVo(goods);
            DataMgr._data.SetCurSelectShopVo(itemVo);

            var shop = DataCache.getDtoByCls<Shop>(goods.shopId);
            DataMgr._data.SetCurShopTab(shop.shopType);
            DataMgr._data.SetCurShopId(shop.id);
            FireData();
        }
    }
}

