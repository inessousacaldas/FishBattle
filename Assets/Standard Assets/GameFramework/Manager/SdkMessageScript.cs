using UnityEngine;
using System;
using LITJson;

public class SdkMessageScript : MonoBehaviour
{
	public static SdkMessageScript Setup()
	{
		SdkMessageScript sdkMessageScript = null;

		GameObject go = GameObject.Find ("_SdkMessageHandler");
		if (go == null)
		{
			go = new GameObject("_SdkMessageHandler");
			DontDestroyOnLoad(go);
			sdkMessageScript = go.AddComponent<SdkMessageScript>();
		}
		else
		{
			sdkMessageScript = go.GetComponent<SdkMessageScript>();
		}

		return sdkMessageScript;
	}

	//sdk的回调监听处理
	public Action<string> OnSdkCallbackInfo;

    public Action<string> OnCYSdkCallbackInfo;

    #region 电量 and 网络状况
    public void OnPower(string power)
    {
		string json = "{'type':'power','code':0,'data':'"+power+"'}";

		Debug.Log("OnPower json=" + json);

		if (OnSdkCallbackInfo != null)
		{
			OnSdkCallbackInfo(json);
		}
    }
    #endregion

	#region CLSDK callback
	/// <summary>
	/// Raises the XG register result event.
	/// </summary>
	/// <param name="flag">Flag.  0 success   1 fail</param>
	public void OnXGRegisterResult(string flag)
	{
		string json = "{'type':'XGRegisterResult','code':0,'data':'"+flag+"'}";

		Debug.Log("OnXGRegisterResult json=" + json);

		if (OnSdkCallbackInfo != null)
		{
			OnSdkCallbackInfo(json);
		}
	}
    #endregion

    #region CLSDK callback
    /// <summary>
    /// Raises the XG register result event.
    /// </summary>
    /// <param name="flag">Flag.  0 success   1 fail</param>
    public void OnXGRegisterWithAccountResult(string flag)
    {
		string json = "{'type':'XGRegisterWithAccountResult','code':0,'data':'"+flag+"'}";

		Debug.Log("OnXGRegisterWithAccountResult json=" + json);

		if (OnSdkCallbackInfo != null)
		{
			OnSdkCallbackInfo(json);
		}
    }
    #endregion

	#region CLSDK callback
	public void OnSdkCallback(string json)
	{
		Debug.Log("OnSdkCallback json=" + json);

		if (OnSdkCallbackInfo != null)
		{
			OnSdkCallbackInfo(json);
		}
	}
    #endregion

    #region CLSDK callback
    public void onResult(string json)
    {
      //  Debug.Log("onResult json=" + json);

        if (OnCYSdkCallbackInfo != null)
        {
            OnCYSdkCallbackInfo(json);
        }
    }
    #endregion
}