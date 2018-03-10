// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RedPackViewController.cs
// Author   : DM-PC092
// Created  : 3/2/2018 4:07:45 PM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppDto;
using AppServices;
using UniRx;
using UnityEngine;
public partial interface IRedPackViewController
{
    //UniRx.IObservable<Unit> OnRedPackItem_UIButtonClick { get; }
    IObservableExpand<int> TabStream { get; }
}
public partial class RedPackViewController
{
    private Dictionary<GameObject, RedPackItemController> _redPackItemDic = new Dictionary<GameObject, RedPackItemController>();
    private List<RedPackItemController> _redPackItemList = new List<RedPackItemController>();
    private List<RedPackDetailDto> _redPacktemData = new List<RedPackDetailDto>();

    private TabbtnManager tabMgr;
    public IObservableExpand<int> TabStream
    {
        get { return tabMgr.Stream; }
    }
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        InitTabBtn();
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {
        View.RedPackGrid_UIGrid.onUpdateItem = UpdateRedPackRecycledList;
    }

    protected override void RemoveCustomEvent()
    {
    }

    protected override void OnDispose()
    {

    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {

    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IRedPackData data)
    {
        var mainviewData = data.GetRedPacketViewData;
        IEnumerable<RedPackDetailDto> redpacks = null;        
        if (mainviewData.CurTabMainView == RedPackChannelType.World)
            redpacks = mainviewData.GetWorldRedPacks;
        else if (mainviewData.CurTabMainView == RedPackChannelType.Guild)
            redpacks = mainviewData.GetGuildRedPacks;
        
        // Update Items
        UpdateRedPackItemList(redpacks);
        //UpdateGrid();
    }
    
    public static readonly ITabInfo[] ChannelTabInfos =
   {
        TabInfoData.Create((int)RedPackChannelType.World, "世界"),
        TabInfoData.Create((int)RedPackChannelType.Guild, "公会")
    };
    private void InitTabBtn()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
            _view.TabBtn_UIGrid.gameObject
            , TabbtnPrefabPath.TabBtnWidget_S3.ToString()
            , "Tabbtn_" + i);
        tabMgr = TabbtnManager.Create(ChannelTabInfos, func);
        tabMgr.SetBtnLblFont(20, "2d2d2d", 18, ColorConstantV3.Color_VerticalUnSelectColor2_Str);
    }

    private void UpdateRedPackItemList(IEnumerable<RedPackDetailDto> list)
    {        
        _redPackItemList.ForEachI((item, idx) =>
        {
            var dto = list.TryGetValue(idx);
            if (dto == null)
                item.gameObject.SetActive(false);
            else
            {
                item.gameObject.SetActive(true);
                item.SetItemInfo(dto);
            }
        });
    }
    private void UpdateRedPackRecycledList(GameObject go, int itemIndex, int dataIndex)
    {
        if (_redPackItemDic == null) return;
        RedPackItemController item = null;
        if (_redPackItemDic.TryGetValue(go, out item))
        {
            var info = _redPacktemData.TryGetValue(dataIndex);
            if (info == null) return;
            item.SetItemInfo(info);
            
        }
    }   
    private void MainViewState(bool b)
    {

    }
    //private void UpdateDetailItemList(IEnumerable<PlayerRedPackDto> list)
    //{
    //    _detailItemList.ForEachI((item, idx) =>
    //    {
    //        var dto = list.TryGetValue(idx);
    //        if (dto == null)
    //            item.gameObject.SetActive(false);
    //        else
    //        {
    //            item.gameObject.SetActive(true);
    //            item.SetItemInfo(dto);
    //        }
    //    });
    //}
}
