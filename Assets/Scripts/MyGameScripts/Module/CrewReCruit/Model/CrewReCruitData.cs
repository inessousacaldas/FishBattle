// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 11/20/2017 5:02:26 PM
// **********************************************************************
using AppDto;
using System.Collections.Generic;
using System;

public interface ICrewReCruitData
{
    ServerType IGetmServerType();
    IEnumerable<CrewInfoDto> IGetCrewInfoDtoList();
    IEnumerable<CrewChipDto> IGetCrewChipDtoList();
    int IGetmainRecruitType();
    CrewInfoDto GetMainCrewInfoDto();
}

public sealed partial class CrewReCruitDataMgr
{
    public sealed partial class CrewReCruitData:ICrewReCruitData
    {
        /** 获得伙伴信息 **/
        private List<CrewInfoDto> mCrewInfos = new List<CrewInfoDto>();
        /** 伙伴转化为碎片信息 */
        private List<CrewChipDto> mCrewChipInfos = new List<CrewChipDto>();
        private ServerType mServerType = ServerType.None;
        //购买当前类型
        private int mainRecruitType;
        public CrewInfoDto _mainCrewInfoDto;
        public void InitData()
        {
        }

        public void Dispose()
        {

        }

        public void UpdateCrewReward(List<CrewInfoDto> tCrewInfos,List<CrewChipDto> tCrewChipInfos,int tRecruitType)
        {
            mCrewInfos.Clear();
            mCrewChipInfos.Clear();
            mCrewInfos = tCrewInfos;
            mCrewChipInfos = tCrewChipInfos;
            mServerType = ServerType.UpDate;
            mainRecruitType = tRecruitType;
            FireData();
            mServerType = ServerType.None;
        }

        public ServerType IGetmServerType()
        {
            return mServerType;
        }


        public IEnumerable<CrewInfoDto> IGetCrewInfoDtoList()
        {
            return mCrewInfos;
        }

        public IEnumerable<CrewChipDto> IGetCrewChipDtoList()
        {
            return mCrewChipInfos;
        }

        public int IGetmainRecruitType() {
            return mainRecruitType;
        }

        public CrewInfoDto GetMainCrewInfoDto() {
            return _mainCrewInfoDto;
        }

    }
}
