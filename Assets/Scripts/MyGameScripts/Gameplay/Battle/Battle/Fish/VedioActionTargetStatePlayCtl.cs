using AppDto;

namespace Fish
{
    public class VedioActionTargetStatePlayCtl:BattlePlayCtlBasic
    {
        private VideoActionTargetState _state;
        private VideoTargetStateGroup _group;

        public VedioActionTargetStatePlayCtl(VideoActionTargetState videoActionTargetState,
            VideoTargetStateGroup targetStateGroup)
        {
            _state = videoActionTargetState;
            _group = targetStateGroup;
        }

        protected override IPlayFinishedState GenFinishedState()
        {
            throw new System.NotImplementedException();
        }

        public override float Duaration()
        {
            throw new System.NotImplementedException();
        }

        protected override IPlayFinishedState CustomCancel()
        {
            //throw new System.NotImplementedException();
            return base.CustomCancel();
        }

        protected override void CustomDispose()
        {
            _state = null;
        }

        protected override void CustomStart()
        {
            var mc = BattleDataManager.MonsterManager.Instance.GetMonsterFromSoldierID(_state.id);
            if (mc != null)
            {
                mc.ClearMessageEffect(true);
            }
            else
            {
                GameDebuger.TODO(@"if (bas is VideoCallSoldierState)
            {
                VideoCallSoldierState videoCallSoldierState = bas as VideoCallSoldierState;
                         BattleController.Instance.CallPet(videoCallSoldierState.soldier);
            }");
                return;
            }

            mc.UpdateHpEpCp(_state);
            if (_state.crit)
                mc.AddMessageEffect( MonsterController.ShowMessageEffect.CRITICAL );
            mc.dead = _state.dead;
            mc.leave = _state.leave;
            mc.PlayInjure();
        }
    }
}