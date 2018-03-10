using AppDto;

public static class QuartzHelper
{
    private static string[] romaNum = {"", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };

    public static string GetItemName(BagItemDto bagItem)
    {
        var extra = bagItem.extra as QuartzExtraDto;
        if (extra == null) return string.Empty;

        Skill skill;
        string str;

        //潜规则：当basePropertise.count == 0时,把被动技能名放到名称中显示         ---xush
        //潜规则：当basePropertise.count > 0时,basePropertise[0]代表名称中的属性类型显示出来   
        if (extra.baseProperties.Count == 0)
        {
            skill = DataCache.getDtoByCls<Skill>(extra.passiveSkill);
            str = skill == null ? "" : skill.name;
        }
        else
        {
            var propId = extra.baseProperties[0].propId;
            var prop = DataCache.getDtoByCls<QuartzBaseProperty>(propId);
            if (prop == null)
            {
                GameDebuger.LogError(string.Format("QuartzBaseProperty表不存在{0},请检查", propId));
                return "";
            }
            str = prop.name;
        }

        string nameStr;
        if (extra.strengGrade == 0)
        {
            nameStr = string.Format("{0}·{1}",    //名称格式：配表中的名称,属性类型,强化等级
            bagItem.item.name,
            str);
        }
        else
        {
            nameStr = string.Format("{0}·{1} {2}",    //名称格式：配表中的名称,属性类型,强化等级
            bagItem.item.name,
            str,
            ("+" + extra.strengGrade).WrapColor(ColorConstantV3.Color_MissionGreen_Str));
        }
        return nameStr;
    }
}
