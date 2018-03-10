// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Zijian
// Created  : 8/24/2017 2:54:17 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class ExChangeMainDataMgr
{
    public static class ExChangeMainNetMsg
    {
        //快捷兑换
        public static void ReqCurrencyConvert(int fromId, int toId, int count, bool isAuto, Action callback=null)
        {
            GameUtil.GeneralReq(Services.ExchangeCurrency_Exchange(fromId, toId, count, isAuto), resp => {
                var scale = ExChangeHelper.GetConvertScale(fromId, toId);
                TipManager.AddTip(string.Format("兑换成功，获得{0}{1}", "#w"+toId, count* scale));
                ProxyExChangeMain.CloseExChangeFast();
                ProxyExChangeMain.CloseExChangeMain();
                GameUtil.SafeRun(callback);

                //本地设置今日 剩余可兑换米拉数量
                if (fromId == (int)AppVirtualItem.VirtualItemEnum.MIRA)
                    ReqGetShop_MiraCount();
            });
        }

        //一般兑换
        public static void Req_Convert_Select(int fromId, int toId, int count)
        {
            GameUtil.GeneralReq(Services.ExchangeCurrency_SelectExchange(fromId, toId, count), resp =>
            {
                TipManager.AddTip("兑换货币成功");
                ProxyExChangeMain.CloseExChangeMain();
            });
        }

        public static void ReqGetShop_MiraCount()
        {
            GameUtil.GeneralReq(Services.ExchangeCurrency_MiraCount(), resp => 
            {
                var dto = resp as MiraConvertCountDto;
                DataMgr._data.SetHasConvertMiraCount(dto.miraCount);
                FireData();
            });
        }
    }
}
