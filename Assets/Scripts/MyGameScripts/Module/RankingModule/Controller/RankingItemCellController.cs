// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RankingItemCellController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public partial interface IRankingItemCellController
{
    UniRx.IObservable<IRankItemData> ClickHandler { get; }
}

public partial class RankingItemCellController
{
    private Subject<IRankItemData> _clickEvt = new Subject<IRankItemData>();
    public UniRx.IObservable<IRankItemData> ClickHandler { get { return _clickEvt; } }  

    private List<UILabel> labels = new List<UILabel>();

    private readonly float[] COLUMN_POS_5 = { -265, -148f, 16f, 120, 237 };
    private readonly float[] COLUMN_POS_4 = { -265, -116, 61, 230, 0 };

    private IRankItemData _itemdata;
    private int _index;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        labels.Add(_view.FirstLabel_UILabel);
        labels.Add(_view.SecondLabel_UILabel);
        labels.Add(_view.ThirdLabel_UILabel);
        labels.Add(_view.FurthLabel_UILabel);
        labels.Add(_view.FifthLabel_UILabel);
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Add(_view.RankingItemCell_UIButton.onClick, () => { _clickEvt.OnNext(_itemdata); });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void SetLablePosition(int column)
    {
        float[] pos = COLUMN_POS_4;
        if (column == 5)
        {
            pos = COLUMN_POS_5;
        }
        else if (column == 4)
        {
            pos = COLUMN_POS_4;
        }

        Vector3 vec;
        for (int i = 0; i < labels.Count; i++)
        {
            if(i>=column)
            {
                labels[i].gameObject.SetActive(false);
                continue;
            }
            vec = labels[i].transform.localPosition;
            vec.x = pos[i];
            labels[i].transform.localPosition = vec;
            labels[i].gameObject.SetActive(true);
        }
    }

    public void SetItemInfo(RankItemDto dto, int index, int rankId = -1)
    {
        if (dto == null)
        {
            ResetUI(-1);
            return;
        }

        Rankings rankings = DataCache.getDtoByCls<Rankings>(rankId);
        _itemdata = RankItemData.Create(dto.id, rankings);
        var type = dto.GetType();
        switch (type.ToString())
        {
            case "AppDto.RankPlayerGradeDto":
                SetGradeItem(dto as RankPlayerGradeDto, index, rankId);
                break;
            case "AppDto.RankCrewDto":
                SetRankCrewItem(dto as RankCrewDto, index, rankId);
                break;
            case "AppDto.RankFlowersDto":
                SetFlowerItem(dto as RankFlowersDto, index, rankId);
                break;
            case "AppDto.RankTowerDto":
                SetRankTowerItem(dto as RankTowerDto, index, rankId);
                break;
            case "AppDto.RankArenaDto":
                SetArneaItem(dto as RankArenaDto, index, rankId);
                break;
            case "AppDto.RankKungfuDto":
                SetMartialItem(dto as RankKungfuDto, index);
                break;
        }
    }

    //等级排行榜
    private void SetGradeItem(RankPlayerGradeDto gradeDto, int index, int rankId)
    {
        ResetUI(index);
        _view.SecondLabel_UILabel.text = gradeDto.name;
        var faction = DataCache.getDtoByCls<Faction>(gradeDto.factionId);
        _view.ThirdLabel_UILabel.text = faction == null ? string.Empty : faction.name;
        _view.FurthLabel_UILabel.text = string.Format("{0}级", gradeDto.grade);
        var rank = DataCache.getDtoByCls<Rankings>(rankId);
        if (rank == null)
        {
            GameDebuger.LogError(string.Format("请检查rankings表,找不到{0}", rankId));
            return;
        }
        SetLablePosition(rank.rankColShow);
        _view.rankMark_UISprite.gameObject.SetActive(index <= 3 && rank.rankStyle == (int)Rankings.RankStyle.Normal);
        if (rank.rankStyle == (int)Rankings.RankStyle.Normal)
            _view.FirstLabel_UILabel.text = index <= 3 ? "" : index.ToString();
        else
            _view.FirstLabel_UILabel.text = (index + 3).ToString();
    }

    //伙伴战力排行榜
    private void SetRankCrewItem(RankCrewDto dto, int index, int rankId)
    {
        ResetUI(index);
        _view.SecondLabel_UILabel.text = dto.name;
        _view.ThirdLabel_UILabel.text = dto.playerName;
        _view.FurthLabel_UILabel.text = dto.crewPower.ToString();
        var rank = DataCache.getDtoByCls<Rankings>(rankId);
        if (rank == null)
        {
            GameDebuger.LogError(string.Format("请检查rankings表,找不到{0}", rankId));
            return;
        }
        SetLablePosition(rank.rankColShow);
        _view.rankMark_UISprite.gameObject.SetActive(index <= 3 && rank.rankStyle == (int)Rankings.RankStyle.Normal);
        if (rank.rankStyle == (int)Rankings.RankStyle.Normal)
            _view.FirstLabel_UILabel.text = index <= 3 ? "" : index.ToString();
        else
            _view.FirstLabel_UILabel.text = (index + 3).ToString();
    }

    //鲜花榜
    private void SetFlowerItem(RankFlowersDto dto, int index, int rankId)
    {
        ResetUI(index);
        _view.SecondLabel_UILabel.text = dto.name;
        var faction = DataCache.getDtoByCls<Faction>(dto.factionId);
        _view.ThirdLabel_UILabel.text = faction == null ? string.Empty : faction.name;
        _view.FurthLabel_UILabel.text = dto.flowers.ToString();
        var rank = DataCache.getDtoByCls<Rankings>(rankId);
        if (rank == null)
        {
            GameDebuger.LogError(string.Format("请检查rankings表,找不到{0}", rankId));
            return;
        }
        SetLablePosition(rank.rankColShow);
        _view.rankMark_UISprite.gameObject.SetActive(index <= 3 && rank.rankStyle == (int)Rankings.RankStyle.Normal);
        if (rank.rankStyle == (int)Rankings.RankStyle.Normal)
            _view.FirstLabel_UILabel.text = index <= 3 ? "" : index.ToString();
        else
            _view.FirstLabel_UILabel.text = (index + 3).ToString();
    }

    //四轮之塔排行榜
    private void SetRankTowerItem(RankTowerDto dto, int index, int rankId)
    {
        ResetUI(index);
        _view.SecondLabel_UILabel.text = dto.name;
        var faction = DataCache.getDtoByCls<Faction>(dto.factionId);
        _view.ThirdLabel_UILabel.text = faction == null ? string.Empty : faction.name;
        _view.FurthLabel_UILabel.text = dto.towerId.ToString();
        _view.FifthLabel_UILabel.text = DateUtil.GetDayHourMinuteSecond(dto.useTime);
        var rank = DataCache.getDtoByCls<Rankings>(rankId);
        if (rank == null)
        {
            GameDebuger.LogError(string.Format("请检查rankings表,找不到{0}", rankId));
            return;
        }
        SetLablePosition(rank.rankColShow);
        _view.rankMark_UISprite.gameObject.SetActive(index <= 3 && rank.rankStyle == (int)Rankings.RankStyle.Normal);
        if (rank.rankStyle == (int)Rankings.RankStyle.Normal)
            _view.FirstLabel_UILabel.text = index <= 3 ? "" : index.ToString();
        else
            _view.FirstLabel_UILabel.text = (index + 3).ToString();
    }

    //竞技场排行榜
    private void SetArneaItem(RankArenaDto dto, int index, int rankId)
    {
        ResetUI(index);
        _view.SecondLabel_UILabel.text = dto.name;
        var faction = DataCache.getDtoByCls<Faction>(dto.factionId);
        _view.ThirdLabel_UILabel.text = faction == null ? string.Empty : faction.name;
        _view.FurthLabel_UILabel.text = dto.trophyCount.ToString();
        var rank = DataCache.getDtoByCls<Rankings>(rankId);
        if (rank == null)
        {
            GameDebuger.LogError(string.Format("请检查rankings表,找不到{0}", rankId));
            return;
        }
        SetLablePosition(rank.rankColShow);
        _view.rankMark_UISprite.gameObject.SetActive(index <= 3 && rank.rankStyle == (int)Rankings.RankStyle.Normal);
        if (rank.rankStyle == (int)Rankings.RankStyle.Normal)
            _view.FirstLabel_UILabel.text = index <= 3 ? "" : index.ToString();
        else
            _view.FirstLabel_UILabel.text = (index + 3).ToString();
    }

    //比武大会
    private void SetMartialItem(RankKungfuDto dto, int index)
    {
        ResetUI(index);

        _view.rankMark_UISprite.gameObject.SetActive(index <= 3);
        _view.FirstLabel_UILabel.gameObject.SetActive(index > 3);
        _view.FirstLabel_UILabel.text = index%2 == 1
            ? index.ToString().WrapColor(ColorConstantV3.Color_Black_Str)
            : index.ToString().WrapColor(ColorConstantV3.Color_White);
        _view.SecondLabel_UILabel.text = index % 2 == 1
            ? dto.name.WrapColor(ColorConstantV3.Color_Black_Str)
            : dto.name.WrapColor(ColorConstantV3.Color_White);
        _view.ThirdLabel_UILabel.text = index % 2 == 1
            ? dto.winRate.WrapColor(ColorConstantV3.Color_Black_Str)
            : dto.winRate.WrapColor(ColorConstantV3.Color_White);
        _view.FurthLabel_UILabel.text = index%2 == 1
            ? dto.score.ToString().WrapColor(ColorConstantV3.Color_Black_Str)
            : dto.score.ToString().WrapColor(ColorConstantV3.Color_White);
        _view.bg_UISprite.gameObject.SetActive(index % 2 == 1);
    } 

    private void ResetUI(int index)
    {
        labels.ForEach(label => label.text = string.Empty);
        if(_view.factionMark_UISprite != null)
            _view.factionMark_UISprite.gameObject.SetActive(false);

        if (index < 0)
            return;
        
        if(index <= 3)
            _view.rankMark_UISprite.spriteName = "Ranking_" + index;
    }

}
