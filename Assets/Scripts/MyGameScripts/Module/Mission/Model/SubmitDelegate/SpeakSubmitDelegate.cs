using UnityEngine;
using System.Collections;
using AppDto;
using System;

public class SpeakSubmitDelegate : BaseSubmitDelegate, ISubmitDelegate
{
    public SpeakSubmitDelegate(MissionDataMgr.MissionData model,string submitName) : base(model,submitName) {

    }

    public bool FindToNpc(Mission mission,SubmitDto submitDto)
    {
        return false;
    }

    public void FinishSubmitDto(Mission mission,SubmitDto submitDto,Npc npc,int battleIndex)
    {
        SpeakSubmitDto dto = submitDto as SpeakSubmitDto;
        if(dto != null) {
            if(dto.count >= dto.needCount)
                MissionDataMgr.MissionNetMsg.FinishMission(mission);
        }
    }

    public NpcInfoDto GetMissionNpcInfo(SubmitDto submitDto,bool isGetSubmitNpc)
    {
        SpeakSubmitDto dto = submitDto as SpeakSubmitDto;
        if(dto != null)
        {
            NpcInfoDto tNpcInfoDto = null;
            isGetSubmitNpc = submitDto.count >= submitDto.needCount || isGetSubmitNpc;
            Npc npc = new Npc();
            npc.id = -1;
            npc.name = dto.acceptScene.sceneMap.name;
            npc.sceneId = dto.acceptScene.id;
            npc.x = dto.acceptScene.x;
            npc.z = dto.acceptScene.z;
            return isGetSubmitNpc ? dto.submitNpc : MissionHelper.GetNpcInfoDtoByNpc(npc);
        }
        return null;
    }

    public bool IsBattleDelay(SubmitDto submitDto,MissionSubmitStateNotify notify)
    {
        return false;
    }

    public void SubmitClear(SubmitDto submitDto)
    {
        SpeakSubmitDto dto = submitDto as SpeakSubmitDto;
        if(dto != null) {
            NpcInfoDto npcinfo = GetNpc(dto.submitNpc);
            if(npcinfo != null && npcinfo.npc is NpcMonster) {
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(npcinfo.id);
            }
        }
    }

    public void SubmitConditionFinish(Mission mission,SubmitDto submitDto)
    {
        SpeakSubmitDto dto = submitDto as SpeakSubmitDto;
        if(dto != null && dto.count >= dto.needCount && dto.finish)
        {
            NpcInfoDto npcInfo = GetNpc(dto.submitNpc);
            if(npcInfo != null && npcInfo.npc is NpcMonster)
            {
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(npcInfo.id);
            }
        }
    }

    public void SubmitConditionReach(Mission mission,SubmitDto submitDto)
    {
        SpeakSubmitDto dto = submitDto as SpeakSubmitDto;
        if(dto != null && dto.count >= dto.needCount && !dto.finish)
        {
            NpcInfoDto npcInfo = GetNpc(dto.submitNpc);
            WorldManager.Instance.GetNpcViewManager().AddNpcUnit(npcInfo);
            _model.WaitFindToMissionNpc(mission);
        }
    }

    public void SubmitConditionUpdate(Mission mission,SubmitDto submitDto)
    {
        
    }
}
