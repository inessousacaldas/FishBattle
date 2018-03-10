// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : CL-PC007
// Created  : 7/1/2017 10:17:33 AM
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AppDto;

public interface ITradeData
{
    TradeTab CurTab { get; }
    IPitchData PitchCtrl { get; }
    ICmomerceData CmomerceCtrl { get; }
    IPitchSellViewData SellViewCtrl { get; }
    int CmomerceGoodsId { get; }
    int CurPitchId { get; }
}

public interface ICmomerceData
{
    IEnumerable<TradeGoodsDto> GetTradeGoodsDto { get; }
}

//摆摊
public interface IPitchData
{
    IEnumerable<StallGoodsDto> GetStallItems { get; }
    TradeMenu CurStellMenu { get; }
    Dictionary<int, StallCenterDto> GetStallCenterDto { get; }
    int Capability { get; }
    IEnumerable<StallDrawNotify> GetStallDrawList { get; }
    int PitchCDTime { get; }
}

//批量上架
public interface IPitchSellViewData
{
    int Capability { get; }
}

public enum TradeTab
{
    Cmomerce = 0,
    Pitch = 1,
    Auction = 2,
    Sales = 3
}

public sealed partial class TradeDataMgr
{
    public sealed partial class TradeData
        :ITradeData
        ,IPitchData
        ,ICmomerceData
        ,IPitchSellViewData
    {
        public IPitchData PitchCtrl { get { return this; } }
        public ICmomerceData CmomerceCtrl { get { return this; } }
        public IPitchSellViewData SellViewCtrl { get { return this; } }
        public void InitData()
        {
            _tradeMenuList = DataCache.getArrayByCls<TradeMenu>();
            CurStellMenu = _tradeMenuList.Filter(d=>d.type == (int)TradeMenu.TradeMenuEnum.Stall).TryGetValue(0);    //默认选中第一项
        }

        public void Dispose()
        {

        }

        #region UpdateData

        #region 摆摊
        public void UpdateStallItems(List<StallGoodsDto> itemlist)
        {
            _stallItems = itemlist;
            SortStallItems();
        }

        public static Comparison<StallGoodsDto> _comparison = null;
        private void SortStallItems()
        {
            if (_comparison == null)
            {
                _comparison = (a, b) =>
                {
                    if (a.expiredTime == b.expiredTime)
                        return a.itemId - b.itemId;
                    return (int)(a.expiredTime - b.expiredTime);
                };
            }
            _stallItems.Sort(_comparison);
        }

        public void UpdateOrAddStallItems(StallGoodsDto goodsDto)
        {
            _stallItems.ReplaceOrAdd(d => d.stallId == goodsDto.stallId, goodsDto);
        }

        //下架
        public void SoldOutItem(long uid)
        {
            var goodsIdx = _stallItems.FindIndex(d => d.stallId == uid);
            if (goodsIdx >= 0)
                _stallItems.RemoveAt(goodsIdx);
        }

        //提现
        public void StallCash(long uid)
        {
            var goods = _stallItems.Find(d => d.stallId == uid);
            if (goods == null) return;
            if (goods.amount == 0)
            {
                var goodsIdx = _stallItems.FindIndex(d => d.stallId == uid);
                _stallItems.RemoveAt(goodsIdx);
            }
            else
            {
                goods.count = 0;
                UpdateOrAddStallItems(goods);
            }
        }

        public void UpdateStallCenterDto(int menuId , StallCenterDto dto)
        {
            var stalls = _stallCenterDto.Find(d => d.Key == menuId);
            if (stalls.Value == null)
                _stallCenterDto.Add(menuId, dto);
            else
                _stallCenterDto[menuId] = dto;
        }

        public void UpdateStallGoodsDto(StallGoodsDto dto)
        {
            var menu = DataCache.getDtoByCls<TradeMenu>(dto.item.tradeMenuId);
            if (menu == null)
            {
                TipManager.AddTip("请检查tradeMenu表中是否有dto.item.tradeMenuId");
                return;
            }
            _stallCenterDto[menu.parentId].items.ReplaceOrAdd(d => d.stallId == dto.stallId, dto);
        }

        public void UpdateCapability(int i)
        {
            _capability = i;
        }

        public void UpdateStallDrawList(StallDrawNotify notify)
        {
            _stallDrawList.ReplaceOrAdd(d=>d.stallId == notify.stallId, notify);
            var goods = _stallItems.Find(d => d.stallId == notify.stallId);
            goods.amount = notify.amount;
            goods.count += notify.count;
            UpdateOrAddStallItems(goods);
        }
        #endregion

        #region 商会

        public void UpdateTradeData(List<TradeGoodsDto> tradeGoods)
        {
            _tradeGoodsDto = tradeGoods;
        }

        public void UpdatTradeGoods(List<TradeGoodsDto> goodsDtoDtos)
        {
            goodsDtoDtos.ForEach(d =>
            {
                _tradeGoodsDto.ReplaceOrAdd(goods => goods.itemId == d.itemId, d);
            });
        }
        #endregion
        #endregion

        #region 主界面

        private int _curCmomerceGoodsId;
        public int CmomerceGoodsId
        {
            get { return _curCmomerceGoodsId; }
            set { _curCmomerceGoodsId = value; }
        }

        private int _curPitchId;

        public int CurPitchId
        {
            get { return _curPitchId; }
            set { _curPitchId = value; }
        }

        private IEnumerable<TradeMenu> _tradeMenuList; 
        private TradeTab _curTab = TradeTab.Cmomerce;

        public TradeTab CurTab
        {
            get { return _curTab; }
            set { _curTab = value; }
        }

        #endregion

        #region 商会

        private List<TradeGoodsDto> _tradeGoodsDto;
        public IEnumerable<TradeGoodsDto> GetTradeGoodsDto { get { return _tradeGoodsDto; } } 
        #endregion

        #region 摆摊
        private List<int> _hadDataMenuList = new List<int>();   //已经请求过数据的菜单编号list

        public void RefreshDataList(bool isClear = false, int menuId = 0)
        {
            if(isClear)
                _hadDataMenuList.Clear();
            else
                _hadDataMenuList.Add(menuId);
        }

        //该类商品是否请求过数据
        public bool HasDataMenuData(int menuId)
        {
            var idx = _hadDataMenuList.FindIndex(d=>d == menuId);
            return idx >= 0;
        }

        private int _capability;
        public int Capability { get { return _capability;} }
        private TradeMenu _curStellStellMenu;
        public TradeMenu CurStellMenu
        {
            get { return _curStellStellMenu; }
            set { _curStellStellMenu = value; }
        }

        private Dictionary<int, StallCenterDto> _stallCenterDto = new Dictionary<int, StallCenterDto>();
        public Dictionary<int, StallCenterDto> GetStallCenterDto { get { return _stallCenterDto; } }

        private List<StallGoodsDto> _stallItems; 
        public IEnumerable<StallGoodsDto> GetStallItems { get { return _stallItems; } } 

        private List<StallDrawNotify> _stallDrawList = new List<StallDrawNotify>();
        public IEnumerable<StallDrawNotify> GetStallDrawList { get { return _stallDrawList; } }  
        //5分钟的刷新冷却时间
        public readonly int StellRefreshCD = DataCache.GetStaticConfigValue(AppStaticConfigs.STALL_ITEM_REFRESH_TIME, 300);
        private int _pitchCDTime = 300;    
        public int PitchCDTime
        {
            get { return _pitchCDTime; }
            set { _pitchCDTime = value; }
        }
        #endregion
    }
    
    public enum PitchTab
    {
        BuyGroup = 0,
        SellGroup = 1
    }
}


