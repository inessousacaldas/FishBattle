using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts.MyGameScripts.Utils {
    public static class StringUtil {
        public static readonly string[] CATE_NAME ={
          "", "狂战","飞羽","舞天"
                                                  };
        public static readonly string[] CN_NUMS = {
            "", "一", "二", "三", "四",
            "五", "六", "七", "八", "九", 
            "十", "十一", "十二", "十三","十四","十五",
            "十六","十七","十八","十九","二十"
        };

        public static readonly string[] CN_NUMS2 = {
            "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛"
        };

        public static readonly string[] WEEK_DAYS = {
            "日", "一", "二", "三", "四", "五", "六"
        };

        public static string Num2Cn(int num) {
            if (num < 0 || num > 15) return "";
            return CN_NUMS[num];
        }

        public static readonly Regex reDigital = new Regex(@"\d+");

        public static void CopyString(string msg) {
            TextEditor te = new TextEditor();
            te.content = new GUIContent(msg);
            te.SelectAll();
            te.Copy();
        }

        /// <summary>
        /// 数字转中文
        /// </summary>
        /// <param name="number">eg: 22</param>
        /// <returns></returns>
        public static string NumberToChinese(int number) {
            string res = string.Empty;
            string str = number.ToString();
            string schar = str.Substring(0, 1);
            switch (schar) {
                case "1":
                    res = "一";
                    break;
                case "2":
                    res = "二";
                    break;
                case "3":
                    res = "三";
                    break;
                case "4":
                    res = "四";
                    break;
                case "5":
                    res = "五";
                    break;
                case "6":
                    res = "六";
                    break;
                case "7":
                    res = "七";
                    break;
                case "8":
                    res = "八";
                    break;
                case "9":
                    res = "九";
                    break;
                default:
                    res = "零";
                    break;
            }
            if (str.Length > 1) {
                switch (str.Length) {
                    case 2:
                    case 6:
                        res += "十";
                        break;
                    case 3:
                    case 7:
                        res += "百";
                        break;
                    case 4:
                        res += "千";
                        break;
                    case 5:
                        res += "万";
                        break;
                    default:
                        res += "";
                        break;
                }
                res += NumberToChinese(StringHelper.ToInt(str.Substring(1, str.Length - 1)));
            }
            return res;
        }

        public static bool IsNotNullOrEmpty(this string str) {
            return string.IsNullOrEmpty(str) == false;
        }

        public static bool IsNullOrEmpty(this string str) {
            return string.IsNullOrEmpty(str);
        }
    }
}
