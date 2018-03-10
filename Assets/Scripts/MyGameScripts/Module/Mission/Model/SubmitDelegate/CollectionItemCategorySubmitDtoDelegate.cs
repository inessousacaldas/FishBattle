using UnityEngine;
using System.Collections;
using AppDto;
using System;

public class CollectionItemCategorySubmitDtoDelegate:BaseSubmitDelegate, ISubmitDelegate
{
    public CollectionItemCategorySubmitDtoDelegate(MissionDataMgr.MissionData model,string submitName) : base(model,submitName) {

    }
    public bool FindToNpc(Mission mission,SubmitDto submitDto)
    {
        return false;
    }

    public void FinishSubmitDto(Mission mission,SubmitDto submitDto,Npc npc,int battleIndex)
    {
        
    }

    public NpcInfoDto GetMissionNpcInfo(SubmitDto submitDto,bool isGetSubmitNpc)
    {
        CollectionItemCategorySubmitDto dto = submitDto as CollectionItemCategorySubmitDto;
        if(dto.count >= dto.needCount)
        {
            return dto.submitNpc;
        }
        else {
            return dto.acceptNpc;
        }
    }

    public bool IsBattleDelay(SubmitDto submitDto,MissionSubmitStateNotify notify)
    {
        return false;
    }

    public void SubmitClear(SubmitDto submitDto)
    {
        
    }

    public void SubmitConditionFinish(Mission mission,SubmitDto submitDto)
    {
        
    }

    public void SubmitConditionReach(Mission mission,SubmitDto submitDto)
    {
        
    }

    public void SubmitConditionUpdate(Mission mission,SubmitDto submitDto)
    {
        
    }
}
