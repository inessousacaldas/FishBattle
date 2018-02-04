using AppDto;
using System.Collections.Generic;
using System.Linq;
using MonsterManager = BattleDataManager.MonsterManager;

public static class VideoRoundSimulater
{
    public static long SIMULATE_BATTLE_ID = System.DateTime.Now.Ticks;
    #region interface

    public static VideoRound SimulateVideoRound(List<VideoSoldier> pTeamSoldiersPlayer, List<VideoSoldier> pTeamSoldiersEnemy, long pAttackId = 0, Skill pSkill = null, int round = 0)
    {
        GameLog.Log_Battle("SimulateVideoRound");
        long tCurAttackId = 0;
        long tDefenderId = 0;
        VideoSoldier tVideoSoldier = null;
        SimulateActionPlayerInfo(out tCurAttackId, out tDefenderId, out tVideoSoldier, pTeamSoldiersPlayer, pTeamSoldiersEnemy, pAttackId);

        var tVideoRound = new VideoRound();
        tVideoRound.battleId = SIMULATE_BATTLE_ID;
        tVideoRound.skillActions = SimulateVideoSkillActionList(pTeamSoldiersPlayer, pTeamSoldiersEnemy, tCurAttackId, pSkill);
        tVideoRound.id = tCurAttackId;
        tVideoRound.name = tVideoSoldier.name;
        tVideoRound.actionQueue = SimulateActionQueue(pTeamSoldiersPlayer.Concat(pTeamSoldiersEnemy));
        tVideoRound.round = round;
        return tVideoRound;
    }

    private static ActionQueueDto SimulateActionQueue( IEnumerable<VideoSoldier> soldiers)
    {
        var temp = soldiers.ToList();
        var cnt = temp.Count;
        var result = new List<long>(cnt);

        var _randomIdx = 0;
        VideoSoldier soldier = null;
        
        var atList = new List<int>(cnt);
        var descList = new List<int>(cnt);
        
        for (var i = 0; i < cnt; i++)
        {
            atList.Add(i);
            descList.Add(i);
            _randomIdx = UnityEngine.Random.Range(0, temp.Count);
            soldier = temp[_randomIdx];
            result.Add(soldier.id);
            temp.RemoveItem(soldier);   
        }
        
        var dto = new ActionQueueDto();
        dto.soldierQueue = result;
        dto.actionTimeQueue = atList;
        dto.rewardQueue = descList;
        return dto;
    }

    #endregion

    private static bool SimulateActionPlayerInfo(
        out long tCurAttackId
        , out long tDefenderId
        , out VideoSoldier tVideoSoldier
        , List<VideoSoldier> pTeamSoldiersPlayer
        , List<VideoSoldier> pTeamSoldiersEnemy
        , long pAttackId = 0)
    {
        UnityEngine.Assertions.Assert.IsNotNull(pTeamSoldiersEnemy, "敌人队伍数据不得为空！");
        tCurAttackId = ModelManager.Player.GetPlayerId();
        tDefenderId = pTeamSoldiersEnemy[0].id;
        if (pAttackId > 0)
        {
            tCurAttackId = pAttackId;
            if (MonsterManager.Instance.IsEnemy(pAttackId))
                tDefenderId = pTeamSoldiersPlayer[UnityEngine.Random.Range(0, pTeamSoldiersPlayer.Count)].id;
            else
                tDefenderId = pTeamSoldiersEnemy[UnityEngine.Random.Range(0, pTeamSoldiersEnemy.Count)].id;
        }
        tVideoSoldier = MonsterManager.Instance.GetMonsterFromSoldierID(tCurAttackId).videoSoldier;
        return true;
    }

    private static List<VideoSkillAction> SimulateVideoSkillActionList(List<VideoSoldier> pTeamSoldiersPlayer, List<VideoSoldier> pTeamSoldiersEnemy, long pAttackId = 0, Skill pSkill = null)
    {
        var tVideoSkillActionList = new List<VideoSkillAction>();
        for (var tCounter = 0; tCounter < 1; tCounter++)
        {
            tVideoSkillActionList.Add(SimulateVideoSkillAction(pTeamSoldiersPlayer, pTeamSoldiersEnemy, pAttackId, pSkill));
        }
        return tVideoSkillActionList;
    }

    private static VideoSkillAction SimulateVideoSkillAction(
        List<VideoSoldier> pTeamSoldiersPlayer
        , List<VideoSoldier> pTeamSoldiersEnemy
        , long pAttackId = 0
        , Skill pSkill = null)
    {
        var tSkillID = 0;
        long tCurAttackId = 0;
        long tDefenderId = 0;
        VideoSoldier tVideoSoldier = null;
        List<VideoTargetStateGroup> tTargetStateGroups = null;
        
        SimulateActionPlayerInfo(
            out tCurAttackId
            , out tDefenderId
            , out tVideoSoldier
            , pTeamSoldiersPlayer
            , pTeamSoldiersEnemy
            , pAttackId);   

        if (null == pSkill)
        {
            tSkillID = tVideoSoldier.defaultSkillId; 
            tTargetStateGroups = SimulateVideoTargetStateGroupList(tDefenderId);
        }
        else
        {
            tSkillID = pSkill.id;
            tTargetStateGroups = SimulateVideoTargetStateGroupList(GetMonsterUIDList(pTeamSoldiersEnemy), -666);
        }

        var tVideoSkillAction = new VideoSkillAction();
        tVideoSkillAction.actionSoldierId = tCurAttackId;//ModelManager.BattleDemo.GameVideo.myTeam.fighters[0].id;
        tVideoSkillAction.skillId = tSkillID;
        tVideoSkillAction.skill = pSkill;
        tVideoSkillAction.targetStateGroups = tTargetStateGroups;
        return tVideoSkillAction;
    }

    /// <summary>
    /// 整个攻击过程的受击者信息
    /// 比如一个受击者受击了多次，或者多个受击者受击。
    /// </summary>
    /// <returns>The video target state group list.</returns>
    /// <param name="pDefenderId">P defender identifier.</param>
    private static List<VideoTargetStateGroup> SimulateVideoTargetStateGroupList(long pDefenderId, int pDamage = 0)
    {
        return SimulateVideoTargetStateGroupList(new List<long>{ pDefenderId });
    }

    private static List<VideoTargetStateGroup> SimulateVideoTargetStateGroupList(List<long> pDefenderIdList, int pDamage = 0)
    {
        var tVideoTargetStateGroupList = new List<VideoTargetStateGroup>();
        if (null == pDefenderIdList || pDefenderIdList.Count <= 0)
            return null;
        for (var tCounter = 0; tCounter < pDefenderIdList.Count; tCounter++)
        {
            tVideoTargetStateGroupList.Add(SimulateVideoTargetStateGroup(pDefenderIdList[tCounter], pDamage));
        }
        //        if (UnityEngine.Random.Range(0, 100) < 20)
        //            tVideoTargetStateGroupList.Add(SimulateVideoTargetStateGroup(pDefenderId, 1, true));
        //        else
        //            tVideoTargetStateGroupList.Add(SimulateVideoTargetStateGroup(pDefenderId));
        return tVideoTargetStateGroupList;
    }

    private static VideoTargetStateGroup SimulateVideoTargetStateGroup(long pDefenderId, int pBuffId = 0, bool pAdd = false, int pDamage = 0)
    {
        var tVideoTargetStateGroup = new VideoTargetStateGroup();
        tVideoTargetStateGroup.targetStates = SimulateVideoActionTargetStateList(pDefenderId, pBuffId, pAdd, pDamage);
        return tVideoTargetStateGroup;
    }

    private static VideoActionTargetState SimulateVideoActionTargetState(long pDefenderId, int pDamage = 0)
    {
        var tVideoActionTargetState = new VideoActionTargetState();
        tVideoActionTargetState.crit = UnityEngine.Random.Range(0, 100) < 25;
        tVideoActionTargetState.currentHp = pDamage == 0 ? 1000 : (pDamage + 1000);
        tVideoActionTargetState.currentCp = 1000;
//        tVideoActionTargetState.hp = pDamage == 0 ? -100 : pDamage;
        tVideoActionTargetState.dead = false;
        tVideoActionTargetState.id = pDefenderId;
        return tVideoActionTargetState;
    }

    /// <summary>
    /// 每一次攻击的受击信息。比如一次受击者的掉血信息。
    /// </summary>
    /// <returns>The video action target state list.</returns>
    /// <param name="pDefenderId">P defender identifier.</param>
    private static List<VideoTargetState> SimulateVideoActionTargetStateList(long pDefenderId, int pBuffId = 0, bool pAdd = false, int pDamage = 0)
    {
        var tVideoTargetStateGroupList = new List<VideoTargetState>();
        for (int tCounter = 0; tCounter < 1; tCounter++)
        {
            if (pBuffId > 0)
            {
                if (pAdd)
                    tVideoTargetStateGroupList.Add(SimulateVideoBuffAddTargetState(pDefenderId, pBuffId) as VideoTargetState);
                else
                    tVideoTargetStateGroupList.Add(SimulateVideoBuffRemoveTargetState(pDefenderId, pBuffId) as VideoTargetState);        
            }
            else
                tVideoTargetStateGroupList.Add(SimulateVideoActionTargetState(pDefenderId, pDamage) as VideoTargetState);
        }
        return tVideoTargetStateGroupList;
    }

    private static VideoBuffAddTargetState SimulateVideoBuffAddTargetState(long pDefenderId, int pbattleBuffId)
    {
        VideoBuffAddTargetState tVideoActionTargetState = new VideoBuffAddTargetState();
        tVideoActionTargetState.battleBuffId = pbattleBuffId;
        tVideoActionTargetState.dead = false;
        tVideoActionTargetState.id = pDefenderId;
        tVideoActionTargetState.durationTime = 2000;
        return tVideoActionTargetState;
    }

    private static VideoBuffRemoveTargetState SimulateVideoBuffRemoveTargetState(long pDefenderId, int pBuffId)
    {
        VideoBuffRemoveTargetState tVideoActionTargetState = new VideoBuffRemoveTargetState();
        tVideoActionTargetState.buffId = new List<int>(){ pBuffId };
        tVideoActionTargetState.dead = false;
        tVideoActionTargetState.id = pDefenderId;
        return tVideoActionTargetState;
    }

    private static List<long> GetMonsterUIDList(List<VideoSoldier> pTeamSoldiersEnemy)
    {
        if (null == pTeamSoldiersEnemy || pTeamSoldiersEnemy.Count <= 0)
            return null;
        List<long> tUIDList = new List<long>();
        VideoSoldier tVideoSoldier = null;
        for (int tCounter = 0; tCounter < pTeamSoldiersEnemy.Count; tCounter++)
        {
            tVideoSoldier = pTeamSoldiersEnemy[tCounter];
            tUIDList.Add(tVideoSoldier.id);
        }
        return tUIDList;
    }
}
