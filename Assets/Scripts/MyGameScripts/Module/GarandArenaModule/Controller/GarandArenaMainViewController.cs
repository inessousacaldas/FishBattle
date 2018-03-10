// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GarandArenaMainViewController.cs
// Author   : xjd
// Created  : 12/14/2017 2:45:59 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppDto;
using System.Collections.Generic;

public partial interface IGarandArenaMainViewController
{

}
public partial class GarandArenaMainViewController    {

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

    private List<GarandArenaRankItemController> _ArenaRankItemList = new List<GarandArenaRankItemController>();
    private List<GarandArenaRivalItemController> _ArenaRivalItemList = new List<GarandArenaRivalItemController>();

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IGarandArenaMainViewData data)
    {
        View.CupNum_UILabel.text = data.CupCount.ToString();
        var wealthCount = ModelManager.Player.GetPlayerWealthById((int)AppVirtualItem.VirtualItemEnum.ARENA_CURRENCY);
        View.IntergralNum_UILabel.text = wealthCount >= 10000000 ? string.Format("{0}万", wealthCount / 10000) : wealthCount.ToString();
        View.LeftlNum_UILabel.text = data.RemainTimes.ToString();
        //我的排行
        if (data.MyRank >= 1 && data.MyRank <= 3)
        {
            View.MyRankIcon_UISprite.enabled = true;
            View.MyRankIcon_UISprite.spriteName = string.Format("Ranking_{0}", data.MyRank);
            View.MyRank_UILabel.enabled = false;
        }
        else
        {
            View.MyRankIcon_UISprite.enabled = false;
            View.MyRank_UILabel.enabled = true;
            View.MyRank_UILabel.text = data.MyRank == 0 ? "未上榜" : data.MyRank.ToString();
        }
        View.MyCup_UILabel.text = data.CupCount.ToString();
        View.MyFaction_UILabel.text = ModelManager.Player.FactionName;
        View.MyName_UILabel.text = ModelManager.Player.GetPlayerName();

        //排行
        var itemCount = 0;
        _ArenaRankItemList.GetElememtsByRange(itemCount, -1).ForEach(s => s.Hide());
        data.RankList.ForEachI((itemDto, index) =>
        {
            var itemCtrl = AddRankItemIfNotExist(index);
            itemCtrl.UpdateView(index, itemDto as RankArenaDto);
            itemCtrl.Show();
        });

        _ArenaRivalItemList.GetElememtsByRange(itemCount, -1).ForEach(s => s.Hide());
        data.RivalList.ForEachI((itemDto, index) =>
        {
            var itemCtrl = AddRivalItemIfNotExist(index);
            itemCtrl.UpdateView(itemDto);
            itemCtrl.Show();
        });

        View.RankGrid_UIGrid.Reposition();
        View.RivalGrid_UIGrid.Reposition();
    }

    private GarandArenaRankItemController AddRankItemIfNotExist(int idx)
    {
        GarandArenaRankItemController ctrl = null;
        _ArenaRankItemList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<GarandArenaRankItemController, GarandArenaRankItem>(View.RankGrid_UIGrid.gameObject, GarandArenaRankItem.NAME);
            _ArenaRankItemList.Add(ctrl);
        }

        return ctrl;
    }

    private GarandArenaRivalItemController AddRivalItemIfNotExist(int idx)
    {
        GarandArenaRivalItemController ctrl = null;
        _ArenaRivalItemList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<GarandArenaRivalItemController, GarandArenaRivalItem>(View.RivalGrid_UIGrid.gameObject, GarandArenaRivalItem.NAME);
            _ArenaRivalItemList.Add(ctrl);
        }

        return ctrl;
    }
}
