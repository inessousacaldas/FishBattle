#define ProfileHelper
//#define ProfileHelperDebug
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class ProfileHelper
{

#if ProfileHelper
    public static void ProfilerBegin(string message, int index)
    {
        string newMessage = string.Format("{0} : Index {1}", message, index);
#if ProfileHelperDebug
        Debug.LogError(newMessage);
#endif
        UnityEngine.Profiling.Profiler.BeginSample(newMessage);
    }
    public static void ProfilerEnd(string messgae, int index)
    {
        string newMessgae = string.Format("{0} : Index {1}", messgae, index);
#if ProfileHelperDebug
        Debug.LogError(newMessgae);
#endif
        UnityEngine.Profiling.Profiler.EndSample();
    }
    public static void ProfilerBegin(string message)
    {
        string newMessage = message;
#if ProfileHelperDebug
        Debug.LogError(newMessage);
#endif
        UnityEngine.Profiling.Profiler.BeginSample(newMessage);
    }
    public static void ProfilerEnd(string messgae)
    {
        string newMessgae = messgae;
#if ProfileHelperDebug
        Debug.LogError(newMessgae);
#endif
        UnityEngine.Profiling.Profiler.EndSample();
    }

    private static Dictionary<string, Stopwatch> stopwatches = new Dictionary<string, Stopwatch>(); 

    public static void SystimeBegin(string msg)
    {
        if (!string.IsNullOrEmpty(msg))
        {
            Debug.Log(string.Format("----------------msg:{0} FrameCount:{1}", msg, Time.frameCount));
            stopwatches[msg] = Stopwatch.StartNew();
        }
    }

    public static void SystimeEnd(string message)
    {
        Stopwatch stopwatch;
        if (stopwatches.TryGetValue(message, out stopwatch))
        {
            Debug.Log(string.Format(message + " TimeSpan : ms:{0} FrameCount:{1}", stopwatch.ElapsedTicks, Time.frameCount));
        }
        else
        {
            Debug.Log(string.Format(message + " can not find Stopwatch"));
        }
    }

#endif

}

