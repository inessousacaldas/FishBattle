using AppDto;

namespace Fish
{
    public class InjuredEffectPlayCtl : BattlePlayCtlBasic
    {
        private long _targetId;
        private VideoTargetStateGroup _stateGroup;

        private InjuredEffectPlayCtl(long victim, VideoTargetStateGroup stateGroup)
        {
            _targetId = victim;
            _stateGroup = stateGroup;
        }

        public static IBattlePlayCtl Create(long victim, VideoTargetStateGroup stateGroup)
        {
            return new InjuredEffectPlayCtl(victim,stateGroup);
        }

        protected override IPlayFinishedState GenFinishedState()
        {
            return null;
        }

        protected override IPlayFinishedState CustomCancel()
        {
            _stateGroup = null;
            return null;
        }

        protected override void CustomStart()
        {
            //var mc = BattleDataManager.MonsterManager.Instance.GetMonsterFromSoldierID(_targetId);
            BattleStateHandler.HandleBattleState(_targetId, _stateGroup.targetStates, BattleDataManager.DataMgr.IsInBattle);

        }

        public override float Duaration()
        {
            return LessThanOneFrame;
        }
    }
}