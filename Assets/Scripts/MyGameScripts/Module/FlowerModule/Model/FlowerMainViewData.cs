// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 1/13/2018 10:18:38 AM
// **********************************************************************

using AppDto;
using System.Collections.Generic;
using System;

public interface IFlowerMainViewData
{
    long CurFriendId { get; set; }
    int CurFlowerId { get; set; }
    IEnumerable<FriendInfoDto> SearchList { get; }
    int CurFlowerCount { get; set; }
    string CurFlowerContent { get; set; }
}

public sealed partial class FlowerMainViewDataMgr
{
    public sealed partial class FlowerMainViewData:IFlowerMainViewData
    {

        public void InitData()
        {
            CurFlowerCount = 1;
        }

        public void Dispose()
        {

        }

        public long CurFriendId { get; set; }
        public int CurFlowerId { get; set; }
        public int CurFlowerCount { get; set; }
        public string CurFlowerContent { get; set; }

        private List<FriendInfoDto> _searchList = new List<FriendInfoDto>();
        public void UpdateSearchList(FriendListDto dto)
        {
            if (dto == null) return;
            if (dto.friendsInfoDtos.IsNullOrEmpty())
            {
                TipManager.AddTip("没有搜到匹配玩家");
                return;
            }
            _searchList = dto.friendsInfoDtos;
            CurFriendId = _searchList.IsNullOrEmpty() ? 0 : _searchList[0].friendId;
        }
        public IEnumerable<FriendInfoDto> SearchList { get { return _searchList; } }

        //好友排序规则
        private Comparison<FriendInfoDto> _comparison = null;
        private List<FriendInfoDto> FriendSort(IEnumerable<FriendInfoDto> targetList)
        {
            #region 快排 不稳定 艹
            //if (_comparison == null)
            //{
            //    _comparison = (a, b) =>
            //    {
            //        if (b.online == a.online)
            //        {
            //            if(b.degree == a.degree)
            //                return b.grade.CompareTo(a.grade);
            //            else
            //                return b.degree.CompareTo(a.degree);
            //        }
            //        else
            //            return b.online.CompareTo(a.online);
            //    };
            //}

            //list.Sort(_comparison);
            //return list;
            #endregion

            //这里用冒泡
            var list = targetList.ToList();
            for (var i = 0; i < list.Count; i++)
            {
                for (var j = i; j < list.Count; j++)
                {
                    if (list[i].online == list[j].online)
                    {
                        if (list[i].degree < list[j].degree ||
                            list[i].degree == list[j].degree && list[i].grade < list[j].grade)
                        {
                            var temp = list[i];
                            list[i] = list[j];
                            list[j] = temp;
                        }
                    }
                    else if (list[j].online)
                    {
                        var temp = list[i];
                        list[i] = list[j];
                        list[j] = temp;
                    }
                }
            }

            return list;
        }

        public void AddAndSort(FriendInfoDto curDto=null)
        {
            _searchList = FriendDataMgr.DataMgr.GetMyFriendList().ToList();
            var tempQueue = new Queue<FriendInfoDto>();

            if (curDto != null)
            {
                tempQueue.Enqueue(curDto);
                if (FriendDataMgr.DataMgr.IsMyFriend(curDto.friendId))
                    _searchList.Remove(curDto);
            }

            _searchList.ForEach(itemDto =>
            {
                tempQueue.Enqueue(itemDto);
            });
            _searchList = tempQueue.ToList();

            if (!_searchList.IsNullOrEmpty())
                CurFriendId = _searchList[0].friendId;
        }
    }
}
