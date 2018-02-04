package com.cilugame.h1.model;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.wifi.WifiManager;

public class NetworkInfo
{
  public boolean available;
  public int networkType;
  public int wifiState;

  public static NetworkInfo GetNetworkInfo(Context paramContext)
  {
    NetworkInfo localNetworkInfo = new NetworkInfo();
    /*
    localNetworkInfo.wifiState = ((WifiManager)paramContext.getSystemService("wifi")).getWifiState();
    paramContext = ((ConnectivityManager)paramContext.getSystemService("connectivity")).getActiveNetworkInfo();
    if (paramContext != null)
    {
      localNetworkInfo.networkType = paramContext.getType();
      localNetworkInfo.available = paramContext.isAvailable();
      return localNetworkInfo;
    }
    localNetworkInfo.networkType = -1;
    localNetworkInfo.available = false;
    */
    return localNetworkInfo;
  }
}