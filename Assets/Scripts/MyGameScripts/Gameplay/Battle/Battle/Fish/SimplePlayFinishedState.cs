using System;
using System.Collections.Generic;

namespace Fish
{
    public class SimplePlayFinishedState : IPlayFinishedState
    {
        private readonly PlayErrState _st;

        public SimplePlayFinishedState(PlayErrState st)
        {
            _st = st;
        }

        public PlayErrState LastError()
        {
            return _st;
        }
    }
    
    //多个动画复合的完成状态
    public class MultiPlayFinishedState : SimplePlayFinishedState
    {
        private readonly ICollection<Tuple<int, IPlayFinishedState>> _errLst;

        public MultiPlayFinishedState(PlayErrState st, ICollection<Tuple<int, IPlayFinishedState>> errLst) : base(st)
        {
            _errLst = errLst;
        }

        public ICollection<Tuple<int, IPlayFinishedState>> GetErrorList()
        {
            return _errLst;
        }
    }

    public class SinglePlayFinishedState : SimplePlayFinishedState
    {
        private Exception _exec;

        public Exception Exec
        {
            get { return _exec; }
        }

        public SinglePlayFinishedState(Exception exec, PlayErrState st) : base(st)
        {
            _exec = exec;
        }
    }
}