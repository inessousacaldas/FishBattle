// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MoneyInfoItemViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;

public partial class MoneyInfoItemViewController
{
    protected override void AfterInitView()
    {

    }

    protected override void RegistCustomEvent()
    {
        AddBtn_UIButtonEvt.Subscribe(_ =>
        {
            if(_id == AppVirtualItem.VirtualItemEnum.SILVER || _id == AppVirtualItem.VirtualItemEnum.GOLD)
                ProxyExChangeMain.OpenExChangeMain(_id);
            //else
                //打开充值界面
        });

        ShopBtn_UIButtonEvt.Subscribe(_ =>
        {
            ProxyShop.OpenShop(ShopTypeTab.ScoreShop, ShopTypeTab.ScroeShopId);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {
        
    }

    private AppVirtualItem.VirtualItemEnum _id = AppVirtualItem.VirtualItemEnum.NONE;
    public void UpdateView(AppVirtualItem.VirtualItemEnum curId, long moneyCount)
    {
        _id = curId;
        View.AddBtn_UIButton.gameObject.SetActive(true);
        if (curId == AppVirtualItem.VirtualItemEnum.DIAMOND)
        {
            //打开充值界面 todo xjd
            View.AddBtn_UIButton.gameObject.SetActive(false);
            View.CountLbl_UILabel.transform.localPosition = new UnityEngine.Vector3(72, View.CountLbl_UILabel.transform.localPosition.y);
        } 

        //生活积分
        if (curId == AppVirtualItem.VirtualItemEnum.NONE && moneyCount == -1)
        {
            View.Shop_UIButton.gameObject.SetActive(true);
            View.CurrencyObj.SetActive(false);

            View.Name_UILabel.text = "积分商城";
            UIHelper.SetAppVirtualItemIcon(View.Icon_UISprite, curId);
        }
        else
        {
            View.Shop_UIButton.gameObject.SetActive(false);
            View.CurrencyObj.SetActive(true);

            string countStr = moneyCount >= 10000000 ? string.Format("{0}万", moneyCount / 10000) : moneyCount.ToString();
            View.CountLbl_UILabel.text = countStr;
            UIHelper.SetAppVirtualItemIcon(View.Icon_UISprite, curId);
        }
    }
}
