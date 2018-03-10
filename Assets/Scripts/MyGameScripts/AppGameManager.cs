using System;
using AppDto;
using DG.Tweening;
//using GamePlot;
using UnityEngine;

/// <summary>
///     经由游戏资源与代码更新后启动,主要处理游戏业务系统相关初始化工作,以及登录流程处理
/// </summary>
public class AppGameManager : MonoBehaviour
{
	public static AppGameManager Instance { get; private set; }

    public static event Action InitHook;

    private void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	private void Start()
	{
		Setup();
	}

	private void Setup()
	{
		JsHelper.SetupJsProto();
        //----------SPSdkManager初始化----------
        SPSdkManager.Instance.Setup();
        SPSdkManager.LoadSPChannelConfig(OnSPChanelConfigLoadFinish);
	}

	//加载完毕渠道配置
	private void OnSPChanelConfigLoadFinish(bool success)
	{
		if (!success)
		{
			Debug.LogError("渠道配置加载失败，请检查资源");
			GameLauncher.ShowTips("加载渠道配置失败，请检查资源");
			return;
		}

		InitChannelInfo();

        TestinAgentHelper.Init(GameConfig.TESTIN_APPKEY, GameSetting.Channel);
		//----------SPSdkManager初始化----------

		gameObject.GetMissingComponent<LayerManager>();
		Shader.WarmupAllShaders();
		Resources.UnloadUnusedAssets();
		AssetPipeline.ResourcePoolManager.Instance.Setup();

		//var go = new GameObject("_GameDebuger");
		//DontDestroyOnLoad(go);
		//go.AddComponent<GameDebuger>();

		var go = new GameObject("_ExitGameHandler");
		DontDestroyOnLoad(go);
		go.AddComponent<ExitGameScript>();

		go = new GameObject("_SystemTimeManager");
		DontDestroyOnLoad(go);
		go.AddComponent<SystemTimeManager>();

		SdkMessageManager.Instance.Setup();

		XinGeSdk.Setup();
		XinGeSdk.Register();

		DataManager.Instance.Setup();
		NotifyListenerRegister.AddLoginQueueNotifyListener();

        GameDebuger.TODO(" SystemSetting.Setup();");
		AudioManager.Instance.Setup();
		LoadingTipManager.Setup();
        RequestLoadingTip.Setup();
		TipManager.Setup();
        BattleConfigManager.Instance.Setup();
        ModelHelper.Setup();
		VoiceRecognitionManager.Instance.Setup();
		//		PayManager.Instance.Setup();

        ExpressionManager.Setup();
		DOTween.Init();

		// 为了保证没有bug，索性不初始化代码
		#if (UNITY_STANDALONE_WIN || UNITY_EDITOR) && !ENABLE_JSB
		InputManager.Instance.Setup();
		#endif

		GameLauncher.Instance.DestroyGameLoader(LayerManager.Root.UIModuleRoot);

		IconConfigManager.Setup(() =>
			{
				//检查是否需要播放CG -> 播放CG -> 检查是否需要播放剧情 -> 载入剧情场景 -> 播放剧情 -> 初始化SDK -> 打开登录界面
				if (GameSetting.TestServerMode)
				{
					OnLoadServerListFinish();
				}
				else
				{
					LoadStaticServerList();
				}
			});
	}

    private void InitChannelInfo()
    {
        GameSetting.Channel = SPSdkManager.Instance.GetChannel();
        GameSetting.SubChannel = SPSdkManager.Instance.GetChannel();
        GameSetting.LoginWay = GameSetting.Channel;
    }

    /// <summary>
    /// 本应用于重新设置，由于jsb关系不改名
    /// </summary>
	public void SetupChannel()
	{
        InitChannelInfo();
    }

	/// <summary>
	///     加载静态服务器列表信息
	/// </summary>
	private void LoadStaticServerList()
	{
		TalkingDataHelper.OnEventSetp("AppGameManager/LoadStaticServerList");
		GameLauncher.ShowTips("加载静态服务器列表...");
		GameServerInfoManager.Setup(LoadDynamicServerList,
			errorMsg => { OpenRetryWindow(errorMsg, LoadStaticServerList); });
	}

	/// <summary>
	///     加载动态服务器列表信息
	/// </summary>
	private void LoadDynamicServerList()
	{
		TalkingDataHelper.OnEventSetp("AppGameManager/LoadDynamicServerList");
        SPSDK.gameEvent("10009");   //请求服务器列表
		GameLauncher.ShowTips("加载动态服务器列表...");
		GameServerInfoManager.RequestDynamicServerList(AppGameVersion.SpVersionCode, GameSetting.Channel,
			GameSetting.PlatformTypeId, OnLoadServerListFinish,
			() => { OpenRetryWindow("加载服务器列表失败, 请重新进入游戏", LoadDynamicServerList); });
	}

	private void OpenRetryWindow(string errorMsg, Action retryAction)
	{
		GameLauncher.ShowTips(errorMsg);
		ProxyWindowModule.OpenSimpleMessageWindow(errorMsg, retryAction, UIWidget.Pivot.Left, null,
			UILayerType.TopDialogue);
	}

	#region 进入登录界面或新号剧情

	private void OnLoadServerListFinish()
	{
		CheckGameMov();
	}

	private void CheckGameMov()
	{
		if (AppGameVersion.EnableStartMovieMode && !PlayerPrefsExt.GetBool("GameStartCG"))
		{
			GameLauncher.ShowTips("");
			#if UNITY_STANDALONE && UNITY_EDITOR
            GameDebuger.TODO(" CGPlayer.PlayCG('Assets/GameResources/ArtResources/' + PathHelper.CG_Asset_PATH, OnPlayCGFinish);");
            OnPlayCGFinish();
			#else
			CGPlayer.PlayCG(PathHelper.CG_Asset_PATH, OnPlayCGFinish);
			#endif
		}
		else
		{
			OnPlayCGFinish();
		}
	}

	private void OnPlayCGFinish()
	{
		PlayerPrefsExt.SetBool("GameStartCG", true);
		if (AppGameVersion.EnableStartPlotMode && !PlayerPrefsExt.GetBool("PassRoleCreatePlot"))
		{
			//重要！！！这里先检查包内的数据是否正常， 如果不正常， 则取消新号剧情
			//导致这个问题的原因是新旧版本的数据线协议兼容性问题			

            GameDebuger.TODO(@"var plot = DataCache.getDtoByCls<Plot>(1);
			if (plot != null)
			{
				ReadyPlotMapRes();
			}
			else ");
			{
				InitSPSdk();
			}
		}
		else
		{
			InitSPSdk();
		}
	}

	private void ReadyPlotMapRes()
	{
		TalkingDataHelper.OnEventSetp("GameStartPlot/ReadyPlotMapRes"); //载入剧情场景
		GameLauncher.ShowTips("载入场景资源...");
		int sceneId = 3007;


		WorldMapLoader.Instance.LoadWorldMap(sceneId, () =>
			{
				GameLauncher.ShowTips("");
				PlayGamePlot();
			});

        GameLauncher.ShowTips("");
        PlayGamePlot();
	}

	private void PlayGamePlot()
	{
        ScreenMaskManager.FadeOut(delegate 
			{
				GameLauncher.CloseLauncherView();

				Screen.sleepTimeout = SleepTimeout.NeverSleep;

            GameDebuger.TODO(@"var plot = DataCache.getDtoByCls<Plot>(1);
				if (plot != null)
				{
					var playerDto = new PlayerDto();
					playerDto.id = 1001;
                    playerDto.nickname = '明月';
					playerDto.grade = 1;
					playerDto.factionId = 5;
					playerDto.charactorId = 4;

					ModelManager.Player.SetupTempPlayerDto(playerDto);

					GamePlotManager.Instance.PlayPlot(plot);
				}
                else ");
				{
					InitSPSdk();
				}
			});
	}

	public void InitSPSdk()
	{
		TalkingDataHelper.OnEventSetp("GameAccountLogin/InitSPSdk"); //初始化SDK
		GameLauncher.ShowTips("初始化SDK...");
		SPSdkManager.Instance.Init(OnInitSuccess);
	}

	private void OnInitSuccess(bool success)
	{
		if (success)
		{
			GameLauncher.CloseLauncherView();

            if (InitHook == null)
            {
                ProxyLoginModule.Open();
            }
            else
            {
                InitHook();
            }

        }
        else
		{
			OpenRetryWindow("初始化SDK失败", LoadDynamicServerList);
		}
	}

	#endregion
}