// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 9/1/2017 11:01:48 AM
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///    预览提示的展示方法~
///    =============================ReadMe=============================
///     额外的PreviewTips主要用来处理可能同时存在多个预览页面的情况
///     如果你想单个独自操作~可以直接使用PreviewContainerControll ~
/// </summary>

public enum PvBtnType
{
    Lock,//加锁
    Sell,//出售
    Split,//分解
    Strange,//进阶
    Equip,//装备
    TakeOff,//脱下
    Use,//使用
}
public static class PreviewShowHelper
{

    /// <summary>
    /// 所有存在的Tips按钮
    /// </summary>
    static Dictionary<PvBtnType, Pv_ButtonVo> pvButtonType = new Dictionary<PvBtnType, Pv_ButtonVo>()
    {
        { PvBtnType.Lock,    new Pv_ButtonVo((int)PvBtnType.Lock,"加锁") },
        { PvBtnType.Sell,    new Pv_ButtonVo((int)PvBtnType.Sell,"摆摊") },
        { PvBtnType.Split,   new Pv_ButtonVo((int)PvBtnType.Split,"分解") },
        { PvBtnType.Strange, new Pv_ButtonVo((int)PvBtnType.Strange,"进阶") },
        { PvBtnType.Equip,   new Pv_ButtonVo((int)PvBtnType.Equip,"装备") },
        { PvBtnType.TakeOff, new Pv_ButtonVo((int)PvBtnType.TakeOff,"卸下") },
        { PvBtnType.Use,     new Pv_ButtonVo((int)PvBtnType.TakeOff,"使用") }
    };
    public static Pv_ButtonVo GetPvBtn(PvBtnType type)
    {
        Pv_ButtonVo res = null;
        pvButtonType.TryGetValue(type, out res);
        return res;
    }
    /// <summary>
    /// 展示装备提示
    /// </summary>
    /// <param name="vo_1">原本的</param>
    /// <param name="vo_2">对比项</param>
    public static IPreviewTipsMainViewController ShowEquipment(Pv_EquipmentVo vo_1,Pv_EquipmentVo vo_2)
    {
        var ctrl = PreviewTipsMainViewController.Show<PreviewTipsMainViewController>(PreviewTipsMainView.NAME, UILayerType.ThreeModule, false);
        ctrl.ShowEquipmentView(vo_1,vo_2);
        return ctrl;
    }
    /// <summary>
    /// 展示装备预览
    /// </summary>
    /// <param name="equipment"></param>
    /// <param name="quality"></param>
    public static void ShowPreviewEquipment(Equipment equipment,int quality)
    {
        var ctrl = PreviewTipsMainViewController.Show<PreviewTipsMainViewController>(PreviewTipsMainView.NAME, UILayerType.ThreeModule, false);
        ctrl.ShowPreviewEquipment(equipment,quality);
    }

    /// <summary>
    /// 展示背包里面的
    /// </summary>
    public static void ShowBagItemPreview(BagItemDto dto,Rect targetRect)
    {
        var ctrl = PreviewTipsMainViewController.Show<PreviewTipsMainViewController>(PreviewTipsMainView.NAME, UILayerType.ThreeModule, false);
        ctrl.ShowBagItemPreview(dto);
        ctrl.ContainerCtrls[0].SetPosWithAnchor(targetRect);
    }
    public static void Close()
    {
        UIModuleManager.Instance.CloseModule(PreviewTipsMainView.NAME);
    }
}

