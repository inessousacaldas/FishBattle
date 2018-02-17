using AppDto;

namespace Fish
{
    public class TweenMovePlayCtl : BattlePlayCtlBasic
    {
        public static TweenMovePlayCtl Create(MoveActionInfo moveActionInfo, Skill skill, VideoSkillAction vsAct,
            long initiator, long targetId)
        {
            return new TweenMovePlayCtl(moveActionInfo, skill, vsAct, initiator, targetId);
        }

        private readonly float _duration;
        private MonsterController _initiator;
        private MonsterController _target;
        private MoveActionInfo _actInfo;
        private Skill _skill;
        private VideoSkillAction _vsAct;

        private TweenMovePlayCtl(MoveActionInfo moveActionInfo, Skill skill, VideoSkillAction vsAct, long initiator,
            long targetId)
        {
            _actInfo = moveActionInfo;
            _skill = skill;
            _vsAct = vsAct;
            _duration = moveActionInfo.time;
            _initiator = BattleDataManager.MonsterManager.Instance.GetMonsterFromSoldierID(initiator);
            _target = BattleDataManager.MonsterManager.Instance.GetMonsterFromSoldierID(targetId);
        }

        protected override IPlayFinishedState GenFinishedState()
        {
            var started = IsStarted();
            var errCode = !started
                ? PlayErrState.NotStarted
                : _initiator == null
                    ? PlayErrState.Exception
                    : CurrentProgress() < _duration
                        ? PlayErrState.NotFinished
                        : PlayErrState.Success;
            return new SimplePlayFinishedState(errCode);
        }

        protected override void CustomDispose()
        {
            _initiator = null;
            _target = null;
            _actInfo = null;
            _skill = null;
            _vsAct = null;
        }

        public override float Duaration()
        {
            return _duration;
        }

        protected override void CustomStart()
        {
            //_initiator.DoMove(_target.transform.position, _duration);
            _initiator.SetSkillTarget(_target);
            _initiator.PlayMoveNode(_actInfo);
        }
    }

    public class TweenMoveBackPlayCtl : BattlePlayCtlBasic
    {
        public static TweenMoveBackPlayCtl Create(MoveBackActionInfo moveActionInfo, Skill skill,
            VideoSkillAction vsAct, long initiator, long targetId)
        {
            return new TweenMoveBackPlayCtl(moveActionInfo, skill, vsAct, initiator, targetId);
        }

        private readonly float _duration;
        private MonsterController _initiator;
        private MonsterController _target;
        private MoveBackActionInfo _actInfo;
        private Skill _skill;
        private VideoSkillAction _vsAct;

        private TweenMoveBackPlayCtl(MoveBackActionInfo moveActionInfo, Skill skill, VideoSkillAction vsAct,
            long initiator, long targetId)
        {
            _actInfo = moveActionInfo;
            _skill = skill;
            _vsAct = vsAct;
            _duration = moveActionInfo.time;
            _initiator = BattleDataManager.MonsterManager.Instance.GetMonsterFromSoldierID(initiator);
            _target = BattleDataManager.MonsterManager.Instance.GetMonsterFromSoldierID(targetId);
        }

        protected override IPlayFinishedState GenFinishedState()
        {
            var started = IsStarted();
            var errCode = !started
                ? PlayErrState.NotStarted
                : _initiator == null
                    ? PlayErrState.Exception
                    : CurrentProgress() < _duration
                        ? PlayErrState.NotFinished
                        : PlayErrState.Success;
            return new SimplePlayFinishedState(errCode);
        }

        protected override void CustomDispose()
        {
            _initiator = null;
            _target = null;
            _actInfo = null;
            _skill = null;
            _vsAct = null;
        }

        public override float Duaration()
        {
            return _duration;
        }

        protected override void CustomStart()
        {
            //_initiator.DoMove(_target.transform.position, _duration);
            _initiator.SetSkillTarget(_target);
            _initiator.PlayMoveBackNode(_actInfo);
        }
    }
}