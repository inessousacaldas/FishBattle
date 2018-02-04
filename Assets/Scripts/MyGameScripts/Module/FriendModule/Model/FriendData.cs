using System.Collections.Generic;
using AppDto;
using System;


public enum FriendViewTab
{
    MyFriend, //好友
    AddFriend, //添加
    RecentlyTeammates, //最近队友
    BlackFriend, //黑名单
}

public interface IFriendData
{
    //好友列表
    List<FriendInfoDto> CacheFriendInfoDtos { get; }

    //黑名单
    List<FriendInfoDto> CacheBlackList { get; }

    //最近组队
    List<FriendInfoDto> CacheTeammateList { get; set; }

    FriendInfoDto GetFriendDtoById(long id);

    FriendViewTab CurTab { get; set; }
}

public sealed partial class FriendDataMgr
{
    public sealed partial class FriendData : IFriendData
    {
        #region 机器生成
        public FriendData()
        {

        }

        public void InitData()
        {
        }

        public void Dispose()
        {

        }
        #endregion

        public static readonly List<ITabInfo> _TabInfos = new List<ITabInfo>()
        {
            TabInfoData.Create((int)FriendViewTab.MyFriend, "好友"),
            TabInfoData.Create((int)FriendViewTab.AddFriend, "添加"),
            TabInfoData.Create((int)FriendViewTab.RecentlyTeammates, "最近队友"),
            TabInfoData.Create((int)FriendViewTab.BlackFriend, "黑名单")
        };

        public FriendViewTab CurTab { get; set; }

        //缓存登录拉取总信息
        private FriendsDto _cacheFriendsDto;

        public void UpdateData(FriendsDto dto)
        {
            _cacheFriendsDto = dto;
        }

        //客户端缓存的好友列表
        public List<FriendInfoDto> CacheFriendInfoDtos          
        {
            get
            {
                if (_cacheFriendsDto == null)
                    return new List<FriendInfoDto>();

                return FriendSort(_cacheFriendsDto.friendsInfoDtos);
            }
        }

        //缓存的黑名单
        public List<FriendInfoDto> CacheBlackList                        
        {
            get
            {
                if (_cacheFriendsDto == null)
                    return new List<FriendInfoDto>();

                return _cacheFriendsDto.blackList;
            }
        }

        //最近组队队友
        private List<FriendInfoDto> _cacheTeammateList = new List<FriendInfoDto>();
        public List<FriendInfoDto> CacheTeammateList
        {
            get
            {
                _cacheTeammateList.Reverse();
                return _cacheTeammateList;
            }
            set
            {
                _cacheTeammateList = value;
            }
        }

        public FriendInfoDto GetFriendDtoById(long id)   //获取好友
        {
            return CacheFriendInfoDtos.Find(dto => dto.friendId == id) == null ? null 
                : CacheFriendInfoDtos.Find(dto => dto.friendId == id);
        }

        //好友排序规则
        private static Comparison<FriendInfoDto> _comparison = null;
        private static List<FriendInfoDto> FriendSort(List<FriendInfoDto> list)
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
            for(var i=0; i<list.Count; i++)
            {
                for(var j=i; j<list.Count; j++)
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
                    else if(list[j].online)
                    {
                        var temp = list[i];
                        list[i] = list[j];
                        list[j] = temp;
                    }
                }
            }

            return list;
        }
    }
}

