// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MisRewardTipViewController.cs
// Author   : DM-PC092
// Created  : 12/12/2017 11:42:28 AM
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;
using AppDto;

public partial interface IMisRewardTipViewController
{
}
public partial class MisRewardTipViewController    {

    private List<ItemCellController> tipItemList = new List<ItemCellController>();

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
        JSTimer.Instance.CancelCd(MisRewardTipDataMgr.MisRewardTipViewLogic.UpdateItemTipsInBattle);
        JSTimer.Instance.CancelCd(MisRewardTipDataMgr.MisRewardTipViewLogic.DelayItemTipsInBattle);
        JSTimer.Instance.CancelCd(MisRewardTipDataMgr.MisRewardTipViewLogic.DelayCommonTipsInBattle);
        tipItemList.Clear();
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IMisRewardTipData data){
        UpdateItemTips(data);
    }
    #region Tips

    public void UpdateItemTip(IMisRewardTipData data)
    {
        var dic = data.ItemTipDic;
        int id = data.ItemTipsNotify.itemTipsId;
        var val = dic[id];
        View.lblDes_UILabel.text = val.tips;
        View.lblTitle_UILabel.text = val.title;
        CreateTipsItem(data.ItemTipsNotify.itemDtos);
        View.rewardTrans_UIGrid.Reposition();
        JSTimer.Instance.SetupCoolDown("ItemTips", 5, null, HideTipsPanel);
    }
    public void UpdateItemTips(IMisRewardTipData data)
    {
        UpdateItemTip(data);
    }
    
    private void CreateTipsItem(List<ItemDto> list)
    {
        int poolCount = tipItemList.Count;
        int count = list.Count;
        for (int i = 0; i < poolCount; i++)
        {
            tipItemList[i].gameObject.SetActive(i < count);
            if (i < count)
            {
                tipItemList[i].UpdateView(list[i], null, 0, true);
            }
        }
        for (; poolCount < count; poolCount++)
        {
            var item = AddChild<ItemCellController, ItemCell>(
                View.rewardTrans_UIGrid.gameObject,
                ItemCell.Prefab_ItemCell
                );
            item.UpdateView(list[poolCount], null, 0, true);
            tipItemList.Add(item);
        }
    }
    public void HideTipsPanel()
    {
        JSTimer.Instance.CancelTimer("ItemTips");
        UIModuleManager.Instance.CloseModule(MisRewardTipView.NAME);
    }
    
    #endregion

}
