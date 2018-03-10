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
            var combined = allSubPlayCtl.ToSequence();
            
            //todo fish
            //var a = combined as SeqCompositePlayCtl;
            //var overPlayCtl = InterpreteFightOver(vRound);
            return combined;
            //return combined.Chain(overPlayCtl);
        }

        private CorrectSkillConfig GetSkillConfig(VideoSkillAction vsAct)
        {
            //TODO fish：有特殊情况再说 GameVideoGeneralActionPlayer.DoSkillAction line : 169
            // 需要考虑物品使用等情况
            //return BattleConfigManager.Instance.getSkillConfigInfo(vsAct.skillId);
            return BattleConfigManager.Instance.GetCorrectConfig(vsAct.skillId);
        }

        private IBattlePlayCtl InterpreteFightOver(VideoRound vRound)
        {
            return null;
            throw new NotImplementedException();
        }

        private IBattlePlayCtl InterpreteSkillName(VideoSkillAction vsAct)
        {
            return SkillNamePlayCtrl.Create(vsAct.skill, vsAct.actionSoldierId);
        }

        private IBattlePlayCtl InterpreteVideoSkillAction(VideoSkillAction vsAct)
        {
            //reference : GameVideoGeneralActionPlayer.DoSkillAction
            var skill = vsAct.skill;
            var skillCfg = GetSkillConfig(vsAct);
            if (vsAct.actionSoldierId == 0)
            {
                // todo fish:这种情况应该不需要被特殊化吧？
                return ImmediaBattleActionCtl.Create(() =>
                {
                    BattleStateHandler.HandleBattleStateGroup(0, vsAct.targetStateGroups);
                });
                //return InterpreteTargetStateGroupList(vsAct.targetStateGroups, skillCfg, skill);
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
            var showInjure = ImmediaBattleActionCtl.Create(() =>
            {
                var mc = BattleDataManager.MonsterManager.Instance.GetMonsterFromSoldierID(vsAct.actionSoldierId);
                mc.AddHPMPValue(vsAct);
            });
            var combined = InterpreteSkillInfo(skillCfg, skill, vsAct);

            return playSkillNameCtl.Chain(showInjure).Chain(combined);
        }

        private IBattlePlayCtl InterpreteBattlePhrase(BattlePhraseBase phrase, Skill skill, VideoSkillAction vsAct)
        {
            return phrase.Interprete(skill, vsAct);
        }
        
        private IBattlePlayCtl InterpreteSkillInfo(CorrectSkillConfig skillCfg, Skill skill, VideoSkillAction vsAct)
        {
            return InterpreteBattlePhrase(skillCfg.battlePhrase,skill,vsAct);
            /*var beforeAttack = InterpreteBeforeAttack(skillCfg, skill, vsAct);
            var attackPlayCtl = new List<IBattlePlayCtl>();
            for (var i = 0; i < skillCfg.attackerActions.Count; i++)
            {
                var attPlay = InterpreteActionInfo(skillCfg.attackerActions[i], skillCfg, skill,vsAct);
                if (attPlay == null) continue;
                attackPlayCtl.Add(attPlay);
            }
            var attPlays = SeqCompositePlayCtl.Create(attackPlayCtl);
            var afterAttack=InterpreteAfterAttack(skillCfg, skill, vsAct);
            var allAttackPlay = beforeAttack.Chain(attPlays).Chain(afterAttack);
            
            var beforeHit=InterpreteBeforeHit(skillCfg, skill, vsAct);
            var hitPlayCtl = new List<IBattlePlayCtl>();
            for (var i = 0; i < skillCfg.injurerActions.Count; i++)
            {
                var hitPlay = InterpreteActionInfo(skillCfg.injurerActions[i], skillCfg, skill,vsAct);
                if (hitPlay == null) continue;
                hitPlayCtl.Add(hitPlay);
            }
            var hitPlays=SeqCompositePlayCtl.Create(hitPlayCtl);
            var afterHit=InterpreteAfterHit(skillCfg, skill, vsAct);
            var allHitPlay = beforeHit.Chain(hitPlays).Chain(afterHit);

            return allAttackPlay.Parall(allHitPlay);*/
        }

        private IBattlePlayCtl InterpreteAfterHit(SkillConfigInfo skillCfg, Skill skill, VideoSkillAction vsAct)
        {
            return null;
            throw new NotImplementedException();
        }

        private IBattlePlayCtl InterpreteBeforeHit(SkillConfigInfo skillCfg, Skill skill, VideoSkillAction vsAct)
        {
            return null;
            throw new NotImplementedException();
        }

        private IBattlePlayCtl InterpreteAfterAttack(SkillConfigInfo skillCfg, Skill skill, VideoSkillAction vsAct)
        {
            return null;
            throw new NotImplementedException();
        }

        private IBattlePlayCtl InterpreteBeforeAttack(SkillConfigInfo skillCfg, Skill skill, VideoSkillAction vsAct)
        {
            return null;
            throw new NotImplementedException();
        }

        private IBattlePlayCtl InterpreteActionInfo(BaseActionInfo skillCfgAttackerAction, SkillConfigInfo skillCfg,
            Skill skill, VideoSkillAction vsAct)
        {
            return skillCfgAttackerAction.Interprete(skillCfg,skill,vsAct);
        }

        private IBattlePlayCtl InterpreteTargetStateGroupList(List<VideoTargetStateGroup> vsActTargetStateGroups,
            CorrectSkillConfig skillCfg, Skill skill)
        {
            //reference: BattleStateHandler.HandleBattleStateGroup
            var allSubPlayCtl = new List<IBattlePlayCtl>(vsActTargetStateGroups.Count);
            for (var i = 0; i < vsActTargetStateGroups.Count; i++)
            {
                var targetStateGroup = vsActTargetStateGroups[i];
                var targetPlayCtl = InterpreteTargetStateGroup(targetStateGroup,skillCfg,skill);
                allSubPlayCtl.Add(targetPlayCtl);
            }
            var combined = allSubPlayCtl.ToParallel();
            return combined;
        }

        private IBattlePlayCtl InterpreteTargetStateGroup(VideoTargetStateGroup targetStateGroup,
            CorrectSkillConfig skillCfg, Skill skill)
        {
            var videoTargetStates = targetStateGroup.targetStates;
            var allSubPlayCtl = new List<IBattlePlayCtl>(videoTargetStates.Count);
            for (var i = 0; i < videoTargetStates.Count; i++)
            {
                var targetState = videoTargetStates[i];
                var targetPlayCtl = InterpreteTargetState(targetState,targetStateGroup,skillCfg,skill);
                allSubPlayCtl.Add(targetPlayCtl);
            }
            var combined = allSubPlayCtl.ToParallel();
            return combined;
        }

        private IBattlePlayCtl InterpreteTargetState(VideoTargetState targetState,
            VideoTargetStateGroup targetStateGroup, CorrectSkillConfig skillCfg, Skill skill)
        {
            //reference: BattleStateHandler.PlayState
            //for each type of VideoTargetState, do their own interpretation!
            return targetState.Interpret(targetStateGroup,skillCfg,skill);
        }
    }
}