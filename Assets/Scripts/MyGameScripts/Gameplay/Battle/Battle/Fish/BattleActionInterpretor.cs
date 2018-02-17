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


    private List<long> _victimList;

    protected List<long> GetVictimList(VideoSkillAction vsAct, Skill skill)
    {
        if (_victimList != null) return _victimList;
        
        _victimList = new List<long>();
        var vsActTargetStateGroups = vsAct.targetStateGroups;
        List<long> callSoldierIds=new List<long>();
        for (var i = 0; i < vsActTargetStateGroups.Count; i++)
        {
            var injureId = GetInjureId(vsActTargetStateGroups[i], callSoldierIds);
            if (injureId >0)
                _victimList.Add(injureId);
        }

        return _victimList;
    }

    private long GetInjureId(VideoTargetStateGroup group,List<long> callSoldierIds)
    {
        var groupTargetStates = group.targetStates;
        for (var i = 0; i < groupTargetStates.Count; i++)
        {
            var state = groupTargetStates[i];
            if (state is VideoRetreatState)
                continue;
			
            var videoBuffAddTargetState = state as VideoBuffAddTargetState;
            if (videoBuffAddTargetState != null)
            {
                if (callSoldierIds.Contains(videoBuffAddTargetState.id))
                {
                    continue;
                }

                //当只有buff添加state的时候， 才考虑加入受击者，如果不是， 就不需要加入受击者
                if (groupTargetStates.Count > 1)
                {
                    continue;
                }
            }
            if (state.id > 0)
            {
                return state.id;
            }
        }

        return 0;
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
                return AnimatorPlayCtrl.Create(this, skill, vsAct,vsAct.actionSoldierId);
            }
            
            case ActionInitiator.Victim:
            {
                var victimList = GetVictimList(vsAct,skill);
                var aniPlayList = new List<IBattlePlayCtl>(victimList.Count);
                for (var i = 0; i < victimList.Count; i++)
                {
                    var ctl = AnimatorPlayCtrl.Create(this, skill, vsAct,victimList[i]);
                    aniPlayList.Add(ctl);
                }

                switch (instMode)
                {
                    case ActionSchemeInstantiationMode.Seq:
                        return SeqCompositePlayCtl.Create(aniPlayList);
                    case ActionSchemeInstantiationMode.Par:
                        return ParallCompositePlayCtl.Create(aniPlayList);
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
                var victimList = GetVictimList(vsAct,skill);
                return TweenMovePlayCtl.Create(this,skill,vsAct,vsAct.actionSoldierId,victimList[0]);
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
                var victimList = GetVictimList(vsAct,skill);
                return TweenMoveBackPlayCtl.Create(this,skill,vsAct,vsAct.actionSoldierId,victimList[0]);
        }

        return null;
    }
}