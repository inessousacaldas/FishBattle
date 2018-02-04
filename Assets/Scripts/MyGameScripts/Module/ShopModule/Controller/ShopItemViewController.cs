// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ShopItemViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************
using AppDto;
using UniRx;

public interface IShopItemViewController
{
    IObservable<ShopItemViewController.ShopItemClickEvt> OnClickItemStream { get; }
    ShopItemVo GetItemVo { get; }
}
public partial class ShopItemViewController:IShopItemViewController
{
    ShopItemVo _vo;
    ShopItemClickEvt tempClickEvtParam = new ShopItemClickEvt();

    public ShopItemVo GetItemVo { get { return _vo; } }
    public class ShopItemClickEvt
    {
        public ClickType clickType;
        public ShopItemVo vo;
    }
    public enum ClickType
    {
        ClickItem,
        ClickItemIcon,
    }
    public enum ShopSellType
    {

    }
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        ItemBg_UIButtonEvt.Subscribe(_=>
        {
            tempClickEvtParam.clickType = ClickType.ClickItem;
            tempClickEvtParam.vo = _vo;
            clickItemStream.OnNext(tempClickEvtParam);
        });
        Icon_UIButtonEvt.Subscribe(_ =>
        {
            tempClickEvtParam.clickType = ClickType.ClickItemIcon;
            tempClickEvtParam.vo = _vo;
            clickItemStream.OnNext(tempClickEvtParam);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        ItemBg_UIButtonEvt.CloseOnceNull();
        Icon_UIButtonEvt.CloseOnceNull();
    }

    public void UpdateView(ShopItemVo vo,bool isChoice)
    {
        _vo = vo;

        //如果打折后的价格与 原价相同，则不显示打折
        View.price_raw_UILabel.gameObject.SetActive(vo.DiscountPrice != vo.Price);
        //===============如果物品售罄=====
            //TODO:显示售罄遮罩
        View.SellNothing.gameObject.SetActive(vo.RemainNumber == 0);
        View.SellStauteTips_UISprite.gameObject.SetActive(vo.ExpendType == ExpandType.Hot);
        View.SellNewStatue_UISprite.gameObject.SetActive(vo.ExpendType == ExpandType.New);
        View.ChoiceMark_UISprite.gameObject.SetActive(isChoice);

        var priceIconstr = ItemIconConst.GetIconConstByItemId((AppVirtualItem.VirtualItemEnum)vo.ExpendItemId);

        View.price_UILabel.text = priceIconstr + vo.DiscountPrice;
        View.price_raw_UILabel.text = priceIconstr + vo.Price;

        SetIconSprite(vo.Icon);
        View.name_Label_UILabel.text = vo.Name;
        UIHelper.SetItemQualityIcon(View.IconMask_UISprite, vo.quality);
    }

    private void SetIconSprite(string icon)
    {
        var list = icon.Split(':');
        if(list.Length > 1)
            UIHelper.SetItemIcon(View.Icon_UISprite, list[1]);
        else
            UIHelper.SetItemIcon(View.Icon_UISprite, icon);
    }

    Subject<ShopItemClickEvt> clickItemStream = new Subject<ShopItemClickEvt>();
    public IObservable<ShopItemClickEvt> OnClickItemStream
    {
        get { return clickItemStream; }
    }
}
