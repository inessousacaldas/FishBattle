// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 9/20/2017 7:57:58 PM
// **********************************************************************

using AppDto;
using System.Collections.Generic;
using System.Text;

public enum AssistViewTab : int
{
    ChooseView = 0,  //初次进入选择界面
    LearnCookView = 1,  //选择学习料理
    LearnForceView = 2, //选择学习导力技术
    CookUpGradeView = 3, //料理升级
    CookProductView = 4, //料理生产
    ForceUpGradeView = 5, //导力升级
    ForceProductView = 6, //导力生产
    LearnBaseView = 7, //学习基础界面

    AssistDelegateMain = 8, //委托任务界面
    //AssistDelegateCrew = 9, //委托任务伙伴选择界面
    //AssistDelegateFriend = 10, //委托任务好友选择界面
}

public enum RightUpTab : int
{
    Learn = 0, //学习
    Product = 1,//生产
}

public enum RightViewTab : int
{
    AssistSkillView = 0, //生活技能
    AssistDelegateView = 1 //委托任务
}

public enum AssistFriendViewTab : int
{
    DelegateFriend = 0, //好友
    DelegateGuild = 1 //协会成员
}

public interface IAssistSkillMainData
{
    //生活技能
    bool IsResp { get; }

    int SkillId { get; }

    int SkillLevel { get; }

    int ChosedSkillId { get; set; }

    int CurRecipeId { get; set; }

    AssistViewTab CurTab { get; }

    RightViewTab CurRightTab { get; }

    //委托任务
    AssistViewTab CurDelegateTab { get; set; }
    long ChoseFriendId { get; set; }
    IEnumerable<int> ChoseCrewCrewIdList { get; }

    int CurMissionId { get; set; }
    //DelegateMissionDto CurMissionDto { get; }
    int AcceptNum { get; set; }
    int AcceptLimit { get; set; }
    IEnumerable<DelegateMissionDto> MissionList { get; }
}

public sealed partial class AssistSkillMainDataMgr
{
    public sealed partial class AssistSkillMainData : IAssistSkillMainData
    {
        private Dictionary<int, AssistSkillGradeConsume> gradeConsumeData = DataCache.getDicByCls<AssistSkillGradeConsume>();

        public AssistViewTab CurTab { get; set; }
        public AssistViewTab CurDelegateTab { get; set; }
        public RightViewTab CurRightTab { get; set; }

        private int _skillId = 0;
        private int _skillLevel = 0;
        private bool _isResp = false;
        public int ChosedSkillId { get; set; }
        public int CurRecipeId { get; set; }
        private bool _isFirstForget = true;

        //升级消耗
        private Dictionary<int, int> costList = new Dictionary<int, int>();
        //升级5次消耗
        private Dictionary<int, int> costFiveList = new Dictionary<int, int>();
        public Dictionary<int, int> GetCostList { get { return costList; } }
        public Dictionary<int, int> GetCostFiveList { get { return costFiveList; } }


        public static readonly List<ITabInfo> _TabInfos = new List<ITabInfo>()
        {
            TabInfoData.Create((int)RightUpTab.Learn,"学习"),
            TabInfoData.Create((int)RightUpTab.Product,"生产"),
        };

        public static readonly List<ITabInfo> _RightViewTabInfos = new List<ITabInfo>()
        {
            TabInfoData.Create((int)RightViewTab.AssistSkillView,"生活技能"),
            TabInfoData.Create((int)RightViewTab.AssistDelegateView,"委托任务"),
        };

        public static readonly List<ITabInfo> _ChoseFriendTabInfos = new List<ITabInfo>()
        {
            TabInfoData.Create((int)AssistFriendViewTab.DelegateFriend,"我的好友"),
            TabInfoData.Create((int)AssistFriendViewTab.DelegateGuild,"协会成员"),
        };

        public static readonly List<ITabInfo> _ChoseCrewTabInfos = new List<ITabInfo>()
        {
            TabInfoData.Create((int)PropertyType.All,"全部"),
            TabInfoData.Create((int)PropertyType.Power,"力量"),
            TabInfoData.Create((int)PropertyType.Magic,"魔法"),
            TabInfoData.Create((int)PropertyType.Control,"控制"),
            TabInfoData.Create((int)PropertyType.Treat,"辅助"),
        };

        public void InitData()
        {
            RefreshIsNoTips = false;
        }

        public void Dispose()
        {

        }

        //初始化数据请求返回
        public bool IsResp
        {
            get
            {
                return _isResp;
            }
        }

        public int SkillId
        {
            get
            {
                return _skillId;
            }
        }

        public int SkillLevel
        {
            get
            {
                return _skillLevel;
            }
        }

        public bool FirstForget
        {
            get
            {
                return _isFirstForget;
            }
            set
            {
                _isFirstForget = value;
            }
        }

        public void UpdateInitData(List<AssistSkillModelDto> data)
        {
            _isResp = true;

            if (data.IsNullOrEmpty())
            {
                _skillId = 0;
                _skillLevel = 0;

                CurTab = AssistViewTab.ChooseView;
            }
            else
            {
                data.ForEach(x =>
                {
                    _skillId = x.id;
                    _skillLevel = x.level;
                });

                if (_skillId == (int)AssistSkill.AssistSkillEnum.CarryCook)
                    CurTab = AssistViewTab.CookUpGradeView;
                else if (_skillId == (int)AssistSkill.AssistSkillEnum.GrailCook)
                    CurTab = AssistViewTab.CookUpGradeView;
                else if (_skillId == (int)AssistSkill.AssistSkillEnum.LeadForceSkill)
                    CurTab = AssistViewTab.ForceUpGradeView;

                UpdateUpgradeCost();
            }
        }

        //升级消耗
        private void UpdateUpgradeCost()
        {
            int index = 1;
            bool isSet = false;
            int times = 5;
            costList.Clear();
            costFiveList.Clear();

            gradeConsumeData.ForEach(dataItem =>
            {
                if (index <= times && dataItem.Value.id == _skillLevel + index && gradeConsumeData.Find(x => x.Value.id == _skillLevel).Value != null)
                {
                    gradeConsumeData.Find(x => x.Value.id == _skillLevel + index).Value.studyConsumeItem.ForEachI((itemDto, i) =>
                      {
                          if (!isSet)
                          {
                              costList.Add(itemDto.itemId, itemDto.count);
                              costFiveList.Add(itemDto.itemId, itemDto.count);
                          }
                          else
                          {
                              if (costFiveList.Find(x => x.Key == itemDto.itemId).Key > 0)
                                  costFiveList[itemDto.itemId] = costFiveList.Find(x => x.Key == itemDto.itemId).Value + itemDto.count;
                              else
                                  costFiveList.Add(itemDto.itemId, itemDto.count);
                          }
                      });

                    isSet = true;
                    index++;
                }
            });
        }

        public void UpdateForgetData()
        {
            _skillId = 0;
            _skillLevel = 0;
            ChosedSkillId = 0;
            CurTab = AssistViewTab.ChooseView;
            _isFirstForget = false;
        }

        public void UpgradeData(AssistSkillDto dto)
        {
            //第一次选择学习
            if (_skillId <= 0)
            {
                if (dto.id == (int)AssistSkill.AssistSkillEnum.CarryCook || dto.id == (int)AssistSkill.AssistSkillEnum.GrailCook)
                    CurTab = AssistViewTab.CookUpGradeView;
                else if (dto.id == (int)AssistSkill.AssistSkillEnum.LeadForceSkill)
                    CurTab = AssistViewTab.ForceUpGradeView;
            }

            _skillId = dto.id;
            _skillLevel = dto.level;

            UpdateUpgradeCost();
        }


        //委托任务
        public long ChoseFriendId { get; set; }
        public int CurMissionId { get; set; }
        //public DelegateMissionDto CurMissionDto { get { return _missionList.Find(x => x.id == CurMissionId); } }
        public List<DelegateMissionDto> _missionList = new List<DelegateMissionDto>();

        public IEnumerable<DelegateMissionDto> MissionList
        {
            get { return _missionList; }
        }

        private string _choseCrewStr = string.Empty;
        public string ChoseCrewStr
        {
            get
            {
                var sb = new StringBuilder();
                _choseCrewIdList.ForEach(item =>
                {
                    sb.Append(item);
                    sb.Append(",");
                });
                if(sb.Length > 0)
                    sb.Remove(sb.ToString().LastIndexOf(","), 1);
                _choseCrewStr = sb.ToString();
                return _choseCrewStr;
            }
        }
        //正在任务中的伙伴list
        private List<long> _allIngCrewIdList = new List<long>();
        public IEnumerable<long> AllIngCrewIdList
        {
            get { return _allIngCrewIdList; }
        }
        //今天已协助过的玩家list
        private List<long> _allHelpedFriendIdList = new List<long>();
        public IEnumerable<long> AllHelpedFriendIdList
        {
            get { return _allHelpedFriendIdList; }
        }
        private List<long> _choseCrewIdList = new List<long>();
        private List<int> _choseCrewCrewIdList = new List<int>();
        public IEnumerable<long> ChoseCrewIdList
        {
            get { return _choseCrewIdList; }
        }
        public IEnumerable<int> ChoseCrewCrewIdList
        {
            get { return _choseCrewCrewIdList; }
        }
        public void ResetChoseCrewList(List<long> choseIdList)
        {
            if (choseIdList == null) return;
            _choseCrewIdList = choseIdList;
            _choseCrewCrewIdList.Clear();
            choseIdList.ForEach(itemId =>
            {
                var infoItem = _crewInfoList.Find(x => x.id == itemId);
                if (infoItem != null)
                    _choseCrewCrewIdList.Add(infoItem.crewId);
                else
                    _choseCrewIdList.Remove(itemId);
            });
        }
        public bool RemoveChoseCrewByIndex(int idx)
        {
            if(_choseCrewIdList.Count > idx && _choseCrewCrewIdList.Count > idx)
            {
                _choseCrewIdList.RemoveAt(idx);
                _choseCrewCrewIdList.RemoveAt(idx);
                return true;
            }
            return false;
        }
        public void ResetChoseCrewAndFriend(List<long> crewIds=null, long friendId=0)
        {
            if (crewIds == null && friendId == 0)
            {
                var choseMissionDto = _missionList.Find(x => x.id == CurMissionId);
                if (choseMissionDto == null) return;

                crewIds = choseMissionDto.crewIds;
                friendId = choseMissionDto.friendId;
            }
            
            ChoseFriendId = friendId;
            ResetChoseCrewList(crewIds);
            ResetIngCrewIdList(_missionList);
        }

        private List<CrewShortDto> _crewInfoList = new List<CrewShortDto>();
        public IEnumerable<CrewShortDto> CrewInfoList
        {
            get
            {
                return _crewInfoList;
            }
        }

        public void UpdateCrewInfo(CrewShortListDto dto)
        {
            _crewInfoList = dto.crewShortDtos;
        }

        public int AcceptNum { get; set; }
        public int AcceptLimit { get; set; }
        public bool RefreshIsNoTips { get; set; }
        public void UpdateMissionData(DelegateMissionHoleDto dto)
        {
            if (dto == null) return;
            AcceptNum = dto.acceptNum;
            AcceptLimit = dto.acceptLimit;
            _missionList.Clear();
            _allHelpedFriendIdList.Clear();
            
            if(!dto.delegateMissionDtos.IsNullOrEmpty())
            {
                _missionList = dto.delegateMissionDtos;
                //默认选择第一个任务
                CurMissionId = _missionList[0].id;
                ResetChoseCrewAndFriend(_missionList[0].crewIds, _missionList[0].friendId);
                
                _allHelpedFriendIdList = dto.helpFriend;
            }
            else
            {
                CurMissionId = 0;
            }
            //红点测试xjd 注释了免得显示红点影响界面 要用就打开
            //UpdateDelegateRed();
        }

        //红点测试xjd
        public void UpdateDelegateRed()
        {
            var isHavCompleteMission = false;
            _missionList.ForEach(itemDto =>
            {
                //如果有未开始的任务就显示红点 测试方便
                if (itemDto.finishTime <= 0)
                    isHavCompleteMission = true;
            });
//            RedPointDataMgr.DataMgr.UpdateSingleData(2, isHavCompleteMission);
        }

        //接受任务更新dto
        public void UpdateCurMissionData(DelegateMissionDto dto)
        {
            if (dto == null) return;
            _missionList.ReplaceOrAdd(x => x.id == dto.id, dto);
            CurMissionId = dto.id;
            ResetChoseCrewAndFriend(dto.crewIds, dto.friendId);
            AddIngCrewIdList(dto.crewIds);
            if(dto.friendId != 0)
                _allHelpedFriendIdList.Add(dto.friendId);
        }

        public void ReplaceCurMissionDataWithNew(int missionId, DelegateMissionDto dto)
        {
            _missionList.Remove(x => x.id == missionId);
            if (dto == null) return;
            _missionList.Add(dto);
            CurMissionId = _missionList[0].id;
            ResetChoseCrewAndFriend(_missionList[0].crewIds, _missionList[0].friendId);
        }

        private void ResetIngCrewIdList(List<DelegateMissionDto> delegateMissionDtos)
        {
            _allIngCrewIdList.Clear();
            delegateMissionDtos.ForEach(itemMis =>
            {
                itemMis.crewIds.ForEach(ingId =>
                {
                    _allIngCrewIdList.Add(ingId);
                });
            });
        }

        private void AddIngCrewIdList(List<long> crewIds)
        {
            _allIngCrewIdList.Clear();
            crewIds.ForEach(ingId =>
            {
                _allIngCrewIdList.Add(ingId);
            });
        }
    }
}
