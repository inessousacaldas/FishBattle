using System;
using System.Collections.Generic;
using UnityEngine;

public class GameServerInfoManager
{
    private static GameServerConfig _gameServerConfigs;
    public static List<GameServerInfo> ServerList
    {
        get { return _gameServerConfigs.serverInfoList; }
    }

    #region 推荐
    //推荐区的服务器列表
    private static List<GameServerInfo> _recommendServerList;
	public static List<GameServerInfo> GetRecommendServerList(bool needRefresh)
	{
		if (_recommendServerList == null || needRefresh)
		{
			if (_recommendServerList == null)
			{
				_recommendServerList = new List<GameServerInfo>();
			}
			else
			{
				_recommendServerList.Clear();
			}

			List<GameServerInfo> serveromInfos = _gameServerConfigs.serverInfoList;

			//推荐服客户端区分
			//1.有玩家角色的的优先，最近登录过的优先，推荐的优先，剩下的按较新的。
			//2.不够八个的话加上没有玩家的新服,时间比较晚开的优先
			//3.下限为八个

			List<GameServerInfo> tRecomentList = new List<GameServerInfo>();
			List<GameServerInfo> hasRoleList = new List<GameServerInfo>();
			string lastServerId = PlayerPrefs.GetString(GameSetting.LastServerPrefsName);
			GameServerInfo lastSelect = null;
			List<GameServerInfo> lestServerInfoList = new List<GameServerInfo>();
			for (int i = 0; i < serveromInfos.Count; i++)
			{
				GameServerInfo serverInfo = serveromInfos[i];
				if (serverInfo.dboState == 0)
				{
					continue;
				}

				if (lastServerId == serverInfo.GetServerUID())
				{
					lastSelect = serverInfo;
				}
				else if (ServerManager.Instance.GetPlayersAtServer(serverInfo.serverId).Count > 0)
				{
					hasRoleList.Add(serverInfo);
				}
				else if (serverInfo.recommendType > 0)
				{
					tRecomentList.Add(serverInfo);
				}
				else
				{
					lestServerInfoList.Add(serverInfo);
				}
			}

			//有角色的服务器排序，按最后角色登陆时间
			hasRoleList.Sort((a, b) =>
				{
					long aRecentLoginTime = ServerManager.Instance.GetPlayerRecentLoginTime(b.serverId);
					long bRecentLoginTime = ServerManager.Instance.GetPlayerRecentLoginTime(a.serverId);
					return aRecentLoginTime.CompareTo(bRecentLoginTime);
				});

			//推荐服务器排序，随机
			new RandomHelper().GetRandomArray<GameServerInfo>(tRecomentList);

			//剩余服务器排序,按开服时间
			lestServerInfoList.Sort((a, b) => b.openTime.CompareTo(a.openTime));

			if (lastSelect != null)
			{
				_recommendServerList.Add(lastSelect);
			}
			_recommendServerList.AddRange(hasRoleList);
			_recommendServerList.AddRange(tRecomentList);
			_recommendServerList.AddRange(lestServerInfoList);

			//切割多余的服务器，保留8个
			int limitCount = 8;
			if (_recommendServerList.Count > limitCount)
			{
				_recommendServerList.RemoveRange(limitCount,_recommendServerList.Count-limitCount);
			}

		}

		return _recommendServerList;
	}
    #endregion

    #region 最近登录的服务器
    private static List<GameServerInfo> _recentlyServerList;
    public static List<GameServerInfo> GetRecentlyServerList(bool needRefresh)
    {
        if(_recentlyServerList == null || needRefresh)
        {
            if (_recentlyServerList == null)
                _recentlyServerList = new List<GameServerInfo>();
            else
                _recentlyServerList.Clear();

            List<GameServerInfo> serverInfos = _gameServerConfigs.serverInfoList;

            List<GameServerInfo> hasRoleList = new List<GameServerInfo>();

            for (int i = 0, max = serverInfos.Count; i < max; i++)
            {
                GameServerInfo serverInfo = serverInfos[i];
                if (serverInfo.dboState == 0)
                    continue;
                if (ServerManager.Instance.GetPlayersAtServer(serverInfo.serverId).Count > 0)
                {
                    hasRoleList.Add(serverInfo);
                }
            }
            hasRoleList.Sort((a, b) =>
            {
                long aRecentLoginTime = ServerManager.Instance.GetPlayerRecentLoginTime(b.serverId);
                long bRecentLoginTime = ServerManager.Instance.GetPlayerRecentLoginTime(a.serverId);
                return aRecentLoginTime.CompareTo(bRecentLoginTime);
            });
            _recentlyServerList.AddRange(hasRoleList);
            int limitCount = 2;
            if (_recentlyServerList.Count > limitCount)
            {
                _recentlyServerList.RemoveRange(limitCount, _recentlyServerList.Count - limitCount);
            }
        }
        return _recentlyServerList;
    }
    
    #endregion

    public static void Setup(Action onFinish, Action<string> onError)
    {
        if (GameSetting.TestServerMode)
        {
            var textAsset = Resources.Load("Setting/LocalServerConfig") as TextAsset;
            if (textAsset != null)
            {
                var config = JsHelper.ToObject<GameServerConfig>(textAsset.text);
                if (config != null)
                {
                    _gameServerConfigs = config;
                    if (onFinish != null)
                        onFinish();
                }
                else
                {
                    if (onError != null)
                        onError("加载服务器列表失败");
                }
            }
            else
            {
                if (onError != null)
                    onError("加载< LocalServerConfig >失败");
            }
        }
        else
        {
            TalkingDataHelper.OnEventSetp("GameServerInfoManager/Setup"); //加载静态服务器列表
            GameStaticConfigManager.Instance.LoadStaticConfig(GameStaticConfigManager.Type_StaticServerList,
                json =>
                {
                    var config = JsHelper.ToObject<GameServerConfig>(json);
                    if (config != null)
                    {
                        _gameServerConfigs = config;
                        if (onFinish != null)
                            onFinish();
                    }
                    else
                    {
                        if (onError != null)
                            onError("加载服务器列表失败");
                    }
                }, onError);
        }
    }

	//根据合服状态调整服务器列表
	private static void AdjustGameServerConfigs(GameServerConfig gameServerConfig)
	{
		for (int i = 0; i < gameServerConfig.serverInfoList.Count; i++)
		{
			var serverInfo = gameServerConfig.serverInfoList[i];
			if (serverInfo.destServerId != serverInfo.serverId)
			{
				GameServerInfo destServerInfo = GetServerInfoWithDestServerId(gameServerConfig, serverInfo.destServerId);
                if (null == destServerInfo)
                    continue;
				serverInfo.serviceId = destServerInfo.serviceId;
				serverInfo.host = destServerInfo.host;
				serverInfo.port = destServerInfo.port;
				serverInfo.haVer = destServerInfo.haVer;
				serverInfo.runState = destServerInfo.runState;
				serverInfo.dboState = destServerInfo.dboState;
			}
		}
	}

	//获取目标服务器的配置
	private static GameServerInfo GetServerInfoWithDestServerId(GameServerConfig gameServerConfig, int destServerId)
	{
		for (int i = 0; i < gameServerConfig.serverInfoList.Count; i++)
		{
			var serverInfo = gameServerConfig.serverInfoList[i];
			if (serverInfo.destServerId == serverInfo.serverId && serverInfo.destServerId == destServerId)
			{
				return serverInfo;
			}
		}

		return null;
	}

    public static void InitDefaultServer()
    {
		var list = GetRecommendServerList(false);

        //更新服务器列表,如果存在默认选默认，否则选择最后一个推荐的服务器
        if (list.Count > 0)
        {
			PlayerPrefs.SetString(GameSetting.LastServerPrefsName, list[0].GetServerUID());
        }
    }

    /**
	 * name的格式是  host + "|" + accessId + "|" + serviceId   192.168.1.90|65537|30
	 */

    public static GameServerInfo GetServerInfoByName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        var splits = name.Split('|');

        for (int i = 0; i < _gameServerConfigs.serverInfoList.Count; i++)
        {
            var serverInfo = _gameServerConfigs.serverInfoList[i];
            if (splits.Length == 2)
            {
                if (serverInfo.host == splits[0] && serverInfo.serviceId == StringHelper.ToInt(splits[1]))
                {
                    return serverInfo;
                }
            }
            else if (splits.Length == 3)
            {
                if (serverInfo.host == splits[0] && serverInfo.serviceId == StringHelper.ToInt(splits[1]) &&
                    serverInfo.serverId == StringHelper.ToInt(splits[2]))
                {
                    return serverInfo;
                }
            }
        }
        return null;
    }

    public static GameServerInfo GetServerInfoById(int serverId)
    {
        for (int i = 0; i < _gameServerConfigs.serverInfoList.Count; i++)
        {
            var serverInfo = _gameServerConfigs.serverInfoList[i];
            if (serverInfo.serverId == serverId)
                return serverInfo;
        }
        return null;
    }

	public static string GetServerName(int serverId)
	{
		//临时处理：取不到服务器名的，id>=4000 安卓&越狱； id<4000 iOS
		string tServerName = string.Empty;
		GameServerInfo tGameServerInfo = GetServerInfoById (serverId);
		if (null != tGameServerInfo)
			tServerName =  tGameServerInfo.name;
		if (string.IsNullOrEmpty (tServerName)) {
			if (serverId >= 4000)
				tServerName = "安卓&越狱";
			else
				tServerName = "iOS";
		}
		return tServerName;
	}

    public static string GetAreaName(int areaId)
    {
        if (areaId == -1)
            return "推荐";
        else if (areaId == 0)
            return "最近登陆";

        for (int i = 0; i < _gameServerConfigs.areaInfoList.Count; i++)
        {
            if (_gameServerConfigs.areaInfoList[i].id == areaId)
            {
                return _gameServerConfigs.areaInfoList[i].name;
            }
        }
        return "区名";
    }

    /// <summary>
    ///     获取已开放的服务器列表
    /// </summary>
    /// <returns></returns>
    public static List<GameServerInfo> GetOpenServerList()
    {
        var list = new List<GameServerInfo>();
        for (int i = 0, len = _gameServerConfigs.serverInfoList.Count; i < len; i++)
        {
            var serverInfo = _gameServerConfigs.serverInfoList[i];
            //			if (GameSetting.SDK_SERVER.Contains( serverInfo.host ) && serverInfo.dboState != 0)
            //			{
            //				list.Add(serverInfo);
            //			}
            if (serverInfo.dboState != 0)
            {
                list.Add(serverInfo);
            }
        }
        return list;
    }

	//检查是否是合服后的相同服务器
	public static bool CheckSameDestServer(int serverId1, int serverId2)
	{
		if (serverId1 == 0 || serverId2 == 0)
			return true;
		else {
			GameServerInfo serverInfo1 = GetServerInfoById (serverId1);
			GameServerInfo serverInfo2 = GetServerInfoById (serverId2);
			if (serverInfo1 != null && serverInfo2 != null) {
				return serverInfo1.destServerId == serverInfo2.destServerId;
			} else {
				return serverId1 == serverId2;
			}
		}
	}

    #region DynamicServerInfo

    /// <summary>
    ///     获取服务器动态数据
    ///     http://dev.h5.cilugame.com/h5/gssoc/gameserver/list.json?version=5049&channel=nucleus&platform=1
    /// </summary>
    /// <param name="version"></param>
    /// <param name="channel"></param>
    /// <param name="platform"></param>
    /// <param name="onFinish"></param>
    /// <param name="onError"></param>
    public static void RequestDynamicServerList(int version, string channel, int platform,
        Action onFinish, Action onError)
    {
        string url = GameSetting.SSO_SERVER + "/gssoc/gameserver/list.json?version={0}&channel={1}&platform={2}";
        url = string.Format(url, version, channel, platform);
        ServiceProviderManager.RequestJson(url, "RequestDynamicServerList", delegate(string json)
        {
            var dynamicServerInfos = JsHelper.ToCollection<List<DynamicServerInfo>, DynamicServerInfo>(json);
            if (dynamicServerInfos != null)
            {
                UpdateServerInfo(dynamicServerInfos);
				AdjustGameServerConfigs(_gameServerConfigs);
                if (onFinish != null)
                    onFinish();
            }
            else
            {
                if (onError != null)
                    onError();
            }
        }, false, true);
    }

    /// <summary>
    /// 根据DynamicServer信息,更新ServerInfoConfig中的服务器列表信息
    /// </summary>
    /// <param name="dynamicServerInfos"></param>
    private static void UpdateServerInfo(List<DynamicServerInfo> dynamicServerInfos)
    {
        SPSDK.gameEvent("10010");   //返回服务器列表
        for (int i = 0; i < _gameServerConfigs.serverInfoList.Count; i++)
        {
            var gameServerInfo = ServerList[i];
            DynamicServerInfo dynamicServerInfo = null;
            for (int j = 0; j < dynamicServerInfos.Count; j++)
            {
                if (gameServerInfo.serverId == dynamicServerInfos[j].serverId)
                {
                    dynamicServerInfo = dynamicServerInfos[j];
                    break;
                }
            }

            if (dynamicServerInfo != null)
            {
                gameServerInfo.host = dynamicServerInfo.host;
                gameServerInfo.port = dynamicServerInfo.port;
                gameServerInfo.runState = dynamicServerInfo.runState;

                var openDate =
                    new DateTime(1970, 1, 1, 0, 0, 0, 0).AddTicks(gameServerInfo.openTime*10000).ToLocalTime();
                var span = DateTime.Now - openDate;
				gameServerInfo.newServer = span.Days < 14;
                gameServerInfo.dboState = 1;
            }
            else
            {
                gameServerInfo.dboState = 0;
            }
        }
    }

    #endregion

    #region 重新获取动态服务器信息

    private const float RefreshTime = 10f;
    private const string RefreshServerListTimer = "RefreshServerList";
    public static bool CanRefreshServerList = true;
    private static void SetupRefreshTimer()
    {
        JSTimer.Instance.SetupCoolDown(RefreshServerListTimer, RefreshTime, null, () =>
        {
            CanRefreshServerList = true;
        });
    }

    public static void RefreshServerList()
    {
        CanRefreshServerList = false;
        RequestDynamicServerList(AppGameVersion.SpVersionCode, GameSetting.Channel,
           GameSetting.PlatformTypeId, null, null);
        SetupRefreshTimer();
//        Debug.LogError("111");
    }

    public static void Dispose()
    {
        CanRefreshServerList = true;
    }

    #endregion

}