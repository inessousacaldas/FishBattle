using System;
using UnityEngine;
using System.Collections;

internal static class EnumHelper
{
    /// <summary>
    /// 获取该枚举名称Str
    /// </summary>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static string GetEnumNameString(ValueType enumValue)
    {
        return enumValue.ToString();
    }


    /// <summary>
    /// 将值转为字符串
    /// </summary>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static string GetEnumValueString(ValueType enumValue)
    {
        var uType = Enum.GetUnderlyingType(enumValue.GetType());
        return Convert.ChangeType(enumValue, uType).ToString();
    }


    /// <summary>
    /// 将值转为想要的类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static T GetEnumValue<T>(ValueType enumValue)
    {
        return (T)Convert.ChangeType(enumValue, typeof(T));
    }


    /// <summary>
    /// 根据下标获取枚举值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <returns></returns>
    public static T GetEnumByIndex<T>(int index)
    {
        return (T)Convert.ChangeType(Enum.GetValues(typeof (T)).GetValue(index), typeof (T));
    }


    /// <summary>
    /// 根据下标获取枚举值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <returns></returns>
    public static V GetEnumValueByIndex<T, V>(int index)
    {
        return (V)Convert.ChangeType(Enum.GetValues(typeof(T)).GetValue(index), typeof(V));
   }
}
