// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GMDtoConnViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************
using UniRx;
using System;
using System.Collections.Generic;
using Assets.Scripts.MyGameScripts.UI;

public partial class GMDtoConnViewController
{
    private List<GMDtoConnItemController> itemList = new List<GMDtoConnItemController>();
    private int curCount;
    public GenericCheckBoxController chbStartCtrl;
    public GenericCheckBoxController chbShieldCtrl;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        InitCheckBox();
    }

    private void InitCheckBox()
    {
        chbStartCtrl = AddController<GenericCheckBoxController, GenericCheckBox>(View.chbStart.gameObject);
        chbStartCtrl.UpdateView(GenericCheckBoxData.Create("开始打印协议",false));
        chbShieldCtrl = AddController<GenericCheckBoxController, GenericCheckBox>(View.chbShield.gameObject);
        chbShieldCtrl.UpdateView(GenericCheckBoxData.Create("屏蔽高频协议",true));

        //chbStartCtrl.ClickStateHandler.Subscribe();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {

    }

    protected override void OnDispose()
    {
        base.OnDispose();
        if(itemList.Count > 0)
        {
            ClearItem();
            itemList.Clear();
        }
        curCount = 0;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {

    }

    public void UpdateView(IGMData data)
    {
        curCount = 0;
        chbStartCtrl.IsSelect = GMDataMgr.isOpenDtoConn;
        chbShieldCtrl.IsSelect = data.DtoConnData.isShield;
        UpdateItem(data.DtoConnData.ListDto);
        UpdateInfo(data,data.DtoConnData.curCtrl);
        HideItem();
    }

    private void UpdateItem(List<GMDtoConnVO> list)
    {
        var row = 8;
        for(var i = 0;i < list.Count;i++)
        {
            GMDtoConnItemController itemCtrl;
            if(curCount < itemList.Count)
            {
                itemCtrl = itemList[curCount];
            }
            else
            {
                itemCtrl = AddChild<GMDtoConnItemController,GMDtoConnItem>(View.containerList_UIWidget.gameObject,GMDtoConnItem.NAME,GMDtoConnItem.NAME);
                itemCtrl.View.gameObject.name = GMDtoConnItem.NAME + curCount;
                GMDataMgr.GMDtoConnViewLogic.InitReactiveEvents(itemCtrl);
                itemList.Add(itemCtrl);
            }
            curCount++;
            itemCtrl.Show();
            itemCtrl.View.transform.localPosition = new UnityEngine.Vector3(248,-15 - i * 35);
            itemCtrl.UpdateView(list[i]);
        }
        View.containerList_UIWidget.height = curCount *35;
    }

    private void UpdateInfo(IGMData data,GMDtoConnItemController ctrl)
    {
        View.scrollViewRight_UIScrollView.ResetPosition();
        if(ctrl != null)
        {
            data.DtoConnData.stringBuilder = new System.Text.StringBuilder();
            var vo = ctrl.vo;
            var stringBuilder = data.DtoConnData.PrintProto(vo.dto);
            var strB = string.Format("协议名称：{0} 时间：{1} 大小：{2} \n{3}{4}",vo.Name,vo.time,vo.size,stringBuilder.ToString(),vo.stackTrace);
            View.lblInfo_UILabel.text = HtmlUtil.Font2(strB,GameColor.BLACK);
            View.lblInfo_UILabel.height = (int)View.lblInfo_UILabel.transform.localPosition.y + (int)View.lblInfo_UILabel.printedSize.y + 10;
            View.Container_UIWidget.height = View.lblInfo_UILabel.height;
        }
        else
        {
            View.lblInfo_UILabel.text = "";
        }
    }

    private void HideItem()
    {
        if(curCount < itemList.Count)
        {
            for(var i = curCount;i < itemList.Count;i++)
            {
                itemList[i].Hide();
            }
        }
    }

    private void ClearItem()
    {
        if(itemList.Count > 0)
        {
            for(var i = 0;i < itemList.Count;i++)
            {
                RemoveChild<GMDtoConnItemController,GMDtoConnItem>(itemList[i]);
            }
        }
    }

}