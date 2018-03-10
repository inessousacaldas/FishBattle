// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 1/3/2018 3:27:06 PM
// **********************************************************************

public class ProxyItemQuickBuy
{
    public static void OpenQuickBuyView(int itemId)
    {
        ItemQuickBuyDataMgr.ItemQuickBuyNetMsg.RemainQuantity(itemId, () =>
        {
            ItemQuickBuyDataMgr.ItemQuickBuyViewLogic.Open(itemId);
        });
    }
}

