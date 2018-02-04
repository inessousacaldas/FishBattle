// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BracerLvUpViewController.cs
// Author   : xjd
// Created  : 11/9/2017 5:19:35 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;
using AppDto;
using UnityEngine;

public partial interface IBracerLvUpViewController
{

}
public partial class BracerLvUpViewController    {

    public static IBracerLvUpViewController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IBracerLvUpViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IBracerLvUpViewController;
            
        return controller;        
    }


    private List<BracerLvUpItemController> _bracerLvUpCtrlList = new List<BracerLvUpItemController>();
    private Dictionary<int, BracerGrade> _bracerLvGradeDic = DataCache.getDicByCls<BracerGrade>();

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        UpdateView();
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

    public void UpdateView()
    {
        
        var grade = ModelManager.Player.GetBracerGrade;
        if (!_bracerLvGradeDic.ContainsKey(grade))
            return;

        View.Icon_UISprite.spriteName = _bracerLvGradeDic[grade].icon;
        View.LvName_UILabel.text = _bracerLvGradeDic[grade].name;

        var itemCount = 0;
        _bracerLvUpCtrlList.GetElememtsByRange(itemCount, -1).ForEach(s => s.Hide());

        _bracerLvGradeDic[grade].attrId.ForEachI((id, index) =>
        {
            var itemCtrl = AddLvUpItemIfNotExist(index);
            itemCtrl.UpdateView(id, _bracerLvGradeDic[grade].attrAdd[index]);
            itemCtrl.Show();
        });

        View.Grid_UIGrid.Reposition();
    }

    private BracerLvUpItemController AddLvUpItemIfNotExist(int idx)
    {
        BracerLvUpItemController ctrl = null;
        _bracerLvUpCtrlList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<BracerLvUpItemController, BracerLvUpItem>(View.Grid_UIGrid.gameObject, BracerLvUpItem.NAME);
            _bracerLvUpCtrlList.Add(ctrl);
        }

        return ctrl;
    }

    public void OnClose(GameObject go)
    {
        var sprite = go.GetComponent<UISprite>();
        if (sprite == View.ShadowBg_UISprite)
            UIModuleManager.Instance.CloseModule(BracerLvUpView.NAME);
    }
}
