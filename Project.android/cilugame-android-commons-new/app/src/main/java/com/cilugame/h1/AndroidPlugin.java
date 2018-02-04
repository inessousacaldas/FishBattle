package com.cilugame.h1;

import android.app.Activity;
import android.app.ActivityManager;
import android.app.ActivityManager.MemoryInfo;
import android.content.ClipData;
import android.content.ClipData.Item;
import android.content.ClipboardManager;
import android.content.Context;
import android.content.Intent;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.PackageManager.NameNotFoundException;
import android.content.res.AssetManager;
import android.graphics.Bitmap;
import android.graphics.Bitmap.CompressFormat;
import android.os.Bundle;
import android.os.Debug;
import android.util.Log;

import com.cilugame.h1.model.DeviceInfo;
import com.cilugame.h1.model.NetworkInfo;

import com.cilugame.h1.util.Logger;
import com.cilugame.h1.view.UnityEditTextStyle;
import com.cilugame.h1.view.ViewManager;
import java.io.File;
import java.io.FileOutputStream;

public class AndroidPlugin
{
  private static ActivityManager.MemoryInfo activityMemInfo;
  private static Debug.MemoryInfo debugMemInfo;
  private static AssetManager assetManager; 
  private static long[] infos;
  private static boolean libLoaded = false;

  static
  {
    infos = new long[3];
    activityMemInfo = new ActivityManager.MemoryInfo();
    debugMemInfo = new Debug.MemoryInfo();
  }

  public static boolean CheckPermission(String permission)
  {
	  if (permission.isEmpty())
	  {
		  return false;
	  }
	  else  
	  {
		  Activity cur = UnityPlayerActivity.instance;
		  return cur.getPackageManager().checkPermission(permission, cur.getPackageName()) != 0;
	  }
  }

  public static boolean CreateQRCode(String paramString1, String paramString2, String paramString3)
  {
	  /*
    if ((paramString1 == null) || ("".equals(paramString1)))
      return false;
    if (paramString3 == null)
      paramString2 = QRCodeUtil.CreateQRImage(paramString2, 2130837505);
    while (true)
    {
      if (paramString2 != null);
      try
      {
        paramString1 = new File(paramString1);
        if (paramString1.exists())
          paramString1.delete();
        paramString1 = new FileOutputStream(paramString1);
        paramString2.compress(Bitmap.CompressFormat.JPEG, 100, paramString1);
        paramString1.flush();
        paramString1.close();
        paramString2.recycle();
        return true;
        paramString2 = QRCodeUtil.CreateQRImage(paramString2, paramString3);
      }
      catch (Exception paramString1)
      {
        Logger.Error(paramString1.getLocalizedMessage());
      }
    }
    */
    return false;
  }

  public static String GetAppMetaData(String name)
  {
	  Logger.Log("getMetaData name="+name);
		Activity cur = UnityPlayerActivity.instance;
		String value = "";
		try {
			ApplicationInfo appInfo = cur.getPackageManager().getApplicationInfo(cur.getPackageName(), PackageManager.GET_META_DATA);
			value = appInfo.metaData.get(name).toString();
			 Logger.Log("getMetaData value="+value);
		} catch (Exception e) {
			Logger.Log("getMetaData Exception="+e.toString());
		}
		return value;	
  }

  public static void GetClipboardText()
  {
    UnityPlayerActivity.instance.runOnUiThread(new Runnable()
    {
      public void run()
      {
        Object localObject = ((ClipboardManager)UnityPlayerActivity.instance.getSystemService("clipboard")).getPrimaryClip();
        if ((localObject != null) && (((ClipData)localObject).getItemCount() > 0))
        {
          localObject = ((ClipData)localObject).getItemAt(0);
          if (localObject != null)
          {
           // UnityCallbackWrapper.OnGetClipboardText(((ClipData.Item)localObject).getText().toString());
            return;
          }
        }
        //UnityCallbackWrapper.OnGetClipboardText(null);
      }
    });
  }

  public static DeviceInfo GetDeviceInfo()
  {
    return DeviceInfo.GetDeviceInfo(UnityPlayerActivity.instance);
  }

  public static String GetInternalStoragePath()
  {
    return UnityPlayerActivity.instance.getFilesDir().getAbsolutePath();
  }

  public static long[] GetMemoryInfo()
  {
    ((ActivityManager)UnityPlayerActivity.instance.getSystemService("activity")).getMemoryInfo(activityMemInfo);
    Debug.getMemoryInfo(debugMemInfo);
    infos[0] = debugMemInfo.getTotalPss();
    infos[1] = (activityMemInfo.availMem / 1024L);
    infos[2] = (activityMemInfo.threshold / 1024L);
    return infos;
  }

  public static NetworkInfo GetNetworkInfo()
  {
    return NetworkInfo.GetNetworkInfo(UnityPlayerActivity.instance);
  }

  public static PackageInfo GetPackageInfo()
  {
    try
    {
      PackageInfo localPackageInfo = UnityPlayerActivity.instance.getPackageManager().getPackageInfo(UnityPlayerActivity.instance.getPackageName(), 0);
      return localPackageInfo;
    }
    catch (PackageManager.NameNotFoundException localNameNotFoundException)
    {
      Logger.Error(localNameNotFoundException.getMessage(), localNameNotFoundException);
    }
    return null;
  }

  public static void HideEditDialog()
  {
    UnityPlayerActivity.instance.GetViewManager().HideEditDialog();
  }

  public static boolean IsLibraryLoaded()
  {
    return libLoaded;
  }

  public static UnityEditTextStyle NewEditTextStyle()
  {
    return new UnityEditTextStyle();
  }

  public static void Quit()
  {
    Logger.Log("Call Quit");
  }

  public static void SetAssetManager(AssetManager paramAssetManager)
  {
    assetManager = paramAssetManager;
  }

  public static void SetClipboardText(final String paramString)
  {
	  
    if (paramString == null)
      return;
    UnityPlayerActivity.instance.runOnUiThread(new Runnable()
    {
      public void run()
      {
        ((ClipboardManager)UnityPlayerActivity.instance.getSystemService("clipboard")).setPrimaryClip(ClipData.newPlainText("CopyText", paramString));
      }
    });
    
  }

  public static void SetEditText(String paramString)
  {
    UnityPlayerActivity.instance.GetViewManager().SetEditText(paramString);
  }

  public static void ShowEditDialog(String paramString, UnityEditTextStyle paramUnityEditTextStyle)
  {
    UnityPlayerActivity.instance.GetViewManager().ShowEditDialog(paramString, paramUnityEditTextStyle);
  }
}