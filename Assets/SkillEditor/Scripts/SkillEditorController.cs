using System.Collections.Generic;
using System.Linq;
using AppDto;
using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public class SkillEditorController
    {
        public enum SkillActionType
        {
            DelHp,
            AddHp,
            Buff,
        }

        private static SkillEditorController _instance;

        public static SkillEditorController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SkillEditorController();
                }

                return _instance;
            }
        }


        private bool _isFirstEnter = true;

        private List<SkillConfigInfo> _skillInfoList;

        public List<SkillConfigInfo> SkillInfoList
        {
            get { return _skillInfoList; }
        }

        private List<int> _maxPositionList;

        private List<int> MaxPositionList
        {
            get
            {
                if (_maxPositionList == null)
                {
                    _maxPositionList = new List<int>(SkillEditorConst.BattleSideMaxNum);
                    for (int i = 0; i < SkillEditorConst.BattleSideMaxNum; i++)
                    {
                        _maxPositionList.Add(i);
                    }
                }
                return _maxPositionList;
            }
        }

        private CharactorAgent _curChar;

        public CharactorAgent CurChar
        {
            get { return _curChar; }
        }

        public void EnterBattle()
        {
            EnterScene(SkillEditorInfoCollection.GetBattleSceneList().Random());
        }


        #region 场景
        private void EnterScene(int sceneId)
        {
            SceneFadeEffectController.Show(sceneId, false, sceneId, true, OnBattleMapLoadFinshed, null);
        }

        private void OnBattleMapLoadFinshed(int id)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(string.Format("Scene_{0}", id)));
            InitBattle();
        }

        private void InitBattle()
        {
            if (_isFirstEnter)
            {
                _isFirstEnter = false;

                _skillInfoList = SkillEditorInfoCollection.LoadBattleConfig();
                UpdateBattle(SkillEditorConst.DefaultMemberNum, SkillEditorConst.DefaultMemberNum,
                    SkillEditorInfoCollection.GetCharacterList().Random());

                ProxySkillEditorModule.OpenMainView();
            }
        }

        public void UpdateBattleCameraView(float x, float y, float width, float height)
        {
            var camera = SkillEditorInfoCollection.GetBattleCamera();
            camera.rect = new Rect(x, y, width, height);
        }

        #endregion

        #region 技能编辑

        private void UpdateSkillInfoWithoutSave(SkillConfigInfo skillInfo)
        {
            skillInfo = SkillEditorInfoCollection.DeepCopySkillInfo(skillInfo);
            BattleConfigManager.Instance.UpdateSkillConfigInfo(skillInfo);
        }

        public void AddOrUpdateSkillInfo(SkillConfigInfo skillInfo)
        {
            UpdateSkillInfoWithoutSave(skillInfo);

            skillInfo = SkillEditorInfoCollection.DeepCopySkillInfo(skillInfo);
            var index = _skillInfoList.FindIndex(info => info.id == skillInfo.id);
            if (index >= 0)
            {
                _skillInfoList[index] = skillInfo;
            }
            else
            {
                for (int i = _skillInfoList.Count - 1; i >= 0; i--)
                {
                    if (_skillInfoList[i].id < skillInfo.id)
                    {
                        _skillInfoList.Insert(i + 1, skillInfo);
                        break;
                    }
                }
            }

            SaveBattleConfig();
        }

        public bool DeleteSkillInfo(int id)
        {
            if (_skillInfoList.Remove(info => info.id == id))
            {
                SaveBattleConfig();
                return true;
            }

            return false;
        }


        private void SaveBattleConfig()
        {
            SkillEditorInfoCollection.SaveBattleConfig(_skillInfoList);
        }
        #endregion


        public void PlayBattle(int teamANum, int teamBNum, SkillConfigInfo info, long attackUid, long defendUid, int targetNum, bool atOnce,
            int multipart)
        {
            if (info == null)
            {
                return;
            }

            UpdateSkillInfoWithoutSave(info);

            if (teamANum != GetMonsterListNum(BattlePositionAgent.BattleSide.TeamA) ||
                teamBNum != GetMonsterListNum(BattlePositionAgent.BattleSide.TeamB))
            {
                UpdateBattle(teamANum, teamBNum, _curChar);
            }

            var player = new GameVideoGeneralActionPlayer();
            player.Excute(CreateVideoRound(info, attackUid, defendUid, targetNum, atOnce, multipart).skillActions[0]);
        }


        #region 人物及战斗编辑
        private Video CreateVideo(int teamANum, int teamBNum, int charId)
        {
            var uidList = new List<int>();
            for (int i = 0; i < teamANum + teamBNum; i++)
            {
                uidList.Add(i + 1);
            }
            var posList = new List<int>();
            posList.AddRange(GetRandomPosList(teamANum));
            posList.AddRange(GetRandomPosList(teamBNum));

            return SkillEditorInfoCollection.SimulateVideo(uidList, posList, teamANum, charId);
        }

        private List<int> GetRandomPosList(int num)
        {
            var list = MaxPositionList.ToList();
            for (int i = MaxPositionList.Count - num - 1; i >= 0; i--)
            {
                list.RemoveAt(Random.Range(0, list.Count));
            }

            return list;
        }

        private void ClearMonsters()
        {
            //            BattleDataManager.MonsterManager.Instance.Dispose();
        }

        public void UpdateBattle(int teamANum, int teamBNum, CharactorAgent charac)
        {
            _curChar = charac;
            CreateMonsters(CreateVideo(teamANum, teamBNum, SkillEditorInfoCollection.GetCharacterModelId(_curChar)));
        }

        private void CreateMonsters(Video video)
        {
            ClearMonsters();
            BattleDataManager.MonsterManager.Instance.ShowMonsters(video);
            //            CreateMonsters(video.ateam.teamSoldiers, BattlePosition.MonsterSide.Player);
            //            CreateMonsters(video.bteam.teamSoldiers, BattlePosition.MonsterSide.Enemy);
        }

        private void CreateMonsters(List<VideoSoldier> vsList, BattlePositionAgent.BattleSide side)
        {
            foreach (var videoSoldier in vsList)
            {
                CreateMonster(videoSoldier, side);
            }
        }

        private MonsterController CreateMonster(VideoSoldier vs, BattlePositionAgent.BattleSide side)
        {
            var go = new GameObject();
            var mc = go.AddComponent<MonsterController>();
            GameObjectExt.AddPoolChild(LayerManager.Root.BattleActors, go);

            //            mc.transform.localEulerAngles = BattlePositionCalculator.GetMonsterRotationD(vs.position, side);
            //            mc.transform.localScale = Vector3.one;
            //            mc.SetPosition(BattlePositionCalculator.GetMonsterPosition(vs, side));

            BattleDataManager.MonsterManager.Instance.AddMonsterController(mc);
            mc.InitMonster(vs, BattlePositionAgent.ConvertBattleSide(side));

            return mc;
        }


        public List<MonsterController> GetMonsterList(BattlePositionAgent.BattleSide side)
        {
            return BattleDataManager.MonsterManager.Instance.GetMonsterList(BattlePositionAgent.ConvertBattleSide(side)).ToList();
        }

        public int GetMonsterListNum(BattlePositionAgent.BattleSide side)
        {
            return BattleDataManager.MonsterManager.Instance.GetMonsterList(BattlePositionAgent.ConvertBattleSide(side)).ToList().Count;
        }
        #endregion


        #region 模拟数据计算表现

        private VideoRound CreateVideoRound(SkillConfigInfo info, long attackUid, long defendUid, int targetNum, bool atOnce,
            int multipart)
        {
            var skill = FixSkill(info, targetNum, atOnce, multipart);

            var defendMC = BattleDataManager.MonsterManager.Instance.GetMonsterFromSoldierID(defendUid);
            var side = defendMC.side;
            List<MonsterController> defendMCList = null;
            if (targetNum > 1)
            {
                defendMCList = GetMonsterList(BattlePositionAgent.ConvertBattleSide(side));
                var delNum = defendMCList.Count - targetNum;
                for (int i = 0; i < delNum; i++)
                {
                    defendMCList.RemoveAt(Random.Range(0, defendMCList.Count));
                }
            }
            else
            {
                defendMCList = new List<MonsterController>();
                defendMCList.Add(defendMC);
            }

            var sameSide = BattleDataManager.MonsterManager.Instance.GetMonsterFromSoldierID(attackUid).side == side;

            var groupList = new List<VideoTargetStateGroup>();
            foreach (var mc in defendMCList)
            {
                for (int i = 0; i < multipart; i++)
                {
                    var stateList = new List<VideoTargetState>();
                    var changeHp = GetChangeHp(skill, sameSide);
                    var curHp = mc.currentHP + changeHp;

                    stateList.Add(SkillEditorInfoCollection.SimulateVideoTargetState(mc.GetId(), curHp, changeHp, IsCritical(skill, sameSide)));

                    groupList.Add(SkillEditorInfoCollection.SimulateVideoTargetStateGroup(stateList));
                }
            }

            var round =
                SkillEditorInfoCollection.SimulateVideoRound(
                    SkillEditorInfoCollection.SimulateVideoActionList(
                        SkillEditorInfoCollection.SimulateVideoAction(attackUid, info.id, groupList)));

            return round;
        }


        private Skill FixSkill(SkillConfigInfo info, int targetNum, bool atOnce, int multipart)
        {
            var skillId = info.id;
            var skill = DataCache.getDtoByCls<Skill>(skillId);
            if (skill == null)
            {
                skill = new Skill();

                skill.id = skillId;
                skill.name = info.name;
                skill.type = (int)Skill.SkillEnum.Crafts;
                if (info.attackerActions.First().type == MoveActionInfo.TYPE)
                {
                    skill.singleActionPlayTime = SkillEditorConst.MoveActionPlayTime;
                    skill.skillAttackType = (int)Skill.SkillAttackType.PhyAttack;
                    skill.clientSkillType = SkillEditorConst.NearClientAttackSkillType;
                }
                else
                {
                    skill.singleActionPlayTime = SkillEditorConst.WithoudMoveActionPlayTime;
                    skill.skillAttackType = (int)Skill.SkillAttackType.MagicAttack;
                    skill.clientSkillType = SkillEditorConst.FarClientAttackSkillType;
                }

                DataCache.getDicByCls<Skill>()[skillId] = skill;
            }

            skill.targetNum = targetNum;
            skill.atOnce = atOnce;

            return skill;
        }

        private int GetChangeHp(Skill skill, bool sameSide)
        {
            var type = GetSkillActionType(skill, sameSide);
            var changeHp = 0;
            switch (type)
            {
                case SkillActionType.DelHp:
                    {
                        changeHp = -Random.Range(SkillEditorConst.ChangeMinHp, SkillEditorConst.ChangeMaxHp);
                        break;
                    }
                case SkillActionType.AddHp:
                    {
                        changeHp = Random.Range(SkillEditorConst.ChangeMinHp, SkillEditorConst.ChangeMaxHp);
                        break;
                    }
                case SkillActionType.Buff:
                    {
                        break;
                    }
            }

            return changeHp;
        }

        private bool IsCritical(Skill skill, bool sameSide)
        {
            var type = GetSkillActionType(skill, sameSide);
            var crit = false;
            switch (type)
            {
                case SkillActionType.DelHp:
                    {
                        crit = Random.Range(0, 2) != 0;
                        break;
                    }
                case SkillActionType.AddHp:
                    {
                        break;
                    }
                case SkillActionType.Buff:
                    {
                        break;
                    }
            }

            return crit;
        }


        private SkillActionType GetSkillActionType(Skill skill, bool sameSide)
        {
            return sameSide ? SkillActionType.AddHp : SkillActionType.DelHp;
        }
        #endregion
    }
}

#endif