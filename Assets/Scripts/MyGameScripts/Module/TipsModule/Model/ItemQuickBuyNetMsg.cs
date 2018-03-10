// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 1/3/2018 3:25:55 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class ItemQuickBuyDataMgr
{
    public static class ItemQuickBuyNetMsg
    {
        //请求快捷购买的数据
        //ShopGoodsListDto
        public static void RemainQuantity(int itemId, Action callback)
        {
            ServiceRequestAction.requestServer(Services.Shop_RemainQuantity(itemId), "RemainQuantity", e =>
            {
                var shopList = e as ShopGoodsListDto;
                if (shopList == null)
                {
                    GameDebuger.LogError("服务器下发的数据有误,请检查");
                    return;
                }

                DataMgr._data.ShopGoodsList = shopList;
                if (callback != null)
                    callback();
            });
        }

        public static void QuickBuyItem(int goodsId, int count)
        {
            var time = SystemTimeManager.Instance.GetUTCTimeStamp();
            GameUtil.GeneralReq(Services.Shop_Buy(goodsId, count, time), resp =>
            {
                TipManager.AddTip("购买成功!");
                ShopGoodsDto dto = resp as ShopGoodsDto;
                DataMgr._data.ShopGoodsList.shopGoodsDtos.ReplaceOrAdd(d => d.goodsId == dto.goodsId, dto);
                FireData();
            });
        }
    }
}
