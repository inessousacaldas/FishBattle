using UnityEngine;

namespace Fish
{
    public class TurnPlayCtrl : BattlePlayCtlBasic
    {
        private readonly Vector3 _turnOffset;
        private readonly MonsterController _mc;
        public TurnPlayCtrl(MonsterController mc, Vector3 offset)
        {
            _turnOffset = offset;
            _mc = _mc;
        }

        protected override IPlayFinishedState GenFinishedState()
        {
            var started = IsStarted();
            var errCode = !started
                ? PlayErrState.NotStarted
                : _mc == null
                ? PlayErrState.Exception: PlayErrState.Success;
            return new SimplePlayFinishedState(PlayErrState.Success);
        }

        public override float Duaration()
        {
            return 0f;
        }

        protected override void CustomStart()
        {
            _mc.Rotate(_turnOffset);
            
        }
    }
}