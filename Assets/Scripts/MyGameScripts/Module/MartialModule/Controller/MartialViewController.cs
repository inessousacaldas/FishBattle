// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MartialViewController.cs
// Author   : xush
// Created  : 1/17/2018 5:10:21 PM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UnityEngine;

public partial interface IMartialViewController
{
    TabbtnManager GetTabManager { get; }
    void SetRankList(RankInfoDto dto);
}

public partial class MartialViewController
{
    private const int PlayerMax = 20;   //界面上只显示前20名
    private List<RankingItemCellController> _playerList = new List<RankingItemCellController>();

    public TabbtnManager GetTabManager { get { return _tabMgr; } }
    private TabbtnManager _tabMgr = null;

    private readonly ITabInfo[] TradeTabInfos =
    {
        TabInfoData.Create((int)MartialDataMgr.MartialData.MartialTab.Win, "胜者组"),
        TabInfoData.Create((int)MartialDataMgr.MartialData.MartialTab.loser, "败者组")
    };

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        for (int i = 0; i < PlayerMax; i++)
        {
            var item = AddChild<RankingItemCellController, RankingItemCell>(_view.PlayerGrid_UIGrid.gameObject,
                "MartialRankItem");
            _playerList.Add(item);
        }
        _view.PlayerGrid_UIGrid.Reposition();
        _view.PlayerScrollView_UIScrollView.ResetPosition();
        CreateTabInfo();
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
	{
	    EventDelegate.Add(_view.TipsBtn_UIButton.onClick, () =>
	    {
            ProxyTips.OpenTextTips(27, new Vector3(269, 199, 0));
        });
	}

    protected override void RemoveCustomEvent ()
    {
        
    }
        
    protected override void OnDispose()
    {
        JSTimer.Instance.CancelCd("KungfuTime");
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IMartialData data)
    {
        if(data.CurTab == MartialDataMgr.MartialData.MartialTab.Win)
            SetRankList(data.WinRankInfo);
        else
            SetRankList(data.LoserRankInfo);
        UpdateRewardBox(data.KungFuInfo.joinBattleGiftState, data.KungFuInfo.battleWinGiftState);
        UpdateMatchState(data.KungFuInfo.matchState);
        _view.TimeLb_UILabel.text = string.Format("剩余时间: {0}", DateUtil.FormatSeconds(data.EndAtTime)); //先显示一次,避免时间会等1s才显示的问题
        UpdateTime(data.EndAtTime);
    }

    private void CreateTabInfo()
    {
        _tabMgr = TabbtnManager.Create(TradeTabInfos, GetFunc());
        _tabMgr.SetBtnLblFont(20, "2d2d2d", 18, ColorConstantV3.Color_VerticalUnSelectColor2_Str);
    }

    public Func<int, ITabBtnController> GetFunc()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
            _view.TabGrid_UIGrid.gameObject
            , TabbtnPrefabPath.TabBtnWidget_S3.ToString()
            , "Tabbtn_" + i);

        _view.TabGrid_UIGrid.Reposition();
        return func;
    }

    public void SetRankList(RankInfoDto dto)
    {
        _playerList.ForEachI((item, idx) =>
        {
            var data = dto.list.TryGetValue(idx);
            if (data == null)
                item.gameObject.SetActive(false);
            else
            {
                item.gameObject.SetActive(true);
                item.SetItemInfo(data, idx + 1);
            }
        });

        UpdateMyRank(dto.myData, dto.myRank);
    }

    private void UpdateMyRank(RankItemDto myInfo, int rank)
    {
        _view.RankLb_UILabel.text = rank == 0 ? "榜外" : string.Format("我的排名: {0}", rank);
        var name = ModelManager.Player.GetPlayerName();
        _view.PlayerNameLb_UILabel.text = name;
        if (myInfo == null)
        {
            _view.WinLb_UILabel.text = "0/0";
            _view.ScroeLb_UILabel.text = "0";
            return;
        }
        
        var dto = myInfo as RankKungfuDto;
        if (dto != null)
        {
            _view.WinLb_UILabel.text = dto.winRate;
            _view.ScroeLb_UILabel.text = dto.score.ToString();
        }
    }

    private void UpdateRewardBox(int battleGift, int winGift)
    {
        _view.FirstWarBox_UISprite.isGrey = battleGift == (int)KungfuInfoDto.GiftState.UnAble;
        _view.FirstWinBox_UISprite.isGrey = winGift == (int) KungfuInfoDto.GiftState.UnAble;
        if (battleGift == (int) KungfuInfoDto.GiftState.UnAble)
            _view.FirstWarBox_UISprite.spriteName = "Box_Normal";
        else if (battleGift == (int)KungfuInfoDto.GiftState.Able)
            _view.FirstWarBox_UISprite.spriteName = "Box_Normal";
        else if (battleGift == (int) KungfuInfoDto.GiftState.Received)
            _view.FirstWarBox_UISprite.spriteName = "Box_Opend";
        else{}

        if (winGift == (int) KungfuInfoDto.GiftState.UnAble)
            _view.FirstWinBox_UISprite.spriteName = "Box_Normal";
        else if (winGift == (int)KungfuInfoDto.GiftState.Able)
            _view.FirstWinBox_UISprite.spriteName = "Box_Normal";
        else if (winGift == (int)KungfuInfoDto.GiftState.Received)
            _view.FirstWinBox_UISprite.spriteName = "Box_Opend";
        else { }
    }

    private void UpdateMatchState(int state)
    {
        switch (state)
        {
            case (int)KungfuInfoDto.MatchState.Unknow:
                _view.StateLb_UILabel.text = "";
                break;
            case (int)KungfuInfoDto.MatchState.MatchState_1:
                _view.StateLb_UILabel.text = "匹配尚未开始";
                break;
            case (int)KungfuInfoDto.MatchState.MatchState_2:
                _view.StateLb_UILabel.text = "正在匹配中…";
                break;
            case (int)KungfuInfoDto.MatchState.MatchState_3:
                _view.StateLb_UILabel.text = "暂离队员不能匹配";
                break;
            case (int)KungfuInfoDto.MatchState.MatchState_4:
                _view.StateLb_UILabel.text = "没有可匹配的队伍";
                break;
        }
    }

    public void UpdateTime(long endAt)
    {
        JSTimer.Instance.SetupCoolDown("KungfuTime", endAt, e =>
        {
            endAt -= 1;
            _view.TimeLb_UILabel.text = string.Format("剩余时间: {0}", DateUtil.FormatSeconds(endAt));
        }, () =>
        {
            _view.TimeLb_UILabel.text = "剩余时间: 00:00:00";
            JSTimer.Instance.CancelCd("KungfuTime");
        }, 1f);
        
    }
}
