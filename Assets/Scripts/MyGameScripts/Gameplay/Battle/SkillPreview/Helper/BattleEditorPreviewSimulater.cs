using System;
using AppDto;
using System.Collections.Generic;

namespace SkillEditor
{
    /// <summary>
    /// 模拟战斗编辑器下演示用数据
    /// @MarsZ 2017年05月03日19:54:53
    /// </summary>
    public static class BattleEditorPreviewSimulater
    {
        //主角类型ID，不用枚举的原因在于，部分项目其类型并不是枚举
        private const int MTYPE_MAIN_CHARACTORID = 1;
        private const int MTYPE_MONSTER_ID = 5;

        public static long SIMULATE_BATTLE_ID = System.DateTime.Now.Ticks;

        public static PlayerDto PlayerDto
        {
            get
            { 
                //考虑到切换角色等时的情况，且本类没有销毁清除的打算，所以一直直接获取。
                return ModelManager.Player.GetPlayer();
            }
        }

        #region Video simulate

        public static Video SimulateVideo(int pSkillId, int pEnemyCount, int pEnemyFormation)
        {
            Video tDemoVideo = new Video();
            tDemoVideo.id = SIMULATE_BATTLE_ID;
            //A队玩家，B队敌方
            UnityEngine.Assertions.Assert.IsNotNull(PlayerDto, "玩家数据不得为空！");
            tDemoVideo.ateam = SimulateVideoTeam(MTYPE_MAIN_CHARACTORID, PlayerDto.id, PlayerDto.id, 
                10000, pSkillId, PlayerDto.charactorId, new List<int>(){ pSkillId });
            tDemoVideo.bteam = SimulateVideoTeam(MTYPE_MONSTER_ID, 2000001, 2000001, 10000, 1, 1, new List<int>(), pEnemyFormation, pEnemyCount);
            return tDemoVideo;
        }

        public static VideoTeam SimulateVideoTeam(int pCharactorType, long pUID, long pPlayerUID, int pHP, int pDefaultSkillId, int pCharacterId, List<int> pActiveSkillIds, int pFormationId = 1, int pEnemyCount = 1)
        {
            VideoTeam tVideoTeam = new VideoTeam();
            List<long> tPlayerIdList = new List<long>(); 
            List<VideoSoldier> tTeamSoldiers = new List<VideoSoldier>();
            VideoSoldier tVideoSoldier;
            Formation tFormation = DataCache.getDtoByCls<Formation>(pFormationId);
            UnityEngine.Assertions.Assert.IsNotNull(tFormation, string.Format("找不到目标阵型配置！pFormationId:{0}", pFormationId));
            for (int tCounter = 0; tCounter < pEnemyCount; tCounter++)
            {
                var uid = pUID + tCounter;
                var pos = tCounter;
                if (uid == PlayerDto.id)
                {
                    pos = 9;
                }
                else if (pEnemyCount < 3)
                {
                    pos = tCounter + 5;
                }
                
                tVideoSoldier = SimulateVideoSoldier(pCharactorType, uid, pPlayerUID + tCounter, pHP, pDefaultSkillId, pCharacterId, pActiveSkillIds, pos);
                tPlayerIdList.Add(tVideoSoldier.id);
                tTeamSoldiers.Add(tVideoSoldier);
            }
            tVideoTeam.playerIds = tPlayerIdList;
            tVideoTeam.teamSoldiers = tTeamSoldiers;
            tVideoTeam.formationId = pFormationId;
            return tVideoTeam;
        }

        public static VideoSoldier SimulateVideoSoldier(int pCharactorType, long pUID, long pPlayerUID, int pHP, int pDefaultSkillId, int pCharacterId, List<int> pActiveSkillIds, int pPosition)
        {
            VideoSoldier tVideoSoldier = new VideoSoldier();
            if (pCharactorType == MTYPE_MAIN_CHARACTORID)
                tVideoSoldier.id = pUID;
            else
                tVideoSoldier.id = System.DateTime.Now.Ticks;
            tVideoSoldier.name = (pUID == PlayerDto.id) ? "我" : ("C" + pUID);
            tVideoSoldier.playerId = pPlayerUID;
            tVideoSoldier.charactorType = (int)pCharactorType;
            tVideoSoldier.defaultSkillId = pDefaultSkillId;
            tVideoSoldier.hp = pHP;
            tVideoSoldier.maxHp = pHP;
            tVideoSoldier.position = pPosition;
            tVideoSoldier.charactorId = pCharacterId;
            return tVideoSoldier;
        }

        #endregion

        #region VideoRound simulate

        #region interface

        public static VideoRound SimulateVideoRound(List<VideoSoldier> pTeamSoldiersPlayer, List<VideoSoldier> pTeamSoldiersEnemy, long pAttackId = 0, Skill pSkill = null)
        {
            long tCurAttackId = 0;
            long tUselessDefenderId = 0;
            VideoSoldier tVideoSoldier = null;
            SimulateActionPlayerInfo(out tCurAttackId, out tUselessDefenderId, out tVideoSoldier, pTeamSoldiersPlayer, pTeamSoldiersEnemy, pAttackId);

            VideoRound tVideoRound = new VideoRound();
            tVideoRound.battleId = SIMULATE_BATTLE_ID;
            tVideoRound.skillActions = SimulateVideoSkillActionList(pTeamSoldiersPlayer, pTeamSoldiersEnemy, tCurAttackId, pSkill);
            return tVideoRound;
        }

        #endregion

        private static bool SimulateActionPlayerInfo(out long tCurAttackId, out long tDefenderId, out VideoSoldier tVideoSoldier, List<VideoSoldier> pTeamSoldiersPlayer, List<VideoSoldier> pTeamSoldiersEnemy, long pAttackId = 0)
        {
            UnityEngine.Assertions.Assert.IsNotNull(pTeamSoldiersEnemy, "敌人队伍数据不得为空！");
            tCurAttackId = PlayerDto.id;
            tDefenderId = pTeamSoldiersEnemy[0].id;
            if (pAttackId > 0)
            {
                tCurAttackId = pAttackId;
                if (IsEnemy(pAttackId, pTeamSoldiersEnemy))
                    tDefenderId = pTeamSoldiersPlayer[UnityEngine.Random.Range(0, pTeamSoldiersPlayer.Count)].id;
                else
                    tDefenderId = pTeamSoldiersEnemy[UnityEngine.Random.Range(0, pTeamSoldiersEnemy.Count)].id;
            }
            List<VideoSoldier> tAllVideoSoldierList = new List<VideoSoldier>(pTeamSoldiersPlayer);
            tAllVideoSoldierList.AddRange(pTeamSoldiersEnemy);
            tVideoSoldier = GetVideoSoldierBySoldierUID(tCurAttackId, tAllVideoSoldierList);
            return true;
        }

        private static List<VideoSkillAction> SimulateVideoSkillActionList(List<VideoSoldier> pTeamSoldiersPlayer, List<VideoSoldier> pTeamSoldiersEnemy, long pAttackId = 0, Skill pSkill = null)
        {
            List<VideoSkillAction> tVideoSkillActionList = new List<VideoSkillAction>();
            for (int tCounter = 0; tCounter < 1; tCounter++)
            {
                tVideoSkillActionList.Add(SimulateVideoSkillAction(pTeamSoldiersPlayer, pTeamSoldiersEnemy, pAttackId, pSkill));
            }
            return tVideoSkillActionList;
        }

        private static VideoSkillAction SimulateVideoSkillAction(List<VideoSoldier> pTeamSoldiersPlayer, List<VideoSoldier> pTeamSoldiersEnemy, long pAttackId = 0, Skill pSkill = null)
        {
            int tSkillID = 0;
            long tCurAttackId = 0;
            long tDefenderId = 0;
            VideoSoldier tVideoSoldier = null;
            List<VideoTargetStateGroup> tTargetStateGroups = null;
            SimulateActionPlayerInfo(out tCurAttackId, out tDefenderId, out tVideoSoldier, pTeamSoldiersPlayer, pTeamSoldiersEnemy, pAttackId);   

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


            VideoSkillAction tVideoSkillAction = new VideoSkillAction();
            tVideoSkillAction.actionSoldierId = tCurAttackId;
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
            List<VideoTargetStateGroup> tVideoTargetStateGroupList = new List<VideoTargetStateGroup>();
            if (null == pDefenderIdList || pDefenderIdList.Count <= 0)
                return null;
            for (int tCounter = 0; tCounter < pDefenderIdList.Count; tCounter++)
            {
                tVideoTargetStateGroupList.Add(SimulateVideoTargetStateGroup(pDefenderIdList[tCounter], pDamage));
            }
            return tVideoTargetStateGroupList;
        }

        private static VideoTargetStateGroup SimulateVideoTargetStateGroup(long pDefenderId, int pBuffId = 0, bool pAdd = false, int pDamage = 0)
        {
            VideoTargetStateGroup tVideoTargetStateGroup = new VideoTargetStateGroup();
            tVideoTargetStateGroup.targetStates = SimulateVideoActionTargetStateList(pDefenderId, pBuffId, pAdd, pDamage);
            return tVideoTargetStateGroup;
        }

        private static VideoActionTargetState SimulateVideoActionTargetState(long pDefenderId, int pDamage = 0)
        {
            var tVideoActionTargetState = new VideoActionTargetState();
            tVideoActionTargetState.crit = false;
            tVideoActionTargetState.currentHp = pDamage == 0 ? 1000 : (pDamage + 1000);
            tVideoActionTargetState.currentCp = 1000;
            tVideoActionTargetState.hp = pDamage == 0 ? -100 : pDamage;
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
            List<VideoTargetState> tVideoTargetStateGroupList = new List<VideoTargetState>();
            for (int tCounter = 0; tCounter < 1; tCounter++)
            {
                tVideoTargetStateGroupList.Add(SimulateVideoActionTargetState(pDefenderId, pDamage) as VideoTargetState);
            }
            return tVideoTargetStateGroupList;
        }

        #endregion

        #region 辅助方法

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

        private static bool IsEnemy(long pUID, List<VideoSoldier> pEnemyVideoSoldierList)
        {
            UnityEngine.Assertions.Assert.IsNotNull(pEnemyVideoSoldierList, "敌方列表不得为空！");
            pEnemyVideoSoldierList.Exists((pVideoSoldier) =>
                {
                    return null != pVideoSoldier && pVideoSoldier.id == pUID;
                });
            return false;
        }

        public static VideoSoldier GetVideoSoldierBySoldierUID(long pUID, Video pVideo)
        {
            List<VideoSoldier> tAllVideoSoldierList = new List<VideoSoldier>(pVideo.ateam.teamSoldiers);
            tAllVideoSoldierList.AddRange(pVideo.bteam.teamSoldiers);
            return GetVideoSoldierBySoldierUID(pUID, tAllVideoSoldierList);
        }

        public static VideoSoldier GetVideoSoldierBySoldierUID(long pUID, List<VideoSoldier> pAllVideoSoldierList)
        {
            UnityEngine.Assertions.Assert.IsNotNull(pAllVideoSoldierList, "敌我全体列表不得为空！");
            pAllVideoSoldierList.Find((pVideoSoldier) =>
                {
                    return null != pVideoSoldier && pVideoSoldier.id == pUID;
                });
            return null;
        }

        #endregion
    }
}

