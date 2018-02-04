// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FlowerReceiveViewController.cs
// Author   : xjd
// Created  : 1/12/2018 6:24:24 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppDto;

public partial interface IFlowerReceiveViewController
{
    void UpdateView(FriendFlowersNotify notify);
}

public partial class FlowerReceiveViewController    {

    public static IFlowerReceiveViewController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IFlowerReceiveViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IFlowerReceiveViewController;
            
        return controller;        
    }         
        
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
            
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        GiveBackBtn_UIButtonEvt.Subscribe(_ =>
        {
            ProxyFlowerMainView.Open(_friendInfoDto);
            UIModuleManager.Instance.CloseModule(FlowerReceiveView.NAME);
        });

        ThxBtn_UIButtonEvt.Subscribe(_ =>
        {
            PrivateMsgDataMgr.DataMgr.SendMessageWithoutView(_friendId, _friendName, "感谢你送的鲜花。");
            UIModuleManager.Instance.CloseModule(FlowerReceiveView.NAME);
            TipManager.AddTip("感谢成功");
        });
    }

    protected override void RemoveCustomEvent ()
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

    private long _friendId = 0;
    private string _friendName = string.Empty;
    private FriendInfoDto _friendInfoDto = new FriendInfoDto();
    public void UpdateView(FriendFlowersNotify notify)
    {
        var itemData = ItemHelper.GetGeneralItemByItemId(notify.itemId);
        if (itemData == null) return;
        _friendId = notify.fromId;
        _friendName = notify.fromName;
        View.Content_UILabel.text = string.Format("玩家{0}向你赠送{1}朵{2}，并对你说：", notify.fromName, notify.flowersCount, itemData.name);
        View.WishLabel_UILabel.text = notify.content;

        if (FriendDataMgr.DataMgr.GetFriendDtoById(notify.fromId) != null)
            _friendInfoDto = FriendDataMgr.DataMgr.GetFriendDtoById(notify.fromId);
        else
        {
            _friendInfoDto.friendId = notify.fromId;
            _friendInfoDto.name = notify.fromName;
        }
                
    }
}
