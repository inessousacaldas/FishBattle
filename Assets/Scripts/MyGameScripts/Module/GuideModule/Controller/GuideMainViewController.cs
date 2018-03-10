// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuideMainViewController.cs
// Author   : xjd
// Created  : 12/19/2017 11:25:11 AM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;

public partial interface IGuideMainViewController
{

}
public partial class GuideMainViewController    {

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

    private List<GuideItemViewController> _guideItemList = new List<GuideItemViewController>();
    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IGuideMainViewData data)
    {
        var itemCount = 0;
        _guideItemList.GetElememtsByRange(itemCount, -1).ForEach(s => s.Hide());

        if (data.GuideInfoList.IsNullOrEmpty())
        {
            View.EmptyPanel.SetActive(true);
            return;
        }
        View.EmptyPanel.SetActive(false);
        data.GuideInfoList.ForEachI((itemDto, index) =>
        {
            var itemCtrl = AddGuideItemIfNotExist(index);
            itemCtrl.UpdateView(itemDto);
            itemCtrl.Show();
        });

        View.Grid_UIGrid.Reposition();
    }

    private GuideItemViewController AddGuideItemIfNotExist(int idx)
    {
        GuideItemViewController ctrl = null;
        _guideItemList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<GuideItemViewController, GuideItemView>(View.Grid_UIGrid.gameObject, GuideItemView.NAME);
            _guideItemList.Add(ctrl);
        }

        return ctrl;
    }
}
