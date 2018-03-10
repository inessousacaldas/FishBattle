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
        _redPointDic.Clear();
        GameLog.LogRedPoint("redpointmanager ClearData------");
    }

    public static void UpdateRedPoint(IEnumerable<ShowRedPointTypeDto> set)
    {
        set.ForEach(delegate(ShowRedPointTypeDto dto)
        {
            if (dto != null)
            {
                var ty = ConvertUILayoutToRedPointType(dto.redPointId);
                var isOpen = IsFunctionOpen(ty);
                var isShow = dto.count > 0;
                var data = new RedPointInfo(
                    isShow
                    , ty
                    , dto.count
                    , isOpen);
                DataMgr.UpdateRedPointDic(data);

            }
        });

        FireData();
    }
    public static void UpdateRedPoint(ShowRedPointTypeDto dto)
    {
        if (dto == null)
        {
            return;
        }

        var ty = ConvertUILayoutToRedPointType(dto.redPointId);
        if (ty != RedPointType.invalid)
        {
            var isOpen = IsFunctionOpen(ty);
            var data = new RedPointInfo(dto.count > 0, ty, dto.count, isOpen);
            UpdateRedPoint(data);
        }
    }

    public static void UpdateRedPoint(bool isShown, RedPointType rpEnum, int rpCnt = -1)
    {
        RedPointInfo info = null;
        DataMgr._redPointDic.TryGetValue(rpEnum, out info);
        var isOpen = IsFunctionOpen(rpEnum);
        if (info == null)
        {
            var cnt = rpCnt >= 0 ? rpCnt : (isShown ? 1 : 0);
            info = new RedPointInfo(isShown, rpEnum, cnt, isOpen);
        }
        else
        {
            info.isShow = isShown;
            info.isOpen = isOpen;
            if (rpCnt >= 0)
            {
                info.num = rpCnt;
            }
        }
        UpdateRedPoint(info);
    }

    public static void UpdateRedPoint(RedPointInfo info)
    {
        if (info == null)
        {
            return;
        }
        info.Print("UpdateRedPoint------------");

        DataMgr.UpdateRedPointDic(info);
        FireData();
    }

    public static RedPointInfo Filter(RedPointType redPointEnum)
    {
        return DataMgr.GetDataByRPEnum(redPointEnum);
    }

    /// <summary>
    /// The _red point dic.
    /// </summary>
    /// 红点id(对应的界面上的Id)，数量(如果不需要显示具体数量的话，>0 表示需要显示红点)
    Dictionary<RedPointType, RedPointInfo> _redPointDic = new Dictionary<RedPointType, RedPointInfo>(new EnumEqualityComparer());

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
            _redPointDic[info.redPointEnum] = info;
        }
    }

    private RedPointInfo GetDataByRPEnum(RedPointType redPointEnum)
    {
        RedPointInfo info = null;
        _redPointDic.TryGetValue(redPointEnum, out info);
        return info;
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
        _redPointDic.TryGetValue(id, out info);
        return info == null ? 0 : info.num;
    }

    #endregion

    private int GetShowNum(RedPointType rpEnum)
    {
        Func<RedPointType, bool> checkIgnore = delegate(RedPointType typeId)
        {
            return false;
            //todo fish:日程红点忽略计数： to be check
//            int activityId = ConvertRedpointTypeToDailyInfoID(typeId);
//            DailyActivityInfo activityInfo = ModelManager.DailyPush.GetDailyInfoByID(activityId);
//            return activityInfo != null && ModelManager.DailyPush.CheckIgnore(activityId);
        };

        RedPointInfo info = null;
        _redPointDic.TryGetValue(rpEnum, out info);

        if (info == null)
        {
            return 0;
        }
        else
        {
            return !checkIgnore(rpEnum) ? info.num : 0;
        }
    }

    public static bool GetRedPointShowState(int actID)
    {
        var data = GetMergeData(new RedPointType[1] { (RedPointType)actID });
        return data.IsShow();
    }

    public static bool GetRedPointShowState(RedPointType ty)
    {
        var data = GetMergeData(new RedPointType[1] { ty });
        return data.IsShow();
    }

    public static RedPointInfo GetMergeData(RedPointType ty)
    {
        return GetMergeData(new RedPointType[1] { ty });
    }

    public static RedPointInfo GetMergeData(RedPointType[] redPointEnumArray)
    {
        if (redPointEnumArray == null)
        {
            return null;
        }
        return GetMergeData(redPointEnumArray, redPointEnumArray[0]);
    }

    public static RedPointInfo GetMergeData(RedPointType[] redPointEnumArray, RedPointType rpEnum)
    {
        GameLog.LogRedPoint("get mergeData by enum : " + rpEnum.ToString());
        if (rpEnum == RedPointType.invalid)
        {
            GameLog.LogRedPoint("GetMergeData------- rpEnum = DeliverShowRedPoint_Unknown");
            return null;
        }
        var num = 0;
        
        var isShow = false;
        var isOpen = false;
        foreach (var id in redPointEnumArray)
        {
            if (DataMgr.GetDataByRPEnum(id) != null)
            {
                RedPointInfo info = null;
                DataMgr._redPointDic.TryGetValue(id, out info);

                info.Print(string.Format("GetMergeData------- rpenum = {0}  ", id));
                if (info != null && info.IsShow())
                {
                    num += DataMgr.GetShowNum(info.redPointEnum);
                    isShow = true;
                    isOpen = true;
                }
            }
        }

        var mergedData = new RedPointInfo(isShow, rpEnum, num, isOpen);
        mergedData.Print();
        return mergedData;
    }
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
