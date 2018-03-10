using AppDto;
using UnityEngine;
using System.Collections.Generic;
using System;


//--------------------玩家采集任务功能类，因为暂时其他功能需要修复和优化，暂缓处理-----------------
public class MissionApplyItemSubmitDtoDelegate {
    private MissionDataMgr.MissionData mMissionData;
    private bool _applyMissionToTargetState = false;
    //	是否可穿越传送阵(true:不可 | ~)
    private bool _heroCharacterControllerEnable = true;
    public bool heroCharacterControllerEnable
    {
        get { return _heroCharacterControllerEnable; }
        set { _heroCharacterControllerEnable = value; }
    }


    public MissionApplyItemSubmitDtoDelegate(MissionDataMgr.MissionData _model)
    {
        mMissionData = _model;
    }
    public void WalkEndToFinishSubmitDto(Mission mission,bool isExistState,Npc tFindToTargetNpc)
    {
        if(isExistState)
        {
            ApplyItemSubmitDto tApplyItemSubmitDto = MissionHelper.GetSubmitDtoByMission(mission) as ApplyItemSubmitDto;
            MissionHelper.SubmitDtoType tSubmitDtoType = MissionHelper.GetSubmitDtoTypeByMission(mission);

            //	当前任务是使用物品类型
            if(tSubmitDtoType == MissionHelper.SubmitDtoType.ApplyItem)
            {
                Vector3 tPlayerPosition = WorldManager.Instance.GetHeroWorldPos();

                Vector2 tPlayerV2 = new Vector2(tPlayerPosition.x, tPlayerPosition.z);
                int tSceneID = MissionHelper.GetCurrentSceneID();

                //	当前场景是否使用物品场景判断
                if(tApplyItemSubmitDto.acceptScene.id == tSceneID)
                {
                    Vector2 tCenterV2 = new Vector2(tApplyItemSubmitDto.acceptScene.x, tApplyItemSubmitDto.acceptScene.z);
                    bool appltMissionToTargetState = IsApplyItemPlayerOnRadius(tPlayerV2, tCenterV2, tApplyItemSubmitDto.acceptRadius);

                    _applyMissionToTargetState = true;
                    if(appltMissionToTargetState)
                    {
                        ApplySubmitProcessing(mission);
                    }
                }
            }
            else if(tSubmitDtoType == MissionHelper.SubmitDtoType.GuildSpeak) {
                SpeakSubmitDto dto = MissionHelper.GetSubmitDtoByMission(mission) as SpeakSubmitDto;
                if(dto != null)
                {
                    MissionDataMgr.MissionNetMsg.FinishMission(mission);
                }
            }
            else if(tSubmitDtoType == MissionHelper.SubmitDtoType.Nothing)
            {
                GameDebuger.OrangeDebugLog("WARNING:@策划 未知SubmitDto类型");
            }
        }
    }

    #region 采集任务进行部分（注意：只针对采集任务）
    public void ApplySubmitProcessing(Mission mission)
    {
        if(!TeamDataMgr.DataMgr.IsLeader())
        {
            if(TeamDataMgr.DataMgr.HasTeam())
            {
                var val = WorldManager.Instance.GetModel().GetPlayerDto(ModelManager.Player.GetPlayerId());
                if(val != null)
                {
                    if(val.teamStatus != (int)TeamMemberDto.TeamMemberStatus.Away)
                    {
                        if(mission.type != (int)MissionType.MissionTypeEnum.Ghost)
                        {
                            TipManager.AddTip("正在组队跟随状态中，不能进行此操作！");
                        }
                        return;
                    }
                }
            }
        }
        SubmitDto tSubmitDto = MissionHelper.GetSubmitDtoByMission(mission);
        ApplyItemSubmitDto tApplyItemSubmitDto = tSubmitDto as ApplyItemSubmitDto;
        Npc tFindToTargetNpc = mMissionData.GetNpcByNpcInfoDto(mMissionData.GetBindMissionNpcInfoDtoBySubmit(tSubmitDto));
        if(tFindToTargetNpc == null)
            return;

        //当改NPC是虚拟NPC是转换为具体NPC
        tFindToTargetNpc = MissionHelper.NpcVirturlToEntity(tFindToTargetNpc);
        if(tApplyItemSubmitDto.item == null)
        {
            MissionDataMgr.MissionNetMsg.FinishMission(mission);
        }
        if(tSubmitDto != null)// && tApplyItemSubmitDto.item != null)
        {
            //弹出使用物品tips
            AppMissionItem tAppMissionItem = tApplyItemSubmitDto.item as AppMissionItem;
            if(tAppMissionItem == null)
            {
                GameDebuger.LogError("任务 id = " + mission.id + " ,物品物品设置错误: " + tApplyItemSubmitDto.itemId);
            }
            MainUIDataMgr.MainUIViewLogic.OpenMissionProps(tAppMissionItem,() =>
            {
                //	马上同步服务器一次
                WorldManager.Instance.SyncWithServer();
                int tSceneID = MissionHelper.GetCurrentSceneID();
                if(tFindToTargetNpc.sceneId == tSceneID)
                {
                    //var tDesStr = tAppMissionItem.usedTip.WrapColor(ColorConstantV3.Color_White_Str);
                    ProxyProgressBar.ShowProgressBar(tAppMissionItem.icon,tAppMissionItem.usedTip,null,"__PopupUseMissionProps",delegate
                    {
                        MissionDataMgr.MissionNetMsg.MissionAppleItem(mission.id,tSubmitDto.index);
                        //MissionDataMgr.DataMgr.FinishTargetSubmitDto(mission,tFindToTargetNpc,tSubmitDto.index);
                        //  【任务】使用道具的任务，在道具使用完成后，需要弹出一个飘字（客户端）
                        //  http://oa.cilugame.com/redmine/issues/9116
                        TipManager.AddTip(tAppMissionItem.finishTip);
                    },3f);
                }
                else
                {
                    //mMissionData.FindToMissionNpcByMission(mission,true);
                    mMissionData.WaitFindToMissionNpc(mission);
                }
            },true);
        }
    }
    #endregion


    public Mission GetCurrentMissionByMissionID(int missionID)
    {
        Mission tMission = null;
        PlayerMissionDto tPlayerMissionDto = mMissionData.GetPlayerMissionDtoByMissionID(missionID);
        if(tPlayerMissionDto != null)
        {
            tMission = tPlayerMissionDto.mission;
        }

        return tMission;
    }

    #region 玩家采集任务玩家移动回调,暂时功能
    private Vector3 _lastPlayerPos = Vector3.zero;
    //	维护一个采集任务Dic<missionID, ApplyItemSubmitDto>
    private Dictionary<int, ApplyItemSubmitDto> _applyItemSubmitDtoDic = new Dictionary<int, ApplyItemSubmitDto>();
    public bool __isPlayerViewCallback = false;

    public void ApplyMissionPlayerInRadius()
    {
        var heroView = WorldManager.Instance.GetHeroView();
        if(heroView == null)
        {
            return;
        }
        Vector3 tPlayerPosition = heroView.transform.position;
        if(_lastPlayerPos == tPlayerPosition)
        {
            return;
        }
        _lastPlayerPos = tPlayerPosition;
        Vector2 tPlayerV2 = new Vector2(tPlayerPosition.x,tPlayerPosition.z);
        int tSceneID = MissionHelper.GetCurrentSceneID();
        foreach(int missionID in _applyItemSubmitDtoDic.Keys)
        {
            ApplyItemSubmitDto tApplyItemSubmitDto = _applyItemSubmitDtoDic[missionID];
            if(tApplyItemSubmitDto.acceptScene.id != tSceneID)
            {
                continue;
            }
            Vector2 tCenterV2 = new Vector2(tApplyItemSubmitDto.acceptScene.x,tApplyItemSubmitDto.acceptScene.z);
            bool appltMissionToTargetState = IsApplyItemPlayerOnRadius(tPlayerV2,tCenterV2,tApplyItemSubmitDto.acceptRadius);
            if(_applyMissionToTargetState != appltMissionToTargetState)
            {
                _applyMissionToTargetState = appltMissionToTargetState;
                if(appltMissionToTargetState)
                {
                    ApplySubmitProcessing(GetCurrentMissionByMissionID(missionID));
                    break;
                }
                else
                {
                    MainUIDataMgr.MainUIViewLogic.OpenMissionProps(null,null,true);
                }
            }
        }
    }

    // 玩家采集任务玩家移动回调
    public void ResetApplyMission(List<Mission> tSubMissionMenuList,int tSceneID)
    {
        _applyItemSubmitDtoDic.Clear();
        //	设置收集任务回调
        for(int i = 0, len = tSubMissionMenuList.Count;i < len;i++)
        {
            Mission tMission = tSubMissionMenuList[i];
            SubmitDto tSubmitDto = MissionHelper.GetSubmitDtoByMission(tMission);
            if(MissionHelper.IsApplyItem(tSubmitDto) && tSubmitDto.count < tSubmitDto.needCount)
            {
                if(!_applyItemSubmitDtoDic.ContainsKey(tMission.id))
                {
                    if(!_applyItemSubmitDtoDic.ContainsKey(tMission.id))
                    {
                        _applyItemSubmitDtoDic.Add(tMission.id,tSubmitDto as ApplyItemSubmitDto);
                    }
                }
            }
        }
        //	当当前有某个任务的目标是收集任务，设置人物角色移动回调到MissionDataModel
        var heroView = WorldManager.Instance.GetHeroView();
        if(heroView != null)
        {
            //MainUIViewController.Instance.OnClickPopupUseMissionPropsBtn(null, null, true);
            __isPlayerViewCallback = _applyItemSubmitDtoDic.Count > 0;
            if(_applyItemSubmitDtoDic.Count > 0)
            {
                if(heroView.checkMissioinCallback == null)
                {
                    heroView.checkMissioinCallback = ApplyMissionPlayerInRadius;
                }
                ApplyMissionPlayerInRadius();
            }
            else
            {
                if(heroView.checkMissioinCallback != null)
                {
                    heroView.checkMissioinCallback = null;
                }
            }
            if(WorldManager.Instance.GetView().InitPlayerMission != null)
            {
                WorldManager.Instance.GetView().InitPlayerMission = null;
            }
        }
        else
        {
            //不停的回调，知道hero不为空
            //GetSubMissionMenuListInMainUIExpand
            WorldManager.Instance.GetView().InitPlayerMission = mMissionData.GetSubMissionMenuListInMainUIExpand;
        }
    }

    public bool IsApplyItemPlayerOnRadius(Vector2 v1,Vector2 v2,int r)
    {
        r--;
        float tDistanceInRadius = Vector2.Distance(v1, v2);
        int tAcceptRadius = r < 1 ? 1 : r;

        return tDistanceInRadius <= tAcceptRadius;
    }
    #endregion
}