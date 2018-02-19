using AppDto;

namespace Fish
{
    public class AnimatorPlayCtrl : BattlePlayCtlBasic
    {
        public static AnimatorPlayCtrl Create(NormalActionInfo normalActionInfo, Skill skill, VideoSkillAction vsAct,
            long targetId, VideoTargetStateGroup stateGroup)
        {
            return new AnimatorPlayCtrl(normalActionInfo, skill, vsAct, targetId,stateGroup);
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
            _mc = BattleDataManager.MonsterManager.Instance.GetMonsterFromSoldierID(monsterId);
            _animationName = _actInfo.AnimationType;
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
        }

        public override float Duaration()
        {
            return _duration;
        }

        protected override void CustomStart()
        {
            _mc.PlayAnimation(_animationName);
            if (_stateGroup != null)
                BattleStateHandler.HandleBattleState(_monsterId, _stateGroup.targetStates, BattleDataManager.DataMgr.IsInBattle);
            
            foreach (var eff in _actInfo.effects)
            {
                eff.Play(_mc, _actInfo, _vsAct, _stateGroup);
            }
        }
    }
}