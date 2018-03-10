using System.Collections.Generic;

namespace MyGameScripts.Gameplay.Player
{
    public class GlobalAttr
    {  
        #region 基本常量
        public const int PHYSIQUE = 101; //体质
        public const int POWER = 102;//力量
        public const int SPIRIT = 103;//精神
        public const int MAGIC = 104;//魔力
        public const int MAX_HP = 201;//生命值
        public const int PHY_ATTACK = 202;//物理攻击
        public const int PHY_DEFENSE = 203;//物理防御
        public const int MAGIC_ATTACK = 204;//魔法攻击
        public const int MAGIC_DEFENSE = 205;//魔法防御
        public const int PHY_CRIT = 206;//物理暴击
        public const int MAGIC_CRIT = 207;//魔法暴击
        public const int MAX_EP = 208;//能量值
        public const int PHY_STRIKE = 209;//物理穿透
        public const int MAGIC_STRIKE = 210;//魔法穿透
        public const int EXCEPT_ADDITION = 211;//异常加成
        public const int MAX_CP = 212;//怒气值
        public const int ANTI_CRIT = 213;//抗暴   
        public const int DEXT = 214;//灵活度
        public const int SENS = 215;//敏捷度
        public const int CRIT_ADDITION = 216;//暴击伤害加成
        public const int CRIT_ANTI = 217;//暴击伤害减免
        public const int TREAT = 218;//治疗能力
        public const int SPEED = 219;//行动速度
        public const int TENA = 220;//韧性
        public const int PHY_CRIT_RATE = 301;//物理暴击率
        public const int MAGIC_CRIT_RATE = 302;//魔法暴击率
        public const int TREAT_CRIT_RATE = 303;//治疗暴击率
        public const int PHY_ANTI_CRIT_RATE = 304;//物理抗暴率
        public const int MAGIC_ANTI_CRIT_RATE = 305;//魔法抗暴率
        public const int PHY_HIT_RATE = 306;//物理命中率
        public const int MAGIC_HIT_RATE = 307;//魔法命中率
        public const int PHY_DODGE_RATE = 308;//物理闪避率
        public const int MAGIC_DODGE_RATE = 309;//魔法闪避率
        public const int EXCEPT_HIT_RATE = 310;//异常触发率
        public const int EXCEPT_ANTI_RATE = 311;//异常抵抗率
        public const int PHY_STRIKE_RATE = 312;//物理穿透率
        public const int MAGIC_STRIKE_RATE = 313;//魔法穿透率
        public const int FIXED_RELEASE_TIME = 314;//驱动系数
        public const int FIRE_MAGIC_ADDITION = 315;//火系魔法伤害加成
        public const int WATER_MAGIC_ADDITION = 316;//水系魔法伤害加成
        public const int LAND_MAGIC_ADDITION = 317;//地系魔法伤害加成
        public const int WIND_MAGIC_ADDITION = 318;//风系魔法伤害加成
        public const int TIME_MAGIC_ADDITION = 319;//时系魔法伤害加成
        public const int SKY_MAGIC_ADDITION = 320;//空系魔法伤害加成
        public const int DREAM_MAGIC_ADDITION = 321;//幻系魔法伤害加成
        public const int FIRE_MAGIC_ANIT = 322;//火系魔法伤害减免
        public const int WATER_MAGIC_ANIT = 323;//水系魔法伤害减免
        public const int LAND_MAGIC_ANIT = 324; //地系魔法伤害减免
        public const int WIND_MAGIC_ANIT = 325;//风系魔法伤害减免
        public const int TIME_MAGIC_ANIT = 326;//时系魔法伤害减免
        public const int SKY_MAGIC_ANIT = 327;//空系魔法伤害减免
        public const int DREAM_MAGIC_ANIT = 328;//幻系魔法伤害减免
        public const int PHY_DAMAGE_ADDIDTION = 329; //物伤加成
        public const int MAGIC_DAMAGE_ADDIDTION = 330; //法伤加成 
        public const int PHY_DAMAGE_REDUCE = 331; //物伤减免
        public const int MAGIC_DAMAGE_REDUCE = 332; //法伤减免
        public const int IMPRISON_ADDITION = 333;//封技加成
        public const int TRANCE_ADDITION = 334;//睡眠加成
        public const int ANGER_ADDITION = 335;//气绝加成
        public const int CHAOS_ADDITION = 336; //混乱加成
        public const int FROZEN_ADDITION = 337; //冻结加成
        public const int SILENCE_ADDITION = 338; //封魔加成
        public const int SNEER_ADDITION = 339;//嘲讽加成
        public const int ANTI_IMPRISON = 340; //封技抵抗
        public const int ANTI_TRANCE = 341;//睡眠抵抗
        public const int ANTI_ANGER = 342;//气绝抵抗
        public const int ANTI_CHAOS = 343;//混乱抵抗
        public const int ANTI_FROZEN = 344;//冻结抵抗
        public const int ANTI_SILENCE = 345;//封魔抵抗
        public const int ANTI_SNEER = 346;//嘲讽抵抗 
        public const int HP_ADDITION = 347;//生命加成
        public const int SPEED_ADDITION = 348;//速度加成
        public const int Crew_Grow = 500;//伙伴成长率
        #endregion

        public static readonly Dictionary<int,string> ATTRNAMES = new Dictionary<int, string>
        {
            {PHYSIQUE,"体质"},{POWER, "力量"},{SPIRIT,"精神"},{MAGIC,"魔力"},{MAX_HP,"生命值"},{PHY_ATTACK,"物理攻击"},
            {PHY_DEFENSE,"物理防御"},{MAGIC_ATTACK,"魔法攻击"},{MAGIC_DEFENSE,"魔法防御"},{PHY_CRIT,"物理暴击"},{MAGIC_CRIT,"魔法暴击"}
            ,{MAX_EP,"能量值"},{PHY_STRIKE,"物理穿透"},{MAGIC_STRIKE,"魔法穿透"},{EXCEPT_ADDITION,"异常加成"},{MAX_CP,"怒气值"},
            {ANTI_CRIT,"抗暴"},{DEXT,"灵活度"},{SENS,"敏捷度"},{CRIT_ADDITION,"暴击伤害加成"},{CRIT_ANTI,"暴击伤害减免"},{TREAT,"治疗能力"},
            {SPEED,"行动速度"},{TENA,"韧性"},{PHY_CRIT_RATE,"物理暴击率"},{MAGIC_CRIT_RATE,"魔法暴击率"},{TREAT_CRIT_RATE, "治疗暴击率"},            
            {PHY_ANTI_CRIT_RATE,"物理抗暴率"},{MAGIC_ANTI_CRIT_RATE,"魔法抗暴率"},{PHY_HIT_RATE,"物理命中率"},{MAGIC_HIT_RATE,"魔法命中率"},  
            {PHY_DODGE_RATE,"物理闪避率"},{MAGIC_DODGE_RATE,"魔法闪避率"},{EXCEPT_HIT_RATE,"异常触发率"},{EXCEPT_ANTI_RATE,"异常抵抗率"},
            {PHY_STRIKE_RATE,"物理穿透率"},{MAGIC_STRIKE_RATE,"魔法穿透率"},{FIXED_RELEASE_TIME,"驱动系数"},{FIRE_MAGIC_ADDITION,"火系伤害加成"},
            {WATER_MAGIC_ADDITION,"水系伤害加成"},{LAND_MAGIC_ADDITION,"地系伤害加成"},{WIND_MAGIC_ADDITION,"风系伤害加成"},
            {TIME_MAGIC_ADDITION,"时系伤害加成"},{SKY_MAGIC_ADDITION,"空系伤害加成"},{DREAM_MAGIC_ADDITION,"幻系伤害加成"},
            {FIRE_MAGIC_ANIT,"火系伤害减免"},{WATER_MAGIC_ANIT,"水系伤害减免"},{LAND_MAGIC_ANIT,"地系伤害减免"},
            {WIND_MAGIC_ANIT,"风系伤害减免"},{TIME_MAGIC_ANIT,"时系伤害减免"},{SKY_MAGIC_ANIT,"空系伤害减免"},
            {DREAM_MAGIC_ANIT,"幻系伤害减免"},{PHY_DAMAGE_ADDIDTION,"物伤加成"},{MAGIC_DAMAGE_ADDIDTION,"法伤加成"},{PHY_DAMAGE_REDUCE,"物伤减免"},
            {MAGIC_DAMAGE_REDUCE,"法伤减免"},{IMPRISON_ADDITION,"封技加成"},{TRANCE_ADDITION,"睡眠加成"},{ANGER_ADDITION,"气绝加成"},
            {CHAOS_ADDITION,"混乱加成"},{FROZEN_ADDITION,"冻结加成"},{SILENCE_ADDITION,"封魔加成"},{SNEER_ADDITION,"嘲讽加成"},{ANTI_IMPRISON,"封技抵抗"},
            {ANTI_TRANCE,"睡眠抵抗"},{ANTI_ANGER,"气绝抵抗"},{ANTI_CHAOS,"混乱抵抗"},{ANTI_FROZEN,"冻结抵抗"},{ANTI_SILENCE,"封魔抵抗"},{ANTI_SNEER,"嘲讽抵抗"},
            {Crew_Grow,"成长率"}, {HP_ADDITION,"生命加成"}, {SPEED_ADDITION,"速度加成"}
        };

        //魔法技能id对应图片
        public static readonly Dictionary<int, string> MAGICICON = new Dictionary<int, string>
        {
            {0, "Attribute_bg_fire"},{ 1, "Attribute_bg_fire"}, { 2, "Attribute_bg_water"}, {3, "Attribute_bg_ground" }, {4, "Attribute_bg_wind" },
            { 5, "Attribute_bg_time"}, { 6, "Attribute_bg_space"}, {7, "Attribute_bg_unrele"}
        };

        public static string GetMagicIcon(int key)
        {
            return MAGICICON[key];
        }

        public static string GetAttrName(int key) {
            return ATTRNAMES[key];
        }      

        //伙伴一级属性
        public static readonly int[] PANTNER_BASE_ATTRS =
        {
            POWER, PHYSIQUE, SPIRIT, MAGIC
        };

        //伙伴/玩家 二级属性
        public static readonly int[] SECOND_ATTRS =
        {
            MAX_HP, SPEED, PHY_ATTACK, PHY_DEFENSE, MAGIC_ATTACK, MAGIC_DEFENSE,
            PHY_STRIKE, MAGIC_STRIKE, PHY_CRIT, MAGIC_CRIT, TREAT,EXCEPT_ADDITION
        };

        //伙伴/玩家 二级属性Tips
        public static readonly int[] SECOND_ATTRS_TIPS =
        {
            MAX_HP,PHY_ATTACK,PHY_DEFENSE,MAGIC_ATTACK,MAGIC_DEFENSE,PHY_CRIT,MAGIC_CRIT,
            MAX_EP,PHY_STRIKE,MAGIC_STRIKE,EXCEPT_ADDITION,MAX_CP,ANTI_CRIT,DEXT,SENS,
            CRIT_ADDITION,CRIT_ANTI,TREAT,SPEED,TENA
        };
    }
}