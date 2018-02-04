// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PitchItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;
using UnityEngine;

public partial interface IPitchItemController
{
    void IsSelect(bool b);
    int GetItemId();
}

public partial class PitchItemController
{
    private CompositeDisposable _disposable;
    private TradeMenu _tradeMenu;
    private int _itemId;
    #region Sub
    private Subject<TradeMenu> _itemClickEvt = new Subject<TradeMenu>();
    public UniRx.IObservable<TradeMenu> GetItemClick { get { return _itemClickEvt; } }  
    #endregion
    
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
        _view.SelectSpr_UISprite.gameObject.SetActive(false);
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        _disposable.Add(OnPitchItem_UIButtonClick.Subscribe(_=> _itemClickEvt.OnNext(_tradeMenu)));
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void HideDragScrollView(bool b)
    {
        _view.PitchItem_UIDragScrollView.enabled = b;
    }

    public void UpdateItemNull()
    {
        _view.IconSpr_UISprite.gameObject.SetActive(false);
        _view.AddBtn_UIButton.gameObject.SetActive(false);
        _view.CashLb_UILabel.text = "";
        _view.LittleIcon_UISprite.gameObject.SetActive(false);
        _view.NameLb_UILabel.text = "";
        _view.LockBtn_UIButton.gameObject.SetActive(false);
        _view.CashIcon_UISprite.gameObject.SetActive(false);
        _view.State_UISprite.gameObject.SetActive(false);
        _view.DescLb_UILabel.gameObject.SetActive(false);
        _view.Arrow_UISprite.gameObject.SetActive(false);
        _view.CountLb_UILabel.text = "";
        _view.LockLb_UILabel.gameObject.SetActive(false);
        UIHelper.SetItemQualityIcon(_view.Icon_UISprite, 3);
    }

    public void IsSelect(bool b)
    {
        _view.SelectSpr_UISprite.gameObject.SetActive(b);
    }

    public void SetItemSize(int width)
    {
        var y = _view.LockBtn_BoxCollider.size.y;
        _view.LockBtn_BoxCollider.size = new Vector3(width, y, 0f);
        _view.Background_UISprite.width = width;
        _view.SelectSpr_UISprite.width = width + 11;
        _view.UIWidget.width = width;
        _view.BoxCollider.size = new Vector3(width, y, 0f);
    }

    public void SetLockBoxColliderPos(int lockPos, int btnPos)
    {
        var pos = _view.LockBtn_BoxCollider.center;
        _view.LockBtn_BoxCollider.center = new Vector3(lockPos, pos.y, 0);
        _view.BoxCollider.center = new Vector3(btnPos, pos.y, 0f);
    }

    public int GetItemId()
    {
        return _itemId;
    }

    #region 摆摊
    private void SetItemInfo(StallGoodsDto dto)
    {
        if (dto == null)
        {
            UpdateItemNull();
            return;
        }

        _itemId = dto.itemId;
        var item = DataCache.getDtoByCls<GeneralItem>(dto.itemId) as Props;
        _view.IconSpr_UISprite.gameObject.SetActive(true);
        UIHelper.SetItemIcon(_view.IconSpr_UISprite, item == null ? "" : item.icon);
        UIHelper.SetItemQualityIcon(_view.Icon_UISprite, item == null ? 3 : item.quality);
        _view.NameLb_UILabel.text = item == null ? "" : item.name;
        _view.NameLb_UILabel.transform.localPosition = new Vector3(86, 20, 0);
        _view.CashLb_UILabel.text = dto.price.ToString();
        _view.CashIcon_UISprite.gameObject.SetActive(true);
        UIHelper.SetAppVirtualItemIcon(_view.CashIcon_UISprite, AppVirtualItem.VirtualItemEnum.SILVER);

        _view.State_UISprite.gameObject.SetActive(dto.amount == 0);
        _view.State_UISprite.spriteName = dto.amount == 0 ? "ect_SellOut" : "";
        _view.IconSpr_UISprite.isGrey = dto.amount == 0;

        var stallgoods = DataCache.getDtoByCls<StallGoods>(dto.itemId);
        if (stallgoods == null)
        {
            GameDebuger.LogError(string.Format("StallGoods表找不到{0},请检查", dto.itemId));
            return;
        }
        var menu = DataCache.getDtoByCls<TradeMenu>(stallgoods.tradeMenuId);
        if (menu == null)
        {
            GameDebuger.LogError(string.Format("TradeMenu表找不到{0},请检查", stallgoods.tradeMenuId));
            return;
        }

        if (menu.parentId == 2200 || menu.parentId == 2000)
            _view.CountLb_UILabel.text = string.Format("美味度{0}", item == null ? "": (dto.extra as PropsExtraDto).rarity.ToString());
        else
            _view.CountLb_UILabel.text = dto.amount > 0 ? dto.amount.ToString() : "";
    }

    public void SetBuyItemInfo(StallGoodsDto dto)
    {
        SetItemInfo(dto);
    }

    public void SetSellItemInfo(StallGoodsDto dto)
    {
        SetItemInfo(dto);
        _view.AddBtn_UIButton.gameObject.SetActive(dto == null);
        var outTime = SystemTimeManager.Instance.GetUTCTimeStamp() > dto.expiredTime;
        
        if (dto.amount == 0) //售罄
        {
            _view.State_UISprite.gameObject.SetActive(true);
            _view.State_UISprite.spriteName = "ect_SellOut";
            _view.IconSpr_UISprite.isGrey = true;
        }
        else if (dto.count > 0 && dto.amount != 0)  //有物品被出售
        {
            _view.State_UISprite.gameObject.SetActive(true);
            _view.State_UISprite.spriteName = "ect_GetMoney";
            _view.IconSpr_UISprite.isGrey = false;
        }
        else if (outTime)
        {
            _view.State_UISprite.gameObject.SetActive(true); //超时
            _view.State_UISprite.spriteName = "ect_OverTime";
            _view.IconSpr_UISprite.isGrey = true;
        }
        else
        {
            _view.State_UISprite.gameObject.SetActive(false);
            _view.IconSpr_UISprite.isGrey = false;
        }
    }

    public void SetMenuInfo(TradeMenu menu, int propsId = -1)
    {
        UpdateItemNull();
        if (menu == null) return;

        _tradeMenu = menu;
        _view.IconSpr_UISprite.gameObject.SetActive(true);
        if (propsId > 0)
        {
            var item = DataCache.getDtoByCls<GeneralItem>(propsId) as Props;
            UIHelper.SetItemIcon(_view.IconSpr_UISprite, item == null ? "" : item.icon);
        }
        _view.NameLb_UILabel.text = menu.name;
        _view.NameLb_UILabel.transform.localPosition = new Vector3(86, 0, 0);
    }

    public void ShowAddBtn(bool b)
    {
        _view.AddBtn_UIButton.gameObject.SetActive(b);
    }

    public void ShowLockBtn(bool b)
    {
        _view.LockBtn_UIButton.gameObject.SetActive(b);
    }

    public void SetLockLb(string txt)
    {
        _view.LockLb_UILabel.gameObject.SetActive(true);
        _view.LockLb_UILabel.text = string.Format("{0}解锁", txt);
    }
    #endregion

    #region 商会
    public void SetTradeItem(TradeGoodsDto dto)
    {
        UpdateItemNull();
        if (dto == null) return;

        _itemId = dto.itemId;
        _view.IconSpr_UISprite.gameObject.SetActive(true);
        _view.CashIcon_UISprite.gameObject.SetActive(true);
        _view.DescLb_UILabel.gameObject.SetActive(true);

        var tradeGoods = DataCache.getDtoByCls<GeneralItem>(dto.itemId);
        if (tradeGoods == null)
        {
            GameDebuger.LogError(string.Format("GeneralItem表找不到{0},请检查", dto.itemId));
            return;
        }
        UIHelper.SetItemIcon(_view.IconSpr_UISprite,tradeGoods.icon);
        if (tradeGoods is AppItem)
            UIHelper.SetItemQualityIcon(_view.Icon_UISprite, (tradeGoods as AppItem).quality);
        else
            UIHelper.SetItemQualityIcon(_view.Icon_UISprite, 3);

        _view.NameLb_UILabel.text = tradeGoods.name;
        _view.NameLb_UILabel.transform.localPosition = new Vector3(86, 20, 0);
        _view.DescLb_UILabel.text = dto.item.labelParams;
        _view.CashLb_UILabel.text = Mathf.Ceil(dto.price).ToString();

        var price = dto.price - dto.originalPrice;
        _view.Arrow_UISprite.spriteName = price < 0 ? "ect_Arrow_down" : "ect_Arrow_up";
        var addPrice = price/dto.price*100;
        //小于0.01%不显示
        if (addPrice < 0.01 && addPrice > -0.01)
        {
            _view.Arrow_UISprite.gameObject.SetActive(false);
            _view.PriceLb_UILabel.gameObject.SetActive(false);
            _view.PriceLb_UILabel.text = "";
        }
        else
        {
            _view.Arrow_UISprite.gameObject.SetActive(true);
            _view.PriceLb_UILabel.gameObject.SetActive(true);
            _view.PriceLb_UILabel.text = price < 0f
                ? string.Format("{0}%", Math.Abs(price/dto.price*100).ToString("#0.00"))
                    .WrapColor(ColorConstantV3.Color_Red_Str)
                : string.Format("{0}%", Math.Abs(price/dto.price*100).ToString("#0.00"))
                    .WrapColor(ColorConstantV3.Color_Green_Str);
        }

        UIHelper.SetAppVirtualItemIcon(_view.CashIcon_UISprite, AppVirtualItem.VirtualItemEnum.GOLD);
    }
    #endregion
}
