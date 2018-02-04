using System.Collections;
using System.Collections.Generic;

public class TalkingDataHelper
{
    public static string appID = "75DB7BC2B38B42958687FE14C0907959";

    private static TDGAAccount account;

    public static void OnEvent(string actionId, Dictionary<string, object> parameters = null)
    {
        TalkingDataGA.OnEvent(actionId, parameters);
        BehaviorHelper.OnEvent(actionId, parameters);
    }

    public static void OnEvent(string actionId, string paramKey, string paramValue)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add(paramKey, paramValue);
        OnEvent(actionId, parameters);
    }

    //linkEvent format is action/step, such as GameStart/LoadStaticVersion
    public static void OnEventSetp(string linkEvent)
    {
        string[] links = linkEvent.Split('/');
        if (links.Length != 2)
        {
            return;
        }

        OnEventSetp(links[0], links[1]);
    }

    public static void OnEventSetp(string actionId, string paramValue)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("step", paramValue);
        OnEvent(actionId, parameters);
    }

    public static void OnEventSetp(string actionId, string paramValue, string param2, string paramValue2)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("step", paramValue);
        parameters.Add(param2, paramValue2);
        OnEvent(actionId, parameters);
    }

    #region 统计启动,结束

    public static void Setup()
    {
        TalkingDataGA.OnStart(appID, GameSetting.Channel);
    }

    public static void Dispose()
    {
        TalkingDataGA.OnEnd();
    }

    #endregion

    #region 设置玩家信息

    public static void SetupAccount(int serviceId, long playerId, string playerName, int grade, int factionId,
        int gender)
    {
        account = TDGAAccount.SetAccount(playerId.ToString());
        //account.SetAccountType(AccountType.ANONYMOUS); // 账号来源
        account.SetGameServer(serviceId.ToString()); // 玩家所在服务器
        account.SetAccountName(playerName);
        account.SetLevel(grade);
        account.SetGender(gender == 1 ? Gender.MALE : Gender.FEMALE);

        BehaviorHelper.SetupAccount(serviceId, playerId, playerName, grade, factionId, gender);
    }

    public static void DisposeAccount()
    {
        account = null;
        BehaviorHelper.DisposeAccount();
    }

    public static void SetNickname(string name)
    {
        if (account != null)
        {
            account.SetAccountName(name);
        }
        BehaviorHelper.SetNickname(name);
    }

    public static void SetLevel(int level)
    {
        if (account != null)
        {
            account.SetLevel(level);
        }
        BehaviorHelper.SetLevel(level);
    }

    #endregion
}