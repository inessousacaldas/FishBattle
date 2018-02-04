// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 12/17/2017 2:57:47 PM
// **********************************************************************

using AppDto;
using System.Collections.Generic;

public interface IContestData
{
    void SetCurBagItem(BagItemDto tCurBagItem);
    ServerType GetServerType();
    Crew MianCrew();
    BagItemDto GetItem();
}

public sealed partial class ContestDataMgr
{
    public sealed partial class ContestData:IContestData
    {
        private ServerType mServerType = ServerType.None;
        private BagItemDto mCurBagItem;
        private Crew mCrew;
        public void InitData()
        {
        }

        public void Dispose()
        {

        }

        public ServerType GetServerType() {
            return mServerType;
        }

        public Crew MianCrew() {
            return mCrew;
        }

        public BagItemDto GetItem() {
            return mCurBagItem;
        }

        public void SetCurBagItem(BagItemDto tCurBagItem)
        {
            mCurBagItem = tCurBagItem;
            Dictionary<int,GeneralCharactor> tCrewDataDic =  DataCache.getDicByCls<GeneralCharactor>();
            PropsParam_19 itemDto = (tCurBagItem.item as Props).propsParam as PropsParam_19;
            int mCrewID = itemDto.crewId;
            if(tCrewDataDic.ContainsKey(mCrewID))
            {
                mCrew = tCrewDataDic[mCrewID] as Crew;
            }
            mServerType = ServerType.Init;
            FireData();
        }
    }
}
