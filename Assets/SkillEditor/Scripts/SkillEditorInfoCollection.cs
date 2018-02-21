#if ENABLE_SKILLEDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppDto;
using AssetPipeline;
using UnityEngine;
using Fish;
using Newtonsoft.Json;
using JsonC = Newtonsoft.Json.JsonConvert;

namespace SkillEditor
{
    public static class SkillEditorInfoCollection
    {
        private static List<int> _cacheBattleSceneList;
        private static StringBuilder _cacheSB = new StringBuilder();
        private static List<CharactorAgent> _cacheCharactorAgents;

        public static List<int> GetBattleSceneList()
        {
            if (_cacheBattleSceneList == null)
            {
                _cacheBattleSceneList = SkillEditorConst.BattleScenes;
            }
            return _cacheBattleSceneList;
        }


        public static Camera GetBattleCamera()
        {
            return LayerManager.Root.BattleCamera_Camera;
        }


        public static List<CharactorAgent> GetCharacterList()
        {
            if (_cacheCharactorAgents == null)
            {
                _cacheCharactorAgents =
                    DataCache.getArrayByCls<GeneralCharactor>()
                        .Select(charactor => new CharactorAgent(charactor))
                        .ToList();
            }
            return _cacheCharactorAgents;
        }

        public static SkillConfigInfo DeepCopySkillInfo(SkillConfigInfo info)
        {
            var json = JsHelper.ToJson(info);
            return JsHelper.ToObject<JsonSkillConfigInfo>(json).ToSkillConfigInfo();
        }

        public static string GetSkillInfoName(CorrectSkillConfig info)
        {
            return string.Format("{0}, {1}", info.id, info.name);
        }

        public static string GetCharcterName(CharactorAgent charac)
        {
            return charac.Charactor.name;
        }

        public static int GetCharacterModelId(CharactorAgent charac)
        {
            return charac.Charactor.id;
        }

        public static string GetNodeItemName(SkillEditorInfoNode node, int mainIndex, int subIndex)
        {
            _cacheSB.Length = 0;

            if (node.IsActionInfo())
            {
                _cacheSB.Append(mainIndex.ToString().PadRight(4, ' '));
            }
            else
            {
                _cacheSB.Append(string.Format("{0}.{1}", mainIndex, subIndex).PadRight(4, ' '));
            }
            var nodeType = node.IsActionInfo() ? node.ActionInfo.GetType() : node.EffectInfo.GetType();
            _cacheSB.AppendLine(SkillEditorInfoTypeNodeConfig.ShowTypeNodeDict[nodeType].Name);
            _cacheSB.Append(string.Format("{0} ", node.IsAttack ? "攻方" : "受方"));
            if (!node.IsActionInfo())
            {
                _cacheSB.Append(string.Format("延迟{0}s", node.EffectInfo.playTime));
            }

            return _cacheSB.ToString();
        }

        public static List<CorrectSkillConfig> LoadBattleConfig()
        {
            return OldBattleConfigConverter.LoadConvertedBattleConfigList();
        }

        public static void SaveBattleConfig(List<CorrectSkillConfig> skillInfos)
        {
            OldBattleConfigConverter.SaveCorrectBattleConfigInfo(skillInfos);
            /*var skillConfig = new BattleConfigInfo();
            skillConfig.time = (DateTime.UtcNow.Ticks / 10000).ToString();
            skillConfig.list = skillInfos;
            var jsonStr = skillConfig.ToBattleJsonStr();
            FileHelper.SaveJsonObj(skillConfig, OldBattleConfigConverter.BattleConfig_Path);*/
            UnityEditor.AssetDatabase.Refresh();
        }

        // 不在编辑器那里修复一些数值，在这里进行
        public static void FixInfoNode(SkillEditorInfoNode infoNode)
        {
            if (infoNode.IsActionInfo())
            {
                var actionInfo = infoNode.ActionInfo;
                actionInfo.effects = new List<BaseEffectInfo>();
                actionInfo.type = actionInfo.GetType().GetField("TYPE").GetValue(actionInfo).ToString();
            }
            else
            {
                var effectInfo = infoNode.EffectInfo;
                effectInfo.type = effectInfo.GetType().GetField("TYPE").GetValue(effectInfo).ToString();
            }
        }


        #region 场景化需要的信息
        public static Video SimulateVideo(List<int> uidList, List<int> posList, int teamANum, int charId)
        {
            var video = new Video();
            video.ateam = SimulateVideoTeam(uidList.GetRange(0, teamANum), posList.GetRange(0, teamANum), charId, SkillEditorConst.MainCharactorType, SkillEditorConst.DefaultHp,
                SkillEditorConst.MaxHp);
            video.bteam = SimulateVideoTeam(uidList.GetRange(teamANum, uidList.Count - teamANum), posList.GetRange(teamANum, uidList.Count - teamANum), charId, SkillEditorConst.MainCharactorType, SkillEditorConst.DefaultHp,
                SkillEditorConst.MaxHp);

            return video;
        }


        private static VideoTeam SimulateVideoTeam(List<int> uidList, List<int> posList, int charId, int charType, int hp, int maxHp)
        {
            var vt = new VideoTeam();
            vt.teamSoldiers = new List<VideoSoldier>();
            for (int i = 0; i < uidList.Count; i++)
            {
                vt.teamSoldiers.Add(SimulateVideoSoldier(uidList[i], charId, charType, hp, maxHp, posList[i]));
            }

            return vt;
        }


        private static VideoSoldier SimulateVideoSoldier(int uid, int charId, int charType, int hp, int maxHp, int pos)
        {
            var vs = new VideoSoldier();
            vs.id = uid;
            vs.name = uid.ToString();
            vs.playerId = uid;
            vs.charactorId = charId;
            vs.charactorType = charType;
            vs.hp = hp;
            vs.maxHp = maxHp;
            vs.position = pos;

            return vs;
        }
        #endregion

        #region 播放技能战斗的信息
        public static VideoRound SimulateVideoRound(List<VideoSkillAction> actionList)
        {
            var round = new VideoRound();
            round.skillActions = actionList;

            return round;
        }

        public static List<VideoSkillAction> SimulateVideoActionList(VideoSkillAction action)
        {
            var actionList = new List<VideoSkillAction>();
            // 考虑单人操作就可以了
            actionList.Add(action);

            return actionList;
        }

        public static VideoSkillAction SimulateVideoAction(long attackUid, int skillId, List<VideoTargetStateGroup> groupList)
        {
            var action = new VideoSkillAction();
            action.actionSoldierId = attackUid;
            action.skillId = skillId;

            action.targetStateGroups = groupList;

            return action;
        }

        public static VideoTargetStateGroup SimulateVideoTargetStateGroup(List<VideoTargetState> stateList)
        {
            var group = new VideoTargetStateGroup();
            group.targetStates = stateList;

            return group;
        }

        public static VideoTargetState SimulateVideoTargetState(long defendUid, int curHp, int changeHp, bool crit)
        {
            var state = new VideoActionTargetState();
            state.id = defendUid;
            state.currentHp = curHp;
            state.hp = changeHp;
            state.crit = crit;

            return state;
        }

        public static CorrectSkillConfig DeepCopySkillInfo(CorrectSkillConfig info)
        {
            return OldBattleConfigConverter.DeepCopySkillInfo(info);
        }
    }
    #endregion
}

#endif