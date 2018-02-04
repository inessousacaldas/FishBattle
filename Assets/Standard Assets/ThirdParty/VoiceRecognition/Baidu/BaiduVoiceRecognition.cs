// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  BaiduVoiceRecognition.cs
// Author   : willson
// Created  : 2014/11/4 
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaiduVoiceRecognition : MonoBehaviour
{
    private static string serverURL = "https://vop.baidu.com/server_api";
    private static string token; // = "24.85e67dfebb06c06c1cf41626cb21cc71.2592000.1417863541.282335-4534789";

    // 用户id，推荐使用mac地址/手机IMEI等类似参数
    //private static string cuid = PlayerModel.Instance.GetPlayerId();

    private static BaiduVoiceRecognition _instance;

    private Action<string> _GetVoiceTextAction;

    public static BaiduVoiceRecognition Instance
    {
        get
        {
            CreateInstance();
            return _instance;
        }
    }

    private static void CreateInstance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType(typeof (BaiduVoiceRecognition)) as BaiduVoiceRecognition;

            if (_instance == null)
            {
                GameObject go = new GameObject("_BaiduVoiceRecognition");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<BaiduVoiceRecognition>();
            }
        }
    }

    public void getToken(string apiKey, string secretKey)
    {
#if !UNITY_EDITOR
        string getTokenURL = "https://openapi.baidu.com/oauth/2.0/token?grant_type=client_credentials" +
                             "&client_id=" + apiKey + "&client_secret=" + secretKey;

        Debug.Log(getTokenURL);

        StartCoroutine(_Sent(getTokenURL, SetToken));
#endif
    }

    private void SetToken(string text)
    {
        BaiduToken baiduToken = null;
        if (!string.IsNullOrEmpty(text))
        {
            Hashtable tempDic = MiniJSON.jsonDecode(text) as Hashtable;
            if (tempDic.Count > 0)
            {
                JsonBaiduTokenParser info = new JsonBaiduTokenParser();
                baiduToken = info.DeserializeJson_BaiduToken(tempDic);
            }
        }

        if (baiduToken != null)
        {
            token = baiduToken.access_token;
            Debug.Log(token);
        }
        else
        {
            token = "";
        }
    }

    /*
	public void GetVoiceText(AudioClip clip,System.Action<string> action)
	{
		GetVoiceText(SavWav.ToWav(clip),action);
	}
	*/

    /**
	 *	BaiduVoiceRecognition.GetVoiceText(SavWav.ToWav(AudioSource.clip),VoiceRecognitionBack);
	 * */

    public void GetVoiceText(long playerId, byte[] samples, int dataLength, Action<string> action)
    {
        string url = serverURL + "?cuid=" + playerId + "&token=" + token;

        Debug.Log("GetVoiceText " + url);

        _GetVoiceTextAction = action;
        StartCoroutine(_Post(url, samples, dataLength, GetVoiceTextHandle));
    }

    public void StopGetVoiceText()
    {
        StopCoroutine("_Post");
    }

    private void GetVoiceTextHandle(string text)
    {
        Debug.Log("GetVoiceTextHandle " + text);

        string beginStr = "\"result\":[\"";
        int begin = text.IndexOf(beginStr);
        if (begin != -1)
        {
            begin = begin + beginStr.Length;
            int end = text.IndexOf("\"]") - begin;

            if (begin != -1 && end != -1)
            {
                text = text.Substring(begin, end);
#if UNITY_EDITOR || UNITY_IPHONE
                if (!string.IsNullOrEmpty(text))
                {
                    text = BaiduEncoding.NormalU2C(text);
                    Debug.Log("BaiduEncoding " + text);
                }
#endif
            }

            if (_GetVoiceTextAction != null)
                _GetVoiceTextAction(text);
        }
    }

    private IEnumerator _Post(string url, byte[] data, int dataLength, Action<string> action)
    {
        var postHeader = new Dictionary<string, string>();

        postHeader.Add("Content-Type", "audio/amr; rate=8000");
//		postHeader.Add("Content-Type", "audio/wav; rate=8000");
        postHeader.Add("Content-Length", dataLength.ToString());
        postHeader.Add("Connection", "close");

//		#if UNITY_IPHONE
//			WWW sendWWW = new WWW( url, data, postHeader);
//			Debug.Log("_Post " + url);
//		#else
//			byte[] amrData = new byte[dataLength];
//			Array.Copy(data, amrData, dataLength);
        WWW sendWWW = new WWW(url, data, postHeader);
        Debug.Log("_Post " + url + "dataLength=" + dataLength + "dataLen=" + data.Length);
//		#endif

        yield return sendWWW;

        Debug.Log("Return " + url);

        if (sendWWW.isDone && sendWWW.error == null)
        {
            Debug.Log("isDone " + sendWWW.text);
            if (action != null)
                action(sendWWW.text);
        }
        else
        {
            Debug.Log("Error: " + sendWWW.error);
            if (action != null)
                action("Error: " + sendWWW.error);
        }
    }

    private IEnumerator _Sent(string url, Action<string> action)
    {
        WWW sendWWW = new WWW(url);
        yield return sendWWW;

        if (sendWWW.isDone && sendWWW.error == null)
        {
            if (action != null)
                action(sendWWW.text);
        }
        else
        {
            Debug.Log("Error: " + sendWWW.error);
        }
    }
}