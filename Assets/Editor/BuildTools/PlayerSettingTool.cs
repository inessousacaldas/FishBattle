// **********************************************************************
// Copyright  2013 Baoyugame. All rights reserved.
// File     :  GameSettingWindow.cs
// Author   : wenlin
// Created  : 2013/5/6 20:15:32
// Purpose  : 
// **********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AssetPipeline;
using LITJson;
using UnityEditor;
using UnityEditor.XCodeEditor;
using UnityEngine;
public class PlayerSettingTool : EditorWindow
{
    private static PlayerSettingTool instance = null;

    [MenuItem("Tools/persistentDataPath", false, 0)]
    static void PersistentDataPath()
    {
        System.Diagnostics.Process.Start(Application.persistentDataPath);
    }

    [MenuItem("Tools/temporaryCachePath", false, 0)]
    static void TemporaryCachePath()
    {
#if UNITY_EDITOR_WIN
        System.Diagnostics.Process.Start("C:\\Users\\Administrator\\AppData\\LocalLow\\Unity\\WebPlayer\\Cache");
#else
		System.Diagnostics.Process.Start (Application.temporaryCachePath);
#endif
    }

    [MenuItem("Tools/PlayerSettingTool %#t", false, 0)]
    public static void ShowWindow()
    {
        if (instance == null)
        {
            PlayerSettingTool window = (PlayerSettingTool)EditorWindow.GetWindow(typeof(PlayerSettingTool));
            window.minSize = new Vector2(562, 562);
            window.Show();
            PlayerSettingTool.instance = window;
        }
        else
        {
            PlayerSettingTool.instance.Close();
        }
    }

    private void OnEnable()
    {
        PlayerSettingTool.instance = this;

        _enableJSB = HasEnableJSBDefine();
        _useJsz = HasUseJszDefine();
        _minResBuild = IsMinResBuild();
        _resLoadMode = (AssetPipeline.AssetManager.LoadMode)EditorPrefs.GetInt("ResLoadMode", 0);
        if (_gameSettingData == null)
            GetGameSettingData();
    }

    private void OnDisable()
    {
        PlayerSettingTool.instance = null;
    }

    private Dictionary<string, GameInfo> _gameInfoDic;

    //游戏类型
    private int[] _gameTypeKeys;
    private string[] _gameTypeValues;

    //运行域
    private int[] _domainKeys;
    private string[] _domainValues;

    //渠道平台
    private int[] _channelKeys;
    private string[] _channelValues;

    private GameSettingData _gameSettingData = null;
    private bool _developmentBuild = false;
    private bool _minResBuild = false;
    private bool _enableJSB = false;
    private bool _useJsz = false;
    private AssetPipeline.AssetManager.LoadMode _resLoadMode;
    private Dictionary<string, SPChannel> _spChannelDic;
    private const string developmentBuildKey = "PlayerSettingTool._developmentBuild";
    private bool developmentBuild
    {
        get { return _developmentBuild; }
        set
        {
            if (_developmentBuild != value)
            {
                _developmentBuild = value;
                EditorPrefs.SetBool(developmentBuildKey, _developmentBuild);
            }
        }
    }

    //运行域别名
    private Dictionary<string, string> _domainAliasNameDic;

    private static string DomainHttpConfigPath = "Assets/Editor/BuildTools/Configs/DomainHttpConfig.json";

    private void GetGameSettingData()
    {
        _gameInfoDic = FileHelper.ReadJsonFile<Dictionary<string, GameInfo>>(DomainHttpConfigPath);

        _gameSettingData = GameSetting.LoadGameSettingData();
        if (_gameSettingData == null)
        {
            _gameSettingData = new GameSettingData();
        }
        InitDomainAliasNameDic();
        InitGameTypeConfig();
        LoadSPChannelConfig(_gameSettingData.configSuffix, true);
        developmentBuild = EditorPrefs.GetBool(developmentBuildKey, false);
    }

    private void InitDomainAliasNameDic()
    {
        if (_domainAliasNameDic == null)
        {
            _domainAliasNameDic = new Dictionary<string, string>();
            _domainAliasNameDic.Add("LocalDev", "_内开发");
            _domainAliasNameDic.Add("LocalTest", "_内测试");
            _domainAliasNameDic.Add("LocalRelease", "_仿正式");
            _domainAliasNameDic.Add("LocalForever", "_仿永测");
            _domainAliasNameDic.Add("Business", "_商务");
        }
    }

    private string GetDomainAliasName(string domainType)
    {
        string name = "";
        if (_domainAliasNameDic != null)
        {
            _domainAliasNameDic.TryGetValue(domainType, out name);
        }
        return name;
    }

    private void InitGameTypeConfig()
    {
        _gameTypeKeys = new int[_gameInfoDic.Count];
        _gameTypeValues = new string[_gameInfoDic.Count];
        int index = 0;
        foreach (string key in _gameInfoDic.Keys)
        {
            _gameTypeKeys[index] = index;
            _gameTypeValues[index] = key;
            index++;
        }
    }

    private void LoadSPChannelConfig(string configSuffix, bool forceLoad)
    {
        _spChannelDic = SPSdkManager.SpChannelDic(configSuffix, forceLoad);
    }

    private DomainInfo GetDomainInfo(string gameType, string domainType)
    {
        if (_gameInfoDic.ContainsKey(gameType))
        {
            GameInfo info = _gameInfoDic[gameType];
            if (info.domains != null)
            {
                for (int i = 0; i < info.domains.Count; i++)
                {
                    DomainInfo domainInfo = info.domains[i];
                    if (domainInfo.type == domainType)
                    {
                        return domainInfo;
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public string GetChannelAlias(string id)
    {
        SPChannel info = null;
        if (_spChannelDic.TryGetValue(id, out info))
        {
            return info.alias;
        }
        else
        {
            return "无";
        }
    }

    public string GetChannelBundleId(string id)
    {
        SPChannel info = null;
        if (_spChannelDic.TryGetValue(id, out info))
        {
            return info.bundleId;
        }
        else
        {
            return "null";
        }
    }

    public string GetChannelSymbol(string id)
    {
        SPChannel info = null;
        if (_spChannelDic.TryGetValue(id, out info))
        {
            return info.symbol;
        }
        else
        {
            return "";
        }
    }

    public string GetChannelProjmods(string channel)
    {
        SPChannel info = null;
        if (_spChannelDic.TryGetValue(channel, out info))
        {
            return info.projmods;
        }
        else
        {
            return "";
        }
    }

    private string _lastSelectGameType;
    private int _lastSelectGameTypeIndex = 0;
    private int _lastSelectDomainIndex = 0;
    private int _lastSelectChannelIndex = 0;

    private void UpdateDomainList(string gameType, string domainType)
    {
        if (_lastSelectGameType != gameType || _domainKeys == null)
        {
            _lastSelectDomainIndex = 0;
            _lastSelectGameType = gameType;
            if (_gameInfoDic.ContainsKey(gameType))
            {
                GameInfo gameInfo = _gameInfoDic[gameType];
                _domainKeys = new int[gameInfo.domains.Count];
                _domainValues = new string[gameInfo.domains.Count];

                for (int i = 0; i < gameInfo.domains.Count; i++)
                {
                    DomainInfo domainInfo = gameInfo.domains[i];
                    _domainKeys[i] = i;
                    _domainValues[i] = domainInfo.type;

                    if (domainInfo.type == domainType)
                    {
                        _lastSelectDomainIndex = i;
                    }
                }
            }
        }
    }

    private string GetDomianType(string gameType, int domainIndex)
    {
        GameInfo gameInfo = _gameInfoDic[gameType];
        if (gameInfo.domains.Count > domainIndex)
        {
            return gameInfo.domains[domainIndex].type;
        }
        return "";
    }

    private void UpdateSpSdkList(string domainType, GameSetting.PlatformType platformType, string selectChannel)
    {
        List<SPChannel> spList = new List<SPChannel>();
        foreach (SPChannel sp in _spChannelDic.Values)
        {
            if (!string.IsNullOrEmpty(sp.platforms) && sp.platforms.Contains(((int)platformType).ToString())
                && !string.IsNullOrEmpty(sp.domains) && sp.domains.Contains(domainType))
            {
                spList.Add(sp);
            }
        }

        _channelKeys = new int[spList.Count];
        _channelValues = new string[spList.Count];
        for (int i = 0; i < spList.Count; i++)
        {
            _channelKeys[i] = i;
            _channelValues[i] = spList[i].name;
            if (selectChannel == spList[i].name)
            {
                _lastSelectChannelIndex = i;
            }
        }
    }

    private void SaveGameSettingData()
    {
        if (_gameSettingData != null)
        {
            FileHelper.SaveJsonObj(_gameSettingData, GameSetting.Config_WritePathV2, false);
        }
        EditorPrefs.SetInt("ResLoadMode", (int)_resLoadMode);
        AssetDatabase.Refresh();
    }

    private Vector2 _scrollPos;
    private string _buildToFileName = "";
    private string _buildProjmods = "";

    private void OnGUI()
    {
        if (_gameSettingData == null)
            GetGameSettingData();

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("ProductName ： " + PlayerSettings.productName);
            EditorGUILayout.LabelField("Bundle Identifier ： " + PlayerSettings.applicationIdentifier);
            EditorGUILayout.LabelField("Bundle Version ： " + PlayerSettings.bundleVersion);
            EditorGUILayout.LabelField("Bundle Version Code： " + PlayerSettings.Android.bundleVersionCode);
            EditorGUILayout.LabelField("HttpRoot： " + _gameSettingData.httpRoot);
            EditorGUILayout.LabelField("BuildToFile： " + _buildToFileName);
            EditorGUILayout.LabelField("projmods： " + _buildProjmods);
            EditorGUILayout.Space();

            //GameType选项
            int selectGameTypeIndex = EditorGUILayout.IntPopup("GameType :", _lastSelectGameTypeIndex,
                _gameTypeValues, _gameTypeKeys);

            if (_lastSelectGameTypeIndex != selectGameTypeIndex)
            {
                _lastSelectGameTypeIndex = selectGameTypeIndex;
                _gameSettingData.gameType = _gameTypeValues[_lastSelectGameTypeIndex];
                GameInfo gameInfo = _gameInfoDic[_gameSettingData.gameType];
                _gameSettingData.gamename = gameInfo.gamename;
                _gameSettingData.configSuffix = gameInfo.configSuffix;

                LoadSPChannelConfig(_gameSettingData.configSuffix, true);
            }

            if (_gameInfoDic.ContainsKey(_gameSettingData.gameType) == false)
            {
                EditorGUILayout.LabelField(string.Format("GameType {0} no support!!!", _gameSettingData.gameType));
                EditorGUILayout.EndScrollView();
                return;
            }

            //Domain类型选项
            UpdateDomainList(_gameSettingData.gameType, _gameSettingData.domainType);
            _lastSelectDomainIndex = EditorGUILayout.IntPopup("DomainType : ", _lastSelectDomainIndex,
                _domainValues, _domainKeys);

            _gameSettingData.domainType = GetDomianType(_gameSettingData.gameType, _lastSelectDomainIndex);

            DomainInfo domainInfo = GetDomainInfo(_gameSettingData.gameType, _gameSettingData.domainType);
            if (domainInfo == null)
            {
                EditorGUILayout.LabelField(string.Format("DomainType {0} no support!!!", _gameSettingData.domainType));
                EditorGUILayout.EndScrollView();
                return;
            }
            _gameSettingData.httpRoot = domainInfo.url;
            _gameSettingData.resdir = domainInfo.resdir;

            //Platform类型选项
            _gameSettingData.platformType = (GameSetting.PlatformType)EditorGUILayout.EnumPopup("PlatformType :", _gameSettingData.platformType);

            //Channel类型选项
            UpdateSpSdkList(_gameSettingData.domainType, _gameSettingData.platformType, _gameSettingData.channel);
            if (_channelKeys != null && _channelValues != null)
            {
                _lastSelectChannelIndex = EditorGUILayout.IntPopup("Channel : ", _lastSelectChannelIndex,
                    _channelValues, _channelKeys);
            }
            else
            {
                EditorGUILayout.LabelField("Channel : Load channel Info Failed!!!");
                EditorGUILayout.EndScrollView();
                return;
            }

            if (_channelValues.Length > _lastSelectChannelIndex)
            {
                _gameSettingData.channel = _channelValues[_lastSelectChannelIndex];
            }

            //资源加载配置选项
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("注意：加载模式的更改只对编辑器运行时有效，打包手机包时选择Mobile模式");
            _resLoadMode = (AssetPipeline.AssetManager.LoadMode)EditorGUILayout.EnumPopup("资源加载模式 :", _resLoadMode);

            //调试选项
            EditorGUILayout.Space();
            _gameSettingData.release = EditorGUILayout.Toggle("Client Release : ", _gameSettingData.release);
            _gameSettingData.testServerMode = EditorGUILayout.Toggle("压测模式 : ", _gameSettingData.testServerMode);
            _gameSettingData.gmMode = EditorGUILayout.Toggle("GM模式 : ", _gameSettingData.gmMode);
            developmentBuild = EditorGUILayout.Toggle("Development Build : ", developmentBuild);
            EditorGUILayout.Toggle("MinRes Build : ", _minResBuild);
            _enableJSB = EditorGUILayout.Toggle("Enable JSB: ", _enableJSB);
            _useJsz = EditorGUILayout.Toggle("Use Jsz: ", _useJsz);
            _gameSettingData.logType = (GameSetting.DebugInfoType)EditorGUILayout.EnumPopup("调试信息类型 :", _gameSettingData.logType);

            EditorGUILayout.Space();
            GUI.color = Color.yellow;
            if (GUILayout.Button("保存配置", GUILayout.Height(40)))
            {
                if (!CheckIsCompiling())
                {
                    SaveComeFromConfig();
                }
            }

#if UNITY_ANDROID
            EditorGUILayout.Space();
            if (GUILayout.Button("导出Android Project", GUILayout.Height(40)))
            {
                string outPutPath = EditorUtility.OpenFolderPanel("导出Android工程目录", "", "");
                if (!string.IsNullOrEmpty(outPutPath))
                {
                    EditorHelper.Run(() => ExportAndroidProject(outPutPath), true, false);
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("打包当前渠道", GUILayout.Height(40)))
            {
                if (EditorUtility.DisplayDialog("打包确认", "是否确认打包当前渠道?", "确认打包", "取消"))
                {
                    EditorApplication.delayCall += () => BuildAndroid();
                }
            }

            //			EditorGUILayout.Space ();
            //			if (GUILayout.Button ("打包所有商务渠道", GUILayout.Height (40))) {
            //				if (EditorUtility.DisplayDialog ("打包确认", "是否确认打包所有渠道?", "确认打包", "取消")) {
            //					
            //				}
            //			}
#endif

#if UNITY_IPHONE
			EditorGUILayout.Space ();
			if (GUILayout.Button ("Expor2XCODE", GUILayout.Height (40))) {
				if (EditorUtility.DisplayDialog ("导出确认", "是否确认导出XCODE?", "确认", "取消")) 
				{
                     EditorApplication.delayCall += () =>
                     {
                         string applicationPath = Application.dataPath.Replace("/Assets", "/../..");
                         string target_dir = EditorUtility.OpenFolderPanel("导出目录", applicationPath, "xcode");
                         BuildIOS(target_dir);
                     };
				}
			}

			EditorGUILayout.Space ();
			if (GUILayout.Button ("ExportAllIpa", GUILayout.Height (40))) {
				 EditorApplication.delayCall += () => BuildAllIpa();
			}
#endif

#if UNITY_STANDALONE_WIN && !UNITY_IPHONE
            EditorGUILayout.Space();
            if (GUILayout.Button("一按键打包PC", GUILayout.Height(40)))
            {
                if (EditorUtility.DisplayDialog("打包确认", "是否确认打包PC版?", "确认打包", "取消"))
                {
                    EditorApplication.delayCall += () => BuildPC();
                }
            }
#endif
            EditorGUILayout.Space();
            GUI.color = Color.green;
            if (GUILayout.Button("清除本地数据和PlayerPrefs", GUILayout.Height(40)))
            {
                EditorApplication.delayCall += () =>
                {
                    Debug.Log("清除本地数据和PlayerPrefs");
                    PlayerPrefs.DeleteAll();
                    FileUtil.DeleteFileOrDirectory(Application.persistentDataPath);
                };
            }
            GUI.color = Color.white;

            EditorGUILayout.Space();
            if (GUILayout.Button("上传Dlls", GUILayout.Height(40)))
            {
				GameResVersionManager.UploadDllsPatch();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("一键打包", GUILayout.Height(40)))
            {
                OneKeyBuildStep1();
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private string GetOneKeyBuildPrefKeyStep1()
    {
        return Application.dataPath + EditorUserBuildSettings.activeBuildTarget + "OneKeyBuildStep1";
    }

    private static string GetOneKeyBuildPrefKeyStep2()
    {
        return Application.dataPath + EditorUserBuildSettings.activeBuildTarget + "OneKeyBuildStep2";
    }
    private void OneKeyBuildStep1()
    {
        EditorApplication.delayCall += () =>
        {
            if (CheckIsCompiling())
                return;
#if USE_JSZ
            bool converJSB = EditorUtility.DisplayDialog("提示", "是否转换JSB", "是", "否");
            bool reverTempJSB = false;
            if (converJSB)
                reverTempJSB = JsExternalTools.CheckTempJSBCodeRoot();
#endif
            bool useCodeMove = EditorUtility.DisplayDialog("提示", "是否移除JSB或业务代码", "移除", "不移除");
            JsonData jsonData = new JsonData();
            jsonData["useCodeMove"] = useCodeMove;

#if UNITY_IPHONE
            string applicationPath = Application.dataPath.Replace("/Assets", "/../..");
            string target_dir = EditorUtility.OpenFolderPanel("导出目录", applicationPath, "xcode");
            jsonData["target_dir"] = target_dir;
#endif
            Debug.Log("PlayerSettingTool Begin OneKeyBuildStep1");
#if USE_JSZ
            if (converJSB)
            {
                if(reverTempJSB)
                    CodeManagerTool.revertUnUsedMonoCode(true);
                JsExternalTools.OneKeyBuildAll(true, true);
            }
#endif
            EditorPrefs.SetString(GetOneKeyBuildPrefKeyStep1(), jsonData.ToJson());
#if !USE_JSZ
            OneKeyBuildStep2();
#else
            if (!converJSB)
                OneKeyBuildStep2();
#endif
        };
    }

    public static void OneKeyBuildStep2()
    {
        if (instance != null)
            instance.mOneKeyBuildStep2();
    }
    private void mOneKeyBuildStep2()
    {
        string str = EditorPrefs.GetString(GetOneKeyBuildPrefKeyStep1(), string.Empty);
        EditorPrefs.DeleteKey(GetOneKeyBuildPrefKeyStep1());
        if (string.IsNullOrEmpty(str))
        {
            Debug.LogError("Can Not Find OneKeyBuildStep2 param");
            return;
        }
        Debug.Log("PlayerSettingTool Begin OneKeyBuildStep2");
        JsonData jsonData = JsonMapper.ToObject(str);
        bool useCodeMove = jsonData["useCodeMove"].GetBoolean();

#if UNITY_IPHONE
        string target_dir = jsonData["target_dir"].GetString();
#endif
        if (useCodeMove)
        {
#if USE_JSZ
            CodeManagerTool.moveUnUsedMonoCode(true);
#else
            CodeManagerTool.moveJSBFramework(true);
#endif         
        }
        AssetBundleBuilder.ShowWindow();
        AssetBundleBuilder.Instance._slientMode = true;
        Debug.Log("PlayerSettingTool UpdateAllBundleName");
        AssetBundleBuilder.Instance.UpdateAllBundleName();
        if (AssetBundleBuilder._curResConfig == null)
        {
            throw new SystemException("AssetBundleBuilder._curResConfig == null");
        }
        Debug.Log("PlayerSettingTool BuildAllAssetBundle");
        AssetBundleBuilder.Instance.BuildAll(AssetBundleBuilder._curResConfig.Version + 1, true);
        AssetBundleBuilder.ShowWindow();
        EditorPrefs.SetString(GetOneKeyBuildPrefKeyStep2(), jsonData.ToJson());
        Debug.Log("PlayerSettingTool OneKeyBuildStep2 Finish");
#if UNITY_IPHONE
        mOneKeyBuildStep3();
#else
        if (!EditorApplication.isCompiling)
            mOneKeyBuildStep3();
#endif
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OneKeyBuildStep3()
    {
        EditorApplication.delayCall += () =>
        {
            if (instance != null)
                instance.mOneKeyBuildStep3();
        };
    }
    private void mOneKeyBuildStep3()
    {
        string str = EditorPrefs.GetString(GetOneKeyBuildPrefKeyStep2(), string.Empty);
        EditorPrefs.DeleteKey(GetOneKeyBuildPrefKeyStep2());
        JsonData jsonData = JsonMapper.ToObject(str);
        if (string.IsNullOrEmpty(str))
            return;
        Debug.Log("PlayerSettingTool Begin OneKeyBuildStep3");
#if UNITY_IPHONE
        string target_dir = jsonData["target_dir"].GetString();
        BuildIOS(target_dir, true);
#endif
#if UNITY_STANDALONE_WIN && !UNITY_IPHONE
        BuildPC(true);
#endif
#if UNITY_ANDROID
        BuildAndroid(true);
#endif
        Debug.Log("PlayerSettingTool OneKeyBuild Finish");
    }
    private void BuildIOS(string target_dir, bool cmd = false)
    {
        if (string.IsNullOrEmpty(target_dir))
        {
            return;
        }

        string target_name = string.Format("H1_{0}_{1}_{2}", AppGameVersion.ShortBundleVersion, _gameSettingData.channel, _gameSettingData.domainType.ToString());

        if (!Directory.Exists(target_dir))
        {
            Directory.CreateDirectory(target_dir);
        }

        string fullPath = target_dir + "/" + target_name;

        Debug.Log(target_dir + "/" + target_name);

        if (cmd || EditorUtility.DisplayDialog("导出确认", string.Format("是否确认导出 {0}?", fullPath), "确认", "取消"))
        {
            var ipaFlag = cmd ? true : EditorUtility.DisplayDialog("导出ipa", "是否导出ipa？", "确认", "取消");
            var buildoption = BuildOptions.ShowBuiltPlayer;
            if (developmentBuild)
            {
                buildoption = buildoption | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler |
                              BuildOptions.Development;
            }
            string res = BuildPipeline.BuildPlayer(FindEnabledEditorScenes(), target_dir + "/" + target_name, BuildTarget.iOS, buildoption);
            if (res.Length > 0)
            {
                throw new Exception("BuildPlayer failure: " + res);
            }
            if (ipaFlag)
            {
                XCodeToIpaPostProcess.OnPostProcessBuild(BuildTarget.iOS, target_dir + "/" + target_name);
            }
        }
    }

    private void BuildPC(bool cmd = false)
    {
        var exportExe = cmd ? true : EditorUtility.DisplayDialog("导出确认", "是否导出exe", "确认", "取消");
        if (!CheckIsCompiling())
        {
            SaveComeFromConfig();
        }

        string app_name = string.Format("H1_{0}_{1}", PlayerSettings.bundleVersion, _gameSettingData.channel);
        string outputPath = string.Format("Pcbao/{0}/xlsj.exe", app_name);
        string outputDir = Path.GetDirectoryName(outputPath);

        FileHelper.DeleteDirectory(outputDir, true);
        FileHelper.CreateDirectory(outputDir);
        var buildoption = BuildOptions.ShowBuiltPlayer;
        if (developmentBuild)
        {
            buildoption = buildoption | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler |
                          BuildOptions.Development;
        }
        string res = BuildPipeline.BuildPlayer(FindEnabledEditorScenes(), outputPath, BuildTarget.StandaloneWindows, buildoption);
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }

        CommonPostProcessBuild.GenerateWinDll(outputPath);

        if (exportExe)
        {
            CommonPostProcessBuild.GenerateWinExe(BuildTarget.StandaloneWindows, outputPath);
        }
    }
    private void BuildAndroid(bool cmd = false)
    {
        if (!CheckIsCompiling())
        {
            var isExportDll = cmd ? true : EditorUtility.DisplayDialog("导出dll", "是否需要对dll处理？", "确认", "取消");

            SaveComeFromConfig();
            BulidTargetApk(_gameSettingData.channel, isExportDll);
        }
    }
    private bool CheckIsCompiling()
    {
        if (EditorApplication.isCompiling)
        {

            EditorUtility.DisplayDialog("Tip:",
                "please wait EditorApplication Compiling",
                "OK"
            );
            return true;

        }

        return false;
    }

    //保存渠道配置
    private void SaveComeFromConfig()
    {
        SaveGameSettingData();

        // 保存后做特定的回调
        ProjectCallback.TriggerAfterPlayerSettingToolSave();

        //修改游戏签名
        ChangeKeystorePass();

        //修改版本号
        ChangeBundleVersion();

        //修改游戏IconAndSplash
        ChangeIconAndSplash();

        //修改游戏标签
        ChangeBundleIdentifier(_gameSettingData.channel);

        //更新打包文件名
        UpdateBuildToFileName(_gameSettingData.channel);

        //改变需要的宏
        ChangDefineSymbols(_gameSettingData.channel);

        //修改产品名
        ChangeProductName(_gameSettingData.channel);

        //修改BuildeSettings
        ChangBuildSettings();

        //修改打包projmods
        ChangeProjmods(_gameSettingData.channel);

        // 修改打包SDK版本
        ChangeIOSTargetOSVersion();

    }

    private void ChangeKeystorePass()
    {
#if UNITY_EDITOR && UNITY_ANDROID
        PlayerSettings.Android.keystoreName = "PublishKey/nucleus.keystore";
        PlayerSettings.keystorePass = "nucleus123";

        PlayerSettings.Android.keyaliasName = "nucleus";
        PlayerSettings.Android.keyaliasPass = "nucleus123";
#endif
    }

    private void ChangeBundleVersion()
    {
        PlayerSettings.bundleVersion = AppGameVersion.BundleVersion;
        PlayerSettings.Android.bundleVersionCode = AppGameVersion.BundleVersionCode;
#if UNITY_4_6 || UNITY_4_7
        if (_gameSettingData.platformType == GameSetting.PlatformType.ROOTIOS)
        {
            PlayerSettings.shortBundleVersion = AppGameVersion.BundleVersion;
        }
        else
        {
            PlayerSettings.shortBundleVersion = AppGameVersion.ShortBundleVersion;
        }
#endif
    }

    private bool IsMinResBuild()
    {
        string configPath = Path.Combine(Application.streamingAssetsPath, AssetPipeline.GameResPath.MINIRESCONFIG_FILE);
        if (FileHelper.IsExist(configPath))
        {
            return true;
        }
        return false;
    }

    private bool HasEnableJSBDefine()
    {
#if UNITY_IPHONE
        string symbolsDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
        return symbolsDefines.Contains("ENABLE_JSB");
#elif UNITY_ANDROID
        string symbolsDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        return symbolsDefines.Contains("ENABLE_JSB");
#else
        string symbolsDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        return symbolsDefines.Contains("ENABLE_JSB");
#endif
    }

    private bool HasUseJszDefine()
    {
#if UNITY_IPHONE
        string symbolsDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
        return symbolsDefines.Contains("USE_JSZ");
#elif UNITY_ANDROID
        string symbolsDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        return symbolsDefines.Contains("USE_JSZ");
#else
        string symbolsDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        return symbolsDefines.Contains("USE_JSZ");
#endif
    }

    private void ChangDefineSymbols(string channel)
    {
        List<string> defineSymbolsList = new List<string>();

        string defineSymbols = string.Empty;

        string spDefineSymbol = GetChannelSymbol(channel);
        if (!string.IsNullOrEmpty(spDefineSymbol))
        {
            defineSymbolsList.Add(spDefineSymbol);
        }

        if (_enableJSB)
        {
            defineSymbolsList.Add("ENABLE_JSB");
        }

        if (_useJsz)
        {
            defineSymbolsList.Add("USE_JSZ");
        }

        defineSymbols = string.Join(";", defineSymbolsList.ToArray());

#if UNITY_IPHONE
		PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.iOS, defineSymbols);
#elif UNITY_ANDROID
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defineSymbols);
#else
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defineSymbols);
#endif
    }

    private void ChangeProjmods(string channel)
    {
        _buildProjmods = GetChannelProjmods(channel);
        EditorPrefs.SetString("selectProjmods", _buildProjmods);
    }

    private void ChangeIOSTargetOSVersion()
    {
        //		// 仅针对ios渠道
        //		if (_gameSettingData.platformType != GameSetting.PlatformType.ROOTIOS &&
        //		    _gameSettingData.platformType != GameSetting.PlatformType.IOS)
        //		{
        //			return;
        //		}

        var modFolder = Application.dataPath.Replace("/Assets", "/Mods");
        var modName = GetChannelProjmods(_gameSettingData.channel);
        var modPath = string.Format("{0}/{1}", modFolder, modName);
        if (string.IsNullOrEmpty(modName) || !File.Exists(modPath))
        {
            return;
        }

        var mod = new XCMod(modPath);
        PlayerSettings.iOS.targetOSVersion = mod.IOSTargetOSVersion >= iOSTargetOSVersion.iOS_6_0
            ? mod.IOSTargetOSVersion : iOSTargetOSVersion.iOS_6_0;
    }

    private void ChangeProductName(string channel)
    {
        PlayerSettings.companyName = "cilugame";
        string suffix = GetDomainAliasName(_gameSettingData.domainType);
        PlayerSettings.productName = _gameSettingData.gamename + suffix;
    }

    private void ChangeBundleIdentifier(string channel)
    {
        string bundleId = GetChannelBundleId(channel);
        string bundleIdentifier = "";
        //com.cilugame.h1.{0}.xx.{1}
        if (bundleId.Contains("{0}"))
        {
            bundleIdentifier = string.Format(bundleId, _gameSettingData.platformType.ToString().ToLower(), _gameSettingData.domainType.ToLower());
        }
        else
        {
            bundleIdentifier = bundleId;
        }

        //如果是正式包， 则去掉后面的域定义
        bundleIdentifier = bundleIdentifier.Replace("." + "release", "");

        PlayerSettings.applicationIdentifier = bundleIdentifier;
    }

    private void UpdateBuildToFileName(string channel)
    {
        if (_gameSettingData.platformType == GameSetting.PlatformType.Android)
        {
            _buildToFileName = GetBuildTargetDir(channel) + "/" + GetBuildTargetFileName(channel);
        }
        else
        {
            _buildToFileName = "need xcode do it";
        }
    }

    private static string IconAndSplashRoot = "Assets/IconAndSplash";

    private void ChangeIconAndSplash()
    {
        //修改默认ICON
        Texture2D defaultIcon = AssetDatabase.LoadMainAssetAtPath(IconAndSplashRoot + "/Icon/Other/icon.png") as Texture2D;
        if (defaultIcon != null)
        {
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[1] { defaultIcon });
        }
        else
        {
            Debug.Log("Set Defaut ICON Error!!!");
        }

#if UNITY_ANDROID
        String iconUrl = IconAndSplashRoot + "/Icon/Android/{0}x{0}.png";
        Texture2D icon_192 = AssetDatabase.LoadMainAssetAtPath(string.Format(iconUrl, 192)) as Texture2D;
        Texture2D icon_144 = AssetDatabase.LoadMainAssetAtPath(string.Format(iconUrl, 144)) as Texture2D;
        Texture2D icon_96 = AssetDatabase.LoadMainAssetAtPath(string.Format(iconUrl, 96)) as Texture2D;
        Texture2D icon_72 = AssetDatabase.LoadMainAssetAtPath(string.Format(iconUrl, 72)) as Texture2D;
        Texture2D icon_48 = AssetDatabase.LoadMainAssetAtPath(string.Format(iconUrl, 48)) as Texture2D;
        Texture2D icon_36 = AssetDatabase.LoadMainAssetAtPath(string.Format(iconUrl, 36)) as Texture2D;

        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, new Texture2D[6] {
            icon_192,
            icon_144,
            icon_96,
            icon_72,
            icon_48,
            icon_36
        });
#elif UNITY_IPHONE
		//修改平台ICON
		String iconUrl = IconAndSplashRoot + "/Icon/iOS/{0}x{0}.png";
		Texture2D icon_180 = AssetDatabase.LoadMainAssetAtPath (string.Format (iconUrl, 180)) as Texture2D;
		Texture2D icon_152 = AssetDatabase.LoadMainAssetAtPath (string.Format (iconUrl, 152)) as Texture2D;
		Texture2D icon_144 = AssetDatabase.LoadMainAssetAtPath (string.Format (iconUrl, 144)) as Texture2D;
		Texture2D icon_120 = AssetDatabase.LoadMainAssetAtPath (string.Format (iconUrl, 120)) as Texture2D;
		Texture2D icon_114 = AssetDatabase.LoadMainAssetAtPath (string.Format (iconUrl, 114)) as Texture2D;
		Texture2D icon_76 = AssetDatabase.LoadMainAssetAtPath (string.Format (iconUrl, 76)) as Texture2D;
		Texture2D icon_72 = AssetDatabase.LoadMainAssetAtPath (string.Format (iconUrl, 72)) as Texture2D;
		Texture2D icon_57 = AssetDatabase.LoadMainAssetAtPath (string.Format (iconUrl, 57)) as Texture2D;

		PlayerSettings.SetIconsForTargetGroup (BuildTargetGroup.iOS, new Texture2D[8] {
			icon_180,
			icon_152,
			icon_144,
			icon_120,
			icon_114,
			icon_76,
			icon_72,
			icon_57
		});
#endif

        /*
		 * 
    320x480 pixels for 1–3rd gen devices
    1024x768 for iPad mini/iPad 1st/2nd gen
    2048x1536 for iPad 3th/4th gen
    640x960 for 4th gen iPhone / iPod devices
    640x1136 for 5th gen devices
		 */

        //设置开始界面
        //        if (File.Exists(splashPath))
        //        {
        //            File.Delete(splashPath);
        //        }
        //
        //        File.Copy(platformIconPath + splash, splashPath);
    }

    private void ChangBuildSettings()
    {
        EditorBuildSettingsScene[] original = EditorBuildSettings.scenes;
        EditorBuildSettingsScene[] newSettings = new EditorBuildSettingsScene[original.Length];
        System.Array.Copy(original, newSettings, original.Length);

        if (_resLoadMode == AssetPipeline.AssetManager.LoadMode.EditorLocal)
        {
            foreach (EditorBuildSettingsScene scene in newSettings)
            {
                scene.enabled = true;
            }
        }
        else
        {
            //打包资源模式下，禁用掉所有游戏场景，只保留入口场景
            foreach (EditorBuildSettingsScene scene in newSettings)
            {
                if (scene.path == "Assets/Scenes/GameStartScene.unity")
                {
                    scene.enabled = true;
                }
                else
                {
                    scene.enabled = false;
                }
            }
        }
        EditorBuildSettings.scenes = newSettings;
    }

    #region 批量打包渠道

    /// <summary>
    /// 导出Android工程
    /// </summary>
    public static void ExportAndroidProject(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new Exception("不存在路径：" + path);
        }

        var buildOption = BuildOptions.AcceptExternalModificationsToPlayer |
                          BuildOptions.ShowBuiltPlayer;
        BuildPipeline.BuildPlayer(FindEnabledEditorScenes(), path, BuildTarget.Android,
            buildOption);

    }

    private string GetBuildTargetDir(string channel)
    {
        string applicationPath = Application.dataPath.Replace("/Assets", "");
        string target_dir = string.Format(applicationPath + "/APK/{0}/{1}/{2}", _gameSettingData.gameType.ToString(), _gameSettingData.domainType.ToString(), channel);
        return target_dir;
    }

    private string GetBuildTargetFileName(string channel)
    {
        string app_name = _gameSettingData.gameType.ToString() + "_" + PlayerSettings.bundleVersion + "_" + channel + "_" + _gameSettingData.domainType.ToString();

        app_name = app_name.Replace("_Release", "");

        if (!_gameSettingData.release)
        {
            app_name += "_debug";
        }

        if (developmentBuild)
        {
            app_name += "_dev";
        }

        if (_enableJSB)
        {
            app_name += "_JSB";
        }

        if (_minResBuild)
        {
            app_name += "_min";
        }

        return app_name + ".apk";
    }

    //这里封装了一个简单的通用方法。
    private void BulidTargetApk(string channel, bool isExportDll)
    {
        BuildTarget buildTarget = BuildTarget.Android;

        string target_dir = GetBuildTargetDir(channel);
        string target_name = GetBuildTargetFileName(channel);

        Debug.Log(target_name);

        ShowNotification(new GUIContent("正在打包:" + target_name));

        //每次build删除之前的残留
        if (Directory.Exists(target_dir))
        {
            if (File.Exists(target_name))
            {
                File.Delete(target_name);
            }
        }
        else
        {
            Directory.CreateDirectory(target_dir);
        }

        string[] scenes = FindEnabledEditorScenes();

        //开始Build场景，等待吧～
        BuildOptions buildOption = BuildOptions.ShowBuiltPlayer;
        if (developmentBuild)
        {
            buildOption = BuildOptions.ShowBuiltPlayer | BuildOptions.Development | BuildOptions.ConnectWithProfiler;
        }

        GenericBuild(scenes, target_dir + "/" + target_name, buildTarget, buildOption);
        if (isExportDll)
        {
            BuildPipelineEx.GenerateAndroidDll(target_dir + "/" + target_name,_gameSettingData.domainType);
        }
    }

    private string GetBuildDateTime()
    {
        DateTime dateTtime = DateTime.UtcNow.ToLocalTime();
        string str = string.Format("{0:D2}{1:D2}{2:D2}_{3:D2}{4:D2}", dateTtime.Year, dateTtime.Month, dateTtime.Day, dateTtime.Hour, dateTtime.Minute);
        return str;
    }

    public static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled)
                continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    private void GenericBuild(string[] scenes, string targetPath, BuildTarget build_target, BuildOptions build_options)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
        string error = BuildPipeline.BuildPlayer(scenes, targetPath, build_target, build_options);

        if (error.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + error);
        }
    }


    private void BuildAllIpa()
    {
        if (EditorUtility.DisplayDialog("打包全部Ipa", "先保存到任意一个渠道环境！\n你准备好卡死了么!", "确定", "取消"))
        {
            string applicationPath = Application.dataPath.Replace("/Assets", "/../..");
            string target_dir = EditorUtility.OpenFolderPanel("导出目录", applicationPath, "xcode");
            if (string.IsNullOrEmpty(target_dir))
            {
                return;
            }

            foreach (var channel in _channelValues)
            {
                _gameSettingData.channel = channel;
                SaveComeFromConfig();

                string target_name = string.Format("H1_{0}_{1}_{2}", AppGameVersion.ShortBundleVersion, _gameSettingData.channel, _gameSettingData.domainType.ToString());

                if (!Directory.Exists(target_dir))
                {
                    Directory.CreateDirectory(target_dir);
                }

                string fullPath = target_dir + "/" + target_name;

                Debug.Log(fullPath);

                string res = BuildPipeline.BuildPlayer(FindEnabledEditorScenes(), fullPath, BuildTarget.iOS, BuildOptions.ShowBuiltPlayer);
                if (res.Length > 0)
                {
                    throw new Exception("BuildPlayer failure: " + res);
                }
                //				break;
            }
        }
    }

    #endregion
}

//打包域信息
public class GameInfo
{
    //游戏ID
    //public string gametype;
    //游戏名
    public string gamename;
    //配置标示识别
    public string configSuffix;
    //打包域信息
    public List<DomainInfo> domains;
}

//打包域信息
public class DomainInfo
{
    //域名字,打包放置目录也是它
    public string type;
    //根地址
    public string url;
    //资源加载啊目录，结合CDN路径使用
    public string resdir;
    //打包识别
    public string bundleId;
}