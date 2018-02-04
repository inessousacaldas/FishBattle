// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  GameDebugerText.cs
// Author   : SK
// Created  : 2013/3/4
// Purpose  : 
// **********************************************************************

using System;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

public static class GameDebuger
{
    public static bool debugIsOn = false;

    public static bool DebugForExit = false;

    public static bool DebugForLogout = false;

    public static bool DebugForDisconnect = false;

    public static bool Release = false;

    public static long Debug_PlayerId = 0;

    public static void UpdateSetting()
    {
        debugIsOn = CSGameDebuger.debugIsOn;
        DebugForExit = CSGameDebuger.DebugForExit;
        DebugForLogout = CSGameDebuger.DebugForLogout;
        DebugForDisconnect = CSGameDebuger.DebugForDisconnect;
        Release = CSGameDebuger.Release;
        Debug_PlayerId = CSGameDebuger.Debug_PlayerId;
        openDebugLogOrange = CSGameDebuger.openDebugLogOrange;
    }

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

    public static void SendExceptionForJS(string log)
    {
        Debug.Log("SendExceptionForJS");
        //Crasheye.SendScriptException("JSB", log, "JS");
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

    #region Temp log function for refactor 
    /// <summary>
    ///  重构时专用提示日志方法，标识着本日志后续需要删除或修正。
    /// </summary>
    public static void Note(string pParam)
    {
        GameDebuger.Log(string.Format("[Note]临时屏蔽，信息：\n{0}", pParam));
    }

    /// <summary>
    ///  重构时专用待办日志方法，标识着本日志后续必须修正。
    /// </summary>
    public static void TODO(string pParam)
    {
        //太多了，引起性能问题，直接屏蔽掉，要查找直接搜索TODO就好
        //GameDebuger.LogWarning(string.Format("[TODO]临时屏蔽，信息：\n{0}",pParam));
    }
    #endregion

    public static string GetStackTrace(string msg,int index = 1,int max = 9999)
    {
        if(msg.IndexOf(".cs") != -1) { return msg; }
        System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
        StringBuilder sb = new StringBuilder();
        sb.Append(msg + "\n");
        var len = Mathf.Max(st.FrameCount - index - 3, 2);
        for(int i = 1;i < len;i++)
        {
            System.Diagnostics.StackFrame sf = st.GetFrame(i);
            if(sf != null)
            {
                var name = sf.GetFileName();
                if(string.IsNullOrEmpty(name) == false)
                {
                    name = name.Replace(Environment.CurrentDirectory + "\\Assets\\Scripts\\","　　");
                }
                sb.Append(name + " 方法名: " + sf.GetMethod().DeclaringType.FullName + "." + sf.GetMethod().Name);
                var line = sf.GetFileLineNumber();
                if(line > 0)
                {
                    sb.Append(" 行数：" + sf.GetFileLineNumber() + "\n");
                }
                else
                {
                    sb.Append("\n");
                }
            }
            if(sb.Length > max) break;
        }
        return sb.ToString();
    }
}