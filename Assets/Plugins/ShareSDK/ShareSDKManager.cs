using System;
using System.Collections;
using System.Collections.Generic;
using cn.sharesdk.unity3d;
using UnityEngine;

public class ShareSDKManager : MonoBehaviour
{
	public static readonly Dictionary<PlatformType, string> SupportedPlatformTypeDict = new Dictionary
		<PlatformType, string>
	{
		{PlatformType.WeChat, "com.tencent.mm"},
		{PlatformType.WeChatMoments, "com.tencent.mm"},
		{PlatformType.Unknown, ""}
	};


	protected static ShareSDKManager _instance;

	private Dictionary<int, Action<int, ResponseState, PlatformType, Hashtable>> _shareResultDict =
		new Dictionary<int, Action<int, ResponseState, PlatformType, Hashtable>>();

	private ShareSDK _shareSDK;

	public static ShareSDKManager Instance
	{
		get
		{
			if (_instance == null)
			{
				var go = new GameObject(typeof(ShareSDKManager).Name);
				go.AddComponent<ShareSDKManager>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Init();
		SetPlatformConfig();
	}


	/// <summary>
	///     初始化 ShareSDK
	/// </summary>
	private void Init()
	{
		_instance = this;
		DontDestroyOnLoad(gameObject);

		// 注册监听回调
		_shareSDK = GetComponent<ShareSDK>();
		if (_shareSDK == null)
		{
			_shareSDK = gameObject.AddComponent<ShareSDK>();
		}
		_shareSDK.shareHandler += ShareResult;
	}

	private void OnDestroy()
	{
		_instance = null;

		_shareSDK.shareHandler -= ShareResult;
	}


	/// <summary>
	///     设置平台配置，其实只有 ios 用到
	/// </summary>
	private void SetPlatformConfig()
	{
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
        _shareSDK.InitSDK(ShareSDKConfig.GetAppKey());
        _shareSDK.SetPlatformConfig(ShareSDKConfig.GetConfigEx());
#endif
	}


	public static ShareContent CreateContent(string title, string content, string image, string url,
		PlatformType platform = PlatformType.Unknown)
	{
		var table = new ShareContent();

		table.SetTitle(title);
		table.SetText(content);
		if (!image.Contains("http"))
		{
			table.SetImagePath(image);
		}
		else
		{
			table.SetImageUrl(image);
		}
		if (!string.IsNullOrEmpty(url))
		{
			table.SetUrl(url);

			table.SetShareType(ContentType.Webpage);
		}
		else
		{
			table.SetShareType(ContentType.Image);
		}

		switch (platform)
		{
			case PlatformType.WeChatMoments:
				{
					table.SetTitle(content);
					break;
				}
		}

		return table;
	}


	public static ShareContent CreateContent(string image, PlatformType platform = PlatformType.Unknown)
	{
		return CreateContent("标题", "内容", image, "www.baidu.com", platform);
	}

	public int ShareContent(Action<int, ResponseState, PlatformType, Hashtable> callback,
		ShareContent content, PlatformType platform = PlatformType.Unknown)
	{
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
		var reqId = _shareSDK.ShareContent(platform, content);
		_shareResultDict[reqId] = callback;
		return reqId;
#endif
		return 0;
	}

	public void ShareResult(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (_shareResultDict.ContainsKey(reqID))
		{
			var callback = _shareResultDict[reqID];
			RemoveShareResultCallback(reqID);
			if (callback != null)
			{
				callback(reqID, state, type, result);
			}
		}
	}


	public bool RemoveShareResultCallback(int reqID)
	{
		if (_shareResultDict.ContainsKey(reqID))
		{
			_shareResultDict.Remove(reqID);
			return true;
		}
		return false;
	}


	/// <summary>
	///     检查分享系统是否设置了支持该平台
	/// </summary>
	/// <param name="platform"></param>
	/// <returns></returns>
	public bool CheckSupportedPlatfromType(PlatformType platform)
	{
		if (platform == PlatformType.Unknown)
		{
			return true;
		}

		return SupportedPlatformTypeDict.ContainsKey(platform);
	}


	/// <summary>
	///     判断客户端是否安装了
	/// </summary>
	/// <param name="platform"></param>
	/// <returns></returns>
	public bool IsClientInstalled(PlatformType platform)
	{
		//        return true;
		if (!CheckSupportedPlatfromType(platform))
		{
			return false;
		}

		if (platform == PlatformType.Unknown)
		{
			return true;
		}

#if UNITY_EDITOR || UNITY_STANDALONE
		return false;
#else
		return _shareSDK.IsClientValid(platform);
#endif
	}
}