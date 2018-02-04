// --------------------------------------
//  Unity Foundation
//  StringHelper.cs
//  copyright (c) 2014 Nicholas Ventimiglia, http://avariceonline.com
//  All rights reserved.
//  -------------------------------------
// 

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SharpKit.JavaScript;
using UnityEngine;
using System.Text;

/// <summary>
///     字符串工具类
/// </summary>
internal static class AppStringHelper
{
    public static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double len = bytes;
        while (len >= 1024 && order + 1 < sizes.Length)
        {
            order++;
            len = len / 1024;
        }

        // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
        // show a single decimal place, and no space.
        return string.Format("{0:0.##} {1}", len, sizes[order]);
    }

    /// <summary>
    ///     splits by newline
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string[] SplitByNewline(this string s)
    {
        s = s.Replace("\r\n", "\n");
        return s.Split('\n');
    }

    /// <summary>
    ///     Removes any newline
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string RemoveNewline(this string s)
    {
        return s.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
    }

    #region 字符串转换成数字数组

    /// <summary>
    ///     Tos the list.
    /// </summary>
    /// 区别于 using System.Linq 为String类扩展的ToList
    /// <T>
    ///     :比如 List
    ///     <int>
    ///         list = "5,8,9".Split(',').ToList
    ///         <int>
    ///             ();
    ///             用法:
    ///             List
    ///             <int>
    ///                 list =  "10,20".ToList(',',s=>StringHelper.ToInt(s));
    ///                 List<float> fList =  "10.55,3.88".ToList(',',s=>float.Parse(s));
    public static List<T> ParseToList<T>(this string str, char split, Converter<string, T> convertHandler)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        var array = str.Split(split);
        if (array.Length > 0)
        {
            var list = new List<T>(array.Length);
            for (int index = 0; index < array.Length; ++index)
            {
                list.Add(convertHandler(array[index]));
            }
            return list;
        }
        return null;
    }

    #endregion

    #region 聊天相关方法

    /// <summary>
    /// 剥除聊天输入内容的非法字符,防止用户输入颜色,url等非法标记
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
//    [JsMethod(Code = @"return str.replace(/\[url=.*\].*\[\/url\]|\[\/?-?[a-z0-9]{0,8}\]/i, """");")]
    public static string StripChatSymbols(this string str)
    {
        return NGUIText.StripSymbols(str);
        //        return Regex.Replace(str, @"\[url=.*\].*\[/url\]|\[\/?-?[a-z0-9]{0,8}\]", "", RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// 转换内容里面的颜色值为大写
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    [JsMethod(Code = @"if(System.String.IsNullOrEmpty(str)) return str;
            return str.replace(/\[[a-z0-9]{6,8}\]/ig, function(match){ return match.toUpperCase(); });")]
    public static string ChangeColorToUpper(this string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return Regex.Replace(str, @"\[[a-z0-9]{6,8}\]", match => match.Value.ToUpper(), RegexOptions.IgnoreCase);
    }

    #endregion

    #region Wrap Symbol Text

    /// <summary>
    ///     Creates the color symbol text.
    /// </summary>
    public static string WrapColor(this string txt, string colorSymbol)
    {
        if (string.IsNullOrEmpty(colorSymbol))
            return txt;
        return "[c][" + colorSymbol + "]" + txt + "[-][/c]";
    }

    public static string WrapColor(this int val, string colorSymbol)
    {
        return WrapColor(val.ToString(), colorSymbol);
    }

    public static string WrapColorWithLog(this string txt, string color = "orange")
    {
        return "<color=" + color + ">" + txt + "</color>";
    }

    public static string WrapColor(this string txt, Color color)
    {
        return WrapColor(txt, NGUIText.EncodeColor(color));
    }

    /// <summary>
    ///     <para>Wraps the symbol.</para>
    ///     <para>[b] 加粗</para>
    ///     <para>[i] 斜体</para>
    ///     <para>[u] 下划线</para>
    ///     <para>[s] 删除线</para>
    /// </summary>
    public static string WrapSymbol(this string txt, string symbol)
    {
        return "[" + symbol + "]" + txt + "[/" + symbol + "]";
    }

    public static string WrapURL(this string txt, string link)
    {
        return "[url=" + link + "]" + txt + "[/url]";
    }

    #endregion

    #region 字符串匹配

    [JsMethod(Code = @"return /^[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,6}$/i.test(input);")]
    public static bool IsEmail(string input)
    {
        return Regex.IsMatch(input, @"^[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,6}$", RegexOptions.IgnoreCase);
    }

    [JsMethod(Code = @"return /^[a-zA-Z0-9]+$/.test(input);")]
    public static bool ValidateAccount(string input)
    {
        return Regex.IsMatch(input, @"^[a-zA-Z0-9]+$");
    }

    [JsMethod(Code = @"return /^[0-9]+$/.test(input);")]
    public static bool IsNum(this string num)
    {
        return Regex.IsMatch(num, @"^[0-9]+$");
    }

    #endregion

    #region 中英文判断

    /// <summary>
    ///     获取带中文字符的字符串长度
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    [JsMethod(Code = @"var len = 0;
            for (var i = str.length - 1; i >= 0; i--) {
                var c = str.charCodeAt(i);
                if((c >= 0x0001 && c <= 0x007e) || (0xff60 <= c && c <= 0xff9f)) {
                    len++;
                }else{
                    len +=2;
                }
            }
            return len;")]
    public static int GetGBLength(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0;
        int len = 0;
        for (var i = str.Length - 1; i >= 0; i--)
        {
            var c = str[i];
            if ((c >= 0x0001 && c <= 0x007e) || (0xff60 <= c && c <= 0xff9f))
            {
                len++;
            }
            else
            {
                len += 2;
            }
        }
        return len;
    }

    /// <summary>
    ///     裁剪当前字符串长度,返回适配的最大长度值
    /// </summary>
    /// <param name="str"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    [JsMethod(Code = @"var len = 0;
            for (var i = str.length - 1; i >= 0; i--) {
                var c = str.charCodeAt(i);
                if((c >= 0x0001 && c <= 0x007e) || (0xff60 <= c && c <= 0xff9f)) {
                    len++;
                }else{
                    len +=2;
                }
                
                if(len > max){
                    return str.substr(0,i);
                }
            }
            return str;")]
    public static string TrimInputStr(string str, int max)
    {
        int len = 0;
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            if ((c >= 0x0001 && c <= 0x007e) || (0xff60 <= c && c <= 0xff9f))
            {
                len++;
            }
            else
            {
                len += 2;
            }

            if (len > max)
            {
                return str.Substring(0, i);
            }
        }
        return str;
    }

    /// <summary>
    ///     判断一个字符串是否符合规定长度
    /// </summary>
    /// <param name="str"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static string ValidateStrLength(this string str, int min = 4, int max = 14)
    {
        int length = GetGBLength(str);
        GameDebuger.Log(string.Format("ValidateStrLength str={0} len={1} min={2} max={3}", str, length, min, max));
        if (length < min)
        {
            //return "输入文字太短了，不合适啊";
            return string.Format("角色名字必须在{0}-{1}个字符之间", min, max);
        }

        if (length > max)
        {
            if (min == max)
            {
                return "输入文字长度不符合要求";
            }
            int chCount = Mathf.FloorToInt(max / 2f);
            return string.Format("角色名字必须在{0}-{1}个字符之间", min, max);
            //return string.Format("输入文字不能超过{0}个中文字符，{1}个英文字符", chCount, max);
        }

        return null;
    }

    // 是否中文
    [JsMethod(Code = @"return /[\u4e00-\u9fa5]/.test(input);")]
    public static bool IsHasCN(string input)
    {
        return Regex.IsMatch(input, "[\u4e00-\u9fa5]");
    }

    // 是否双字节字符
    [JsMethod(Code = @"return /[^\x00-\xff]/.test(input);")]
    public static bool IsHasWChar(string input)
    {
        return Regex.IsMatch(input, "[^\x00-\xff]");
    }

    // 是否英文 or 数字
    [JsMethod(Code = @"return /^[A-Za-z0-9]{1}$/.test(input);")]
    public static bool IsHasENAndDigit(string input)
    {
        return Regex.IsMatch(input, "^[A-Za-z0-9]{1}$");
    }


    //是否中英文或者数字
    public static bool IsHasChZNAndEngAndFigure(string str)
    {
        char[] strArr = str.ToCharArray();
        for (int index = 0; index < strArr.Length; index++)
        {
            if (!AppStringHelper.IsHasCN(strArr[index].ToString())
                && !AppStringHelper.IsHasENAndDigit(strArr[index].ToString()))
            {
                return false;
            }
        }

        return true;
    }

    #endregion


    #region UIInput

    public static void AdjustInputText(UIInput pUIInput)
    {
        if (null != pUIInput)
        {
            string tContent = pUIInput.value;
            if (!string.IsNullOrEmpty(tContent) && tContent.Length > 1)
                pUIInput.value = tContent.Substring(0, tContent.Length - 1);
        }
    }

    [JsMethod(Code = @"if(System.String.IsNullOrEmpty(rawStr)) 
                return rawStr;
            return rawStr.replace(/\uD83C[\uDF00-\uDFFF]|\uD83D[\uDC00-\uDEFF]|[\u2600-\u26FF]/g, """");")]
    public static string FilterEmoji(string rawStr)
    {
        if (string.IsNullOrEmpty(rawStr))
            return rawStr;

        return Regex.Replace(rawStr, @"\uD83C[\uDF00-\uDFFF]|\uD83D[\uDC00-\uDEFF]|[\u2600-\u26FF]", "");
    }

    /// <summary>
    /// 把输入框中的Mac系统等自带的Emoji过滤掉。
    /// </summary>
    /// <param name="pUIInput">P user interface input.</param>
    public static void FilterEmoji(UIInput pUIInput)
    {
        if (null != pUIInput)
        {
            pUIInput.value = FilterEmoji(pUIInput.value);
        }
    }
    #endregion
}