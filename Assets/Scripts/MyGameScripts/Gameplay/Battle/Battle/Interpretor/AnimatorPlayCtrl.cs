using System.Collections.Generic;
using AppDto;

namespace Fish
{
    public class AnimatorPlayCtrl : BattlePlayCtlBasic
    {
        //index用以标记攻击者(-1)以及受击者(0开始)，方便链式攻击特效寻找目标
        public static IBattlePlayCtl Create(
            NormalActionInfo normalActionInfo
            , Skill skill
            , VideoSkillAction vsAct
            , long targetId
            , VideoTargetStateGroup stateGroup
            , int index)
        {
            var delayTime = normalActionInfo.startTime;
            var ctl = new AnimatorPlayCtrl(normalActionInfo, skill, vsAct, targetId, stateGroup);
            if (ctl._mc == null) return null;
            var effList = new List<IBattlePlayCtl>();
            foreach (var eff in normalActionInfo.effects)
            {
                effList.Add(Create(eff, ctl._mc, vsAct, stateGroup, normalActionInfo));
            }

            var waitCtl = (delayTime > 0f) ? KeepStatePlayCtl.Create(delayTime) : null;
            var aa = ImmediaBattleActionCtl.Create(() => ctl._mc.HandleMonsterAfterAction());
            var combinedAct = waitCtl.Chain(ctl).Chain(aa);
            return combinedAct.Branch(effList.ToParallel());
        }

        private static IBattlePlayCtl Create(BaseEffectInfo eff, MonsterController mc, VideoSkillAction vsAct,
            VideoTargetStateGroup stateGroup, NormalActionInfo actInfo)
        {
            var waitCtl = (eff.playTime > 0f) ? KeepStatePlayCtl.Create(eff.playTime) : null;
            var effCtl=ImmediaBattleActionCtl.Create(() => { eff.Play(mc, actInfo, vsAct, stateGroup); });
            return waitCtl.Chain(effCtl);
        }
        
        private NormalActionInfo _actInfo;
        private Skill _skill;
        private VideoSkillAction _vsAct;
        
        private MonsterController _mc;
        
        private readonly float _duration;
        private readonly ModelHelper.AnimType _animationName;
        private readonly long _monsterId;
        private VideoTargetStateGroup _stateGroup;

        private AnimatorPlayCtrl(NormalActionInfo normalActionInfo, Skill skill, VideoSkillAction vsAct, long monsterId,
            VideoTargetStateGroup stateGroup)
        {
            _actInfo = normalActionInfo;
            _skill = skill;
            _vsAct = vsAct;
            _monsterId = monsterId;
            _stateGroup = stateGroup;
            _animationName = _actInfo.AnimationType;
            _mc = BattleDataManager.MonsterManager.Instance.GetMonsterFromSoldierID(monsterId);
            if (_mc != null)
                _duration = _mc.GetAnimationDuration(_animationName) + _actInfo.delayTime;
        }

        protected override IPlayFinishedState GenFinishedState()
        {
            var started = IsStarted();
            var errCode = !started
                ? PlayErrState.NotStarted
                : _mc == null
                    ? PlayErrState.Exception
                    : CurrentProgress() < _duration
                        ? PlayErrState.NotFinished
                        : PlayErrState.Success;
            return new SimplePlayFinishedState(errCode);
        }

        protected override void CustomDispose()
        {
            _actInfo = null;
            _skill = null;
            _vsAct = null;
            _mc = null;
            _stateGroup = null;
        }

        public override float Duaration()
        {
            return _duration;
        }

        protected override void CustomStart()
        {
            _mc.PlayAnimation(_animationName);
            if (_stateGroup != null)
            {
                if (_actInfo.initiator != ActionInitiator.Victim)
                {//受击扣血会多播一次，因此限制只要攻击方才播放
                    BattleStateHandler.HandleBattleState(_monsterId, _stateGroup.targetStates, BattleDataManager.DataMgr.IsInBattle);
                }
            }
            OnEnd += CheckMonsterState;
        }

        private void CheckMonsterState(IPlayFinishedState obj)
        {
            //TODO fish: BattleActionPlayer.OnActionEnd Line:946
            OnEnd -= CheckMonsterState;
        }
    }
}