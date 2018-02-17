// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  BattleInstController.cs
// Author   : fish
// Created  : 2018/2/16
// Purpose  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using MyGameScripts.Gameplay.Battle.Demo.Helper;
using Fish;

public partial class BattleDataManager
{

    public class BattleInstController
    {
        //用battleid==0来过滤掉不和谐的回合数据，2017-03-13 09:26:23
        public const int SYMBOL_INVALID_VIDEO_ROUND = 0;

        private VideoRoundInterpreter _interpreter;
        private VideoAction _curVideoAction;

        // 一个videoround的播放状态
        private bool _playing;

        // 当前全部videoround是否播放完毕
        private bool _videoPlaying;
        public bool VideoPlaying
        {
            get { return _videoPlaying; }
        }

        public int CurRoundIdx
        {
            get { return _playRound == null ? 0 : _playRound.round; }
        }

        // 战斗中只通过notify更新数据，与video中的数据无关，重登录后直接播放下发的最新videoround 
        // 将来要移到battlemodelData中，收回写权限, 明确只由notify写入   －－fish
        public List<VideoRound> _videoRounds = new List<VideoRound>();
        public IEnumerable<VideoRound> VideoRounds
        {
            get { return _videoRounds; }
        }

        public int VideoRoundsCnt
        {
            get { return _videoRounds == null ? 0 : _videoRounds.Count; }
        }
        public VideoRound _playRound { get; private set; }

        private SkillPlayTimeHelper _skillPlayTimeHelper;
        //  private MonsterShoutHelper _monsterShoutHelper;

        // Use this for initialization
        public void Setup(Video gv)
        {

            if (gv is VideoRecord)
            {
                var record = (VideoRecord)gv;
                _videoRounds = record.rounds != null ? record.rounds.videoRounds : new List<VideoRound>(10);
            }
            else
            {
                _videoRounds = new List<VideoRound>(10);
            }

            _skillPlayTimeHelper = new SkillPlayTimeHelper();
            GameDebuger.TODO(@"_monsterShoutHelper = new MonsterShoutHelper();");

            _playing = false;
            _videoPlaying = false;
            _startCheckTime = 0;
            _playRound = null;

        }

        //    todo fish
        public void Dispose()
        {
            _videoRounds.Clear();
            _videoRounds = null;
            _playRound = null;
            _interpreter = null;
        }

        public static void DisposeOnExit()
        {
            if (mInstance != null)
                mInstance.Dispose();

            mInstance = null;
        }

        // Video 只读，写权限只保留在BattleManager   －－fish 2017.7.21
        public void PlayGameVideo()
        {
            CheckNextRound();
        }

        /**
         *是否自动播放模式 
         * @return
         * 
         */

        public bool IsAutoPlayMode()
        {
            return false;
        }

        // 魅怪在第一回合开始时，需要根据玩家队伍中有没有普陀、方寸、盘丝、化生、蓬莱、地府 门派进行喊话：
        //   有的情况：有方寸、盘丝、金山、普陀、蓬莱和地府的弟子在，真是不爽，还是跑路算了！
        //   无的情况：方寸、盘丝、金山、普陀、蓬莱和地府的弟子不在眼前，真是轻松愉快啊！
        private void CheckMeiMonster()
        {
            var list = MonsterManager.Instance.GetMonsterList(BattlePosition.MonsterSide.Enemy,
                                               false);

            var hasAntiMei = HasAntiMeiFaction();

            list.ForEach(mc =>
            {
                GameDebuger.TODO(@"if (mc.IsMonster() && mc.videoSoldier.monster.mei)
            {
                string shoutStr = '方寸、盘丝、金山、普陀、蓬莱和地府的弟子不在眼前，真是轻松愉快啊！';

                if (hasAntiMei)
                {
                    shoutStr = '有方寸、盘丝、金山、普陀、蓬莱和地府的弟子在，真是不爽，还是跑路算了！';
                }
                mc.Shout(shoutStr);
            }");
            });
        }

        private bool HasAntiMeiFaction()
        {
            var list = MonsterManager.Instance.GetMonsterList();
            var tFactionType = Faction.FactionType.Unknown;
            list.ForEach(mc =>
            {
                tFactionType = (Faction.FactionType)mc.videoSoldier.factionId;
                GameDebuger.TODO(@"if (tFactionType == Faction.FactionType.FangCun
                || tFactionType == Faction.FactionType.PanSi
                || tFactionType == Faction.FactionType.HuaSheng
                || tFactionType == Faction.FactionType.PuTuo
                || tFactionType == Faction.FactionType.PengLai
                || tFactionType == Faction.FactionType.Difu)
            {
                return true;
            }");
            });
            return false;
        }

        private long _startCheckTime = 0;

        private void PlayGameRound(VideoRound gameRound)
        {
            if (_playing)
                return;
            _playing = true; 
            _startCheckTime = DateTime.Now.Ticks;

            DataMgr.UpdateMonsterAttr(gameRound);
            GameDebuger.TODO(@"_monsterShoutHelper.UpdateMonsterShoutList(gameRound.shoutStates);");

            GameDebuger.TODO(@"if (gameRound.count == 1)
        {
            CheckMeiMonster();
        }");

            //处理回合准备Action
            GameDebuger.TODO(@"if (gameRound.readyAction != null && gameRound.readyAction.targetStateGroups.Count > 0)
        {
            _actionList.Add(gameRound.readyAction);
        }");
            //处理当前回合过程所有的动作
            _interpreter.InterpreteVideoRound(gameRound);
            LogBattleInfo(gameRound.debugInfo);
            _playing = true;
        }

        private void LogBattleInfo(DebugVideoRound debugInfo)
        {
            if (debugInfo == null || !GameDebuger.debugIsOn) return;
            GameDebuger.LogBattleInfo("回合:" + debugInfo.round);
            GameDebuger.LogBattleInfo("准备信息:");
            for (int i = 0, len = debugInfo.readyInfo.Count; i < len; i++)
            {
                var info = debugInfo.readyInfo[i];
                GameDebuger.LogBattleInfo(info);
            }
            GameDebuger.LogBattleInfo("过程信息:");
            for (int i = 0, len = debugInfo.progressInfo.Count; i < len; i++)
            {
                var info = debugInfo.progressInfo[i];
                GameDebuger.LogBattleInfo(info);
            }
        }

        /// <summary>
        /// 触发怪物喊话
        /// </summary>
        /// <param name="monsterId">怪物ID</param>
        /// <param name="shountType">喊话类型，参照ShoutConfig</param>
        //  public void TriggerMonsterShount(long monsterId, int shoutType, bool delayShout = false)
        //  {
        //      if (_monsterShoutHelper != null)
        //      {
        //          _monsterShoutHelper.TriggerMonsterShount(monsterId, shoutType, delayShout);
        //      }
        //  }

        //check can start next round when gamestate is ready
        public void CheckNextRound()
        {
            if (_playing)
            {
                return;
            }

            if (DEBUG)
            {
                if (_startCheckTime > 0)
                {
                    var passTime = DateTime.Now.Ticks - _startCheckTime;
                    var elapsedSpan = new TimeSpan(passTime);

                    var playTime = _skillPlayTimeHelper.GetVideoRoundPlayTime(_playRound);

                    GameLog.Log_Battle(string.Format("回合{0} 预估播放时长={1}S 真实播放时长={2}S", _playRound.round, playTime, elapsedSpan.TotalSeconds));
                    _startCheckTime = 0;
                }
            }

            if (_playRound != null && _playRound.winId == ModelManager.IPlayer.GetPlayerId())
                GameLog.Log_Battle_RESP("round" + _playRound.round + "    _playRound.winId  " + _playRound.winId);

            PlayNextRound(ref _videoPlaying);

            FireData();
        }

        private void PlayNextRound(ref bool videoPlaying)
        {
            GameLog.Log_Battle(string.Format("PlayNextRound ;_videoRounds.Count {0} CurRoundIdx {1}", _videoRounds.Count, CurRoundIdx), "orange");

            var _videoRound = _videoRounds.Find(v => v.round > CurRoundIdx);

            videoPlaying = _videoRound == null;

            if (_videoRound == null)
            {

                GameLog.Log_Battle("PlayNextRound :_videoRound = null", "orange");
                videoPlaying = false;
                return;
            }

            _playRound = _videoRound;
            videoPlaying = true;
            PlayGameRound(_videoRound);
        }

        //因中了封印等类型的DEBUFF而取消回合行动
        public void RemoveRound(long pActionPlayerUID)
        {
            if (null == _videoRounds || _videoRounds.Count <= 0)
                return;
            VideoRound tVideoRound = null;
            for (int tCounter = 0; tCounter < _videoRounds.Count; tCounter++)
            {
                tVideoRound = _videoRounds[tCounter];
                if (tVideoRound.IsBattleValid() && tVideoRound.id == pActionPlayerUID)
                {
                    tVideoRound.battleId = SYMBOL_INVALID_VIDEO_ROUND;
                    return;
                }
            }
        }

        public VideoAction GetCurVideoAction()
        {
            return _curVideoAction;
        }

        public void FinishInst()
        {
        }

        public void AddVideoRound(VideoRound videoRound)
        {
            _videoRounds.ReplaceOrAdd(v => v.round == videoRound.round && v.battleId == BattleDataManager.DataMgr.BattleDemo.GameVideo.id, videoRound);
            _videoRounds.Sort(BattleDtoExt.RoundAssendSorter);
            GameEventCenter.SendEvent(GameEvent.BATTLE_FIGHT_QUEUE_UPDATE, videoRound.id);
            //        BattleNetworkManager.DataMgr.HandlerActionQueueAddNotifyDto(videoRound.id, videoRound.name, ModelManager.BattleDemo.GetLastActionPlayedTime(), (int)_skillPlayTimeHelper.GetVideoRoundPlayTime(videoRound));

            //      if (videoRound.over)
            //      {
            //          _battleController._winId = videoRound.winId;
            //          _battleController.IsGameOver = true;
            //
            //          //TODO 这里是判断服务器下发的回合比正在播放的回合要快
            //          //int checkCount = _playing?_videoRounds.Count:(_videoRounds.Count-1);
            //          //TODO 临时改成快2回合才结束
            //          int checkCount = _playing?_videoRounds.Count:(_videoRounds.Count-2);
            //
            //          if (_playRoundIndex < checkCount)
            //          {
            //              ShowBattleResult();
            //          }
            //      }
        }

        public int GetVideoRoundIndexByPlayerUID(long pPlayerUID)
        {
            if (_videoRounds.IsNullOrEmpty())
                return -1;
            VideoRound tVideoRound = null;
            var tIndex = 0;
            for (var tCounter = 0; tCounter < _videoRounds.Count; tCounter++)
            {
                tVideoRound = _videoRounds[tCounter];
                if (!tVideoRound.IsBattleValid())
                    continue;
                if (tVideoRound.id == pPlayerUID)
                    return tIndex;
                tIndex++;
            }
            return -1;
        }

        //马上出战斗结果
        public void ShowBattleResult()
        {
            if (_playing)
            {
                var lastRound = _videoRounds[_videoRounds.Count - 1];
                if (CurRoundIdx < lastRound.round)
                {
                    var gameRound = lastRound;
                    gameRound.over = true;
                    GameDebuger.TODO(@"gameRound.readyAction = null;");
                    GameDebuger.TODO(@"gameRound.endAction = null;");
                    gameRound.skillActions.Clear();

                    CheckNextRound();
                }
            }
            else
            {
                DataMgr.CheckGameState();
            }
        }

        public bool IsInActionQueue(long pPlayerUID)
        {
            var tCurrentIndex = CurRoundIdx;
            var tVideoRoundList = _videoRounds;

            if (null == tVideoRoundList || _videoRounds[_videoRounds.Count - 1].round < CurRoundIdx)
                return false;
            VideoRound tVideoRound;

            for (var tCounter = tCurrentIndex; tCounter < _videoRounds.Count; tCounter++)
            {
                tVideoRound = tVideoRoundList[tCounter];
                if (!tVideoRound.IsBattleValid())
                    continue;
                if (tVideoRound.id == pPlayerUID)
                    return true;
            }
            return false;
        }

        public void Destroy()
        {

        }

        #region 单例

        private static BattleInstController mInstance;

        public static BattleInstController Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new BattleInstController();
                    mInstance.Init();
                }


                return mInstance;
            }
        }

        private void Init()
        {
            _interpreter = new VideoRoundInterpreter();
        }

        #endregion
        public long PreFinishedActionPlayerUID()
        {
            return _playRound == null ? 0L : _playRound.skillActions[0].actionSoldierId;
        }

        private void RemoveCurRound()
        {
            if (_playRound != null)
                RemoveRound(_playRound.id);
        }

        public void ClearUnPlayVideo()
        {
            _videoRounds.RemoveItems(item => item.round > CurRoundIdx);
        }

        /// <summary>
        /// 触发怪物喊话
        /// </summary>
        /// <param name="monsterId">怪物ID</param>
        /// <param name="shountType">喊话类型，参照ShoutConfig</param>
        public void TriggerMonsterShount(long monsterId, int shoutType, bool delayShout = false)
        {
            //          if (_monsterShoutHelper != null)
            //          {
            //              _monsterShoutHelper.TriggerMonsterShount(monsterId, shoutType, delayShout);
            //          }
        }
    }
}


