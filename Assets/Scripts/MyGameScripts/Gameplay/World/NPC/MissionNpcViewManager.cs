using System.Collections.Generic;
using AppDto;

public partial class NpcViewManager
{
    public NpcMissionMark GetNpcMissionMarkByNpcInternal(Npc npc,int submitIndex) {
        NpcMissionMark tNpcMissionMark = NpcMissionMark.Nothing;
        //	已接取任务筛选
        List<Mission> tCurMissionList = MissionDataMgr.DataMgr.GetExistSubMissionMenuList();
        for(int i = 0, len = tCurMissionList.Count;i < len;i++) {
            Mission tMission = tCurMissionList[i];
            bool tIsTheSameNpc = IsTheSameSignNpcByMission(tMission,npc.id,submitIndex);
            if(tIsTheSameNpc == false)
                continue;
            //	已接任务获取SubmitDto（提交项）
            SubmitDto tSubmitDto = null;
            if(submitIndex < 0)
            {
                tSubmitDto = MissionDataMgr.DataMgr.GetSubmitDto(tMission);
            }
            else {
                tSubmitDto = MissionDataMgr.DataMgr.GetSubmitDto(tMission,submitIndex);
            }
            //是否是可提交状态
            bool tSubmitSta = tSubmitDto == null?true:(tSubmitDto.count >= tSubmitDto.needCount && !tSubmitDto.finish)
                ||(MissionHelper.IsTalkItem(tSubmitDto) && !tSubmitDto.auto);
            if(tSubmitSta) {
                tNpcMissionMark = NpcMissionMark.Submit;
                if(MissionHelper.IsMainMissionType(tMission)) {
                    tNpcMissionMark = NpcMissionMark.MainSubmit;
                    //	若是可提交的主线状态项，已经到最顶层，直接退出该层遍历
                    return tNpcMissionMark;
                }
                break;
            }

            if(tNpcMissionMark == NpcMissionMark.Nothing || tNpcMissionMark == NpcMissionMark.Accept || tNpcMissionMark == NpcMissionMark.Process) {
                tNpcMissionMark = NpcMissionMark.Nothing;

                if(MissionHelper.IsShowMonster(tSubmitDto))
                {
                    Npc tAccNpc = GetMissionNpcByMission(tMission,true);
                    if(tAccNpc != null && npc.id == tAccNpc.id)
                    {
                        tNpcMissionMark = NpcMissionMark.Battle;
                    }
                }
                else if(MissionHelper.IsMainMissionType(tMission))
                {
                    tNpcMissionMark = NpcMissionMark.MainProcess;
                }
                else if(MissionHelper.IsTalkItem(tSubmitDto)) {
                    tNpcMissionMark = NpcMissionMark.Submit;
                }
            }
        }

        if(tNpcMissionMark != NpcMissionMark.Nothing)
            return tNpcMissionMark;

        List<MissionOption> tMissionOptionList =MissionDataMgr.DataMgr.GetMissionOptionListByNpcInternal(npc);
        for(int i = 0,len = tMissionOptionList.Count;i < len;i++) {
            MissionOption tMissionOption = tMissionOptionList[i];
            if(tMissionOption.isExis == true)
                continue;
            Mission tMission = tMissionOption.mission;
            bool tOccState = MissionHelper.IsFactionMissionType(tMission)
                            || MissionHelper.IsBuffyMissionType(tMission);

            bool tOpenState = FunctionOpenHelper.isFuncOpen(tMission.missionType.functionOpenId,false);
            if(tOccState && tOpenState || !tOccState) {
                if(tNpcMissionMark == NpcMissionMark.Nothing || tNpcMissionMark == NpcMissionMark.Process) {
                    tNpcMissionMark = NpcMissionMark.Accept;
                    if(MissionHelper.IsMainMissionType(tMissionOption.mission))
                    {
                        tNpcMissionMark = NpcMissionMark.MainAccept;
                    }
                    if(MissionHelper.IsFactionMissionType(tMission)) {
                        tNpcMissionMark = NpcMissionMark.Nothing;
                    }
                    break;
                }
            }
        }
        return tNpcMissionMark;
    }

    #region 获取任务接取 Npc \ 目标（提交）Npc \ 任务提交Npc
    /// <summary>
    /// 获取任务接取 Npc \ 目标（提交）Npc \ 任务提交Npc -- Gets the mission all npc by mission.
    /// </summary>
    /// <param name="mission"></param>
    /// <param name="npcID"></param>
    /// <param name="submitIndex"></param>
    /// <returns></returns>
    private bool IsTheSameSignNpcByMission(Mission mission,int npcID,int submitIndex) {
        SubmitDto tSubmitDto = null;
        if(submitIndex < 0)
        {
            tSubmitDto = MissionDataMgr.DataMgr.GetSubmitDto(mission);
        }
        else {
            tSubmitDto = MissionDataMgr.DataMgr.GetSubmitDto(mission,submitIndex);
        }
        int sceneId = WorldManager.Instance.GetModel().GetSceneId();
        if(tSubmitDto != null && MissionHelper.IsShowMonster(tSubmitDto)) {
            NpcInfoDto tShowMonsterNpc = MissionDataMgr.DataMgr.GetCompletionConditionNpc(mission,submitIndex,false);
            if(tShowMonsterNpc.id == npcID && tShowMonsterNpc.sceneId == sceneId) {
                return true;
            }
        }
        //	提交项Npc
        NpcInfoDto submitDtoNpc = MissionDataMgr.DataMgr.GetCompletionConditionNpc(mission,submitIndex,true);
        if(submitDtoNpc != null && submitDtoNpc.id == npcID && submitDtoNpc.sceneId == sceneId) {
            return true;
        }
        return false;
    }

    #endregion

    #region  #region 通过mission获取任务Npc(当true：返回已接任务寻路NPC， false：返回接取NPC)
    public Npc GetMissionNpcByMission(Mission mission,bool existSta) {
        Npc tMissionNpc = null;
        if(existSta)
        {
            NpcInfoDto tNpcInfoDto = MissionDataMgr.DataMgr.GetCompletionConditionNpc(mission);
            tMissionNpc = MissionHelper.GetNpcByNpcInfoDto(tNpcInfoDto);
        }
        else {
            tMissionNpc = mission.missionType.acceptNpc;
            tMissionNpc = tMissionNpc == null ? mission.acceptNpc : tMissionNpc;
            //	当改NPC是虚拟NPC是转换为具体NPC
            tMissionNpc = MissionHelper.NpcVirturlToEntity(tMissionNpc);
        }
        return tMissionNpc;
    }
    #endregion
}
