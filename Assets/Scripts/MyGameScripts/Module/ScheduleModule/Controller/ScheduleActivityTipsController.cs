// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ScheduleActivityTipsController.cs
// Author   : xjd
// Created  : 1/23/2018 5:30:06 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppDto;
using System.Collections.Generic;

public partial interface IScheduleActivityTipsController
{
    void UpdateView(ActiveDto dto, ScheduleActivity itemData);
    void SetTipsPosition(UnityEngine.Vector3 pos);
}

public partial class ScheduleActivityTipsController
{ 
    public static IScheduleActivityTipsController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IScheduleActivityTipsController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IScheduleActivityTipsController;
            
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

    private List<ItemCellController> _itemCellCtrlList = new List<ItemCellController>();
    public void UpdateView(ActiveDto dto, ScheduleActivity itemData)
    {
        if (dto == null || itemData == null) return;

        UIHelper.SetOtherIcon(View.Icon_UISprite, itemData.icon);
        View.Name_UILabel.text = itemData.name;
        View.TimeLabel_UILabel.text = itemData.activityTime;
        View.LvLable_UILabel.text = string.Format("达到{0}级", itemData.openGrade == null ? 30 : itemData.openGrade.grade);
        View.FormLabel_UILabel.text = itemData.joinType;
        View.DescribeLabel_UILabel.text = itemData.desc;
        View.NumberLabel_UILabel.text = itemData.rewardCount == -1 ? "不限" : string.Format("{0}/{1}", dto.count, itemData.rewardCount);
        View.RewardLabel_UILabel.text = string.Format("{0}/{1}", dto.active, itemData.activeDegree);

        _itemCellCtrlList.ForEach(x => x.Hide());
        itemData.items.ForEachI((rewardId, index) =>
        {
            var ctrl = AddRewardItemIfNotExist(index);
            ctrl.UpdateView(ItemHelper.GetGeneralItemByItemId(rewardId));
            ctrl.Show();
        });

        View.RewardGrid_UIGrid.Reposition();
    }

    private ItemCellController AddRewardItemIfNotExist(int idx)
    {
        ItemCellController ctrl = null;
        _itemCellCtrlList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<ItemCellController, ItemCell>(View.RewardGrid_UIGrid.gameObject, ItemCell.NAME);
            _itemCellCtrlList.Add(ctrl);
        }

        return ctrl;
    }

    public void SetTipsPosition(UnityEngine.Vector3 pos)
    {
        View.Bg_UISprite.transform.localPosition = pos;
    }
}
