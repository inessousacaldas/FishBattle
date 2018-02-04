// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildEventViewController.cs
// Author   : DM-PC092
// Created  : 1/24/2018 11:23:01 AM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public partial interface IGuildEventViewController
{

}
public partial class GuildEventViewController
{
    private CompositeDisposable _disposable = new CompositeDisposable();

    private List<GuildEventItemCellController> _messageItemList = new List<GuildEventItemCellController>();

    private List<messageInfoDto> _messageDtoList = new List<messageInfoDto>();

    private Dictionary<GameObject, GuildEventItemCellController> _messageItemDic = new Dictionary<GameObject, GuildEventItemCellController>();


    private int _messageItemMax = 9;

    public static IGuildEventViewController Show<T>(
          string moduleName
          , UILayerType layerType
          , bool addBgMask
          , bool bgMaskClose = true)
          where T : MonoController, IGuildEventViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IGuildEventViewController;

        return controller;
    }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {

    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {
        View.MessageContent_UIRecycledList.onUpdateItem = UpdateEventItem;
        _disposable.Add(OnCloseBtn_UIButtonClick.Subscribe(_ => { UIModuleManager.Instance.CloseModule(GuildEventView.NAME); }));
        
    }

    protected override void RemoveCustomEvent()
    {
    }

    protected override void OnDispose()
    {
        base.OnDispose();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {

    }

    public void UpdateMessageItem(List<messageInfoDto> messageInfoDto)
    {
        _messageDtoList = messageInfoDto;

        InitMessageItem();
    }

    private void InitMessageItem()
    {
        if (_messageItemDic.Count == 0)
        {
            for (int i = 0; i < _messageItemMax; i++)
            {
                var ctrl = AddChild<GuildEventItemCellController, GuildEventItemCell>(
                    _view.MessageContent_UIRecycledList.gameObject
                    , GuildEventItemCell.NAME);
                _messageItemDic.Add(ctrl.gameObject, ctrl);
            }
        }
        UpdateScrollViewPos(_messageDtoList);
    }

    private void UpdateEventItem(GameObject go, int itemIndex, int dataIndex)
    {
        if (_messageItemDic == null) return;
        GuildEventItemCellController item = null;
        if (_messageItemDic.TryGetValue(go, out item))
        {
            var info = _messageDtoList.TryGetValue(dataIndex);
            if (info == null) return;
            item.SetData(info);
        }
    }
    public void UpdateScrollViewPos(IEnumerable<messageInfoDto> ShopItems)
    {
        View.MessageContent_UIRecycledList.UpdateDataCount(ShopItems.ToList().Count, true);
        View.ScrollView_UIScrollView.ResetPosition();
        //View.ScrollView_UIScrollView.transform.localPosition = new Vector3(80, -18);
    }
}
