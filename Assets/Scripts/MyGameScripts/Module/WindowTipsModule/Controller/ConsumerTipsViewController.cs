// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  ConsumerTipsViewController.cs
// Author   : willson
// Created  : 2015/1/19 
// Porpuse  : 
// **********************************************************************

using System;
using UnityEngine;
using AppDto;
using System.Collections.Generic;
using UniRx;

public struct ConsumerTipsViewData
{
    public string titleStr;
    public string contentStr;
    public bool isShowCheckBox;
    public string checkBoxLabel;
    public bool checkBoxState;
    public bool isOKClose;
    public string optBtnStr;
    
    public static ConsumerTipsViewData Create(
        string titleStr = ""
    , string contentStr = ""
    , bool isShowCheckBox = false
    , string checkBoxLabel = ""
    , bool checkBoxState = false
    , bool isOKClose = true
    , string optBtnStr = "确定")
    {
        var data = new ConsumerTipsViewData();
        data.titleStr = titleStr;
        data.contentStr = contentStr;
        data.isShowCheckBox = isShowCheckBox;
        data.checkBoxLabel = checkBoxLabel;
        data.checkBoxState = checkBoxState;
        data.isOKClose = isOKClose;
        data.optBtnStr = optBtnStr;
        return data;
    }
}

public class ConsumerTipsViewController : MonoViewController<ConsumerTipsView>
{
    public Action<bool> optCallback;
    public Action<bool> cancelCallback;

    protected override void AfterInitView()
    {
        
    }

    protected override void RegistCustomEvent()
    {
        EventDelegate.Set(View.CloseButton.onClick, OnClose);
        EventDelegate.Set(View.OptButton_UIButton.onClick, OnOptBtn);
    }

    /// <summary>
    /// Open the specified tips, items, ingot, OnOptBtnClick, OnDisposeHandle, saveMark and showLbl.
    /// </summary>
    /// <param name="tips">Tips.</param>
    /// <param name="items">Items.</param>
    /// <param name="ingot">Ingot. -1为不显示元宝价值    0为显示原价  >0显示可兑换</param></param>
    /// <param name="OnOptBtnClick">On opt button click.</param>
    /// <param name="OnDisposeHandle">On dispose handle.</param>
    /// <param name="saveMark">Save mark.</param>
    /// <param name="showLbl">Show lbl.</param>
    public void UpdateView(
        ConsumerTipsViewData data)
    {
        _view.optLabel_UILabel.text = data.optBtnStr;
        _view.TipsLabel.text = data.titleStr;
        _view.ExplainLabel_UILabel.gameObject.SetActive(!string.IsNullOrEmpty(data.contentStr));
        _view.ExplainLabel_UILabel.text = data.contentStr;

        var toggle = _view.SaveToggle_UIToggle; 
        toggle.gameObject.SetActive(data.isShowCheckBox);
        if (data.isShowCheckBox)
        {
            toggle.value = data.checkBoxState;
            _view.toggleLabel_UILabel.text = data.checkBoxLabel;
        }

        _view.optLabel_UILabel.text = data.optBtnStr;
        
        _view.ContentTable_UITable.Reposition();

        var bounds = NGUIMath.CalculateRelativeWidgetBounds (_view.ContentTable_UITable.transform);
        _view.ContentBg.height = (int)bounds.size.y + 100; 
        
    }
    
    private void OnOptBtn()
    {
        GameUtil.SafeRun<bool>(optCallback, _view.SaveToggle_UIToggle.value);
    }
    
    private void OnClose()
    {
        UIModuleManager.Instance.CloseModule(ConsumerTipsView.NAME);        
    }


    protected override void OnDispose()
    {
        
    }
}

