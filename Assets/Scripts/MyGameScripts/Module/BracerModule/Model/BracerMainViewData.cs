// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 11/9/2017 3:18:17 PM
// **********************************************************************

using System.Collections.Generic;
using AppDto;

public interface IBracerMainViewData
{
    int BracerRank { get; }
    long BracerExp { get; }
    IEnumerable<BracerMissionDto> MissionDataList { get; }
}

public sealed partial class BracerMainViewDataMgr
{
    public sealed partial class BracerMainViewData:IBracerMainViewData
    {
        public void InitData()
        {
        }

        public void Dispose()
        {

        }


        private List<BracerMissionDto> _missionDataList = new List<BracerMissionDto>();
        public IEnumerable<BracerMissionDto> MissionDataList
        {
            get { return _missionDataList; }
        }

        private int _bracerRank;
        public int BracerRank { get { return _bracerRank; } }
        private long _bracerExp;
        public long BracerExp { get { return _bracerExp; } }
        public void UpdateData(BracerEnterDto dto)
        {
            _bracerRank = dto.bracerRank;
            //同步玩家信息
            ModelManager.Player.SetBracerGrade(_bracerRank);
            _bracerExp = dto.bracerExp;
            if (dto.missionList != null)
                _missionDataList = dto.missionList;
        }

        public void UpdateBracerRankAndMissionList(BracerMissionListDto dto)
        {
            if (dto.missionList != null)
                _missionDataList = dto.missionList;
            _bracerRank += 1;
            _bracerExp = 0;
            //同步玩家信息
            ModelManager.Player.SetBracerGrade(_bracerRank);
        }
    }
}
