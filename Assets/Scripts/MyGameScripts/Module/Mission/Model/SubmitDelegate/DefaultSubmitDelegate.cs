using UnityEngine;
using System.Collections;
using AppDto;

public class DefaultSubmitDelegate : BaseSubmitDelegate,ISubmitDelegate {
    public DefaultSubmitDelegate(MissionDataMgr.MissionData model,string submitName):base(model,submitName){ }

    public NpcInfoDto GetMissionNpcInfo(SubmitDto submitDto,bool isGetSubmitNpc) {
        return null;
    }

    public bool IsBattleDelay(SubmitDto submitDto,MissionSubmitStateNotify notify) {
        return true;
    }

    public void SubmitConditionUpdate(Mission mission,SubmitDto submitDto) {
        SubmitDto dto = submitDto as SubmitDto;
        if(dto != null && dto.count < dto.needCount && !dto.finish) {
        }
    }

    public void SubmitConditionReach(Mission mission,SubmitDto submitDto) {
        SubmitDto dto = submitDto as SubmitDto;
        if(dto != null && dto.count >= dto.needCount && !dto.finish) {
        }
    }

    public void SubmitConditionFinish(Mission mission,SubmitDto submitDto) {
        SubmitDto dto = submitDto as SubmitDto;
        if(dto != null && dto.count >= dto.needCount && dto.finish) {
        }
    }

    public void FinishSubmitDto(Mission mission,SubmitDto submitDto,Npc npc,int battleIndex) {
        GameDebuger.LogError("FinishSubmitDto 未实现");
    }

    public bool FindToNpc(Mission mission,SubmitDto submitDto) {
        return false;
    }

    public void SubmitClear(SubmitDto submitDto) {
        SubmitDto dto = submitDto as SubmitDto;
        if(dto != null) {

        }
    }
}
