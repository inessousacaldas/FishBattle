// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GMViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;

public partial interface IGMViewController
{
    TabbtnManager TabMgr { get; }
    void OnTabChange(GMDataTab index,IGMData data);
}
public partial class GMViewController
{
    private TabbtnManager tabMgr = null;
    private BaseView curView;

    private GMCodeViewController codeViewCtrl;
    private GMDtoConnViewController dtoConnViewCtrl;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        CreateTabItem();
    }

    private void CreateTabItem()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
                    View.tabBtn_UIGrid.gameObject
                    , TabbtnPrefabPath.TabBtnWidget_H2.ToString()
                    , "Tabbtn_" + i);
        ITabInfo[] tabInfoList =
                {
                    TabInfoData.Create((int)GMDataTab.Code, "GM指令"),
                    TabInfoData.Create((int)GMDataTab.Dto, "协议记录"),
                    TabInfoData.Create((int)GMDataTab.DtoConn, "虚拟协议"),
                };
        tabMgr = TabbtnManager.Create(tabInfoList,func);
    }

    public TabbtnManager TabMgr
    {
        get { return tabMgr; }
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {

    }

    protected override void OnDispose()
    {
        base.OnDispose();
        codeViewCtrl = null;
        dtoConnViewCtrl = null;
        tabMgr = null;
        curView = null;
        GMDataMgr.GMViewLogic.DisposeModule();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {

    }

    protected override void UpdateDataAndView(IGMData data)
    {
        UpdateView(data.MainTab,data);
    }

    public void UpdateView(GMDataTab index,IGMData data)
    {
        switch(index)
        {
            case GMDataTab.Code:
                codeViewCtrl.UpdateView(data);
                break;
            case GMDataTab.DtoConn:
                dtoConnViewCtrl.UpdateView(data);
                break;
        }
    }

    public void OnTabChange(GMDataTab index,IGMData data)
    {
        if(data.MainTab != index)
        {
            if(curView != null)
            {
                curView.Hide();
                curView = null;
            }
        }
        switch(index)
        {
            case GMDataTab.Code:
                if(codeViewCtrl == null)
                {
                    codeViewCtrl = AddChild<GMCodeViewController,GMCodeView>(View.Content_Transform.gameObject,GMCodeView.NAME,GMCodeView.NAME);
                    GMDataMgr.GMCodeViewLogic.InitReactiveEvents(codeViewCtrl);
                }
                curView = codeViewCtrl.View;
                UpdateView(index,data);
                break;
            case GMDataTab.DtoConn:
                if(dtoConnViewCtrl == null)
                {
                    dtoConnViewCtrl = AddChild<GMDtoConnViewController,GMDtoConnView>(View.Content_Transform.gameObject,GMDtoConnView.NAME,GMDtoConnView.NAME);
                    GMDataMgr.GMDtoConnViewLogic.InitReactiveEvents(dtoConnViewCtrl);
                }
                curView = dtoConnViewCtrl.View;
                UpdateView(index,data);
                break;
        }
        if(curView != null)
        {
            curView.Show();
        }
    }
}
