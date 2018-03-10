// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PitchPutawayAgainViewController.cs
// Author   : xush
// Created  : 7/1/2017 5:12:12 PM
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;
using UnityEngine;

public interface IPutawayViewData
{
    long Getuid { get; }
    int GetPrice { get; }
    int GetItemId { get; }
    int GetItemCount { get; }
}

public class PutawayViewData:IPutawayViewData
{
    private long _uid;
    private int _price;
    private int _itemId;
    private int _itemCount;

    public long Getuid { get { return _uid; } }
    public int GetPrice { get { return _price; } }
    public int GetItemId { get { return _itemId; } }
    public int GetItemCount { get { return _itemCount; } }

    public static PutawayViewData Create(long id, int price, int itemId, int itemCount)
    {
        PutawayViewData data = new PutawayViewData();
        data._uid = id;
        data._price = price;
        data._itemId = itemId;
        data._itemCount = itemCount;
        return data;
    }
}

public partial interface IPitchPutawayAgainViewController:ICloseView
{
	void SetViewInfo(StallGoodsDto goodsDto);
	void UpdateCash(bool isAdd);
    UniRx.IObservable<PutawayViewData> GetPutawayHandler { get; }
}

public partial class PitchPutawayAgainViewController
{
    private StallGoodsDto _goodsDto;
    private int _basePrice;
    private CompositeDisposable _disposable;
    private int _precent = 10;

    public static IPitchPutawayAgainViewController Show<T>(
		  string moduleName
		  , UILayerType layerType
		  , bool addBgMask
		  , bool bgMaskClose = true)
		  where T : MonoController, IPitchPutawayAgainViewController
	{
		var controller = UIModuleManager.Instance.OpenFunModule<T>(
				moduleName
				, layerType
				, addBgMask
				, bgMaskClose) as IPitchPutawayAgainViewController;
		
		return controller;        
	}         
	
    private Subject<PutawayViewData> _putawayClickEvt = new Subject<PutawayViewData>();
    public UniRx.IObservable<PutawayViewData> GetPutawayHandler { get { return _putawayClickEvt; } }

	// 界面初始化完成之后的一些后续初始化工作
	protected override void AfterInitView ()
	{
        _disposable = new CompositeDisposable();

    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
	{
        _disposable.Add(OnPutawayBtn_UIButtonClick.Subscribe(_ =>
        {
            float f;
            var b = float.TryParse(_view.PriceLb_UILabel.text, out f);
            var data = PutawayViewData.Create(_goodsDto.stallId, Mathf.CeilToInt(f), _goodsDto.itemId, _goodsDto.amount);
            _putawayClickEvt.OnNext(data);
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

	public void SetViewInfo(StallGoodsDto goodsDto)
	{
        Props props = DataCache.getDtoByCls<GeneralItem>(goodsDto.itemId) as Props;
        _goodsDto = goodsDto;
	    if (goodsDto == null || goodsDto.item == null)
	    {
	        GameDebuger.LogError("goodsDto数据有问题,请检查");
	        return;
	    }
	    _basePrice = ExpressionManager.StallGoodsBasePrice(string.Format("goodsDto{0}", goodsDto.stallId), goodsDto.item.basePriceFormula);
        _precent = goodsDto.price * 10 / _basePrice;
        UIHelper.SetItemIcon(_view.Icon_UISprite, props != null ? props.icon : "");
	    _view.NameLb_UILabel.text = props != null ? props.name : "";
        _view.DescLb_UILabel.text = string.Format("{0}%", Mathf.Ceil(_precent * 10));
        _view.PriceLb_UILabel.text = goodsDto.price.ToString();
        _view.TotalLb_UILabel.text = goodsDto.price.ToString();
        _view.HandleLb_UILabel.text = Mathf.Ceil(goodsDto.price * _goodsDto.item.taxRate).ToString();
        bool outtime = SystemTimeManager.Instance.GetUTCTimeStamp() > goodsDto.expiredTime;
	    _view.PutawayBtn_UIButton.sprite.isGrey = !outtime;
	}

	public void UpdateCash(bool isAdd)
	{
        var stalldto = DataCache.getDtoByCls<StallGoods>(_goodsDto.itemId);
	    if (isAdd && _precent >= stalldto.maxSaleFactor*10)
	    {
            TipManager.AddTip("已达到最高价格");
            return;
        }
        if (!isAdd && _precent <= stalldto.minSaleFactor * 10)
        {
            TipManager.AddTip("已达到最高价格");
            return;
        }

	    _precent = isAdd ? _precent + 1 : _precent - 1;
        _view.DescLb_UILabel.text = string.Format("{0}%", _precent * 10);
        _view.PriceLb_UILabel.text = (_precent * _basePrice / 10).ToString();
        _view.TotalLb_UILabel.text = (_precent * _basePrice / 10).ToString();
    }
}
