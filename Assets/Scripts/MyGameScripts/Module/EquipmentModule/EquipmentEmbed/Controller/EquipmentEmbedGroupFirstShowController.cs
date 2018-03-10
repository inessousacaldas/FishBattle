// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EquipmentEmbedGroupFirstShowController.cs
// Author   : Zijian
// Created  : 10/25/2017 9:41:34 AM
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
using UniRx;

public partial interface IEquipmentEmbedGroupFirstShowController
{
    void InitData(int curEmbedPhase, int lastEmbedPhase);
}
public partial class EquipmentEmbedGroupFirstShowController    {
        

    public static void Show(int curPhase,int lastPhase)
    {
        var ctrl = Show<EquipmentEmbedGroupFirstShowController>(EquipmentEmbedGroupFirstShow.NAME,UILayerType.DefaultModule,true,true);
        ctrl.InitData(curPhase, lastPhase);
    }
    public static IEquipmentEmbedGroupFirstShowController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IEquipmentEmbedGroupFirstShowController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IEquipmentEmbedGroupFirstShowController;
            
        return controller;        
    }


    List<EmbebGroupFristShowAttrController> attrCtrls = new List<EmbebGroupFristShowAttrController>();
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
    }
    public void InitData(int curEmbedPhase,int lastEmbedPhase)
    {
        var curPhase = DataCache.getDtoByCls<EmbedPhase>(curEmbedPhase);
        List<CharacterPropertyDto> lastPropertys = new List<CharacterPropertyDto>();

        View.MainLabel_UILabel.text = "成功激活宝石" + curPhase+"阶套装";
        var CurProperttyGroup = curPhase.propertty.Split(',');
        var lastPhase = DataCache.getDtoByCls<EmbedPhase>(lastEmbedPhase);
        if (lastPhase != null)
        {
            var LastPropettyGroup = lastPhase.propertty.Split(',');
            LastPropettyGroup.ForEachI((props, i) =>
            {
                var prop = props.Split(':');
                int id = StringHelper.ToInt(prop[0]);
                int value = StringHelper.ToInt(prop[1]);
                CharacterPropertyDto property = new CharacterPropertyDto() { propId = id, propValue = value };
                lastPropertys.Add(property);
            });
        }
        
        CurProperttyGroup.ForEachI((props, i) => {
            var prop = props.Split(':');
            int id = StringHelper.ToInt(prop[0]);
            int value = StringHelper.ToInt(prop[1]);
            EmbebGroupFristShowAttrController ctrl = null;
            if (!attrCtrls.TryGetValue(i, out ctrl)) {
                ctrl = AddChild<EmbebGroupFristShowAttrController, EmbebGroupFristShowAttr>(View.AttrContent_UIGrid.gameObject, EmbebGroupFristShowAttr.NAME);
            }
            var lastProperty = lastPropertys.Find(x => x.propId == id);
            if (lastProperty != null)
                ctrl.UpdateData(id, (int)lastProperty.propValue, value);
            else
                ctrl.UpdateData(id, value, -1);
        });
        View.AttrContent_UIGrid.Reposition();
    }
}
