using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class JsFrameworkUpgrade
{
#region 对新添加的类进行排序
    private static readonly Type[] _exportTypeArray = new Type[]
    {
//        typeof(FrameworkVersion),
    };

    public static Type[] ExportTypeArray
    {
        get
        {
            return _exportTypeArray;
        }
    }


    public static void ExportTypeExcept(HashSet<Type> exportType)
    {
        if (_exportTypeArray != null)
        {
            exportType.ExceptWith(_exportTypeArray);
        }
    }

#endregion


    #region 对框架中新添加的函数进行排序
    private static readonly Dictionary<Type, MethodInfo[]> MethodDict = new Dictionary<Type, MethodInfo[]>()
    {
        //{typeof (ZipLibUtils), new []{typeof(ZipLibUtils).GetMethod("UnzipFromStream") }},
    };


    public static int MethodInfoComparison(GeneratorHelp.MethodInfoAndIndex mi1, GeneratorHelp.MethodInfoAndIndex mi2)
    {
        var result = 0;

        if (MethodDict != null && MethodDict.ContainsKey(mi1.method.DeclaringType) &&
            MethodDict.ContainsKey(mi2.method.DeclaringType))
        {
            var methodArray = MethodDict[mi1.method.DeclaringType];
            result = Array.IndexOf(methodArray, mi1.method).CompareTo(Array.IndexOf(methodArray, mi2.method));
        }

        return result;
    }
    #endregion
}
