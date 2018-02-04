// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 7/1/2017 5:12:12 PM
// **********************************************************************

using AppDto;
using UniRx;

public static partial class PitchPutawayAgainViewLogic
{
    private static CompositeDisposable _disposable;
    private static StallGoodsDto _goodsDto;

    public static void Open(StallGoodsDto goodsDto)
    {
        // open的参数根据需求自己调整
	    var layer = UILayerType.SubModule;
        var ctrl = PitchPutawayAgainViewController.Show<PitchPutawayAgainViewController>(
            PitchPutawayAgainView.NAME
            , layer
            , true
            ,false);
        _goodsDto = goodsDto;
        InitReactiveEvents(ctrl);
    }

    private static void InitReactiveEvents(IPitchPutawayAgainViewController ctrl)
    {
        if (ctrl == null) return;
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
        {
            _disposable.Clear();
        }

        ctrl.SetViewInfo(_goodsDto);
        _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
        _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_=>CloseBtn_UIButtonClick()));
        _disposable.Add(ctrl.OnSoldOutBtn_UIButtonClick.Subscribe(_=>SoldOutBtn_UIButtonClick()));
        _disposable.Add(ctrl.GetPutawayHandler.Subscribe(data=>PutawayBtn_UIButtonClick(data)));
        _disposable.Add(ctrl.OnPriceAddBtn_UIButtonClick.Subscribe(_ =>
        {
            ctrl.UpdateCash(true);
        }));
        _disposable.Add(ctrl.OnPriceReduceBtn_UIButtonClick.Subscribe(_ =>
        {
            ctrl.UpdateCash(false);
        }));
    }

	private static void Dispose()
    {
        _disposable = _disposable.CloseOnceNull();
        OnDispose();
    }

    // 如果有自定义的内容需要清理，在此实现
    private static void OnDispose()
    {

    }

    private static void CloseBtn_UIButtonClick()
    {
        ProxyTrade.ClosePitchPutawayAgainView();
    }
    
    private static void SoldOutBtn_UIButtonClick()
    {
        TradeDataMgr.TradeNetMsg.SoldOut(_goodsDto.stallId);
        CloseBtn_UIButtonClick();
    }
    
    private static void PutawayBtn_UIButtonClick(IPutawayViewData data)
    {
        bool outtime = SystemTimeManager.Instance.GetUTCTimeStamp() > _goodsDto.expiredTime;
        if (!outtime)   //只允许重新上架超时物品,策划需求
            return;
        TradeDataMgr.TradeNetMsg.StallReup(data);
        CloseBtn_UIButtonClick();
    }
}

