// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 7/1/2017 10:17:33 AM
// **********************************************************************

using AppDto;

public class ProxyTrade
{
    public static void OpenTradeView(TradeTab tab = TradeTab.Cmomerce, int goodsId = -1)
    {
        TradeDataMgr.TradeNetMsg.OpenTradeView(() =>
        {
            TradeDataMgr.TradeViewLogic.Open(tab, goodsId);
        });
    }

    public static void CloseTradeView()
    {
        UIModuleManager.Instance.CloseModule(TradeView.NAME);
        TradeDataMgr.TradeNetMsg.TradeExit();   //关闭界面通知服务端不再刷新数据
        TradeDataMgr.TradeNetMsg.StallExit();
    }

    public static void OpenPitchPutawayAgainView(StallGoodsDto goodsDto)
    {
        if (goodsDto == null || goodsDto.item == null)
        {
            GameDebuger.LogError("数据有误,请检查");
            return;
        }
        PitchPutawayAgainViewLogic.Open(goodsDto);
    }

    public static void ClosePitchPutawayAgainView()
    {
        UIModuleManager.Instance.CloseModule(PitchPutawayAgainView.NAME);
    }

    public static void OpenPitchSellView()
    {
        TradeDataMgr.PitchSellViewLogic.Open();
    }

    public static void ClosePitchSellView()
    {
        UIModuleManager.Instance.CloseModule(PitchSellView.NAME);
    }

    public static void OpenCmomerceSellView(BagItemDto dto)
    {
        if (dto == null) TipManager.AddTip("异常");
        TradeDataMgr.TradeNetMsg.TradeQuickSell(dto.itemId, (e) =>
        {
            if (e == null)
            {
                TipManager.AddTip("物品不能出售");
                return;
            }
            dto.tradePrice = (int) e.price;
            CmomerceSellViewLogic.Open(dto);
        });
        
    }

    public static void CloseCmomerceSellView()
    {
        UIModuleManager.Instance.CloseModule(CmomerceSellView.NAME);
    }
}

