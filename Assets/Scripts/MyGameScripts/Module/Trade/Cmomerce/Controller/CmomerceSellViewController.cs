// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CmomerceSellViewController.cs
// Author   : xush
// Created  : 2/10/2018 4:21:07 PM
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;
using UnityEngine;

public partial interface ICmomerceSellViewController
{
    void SetSellItemInfo(BagItemDto dto);
    UniRx.IObservable<ICmomerItemData> SellBtnHandler { get; }
}

public partial class CmomerceSellViewController
{
    private int _sellCout = 1;
    private int _maxCout;

    private BagItemDto _dto;
    private TradeGoods _tradeGoods;
    private Subject<ICmomerItemData> _sellBtnEvt = new Subject<ICmomerItemData>(); 
    public UniRx.IObservable<ICmomerItemData> SellBtnHandler { get { return _sellBtnEvt; } } 

    public static ICmomerceSellViewController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, ICmomerceSellViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as ICmomerceSellViewController;
            
        return controller;        
    }         
        
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
            
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
	{
	    EventDelegate.Add(_view.AddBtn_UIButton.onClick, () => { UpdateCount(true); });
	    EventDelegate.Add(_view.ReduceBtn_UIButton.onClick, () => { UpdateCount(false); });
	    EventDelegate.Add(_view.SellBtn_UIButton.onClick, () =>
	    {
            var itemdata = CmomerceItemData.Create(_dto.index, _sellCout);
            _sellBtnEvt.OnNext(itemdata);
        });
	}

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    private void UpdateCount(bool isAdd)
    {
        if (_tradeGoods == null)
        {
            GameDebuger.LogError(string.Format("tradegoods表找不到{0},请检查", _dto.itemId));
            return;
        }

        if (!isAdd && _sellCout <= 1)
        {
            TipManager.AddTip("不能再减少了");
            return;
        }

        if(isAdd && _sellCout >= _maxCout)
        {
            TipManager.AddTip("不能再增加了");
            return;
        }

        _sellCout = isAdd ? _sellCout + 1 : _sellCout - 1;
        _view.PriceLb_UILabel.text = Mathf.Floor(_dto.tradePrice * _sellCout * _tradeGoods.taxRate).ToString();
    }

    public void SetSellItemInfo(BagItemDto dto)
    {
        _dto = dto;
        var item = DataCache.getDtoByCls<GeneralItem>(dto.itemId);
        _tradeGoods = DataCache.getDtoByCls<TradeGoods>(dto.itemId);
        if (_tradeGoods == null)
        {
            GameDebuger.LogError(string.Format("tradegoods表找不到{0},请检查", dto.itemId));
            return;
        }

        _maxCout = dto.count;
        UIHelper.SetItemIcon(_view.Icon_UISprite, item.icon);
        UIHelper.SetItemQualityIcon(_view.IconBG_UISprite, (item as AppItem) == null ? 3 : (item as AppItem).quality);
        _view.NumLb_UILabel.text = dto.count.ToString();
        _view.NameLb_UILabel.text = item.name;
        _view.DescLb_UILabel.text = item.description;
        _view.SellNumLb_UILabel.text = _sellCout.ToString();

        var price = (int)Mathf.Floor(dto.tradePrice * (1 - _tradeGoods.taxRate));
        _view.PriceLb_UILabel.text = price.ToString(); 
        _view.TotelLb_UILabel.text = ModelManager.Player.GetPlayerWealthGold().ToString();
    }
}
