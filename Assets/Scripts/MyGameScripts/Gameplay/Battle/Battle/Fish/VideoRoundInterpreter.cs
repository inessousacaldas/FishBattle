using System;
using System.Collections.Generic;
using AppDto;

namespace Fish
{
    public class VideoRoundInterpreter
    {
        public IBattlePlayCtl InterpreteVideoRound(VideoRound vRound)
        {
            if (vRound.skillActions.IsNullOrEmpty())
            {
                GameLog.Log_BattleError("invalid skillActions: empty or null");
                return InterpreteFightOver(vRound);
            }
            
            var allSubPlayCtl = new List<IBattlePlayCtl>(vRound.skillActions.Count);
            
            for (var i = 0; i < vRound.skillActions.Count; i++)
            {
                var sAct = vRound.skillActions[i];
                var playCtl = InterpreteVideoSkillAction(sAct);
                allSubPlayCtl.Add(playCtl);
            }
            var combined = new SeqCompositePlayCtl(allSubPlayCtl);

            var overPlayCtl = InterpreteFightOver(vRound);

            return combined.Chain(overPlayCtl);
        }

        private SkillConfigInfo GetSkillConfig(VideoSkillAction vsAct)
        {
            //TODO 有特殊情况再说 GameVideoGeneralActionPlayer.DoSkillAction line : 169
            return BattleConfigManager.Instance.getSkillConfigInfo(vsAct.skillId);
        }

        private IBattlePlayCtl InterpreteFightOver(VideoRound vRound)
        {
            throw new NotImplementedException();
        }

        private IBattlePlayCtl InterpreteSkillName(VideoSkillAction vsAct)
        {
            //TODO ShowSkillNameAndAction
            throw new NotImplementedException();
        }

        private IBattlePlayCtl InterpreteVideoSkillAction(VideoSkillAction vsAct)
        {
            //reference : GameVideoGeneralActionPlayer.DoSkillAction
            var skill = DataCache.getDtoByCls<Skill>(vsAct.skillId);
            var skillCfg = GetSkillConfig(vsAct);
            if (vsAct.actionSoldierId == 0)
            {
                return InterpreteTargetStateGroupList(vsAct.targetStateGroups, skillCfg, skill);
            }
            

            if (skill == null)
            {
                GameLog.Log_BattleError("invalid skillActions: skill not found"+vsAct.skillId);
                return null;
            }
            
            if (skillCfg == null)
            {
                GameLog.Log_BattleError("invalid skillActions: skill config not found"+vsAct.skillId);
                return null;
            }

            var playSkillNameCtl = InterpreteSkillName(vsAct);
            
            var combined = InterpreteTargetStateGroupList(vsAct.targetStateGroups,skillCfg,skill);

            return playSkillNameCtl.Chain(combined);
        }

        private IBattlePlayCtl InterpreteTargetStateGroupList(List<VideoTargetStateGroup> vsActTargetStateGroups,
            SkillConfigInfo skillCfg, Skill skill)
        {
            //reference: BattleStateHandler.HandleBattleStateGroup
            var allSubPlayCtl = new List<IBattlePlayCtl>(vsActTargetStateGroups.Count);
            for (var i = 0; i < vsActTargetStateGroups.Count; i++)
            {
                var targetStateGroup = vsActTargetStateGroups[i];
                var targetPlayCtl = InterpreteTargetStateGroup(targetStateGroup,skillCfg,skill);
                allSubPlayCtl.Add(targetPlayCtl);
            }
            var combined = new ParallCompositePlayCtl(allSubPlayCtl);
            return combined;
        }

        private IBattlePlayCtl InterpreteTargetStateGroup(VideoTargetStateGroup targetStateGroup,
            SkillConfigInfo skillCfg, Skill skill)
        {
            var videoTargetStates = targetStateGroup.targetStates;
            var allSubPlayCtl = new List<IBattlePlayCtl>(videoTargetStates.Count);
            for (var i = 0; i < videoTargetStates.Count; i++)
            {
                var targetState = videoTargetStates[i];
                var targetPlayCtl = InterpreteTargetState(targetState,targetStateGroup,skillCfg,skill);
                allSubPlayCtl.Add(targetPlayCtl);
            }
            var combined = new ParallCompositePlayCtl(allSubPlayCtl);
            return combined;
        }

        private IBattlePlayCtl InterpreteTargetState(VideoTargetState targetState,
            VideoTargetStateGroup targetStateGroup, SkillConfigInfo skillCfg, Skill skill)
        {
            //reference: BattleStateHandler.PlayState
            //for each type of VideoTargetState, do their own interpretation!
            return targetState.Interpret(targetStateGroup,skillCfg,skill);
        }
    }
}