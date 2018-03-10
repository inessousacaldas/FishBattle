// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GarandArenaRankItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using System.Collections.Generic;

public partial class GarandArenaRankItemController
{
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

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private Dictionary<int, Faction> _fationDic = DataCache.getDicByCls<Faction>();
    public void UpdateView(int index, RankArenaDto dto)
    {
        //rank名次为index+1
        var rank = index + 1;
        if (index % 2 == 0)
            View.Bg_UISprite.enabled = true;
        else
            View.Bg_UISprite.enabled = false;

        if (rank >= 1 && rank <= 3)
        {
            View.Icon_UISprite.enabled = true;
            View.Icon_UISprite.spriteName = string.Format("Ranking_{0}", rank);
            View.Rank_UILabel.enabled = false;
        }
        else
        {
            View.Icon_UISprite.enabled = false;
            View.Rank_UILabel.enabled = true;
            View.Rank_UILabel.text = rank.ToString();
        }

        View.Name_UILabel.text = dto.name;
        View.Profession_UILabel.text = _fationDic.ContainsKey(dto.factionId) ? _fationDic[dto.factionId].name : "魔枪手";
        View.Cup_UILabel.text = dto.trophyCount.ToString();
    }
}
