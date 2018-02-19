using System.Collections.Generic;
using AppDto;
using Fish;
using Newtonsoft.Json;

public partial class BaseActionInfo
{
    public virtual IBattlePlayCtl Interprete(SkillConfigInfo skillCfg, Skill skill, VideoSkillAction vsAct)
    {
        return null;
    }
	
    public virtual IBattlePlayCtl Interprete(Skill skill, VideoSkillAction vsAct)
    {
        return null;
    }
    
    private ModelHelper.AnimType? _animationName;
    protected ModelHelper.AnimType _defaultAnimationName=ModelHelper.AnimType.hit;

    [JsonIgnore]
    public ModelHelper.AnimType AnimationType
    {
        get
        {
            if (!_animationName.HasValue)
                _animationName = name.GetAnimType(_defaultAnimationName);
            return _animationName.Value;
        }
    }
}

public partial class NormalActionInfo
{
    public override IBattlePlayCtl Interprete(Skill skill, VideoSkillAction vsAct)
    {
        switch (initiator)
        {
            case ActionInitiator.Attacker:
            {
                return AnimatorPlayCtrl.Create(this, skill, vsAct,vsAct.actionSoldierId,vsAct.GetAttackerStateGroup(0),-1);
            }
            
            case ActionInitiator.Victim:
            {
                var victimList = vsAct.GetVictimStateGroups();
                var aniPlayList = new List<IBattlePlayCtl>(vsAct.GetVictimStateGroupCount());
                var index = 0;
                foreach (var tuple in victimList)
                {
                    var ctl = AnimatorPlayCtrl.Create(this, skill, vsAct,tuple.p1,tuple.p2,index);
                    index++;
                    aniPlayList.Add(ctl);
                }

                switch (instMode)
                {
                    case ActionSchemeInstantiationMode.Seq:
                        return aniPlayList.ToSequence();
                    case ActionSchemeInstantiationMode.Par:
                        return aniPlayList.ToParallel();
                }
            }
                break;
        }

        return null;
    }
}

public partial class MoveActionInfo
{
    public MoveActionInfo()
    {
        _defaultAnimationName=ModelHelper.AnimType.battle;
    }

    public override IBattlePlayCtl Interprete(Skill skill, VideoSkillAction vsAct)
    {
        switch (initiator)
        {
            case ActionInitiator.Attacker:
                return TweenMovePlayCtl.Create(this,skill,vsAct,vsAct.actionSoldierId,vsAct.GetVictim(0));
        }

        return null;
    }

    /*public override IBattlePlayCtl Interprete(SkillConfigInfo skillCfg, Skill skill, VideoSkillAction vsAct)
    {
        var allEff = new List<IBattlePlayCtl>();
        for (var i = 0; i < effects.Count; i++)
        {
            var eff = effects[i];
            var effCtl = eff.Interprete(this,skillCfg,skill,vsAct);
            if (effCtl == null) continue;
            allEff.Add(effCtl);
        }
        var palEff= ParallCompositePlayCtl.Create(allEff);
        //TODO
        return null;
    }*/
}

public partial class MoveBackActionInfo
{
    public MoveBackActionInfo()
    {
        _defaultAnimationName = ModelHelper.AnimType.run;
    }

    public override IBattlePlayCtl Interprete(Skill skill, VideoSkillAction vsAct)
    {
        switch (initiator)
        {
            case ActionInitiator.Attacker:
                return TweenMoveBackPlayCtl.Create(this,skill,vsAct,vsAct.actionSoldierId,vsAct.GetVictim(0));
        }

        return null;
    }
}