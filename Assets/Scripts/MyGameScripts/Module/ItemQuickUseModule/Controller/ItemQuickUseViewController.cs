// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ItemQuickUseViewController.cs
// Author   : xush
// Created  : 3/3/2018 3:30:06 PM
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;

public partial interface IItemQuickUseViewController
{
    UniRx.IObservable<BagItemDto> UseBtnHandler { get; }
}

public partial class ItemQuickUseViewController
{
    private CompositeDisposable _disposable = new CompositeDisposable();
    private BagItemDto _itemDto;
    private Subject<BagItemDto> _useBtnEvt = new Subject<BagItemDto>();
    public UniRx.IObservable<BagItemDto> UseBtnHandler { get { return _useBtnEvt; } } 
     
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
            
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
	{
	    _disposable.Add(UseBtn_UIButtonEvt.Subscribe(_ => { _useBtnEvt.OnNext(_itemDto); }));
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
    protected override void UpdateDataAndView(IItemQuickUseData data)
    {
        if(data.CurItemDto != null)
            SetItemInfo(data.CurItemDto);
        else
            ProxyItemQuickUse.CloseItemQuickItemView();
    }

    private void SetItemInfo(BagItemDto dto)
    {
        _itemDto = dto;
        _view.NameLb_UILabel.text = dto.item.name;
        UIHelper.SetItemIcon(_view.IconSprite_UISprite, dto.item.icon);
        UIHelper.SetItemQualityIcon(_view.IconSpriteBg_UISprite, dto.item.quality);
        if (dto.item.itemType == (int) AppItem.ItemTypeEnum.Equipment)
            _view.BtnNameLb_UILabel.text = "装备";
        else
            _view.BtnNameLb_UILabel.text = "使用";
    }

}
