// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  ItemIconConst.cs
// Author   : willson
// Created  : 2015/1/27
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using VirtualItemEnum = AppDto.AppVirtualItem.VirtualItemEnum;

public class ItemIconConst
{
    public const string Voucher = "#wb1"; //绑定元宝
    public const string Ingot = "#w1"; //元宝
    public const string Silver = "#w2";//银币
    public const string Copper = "#w3";//铜币
    public const string Score = "#w4";//积分
    public const string Contribute = "#w5";//帮贡
    public const string Trophy = "#w6";//奖杯
    public const string ClothFragment = "#w7";//时装碎片
    public const string ExpCurrency = "#w8";//经验货币
    public const string Exp = "#exp1";//经验
    public const string Exp2 = "#exp2";//经验2
	public const string Rmb = "#wrmb";//经验2
	
	public const string Prestige = "#prestige";	//	威望

    public const string VoucherAltas = "ICON_1b"; //绑定元宝
    public const string IngotAltas = "ICON_1"; //元宝
    public const string SilverAltas = "ICON_2";//银币
    public const string CopperAltas = "ICON_3";//铜币
    public const string ScoreAltas = "ICON_4";//积分
    public const string ContributeAltas = "ICON_5";//帮贡
    public const string TrophyAltas = "ICON_6";//奖杯
    public const string ExpCurrencyAltas = "ICON_7";//经验货币
    public const string ExpAltas = "ICON_11";//经验
    public const string Exp2Altas = "ICON_13";//经验2
	public const string RmbAltas = "RMB-little-icon";//经验2

    public const string GrowthFlag_Green = "#gf1";
    public const string GrowthFlag_Blue = "#gf2";
    public const string GrowthFlag_Orange = "#gf3";
    public const string GrowthFlag_Purple = "#gf4";
    public const string GrowthFlag_Red = "#gf5";

    public const string Achievement_Star = "#ac1";
    public const string Achievement_Moon = "#ac2";
    public const string Achievement_Sun = "#ac3";
    public const string Achievement_Crown = "#ac4";
    /*
	 * 根据虚拟物品的type来获取相应的图标名
	*/
    public static string GetIconConstByItemId(VirtualItemEnum itemId)
    {
        if(itemId == VirtualItemEnum.NONE) return string.Empty;

        var idx = IntEnumHelper.getIndex(typeof(VirtualItemEnum), (object)itemId);
        if (idx > 9)
        {
            idx -= 10;
            return string.Format("#ww{0}", (int)itemId);
        }
            
        return idx < 0 ? string.Empty : string.Format("#w{0}", (int)itemId);

    }

    public static bool IsCurrencyItem(VirtualItemEnum itemId)
    {
        return itemId != VirtualItemEnum.NONE && Enum.IsDefined(typeof(VirtualItemEnum), itemId);
    }

    /*
	 * 根据虚拟物品的type来获取 CommonUIAltas 相应的图标名
	*/
    public static string GetVirtualItemIconItemId(VirtualItemEnum itemId)
    {
        var item = DataCache.getDtoByCls<AppVirtualItem>((int)itemId);
        return item == null ? string.Empty : item.icon;
    }

    public static string GetCirculationTypeSprite(RealItem.CirculationType circulationType)
    {
        GameDebuger.TODO(@"if (circulationType == RealItem.CirculationType.Stall)
        {
            return 'h1-transaction-title';
        }
        else");
            if (circulationType == RealItem.CirculationType.Gift)
        {
            return "send";
        }
        else if (circulationType == RealItem.CirculationType.Bind)
        {
            return "specialues";
        }
        else
        {
            return "";
        }
    }
}