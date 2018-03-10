// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 2/8/2018 3:10:11 PM
// **********************************************************************


public interface ISpecialCopyData
{
    int GetMission { get; set; }
}

public sealed partial class SpecialCopyDataMgr
{
    public sealed partial class SpecialCopyData:ISpecialCopyData
    {
        private int missionID;
        public int GetMission
        {
            get
            {
                return missionID;
            }
            set {
                missionID = value;
            }

        }
        public void InitData()
        {
            
        }

        public void Dispose()
        {

        }
    }
}
