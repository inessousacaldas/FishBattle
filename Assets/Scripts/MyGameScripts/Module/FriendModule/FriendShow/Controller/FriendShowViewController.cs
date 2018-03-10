// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FriendShowViewController.cs
// Author   : xjd
// Created  : 10/18/2017 11:22:44 AM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using UnityEngine;
using AppDto;

public partial interface IFriendShowViewController
{
    void UpdateView(FriendDynamicNotify notify, string friendName);
}

public partial class FriendShowViewController
{
    private long _id = 0;

    public static IFriendShowViewController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IFriendShowViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IFriendShowViewController;
            
        return controller;        
    }         
        
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
            
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        UICamera.onClick += OnClose;

        EggBtn_UIButtonEvt.Subscribe(_ =>
        {
            TipManager.AddTip("丢鸡蛋");
            FriendDataMgr.FriendNetMsg.ReqActionFriend((int)FriendActionNotify.FriendActionEnum.Egg, _id);

            Close();
        });

        SupportBtn_UIButtonEvt.Subscribe(_ =>
        {
            TipManager.AddTip("点赞");
            FriendDataMgr.FriendNetMsg.ReqActionFriend((int)FriendActionNotify.FriendActionEnum.Like, _id);

            Close();
        });

        FlowerBtn_UIButtonEvt.Subscribe(_ =>
        {
            TipManager.AddTip("打开鲜花界面");

            Close();
        });
    }

    protected override void RemoveCustomEvent ()
    {
        UICamera.onClick -= OnClose;
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    public void UpdateView(FriendDynamicNotify notify, string friendName)
    {
        _id = notify.playerId;
        string des = "你的好友{0}等级提高到{1}级。";

        switch ((FriendDynamicNotify.FriendDynamicEnum)notify.dynamicId)
        {
            case FriendDynamicNotify.FriendDynamicEnum.PlayerGrade:

                View.Name_UILabel.text = friendName;
                View.Des_UILabel.text = string.Format(des, friendName, notify.dynamicParam);

                break;
        }
    }

    public void OnClose(GameObject go)
    {
        var panel = UIPanel.Find(go.transform);
        if (panel != View.GetComponent<UIPanel>())
            Close();
    }

    public void Close()
    {
        UIModuleManager.Instance.CloseModule(FriendShowView.NAME);
    }
}
