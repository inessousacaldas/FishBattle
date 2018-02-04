using System;
using UnityEngine;

namespace Assets.Scripts.MyGameScripts.UI
{
    public class GameColor {
        public static readonly Color HUNT_COLOR_START = new Color(17 / 255f, 17 / 255f, 17 / 255f, 255 / 255f);
        public static readonly Color HUNT_LIGHT_START = new Color(150 / 255f, 150 / 255f, 150 / 255f, 255 / 255f);
        public static readonly Color HUNT_COLOR = new Color(66 / 255f, 61 / 255f, 61 / 255f, 255 / 255f);
        public static readonly Color HUNT_LIGHT = new Color(144 / 255f, 68 / 255f, 68 / 255f, 255 / 255f);

        public static readonly Color BUF_POISON_LIGHT = new Color(109 / 255f, 176 / 255f, 106 / 255f, 234 / 255f);//毒
        public static readonly Color BUF_POISON_ADD = new Color(109 / 255f, 176 / 255f, 106 / 255f, 234 / 255f);

        public static readonly Color BUF_FREEZE_LIGHT = new Color(106 / 255f, 124 / 255f, 176 / 255f, 234 / 255f);//冰
        public static readonly Color BUF_FREEZE_ADD = new Color(17 / 255f, 96 / 255f, 174 / 255f, 0 / 255f);

        public static readonly Color BUF_PARALYSIS_LIGHT = new Color(149 / 255f, 106 / 255f, 176 / 255f, 234 / 255f);//雷
        public static readonly Color BUF_PARALYSIS_ADD = new Color(68 / 255f, 17 / 255f, 174 / 255f, 0 / 255f);

        public static readonly Color BUF_BURNING_LIGHT = new Color(176 / 255f, 106 / 255f, 119 / 255f, 234 / 255f);//毒
        public static readonly Color BUF_BURNING_ADD = new Color(174 / 255f, 17 / 255f, 26 / 255f, 0 / 255f);

        public static readonly Color BUF_MONSTER_WEAK_LIGHT = new Color(205 / 255f, 225 / 255f, 20 / 255f, 255 / 255f);//虚弱
        public static readonly Color BUF_MONSTER_WEAK_ADD = new Color(79 / 255f, 69 / 255f, 16 / 255f, 255 / 255f);

        public const int WHITE_COLOR = 1;
        public const int GREEN_COLOR = 2;
        public const int BLUE_COLOR = 3;
        public const int PURPLE_COLOR = 4;
        public const int ORANGE_COLOR = 5;
        public const int GOLD_COLOR = 6;
        public static readonly string[] COLOR_STR = new string[7] { "无", "白", "绿", "蓝", "紫", "橙", "金" };
        public static readonly uint[] COLOR_VALUES = new uint[7] { 0xffffff, 0xffffff, 0x86ff05, 0x05cdff, 0xff00fc, 0xff9600, 0xffea05 };
        public static readonly Color[] COLOR_RGB_VALUES = {
            Color.white, Color.white, new Color(114/255f, 1f, 5/255f), new Color(5/255f, 205/255f, 1f),
            new Color(1, 0, 252/255f), new Color(1, 150/255f, 0), new Color(1,234/255f,5/255f)
        };
        public static readonly string[] HTML_COLORS = new string[7] { "ffffff", "ffffff", "86ff05", "05cdff", "ff00fc", "ff9600", "ffea05" };
        public static readonly string[] countryColor = new string[3] { "00FF00", "F600FF", "00CCFF" };
        public static readonly string[] COLOR_STR_2 = new string[7] { "Gray", "White", "Green", "Blue", "Purple", "Orange", "Golden" };
        public static readonly string[] COLOR_SKILL = new string[] { "86FF05", "0184ff", "b301df", "ba6d00", "c09100" };
        public static readonly string[] COLOR_SKILL_OUTLINE = new string[] { GREEN, BLUE, PURPLE, ORANGE, GOLD };
        public const string systemColor = "c8ef1d";

        public const string WHITE = "ffffff";
        public const string BLACK = "000000";
        public const string GREEN = "86FF05";
        public const string BLUE = "05CDFF";
        public const string PURPLE = "FF00FC";
        public const string ORANGE = "FF9600";
        public const string GOLD = "FFEA05";
        public const string YELLOW = "fff000";
        public const string RED = "ff0000";
        public const string GARY = "737373";

        public const string MONEY_0 = "fff000";
        public const string EXP_0 = "aaff00";
        public const string ICON_GREEN = "aaff00";

        public const string PANEL_BLUE_0 = "1e7ed0";
        public const string PANEL_BLUE_1 = "1792a8";
        public const string PANEL_BLUE_2 = "00ffff";
        public const string PANEL_BLUE_3 = "1b558b";
        public const string PANEL_BLUE_4 = "1b6fbe";
        public const string PANEL_BLUE_5 = "0cc3c4";

        public const string PANEL_BROWN_0 = "d3bb70";

        public const string NPC_NAME = "ffe100";
        public const string ROLE_NAME = "ffee33";
        public const string ROLE_NAME_GRAY = "c9c9c9";
        public const string ROLE_NAME_RED = "ff0000";
        public const string ROLE_NAME_DEAD = "5a5aad";
        public const string ROLE_BATTLE_ENEMY = "fc01b8";
        public const string ROLE_BATTLE_ENEMY_DEAD = "FFFFFF";
        public const string ROLE_TITLE = "ff00fc";
        public const string ROLE_FAMILY = "74fdff";
        public const string MONSTER_NAME = "ffffff";
        public const string MONSTER_ELITE_NAME = "00febe";
        public const string MONSTER_BOSS_NAME = "ff1979";
        public const string LINK_ROLE_NAME = "99ffff";

        public static Color COLOR_TITLE = new Color(255 / 255f, 234 / 255f, 127 / 255f);

        public const string PANEL_COMMON_POPUP = "94cbde";
        public static Color COLOR_PANEL_COMMON_POPUP = new Color(185 / 255f, 170 / 255f, 130 / 255f);
        public static Color COLOR_BTN = new Color(1, 229 / 255f, 178 / 255f);
        public static Color COLOR_GRAY = new Color(0, 0, 0);
        public static Color COLOR_BTN_EFFECT = new Color(51 / 255f, 51 / 255f, 51 / 255f);


        public static Color COLOR_PANEL_EX_LIGHT = new Color(254 / 255f, 241 / 255f, 177 / 255f);
        public static Color COLOR_PANEL_EX_DARK = new Color(115 / 255f, 115 / 255f, 115 / 255f);

        public const string MAN = "1cb7e3"; //角色名
        public const string WEMAN = "df4bff"; //仙盟名

        //聊天
        public const string CHAT_COMMON = "fff7cc"; //普通频道信息颜色
        public const string CHAT_CHUAN_WEN = "ffa64c"; //传闻信息颜色
        public const string CHAT_SYSTEM = "ff6666"; //系统信息颜色
        public const string CHAT_PRIVATE = "fb60fd"; //私聊颜色
        public const string CHAT_SEX_MAN = "66ccff"; //性别男颜色
        public const string CHAT_SEX_WOMAN = "ff8cff"; //性别女颜色
        public const string CHAT_NAME = "99ffff"; //玩家名字信息颜色


        //场景单位名字
        public const string MAP_ROLE_NAME = "ffffb2"; //角色名
        public const string MAP_ROLE_FAMILY = "74fdff"; //仙盟名
        public const string MAP_ROLE_FACTION = "00CCFF"; //国家名
        public const string MAP_ENERMY = "ee0065"; //异国名
        public const string MAP_ENERMY_CROSS = "FF6600"; //跨服别人的

        //全局界面
        public const string PANEL_ROLE_NAME = "f9c174";

        //全屏界面
        public const string TITLE_2_FULL = "fee19b"; //普通二级标题
        public const string TITLE_3_FULL = "9bdfff"; //普通三级标题
        public const string PANEL_1_FULL = "a5e8fe"; //普通文字亮
        public const string PANEL_2_FULL = "2b4f7b"; //普通文字暗
        public const string PANEL_3_FULL = "33fdff"; //普通文字强调
        public const string PANEL_4_FULL = "4e65bd"; //备注字1
        public const string PAENL_5_FULL = "8d682e"; //备注字2  

        //弹窗界面
        public const string TITLE_1_POP = "ffea7f"; //一级标题
        public const string TITLE_LIGHT_POP = "fff2b2"; //普通标题亮
        public const string TITLE_DARK_POP = "988d71"; //普通标题暗
        public const string PANEL_NUM_LIGHT_POP = "fff2b2"; //数字文字亮
        public const string PANEL_NUM_DARK_POP = "99947a"; //数字文字暗
        public const string PANEL_EXPLAIN_NORMAL_POP = "b9aa82"; //说明文字1普通
        public const string PANEL_EXPLAIN_LIGHT_POP = "fff2b2"; //说明文字2亮
        public const string PANEL_5_POP = "ffea7f"; //说明文字1最亮
        public const string PANEL_EXPLAIN_DARK_POP = "737373"; //说明文字2最暗

        //public const string TAB_NORMAL = "bfb273"; //tab平时态
        //public const string TAB_SELECT = "fff58c"; //tab选中态

        //属性类&数值文字色值
        public const string ATTR_DARK = "99947a"; //暗色文字
        public const string ATTR_LIGHT = "fff2b2"; //亮色文字
        public const string ATTR_ADD = "00ff00"; //增值性附加数值色值
        public const string ATTR_LOSE = "ff0000"; //负值性附加数值色值

        public const string LINK = "00ff00";

        public static Color TAB_N = new Color(191 / 255f, 178 / 255f, 115 / 255f);
        public static Color TAB_S = new Color(255 / 255f, 245 / 255f, 140 / 255f);


        public const string BTN_TEXT_NORMAL_POP = "ffe5b2"; //正常按钮文字

        public const string TIPS_TITLE_1 = "ffea7f";
        public const string TIPS_TITLE_2 = "fff2b2";
        public const string TIPS_NORMAL = "99917a";

        //进程面板
        public const string TITLE_1_MIS = "f2e6a9"; //一级标题
        public const string TITLE_2_MIS = "f9be61"; //
        public const string TITLE_3_MIS = "ffae00"; //
        public const string PANEL_1_MIS = "ede3bb"; //普通文字亮
        public const string PANEL_2_MIS = "cab889"; //普通文字an

        public const string LEFT_LABEL_SELECT = "fff0a9"; //左侧标签选中
        public const string LEFT_LABEL_NORMAL = "81b5d6"; //正常
        public const string LEFT_LABEL_OVER = "81b5d6"; //经过
        public const string LEFT_LABEL_DOWN = "81b5d6"; //点击

        public const string MISSION_TAG = "55fc68"; //任务标签
        public const string MISSION_STATUS_DEFAULT = "00ff00";
        public const string MISSION_STATUS_DOING = "fb8383";
        public const string MISSION_STATUS_COMPLETE = "cafb83";

        //登陆大奖面板
        public const string TITLE_A = "fee19b"; //亮标题文字
        public const string PANEL_1_INFO = "8fb0dd"; //说明文字2
        //查看他人角色界面
        public const string TAB_COLOR = "fae489";
        public const string ATTARR_A_COLOR = "b9aa82";
        public const string ATTARR_B_COLOR = "fff2b2";
        //聊天界面
        public const string CHAT_TAB_SELECT = "ffee99";
        public const string CHAT_TAB_NORMAL = "fece61";
        //广播
        public const string BROADCAST_CENTER = "fff7cc";
        //新的tabBar
        public static Color TAB_OUTLINE_SEL = new Color(64 / 255f, 12 / 255f, 2 / 255f);
        public static Color TAB_SELECT = new Color(255 / 255f, 242 / 255f, 178 / 255f);
        public static Color TAB_NORMAL = new Color(185 / 255f, 170 / 255f, 130 / 255f);

        public static Color TAB_NORMAL_2 = new Color(255 / 255f, 206 / 255f, 97 / 255f);
        public static Color TAB_SELECT_2 = new Color(255 / 255f, 238 / 255f, 153 / 255f);

        //黄色地板相关色值
        public static string YELLOW_BG_EXP = "6ea600";
        public static string YELLOW_BG_SCORE = "0098a6";
        public static string YELLOW_BG_MONEY = "b24a00";

        public static Color ValueToColor(string color) {
            byte r = byte.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color(r / 255f, g / 255f, b / 255f); //r,g,b都是<=1的值，如果计算出有效的值，要除以255f
        }

        public static uint getColorByIndex(uint index) {
            uint color = 0xffffff;
            if (index <= 6) {
                color = COLOR_VALUES[index];
            } else {
                //				throw new Error("COLOR_OVER_SORT");
            }
            return color;
        }

        public static string getHtmlColorByIndex(uint index) {
            string color = "ffffff";
            if (index <= 6) {
                color = HTML_COLORS[index];
            } else {
                //				throw new Error("找不到对应颜色");
            }
            return color;
        }

        public static string getColorName(uint index) {
            string color = "";
            if (index <= 6) {
                color = COLOR_STR[index];
            } else {
                //				throw new Error("找不到对应颜色");
            }
            return color;
        }

        public static Color ConverHtmlClrToUnityClr(string htmlColor) {
            int r = Convert.ToInt32(htmlColor.Substring(0, 2), 16);
            int g = Convert.ToInt32(htmlColor.Substring(2, 2), 16);
            int b = Convert.ToInt32(htmlColor.Substring(4, 2), 16);
            return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
        }

        //包装个NGUI颜色文本
        public static string GetColorText(string s, string color) {
            return "[" + color + "]" + s + "[-]";
        }

        //一级标题
        public static string GetTitle_1_POP_Text(string s) {
            return "[" + TITLE_1_POP + "]" + s + "[-]";
        }

        //普通标题亮
        public static string GetTitle_2_POP_Text(string s) {
            return "[" + TITLE_LIGHT_POP + "]" + s + "[-]";
        }

        //普通标题暗
        public static string GetTitle_3_POP_Text(string s) {
            return "[" + TITLE_DARK_POP + "]" + s + "[-]";
        }

        //普通文字亮
        public static string GetPANEL_1_POP_Text(string s) {
            return "[" + PANEL_1_FULL + "]" + s + "[-]";
        }

        //普通文字暗
        public static string GetPANEL_2_POP_Text(string s) {
            return "[" + PANEL_2_FULL + "]" + s + "[-]";
        }

        //说明文字1
        public static string GetPANEL_3_POP_Text(string s) {
            return "[" + PANEL_3_FULL + "]" + s + "[-]";
        }

        //说明文字2
        public static string GetPANEL_4_POP_Text(string s) {
            return "[" + PANEL_4_FULL + "]" + s + "[-]";
        }

        //说明文字2
        public static string GetLinkText(string s) {
            return "[u][" + GREEN + "]" + s + "[-][-]";
        }
    }
}