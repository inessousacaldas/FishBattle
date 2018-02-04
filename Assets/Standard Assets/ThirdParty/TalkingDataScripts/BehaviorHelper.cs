using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BehaviorHelper
{
    private static string behaviorUrl;

    public static void Setup(string url, string uuid)
    {
        behaviorUrl = url;
        Behavior.UUID = uuid;
        //Debug.Log(string.Format("BehaviorHelper Setup url={0} UUID={1}", behaviorUrl, uuid));
        OnEvent("Setup");
    }

    public static void Dispose()
    {
        OnEvent("Dispose");
        //DisposeAccount();
    }

    public static void OnEvent(string actionId, Dictionary<string, object> parameters = null)
    {
        if (string.IsNullOrEmpty(behaviorUrl))
        {
            return;
        }

		string json = Behavior.ToJson(actionId, parameters);
        //GameDebuger.Log(b, "Fuchsia");
        json = WWW.EscapeURL(json);
        string url = behaviorUrl + json;
        HttpController.Instance.DownLoad(url, delegate(ByteArray byteArray)
        {
#pragma warning disable 0219
            string str = byteArray.ToEncodingString();
#pragma warning restore
            //GameDebuger.Log(str);
        }, null, delegate
        {
            //GameDebuger.Log(obj.ToString());
        }, false, SimpleWWW.ConnectionType.Short_Connect);
    }

    public static void OnEvent(string actionId, string paramKey, string paramValue)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add(paramKey, paramValue);
        OnEvent(actionId, parameters);
    }

    public static void OnEventSetp(string actionId, string paramValue)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("step", paramValue);
        OnEvent(actionId, parameters);
    }

    #region 设置玩家信息

    public static void SetupSid(string sid)
    {
        Behavior.sid = sid;
    }

    public static void SetupAccount(int serviceId, long playerId, string playerName, int grade, int factionId,
        int gender)
    {
        Behavior.SetupAccount(serviceId, playerId, playerName, grade, factionId, gender);
        OnEvent("InitPlayer");
    }

    public static void DisposeAccount()
    {
        Behavior.DisposeAccount();
        OnEvent("DisposePlayer");
    }

    public static void SetNickname(string name)
    {
        Behavior.NickName = name;
        if (Behavior.Id > 0)
            OnEvent("ChangeName");
    }

    public static void SetLevel(int grade)
    {
        Behavior.Grade = grade;
        if (Behavior.Id > 0)
            OnEvent("LevelUp");
    }

    #endregion
}

public class Behavior
{
    // 机器信息
    public static string UUID = "1111-2222-3333-4444";

    // 平台渠道
    // GameSetting.Channel;
    // GameSetting.Platform;

    // 服务器信息
    public static int ServerId = -1;

    // 用户信息
    public static string sid = "";

    public static long Id = -1;
    public static string NickName = "";
    public static int Grade = -1;
    public static int FactionId = -1;
    public static int Gender = -1;

    // 事件
    private string eventId;
    private Dictionary<string, object> eventParameters;
    private long eventTime;

    public Behavior(string actionId, Dictionary<string, object> parameters)
    {
        eventId = actionId;
        eventTime = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000)/10000000;
        eventParameters = parameters;
    }

    public static void SetupAccount(int serviceId, long playerId, string playerName, int grade, int factionId,
        int gender)
    {
        ServerId = serviceId;
        Id = playerId;
        NickName = playerName;
        Grade = grade;
        FactionId = factionId;
        Gender = gender;
    }

    public static void DisposeAccount()
    {
        ServerId = -1;

        Id = -1;
        NickName = "";
        Grade = -1;
        FactionId = -1;
        Gender = -1;
    }

    public static string ToJson(string actionId, Dictionary<string, object> parameters = null)
    {
        Behavior b = new Behavior(actionId, parameters);
        return b.ToJson();
    }

    public string ToJson()
    {
        List<string> results = new List<string>();

        // 机器信息
        results.Add(propertyToJsonItem("uuid", UUID));

        // 平台渠道
        results.Add(propertyToJsonItem("platform", GameSetting.PlatformTypeId.ToString()));
        results.Add(propertyToJsonItem("channel", GameSetting.Channel));

        // 服务器信息
        if (ServerId > 0)
        {
            results.Add(propertyToJsonItem("gameServerId", ServerId.ToString()));
        }

        if (!string.IsNullOrEmpty(sid))
        {
            results.Add(propertyToJsonItem("sid", sid));
        }

        // 用户信息
        if (Id > 0)
        {
            results.Add(propertyToJsonItem("Id", Id.ToString()));
            results.Add(propertyToJsonItem("nickname", NickName));
            results.Add(propertyToJsonItem("grade", Grade.ToString()));
            results.Add(propertyToJsonItem("factionId", FactionId.ToString()));
            results.Add(propertyToJsonItem("gender", Gender.ToString()));
        }

        // 事件
        results.Add(propertyToJsonItem("eventId", eventId));
        results.Add(propertyToJsonItem("eventTime", eventTime.ToString()));
        if (eventParameters != null && eventParameters.Count > 0)
        {
            List<string> events = new List<string>();
            foreach (string key in eventParameters.Keys)
            {
                events.Add(propertyToJsonItem(key, eventParameters[key].ToString()));
            }
            results.Add(string.Format("\"{0}\":{1}", "eventParameters", "[{" + string.Join(",", events.ToArray()) + "}]"));
        }
        return "{" + string.Join(",", results.ToArray()) + "}";
    }

    private string propertyToJsonItem(string name, string property)
    {
        //return string.Format("\"{0}\":\"{1}\"", name, WWW.EscapeURL(property));
        return string.Format("\"{0}\":\"{1}\"", name, property);
    }
}
