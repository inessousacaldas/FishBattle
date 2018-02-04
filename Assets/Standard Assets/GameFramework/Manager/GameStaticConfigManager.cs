// **********************************************************************
// Copyright (c) 2015 cilugame. All rights reserved.
// File     : GameStaticConfigManager.cs
// Author   : senkay <senkay@126.com>
// Created  : 11/09/2015 
// Porpuse  : 服务器后台静态配置管理器
// **********************************************************************
//

using System;
using System.IO;
using System.Text;
using LITJson;
using UnityEngine;
using System.Collections.Generic;
using AssetPipeline;

public class GameStaticConfigManager
{

	public const string Type_StaticVersion = "staticVersion.txt";
	public const string Type_StaticServerList = "staticServerList.txt";

	private static readonly GameStaticConfigManager instance = new GameStaticConfigManager ();

	public static string Config_Path = "/servers/";
	private StaticVersion _remoteStaticVersion;

    public static GameStaticConfigManager Instance
    {
        get { return instance; }
    }

    public void Setup(Action onFinish, Action<string> onError)
    {
        GameSetting.CONFIG_SERVER = GameSetting.PlatformHttpRoot;

        LoadStaticConfig(Type_StaticVersion, delegate (string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                _remoteStaticVersion = JsonMapper.ToObject<StaticVersion>(json);
                FormatStaticVersion(_remoteStaticVersion);

                string configUrl = GetStaticUrl("configUrl");
                if (!string.IsNullOrEmpty(configUrl))
                {
                    GameSetting.CONFIG_SERVER = string.Format("{0}/{1}/{2}", configUrl,
                        GameSetting.ResDir, GameSetting.PlatformTypeName);
				} else {
					GameSetting.CONFIG_SERVER = GameSetting.PlatformHttpRoot;
				}
				if (!string.IsNullOrEmpty(GetStaticUrl("behaviorUrl")))
				{
					BehaviorHelper.Setup (GetStaticUrl ("behaviorUrl") + "/spreadc/behavior/info.json?message=", BaoyugameSdk.getUUID ());
				}
				if (!string.IsNullOrEmpty(GetStaticUrl("spreadUrl")))
				{
					CdnReportHelper.Setup (GetStaticUrl ("spreadUrl") + "/spreadc/cdn/report.json");
				}
                HaApplicationContext.getConfiguration().SetEncrypt(_remoteStaticVersion.encrypt);

                if (onFinish != null)
                    onFinish();
            }
            else
            {
                if (onError != null)
                    onError("加载静态数据版本信息失败");
            }
        }, onError);
    }

    //临时方式，兼容旧版本配置，全部版本更新后可以删除
    private void FormatStaticVersion(StaticVersion versionData)
    {
        if (versionData != null)
        {
            if (versionData.urlMaps == null)
            {
                versionData.urlMaps = new Dictionary<string, string>();
                versionData.urlMaps.Add("configUrl", versionData.configUrl);
                versionData.urlMaps.Add("behaviorUrl", versionData.behaviorUrl);
            }

            if (versionData.versionMaps == null)
            {
                versionData.versionMaps = new Dictionary<string, string>();
                versionData.versionMaps.Add("staticServerList.txt", versionData.staticServerList);
                versionData.versionMaps.Add("announcementData.txt", versionData.announcementData);
                versionData.versionMaps.Add("customerServiceData.txt", versionData.customerServiceData);
            }
        }
    }

    private string GetLocalDir()
    {
        return GameResPath.persistentDataPath + "/staticConfig/";
    }

    private string GetLocalPath(string staticFileName)
    {
        return GetLocalDir() + staticFileName;
    }

	/// <summary>
	///     加载静态配置
	/// </summary>
	/// <param name="configName">配置类型</param>
	/// <param name="onLoadFinish">加载完毕回调</param>
	public void LoadStaticConfig (string configName, Action<string> onLoadFinish, Action<string> onError)
	{
		bool sameVer = GetLocalStaticDataVer (configName) == GetRemoteStaticDataVer (configName);

		#if UNITY_EDITOR
		sameVer = false;
		#endif

		string localPath = GetLocalPath (configName);

		string localJsonStr = "";

        try
        {
            if (sameVer && File.Exists(localPath))
            {
                var fileBytes = FileHelper.ReadAllBytes(localPath);
                var jsonBytes = ZipLibUtils.Uncompress(fileBytes);
                localJsonStr = Encoding.UTF8.GetString(jsonBytes);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        if (string.IsNullOrEmpty(localJsonStr))
        {
            string url = string.Format("{0}/servers/{1}?ver={2}", GameSetting.CONFIG_SERVER, configName,
                                        GetRemoteStaticDataVer(configName));

			Debug.Log ("LoadStaticConfig url= " + url);

            HttpController.Instance.DownLoad(url, delegate (ByteArray byteArray)
            {
                string json = byteArray.ToUTF8String();
                if (configName != Type_StaticVersion)
                {
                    FileHelper.SaveJsonText(json, GetLocalPath(configName), true);
                    SaveLocalStaticDataVer(configName, GetRemoteStaticDataVer(configName));
                }
                if (onLoadFinish != null)
                    onLoadFinish(json);
			}, 
			null, 
			delegate(Exception obj) {
				if (onError != null)
                {
                    CSGameDebuger.Log(string.Format("游戏数据({0})加载出错({1})，请重试", configName, obj.ToString()));
					onError ("游戏数据加载出错，请重试");
                }
			}, false, SimpleWWW.ConnectionType.Short_Connect);
		} else {
			if (onLoadFinish != null)
				onLoadFinish (localJsonStr);
		}
	}

	private static string LocalStaticDataVer_PREFIX = "LocalStaticDataVer_";

	//获取本地静态数据版本
	public string GetLocalStaticDataVer (string configName)
	{
		string version = PlayerPrefs.GetString (LocalStaticDataVer_PREFIX + configName);

        if (string.IsNullOrEmpty(version))
        {
            version = DateTime.Now.Ticks.ToString();
        }

		return version;
	}

	//保存数据版本到本地
	private void SaveLocalStaticDataVer (string configName, string version)
	{
		PlayerPrefs.SetString (LocalStaticDataVer_PREFIX + configName, version);
	}

    //获取远程静态数据版本
    private string GetRemoteStaticDataVer(string configName)
    {
        string version = null;
        if (_remoteStaticVersion != null)
        {
            _remoteStaticVersion.versionMaps.TryGetValue(configName, out version);
        }

        if (string.IsNullOrEmpty(version))
        {
            version = DateTime.Now.Ticks.ToString();
        }

		return version;
	}

    //获取远程静态路径
    //这里是0.9版本修改为public的
    public string GetStaticUrl(string key)
    {
        string url = "";
        if (_remoteStaticVersion != null)
        {
            _remoteStaticVersion.urlMaps.TryGetValue(key, out url);
        }

		return url;
	}

	//清理保存的文件
	public void CleanData()
	{
		String path = GetLocalDir();
		if (Directory.Exists(path))
		{
			Directory.Delete(path, true);
		}
	}
}

public class StaticVersion
{
	public Dictionary<string, string> urlMaps;
	public Dictionary<string, string> versionMaps;

	//临时兼容旧的配置,全部版本更新后可以删除
	public string customerServiceData;
	public string announcementData;
	public string behaviorUrl; //这个0.9版本可废弃 由spreadUrl+/spreadc/behavior/info.json?message=组合而成
	public string configUrl;
	public string staticServerList;
	//

	public bool encrypt = true;
}