// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GarandArenaRivalItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppDto;
using MyGameScripts.Gameplay.Player;
using System.Collections.Generic;

public partial class GarandArenaRivalItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        ChallengeBtn_UIButtonEvt.Subscribe(_ =>
        {
            GarandArenaMainViewDataMgr.GarandArenaMainViewNetMsg.ReqChallenge(_id);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private long _id = 0;
    public void UpdateView(OpponentInfoDto dto)
    {
        _id = dto.playerInfo.id;
        //UIHelper.SetCommonIcon(View.Magic_UISprite, GlobalAttr.MAGICICON[dto.playerInfo.guildId]);
        UIHelper.SetCommonIcon(View.Faction_UISprite, string.Format("faction_{0}", ModelManager.Player.FactionID));
        View.CupNum_UILabel.text = dto.trophyCount.ToString();
        View.Lv_UILabel.text = dto.playerInfo.grade.ToString();
        if (dto.playerInfo.charactor != null)
            UIHelper.SetPetIcon(View.PlayerIcon_UISprite, dto.playerInfo.charactor.gender == 1 ? "101" : "103");
    }
}
