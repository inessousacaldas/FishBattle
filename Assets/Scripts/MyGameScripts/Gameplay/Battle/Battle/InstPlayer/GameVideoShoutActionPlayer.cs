// **********************************************************************
// Copyright (c) 2015 cilugame. All rights reserved.
// File     : GameVideoShoutActionPlayer.cs
// Author   : senkay <senkay@126.com>
// Created  : 10/20/2015 
// Porpuse  : 
// **********************************************************************
//

using AppDto;
using MonsterManager = BattleDataManager.MonsterManager;

public class GameVideoShoutActionPlayer : BaseBattleInstPlayer
{
    public override void DoExcute(VideoAction inst)
    {
        var action = inst as VideoShoutAction;
        var mc = MonsterManager.Instance.GetMonsterFromSoldierID(action.actionSoldierId);
        if (mc != null)
        {
            mc.Shout(action.shoutContent);
        }
        DelayFinish(0.5f);
    }
}