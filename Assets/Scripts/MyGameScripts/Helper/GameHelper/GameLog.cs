#if UNITY_EDITOR
//#define LOG_Notify
//#define LOG_Dispose
//#define LOG_MODULE
//#define LOG_REDPOINT
//#define LOG_Fish
//#define Log_Team
//#define Log_BAG
//#define Log_Formation
//#define Log_Battle
//#define Log_Equipment
//#define Log_CrewFetter
//#define Log_PanelDepth
//#define Log_Battle_Attr
//#define Log_Battle_Anim
//#define Log_Battle_RESP
//#define Log_Battle_Debug
//#define Log_BattleError
#endif

public class GameLog
{
    public static void Log_BattleError(string msg)
    {
#if Log_BattleError
        LogGeneral("Log_BattleError---" + msg, "#AA49F3FF");
#endif
    }
    
    public static void Log_Battle_Debug(string msg)
    {
#if Log_Battle_Debug
        LogGeneral("Log_Battle_Debug---" + msg, "#AA49F3FF");
#endif
    }
    
    public static void Log_Battle_RESP(string msg)
    {
#if Log_Battle_RESP
        LogGeneral("Log_Battle_RESP---" + msg, "#AA49F3FF");
#endif
    }

    public static void Log_Battle_Anim(string msg)
    {
#if Log_Battle_Anim
        LogGeneral("Log_Battle_Anim---" + msg, "#AA49F3FF");
#endif
    }
    public static void Log_Battle_PlayerAttr(string msg)
    {
#if Log_Battle_Attr
        LogGeneral("Log_Battle_PlayerAttr---" + msg, "#AA49F3FF");
#endif
    }

    public static void Log_Battle(string msg, string color = "")
    {
#if Log_Battle
        LogGeneral("Log_Battle---" + msg, color);
#endif
    }

    private static void LogGeneral(string msg, string color = "")
    {
        var log = msg == null ? "Null" : msg.ToString();
        if (!string.IsNullOrEmpty(color))
            log = "<color=" + color + ">" + log + "</color>";
        GameDebuger.LogError(log);
    }

    public static void LOG_Dispose(string msg)
    {
#if LOG_Dispose
        GameDebuger.LogError(msg);
        #endif
    }
    
    public static void LogPanelDepth(string msg)
    {
#if Log_PanelDepth
        LogGeneral(msg, "#AA49F3FF");
        #endif
    }
    
    public static void Log_Formation(string msg)
    {
#if Log_Formation
        GameDebuger.LogError(msg);
        #endif
    }

    public static void Log_BAG(string msg)
    {
#if Log_BAG
        GameDebuger.LogError(msg);
        #endif
    }

    public static void LOG_Notify(string msg)
    {
#if LOG_Notify
        GameDebuger.LogError(msg);
        #endif
    }
    public static void LogTeam(string msg)
    {
#if Log_Team
        GameDebuger.LogError(msg);
        #endif
    }

    public static void LogFish(string msg){

#if  LOG_Fish
        GameDebuger.LogError(msg);
        #endif
    }
    public static void LogRedPoint(string msg){

#if  LOG_REDPOINT
        GameDebuger.LogError(msg);
        #endif
    }

    public static void LOGModule(string msg){
#if LOG_MODULE
        GameDebuger.LogError(msg);
        #endif
    }

    public static void LogEquipment(string msg)
    {
#if Log_Equipment
        GameDebuger.Log(msg, "#AA49F3FF");
#endif
    }
    public static void LogCrewFettew(string msg)
    {
#if Log_CrewFetter
        GameDebuger.Log(msg, "#AA49F3FF");
#endif
    }
    /// <summary>
    /// 后续会接入游戏界面弹出错误信息
    /// </summary>
    /// <param name="msg"></param>
    public static void ShowError(string msg,string detail = "")
    {
        ErrorViewLogic.ShowError(msg,detail);
    }
}