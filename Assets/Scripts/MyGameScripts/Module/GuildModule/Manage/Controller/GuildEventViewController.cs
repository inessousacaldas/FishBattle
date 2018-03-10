// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildEventViewController.cs
// Author   : DM-PC092
// Created  : 1/24/2018 11:23:01 AM
// Porpuse  : 
// **********************************************************************

using AppDto;
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

    private IEnumerable<GuildEventDto> _messageDtoList = null;

    private Dictionary<GameObject, GuildEventItemCellController> _messageItemDic = new Dictionary<GameObject, GuildEventItemCellController>();


    private int _messageItemMax = 11;

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
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
        {
            _disposable.Clear();
        }
        for (int i = 0; i < _messageItemMax; i++)
        {
            var ctrl = AddChild<GuildEventItemCellController, GuildEventItemCell>(
                _view.MessageContent_UIRecycledList.gameObject
                , GuildEventItemCell.NAME);
            _messageItemDic.Add(ctrl.gameObject, ctrl);
        }
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
        _messageItemList.Clear();
        _messageDtoList = null;
        _messageItemDic.ForEach(e =>
        {
            Destroy(e.Key);
        });
        _messageItemDic.Clear();
        _disposable = _disposable.CloseOnceNull();
        base.OnDispose();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {

    }

    public void UpdateMessageItem(IEnumerable<GuildEventDto> messageInfoDto)
    {
        _messageDtoList = messageInfoDto;
        View.MessageContent_UIRecycledList.UpdateDataCount(messageInfoDto.ToArray().Length, true);
    }
    

    private void UpdateEventItem(GameObject go, int itemIndex, int dataIndex)
    {
        if (_messageItemDic == null || _messageDtoList == null) return;
        GuildEventItemCellController item = null;
        if (_messageItemDic.TryGetValue(go, out item))
        {
            var info = _messageDtoList.TryGetValue(dataIndex);
            if (info == null) return;
            item.SetData(info);
        }
    }
}
