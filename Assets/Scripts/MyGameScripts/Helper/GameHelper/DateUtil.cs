// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  DateUtil.cs
// Author   : willson
// Created  : 2013/3/15 
// Porpuse  : 
// **********************************************************************

using System;
using SharpKit.JavaScript;

public static class DateUtil
{
    /// <summary>
    ///     将单位为秒的数值格式化为HH:mm:ss或mm:ss的格式
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="withHour"></param>
    /// <returns></returns>
    public static string FormatSeconds(long seconds, bool withHour = true)
    {
        if (seconds <= 0)
        {
            return withHour ? "00:00:00" : "00:00";
        }
        if (withHour)
        {
            return (seconds / 3600).ToString("D2") + ":" + (seconds / 60 % 60).ToString("D2") + ":" +
                   (seconds % 60).ToString("D2");
        }
        return (seconds / 60).ToString("D2") + ":" + (seconds % 60).ToString("D2");
    }

    /// <summary>
    ///     单位为秒,获取其Vip时间显示格式
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="showSeconds"></param>
    /// <param name="showMinutes"></param>
    /// <returns></returns>
    public static string GetVipTime(long seconds, bool showSeconds, bool showMinutes = true)
    {
        var timeSpane = TimeSpan.FromSeconds(seconds);
        //1、当前剩余时间超过1天，即大于等于24小时，则显示:xx天xx小时
        int d = timeSpane.Days;
        int h = timeSpane.Hours;
        int m = timeSpane.Minutes;
        int s = timeSpane.Seconds;
        if (timeSpane.Days >= 1)
        {
            if (h == 0) h = 1;
            return d + "天" + h + "小时";
        }
        //2、当前剩余时间少于1天，即小于24小时并大于等于60分钟。 则显示：XX小时xx分钟
        if (h >= 1)
        {
            if (m > 0)
                return h + "小时" + m + "分钟";
            return h + "小时";
        }
        //3、当前剩余时间少于60分钟小时并且大于60秒。 则显示：xx分钟
        if (m >= 1)
        {
            if (showSeconds && showMinutes)
                return m + "分" + s + "秒";
            return m + "分钟";
        }
        //4、当前剩余时间少于1分钟。 则显示：xx秒

        if (showSeconds)
            return s + "秒";
        return "小于1分钟";
    }

    /// <summary>
    ///    单位为秒,个人空间时间显示的格式
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="showSeconds"></param>
    /// <param name="showMinutes"></param>
    /// <returns></returns>
    public static string GetSelfZoneTime(long seconds, bool showSeconds, bool showMinutes = true)
    {
        var timeSpane = TimeSpan.FromSeconds(seconds);
        //1、当前剩余时间超过1天，即大于等于24小时，则显示:xx天
        int d = timeSpane.Days;
        int h = timeSpane.Hours;
        int m = timeSpane.Minutes;
        int s = timeSpane.Seconds;
        if (timeSpane.Days >= 1)
        {
            if (h == 0) h = 1;
            return d + "天";
        }
        //2、当前剩余时间少于1天，即小于24小时并大于等于60分钟。 则显示：XX小时xx分钟
        if (h >= 1)
        {
            if (m > 0)
                return h + "小时" + m + "分钟";
            return h + "小时";
        }
        //3、当前剩余时间少于60分钟小时并且大于60秒。 则显示：xx分钟
        if (m >= 1)
        {
            if (showSeconds && showMinutes)
                return m + "分" + s + "秒";
            return m + "分钟";
        }
        //4、当前剩余时间少于1分钟。 则显示：xx秒

        if (showSeconds)
            return s + "秒";
        return "刚刚";
    }

    /// <summary>
    ///     单位为秒
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static string GetMinuteSeconds(long seconds)
    {
        return (seconds / 60).ToString("D") + "分" + (seconds % 60).ToString("D") + "秒";
    }

    /// <summary>
    ///     单位为秒
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static string GetDayHourMinute(long seconds)
    {
        var timeSpane = TimeSpan.FromSeconds(seconds);
        int d = timeSpane.Days;
        int h = timeSpane.Hours;
        int m = timeSpane.Minutes;
        int s = timeSpane.Seconds;

        return string.Format("{0}天{1}小时{2}分",d, h,m);
    }

    public static string GetDayHourMinuteSecond(long seconds)
    {
        var timeSpane = TimeSpan.FromSeconds(seconds);
        int d = timeSpane.Days;
        int h = timeSpane.Hours;
        int m = timeSpane.Minutes;
        int s = timeSpane.Seconds;
        if(d < 1)
            return string.Format("{0}小时{1}分{2}秒", h, m, s);

        return string.Format("{0}天{1}小时{2}分", d, h, m);
    }
    /// <summary>
    ///     单位为秒,获取其分钟数
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static string GetMinutes(long seconds)
    {
        long m = seconds / 60;
        if (m >= 1)
            return m.ToString("D") + "分钟";
        return "小于1分钟";
    }

    /**
	 * 返回指定日期的中国标准的周几序号， 1-7
	 */

    public static int CNDayOfWeek(this DateTime dateTime)
    {
        int dayOfWeek = (int)dateTime.DayOfWeek;
        if (dayOfWeek == 0)
        {
            //国际标准的是从0-6的， 所有周日要改为7
            dayOfWeek = 7;
        }
        return dayOfWeek;
    }

    #region UnixTimestamp相关方法

    /// <summary>
    ///     获取unixTimestamp表示的日期
    /// </summary>
    /// <param name="unixTimestamp"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static string GetDateStr(long unixTimestamp, string format = "yyyy-MM-dd")
    {
        var dt = UnixTimeStampToDateTime(unixTimestamp);
        return dt.ToString(format);
    }

	public static TimeSpan UnixTimeStampToTimeSpan(long unixTimestamp)
	{ 
		DateTime dateTime = UnixTimeStampToDateTime(unixTimestamp);
		TimeSpan tTimeSpan = TimeSpan.FromTicks (dateTime.Ticks);
		return tTimeSpan;
	}

	public static double UnixTimeStampToTotalMilliseconds(long unixTimestamp)
	{ 
		TimeSpan tTimeSpan = UnixTimeStampToTimeSpan (unixTimestamp);
		return tTimeSpan.TotalMilliseconds;
	}

    /// <summary>
    ///     unixTimestamp单位为毫秒
    /// </summary>
    /// <param name="unixTimestamp"></param>
    /// <returns></returns>
    [JsMethod(Code = "return new Date(unixTimestamp);")]
    public static DateTime UnixTimeStampToDateTime(long unixTimestamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return dateTime.AddTicks(unixTimestamp * 10000).ToLocalTime();
    }

    /// <summary>
    ///     返回的unixTimestamp单位为毫秒
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    [JsMethod(Code = "return dateTime.getTime();")]
    public static long DateTimeToUnixTimestamp(DateTime dateTime)
    {
        return (dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).Ticks / 10000;
    }

    public static string getNewestActivityDate(long timeStamp)
    {
        var dt = UnixTimeStampToDateTime(timeStamp);
        return string.Format("{0}月{1}日", dt.Month, dt.Day);
    }

	#endregion
	
	
	#region 判断当前是否在某个时间区间内
	public static bool IsBetweenTimeArea(DateTime startTime, DateTime endTime, DateTime compareTime) {
		//	当前时间大于开始时间，小于结束时间，即在区间内
		bool tCompareStartSta = DateTime.Compare (compareTime, startTime) > 0;
		bool tCompareEndSta = DateTime.Compare (compareTime, endTime) < 0;
		return tCompareStartSta && tCompareEndSta;
	}

	public static bool IsBetweenTimeArea(long startTick,long endTick)
	{
		DateTime startTime = DateUtil.UnixTimeStampToDateTime(startTick);
		DateTime endTime = DateUtil.UnixTimeStampToDateTime(endTick);
		DateTime curTime = SystemTimeManager.Instance.GetServerTime();

		bool tInRacingSta = IsBetweenTimeArea(startTime, endTime, curTime);
		return tInRacingSta;
	}
	#endregion
}