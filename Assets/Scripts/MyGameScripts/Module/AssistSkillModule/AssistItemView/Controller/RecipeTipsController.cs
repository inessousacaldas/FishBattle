// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RecipeTipsController.cs
// Author   : xjd
// Created  : 10/12/2017 3:15:01 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppDto;
using System.Collections.Generic;
using UnityEngine;

public partial interface IRecipeTipsController
{
    void InitTips(int id);

    void SetPosition(Vector3 position);
}
public partial class RecipeTipsController    {

    public static IRecipeTipsController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IRecipeTipsController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IRecipeTipsController;
            
        return controller;        
    }         
        
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

    public void InitTips(int id)
    {
        var clsData = DataCache.getArrayByCls<AssistSkillMakeConsume>();
        var itemData = clsData.Find(x => x.id == id);
        List<int> list = new List<int>();
        if (itemData != null)
        {
            View.Name_UILabel.text = itemData.name;

            itemData.gradeMaitchItem.ForEach(maitchItem =>
            {
                maitchItem.itemId.ForEach(itemid =>
                {
                    var ctrl = AddChild<RecipeTipsItemController, RecipeTipsItem>(View.Grid_UIGrid.gameObject, RecipeTipsItem.NAME);
                    ctrl.UpdateView(itemid);
                });
            });
        }

        View.Grid_UIGrid.Reposition();

        Bounds b = NGUIMath.CalculateRelativeWidgetBounds(View.Grid_UIGrid.transform);
        View.Bg_UISprite.SetDimensions(View.Bg_UISprite.width, (int)b.size.y + 40);
    }

    public void SetPosition(Vector3 position)
    {
        View.Bg_UISprite.transform.localPosition = position;
    }

    private void OnClose(GameObject go)
    {
        UIPanel panel = UIPanel.Find(go.transform);
        if (panel != View.GetComponent<UIPanel>())
            UIModuleManager.Instance.CloseModule(RecipeTips.NAME);
    }
}
