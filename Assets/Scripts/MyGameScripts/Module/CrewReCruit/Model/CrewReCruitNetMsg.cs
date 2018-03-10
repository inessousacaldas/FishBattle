// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 11/20/2017 5:22:48 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class CrewReCruitDataMgr
{
    public static class CrewReCruitNetMsg
    {
        public static void AddCrewSend(int currencyType,bool useProp)
        {
            GameUtil.GeneralReq<CrewInfosDto>(Services.Crew_CurrencyAdd(currencyType,useProp),delegate (CrewInfosDto e)
            {
                ProxyCrewReCruit.crewCurrencyAddTimes--;
                DataMgr._data.UpdateCrewReward(e.crewInfos,e.crewChipInfos,currencyType);
            },
            (e) =>
            {

            });
        }

        public static void Crew_List()
        {
            GameUtil.GeneralReq(Services.Crew_Info(),response =>
            {
                CrewDto _dto = response as CrewDto;
                if(_dto.crewInfos.Count == 1) {
                    DataMgr._data._mainCrewInfoDto = _dto.crewInfos[0];
                    //GameEventCenter.SendEvent(GameEvent.CHANGE_MAIN_CREW,_dto.crewInfos[0].id);
                }
            });
        }
    }
}
