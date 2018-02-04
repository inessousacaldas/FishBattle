// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GarandArenaReportViewController.cs
// Author   : xjd
// Created  : 12/14/2017 2:41:49 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppServices;
using AppDto;
using System.Collections.Generic;

public partial interface IGarandArenaReportViewController
{

}
public partial class GarandArenaReportViewController
{
    public static IGarandArenaReportViewController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IGarandArenaReportViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IGarandArenaReportViewController;
            
        return controller;        
    }         
        
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        GameUtil.GeneralReq(Services.Arena_Report(), resp =>
        {
            var dto = resp as ArenaReportsDto;
            UpdateView(dto);
        });
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        CloseBtn_UIButtonEvt.Subscribe(_ =>
        {
            UIModuleManager.Instance.CloseModule(GarandArenaReportView.NAME);
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

    private List<GarandArenaReportItemController> _ArenaReportItemList = new List<GarandArenaReportItemController>();
    public void UpdateView(ArenaReportsDto dto)
    {
        if (dto == null) return;
        var itemCount = 0;
        _ArenaReportItemList.GetElememtsByRange(itemCount, -1).ForEach(s => s.Hide());
        if (dto.reportDtos.IsNullOrEmpty())
        {
            View.Label_UILabel.enabled = true;
            return;
        }
            
        View.Label_UILabel.enabled = false;
        dto.reportDtos.ForEachI((itemDto, index) =>
        {
            var itemCtrl = AddReportItemIfNotExist(index);
            itemCtrl.UpdateView(itemDto);
            itemCtrl.Show();
        });

        View.Grid_UIGrid.Reposition();
    }

    private GarandArenaReportItemController AddReportItemIfNotExist(int idx)
    {
        GarandArenaReportItemController ctrl = null;
        _ArenaReportItemList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<GarandArenaReportItemController, GarandArenaReportItem>(View.Grid_UIGrid.gameObject, GarandArenaReportItem.NAME);
            _ArenaReportItemList.Add(ctrl);
        }

        return ctrl;
    }
}
