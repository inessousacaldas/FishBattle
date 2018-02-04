// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 1/16/2018 12:11:22 PM
// **********************************************************************

using System.Collections.Generic;
using AppDto;

public interface IRankData
{
    int CurRankId { get; }
    Dictionary<int, RankInfoDto> GetAllRankData { get; }
    RankInfoDto GetRankInfoByRank(int rankId);
}

public sealed partial class RankDataMgr
{
    public sealed partial class RankData:IRankData
    {
        #region Ranking

        private int _curRankId; //打开界面时选中的分页
        private Dictionary<int, RankInfoDto> _allRankData = new Dictionary<int, RankInfoDto>();
        public Dictionary<int, RankInfoDto> GetAllRankData { get { return _allRankData; } } 
        public int CurRankId
        {
            get { return _curRankId; }
            set { _curRankId = value; }
        }

        #endregion
        public void InitData()
        {
        }

        public void Dispose()
        {
            _allRankData.Clear();
        }

        public void UpdateRankData(RankInfoDto dto, int menuId)
        {
            var idx = _allRankData.Keys.FindElementIdx(d => d == menuId);
            if (idx < 0)
                _allRankData.Add(menuId, dto);
            else
                _allRankData[menuId] = dto;
        }

        public RankInfoDto GetRankInfoByRank(int rankId)
        {
            var idx = _allRankData.Keys.FindElementIdx(d => d == rankId);
            return idx < 0 ? null : _allRankData.Values.TryGetValue(idx);
        }
    }
}
