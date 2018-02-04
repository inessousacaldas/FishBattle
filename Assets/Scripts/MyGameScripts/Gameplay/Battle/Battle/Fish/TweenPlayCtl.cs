using System;
using UnityEngine;

namespace Fish
{
    public class TweenPlayCtl:BattlePlayCtlBasic
    {
        private float duaration;
        private Vector3 pos;
        protected override IPlayFinishedState GenFinishedState()
        {
            throw new NotImplementedException();
        }

        public override float Duaration()
        {
            return duaration;
        }

        protected override void CustomStart()
        {
            _mc.DoMove(pos, duaration);
        }
    }
}