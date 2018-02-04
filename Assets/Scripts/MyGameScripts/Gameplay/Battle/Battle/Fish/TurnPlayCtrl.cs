namespace Fish
{
    public class TurnPlayCtrl : BattlePlayCtlBasic
    {
//    _mTrans.eulerAngles = new Vector3(_mTrans.eulerAngles.x, _mTrans.eulerAngles.y + 180,
//        _mTrans.eulerAngles.z);
    
        protected override IPlayFinishedState GenFinishedState()
        {
            var started = IsStarted();
            var errCode = !started
                ? PlayErrState.NotStarted
                : PlayErrState.Success;
            return new SimplePlayFinishedState(PlayErrState.Success);
        }

        public override float Duaration()
        {
            return 0f;
        }
    }
}