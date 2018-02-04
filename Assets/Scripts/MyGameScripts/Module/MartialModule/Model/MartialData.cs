// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 1/17/2018 5:10:21 PM
// **********************************************************************

using System.Collections.Generic;
using AppDto;

public interface IMartialData
{
    IEnumerable<RankItemDto> PlayerList { get; }
    RankInfoDto RankInfo { get; }
    KungfuInfoDto KungFuInfo { get; }
    KungfuActivityInfo ActivityInfo { get; }
    int EndAtTime { get; }
}

public sealed partial class MartialDataMgr
{
    public sealed partial class MartialData:IMartialData
    {
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

        private RankInfoDto _rankInfo;
        public RankInfoDto RankInfo
        {
            get { return _rankInfo; }
            set { _rankInfo = value; }
        }

        private KungfuInfoDto _kungfuInfo;

        public KungfuInfoDto KungFuInfo
        {
            get { return _kungfuInfo;}
            set { _kungfuInfo = value; }
        }

        private IEnumerable<RankItemDto> _playerList = new List<RankItemDto>();  
        public IEnumerable<RankItemDto> PlayerList { get { return _playerList; } }

        public void UpdatePlayerList(List<RankItemDto> list)
        {
            _playerList = list;
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
