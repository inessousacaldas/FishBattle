using System.Collections.Generic;
using AppDto;
using Fish;

public abstract partial class BattlePhraseBase
{
    public abstract IBattlePlayCtl Interprete(Skill skill, VideoSkillAction vsAct);
}

public partial class SeqPhrase
{
    public override IBattlePlayCtl Interprete(Skill skill, VideoSkillAction vsAct)
    {
        var ctlList = new List<IBattlePlayCtl>();
        foreach (var phrase in _lst)
        {
            var ctl = phrase.Interprete(skill, vsAct);
            ctlList.Add(ctl);
        }

        return SeqCompositePlayCtl.Create(ctlList);
    }
}

public partial class ParPhrase
{
    public override IBattlePlayCtl Interprete(Skill skill, VideoSkillAction vsAct)
    {
        var ctlList = new List<IBattlePlayCtl>();
        foreach (var phrase in _lst)
        {
            var ctl = phrase.Interprete(skill, vsAct);
            ctlList.Add(ctl);
        }

        return ParallCompositePlayCtl.Create(ctlList);
    }
}

public partial class BranchPhrase
{
    public override IBattlePlayCtl Interprete(Skill skill, VideoSkillAction vsAct)
    {
        var m = _main.Interprete(skill, vsAct);
        var o = _other.Interprete(skill, vsAct);
        if (o == null)
            return m;
        return BranchCompositePlayCtl.Create(m, o);
    }
}

public partial class WaitPhrase
{
    public override IBattlePlayCtl Interprete(Skill skill, VideoSkillAction vsAct)
    {
        return new KeepStatePlayCtl(_duration);
    }
}

public partial class ActionPhrase
{
    public override IBattlePlayCtl Interprete(Skill skill, VideoSkillAction vsAct)
    {
        return _actInfo.Interprete(skill, vsAct);
    }
}

public partial class EffectPhrase
{
    public override IBattlePlayCtl Interprete(Skill skill, VideoSkillAction vsAct)
    {
        return _effCfg.Interprete(skill, vsAct);
    }
}