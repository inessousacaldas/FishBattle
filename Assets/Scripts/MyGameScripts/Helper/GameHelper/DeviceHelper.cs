using UnityEngine;

public static class DeviceHelper
{
    public const int WIFI_MAX = 3; //wifi 最多只有三格 
    public static int GetWifiSignel()
    {
        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            return GameSetting.IsOriginWinPlatform
                ? WIFI_MAX 
                : BaoyugameSdk.getWifiSignal() / 40;
        }
        else
        {
            return 0;
        }
    }
}