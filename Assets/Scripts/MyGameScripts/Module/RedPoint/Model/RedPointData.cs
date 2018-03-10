// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : CL-PC080
// Created  : 03/07/2018 15:37:18
// **********************************************************************

public interface IRedPointData
{

}

public class RedPointInfo
{
    public RedPointInfo(
        bool _isShow = false
        , RedPointType _redPointEnum = 0
        , int _num = 0
        , bool _isOpen = false)
    {
        isShow = _isShow;
        num = _num;
        redPointEnum = _redPointEnum;
        isOpen = _isOpen;
    }

    public bool isShow;
    public int num;
    readonly public RedPointType redPointEnum;
    public bool isOpen;
    public bool IsOpen { get { return isOpen; } }
    
    public bool IsShow()
    {
        return IsOpen && isShow;
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

            strBuilder.Append(string.Format("---info.redPointEnum = {0}, info.isShow = {1}, info.isOpen = {2}, info.num = {3}", info.redPointEnum.ToString(), info.isShow, info.isOpen, info.num));
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

        public void InitData()
        {
        }

        public void Dispose()
        {

        }
    }
}
