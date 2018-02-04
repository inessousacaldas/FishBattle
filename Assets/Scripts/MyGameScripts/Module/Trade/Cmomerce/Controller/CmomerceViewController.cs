// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CmomerceViewController.cs
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

public class CmomerceItemData: ICmomerItemData
{
    private int _itemId;    //选中物品ID
    private int _num;       //数量
    private int _price;     //价格

    public int GetItemId { get { return _itemId; } }
    public int GetItemNum { get { return _num; } }
    public int GetPrice { get { return _price; } }

    public static ICmomerItemData Create(int id, int num, int price = -1)
    {
        CmomerceItemData data = new CmomerceItemData();
        data._itemId = id;
        data._num = num;
        data._price = price;
        return data;
    }
}

public interface ICmomerItemData
{
    int GetItemId { get; }
    int GetItemNum { get; }
    int GetPrice { get; }
}

public partial interface ICmomerceViewController
{
    UniRx.IObservable<ICmomerItemData> GetOnBuyHandler { get; }
    UniRx.IObservable<ICmomerItemData> GetSellHandler { get; }

    void UpdateDataAndView(ITradeData data);
}

public partial class CmomerceViewController
{
    private CompositeDisposable _disposable = new CompositeDisposable();

    private ITradeData _data;
    private PageType _curPage = PageType.BuyPage;
    private int _curItemNum = 0;    //选中物品数量
    private int _price;             //选中物品单价
    private enum PageType
    {
        BuyPage = 0,
        SellPage = 1
    }

    private TabbtnManager tabMgr;
    public static readonly ITabInfo[] TeamTabInfos =
    {
        TabInfoData.Create((int) PageType.BuyPage, "我要购买"),
        TabInfoData.Create((int) PageType.SellPage, "我要出售")
    };

    #region subject
    private Subject<ICmomerItemData> _onBuyClickEvt = new Subject<ICmomerItemData>();
    public UniRx.IObservable<ICmomerItemData> GetOnBuyHandler { get { return _onBuyClickEvt; } }

    private Subject<ICmomerItemData> _onSellClickEvt = new Subject<ICmomerItemData>();
    public UniRx.IObservable<ICmomerItemData> GetSellHandler { get { return _onSellClickEvt; } }
    #endregion

    #region 我要购买
    private IEnumerable<TradeMenu> _allMenuList = new List<TradeMenu>(); 
    private IEnumerable<TradeMenu> _firstMenuList = new List<TradeMenu>();
    private IEnumerable<TradeMenu> _secondMenuList = new List<TradeMenu>();
    private List<TradeOptionBtnController> _optionList = new List<TradeOptionBtnController>();
    private List<TradeOptionBtnController> _secondOptionList = new List<TradeOptionBtnController>(); 
    private List<PitchItemController> _tradeItemList = new List<PitchItemController>();

    private SimpleNumberInputerController _numberInputer;

    private delegate void MenuBtnClick(TradeOptionBtnController btn, TradeMenu data, int idx);
    private delegate void TradeItemClick(PitchItemController item, int idx);

    private int _curItemId = 0;     //选中物品id
    private int _curItemIndex = -1; //出售界面选中的idx
    private TradeMenu _curFirstMenu;
    private TradeMenu _secondCurMenu;
    #endregion

    #region 我要出售
    private int _itemCountMax;
    private List<ItemCellController> _bagItemList;
    private IEnumerable<BagItemDto> _bagDataList = new List<BagItemDto>();
    private delegate void BagItemClick(ItemCellController item, int idx);
    #endregion

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        InitTabBtn();
        InitData();
        InitOptionBtn();
        MainViewState(true);
        IsSelectItem(false);
        _view.SecondTable_UITable.gameObject.SetActive(false);
        _view.CashLb_UILabel.text = ModelManager.Player.GetPlayerWealthGold().ToString();
    }

    private void InitTabBtn()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
            _view.TabBtn_UIGrid.gameObject
            , TabbtnPrefabPath.TabBtnWidget_S3.ToString()
            , "Tabbtn_" + i);

        tabMgr = TabbtnManager.Create(TeamTabInfos, func);
        tabMgr.SetBtnLblFont(20, "2d2d2d", 18, ColorConstantV3.Color_VerticalUnSelectColor2_Str);

        _disposable.Add(tabMgr.Stream.Subscribe(pageIndex =>
        {
            MainViewState(pageIndex == (int) PageType.BuyPage);
        }));
    }

    private void InitData()
    {
        _allMenuList = DataCache.getArrayByCls<TradeMenu>();
        _firstMenuList = _allMenuList.Filter(d => d.type == (int)TradeMenu.TradeMenuEnum.Trade && d.parentId == 0);
        _bagDataList = BackpackDataMgr.DataMgr.GetTradeItems();
        _curFirstMenu = _firstMenuList.TryGetValue(0);
        var list = _allMenuList.Filter(d => d.type == (int)TradeMenu.TradeMenuEnum.Trade && d.parentId == _curFirstMenu.id);
        _secondCurMenu = list.Count() == 0 ? _curFirstMenu : list.TryGetValue(0);
    }

    private void IsSelectItem(bool b)
    {
        _view.Texture.gameObject.SetActive(!b);
        _view.ItemDescLb_UILabel.gameObject.SetActive(b);
        _view.ItemNameLb_UILabel.gameObject.SetActive(b);
        _view.NumLb_UILabel.text = "0";
        if (!b)
            _view.TotalLb_UILabel.text = "-----";
        _view.ReduceNumBtn_UIButton.sprite.isGrey = true;
        _view.PropsIcon_UISprite.gameObject.SetActive(b);
    }

    private void MainViewState(bool b)
    {
        _curItemNum = 0;
        _price = 0;
        _curItemId = 0;
        _curItemIndex = -1;

        _view.BuyGroup.SetActive(b);
        _view.SellGroup.SetActive(!b);
        _view.BuyBtn_UIButton.gameObject.SetActive(b);
        _view.SellBtn_UIButton.gameObject.SetActive(!b);

        _curPage = b ? PageType.BuyPage : PageType.SellPage;
        if (!b)
        {
            if (_bagItemList == null)
                _bagItemList = new List<ItemCellController>();

            InitBagItemList();
            UpdateSellItemList();
        }
        UpdateBuyInfo();
        IsSelectItem(false);
        _tradeItemList.ForEach(item =>
        {
            item.IsSelect(item.GetItemId() == _curItemId);
        });
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        _disposable.Add(PlayerModel.Stream.Subscribe(_ => { UpdateCashLb(); }));
        _disposable.Add(BackpackDataMgr.Stream.Subscribe(_ =>
        {
            if (_curPage == PageType.SellPage)
                UpdateSellItemList();
        }));
        _disposable.Add(OnAddNumBtn_UIButtonClick.Subscribe(_ => { UpdateItemNum(true); }));
        _disposable.Add(OnReduceNumBtn_UIButtonClick.Subscribe(_ => { UpdateItemNum(false); }));
        _disposable.Add(OnBuyCashBtn_UIButtonClick.Subscribe(_ =>
        {
            ProxyExChangeMain.OpenExChangeMain(AppVirtualItem.VirtualItemEnum.GOLD);
        }));
        _disposable.Add(OnBuyBtn_UIButtonClick.Subscribe(_ =>
        {
            var itemdata = CmomerceItemData.Create(_curItemId, _curItemNum, _price);
            if (ModelManager.Player.GetPlayerWealthGold() < _curItemNum*_price)
            {
                ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.GOLD, (int) _curItemNum*_price,
                    () =>
                    {
                        _onBuyClickEvt.OnNext(itemdata);
                    });
            }
            else
                _onBuyClickEvt.OnNext(itemdata);
        }));
        _disposable.Add(OnSellBtn_UIButtonClick.Subscribe(_ =>
        {
            if (_curItemId == 0)
            {
                TipManager.AddTip("请先选择物品");
                return;
            }
            var itemdata = CmomerceItemData.Create(_curItemIndex, _curItemNum);
            _onSellClickEvt.OnNext(itemdata);
        }));
        _disposable.Add(OnOpenInputBtn_UIButtonClick.Subscribe(_ =>
        {
            if (_curItemId == 0)
            {
                TipManager.AddTip("请先选择物品");
                return;
            }
            if (_numberInputer == null)
                _numberInputer = ProxySimpleNumberModule.Open();

            _numberInputer.InitData(0, _itemCountMax, _view.OpenInputBtn_UIButton.gameObject);
            _numberInputer.OnValueChangeStream.Subscribe(s =>
            {
                _curItemNum = s;
                _view.NumLb_UILabel.text = s.ToString();
                _view.ReduceNumBtn_UIButton.sprite.isGrey = _curItemNum <= 1;
                _view.AddNumBtn_UIButton.sprite.isGrey = _curItemNum >= _itemCountMax;
                UpdateBuyInfo();
            });
            _numberInputer.SetPos(new Vector3(210, 0, 0));
        }));
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        _view.AddNumBtn_UIEventListener.onPress = null;
        _view.ReduceNumBtn_UIEventListener.onPress = null;
    }

    #region 我要购买
    private void InitOptionBtn()
    {
        MenuBtnClick func = (btn, data, idx) =>
        {
            _disposable.Add(btn.GetClickHandler.Subscribe(_ =>
            {
                _curItemId = 0;
                _optionList.ForEach(button => button.IsSelect(btn == button));
                InitSecondMenu(data, btn);
                IsSelectItem(false);
                _view.BuyItemScrollView_UIScrollView.ResetPosition();
            }));
        };

        _firstMenuList.ForEachI((data, idx) =>
        {
            var option = AddChild<TradeOptionBtnController, TradeOptionBtn>(
                _view.OptionTable_UITable.gameObject,
                TradeOptionBtn.NAME);
            option.SetTradeMenuBtn(data);
            _optionList.Add(option);
            func(option, data, idx);
        });

        _view.OptionTable_UITable.Reposition();
        _optionList[0].IsSelect(true);
    }

    private void InitSecondMenu(TradeMenu menu, TradeOptionBtnController parent)
    {
        if (_curFirstMenu == menu && _view.SecondTable_UITable.gameObject.activeSelf)
        {
            _view.SecondTable_UITable.gameObject.SetActive(false);
            _view.OptionTable_UITable.Reposition();
            parent.SetArrowAngles(false);
            return;
        }

        _curFirstMenu = menu;
        _secondMenuList = _allMenuList.Filter(d => d.type == (int)TradeMenu.TradeMenuEnum.Trade && d.parentId == menu.id);
        _view.SecondTable_UITable.gameObject.SetActive(_secondMenuList.Count() != 0);
        //如果某项物品分类没有二级菜单,则直接显示物品,否则显示二级菜单
        if (_secondMenuList.Count() == 0)
            InitItemList(menu);
        else
        {
            MenuBtnClick func = (btn, data, idx) =>
            {
                _disposable.Add(btn.GetClickHandler.Subscribe(_ =>
                {
                    _curItemNum = 0;
                    _curItemId = 0;
                    InitItemList(idx);
                    UpdateBuyInfo();
                    _secondOptionList.ForEach(button => button.IsSelect(btn == button));
                    IsSelectItem(false);
                    _view.BuyItemScrollView_UIScrollView.ResetPosition();
                }));
            };

            _secondOptionList.ForEach(item => item.gameObject.SetActive(false));
            _secondMenuList.ForEachI((data, idx) =>
            {
                if (idx >= _view.SecondTable_UITable.transform.childCount)
                {
                    var option = AddChild<TradeOptionBtnController, TradeOptionBtn>(
                        _view.SecondTable_UITable.gameObject,
                        TradeOptionBtn.NAME);
                    option.SetTradeMenuBtn(data, true);
                    _secondOptionList.Add(option);
                    func(option, data, idx);
                }
                else
                {
                    var item = _secondOptionList.TryGetValue(idx);
                    if (item != null)
                    {
                        item.SetTradeMenuBtn(data, true);
                        item.gameObject.SetActive(true);
                    }
                }
            });
            _secondOptionList.ForEachI((go, idx) =>
            {
                if (idx == 0)
                {
                    InitItemList(0);
                    go.IsSelect(true);
                }else
                    go.IsSelect(false);
            });

            _view.SecondTable_UITable.transform.parent = parent.transform;
            _view.SecondTable_UITable.transform.localPosition = new Vector3(-76, -25, 0);
            _view.SecondTable_UITable.Reposition();
        }
        _view.OptionTable_UITable.Reposition();
        _view.BuyScollView_UIScrollView.ResetPosition();
    }

    private void InitItemList(int menuIdx)
    {
        InitItemList(_secondMenuList.TryGetValue(menuIdx));
    }

    private void InitItemList(TradeMenu menu)
    {
        if (menu == null || _data == null)
            return;

        _secondCurMenu = menu;
        if (_data.CmomerceCtrl.GetTradeGoodsDto == null)
        {
            GameDebuger.LogError("_data.CmomerceCtrl.GetTradeGoodsDto为空,请检查");
            return;
        }
        var list = _data.CmomerceCtrl.GetTradeGoodsDto.Filter(d => d.item.tradeMenuId == menu.id);
        TradeItemClick func = (item, idx) =>
        {
            _disposable.Add(item.GetItemClick.Subscribe(_ =>
            {
                _itemCountMax = 200;
                ShowSelectItemInfo(idx);
                _tradeItemList.ForEachI((go, index) => { go.IsSelect(index == idx);});
            }));
        };

        _tradeItemList.ForEachI((item, idx) => { item.gameObject.SetActive(idx < list.Count()); });
        list.ForEachI((data, idx) =>
        {
            if (idx >= _tradeItemList.Count)
            {
                var item = AddChild<PitchItemController, PitchItem>(
                    _view.BuyItemGrid_UIGrid.gameObject,
                    PitchItem.NAME);
                _tradeItemList.Add(item);
                func(item, idx);
                item.SetTradeItem(data);
            }
            else
            {
                _tradeItemList[idx].gameObject.SetActive(true);
                _tradeItemList[idx].SetTradeItem(data);
                _tradeItemList[idx].IsSelect(data.itemId == _curItemId);
            }
            _tradeItemList[idx].HideDragScrollView(list.Count() > 5);
        });

        _view.BuyItemGrid_UIGrid.Reposition();
    }

    //选中某个商品
    private void ShowSelectItemInfo(int idx)
    {
        var list = _data.CmomerceCtrl.GetTradeGoodsDto.Filter(d => d.item.tradeMenuId == _secondCurMenu.id);
        var goodstDto = list.TryGetValue(idx);
        if (goodstDto == null)
        {
            GameDebuger.LogError("=======数据有误=====");
            return;
        }
        var item = DataCache.getDtoByCls<GeneralItem>(goodstDto.itemId);
        if (item == null)
        {
            GameDebuger.LogError(string.Format("GeneralItem表找不到{0},请检查", goodstDto.itemId));
            return;
        }
        IsSelectItem(true);
        _curItemId = goodstDto.itemId;
        _view.ItemNameLb_UILabel.text = item.name;
        _view.ItemDescLb_UILabel.text = item.description;
        UIHelper.SetItemIcon(_view.PropsIcon_UISprite, item.icon);
        if(item is AppItem)
            UIHelper.SetItemQualityIcon(_view.IconBG_UISprite, (item as AppItem).quality);
        else
            UIHelper.SetItemQualityIcon(_view.IconBG_UISprite, 3);
        _curItemNum = 1;    //选中物品时,购买数量默认为1
        _price = (int)Mathf.Ceil(goodstDto.price);
        UpdateBuyInfo();
    }
    #endregion

    #region 我要出售

    private void InitBagItemList()
    {
        BagItemClick fun = (item, idx) =>
        {
            _disposable.Add(item.OnCellClick.Subscribe(_ =>
            {
                _bagItemList.ForEach(d => d.isSelect = d == item);
                OnSellItemClick(idx);
            }));
        };

        _bagDataList = BackpackDataMgr.DataMgr.GetTradeItems();
        if (_bagDataList == null || _bagDataList.Count() == 0) return;

        for (int i = _view.SellGrid_UIGrid.transform.childCount; i < _bagDataList.Count(); i++)
        {
            var dto = _bagDataList.TryGetValue(i);
            var item = AddChild<ItemCellController, ItemCell>(_view.SellGrid_UIGrid.gameObject, ItemCell.NAME);
            _bagItemList.Add(item);
            item.SetTradeItemCell(dto);
            item.SetSelectSprite("ect_select_3", "CommonUIAltas");
            fun(item, i);
        }
        _view.SellGrid_UIGrid.Reposition();
    }

    private void OnSellItemClick(int idx)
    {
        var itemdto = _bagDataList.TryGetValue(idx);
        if (itemdto == null)
        {
            GameDebuger.LogError("====数据有误====");
            return;
        }
        _itemCountMax = itemdto.count;
        _curItemNum = itemdto.count;
        IsSelectItem(true);
        _curItemId = itemdto.itemId;
        _curItemIndex = itemdto.index;
        _view.ItemNameLb_UILabel.text = itemdto.item.name;
        _view.ItemDescLb_UILabel.text = itemdto.item.description;
        UIHelper.SetItemIcon(_view.PropsIcon_UISprite, itemdto.item.icon);
        UIHelper.SetItemQualityIcon(_view.IconBG_UISprite, itemdto.item.quality);
        var dto = _data.CmomerceCtrl.GetTradeGoodsDto.Find(d => d.itemId == itemdto.itemId);
        if (dto == null)
        {
            GameDebuger.LogError("商会中找不到该物品,请检查");
            return;
        }
        if (itemdto.tradePrice == 0)
            _price = (int)Mathf.Floor(dto.price * (1 - dto.item.taxRate));
        else
            _price = (int)Mathf.Floor(itemdto.tradePrice * (1 - dto.item.taxRate));

        UpdateBuyInfo();
    }

    private void UpdateSellItemList()
    {
        _bagDataList = BackpackDataMgr.DataMgr.GetTradeItems();
        IsSelectItem(false);
        _curItemId = 0;
        if (_bagDataList == null)
        {
            if (_bagItemList != null)
                _bagItemList.ForEach(item => item.gameObject.SetActive(false));
            return;
        }
        _bagItemList.ForEachI((item, idx) =>
        {
            if (_bagDataList.TryGetValue(idx) != null)
            {
                var dto = _bagDataList.TryGetValue(idx);
                item.SetTradeItemCell(dto);
                item.gameObject.SetActive(true);
            }
            else
                item.gameObject.SetActive(false);
            item.isSelect = false;
        });
        
    }
    #endregion

    private void UpdateItemNum(bool IsAdd)
    {
        if (_curItemId <= 0)
        {
            TipManager.AddTip("请先选择商品");
            return;
        }

        if (IsAdd)
            _curItemNum = _curItemNum + 1 > _itemCountMax ? _curItemNum = _itemCountMax : _curItemNum + 1;
        else
            _curItemNum = _curItemNum > 1 ? _curItemNum - 1 : _curItemNum = 1;

        UpdateBuyInfo();
    }

    private void UpdateBuyInfo()
    {
        _view.NumLb_UILabel.text = _curItemNum.ToString();
        _view.ReduceNumBtn_UIButton.sprite.isGrey = _curItemNum <= 1;
        _view.AddNumBtn_UIButton.sprite.isGrey = _curItemNum >= _itemCountMax;
        
        var totle = _curItemNum*_price;
        _view.TotalLb_UILabel.text = _curItemNum > 0 ? totle.ToString() : "-----";
    }

    private void UpdateCashLb()
    {
        var gold = ModelManager.Player.GetPlayerWealthGold();
        if (_view == null || _view.CashLb_UILabel == null)
            return;

        _view.CashLb_UILabel.text = _curPage == PageType.BuyPage
            ? gold >= _curItemNum * _price
                ? gold.ToString().WrapColor(ColorConstantV3.Color_White)
                : gold.ToString().WrapColor(ColorConstantV3.Color_Red_Str)
            : gold.ToString().WrapColor(ColorConstantV3.Color_White);
    }

    public void UpdateDataAndView(ITradeData data)
    {
        _data = data;
        InitItemList(_secondCurMenu);

        if (_data != null && _curItemId != 0)
        {
            var goods = _data.CmomerceCtrl.GetTradeGoodsDto.Find(d => d.itemId == _curItemId);
            if (_curPage == PageType.BuyPage)
            {
                _price = (int) Mathf.Ceil(goods.price);
                UpdateBuyInfo();
            }
            else
            {
                _price = (int)Mathf.Floor(goods.price * (1 - goods.item.taxRate));
                //UpdateSellItemList();
            }
        }
    }
}
