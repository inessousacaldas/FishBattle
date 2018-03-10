// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EquipmentInsetMedallionViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;
using UnityEngine;
using AppDto;


public partial interface IEquipmentInsetMedallionViewController
{
    IEquipmentChoiceContentController EquipmentChoiceCtrl { get; }

    UniRx.IObservable<SignItemViewController.SignClickEvent> OnMedallionPanelInsetStream { get; }

    UniRx.IObservable<bool> OnMedallionPenelIsOpenStream { get; }

    SmithItemCellController SmithItemCellCtrl { get; }
}

public partial class EquipmentInsetMedallionViewController
{
    private CompositeDisposable _disposable = null;

    public static Comparison<Transform> _comparison = null;

    EquipmentChoiceContentController _equipmentChoiceCtrl;
    EquipmentMedallionAttrContentCtrl oldAttrContentCtrl, newAttrContentCtrl;
    SmithItemCellController smithItemCell;
    MedallionPanelController _medallionPanelCtrl = null;


    Subject<SignItemViewController.SignClickEvent> medallionPenelInsetStream = new Subject<SignItemViewController.SignClickEvent>();
    Subject<bool> medallionPenelIsOpenStream = new Subject<bool>();
    

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        if(_disposable == null)
            _disposable = new CompositeDisposable();

        _equipmentChoiceCtrl = AddChild<EquipmentChoiceContentController, EquipmentChoiceContent>(View.EquipmentChoiceContent, EquipmentChoiceContent.NAME);
        smithItemCell = AddChild<SmithItemCellController, SmithItemCell>(View.MedallionIconAnchor, SmithItemCell.NAME);
        oldAttrContentCtrl = AddController<EquipmentMedallionAttrContentCtrl, EquipmentAttrContent>(View.LeftAttrContentAnchor);
        newAttrContentCtrl = AddController<EquipmentMedallionAttrContentCtrl, EquipmentAttrContent>(View.RightAttrContentAnchor);
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        
    }

    public void UpdateView(IEquipmentInsetMedallionViewData data)
    {
        //UpdatePropertyLeftPanel(data);
        //UpdatePropertyRightPanel(data);
        
        oldAttrContentCtrl.UpdateViewData(data.OldAttrContentVo);
        if (data.CurChoiceEquipment != null)
        {
            oldAttrContentCtrl.SetIconInfo(data.CurChoiceEquipment);
            oldAttrContentCtrl.SetIconActive(true);
            _equipmentChoiceCtrl.View.EmptyPanel.SetActive(false);
        }
        else
        {
            _equipmentChoiceCtrl.View.EmptyLabel_UILabel.text = "没有装备可以镶嵌纹章哦！";
            _equipmentChoiceCtrl.View.EmptyPanel.SetActive(true);
        }           

        newAttrContentCtrl.UpdateViewData(data.NewAttrContentVo);
        updateMedallionPanel(data);
        UpdateMiddlePanel(data);
        _equipmentChoiceCtrl.UpdateViewData(data.CurTab, data.EquipmentItems, data.CurChoiceEquipment);
    }

    public void updateMedallionPanel(IEquipmentInsetMedallionViewData data)
    {
        if (_medallionPanelCtrl == null)
        {
            _medallionPanelCtrl = AddChild<MedallionPanelController, MedallionPanel>(this.gameObject, MedallionPanel.NAME);

            _medallionPanelCtrl.transform.position = View.MedallionPanel_Transform.position;
            _disposable.Add(_medallionPanelCtrl.OnMedallionPanelStream.Subscribe(item =>
            {
                medallionPenelInsetStream.OnNext(item);
            }));

            _disposable.Add(_medallionPanelCtrl.OnMedallionPenelIsOpenStream.Subscribe(isOpen =>
            {
                medallionPenelIsOpenStream.OnNext(isOpen);
            }));
        }
        _medallionPanelCtrl.UpdateMedallionList(data);
    }

    //中间图标、按钮、属性刷新
    public void UpdateMiddlePanel(IEquipmentInsetMedallionViewData data)
    {
        smithItemCell.SetButtonEnable(true);
        if (data.CurSelMedallionBagDto == null)
            smithItemCell.SetEmpty();
        else
        {
            smithItemCell.UpdateViewData(data.CurSelMedallionBagDto.item);
        }    
    }

    protected override void OnDispose()
    {
        if (_disposable != null)
            _disposable.Dispose();
        _disposable = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }


    public UniRx.IObservable<SignItemViewController.SignClickEvent> OnMedallionPanelInsetStream { get { return medallionPenelInsetStream; } }
    public UniRx.IObservable<bool> OnMedallionPenelIsOpenStream { get { return medallionPenelIsOpenStream; } }
    public IEquipmentChoiceContentController EquipmentChoiceCtrl { get { return _equipmentChoiceCtrl; } }
    public SmithItemCellController SmithItemCellCtrl { get { return smithItemCell; } }
}
