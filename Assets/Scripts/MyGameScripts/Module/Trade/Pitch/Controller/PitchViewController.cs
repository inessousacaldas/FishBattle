// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PitchViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AppDto;
using UniRx;
using UnityEngine;

public interface IPitchItemData
{
    StallGoodsDto GetDto { get; }
    int GetNum { get; }
}

public class PitchItemData : IPitchItemData
{
    private StallGoodsDto _dto;
    private int _num;

    public StallGoodsDto GetDto { get { return _dto; } }
    public int GetNum { get { return _num; } }

    public static PitchItemData Create(StallGoodsDto dto, int num)
    {
        PitchItemData data = new PitchItemData();
        data._dto = dto;
        data._num = num;
        return data;
    }
}

public partial interface IPitchViewController
{
    UniRx.IObservable<int> GetLockClick { get; }
    UniRx.IObservable<IPitchItemData> GetBuyGoodHandler { get; }
    UniRx.IObservable<int> GetRefreshMenuHander { get; }
    UniRx.IObservable<long> GetPutawayClick { get;  }
    void ShowBuyGroup(bool b);
    void ChangeItemListPage(int page);
    void UpdateRefreshLb(int s);
    void UpdateDataAndView(ITradeData data);
    UniRx.IObservable<Unit> GetRefreshCDTimeHandler { get; }
    PitchViewController.PageType GetCurTab { get; }
    UniRx.IObservable<int> CDTimeHandler { get; }
    UniRx.IObservable<TradeMenu> RefreshHandler { get; }
    void ChoiseGoods(int goodsId);
}

public partial class PitchViewController
{
    private CompositeDisposable _disposable;

    private int _sellItemMax = DataCache.GetStaticConfigValue(AppStaticConfigs.STALL_MAX_CAPABILITY, 10); //摊位最大数量
    private const int _buyItemMax = 8;
    private int _selectItemCount = 1;       //购买摆摊物品的数量
    private TradeMenu _curFirstMenu;
    private TradeMenu _curSecondMenu;
    private StallGoodsDto _curTradeGoods;      //选中要购买的物品
    private ITradeData _data;
    private List<PitchItemController> _sellItemList = new List<PitchItemController>();
    private List<PitchItemController> _buyItemList = new List<PitchItemController>();
    private List<TradeOptionBtnController> _optionBtnList = new List<TradeOptionBtnController>();   //一级菜单按钮   
    private List<PitchItemController> _secondOptionBtnList = new List<PitchItemController>();     //二级菜单按钮

    private IEnumerable<TradeMenu> _allStellMenuList = new List<TradeMenu>();
    private IEnumerable<TradeMenu> _firstOptionList = new List<TradeMenu>();     //一级菜单
    private IEnumerable<TradeMenu> _secondOptionList = new List<TradeMenu>();   //二级菜单 
    private IEnumerable<StallGoodsDto> _curMenuGoods = new List<StallGoodsDto>();

    private SimpleNumberInputerController _numberInputer;

    private delegate void ItemClickFunc(PitchItemController item, int idx);
    private delegate void OptionFunc(TradeOptionBtnController btn, TradeMenu menu);
    private delegate void SecondOptinFunc(PitchItemController item, TradeMenu menu);

    private IEnumerable<StallGoodsDto> _stallGoods = new List<StallGoodsDto>();
    private List<StallGoods> _allStallGoods = new List<StallGoods>();

    private TabbtnManager tabMgr;
    public static readonly ITabInfo[] TeamTabInfos =
    {
        TabInfoData.Create((int) PageType.BuyPage, "我要购买"),
        TabInfoData.Create((int) PageType.SellPage, "我要出售")
    };

    private Subject<TradeMenu> _refreshEvt = new Subject<TradeMenu>();
    public UniRx.IObservable<TradeMenu> RefreshHandler { get { return _refreshEvt; } } 

    private PageType _curPageType;

    public enum PageType
    {
        BuyPage = 0,
        SellPage = 1
    }

    #region subject
    private Subject<int> _lockClickEvt = new Subject<int>();
    public UniRx.IObservable<int> GetLockClick{get { return _lockClickEvt; }}
    
    private Subject<long> _putawayEvt = new Subject<long>();
    public UniRx.IObservable<long> GetPutawayClick{get { return _putawayEvt; }}

    private Subject<IPitchItemData> _buyGoodClick = new Subject<IPitchItemData>(); 
    public UniRx.IObservable<IPitchItemData> GetBuyGoodHandler { get { return _buyGoodClick; } }
     
    private Subject<int> _refreshMenuEvt = new Subject<int>();
    public UniRx.IObservable<int> GetRefreshMenuHander { get { return _refreshMenuEvt; } }
    private Subject<int> _cdTimeEvt = new Subject<int>();
    public UniRx.IObservable<int> CDTimeHandler { get { return _cdTimeEvt; } }  

    private Subject<Unit> _refreshCDTime = new Subject<Unit>();
    public UniRx.IObservable<Unit> GetRefreshCDTimeHandler { get { return _refreshCDTime; } }  
    #endregion

    #region 筛选等级下拉菜单

    private enum PopupEnum
    {
        One,Two,Three,Four
    }
    private S3PopupListController _teamPopupListCtrl;
    List<PopUpItemInfo> _popupNameList = new List<PopUpItemInfo>()
    {
        new PopUpItemInfo("10-30", (int)PopupEnum.One),
        new PopUpItemInfo("31-50", (int)PopupEnum.Two),
        new PopUpItemInfo("51-70", (int)PopupEnum.Three),
        new PopUpItemInfo("71-90", (int)PopupEnum.Four),
    };
    #endregion

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();

        InitTabBtn();
        InitData();
        InitBuyOptionList();
        ShowBuyGroup(true);
        UpdateCaseLb();
        InitPopupList();
        _view.LastBtn_UIButton.sprite.isGrey = true;
        _view.PopupLb_UILabel.text = string.Format("{0}-{1}", 10, 90);
    }

    private void InitPopupList()
    {
        _teamPopupListCtrl = AddChild<S3PopupListController, S3PopupList>(View.gameObject, S3PopupList.PREFAB_WAREHOUSE);
        _teamPopupListCtrl.InitData(S3PopupItem.PREFAB_TEAMBTN, _popupNameList,
            isClickHide: false, isShowList: true, isShowBg: false);
        _teamPopupListCtrl.Hide();
        _teamPopupListCtrl.OnClickOtherEvt += _teamPopupListCtrl_OnClickOtherEvt;
        _teamPopupListCtrl.OnChoiceIndexStream.Subscribe(item =>
        {
            switch ((PopupEnum)item.EnumValue)
            {
                case PopupEnum.One:
                    OnPupupListBtnClick(10, 30);
                    break;
                case PopupEnum.Two:
                    OnPupupListBtnClick(31, 50);
                    break;
                case PopupEnum.Three:
                    OnPupupListBtnClick(51, 70);
                    break;
                case PopupEnum.Four:
                    OnPupupListBtnClick(71, 90);
                    break;
            }
        });
    }

    private void _teamPopupListCtrl_OnClickOtherEvt()
    {
        _teamPopupListCtrl.Hide();
    }

    private void InitTabBtn()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
            _view.TabGrid_UIGrid.gameObject
            , TabbtnPrefabPath.TabBtnWidget_S3.ToString()
            , "Tabbtn_" + i);

        tabMgr = TabbtnManager.Create(TeamTabInfos, func);
        tabMgr.SetBtnLblFont(20, "2d2d2d", 18, ColorConstantV3.Color_VerticalUnSelectColor2_Str);

        _disposable.Add(tabMgr.Stream.Subscribe(pageIndex =>
        {
            ShowBuyGroup(pageIndex == (int)PageType.BuyPage);
            if (pageIndex == (int) PageType.SellPage && _sellItemList.Count == 0)
            {
                InitSellItemList();
                UpdateSellItemList(_data.PitchCtrl);
            }
            _curPageType = (PageType) pageIndex;
        }));
    }

    private void InitData()
    {
        _allStellMenuList = DataCache.getArrayByCls<TradeMenu>();
        _firstOptionList = _allStellMenuList.Filter(d => d.type == (int) TradeMenu.TradeMenuEnum.Stall && d.parentId == 0);
        _allStallGoods = DataCache.getArrayByCls<StallGoods>();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        _disposable.Add(OnBuyItemBtn_UIButtonClick.Subscribe(_ =>
        {
            if (_curTradeGoods == null)
            {
                TipManager.AddTip("请选择物品");
                return;
            }
            var price = _curTradeGoods.price*_selectItemCount;
            var data = PitchItemData.Create(_curTradeGoods, _selectItemCount);
            if (ModelManager.Player.GetPlayerWealthSilver() < price)
            {
                ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.SILVER, price, () =>
                {
                    _buyGoodClick.OnNext(data);
                });
            }
            else
                _buyGoodClick.OnNext(data);

        }));
        _disposable.Add(_view.PopupBtn_UIButton.AsObservable().Subscribe(_ => { OnPopupBtnClick(); }));
        _disposable.Add(OnAddCountBtn_UIButtonClick.Subscribe(_ => { UpdateCount(true); }));
        _disposable.Add(OnReduceCountBtn_UIButtonClick.Subscribe(_ => { UpdateCount(false); }));
        _disposable.Add(OnChoiseCount_UIButtonClick.Subscribe(_ => { OpenSimpleNumber(); }));
        _disposable.Add(OnAddCashBtn_UIButtonClick.Subscribe(_ => { AddCashBtnClick(); }));
        _disposable.Add(OnRefreshBtn_UIButtonClick.Subscribe(_ => { _refreshEvt.OnNext(_curFirstMenu);}));
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        _teamPopupListCtrl.OnClickOtherEvt -= _teamPopupListCtrl_OnClickOtherEvt;
        _teamPopupListCtrl = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private void InitSellItemList()
    {
        ItemClickFunc func = (item, idx) =>
        {
            _disposable.Add(item.OnAddBtn_UIButtonClick.Subscribe(_ => OpenPitchSellView()));
            _disposable.Add(item.OnLockBtn_UIButtonClick.Subscribe(_ => _lockClickEvt.OnNext(idx)));
            _disposable.Add(item.OnPitchItem_UIButtonClick.Subscribe(_=>OnPitchItemClick(idx)));
        };
        
        for (int i = 0; i < _sellItemMax; i++)
        {
            var item = AddChild<PitchItemController, PitchItem>(_view.SellItemGrid_UIGrid.gameObject, PitchItem.NAME);
            _sellItemList.Add(item);
            func(item, i);
            item.UpdateItemNull();
            item.SetItemSize(265);  //设置item大小
            item.SetLockBoxColliderPos(111, 131);
        }
        _view.SellItemGrid_UIGrid.Reposition();
    }

    private void InitBuyItemList()
    {
        ItemClickFunc itemfunc = (item, idx) =>
        {
            _disposable.Add(item.OnPitchItem_UIButtonClick.Subscribe(_ =>
            {
                OnPitchBuyItemClick(idx);
            }));
        };

        for (int i = 0; i < _buyItemMax; i++)
        {
            var item = AddChild<PitchItemController, PitchItem>(_view.BuyGoodsGrid_UIGrid.gameObject, PitchItem.NAME);
            _buyItemList.Add(item);
            itemfunc(item, i);
            item.UpdateItemNull();
            item.SetItemSize(310);
        }
        _view.BuyGoodsGrid_UIGrid.Reposition();
        //_view.BuyScrollView_UIScrollView.ResetPosition();
    }

    private void OnPitchBuyItemClick(int idx)
    {
        _curTradeGoods = _curMenuGoods.TryGetValue(idx);
        if (_curTradeGoods == null)
            return;

        _buyItemList.ForEachI((go, index) => { go.IsSelect(index == idx); });
        _selectItemCount = 1;
        _view.CountLb_UILabel.text = "1";
        ShowSelectCount(_curTradeGoods.amount > 1);
    }

    //左侧菜单
    private void InitBuyOptionList()
    {
        OptionFunc func = (btn, menu) =>
        {
            _disposable.Add(btn.GetClickHandler.Subscribe(i =>
            {
                OnFirstOptionClick(btn, menu);
            }));
        };

        _firstOptionList.ForEach(dto =>
        {
            var button = AddChild<TradeOptionBtnController, TradeOptionBtn>
                    (_view.OptionGrid_UIGrid.gameObject, TradeOptionBtn.NAME);

            _optionBtnList.Add(button);
            button.SetTradeMenuBtn(dto);
            button.HideArrow();
            func(button, dto);
        });

        _optionBtnList[0].IsSelect(true);
        _view.OptionGrid_UIGrid.Reposition();
        UpdateCurMenu(_firstOptionList.TryGetValue(0));
        IsSelectGoodsMenu(false);
    }

    private void OnFirstOptionClick(TradeOptionBtnController btn, TradeMenu menu)
    {
        UpdateCurMenu(menu);
        _curTradeGoods = null;
        IsSelectGoodsMenu(false);
        _optionBtnList.ForEach(button =>
        {
            button.IsSelect(btn == button);
        });
        ShowSelectCount(false);
        _view.BuyScrollView_UIScrollView.ResetPosition();
    }

    //显示某个一级菜单的子类
    private void UpdateCurMenu(TradeMenu menu)
    {
        _curFirstMenu = menu;
        _refreshMenuEvt.OnNext(menu.id);
        _secondOptionList = _allStellMenuList.Filter(d => d.parentId == menu.id);
        
        InitSecondMenuBtnList(_secondOptionList);
        _view.DescLb_UILabel.text = string.Format("请选择{0}商品",
                    menu.name.WrapColor(ColorConstantV3.Color_Green_Str));
        _buyItemList.ForEach(go=>go.IsSelect(false));
    }

    private void InitSecondMenuBtnList(IEnumerable<TradeMenu> menuList)
    {
        SecondOptinFunc func = (item, menu) =>
        {
            _disposable.Add(item.GetItemClick.Subscribe(m =>
            {
                OnSecondOptionClick(m);
            }));
            _curTradeGoods = _curMenuGoods.TryGetValue(0); //选种类后默认选中第一个

            if (_curMenuGoods.Count() <= 8)
                _view.PageLb_UILabel.text = "1/1";
            else
                _view.PageLb_UILabel.text = "1/2";
        };

        _secondOptionBtnList.ForEachI((item, idx) => { item.gameObject.SetActive(idx < menuList.Count()); });
        
        menuList.ForEachI((menu, idx) =>
        {
            var goods = _allStallGoods.Find(d => d.tradeMenuId == menu.id);
            if (idx >= _secondOptionBtnList.Count)
            {
                var btn = AddChild<PitchItemController, PitchItem>(_view.MenuGtid_UIGrid.gameObject, PitchItem.NAME);
                _secondOptionBtnList.Add(btn);
                func(btn, menu);

                btn.SetMenuInfo(menu, goods == null ? -1 : goods.id);
                btn.SetItemSize(310);
            }
            else
                _secondOptionBtnList[idx].SetMenuInfo(menu, goods == null ? -1 : goods.id);
        });

        _view.MenuGtid_UIGrid.Reposition();
        //_view.BuyScrollView_UIScrollView.ResetPosition();
    }

    private void OnSecondOptionClick(TradeMenu m)
    {
        if(_buyItemList.Count == 0)
            InitBuyItemList();
        ShowItemList(m);
        IsSelectGoodsMenu(true);
        ShowSelectCount(false);
        _view.BuyScrollView_UIScrollView.ResetPosition();
    }

    //显示某个二级菜单的商品
    private void ShowItemList(TradeMenu secondMenu)
    {
        if (secondMenu == null)
            return;

        _curSecondMenu = secondMenu;
        
        var key = _data.PitchCtrl.GetStallCenterDto.Keys.FindElementIdx(d => d == _curFirstMenu.id);
        if (key == -1)
        {
            _buyItemList.ForEach(item =>
            {
                item.UpdateItemNull();
                item.gameObject.SetActive(false);
            });
            return;
        }

        var dtos = _data.PitchCtrl.GetStallCenterDto[_curFirstMenu.id];
        _curMenuGoods = dtos.items.Filter(d=>d.item.tradeMenuId == secondMenu.id);
        var list = _curMenuGoods.ToList();
        _buyItemList.ForEachI((item, idx) =>
        {
            item.gameObject.SetActive(_curMenuGoods.TryGetValue(idx) != null);
            var goodDto = _curMenuGoods.TryGetValue(idx);
            if (goodDto != null)
                item.SetBuyItemInfo(goodDto);
            else
                item.UpdateItemNull();
            item.HideDragScrollView(_curMenuGoods.Count() > 6);
        });

        var goods = _curMenuGoods.TryGetValue(0);
        //有些商品没有品质,所以隐藏筛选匡
        if (goods == null || goods.extra == null || goods.extra as PropsExtraDto == null) 
            _view.PopupBtn_UIButton.gameObject.SetActive(false);

        _view.BuyGoodsGrid_UIGrid.Reposition();
        //_view.BuyScrollView_UIScrollView.ResetPosition();
    }

    private void ShowItemByQuilty(int min, int max)
    {
        var dtos = _data.PitchCtrl.GetStallCenterDto[_curFirstMenu.id];
        var allDataList = dtos.items.Filter(d => d.item.tradeMenuId == _curSecondMenu.id);
        List<StallGoodsDto> list = new List<StallGoodsDto>();
        allDataList.ForEach(dto =>
        {
            var goods = dto.extra as PropsExtraDto;
            if (goods != null && goods.rarity >= min && goods.rarity < max)
                list.Add(dto);
        });
       
        _buyItemList.ForEachI((item, idx) =>
        {
            if (idx < list.Count)
            {
                item.gameObject.SetActive(true);
                item.SetBuyItemInfo(list.TryGetValue(idx));
            }
            else
                item.gameObject.SetActive(false);
        });
        _curMenuGoods = list;
    }

    private void IsSelectGoodsMenu(bool b)
    {
        _view.MenuGtid_UIGrid.gameObject.SetActive(!b);
        _view.BuyGoodsGrid_UIGrid.gameObject.SetActive(b);
        _view.PageGrounp.gameObject.SetActive(b);
        _view.PopupBtn_UIButton.gameObject.SetActive(b);
        _view.LastBtn_UIButton.sprite.isGrey = true;
        _view.NextBtn_UIButton.sprite.isGrey = _curMenuGoods != null && _curMenuGoods.Count() <= 8;
    }

    private void ShowSelectCount(bool b)
    {
        _view.ChoiseCount_UIButton.gameObject.SetActive(b);
    }

    private void OpenPitchSellView()
    {
        var itemList = BackpackDataMgr.DataMgr.GetStallDtos();
        if (itemList == null || itemList.Count() == 0)
        {
            TipManager.AddTip("身上没有可以出售的商品");
            return;
        }
        ProxyTrade.OpenPitchSellView();
    }

    private void OnPitchItemClick(int idx)
    {
        var item = _data.PitchCtrl.GetStallItems.TryGetValue(idx);
        if (item == null)
        {
            var itemList = BackpackDataMgr.DataMgr.GetStallDtos();
            if (itemList == null || itemList.Count() == 0)
            {
                TipManager.AddTip("身上没有可以出售的商品");
                return;
            }
            ProxyTrade.OpenPitchSellView();
        }
        else
        {
            if (item.count > 0 || item.amount == 0) //有物品被出售
                _putawayEvt.OnNext(item.stallId);
            else
            {
                var goodsdto = _data.PitchCtrl.GetStallItems.TryGetValue(idx);
                ProxyTrade.OpenPitchPutawayAgainView(goodsdto);
            }
        }
    }

    private void UpdateSellItemList(IPitchData data)
    {
        if (data.GetStallItems == null || _sellItemList.Count == 0) return;

        _stallGoods = data.GetStallItems;
        _sellItemList.ForEachI((item, idx) =>
        {
            if (idx < data.GetStallItems.Count())
            {
                var goodsDto = _stallGoods.TryGetValue(idx);
                item.SetSellItemInfo(goodsDto);
            }
            else
            {
                item.UpdateItemNull();
                item.ShowAddBtn(idx < data.Capability);
                item.ShowLockBtn(idx >= data.Capability);
                if (idx == data.Capability)
                {
                    item.SetLockLb(TradeHelper.GetLockTxtByCapability(data.Capability));
                }
            }
        });
    }

    public void UpdateDataAndView(ITradeData data)
    {
        _data = data;
        UpdateSellItemList(data.PitchCtrl);
        UpdateCaseLb();
        ShowItemList(_curSecondMenu);

        //表示可以刷新倒计时
        if (data.PitchCtrl.PitchCDTime == 300)
            _refreshCDTime.OnNext(new Unit());
        if (_data.PitchCtrl.PitchCDTime <= 0 && _curPageType == PageType.BuyPage)
            _cdTimeEvt.OnNext(_curFirstMenu.id);
    }

    public void ChoiseGoods(int goodsId)
    {
        var stallgoods = _allStallGoods.Find(d => d.id == goodsId);
        if (stallgoods == null)
        {
            GameDebuger.LogError(string.Format("stallGoods表找不到{0},请检查", goodsId));
            return;
        }
        
        var secondMenu = _allStellMenuList.Find(d => d.id == stallgoods.tradeMenuId);
        var idx = _firstOptionList.FindElementIdx(d => d.id == secondMenu.parentId);
        var firstMenu = _firstOptionList.TryGetValue(idx);
        OnFirstOptionClick(_optionBtnList[idx], firstMenu);
        OnSecondOptionClick(secondMenu);
        OnPitchBuyItemClick(0); //默认选中第一个
    }


    public void ShowBuyGroup(bool b)
    {
        _view.BuyGroup.SetActive(b);
        _view.SellGroup.SetActive(!b);
        _view.ChoiseCount_UIButton.gameObject.SetActive(b);
        
        if(_data != null && b && _data.PitchCtrl.PitchCDTime == 0)
            _cdTimeEvt.OnNext(_curFirstMenu.id);

    }

    public void ChangeItemListPage(int page)
    {
        var showData = _curMenuGoods.GetElememtsByRange(page * 8, 8);
        if (showData == null)
        {
            TipManager.AddTip("没有更多物品");
            return;
        }

        if (_curTradeGoods.count <= 8)
        {
            _view.PageLb_UILabel.text = "1/1";
            _view.LastBtn_UIButton.sprite.isGrey = true;
            _view.NextBtn_UIButton.sprite.isGrey = true;
        }
        else
        {
            _view.PageLb_UILabel.text = page == 0 ? "1/2" : "2/2";
            _view.LastBtn_UIButton.sprite.isGrey = page == 0;
            _view.NextBtn_UIButton.sprite.isGrey = page != 0;
        }

        ShowItemByPage(page);
    }

    //某项物品翻页刷新界面
    private void ShowItemByPage(int page)
    {
        var showData = _curMenuGoods.GetElememtsByRange(page * 8, 8);
        if (showData == null)
            return;

        _buyItemList.ForEachI((item, idx) =>
        {
            if (idx < showData.Count())
            {
                item.gameObject.SetActive(true);
                item.SetBuyItemInfo(showData.TryGetValue(idx));
            }
            else
                item.gameObject.SetActive(false);
        });
    }

    public void UpdateRefreshLb(int s)
    {
        if (_view != null)
        {
            string txt = s > 0 ? string.Format("{0}分{1}秒后免费刷新", s/60, s%60) : "可免费刷新";
            _view.RefreshLb_UILabel.text = txt;
        }
    }

    public void UpdateCaseLb()
    {
        _view.CashLb_UILabel.text = ModelManager.Player.GetPlayerWealthSilver().ToString();
    }

    private void OpenSimpleNumber()
    {
        if (_curTradeGoods == null)
        {
            TipManager.AddTip("请选择要购买的物品");
            return;
        }

        if (_numberInputer == null)
            _numberInputer = ProxySimpleNumberModule.Open();

        _numberInputer.InitData(1, _curTradeGoods.amount, _view.ChoiseCount_UIButton.gameObject);
        _numberInputer.OnValueChangeStream.Subscribe(s =>
        {
            _selectItemCount = s;
            _view.CountLb_UILabel.text = s.ToString();
        });
        _numberInputer.SetPos(new Vector3(106, -163, 0));
    }

    private void UpdateCount(bool isAdd)
    {
        if (_curTradeGoods == null)
        {
            TipManager.AddTip("请选择要购买的物品");
            return;
        }

        if (isAdd)
            _selectItemCount = _selectItemCount + 1 > _curTradeGoods.amount
                ? _curTradeGoods.amount
                : _selectItemCount + 1;
        else
            _selectItemCount = _selectItemCount - 1 <= 1 ? 1 : _selectItemCount - 1;

        _view.CountLb_UILabel.text = _selectItemCount.ToString();
    }

    #region 筛选下拉菜单
    private void OnPopupBtnClick()
    {
        if (_curSecondMenu == null)
        {
            TipManager.AddTip("请选择某类商品");
            return;
        }
        _teamPopupListCtrl.UpdateView(_popupNameList);
        _teamPopupListCtrl.Show();
        _teamPopupListCtrl.SetPos(_view.PopupPos_Transform.position);
        _teamPopupListCtrl.SetListBgDimensions(126, 52 * _popupNameList.Count + 5);
    }

    private void OnPupupListBtnClick(int minLv, int maxLv)
    {
        ShowItemByQuilty(minLv, maxLv);
        _view.PopupLb_UILabel.text = string.Format("{0}-{1}", minLv, maxLv);
    }
    #endregion

    private void AddCashBtnClick()
    {
        ProxyExChangeMain.OpenExChangeMain(AppVirtualItem.VirtualItemEnum.SILVER);
    }

    public PageType GetCurTab { get { return _curPageType; } }
}
