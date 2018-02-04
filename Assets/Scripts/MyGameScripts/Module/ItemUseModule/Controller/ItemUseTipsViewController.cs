// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ItemUseTipsViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;

public class ItemUseTipsCellVo
{
    public GeneralItem item;
    public int needCount;
    public long hadCount;
}
public partial class ItemUseTipsViewController
{
    public static void Open(
        IEnumerable<ItemUseTipsCellVo> itemSet
        , string title
        , string content = ""
        , Action onOKCallback = null
        , Action onCancelCallback = null)
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<ItemUseTipsViewController>(
            ItemUseTipsView.NAME
            , UILayerType.ThreeModule
            , true
            , false);

        ctrl.UpdateView(itemSet, title, content, onOKCallback, onCancelCallback);
    }

    private Action onOKClick;
    public Action OnOKClick {
        set { onOKClick = value; }
    }

    private Action onCancelClick;
    public Action OnCancelClick {
        set { onCancelClick = value; }
    }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {

    }

    // 客户端自定义代码
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {
        _disposable.Dispose();
    }

    private void OptBtn_UIButtonClickHandler()
    {
        GameUtil.SafeRun(onOKClick);
        //UIModuleManager.Instance.CloseModule(ItemUseTipsView.NAME);
    }
    private void CloseBtn_UIButtonClickHandler()
    {
        GameUtil.SafeRun(onCancelClick);
        UIModuleManager.Instance.CloseModule(ItemUseTipsView.NAME);
    }

    private void UpdateView(
        IEnumerable<ItemUseTipsCellVo> itemSet
        , string title
        , string content = ""
        , Action onOKCallback = null
        , Action onCancelCallback = null)
    {

        itemSet.ForEachI((x,i)=> {
            var ctrl = AddCachedChild<ItemCellController, ItemCell>(
                View.ItemGrid.gameObject
                , ItemCell.Prefab_ItemUseCell
                , string.Format("{0}_{1}", ItemCell.NAME, i)
            );
            ctrl.UpdateView_ItemUse(x.item,(int)x.hadCount,x.needCount);
        });

        View.SetTitle(title);
        View.SetContent(content);
        onOKClick = onOKCallback;
        onCancelClick = onCancelCallback;


        View.ItemGrid.Reposition();
    }

}
