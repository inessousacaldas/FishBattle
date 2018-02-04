// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ShortCutMessageViewController.cs
// Author   : Cilu
// Created  : 11/14/2017 10:31:50 AM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public partial interface IShortCutMessageViewController
{
    UniRx.IObservable<int> OnClickDeleteStream { get; }
    UniRx.IObservable<int> OnClickMsgStream { get; }
    UniRx.IObservable<Unit> OnClickAddStream { get; }
}
public partial class ShortCutMessageViewController
{
    List<ShortCutMessageItemController> shortCutMessageCtrls = new List<ShortCutMessageItemController>();

    Subject<Unit> _OnClickAddStream = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnClickAddStream { get { return _OnClickAddStream; } }

    Subject<int> _onClickDeleteStream = new Subject<int>();
    public UniRx.IObservable<int> OnClickDeleteStream { get { return _onClickDeleteStream; } }

    Subject<int> _onClickMsgStream = new Subject<int>();
    public UniRx.IObservable<int> OnClickMsgStream { get { return _onClickMsgStream; } }

    CompositeDisposable mydisposable = new CompositeDisposable();
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {

    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {
        UICamera.onClick += OnCameraClick;
    }

    private void OnCameraClick(GameObject go)
    {
        var panel = UIPanel.Find(go.transform);
        if (go.name == "MaskBg" || panel != this.GetComponent<UIPanel>() && panel != View.ItemScrollView_UIScrollView.GetComponent<UIPanel>())
        {
            UIModuleManager.Instance.CloseModule(ShortCutMessageView.NAME);
        }
    }

    protected override void RemoveCustomEvent()
    {
        UICamera.onClick -= OnCameraClick;
    }

    protected override void OnDispose()
    {
        base.OnDispose();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {

    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IShortCutMessageViewData data)
    {
        switch (data.CurSelect)
        {
            case ShortCutMessageEnum.ShorCut:
                View.PlayerAnimContent_UIPageGrid.gameObject.SetActive(false);
                View.ShotCurMsgContent_UIPageGrid.gameObject.SetActive(true);

                UpdateShortCutMessage(data.shortCutMessageList, data.isEdit);
                break;
            case ShortCutMessageEnum.PlayerAnim:
                View.PlayerAnimContent_UIPageGrid.gameObject.SetActive(true);
                View.ShotCurMsgContent_UIPageGrid.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    private void UpdateShortCutMessage(List<ShortCutMessage> data,bool isEdit) {
        mydisposable.Clear();
        data.ForEachI((x, i) => {
            if(shortCutMessageCtrls.Count <= i)
            {
                var ctrl = AddChild<ShortCutMessageItemController, ShortCutMessageItem>(View.ShotCurMsgContent_UIPageGrid.gameObject, ShortCutMessageItem.NAME);
                shortCutMessageCtrls.Add(ctrl);
            }
            var shorCutCtrl = shortCutMessageCtrls[i];
            shorCutCtrl.UpdateView(x,isEdit);

            mydisposable.Add(shorCutCtrl.OnDeleteBtn_UIButtonClick.Subscribe(_=>_onClickDeleteStream.OnNext(x.Id)));
            mydisposable.Add(shorCutCtrl.OnShortCutMessageItem_UIButtonClick.Subscribe(_ => { _onClickMsgStream.OnNext(x.Id); }));
            mydisposable.Add(shorCutCtrl.OnAddBtn_UIButtonClick.Subscribe(_ => { _OnClickAddStream.OnNext(new Unit()); }));
        });
        View.ShotCurMsgContent_UIPageGrid.Reposition();
    }
}
