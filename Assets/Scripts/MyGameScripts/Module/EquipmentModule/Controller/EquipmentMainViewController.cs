// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EquipmentMainViewController.cs
// Author   : Zijian
// Created  : 8/29/2017 8:21:33 PM
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
using UniRx;

public partial interface IEquipmentMainViewController
{
    TabbtnManager TabbtnMgr { get; }

    //初始化子Ctrl事件
    event Action<EquipmentViewTab, IMonolessViewController> OnChildCtrlAdd;
}
public partial class EquipmentMainViewController    {


    TabbtnManager tabbtnMgr;
    public TabbtnManager TabbtnMgr { get { return tabbtnMgr; } }
    public event Action<EquipmentViewTab, IMonolessViewController> OnChildCtrlAdd;

    EquipmentSmithController smithCtrl;
	EquipmentResetController resetCtrl;
    EquipmentInsetMedallionViewController medallionCtrl;
    EquipmentEmbedController embedCtrl;
    IMonolessViewController curCtrl;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
        OnChildCtrlAdd = null;
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
                    View.TabBtn_1
                    , TabbtnPrefabPath.TabBtnWidget.ToString()
                    , "Tabbtn_" + i);
        tabbtnMgr = TabbtnManager.Create(EquipmentMainDataMgr.EquipmentMainData._TabInfos, func);
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IEquipmentMainData data){
        var lastCtrl = curCtrl;
        switch (data.CurTab)
        {
            case EquipmentViewTab.Smith:
                if(smithCtrl == null)
                {
                    smithCtrl = AddChild<EquipmentSmithController, EquipmentSmith>(View.MainContent, EquipmentSmith.NAME);
                    OnChildCtrlAdd(EquipmentViewTab.Smith, smithCtrl);
                }
                smithCtrl.UpdateViewData(data.SmithViewData);
                curCtrl = smithCtrl;
                break;
			case EquipmentViewTab.EquipmentMedallion:
                if (medallionCtrl == null)
                {
                    medallionCtrl = AddChild<EquipmentInsetMedallionViewController, EquipmentInsetMedallionView>(View.MainContent, EquipmentInsetMedallionView.NAME);
                    OnChildCtrlAdd(EquipmentViewTab.EquipmentMedallion, medallionCtrl);
                }
                medallionCtrl.UpdateView(data.MedallionViewData);
                curCtrl = medallionCtrl;
                break;
            case EquipmentViewTab.EquipmentReset:
                if(resetCtrl == null)
                {
                    resetCtrl = AddChild<EquipmentResetController, EquipmentReset>(View.MainContent, EquipmentReset.NAME);
                    OnChildCtrlAdd(EquipmentViewTab.EquipmentReset, resetCtrl);
                }
                resetCtrl.UpdateDataView(data.ResetViewData);
                curCtrl = resetCtrl;
                break;
            case EquipmentViewTab.EquipmentEmbed:
                if(embedCtrl == null)
                {
                    embedCtrl = AddChild<EquipmentEmbedController, EquipmentEmbed>(View.MainContent, EquipmentEmbed.NAME);
                    OnChildCtrlAdd(EquipmentViewTab.EquipmentEmbed, embedCtrl);
                }
                embedCtrl.UpdateViewData(data.EmbedViewData);
                curCtrl = embedCtrl;
                break;
        }

        if (lastCtrl == curCtrl)
            return;
        else
        {
            if(lastCtrl != null)
            lastCtrl.Hide();
            curCtrl.Show();
        }
    }

}
