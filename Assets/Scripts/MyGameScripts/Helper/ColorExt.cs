// --------------------------------------
//  Unity Foundation
//  ColorExt.cs
//  copyright (c) 2014 Nicholas Ventimiglia, http://avariceonline.com
//  All rights reserved.
//  -------------------------------------
// 

using UnityEngine;

/// <summary>
///     Static helper for Colors
/// </summary>
public static class ColorExt
{
    /// <summary>
    ///     returns the same color with a new alpha value
    /// </summary>
    /// <param name="c"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static Color SetAlpha(this Color c, float a)
    {
        return new Color(c.r, c.g, c.b, a);
    }

    /// <summary>
    ///     Color To String
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static string Serialize(this Color c)
    {
        return string.Format("{0},{1},{2},{3}", c.r, c.g, c.b, c.a);
    }

    /// <summary>
    ///     String to Color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color Parse(string color)
    {
        var s = color.Split(',');

        return new Color(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]), float.Parse(s[3]));
    }

    public static Color HexStrToColor(string hex)
    {
        return hex.Length > 6 ? NGUIText.ParseColor32(hex, 0) : NGUIText.ParseColor24(hex, 0);
    }
}