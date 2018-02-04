// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 10/13/2017 6:07:28 PM
// **********************************************************************

using AppDto;
using System.Collections.Generic;

public interface ISearchFriendData
{
    bool CanSearch { get; set; }

    long ChangeCDTime { get; set; }

    List<FriendInfoDto> SearchItemList { get; }

    bool IsChange { get; set; }
}

public sealed partial class SearchFriendDataMgr
{
    public sealed partial class SearchFriendData : ISearchFriendData
    {

        public void InitData()
        {
        }

        public void Dispose()
        {

        }

        private List<FriendInfoDto> searchItemList = new List<FriendInfoDto>();
        public List<FriendInfoDto> SearchItemList { get { return searchItemList; } }

        bool _isChange = true;
        public bool IsChange
        {
            get { return _isChange; }
            set { _isChange = value; }
        }

        bool _isCanSearch = true;
        public bool CanSearch
        {
            get { return _isCanSearch; }
            set { _isCanSearch = value; }
        }

        bool _isCanChange = true;
        public bool CanChange
        {
            get { return _isCanChange; }
            set { _isCanChange = value; }
        }

        public long ChangeCDTime { get; set; }

        public void UpdateSearch(FriendListDto dto)
        {
            searchItemList = dto.friendsInfoDtos;

            if(searchItemList.IsNullOrEmpty())
            {
                TipManager.AddTip("您搜索的玩家不存在或者不在线");
            }
            
            JSTimer.Instance.SetupCoolDown("SearchFriend", 5f, null, delegate
            {
                //if (!UIModuleManager.Instance.IsModuleOpened(SearchFriendView.NAME))
                //    return;

                _isCanSearch = true;
            });
        }

        public void UpdateRecommend(FriendListDto dto)
        {
            searchItemList = dto.friendsInfoDtos;
        }

        public void UpdateChange(FriendListDto dto)
        {
            if (!UIModuleManager.Instance.IsModuleOpened(SearchFriendView.NAME))
                return;

            searchItemList = dto.friendsInfoDtos;
            IsChange = true;
            _isCanChange = false;

            ChangeCDTime = SystemTimeManager.Instance.GetUTCTimeStamp() + 120 * 1000;
            JSTimer.Instance.SetupCoolDown("RefreshTime", 120f, null, delegate
            {
                if (!UIModuleManager.Instance.IsModuleOpened(SearchFriendView.NAME))
                    return;

                _isCanChange = true;
            });
        }
    }
}
