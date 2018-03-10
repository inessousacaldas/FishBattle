// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GoodsItemTipsMainViewController.cs
// Author   : xjd
// Created  : 9/6/2017 3:14:28 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using UnityEngine;
using System.Collections.Generic;

public partial interface IGoodsItemTipsMainViewController
{
    void SetTitle(string icon, string quality, string name, string lv);

    void SetLabelView(string str);

    void SetRuneView(string str, List<string> list);

    void SetBtnView(string left, string right, Action leftClick = null, Action rightClick = null);

    void SetBtnView(Dictionary<string, Action> leftDic, Dictionary<string, Action> rightDic);

    void SetLineView();
}
public partial class GoodsItemTipsMainViewController    {

    public static IGoodsItemTipsMainViewController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IGoodsItemTipsMainViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IGoodsItemTipsMainViewController;
            
        return controller;        
    }         
        
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        //goodsItemTips  test
        //var ctrl = GoodsItemTipsMainViewController.Show<GoodsItemTipsMainViewController>(GoodsItemTipsMainView.NAME, UILayerType.ThreeModule, false, true);
        //ctrl.SetTitle("icon", "name", "quality", "99");

        //ctrl.SetLabelView("攻击力：+99999");

        //ctrl.SetLineView();

        //List<string> list2 = new List<string>();
        //for (int i = 0; i < 1; i++)
        //{
        //    list2.Add(string.Format("{0}qwer", i));
        //}
        //ctrl.SetRuneView(list);

        //ctrl.SetLineView();

        //Dictionary<string, Action> mm = new Dictionary<string, Action>();
        //mm.Add("确定", null);
        //mm.Add("确定1", null);
        //mm.Add("确定2", OnClickLeft);
        //Dictionary<string, Action> nn = new Dictionary<string, Action>();
        //nn.Add("取消", null);
        //nn.Add("取消1", OnClickRight);
        //nn.Add("取消2", null);
        //nn.Add("取消3", null);
        //nn.Add("取消4", OnClickRight);
        //ctrl.SetBtnView(mm, nn);
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

    public void SetTitle(string icon, string quality, string name, string lv)
    {
        var ctrl = AddChild<GoodsTipsTitleViewController, GoodsTipsTitleView>(View.Table_UITable.gameObject, GoodsTipsTitleView.NAME);
        ctrl.UpdateView(icon,quality,name,lv);

        View.Table_UITable.Reposition();
    }

    public void SetLabelView(string str)
    {
        var ctrl = AddChild<GoodsTipsLabelItemController, GoodsTipsLabelItem>(View.Table_UITable.gameObject, GoodsTipsLabelItem.NAME);
        ctrl.UpdateView(str);

        View.Table_UITable.Reposition();
    }

    public void SetRuneView(string str, List<string> list)
    {
        var ctrl = AddChild<GoodsTipsRuneViewController, GoodsTipsRuneView>(View.Table_UITable.gameObject, GoodsTipsRuneView.NAME);
        ctrl.UpdateView(str,list);

        View.Table_UITable.Reposition();
    }

    public void SetBtnView(string left, string right, Action leftClick=null, Action rightClick=null)
    {
        var ctrl = AddChild<GoodsTipsBtnPanelViewController, GoodsTipsBtnPanelView>(View.Table_UITable.gameObject, GoodsTipsBtnPanelView.NAME);
        ctrl.UpdateView(left, right, leftClick, rightClick);

        View.Table_UITable.Reposition();

        Bounds b = NGUIMath.CalculateRelativeWidgetBounds(View.Table_UITable.transform);

        View.Bg_UISprite.SetDimensions((int)b.size.x, (int)b.size.y);
    }

    public void SetBtnView(Dictionary<string, Action> leftDic, Dictionary<string, Action> rightDic)
    {
        var ctrl = AddChild<GoodsTipsBtnPanelViewController, GoodsTipsBtnPanelView>(View.Table_UITable.gameObject, GoodsTipsBtnPanelView.NAME);
        ctrl.UpdateView(leftDic, rightDic);

        View.Table_UITable.Reposition();

        Bounds b = NGUIMath.CalculateRelativeWidgetBounds(View.Table_UITable.transform);

        View.Bg_UISprite.SetDimensions((int)b.size.x, (int)b.size.y);
    }

    public void SetLineView()
    {
        var ctrl = AddChild<GoodsTipsLineController, GoodsTipsLine>(View.Table_UITable.gameObject, GoodsTipsLine.NAME);

        View.Table_UITable.Reposition();
    }

    private void OnClose(GameObject go)
    {
        UIPanel panel = UIPanel.Find(go.transform);
        if (panel != View.GetComponent<UIPanel>())
            UIModuleManager.Instance.CloseModule(GoodsItemTipsMainView.NAME);
    }
}
