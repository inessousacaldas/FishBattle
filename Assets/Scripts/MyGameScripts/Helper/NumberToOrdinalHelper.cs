// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File          :  NumberToOrdinalHelper.cs
// Author    : xl.s
// Created  : 2016/6/22 
// Porpuse  : 数字转序号工具类
// **********************************************************************
public class NumberToOrdinalHelper
{
    /// <summary>
    /// 返回带有圈的 ①- ⑳
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string NumAddCircle(int num)
    {
        if (num <= 0 || num > 20)
        {
            return num.ToString();
        }
        char c = (char)(9311 + num);
        return c.FromCharCode();
    }

    /// <summary>
    /// 返回带有括号的 ⑴- ⒇
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string NumAddBrackets(int num)
    {
        if (num <= 0 || num > 20)
        {
            return num.ToString();
        }
        char c = (char)(9331 + num);
        return c.FromCharCode();
    }

    /// <summary>
    /// 返回带有点的 ⒈- ⒛
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string NumAddPoint(int num)
    {
        if (num <= 0 || num > 20)
        {
            return num.ToString();
        }
        char c = (char)(9351 + num);
        return c.FromCharCode();
    }

    /// <summary>
    /// 返回带括号的26个字母⒜ - ⒵
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string NumToLetters(int num)
    {
        if (num <= 0 || num > 26)
        {
            return num.ToString();
        }
        char c = (char)(9371 + num);
        return c.FromCharCode();
    }

    /// <summary>
    /// 返回带圈的26个字母Ⓐ - Ⓩ
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string NumToLettersWithCircle(int num)
    {
        if (num <= 0 || num > 26)
        {
            return num.ToString();
        }
        char c = (char)(9397 + num);
        return c.FromCharCode();
    }
}
