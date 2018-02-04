// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : CL-PC007
// Created  : 7/1/2017 10:17:33 AM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class TradeDataMgr
{
    public static class TradeNetMsg
    {
        #region 摆摊
        //打开界面
        //PlayerStallGoodsDto
        public static void OpenPitchView(Action callback)
        {
            GameUtil.GeneralReq(Services.Stall_Enter(), response =>
            {
                PlayerStallGoodsDto dto = response as PlayerStallGoodsDto;
                if (dto != null)
                {
                    DataMgr._data.UpdateCapability(dto.capability);
                    var time = SystemTimeManager.Instance.GetUTCTimeStamp();
                    DataMgr._data.PitchCDTime = (int)((dto.refreshTime - time) / 1000);    //开始倒计时
                    DataMgr._data.UpdateStallItems(dto.playerStallItems);
                    FireData();
                    callback();
                }
                else
                    GameDebuger.LogError("Stall_Enter协议返回数据有误=======");
            });
        }

        //购买摆摊商品
        public static void BuyPitchItem(IPitchItemData itemdata)
        {
            GameUtil.GeneralReq(Services.Stall_Buy(itemdata.GetDto.stallId, itemdata.GetDto.price, itemdata.GetNum), e =>
            {
                StallBuyDto dto = e as StallBuyDto;
                if (dto == null)
                {
                    GameDebuger.LogError("Stall_Buy返回数据有误,请检查");
                    return;
                }

                switch (dto.buyStateId)
                {
                    case (int)StallGoods.BuyStateEnum.Success:
                        TipManager.AddTip("购买成功");
                        break;
                    case (int)StallGoods.BuyStateEnum.Notenough:
                        TipManager.AddTip("剩余数量不足");
                        break;
                    case (int)StallGoods.BuyStateEnum.Notsell:
                        TipManager.AddTip("该物品售罄");
                        break;
                }
                DataMgr._data.UpdateStallGoodsDto(dto.stallGoodsDto);
                FireData();
            });
        }

        //增加摊位格子
        public static void OpenLockPitchitem()
        {
            if (DataMgr._data.Capability >= DataCache.GetStaticConfigValue(AppStaticConfigs.STALL_MAX_CAPABILITY, 10))
            {
                TipManager.AddTip("已达到摊位数量上限");
                return;
            }

            GameUtil.GeneralReq(Services.Stall_Expand(), response =>
            {
                TipManager.AddTip("解锁摊位成功");
                DataMgr._data.UpdateCapability(DataMgr._data.Capability + 1);
                FireData();
            });
        }

        //下架
        public static void SoldOut(long stallId)
        {
            GameUtil.GeneralReq(Services.Stall_Down(stallId), response =>
            {
                TipManager.AddTip("免费下架成功");
                DataMgr._data.SoldOutItem(stallId);
                FireData();
            });
        }

        //重新上架
        //PlayerStallGoodsDto
        public static void StallReup(IPutawayViewData data)
        {
            GameUtil.GeneralReq(Services.Stall_Reup(data.Getuid, data.GetPrice, data.GetItemId, data.GetItemCount),
                response =>
                {
                    PlayerStallGoodsDto dto = response as PlayerStallGoodsDto;
                    if (dto == null)
                    {
                        GameDebuger.LogError("Stall_Reup返回的数据有问题");
                        return;
                    }
                    DataMgr._data.UpdateStallItems(dto.playerStallItems);
                    FireData();
                });
        }

        //超时物品一键上架
        //PlayerStallGoodsDto
        public static void StallReupAll()
        {
            GameUtil.GeneralReq(Services.Stall_ReupAll(), response =>
            {
                PlayerStallGoodsDto dto = response as PlayerStallGoodsDto;
                if (dto == null)
                {
                    GameDebuger.LogError("Stall_ReupAll返回的数据有问题");
                    return;
                }
                TipManager.AddTip("上架成功");
                DataMgr._data.UpdateStallItems(dto.playerStallItems);
                FireData();
            });
        }

        //一键提现
        //StallCashDto
        public static void StallCash()
        {
            GameUtil.GeneralReq(Services.Stall_Cash(), response =>
            {
                StallCashDto stallCashDto = response as StallCashDto;
                if (stallCashDto != null)
                {
                    if (stallCashDto.stallIds.Count == 0)
                    {
                        TipManager.AddTip("没有银币可以提现，快点上架商品吧");
                        return;
                    }
                    TipManager.AddTip("提现成功");
                    stallCashDto.stallIds.ForEach(id =>
                    {
                        DataMgr._data.StallCash(id);
                    });
                    FireData();
                }
                else
                    GameDebuger.LogError("StallCash协议返回数据有误=======");
            });
        }

        //提现单个
        public static void StallOneItemCash(long stallId)
        {
            GameUtil.GeneralReq(Services.Stall_Cash4single(stallId), response =>
            {
                TipManager.AddTip("提现成功");
                DataMgr._data.StallCash(stallId);
                FireData();
            });
        }

        //批量上架
        //@param items 附件背包索引:单价:数量，多个以逗号分隔
        //PlayerStallGoodsDto
        public static void StallUp(string items)
        {
            GameUtil.GeneralReq(Services.Stall_Up(items), response =>
            {
                PlayerStallGoodsDto dto = response as PlayerStallGoodsDto;
                if (dto != null)
                {
                    TipManager.AddTip("上架成功");
                    DataMgr._data.UpdateStallItems(dto.playerStallItems);
                    ProxyTrade.ClosePitchSellView();
                    FireData();
                }
                else
                    GameDebuger.LogError("StallUp协议返回数据有误=======");
            });
        }

        //刷新摆摊界面
        //StallCenterDto
        public static void StallRefresh(int parentMenuId)
        {
            GameUtil.GeneralReq(Services.Stall_Refresh(parentMenuId), response =>
            {
                StallCenterDto centerDto = response as StallCenterDto;
                DataMgr._data.RefreshDataList(true);
                TipManager.AddTip("刷新成功");
                if (centerDto != null)
                {
                    DataMgr._data.UpdateStallCenterDto(parentMenuId, centerDto);
                    DataMgr._data.PitchCDTime = 300;
                    FireData();
                }
                else
                    GameDebuger.LogError("StallMenu协议返回数据有误=======");
            });
        }

        //刷新某一类物品数据
        //StallCenterDto
        public static void StallMenu(int parentMenuId)
        {
            if (DataMgr._data.HasDataMenuData(parentMenuId))    //使用缓存中的数据
                return;

            GameUtil.GeneralReq(Services.Stall_Menu(parentMenuId), response =>
            {
                StallCenterDto centerDto = response as StallCenterDto;
                DataMgr._data.RefreshDataList(menuId: parentMenuId);
                if (centerDto != null)
                    DataMgr._data.UpdateStallCenterDto(parentMenuId, centerDto);
                else
                    GameDebuger.LogError("StallMenu协议返回数据有误=======");
            });
        }
        #endregion

        #region 商会

        //打开商会界面
        //TradeCenterDto
        public static void OpenTradeView(Action callback)
        {
            GameUtil.GeneralReq(Services.Trade_Enter(), response =>
            {
                TradeCenterDto tradeCenterDto = response as TradeCenterDto;
                if (tradeCenterDto != null || tradeCenterDto.items == null)
                {
                    DataMgr._data.UpdateTradeData(tradeCenterDto.items);
                    if(callback != null)
                        callback();
                }
                else
                    GameDebuger.LogError("Trade_Enter协议返回数据有误=======");
            });
        }

        //购买商会商品
        public static void TradeBuy(int itemid, int count, int price)
        {
            GameUtil.GeneralReq(Services.Trade_Buy(itemid, count), response =>
            {
                TradeBuyPriceDto dto = response as TradeBuyPriceDto;
                var goods = DataCache.getDtoByCls<GeneralItem>(itemid);
                TipManager.AddTip(string.Format("花费{0}金币购买{1}个{2}",
                    dto.total.WrapColor(ColorConstantV3.Color_Green_Str),
                    dto.count.WrapColor(ColorConstantV3.Color_Green_Str), 
                    goods.name.WrapColor(ColorConstantV3.Color_Green_Str)));
                FireData();
            });
        }

        //出售商品
        public static void TradeSell(int itemIdx, int count)
        {
            GameUtil.GeneralReq(Services.Trade_Sell(itemIdx, count), response =>
            {
                TipManager.AddTip("出售成功");
                //FireData();
            });
        }

        public static void TradeExit()
        {
            GameUtil.GeneralReq(Services.Trade_Exit(), response => { });
        }
        #endregion

        public static void StallExit()
        {
            GameUtil.GeneralReq(Services.Stall_Exit());
        }
    }
}
