// **********************************************************************
// Copyright (c) 2015 cilugame. All rights reserved.
// File     : TestSdkController.cs
// Author   : senkay <senkay@126.com>
// Created  : 05/18/2015 
// Porpuse  : 
// **********************************************************************
//
using UnityEngine;

public class TestSdkController : MonoViewController<TestSdkView>
{
    public const float CheckQRCodeLoginInterval = 1f;



    private string _qrCodeSid;
    private bool _requestLoginNow;

	#region IViewController

    

	protected override void RegistCustomEvent ()
	{
		base.RegistCustomEvent ();
		EventDelegate.Set(View.LocalLoginButton_UIButton.onClick, OnLocalLoginButtonClick);
		EventDelegate.Set(View.ExternalLoginButton_UIButton.onClick, OnExternalLoginButtonClick);
		EventDelegate.Set(View.TestLoginButton_UIButton.onClick, OnTestLoginButtonClick);
		EventDelegate.Set(View.RegButton_UIButton.onClick, OnRegButtonClick);
	    EventDelegate.Set(View.UseGuideLabel_UIButton.onClick, OnUseGuideBtnClick);
	    EventDelegate.Set(View.DownloadLabel_UIButton.onClick, OnDownloadBtnClick);
	    EventDelegate.Set(View.RefreshBtn_UIButton.onClick, RequestQRCode);
	    EventDelegate.Set(View.RefreshScanBtn_UIButton.onClick, RequestQRCode);
    }

	private void LockLoginButton(bool locked)
	{
		View.LocalLoginButton_UIButton.GetComponent<UISprite>().isGrey = locked;
		View.LocalLoginButton_UIButton.enabled = !locked;

		View.ExternalLoginButton_UIButton.GetComponent<UISprite>().isGrey = locked;
		View.ExternalLoginButton_UIButton.enabled = !locked;

		View.TestLoginButton_UIButton.GetComponent<UISprite>().isGrey = locked;
		View.TestLoginButton_UIButton.enabled = !locked;
	}

	//赐麓SDK渠道登陆处理
	private void OnLocalLoginButtonClick()
	{
		string account = View.NameInput_UIInput.value;
		string password = View.PasswordInput_UIInput.value;

		RequestSdkAccountLogin(account, password);
	}

	//GM登陆处理
	private void OnExternalLoginButtonClick()
	{
		string playerId = View.NameInput_UIInput.value;
		string password = View.PasswordInput_UIInput.value;

		if (string.IsNullOrEmpty(playerId))
		{
			TipManager.AddTip("提示：请先输入账号");
			return;
		}

		if (string.IsNullOrEmpty(password))
		{
			TipManager.AddTip("提示：请先输入密码");
			return;
		}

		LockLoginButton(true);

		ServiceProviderManager.RequestTokenByGM(playerId, password, delegate(string token) {
			//判断token格式， 需要含有 - 符号
			if (token.Contains("-"))
			{
				GameDebuger.Debug_PlayerId = StringHelper.ToInt(playerId);
				AccountSession accountSession = new AccountSession();
				accountSession.sid = token;
				RequestSsoAccountLogin(accountSession);
			}
			else
			{
				ProxyWindowModule.OpenMessageWindow (token);
				LockLoginButton(false);
			}
		});
	}

	private void OnTestLoginButtonClick()
	{
		GameSetting.SSO_SERVER = "http://t2.cilugame.com/h1";

		string accountName = View.NameInput_UIInput.value;
		string password = View.PasswordInput_UIInput.value;

		RequestSdkAccountLogin(accountName, password);
	}

	private void OnRegButtonClick()
	{
		Application.OpenURL("http://bbs.cilugame.com/member.php?mod=register");
	}

    private void OnUseGuideBtnClick()
    {
        Application.OpenURL("http://xl.tiancity.com/homepage/article/2016/05/05/44674.html");
    }

    private void OnDownloadBtnClick()
    {
		Application.OpenURL("http://xl.tiancity.com/homepage/");
	}

	private void RequestSdkAccountLogin(string accountName, string password)
	{
        if (GameSetting.Channel == AgencyPlatform.Channel_cilugame)
		{
			if (string.IsNullOrEmpty(accountName))
			{
				TipManager.AddTip("提示：请先输入账号");
				return;
			}
			else if (accountName.Length > 20)
			{
				TipManager.AddTip("提示：用户名最多20个字符");
				return;
			}

			if (GameSetting.DeviceLoginMode)
			{
				if (AppStringHelper.ValidateAccount(accountName) == false)
				{
					TipManager.AddTip("提示：用户名含有非法字符");
					return;
				}
			}
			else
			{
//				if (StringHelper.IsEmail(accountName) == false)
//				{
//					TipManager.AddTip("提示：用户名需要用邮箱格式");
//					return;
//				}
			}
		}
		if (!GameSetting.DeviceLoginMode)
		{
			if (string.IsNullOrEmpty(password))
			{
				
				TipManager.AddTip("提示：请先输入密码");
				return;
			}

		}

		LockLoginButton(true);

		if (GameSetting.DeviceLoginMode)
		{
			ServiceProviderManager.RequestSdkAccountLogin(accountName, GameSetting.AppId, GameSetting.CpId, AppGameVersion.SpVersionCode, delegate(AccountResponse response) {
				LoginManager.Instance.LoginId = accountName;
				HandleSdkAccountLoginResponse(response);
			});
		}
		else
		{
			ServiceProviderManager.RequestSdkAccountLogin(accountName, password, GameSetting.AppId, GameSetting.CpId, AppGameVersion.SpVersionCode, delegate(AccountResponse response) {
				LoginManager.Instance.LoginId = accountName;
				HandleSdkAccountLoginResponse(response);
			});
		}
	}

	private void HandleSdkAccountLoginResponse(AccountResponse response)
	{
		if (response != null && response.code == 0)
		{
			if (response.data == null)
			{
				ProxyWindowModule.OpenMessageWindow ("登陆数据返回为空");
				LockLoginButton(false);
			}
			else
			{
				RequestSsoAccountLogin(response.data);
			}
		}
		else if(response != null && response.code == 200)
		{
			OpenInputVerifyCode(response);
		}
		else
		{
			string msg = "服务器请求失败，请检查网络";
			if (response != null)
			{
				msg = response.msg;
			}
			ProxyWindowModule.OpenMessageWindow (msg);
			LockLoginButton(false);
		}
	}

	private void OpenInputVerifyCode(AccountResponse response)
	{
		ProxyWindowModule.OpenInputWindow(10,10,"激活","请输入10位激活码进行账号激活", "点击输入激活码","",delegate(string verifyCode) {
			ServiceProviderManager.RequestVerifyCode(response.data.sid, verifyCode, delegate(AccountResponse response2) {
				if (response2 != null && response2.code == 0)
				{
					if (response2.data == null)
					{
						ProxyWindowModule.OpenMessageWindow ("登陆数据返回为空");
						LockLoginButton(false);
					}
					else
					{
						RequestSsoAccountLogin(response2.data);
					}
				}
				else
				{
					string msg2 = "服务器请求失败，请检查网络";
					if (response2 != null)
					{
						msg2 = response2.msg;
					}
					ProxyWindowModule.OpenMessageWindow (msg2,"", delegate() {
						OpenInputVerifyCode(response);
					});
				}
			});
		},
		delegate() {
			LockLoginButton(false);
		});
	}

	private void RequestSsoAccountLogin(AccountSession session)
	{
		SPSdkManager.Instance.CallbackLoginSuccess(false, session.sid);

		string account = View.NameInput_UIInput.value;
		string password = View.PasswordInput_UIInput.value;
		
        PlayerPrefs.SetString(GameSetting.Channel + "_name", account );
        PlayerPrefs.SetString(GameSetting.Channel + "_psw", password );

		ProxyLoginModule.CloseTestSdk();
	}

    private void RequestQRCode()
    {
        JSTimer.Instance.CancelTimer("CheckQRCodeLogin");

        View.WaitScanGroup_Transform.gameObject.SetActive(true);
        View.OverTimeGroup_Transform.gameObject.SetActive(false);
        View.WaitEnsureGroup_Transform.gameObject.SetActive(false);

        ServiceProviderManager.RequestQRCodeSid(dto =>
        {
            if (View == null)
            {
                return;
            }

            if (dto != null)
            {
                if (dto.code == 0)
                {
                    _qrCodeSid = dto.msg;
                    UpdateQRCodeTexture();
                    _requestLoginNow = false;
                    JSTimer.Instance.SetupTimer("CheckQRCodeLogin", CheckQRCodeLogin, CheckQRCodeLoginInterval);
                }
                else
                {
                    ProxyWindowModule.OpenMessageWindow(dto.msg, null, RequestQRCode);
                }
            }
            else
            {
                RequestQRCode();
            }
        });
    }


    private void UpdateQRCodeTexture()
    {
        var tex = AntaresQRCodeUtil.Encode(QRCodeHelper.EncodeLoginQRCode(_qrCodeSid, AppGameVersion.SpVersionCode), View.QRTexture_UITexture.width);
//        Destroy(tex);
//        Resources.UnloadAsset(tex);
//        tex = null;
//        var tex = ZXingUtils.EncodeQR("坑爹", 256, ZXingUtils.ErrorCorrectionType.M);
//        var tex = ZXingUtils.EncodeQR(_qrCodeSid, View.QRTexture_UITexture.width, ZXingUtils.ErrorCorrectionType.L, "ISO-8859-1");
//        Debug.Log(ZXingUtils.DecodeQR(tex));
//        Debug.Log(AntaresQRCodeUtil.Decode(tex));
        View.QRTexture_UITexture.mainTexture = tex;
//        View.QRTexture_UITexture.mainTexture = null;
    }

    private void CheckQRCodeLogin()
    {
        if (_requestLoginNow)
        {
            return;
        }

        _requestLoginNow = true;

        ServiceProviderManager.RequestQRCodeLoginState(_qrCodeSid, dto =>
        {
            _requestLoginNow = false;

            if (View == null)
            {
                return;
            }

            if (dto == null)
            {
                return;
            }

            if (dto.code == 0)
            {
                JSTimer.Instance.CancelTimer("CheckQRCodeLogin");

	            if (!string.IsNullOrEmpty(dto.clientExtra))
	            {
		            WinGameSetting.Setup(WinGameSetting.WinGameSettingData.CreateFromBase64UrlSafeJson(dto.clientExtra),
			            () =>
			            {
							LoginManager.Instance.LoginId = dto.loginAccount.accountId.ToString();
							ServerManager.Instance.loginAccountDto = dto.loginAccount;
							RequestQRCodeSsoAccountLogin(dto.loginAccount.token);
						},
			            error =>
			            {
							ProxyWindowModule.OpenMessageWindow(error, null, RequestQRCode);
						});
	            }
	            else
	            {
					ProxyWindowModule.OpenMessageWindow("登陆信息不完整，请重试", null, RequestQRCode);
				}
            }
            else if (dto.code == 1101)
            {
                // 等待
            }
            else if (dto.code == 1102)
            {
                View.WaitScanGroup_Transform.gameObject.SetActive(false);
                View.WaitEnsureGroup_Transform.gameObject.SetActive(true);
            }
            else if (dto.code == 1100)
            {
                JSTimer.Instance.CancelTimer("CheckQRCodeLogin");
                View.OverTimeGroup_Transform.gameObject.SetActive(true);
                View.WaitScanGroup_Transform.gameObject.SetActive(true);
                View.WaitEnsureGroup_Transform.gameObject.SetActive(false);
            }
            else if (dto.code == 1104)
            {
                JSTimer.Instance.CancelTimer("CheckQRCodeLogin");
                ProxyWindowModule.OpenMessageWindow("账号登录会话(token)失效", null, RequestQRCode);
            }
            else
            {
                JSTimer.Instance.CancelTimer("CheckQRCodeLogin");
                ProxyWindowModule.OpenMessageWindow(dto.msg, dto.code.ToString(), RequestQRCode);
            }
        });
    }

    private void RequestQRCodeSsoAccountLogin(string sid)
    {
        SPSdkManager.Instance.CallbackLoginSuccess(false, sid);
        ProxyLoginModule.CloseTestSdk();
    }

protected override void OnDispose ()
	{
		base.OnDispose ();
    
        JSTimer.Instance.CancelTimer("CheckQRCodeLogin");
    }
    #endregion

    public void Open()
	{
		

        View.AccountInputGroup_Transform.gameObject.SetActive(!GameSetting.IsOriginWinPlatform);
		View.PasswordInputGroup.SetActive(GameSetting.GMMode && !GameSetting.IsOriginWinPlatform);
		View.ExternalLoginGroup.SetActive(GameSetting.GMMode && !GameSetting.IsOriginWinPlatform);
		View.LocalLoginGroup.SetActive(!GameSetting.GMMode && !GameSetting.IsOriginWinPlatform);

        View.QRCodeLoginGroup_Transform.gameObject.SetActive(GameSetting.IsOriginWinPlatform);

		View.TestLoginGroup.SetActive(false);
		View.RegGroup.SetActive(false);

        _view.NameInput_UIInput.value = PlayerPrefs.GetString(GameSetting.Channel + "_name");
        _view.PasswordInput_UIInput.value = PlayerPrefs.GetString(GameSetting.Channel + "_psw");

	    if (GameSetting.IsOriginWinPlatform)
	    {
	        View.Describe_UILabel.text = string.Format("[{0}]在[{1}]{2}[-]移动设备端登录\n界面点击    按钮扫码登录[-]", ColorConstantV3.Color_White_Str, ColorConstantV3.Color_QRCodeYellow_Str, GameSetting.GameName);
	        RequestQRCode();
	    }
	}
}

