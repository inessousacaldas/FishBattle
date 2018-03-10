using UnityEngine;
using System.Collections;
using AppDto;
using System;

public class CollectionSubmitDelegate:BaseSubmitDelegate, ISubmitDelegate
{
    public CollectionSubmitDelegate(MissionDataMgr.MissionData model,string submitName)
        :base(model,submitName){

    }
    public bool FindToNpc(Mission mission,SubmitDto submitDto)
    {
        return false;
    }

    public void FinishSubmitDto(Mission mission,SubmitDto submitDto,Npc npc,int battleIndex)
    {
        PickItemSubmitInfoDto dto = submitDto as PickItemSubmitInfoDto;
        if(dto == null)
            return;
        bool isFinish = true;
        if(dto != null)
        {
            for(int i = 0;i < dto.pickNpcs.Count;i++)
            {
                if(!dto.pickNpcs[i].pick) {
                    isFinish = false;
                    break;
                }
            }
        }
        if(isFinish)
        {
            MissionDataMgr.MissionNetMsg.FinishMission(mission);
        }
        else {
            TipManager.AddTip("采集任务没有完成，不能将任务的Finis设置为真");
        }
    }

    public NpcInfoDto GetMissionNpcInfo(SubmitDto submitDto,bool isGetSubmitNpc)
    {
        PickItemSubmitInfoDto dto = submitDto as PickItemSubmitInfoDto;
        NpcInfoDto tNpcInfoDto = null;
        isGetSubmitNpc = submitDto.count >= submitDto.needCount || isGetSubmitNpc;
        if(dto != null) {
            for(int i = 0;i < dto.pickNpcs.Count;i++)
            {
                if(!dto.pickNpcs[i].pick)
                {
                    tNpcInfoDto = dto.pickNpcs[i].npcInfoDto;
                    break;
                }
            }
            if(isGetSubmitNpc)
            {
                if(!submitDto.auto && (dto.submitNpc.id == 0 || dto.submitNpc.npc == null)
                    && dto.dialog != null && dto.dialog.submitDialogSequence.Count > 0)
                    return dto.submitNpc;
                else
                {
                    return tNpcInfoDto;
                }
            }
            else
                return tNpcInfoDto;
        }
        return null;
    }

    public bool IsBattleDelay(SubmitDto submitDto,MissionSubmitStateNotify notify)
    {
        return false;
    }

    public void SubmitClear(SubmitDto submitDto)
    {
        PickItemSubmitInfoDto dto = submitDto as PickItemSubmitInfoDto;
        if(dto != null) {
            for(int i = 0;i < dto.pickNpcs.Count;i++)
            {
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(dto.pickNpcs[i].id);
            }
        }
    }

    public void SubmitConditionFinish(Mission mission,SubmitDto submitDto)
    {
        PickItemSubmitInfoDto dto = submitDto as PickItemSubmitInfoDto;
        if(dto != null && dto.count >= dto.needCount && dto.finish)
        {
            _model.WaitFindToMissionNpc(mission);
            //NpcInfoDto npcInfo = GetNpc(dto.submitNpc);
            //if(npcInfo != null && npcInfo.npc is NpcMonster)
                //WorldManager.Instance.GetNpcViewManager().RemoveNpc(npcInfo.id);
        }
    }

    public void SubmitConditionReach(Mission mission,SubmitDto submitDto)
    {
        PickItemSubmitInfoDto dto = submitDto as PickItemSubmitInfoDto;
        if(dto != null && dto.count >= dto.needCount && !dto.finish) {
            NpcInfoDto npcinfo = GetNpc(dto.submitNpc);
            WorldManager.Instance.GetNpcViewManager().AddNpcUnit(npcinfo);
            MissionDataMgr.MissionNetMsg.FinishMission(mission);
        }

    }

    public void SubmitConditionUpdate(Mission mission,SubmitDto submitDto)
    {
        PickItemSubmitInfoDto dto = submitDto as PickItemSubmitInfoDto;
        if(dto != null)
        {
            NpcInfoDto tNpcInfoDto = null;
            for(int i = 0;i < dto.pickNpcs.Count;i++)
            {
                if(!dto.pickNpcs[i].pick)
                {
                    tNpcInfoDto = dto.pickNpcs[i].npcInfoDto;
                    _model.WaitFindToMissionNpc(mission);
                    break;
                }
            }
        }
    }
}
