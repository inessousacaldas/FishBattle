namespace Fish
{
    public class AnimatorPlayCtrl : BattlePlayCtlBasic
    {
        private readonly float duaration;
        private readonly MonsterController mc;
        private readonly ModelHelper.AnimType animType;
        // 时间由配置表读入
        public AnimatorPlayCtrl(MonsterController mc, float duaration, ModelHelper.AnimType animType)
        {
            this.mc = mc;
            this.duaration = duaration;
            this.animType = animType;
        }

        protected override IPlayFinishedState GenFinishedState()
        {
            var started = IsStarted();
            var errCode = !started
                ? PlayErrState.NotStarted
                : mc == null
                    ? PlayErrState.Exception
                    : CurrentProgress() < duaration
                        ? PlayErrState.NotFinished
                        : PlayErrState.Success;
            return new SimplePlayFinishedState(errCode);
        }

        public override float Duaration()
        {
            return duaration;
        }

        protected override void CustomStart()
        {
            mc.PlayAnimation(animType);
        }
    }
}