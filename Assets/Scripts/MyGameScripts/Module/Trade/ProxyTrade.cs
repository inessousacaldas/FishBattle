// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 7/1/2017 10:17:33 AM
// **********************************************************************

using AppDto;

public class ProxyTrade
{
    public static void OpenTradeView()
    {
        TradeDataMgr.TradeNetMsg.OpenTradeView(() =>
        {
            TradeDataMgr.TradeViewLogic.Open();
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
}

