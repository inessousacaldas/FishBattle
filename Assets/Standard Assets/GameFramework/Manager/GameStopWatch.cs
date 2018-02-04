using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Debug = UnityEngine.Debug;

public static class GameStopwatch
{
    private static Dictionary<string, Stopwatch> _stopwatchDic;

    public static Stopwatch GetStopwatch(string key)
    {
        if (_stopwatchDic == null)
            return null;

        Stopwatch stopwatch = null;
        if (_stopwatchDic.TryGetValue(key, out stopwatch))
        {
            return stopwatch;
        }
        return null;
    }
    /// <summary>
    /// 开启或者重置一个计时器
    /// </summary>
    /// <param name="key"></param>
    public static void Begin(string key)
    {
        if (_stopwatchDic == null)
        {
            _stopwatchDic = new Dictionary<string, Stopwatch>();
        }

        Stopwatch stopwatch = null;
        if (_stopwatchDic.TryGetValue(key, out stopwatch))
        {
            stopwatch.Reset();
        }
        else
        {
            stopwatch = new Stopwatch();
            _stopwatchDic.Add(key, stopwatch);
        }

        stopwatch.Start();
    }

    /// <summary>
    /// 停止指定计时器
    /// </summary>
    /// <param name="key"></param>
    public static void End(string key, bool log = false)
    {
        if (_stopwatchDic == null)
            return;

        Stopwatch stopwatch = null;
        if (_stopwatchDic.TryGetValue(key, out stopwatch))
        {
            stopwatch.Stop();
            if (log)
                Debug.LogError(DumpInfo(key));
        }
    }

    /// <summary>
    /// 清空所有计时器记录
    /// </summary>
    public static void Clear()
    {
        if (_stopwatchDic != null)
        {
            _stopwatchDic.Clear();
        }
    }

    /// <summary>
    /// 获取某个计时器信息
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string DumpInfo(string key)
    {
        if (_stopwatchDic == null)
        {
            return "=======暂无任何计时器记录=======";
        }

        Stopwatch stopwatch = null;
        if (_stopwatchDic.TryGetValue(key, out stopwatch))
        {
            TimeSpan elapsed = stopwatch.Elapsed;
            return string.Format("[{0}] 耗时:{1:00}:{2:00}:{3:00}:{4:00}",
                key,
                elapsed.Hours,
                elapsed.Minutes,
                elapsed.Seconds,
                elapsed.Milliseconds / 10
                );
        }
        return string.Format("暂无[{0}]计时器记录", key);
    }

    /// <summary>
    /// 获取所有计时器信息
    /// </summary>
    /// <returns></returns>
    public static string DumpAllInfo()
    {
        if (_stopwatchDic == null)
        {
            return "=======暂无任何计时器记录=======";
        }

        StringBuilder sb = new StringBuilder("=======GameStopWatch=======\n");
        foreach (var pair in _stopwatchDic)
        {
            TimeSpan elapsed = pair.Value.Elapsed;
            sb.AppendLine(string.Format("[{0}] 耗时:{1:00}:{2:00}:{3:00}:{4:00}",
                pair.Key,
                elapsed.Hours,
                elapsed.Minutes,
                elapsed.Seconds,
                elapsed.Milliseconds / 10
                ));
        }
        sb.AppendLine("=======GameStopWatch=======");
        return sb.ToString();
    }
}