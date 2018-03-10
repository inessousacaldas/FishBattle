// **********************************************************************
// Copyright (c) 2016 cilugame. All rights reserved.
// File     : QQLoginHelper.cs
// Author   : senkay <senkay@126.com>
// Created  : 12/19/2016 
// Porpuse  : 德米游戏的QQ登陆方式
// **********************************************************************
//
using System;
using UnityEngine;
using System.Text.RegularExpressions;
using SdkAccountDto;

public class QQLoginHelper
{
    public const string QQLogin_APPID = "101369233";//德米游戏
    public const string QQLogin_redirect_uri = "http://qzs.qq.com/qzone/openapi/success.html";
    //public const string QQLogin_redirect_uri = "http://demigame.com";
    public const string QQLogin_scope = "get_user_info";
    public const string QQLogin_URL = "https://graph.qq.com/oauth2.0/authorize?response_type=token&client_id={0}&redirect_uri={1}&scope={2}&display={3}";

    public static string QQ_accessToken = "";
    public static string QQ_OpenId = "";

    //打开登录页
    static public void OpenLoginPage()
    {
        string display = "mobile"; //pc or mobile

        string loginUrl = string.Format(QQLogin_URL, QQLogin_APPID, WWW.EscapeURL(QQLogin_redirect_uri), QQLogin_scope, display);
        ProxyBuiltInWebModule.Open(loginUrl);
    }

    //通过accessToken获取用户的OpenID
    static public void RequestQQOpenID()
    {
        var url = string.Format("https://graph.qq.com/oauth2.0/me?access_token={0}", QQ_accessToken);
        ServiceProviderManager.RequestJson(url, "RequestQQOpenID", result =>
            {
//              callback( {"client_id":"YOUR_APPID","openid":"YOUR_OPENID"} );
                GameDebuger.Log("RequestQQOpenID：" + result);
                if (string.IsNullOrEmpty(result))
                {
                    GameDebuger.LogError("RequestQQOpenID result is null");
                }
                else
                {
                    Regex regex = new Regex("callback(\\s)*?\\((?<STR>([^()])+?)\\)(\\s)*?\\;");
                    Match match = regex.Match(result);
                    if (match.Success)
                    {
                        string json = match.Groups["STR"].ToString();
                        GameDebuger.Log("json=" + json);
                        QQOpenIDInfo info = JsHelper.ToObject<QQOpenIDInfo>(json);
                        GameDebuger.Log(string.Format("client_id={0} openid={1}", info.client_id, info.openid));

                        LoginSessionDto loginDto = new LoginSessionDto();
                        loginDto.sid = info.openid;
                        loginDto.uid = info.openid;
                        QQLoginHelper.QQ_OpenId = info.openid;

                        AccountDto accountDto = new AccountDto();
                        accountDto.seesionDto = loginDto;
                        accountDto.type = AccountDto.AccountType.qq;
                        
                        SdkAccountModel.Instance.OnLoginSuccess(accountDto);
                    }
                }
            }, true, true);
    }

    //通过accessToken获取用户的OpenID
    static public void RequestQQUserInfo()
    {
        var url = string.Format("https://graph.qq.com/user/get_user_info?access_token={0}&oauth_consumer_key={1}&openid={2}", QQ_accessToken, QQLogin_APPID, QQ_OpenId);
        ServiceProviderManager.RequestJson(url, "GetQQUserInfo", result =>
            {           
//                {
//                    "ret":0,
//                    "msg":"",
//                    "nickname":"YOUR_NICK_NAME",
//                        ...
//                    }

                GameDebuger.Log("RequestQQUserInfo：" + result);
                if (string.IsNullOrEmpty(result))
                {
                    GameDebuger.LogError("RequestQQUserInfo result is null");
                }
                else
                {

                }
            }, true, true);
    }
}
    

public class QQOpenIDInfo
{
    public string client_id;
    public string openid;
}