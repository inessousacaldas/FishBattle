// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PitchSellViewController.cs
// Author   : xush
// Created  : 7/1/2017 6:01:01 PM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppDto;
using UniRx;
using UnityEngine;

public class PitchSellViewData: IPitchSellData
{
    private int _idx;   //背包索引 
    private int _price;
    private int _count;
    private float _saleFactor;  //涨跌幅度
    private int _itemId;

    public int GetItemId { get { return _itemId; } set { _itemId = value; } }
    public int GetIdx { get { return _idx; } set { _idx = value; } }
    public int GetPrice { get { return _price;} set { _price = value; } }
    public int GetCount { get { return _count; } set { _count = value; } }
    public float GetSaleFactor { get { return _saleFactor; } set { _saleFactor = value; } }

    public static PitchSellViewData Create(int idx, 
        int price, 
        int count, 
        float saleFactor,
        int itemId)
    {
        PitchSellViewData data = new PitchSellViewData();
        data._idx = idx;
        data._price = price;
        data._count = count;
        data._saleFactor = saleFactor;
        data._itemId = itemId;
        return data;
    }
}

public interface IPitchSellData
{
    int GetIdx { get; set; }
    int GetPrice { get; set; }
    int GetCount { get; set; }
    float GetSaleFactor { get; set; }
    int GetItemId { get; set; }
}

public partial interface IPitchSellViewController
{
    void AddPriceLb(bool b);
    void UpdateCountLb(bool b);
    UniRx.IObservable<string> GetSellHandler { get; }  
}

public partial class PitchSellViewController
{
    private int _curItemPriceBase;
    private int _price;
    private int _cout;
    private float _handle;
    private float _saleFactor;
    private int _curItemId;
    private int _curPos;
    private int _maxCount;

    private ITradeData _data;
    private CompositeDisposable _disposable;
	private List<ItemCellController> _itemList = new List<ItemCellController>();
    private List<IPitchSellData> _curItemList = new List<IPitchSellData>();//选中物品数量

    private delegate void ItemCellClick(ItemCellController con, int idx);

    private Subject<string> _onSellClickEvt = new Subject<string>();
    public UniRx.IObservable<string> GetSellHandler { get { return _onSellClickEvt; } }  
	
	// 界面初始化完成之后的一些后续初始化工作
	protected override void AfterInitView ()
	{
		_disposable = new CompositeDisposable();
	    SelectItem(false);
	}

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
	{
	    _disposable.Add(OnSellBtn_UIButtonClick.Subscribe(_ =>
	    {
	        UpdateCurItem();
            StringBuilder str = new StringBuilder();
	        int handler = 0;
            _curItemList.ForEach(dto =>
            {
                var stalldto = DataCache.getDtoByCls<StallGoods>(dto.GetItemId);
                float rate = stalldto == null ? 0.05f : stalldto.taxRate;
                str.Append(dto.GetIdx + ":" + dto.GetPrice + ":" + dto.GetCount + ",");
                handler = (int)Mathf.Ceil(dto.GetPrice * dto.GetCount * rate) + handler;
            });
	        
	        if (handler > ModelManager.Player.GetPlayerWealthSilver())
	            ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.SILVER, handler,
	                () => { _onSellClickEvt.OnNext(str.ToString()); });
            else
                _onSellClickEvt.OnNext(str.ToString());
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
	}

	// 业务逻辑数据刷新
	protected override void UpdateDataAndView(ITradeData data)
	{
	    _data = data;
        UpdateItemList();
	    UpdateBoothLb(data);

        if(_curPos != 0)
            ProxyTrade.ClosePitchSellView();
    }

	private void UpdateItemList()
	{
		ItemCellClick fun = (item, idx) =>
		{
            _disposable.Add(item.OnCellClick.Subscribe(_ =>
            {
                UpdateCurItem();
                var index = _curItemList.FindIndex(d => d.GetIdx == item.GetData().index);
                if (_curItemList.Count >= (_data.SellViewCtrl.Capability - _data.PitchCtrl.GetStallItems.Count())
                && index < 0)
                {
                    TipManager.AddTip("剩余摊位不足");
                    return;
                }
                UpdateItemInfo(item.GetData());
                UpdateCurItemList(item.GetData(), item);
            }));
		};
		
		var itemList = BackpackDataMgr.DataMgr.GetStallDtos();
	    if (itemList == null) return;

		for (int i = _view.ItemGrid_UIGrid.transform.childCount; i < itemList.Count(); i++)
		{
			var item = AddChild<ItemCellController, ItemCell>(_view.ItemGrid_UIGrid.gameObject, ItemCell.NAME);
            item.UpdateView(itemList.TryGetValue(i), false);
            item.SetSelectSprite("ect_select_3", "CommonUIAltas");
            _itemList.Add(item);
            item.SetShowTips(false);
            fun(item, i);
		}

        _view.ItemGrid_UIGrid.Reposition();
	}

	private void UpdateItemInfo(BagItemDto itemDto)
	{
	    var stalldto = DataCache.getDtoByCls<StallGoods>(itemDto.itemId);
        if (stalldto == null)
	    {
            GameDebuger.LogError(string.Format("stallGood表读不到数据{0},检查导表是否异常", itemDto.itemId));
            return;
        }

	    var dto = _curItemList.Find(d => d.GetIdx == itemDto.index || d.GetItemId == itemDto.itemId);

        _curItemPriceBase =
            ExpressionManager.StallGoodsBasePrice(string.Format("itemto{0}{1}{2}", itemDto.index, _price, _cout),
                stalldto.basePriceFormula);
        _saleFactor = dto == null ? 1f : dto.GetSaleFactor;
	    _price = dto == null
	        ? _curItemPriceBase
            : dto.GetPrice;

        _cout = dto == null ? 1 : dto.GetCount;
	    _curPos = itemDto.index;
       
        _handle = stalldto == null ? 0f : stalldto.taxRate;
        UIHelper.SetItemIcon(_view.Icon_UISprite, itemDto.item.icon);
        UIHelper.SetItemQualityIcon(_view.IconBG_UISprite, itemDto.item.quality);
	    _view.NameLb_UILabel.text = itemDto.item.name;

        _maxCount = itemDto.count;

        _curItemId = itemDto.itemId;
        SelectItem(true);
        UpdateCashInfo();
    }

    private void UpdateCurItemList(BagItemDto dto, ItemCellController itemCell)
    {
        var item = _curItemList.Find(d => d.GetIdx == dto.index);
        if (item == null)
        {
            var data = PitchSellViewData.Create(dto.index, _price, _cout, _saleFactor, dto.itemId);
            _curItemList.ReplaceOrAdd(d => d.GetIdx == dto.index, data);
        }
        else
        {
            var idx = _curItemList.FindIndex(d => d.GetIdx == item.GetIdx);
            _curItemList.RemoveAt(idx);

            if (_curItemList.Count > 0)
            {
                var datalist = BackpackDataMgr.DataMgr.GetStallDtos();
                var sellData = _curItemList.TryGetValue(_curItemList.Count - 1);
                var data = datalist.Find(d => d.index == sellData.GetIdx);
                UpdateItemInfo(data);
            }
            else
                SelectItem(false);
        }

        itemCell.isSelect = !itemCell.isSelect;
    }

    private void UpdateCurItem()
    {
        _curItemList.ForEachI((d, idx) =>
        {
            if (d.GetItemId == _curItemId)
            {
                d.GetPrice = _price;
                d.GetCount = _cout;
                d.GetSaleFactor = _saleFactor;
            }
        });
    }

    private void UpdateCashInfo()
    {
        _view.PriceLb_UILabel.text = _price.ToString();
        _view.NumLb_UILabel.text = _cout.ToString();
        _view.TotalLb_UILabel.text = string.Format("{0}", _price * _cout);
        _view.HandleLb_UILabel.text = string.Format("{0}", Mathf.Ceil(_price * _cout * _handle));

        if (_saleFactor > 1f)
            _view.DescLb_UILabel.text = string.Format("{0}%", Mathf.Floor(_saleFactor * 100));
        else
            _view.DescLb_UILabel.text = string.Format("{0}%", Mathf.Ceil(_saleFactor * 100));
    }

    private void SelectItem(bool b)
    {
        _view.TextureGroup.SetActive(!b);
        _view.ContentGroup.SetActive(b);
    }

    public void AddPriceLb(bool isAdd)
    {
        if (_curItemId == 0)
        {
            TipManager.AddTip("请先选择物品");
            return;
        }

        var stalldto = DataCache.getDtoByCls<StallGoods>(_curItemId);
        if (isAdd)
        {
            if((_saleFactor + 0.1f) - stalldto.maxSaleFactor > 0.01f) 
            {
                TipManager.AddTip("已达到最高价格");
                return;
            }
        }
        else
        {
            if ((_saleFactor - 0.1f) - stalldto.minSaleFactor < -0.01f)
            {
                TipManager.AddTip("已达到最低价格");
                return;
            }
        }
        _saleFactor = isAdd ? _saleFactor + 0.1f : _saleFactor - 0.1f;

        if (_saleFactor >= 1)
            _price = (int)Mathf.Floor(_curItemPriceBase * _saleFactor);
        else
            _price = (int)Mathf.Ceil(_curItemPriceBase * _saleFactor);
        UpdateCashInfo();
    }

    public void UpdateCountLb(bool isAdd)
    {
        if (_curItemId == 0)
        {
            TipManager.AddTip("请先选择物品");
            return;
        }


        if (!isAdd && _cout <= 1)
        {
            _view.ReduceNumBtn_UIButton.sprite.isGrey = true;
            return;
        }
        _view.ReduceNumBtn_UIButton.sprite.isGrey = false;
        if (isAdd)
            _cout = _cout + 1 > _maxCount ? _maxCount : _cout + 1;
        else
            _cout = _cout - 1 < 0 ? 0 : _cout - 1;
        UpdateCashInfo();
    }

    private void UpdateBoothLb(ITradeData data)
    {
        _view.BoothLb_UILabel.text = string.Format("{0}/{1}", 
            data.PitchCtrl.GetStallItems.Count(), data.SellViewCtrl.Capability);
    }
}
