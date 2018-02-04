using System;
using System.Collections.Generic;
using AppDto;

namespace Fish
{
    public class VideoRoundInterpreter
    {
        private SkillConfigInfo GetSkillConfig(VideoSkillAction vsAct)
        {
            throw new NotImplementedException();
        }

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
            var combined = new ParallCompositePlayCtl(allSubPlayCtl);

            var overPlayCtl = InterpreteFightOver(vRound);

            return combined.Chain(overPlayCtl);
        }

        private IBattlePlayCtl InterpreteFightOver(VideoRound vRound)
        {
            throw new NotImplementedException();
        }

        private IBattlePlayCtl InterpreteVideoSkillAction(VideoSkillAction vsAct)
        {
            if (vsAct.actionSoldierId == 0)
            {
                return InterpreteTargetStateGroupList(vsAct.targetStateGroups);
            }
            
            var skill = DataCache.getDtoByCls<Skill>(vsAct.skillId);

            if (skill == null)
            {
                GameLog.Log_BattleError("invalid skillActions: skill not found"+vsAct.skillId);
                return null;
            }
            
            var skillCfg = GetSkillConfig(vsAct);
            if (skillCfg == null)
            {
                GameLog.Log_BattleError("invalid skillActions: skill config not found"+vsAct.skillId);
                return null;
            }

            var playSkillNameCtl = InterpreteSkillName(vsAct);
            
            var combined = InterpreteTargetStateGroupList(vsAct.targetStateGroups);

            return playSkillNameCtl.Chain(combined);
        }

        private IBattlePlayCtl InterpreteSkillName(VideoSkillAction vsAct)
        {
            throw new NotImplementedException();
        }

        private IBattlePlayCtl InterpreteTargetStateGroupList(List<VideoTargetStateGroup> vsActTargetStateGroups)
        {
            //reference: BattleStateHandler.HandleBattleStateGroup
            var allSubPlayCtl = new List<IBattlePlayCtl>(vsActTargetStateGroups.Count);
            for (var i = 0; i < vsActTargetStateGroups.Count; i++)
            {
                var targetStateGroup = vsActTargetStateGroups[i];
                var targetPlayCtl = InterpreteTargetStateGroup(targetStateGroup);
                allSubPlayCtl.Add(targetPlayCtl);
            }
            var combined = new ParallCompositePlayCtl(allSubPlayCtl);
            return combined;
        }

        private IBattlePlayCtl InterpreteTargetStateGroup(VideoTargetStateGroup targetStateGroup)
        {
            var videoTargetStates = targetStateGroup.targetStates;
            var allSubPlayCtl = new List<IBattlePlayCtl>(videoTargetStates.Count);
            for (var i = 0; i < videoTargetStates.Count; i++)
            {
                var targetState = videoTargetStates[i];
                var targetPlayCtl = InterpreteTargetState(targetState);
                allSubPlayCtl.Add(targetPlayCtl);
            }
            var combined = new ParallCompositePlayCtl(allSubPlayCtl);
            return combined;
        }

        private IBattlePlayCtl InterpreteTargetState(VideoTargetState targetState)
        {
            //for each type of VideoTargetState, do their own interpretation!
            return targetState.Interprete();
        }
    }
}