// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 1/10/2018 2:11:21 PM
// **********************************************************************

using System.Collections.Generic;
public static class GuildMainHelper
{

    private const string GuildGradeShortDes = "提升公会等级上限，提高公会建筑等级上限";
    private const string GuildBurPubShortDes = "扩大公会人数上限，增加管理人员数量";
    private const string GuildTreasuryShortDes = "提升公会资金存储上限、提升公会宝箱等级上限、提升公会工资";
    private const string GuildGuardTowerShortDes = "提高发现猎兵团的星级";
    private const string GuildWorkshopShortDes = "增加公会商店出售的商品、提高公会捐赠时候获得的公会资金和贡献";

    private const string LvGuildGradeDes = "公会等级\t{0}级\n其他建筑等级上限\t{1}级";
    private const string LvGuildBurPubDes = "公会成员上限\t{0}人\n副会长\t{1}人\n高级官员\t{2}人\n官员\t{3}人\n特色职位\t{4}人";
    private const string LvGuildTreasuryDes = "公会资金上限\t{0}级";
    private const string LvGuildGuardTowerDes = "提高发现猎兵团的星级";
    private const string LvGuildWorkshopDes = "增加公会商店出售商品类型\n提示捐献物资时获得的公会资金和公会贡献";

    //按顺序 等级，酒馆，金库，哨塔，工坊
    private static List<string> shortDesList = new List<string>() { "",GuildGradeShortDes, GuildBurPubShortDes, GuildTreasuryShortDes, GuildGuardTowerShortDes, GuildWorkshopShortDes };
    private static List<string> lvDesList = new List<string>() { "",LvGuildGradeDes, LvGuildBurPubDes, LvGuildTreasuryDes, LvGuildGuardTowerDes, LvGuildWorkshopDes };

    public static List<string> ShortDesList { get { return shortDesList; } }
    public static List<string> LvDesList { get { return lvDesList; } }
    
    public static string ValidateStrLength(this string str, int min = 4, int max = 10,bool guildName = true)
    {
        int length = AppStringHelper.GetGBLength(str);
        GameDebuger.Log(string.Format("ValidateStrLength str={0} len={1} min={2} max={3}", str, length, min, max));
        if (length < min)
        {
            if (guildName)
                return "公会名不符合要求";
            else
                return "公会对外宣言不符合要求";
        }

        if (length > max)
        {
            if (min == max)
            {
                return "输入文字长度不符合要求";
            }
            if (guildName)
                return "公会名不符合要求";
            else
                return "公会对外宣言不符合要求";
        }
        return null;
    }
}

