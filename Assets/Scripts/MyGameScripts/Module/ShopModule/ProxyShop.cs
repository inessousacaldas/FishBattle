// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : $Author$
// Created  : 8/17/2017 2:13:45 PM
// **********************************************************************

public class ProxyShop
{
    /// <summary>
    /// 打开商城
    /// </summary>
    public static void OpenShop(ShopTypeTab tab = ShopTypeTab.LimitShop, 
        ShopTypeTab shopId = ShopTypeTab.LimitShopId, 
        int selectShopGoodId = -1)
    {
        ShopDataMgr.ShopMainViewLogic.Open(tab, shopId, selectShopGoodId);
    }

    /// <summary>
    /// 单独打开一个类别的商店
    /// </summary>
    /// <param name="ShopType"></param>
    public static void OpenShopByType(int ShopType,int selectShopGoodId = -1,int selectCount = 1)
    {
        ShopDataMgr.ShopMainViewLogic.OpenSystemShop(ShopType, selectShopGoodId, selectCount);
    }
}

