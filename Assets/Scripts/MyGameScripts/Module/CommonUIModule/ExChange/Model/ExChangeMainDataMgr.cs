// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Zijian
// Created  : 8/24/2017 2:54:17 PM
// **********************************************************************

using System.Collections.Generic;
using UniRx;
using AppDto;

namespace StaticInit
{
    public partial class StaticInit
    {
        //先初始化数据
        private StaticDispose.StaticDelegateRunner disposeExChangeMainDataMgr = new StaticDispose.StaticDelegateRunner(
            ()=> { var mgr = ExChangeMainDataMgr.DataMgr; });
    }
}

public sealed partial class ExChangeMainDataMgr
{
    // 初始化
    private void LateInit()
    {
        ExChangeMainNetMsg.ReqGetShop_MiraCount();
    }
    
    public void OnDispose(){
            
    }

    public int GetRemainMiraCount()
    {
        return _data.RemainConvertMiraCount;
    }

    /// <summary>
    /// 获取最近的货币爸爸
    /// </summary>
    /// <returns></returns>
    //public int GetFirstConvertType(AppVirtualItem.VirtualItemEnum type)
    //{
    //    int res = -1;
    //    switch(type)
    //    {
    //        case AppVirtualItem.VirtualItemEnum.Mira:
    //            res = (int)AppVirtualItem.VirtualItemEnum.DIAMOND;
    //            break;
    //        case AppVirtualItem.VirtualItemEnum.Gold:
    //        case AppVirtualItem.VirtualItemEnum.Silver:
    //            res = (int)AppVirtualItem.VirtualItemEnum.MIRA;
    //            break;
    //    }
    //    return res;
    //}

    #region 本次登录是否 自动兑换 即不再弹出兑换框
    public void SetNotShowView(int virtualId)
    {
        if (_data.GetIsShowFastExView.ContainsKey(virtualId))
            _data.GetIsShowFastExView[virtualId] = false;
    }

    public bool GetIsShowFastView(int virtualId)
    {
        if (_data.GetIsShowFastExView.ContainsKey(virtualId))
            return _data.GetIsShowFastExView[virtualId];

        return true;
    }

    public long GetWealth(ExChangeUseTabType id)
    {
        return _data.GetWealth(id);
    }
    #endregion
}
