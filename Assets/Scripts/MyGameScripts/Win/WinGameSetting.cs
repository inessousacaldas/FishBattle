using System;
using UnityEngine;
using System.Collections;
using System.IO;


public static class WinGameSetting
{
    /// <summary>
    /// PC端需要登陆后重新赋值的
    /// ReloadPlatformType 默认IOS
    /// ReloadDomainType 没发现影响
    /// ReloadChannel 涉及到比较重要的 ChannelId,Channel，SubChannel，LoginWay，AreaId
    /// ReloadStaticData 静态数据，服务器等
    /// </summary>
    /// <param name="data"></param>
    /// <param name="onFinish"></param>
    /// <param name="onError"></param>
	public static void Setup(WinGameSettingData data, Action onFinish, Action<string> onError)
    {
        GameDebuger.Log("WinGameSetting Setup");
        GameDebuger.Log(JsHelper.ToJson(data, true));

        if (!GameSetting.IsOriginWinPlatform)
        {
            return;
        }

        Data = data;

        ReloadPlatformType();
        ReloadDomainType();
        ReloadChannel();
        ReloadStaticData(onFinish, onError);
    }


    private static void ReloadPlatformType()
    {
        GameDebuger.Log("WinGameSetting ReloadPlatformType");

        GameSetting.Platform = Data.PlatformType;
    }

    private static void ReloadDomainType()
    {
        GameDebuger.Log("WinGameSetting ReloadDomainType");

        GameSetting.DomainName = Data.DomainType;
        GameSetting.ResDir = Data.ResDir;
        GameSetting.PlatformHttpRoot = string.Format("{0}/{1}/{2}", Data.HttpRoot,
                Data.ResDir,
                GameSetting.PlatformTypeName);
    }

    private static void ReloadChannel()
    {
        GameDebuger.Log("WinGameSetting ReloadChannel");

        AppGameManager.Instance.SetupChannel();
    }

    private static void ReloadStaticData(Action onFinish, Action<string> onError)
    {
        GameDebuger.Log("WinGameSetting ReloadStaticData");

        GameStaticConfigManager.Instance.Setup(() =>
        {
            GameStaticConfigManager.Instance.LoadStaticConfig(GameStaticConfigManager.Type_StaticServerList,
                json =>
                {
                    ServerUrlConfig config = JsHelper.ToObject<ServerUrlConfig>(json);
                    if (config != null)
                    {
                        GameSetting.SetupServerUrlConfig(config);
                        GameServerInfoManager.Setup(() =>
                        {
                            GameServerInfoManager.RequestDynamicServerList(AppGameVersion.SpVersionCode, GameSetting.Channel,
                            GameSetting.PlatformTypeId, onFinish,
							() => { onError("加载服务器列表失败, 请重新进入游戏"); });
						}, onError);
					}
					else
                    {
                        onError("解析服务器配置信息失败");
                    }
                }, onError);
        }, onError);
    }


	#region 接口
    public static string Channel
	{
        get { return Data.Channel; }
	}

    #endregion


    #region 数据
    private static WinGameSettingData _data;


    public static WinGameSettingData Data
    {
        get
        {
            if (_data == null)
            {
                _data = WinGameSettingData.CreateOriginWinGameSettingData();
            }
            return _data;
        }
        set { _data = value; }
    }


    /// <summary>
    /// 默认值使用打包的参数
    /// </summary>
    public class WinGameSettingData
    {
        // 这里的值根据实际项目需求来取
        public string Channel;
        public GameSetting.PlatformType PlatformType;

        public string DomainType;
        public string HttpRoot;
        public string ResDir;

        public static WinGameSettingData CreateOriginWinGameSettingData()
        {
            var winData = new WinGameSettingData();

            var originData = GameSetting.LoadGameSettingData();
            if (!GameSetting.IsOriginWinPlatform)
            {
			winData.Channel = originData.channel;
                winData.PlatformType = originData.platformType;

                winData.DomainType = GameSetting.DomainName;
			winData.HttpRoot = Path.GetDirectoryName(Path.GetDirectoryName(GameSetting.PlatformHttpRoot));
                winData.ResDir = GameSetting.ResDir;
            }
            else
            {
                winData.Channel = originData.channel;
                winData.PlatformType = GameSetting.PlatformType.Android;

                winData.DomainType = originData.domainType;
                winData.HttpRoot = originData.httpRoot;
                winData.ResDir = originData.resdir;
            }

            return winData;
        }

        public string ToBase64UrlSafeJson()
        {
            var json = JsHelper.ToJson(this);
            GameDebuger.Log("WinGameSettingData ToBase64UrlSafeJson: " + json);
            return WWW.EscapeURL(ByteArray.CreateFromUtf8(json).ToBase64String());
        }


        public static WinGameSettingData CreateFromBase64UrlSafeJson(string json)
        {
            json = ByteArray.CreateFromBase64(WWW.UnEscapeURL(json)).ToUTF8String();
            GameDebuger.Log("WinGameSettingData CreateFromBase64UrlSafeJson: " + json);
            return JsHelper.ToObject<WinGameSettingData>(json);
        }
    }
    #endregion
}
