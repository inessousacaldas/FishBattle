// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 1/19/2018 5:35:39 PM
// **********************************************************************

using AppDto;
using System.Collections.Generic;
using System.Text;
using System;

public enum ScheduleRightViewTab : int
{
    DaliyActView = 0, //日常活动
    LimitActView = 1, //限时活动
    RewardBackView = 2, //奖励找回
}

public enum ScheduleActivityState : int
{
    Activate = 0,//激活
    Normal = 1,//正常
    Miss = 2,//错过
    Complete = 3,//完成
}

public interface IScheduleMainViewData
{
    IEnumerable<ScheduleActivity> ScheduleActivityList { get; }
    ScheduleRightViewTab CurRightTab { get; }
    IEnumerable<ActiveDto> DailyActivityList { get; }
    IEnumerable<ActiveDto> LimitActivityList { get; }
    IEnumerable<RegainInfoDto> RewardBackList { get; }
    IEnumerable<int> ActivityRewardList { get; }
    int ActiveValue { get; }
    int ActiveMaxValue { get; }
    ScheduleActivity.ControlTypeEnum CurTypeBtn { get; set; }
}

public sealed partial class ScheduleMainViewDataMgr
{
    public sealed partial class ScheduleMainViewData:IScheduleMainViewData
    {
        public ScheduleRightViewTab CurRightTab { get; set; }

        public static List<ITabInfo> _RightViewTabInfos = new List<ITabInfo>()
        {
            TabInfoData.Create((int)ScheduleRightViewTab.DaliyActView,"日常活动"),
            TabInfoData.Create((int)ScheduleRightViewTab.LimitActView,"限时活动"),
            TabInfoData.Create((int)ScheduleRightViewTab.RewardBackView,"奖励找回")
        };

        public List<ITabInfo> RightViewTabInfos
        {
            
            get
            {
                if (_rewardBackList.Count == 0)
                    _RightViewTabInfos.Remove(e => e.EnumValue == (int)ScheduleRightViewTab.RewardBackView);
                else
                {
                    var item = _RightViewTabInfos.Find(e => e.EnumValue == (int)ScheduleRightViewTab.RewardBackView);
                    if(item == null)
                        _RightViewTabInfos.Add(TabInfoData.Create((int)ScheduleRightViewTab.RewardBackView, "奖励找回"));
                }
                return _RightViewTabInfos;
            }
        }

        public void InitData()
        {
            CurTypeBtn = ScheduleActivity.ControlTypeEnum.All;

            //客户端监测是否到推送时间 每分钟检测一次
            _remindActivityTimer = JSTimer.Instance.SetupTimer(TimerName + this.GetHashCode(), () =>
            {
                _remindActivityList.ForEach(itemData =>
                {
                    for(int i=0; i< itemData.Value.Count; i++)
                    {
                        var strs1 = itemData.Value[i].Split('-');
                        if (strs1.Length < 2) continue;
                        if (GetIsRemindTime(strs1[0]))
                        {
                            TipManager.AddTip(string.Format("【{0}】还有五分钟开始", itemData.Key));
                            break;
                        }
                    }
                });
            }, 10f, false);
        }

        public void Dispose()
        {
            if (_remindActivityTimer != null)
            {
                _remindActivityTimer.Cancel();
                _remindActivityTimer = null;
            }
        }

        private JSTimer.TimerTask _remindActivityTimer;
        private static readonly string TimerName = "ScheduleViewData";
        private Dictionary<string, List<string>> _remindActivityList = new Dictionary<string, List<string>>();
        private List<ScheduleActivity> scheduleActivityList;
        public IEnumerable<ScheduleActivity> ScheduleActivityList   //日程表格数据
        {
            get
            {
                if (scheduleActivityList == null)
                    scheduleActivityList = DataCache.getArrayByCls<ScheduleActivity>();
                return scheduleActivityList;
            }
        }

        //日常活动
        private List<ActiveDto> _dailyActivityList = new List<ActiveDto>();
        public IEnumerable<ActiveDto> DailyActivityList
        {
            get { return _dailyActivityList; }
        }
        //限时活动
        private List<ActiveDto> _limitActivityList = new List<ActiveDto>();
        public IEnumerable<ActiveDto> LimitActivityList
        {
            get { return _limitActivityList; }
        }
        //奖励找回
        private List<RegainInfoDto> _rewardBackList = new List<RegainInfoDto>();
        public IEnumerable<RegainInfoDto> RewardBackList
        {
            get { return _rewardBackList; }
        }
        //已领取活跃度奖励
        private List<int> _activityRewardList = new List<int>();
        public IEnumerable<int> ActivityRewardList
        {
            get { return _activityRewardList; }
        }
        private int _activeValue = 0;
        public int ActiveValue
        {
            get { return _activeValue; }
        }
        public void AddActivityValue(int addNum)
        {
            _activeValue += addNum;
        }
        private readonly int _activeMaxValue = 120;
        public int ActiveMaxValue
        {
            get { return _activeMaxValue; }
        }
        private List<int> _cancelNotifyIdList = new List<int>();
        public IEnumerable<int> CancelNotifyIdList { get { return _cancelNotifyIdList; } }
        public ScheduleActivity.ControlTypeEnum CurTypeBtn { get; set; }
        public void UpdateActivityData(ScheduleDto dto)
        {
            if (dto == null) return;

            _activeValue = dto.activeValue;
            _activityRewardList.Clear();
            dto.actives.ForEach(itemId =>
            {
                _activityRewardList.Add(itemId);
            });

            _dailyActivityList.Clear();
            _limitActivityList.Clear();
            _rewardBackList.Clear();
            _rewardBackList = dto.regainInfos;
            var dailyStateList = new Dictionary<int, int>();
            var limitStateList = new Dictionary<int, int>();
            dto.activeList.ForEach(activityItem =>
            {
                var val = ScheduleActivityList.Find(e => e.id == activityItem.id);
                if (val != null && FunctionOpenHelper.isFuncOpen(val.openGradeId))
                {
                    if (activityItem.type == 1) //日常活动
                    {
                        _dailyActivityList.Add(activityItem);
                        dailyStateList.Add(activityItem.id, GetActivityState(activityItem, val));
                    }
                    else if (activityItem.type == 2) //显示活动
                    {
                        _limitActivityList.Add(activityItem);
                        limitStateList.Add(activityItem.id, GetActivityState(activityItem, val));
                    }
                }
            });
            ActivitySort(_dailyActivityList, dailyStateList);
            ActivitySort(_limitActivityList, limitStateList);
            //找回奖励按时间排序
            _rewardBackList.Sort((a, b) =>
            {
                return a.expiredTime.CompareTo(b.expiredTime);
            });
            RemoveOverTimeReward();
        }

        //活动按状态排序
        private static Comparison<ActiveDto> _comparison = null;
        private static List<ActiveDto> ActivitySort(List<ActiveDto> list, Dictionary<int, int> stateList)
        {
            if (_comparison == null)
            {
                _comparison = (a, b) =>
                {
                    var stateA = stateList[a.id];
                    var stateB = stateList[b.id];
                    return stateA.CompareTo(stateB);
                };
            }

            list.Sort(_comparison);
            _comparison = null;
            return list;
        }

        public void AddActivityReward(int id)
        {
            if (!_activityRewardList.Contains(id))
                _activityRewardList.Add(id);
        }

        public void UpdateCancelList()
        {
            _cancelNotifyIdList.Clear();
            ModelManager.Player.ScheduleCancelNotify.ForEach(itemId =>
            {
                _cancelNotifyIdList.Add(itemId);
            });

            UpdateRemindList();
        }

        public ActiveDto GetActivityDtoById(int id)
        {
            if (_dailyActivityList.Find(item => item.id == id) != null)
                return _dailyActivityList.Find(item => item.id == id);
            else if (_limitActivityList.Find(item => item.id == id) != null)
                return _limitActivityList.Find(item => item.id == id);
            else
                return null;
        }

        //更新提醒列表
        private void UpdateRemindList()
        {
            _remindActivityList.Clear();
            ScheduleActivityList.ForEach(itemData =>
            {
                if (itemData.deliver && !_cancelNotifyIdList.Contains(itemData.id))
                {
                    _remindActivityList.Add(itemData.name, itemData.openTimes);
                }
            });
        }

        //获取活动是否到了推送时间（活动开始前5分钟）
        private bool GetIsRemindTime(string time)
        {
            var strs = time.Split(':');
            if (strs.Length < 3) return false;

            var timeHour = int.Parse(strs[0]);
            var timeMin = int.Parse(strs[1]);
            var timeSec = int.Parse(strs[2]);

            if (SystemTimeManager.Instance.GetServerTime().Hour == timeHour)
            {
                if (SystemTimeManager.Instance.GetServerTime().Minute < timeMin)
                {
                    if (timeMin - SystemTimeManager.Instance.GetServerTime().Minute == 5)
                        return true;
                    return false;
                }
                return false;
            }
            return false;
        }
        
        //时间是否超过 用于判断活动当前状态
        private bool GetIsOverTime(string time)
        {
            var strs = time.Split(':');
            if (strs.Length < 3) return false;

            var timeHour = int.Parse(strs[0]);
            var timeMin = int.Parse(strs[1]);
            var timeSec = int.Parse(strs[2]);

            if (SystemTimeManager.Instance.GetServerTime().Hour < timeHour)
                return false;
            else if (SystemTimeManager.Instance.GetServerTime().Hour == timeHour)
            {
                if (SystemTimeManager.Instance.GetServerTime().Minute < timeMin)
                    return false;
                else if (SystemTimeManager.Instance.GetServerTime().Minute == timeMin)
                {
                    if (SystemTimeManager.Instance.GetServerTime().Second < timeSec)
                        return false;
                    else
                        return true;
                }
                else
                    return true;
            }
            else
                return true;
        }

        //获取活动状态 0：激活 1：整除 2：完成 3：错过
        public int GetActivityState(ActiveDto dto, ScheduleActivity itemData)
        {
            if (itemData.rewardCount != -1 && dto.count >= itemData.rewardCount)  //已完成
            {
                return (int)ScheduleActivityState.Complete;
            }
            else
            {
                if (itemData.openTimes.IsNullOrEmpty())  //全天开放
                    return (int)ScheduleActivityState.Activate;
                else  //有开放时间
                {
                    //是否过了开放时间
                    var isOutOfTime = true;
                    //活动是否在进行中  false为活动没开始
                    var isActivityIng = false;
                    var showStartTime = string.Empty;
                    for (int i = 0; i < itemData.openTimes.Count; i++)
                    {
                        var str = itemData.openTimes[i].Split('-');
                        if (str.Length < 2) return (int)ScheduleActivityState.Activate;
                        var startTime = str[0];
                        var endTime = str[1];

                        //活动未开始
                        if (!GetIsOverTime(startTime))
                        {
                            isOutOfTime = false;
                            showStartTime = startTime;
                            break;
                        }
                        else  //活动已开始（进行中或已结束）
                        {
                            //活动进行中
                            if (!GetIsOverTime(endTime))
                            {
                                isOutOfTime = false;
                                isActivityIng = true;
                                break;
                            }
                            else  //活动已结束
                                showStartTime = startTime;
                        }
                    }

                    if (isOutOfTime)
                    {
                        return (int)ScheduleActivityState.Miss;
                    }
                    else
                    {
                        if (isActivityIng)
                            return (int)ScheduleActivityState.Activate;
                        else
                            return (int)ScheduleActivityState.Normal;
                    }
                }
            }
        }

        //获取未开始活动的开始时间 和已错过活动的最后开始时间
        public string GetActivityStartTime(ActiveDto dto, ScheduleActivity itemData)
        {
            if (itemData.rewardCount != -1 && dto.count >= itemData.rewardCount)  //已完成
            {
                return string.Empty;
            }
            else
            {
                if (itemData.openTimes.IsNullOrEmpty())  //全天开放
                    return string.Empty;
                else  //有开放时间
                {
                    //是否过了开放时间
                    var isOutOfTime = true;
                    //活动是否在进行中  false为活动没开始
                    var isActivityIng = false;
                    var showStartTime = string.Empty;
                    for (int i = 0; i < itemData.openTimes.Count; i++)
                    {
                        var str = itemData.openTimes[i].Split('-');
                        if (str.Length < 2) return string.Empty;
                        var startTime = str[0];
                        var endTime = str[1];

                        //活动未开始
                        if (!GetIsOverTime(startTime))
                        {
                            isOutOfTime = false;
                            showStartTime = startTime;
                            break;
                        }
                        else  //活动已开始（进行中或已结束）
                        {
                            //活动进行中
                            if (!GetIsOverTime(endTime))
                            {
                                isOutOfTime = false;
                                isActivityIng = true;
                                break;
                            }
                            else  //活动已结束
                                showStartTime = startTime;
                        }
                    }

                    if (isOutOfTime)
                    {
                        return showStartTime;
                    }
                    else
                    {
                        if (isActivityIng)
                            return string.Empty;
                        else
                            return showStartTime;
                    }
                }
            }
        }

        //奖励找回，数据更新
        public void UpdateRewardBack(int regainType, long regainId)
        {
            ScheduleActivity.RegainTypeEnum type = (ScheduleActivity.RegainTypeEnum)regainType;
            JSTimer.Instance.CancelCd(ScheduleMainViewLogic.ScheduleRewardBackItemStr + regainId);
            if (type == ScheduleActivity.RegainTypeEnum.RegainType_3 || type == ScheduleActivity.RegainTypeEnum.RegainType_4)
            {
                //_rewardBackList.ForEach(e =>
                //{
                //    e.receive = true;
                //});
                _rewardBackList.Clear();
            }
            else
            {
                //var item = _rewardBackList.Find(e => e.regainId == regainId);
                //item.receive = true;
                //_rewardBackList.Replace<RegainInfoDto>(e => e.regainId == regainId, item);
                _rewardBackList.Remove(e => e.regainId == regainId);
            }
            RemoveOverTimeReward();
        }
        //奖励到期，数据更新
        public void UpdateRewardBack(long regainId)
        {
            _rewardBackList.Remove(e => e.regainId == regainId);
        }

        #region 移除已过期奖励
        //移除已过期奖励
        private void RemoveOverTimeReward()
        {
            _rewardBackList.RemoveItems(e => RemoveOver(e.expiredTime));
        }

        private bool RemoveOver(long ms)
        {
            var timer = GetTimer(ms);
            return timer <= 0;
        }
        private long GetTimer(long ms)
        {
            var _unixTimeStamp = DateUtil.DateTimeToUnixTimestamp(DateTime.Now);
            var res = ms - _unixTimeStamp;
            return res;
        }
        #endregion
    }
}
