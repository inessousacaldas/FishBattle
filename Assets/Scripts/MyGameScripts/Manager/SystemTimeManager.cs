// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  SystemTimeManager.cs
// Author   : SK
// Created  : 2013/8/29
// Purpose  : 
// **********************************************************************
using UnityEngine;
using System;

public class SystemTimeManager : MonoBehaviour
{
    //这里因为要处理服务器返回的延时， 所以把时间调快1秒
    private const long TIME_FOR_CLIENT_DELAY = 1000L;
    //使用本地时间测试模式
    #if UNITY_EDITOR
    private const bool LocalTimeTest = false;
    #else
	private const bool LocalTimeTest = false;
	#endif

    private static SystemTimeManager _instance = null;

    public static SystemTimeManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private long _unixTimeStamp;
    //unix时间戳以毫秒为单位
    private long _lastTimeStamp;
    //	上一次服务器给的时间戳/ms
	
    public event System.Action<bool> OnSystemWeatherChange;
    //系统天气变化事件
    public event System.Action<long> OnSystemTimeChange;
    public event System.Action OnChangeNextDay;
    public event System.Action OnChangeNextWeek;

    public bool night = false;
    private NpcViewManager npcMgr;
    void Awake()
    {
        _instance = this;
        _unixTimeStamp = DateUtil.DateTimeToUnixTimestamp(DateTime.Now);

    }

    void Update()
    {
        //--todo 明达
        //发送Update事件到NpcViewManager
        if (npcMgr == null && WorldManager.Instance != null){
            npcMgr = WorldManager.Instance.GetNpcViewManager();
        }
//        var npcMgr = WorldManager.Instance.GetNpcViewManager();
        if (npcMgr != null)
        {
            npcMgr.Tick();
        }
        _unixTimeStamp += (long)(Time.unscaledDeltaTime * 1000f);
    }

    //获取PlayerDto时初始化
    public void Setup(long newTime)
    {
        if (LocalTimeTest)
        {
            _unixTimeStamp = DateUtil.DateTimeToUnixTimestamp(DateTime.Now);
        }
        else
        {
            //这里因为要处理服务器返回的延时， 所以把时间调快1秒
            _unixTimeStamp = newTime + TIME_FOR_CLIENT_DELAY;
        }
        _lastTimeStamp = _unixTimeStamp;

        JSTimer.Instance.SetupCoolDown("SystemCheckTimer", 1f, null, OnTimerFinish);
    }

    public void Dispose()
    {
        JSTimer.Instance.CancelCd("SystemCheckTimer");
    }

    #region Getter

    public DateTime GetServerTime()
    {
        return DateUtil.UnixTimeStampToDateTime(_unixTimeStamp);
    }

    public DateTime GetLastServerTime()
    {
        return DateUtil.UnixTimeStampToDateTime(_lastTimeStamp);
    }

    /**
	 * 返回当前日期的中国标准的周几序号， 1-7, 周日是7
	 */ 
    public int GetCHDayOfWeek()
    {
        return GetServerTime().CNDayOfWeek();
    }

    /**
	 * 返回当前日期的国际标准的周几序号， 0-6, 周日是0
	 */ 
    public int GetDayOfWeek()
    {
        return (int)GetServerTime().DayOfWeek;
    }

    public long GetUTCTimeStamp()
    {
        return _unixTimeStamp;
    }

    public long GetLastTimeStamp()
    {
        return _lastTimeStamp;
    }

    #endregion

    private void OnTimerFinish()
    {
        int hourMinute = GetServerTime().Minute;
        if (hourMinute >= 30)
        {
            hourMinute -= 30;
        }
		
        bool newNight = (hourMinute >= 15);
        if (night != newNight)
        {
            night = newNight;
            GameUtil.SafeRun<bool>(OnSystemWeatherChange, night);
        }
		
        if (OnSystemTimeChange != null)
        {
            OnSystemTimeChange(_unixTimeStamp);
        }

        JSTimer.Instance.SetupCoolDown("SystemCheckTimer", 1f, null, OnTimerFinish);
    }

    public void SyncServerTime(long newTime)
    {
        DateTime lastDateTime = GetLastServerTime();

        if (LocalTimeTest)
        {
            _unixTimeStamp = DateUtil.DateTimeToUnixTimestamp(DateTime.Now);
        }
        else
        {
            //这里因为要处理服务器返回的延时， 所以把时间调快1秒
            _unixTimeStamp = newTime + 1000L;
        }
        _lastTimeStamp = _unixTimeStamp;

        DateTime newDateTime = GetServerTime();

        if (lastDateTime.Date != newDateTime.Date)
        {
            //跨日事件处理
            if (OnChangeNextDay != null)
                OnChangeNextDay();
        }
        if (lastDateTime.DayOfWeek > newDateTime.DayOfWeek)
        {
            //跨周事件处理
            if (OnChangeNextWeek != null)
                OnChangeNextWeek();
        }
//		Debug.LogError(DateUtil.UnixTimeStampToDateTime(_unixTimeStamp));
    }

    //获取没有考虑延迟加多的那一秒的时间
    public long GetDelaylessTime(long pTargetTime)
    {
        return pTargetTime + TIME_FOR_CLIENT_DELAY;
    }
}