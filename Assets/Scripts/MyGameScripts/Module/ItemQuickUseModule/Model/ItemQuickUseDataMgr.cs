// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 3/3/2018 3:30:06 PM
// **********************************************************************

using AppDto;
using UniRx;

public sealed partial class ItemQuickUseDataMgr
{
    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<BagItemNotify>(ShowItemQuickUseView));
    }

    private void OnDispose()
    {

    }

    private void ShowItemQuickUseView(BagItemNotify notify)
    {
        if (notify.updateItems.Count > 0)
        {
            var list = notify.updateItems;
            list.ForEach(d =>
            {
                var props = DataCache.getDtoByCls<GeneralItem>(d.itemId) as Props;
                if(props != null && props.easyToUse)
                    DataMgr._data.AddItemInItemList(d);
            });
            if(DataMgr._data.CurItemDto != null)
                ProxyItemQuickUse.OpenItemQuickItemView();
        }
    }
}
