// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : CL-PC080
// Created  : 03/07/2018 15:37:18
// **********************************************************************

using System;
using System.Collections.Generic;

public interface IRedPointData
{
    RedPointInfo GetMergeData(RedPointType[] redPointEvtArr);
}

public class RedPointInfo
{
    public RedPointInfo(
        bool _isShow = false
        , RedPointType _redPointEnum = 0
        , int _num = 0
        , bool pIsActive = false)
    {
        isShow = _isShow;
        num = _num;
        redPointEnum = _redPointEnum;
        isActive = pIsActive;
    }

    public bool isShow;
    public int num;
    readonly public RedPointType redPointEnum;
    public bool isActive;
    public bool IsActive { get { return isActive; } }
    
    public bool IsShow()
    {
        return IsActive && isShow;
    }
}

public static class RedPointInfoExt
{
    public static void Print(this RedPointInfo info, string headTitle = "")
    {
//        if (info != null && info.redPointEnum == RedPointType.Achieve_GetReward)
//            info.Print(info.redPointEnum, headTitle);
    }


    public static void Print(this RedPointInfo info, RedPointType rpEnum, string headTitle = "")
    {
        System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
        if (!string.IsNullOrEmpty(headTitle))
            strBuilder.Append(headTitle);

        strBuilder.Append("   print rpenum = " + rpEnum.ToString());

        if (info != null)
        {

            strBuilder.Append(string.Format("---info.redPointEnum = {0}, info.isShow = {1}, info.isOpen = {2}, info.num = {3}", info.redPointEnum.ToString(), info.isShow, info.isActive, info.num));
        }
        else
        {
            strBuilder.Append("info is Null");
        }

//        if (info != null && (info.redPointEnum == RedPointType.DeliverShowRedPoint_Unknown || info.redPointEnum == rpEnum))
//        {
//            GameLog.LogRedPoint(strBuilder.ToString());
//        }

    }
}

public sealed partial class RedPointDataMgr
{
    public sealed partial class RedPointData:IRedPointData
    {
        /// <summary>
        /// The _red point dic.
        /// </summary>
        /// 红点id(对应的界面上的Id)，数量(如果不需要显示具体数量的话，>0 表示需要显示红点)
        public Dictionary<RedPointType, RedPointInfo> _redPointDic = new Dictionary<RedPointType, RedPointInfo>(new EnumEqualityComparer());
        
        public void InitData()
        {
        }

        public void Dispose()
        {
            _redPointDic.Clear();
        }
        
        public RedPointInfo GetMergeData(RedPointType[] redPointEnumArray, RedPointType rpEnum)
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
                if (GetDataByRPEnum(id) != null)
                {
                    RedPointInfo info = null;
                    _redPointDic.TryGetValue(id, out info);

                    info.Print(string.Format("GetMergeData------- rpenum = {0}  ", id));
                    if (info != null && info.IsShow())
                    {
                        num += GetShowNum(info.redPointEnum);
                        isShow = true;
                        isOpen = true;
                    }
                }
            }

            var mergedData = new RedPointInfo(isShow, rpEnum, num, isOpen);
            mergedData.Print();
            return mergedData;
        }
        
        private RedPointInfo GetDataByRPEnum(RedPointType redPointEnum)
        {
            RedPointInfo info = null;
            _data._redPointDic.TryGetValue(redPointEnum, out info);
            return info;
        }
        public RedPointInfo GetMergeData(RedPointType[] redPointEnumArray)
        {
            if (redPointEnumArray == null)
            {
                return null;
            }
            return GetMergeData(redPointEnumArray, redPointEnumArray[0]);
        }
        
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

        public bool GetRedPointShowState(int actID)
        {
            var data = GetMergeData(new RedPointType[1] { (RedPointType)actID });
            return data.IsShow();
        }

        public bool GetRedPointShowState(RedPointType ty)
        {
            var data = GetMergeData(new RedPointType[1] { ty });
            return data.IsShow();
        }

        public RedPointInfo GetMergeData(RedPointType ty)
        {
            return GetMergeData(new RedPointType[1] { ty });
        }
    }
}
