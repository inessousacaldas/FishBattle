// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 12/13/2017 4:15:20 PM
// **********************************************************************

using AppDto;

public interface ITreasureMissionData
{
    int GetCurIndex();
    ServerType GetServerType();
    int GetDiamondsNumber();
    void HighTreasuryNotify(HighTreasuryNotify tHighTreasuryNotify);
    string GetItemNumber();
    int GetBagIndex();
    int GetMovePoint();
    int GetPropsTreasureRewardId();
    GeneralItem GetGeneralItem();
    bool ItemNumberBool();

    int GetTreasureNumber();

    void OnChangeNextDay();
    int GetGeneralItemNumber { get; }
}

public sealed partial class TreasureMissionDataMgr
{
    public sealed partial class TreasureMissionData:ITreasureMissionData
    {
        private ServerType mServerType = ServerType.None;
        private GeneralItem mGeneralItem;
        private int mCurIndex = 0;
        private int mMovePoint = 0;
        private int mDiamondsNumber = 0;
        private int mNumber;        //拥有高级宝图的数量
        private int BagIndex;       //背包索引
        private string itemName = string.Empty;
        private int mPropsTreasureRewardId;
        private int mTreasureNumber;
        public void InitData()
        {

        }

        public void Dispose()
        {

        }

        public void SetInitData(HighTreasuryInfoDto highTreasuryInfoDto)
        {
            UpdateBagIndex();
            mServerType = ServerType.Init;
            mCurIndex = highTreasuryInfoDto.highTreasuryPosition;
            mDiamondsNumber = highTreasuryInfoDto.rewardPool;
            mTreasureNumber = highTreasuryInfoDto.useCount;
            FireData();
            mServerType = ServerType.None;
        }

        public ServerType GetServerType()
        {
            return mServerType;
        }

        public int GetCurIndex()
        {
            return mCurIndex;
        }

        public int GetDiamondsNumber()
        {
            return mDiamondsNumber;
        }

        //这个是获得使用羊波波劵的数量主界面 1/10 这样
        public string GetItemNumber() {
            return itemName + " " + mNumber + "/1";
        }

        public int GetBagIndex() {
            return BagIndex;
        }

        public GeneralItem GetGeneralItem() {
            return mGeneralItem;
        }


        public int GetPropsTreasureRewardId() {
            return mPropsTreasureRewardId;
        }

        void UpdateBagIndex() {
            mNumber = 0;
            var bagItems = BackpackDataMgr.DataMgr.GetBagItems().ToList();
            for(int i = 0;i < bagItems.Count;i++)
            {
                if(bagItems[i].itemId == 100005)
                {
                    if(itemName == "")
                    {
                        itemName = bagItems[i].item.name;
                    }
                    BagIndex = bagItems[i].index;
                    mNumber += bagItems[i].count;
                }
            }
        }

        public int GetTreasureNumber() {
            return mTreasureNumber;
        }

        public int GetMovePoint() {
            return mMovePoint;
        }

        public bool ItemNumberBool() {
            return mNumber > 0;
        }


        private int mGeneralItemNumber;
        //这个是获得抽奖，获得奖励的数量，用于飘字
        public int GetGeneralItemNumber { get { return mGeneralItemNumber; } }


        public void HighTreasuryNotify(HighTreasuryNotify tHighTreasuryNotify) {
            UpdateBagIndex();
            mServerType = ServerType.UpDate;
            mGeneralItem = DataCache.getDtoByCls<GeneralItem>(tHighTreasuryNotify.thingId);
            mMovePoint = tHighTreasuryNotify.rollPoint;
            mCurIndex = tHighTreasuryNotify.highTreasuryPosition;
            mPropsTreasureRewardId = tHighTreasuryNotify.propsTreasureRewardId;
            mGeneralItemNumber = tHighTreasuryNotify.count;
            mTreasureNumber++;
            if(mTreasureNumber >= 100)
                mTreasureNumber = 100;
            FireData();
            mServerType = ServerType.None;
        }

        public void UpdataDiamondsNotify(HighTreasueyChangeNotify tHighTreasueyChangeNotify)
        {
            mServerType = ServerType.None;
            mDiamondsNumber = tHighTreasueyChangeNotify.diomandPool;
            FireData();
        }

        public void OnChangeNextDay() {
            mTreasureNumber = 0;
            FireData();
        }
    }
}
