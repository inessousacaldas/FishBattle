using UnityEngine;
using System.Collections;
using AppDto;

public class TalkSubmitDelegate :BaseSubmitDelegate, ISubmitDelegate
{
    public TalkSubmitDelegate(MissionDataMgr.MissionData model,string submitName) : base(model,submitName) { }

    public NpcInfoDto GetMissionNpcInfo(SubmitDto submitDto,bool isGetSubmitNpc) {
        TalkSubmitDto dto = submitDto as TalkSubmitDto;
        if(dto != null) {
            return dto.submitNpc;
        }
        return null;
    }

    public bool IsBattleDelay(SubmitDto submitDto,MissionSubmitStateNotify notify)
    {
        return true;
    }

    public void SubmitConditionUpdate(Mission mission,SubmitDto submitDto) {
        TalkSubmitDto dto = submitDto as TalkSubmitDto;
        if(dto != null && dto.count < dto.needCount && !dto.finish) {
            NpcInfoDto npcinfo = GetNpc(dto.submitNpc);
            WorldManager.Instance.GetNpcViewManager().AddNpcUnit(npcinfo);
        }
    }

    public void SubmitConditionReach(Mission mission,SubmitDto submitDto) {
        TalkSubmitDto dto = submitDto as TalkSubmitDto;
        if(dto != null && dto.count >= dto.needCount && !dto.finish) {
            NpcInfoDto npcinfo = GetNpc(dto.submitNpc);
            WorldManager.Instance.GetNpcViewManager().AddNpcUnit(npcinfo);
            MissionDataMgr.MissionNetMsg.FinishMission(mission);
        }
    }

    public void SubmitConditionFinish(Mission mission,SubmitDto submitDto) {
        TalkSubmitDto dto = submitDto as TalkSubmitDto;
        if(dto != null && dto.count >= dto.needCount && dto.finish) {
            NpcInfoDto npcInfo = GetNpc(dto.submitNpc);
            if(npcInfo != null && npcInfo.npc is NpcMonster)
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(npcInfo.id);
        }
    }

    public void FinishSubmitDto(Mission mission,SubmitDto submitDto,Npc npc,int battleIndex) {
        MissionDataMgr.MissionNetMsg.TalkMission(npc,mission,submitDto);
    }

    public bool FindToNpc(Mission mission,SubmitDto submitDto)
    {
        return false;
    }

    public void SubmitClear(SubmitDto submitDto) {
        TalkSubmitDto dto = submitDto as TalkSubmitDto;
        if(dto != null)
        {
            NpcInfoDto npcInfo = GetNpc(dto.submitNpc);
            if(npcInfo != null && npcInfo.npc is NpcMonster)
            {
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(npcInfo.id);
            }
        }
    }
}
