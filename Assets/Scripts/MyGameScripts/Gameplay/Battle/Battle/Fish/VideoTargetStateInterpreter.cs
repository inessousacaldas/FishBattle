using Fish;

namespace AppDto
{
    //using vistor pattern to calculate battle play controler
    public partial class VideoTargetState
    {
        public virtual IBattlePlayCtl Interpret(VideoTargetStateGroup targetStateGroup, SkillConfigInfo skillCfg,
            Skill skill)
        {
            return null;
        }
    }

    public partial class VideoActionTargetState
    {
        public override IBattlePlayCtl Interpret(VideoTargetStateGroup targetStateGroup, SkillConfigInfo skillCfg,
            Skill skill)
        {
            return new VedioActionTargetStatePlayCtl(this,targetStateGroup);
        }
    }

    public partial class VideoBuffAddTargetState
    {
        public override IBattlePlayCtl Interpret(VideoTargetStateGroup targetStateGroup, SkillConfigInfo skillCfg,
            Skill skill)
        {
            return base.Interpret(targetStateGroup, skillCfg, skill);
        }
    }

    public partial class VideoBuffRemoveTargetState
    {
        public override IBattlePlayCtl Interpret(VideoTargetStateGroup targetStateGroup, SkillConfigInfo skillCfg,
            Skill skill)
        {
            return base.Interpret(targetStateGroup, skillCfg, skill);
        }
    }

    public partial class VideoDodgeTargetState
    {
        public override IBattlePlayCtl Interpret(VideoTargetStateGroup targetStateGroup, SkillConfigInfo skillCfg,
            Skill skill)
        {
            return base.Interpret(targetStateGroup, skillCfg, skill);
        }
    }

    public partial class VideoDrivingTargetState
    {
        public override IBattlePlayCtl Interpret(VideoTargetStateGroup targetStateGroup, SkillConfigInfo skillCfg,
            Skill skill)
        {
            return base.Interpret(targetStateGroup, skillCfg, skill);
        }
    }

    public partial class VideoRetreatState
    {
        public override IBattlePlayCtl Interpret(VideoTargetStateGroup targetStateGroup, SkillConfigInfo skillCfg,
            Skill skill)
        {
            return base.Interpret(targetStateGroup, skillCfg, skill);
        }
    }

    public partial class VideoSoldierSwtichState
    {
        public override IBattlePlayCtl Interpret(VideoTargetStateGroup targetStateGroup, SkillConfigInfo skillCfg,
            Skill skill)
        {
            return base.Interpret(targetStateGroup, skillCfg, skill);
        }
    }

    public partial class VideoTargetExceptionState
    {
        public override IBattlePlayCtl Interpret(VideoTargetStateGroup targetStateGroup, SkillConfigInfo skillCfg,
            Skill skill)
        {
            return base.Interpret(targetStateGroup, skillCfg, skill);
        }
    }
}