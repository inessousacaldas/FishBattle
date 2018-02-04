using System;
using UnityEngine;
using System.Collections;

public class QRCodeScanViewController : MonoViewController<QRCodeScanView>
{
	private Action _closeCallback;
	private static Action _internalCloseCallback;
	public const float CloseDelay = 0.1f;

	private WebCamTexture _webCamTexture;
	public const float DecodeInterval = 1f;
	private float _lastDecodeTime;
    /// <summary>
    /// 检测是否正在等待服务器回应
    /// </summary>
    private bool _waitServerResponse;

    /// <summary>
    /// 用于判断摄像机是否初始化成功的值，据说有可能会出现这个问题
    /// </summary>
	public const int WebCamTextureInitSuccessSize = 100;
	private bool _firstInit;
	private int _viewSize;

	public void SetData(Action closeCallback)
	{
		_closeCallback = closeCallback;
		_webCamTexture = WebCamTextureHelper.GetNewWebCamTexture(800, 480);

		string error = null;
		do
		{
			if (_webCamTexture == null)
			{
				error = "找不到可用的后置摄像头";
				break;
			}

#if UNITY_IPHONE
			if (!_webCamTexture.isReadable)
			{
				error = "摄像头被禁止使用";
				break;
			}
#endif
		} while (false);


		if (string.IsNullOrEmpty(error))
		{
			
		}
		else
		{
			ProxyWindowModule.OpenMessageWindow(error, "获取摄像头错误");
			OnReturnBtnClick();
		}
	}


	protected override void AfterInitView ()
	{
		if (_webCamTexture != null)
		{
			_webCamTexture.Play();
		}
		View.ScanTexture_UITexture.mainTexture = _webCamTexture;
		_firstInit = true;
        _waitServerResponse = false;
        View.TipLabel_UILabel.text = string.Format("请把取景框对准{0}电脑微端二维码", GameSetting.GameName);
	}

    protected override void RegistCustomEvent ()
    {
        base.RegistCustomEvent();
        EventDelegate.Set(View.ReturnBtn_UIButton.onClick, OnReturnBtnClick);
    }


	protected override void OnDispose()
	{
		View.ScanTexture_UITexture.mainTexture = null;
		if (_webCamTexture != null)
		{
			_webCamTexture.Stop();
			Destroy(_webCamTexture);
			_webCamTexture = null;
		}
        _waitServerResponse = false;

		base.OnDispose();
	}

	/// <summary>
	/// 这里就不交给CoolDown来处理了，方便一点
	/// </summary>
	private void Update()
	{
		if (_webCamTexture != null && _webCamTexture.didUpdateThisFrame && _webCamTexture.width > WebCamTextureInitSuccessSize && _webCamTexture.height > WebCamTextureInitSuccessSize)
		{
			// 必须每次都计算
			View.ScanTexture_UITexture.transform.localScale = new Vector3(1, _webCamTexture.videoVerticallyMirrored ? -1f : 1f, 1);
			View.ScanTexture_UITexture.transform.rotation = Quaternion.AngleAxis(_webCamTexture.videoRotationAngle, Vector3.forward);

			if (_firstInit)
			{
				_firstInit = false;
				var isBasedOnWidth = 1f * Screen.width / Screen.height >
									 1f * _webCamTexture.width / _webCamTexture.height;

				View.ScanTexture_UITexture.keepAspectRatio = isBasedOnWidth
					? UIWidget.AspectRatioSource.BasedOnWidth
					: UIWidget.AspectRatioSource.BasedOnHeight;
				View.ScanTexture_UITexture.aspectRatio = 1f * _webCamTexture.width / _webCamTexture.height;
				View.ScanTexture_UITexture.ResetAndUpdateAnchors();

				var scale = isBasedOnWidth
					? 1f * View.ScanTexture_UITexture.width / _webCamTexture.width
					: 1f * View.ScanTexture_UITexture.height / _webCamTexture.height;
				// 正方形
				_viewSize = (int)(View.ViewTexture_UIWidget.width / scale);
			}
			// 保证初始化完毕
            if (Time.realtimeSinceStartup - _lastDecodeTime > DecodeInterval && ! _waitServerResponse)
			{
				var result = AntaresQRCodeUtil.Decode(_webCamTexture, _viewSize, _viewSize);
				// 放在后面可以把解密的时间也算上去
				_lastDecodeTime = Time.realtimeSinceStartup;

				if (result != null && !string.IsNullOrEmpty(result.Text))
				{
					// 登陆
					var loginCode = QRCodeHelper.Decode<LoginQRCode>(result.Text);
					if (loginCode != null)
					{
                        // 非Release不检查版本，方便登陆
                        if (GameSetting.Release && (loginCode.SpVersionCode / 1000) != (AppGameVersion.SpVersionCode / 1000))
						{
							_internalCloseCallback = () =>
							{
								ProxyWindowModule.OpenMessageWindow("扫码端和PC端版本不符");
							};

							OnReturnBtnClick();
							return;
						}

                        _waitServerResponse = true;
                        ServiceProviderManager.RequestQRCodeLogin(loginCode.Sid, ServerManager.Instance.loginAccountDto.token, dto =>
                        {
                            _waitServerResponse = false;

                            if (View == null || dto == null)
                            {
                                return;
                            }

							if (dto.code == 0)
							{
								var callback = _closeCallback;
								_closeCallback = null;

								_internalCloseCallback = () =>
								{
									ProxyQRCodeModule.OpenQRCodeEnsureView(callback, loginCode.Sid);
								};

								OnReturnBtnClick();
							}
							else if (dto.code == 1103)
							{
								_internalCloseCallback = () =>
								{
									ProxyWindowModule.OpenMessageWindow("二维码会话(sid)失效");
								};

								OnReturnBtnClick();
							}
							else if (dto.code == 1104)
							{
								_internalCloseCallback = () =>
								{
									ProxyWindowModule.OpenMessageWindow("账号登录会话(token)失效");
								};

								OnReturnBtnClick();
							}
							else
							{
								_internalCloseCallback = () =>
								{
									ProxyWindowModule.OpenMessageWindow(string.Format("扫码失败，请把取景框对准{0}电脑微端二维码进行扫描，{0}电脑微端可在官网下载", GameSetting.GameName), "扫描结果", null, UIWidget.Pivot.Left, "重新扫码");
								};

								OnReturnBtnClick();
							}
						});
						return;
					}

					// 商品
					var productCode = QRCodeHelper.Decode<ProductQRCode>(result.Text);
					if (productCode != null)
					{
						_internalCloseCallback = () =>
						{
							ServiceProviderManager.RequestQRCodeScanPaySuccess(productCode.OrderDto.orderId);

							PayManager.Instance.ChargeByOrderJsonDto(productCode.ItemDto, productCode.Quantity, productCode.OrderDto);

							ProxyWindowModule.OpenMessageWindow("扫码成功，请耐心等待服务器返回订单信息。");
						};

						OnReturnBtnClick();
						return;
					}

					GameDebuger.LogError(result.Text);
					_internalCloseCallback = () =>
					{
						ProxyWindowModule.OpenMessageWindow("该二维码无效");
					};

					OnReturnBtnClick();
				}
			}
		}
	}


	private void OnReturnBtnClick()
	{
		QRCodeScanDelayCallback(_closeCallback);
		_closeCallback = null;
		ProxyQRCodeModule.CloseQRCodeScanView();
	}


	private static void QRCodeScanDelayCallback(Action callback)
	{
		JSTimer.Instance.SetupCoolDown("QRCodeScanDelayCallback", CloseDelay, null, () =>
		{
			if (callback != null)
			{
				callback();
			}

			if (_internalCloseCallback != null)
			{
				_internalCloseCallback();
				_internalCloseCallback = null;
			}
		});
	}
}
