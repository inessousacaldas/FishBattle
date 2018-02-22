using System;

namespace Fish
{
    public class ImmediaBattleActionCtl:BattlePlayCtlBasic
    {
        public static ImmediaBattleActionCtl Create(Action immediaAction)
        {
            return new ImmediaBattleActionCtl(immediaAction);
        }

        private Action immediaAction;
        private Exception _excep;

        private ImmediaBattleActionCtl(Action immediaAction)
        {
            this.immediaAction = immediaAction;
        }

        protected override IPlayFinishedState GenFinishedState()
        {
            var started = IsStarted();
            if (!started) return new SinglePlayFinishedState(null, PlayErrState.NotStarted);
            
            if (_excep == null)
                return null;
            return new SinglePlayFinishedState(_excep,PlayErrState.Exception);

        }

        protected override IPlayFinishedState CustomCancel()
        {
            var result = GenFinishedState();
            _excep = null;
            return result;
        }

        protected override void CustomDispose()
        {
            immediaAction = null;
            _excep = null;
        }

        protected override void CustomStart()
        {
            try
            {
                immediaAction();
            }
            catch (Exception e)
            {
                _excep = e;
            }

            CallOnEnd(GenFinishedState());
        }

        public override float Duaration()
        {
            return LessThanOneFrame;
        }
    }
}