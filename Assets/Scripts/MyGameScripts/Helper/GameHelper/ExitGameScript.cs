// **********************************************************************
// Copyright  2013 Baoyugame. All rights reserved.
// File     :  ExitGameScript.cs
// Author   : senkay
// Created  : 6/26/2013 9:29:59 AM
// Purpose  : 检查是否按了退出按钮
// **********************************************************************

using System;
using UnityEngine;
using AppServices;
//using GamePlot;
using AssetPipeline;
using AppDto;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExitGameScript : MonoBehaviour
{

    private static ExitGameScript _instance = null;

    public static ExitGameScript Instance
    {
        get
        {
            return _instance;
        }
    }

    private bool isClick;

    static public bool CheckConnected = false;
    static public bool NeedReturnToLogin = false;

    static public bool WaitForReConnect = false;

    private int ReConnectTryCount = 0;

    private int ReConnectMaxCount = 2;

	private int ReConnectTipMaxCount = 5;

    public Action LogOutNotify;

    void Awake()
    {
        _instance = this;

        if (GameSetting.Release)
        {
            ReConnectMaxCount = 2;
        }
        else
        {
            ReConnectMaxCount = 1;
        }

        UICamera.onKey += OnPressKey;

        if (!Application.isMobilePlatform)
        {
            UICamera.onScreenResize += OnScreenResize;
        }
    }

    void OnDestroy()
    {
        UICamera.onKey -= OnPressKey;

        if (!Application.isMobilePlatform)
        {
            UICamera.onScreenResize -= OnScreenResize;
        }
    }

	private void OnPressKey(GameObject go, KeyCode key)
    {
        if (key == KeyCode.Escape)
        {
			if (!Application.isEditor && !GameSetting.IsOriginWinPlatform || GameDebuger.DebugForExit)
            {
                OpenExitDialogue();
            }

            //BattleDataManager.DataMgr.ExitBattle();

            DumpPlayerInfo();
        }
    }

	private void OnScreenResize()
	{
//		LayerManager.Root.SceneHUDCamera.Render();
		LayerManager.Root.SceneHUDCamera.ResetAspect();
	}

	public void DumpPlayerInfo()
    {
        //Debug Info
        if (GameSetting.Release) return;

        PlayerDto playerDto = ModelManager.Player.GetPlayer();
        if (playerDto == null) return;
        string info = " 账号:" + LoginManager.Instance.LoginId;
        info += " ID:" + playerDto.id;
        info += " 昵称:" + playerDto.name;
        //info += " token:" + LoginManager.DataMgr.GetPlayerID();

        GameServerInfo serverInfo =
            GameServerInfoManager.GetServerInfoByName(PlayerPrefs.GetString(GameSetting.LastServerPrefsName));
        if (serverInfo != null)
        {
            info += " 服务器:" + serverInfo.name;
        }

        info += " 时间:" + DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss");

        GameDebuger.TODO(@"
        SceneDto sceneDto = WorldManager.DataMgr.GetModel().GetSceneDto();
        if (sceneDto != null)
        {
            info += ' 当前场景:' + sceneDto.name;
        }

        info += BattleDataManager.DataMgr.GetBattleInfo();
            ");

        FileHelper.ClipBoard = info;

        GameDebuger.Log(info);

        TipManager.AddTip(info);
        //Debug Info
    }

    public void OpenExitDialogue()
    {
        if (isClick == false)
        {
            isClick = true;

            GameDebuger.Log("InputKey is Escape");
            if (GameDebuger.DebugForLogout)
            {
                isClick = false;
                SPSdkManager.Instance.CallbackLogout(true);
            }
            else if (GameDebuger.DebugForDisconnect)
            {
                isClick = false;
                SocketManager.Instance.Close(false);
            }
            else
            {
                DoExiter();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckConnected && !NeedReturnToLogin)
        {
            if (reConnecting)
            {
                reConnecting = false;
                //RequestLoadingTip.Reset();
            }

            if (SocketManager.IsOnLink == false)
            {
				LayerManager.Instance.LockUICamera(false);

                RequestLoadingTip.Reset();

                ProxyWorldMapModule.CloseMiniMap();

                if (LoginManager.LeaveState == EventObject.Leave_status_duplicate)
                {
                    // 苹果PC端特殊处理
                    if (!GameSetting.IsOriginWinPlatform)
                    {
                        ServerManager.Instance.loginAccountDto = null;
                    }

                    OpenServerCloseTip("你的角色已从其他客户端登录，如非本人操作，请注意账号安全！");
                }
                else if (LoginManager.LeaveState == EventObject.Leave_status_kickout)
                {
                    OpenServerCloseTip("网络中断, 请重新进入游戏");
                }
                else if (LoginManager.LeaveState == EventObject.Leave_status_destroy)
                {
                    OpenServerCloseTip("服务器维护，请重新进入游戏");
                }
                else if (LoginManager.LeaveState == EventObject.Leave_status_logout)
                {
                    OpenServerCloseTip("网络中断, 请重新进入游戏");
                }
                else if (LoginManager.LeaveState == EventObject.Leave_status_disconnect)
                {
                    OpenReloginTip();
                }
                else if (LoginManager.LeaveState == EventObject.Leave_status_unkonwn)
                {
                    OpenReloginTip();
                }
                else
                {
                    OpenReloginTip();
                }

                GameDebuger.TODO("PlayerGameState.Save();");
                GameDebuger.TODO(" ModelManager.Player.StopAutoRun();");

                CheckConnected = false;
            }
            else
            {
                ReConnectTryCount = 0;
            }
        }
    }

    void OnApplicationPause(bool paused)
    {
        GameDebuger.Log("OnApplicationPause " + paused);

		if (!paused)
		{
			TalkingDataHelper.Setup();
            GameDebuger.TODO("ModelManager.SystemData.ResetIdleCheck();");
            GameDebuger.TODO("BattleDataManager.DataMgr.CheckResumeBattle();");

			CancelInvoke("ResetClickFlag");
			Invoke("ResetClickFlag",0.5f);
		}
		else
		{
			CancelInvoke("ResetClickFlag");
			TalkingDataHelper.Dispose();
		}
    }

	private void ResetClickFlag()
	{
	    if (!isClick) return;
	    GameDebuger.Log("Check isClick and set false");
	    isClick = false;
	}

    void OnApplicationQuit()
    {
        TalkingDataHelper.Dispose();

        GameDebuger.Log("OnApplicationQuit");

        if (BattleDataManager.DataMgr.IsInBattle)
        {
            GameDebuger.LogError("[DEMO/非错误]为避免再次登录本角色时，服务端下发退出游戏前的战斗数据，干扰游戏。前端在退出游戏时发送退出战斗的协议。正式时登录游戏服务端应当下发之前的战斗Video。");
            BattleDataManager.DataMgr.ExitBattle();
        }

        if (SocketManager.IsOnLink)
        {
            LoginManager.Instance.RemoveListener();
            GameDebuger.TODO(" ServiceRequestAction.requestServer(PlayerService.logout());");
        }
        SocketManager.Instance.Close(false);

        GameDebuger.Log("Exit Game!!!");

        DisposeOnApplicationQuit();
        
        FixedAnimatorDeactiveBug();

        GameDebuger.Log("Exit Game Success");
    }

    /// <summary>
    /// 修复Unity当PC关闭之后，Animator关闭激活状态时候的内存释放问题
    /// </summary>
    private void FixedAnimatorDeactiveBug()
    {
#if !UNITY_STANDALONE
        return;
#endif

        var objs = FindObjectsOfType<Animator>();
        if (objs != null)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                // 关脚本还不行，一定得将gameobject的激活关掉
                //                objs[i].enabled = false;
                objs[i].gameObject.SetActive(false);
            }
        }
    }

    //	void OnApplicationFocus(bool isFocus)
    //	{
    //		GameDebuger.Log("OnApplicationFocus " + isFocus);
    //	}

    //重新连接销毁处理
    void DisposeOnReconnect()
    {
        GameDebuger.Log("DisposeOnReconnect");
        SavePlayerData();
        DisposeModuleData();
    }

    //重新登陆销毁处理
    void DisposeOnReLogin()
    {
        GameDebuger.Log("DisposeOnReLogin");

        DisposeSceneData();
        //	模块数据先处理
        DisposeModuleData();
		//聊天模块改为重登时候才销毁
        GameDebuger.TODO("ModelManager.Chat.Clear();");
        GameDebuger.TODO("PlayerGameState.Reset();");
        SavePlayerData();

        //清空游戏内的计时器
        JSTimer.Instance.Dispose();
        UIModulePool.Instance.SetupTimer();

        GameDebuger.TODO("ProxyMainUI.Hide();");
        ProxyRoleCreateModule.Close();

        if (SocketManager.IsOnLink)
        {
            GameDebuger.TODO("ServiceRequestAction.requestServer(PlayerService.logout());");
        }

        LoginManager.Instance.RemoveListener();
        SocketManager.Instance.Close(false);
		DataManager.Reset();
    }

    //应用退出的销毁处理
    void DisposeOnApplicationQuit()
    {
        GameDebuger.Log("DisposeOnApplicationQuit");

        SavePlayerData();

        JSTimer.Instance.Dispose();
        CSTimer.Instance.Dispose();
        GameDebuger.TODO("ModelManager.SystemData.Dispose();");
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        //VoiceRecognitionManager.DataMgr.DelTalkCache();
        VoiceRecognitionManager.Instance.CleanupVoiceCache();
        BaoyugameSdk.UnregisterPower();
        //BaiduASRSdk.Destroy();
        //BaoyugameSdk.UnregisterGsmSignalStrength();
    }

    //保存用户数据
    void SavePlayerData()
    {
        GameDebuger.Log("SavePlayerData");

        GameDebuger.TODO(@"ModelManager.Friend.Dispose();   //好友数据的保存
        ModelManager.Chat.SaveChatRecord();//聊天-表情-记录 
        ModelManager.Barrage.SaveRecord();//保存弹幕发送的记录
        if (NewBieGuideManager.HasInstance)
        {
            NewBieGuideManager.DataMgr.Dispose(); //新手引导数据保存
        }
            ModelManager.DailyPush.Dispose(); //日程数据保存，主要是弹窗的");

        //本地保存 好友聊天记录及相关数据到xml
        GameDebuger.Log("好友：SaveChatRecord");
        PrivateMsgDataMgr.SaveChatRecord();

        PlayerPrefs.Save();
        GameDataManager.Instance.SaveData(); //游戏数据保存
        //GameDebuger.LogError("最后清空玩家数据，某些Dispose操作需要依赖玩家数据--------");
        ModelManager.DisposePlayerModel();     //最后清空玩家数据，某些Dispose操作需要依赖玩家数据

        // 释放红包数据
        GameDebuger.TODO("ModelManager.RedPacket.Dispose();");
    }

    //销毁功能模块数据
    void DisposeModuleData()
    {
        //GameDebuger.LogError("------------DisposeModuleData");

        LoginManager.LeaveState = EventObject.Leave_status_unkonwn;
        GameDebuger.TODO(@"WorldManager.FirstEnter = true;
            WorldManager.DataMgr.Reset();");
        StaticDispose.StaticDispose.doOnce();

        JoystickModule.Dispose();
        UIModuleManager.Instance.ResetWhenExit();
        
        GameServerInfoManager.Dispose();
        if (SystemTimeManager.Instance != null)
        {
            SystemTimeManager.Instance.Dispose();
        }

        TipManager.Dispose();  ///tip数据 避免切换角色后出现前数据提示内容BUG
    }

    //销毁场景数据
    void DisposeSceneData()
    {
        GameDebuger.Log("DisposeSceneData");

        GameDebuger.TODO(@"WorldManager.DataMgr.Destroy();
        WorldMapLoader.DataMgr.Destroy();
            BattleDataManager.DataMgr.Destroy();");
    }

    public void DoExiter()
    {
		LayerManager.Instance.LockUICamera(false);

		CancelInvoke("ResetClickFlag");
		Invoke("ResetClickFlag",0.5f);

        SPSdkManager.Instance.DoExiter(
        delegate (bool exited)
        {
			//渠道有提供退出确认窗口，游戏处理是否退出逻辑
            isClick = false;
            if (exited)
            {
				//确认退出
				HanderExitGame(false);
            }
            else
            {
				//取消退出
                if (reConnecting == false)
                {
                    if (LoginManager.Instance.SupportRelogin())
                    {
                        if (SocketManager.IsOnLink == false)
                        {
                            CheckConnected = true;
                        }
                    }
                }
            }
        },
        delegate ()
        {
			CancelInvoke("ResetClickFlag");
			//渠道没有提供退出确认窗口，需要自己实现
			OpenExitConfirmWindow();
        });
    }

    private void OpenExitConfirmWindow()
    {
        ProxyWindowModule.OpenConfirmWindow("退出游戏\n\n离线自动挂机", "",
            () =>
            {
                isClick = false;
                HanderExitGame(true);
            },
            () =>
            {
                if (reConnecting == false)
                {
                    if (LoginManager.Instance.SupportRelogin())
                    {
                        if (SocketManager.IsOnLink == false)
                        {
                            CheckConnected = true;
                        }
                    }
                }

                isClick = false;
            }, UIWidget.Pivot.Left, null, null, 0);
    }

	//处理退出游戏
	public void HanderExitGame(bool exitSDK = true)
    {
		if (exitSDK)
		{
			SPSdkManager.Instance.Exit();
		}

        LoginManager.Instance.RemoveListener();
        CheckConnected = false;

        ExitGame();
    }

    public void ReloginAccount(bool needLogout)
    {
        if (needLogout)
        {
            SPSdkManager.Instance.Logout(delegate (bool success)
            {
                if (success)
                {
                    DoReloginAccount(true);
                }
                else
                {
                    ProxyWindowModule.OpenMessageWindow("账号退出失败");
                }
            });
        }
        else
        {
            DoReloginAccount(true);
        }
    }

    public void DoReloginAccount(bool cleanSid = true)
    {
        ProxyLoginModule.serverInfo = null;
        ServerManager.Instance.loginAccountDto = null;
        if (cleanSid)
        {
            ServerManager.Instance.sid = null;
        }
        HanderRelogin();
    }

    public void HanderRelogin()
    {
        GameDebuger.Log("HanderRelogin");
        if (LogOutNotify != null)
        {
            LogOutNotify();
        }

        _relogin = false;

        DisposeOnReLogin();

        GotoLoginScene();
    }

    private bool _exited = false;

    private void ExitGame()
    {
#if UNITY_ANDROID
		DoExitGame();
#else
        if (GameDebuger.DebugForExit)
        {
            DoExitGame();
        }
        else
        {
            HanderRelogin();
        }
#endif
    }

    private void DoExitGame()
    {
        if (_exited)
        {
            return;
        }

        _exited = true;
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif

        //		if (SocketManager.IsOnLink)
        //		{
        //			ServiceRequestAction.requestServer(PlayerService.logout());
        //		}
        //		else
        //		{
        //			SocketManager.DataMgr.Close(false);
        //		}
        //		
        //		//		//ServiceProviderManager.Exit();
        //		
        //		// 关闭统计
        //		//UmengAnalyticsHelper.onPause();
        //		
        //		Debug.Log("Exit Game!!!");
        //
        //		DisposeOnApplicationQuit();
        //		
        //		//MachineManager.DataMgr.EndErrorInformation();
        //		
        //		//等待0.5秒后，关闭
        //		Invoke("_ExitGame", 0.2f);	
    }

    private bool _relogin = false;
    private void GotoLoginScene()
    {
        if (_relogin)
        {
            return;
        }
        _relogin = true;

        LoginManager.Instance.GotoLoginScene();
    }

    void ChangeIsClick()
    {
        isClick = false;
    }

    public void EnableClick()
    {
        ChangeIsClick();
    }

    private void OpenServerCloseTip(string tip)
    {
        ProxyWindowModule.OpenSimpleMessageWindow(tip, delegate ()
        {
            HanderRelogin();
        }, UIWidget.Pivot.Left, null, UILayerType.TopDialogue);
    }

    private void OpenReloginTip()
    {
        string tip = "网络不稳定，请重新连接";

        if (forceCheck)
        {
            tip = "网络中断了，请重新游戏";
        }

		tip = string.Format(tip + "[{0}]", LoginManager.CloseState);

        if (forceCheck)
        {
			ProxyWindowModule.OpenSimpleMessageWindow(tip, delegate ()
            {
                HanderRelogin();
            }, UIWidget.Pivot.Left, null, UILayerType.TopDialogue);
        }
        else
        {
            if (reConnecting == false)
            {
                WaitForReConnect = true;

                if (ReConnectTryCount >= ReConnectMaxCount || !Application.isPlaying)
                {
					if (ReConnectTryCount >= ReConnectTipMaxCount)
					{
						ProxyWindowModule.OpenSimpleConfirmWindow(tip,
							() =>
							{
								ReConnectTryCount = 0;
								DelayCheckReConnect();
							},
							() =>
							{
								HanderRelogin();
							},
							UIWidget.Pivot.Left, "重新连接", "返回登陆");
					}
					else
					{
						ProxyWindowModule.OpenSimpleConfirmWindow(tip,
							() =>
							{
								DelayCheckReConnect();
							},
							() =>
							{
								HanderRelogin();
							},
							UIWidget.Pivot.Left, "重连", "返回登陆", ReConnectTryCount*20, UILayerType.Dialogue, true);
					}
                }
                else
                {
                    DelayCheckReConnect();
                }
            }
        }
    }

    private void DelayCheckReConnect()
    {
		GameDebuger.Log("DelayCheckReConnect ReConnectTryCount=" + ReConnectTryCount);
        RequestLoadingTip.Show("正在连接服务器", true, true);
        CancelInvoke("CheckReConnect");
		float delayTime = 0.5f;
		if (ReConnectTryCount >= ReConnectMaxCount)
		{
			delayTime = 0.5f;
		}
		else
		{
			delayTime = ReConnectTryCount*3f+0.5f;
		}
		Invoke("CheckReConnect", delayTime);
    }


    private static bool reConnecting = false;

    private void CheckReConnect()
    {
        if (!(LoginManager.LeaveState == EventObject.Leave_status_duplicate))
        {
            ReConnectTryCount++;
            DisposeOnReconnect();
            reConnecting = true;
            //RequestLoadingTip.Show("正在连接服务器", true, true);
            WaitForReConnect = false;
            LoginManager.Instance.ReConnect();
            //TipManager.AddTip("正在尝试重新连接");
        }
    }

    private static bool forceCheck = false;
    //当网络断开时检查是否需要重连
    static public void CheckReloginWhenConnectClose(bool forceCheck_ = false)
    {
        forceCheck = forceCheck_;

        if (forceCheck)
        {
            reConnecting = true;
        }

        if (reConnecting == true)
        {
            CheckConnected = true;
        }
    }

    public static void OpenReloginTipWindow(string tip, bool exitAccount = false, bool needLogout = false)
    {
		LayerManager.Instance.LockUICamera(false);

        ProxyWindowModule.OpenSimpleMessageWindow(tip, delegate ()
        {
            if (exitAccount)
            {
                ExitGameScript.Instance.ReloginAccount(needLogout);
            }
            else
            {
                ExitGameScript.Instance.HanderRelogin();
            }
        }, UIWidget.Pivot.Left, null, UILayerType.TopDialogue);
    }

    public static void OpenExitTipWindow(string tip)
    {
		LayerManager.Instance.LockUICamera(false);

        BuiltInDialogueViewController.OpenView(tip, ()=> Instance.HanderExitGame(), null, UIWidget.Pivot.Left);
    }

	#if USE_JSZ
	private void OnGUI()
	{
		if (Application.isEditor)
		{
			GUI.color = Color.red;
			GUILayout.Label("JSB模式");
		}
	}
	#endif
}
