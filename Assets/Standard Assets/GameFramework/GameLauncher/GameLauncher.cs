// **********************************************************************
// Copyright (c) 2016 cilugame. All rights reserved.
// File     : GameLauncher.cs
// Author   : senkay <senkay@126.com>
// Created  : 2/22/2016  
// Porpuse  : 
// **********************************************************************
//
using System;
using LITJson;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using AssetPipeline;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class GameLauncher : MonoBehaviour
{
    private static GameLauncher _instance;
    private int getHostCount = 1;

    public static GameLauncher Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        SPSDK.gameEvent("10001");   //启动游戏
        ResolutionCheck();

        InitCYGameUpdateSetting(() =>
        {

            InitGameUpdateSetting();
            SPSDK.UnityInitFinish();
        });
    }

    /// <summary>
    /// 初始化畅游SDK并获取畅游网关数据
    /// </summary>
    /// <param name="onfinish"></param>
    private void InitCYGameUpdateSetting(Action onfinish)
    {

        SdkMessageScript sdkMessageScript = SdkMessageScript.Setup();
        sdkMessageScript.OnCYSdkCallbackInfo = (json) =>
        {
            JsonData jsonData = JsonMapper.ToObject(json);
            int state_code = (int)jsonData["state_code"];
            string message = (string)jsonData["message"];
            JsonData data = jsonData["data"];
            switch (state_code)
            {
                case CySdk.ResultCode.INIT_SUCCESS:
                    GetHost();
                    break;
                case CySdk.ResultCode.INIT_FAILED:
                    break;
                case CySdk.ResultCode.HOST_SUCCESS:
                    //获取网关信息，更新配置表
                    UpdateGameSettingData(data, onfinish);
                    break;
                case CySdk.ResultCode.HOST_FAILED:
                    //获取网关信息失败，重新拉取
                    //TODO 多次失败应该退出游戏
                    GetHostFAILED();
                    break;
            }
        };
        SPSDK.Init();
#if UNITY_EDITOR
        string result = "{\"state_code\":1101,\"message\":\"获取网关成功\",\"data\":{}}";
        sdkMessageScript.onResult(result);
#elif UNITY_IPHONE
#elif UNITY_ANDROID
        //Android 初始化已经在热更dll时完成，而ios不是，所以这里直接返回成功信息
        string result = "{\"state_code\":400,\"message\":\"初始化成功\",\"data\":{}}";
        sdkMessageScript.onResult(result);
#else
        // string dataCont = "{\"host\": \"127.0.0.1\",\"port\": 8080,\"desp\": \"\",\"status\": -1,\"resource_url\": \"http://www.baidu.com\",\"serverlist_url\": \"http://cdn.com\"}";
        string result = "{\"state_code\":1101,\"message\":\"获取网关成功\",\"data\":{}}";
        sdkMessageScript.onResult(result);
#endif


    }

    /// <summary>
    /// 通过畅游网关数据加载必要的配置资源文件
    /// </summary>
    /// <param name="data">网关数据</param>
    /// <param name="onfinish"></param>
    private void UpdateGameSettingData(JsonData data, Action onfinish)
    {
        //TODO 解析网关数据，获取并下载对应的配置资源文件
        SdkMessageScript sdkMessageScript = SdkMessageScript.Setup();
#if !UNITY_EDITOR
        int status = (int)data["status"];
        string desp = (string)data["desp"];
        if (status == -1)
        {
            BuiltInDialogueViewController.OpenView(desp, GetHost, ExitGame, UIWidget.Pivot.Left, "重试", "退出");
        }
        else
        {

            try
            {
                //通过网关获取host域名
                GameSetting.PlatformHttpRoot = null;
                GameSetting.PlatformHttpRoot = String.Format("{0}/{1}/{2}", (string)data["host"],
                GameSetting.ResDir,
                GameSetting.PlatformTypeName);
            }
            catch (Exception ex)
            {
                Debug.LogError("加载<PlatformHttpRoot>失败:message="+ data.ToString());
            }
            sdkMessageScript.OnCYSdkCallbackInfo = null;
            onfinish();
        }
#else
        if (onfinish != null)
        {
            sdkMessageScript.OnCYSdkCallbackInfo = null;
            onfinish();
        }
#endif
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BuiltInDialogueViewController.OpenView("退出游戏", ExitGame, () =>
            {
                Debug.Log("Cancel ExitGame");
            });
        }
    }


    /// <summary>
    /// 防止意外界面大小不正常
    /// </summary>
    private void ResolutionCheck()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            // 临时定死值
            if (Screen.width < 500 || Screen.height < 300)
            {
                Screen.SetResolution(800, 450, false);
            }
        }
    }


    /// <summary>
    ///     初始化游戏更新环境
    /// </summary>
    private void InitGameUpdateSetting()
    {
        //游戏帧率限制
        Application.targetFrameRate = 30;
        //游戏设置为不自动休眠屏幕
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        GameSetting.Setup();

#if UNITY_STANDALONE && !UNITY_EDITOR
		var limitedNum = 2;
		if (GameLauncherMutex.GetMutexTag(GameSetting.GameName) > limitedNum)
		{
			ShowErroMessage("同时最多只能开启两个游戏客户端");
			return;
		}
#endif

        //游戏系统设置(是否开启声音、3d镜头、语音大小......)
        BaoyugameSdk.Setup();
        TalkingDataHelper.Setup();
        HttpController.Instance.Setup();
        //Tencent Bugly setup
        BuglyAgent.InitWithAppId("c180163ca5");

        TalkingDataHelper.OnEventSetp("GameLauncher/InitGameUpdateSetting");
        SPSDK.gameEvent("10002");   //初始化游戏更新环境
        //GameLauncherViewController.OpenView("引擎版本: " + FrameworkVersion.ShowVersion);
        GameLauncherViewController.OpenView("");
        ShowTips("初始化游戏...");

        LoadStaticVersion();
    }

    //显示提示
    public static void ShowTips(string tips)
    {
        GameLauncherViewController.ShowTips(tips);
    }

    public void DestroyGameLoader(GameObject gameUIRoot)
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
        GameLauncherViewController.ChangeParentLayer(gameUIRoot);
    }

    public static void CloseLauncherView()
    {
        GameLauncherViewController.CloseView();
    }

    #region 异常处理
    //获取网关异常
    private void GetHostFAILED()
    {
#if !UNITY_EDITOR
        BuiltInDialogueViewController.OpenView("重新获取", GetHost, ExitGame, UIWidget.Pivot.Left, "重试", "退出");
#endif
    }

    private void OnLoadStaticConfigError(string tips)
    {
        BuiltInDialogueViewController.OpenView(tips, LoadStaticVersion, ExitGame, UIWidget.Pivot.Left, "重试", "退出");
    }

    private void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    private void ShowErroMessage(string msg)
    {
        if (string.IsNullOrEmpty(msg))
        {
            ExitGame();
        }
        else
        {
            BuiltInDialogueViewController.OpenView(msg,
                ExitGame, null, UIWidget.Pivot.Center);
        }
    }

    #endregion

    //加载静态文件版本

    #region 加载静态服务器列表

    private void LoadStaticVersion()
    {
        TalkingDataHelper.OnEventSetp("GameLauncher/LoadStaticVersion"); //加载配置版本
        ShowTips("加载游戏配置...");
        GameStaticConfigManager.Instance.Setup(LoadServerUrlConfig, OnLoadStaticConfigError);
    }

    /// <summary>
    ///     载入静态服务器Url地址信息
    /// </summary>
    private void LoadServerUrlConfig()
    {
        //压测模式下直接加载本地服务器配置信息
        if (GameSetting.TestServerMode)
        {
            TextAsset textAsset = Resources.Load("Setting/LocalServerConfig") as TextAsset;
            if (textAsset != null)
            {
                ServerUrlConfig config = JsonMapper.ToObject<ServerUrlConfig>(textAsset.text);
                GameSetting.SetupServerUrlConfig(config);
                CheckLocalAssets();
            }
            else
            {
                Debug.LogError("加载<LocalServerConfig>失败");
            }
        }
        else
        {
            TalkingDataHelper.OnEventSetp("GameLauncher/LoadServerUrlConfig"); //加载静态服务器列表
            ShowTips("加载服务器配置信息...");
            GameStaticConfigManager.Instance.LoadStaticConfig(GameStaticConfigManager.Type_StaticServerList,
                json =>
                {
                    ServerUrlConfig config = JsonMapper.ToObject<ServerUrlConfig>(json);
                    if (config != null)
                    {
                        GameSetting.SetupServerUrlConfig(config);
                        CheckLocalAssets();
                    }
                    else
                    {
                        ShowTips("解析服务器配置信息失败");
                    }
                }, OnLoadStaticConfigError);
        }
    }

    #endregion

    #region 资源更新

    /// <summary>
    ///     检查本地资源，把部分资源从StreamingAssets copy 到 persistentDataPath
    /// </summary>
    private void CheckLocalAssets()
    {
        TalkingDataHelper.OnEventSetp("GameLauncher/CheckLocalAssets"); //检查本地资源
        AssetManager.Instance.Setup(GameSetting.PlatformResPathList, ShowTips, needRestart =>
        {
            if (needRestart)
            {
                RestartGame();
            }
            else
            {
                CheckUpdateAssets();
            }
        }, ShowErroMessage);
    }

    /// <summary>
    ///     检查是否有网络资源可以更新
    /// </summary>
    private void CheckUpdateAssets()
    {
        Debug.Log("CheckUpdateAssets");

        if (Application.isMobilePlatform)
        {
            //判断是否有网络
            JudgeNetExist();
        }
        else
        {
            StartUpdateAsset();
        }
    }

    /// <summary>
    ///     判断网络的存在
    /// </summary>
    private void JudgeNetExist()
    {
        string curNetType = BaoyugameSdk.getNetworkType();

        //判断是否没有网络
        if (curNetType == BaoyugameSdk.NET_STATE_NONE)
        {
            BuiltInDialogueViewController.OpenView("当前不存在网络连接，请连接上网络",
                JudgeNetExist, null, UIWidget.Pivot.Left, "重新连接");
        }
        else
        {
            //游戏入口
            StartUpdateAsset();
        }
    }

    /// <summary>
    ///     开始更新游戏资源
    /// </summary>
    private void StartUpdateAsset()
    {
        TalkingDataHelper.OnEventSetp("GameLauncher/FetchVersionConfig"); //获取外部版本配置
                                                                          //先获取VersionConfig文件，再开始更新资源
        AssetManager.Instance.FetchVersionConfig(VersionConfig.ServerType.Null, () =>
        {
            TalkingDataHelper.OnEventSetp("GameLauncher/UpdateRemoteResource"); //检查外部资源
            SPSDK.gameEvent("10003");       //检查更新
            CheckClientUpdate();
        });
    }

    /// <summary>
    /// 检查客户端整包更新
    /// </summary>
    private void CheckClientUpdate()
    {
        if (Application.isEditor)
        {
            UpdateDll();
            return;
        }

        Debug.Log("CheckClientUpdate");

        var versionConfig = AssetManager.Instance.CurVersionConfig;
        if (versionConfig == null)
        {
            BuiltInDialogueViewController.OpenView("获取版本信息失败", StartUpdateAsset);
            SPSDK.gameEvent("10004");   //获取版本信息失败
            return;
        }

        //		versionConfig = new VersionConfig();
        //
        //		versionConfig.ios = new VersionInfo();
        //		versionConfig.ios.version = 34904;
        //		versionConfig.ios.force = false;
        //		versionConfig.ios.url = "https://t2.cilugame.com:4433/h1/download/betatest/index.html";

        Debug.Log(string.Format("RemoteVersion={0} LocalVersion={1}", versionConfig.frameworkVer,
            FrameworkVersion.ver));
        if (versionConfig.frameworkVer > FrameworkVersion.ver)
        {
            if (versionConfig.forceUpdate)
            {
                if (string.IsNullOrEmpty(versionConfig.helpUrl))
                {
                    BuiltInDialogueViewController.OpenView("游戏提供了新版本，要更新才能进入游戏", CheckClientUpdate);
                }
                else
                {
                    BuiltInDialogueViewController.OpenView("游戏提供了新版本，需要更新才可进入游戏", () =>
                    {
                        Application.OpenURL(versionConfig.helpUrl);
                        CheckClientUpdate();
                    });
                }
            }
            else
            {
                BuiltInDialogueViewController.OpenView("游戏提供了新版本，现在就要更新吗？", () =>
                {
                    Application.OpenURL(versionConfig.helpUrl);
                    CheckClientUpdate();
                }, UpdateDll, UIWidget.Pivot.Left, "现在更新", "下次更新");
            }
        }
        else
        {
            UpdateDll();
        }
    }

    /// <summary>
    ///     检查更新Dll文件
    /// </summary>
    private void UpdateDll()
    {
        if (AssetManager.Instance.ValidateDllVersion(RestartGame))
        {
            UpdateGameRes();
        }
    }

    /// <summary>
    /// Dll更新完毕提示重启游戏
    /// </summary>
    private void RestartGame()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            BuiltInDialogueViewController.OpenView("检查到更新，点击确认后片刻重新启动", DllHelper.RestartGame);
        }
        else
        {
            DllHelper.RestartGame();
        }
    }

    /// <summary>
    /// 检查更新游戏资源
    /// </summary>
    private void UpdateGameRes()
    {
        if (AssetManager.Instance.ValidateGameResVersion(JudgePhoneMemory))
        {
            JudgePhoneMemory();
        }
    }

    //资源更新完毕，检测手机内存状态
    private void JudgePhoneMemory()
    {
        Debug.Log("JudgePhoneMemory");
        if (Application.isMobilePlatform)
        {
            long freeMemory = BaoyugameSdk.getFreeMemory() / 1024;
            Debug.Log("Free Memory : " + freeMemory);

#if UNITY_ANDROID
            if (freeMemory < 150L)
            {
                BuiltInDialogueViewController.OpenView("您的手机剩余内存剩余较低，运行游戏可能会出现闪退情况，建议先清理内存",
                    ExitGame, OnUpdateAssetFinish,
                    UIWidget.Pivot.Left, "退出", "继续");
                return;
            }
#endif
        }

        OnUpdateAssetFinish();
    }

    /// <summary>
    ///     资源更新完成
    /// </summary>
    private void OnUpdateAssetFinish()
    {
        Debug.Log("OnUpdateAssetFinish");
        TalkingDataHelper.OnEventSetp("GameLauncher/LoadCommonAsset"); //加载公共资源
        AssetManager.Instance.LoadCommonAsset(OnLoadCommonAssetFinish);
    }

    /// <summary>
    ///     加载公共资源完毕,加载GameRoot,启动AppGameManager
    /// </summary>
    private void OnLoadCommonAssetFinish()
    {
        Debug.Log("Load CommonAsset Finish");
#if ENABLE_JSB
        //先加载JSEngine
        GameObject jsEngine =
            Instantiate(AssetManager.Instance.LoadAsset("JSEngine", ResGroup.UIPrefab)) as GameObject;
        jsEngine.name = "JSEngine";
        AssetManager.Instance.UnloadBundle("JSEngine", ResGroup.UIPrefab);
#endif
        GameObject gameRoot =
            Instantiate(AssetManager.Instance.LoadAsset<GameObject>("GameRoot", ResGroup.UIPrefab));
        AssetManager.Instance.UnloadBundle("GameRoot", ResGroup.UIPrefab);
        gameRoot.name = "GameRoot";
        DontDestroyOnLoad(gameRoot);
#if ENABLE_JSB
        var jsCom = gameRoot.AddComponent<JSComponent>();
        jsCom.jsClassName = "AppGameManager";
        jsCom.Setup(); // 要调用 js 的 Awake
#else
        var appGameType =
            Type.GetType("AppGameManager, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        gameRoot.AddComponent(appGameType);
#endif
    }

    #endregion


    private void GetHost()
    {
#if !UNITY_EDITOR
        //后面用于重试多次退出
        getHostCount++;
#endif
        SPSDK.getHost();
    }
}
