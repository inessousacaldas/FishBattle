// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ItemQuickBuyViewController.cs
// Author   : xush
// Created  : 1/3/2018 2:46:44 PM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;

public class QuickBuyItemData
{
    private int _shopId;
    private int _goodsId;
    private int _count;

    public int GetShopId { get { return _shopId; } }
    public int GetGoodsId { get { return _goodsId; } }
    public int GetCount { get { return _count; } }

    public static QuickBuyItemData Create(int shopId, int goodsId, int count)
    {
        QuickBuyItemData data = new QuickBuyItemData();
        data._shopId = shopId;
        data._goodsId = goodsId;
        data._count = count;
        return data;
    }
}

public partial interface IItemQuickBuyViewController
{
    void OnAddBtnHandler();
    void OnReduceBtnHandler();
    void OnMaxBtnHandler();
    UniRx.IObservable<QuickBuyItemData> OnBuyBtnHandler { get ;}
}

public partial class ItemQuickBuyViewController
{
    private CompositeDisposable _disposable;
    private SimpleNumberInputerController _numberInputer;
    private int _selectNum = 1;
    private int _maxNum;
    private bool _isEnouthCash;
    private int _selectShopId;
    private int _curShopIdx = 0;

    private List<Shop> _shopList = new List<Shop>();
    private List<ShopGoodsDto> _goodsList;
    private List<ShopGoods> _propsList; 
    private ShopGoods _props;

    private Subject<QuickBuyItemData> _onBuyBtnClickEvt = new Subject<QuickBuyItemData>();    
    public UniRx.IObservable<QuickBuyItemData> OnBuyBtnHandler { get { return _onBuyBtnClickEvt; } }
        
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();     
        _view.LastBtn_UIButton.gameObject.SetActive(false); 
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        _disposable.Add(OnSimpleBtn_UIButtonClick.Subscribe(_ =>
        {
            if (_numberInputer == null)
                _numberInputer = ProxySimpleNumberModule.Open();

            _numberInputer.InitData(0, _maxNum == -1 ? 9999 : _maxNum, _view.SimpleBtn_UIButton.gameObject);
            _numberInputer.OnValueChangeStream.Subscribe(s =>
            {
                _selectNum = s;
                SetCashInfo();
            });
            _numberInputer.SetPos(new Vector3(0, -153, 0));
        }));
        _disposable.Add(OnNextBtn_UIButtonClick.Subscribe(_ =>
        {
            _selectNum = 1;
            _curShopIdx = _curShopIdx + 1 >= _goodsList.Count ? _curShopIdx : _curShopIdx + 1;
            ChangeShop(_curShopIdx);
        }));

        _disposable.Add(OnLastBtn_UIButtonClick.Subscribe(_ =>
        {
            _selectNum = 1;
            _curShopIdx = _curShopIdx - 1 >= 0 ? _curShopIdx - 1 : 0;
            ChangeShop(_curShopIdx);
        }));

        _disposable.Add(BuyBtn_UIButtonEvt.Subscribe(_ =>
        {
            if (!_isEnouthCash)
            {
                var appitem = DataCache.getDtoByCls<GeneralItem>(_props.expendItemId) as VirtualItem;
                TipManager.AddTip(string.Format("{0}不足", appitem.name));
                return;
            }
            int count;
            var b = int.TryParse(_view.NumLb_UILabel.text, out count);
            if (b && count > 0)
            {
                var data = QuickBuyItemData.Create(_selectShopId, _props.id, count);
                _onBuyBtnClickEvt.OnNext(data);
            }
            else
                TipManager.AddTip("请选择购买数量");
        }));
    }

    protected override void RemoveCustomEvent ()
    {
        
    }
        
    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
	{
	    _shopList = DataCache.getArrayByCls<Shop>();
	    _propsList = DataCache.getArrayByCls<ShopGoods>();
	}

    protected override void UpdateDataAndView(IItemQuickBuyData data)
    {
        _goodsList = data.ShopGoodsList.shopGoodsDtos;
        _props = DataCache.getDtoByCls<ShopGoods>(_goodsList[_curShopIdx].goodsId);
        if (_props == null)
        {
            GameDebuger.LogError(string.Format("props标找不到{0},请检查", 00));
            return;
        }

        _selectShopId = _goodsList[_curShopIdx].goods.shopId;
        var shop = _shopList.Find(d => d.id == _selectShopId);
        _view.Title_UILabel.text = shop == null ? "" : shop.name;
        SetPropsInfo();
        SetCashInfo();
    }

    private void ChangeShop(int shopIdx)
    {
        _view.LastBtn_UIButton.gameObject.SetActive(shopIdx != 0);
        _view.NextBtn_UIButton.gameObject.SetActive(shopIdx < _goodsList.Count - 1);
        _selectShopId = _goodsList[shopIdx].goods.shopId;
        var shop = _shopList.Find(d => d.id == _selectShopId);
        _view.Title_UILabel.text = shop == null ? "": shop.name;
        _props = _goodsList[shopIdx].goods;
        SetPropsInfo();
        SetCashInfo();
    }

    public void OnAddBtnHandler()
    {
        if (_selectNum == 9999)
        {
            TipManager.AddTip("不能选择更多");
            return;
        }

        if (_maxNum == -1 || _selectNum < _maxNum)
            _selectNum += 1;
        else
            _selectNum = _maxNum;

        _view.AddBtn_UIButton.sprite.isGrey = _maxNum != -1 && _selectNum >= _maxNum;
        SetCashInfo();
    }

    public void OnReduceBtnHandler()
    {
        if (_selectNum > 0)
            _selectNum -= 1;
        else
            _selectNum = 0;

        _view.ReduceBtn_UIButton.sprite.isGrey = _selectNum <= 0;
        SetCashInfo();
    }

    public void OnMaxBtnHandler()
    {
        _selectNum = _maxNum;
        SetCashInfo();
    }

    private void SetPropsInfo()
    {
        var generalItem = ItemHelper.GetGeneralItemByItemId(_props.itemId) as AppItem;
        if (generalItem == null)
        {
            GameDebuger.LogError(string.Format("找不到{0}", _props.itemId));
            return;
        }
        var shopGoods = _propsList.Find(d => d.id == _props.id);
        _view.ParopNameLb_UILabel.text = _props.goodsName;
        UIHelper.SetItemIcon(_view.PropsIcon_UISprite, generalItem.icon);
        UIHelper.SetItemQualityIcon(_view.IconBG_UISprite, generalItem.quality);
        var goods = _goodsList.Find(d => d.goods.shopId == _selectShopId);
        _maxNum = shopGoods.limitBuyCount == 0 ? -1 : shopGoods.limitBuyCount;  //如果无限购,则设为-1
        _view.MaxBtn_UIButton.gameObject.SetActive(shopGoods.limitBuyCount != 0);
        if (shopGoods.limitBuyCount == 0)
            _view.DescLb_UILabel.text = "";
        else
            _view.DescLb_UILabel.text = string.Format("每周限购:{0}", shopGoods == null ? 0 : shopGoods.limitBuyCount - goods.num);
    }

    private void SetCashInfo()
    {
        var wealth = ModelManager.Player.GetPlayerWealthById(_props.expendItemId);
        _view.CashLb_UILabe.text = string.Format("{0}", _props.originalPrice * _selectNum).WrapColor(ColorConstantV3.Color_Yellow_Str);
        _view.CashIcon_UISprite.spriteName = string.Format("virualItem_{0}", _props.expendItemId);
        _view.HadCashLb2_UILabel.text = string.Format("{0}", wealth).WrapColor(ColorConstantV3.Color_Yellow_Str);
        _view.CashIcon2_UISprite.spriteName = string.Format("virualItem_{0}", _props.expendItemId);
        _view.NumLb_UILabel.text = _selectNum.ToString();
        
        if (_props.expendItemId == (int) AppVirtualItem.VirtualItemEnum.BINDDIAMOND)
        {
            var diamond = ModelManager.Player.GetPlayerWealthById((int) AppVirtualItem.VirtualItemEnum.DIAMOND);
            _view.CashIcon1_UISprite.gameObject.SetActive(true);
            _view.HadCashLb1_UILabel.gameObject.SetActive(true);
            _view.HadCashLb1_UILabel.text =
                diamond.ToString().WrapColor(ColorConstantV3.Color_Yellow_Str);
            _view.CashIcon1_UISprite.spriteName = "virualItem_1";
            _isEnouthCash = wealth >= _props.originalPrice*_selectNum && diamond < _props.originalPrice;
        }
        else
        {
            _isEnouthCash = wealth >= _props.originalPrice * _selectNum;
            _view.CashIcon1_UISprite.gameObject.SetActive(false);
            _view.HadCashLb1_UILabel.gameObject.SetActive(false);
        }
    }
}
