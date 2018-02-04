package com.cilugame.h1.model;

import android.content.Context;
import android.os.Build;
import android.os.Build.VERSION;
import android.telephony.TelephonyManager;

public class DeviceInfo
{
  public String device;
  public String deviceId;
  public String deviceSoftwareVersion;
  public String display;
  public String lineNumber;
  public String manufacturer;
  public String model;
  public int networkClass;
  public String networkOperator;
  public String networkOperatorName;
  public int networkType;
  public int phoneType;
  public String product;
  public int sdk;
  public String sdkVersion;
  public String simOperator;
  public String simOperatorName;

  public static DeviceInfo GetDeviceInfo(Context paramContext)
  {
	    DeviceInfo localDeviceInfo = new DeviceInfo();
	  /*
    localDeviceInfo.device = Build.DEVICE;
    localDeviceInfo.display = Build.DISPLAY;
    localDeviceInfo.manufacturer = Build.MANUFACTURER;
    localDeviceInfo.model = Build.MODEL;
    localDeviceInfo.product = Build.PRODUCT;
    localDeviceInfo.sdkVersion = Build.VERSION.RELEASE;
    localDeviceInfo.sdk = Build.VERSION.SDK_INT;
    paramContext = (TelephonyManager)paramContext.getSystemService("phone");
    localDeviceInfo.deviceId = paramContext.getDeviceId();
    localDeviceInfo.deviceSoftwareVersion = paramContext.getDeviceSoftwareVersion();
    localDeviceInfo.lineNumber = paramContext.getLine1Number();
    localDeviceInfo.networkType = paramContext.getNetworkType();
    localDeviceInfo.networkClass = GetNetworkClass(localDeviceInfo.networkType);
    localDeviceInfo.networkOperator = paramContext.getNetworkOperator();
    localDeviceInfo.networkOperatorName = paramContext.getNetworkOperatorName();
    localDeviceInfo.simOperator = paramContext.getSimOperator();
    localDeviceInfo.simOperatorName = paramContext.getSimOperatorName();
    localDeviceInfo.phoneType = paramContext.getPhoneType();
    */
    return localDeviceInfo;
  }

  private static int GetNetworkClass(int paramInt)
  {
    switch (paramInt)
    {
    default:
      return 0;
    case 1:
    case 2:
    case 4:
    case 7:
    case 11:
      return 1;
    case 3:
    case 5:
    case 6:
    case 8:
    case 9:
    case 10:
    case 12:
    case 14:
    case 15:
      return 2;
    case 13:
    }
    return 3;
  }
}

/* Location:           C:\dex2jar-2.0\bbxy-1.7.19-b-n-112-dex2jar.jar
 * Qualified Name:     com.yuelang.xiyou.model.DeviceInfo
 * JD-Core Version:    0.5.4
 */