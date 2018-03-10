// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : CL-PC007
// Created  : 7/1/2017 10:17:33 AM
// **********************************************************************

using System.Collections.Generic;
using AppDto;

public sealed partial class TradeDataMgr
{
    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<StallGoodsDto>(HandleStallNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<TradeGoodsListNotify>(HandlerTradeGoodsNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<StallDrawNotify>(HandlerStallDrawNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<TradeCenterDto>(OnTradeCenterDtoNotify));
    }
    
    public void OnDispose(){
            
    }

    private void HandlerTradeGoodsNotify(TradeGoodsListNotify tradeGoods)
    {
        _data.UpdatTradeGoods(tradeGoods.tradeGoodsDtos);
        DataMgr._data.CmomerceGoodsId = 0;
        FireData();
    }

    //重新上架
    private void HandleStallNotify(StallGoodsDto goodsDto)
    {
        DataMgr._data.UpdateOrAddStallItems(goodsDto);
        FireData();
    }

    //上架中物品数量刷新
    private void HandlerStallDrawNotify(StallDrawNotify stallDrawNotify)
    {
        DataMgr._data.UpdateStallDrawList(stallDrawNotify);
        FireData();
    }

    private void OnTradeCenterDtoNotify(TradeCenterDto notify)
    {
        if (notify == null || notify.items == null)
        {
            GameDebuger.LogError("服务器下发数据有问题");
            return;
        }
        DataMgr._data.UpdateTradeData(notify.items);
        FireData();
    }

    public IEnumerable<TradeGoodsDto> GetTradeGoodsDto()
    {
        return _data.GetTradeGoodsDto;
    }
}
