// **********************************************************************
// Copyright (c) 2015 cilugame. All rights reserved.
// File     : AgreementController.cs
// Author   : senkay <senkay@126.com>
// Created  : 05/25/2015 
// Porpuse  : 
// **********************************************************************
//
using UnityEngine;
using System;
using System.Text;

public class AgreementController : MonoViewController<AgreementView>
{

	private AgreementInfo _info;

	private int _currentPage = 0;
	private int _maxPage = 0;

	#region IViewController
	/// <summary>
	/// 从DataModel中取得相关数据对界面进行初始化
	/// </summary>

	/// <summary>
	/// Registers the event.
	/// DateModel中的监听和界面控件的事件绑定,这个方法将在InitView中调用
	/// </summary>
	protected override void RegistCustomEvent ()
	{
		EventDelegate.Set (View.CloseButton_UIButton.onClick, OnCloseButtonClick);
	    EventDelegate.Set(View.SubmitBtn_UIButton.onClick, OnCloseButtonClick);
		EventDelegate.Set (View.LeftArrowSprite_UIButton.onClick, OnLeftButtonClick);
		EventDelegate.Set (View.RightArrowSprite_UIButton.onClick, OnRightButtonClick);
	}
	
	protected override void OnDispose ()
	{
		PlayerPrefsExt.SetBool("PassAgreement", true);
	}
	#endregion

	private event Action _callback;
	public void Open(Action callback)
	{
		_callback = callback;

		if (_info == null)
		{
			var agreementDataAsset = Resources.Load("Setting/AgreementData") as TextAsset;
			if (agreementDataAsset != null)
			{
				string json = Encoding.UTF8.GetString(agreementDataAsset.bytes);
				_info = JsHelper.ToObject<AgreementInfo> (json);
				_maxPage = _info.infoList.Count;
				UpdateInfo (0);
			}
			else
			{
				//这里是兼容旧的包文件处理
                AssetPipeline.ResourcePoolManager.Instance.LoadConfig("AgreementData", (asset) => {
					if (View != null && asset != null) {
						TextAsset textAsset = asset as TextAsset;
						if (textAsset != null) {
							_info = JsHelper.ToObject<AgreementInfo> (textAsset.text);
							_maxPage = _info.infoList.Count;
							UpdateInfo (0);
						}
					}
				});				
			}
		}
		else
		{
			UpdateInfo(0);
		}
	}

	private void OnCloseButtonClick()
	{
        TalkingDataHelper.OnEventSetp("Agreement", "Close");
		ProxyLoginModule.CloseAgreement();

		if (_callback != null)
		{
			_callback();
			_callback = null;
		}
	}

	private void OnLeftButtonClick()
	{
		_currentPage--;
		if (_currentPage < 0)
		{
			_currentPage = 0;
		}
		UpdateInfo(_currentPage);
	}

	private void OnRightButtonClick()
	{
		_currentPage++;
		if (_currentPage >= _maxPage)
		{
			_currentPage = _maxPage-1;
		}
		UpdateInfo(_currentPage);
	}

	private void UpdateInfo(int page)
	{
		if (_info == null) return;

		_currentPage = page;
		View.PageLabel_UILabel.text = string.Format("{0}/{1}", (_currentPage+1), _maxPage);

		string info = _info.infoList[page];
		info = info.Replace("游戏名称", GameSetting.GameName);
		View.ContentLabel_UILabel.text = string.Format("[502E10]{0}[-]", info);
        View.scrollView_UIScrollView.ResetPosition();
	}
}

