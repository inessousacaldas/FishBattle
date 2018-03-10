// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildBoxViewController.cs
// Author   : DM-PC092
// Created  : 3/6/2018 5:43:55 PM
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;
using UnityEngine;
using AppDto;
public partial interface IGuildBoxViewController
{

}
public partial class GuildBoxViewController    {

    private Dictionary<GameObject, GuildBoxEventsItemController> _boxItemDic = new Dictionary<GameObject, GuildBoxEventsItemController>();
    private List<GuildEventDto> _eventList = null;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        CreateBoxEventItem();
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent ()
    {
        View.boxEventsGrid_UIRecycledList.onUpdateItem += UpdateEventsItem;
    }

    protected override void RemoveCustomEvent()
    {
        View.boxEventsGrid_UIRecycledList.onUpdateItem -= UpdateEventsItem;
    }

    protected override void OnDispose()
    {
        _boxItemDic.ForEach(e =>
        {
            Object.Destroy(e.Key);
        });
        _boxItemDic.Clear();
        _eventList = null;
        base.OnDispose();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {

    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IGuildMainData data)
    {
        UpdateGuildBoxView(data);
    }


    //更新宝箱内容
    private void UpdateGuildBoxView(IGuildMainData data)
    {
        var guildBox = data.GuildBox;
        if (guildBox == null) return;

        _eventList = guildBox.events;
        View.boxEventsGrid_UIRecycledList.UpdateDataCount(_eventList.Count, true);

        float fgVal = guildBox.hasPoint;
        float bgVal = guildBox.needPoint;
        var result = fgVal / bgVal;
        View.boxSlider_UIProgressBar.value = float.IsNaN(result) ? 0 : result;
        View.pointsLabel_UILabel.text = fgVal + "/" + bgVal;
        View.boxLevelLabel_UILabel.text = data.GuildDetailInfo.buildingInfo.treasuryGrade + "级宝箱";
    }

    private void CreateBoxEventItem()
    {
        if (_boxItemDic.Count != 0) return;
        for (int i = 0; i < 14; i++)
        {
            var ctrl = AddChild<GuildBoxEventsItemController, GuildBoxEventsItem>(View.boxEventsGrid_UIRecycledList.gameObject,
                GuildBoxEventsItem.NAME);
            _boxItemDic.Add(ctrl.gameObject, ctrl);
        }
    }

    private void UpdateEventsItem(GameObject go, int itemIndex, int dataIndex)
    {
        if (_boxItemDic.Count == 0 || _eventList == null) return;
        GuildBoxEventsItemController item = null;
        if (!_boxItemDic.TryGetValue(go, out item)) return;
        var info = _eventList.TryGetValue(dataIndex);
        if (info == null) return;
        item.UpdateView(info);
    }

}
