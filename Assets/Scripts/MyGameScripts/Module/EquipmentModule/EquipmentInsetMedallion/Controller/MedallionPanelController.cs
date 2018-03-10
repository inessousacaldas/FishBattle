// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MedallionPanelController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using AppDto;


public partial interface IMedallionPanelController
{
    UniRx.IObservable<SignItemViewController.SignClickEvent> OnMedallionPanelStream { get; }
}
public partial class MedallionPanelController
{

    public class MedallionPanelEvent
    {
        public bool isOpen;
        public SignItemViewController.SignClickEvent itemEvent;
    }

    private List<SignItemViewController> _medallionList = new List<SignItemViewController>();

    protected CompositeDisposable _disposable = new CompositeDisposable();

    Subject<SignItemViewController.SignClickEvent> medallionPenelStream = new Subject<SignItemViewController.SignClickEvent>();

    public UniRx.IObservable<SignItemViewController.SignClickEvent> OnMedallionPanelStream { get { return medallionPenelStream; } }

    Subject<bool> medallionPenelIsOpenStream = new Subject<bool>();

    public UniRx.IObservable<bool> OnMedallionPenelIsOpenStream  { get { return medallionPenelIsOpenStream ; } }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        UICamera.onClick += onClickOther;
    }

    protected override void OnDispose()
    {
        _disposable.Clear();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        medallionPenelStream = medallionPenelStream.CloseOnceNull();
        UICamera.onClick -= onClickOther;
    }


    public void UpdateMedallionList(IEquipmentInsetMedallionViewData data)
    {
        if (data.isOpenMedallionPanel)
            Show();
        else
            Hide();
        //this.gameObject.SetActive(data.isOpenMedallionPanel);

        if (!data.isOpenMedallionPanel)
        {
            return;
        }

        _disposable.Clear();
        _medallionList.ForEach(i => i.gameObject.SetActive(false));
        if (data.MedallionList.ToList().IsNullOrEmpty())
        {
            View.Label_UILabel.gameObject.SetActive(true);
            return;
        }

        View.Label_UILabel.gameObject.SetActive(false);

        int dif = data.MedallionList.ToList().Count - _medallionList.Count;
        if (dif > 0)
        {
            for (int i = 0; i < dif; i++)
            {
                var ctrl = AddChild<SignItemViewController, SignItemView>(View.Table_UITable.gameObject, SignItemView.NAME2);
                _medallionList.Add(ctrl);
            }
        }

        data.MedallionList.ToList().ForEachI((itemDto, index) =>
        {
            var ctrl = _medallionList[index];
            ctrl.gameObject.SetActive(true);
            ctrl.UpdateView(itemDto);

            //等级限制
            //if (ModelManager.Player.GetPlayerLevel() < (itemDto.item as MedallionProps).minGrade)
            //    ctrl.View.DisableBg_UISprite.gameObject.SetActive(true);

            _disposable.Add(ctrl.OnClickItemStream.Subscribe(item =>
            {
                medallionPenelStream.OnNext(item);
            }));
        });

        SetCurSelMedallionSpr(data.SelMedallionId);

        View.Table_UITable.Reposition();
    }

    public void SetCurSelMedallionSpr(long id)
    {
        _medallionList.ForEach(x =>
        {
            if (x.GetItemId() == id)
                x.View.BgSprite_UISprite.gameObject.SetActive(true);
            else
                x.View.BgSprite_UISprite.gameObject.SetActive(false);
        });
    }

    private void onClickOther(GameObject go)
    {
        if (!View.gameObject.activeInHierarchy)
            return;

        UIPanel panel = UIPanel.Find(go.transform);
        if (panel.transform.parent.gameObject  != View.gameObject)
            medallionPenelIsOpenStream.OnNext(false);
    }
}
