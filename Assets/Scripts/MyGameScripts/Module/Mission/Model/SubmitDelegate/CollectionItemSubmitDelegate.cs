using UnityEngine;
using System.Collections;
using AppDto;
using System;
using System.Collections.Generic;

public class CollectionItemSubmitDelegate : BaseSubmitDelegate,ISubmitDelegate {
    public CollectionItemSubmitDelegate(MissionDataMgr.MissionData model,string submitName) : base(model,submitName) {

    }

    public bool FindToNpc(Mission mission,SubmitDto submitDto) {
        return false;
    }

    public void FinishSubmitDto(Mission mission,SubmitDto submitDto,Npc npc,int battleIndex)
    {
        CollectionItemSubmitDto dto = submitDto as CollectionItemSubmitDto;
        if(dto != null)
        {
            if(dto.count >= dto.needCount)
            {
                if(dto.confirm)
                {
                    Dictionary<int, GeneralItem> submitItemDic = new Dictionary<int, GeneralItem>();
                    Dictionary<int, int> submitItemNunberDic = new Dictionary<int, int>();
                    submitItemNunberDic.Add(dto.itemId,dto.count);
                    submitItemDic.Add(dto.itemId,dto.item);
                    //
                    ProxyItemUseModule.OpenMissionSubmitItem(submitItemDic,(packItemDto) =>
                    {
                        MissionDataMgr.MissionNetMsg.FinishMission(mission);
                    },submitItemNunberDic);
                }
                else
                {
                    MissionDataMgr.MissionNetMsg.FinishMission(mission);
                }
            }
            else {

            }
        }
    }

    public NpcInfoDto GetMissionNpcInfo(SubmitDto submitDto,bool isGetSubmitNpc) {
        CollectionItemSubmitDto dto = submitDto as CollectionItemSubmitDto;
        if(dto.count >= dto.needCount)
        {
            return dto.submitNpc;
        }
        else {
            return dto.acceptNpc;
        }
    }

    public bool IsBattleDelay(SubmitDto submitDto,MissionSubmitStateNotify notify) {
        return false;
    }

    public void SubmitClear(SubmitDto submitDto)
    {
        
    }

    public void SubmitConditionFinish(Mission mission,SubmitDto submitDto) {

    }

    public void SubmitConditionReach(Mission mission,SubmitDto submitDto) {

    }

    public void SubmitConditionUpdate(Mission mission,SubmitDto submitDto) {

    }
}
