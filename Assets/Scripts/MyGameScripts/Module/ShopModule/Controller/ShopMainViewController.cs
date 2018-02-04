// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ShopMainViewController.cs
// Author   : Zijian
// Created  : 8/17/2017 2:13:45 PM
// Porpuse  : 
// **********************************************************************

using AppDto;
using AssetPipeline;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Debug = GameDebuger;

public partial interface IShopMainViewController
{
    TabbtnManager ShopTabMgr { get; }
    UniRx.IObservable<ShopItemViewController.ShopItemClickEvt> OnClickItemStream { get; }
    UniRx.IObservable<int> OnShopIdTabStream { get; }
    UniRx.IObservable<int> SelectCountStream { get; }
    void UpdatePlayerInfo(IShopData data);

    void UpdateScrollViewPos(IEnumerable<ShopItemVo> ShopItems);
    void SetShopTab(ShopTypeTab tab);
}
public partial class ShopMainViewController {

    TabbtnManager shopTypeTabMgr;
    public TabbtnManager ShopTabMgr
    {
        get { return shopTypeTabMgr; }
    }
    TabbtnManager shopIdTabMgr;
    public TabbtnManager ShopIdTabMgr { get { return shopIdTabMgr; } }

    private List<AppVirtualItem> _virtualItemList = new List<AppVirtualItem>();

    PageTurnViewController ptvCtrl;
    
    private Dictionary<GameObject, ShopItemViewController> _shopItemDic = new Dictionary<GameObject, ShopItemViewController>();
    private List<ShopItemVo> _shopItemData = new List<ShopItemVo>();
    private int _chooseId = 0;

    Subject<ShopItemViewController.ShopItemClickEvt> clickItemStream = new Subject<ShopItemViewController.ShopItemClickEvt>();
    public UniRx.IObservable<ShopItemViewController.ShopItemClickEvt> OnClickItemStream { get { return clickItemStream; } }

    Subject<int> shopIdTabStream = new Subject<int>();
    public UniRx.IObservable<int> OnShopIdTabStream { get { return shopIdTabStream; } }

    public UniRx.IObservable<int> SelectCountStream { get { return ptvCtrl.Stream; } }
    CompositeDisposable disposable;

    //模型展示
    ModelDisplayController modeldisplayer;

    private bool _firstOpen = true;

    //判断是不是切换1级或2级页面
    private int _shopType = 0;
    private int _shopId = 0;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        CreateTabItem();
        ptvCtrl = AddController<PageTurnViewController, PageTurnView>(View.PageTurnView);
        ptvCtrl.InitData_NumberInputer(0, 1, 100);
        disposable = new CompositeDisposable();

        StartTimer();
    }

    private static readonly string TimerName = "addShopItem";
    private int _curCount = 0;
    private void StartTimer()
    {
        //recyclelist 一共12个children
        if (_curCount == 12)
        {
            JSTimer.Instance.CancelTimer(TimerName + this.GetHashCode());
            UpdateScrollViewPos(_shopItemData);
            return;
        }

        var onceCount = 10;//每帧加载4个
        if (_curCount == 10)
            onceCount = 2;
        var timeGap = 0.016f;
        JSTimer.Instance.CancelTimer(TimerName + this.GetHashCode());
        JSTimer.Instance.SetupCoolDown(TimerName + this.GetHashCode(), timeGap, null, delegate
        {
            for (int i = 0; i < onceCount; i++)
            {
                var ctrl = AddChild<ShopItemViewController, ShopItemView>(View.RecycleList.gameObject, ShopItemView.NAME);
                _shopItemDic.Add(ctrl.gameObject, ctrl);
                ctrl.gameObject.transform.localPosition = new Vector3(700, 0);

                disposable.Add(ctrl.OnClickItemStream.Subscribe(e =>
                {
                    clickItemStream.OnNext(e);
                    if (e.clickType == ShopItemViewController.ClickType.ClickItemIcon)
                        ShowItemTips(ctrl.gameObject, e.vo);
                }));
            }

            _curCount += onceCount;
            StartTimer();
        });
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {
        View.RecycleList.onUpdateItem = UpdateShopItem;
    }

    protected override void RemoveCustomEvent()
    {
        clickItemStream = clickItemStream.CloseOnceNull();
        shopIdTabStream = shopIdTabStream.CloseOnceNull();
    }

    protected override void OnDispose()
    {
        disposable = disposable.CloseOnceNull();
        base.OnDispose();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {
        var list = DataCache.getArrayByCls<GeneralItem>().Filter(d => d is AppVirtualItem);
        list.ForEach(d => { _virtualItemList.Add(d as AppVirtualItem); });
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IShopData data)
    {
        if (data.CurShopInfo == null || _curCount == 0)
            return;

        UpdateShopTypeTabs(data);
        UpdateShopIdTab(data.ShopTypeIdMap[(int)data.CurShopTypeTab], data.CurShopInfo.ShopId);
        UpdateShopItems(data);
        UpdateShopInfo(data);
        if (_firstOpen)
        {
            SetTexture(data.GetCurShopId);
            SetTabState(data.CurShopMainType);
        }
        if (data.SelectGoodsId >= 0)
            SetScrollViewPos(data.SelectGoodsId);
    }

    public void UpdateScrollViewPos(IEnumerable<ShopItemVo> ShopItems)
    {
        View.RecycleList.UpdateDataCount(ShopItems.ToList().Count, true);
        View.ScrollView_UIScrollView.ResetPosition();
        View.ScrollView_UIScrollView.transform.localPosition = new Vector3(80, -18);
    }

    public void SetScrollViewPos(int itemId)
    {
        var idx = _shopItemData.FindElementIdx(d => d.Id == itemId);
        if (idx >= 0)
        {
            var posy = idx / 2 * View.RecycleList.cellHeight - 16;
            Vector3 pos = new Vector3(80, posy, 0);
            SpringPanel.Begin(View.ScrollView_UIScrollView.gameObject, pos, 8f);
        }
    }

    public void SetShopTab(ShopTypeTab tab)
    {
        if (shopTypeTabMgr == null)
            return;

        shopTypeTabMgr.SetTabBtn((int)tab);
    }

    private void CreateTabItem()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
                    View.tabBtn_1
                    , TabbtnPrefabPath.TabBtnWidget.ToString()
                    , "Tabbtn_" + i);

        shopTypeTabMgr = TabbtnManager.Create(ShopDataMgr.ShopData.tabInfoList, func);
    }

    //更新1级分页
    private void UpdateShopTypeTabs(IShopData data)
    {
        shopTypeTabMgr.UpdateTabs(ShopDataMgr.ShopData.tabInfoList, ShopDataMgr.ShopData.tabInfoList.FindIndex(x => x.EnumValue == (int)data.CurShopTypeTab));
        var grid = View.tabBtn_1.GetComponent<UIGrid>();
        if (grid != null)
            grid.Reposition();
    }

    //更新2级分页~
    private void UpdateShopIdTab(List<ITabInfo> tabinfos, int shopidIndex)
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
                    View.tabBtn_2
                    , TabbtnPrefabPath.TabBtnShop_2.ToString()
                    , "Tabbtn_" + i);


        View.tabBtn_2.gameObject.SetActive(true);
        //策划要求，只有一个的时候不显示分页~
        if (tabinfos.Count <= 1)
        {
            View.tabBtn_2.gameObject.SetActive(false);
            return;
        }
            
        if (shopIdTabMgr == null)
        {
            shopIdTabMgr = TabbtnManager.Create(tabinfos, func);
            shopIdTabMgr.Stream.Subscribe(i => { shopIdTabStream.OnNext(i); });
            shopIdTabMgr.SetBtnLblFont(20, "444244FF", 18, "B5BAB5FF");
        }

        shopIdTabMgr.UpdateTabs(tabinfos, tabinfos.FindIndex(x => x.EnumValue == shopidIndex));
        View.tabBtn_2.GetComponent<UIGrid>().Reposition();
    }

    private void UpdateShopItems(IShopData data)
    {
        var shopItems = data.CurShopInfo.ShopItems;
        _shopItemData = shopItems.ToList();
        _chooseId = data.CurSelectShopVo.Id;        
        if (_shopType != data.CurShopTypeTab || _shopId != data.CurShopInfo.ShopId)
            UpdateScrollViewPos(data.CurShopInfo.ShopItems);
        else
            View.RecycleList.UpdateChildrenData();

        _shopType = data.CurShopTypeTab;
        _shopId = data.CurShopInfo.ShopId;
    }

    private void UpdateShopItem(GameObject go, int itemIndex, int dataIndex)
    {
        if (_shopItemDic == null) return;
        ShopItemViewController item = null;
        if(_shopItemDic.TryGetValue(go, out item))
        {
            var info = _shopItemData.TryGetValue(dataIndex);
            if(info == null) return;
            bool isChoice = _chooseId == info.Id;
            item.UpdateView(info, isChoice);
        }
    }

    private void ShowItemTips(GameObject gameobject, ShopItemVo vo)
    {
        //var ctrl = ShopItemTipsViewController.Show<ShopItemTipsViewController>(ShopItemTipsView.NAME, UILayerType.SubModule, false);
        //ctrl.ShowInit(anchor, vo.Name, vo.Icon, vo.Des);
        //todo 写的比较草率 
        var scrollviewPosY = View.ScrollView_UIScrollView.panel.clipOffset.y;
        var top = 141;
        var leftX = -148;
        var rightX = 160;
        var pos = gameobject.transform.localPosition;
        var tipsPos = new Vector3(pos.x > 200 ? rightX : leftX, top + pos.y - scrollviewPosY);
        var ctrl = ProxyTips.OpenGeneralItemTips(ItemHelper.GetGeneralItemByItemId(vo.ShopGood.itemId));
        ctrl.SetTipsPosition(tipsPos);
    }

    private void UpdateShopInfo(IShopData data)
    {
        if (data.CurSelectRemainCount < 0)
        {
            View.ShopGoodsRemainCount_UILabel.gameObject.SetActive(false);
            ptvCtrl.SetPageInfo(data.CurSelectCount,9999);
        }
        else
        {
            View.ShopGoodsRemainCount_UILabel.gameObject.SetActive(true);
            View.ShopGoodsRemainCount_UILabel.text = string.Format("本次还可以购买 {0} 个", data.CurSelectRemainCount);
            ptvCtrl.SetPageInfo(data.CurSelectCount, data.CurSelectRemainCount);
        }

        UpdatePlayerInfo(data);

        //TODO: 如果自身货币不足，字体颜色为红色
        var ownerMoney = data.OwenerMoney;
        var totalPrice = data.CurTotalPrice.ToString();
        if (data.CurTotalPrice > ownerMoney)
            totalPrice = string.Format("[FF0000]{0}", totalPrice);
        else
            totalPrice = string.Format("[FFFFFF]{0}", totalPrice);

        //==================TODO:商品价格的图标========================
        string priceIconstr = ItemIconConst.GetIconConstByItemId((AppVirtualItem.VirtualItemEnum)data.CurSelectShopVo.ExpendItemId);
        UIHelper.SetAppVirtualItemIcon(View.PriceIcon_UISprite, (AppVirtualItem.VirtualItemEnum)data.CurSelectShopVo.ShopGood.expendItemId);
        View.PriceLabel_UILabel.text = totalPrice;
        SetOwnerMoney((AppVirtualItem.VirtualItemEnum)data.CurSelectShopVo.ShopGood.expendItemId);
        //===========限购不显示自身拥有多少货币
        if (data.CurShopInfo.ResetRule == Shop.ResetRule.None 
            || data.CurShopInfo.ResetItemCount == 0)
        {
            View.ResetButton_UIButton.gameObject.SetActive(false);
        }
        else
        {
            View.ResetButton_UIButton.gameObject.SetActive(true);
            View.resetPrice_UILabel.text = string.Format("(消耗{0}{1})", data.CurShopInfo.ResetItemCount,
                GetVirtualItemName(data.CurShopInfo.ShopInfo.resetItemId));
            //UIHelper.SetAppVirtualItemIcon(View.restPriceIcon_UISprite, (AppVirtualItem.VirtualItemEnum)data.CurShopInfo.ShopInfo.resetItemId);
        }
    }

    private void SetOwnerMoney(AppVirtualItem.VirtualItemEnum money)
    {
        if (money == AppVirtualItem.VirtualItemEnum.GOLD ||
            money == AppVirtualItem.VirtualItemEnum.SILVER ||
            money == AppVirtualItem.VirtualItemEnum.DIAMOND)
            _view.OwnerMoney.SetActive(false);
        else
        {
            _view.OwnerMoney.SetActive(true);
            UIHelper.SetAppVirtualItemIcon(_view.OwnerMoneyIcon_UISprite, money);
            _view.OwnerMoneyLabel_UILabel.text = ModelManager.Player.GetPlayerWealthById((int) money).ToString();
        }
    }

    private string GetVirtualItemName(int id)
    {
        var v = _virtualItemList.Find(d => d.id == id);
        return v == null ? "" : v.name;
    }

    private void SetTexture(int shopId)
    {
        var shop = DataCache.getDtoByCls<Shop>(shopId);
        if (shop == null)
        {
            TipManager.AddTip(string.Format("shop表不存在{0},请检查",shopId));
            return;
        }
        UIHelper.SetUITexture(_view.PlayerTexture_UITexture, shop.shopRes);
        _view.TitleNameSprite_UISprite.spriteName = shop.res;
        _view.TitleNameSprite_UISprite.MakePixelPerfect();
    }

    private void SetTabState(ShopMainType type)
    {
        _view.TabBg.SetActive(type != ShopMainType.SystemShop);
        _view.tabBtn_1.SetActive(type != ShopMainType.SystemShop);
    }

    private int _cdTime;
    public void UpdatePlayerInfo(IShopData data)
    {
        //商品剩余时间
        if (data.CurShopInfo.ResetRule != (int)AppDto.Shop.ResetRule.None)
        {
            if (data.CurShopInfo.RemainTime > 0)
            {
                View.ShopRemainTime_UILabel.gameObject.SetActive(true);
                View.ShopRemainTime_UILabel.text = string.Format("剩余时间: {0}", data.CurShopInfo.FormatRemainTime);
            }
            else
                View.ShopRemainTime_UILabel.gameObject.SetActive(false);    
        }
        else
            View.ShopRemainTime_UILabel.gameObject.SetActive(false);
    }
}
