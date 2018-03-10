// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 3/3/2018 3:30:06 PM
// **********************************************************************

using System;
using AppDto;
using UniRx;

public sealed partial class ItemQuickUseDataMgr
{
    
    public static partial class ItemQuickUseViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
	    var layer = UILayerType.FourModule;
            var ctrl = ItemQuickUseViewController.Show<ItemQuickUseViewController>(
                ItemQuickUseView.NAME
                , layer
                , false
                , false
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IItemQuickUseViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.UseBtnHandler.Subscribe(dto=> UseBtn_UIButtonClick(dto)));
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_=>CloseBtn_UIButtonClick()));
            _disposable.Add(LayerManager.Stream.Subscribe(mode =>
            {
                if(mode == UIMode.BATTLE)
                    ProxyItemQuickUse.HideItemQuickItemView();
                else
                    ProxyItemQuickUse.OpenItemQuickItemView();
            }));
        }
            
        private static void Dispose()
        {
            _disposable = _disposable.CloseOnceNull();
            DataMgr._data.ClearItemList();
            OnDispose();    
        }
        
        // 如果有自定义的内容需要清理，在此实现
        private static void OnDispose()
        {
            
        }

        private static void UseBtn_UIButtonClick(BagItemDto dto)
        {
            switch ((AppItem.ItemTypeEnum)dto.item.itemType)
            {
                case AppItem.ItemTypeEnum.Equipment:
                    ItemQuickUseNetMsg.ApplyEquipMent(dto);
                    break;
                default:
                    ItemQuickUseNetMsg.ApplyBackpack(dto);
                    break;
            }
        }

        private static void CloseBtn_UIButtonClick()
        {
            ProxyItemQuickUse.CloseItemQuickItemView();
        }
    }
}

