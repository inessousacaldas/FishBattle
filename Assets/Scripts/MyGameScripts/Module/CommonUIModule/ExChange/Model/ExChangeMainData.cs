// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Zijian
// Created  : 8/24/2017 2:54:17 PM
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;

public interface IExChangeMainData
{
    ExChangeUseTabType CurTab { get; }
    //当前使用的货币 钻石/绑钻/米拉
    int CurUseExChangeId { get; }
    //期待兑换的货币
    int ExchangeId { get; }
    int ExpectTotalCount { get; }
    int CurSelectCount { get; }

    long GetWealth(ExChangeUseTabType id);

    //已经兑换了得米拉数量
    int RemainConvertMiraCount { get; }
}

public enum ExChangeUseTabType
{
    UseMira,
    UseDiamondAndBindDiamond,
    UserOnlyDiamond
}

public sealed partial class ExChangeMainDataMgr
{
    public sealed partial class ExChangeMainData:IExChangeMainData    {
        
        private int curSelectCount;
        private int expectItemId;
        private int curUseExChangeId;
        private ExChangeUseTabType curTab;
        private int _hasConvertMiraCount;
        //本次登录 是否显示 各快速兑换界面
        private Dictionary<int, bool> _isShowFastExView = new Dictionary<int, bool>()
        {
            {(int)AppVirtualItem.VirtualItemEnum.MIRA, true },
            {(int)AppVirtualItem.VirtualItemEnum.GOLD, true },
            {(int)AppVirtualItem.VirtualItemEnum.SILVER, true },
        };

        public Dictionary<int, bool> GetIsShowFastExView { get { return _isShowFastExView; } }

        public int CurSelectCount
        {
            get
            {
                return curSelectCount;
            }
        }
        public void SetSelectCount(int count)
        {
            curSelectCount = count;
        }
        public int ExpectTotalCount
        {
            get
            {
                return ExChangeHelper.GetConvertScale(CurUseExChangeId, ExchangeId) * CurSelectCount;
            }
        }

        public int ExchangeId
        {
            get
            {
                return expectItemId;
            }
        }
        public void SetExpectId(int id)
        {
            expectItemId = id;
        }

        public int CurUseExChangeId
        {
            get
            {
                if (CurTab == ExChangeUseTabType.UseDiamondAndBindDiamond)
                    curUseExChangeId = (int)AppVirtualItem.VirtualItemEnum.BINDDIAMOND;
                else if (CurTab == ExChangeUseTabType.UseMira)
                    curUseExChangeId = (int)AppVirtualItem.VirtualItemEnum.MIRA;
                else if (CurTab == ExChangeUseTabType.UserOnlyDiamond)
                    curUseExChangeId = (int)AppVirtualItem.VirtualItemEnum.DIAMOND;
                return curUseExChangeId;
            }
        }

        public ExChangeUseTabType CurTab
        {
            get
            {
                return curTab;
            }
        }
        public void SetCurTab(ExChangeUseTabType tab)
        {
            curTab = tab;
        }

        public int RemainConvertMiraCount
        {
            get
            {
                //计算公式 会改？ todo xjd
                return 1000 + 10 *(Math.Max(ModelManager.Player.GetPlayerLevel() - 50,0)) - _hasConvertMiraCount;
            }
        }
        public void SetHasConvertMiraCount(int count)
        {
            _hasConvertMiraCount = count;
        }

        public void InitData()
        {

        }

        public void Dispose()
        {

        }

        public long GetWealth(ExChangeUseTabType id)
        {
            long ownerMoney = 0;
            if (id == ExChangeUseTabType.UseDiamondAndBindDiamond)
                ownerMoney = ModelManager.Player.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.BINDDIAMOND) + ModelManager.Player.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.DIAMOND);
            else if (id == ExChangeUseTabType.UseMira)
                ownerMoney = ModelManager.Player.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.MIRA);
            else if (id == ExChangeUseTabType.UserOnlyDiamond)
                ownerMoney = ModelManager.Player.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.DIAMOND);
            return ownerMoney;
        }
    }
}
