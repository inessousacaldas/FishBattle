using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using AssetPipeline;
using UnityEditor;


[InitializeOnLoad]
public static class ChannelSetting
{
    #region 图片切换
    public static readonly string ProjectIconPathFormat = Application.dataPath + "/IconAndSplash/Icon/{0}/{1}.png";
	public static readonly string ChannelIconFolderFormat = Path.GetDirectoryName(Application.dataPath) + "/SpRes/{0}/{1}/Icons/{2}";
	public static readonly string CommonIconFolderFormat = Path.GetDirectoryName(Application.dataPath) + "/SpRes/{0}/commons/Icons/{1}";
	public static readonly string ResourcesIconPath = Application.dataPath + "/Resources/Textures/gamelogo_wx.png";
	public static readonly int ResourcesIconSize = 72;

	public static readonly string ProjectSplashPathFormat = Application.dataPath + "/IconAndSplash/Splash/{0}/{1}.jpg";
	public static readonly string ChannelSplashFolderFormat = Path.GetDirectoryName(Application.dataPath) + "/SpRes/{0}/{1}/Splashs/{2}";
	public static readonly string CommonSplashFolderFormat = Path.GetDirectoryName(Application.dataPath) + "/SpRes/{0}/commons/Splashs/{1}";

	public static readonly string ProjectLogoTexturePath = Application.dataPath + "/Resources/Textures/logo.png";
	public static readonly string ProjectTextureFolderFormat = Application.dataPath + "/Resources/Textures/LoadingBG/{0}";
    public static readonly string CommonTextureFolderFormat = Path.GetDirectoryName(Application.dataPath) + "/SpRes/{0}/commons/Textures";

	public static readonly string ProjectSettingPathFormat = Application.dataPath + "/Resources/Setting/{0}";
	public static readonly string CommonSettingFolderFormat = Path.GetDirectoryName(Application.dataPath) + "/SpRes/{0}/commons/Setting";

	private enum ProjectIcon
    {
        Android,
        iOS,
    }


    private enum ChannelIcon
    {
        Android,
        iPad,
        iPhone,
    }

	private enum ChannelSplash
	{
		Android,
		iOS,
	}

//    [MenuItem("Test/Test")]
    public static void Test()
    {
        foreach (var pluginImporter in PluginImporter.GetAllImporters())
        {
            Debug.Log(pluginImporter.assetPath);
        }
    }

    static ChannelSetting()
    {
        ProjectCallback.RegisterAfterPlayerSettingToolSave(ProjectCallback.AfterSaveOrder.ChannelSetting, PlayerSettingToolAfterSave);
    }


    public static void PlayerSettingToolAfterSave()
    {
        ChangeIcons();
		ChangeSplash();
		ChangeTextures();
		ChangeSettings();
        ChangeSDKSetting();

        AssetDatabase.Refresh();
    }

    public static void ChangeIcons()
    {
        var gameSetting = GameSetting.LoadGameSettingData();
		var channelDict = SPSdkManager.SpChannelDic(gameSetting.configSuffix, false);

        if (!channelDict.ContainsKey(gameSetting.channel))
        {
            return;
        }

        var curChannel = channelDict[gameSetting.channel];
        var pIcon = ProjectIcon.Android;
        var cIconList = new List<ChannelIcon>();

        switch (gameSetting.platformType)
        {
            case GameSetting.PlatformType.Android:
                {
                    pIcon = ProjectIcon.Android;
                    cIconList.Add(ChannelIcon.Android);
                    break;
                }
            case GameSetting.PlatformType.ROOTIOS:
            case GameSetting.PlatformType.IOS:
                {
                    pIcon = ProjectIcon.iOS;
                    cIconList.Add(ChannelIcon.iPad);
                    cIconList.Add(ChannelIcon.iPhone);
                    break;
                }
        }

		string configSuffix = gameSetting.configSuffix;
		if (string.IsNullOrEmpty(configSuffix))
		{
			configSuffix = "xlsj";
		}

        var checkChannelFolder = new List<string>();
        foreach (var channelIcon in cIconList)
        {
			var checkFolder = string.Format(ChannelIconFolderFormat, configSuffix, curChannel.name, channelIcon);
            Debug.Log(checkFolder);
            if (!Directory.Exists(checkFolder))
            {
				checkFolder = string.Format(CommonIconFolderFormat, configSuffix, channelIcon);
                Debug.Log(checkFolder);
            }
            checkChannelFolder.Add(checkFolder);
        }

        var tex = new Texture2D(0, 0);
        foreach (var folder in checkChannelFolder)
        {
            foreach (var file in Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories))
            {
//                Debug.Log(file);
                var bytes = File.ReadAllBytes(file);
                tex.LoadImage(bytes);
                var projectIconPath = string.Format(ProjectIconPathFormat, pIcon, string.Format("{0}x{0}", tex.width));
//                Debug.Log(projectIconPath);
                if (File.Exists(projectIconPath))
                {
                    File.WriteAllBytes(projectIconPath, bytes);
                }
                else
                {
//                    Debug.Log(file);
//                    Debug.Log(projectIconPath);
                }

	            if (tex.width == ResourcesIconSize)
	            {
		            File.WriteAllBytes(ResourcesIconPath, bytes);
	            }
            }
        }
    }
    /// <summary>
    /// Android和iOS共用Mobile Splash Screen
    /// </summary>
    public static void ChangeSplash()
    {
        var gameSetting = GameSetting.LoadGameSettingData();
        var channelDict = SPSdkManager.SpChannelDic(gameSetting.configSuffix, false);

        if (!channelDict.ContainsKey(gameSetting.channel))
		{
			return;
		}

        var curChannel = channelDict[gameSetting.channel];

        string configSuffix = gameSetting.configSuffix;
        if (string.IsNullOrEmpty(configSuffix))
        {
            configSuffix = "xlsj";
        }

        foreach (ChannelSplash cSplash in Enum.GetValues(typeof(ChannelSplash)))
        {
            var checkFolder = string.Format(ChannelSplashFolderFormat, configSuffix, curChannel.name, cSplash);
            if (!Directory.Exists(checkFolder))
            {
                checkFolder = string.Format(CommonSplashFolderFormat, configSuffix, cSplash);
            }
            Debug.Log(checkFolder);

            var tex = new Texture2D(0, 0);
            foreach (var file in Directory.GetFiles(checkFolder, "*.jpg", SearchOption.AllDirectories))
            {
                //Debug.Log(file);
                var bytes = File.ReadAllBytes(file);

                if (cSplash == ChannelSplash.iOS)
                {
                    tex.LoadImage(bytes);
                    var projectSplashPath = string.Format(ProjectSplashPathFormat, cSplash, string.Format("{0}x{1}", tex.width, tex.height));
                    //	Debug.Log(projectSplashPath);
                    if (File.Exists(projectSplashPath))
                    {
                        File.WriteAllBytes(projectSplashPath, bytes);
                    }
                    else
                    {
                        //Debug.Log(file);
                        //Debug.Log(projectSplashPath);
                    }
                }

                if (cSplash == ChannelSplash.Android)
                {
                    var projectSplashPath2 = string.Format(ProjectSplashPathFormat, cSplash, "splash");
                    //	Debug.Log(projectSplashPath);
                    if (File.Exists(projectSplashPath2))
                    {
                        File.WriteAllBytes(projectSplashPath2, bytes);
                    }
                    else
                    {
                        //Debug.Log(file);
                        //Debug.Log(projectSplashPath);
                    }
                }
            }
        }
    }


	public static void ChangeTextures()
	{
		var gameSetting = GameSetting.LoadGameSettingData();
		var configSuffix = gameSetting.configSuffix;
		if (string.IsNullOrEmpty(configSuffix))
		{
			configSuffix = "xlsj";
		}

		var commonFloder = string.Format(CommonTextureFolderFormat, configSuffix);
		foreach (var file in Directory.GetFiles(commonFloder))
		{
			string fileName = Path.GetFileName(file);
			string projectTexture = "";
			if (ProjectLogoTexturePath.EndsWith(fileName))
			{
				projectTexture = ProjectLogoTexturePath;
			}
			else
			{
				projectTexture = string.Format(ProjectTextureFolderFormat, fileName);
			}
			if (File.Exists(projectTexture))
			{
				Debug.Log(file);
				File.Copy(file, projectTexture, true);
			}
		}
	}

	public static void ChangeSettings()
	{
		var gameSetting = GameSetting.LoadGameSettingData();
		var configSuffix = gameSetting.configSuffix;
		if (string.IsNullOrEmpty(configSuffix))
		{
			configSuffix = "xlsj";
		}

		var commonFloder = string.Format(CommonSettingFolderFormat, configSuffix);
		foreach (var file in Directory.GetFiles(commonFloder))
		{
			string fileName = Path.GetFileName(file);
			string projectSetting = string.Format(ProjectSettingPathFormat, fileName);
			if (File.Exists(projectSetting))
			{
				Debug.Log(file);
				File.Copy(file, projectSetting, true);
			}
		}
	}
    #endregion

#region 修改是否使用SDK
	private class SDKSetting
	{
		public string propertiesPath;
        public string Symbol;
		public bool IsSymbolEnable;
	    public BuildTarget[] Targets;

	    public SDKSetting(string propertiesPath, string symbol, bool isSymbolEnable, BuildTarget[] targets)
	    {
	        this.propertiesPath = propertiesPath;
	        Symbol = symbol;
	        IsSymbolEnable = isSymbolEnable;
	        Targets = targets;
	    }
	}

	private static readonly List<SDKSetting> _sdkSettingList = new List<SDKSetting>()
	{
		new SDKSetting("Assets/Plugins/Android/TianCityAd", "ENABLE_TCAD", true, new []{BuildTarget.Android, }),
		new SDKSetting("Assets/Plugins/Android/TalkingDataAd", "ENABLE_TDAD", true, new []{BuildTarget.Android, }),
		new SDKSetting("Assets/Plugins/Android/XinGe", "ENABLE_XINGE", true, new []{BuildTarget.Android, }),
	};


    public static void ChangeSDKSetting()
    {
        var gameSetting = GameSetting.LoadGameSettingData();
		var channelDict = SPSdkManager.SpChannelDic(gameSetting.configSuffix, false);

        if (!channelDict.ContainsKey(gameSetting.channel))
        {
            return;
        }

        var curChannel = channelDict[gameSetting.channel];

		foreach (var setting in _sdkSettingList)
		{
			var path = setting.propertiesPath;
		    var plugin = AssetImporter.GetAtPath(path) as PluginImporter;
		    if (plugin == null)
		    {
		        continue;
		    }

		    var openFlag = (setting.IsSymbolEnable && curChannel.symbol.Contains(setting.Symbol)) ||
		               (!setting.IsSymbolEnable && !curChannel.symbol.Contains(setting.Symbol));
		    foreach (var buildTarget in setting.Targets)
		    {
		        plugin.SetCompatibleWithPlatform(buildTarget, openFlag);
		    }
            plugin.SaveAndReimport();
		}
    }
    #endregion

    #region 修改sdk配置
    public static void ChangeAndroidManifest()
    {
        ChangeXinGeManifest();
    }

    private static void ChangeXinGeManifest()
    {
        var path = "Assets/Plugins/Android/XinGe/AndroidManifest.xml";
        var accessIdFormat = @"<meta-data android:name=""com.xinge.AccessId"" android:value=""(.+)""/>";
        var accessKeyFormat = @"<meta-data android:name=""com.xinge.AccessKey"" android:value=""(.+)""/>";
        var bundleNameFormat = @"<action android:name=""(.+).PUSH_ACTION"" />";

        var strs = FileHelper.ReadAllText(path);
        strs = Regex.Replace(strs, accessIdFormat,
            match => match.Value.Replace(match.Groups[1].Value, "2100250562"));
        strs = Regex.Replace(strs, accessKeyFormat,
            match => match.Value.Replace(match.Groups[1].Value, "AZ131KMM66YQ"));
        strs = Regex.Replace(strs, bundleNameFormat,
            match => match.Value.Replace(match.Groups[1].Value, PlayerSettings.applicationIdentifier));
        FileHelper.WriteAllText(path, strs);
    }
    #endregion
}
