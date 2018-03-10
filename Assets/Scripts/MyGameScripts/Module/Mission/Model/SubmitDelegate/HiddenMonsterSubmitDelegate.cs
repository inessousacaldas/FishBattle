using UnityEngine;
using System.Collections;
using AppDto;

public class HiddenMonsterSubmitDelegate : BaseSubmitDelegate,ISubmitDelegate {
    public HiddenMonsterSubmitDelegate(MissionDataMgr.MissionData model,string submitName) : base(model,submitName)
    { }

    public void SubmitConditionUpdate(Mission mission,SubmitDto submitDto) {
        if((_model.GetLastFindMission() != null
         && _model.GetLastFindMission() != mission))
        {
            _model.WaitFindToMissionNpc(_model.GetLastFindMission());
        }
    }

    public void SubmitConditionReach(Mission mission,SubmitDto submitDto) {
        HiddenMonsterSubmitDto dto = submitDto as HiddenMonsterSubmitDto;
        if(dto != null && dto.count >= dto.needCount && !dto.finish) {
            NpcInfoDto npcinfo = GetNpc(dto.submitNpc);
            WorldManager.Instance.GetNpcViewManager().AddNpcUnit(npcinfo);
            ModelManager.Player.StopAutoFram();
        }

        if(dto != null && dto.count >= dto.needCount && dto.finish && !mission.autoSubmit)
        {
            if((_model.GetLastFindMission() != null
                && _model.GetLastFindMission() != mission))
            {
                _model.WaitFindToMissionNpc(_model.GetLastFindMission());
                return;
            }
            _model.FindToMissionNpc(mission);
        }


    }

    public void SubmitConditionFinish(Mission mission,SubmitDto submitDto)
    {
        HiddenMonsterSubmitDto dto = submitDto as HiddenMonsterSubmitDto;
        if(dto != null && dto.count >= dto.needCount && dto.finish)
        {
            NpcInfoDto npcInfo = GetNpc(dto.submitNpc);
            if(npcInfo != null && npcInfo.npc is NpcMonster)
            {
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(npcInfo.id);
            }
        }
        //如果有上个任务，那就执行上一个任务
        if((_model.GetLastFindMission() != null
         && _model.GetLastFindMission() != mission))
        {
            _model.WaitFindToMissionNpc(_model.GetLastFindMission());
            return;
        }
        //如果有上个任务为空，或者上个任务就这个任务，那就执行这个任务呀
        _model.FindToMissionNpc(mission);
    }

    public void SubmitClear(SubmitDto submitDto) {
        HiddenMonsterSubmitDto dto = submitDto as HiddenMonsterSubmitDto;
        if(dto != null) {
            NpcInfoDto npcInfo = GetNpc(dto.submitNpc);
            if(npcInfo != null && npcInfo.npc is NpcMonster)
            {
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(npcInfo.id);
            }
        }
    }

    public NpcInfoDto GetMissionNpcInfo(SubmitDto submitDto,bool isGetSubmitNpc) {
        HiddenMonsterSubmitDto dto = submitDto as HiddenMonsterSubmitDto;
        if(dto != null) {
            isGetSubmitNpc = submitDto.count >= submitDto.needCount || isGetSubmitNpc;
            Npc npc = new Npc();
            npc.id = -1;
            if(dto.acceptScene.sceneMap == null)
            {
                GameDebuger.LogError("HiddenMonsterSubmitDto acceptScene 没有景数据,场景Id = " + dto.acceptScene.id);
                npc.name = "数据出错";
            }
            else {
                npc.name = dto.acceptScene.sceneMap.name;
            }
            npc.sceneId = dto.acceptScene.id;
            npc.x = dto.acceptScene.x;
            npc.z = dto.acceptScene.z;
            return isGetSubmitNpc ? dto.submitNpc : MissionHelper.GetNpcInfoDtoByNpc(npc);
        }
        return null;
    }

    public void FinishSubmitDto(Mission mission,SubmitDto submitDto,Npc npc,int battleIndex) {

    }

    public bool FindToNpc(Mission mission,SubmitDto submitDto) {
        HiddenMonsterSubmitDto dto = submitDto as HiddenMonsterSubmitDto;
        if(dto != null) {
            if(dto.acceptScene.id == WorldManager.Instance.GetModel().GetSceneId())
            {
                ModelManager.Player.StartAutoFram();
            }
            else {
                WorldManager.Instance.Enter(dto.acceptScene.id,false,true);
            }
            return true;
        }
        return false;
    }

    public bool IsBattleDelay(SubmitDto submitDto,MissionSubmitStateNotify notify) {
        return true;
    }
}
