// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildDonateViewController.cs
// Author   : DM-PC092
// Created  : 3/7/2018 2:48:22 PM
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;
using System.Linq;
using AppDto;
using UnityEngine;
using UniRx;

public partial interface IGuildDonateViewController
{
    void OnAddBtnClick();
    void OnMinusBtnClick();
    int DonateCount { get; }
    BagItemDto SelBagItem { get; }

}
public partial class GuildDonateViewController
{

    private Dictionary<GameObject, ItemCellController> _itemCellDic = new Dictionary<GameObject, ItemCellController>();
    private IEnumerable<BagItemDto> _itemList = null;

    private BagItemDto selBagItem = null;
    private IEnumerable<GuildDonate> GuildDonateList = null;

    private int donateCount = 0;

    private UniRx.Subject<string> donateInputChange = new UniRx.Subject<string>();
    public UniRx.IObservable<string> DonateInputChange { get { return donateInputChange; } }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        if(_itemCellDic.Count!=0) return;
        for (int i = 0; i < 25; i++)
        {
            var ctrl = AddChild<ItemCellController, ItemCell>(View.UIGrid_UIRecycledList.gameObject, ItemCell.NAME);
            _disposable.Add(ctrl.OnCellClick.Subscribe(e=>OnBagItemClick(ctrl.GetData())));
            _itemCellDic.Add(ctrl.gameObject, ctrl);
        }
        GuildDonateList = GuildMainDataMgr.DataMgr.GuildDonateList;
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
	{
	    View.UIGrid_UIRecycledList.onUpdateItem += UpdateItem;
        EventDelegate.Set(View.donateCount_UIIpunt.onChange, UpdateInputView);
    }

    protected override void RemoveCustomEvent ()
    {
        View.UIGrid_UIRecycledList.onUpdateItem -= UpdateItem;
        EventDelegate.Remove(View.donateCount_UIIpunt.onChange, UpdateInputView);
    }
        
    protected override void OnDispose()
    {
        _itemCellDic.ForEach(e =>
        {
            Object.Destroy(e.Key);
        });
        _itemCellDic.Clear();
        _itemList = null;
        selBagItem = null;
        donateCount = 0;
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IGuildMainData data)
    {
        UpdateView(data);
    }

    private void UpdateView(IGuildMainData data)
    {
        _itemList = data.DonateItemsList;
        View.UIGrid_UIRecycledList.UpdateDataCount(data.ItemCapacity, true);
        var item = _itemList.TryGetValue(0);
        OnBagItemClick(item, true);
    }

    private void UpdateItem(GameObject go, int itemIndex, int dataIndex)
    {
        if (_itemCellDic.Count == 0 || _itemList == null) return;
        ItemCellController item = null;
        if(!_itemCellDic.TryGetValue(go,out item)) return;
        var info = _itemList.TryGetValue(dataIndex);
        item.UpdateGuildDonateView(info);
    }

    public void OnBagItemClick(BagItemDto dto,bool isInit = false)
    {
        if(selBagItem == dto) return;
        selBagItem = dto;
        if (dto == null)
        {
            if (!isInit) return;
            //todo 第一个物品如果是空则处理。。
            GameDebuger.LogError("展示指引界面");
        }
        else
        {
            if (GuildDonateList == null) return;
            var id = dto.itemId;
            var item = GuildDonateList.Find(e => e.id == id);
            if(item == null) return;
            View.donateCount_UIIpunt.value = 1.ToString();
            donateCount = 1;
            UpdateAssetsView(dto);

            UIHelper.SetItemIcon(View.itemIcon_UISprite,dto.item.icon);
            View.itemLabel_UILabel.text = dto.item.name;
        }
    }

    //更新物资View
    private void UpdateAssetsView(BagItemDto dto)
    {
        var id = dto.itemId;
        var item = GuildDonateList.Find(e => e.id == id);
        var assets = item.assest*donateCount;
        var contribute = item.contribute*donateCount;
        View.getItemCount_UILabel.text = assets.ToString();
        View.getContributionCount_UILabel.text = contribute.ToString();
    }

    private void UpdateInputView()
    {
        if (selBagItem == null || GuildDonateList == null)
        {
            donateCount = 0;
            View.donateCount_UIIpunt.value = "";
            return;
        }
        int changeCount = StringHelper.ToInt(View.donateCount_UIIpunt.value);
        ChangeCount(changeCount);
        UpdateAssetsView(selBagItem);
    }

    //校验最大count
    private void ChangeCount(int changeCount)
    {
        int bagMax = selBagItem.count;
        var donateItem = GuildDonateList.Find(e => e.id == selBagItem.itemId);
        int donateMax = donateItem.maxCommit;
        var count = Mathf.Min(bagMax, donateMax);
        if (changeCount > count) donateCount = count;
        else donateCount = changeCount;
        View.donateCount_UIIpunt.value = donateCount.ToString();
    }

    public void OnAddBtnClick()
    {
        if (selBagItem == null || GuildDonateList == null)
        {
            donateCount = 0;
            return;
        }
        int count = StringHelper.ToInt(View.donateCount_UIIpunt.value);
        count++;
        ChangeCount(count);
        UpdateAssetsView(selBagItem);
    }

    public void OnMinusBtnClick()
    {
        if (selBagItem == null || GuildDonateList == null)
        {
            donateCount = 0;
            return;
        }
        int count = StringHelper.ToInt(View.donateCount_UIIpunt.value);
        count--;
        if (count < 1)
            count = 1;
        donateCount = count;
        View.donateCount_UIIpunt.value = count.ToString();
        UpdateAssetsView(selBagItem);
    }

    //捐献数量
    public int DonateCount { get { return donateCount; } }

    public BagItemDto SelBagItem { get { return selBagItem; } }

}
