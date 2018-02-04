package com.demiframe.game.api.util;

import android.util.Log;
import java.text.SimpleDateFormat;
import java.util.Date;

public class LogUtil
{
  // TODO: 2017/3/23 release设置为false 
  public static boolean DEBUG = true;

  public static String _FILE_()
  {
    return new java.lang.Exception().getStackTrace()[2].getFileName();
  }

  public static String _FUNC_()
  {
    return new java.lang.Exception().getStackTrace()[1].getMethodName();
  }

  public static int _LINE_()
  {
    return new java.lang.Exception().getStackTrace()[1].getLineNumber();
  }

  public static String _TIME_()
  {
    Date localDate = new Date();
    return new SimpleDateFormat("yyyy-MM-dd HH:mm:ss.SSS").format(localDate);
  }

  public static void d(String paramString)
  {
    if (DEBUG)
      Log.d(_FILE_(), "[" + getLineMethod() + "]" + paramString);
  }

  public static void d(String paramString1, String paramString2)
  {
    if (DEBUG)
      Log.d(paramString1, "[" + getFileLineMethod() + "]" + paramString2);
  }

  public static void d(String paramString1, String paramString2, String paramString3)
  {
    if (DEBUG)
      Log.d(paramString1, "[" + paramString2 + "]" + paramString3);
  }

  public static void e(String paramString)
  {
    Log.e(_FILE_(), getLineMethod() + paramString);
  }

  public static void e(String paramString1, String paramString2)
  {
    Log.e(paramString1, getLineMethod() + paramString2);
  }

  public static String getFileLineMethod()
  {
    StackTraceElement localStackTraceElement = new java.lang.Exception().getStackTrace()[2];
    return "[" + localStackTraceElement.getFileName() + " | " + localStackTraceElement.getLineNumber() + " | " + localStackTraceElement.getMethodName() + "]";
  }

  public static String getLineMethod()
  {
    StackTraceElement localStackTraceElement = new java.lang.Exception().getStackTrace()[2];
    return "[" + localStackTraceElement.getLineNumber() + " | " + localStackTraceElement.getMethodName() + "]";
  }

  public static void i(String paramString)
  {
    Log.i(_FILE_(), "[" + getLineMethod() + "]" + paramString);
  }
}