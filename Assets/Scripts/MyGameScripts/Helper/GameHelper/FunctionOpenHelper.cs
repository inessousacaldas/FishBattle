using AppDto;
using System.Collections.Generic;
using UnityEngine;

public class FunctionOpenHelper
{
    private static Dictionary<int, bool> functionCloseDic = new Dictionary<int, bool>();

    public static event System.Action OnFunctionSwitchChange;
    public static event System.Action<int> OnFunctionSwitchChangeID;

    public static void Setup()
    {
        if (functionCloseDic == null)
        {
            functionCloseDic = new Dictionary<int, bool>();
        }
        functionCloseDic.Clear();
    }

    public static void UpdateFunctionSwitch(FunctionOpenNotify notify)
    {
        if (notify.ids == null)
        {
            return;
        }

        for (int i = 0; i < notify.ids.Count; i++)
        {
            int functionid = notify.ids[i];
            functionCloseDic[functionid] = notify.close;
            FunctionOpen functionOpen = DataCache.getDtoByCls<FunctionOpen>(functionid);
            string name = "未定义";
            if (functionOpen != null)
            {
                name = functionOpen.name;

                if (OnFunctionSwitchChangeID != null)
                {
                    OnFunctionSwitchChangeID(functionid);
                }
            }
            GameDebuger.Log(string.Format("UpdateFunctionSwitch id={0} name={1} close={2}", functionid, name, notify.close));
        }

        UpdateFunctionSwitch();
    }

    public static void UpdateFunctionSwitch()
    {
        if (OnFunctionSwitchChange != null)
        {
            OnFunctionSwitchChange();
        }
    }

//    public static bool isFuncOpen(int[] functionIdArr, bool showTip, string tipStr, System.Func<int, bool, bool> pAdditionForbiddenCondition)
//    {
//        if (functionIdArr == null || functionIdArr.Length <= 0)
//            return true;
//        var id = functionIdArr.Find(s => { return isFuncOpen(s, showTip, tipStr, pAdditionForbiddenCondition); });
//        return id > (int)FunctionOpen.FunctionOpenEnum.NONE;
//    }
    

    /// <summary>
    /// Ises the func open.
    /// </summary>
    /// <param name="pAdditionForbiddenCondition">额外的判定条件，条件返回值为true时，本方法返回false，反之亦然。</param>
    public static bool isFuncOpen(
        FunctionOpen.FunctionOpenEnum _functionId
        , bool showTip = false
        , string tipStr = ""
        , System.Func<FunctionOpen.FunctionOpenEnum, bool, bool> pAdditionForbiddenCondition = null)
    {
        var functionId = (int) _functionId;
        return isFuncOpen(functionId, showTip, tipStr, pAdditionForbiddenCondition);
    }
    
    public static bool isFuncOpen(
        int functionId
        , bool showTip = false
        , string tipStr = ""
        , System.Func<FunctionOpen.FunctionOpenEnum, bool, bool> pAdditionForbiddenCondition = null)
    {
        //先判断客户端预判开关是否开启，如果不开启， 则直接返回true
        if (!ServiceRequestAction.ServerRequestCheck)
        {
            return true;
        }
        
        if (null != pAdditionForbiddenCondition)
        {
            if (pAdditionForbiddenCondition((FunctionOpen.FunctionOpenEnum)functionId, showTip))
            {
                return false;
            }
        }

        
        FunctionOpen functionOpen = DataCache.getDtoByCls<FunctionOpen>((int)functionId);
        if (functionOpen == null)
            return functionId == (int)FunctionOpen.FunctionOpenEnum.NONE;

        bool isClose = false;

        //先检查功能开关后台控制表
        if (functionCloseDic.ContainsKey(functionId))
        {
            isClose = functionCloseDic[functionId];
        }
        else
        {
            isClose = functionOpen.close;
        }

        if (isClose)
        {
            if (showTip)
            {
                if (string.IsNullOrEmpty(tipStr) || tipStr.Contains("{0}"))
                    TipManager.AddTip(string.Format("{0}功能暂未开放", functionOpen.name));
                else
                    TipManager.AddTip(tipStr);
            }

            return false;
        }

        int playerGrade = ModelManager.Player.GetPlayerLevel();
        if (playerGrade < functionOpen.grade)
        {
            if (showTip)
            {
                if (string.IsNullOrEmpty(tipStr))
                {
                    TipManager.AddTip(string.Format("{0}开启{1}功能", UIHelper.GetStepGradeName(functionOpen.grade, true), functionOpen.name));
                    //TipManager.AddTip (string.Format ("{0}级开启{1}功能", functionOpen.grade, functionOpen.name));
                }
                else
                {
                    if (tipStr.Contains("{0}"))
                    {
                        tipStr = string.Format(tipStr, functionOpen.grade);
                    }
                    TipManager.AddTip(tipStr);
                }
            }

            return false;
        }

        if (ModelManager.Player.GetServerGradeDto == null)
            return true;

        if (ModelManager.Player.ServerGrade < functionOpen.serverGrade)
        {
            if (showTip)
            {
                if (string.IsNullOrEmpty(tipStr))
                {
                    TipManager.AddTip(string.Format("服务器等级需要达到{0}级开启{1}功能", functionOpen.grade, functionOpen.name));
                }
                else
                {
                    if (tipStr.Contains("{0}"))
                    {
                        tipStr = string.Format(tipStr, functionOpen.grade);
                    }
                    TipManager.AddTip(tipStr);
                }
            }

            return false;
        }

        return true;
    }

    public static int GetFunctionOpenLv(int functionId, int defaultLv = 0)
    {
        FunctionOpen functionOpen = DataCache.getDtoByCls<FunctionOpen>(functionId);
        if (functionOpen != null)
            return functionOpen.grade;
        else
            return defaultLv;
    }
}