// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ShopItemTipsViewController.cs
// Author   : Zijian
// Created  : 8/22/2017 10:13:49 AM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public partial interface IShopItemTipsViewController
{
    void ShowInit(GameObject target, string name, string icon, string des);
    UniRx.IObservable<Unit> GetLeftBtnHandler { get; }
    UniRx.IObservable<Unit> GetRightBtnHandler { get; }
    void SetBtnName(string leftName, string rightName);
}
public partial class ShopItemTipsViewController
{
    private CompositeDisposable _disposable;
    #region
    private Subject<Unit> _leftBtnClickEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> GetLeftBtnHandler { get { return _leftBtnClickEvt; } }
    private Subject<Unit> _rightBtnClickEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> GetRightBtnHandler { get { return _rightBtnClickEvt; } }  
    #endregion  
    public static IShopItemTipsViewController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IShopItemTipsViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IShopItemTipsViewController;

        return controller;
    }

    private Vector2 curSize;
    private Vector2 screenSize;
    public enum TipsPos
    {
        Left = 0,
        Right,
        Top,
        Bottom,
    }
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        _disposable = new CompositeDisposable();

        curSize = new Vector2(View.ShopItemContent_UIWidget.width, View.ShopItemContent_UIWidget.height);
        //TODO:注意时间消耗，待优化
        var root = GetComponentInParent<UIRoot>();
        screenSize = new Vector2(root.manualWidth, root.activeHeight);

        _view.LeftBtn_UIButton.gameObject.SetActive(false);
        _view.RightBtn_UIButton.gameObject.SetActive(false);
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {
        _disposable.Add(CloseMask_UIButtonEvt.Subscribe(_ => { OnClickClose(); }));
        _disposable.Add(LeftBtn_UIButtonEvt.Subscribe(_ =>
        {
            OnClickClose();
            _leftBtnClickEvt.OnNext(new Unit());
        }));
        _disposable.Add(RightBtn_UIButtonEvt.Subscribe(_ =>
        {
            OnClickClose();
            _rightBtnClickEvt.OnNext(new Unit());
        }));
    }

    protected override void RemoveCustomEvent()
    {

    }
    private void OnClickClose()
    {
        UIModuleManager.Instance.CloseModule(ShopItemTipsView.NAME);
    }
    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        base.OnDispose();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {

    }

    public void ShowInit(GameObject target,
        string name,
        string icon,
        string des)
    {
        View.Name_UILabel.text = name;
        SetIconSprite(icon);
        View.Des_UILabel.text = des;

        var anchor = View.ShopItemContent_UIAnchor;
        anchor.container = target;
        var targetWidget = target.GetComponent<UIWidget>();
        //先根据目的计算一次位置，再根据位置判定是否超出屏幕外
        anchor.pixelOffset = new Vector2(curSize.x / 2 + targetWidget.width / 2,-curSize.y /2 + targetWidget.height / 2);
        anchor.Update();
        
        var pos = View.ShopItemContent_UIWidget.transform.localPosition;
        float curTop = pos.y + curSize.y / 2;
        float curBottom = pos.y - curSize.y / 2;
        float curRight = pos.x + curSize.x / 2;

        float topOffset = (screenSize.y / 2 - curTop);
        float bottomOffset = (-screenSize.y / 2 - curBottom);
        float rightOffset = (screenSize.x / 2 - curRight);

        if (rightOffset < 0)
        {
            anchor.pixelOffset = new Vector2(-anchor.pixelOffset.x, anchor.pixelOffset.y);
            anchor.Update();
        }
        if (topOffset < 0)
        {
            anchor.pixelOffset = new Vector2(anchor.pixelOffset.x, anchor.pixelOffset.y + topOffset);
            anchor.Update();
        }

        if(bottomOffset > 0)
        {
            anchor.pixelOffset = new Vector2(anchor.pixelOffset.x, anchor.pixelOffset.y + bottomOffset);
            anchor.Update();
        }
    }

    public void SetBtnName(string leftName, string rightName)
    {
        _view.LeftBtn_UIButton.gameObject.SetActive(leftName != string.Empty);
        _view.RightBtn_UIButton.gameObject.SetActive(rightName != string.Empty);
        _view.LeftBtn_UIButton.sprite.UpdateAnchors();
        _view.RightBtn_UIButton.sprite.UpdateAnchors();
        _view.LeftBtnName_UILabel.text = leftName;
        _view.RightBtnName_UILabel.text = rightName;
    }

    private void SetIconSprite(string icon)
    {
        var list = icon.Split(':');
        if (list.Length > 1)
            UIHelper.SetItemIcon(View.Icon_UISprite, list[1]);
        else
            UIHelper.SetItemIcon(View.Icon_UISprite, icon);
    }
}
