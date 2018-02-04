// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  CurrencyExchange.cs
// Author   : willson
// Created  : 2015/1/19 
// Porpuse  : 
// **********************************************************************
using System;
using AppDto;
using UnityEngine;

public class CurrencyExchange
{
    // 元宝转铜币
	public static long IngotToCopper(long ingot)
	{
        return (long)Math.Ceiling(ingot * 1.0 * (ModelManager.Player.ServerGrade * DataCache.GetStaticConfigValue(AppStaticConfigs.INGOT_CONVERT_COPPER_FACTOR1) + DataCache.GetStaticConfigValue(AppStaticConfigs.INGOT_CONVERT_COPPER_FACTOR2)));
	}

    public static int RedPacketIngotToCopper(int ingot)
    {
        return Mathf.FloorToInt(ingot * 1.1f * (ModelManager.Player.ServerGrade * DataCache.GetStaticConfigValue(AppStaticConfigs.INGOT_CONVERT_COPPER_FACTOR1) + DataCache.GetStaticConfigValue(AppStaticConfigs.INGOT_CONVERT_COPPER_FACTOR2)));
    }

    // 铜币转元宝
    public static int CopperToIngot(long copper)
	{
		// (SLV*1000+10000)
        double value = copper * 1.0 / (ModelManager.Player.ServerGrade * DataCache.GetStaticConfigValue(AppStaticConfigs.INGOT_CONVERT_COPPER_FACTOR1) + DataCache.GetStaticConfigValue(AppStaticConfigs.INGOT_CONVERT_COPPER_FACTOR2));
        return (int)Math.Ceiling(value);
	}

    // 银币转元宝
	public static int SilverToIngot(int silver)
	{
        return (int)Math.Ceiling(silver * 1.0/DataCache.GetStaticConfigValue(AppStaticConfigs.INGOT_CONVERT_SILVER));
	}

    // 帮贡转元宝
	public static int ContributeToCopper(int contribute)
	{
        return contribute * DataCache.GetStaticConfigValue(AppStaticConfigs.ASSIST_SKILL_CONTRIBUTE_TO_COPPER);
	}

    // 把玩家缺少物品,虚拟物品转元宝
    public static int ItemNeedToIngot(int itemId, int needCount, out int costCount)
    {
        costCount = needCount;
        int _ingot = 0;
        GameDebuger.TODO(@"if (itemId < 100)
        {
            //GeneralItem value = DataCache.getDtoByCls<GeneralItem>(itemId);
            if (itemId == AppVirtualItem.VirtualItemEnum_COPPER)
            {
                // 铜币 不足
                if (ModelManager.Player.isEnoughCopper(needCount) == false)
                {
                    costCount = (int)ModelManager.Player.GetPlayerWealth().copper;
                    long needCopper = needCount - ModelManager.Player.GetPlayerWealth().copper;
                    _ingot = CurrencyExchange.CopperToIngot(needCopper);
                }
            }
            else if (itemId == AppVirtualItem.VirtualItemEnum_SILVER)
            {
                // 银币 不足
                if (ModelManager.Player.isEnoughSilver(needCount) == false)
                {
                    costCount = ModelManager.Player.GetPlayerWealth().silver;
                    long needCopper = needCount - ModelManager.Player.GetPlayerWealth().silver;
                    _ingot = CurrencyExchange.SilverToIngot((int)needCopper);
                }
            }
        }
        else
        {
            int itemCount = ModelManager.Backpack.GetItemCount(itemId);
            Props value = DataCache.getDtoByCls<GeneralItem>(itemId) as Props;
            if (itemCount < needCount)
            {
                costCount = itemCount;
                _ingot = value.buyPrice * (needCount - itemCount);
            }
        }");
        return _ingot;
    }
}
