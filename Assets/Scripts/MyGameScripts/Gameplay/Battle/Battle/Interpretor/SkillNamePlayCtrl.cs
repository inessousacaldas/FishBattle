using AppDto;

namespace Fish
{
    public class SkillNamePlayCtrl : BattlePlayCtlBasic
    {
        public static SkillNamePlayCtrl Create(
            Skill skill
            , long initiator)
        {
            var ctl = new SkillNamePlayCtrl(skill, initiator);
            if (ctl._initiator == null)
                return null;
            return ctl;
        }

        private readonly float _duration;
        private MonsterController _initiator;
        private Skill _skill;

        private SkillNamePlayCtrl(Skill skill, long initiator)
        {
            _skill = skill;
            //普通攻击不显示技能名
            _duration = skill == null || skill.type == (int)Skill.SkillEnum.None ? 0f : 0.5f;
            _initiator = BattleDataManager.MonsterManager.Instance.GetMonsterFromSoldierID(initiator);
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
            _skill = null;
        }

        public override float Duaration()
        {
            return _duration;
        }

        protected override void CustomStart()
        {
            //_initiator.DoMove(_target.transform.position, _duration);
            _initiator.PlaySkillName(_skill);
        }
    }
}