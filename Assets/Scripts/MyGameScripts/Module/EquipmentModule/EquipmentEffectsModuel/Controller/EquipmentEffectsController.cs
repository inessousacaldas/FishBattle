// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EquipmentSpecialEffectController.cs
// Author   : Zijian
// Created  : 9/22/2017 3:49:56 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;
using AppDto;

public partial interface IEquipmentEffectsController
{
    UniRx.IObservable<EquipmentDto> OnChoiceEquipmentStream { get; }
    UniRx.IObservable<GeneralItem> OnClickGoodsStream { get; }
}
public partial class EquipmentEffectsController
{
    EquipmentChoiceContentController equipmentChoiceCtrl;
    Eq_GoodsChoiceContentController eq_goodsChoiceCtrl;

    public UniRx.IObservable<EquipmentDto> OnChoiceEquipmentStream { get { return equipmentChoiceCtrl.OnChoiceStream; } }
    public UniRx.IObservable<GeneralItem> OnClickGoodsStream { get { return eq_goodsChoiceCtrl.OnClickGoodsStream; } }
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        equipmentChoiceCtrl = AddChild<EquipmentChoiceContentController, EquipmentChoiceContent>(View.EquipmentChoiceContent, EquipmentChoiceContent.NAME);
        eq_goodsChoiceCtrl = AddChild<Eq_GoodsChoiceContentController, Eq_GoodsChoiceContent>(View.RightChoiceContent, Eq_GoodsChoiceContent.NAME);
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
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IEquipmentEffectsData data){
        eq_goodsChoiceCtrl.UpdateView(data.PropsList);
        equipmentChoiceCtrl.UpdateViewData(data.CurTab,data.EquipmentItems,data.CurChoiceEquipment);
    }
}
