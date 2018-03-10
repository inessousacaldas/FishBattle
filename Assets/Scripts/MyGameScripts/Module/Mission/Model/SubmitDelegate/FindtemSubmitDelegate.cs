using UnityEngine;
using System.Collections;
using AppDto;
using System;

/// <summary>
/// 多人交谈获得任务道具提交
/// </summary>
public class FindtemSubmitDelegate:BaseSubmitDelegate, ISubmitDelegate
{
    public FindtemSubmitDelegate(MissionDataMgr.MissionData model,string submitName) : base(model,submitName) {

    }
    public bool FindToNpc(Mission mission,SubmitDto submitDto)
    {
        return false;
    }


    ///
    public void FinishSubmitDto(Mission mission,SubmitDto submitDto,Npc npc,int battleIndex)
    {
        //现在不根据talk来判断的，根据count来判断
        FindtemSubmitInfoDto dto = submitDto as FindtemSubmitInfoDto;
        //if(dto.count >= dto.needCount && dto.talk) {
        //    MissionDataMgr.MissionNetMsg.FinishMission(mission);
        //    return;
        //}
        MissionDataMgr.MissionNetMsg.TalkFindItemMission(npc,mission,submitDto);
    }

    public NpcInfoDto GetMissionNpcInfo(SubmitDto submitDto,bool isGetSubmitNpc)
    {
        FindtemSubmitInfoDto dto = submitDto as FindtemSubmitInfoDto;
        if(dto != null) {
            isGetSubmitNpc = submitDto.count >= submitDto.needCount || isGetSubmitNpc;
            if(dto.dialog == null) {
                TipManager.AddTip(string.Format("{0}表格错误，对话数据空 -> MissionDialogID:{1}","[c30000]",dto.dialogId));
            }
            if(isGetSubmitNpc)
            {
                if(!submitDto.auto && (dto.submitNpc.id == 0 || dto.submitNpc.npc == null)
                    && dto.dialog != null && dto.dialog.submitDialogSequence.Count > 0)
                    return dto.submitNpc;
                else
                    return dto.acceptNpc;
            }
            else
                return dto.acceptNpc;
        }
        return null;
    }

    public bool IsBattleDelay(SubmitDto submitDto,MissionSubmitStateNotify notify)
    {
        return false;
    }

    public void SubmitClear(SubmitDto submitDto)
    {
        FindtemSubmitInfoDto dto = submitDto as FindtemSubmitInfoDto;
        if(dto != null) {
            NpcInfoDto npcinfo = GetNpc(dto.acceptNpc);
            if(npcinfo != null && npcinfo.npc is NpcMonster) {
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(npcinfo.id);
            }
            npcinfo = GetNpc(dto.submitNpc);
            if(npcinfo != null && npcinfo.npc is NpcMonster) {
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(npcinfo.id);
            }
        }
    }

    public void SubmitConditionFinish(Mission mission,SubmitDto submitDto)
    {
        //FindtemSubmitInfoDto dto = submitDto as FindtemSubmitInfoDto;
        //dto.talk = true;
        //GameDebuger.LogError(dto.submitNpc.name + "," + dto.acceptNpc.name);
        _model.WaitFindToMissionNpc(mission);
    }

    public void SubmitConditionReach(Mission mission,SubmitDto submitDto)
    {
        //FindtemSubmitInfoDto dto = submitDto as FindtemSubmitInfoDto;
        SubmitDto tsubmitDto = _model.GetSubmitDto(mission);
        if(tsubmitDto == null)
        {
            // 所有条件完成,表示该任务可提交（非目标提交）
            MissionDataMgr.MissionNetMsg.FinishMission(mission);
            return;
        }
    }

    public void SubmitConditionUpdate(Mission mission,SubmitDto submitDto)
    {
        
    }
}
