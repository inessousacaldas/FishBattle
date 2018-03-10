// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  DecomposeGainPropsViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;
using AppDto;
// todo fish:指派给xush ，清理无用代码 ，改为独立界面
public partial class DecomposeGainPropsViewController
{
    private int cellCount = 0;

    private List<ItemCellController> gainItemCtrlList = new List<ItemCellController>();

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        //InitGainItems();
        for(int i=0;i< View.GainItemGroup.transform.childCount;i++)
        {
            var ctrl = AddController<ItemCellController, ItemCell>(View.GainItemGroup.transform.GetChild(i).gameObject);
            if (ctrl != null)
                gainItemCtrlList.Add(ctrl);
        }
        
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    //用特定接口类型数据刷新界面
    public void UpdateView(IBackpackData data)
    {
        if (data == null
            || data.CompositeViewData == null
            || data.CompositeViewData.GainDtoList == null)
            return;

        //if (gainItemCtrlList == null) InitGainItems();
        //var idx = 0;

        gainItemCtrlList.ForEach(x => x.Hide());
        data.CompositeViewData.GainDtoList.ForEachI((dto, i) =>
        {
            ItemCellController ctrl;
            ctrl = gainItemCtrlList[i];
            var gItem = ItemHelper.GetGeneralItemByItemId(dto.id);
            ctrl.UpdateView(gItem,dto.count);
            ctrl.SetNameLabel(gItem.name);
            ctrl.Show();
        });
        View.GainItemGroup_UIGrid.Reposition();

        //for (var i = idx; i < gainItemCtrlList.Count; i++)
        //{
        //    gainItemCtrlList[i].Hide();
        //}
    }

    private void InitGainItems()
    {
        gainItemCtrlList = new List<ItemCellController>();
        for (var i = 0; i < cellCount; i++)
        {
            var ctrl = AddChild<ItemCellController, ItemCell>(
                _view.GainItemGroup, ItemCell.NAME
                );
            gainItemCtrlList.Add(ctrl);
            ctrl.Hide();
        }
    }
}