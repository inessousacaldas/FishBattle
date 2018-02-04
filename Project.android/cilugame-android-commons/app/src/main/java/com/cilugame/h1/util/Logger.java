package com.cilugame.h1.util;

import android.util.Log;

public class Logger
{
  private static final String TAG = "H1_SDK";

  public static void Error(String paramString, Exception e)
  {
    Log.e("H1_SDK", paramString, e);
  }

  public static void Log(String paramString)
  {
    Log.d("H1_SDK", paramString);
  }

  public static void Warn(String paramString)
  {
    Log.w("H1_SDK", paramString);
  }
}