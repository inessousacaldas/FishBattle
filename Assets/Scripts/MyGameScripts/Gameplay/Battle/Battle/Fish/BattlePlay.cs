using System;
using System.Collections.Generic;

namespace Fish
{
    //单个或组合的战斗动画
    public interface IBattlePlayCtl
    {
        void Play();
        //Pause,Resume,Reset,Reverse 等方法方便调试，暂不实现
        /*
        void Puase();
        void Resume();
        void Reset();
        void Reverse();
        */

        //取消执行并还原到初始状态，返回当前执行已知错误
        IPlayFinishedState Cancel();
        
        //释放内部所有引用，例如OnEnd事件关联的Delegates
        void Dispose();

        //动画结束后的事件，每个实现必须保证时间到了一定触发，除非被暂停，重置，或者释放
        event Action<IPlayFinishedState> OnEnd;

        //这个战斗动画的总时长，单位秒，后面所有时间单位都是秒
        float Duaration();

        //当前播放的进度
        float CurrentProgress();

        BattlePlayingState CurrentState { get; }
    }

    //实现类除了PlayErrState，还可以提供更多信息，例如提供Exception是什么，或者多个异常是什么，或者异常的时间点等
    public interface IPlayFinishedState
    {
        PlayErrState LastError();
    }

    public enum PlayErrState
    {
        NotStarted,
        NotFinished,
        Success,
        Timeout,
        ///出错但是完成了！
        Exception,
    }

    public enum BattlePlayingState
    {
        Pause,
        Started,
        Dispose,
    }

    public static class BattlePlayCtlExt
    {
        public static IBattlePlayCtl Chain(this IBattlePlayCtl first, IBattlePlayCtl second)
        {
            if (first == null) return second;
            if (second == null) return first;
            return new SeqCompositePlayCtl(new[] {first, second});
        }
    }

    /// 基类需要提供计时操作，底层框架对计时器进行优化。
    /// 实现不依赖具体动画，而是根据时间来计算，保证OnEnd一定能够被调用，并收集结束时的各种错误。
    public abstract class BattlePlayCtlBasic : IBattlePlayCtl
    {
        private float _elapseTime;

        private BattlePlayingState _playState;

        private JSTimer.TimerTask _timer;
        private readonly int _instaceId = ++_customInstanceId;
        private static int _customInstanceId;

        //invoke by timer
        void Update()
        {
            if (_playState != BattlePlayingState.Started) return;
            _elapseTime = _timer.ElapseTime;
            if (_elapseTime > Duaration())
            {
                var playFinishedState = GenFinishedState();
                //TODO debug log playFinishedState
                CallOnEnd(playFinishedState);
            }
        }

        protected void CallOnEnd(IPlayFinishedState playFinishedState)
        {
            GameUtil.SafeRun(OnEnd, playFinishedState);
            _playState = BattlePlayingState.Pause;
        }

        private void StartTimer()
        {
            DisposeTimer();
            _timer = JSTimer.Instance.SetupTimer(GetType().ToString()+_instaceId, Update);
        }

        private void DisposeTimer()
        {
            if (_timer == null) return;
            _timer.Cancel();
            _timer = null;
        }

        protected bool IsStarted()
        {
            return _playState == BattlePlayingState.Started;
        }

        public event Action<IPlayFinishedState> OnEnd;

        public BattlePlayingState CurrentState
        {
           get { return _playState; }
        }
        
        public float CurrentProgress()
        {
            return _elapseTime;
        }

        public IPlayFinishedState Cancel()
        {
            if (_playState == BattlePlayingState.Dispose) return null;
            _elapseTime = 0;
            _playState = BattlePlayingState.Pause;
            DisposeTimer();
            
            var playFinishedState = CustomCancel();
            return playFinishedState;
        }

        protected virtual IPlayFinishedState CustomCancel()
        {
            return null;
        }

        public void Dispose()
        {
            if (_playState == BattlePlayingState.Dispose) return;
            CustomDispose();
            OnEnd = null;
            DisposeTimer();
            _playState = BattlePlayingState.Dispose;
        }

        public void Play()
        {
            if (_playState != BattlePlayingState.Pause) return;

            _playState = BattlePlayingState.Started;
            StartTimer();
            CustomStart();
        }

        protected virtual void CustomDispose()
        {
        }

        protected virtual void CustomStart()
        {
        }

        protected abstract IPlayFinishedState GenFinishedState();

        public abstract float Duaration();
    }

    //保持一段时间状态不变的动画
    public class KeepStatePlayCtl : BattlePlayCtlBasic
    {
        private readonly float _duration;

        public KeepStatePlayCtl(float duaration)
        {
            _duration = duaration;
        }

        protected override IPlayFinishedState GenFinishedState()
        {
            var started = IsStarted();
            var errCode = !started
                ? PlayErrState.NotStarted
                : CurrentProgress() < _duration
                    ? PlayErrState.NotFinished
                    : PlayErrState.Success;
            return new SimplePlayFinishedState(errCode);
        }

        public override float Duaration()
        {
            return _duration;
        }
    }
    
    //顺序复合多个动画
    public class SeqCompositePlayCtl : BattlePlayCtlBasic
    {
        private IBattlePlayCtl[] _playCtlList;
        private readonly float _totalTime;
        private int _playIdx;
        private List<Tuple<int, IPlayFinishedState>> _abnormalList;

        public SeqCompositePlayCtl(IBattlePlayCtl[] lst)
        {
            _playCtlList = lst;
            for (var i = 0; i < _playCtlList.Length; i++)
            {
                var play = _playCtlList[i];
                _totalTime += play.Duaration();
            }
            _abnormalList = new List<Tuple<int, IPlayFinishedState>>();
        }
        
        //intensionally not check (playList != null),thow exception if null
        public SeqCompositePlayCtl(List<IBattlePlayCtl> playList):this(playList.ToArray())
        {
        }

        protected override IPlayFinishedState GenFinishedState()
        {
            var started = IsStarted();
            var errCode = !started
                ? PlayErrState.NotStarted
                : CurrentProgress() < _totalTime
                    ? PlayErrState.NotFinished
                    : !_abnormalList.IsNullOrEmpty()
                        ? PlayErrState.Exception
                        : _playIdx == _playCtlList.Length
                            ? PlayErrState.Success
                            : PlayErrState.Timeout;
            return new MultiPlayFinishedState(errCode, _abnormalList);
        }

        public override float Duaration()
        {
            return _totalTime;
        }

        protected override void CustomStart()
        {
            if (_playIdx >= _playCtlList.Length) return;
            var battlePlay = _playCtlList[_playIdx];
            battlePlay.OnEnd += NextPlay;
            battlePlay.Play();
        }

        private void NextPlay(IPlayFinishedState lastPlayState)
        {
            if (lastPlayState != null)
            {
                var lastErrCode = lastPlayState.LastError();
                if (lastErrCode != PlayErrState.Success)
                {
                    _abnormalList.Add(Tuple.Create(_playIdx, lastPlayState));
                }
            }
            _playCtlList[_playIdx].OnEnd -= NextPlay;
            _playIdx++;
            if (_playIdx == _playCtlList.Length)
            {
                var errCode = _abnormalList.IsNullOrEmpty()
                    ? PlayErrState.Success
                    : PlayErrState.Exception;
                CallOnEnd(new MultiPlayFinishedState(errCode, _abnormalList));
                return;
            }
            CustomStart();
        }

        protected override void CustomDispose()
        {
            if (_playCtlList != null)
            {
                for (var i = 0; i < _playCtlList.Length; i++)
                {
                    _playCtlList[i].Dispose();
                }
            }
            
            _playCtlList = null;
            _abnormalList = null;
        }

        protected override IPlayFinishedState CustomCancel()
        {
            if (_playCtlList != null)
            {
                for (var i = 0; i < _playCtlList.Length; i++)
                {
                    _playCtlList[i].Cancel();
                }
            }
            var currentAb = _abnormalList;
            _abnormalList = null;
            _playIdx = 0;
            return currentAb.IsNullOrEmpty() ? null : new MultiPlayFinishedState(PlayErrState.Exception, _abnormalList);
        }
    }

    //并行复合多个动画
    public class ParallCompositePlayCtl : BattlePlayCtlBasic
    {
        private IBattlePlayCtl[] _playCtlList;
        private readonly float _totalTime;
        private List<Tuple<int, IPlayFinishedState>> _abnormalList;
        private PlayEnd _cb;

        public ParallCompositePlayCtl(IBattlePlayCtl[] playCtlList)
        {
            _playCtlList = playCtlList;
            for (var i = 0; i < _playCtlList.Length; i++)
            {
                var totalTime = _playCtlList[i].Duaration();
                if (_totalTime < totalTime)
                {
                    _totalTime = totalTime;
                }
            }
            _abnormalList = new List<Tuple<int, IPlayFinishedState>>();
        }
        
        public ParallCompositePlayCtl(List<IBattlePlayCtl> playList):this(playList.ToArray())
        {
        }

        protected override IPlayFinishedState GenFinishedState()
        {
            var started = IsStarted();
            var errCode = !started
                ? PlayErrState.NotStarted
                : CurrentProgress() < _totalTime
                    ? PlayErrState.NotFinished
                    : _abnormalList != null && _abnormalList.Count > 0
                        ? PlayErrState.Exception
                        : _cb != null && !_cb.IsAllFinish()
                            ? PlayErrState.Timeout
                            : PlayErrState.Success;
            return new MultiPlayFinishedState(errCode, _abnormalList);
        }

        public override float Duaration()
        {
            return _totalTime;
        }
        
        protected override IPlayFinishedState CustomCancel()
        {
            if (_playCtlList != null)
            {
                for (var i = 0; i < _playCtlList.Length; i++)
                {
                    _playCtlList[i].Cancel();
                }
            }
            var currentAb = _abnormalList;
            _abnormalList = null;
            _cb = null;
            return currentAb.IsNullOrEmpty() ? null : new MultiPlayFinishedState(PlayErrState.Exception, _abnormalList);
        }

        protected override void CustomDispose()
        {
            if (_playCtlList != null)
            {
                for (var i = 0; i < _playCtlList.Length; i++)
                {
                    _playCtlList[i].Dispose();
                }
            }
            _playCtlList = null;
            _abnormalList = null;
            _cb = null;
        }

        protected override void CustomStart()
        {
            _cb = new PlayEnd(_playCtlList.Length, this);
            for (var i = 0; i < _playCtlList.Length; i++)
            {
                var play = _playCtlList[i];
                play.OnEnd += _cb.EndOn(i);
                play.Play();
            }
        }

        private void SomePlayEnd(int index, IPlayFinishedState lastPlayState)
        {
            if (lastPlayState != null)
            {
                var lastErrCode = lastPlayState.LastError();
                if (lastErrCode != PlayErrState.Success)
                {
                    _abnormalList.Add(Tuple.Create(index, lastPlayState));
                }
            }
            _playCtlList[index].OnEnd -= _cb.EndOn(index);
            _cb.Remove(index);
            if (_cb.IsAllFinish())
            {
                var errCode = _abnormalList != null && _abnormalList.Count > 0
                    ? PlayErrState.Exception
                    : PlayErrState.Success;
                CallOnEnd(new MultiPlayFinishedState(errCode, _abnormalList));
                _cb = null;
            }
        }

        private class PlayEnd
        {
            private readonly List<Action<IPlayFinishedState>> _actLst;

            public PlayEnd(int count, ParallCompositePlayCtl ctx)
            {
                _actLst = new List<Action<IPlayFinishedState>>(count);
                for (var i = 0; i < _actLst.Count; i++)
                {
                    var index = i;
                    _actLst[i] = state => { ctx.SomePlayEnd(index, state); };
                }
            }

            public Action<IPlayFinishedState> EndOn(int index)
            {
                return _actLst[index];
            }

            public void Remove(int index)
            {
                _actLst[index] = null;
            }

            public bool IsAllFinish()
            {
                return _actLst.Find<Action<IPlayFinishedState>>(act=>act != null) != null;
            }
        }
    }

    //分支动画
    public class BranchCompositePlayCtl : BattlePlayCtlBasic
    {
        private IBattlePlayCtl _mainThread;
        private IBattlePlayCtl _branchThread;
        private IPlayFinishedState _branchError;
        private IPlayFinishedState _mainThreadErr;
        private bool _isFinished;

        public BranchCompositePlayCtl(IBattlePlayCtl mainThread, IBattlePlayCtl branchThread)
        {
            _mainThread = mainThread;
            _branchThread = branchThread;
        }

        protected override IPlayFinishedState GenFinishedState()
        {
            var started = IsStarted();
            var errCode = !started
                ? PlayErrState.NotStarted
                : CurrentProgress() < Duaration()
                    ? PlayErrState.NotFinished
                    : _branchError != null || _mainThreadErr != null
                        ? PlayErrState.Exception
                        : !_isFinished
                            ? PlayErrState.Timeout
                            : PlayErrState.Success;

            var abnormalList = CollectAbnormalList();

            return new MultiPlayFinishedState(errCode, abnormalList);
        }

        private Tuple<int, IPlayFinishedState>[] CollectAbnormalList()
        {
            if (_mainThreadErr == null && _branchError == null) return null;
            if (_mainThreadErr != null && _branchError != null)
                return new[] {Tuple.Create(0, _mainThreadErr), Tuple.Create(1, _branchError)};
            return _mainThreadErr != null
                ? new[] {Tuple.Create(0, _mainThreadErr)}
                : new[] {Tuple.Create(1, _branchError)};
        }

        public override float Duaration()
        {
            return _mainThread.Duaration();
        }

        protected override IPlayFinishedState CustomCancel()
        {
            _mainThread.Cancel();
            _branchThread.Cancel();
            _isFinished = false;
            var ablst = CollectAbnormalList();
            return ablst == null ? null : new MultiPlayFinishedState(PlayErrState.Exception, ablst);
        }

        protected override void CustomDispose()
        {
            _mainThread.Dispose();
            _branchThread.Dispose();
            _mainThread = null;
            _branchThread = null;
            _mainThreadErr = null;
            _branchError = null;
        }

        protected override void CustomStart()
        {
            _mainThread.Play();
            _mainThread.OnEnd += MainEnd;
            _branchThread.Play();
            _branchThread.OnEnd += BranchEnd;
        }

        private void BranchEnd(IPlayFinishedState obj)
        {
            _branchThread.OnEnd -= BranchEnd;
            if (obj == null) return;
            if (obj.LastError() != PlayErrState.Success)
            {
                _branchError = obj;
            }
        }

        private void MainEnd(IPlayFinishedState obj)
        {
            _mainThread.OnEnd -= MainEnd;
            var mainErr = false;
            if (obj != null)
            {
                if (obj.LastError() != PlayErrState.Success)
                {
                    mainErr = true;
                    _mainThreadErr = obj;
                }
            }
            _isFinished = true;
            if (_branchError == null)
                CallOnEnd(obj);
            else
            {
                var playErrState = obj!=null ? obj.LastError() : PlayErrState.Success;
                var abnormalList = mainErr ? new []{Tuple.Create(0,obj),Tuple.Create(1,_branchError)} : new []{Tuple.Create(1,_branchError)};
                CallOnEnd(new MultiPlayFinishedState(playErrState, abnormalList));
            }
        }
    }
}
/*
{
      "$type": "SkillConfigInfo, Assembly-CSharp",
      "id": 1329,
      "name": "普攻",
      "attackerActions": [
        {
          "$type": "MoveActionInfo, Assembly-CSharp",
		  var totalDis = Vector3.Distance(_mTrans.position, position);
		  var time = totalDis /(catchMode ? ModelHelper.DefaultBattleCatchSpeed * (turn ? 2f : 1f) : ModelHelper.DefaultBattleModelSpeed);
          "distance": 1.8,
          "type": "move",
          "name": "forward",
          "effects": []
        },
        {
          "$type": "NormalActionInfo, Assembly-CSharp",
		  anim.GetClipLength(action.ToString())//actack animation
		  should config Dao Guang
		  PlayDaoGuangEffect(_mc, tActionName);
          "type": "normal",
          "name": "attack",
          "effects": [
            {
              "$type": "TakeDamageEffectInfo, Assembly-CSharp",
			  PlayInjureHandle(ids[i], i, mAttacker);
			  no timing
              "type": "TakeDamage"
            },
            {
              "$type": "NormalEffectInfo, Assembly-CSharp",
            var normalEffectInfo = (NormalEffectInfo)node;
            if (_isAttack == false)
            {
                bool hasDodge = HasVideoDodgeTargetState(_stateGroup.targetStates);
                if (hasDodge && normalEffectInfo.hitEff)
                    return;
            }
            PlayNormalEffect(normalEffectInfo);
			
			PlaySpecialEffect(node, skillName, _mc, mc, clientSkillScale);
			
			default time of effect is 5
              "name": "skill_eff_1329_att",
              "mount": "Mount_Shadow",
              "faceToTarget": true,
              "type": "Normal"
            }
          ]
        },
        {
          "$type": "MoveBackActionInfo, Assembly-CSharp",
          var totalDis = Vector3.Distance(_mTrans.position, position);
          time = totalDis / (catchMode ? ModelHelper.DefaultBattleCatchSpeed : ModelHelper.DefaultBattleModelSpeed);

          "type": "moveBack",
          "name": "forward",
          "effects": []
        }
      ],
      "injurerActions": [
        {
          "$type": "NormalActionInfo, Assembly-CSharp",
			  float delayTime = node.delayTime;
			  if (actionName == ModelHelper.AnimType.hit)
              {
              //这里要特殊处理，因为防御动作结束后不需要播放hit， 需要直接回到battle
              delayTime += 0.3f;
              }
          "startTime": 0.8,
          "delayTime": 0.166666,
          "type": "normal",
          "effects": [
            {
              "$type": "ShowInjureEffectInfo, Assembly-CSharp",
			  ShowInjureEffect(ShowInjureEffectInfo node)
			  no timing
              "type": "ShowInjure",
              "playTime": 0.8
            },
            {
              "$type": "NormalEffectInfo, Assembly-CSharp",
              "name": "skill_eff_1329_hit",
              "mount": "Mount_Hit",
              "hitEff": true,
              "type": "Normal",
              "playTime": 0.8
            }
          ]
        }
      ]
    }


 */