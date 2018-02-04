// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PreviewTipsMainViewController.cs
// Author   : Zijian
// Created  : 9/1/2017 4:20:00 PM
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


/// <summary>
/// 装备打造成功的预览
/// </summary>

#region 数据模型,这里放的都是带对比的

#endregion
public partial interface IPreviewTipsMainViewController
{
    List<PreviewContainerController> ContainerCtrls { get; }
    void ShowEquipmentView(Pv_EquipmentVo vo_1, Pv_EquipmentVo vo_2);
    PreviewContainerController ShowEquipmentView(Pv_EquipmentVo vo);
    PreviewContainerController ShowPreviewEquipment(Equipment equipment, int quality);

    PreviewContainerController ShowBagItemPreview(BagItemDto dto);
}
public partial class PreviewTipsMainViewController    {

    
    List<PreviewContainerController> ctrls = new List<PreviewContainerController>();
    public List<PreviewContainerController> ContainerCtrls { get { return ctrls; } }
    public static IPreviewTipsMainViewController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IPreviewTipsMainViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IPreviewTipsMainViewController;
            
        return controller;        
    }
    //默认的高度~
    private float DefalutH = 243;

	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
            
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        UICamera.onClick += OnClose;   
    }

    protected override void RemoveCustomEvent ()
    {
        UICamera.onClick -= OnClose;
    }
    protected override void OnDispose()
    {
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    /// <summary>
    /// 对按钮的初始化，不用就设置用Null就好
    /// </summary>
    /// <param name="leftBtns"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    private PreviewContainerController BeforeShowView()
    {
        var ctrl = AddCachedChild<PreviewContainerController, PreviewContainer>(View.Content, PreviewContainer.NAME);
        //ctrl.InitButtonView(leftBtns, right);
        ctrls.Add(ctrl);
        return ctrl;
    }
    //装备展示
    public void ShowEquipmentView(Pv_EquipmentVo vo_1, Pv_EquipmentVo vo_2)
    {
        //如果身上没有装备
        if (vo_1 == null)
        {
            var ctrl = BeforeShowView();
            ctrl.MakeEquipmentTips(vo_2);
            ctrl.transform.localPosition = new Vector2(ctrl.transform.localPosition.x, DefalutH);
        }
        else if( vo_2 == null)
        {
            var ctrl = BeforeShowView();
            ctrl.MakeEquipmentTips(vo_1);
            ctrl.transform.localPosition = new Vector2(ctrl.transform.localPosition.x, DefalutH);
        }
        //如果身上有这个部位的已装备位置，则同时显示2个
        else if(vo_1 != null && vo_2 != null)
        {
            var ctrl = BeforeShowView();
            ctrl.MakeEquipmentTips(vo_1);
            float space = ctrl.GetWidth()/2 + 10;
            ctrl.transform.localPosition = new Vector2(ctrl.transform.localPosition.x -space, DefalutH);
            var ctrl_smith = BeforeShowView();
            ctrl_smith.MakeEquipmentTips(vo_2);
            ctrl_smith.transform.localPosition = new Vector2(ctrl_smith.transform.localPosition.x + space, DefalutH);
        }

        //View.Content_UITable.Reposition();
    }
    public PreviewContainerController ShowEquipmentView(Pv_EquipmentVo vo)
    {
        var ctrl = BeforeShowView();
        ctrl.MakeEquipmentTips(vo);
        return ctrl;
    }
    public PreviewContainerController ShowPreviewEquipment(Equipment equipment,int quality)
    {
        var ctrl = BeforeShowView();
        ctrl.MakePreviewEquipment(equipment, quality);
        //float space = ctrl.GetWidth() / 2 + 10;
        ctrl.transform.localPosition = new Vector2(ctrl.transform.localPosition.x, DefalutH);
        //ctrl.transform.localPosition = new Vector2(ctrl.transform.localPosition.x - space, DefalutH);
        //var ctrl2 = BeforeShowView();
        //ctrl2.MakePreviewEquipment(equipment, quality);
        //ctrl2.transform.localPosition = new Vector2(ctrl2.transform.localPosition.x + space, DefalutH);

        //ctrl.OnButtonClickStream.Subscribe(i => {
        //    switch((PvBtnType)i)
        //    {
        //        case PvBtnType.Equip:
        //            GameLog.LogEquipment("装备");
        //            break;
        //        case PvBtnType.Split:
        //            GameLog.LogEquipment("分解");
        //            break;
        //    }
        //});
        //ctrl2.OnButtonClickStream.Subscribe(i =>
        //{
        //    switch ((PvBtnType)i)
        //    {
        //        case PvBtnType.Equip:
        //            GameLog.LogEquipment("装备2");
        //            break;
        //        case PvBtnType.Split:
        //            GameLog.LogEquipment("分解2");
        //            break;
        //    }
        //});
        return ctrl;
    }

    public PreviewContainerController ShowBagItemPreview(BagItemDto dto)
    {
        var ctrl = BeforeShowView();
        ctrl.MakeBagItemPreview(dto);
        return ctrl;
    }
    private void OnClose(GameObject go)
    {
        UIPanel panel = UIPanel.Find(go.transform);
        if (panel != View.transform.GetComponent<UIPanel>())
            UIModuleManager.Instance.CloseModule(PreviewTipsMainView.NAME);
    }
}

#region 废弃
//public class PreviewContainer_EquipmentVo
//{

//    /// <summary>
//    /// 穿着
//    /// </summary>
//    public Pv_EquipmentVo currentEquipVo;
//    /// <summary>
//    /// 新鲜打造~
//    /// </summary>
//    public Pv_EquipmentVo smithEquipVo;

//    public PreviewContainer_EquipmentVo() { }
//    public PreviewContainer_EquipmentVo(Pv_EquipmentVo currentEquipment, Pv_EquipmentVo smithEquipment)
//    {
//        this.currentEquipVo = currentEquipment;
//        this.smithEquipVo = smithEquipment;
//    }
//    /// <summary>
//    /// 获取浮动的数据~
//    /// </summary>
//    public int GetPowerFloat()
//    {
//        var power = smithEquipVo.equipmentDto.equipExtraDto.power - currentEquipVo.equipmentDto.equipExtraDto.power;
//        return power;
//    }
//}
#endregion