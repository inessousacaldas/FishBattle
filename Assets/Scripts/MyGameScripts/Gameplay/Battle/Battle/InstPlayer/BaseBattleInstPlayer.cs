// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  BaseBattleInstPlayer.cs
// Author   : SK
// Created  : 2013/3/8
// Purpose  : 
// **********************************************************************

using System.Collections.Generic;
using AppDto;
using MonsterManager = BattleDataManager.MonsterManager;

public class BaseBattleInstPlayer
{
    private VideoAction _action;

    public virtual void Excute(VideoAction action)
    {
        _action = action;
        DoExcute(action);
    }

    public virtual void DoExcute(VideoAction action)
    {
    }

    public virtual void Destroy()
    {
        JSTimer.Instance.CancelCd("BaseBattleInstPlayer_" + GetHashCode());
    }

    public virtual void CheckFinish()
    {
    }

    public virtual void PlayInjureAction(List<long> ids, float randomTime)
    {
    }

    protected void CheckMonsterDead()
    {
        var monsterList = new List<long>();

        _action.targetStateGroups.ForEach<VideoTargetStateGroup>(stateGroup =>
        {
            stateGroup.targetStates.ForEach<VideoTargetState>(state=>monsterList.AddIfNotExist(state.id));
        });

        var hasDeadMonster = false;
        monsterList.ForEach<long>(m =>
        {
            var monster = MonsterManager.Instance.GetMonsterFromSoldierID(m);
            if (monster == null) return;
            if (monster.IsDead())
            {
                hasDeadMonster = true;
                if (monster.leave)
                {
                    monster.RetreatFromBattle(MonsterController.RetreatMode.Fly, 0.5f);
                }
                else
                {
                    monster.PlayDieAnimation(false);
                    if (!monster.IsMainCharactor()) return;
                    var petController = MonsterManager.Instance.GetPlayerPet(monster.GetPlayerId());
                    if (null != petController)
                    {
                        //触发宠物主人倒地的喊话
                        GameDebuger.TODO(@"BattleController.Instance.GetInstController().TriggerMonsterShount(petController.GetId(), ShoutConfig.BattleShoutTypeEnum_MasterFall);");
                    }
                }
            }
            else
            {
                monster.PlayStateAnimation();
            }
        });
    }

    public virtual void Finish()
    {
        CheckMonsterDead();
        BattleDataManager.BattleInstController.Instance.FinishInst();
    }

    protected void DelayFinish(float delayTime)
    {
        GameDebuger.Log("DelayFinish " + delayTime);
        JSTimer.Instance.SetupCoolDown("BaseBattleInstPlayer_" + GetHashCode(), delayTime, null, Finish);
    }
}