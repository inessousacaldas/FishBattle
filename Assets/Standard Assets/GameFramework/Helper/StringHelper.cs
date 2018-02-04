using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class StringHelper
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
    /// Tos the list.
    /// </summary> 
    /// 区别于 using System.Linq 为String类扩展的ToList<T>:比如 List<int> list = "5,8,9".Split(',').ToList<int>();
    /// 
    /// 用法: 
    /// List<int> list =  "10,20".ToList(',',s=>int.Parse(s));
    /// List<float> fList =  "10.55,3.88".ToList(',',s=>float.Parse(s));
    public static List<T> ConvertToList<T>(this string str, char split, Converter<string, T> convertHandler)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        string[] array = str.Split(split);
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

    /// <summary>
    /// Gets the host from URL.
    /// </summary>
    /// <returns>The host from URL.</returns>
    /// <param name="url">URL.</param>
    public static string GetHostFromUrl(string url)
    {
        if (url.Contains("http://") || url.Contains("https://"))
        {
            string host = "";
            string pattern = @"(?<=//|)((\w)+\.)+\w+";
            Regex regex = new Regex(pattern);

            Match matcher = regex.Match(url);  
            if (matcher != null && matcher.Success)
            {
                host = matcher.ToString();
            }
            return host;            
        }
        else
        {
            return "";
        }
    }

    public static int ToInt(this string pValue)
    {
        if (string.IsNullOrEmpty(pValue))
            return 0;
        int tIntValue = 0;
        int.TryParse(pValue, out tIntValue);
        return tIntValue;
    }

    public static float ToFloat(this string pValue)
    {
        if (string.IsNullOrEmpty(pValue))
            return 0f;
        float tFloatValue = 0;
        float.TryParse(pValue, out tFloatValue);
        return tFloatValue;
    }
}
