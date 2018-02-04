// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : fish
// Created  : 07/15/2017 16:35:59
// **********************************************************************

using System;
using AppDto;
using System.Collections.Generic;
using Asyn;

namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner missionDataMgr = new StaticDispose.StaticDelegateRunner(
            ()=> { var mgr = MissionDataMgr.DataMgr; } );
    }
}

public sealed partial class MissionDataMgr :AbstractAsynInit
{
    public override void StartAsynInit(Action<IAsynInit> onComplete){
        Action act = delegate ()
        {
            onComplete(this);
        };
        MissionNetMsg.ReqEnterMission(act);
    }

    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<MissionSubmitStateNotify>(_data.UpdateSubmitDto));
        _disposable.Add(NotifyListenerRegister.RegistListener<FindItemStateNotify>(_data.UpdateSubmitDto));
        _disposable.Add(NotifyListenerRegister.RegistListener<PickItemStateNotify>(_data.UpdateSubmitDto));
        _disposable.Add(NotifyListenerRegister.RegistListener<PlayerMissionNotify>(_data.UpdataPlayerMissionNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<MissionStatDto>(_data.UpdataMissionStatDtoNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<MissionCleanNotify>(
            e=> 
            {
                _data.DeletePlayerMissionDtoByMissionCleanNotify(e);
                FireData();
            }));
    }

    public void OnDispose(){
        _disposable.Dispose();
    }
    public PlayerMissionDto GetPlayerMissionDtoByMissionID(int missionID)
    {
        return _data.GetPlayerMissionDtoByMissionID(missionID);
    }

    #region 获得可接任务列表 新任务系统
    public SubmitDto GetSubmitDto(Mission mission,int submitIndex = -1) {
        return DataMgr._data.GetSubmitDto(mission,submitIndex);
    }
    public List<Mission> GetExistSubMissionMenuList() {
        return DataMgr._data.GetExistSubMissionMenuList();
    }

    public NpcInfoDto GetCompletionConditionNpc(Mission mission,int submitIndex = -1,bool getSubmitNpc = false) {
        return DataMgr._data.GetCompletionConditionNpc(mission,submitIndex,getSubmitNpc);
    }

    public void ClearLastFindMission() {
        DataMgr._data.ClearLastFindMission();
    }
    #endregion

    #region 接受任务
    /// <summary>
    /// 接受任务
    /// </summary>
    /// <param name="index"></param>
    public void AcceptMission(Mission mission)
    {
        DataMgr._data.Accpet(mission);
    }
    #endregion

    #region 提交多任务
    /// <summary>
    /// 提交多任务
    /// </summary>
    /// <param name="missionId">当前任务ID</param>
    /// <param name="nextMission">选择下一个任务ID</param>
    public void MultiAcceptMission(int missionId,int nextMission)
    {
        MissionNetMsg.MultiAcceptMission(missionId,nextMission);
    }
    #endregion

    #region
    public IEnumerable<NpcInfoDto> GetShowMonsterNpcInfoDtoList()
    {
        List <NpcInfoDto> tNpcInfoDtoList= _data.GetShowMonsterNpcInfoDtoList();
        return tNpcInfoDtoList;
    }
    #endregion
    
    #region 玩家移动到任务相关地点
    public void FindToMissionNpcByMission(Mission mission, bool isExistState, bool showTips = true)
    {
        _data.FindToMissionNpcByMission(mission, isExistState, showTips);
    }
    #endregion

    #region 主界面跟踪面板具体任务信息
    public IEnumerable<object> GetMissionCellData()
    {
        return _data.GetMissionCellData();
    }
    public bool BodyHasMission()
    {
        return _data.GetMissionListDto.ToList().Count > 0;
    }
    #endregion

    #region 使用道具或者提交道具的交任务方法
    public void FinishTargetSubmitDto(Mission mission,Npc npc,int submitIndex)
    {
        _data.FinishTargetSubmitDto(mission,npc,submitIndex);
    }
    #endregion


    /// <summary>
    /// 获得NPC身上的任务
    /// </summary>
    /// <param name="npc"></param>
    /// <returns></returns>
    public List<MissionOption> GetMissionOptionListByNpcInternal(Npc npc)
    {
        List<MissionOption> tMissionOptionList = new List<MissionOption>();
        _data.GetMissionOptionListByNpcInternal(npc).ForEach(m => {
            if(m.mission == _data.GetLastSubmitMission())
                tMissionOptionList.Add(m);
        });
        return tMissionOptionList;
    }

    public void TreasureMission(BagItemDto tCurBagItem) {
        _data.TreasureMission(BackpackDataMgr.DataMgr.GetTreasureMapItems(),tCurBagItem);
    }
    public MissionStatDto GetMissionStatDto(int missiontype) {
        return _data.GetMissionStatDto(missiontype);
    }

}
