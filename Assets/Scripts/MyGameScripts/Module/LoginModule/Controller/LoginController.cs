using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using AppDto;
using LITJson;

public class LoginController : MonoViewController<LoginView>
{

    //	private ModelDisplayController _modelController;
    private OneShotUIEffect _loginEffect;

    #region IViewController

    /// <summary>
    /// 从DataModel中取得相关数据对界面进行初始化
    /// </summary>


    protected override void AfterInitView()
    {
        View.MovieButton_UIButton.gameObject.SetActive(AppGameVersion.startMovieMode);
    }

    /// <summary>
    /// Registers the event.
    /// DateModel中的监听和界面控件的事件绑定,这个方法将在InitView中调用
    /// </summary>
    protected override void RegistCustomEvent()
    {
        EventDelegate.Set(View.StartGameButton_UIButton.onClick, OnClickStartGameButton);
        EventDelegate.Set(View.LastLoginInfo_UIButton.onClick, OnClickLastLoginInfoButton);
        EventDelegate.Set(View.LastLoginRoleInfo_UIButton.onClick, OnClickLastLoginInfoButton);
        EventDelegate.Set(View.MovieButton_UIButton.onClick, OnClickMovieButton);
        EventDelegate.Set(View.NoticeButton_UIButton.onClick, OnNoticeButton);
        EventDelegate.Set(View.AccountButton_UIButton.onClick, OnClickAccountButton);
        EventDelegate.Set(View.AgreementButton_UIButton.onClick, OnClickAgreementButton);
        EventDelegate.Set(View.QRCodeScanBtn_UIButton.onClick, OnQRCodeScanBtnClick);

        EventDelegate.Set(View.VersionLabel_UIButton.onClick, OnVersionButtonClick);
        EventDelegate.Set(View.ChangeUrlBtn_UIButton.onClick, OnChangeUrlBtnClick);
        EventDelegate.Set(View.RestoreBtn_UIButton.onClick, OnRestoreBtnClick);
        EventDelegate.Set(View.ShowBtn_UIButton.onClick, OnShowBtnClick);

        EventDelegate.Set(View.SPLoginButton_UIButton.onClick, OnSPLoginButtonClick);

        //GameServerInfoManager.onServerListReturn += OnServerListReturn;

        LoginManager.Instance.OnLoginMessage += OnLoginMessage;
        LoginManager.Instance.OnLoginProcess += OnLoginProcess;
        //LoginManager.Instance.OnWaitForLoginQueue += OnWaitForLoginQueue;
    }

    protected override void OnDispose()
    {
        if (_loginEffect != null)
        {
            _loginEffect.Dispose();
            _loginEffect = null;
        }
        //GameServerInfoManager.onServerListReturn -= OnServerListReturn;
        UIHelper.DisposeUITexture(View.LoadingTexture_UITexture);
        UIHelper.DisposeUITexture(View.LogoTexture_UITexture);

        LoginManager.Instance.OnLoginMessage -= OnLoginMessage;
        LoginManager.Instance.OnLoginProcess -= OnLoginProcess;
        //LoginManager.Instance.OnWaitForLoginQueue -= OnWaitForLoginQueue;

        SPSdkManager.Instance.OnLoginSuccess -= OnLoginSuccess;
        SPSdkManager.Instance.OnLoginCancel -= OnLoginCancel;
        SPSdkManager.Instance.OnLoginFail -= OnLoginFail;

        SPSdkManager.Instance.OnLoginSuccess -= OnSwitchLoginSuccess;
        SPSdkManager.Instance.OnLogoutNotify -= OnLogout;

        //畅游SDK接口
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.SWITCH_USER_SUCCESS, OnCYSwitchLoginSuccess);
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGOUT, OnCYLogout);
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGIN_SUCCESS, OnCYLoginSuccess);
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGIN_FAILED, OnCYLoginFail);
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGIN_CANCEL, OnCYLoginCancel);




    }

    #endregion

    private GameServerInfo _currentServerInfo;
    private bool _isLogined = false;

    public void Open()
    {
        _isLogined = false;
        _currentServerInfo = null;

        View.VersionLabel_UILabel.text = AppGameVersion.ShowVersion;
        View.BanhaoLabel_UILabel.text = AppGameVersion.GetBanhao();
        ShowVersion(true);

        View.loadingSlider_UISlider.value = 0.1f;
        View.LoadingLabel_UILabel.text = "";
        View.LoadingTips_UILabel.text = LoadingTipManager.GetLoadingTip();

        View.LogoTexture_UITexture.cachedGameObject.SetActive(!GameSetting.IsOriginWinPlatform);
        View.LogoTexture_UITexture.mainTexture = Resources.Load<Texture>("Textures/logo");
        View.LogoTexture_UITexture.MakePixelPerfect();
        View.LoadingTexture_UITexture.mainTexture = Resources.Load<Texture>("Textures/LoadingBG/loginBG");
        //LoadEffect();

        View.ChangeUrlBtn_UIButton.gameObject.SetActive(false);
        View.RestoreBtn_UIButton.gameObject.SetActive(false);
        View.ShowBtn_UIButton.gameObject.SetActive(false);

        //隐藏Vesion标识隐藏功能按钮
        View.ChangeUrlBtn_UIButton.gameObject.SetActive(false);
        View.RestoreBtn_UIButton.gameObject.SetActive(false);
        View.ShowBtn_UIButton.gameObject.SetActive(false);

        View.AgreementButton_UIButton.gameObject.SetActive(false);
        View.NoticeButton_UIButton.gameObject.SetActive(false);
        View.AccountButton_UIButton.gameObject.SetActive(false);
        View.QRCodeScanBtn_UIButton.gameObject.SetActive(false);

        HideSPLoginButton();

        ProxyWindowModule.closeSimpleWinForTop();

        if (ProxyLoginModule.serverInfo != null)
        {
            OnServerListReturn();
            HideOtherUI();

            Login(ProxyLoginModule.serverInfo, ProxyLoginModule.accountPlayerDto);
            ProxyLoginModule.serverInfo = null;
            ProxyLoginModule.accountPlayerDto = null;
        }
        else
        {
            if (ServerManager.Instance.loginAccountDto == null)
            {
                if (string.IsNullOrEmpty(ServerManager.Instance.sid))
                {
                    OnClickAccountButton();
                }
                else
                {
                    HideOtherUI();
                    OnLoginSuccess(ServerManager.Instance.isGuest, ServerManager.Instance.sid);
                }
            }
            else
            {
                HideOtherUI();

                // 苹果PC端特殊处理
                if (!GameSetting.IsOriginWinPlatform)
                {
                    OnLoginSdkSuccess(ServerManager.Instance.loginAccountDto);
                }
                else
                {
                    OnLoginSuccess(ServerManager.Instance.isGuest, ServerManager.Instance.sid);
                }
            }
        }

        AudioManager.Instance.PlayMusic("music_login");
        LayerManager.Instance.SwitchLayerMode(UIMode.LOGIN);

        LayerManager.Instance.LockUICamera(false);
    }

    private void ShowVersion(bool show)
    {
        //2017-12-4 版署包定死了版本，先屏蔽
        View.VersionLabel_UILabel.gameObject.SetActive(false);
        View.BanhaoLabel_UILabel.gameObject.SetActive(show);
    }

    private void LoadEffect()
    {
        GameDebuger.TODO(@"if (SystemSetting.LowMemory)
			return;
            ");
        if (_loginEffect == null)
        {
            _loginEffect = OneShotUIEffect.BeginFollowEffect("ui_eff_Login_Effect", View.LoadingTexture_UITexture,
                Vector2.zero, 1);
        }
        else
        {
            _loginEffect.SetActive(true);
        }
    }

    #region Vesion标识隐藏功能

    private long _startCheckTime;
    private int _versionClickCount;

    private void OnVersionButtonClick()
    {
        if (_versionClickCount == 0)
        {
            _startCheckTime = DateTime.Now.Ticks;
        }

        _versionClickCount++;
        GameDebuger.Log("_versionClickCount " + _versionClickCount);

        if (_versionClickCount > 10)
        {
            EventDelegate.Remove(View.VersionLabel_UIButton.onClick, OnVersionButtonClick);

            long passTime = DateTime.Now.Ticks - _startCheckTime;
            TimeSpan elapsedSpan = new TimeSpan(passTime);

            GameDebuger.Log("passTime = " + elapsedSpan.TotalSeconds + " " + 2);
            if (elapsedSpan.TotalSeconds < 2)
            {
                View.ChangeUrlBtn_UIButton.gameObject.SetActive(true);
                View.RestoreBtn_UIButton.gameObject.SetActive(true);
                View.ShowBtn_UIButton.gameObject.SetActive(true);
            }
        }
    }

    private void OnChangeUrlBtnClick()
    {
        string url = string.Format("http://dev.h5.cilugame.com/h5/{0}/android/dlls/", GameSetting.ResDir);
        GameDebuger.TODO(" BaoyugameSdk.PatchUtilsOpt(PatchVersionNotify.OPT_CHANGE_URL, url + \"version.json\" + \";\" + url); ");
    }

    private void OnRestoreBtnClick()
    {
        GameDebuger.TODO("BaoyugameSdk.PatchUtilsOpt(PatchVersionNotify.OPT_RESTORE, string.Empty);");
    }

    private void OnShowBtnClick()
    {
        GameDebuger.TODO(@"BaoyugameSdk.PatchUtilsOpt(PatchVersionNotify.OPT_SHOW, string.Empty);");
    }

    #endregion

    private void OnLoginSdkSuccess(LoginAccountDto account)
    {
        TalkingDataHelper.OnEventSetp("GameAccountLogin/LoginAccountSuccess"); //进入游戏登陆界面
        SPSDK.gameEvent("10014");   //登陆成功，进入游戏登陆界面

        ServerManager.Instance.loginAccountDto = account;
        ServerManager.Instance.uid = account.uid;
        SPSdkManager.Instance.UpdateUserInfo(account.uid);

        GameServerInfoManager.InitDefaultServer();

        OnServerListReturn();

        string token = account.token;
        GameDebuger.Log("token = " + token);

        ShopAccountTip(string.Format("欢迎进入{0}", GameSetting.GameName));

        string accountId = account.accountId.ToString();
        if (account.firstRegister)
        {
            SPSdkManager.Instance.Regster(accountId, account.uid);
        }

        View.StartPanel_Transform.gameObject.SetActive(true);
        View.ButtonGroup.SetActive(true);
        View.ShowDes_Go.SetActive(true);

        View.LogoTexture_UITexture.cachedGameObject.SetActive(true);
        //View.AgreementButton_UIButton.gameObject.SetActive(IsAgreementSupport());
        View.AgreementButton_UIButton.gameObject.SetActive(false);
        //View.NoticeButton_UIButton.gameObject.SetActive(true);
        View.NoticeButton_UIButton.gameObject.SetActive(false);
        View.AccountButton_UIButton.gameObject.SetActive(true);

        CheckQRCodeScanSupport();

        AnnouncementDataManager.Instance.CheckUpdate();

        PayManager.Instance.Setup();
    }

    //是否支持用户协议
    private bool IsAgreementSupport()
    {
        return true;
    }

    /// <summary>
    /// 统一使用读表判断是否允许使用扫码
    /// </summary>
    private void CheckQRCodeScanSupport()
    {
        if (GameSetting.IsOriginWinPlatform)
        {
            View.QRCodeScanBtn_UIButton.gameObject.SetActive(false);
            return;
        }

        GameStaticConfigManager.Instance.LoadStaticConfig(GameStaticConfigManager.Type_StaticVersion,
            json =>
            {
                var dic = JsHelper.ToObject<Dictionary<string, object>>(json);
                try
                {
                    string qrCodeKey = "supportQRCodeVer";
                    if (GameSetting.Platform != GameSetting.PlatformType.IOS)
                    {
                        qrCodeKey = "androidQRCodeVer";
                    }

                    if (dic.ContainsKey(qrCodeKey) && !string.IsNullOrEmpty((string)dic[qrCodeKey]))
                    {
                        //View.QRCodeScanBtn_UIButton.gameObject.SetActive(AppGameVersion.SpVersionCode <= GameSetting.ParseVersionCode((string)dic[qrCodeKey]));
                        View.QRCodeScanBtn_UIButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        // IOS 默认关闭，其它相反
                        //View.QRCodeScanBtn_UIButton.gameObject.SetActive(GameSetting.Platform != GameSetting.PlatformType.IOS);
                        View.QRCodeScanBtn_UIButton.gameObject.SetActive(false);
                    }
                }
                catch
                {
                    View.QRCodeScanBtn_UIButton.gameObject.SetActive(false);
                }
            }, delegate (string obj)
            {
                View.QRCodeScanBtn_UIButton.gameObject.SetActive(false);
            });
    }

    private void ShopAccountTip(string accountTip)
    {
        View.AccountTipGroupWidget.cachedGameObject.SetActive(true);
        View.AccountTipLabel_UILabel.text = accountTip;

        UIHelper.PlayAlphaTween(View.AccountTipGroupWidget, 1f, 0f, 1f, 2f);
    }

    private void OnServerListReturn()
    {
        OnSelectionChange(PlayerPrefs.GetString(GameSetting.LastServerPrefsName));
    }

    private void OnClickMovieButton()
    {
        TalkingDataHelper.OnEventSetp("PlayCG", "OpenFromClick");
#if UNITY_STANDALONE && UNITY_EDITOR
        CGPlayer.PlayCG("Assets/GameResources/ArtResources/" + PathHelper.CG_Asset_PATH, null);
#else
        CGPlayer.PlayCG(PathHelper.CG_Asset_PATH, null);
#endif
    }

    private void OnNoticeButton()
    {
        TalkingDataHelper.OnEventSetp("Announcement", "Open");
        ProxyLoginModule.OpenAnnouncement();
    }

    private void OnSPLoginButtonClick()
    {
        RequestLoadingTip.Show("", true, false, 1f);
        OpenSdk();
    }

    private void OnClickAccountButton()
    {
        TalkingDataHelper.OnEvent("AccountButton");
        //GameSetting.DeviceLoginMode = !GameSetting.DeviceLoginMode;

        HideAccountUI();

        if (string.IsNullOrEmpty(ServerManager.Instance.sid))
        {
            OpenAccountLogin();
        }
        else
        {
            if (GameSetting.Channel == AgencyPlatform.Channel_cyou)
            {

                if (SPSDK.apiAvailable(CySdk.ApiCode.LOGOUT))
                {
                    System.Action<string> callback = null;
                    callback = (json) =>
                    {
                        OpenAccountLogin();
                        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGOUT, callback);
                    };
                    SPSdkManager.Instance.CYLogout().CYCallback(CySdk.ResultCode.LOGOUT, callback);
                }
                else
                {
                    OpenAccountLogin();
                }
            }
            else
            {
                SPSdkManager.Instance.Logout(delegate (bool success) { OpenAccountLogin(); });
            } 
            
        }
    }

    private void HideAccountUI()
    {
        HideOtherUI();

        View.NoticeButton_UIButton.gameObject.SetActive(false);
        View.AccountButton_UIButton.gameObject.SetActive(false);
        View.QRCodeScanBtn_UIButton.gameObject.SetActive(false);
        View.LogoTexture_UITexture.cachedGameObject.SetActive(!GameSetting.IsOriginWinPlatform);
    }

    private void OnQRCodeScanBtnClick()
    {
        ProxyLoginModule.Hide();

        ProxyQRCodeModule.OpenQRCodeScanView(OnQRCodeScanReturn);
    }

    private void OnQRCodeScanReturn()
    {
        ProxyLoginModule.Show();
    }

    private void OpenAccountLogin()
    {
        if (PlayerPrefsExt.GetBool("PassAgreement") || !IsAgreementSupport())
        {
            JSTimer.Instance.StartCoroutine(WaitOpenSDK());
        }
        else
        {
            ProxyLoginModule.OpenAgreement(OpenSdk);
        }
    }

    private IEnumerator WaitOpenSDK()
    {
        if (GameSetting.IsOriginWinPlatform)
        {
            yield return new WaitForSeconds(0.3f);
            //            yield return null;
        }
        OpenSdk();
    }

    private void OpenSdk()
    {
        //登录前移除旧监听
        #region 旧登录回调
        SPSdkManager.Instance.OnLoginSuccess -= OnSwitchLoginSuccess;
        SPSdkManager.Instance.OnLogoutNotify -= OnLogout;
        SPSdkManager.Instance.OnLoginSuccess -= OnLoginSuccess;
        SPSdkManager.Instance.OnLoginCancel -= OnLoginCancel;
        SPSdkManager.Instance.OnLoginFail -= OnLoginFail;


        TalkingDataHelper.OnEventSetp("GameAccountLogin/OpenSdk"); //进入SDK登陆
        SPSdkManager.Instance.OnLoginSuccess += OnLoginSuccess;
        SPSdkManager.Instance.OnLoginCancel += OnLoginCancel;
        SPSdkManager.Instance.OnLoginFail += OnLoginFail;
        #endregion

        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.SWITCH_USER_SUCCESS, OnCYSwitchLoginSuccess);
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGOUT, OnCYLogout);
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGIN_SUCCESS, OnCYLoginSuccess);
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGIN_FAILED, OnCYLoginFail);
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGIN_CANCEL, OnCYLoginCancel);


        SPSdkManager.Instance.CYCallback(CySdk.ResultCode.SWITCH_USER_SUCCESS, OnCYSwitchLoginSuccess);
        SPSdkManager.Instance.CYCallback(CySdk.ResultCode.LOGOUT, OnCYLogout);
        SPSdkManager.Instance.CYCallback(CySdk.ResultCode.LOGIN_SUCCESS, OnCYLoginSuccess);
        SPSdkManager.Instance.CYCallback(CySdk.ResultCode.LOGIN_FAILED, OnCYLoginFail);
        SPSdkManager.Instance.CYCallback(CySdk.ResultCode.LOGIN_CANCEL, OnCYLoginCancel);



        CancelInvoke("DelayShowSPLoginButton");
        Invoke("DelayShowSPLoginButton", 2f);

        SPSdkManager.Instance.Login();

        //SPSdkManager.Instance.CYLogin();

    }

    private void DelayShowSPLoginButton()
    {
        if (SPSdkManager.Instance.WaitingLoginResult)
        {
            View.SPLoginButton_UIButton.gameObject.SetActive(true);
        }
    }

    private void HideSPLoginButton()
    {
        CancelInvoke("DelayShowSPLoginButton");
        View.SPLoginButton_UIButton.gameObject.SetActive(false);
    }


    private void OnCYLoginSuccess(string jsondata)
    {
        // CySdk.Result result= CySdk.Result.ToObject(jsondata);
        CySdk.Result result = CySdk.Result.ToObject(jsondata);
        OnLoginSuccess(false, result.data.ToJson());

    }

    private void OnCYLoginFail(string jsondata)
    {
        OnLoginFail();
    }

    private void OnCYLoginCancel(string jsondata)
    {
        OnLoginCancel();
    }

    private void OnCYLogout(string jsondata)
    {
        ExitGameScript.OpenReloginTipWindow("您已经注销了账号， 请重新游戏", true);
    }

    private void OnCYSwitchLoginSuccess(string jsondata)
    {
        //CySdk.Result result = JsHelper.ToObject<CySdk.Result>(jsondata);
        CySdk.Result result = CySdk.Result.ToObject(jsondata);

        OnSwitchLoginSuccess(false, result.data.ToJson());
    }

    /// <summary>
    /// 畅游登录成功回调
    /// </summary>
    /// <param name="isGuest"></param>
    /// <param name="sid"></param>
    private void
        OnLoginSuccess(bool isGuest, string sid)
    {
        HideSPLoginButton();

        View.LoadingTips_UILabel.text = LoadingTipManager.GetLoadingTip();

        ProxyWindowModule.closeSimpleWinForTop();

        ServerManager.Instance.isGuest = isGuest;
        ServerManager.Instance.sid = sid;
        BehaviorHelper.SetupSid(sid);

        SPSdkManager.Instance.OnLoginSuccess -= OnLoginSuccess;
        SPSdkManager.Instance.OnLoginCancel -= OnLoginCancel;
        SPSdkManager.Instance.OnLoginFail -= OnLoginFail;

        SPSdkManager.Instance.OnLoginSuccess += OnSwitchLoginSuccess;
        SPSdkManager.Instance.OnLogoutNotify += OnLogout;

        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGIN_SUCCESS, OnCYLoginSuccess);
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGIN_FAILED, OnCYLoginFail);
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGIN_CANCEL, OnCYLoginCancel);


        SPSdkManager.Instance.CYCallback(CySdk.ResultCode.SWITCH_USER_SUCCESS, OnCYSwitchLoginSuccess);
        SPSdkManager.Instance.CYCallback(CySdk.ResultCode.LOGOUT, OnCYLogout);


        if (GameSetting.GMMode)
        {
            LoginAccountDto loginAccountDto = new LoginAccountDto();
            loginAccountDto.token = sid;
            loginAccountDto.players = new List<AccountPlayerDto>();
            OnLoginSdkSuccess(loginAccountDto);
        }
        else if (GameSetting.IsOriginWinPlatform)
        {
            OnLoginSdkSuccess(ServerManager.Instance.loginAccountDto);
        }
        else
        {
            TalkingDataHelper.OnEventSetp("GameAccountLogin/RequestSsoAccountLogin"); //请求token
            SPSDK.gameEvent("10011");       //登陆成功，请求token
            ServiceProviderManager.RequestSsoAccountLogin(ServerManager.Instance.sid, ServerManager.Instance.uid, GameSetting.Channel,
                GameSetting.SubChannel, GameSetting.LoginWay, GameSetting.AppId, GameSetting.PlatformTypeId, SPSDK.deviceId(), SPSDK.channelId(),SPSDK.appName(), SPSDK.appVersionName(), delegate (LoginAccountDto response)
                {
                    if (response != null)
                    {
                        SPSDK.gameEvent("10012");   //token请求成功
                        //畅游渠道处理
                        if (GameSetting.Channel == AgencyPlatform.Channel_cyou)
                        {
                            if (response.code == 0)
                            {
                                //解析服务器返回的数据
                                JsonData retData = response.sdkResData;

                                //解析服务器返回的数据
                                string status = (string)retData["status"];
                                string gameUid = (string)retData["userid"];
                                string channelOid = "";
                                string accessToken = "";
                                if (((IDictionary)retData).Contains("oid") && retData["oid"] != null)
                                    channelOid = (string)retData["oid"];
                                if (((IDictionary)retData).Contains("access_token") && retData["access_token"] != null)
                                    accessToken = (string)retData["access_token"];
                                bool pass = (status == "1");
                                if (((IDictionary)retData).Contains("extension") && retData["extension"] != null)
                                {
                                    string extStr = (string)retData["extension"];
                                    System.Action<string> callback = null;
                                    callback = (json) =>
                                    {

                                        JsonData jsonData = JsonMapper.ToObject(json);
                                        bool type = (bool)jsonData["data"];

                                        if (type)
                                        {
                                            SPSDK.showToast("已经实名验证");
                                        }
                                        else
                                        {
                                            //打开实名绑定页面
                                            SPSDK.showBindingView();
                                        }
                                        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.AUTH_SUCCESS, callback);
                                    };
                                    SPSdkManager.Instance.CYOtherVerify(extStr).CYCallback(CySdk.ResultCode.AUTH_SUCCESS, callback);
                                }

                                SPSdkManager.Instance.CYTokenVerify(pass, gameUid, channelOid, accessToken);
                                if (pass)
                                {
                                    OnLoginSdkSuccess(response);
                                }
                                else
                                {
                                    SPSDK.gameEvent("10013");   //token验证失败
                                    string msg = "账号登陆失败， 请重试";
                                    ProxyWindowModule.OpenMessageWindow(msg, "", OnClickAccountButton);
                                }
                            }
                            else
                            {
                                SPSDK.gameEvent("10013");   //token验证失败
                                //Token验证成功传true,验证失败传false(用于重新使用login接口调起登录框登入)
                                SPSdkManager.Instance.CYTokenVerify(false, "", "", "");
                                string msg = "账号登陆失败， 请重试";
                                ProxyWindowModule.OpenMessageWindow(msg, "", OnClickAccountButton);
                            }
                        }
                        else
                        {
                            OnLoginSdkSuccess(response);
                        }
                    }
                    else
                    {
                        SPSDK.gameEvent("10013");   //token请求失败
                        string msg = "账号登陆失败， 请重试";
                        //畅游渠道处理
                        if (GameSetting.Channel == AgencyPlatform.Channel_cyou)
                        {
                            //Token验证成功传true,验证失败传false(用于重新使用login接口调起登录框登入)
                            SPSdkManager.Instance.CYTokenVerify(false, "", "", "");
                            ProxyWindowModule.OpenMessageWindow(msg, "", OnClickAccountButton);
                        }
                        else
                        {
                            ProxyWindowModule.OpenMessageWindow(msg, "", OnClickAccountButton);

                        }
                    }
                });
        }
    }

    /* 旧登录逻辑
    private void OnLoginSuccess(bool isGuest, string sid)
	{
		HideSPLoginButton();

		View.LoadingTips_UILabel.text = LoadingTipManager.GetLoadingTip();

		ProxyWindowModule.closeSimpleWinForTop();

		ServerManager.Instance.isGuest = isGuest;
		ServerManager.Instance.sid = sid;
		BehaviorHelper.SetupSid(sid);

		SPSdkManager.Instance.OnLoginSuccess -= OnLoginSuccess;
		SPSdkManager.Instance.OnLoginCancel -= OnLoginCancel;
		SPSdkManager.Instance.OnLoginFail -= OnLoginFail;

		SPSdkManager.Instance.OnLoginSuccess += OnSwitchLoginSuccess;
		SPSdkManager.Instance.OnLogoutNotify += OnLogout;

		if (GameSetting.GMMode)
		{
			LoginAccountDto loginAccountDto = new LoginAccountDto();
			loginAccountDto.token = sid;
			loginAccountDto.players = new List<AccountPlayerDto>();
			OnLoginSdkSuccess(loginAccountDto);
		}
		else if (GameSetting.IsOriginWinPlatform)
		{
			OnLoginSdkSuccess(ServerManager.Instance.loginAccountDto);
		}
		else
		{
			TalkingDataHelper.OnEventSetp("GameAccountLogin/RequestSsoAccountLogin"); //请求token
			ServiceProviderManager.RequestSsoAccountLogin(sid, GameSetting.Channel, GameSetting.SubChannel,
                GameSetting.LoginWay, GameSetting.AppId, GameSetting.PlatformTypeId, BaoyugameSdk.getUUID(), GameSetting.Channel, GameSetting.BundleId, delegate (LoginAccountDto response)
				{
					if (response != null && response.code == 0)
					{
						OnLoginSdkSuccess(response);
					}
					else
					{
						string msg = "账号登陆失败， 请重试";
						if (response != null)
						{
							msg = response.msg;
						}
						ProxyWindowModule.OpenMessageWindow(msg, "", OnClickAccountButton);
					}
				});
		}
	}
    */
    private void OnLoginCancel()
    {
        SPSdkManager.Instance.OnLoginSuccess -= OnLoginSuccess;
        SPSdkManager.Instance.OnLoginCancel -= OnLoginCancel;
        SPSdkManager.Instance.OnLoginFail -= OnLoginFail;

        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGIN_SUCCESS, OnCYLoginSuccess);
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGIN_FAILED, OnCYLoginFail);
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGIN_CANCEL, OnCYLoginCancel);


        //因为渠道SDK的回调时序不固定， 会引发客户端重新打开SDK登陆后， SDK又关闭了，所以收到此回调后延迟一下再调用OpenSDK
        //CancelInvoke("OpenSdk");
        RequestLoadingTip.Show("", true, false, 0.5f);
        //Invoke("OpenSdk", 0.5f);

        View.SPLoginButton_UIButton.gameObject.SetActive(true);
    }

    private void OnLoginFail()
    {
        SPSdkManager.Instance.OnLoginSuccess -= OnLoginSuccess;
        SPSdkManager.Instance.OnLoginCancel -= OnLoginCancel;
        SPSdkManager.Instance.OnLoginFail -= OnLoginFail;

        ///畅游sdk回调
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGIN_SUCCESS, OnCYLoginSuccess);
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGIN_FAILED, OnCYLoginFail);
        SPSdkManager.Instance.RemoveCYCallback(CySdk.ResultCode.LOGIN_CANCEL, OnCYLoginCancel);

        ProxyWindowModule.OpenMessageWindow("登陆失败", "提示");
        View.SPLoginButton_UIButton.gameObject.SetActive(true);
    }

    private void OnSwitchLoginSuccess(bool isGuest, string sid)
    {
        if (ServerManager.Instance.sid != sid)
        {
            ServerManager.Instance.isGuest = isGuest;
            ServerManager.Instance.sid = sid;

            ExitGameScript.Instance.DoReloginAccount(false);
        }
        else
        {
            ProxyWindowModule.closeSimpleWinForTop();
        }
    }

    private void OnLogout(bool success)
    {
        if (success)
        {
            ExitGameScript.OpenReloginTipWindow("您已经注销了账号， 请重新游戏", true);
        }
    }

    void OnApplicationPause(bool paused)
    {
        CancelInvoke("CheckLoginResult");

        if (!paused)
        {
            if (SPSdkManager.Instance.WaitingLoginResult)
            {
                RequestLoadingTip.Show("", true, false, 0.5f);
                //Invoke("CheckLoginResult", 0.5f);
            }
        }
        else
        {

        }
    }

    private void CheckLoginResult()
    {
        if (SPSdkManager.Instance.WaitingLoginResult)
        {
            GameDebuger.Log("Check WaitingLoginResult and CancleLogin");
            SPSdkManager.Instance.CallbackLoginCancel();
        }
    }

    private void HideOtherUI()
    {
        View.LoadingPanel_Transform.gameObject.SetActive(false);
        View.AccountTipGroupWidget.cachedGameObject.SetActive(false);
        View.StartPanel_Transform.gameObject.SetActive(false);
        //View.ButtonGroup.SetActive(false);
    }

    private void OnClickAgreementButton()
    {
        TalkingDataHelper.OnEventSetp("Agreement", "Open");
        ProxyLoginModule.OpenAgreement();
    }

    private void OnClickStartGameButton()
    {
        if (_currentServerInfo == null)
        {
            TipManager.AddTip("提示：选择服务器为空");
            return;
        }
        TalkingDataHelper.OnEvent("GameStart");

        AccountPlayerDto playerDto = null;

        if (GameDebuger.Debug_PlayerId != 0)
        {
            playerDto = new AccountPlayerDto();
            playerDto.nickname = GameDebuger.Debug_PlayerId.ToString();
            playerDto.id = GameDebuger.Debug_PlayerId;
            playerDto.gameServerId = 0;
        }
        else
        {
            playerDto = ServerManager.Instance.HasPlayerAtServer(_currentServerInfo.serverId);
        }

        Login(_currentServerInfo, playerDto);
    }

    public void Login(GameServerInfo serverInfo, AccountPlayerDto accountPlayerDto)
    {
        if (serverInfo.runState == 3)
        {
            //TipManager.AddTip("服务器维护中，请稍候");
            //return;
        }

        if (_isLogined == false)
        {
            _isLogined = true;

            PlayerPrefs.SetString(GameSetting.LastServerPrefsName, serverInfo.GetServerUID());

            HideOtherUI();
            View.ButtonGroup.SetActive(false);
            View.ShowDes_Go.SetActive(false);

            ScreenMaskManager.FadeInOut(() =>
            {
                //先屏蔽进度宠物显示
                //                if (_modelController == null)
                //                {
                //                    _modelController = ModelDisplayController.GenerateUICom(_view.loadingSliderThumb);
                //                    _modelController.Init(200, 200, new Vector3(-16.5f, -33.9f, 8.5f), 1f, ModelHelper.Anim_run, false);
                //                    _modelController.SetOrthographic(1.2f);
                //                    _modelController.SetupModel(2000);
                //                    _modelController.transform.localPosition = new Vector3(-20f, 56f, 0f);
                //                }

                View.LogoTexture_UITexture.cachedGameObject.SetActive(false);
                UIHelper.DisposeUITexture(View.LoadingTexture_UITexture);
                View.LoadingTexture_UITexture.mainTexture = Resources.Load<Texture>("Textures/LoadingBG/loadingBG");
                View.LoadingTexture_UITexture.cachedGameObject.SetActive(true);
                View.LoadingPanel_Transform.gameObject.SetActive(true);

                ShowVersion(false);

                if (_loginEffect != null)
                {
                    _loginEffect.SetActive(false);
                }

                ServerManager.Instance.SetServerInfo(serverInfo);
                LoginManager.Instance.start(ServerManager.Instance.loginAccountDto.token, serverInfo, accountPlayerDto);
            }, 0f, 0.3f);
        }
    }

    #region 排队

    //private QueueWindowPrefabController queueWindowCon;

    //private void OnWaitForLoginQueue(LoginQueuePlayerDto dto)
    //{
    //    _isLogined = false;
    //    //        if (queueWindowCon == null)
    //    //        {
    //    //            queueWindowCon = ProxyWindowModule.OpenQueueWindow("test", dto.waitCount.ToString(), dto.waitCd.ToString());
    //    //        }
    //    //        else
    //    //        {
    //    //            queueWindowCon.UpdateData("test",dto.waitCount.ToString(),dto.waitCd.ToString());
    //    //        }
    //}

    #endregion

    public void OnSelectionChange(string name)
    {
        UpdateCurrentServerInfo(GameServerInfoManager.GetServerInfoByName(name));
    }

    private void UpdateCurrentServerInfo(GameServerInfo serverInfo)
    {
        if (serverInfo != null && serverInfo.dboState == 1)
        {
            _currentServerInfo = serverInfo;
            PlayerPrefs.SetString(GameSetting.LastServerPrefsName, serverInfo.GetServerUID());
            ServerManager.Instance.SetServerInfo(serverInfo);
            View.LastLoginInfo_label_UILabel.text = ServerNameGetter.GetServerName(_currentServerInfo);
            View.LastLoginInfo_state_UISprite.spriteName =
                ServerNameGetter.GetServiceStateSpriteName(_currentServerInfo);
            AccountPlayerDto accountPlayerDto = ServerManager.Instance.HasPlayerAtServer(_currentServerInfo.serverId);
            if (accountPlayerDto != null)
            {
                View.LastLoginRoleInfo_label_UILabel.text = accountPlayerDto.nickname;
                View.LastLoginRoleInfo.SetActive(true);
            }
            else
            {
                View.LastLoginRoleInfo_label_UILabel.text = "";
                View.LastLoginRoleInfo.SetActive(false);
            }
        }
        else
        {
            View.LastLoginInfo_label_UILabel.text = "选择服务器";
            View.LastLoginInfo_state_UISprite.spriteName = "";
        }
    }

    private void OnClickLastLoginInfoButton()
    {
        TalkingDataHelper.OnEventSetp("SelectServer", "显示服务器选择框");
        SPSDK.gameEvent("10015");       //点击“服务器”，打开服务器选择界面
        OpenServerListModule();
    }

    public void OpenServerListModule()
    {
        ProxyServerListModule.Open((serverInfo, accountPlayerDto) =>
        {
            UpdateCurrentServerInfo(serverInfo);

            Login(serverInfo, accountPlayerDto);
        });
    }

    void OnLoginMessage(string msg)
    {
        View.LoadingLabel_UILabel.text = msg;
    }

    void OnLoginProcess(float percent)
    {
        View.loadingSlider_UISlider.value = percent;
    }
}