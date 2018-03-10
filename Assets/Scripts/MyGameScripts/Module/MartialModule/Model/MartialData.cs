// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 1/17/2018 5:10:21 PM
// **********************************************************************

using System.Collections.Generic;
using AppDto;

public interface IMartialData
{
    KungfuInfoDto KungFuInfo { get; }
    KungfuActivityInfo ActivityInfo { get; }
    int EndAtTime { get; }

    RankInfoDto WinRankInfo { get; }
    RankInfoDto LoserRankInfo { get; }
    MartialDataMgr.MartialData.MartialTab CurTab { get; }
}

public sealed partial class MartialDataMgr
{
    public sealed partial class MartialData:IMartialData
    {
        private int _sceneId;

        public int SceneId
        {
            get { return _sceneId; }
            set { _sceneId = value; }
        }

        public enum MartialTab
        {
            Win = 0,
            loser = 1
        }

        private MartialTab _curTab;

        public MartialTab CurTab
        {
            get { return _curTab; }
            set { _curTab = value; }
        }

        private int _endAtTime;
        public int EndAtTime
        {
            get { return _endAtTime; }
            set { _endAtTime = value; }
        }

        private KungfuActivityInfo _activityInfo;

        public KungfuActivityInfo ActivityInfo
        {
            get { return _activityInfo; }
            set { _activityInfo = value; }
        }

        private KungfuInfoDto _kungfuInfo;

        public KungfuInfoDto KungFuInfo
        {
            get { return _kungfuInfo;}
            set { _kungfuInfo = value; }
        }
        
        //胜者组
        private RankInfoDto _winRankInfo;
        public RankInfoDto WinRankInfo
        {
            get { return _winRankInfo; }
            set { _winRankInfo = value; }
        }

        //败者组
        private RankInfoDto _loserRankInfo;
        public RankInfoDto LoserRankInfo
        {
            get { return _loserRankInfo; }
            set { _loserRankInfo = value; }
        }

        public void UpdateActiveState(int state)
        {
            if(_kungfuInfo != null)
                _kungfuInfo.matchState = state;
        }

        public void InitData()
        {
        }

        public void Dispose()
        {

        }

        public void UpdateEndTime()
        {
            JSTimer.Instance.SetupCoolDown("MartialDataCDTime", (float) EndAtTime, e =>
            {
                EndAtTime -= 1;
            }, () =>
            {
                EndAtTime = 0;
                JSTimer.Instance.CancelCd("MartialDataCDTime");
            }, 1f);
        }
    }
}
