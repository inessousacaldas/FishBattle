// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PropertyTipController.cs
// Author   : xjd
// Created  : 9/15/2017 5:25:21 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using UnityEngine;

public partial interface IPropertyTipController
{

}
public partial class PropertyTipController    {

    public static IPropertyTipController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IPropertyTipController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IPropertyTipController;
            
        return controller;        
    }         
        
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

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    //type 1->一级属性 2->二级属性
    //#id -1代表玩家,非0代表伙伴
    //#personality代表性格
    public void Init(PlayerPropertyTipType type, int[] propertyArr, int id = 0, int personality = -1) 
    {
        //TODO 职业系统
        if (type == PlayerPropertyTipType.BaseType)
            View.TitleLabel_UILabel.text = "基础属性效果";
        else
            View.TitleLabel_UILabel.text = "战斗属性效果";

        propertyArr.ForEach(d =>
        {
            var com = AddChild<PropertyTipItemController, PropertyTipItem>(View.Table_UITable.gameObject, PropertyTipItem.NAME);
            com.Init(type, d , id < 0);
        });

        if (personality >= 0)
        {
            var com = AddChild<PropertyTipItemController, PropertyTipItem>(View.Table_UITable.gameObject, PropertyTipItem.NAME);
            com.InitPersonality(personality);
        }
        View.Table_UITable.Reposition();
    }

    public void AddMagicprops(int magicId, string typeStr="", int factionId = 0)
    {
        var ctrl = AddChild<PropsMagicItemController, PropsMagicItem>(View.Table_UITable.gameObject, PropsMagicItem.NAME);
        ctrl.UpdateView(magicId, typeStr, factionId);
        ctrl.HideProDescLb(true);
        View.Table_UITable.Reposition();
    }

    public void AddMagicCrewProps(int magicId, string faction)
    {
        var ctrl = AddChild<PropsMagicItemController, PropsMagicItem>(View.Table_UITable.gameObject, PropsMagicItem.NAME);
        ctrl.UpdateView(magicId, faction);
        ctrl.HideProDescLb(false);
        View.Table_UITable.Reposition();
    }

    public void SetPostion(Vector3 pos)
    {
        View.Bg_UISprite.transform.localPosition = pos;
    }
}
