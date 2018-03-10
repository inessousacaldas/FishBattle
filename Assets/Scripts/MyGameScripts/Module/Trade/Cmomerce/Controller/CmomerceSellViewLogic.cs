// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 2/10/2018 4:21:07 PM
// **********************************************************************

using AppDto;
using UniRx;

public static partial class CmomerceSellViewLogic
{
    private static CompositeDisposable _disposable;

    public static void Open(BagItemDto dto)
    {
        // open的参数根据需求自己调整
	var layer = UILayerType.SubModule;
        var ctrl = CmomerceSellViewController.Show<CmomerceSellViewController>(
            CmomerceSellView.NAME
            , layer
            , true
            , false);
        InitReactiveEvents(ctrl);
        ctrl.SetSellItemInfo(dto);
    }

    private static void InitReactiveEvents(ICmomerceSellViewController ctrl)
    {
        if (ctrl == null) return;
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
        {
            _disposable.Clear();
        }
        _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ =>
        {
            ProxyTrade.CloseCmomerceSellView();
        }));
        _disposable.Add(ctrl.SellBtnHandler.Subscribe(itemData =>
        {
            if (itemData.GetItemNum == 0)
            {
                TipManager.AddTip("请选择数量");
                return;
            }
            TradeDataMgr.TradeNetMsg.TradeSell(itemData.GetItemId, itemData.GetItemNum);
            ProxyTrade.CloseCmomerceSellView();
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
}

