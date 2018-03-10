// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : wujunjie
// Created  : 8/10/2017 7:46:59 PM
// **********************************************************************

public enum CrewStrengthenTab
{
    Phase, //进阶
    Raise, //强化
    Craft  //研修
}

public interface ICrewStrengthenViewData
{

}

public sealed partial class CrewStrengthenViewDataMgr
{
    public sealed partial class CrewStrengthenViewData:ICrewStrengthenViewData
    {
        public CrewStrengthenViewData()
        {

        }

        public void InitData()
        {
        }

        public void Dispose()
        {

        }
    }
}
