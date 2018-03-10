// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RankingInfoCellController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using UniRx;
using UnityEngine;

public partial class RankingInfoCellController
{
    private CompositeDisposable _disposable = new CompositeDisposable();

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {

    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
    }

    public void SetItemInfo(RankInfoDto dto)
    {
        if (dto == null || dto.myData == null)
        {
            _view.MyRank_UILabel.text = "我的排名: 未上榜";
            _view.MyScore_UILabel.text = "";
            return;
        }

        _view.MyRank_UILabel.text = dto.myRank == 0 ? "我的排名: 榜外" : string.Format("我的排名: {0}", dto.myRank);
        var type = dto.myData.GetType();
        switch (type.ToString())
        {
            case "AppDto.RankPlayerGradeDto":
                SetGradeItem(dto.myData as RankPlayerGradeDto);
                break;
            case "AppDto.RankCrewDto":
                SetRankCrewItem(dto.myData as RankCrewDto);
                break;
            case "AppDto.RankFlowersDto":
                SetFlowerItem(dto.myData as RankFlowersDto);
                break;
            case "AppDto.RankTowerDto":
                SetRankTowerItem(dto.myData as RankTowerDto);
                break;
            case "AppDto.RankArenaDto":
                SetArneaItem(dto.myData as RankArenaDto);
                break;
        }
    }

    //等级排行榜
    private void SetGradeItem(RankPlayerGradeDto dto)
    {
        _view.MyScore_UILabel.text = string.Format("等级:{0}", dto.grade);
    }

    //伙伴战力排行榜
    private void SetRankCrewItem(RankCrewDto dto)
    {
        _view.MyScore_UILabel.text = string.Format("伙伴:{0}    战力:{1}", dto.name, dto.crewPower);
    }

    //鲜花榜
    private void SetFlowerItem(RankFlowersDto dto)
    {
        _view.MyScore_UILabel.text = string.Format("鲜花:{0}", dto.flowers);
    }

    //四轮之塔排行榜
    private void SetRankTowerItem(RankTowerDto dto)
    {
        _view.MyScore_UILabel.text = string.Format("通关层数:{0}", dto.towerId);
    }

    //竞技场排行榜
    private void SetArneaItem(RankArenaDto dto)
    {
        _view.MyScore_UILabel.text = string.Format("积分:{0}", dto.trophyCount);
    }
}
