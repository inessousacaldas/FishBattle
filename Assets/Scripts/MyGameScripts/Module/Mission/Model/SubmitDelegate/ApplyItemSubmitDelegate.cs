using UnityEngine;
using System.Collections;
using AppDto;

public class ApplyItemSubmitDelegate :BaseSubmitDelegate, ISubmitDelegate
{
    public ApplyItemSubmitDelegate(MissionDataMgr.MissionData model,string submitName):base(model,submitName) {
    }

    public void SubmitConditionUpdate(Mission mission,SubmitDto submitDto) {
        ApplyItemSubmitDto dto = submitDto as ApplyItemSubmitDto;
        if(dto != null && dto.count < dto.needCount && !dto.finish) {

        }
    }

    public void SubmitConditionReach(Mission mission,SubmitDto submitDto) {
        ApplyItemSubmitDto dto = submitDto as ApplyItemSubmitDto;
        if(dto != null && dto.count >= dto.needCount && !dto.finish) {
            NpcInfoDto npcInfo = GetNpc(dto.submitNpc);
            WorldManager.Instance.GetNpcViewManager().AddNpcUnit(npcInfo);
            _model.WaitFindToMissionNpc(mission);
        }
    }

    public void SubmitConditionFinish(Mission mission,SubmitDto submitDto) {
        ApplyItemSubmitDto dto = submitDto as ApplyItemSubmitDto;
        if(dto != null && dto.count >= dto.needCount && dto.finish) {
            NpcInfoDto npcInfo = GetNpc(dto.submitNpc);
            if(npcInfo != null && npcInfo.npc is NpcMonster)
            {
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(npcInfo.id);
            }
        }
    }

    public void SubmitClear(SubmitDto submitDto) {
        ApplyItemSubmitDto dto = submitDto as ApplyItemSubmitDto;
        if(dto != null) {
            NpcInfoDto npcInfo = GetNpc(dto.submitNpc);
            if(npcInfo != null && npcInfo.npc is NpcMonster)
            {
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(npcInfo.id);
            }
        }
    }

    public NpcInfoDto GetMissionNpcInfo(SubmitDto submitDto,bool isGetSubmitNpc) {
        ApplyItemSubmitDto dto = submitDto as ApplyItemSubmitDto;
        if(dto != null) {
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

    public void FinishSubmitDto(Mission mission,SubmitDto submitDto,Npc npc,int battleIndex) {
        MissionDataMgr.MissionNetMsg.FinishMission(mission);
    }

    public bool FindToNpc(Mission mission,SubmitDto submitDto) {
        return false;
    }

    public bool IsBattleDelay(SubmitDto submitDto,MissionSubmitStateNotify notify)
    {
        return true;
    }
}
