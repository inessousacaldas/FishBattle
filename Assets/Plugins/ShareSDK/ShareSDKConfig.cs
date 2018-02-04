using System.Collections;
using cn.sharesdk.unity3d;
using UnityEngine;

public static class ShareSDKConfig
{
    public static string GetAppKey()
    {
        var platform = Application.platform;
        string appKey = null;

        switch (platform)
        {
            case RuntimePlatform.Android:
            {
                appKey = "c3d4ce374eea";

                break;
            }
            case RuntimePlatform.IPhonePlayer:
            {
                appKey = "c3d4ce374eea";

                break;
            }
        }
        return appKey;
    }

    public static Hashtable GetConfigEx()
    {
        var platform = Application.platform;
        Hashtable config = null;

        switch (platform)
        {
            case RuntimePlatform.Android:
            {
                var wxAppId = BaoyugameSdk.getMetaData("LHWXAppId");
                var wxAppKey = BaoyugameSdk.getMetaData("LHWXAppKey");

                config = new Hashtable
                {
                    {
                        (int) PlatformType.WeChat, new Hashtable
                        {
                            {"SortId", "1"},
                            {"AppId", wxAppId},
                            {"AppSecret", wxAppKey},
                            {"BypassApproval", "false"},
                            {"Enable", "true"}
                        }
                    },
                    {
                        (int) PlatformType.WeChatMoments, new Hashtable
                        {
                            {"SortId", "2"},
                            {"AppId", wxAppId},
                            {"AppSecret", wxAppKey},
                            {"BypassApproval", "false"},
                            {"Enable", "true"}
                        }
                    }
                };
                break;
            }
            case RuntimePlatform.IPhonePlayer:
            {
                config = new Hashtable
                {
                    {
                        (int) PlatformType.WechatPlatform, new Hashtable
                        {
                            {"app_id", IosBridge.GetValueFromInfoPlist("ShareSDK.WeiXinAppId")},
                            {"app_secret", IosBridge.GetValueFromInfoPlist("ShareSDK.WeiXinAppSecret")},
							{"Enable", "true"},
						}
                    }
                };
                break;
            }
        }
        return config;
    }
}