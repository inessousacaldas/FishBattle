using UnityEngine;

/// <summary>
/// Color constant.
/// </summary>
public class ColorConstant
{
	//战斗内外名字类，玩家信息， 玩法， 活动名字，道具名
	public const string Color_Name_Str = "39EB3C";
	public static Color Color_Name = ColorExt.HexStrToColor(Color_Name_Str);
	
	//特殊提醒信息，如耗值
	public const string Color_Tip_Str = "fc7b6a";
	public static Color Color_Tip = ColorExt.HexStrToColor(Color_Tip_Str);
	
	//队伍频道信息
	public const string Color_Channel_Team_Str = "d4862f";
	public static Color Color_Channel_Team = ColorExt.HexStrToColor(Color_Channel_Team_Str);
	
	//综合频道信息
	public const string Color_Channel_Zonghe_Str = "a14ad9";
	public static Color Color_Channel_Zonghe = ColorExt.HexStrToColor(Color_Channel_Zonghe_Str);
	
	//帮派频道
	public const string Color_Channel_Guild_Str = "0882d6";
	public static Color Color_Channel_Guild = ColorExt.HexStrToColor(Color_Channel_Guild_Str);
	
	//系统频道
	public const string Color_Channel_System_Str = "d3a017";
	public static Color Color_Channel_System = ColorExt.HexStrToColor(Color_Channel_System_Str);
	
	//称谓
	public const string Color_Title_Str = "4DB0D6";//"1899FF";
	public static Color Color_Title = ColorExt.HexStrToColor(Color_Title_Str);
	
	//当前装备不可用，没激活状态
	public const string Color_UnActive_Str = "b4b5b5";
	public static Color Color_UnActive = ColorExt.HexStrToColor(Color_UnActive_Str);
	
	//名字类（玩家聊天时显示的名字为白色可查信息）
	public const string Color_ChatName_Str = "fff9e3";
	public static Color Color_ChatName = ColorExt.HexStrToColor(Color_ChatName_Str);
	
	//战斗内
	public const string Color_Battle_Str = "d3a017";
	public static Color Color_Battle = ColorExt.HexStrToColor(Color_Battle_Str);
	
	//战斗己方名字
	public const string Color_Battle_Player_Name_Str = "39EB3C";//"5cf37c";
	public static Color Color_Battle_Player_Name = ColorExt.HexStrToColor(Color_Battle_Player_Name_Str);
	
	//战斗敌方名字
	public const string Color_Battle_Enemy_Name_Str = "E7BD37";//"d3a017";
	public static Color Color_Battle_Enemy_Name = ColorExt.HexStrToColor(Color_Battle_Enemy_Name_Str);

	//技能未达到条件提示
	public const string Color_Battle_SkillCanNotUseTip_Str = "ff0000";
	public static Color Color_Battle_SkillCanNotUseTip = ColorExt.HexStrToColor(Color_Battle_SkillCanNotUseTip_Str);

	public const string Color_Battle_SkillCanUseTip_Str = "6F3E1A";
	public static Color Color_Battle_SkillCanUseTip = ColorExt.HexStrToColor(Color_Battle_SkillCanUseTip_Str);

	//小地图_NPC_闲人
	public const string Color_MiniMap_Npc_Idle_Str = "ffec6c";
	public static Color Color_MiniMap_Npc_Idle = ColorExt.HexStrToColor(Color_MiniMap_Npc_Idle_Str);
	
	//小地图_NPC_功能
	public const string Color_MiniMap_Npc_Function_Str = "96f86f";
	public static Color Color_MiniMap_Npc_Function = ColorExt.HexStrToColor(Color_MiniMap_Npc_Function_Str);
	
	//小地图_NPC_区域标识
	public const string Color_MiniMap_Npc_Area_Str = "B25DE8";
	public static Color Color_MiniMap_Npc_Area = ColorExt.HexStrToColor(Color_MiniMap_Npc_Area_Str);
	
	//界面内 小标题及文字描述颜色
	public const string Color_UI_Title_Str = "6f3e1a";
	public static Color Color_UI_Title = ColorExt.HexStrToColor(Color_UI_Title_Str);
	
	//界面内 按钮字体颜色
	public const string Color_UI_Tab_Str = "fff9e3";
	public static Color Color_UI_Tab = ColorExt.HexStrToColor(Color_UI_Tab_Str);
    public const string Color_UI_Tab_N_Selected_Str = "502e10";
	public static Color Color_UI_Tab_N_Selected = ColorExt.HexStrToColor(Color_UI_Tab_N_Selected_Str);

    //提示 物品颜色
    public const string Color_Tip_Item_Str = "0FFF32";  //0c99c7
	public static Color Color_Tip_Item = ColorExt.HexStrToColor(Color_Tip_Item_Str);
	
	//提示 获得货币颜色
	public const string Color_Tip_GainCurrency_Str = "0FFF32";  //209800
	public static Color Color_Tip_GainCurrency = ColorExt.HexStrToColor(Color_Tip_GainCurrency_Str);



    //提示 消耗货币颜色
    public const string Color_Tip_LostCurrency_Str = "fc7b6a";
	public static Color Color_Tip_LostCurrency = ColorExt.HexStrToColor(Color_Tip_LostCurrency_Str);

    //提示 消耗货币足够颜色
    public const string Color_Tips_Enough_Str = "[0c8130]";
    public static Color Color_Tips_Enough = ColorExt.HexStrToColor(Color_Tips_Enough_Str);

    //货币不足的颜色
    public const string Color_Tips_NotEnough_Str = "[FF0000]";
    public static Color Color_Tips_NotEnough = ColorExt.HexStrToColor(Color_Tips_NotEnough_Str);

    //tips 装备特技名字颜色
    public const string Color_Equip_Skill_Str = "e983f5";
    public static Color Color_Equip_Skill = ColorExt.HexStrToColor(Color_Equip_Skill_Str);

    // ui 黄色字体
    public const string Color_UI_Color_1_Str = "ffdd7d";
    public static Color Color_UI_Color_1 = ColorExt.HexStrToColor(Color_UI_Color_1_Str);

    // 排行榜默认字体
    public const string Color_Rank_Str = "502e10";
    public static Color Color_Rank = ColorExt.HexStrToColor(Color_Rank_Str);

    //潜力颜色
    public const string Color_QIANLI_Str = "87F35C";
    public static Color Color_QIANLI = ColorExt.HexStrToColor(Color_QIANLI_Str);

    //合宠预览资质颜色
    public const string COLOR_COMPOUND_PRE_STR = "130101";
    public static Color Color_COMPOUND_PRE = ColorExt.HexStrToColor(COLOR_COMPOUND_PRE_STR);

    // 材料数量颜色
    public const string Color_Item_Enough_Str = "1d8e00";
    public static Color Color_Item_Enough = ColorExt.HexStrToColor(Color_Item_Enough_Str);
    public const string Color_Item_Not_Enough_Str = "c30000";
    public static Color Color_Item_Not_Enough = ColorExt.HexStrToColor(Color_Item_Not_Enough_Str);

    //属性值颜色
    public const string Color_ATT_Str = "130101";
    public static Color Color_ATT = ColorExt.HexStrToColor(Color_ATT_Str);
}