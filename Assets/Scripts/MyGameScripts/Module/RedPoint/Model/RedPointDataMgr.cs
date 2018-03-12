// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : CL-PC080
// Created  : 03/07/2018 15:37:18
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;

public sealed partial class RedPointDataMgr
{
    // 初始化
    private void LateInit()
    {
        
    }
    
    //  退出/重登清除数据
    public void OnDispose()
    {
        _data._redPointDic.Clear();
        GameLog.LogRedPoint("redpointmanager ClearData------");
    }

    public static void UpdateRedPoint(IEnumerable<ShowRedPointTypeDto> set)
    {
        set.ForEach(UpdateRedPoint);
        FireData();
    }

    private static void UpdateRedPoint(ShowRedPointTypeDto dto)
    {
        if (dto == null)
        {
            return;
        }
        var ty = ConvertUILayoutToRedPointType(dto.redPointId);
        if (ty == RedPointType.invalid) return;
        var isOpen = IsFunctionOpen(ty);
        var isShow = dto.count > 0;
        var data = new RedPointInfo(
            isShow
            , ty
            , dto.count
            , isOpen);
        UpdateRedPoint(data);
    }

    public void UpdateRedPoint(bool isShown, RedPointType rpEnum, int rpCnt = -1)
    {
        RedPointInfo info = null;
        _data._redPointDic.TryGetValue(rpEnum, out info);
        var isOpen = IsFunctionOpen(rpEnum);
        if (info == null)
        {
            var cnt = rpCnt >= 0 ? rpCnt : (isShown ? 1 : 0);
            info = new RedPointInfo(isShown, rpEnum, cnt, isOpen);
        }
        else
        {
            info.isShow = isShown;
            info.isActive = isOpen;
            if (rpCnt >= 0)
            {
                info.num = rpCnt;
            }
        }
        UpdateRedPoint(info);
    }

    private static void UpdateRedPoint(RedPointInfo info)
    {
        if (info == null)
        {
            return;
        }
        info.Print("UpdateRedPoint------------");

        DataMgr.UpdateRedPointDic(info);
        FireData();
    }

    /// <summary>
    /// The _action dic.
    /// </summary>

    private void UpdateRedPointDic(RedPointType rpEnum, int num, bool isOpen)
    {
        var isShown = rpEnum > RedPointType.invalid && num > 0;
        int activityId = ConvertRedpointTypeToDailyInfoID(rpEnum);

        //当要显示红点或者特效到时候先要看下是否已经完成了
        if (activityId != -1)
        {
//            DailyActivityInfo activityInfo = ModelManager.DailyPush.GetDailyInfoByID(activityId);
//
//            var finish = ModelManager.DailyPush.JudgeActivityFinish(activityInfo);
//            var show = ModelManager.DailyPush.ExtentJudgeShow(activityInfo);

//            isShown = !finish && show;
        }

        var data = new RedPointInfo(isShown, rpEnum, num, isOpen);
       
        UpdateRedPointDic(data);
    }

    private void UpdateRedPointDic(RedPointInfo info)
    {
        if (info != null && info.redPointEnum != RedPointType.invalid)
        {
            _data._redPointDic[info.redPointEnum] = info;
        }
    }

    public static int ConvertRedpointTypeToDailyInfoID(RedPointType id)
    {
        // 日程红点原来是另一组枚举
//        return id < RedPointType.DeliverShowRedPoint_Email ? (int)id : -1;
        return -1;
    }
    private static bool IsFunctionOpen(RedPointType id)
    {
//        if (id < RedPointType.DeliverShowRedPoint_Email)
//        {
//            var info = ModelManager.DailyPush.GetDailyInfoByID(ConvertRedpointTypeToDailyInfoID(id));
//            return info == null ? true : FunctionOpenHelper.isFuncOpen(info.openGradeId, false);
//        }
//        else if (id == RedPointType.ShopMall_RedPoint)
//        {
//            return FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_Rebate, false);
//        }
//        else
        {
            return true;
        }
    }

    public static bool TransferIntToEnumType<T>(object type, ref T enumValue, T invalidVal) where T : struct
    {
        enumValue = invalidVal;
        var check = Enum.IsDefined(typeof(T), type);
        if (!check)
        {
            GameDebuger.LogWarning(string.Format("warnning:cannot transfer type {0} to enum {1}", type, typeof(T)));
        }
        else
        {
            enumValue = (T)type;
        }

        return check;
    }

    public static RedPointType ConvertDailyActIDToRedPointType(int actID)
    {
        var ty = RedPointType.invalid;
        var check = TransferIntToEnumType<RedPointType>(actID, ref ty, RedPointType.invalid);
        return check ? ty : RedPointType.invalid;
    }

    private static RedPointType ConvertUILayoutToRedPointType(int actID)
    {
        var ty = RedPointType.invalid;
        var check = TransferIntToEnumType<RedPointType>(actID, ref ty, RedPointType.invalid);
        return check ? ty : RedPointType.invalid;
    }
    
    

    /// <summary>
    /// Gets the red point.
    /// </summary>
    /// <returns>The red point.</returns>
    /// <param name="id">红点id(对应的界面上的Id).  UiLayout上的枚举 </param>
    /// 单独的
    #region 根据ID 获取是否有红点
    public int GetRedPoint(RedPointType id)
    {
        RedPointInfo info = null;
        _data._redPointDic.TryGetValue(id, out info);
        return info == null ? 0 : info.num;
    }

    #endregion
}

//
//public class aaaEnumEqualityComparer<T> : IEqualityComparer<T> where T : struct{
//    public bool Equals(T x, T y){
//        return (int)x == (int)y;
//    }
//
//    public int GetHashCode(T type){
//        return type.GetHashCode();
//    }
//}


public class EnumEqualityComparer : IEqualityComparer<RedPointType>
{
    public bool Equals(RedPointType x, RedPointType y)
    {
        return x == y;
    }

    public int GetHashCode(RedPointType type)
    {
        return type.GetHashCode();
    }
}
