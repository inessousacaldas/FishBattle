using System;
using UnityEngine;
using System.Collections;
using AppDto;

public class TradeHelper
{
    public static void OpenLockPitchitem(int capability, Action callback)
    {
        if (callback == null)
        {
            GameDebuger.LogError("没有回调函数");
            return;
        }

        var max = DataCache.GetStaticConfigValue(AppStaticConfigs.STALL_INIT_CAPABILITY, 5); //摊位初始数量
        int cash;
        string menoy;
        AppVirtualItem.VirtualItemEnum itemEnum;
        if (capability - max == 0)
        {
            cash = 2000;
            menoy = "金币";
            itemEnum = AppVirtualItem.VirtualItemEnum.GOLD;
        }
        else if (capability - max == 1)
        {
            cash = 2500;
            menoy = "金币";
            itemEnum = AppVirtualItem.VirtualItemEnum.GOLD;
        }
        else
        {
            cash = 50 * (capability - max - 1);   //前两次用金币开
            menoy = "钻石";
            itemEnum = AppVirtualItem.VirtualItemEnum.DIAMOND;
        }

        var controller = ProxyBaseWinModule.Open();
        var title = "解锁摊位";
        var txt = string.Format("是否花费{0}{1}解锁一个摆摊位？", cash, menoy);
        BaseTipData data = BaseTipData.Create(title, txt, 0, () =>
        {
            if(ModelManager.Player.GetPlayerWealthById((int)itemEnum) < cash)
                ExChangeHelper.CheckIsNeedExchange(itemEnum, cash, () => { callback(); });
            else
                callback();
        }, null);
        controller.InitView(data);
    }

    public static string GetLockTxtByCapability(int capability)
    {
        var max = DataCache.GetStaticConfigValue(AppStaticConfigs.STALL_INIT_CAPABILITY, 5); //摊位初始数量
        int cash;
        string menoy;
        if (capability - max == 0)
        {
            cash = 2000;
            menoy = "#w4";
        }
        else if (capability - max == 1)
        {
            cash = 2500;
            menoy = "#w4";
        }
        else
        {
            cash = 50 * (capability - max - 1);   //前两次用金币开
            menoy = "#w1";
        }
        return string.Format("{0}{1}", menoy, cash);
    }
}
