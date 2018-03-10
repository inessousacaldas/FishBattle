// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GarandArenaReportItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppDto;

public partial class GarandArenaReportItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        FightBackBtn_UIButtonEvt.Subscribe(_ =>
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
    public void UpdateView(ArenaReportDto dto)
    {
        var labelStr = string.Empty;
        var myId = ModelManager.Player.GetPlayerId();
        if (myId == dto.attackId)
            labelStr += "你对" + dto.defenseName;
        else
            labelStr += dto.attackName + "对你";

        if (dto.type == (int)ArenaReportDto.ChallengeTypeEnum.challenge)
            labelStr += "发起了挑战,";
        else
            labelStr += "发起了反击,";

        if (dto.win)
            labelStr += "你获胜了";
        else
            labelStr += "你战败了";

        if (dto.type == (int)ArenaReportDto.ChallengeTypeEnum.challenge)
        {
            if (myId == dto.attackId && dto.win)
                labelStr += ",积分升至" + dto.attackerTrophy;
            else if (myId == dto.defenseId && !dto.win)
                labelStr += ",积分降至" + dto.defenseTrophy;
        }

        View.Label_UILabel.text = labelStr;
        View.FightBackBtn_UIButton.gameObject.SetActive(!dto.revenge);
    }
}
