// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Zijian
// Created  : 8/17/2017 2:13:45 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;
using System.Linq;
using UnityEngine;

public sealed partial class ShopDataMgr
{
    public static class ShopNetMsg
    {
        /// <summary>
        /// 获取商城相关的信息，只限商城
        /// </summary>
        public static void ReqGetAllShopInfo(
            ShopTypeTab tab, 
            ShopTypeTab shopId = ShopTypeTab.LimitShopId, 
            Action onSuccess = null)
        {
            GameUtil.GeneralReq(Services.Shop_AllInfo(), response =>
            {
                AllShopInfoDto all_shop = response as AllShopInfoDto;
                DataMgr._data.UpdateAllShop(all_shop,false);

                DataMgr._data.SetCurShopTab((int)shopId);
                DataMgr._data.SetCurShopId(DataMgr._data.ShopTypeIdMap[(int)shopId][0].EnumValue);
                DataMgr._data.SetCurSelectShopVo(DataMgr._data.CurShopInfo.ShopItems.FirstOrDefault());
                FireData();

                if (onSuccess != null)
                    onSuccess();
            });
        }
        public static void ReqBuyShop(ShopInfoData shopInfoData,ShopGoods shop,int count)
        {
            GameUtil.GeneralReq(Services.Shop_Buy(shop.id,count, shopInfoData.RefreshTime_Raw),resp=>{
                ShopGoodsDto dto = resp as ShopGoodsDto;
                DataMgr._data.UpdateSingleShopGood(shopInfoData.ShopInfo.shopType, dto);
                int newSelectCount = dto.limitNum - dto.num <= 0 ? 0 : 1;
                if (dto.limitNum == 0)
                    newSelectCount = 1;
                DataMgr._data.CurSelectCount = newSelectCount;
                TipManager.AddTopTip(string.Format("您已经成功购买{0}个{1}",count,shop.goodsName));
                FireData();
            });
        }

        public static void ReqGetShopInfo(int shopId)
        {
            GameUtil.GeneralReq(Services.Shop_Info(shopId), resp => 
            {
                ShopInfoDto shopInfo = resp as ShopInfoDto;
                DataMgr._data.UpdateSingleShop(shopInfo);
                FireData();
            });
        }

        public static void ReqShopReset(int shopId)
        {
            GameUtil.GeneralReq<ShopInfoDto>(Services.Shop_Reset(shopId), resp =>
            {
                DataMgr._data.UpdateSingleShop(resp);
                TipManager.AddTopTip("商店重置成功");
                FireData();
            });
        }
    }
}
