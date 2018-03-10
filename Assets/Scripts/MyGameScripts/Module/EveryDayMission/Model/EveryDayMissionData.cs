// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 11/10/2017 4:36:56 PM
// **********************************************************************
using AppDto;
using System.Collections.Generic;


public enum ServerType
{
    None,
    Init = 1,
    UpDate = 2,
    PopUpTip = 3
}

public interface IEveryDayMissionData
{
    ServerType GetmServerType();
    Dictionary<int,FactionMissionRate> GetEveryDayMissionList();
    int GetDailyFinishCount();
}

public sealed partial class EveryDayMissionDataMgr
{
    public sealed partial class EveryDayMissionData:IEveryDayMissionData
    {

        public ServerType mServerType = ServerType.None;
        public Dictionary<int,FactionMissionRate> mEveryDayMissionList = new Dictionary<int, FactionMissionRate>();
        public int mDailyFinishCount;
        public void InitData()
        {
        }

        public void Dispose()
        {
            mEveryDayMissionList.Clear();
        }

        /// <summary>
        /// 打开面板，初始化mission列表
        /// </summary>
        /// <param name="MissionListID"></param>
        public void InitMissionList(FactionMissionIdDto factionMissionIdDto)
        {
            List<int> MissionListID = factionMissionIdDto.factionMissionIds;
            MissionListID.ForEach(e =>
            {
                if(!mEveryDayMissionList.ContainsKey(e))
                {
                    FactionMissionRate tMission = DataCache.getDtoByCls<FactionMissionRate>(e);
                    mEveryDayMissionList.Add(e,DataCache.getDtoByCls<FactionMissionRate>(e));
                }
            });
            if(factionMissionIdDto.dailyFinishCount < DataCache.GetStaticConfigValue(AppStaticConfigs.FACTION_MISSION_DAILY_FINISH_REWARD_PARAM))
            {
                mServerType = ServerType.Init;
            }
            else
            {
                mServerType = ServerType.PopUpTip;
            }
            mDailyFinishCount = factionMissionIdDto.dailyFinishCount;
            FireData();
        }

        public ServerType GetmServerType()
        {
            return mServerType;
        }

        public Dictionary<int,FactionMissionRate> GetEveryDayMissionList()
        {
            return mEveryDayMissionList;
        }


        public int GetDailyFinishCount() {
            return mDailyFinishCount;
        }

        public void UpdataPlayerMissionNotify(PlayerMissionNotify notify)
        {
            //if(notify.playerMissionDto.mission.missionType. == MissionType.MissionTypeEnum.Urgent) {

            //}
        }
    }
}
