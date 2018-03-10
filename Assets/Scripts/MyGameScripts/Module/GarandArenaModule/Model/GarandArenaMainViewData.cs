// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 12/14/2017 2:45:59 PM
// **********************************************************************

using AppDto;
using System.Collections.Generic;

public interface IGarandArenaMainViewData
{
    int CupCount { get; set; }
    int MyRank { get; set; }
    int RemainTimes { get; set; }
    long RefreshCdAt { get; set; }
    IEnumerable<RankItemDto> RankList { get; }
    IEnumerable<OpponentInfoDto> RivalList { get; }

}

public sealed partial class GarandArenaMainViewDataMgr
{
    public sealed partial class GarandArenaMainViewData : IGarandArenaMainViewData
    {
        public void InitData()
        {
            _garandTeamFormationData.InitData();
        }

        public void Dispose()
        {

        }

        private TeamFormationDataMgr.TeamFormationData _garandTeamFormationData = new TeamFormationDataMgr.TeamFormationData();
        public TeamFormationDataMgr.TeamFormationData GarandTeamFormationData { get { return _garandTeamFormationData; } }

        private int _cupCount = 0;
        public int CupCount
        {
            get
            {
                return _cupCount;
            }
            set
            {
                _cupCount = value;
            }
        }

        private int _myRank = 0;
        public int MyRank
        {
            get
            {
                return _myRank;
            }
            set
            {
                _myRank = value;
            }
        }

        private int _remainTimes = 0;
        public int RemainTimes
        {
            get
            {
                return _remainTimes;
            }
            set
            {
                _remainTimes = value;
            }
        }

        public bool CanRefresh { get; set; }
        private long _refreshCdAt;
        public long RefreshCdAt
        {
            get
            {
                return _refreshCdAt;
            }
            set
            {
                _refreshCdAt = value;
            }
        }

        private List<OpponentInfoDto> _rivalList = new List<OpponentInfoDto>();
        public IEnumerable<OpponentInfoDto> RivalList
        {
            get
            {
                return _rivalList;
            }
        }

        private List<RankItemDto> _rankList = new List<RankItemDto>();
        public IEnumerable<RankItemDto> RankList
        {
            get
            {
                return _rankList;
            }
        }

        public void RefreshRivalList(List<OpponentInfoDto> list)
        {
            _rivalList.Clear();
            _rivalList = list;

            //1分钟冷却刷新
            SetRefreshTimer(SystemTimeManager.Instance.GetUTCTimeStamp());
        }

        public void UpdateData(ArenaInfoDto dto)
        {
            if (dto == null) return;
            _rivalList.Clear();
            _rankList.Clear();
            _rivalList = dto.opponents;
            if (dto.rankInfoDto.myData as RankArenaDto != null)
                _cupCount = (dto.rankInfoDto.myData as RankArenaDto).trophyCount;
            _remainTimes = dto.remainTimes;
            _refreshCdAt = dto.refreshCdAt;
            SetRefreshTimer(_refreshCdAt);

            if (dto.rankInfoDto == null) return;
            _rankList = dto.rankInfoDto.list;
            if (dto.rankInfoDto.myData == null) return;
            _myRank = dto.rankInfoDto.myRank;
        }

        private void SetRefreshTimer(long lastRefreshTime)
        {
            //刷新CD 60s
            long cd = SystemTimeManager.Instance.GetUTCTimeStamp() - lastRefreshTime;
            if (60 - cd / 1000 > 0)
            {
                CanRefresh = false;
                RefreshCdAt = lastRefreshTime;
                JSTimer.Instance.SetupCoolDown("RefreshTime", 60 - cd / 1000, null, delegate
                {
                    if (!UIModuleManager.Instance.IsModuleOpened(GarandArenaMainView.NAME))
                        return;

                    CanRefresh = true;
                });
            }
            else
            {
                CanRefresh = true;
            }
        }
    }
}
