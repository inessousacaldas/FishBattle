// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  GameDebugerText.cs
// Author   : SK
// Created  : 2013/3/4
// Purpose  : 
// **********************************************************************

using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

public static class CSGameDebuger
{
    public static bool debugIsOn = false;

    public static bool DebugForExit = false;

    public static bool DebugForLogout = false;

	public static bool DebugForDisconnect = false;

    public static bool Release = false;

    public static long Debug_PlayerId = 0;

    #region LogWrapper

    public static void Log(object message, string color = null)
    {
        if (!debugIsOn && Release) return;

        string log = message == null ? "Null" : message.ToString();
        if (!string.IsNullOrEmpty(color))
            log = "<color=" + color + ">" + log + "</color>";

        Debug.Log(log);
    }

    public static void LogError(object msg, Object context = null)
    {
        Debug.LogError(msg, context);
    }

    public static void LogWarning(object msg, Object context = null)
    {
        Debug.LogWarning(msg, context);
    }

    public static void LogException(Exception e, Object context = null)
    {
        Debug.LogException(e, context);
    }

    public static void LogBattleInfo(object message)
    {
        Log(message, "orange");
    }

    #endregion

    #region DebugError -> Use color=orange

    public static bool openDebugLogOrange = false;

    /// <summary>
    ///     常规项 | Oranges the debug log.
    /// </summary>
    /// <param name="s">S.</param>
    public static void OrangeDebugLog(string s)
    {
        if (openDebugLogOrange)
        {
            Debug.LogError(string.Format("<color=orange> ## {0} ## </color>", s));
        }
    }

    /// <summary>
    ///     特殊项 | Aquas the debug log.
    /// </summary>
    /// <param name="s">S.</param>
    public static void AquaDebugLog(string s)
    {
        if (openDebugLogOrange)
        {
            Debug.LogError(string.Format("<color=aqua> ## {0} ## </color>", s));
        }
    }

    /// <summary>
    ///     警告项 | Yellows the debug log.
    /// </summary>
    /// <param name="s">S.</param>
    /// <param name="b">If set to <c>true</c> b.</param>
    public static void YellowDebugLog(string s, bool b = false)
    {
        if (openDebugLogOrange || b)
        {
            Debug.LogError(string.Format("<color=yellow> ## {0} ## </color>", s));
        }
    }

    #endregion
}