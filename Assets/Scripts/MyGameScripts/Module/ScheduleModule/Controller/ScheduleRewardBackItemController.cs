// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ScheduleRewardBackItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UniRx;
using AppDto;

public partial class ScheduleRewardBackItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        NormalBackBtn_UIButtonEvt.Subscribe(_ =>
        {

        });

        PrefectBackBtn_UIButtonEvt.Subscribe(_ =>
        {

        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private List<ItemCellController> _itemCellCtrlList = new List<ItemCellController>();
    public void UpdateView()
    {
        _itemCellCtrlList.ForEach(item => { item.Hide(); });
        //_typeBtns.ForEachI((btnItem, index) =>
        //{
        //    var ctrl = AddRewardItemIfNotExist(index);
        //    ctrl.UpdateView();
        //    ctrl.Show();
        //});
    }

    private ItemCellController AddRewardItemIfNotExist(int idx)
    {
        ItemCellController ctrl = null;
        _itemCellCtrlList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<ItemCellController, ItemCell>(View.Grid_UIGrid.gameObject, ItemCell.NAME);
            _itemCellCtrlList.Add(ctrl);
        }

        return ctrl;
    }
}
