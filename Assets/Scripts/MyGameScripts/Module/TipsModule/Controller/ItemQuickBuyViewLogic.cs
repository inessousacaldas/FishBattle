// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 1/3/2018 2:46:44 PM
// **********************************************************************

using UniRx;

public sealed partial class ItemQuickBuyDataMgr
{
    public static partial class ItemQuickBuyViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open(int itemId)
        {
            // open的参数根据需求自己调整
            var layer = UILayerType.DefaultModule;
            var ctrl = ItemQuickBuyViewController.Show<ItemQuickBuyViewController>(
                ItemQuickBuyView.NAME
                , layer
                , true
                ,false,
                Stream);
            DataMgr._data.ItemId = itemId;
            InitReactiveEvents(ctrl);
        }

        private static void InitReactiveEvents(IItemQuickBuyViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }

            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => CloseBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnReduceBtn_UIButtonClick.Subscribe(_ => ctrl.OnReduceBtnHandler()));
            _disposable.Add(ctrl.OnAddBtn_UIButtonClick.Subscribe(_ => ctrl.OnAddBtnHandler()));
            _disposable.Add(ctrl.OnMaxBtn_UIButtonClick.Subscribe(_ => ctrl.OnMaxBtnHandler()));
            _disposable.Add(ctrl.OnBuyBtnHandler.Subscribe(data => BuyBtn_UIButtonClick(data.GetGoodsId, data.GetCount)));

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
            UIModuleManager.Instance.CloseModule(ItemQuickBuyView.NAME);
        }

        private static void BuyBtn_UIButtonClick(int goodsId, int count)
        {
            ItemQuickBuyNetMsg.QuickBuyItem(goodsId, count);
        }
    }
}

