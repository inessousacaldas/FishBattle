package com.demiframe.game.api.util;

import android.content.Context;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;

import java.io.IOException;
import java.io.InputStream;

public class IOUtil
{
  public static String getMetaDataByName(Context paramContext, String paramString)
  {
    try
    {
      ApplicationInfo localApplicationInfo = paramContext.getPackageManager().getApplicationInfo(paramContext.getPackageName(), PackageManager.GET_META_DATA);
      if (localApplicationInfo.metaData != null && localApplicationInfo.metaData.containsKey(paramString))
        return String.valueOf(localApplicationInfo.metaData.get(paramString));
      LogUtil.e("not find ：" + paramString);
      return null;
    }
    catch (Exception localException)
    {
      localException.printStackTrace();
    }
    return null;
  }

  public static String inputStreamString(InputStream paramInputStream)
    throws IOException
  {
    StringBuffer localStringBuffer = new StringBuffer();
    byte[] arrayOfByte = new byte[4096];
    while (true)
    {
      int i = paramInputStream.read(arrayOfByte);
      if (i == -1)
        return localStringBuffer.toString();
      localStringBuffer.append(new String(arrayOfByte, 0, i));
    }
  }
}