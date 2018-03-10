using System;
using System.Text;
using LITJson;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
///     游戏加载
///     Local_Assets ： 读取本地Unity资源
///     Remote_Assets： 程序包 全部资源都要下载
///     Local_Packet_Assets : 整包 全部资源都在包里面
///     Local_Experience_Pack_Assets ： 体验包 部分体验的资源在包里面
/// </summary>
public static class GameSetting
{
    public enum DebugInfoType
    {
        None,
        Default,
        Verbose
    }

    public enum PlatformType
    {
        Android = 1,
        ROOTIOS = 2,
        IOS = 3,
        Win = 4
    }

	public const string Config_WritePathV2 = "Assets/Resources/Setting/GameSettingDataV2.txt";
	public const string Config_ReadPathV2 = "Setting/GameSettingDataV2";

    private static GameSettingData _gameSettingData;

    /// <summary>
    ///     游戏类型名
    /// </summary>
    public static string GameType { get; set; }

    /// <summary>
    ///     调试信息类型
    /// </summary>
    public static DebugInfoType LogType
    {
        get { return _gameSettingData.logType; }
    }

    /// <summary>
    ///     客户端模式
    /// </summary>
    public static int ClientMode { get; private set; }

    private static bool mRelease;
    /// <summary>
    ///     Release版本,包含片头动画,关闭GM指令等
    /// </summary>
    public static bool Release
    { 
        get
        {
            return mRelease;
        }
        private set
        {
            mRelease = value;
            CSGameDebuger.Release = Release;
        }
    }

    public static bool CheckUpdate { get; private set; }

    /// <summary>
    ///     压测模式
    /// </summary>
    public static bool TestServerMode { get; private set; }

	/// <summary>
	///     GM模式
	/// </summary>
	public static bool GMMode { get; private set; }

    public static void Setup()
    {
        _gameSettingData = LoadGameSettingData();

        if (_gameSettingData != null)
        {
	        Platform = IsOriginWinPlatform ? PlatformType.IOS : OriginPlatform;

            PlatformHttpRoot = String.Format("{0}/{1}/{2}", _gameSettingData.httpRoot,
				_gameSettingData.resdir,
                PlatformTypeName);
            GameType = _gameSettingData.gameType;
	        DomainName = _gameSettingData.domainType;
	        ResDir = _gameSettingData.resdir;

            Channel = _gameSettingData.channel;
            Release = _gameSettingData.release;
            CheckUpdate = _gameSettingData.checkUpdate;
            TestServerMode = _gameSettingData.testServerMode;
			GMMode = _gameSettingData.gmMode;
        }
        else
        {
            Debug.LogError(" GameSettingData Setup Error !!");
        }
    }

    /// <summary>
    /// 初始化服务器Http请求地址
    /// </summary>
    /// <param name="config"></param>
    public static void SetupServerUrlConfig(ServerUrlConfig config)
    {
        if (config == null)
        {
            Debug.LogError("ServerUrlConfig is null ");
            return;
        }

        SDK_SERVER = config.sdkUrl;
        SSO_SERVER = config.ssoUrl;
        PAY_SERVER = config.payUrl;
		SPACE_SERVER = config.spaceUrl;

        //策划静态数据：套/staticData/
        //战斗数据：套/battle/
        //游戏资源数据：套/平台（Win还是使用Win的）/
		string resDir = ResDir;
		DATA_SERVER = config.dataUrl + "/" + resDir;
		// 资源更新路径
		CDN_SERVER_LIST.Clear();
		if (config.urlMaps != null)
		{
			AddCdnToList(config, "masterCdnUrl");
			AddCdnToList(config, "slaveCdnUrl");
			AddCdnToList(config, "srcCdnUrl");
		}
		else
		{
			AddCdnToList(config.cdnUrl);
		}

		CDN_SERVER = CDN_SERVER_LIST[0];

		BATTLE_SERVER = config.battleUrl + "/" + resDir;

        if (!String.IsNullOrEmpty(config.ports))
        {
            HA_TRY_PORTS = config.ports;
        }
    }

	private static void AddCdnToList(ServerUrlConfig config, string key)
	{
		if (config.urlMaps.ContainsKey(key))
		{
			string url = config.urlMaps[key];
			AddCdnToList(url);
		}
	}

	private static void AddCdnToList(string cdnUrl)
	{
		if (!String.IsNullOrEmpty(cdnUrl))
		{
			if (!CDN_SERVER_LIST.Contains(cdnUrl))
			{
				CDN_SERVER_LIST.Add(cdnUrl + "/" + ResDir + "/" + OriginPlatformTypeName);
			}			
		}
	}

    public static GameSettingData LoadGameSettingData()
    {
		var assetV2 = Resources.Load(Config_ReadPathV2) as TextAsset;
		if (assetV2 != null)
		{
			string json = Encoding.UTF8.GetString(assetV2.bytes);
			GameSettingData data = JsonMapper.ToObject<GameSettingData>(json);
			return data;
		}
        else
        {
            return null;
        }
    }

    #region 平台相关属性
    /// <summary>
    ///     平台对应http根目录
    ///     .../{domain}/{platform}
    /// </summary>
    public static string PlatformHttpRoot { get; set; }

    /// <summary>
    ///     域类型名
    /// </summary>
    public static string DomainName { get; set; }

	/// <summary>
	///     资源目录
	/// </summary>
	public static string ResDir { get; set; }

    /// <summary>
    ///     域类型枚举值
    /// </summary>
//    public static string Domain
//    {
//        get { return _gameSettingData.domainType; }
//    }

    /// <summary>
    ///     平台根目录 例如：内开发 /localdev/android
    /// </summary>
    public static string PlatformTypeName
    {
        get { return Platform.ToString().ToLower(); }
    }

    /// <summary>
    ///     平台标识
    /// </summary>
    public static int PlatformTypeId
    {
        get { return (int)Platform; }
    }

    /// <summary>
    ///     平台枚举值
    /// </summary>
    public static PlatformType Platform { get; set; }


#region Win特用
    /// <summary>
    /// 获取平台原始值，Win特用
    /// </summary>
    public static PlatformType OriginPlatform
    {
        get { return _gameSettingData.platformType; }
    }

    public static bool IsOriginWinPlatform
    {
        get { return OriginPlatform == PlatformType.Win; }
    }

    public static string OriginPlatformTypeName
    {
        get { return OriginPlatform.ToString().ToLower(); }
    }
    #endregion


    /// <summary>
    ///     平台资源目录
    /// </summary>
    public static string PlatformResPath
    {
        get
        {
            return CDN_SERVER + "/staticRes";
        }
    }

	public static List<string> PlatformResPathList
	{
		get
		{
			List<string> list = new List<string>();
			for(int i=0; i<CDN_SERVER_LIST.Count; i++)
			{
				list.Add(CDN_SERVER_LIST[i] + "/staticRes");
			}
			return list;
		}
	}

    public static string PlatformDllPath
    {
        get { 
			return CDN_SERVER + "/dlls";
		}
    }

	/// <summary>
	///     游戏名
	/// </summary>
	public static string GameName
	{
		get {
			if (_gameSettingData == null)
			{
				return "";
			}
			else
			{
				string name = _gameSettingData.gamename;
				if (string.IsNullOrEmpty(name))
				{
					name = "游戏";
				}
				return name;				
			}
		}
	}

	/// <summary>
	///     配置后缀
	/// </summary>
	public static string ConfigSuffix
	{
		get {
			if (_gameSettingData == null)
			{
				return "";
			}
			else
			{
				string suffix = _gameSettingData.configSuffix;
				if (string.IsNullOrEmpty(suffix))
				{
					suffix = "";
				}
				return suffix;				
			}
		}		
	}

	/// <summary>
	///     配置后缀,带分隔符_
	/// </summary>
	public static string ConfigSuffixWithSplit
	{
		get {
			if (_gameSettingData == null)
			{
				return "";
			}
			else
			{
				string suffix = _gameSettingData.configSuffix;
				if (string.IsNullOrEmpty(suffix))
				{
					suffix = "";
				}
				else
				{
					suffix = "_"+suffix;
				}
				return suffix;				
			}
		}		
	}

    #endregion

    #region GameInfo

    /// <summary>
    ///     应用ID
    /// </summary>
    public static int AppId = 1;

    /// <summary>
    ///     CPID
    /// </summary>
    public static int CpId = 1;

    /// <summary>
    ///     包名
    /// </summary>
    public static string BundleId = "";

    /// <summary>
    ///     渠道，大渠道的唯一标示，可以是英文或者数字
    /// </summary>
    public static string Channel = "NULL";

    /// <summary>
    ///     子渠道,如某些渠道打包后，会有很多子ID渠道
    /// </summary>
	public static string SubChannel = "NULL";

    /// <summary>
    ///     登陆方式
    /// </summary>
	public static string LoginWay = "NULL";

    /// <summary>
    ///     是否设备号登录模式
    ///     true deviceId ; false account and passworld
    /// </summary>
    public static bool DeviceLoginMode = true;

    /// <summary>
    /// 游戏金币名字
    /// </summary>
    public static string PayProductName = "元宝";

    /// <summary>
    /// 游戏金币描述
    /// </summary>
    public static string PayProductDesc = "用于购买特殊道具";


    /// <summary>
    ///     CONFIG服务器地址
    /// </summary>
    private static string _CONFIG_SERVER = "http://dev.h5.cilugame.com/h5";

    /// <summary>
    ///     CONFIG服务器地址
    /// </summary>
    public static string CONFIG_SERVER
    {
        get { return _CONFIG_SERVER; }
        set
        {
            _CONFIG_SERVER = value;
            // 如果是在Win平台的话，将服务器根地址改为IOS的
//            if (GameSetting.Platform == GameSetting.PlatformType.Win)
//            {
//                GameSetting._CONFIG_SERVER =
//                    GameSetting._CONFIG_SERVER.Replace(GameSetting.PlatformType.Win.ToString().ToLower(),
//                        GameSetting.PlatformType.IOS.ToString().ToLower());
//            }
        }
    }

    /// <summary>
    ///     SPACE服务器地址
    /// </summary>
    public static string SPACE_SERVER = "http://dev.h5.cilugame.com/h5";

	/// <summary>
	///     CDN服务器地址列表，提供轮询
	/// </summary>
	public static List<string> CDN_SERVER_LIST = new List<string>();

    /// <summary>
    ///     SDK服务器地址
    /// </summary>
    public static string SDK_SERVER = "http://dev.h5.cilugame.com/h5";

    /// <summary>
    ///     SSO服务器地址
    /// </summary>
    public static string SSO_SERVER = "http://dev.h5.cilugame.com/h5";

    /// <summary>
    ///     CDN服务器地址
    /// </summary>
    public static string CDN_SERVER = "http://dev.h5.cilugame.com/h5";

    /// <summary>
    ///     BATTLE服务器地址
    /// </summary>
    public static string BATTLE_SERVER = "http://dev.h5.cilugame.com/h5";

    /// <summary>
    ///     PAY服务器地址
    /// </summary>
    public static string PAY_SERVER = "http://dev.h5.cilugame.com/h5";

    /// <summary>
    ///     DATA服务器地址
    /// </summary>
    public static string DATA_SERVER = "http://dev.h5.cilugame.com/h5";

    public static string HA_SERVICE_MAIN_TYPE = "com/cilugame/core4s3";
    public static string HA_TRY_PORTS = "443,8443";

    public static string LastServerPrefsName
    {
        get
        {
            if (SDK_SERVER == "http://192.168.1.97:9993")
            {
                return "LocalServerPrefsName";
            }
            return "ExternalServerPrefsName";
        }
    }

    public static string LastRolePrefsName
    {
        get
        {
            if (SDK_SERVER == "http://192.168.1.97:9993")
            {
                return "LocalRolePrefsName";
            }
            return "ExternalRolePrefsName";
        }
    }

	public static bool IsCyouChannel
	{
		get { return false; }
	}

	public static long GetLastRolePlayerId()
    {
        long roleId = 0;
        string roleIdStr = PlayerPrefs.GetString(LastRolePrefsName);
        if (!String.IsNullOrEmpty(roleIdStr))
        {
            roleId = Int64.Parse(roleIdStr);
        }
        return roleId;
    }

    #endregion

    public static int ParseVersionCode(string versionStr)
    {
        var version = new Version(versionStr);
        return version.Major*1000*1000 + version.Minor*1000 + version.Build;
    }
}

public class GameSettingData
{
    //渠道
    public string channel = "";     
	//游戏类型
    public string gameType;
	//域类型
	public string domainType;
    //http根目录
    public string httpRoot = "";
	//资源目录
	public string resdir = "";

	//游戏名字
	public string gamename = "";

	//配置后缀
	public string configSuffix = "";

    public GameSetting.DebugInfoType logType;
    public GameSetting.PlatformType platformType;

    public bool release = false;
    public bool checkUpdate = true;

    //新版本资源加载参数
    public bool testServerMode = false;

	//GM模式，可以通过用户ID，进入玩家账号
	public bool gmMode = false;
}

public class ServerUrlConfig
{
    public string battleUrl;
    public string cdnUrl;
    public string dataUrl;
    public string payUrl;
    public string ports; //"443,8443"
    public string sdkUrl;
    public string ssoUrl;
	public string spaceUrl;
    public string version;

	//"urlMaps":{"srcCdnUrl":"http://dev.h5.cilugame.com/h5","masterCdnUrl":"http://dev.h5.cilugame.com/h5","slaveCdnUrl":"http://dev.h5.cilugame.com/h5"}
	public Dictionary<string, string> urlMaps; 
}