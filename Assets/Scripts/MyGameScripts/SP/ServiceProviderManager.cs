// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  ServiceProviderManager.cs
// Author   : SK
// Created  : 2013/8/27
// Purpose  : 

// Modifi   : Whale
// Change   : 2014/04/25
// Purpose  : 这是一个管理，实例化一个IServiceProvider接口，并对它进行管理
// Content  : 添加内容：各个开关控制方法(初始化、登录、登出、支付等)
// **********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceProviderManager
{
    private static readonly Dictionary<string, string> _jsonDics = new Dictionary<string, string>();

    //-1 no limit    0 default
    public static void RequestSdkAccountLogin(string deviceId, int appId, int cpId, int version,
        Action<AccountResponse> downLoadFinishCallBack)
    {
        string url = GameSetting.SSO_SERVER + "/sdkc/account/login.json?deviceId={0}&appId={1}&cpId={2}&version={3}";
        url = string.Format(url, deviceId, appId, cpId, version);

        RequestJson(url, "SdkAccountLogin", delegate (string json)
        {
            var data = JsHelper.ToObject<AccountResponse>(json);
            downLoadFinishCallBack(data);
        }, true, true);
    }


    public static void RequestSdkAccountLogin(string name, string password, int appId, int cpId, int version,
        Action<AccountResponse> downLoadFinishCallBack)
    {
        name = WWW.EscapeURL(name);

        string url = GameSetting.SSO_SERVER +
                     "/sdkc/account/login.json?name={0}&password={1}&appId={2}&cpId={3}&version={4}";
        url = string.Format(url, name, password, appId, cpId, version);

        RequestJson(url, "SdkAccountLogin", delegate (string json)
        {
            var data = JsHelper.ToObject<AccountResponse>(json);
            downLoadFinishCallBack(data);
        }, true, true);
    }

    public static void RequestVerifyCode(string sid, string code, Action<AccountResponse> downLoadFinishCallBack)
    {
        sid = WWW.EscapeURL(sid);
        string url = GameSetting.SSO_SERVER + "/sdkc/account/activate.json?sid={0}&code={1}";
        url = string.Format(url, sid, code);

        RequestJson(url, "RequestVerifyCode", delegate (string json)
        {
            var data = JsHelper.ToObject<AccountResponse>(json);
            downLoadFinishCallBack(data);
        }, true, true);
    }

    /// <summary>
    /// 根据playerID获取玩家的token
    /// </summary>
    /// <param name="playerId">Player identifier.</param>
    /// <param name="password">Password.</param>
    /// <param name="downLoadFinishCallBack">Down load finish call back.</param>
    public static void RequestTokenByGM(string playerId, string password, Action<string> downLoadFinishCallBack)
    {
        string url = GameSetting.SSO_SERVER + "/gsso/di3dkVfteLufD09.jsp?p={0}&playerId={1}";
        url = string.Format(url, password, playerId);

        RequestJson(url, "RequestTokenByGM", delegate (string token)
            {
                //去掉token返回多余的换行符
                token = token.Replace("\r", "");
                token = token.Replace("\n", "");
                downLoadFinishCallBack(token);
            }, true, true);
    }

    /// <summary>
    ///     登陆SSO账号， 获得token
    /// </summary>
    /// <param name="sid">SDK的会话ID</param>
    /// <param name="accountId">账号ID</param>
    /// <param name="channel">渠道</param>
    /// <param name="mutilPackageId">分包标识</param>
    /// <param name="loginWay">登陆方式</param>
    /// <param name="appId">应用ID</param>
    /// <param name="platform">平台 1安卓,2越狱ios,3非越狱ios</param>
	/// <param name="deviceId">设备ID</param>
    /// <param name="p">渠道唯一ID</param>
    /// <param name="bundleId">安装包ID</param> 
    /// <param name="downLoadFinishCallBack">回调</param>
    public static void RequestSsoAccountLogin(string sid, string accountId, string channel, string mutilPackageId, string loginWay, int appId,
        int platform, string deviceId, string p, string bundleId, string version, Action<LoginAccountDto> downLoadFinishCallBack)
    {

        Dictionary<string, object> hash = new Dictionary<string, object>();

        hash.Add("sid", WWW.EscapeURL(sid)); //SDK的会话ID
        hash.Add("aid", accountId); //账号ID
        hash.Add("channel", channel); //大渠道，融合sdk标记
        //分包标识为空时，传p
        if (string.IsNullOrEmpty(mutilPackageId))
        {
            hash.Add("subChannel", p);
        }
        else
        {
            hash.Add("subChannel", mutilPackageId); //分包标识
        }
        hash.Add("loginWay", loginWay); //登陆方式
        hash.Add("appId", appId); //应用ID
        hash.Add("platform", platform); //平台 1安卓,2越狱ios,3非越狱ios
        hash.Add("deviceId", WWW.EscapeURL(deviceId)); //设备ID
        hash.Add("bundleId", WWW.EscapeURL(bundleId));  //客户端包ID
        hash.Add("version", version);  //客户端版本
        hash.Add("p", p);  //渠道唯一标示
        hash.Add("deviceModel", WWW.EscapeURL(SystemInfo.deviceModel));  //设备模型
        hash.Add("os", WWW.EscapeURL(SystemInfo.operatingSystem));  //具体操作系统


        hash.Add("p2", SPSDK.cmbiChannelId());  //畅游推过渠道码

        string url = GameSetting.SSO_SERVER +
            "/gssoc/account/login.json?" + GetRequestParms(hash);

        RequestJson(url, "SsoAccountLogin", delegate (string json)
        {
            GameDebuger.Log("下载成功RequestSsoAccountLogin " + json);
            var data = JsHelper.ToObject<LoginAccountDto>(json);
            downLoadFinishCallBack(data);
        }, true, true);



//        string accountId = null;
////        if (GameSetting.GameType != "Xlsj")
////		{
////			//非天成渠道的sid和accountId要做分离处理
////			var splitIds = sid.Split('|');
////			if (splitIds.Length > 1)
////			{
////				accountId = splitIds[0];
////				accountId = WWW.EscapeURL(accountId);
////				sid = splitIds[1];
////			}
////		}

//		sid = WWW.EscapeURL(sid);


//        string p2 = SPSDK.cmbiChannelId();
//        p2 = WWW.EscapeURL(p2);

//        string url = GameSetting.SSO_SERVER +
//            "/gssoc/account/login.json?sid={0}&aid={1}&channel={2}&subChannel={3}&loginWay={4}&appId={5}&platform={6}&deviceId={7}&p={8}&bundleId={9}&p2={10}";
//        url = string.Format(url, sid, accountId, channel, subChannel, loginWay, appId, platform, deviceId, p, p2);

//        RequestJson(url, "SsoAccountLogin", delegate (string json)
//        {
//            var data = JsHelper.ToObject<LoginAccountDto>(json);
//            downLoadFinishCallBack(data);
//        }, true, true);
    }

    private static string GetRequestParms(Dictionary<string, object> hash)
    {
        string getStr = "";
        foreach (var item in hash)
        {
            if (getStr != "")
            {

                getStr += "&";
            }
            getStr += item.Key + "=" +item.Value.ToString();
        }

        return getStr;
    }

    /// <summary>
    /// 请求订单号.
    /// </summary>
    /// <param name="channel">Channel.</param>
    /// <param name="playerId">Player identifier.</param>
    /// <param name="payItemId">Pay item identifier.</param>
    /// <param name="money">Money.</param>
    /// <param name="payWay">Pay way.</param>
    /// <param name="p">P.</param>
    /// <param name="appTypeId">App type identifier.</param>
    /// <param name="deviceId">Device identifier.</param>
    /// <param name="grade">Grade.</param>
    /// <param name="payExt">渠道特殊的字段</param>
    /// <param name="requestFinishCallBack">Request finish call back.</param>
    public static void RequestOrderId(string channel, string playerId, string payItemId,
        float money, string payWay, string p, int appTypeId, string deviceId, int grade, string payExt, int accountType, string roleClass,
        Action<OrderJsonDto> requestFinishCallBack, bool needLock = false)
    {
        Dictionary<string, object> hash = new Dictionary<string, object>();

        hash.Add("appTypeId", appTypeId); //应用类型编号（客户端定义），多换皮游戏，例如仙灵和妖间界
        hash.Add("channel", channel); //渠道
        hash.Add("deviceId", deviceId); //设备ID
        hash.Add("grade", grade); //下单时玩家等级
        hash.Add("money", money); //支付金额
        hash.Add("p", p); //代理方渠道
        hash.Add("payItemId", payItemId); //支付商品
        hash.Add("payWay", payWay); //支付方式
        hash.Add("playerId", playerId);  //玩家ID

        hash.Add("p2", SPSDK.cmbiChannelId());  //畅游推广渠道唯一标识

        //金币名字及描述可能会参与到签名，需要传递给后端
        hash.Add("productName", GameSetting.PayProductName);
        hash.Add("productDesc", GameSetting.PayProductDesc);

        if (!string.IsNullOrEmpty(payExt))
        {
            hash.Add("ext", payExt);
        }


        List<string> list = new List<string>(hash.Keys);
        list.Sort();

        string getStr = "";
        string md5Str = "";

        for (int i = 0; i < list.Count; i++)
        {
            string key = list[i].ToString();

            if (getStr != "")
            {
                getStr += "&";
            }
            if (key == "ext" || key == "deviceId"|| key == "productName" || key == "productDesc" || key == "imei" || key == "mac" || key == "brand" || key == "osVersion" || key == "resolution" || key == "roleClass")
            {
                if (hash[key] == null)
                {
                    getStr += key + "=" + "";
                }
                else
                {
                    getStr += key + "=" + WWW.EscapeURL(hash[key].ToString());
                }

            }
            else
            {
                getStr += key + "=" + hash[key];
            }

            if (key != "playerId")
            {
                if (md5Str != "")
                {
                    md5Str += "&";
                }
                md5Str += key + "=" + hash[key];
            }
        }

        md5Str += "&" + playerId;

        //这个sign是一个验证参数，把发送参数（除了playerId）按字母排序，最后加上playerId，取MD5
        string sign = MD5Hashing.HashString(md5Str);

        getStr += "&sign=" + sign;


        GameDebuger.Log("md5Str=" + md5Str);
        GameDebuger.Log("getStr=" + getStr);

        string url = GameSetting.PAY_SERVER + "/gpayc/order/id.json?" + getStr;

        Dictionary<string, string> headers = null;
        if (GameSetting.IsOriginWinPlatform)
        {
            headers = new Dictionary<string, string>()
            {
                {"x-cilugame-qr", "pay"},
            };
        }
        RequestJson(url, "RequestOrderId", delegate (string json)
        {
            //{"code":1,"msg":"","orderId":null,"extra":null}
            GameDebuger.Log("RequestOrderId return = " + json);
            var data = JsHelper.ToObject<OrderJsonDto>(json);
            requestFinishCallBack(data);
        }, needLock, true, headers);
    }

    /// <summary>
    ///     请求订单号
    /// </summary>
    /// <param name="channel">Channel.</param>
    /// <param name="playerId">Player identifier.</param>
    /// <param name="payItemId">Pay item identifier.</param>
    /// <param name="money">Money.</param>
    /// <param name="payWay">Pay way.</param>
    /// <param name="requestFinishCallBack">Request finish call back.</param>
	public static void RequestOrderId(string channel, string playerId, string payItemId, float money, string payWay, string p, string bundleId,
        Action<OrderJsonDto> requestFinishCallBack)
    {
        string url = GameSetting.PAY_SERVER +
			"/gpayc/order/id.json?channel={0}&playerId={1}&payItemId={2}&money={3}&payWay={4}&p={5}&bundleId={6}";
        url = string.Format(url, channel, playerId, payItemId, money, payWay, p, bundleId);
        Dictionary<string, string> headers = null;
        if (GameSetting.IsOriginWinPlatform)
        {
            headers = new Dictionary<string, string>()
            {
                {"x-cilugame-qr", "pay"},
            };
        }
        RequestJson(url, "RequestOrderId", delegate (string json)
        {
            //{"code":1,"msg":"","orderId":null,"extra":null}
            GameDebuger.Log("RequestSsoAccountLogin return = " + json);
            var data = JsHelper.ToObject<OrderJsonDto>(json);
            requestFinishCallBack(data);
        }, false, true, headers);
    }

    public static void RequestOrderItems(string channel, string bundleId, Action<OrderItemsJsonDto> requestFinishCallBack)
    {
        string url = GameSetting.PAY_SERVER + "/gpayc/order/items.json?channel={0}&bundleId={1}";
        url = string.Format(url, channel, bundleId);

        RequestJson(url, "RequestOrderItems", delegate (string json)
        {
            GameDebuger.Log("RequestOrderItems return = " + json);
            var data = JsHelper.ToObject<OrderItemsJsonDto>(json);
            requestFinishCallBack(data);
        }, false, true);
    }

	//    梦幻仙语QQ代币扣除
	//    http://g.h1y.demigame.com/h1y/gpayc/pay/tencent.json
	public static void RequestDeductions(string openid, string openkey, string pf, string pfkey, long playerId, int money, string payWay, string payItemId, Action<PayResponse> requestFinishCallBack) {
		string url = GameSetting.PAY_SERVER + "/gpayc/pay/tencent.json?openid={0}&openkey={1}&pf={2}&pfkey={3}&playerId={4}&money={5}&payWay={6}&payItemId={7}";
		url = string.Format(url, openid, openkey, pf, pfkey, playerId, money, payWay, payItemId);


		RequestJson(url, "RequestDeductions", delegate (string json) {
			GameDebuger.Log("RequestRequestDeductions return = " + json);
			var data = JsHelper.ToObject<PayResponse>(json);
			requestFinishCallBack(data);
		}, false, true);
	}

    //请求json
    public static void RequestJson(string url, string jsonName, Action<string> downLoadFinishCallBack,
        bool needLock = true, bool refresh = false, Dictionary<string, string> headers = null)
    {
        if (!refresh && _jsonDics.ContainsKey(jsonName))
        {
            string json = _jsonDics[jsonName];
            downLoadFinishCallBack(json);
            return;
        }

        GameDebuger.Log("RequestJson " + url);


        if (needLock)
        {
            RequestLoadingTip.Show("RequestJson", true, true);
        }

	    Hashtable hashHeaders = null;
	    if (headers != null)
	    {
			hashHeaders = new Hashtable();
            foreach (var header in headers)
		    {
			    hashHeaders[header.Key] = header.Value;
		    }
	    }

        HttpController.Instance.DownLoad(url, delegate (ByteArray byteArray)
        {
            if (needLock)
            {
                RequestLoadingTip.Stop("RequestJson");
            }

            string json = byteArray.ToUTF8String();

            _jsonDics[jsonName] = json;
            downLoadFinishCallBack(json);
        }, null, delegate (Exception exception)
    {
        if (needLock)
        {
            RequestLoadingTip.Stop("RequestJson");
        }

        GameDebuger.Log(string.Format("RequestJson url={0} error={1}", url, exception.ToString()));

        downLoadFinishCallBack(null);
    }, false, SimpleWWW.ConnectionType.Short_Connect, hashHeaders);
    }

    //删除角色
    public static void RequestPlayerDelete(string playerId, string gameServerId, bool game,
        Action<PlayerDeleteResponse> downLoadFinishCallBack)
    {
        string url = GameSetting.SSO_SERVER +
                     "/gssoc/account/playerDelete.json?playerId={0}&gameServerId={1}&game={2}&token={3}";
        url = string.Format(url, playerId, gameServerId, game, ServerManager.Instance.loginAccountDto.token);
        //        Debug.LogError(url);
        RequestJson(url, "PlayerDelete", delegate (string json)
        {
            var data = JsHelper.ToObject<PlayerDeleteResponse>(json);
            downLoadFinishCallBack(data);
        }, true, true);
    }


    /// <summary>
    /// 获取二维码sid
    /// </summary>
    /// <param name="callback"></param>
    public static void RequestQRCodeSid(Action<QrVerifyDto> callback)
    {
        var url = string.Format("{0}/gsso/qr/sid.jsp", GameSetting.SSO_SERVER);
        RequestJson(url, "QRCodeSid", s =>
        {
            if (s != null)
            {
                callback(JsHelper.ToObject<QrVerifyDto>(s));
            }
            else
            {
                callback(null);
            }
        }, true, true);
    }


    /// <summary>
    /// 请求二维码登陆状况
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="callback"></param>
    public static void RequestQRCodeLoginState(string sid, Action<QrVerifyDto> callback)
    {
        var url = string.Format("{0}/gsso/qr/verify.jsp?sid={1}", GameSetting.SSO_SERVER, sid);
        RequestJson(url, "QRCodeLogin", s =>
        {
            if (s != null)
            {
                callback(JsHelper.ToObject<QrVerifyDto>(s));
            }
            else
            {
                callback(null);
            }
        }, false, true);
    }


    public static void RequestQRCodeLogin(string sid, string token, Action<QrVerifyDto> callback)
    {
        var url = string.Format("{0}/gsso/qr/login.jsp?sid={1}&token={2}", GameSetting.SSO_SERVER, sid, token);
        var headers = new Dictionary<string, string>()
        {
            {"x-cilugame-qr", "login"},
        };
        RequestJson(url, "QRCodeLogin", s =>
        {
            //            GameDebuger.Log(s);
            if (s != null)
            {
                callback(JsHelper.ToObject<QrVerifyDto>(s));
            }
            else
            {
                callback(null);
            }
        }, true, true, headers);
    }


    public static void RequestQRCodeEnsureLogin(string sid, string extra, Action<QrVerifyDto> callback)
    {
        var url = string.Format("{0}/gsso/qr/confirm.jsp?sid={1}&extra={2}", GameSetting.SSO_SERVER, sid, extra);
        var headers = new Dictionary<string, string>()
        {
            {"x-cilugame-qr", "confirm"},
        };
        RequestJson(url, "QRCodeLogin", s =>
        {
			GameDebuger.Log("RequestQRCodeEnsureLogin：" + s);
            if (s != null)
            {
                callback(JsHelper.ToObject<QrVerifyDto>(s));
            }
            else
            {
                callback(null);
            }
        }, true, true, headers);
    }


    /// <summary>
    /// 通知服务器扫描支付成功
    /// </summary>
    /// <param name="orderId"></param>
    public static void RequestQRCodeScanPaySuccess(string orderId)
    {
        var url = string.Format("{0}/gpayc/order/confirm.json?orderId={1}", GameSetting.PAY_SERVER, orderId);
        var headers = new Dictionary<string, string>()
        {
            {"x-cilugame-qr", "pay"},
        };
        RequestJson(url, "QRCodeLogin", null, false, true, headers);
    }
}