package com.demiframe.game.api.demi.demiutil;

import android.util.Log;

public class Logger
{
  private static final String TAG = "DEMI";

  public static void Error(String paramString, Exception e)
  {
    Log.e(TAG, paramString, e);
  }

  public static void Log(String paramString)
  {
    Log.d(TAG, paramString);
  }

  public static void Warn(String paramString)
  {
    Log.w(TAG, paramString);
  }
}