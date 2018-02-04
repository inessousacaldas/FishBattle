using UnityEngine;
using System.Collections.Generic;
using UniRx;
using AppDto;
using VirtualItemEnum = AppDto.AppVirtualItem.VirtualItemEnum;
using System;

public static class ExChangeHelper
{
    //检测财富是否足够
    public static long CheckWealthIsEnough(VirtualItemEnum virtualId, int price)
    {
        //1.获取BuyItem的价格 与自身价格对比 获取 缺少货币的价格
        var playerInfo = PlayerModel.Stream.LastValue;
        long ownerMoney = playerInfo.GetPlayerWealth(virtualId);
        if (virtualId == VirtualItemEnum.BINDDIAMOND)
            ownerMoney = playerInfo.GetPlayerWealth(VirtualItemEnum.BINDDIAMOND) + playerInfo.GetPlayerWealth(VirtualItemEnum.DIAMOND);

        long lackNumber = price - ownerMoney;
        return lackNumber;
    }

    //是否需要兑换
    public static bool CheckIsNeedExchange(VirtualItemEnum virtualId, int price, Action callback=null)
    {
        //货币够 仅银币和金币米拉可以兑换
        var lackNum = CheckWealthIsEnough(virtualId, price);
        if (lackNum <= 0 || 
            (virtualId != VirtualItemEnum.MIRA && virtualId != VirtualItemEnum.SILVER && virtualId != VirtualItemEnum.GOLD))
        {
            GameUtil.SafeRun(callback);
            return false;
        }
            
        //钻石和绑钻 打开充值
        //if(virtualId == (int)VirtualItemEnum.DIAMOND || virtualId == (int)VirtualItemEnum.BINDDIAMOND)
        //{

        //    return false;
        //}

        if (!ExChangeMainDataMgr.DataMgr.GetIsShowFastView((int)virtualId))
        {
            var scale = ExChangeHelper.GetConvertScale((int)VirtualItemEnum.MIRA, (int)virtualId);
            var count = (int)Math.Ceiling((float)price / (float)scale);
            ExChangeMainDataMgr.ExChangeMainNetMsg.ReqCurrencyConvert((int)VirtualItemEnum.MIRA, (int)virtualId, count, true, callback);
            return true;
        }

        var ctrl = ExChangeFastViewController.Show<ExChangeFastViewController>(ExChangeFastView.NAME, UILayerType.FiveModule, true, false);
        ctrl.UpdateView((int)virtualId, lackNum, callback);
        return true;
    }

    public static int GetConvertScale(int fromId, int toId)
    {
        var convertData = DataCache.getDtoByCls<ExchangeCurrency>(fromId);
        var Conversion = convertData.convert.Find(x => x.id == toId);
        int scale = Conversion == null ? 0 : Conversion.count;

        return scale;
    }
}
