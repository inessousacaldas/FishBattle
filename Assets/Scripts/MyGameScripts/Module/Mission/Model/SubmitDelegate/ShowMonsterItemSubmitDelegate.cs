using UnityEngine;
using System.Collections;
using AppDto;
using System;

public class ShowMonsterItemSubmitDelegate : BaseSubmitDelegate, ISubmitDelegate
{
    public ShowMonsterItemSubmitDelegate(MissionDataMgr.MissionData model, string submitName) : base(model, submitName)
    {
        
    }

    public bool FindToNpc(Mission mission, SubmitDto submitDto)
    {
        return false;
    }

    public void FinishSubmitDto(Mission mission, SubmitDto submitDto, Npc npc, int battleIndex)
    {
        ShowMonsterItemSubmitDto dto = submitDto as ShowMonsterItemSubmitDto;
        if(dto == null)
            return;

        if(dto.count >= dto.needCount)
        {
            MissionDataMgr.MissionNetMsg.FinishMission(mission);
        }
        else
        {
            //	明雷类型任务，confirm是否在战斗前出现选项 请求战斗
            bool isRequestBattle = true;
            if(MissionHelper.IsBuffyMissionType(mission))
            {
                GhostMissionDelegate tGhostMissionDelegate = new GhostMissionDelegate(_model,(int)MissionType.MissionTypeEnum.Ghost);
                isRequestBattle = tGhostMissionDelegate.MissionTip(false,false);
                if(!isRequestBattle)
                {
                    MissionDataMgr.MissionNetMsg.BattleShowMonster(mission,npc,battleIndex,null);
                }
                return;
            }

            if(isRequestBattle)
            {
                MissionDataMgr.MissionNetMsg.BattleShowMonster(mission,npc,battleIndex,null);
            }
        }
    }

    public NpcInfoDto GetMissionNpcInfo(SubmitDto submitDto, bool isGetSubmitNpc)
    {
        ShowMonsterItemSubmitDto dto = submitDto as ShowMonsterItemSubmitDto;
        if(dto != null)
        {
            isGetSubmitNpc = submitDto.count >= submitDto.needCount || isGetSubmitNpc;
            if(dto.dialog == null)
            {
                TipManager.AddTip(string.Format("{0}表格错误，对话数据空 -> MissionDialogID:{1}","[c30000]",dto.dialogId));
            }

            if(isGetSubmitNpc)
            {
                if(!submitDto.auto && (dto.submitNpc.id == 0 || dto.submitNpc.npc == null)
                    && dto.dialog != null && dto.dialog.submitDialogSequence.Count > 0)
                    return dto.acceptNpc;
                else
                    return dto.submitNpc;
            }
            else
                return dto.acceptNpc;
        }
        return null;
    }

    public bool IsBattleDelay(SubmitDto submitDto, MissionSubmitStateNotify notify)
    {
        return true;
    }

    public void SubmitClear(SubmitDto submitDto)
    {
        ShowMonsterItemSubmitDto dto = submitDto as ShowMonsterItemSubmitDto;
        if(dto != null)
        {
            NpcInfoDto npcInfo = GetNpc(dto.acceptNpc);
            if(npcInfo != null && npcInfo.npc is NpcMonster)
            {
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(npcInfo.id);
            }
            npcInfo = GetNpc(dto.submitNpc);
            if(npcInfo != null && npcInfo.npc is NpcMonster)
            {
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(npcInfo.id);
            }
        }
    }

    public void SubmitConditionFinish(Mission mission, SubmitDto submitDto)
    {
        ShowMonsterItemSubmitDto dto = submitDto as ShowMonsterItemSubmitDto;
        if(dto != null && dto.count >= dto.needCount && dto.finish)
        {
            NpcInfoDto npcInfo = GetNpc(dto.acceptNpc);
            if(npcInfo != null && npcInfo.npc is NpcMonster)
            {
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(npcInfo.id);
            }
            npcInfo = GetNpc(dto.submitNpc);
            if(npcInfo != null && npcInfo.npc is NpcMonster)
            {
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(npcInfo.id);
            }
        }
    }

    public void SubmitConditionReach(Mission mission, SubmitDto submitDto)
    {
        ShowMonsterItemSubmitDto dto = submitDto as ShowMonsterItemSubmitDto;
        if(dto != null && dto.count >= dto.needCount && !dto.finish) {
            NpcInfoDto npcInfo = GetNpc(dto.acceptNpc);
            if(dto.submitNpc.id == 0 || dto.submitNpc == null)
            {
                if(dto.dialog.submitDialogSequence.Count > 0)
                {
                    //打开聊天框
                    OpenNpcDialogue(npcInfo.npc);
                }
                else
                {
                    if(npcInfo != null && npcInfo.npc is NpcMonster)
                    {
                        WorldManager.Instance.GetNpcViewManager().RemoveNpc(npcInfo.id);
                        MissionDataMgr.MissionNetMsg.FinishMission(mission);
                    }
                }
            }
            else {
                if(npcInfo != null && npcInfo.npc is NpcMonster)
                {
                    WorldManager.Instance.GetNpcViewManager().RemoveNpc(npcInfo.id);
                }
                npcInfo = GetNpc(dto.submitNpc);
                WorldManager.Instance.GetNpcViewManager().AddNpcUnit(npcInfo);
                _model.WaitFindToMissionNpc(mission);
            }
        }
    }

    public void SubmitConditionUpdate(Mission mission, SubmitDto submitDto)
    {
        ShowMonsterItemSubmitDto dto = submitDto as ShowMonsterItemSubmitDto;
        if(dto != null && dto.count < dto.needCount && !dto.finish) {
            NpcInfoDto npcInfo = GetNpc(dto.acceptNpc);
            WorldManager.Instance.GetNpcViewManager().AddNpcUnit(npcInfo);
        }
    }

    public void OpenNpcDialogue(Npc npc,bool setZeroPos = false)
    {
        SceneNpcDto npcStateDto = new SceneNpcDto();
        if(!setZeroPos)
        {
            npcStateDto.x = npc.x;
            npcStateDto.z = npc.z;
        }
        else
        {
            npcStateDto.x = 0;
            npcStateDto.z = 0;
        }
        npcStateDto.npcId = npc.id;
        npcStateDto.id = npc.id;
        BaseNpcInfo baseNpcInfo = new BaseNpcInfo();
        baseNpcInfo.npcStateDto = npcStateDto;
        baseNpcInfo.AdjustAppearance();
        ProxyNpcDialogueView.Open(baseNpcInfo);
    }
}
