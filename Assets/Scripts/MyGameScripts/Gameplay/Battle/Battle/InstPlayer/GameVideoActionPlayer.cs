// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  GameVideoActionPlayer.cs
// Author   : SK
// Created  : 2014/3/14
// Purpose  : 
// **********************************************************************

using AppDto;

public class GameVideoActionPlayer : BaseBattleInstPlayer
{
    public override void DoExcute(VideoAction action)
    {
        BattleInfoOutput.ShowVideoAction(action);
        BattleStateHandler.HandleBattleStateGroup(0, action.targetStateGroups, true);
        DelayFinish(0.2f);
    }
}