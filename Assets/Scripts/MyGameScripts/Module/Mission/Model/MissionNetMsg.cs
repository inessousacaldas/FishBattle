// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : fish
// Created  : 07/15/2017 16:35:59
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;
using GamePlot;

public sealed partial class MissionDataMgr
{
    public static class MissionNetMsg
    {
        #region EnterMission
        /// <summary>
        /// 进入游戏获取任务信息
        /// </summary>
        public static void ReqEnterMission(Action completeAction)
        {
            GameUtil.GeneralReq<PlayerMissionListDto>(Services.Mission_Enter(), dto =>
            {
                DataMgr._data.UpdateMissionListDto(dto);
                GameUtil.SafeRun(completeAction);
            });
        }
        
        #endregion

        #region AcceptMission

        /// <summary>
        /// 接受任务
        /// </summary>
        /// <param name="missionId"></param>
        public static void ReqAcceptMission(int missionId)
        {
            GameUtil.GeneralReq<PlayerMissionDto>(Services.Mission_Accept(missionId), RespAcceptMisstion);
        }

        #region 接受公会任务
        public static void AcceptMissionGuild()
        {
            GameUtil.GeneralReq(Services.Mission_AcceptGuild());
        }
        #endregion


        private static void RespAcceptMisstion(PlayerMissionDto dto)
        {
            DataMgr._data.AcceptMission(dto);
            FireData();
        }

        #endregion


        #region 多分支任务提交
        /// <summary>
        /// 多分支任务提交
        /// </summary>
        /// <param name="missionId">完成这次任务的ID</param>
        /// <param name="nextMissionId">接受下个任务的ID</param>
        public static void MultiAcceptMission(int missionId,int nextMissionId)
        {
            GameUtil.GeneralReq<PlayerMissionDto>(Services.Mission_MultiAccept(missionId,nextMissionId),RespAcceptMisstion);
        }
        #endregion

        #region DrpMission
        public static void ReqDropMission(int missionId)
        {
            GameUtil.GeneralReq(Services.Mission_Drop(missionId), resp => { RespDropMission(resp,missionId); });
        }
        private static void RespDropMission(GeneralResponse resp,int missionId)
        {
            //DataMgr._data.RemovePlayerMissionDtoByMissionID(missionId,RemoveMissionType.Drop);
            DataMgr._data.RemovePlayerMissionWhenDrop(missionId);
            TipManager.AddTip("成功放弃该任务");
            FireData();
        }
        #endregion

        public static void SubmitMission(int missionId)
        {
            DataMgr._data.RemovePlayerMissionWhenDrop(missionId);
            GameUtil.GeneralReq(Services.Mission_Submit(missionId));
            FireData();
        }

        #region 对话任务需要先执行对话接口
        public static void TalkMission(Npc npc,Mission mission,SubmitDto submitDto)
        {
            if(GamePlotManager.Instance.TriggerPlot((int)Plot.PlotTriggerType.TalkToMissionSubmitNpc,mission.id))
            {
                GamePlotManager.Instance.OnFinishPlot += (plot) =>
                {
                    GameUtil.GeneralReq<GeneralResponse>(Services.Mission_Talk(npc.id,mission.id),delegate (GeneralResponse e)
                    {
                        if(submitDto.auto)
                        {
                            ProxyNpcDialogueView.Close();
                        }
                        else
                        {
                            FinishMission(mission);
                        }
                        if(MissionHelper.IsMonsterNpc(npc))
                        {
                            WorldManager.Instance.GetNpcViewManager().RemoveNpc(npc.id);
                        }
                    },
                    (e) =>
                    {
                        TipManager.AddTip(e.message);
                    });
                };
            }
            else {
                GameUtil.GeneralReq<GeneralResponse>(Services.Mission_Talk(npc.id,mission.id),delegate (GeneralResponse e)
                {
                    if(submitDto.auto)
                    {
                        ProxyNpcDialogueView.Close();
                    }
                    else
                    {
                        FinishMission(mission);
                    }
                    if(MissionHelper.IsMonsterNpc(npc))
                    {
                        WorldManager.Instance.GetNpcViewManager().RemoveNpc(npc.id);
                    }
                },
                (e) =>
                {
                    TipManager.AddTip(e.message);
                });
            }
        }
        #endregion

        #region 提交任务使用道具
        public static void MissionAppleItem(int missionid,int index) {
            GameUtil.GeneralReq(Services.Mission_ApplyItem(missionid,index));
        }


        #endregion

        #region 手动提交某任务目标至完成状态，有可能触发奖励（这里指任务中的某个目标）
        public static void FinishMission(Mission mission,object obj = null)
        {
            int missionID=mission.id;
            bool doublePointTips=MissionHelper.IsMainMissionType(mission);
            if(obj == null)
            {
                GameUtil.GeneralReq<GeneralResponse>(Services.Mission_Finish(mission.id),delegate (GeneralResponse e)
                {
                    WorldManager.Instance.GetNpcViewManager().RefinishMissionFlag();
                },
                (e) =>
                {
                    //TipManager.AddTip(e.message);
                });
            }
            else
            {
                if(obj is List<BagItemDto>)
                {
                    List<BagItemDto> tBagItemDto = obj as List<BagItemDto>;
                    string submitItems = string.Empty;
                    for(int i = 0; i< tBagItemDto.Count;i++)
                    {
                        submitItems += tBagItemDto[i].uniqueId + ",";
                    }
                    submitItems = submitItems.Substring(0,submitItems.LastIndexOf(","));
                    GameUtil.GeneralReq<GeneralResponse>(Services.Mission_FinishWithItems(missionID,submitItems),delegate (GeneralResponse e)
                    {
                        WorldManager.Instance.GetNpcViewManager().RefinishMissionFlag();

                    },(e) =>
                    {
                        TipManager.AddTip(e.message);
                    });
                }
            }
        }
        #endregion


        #region 手动提交某任务目标至完成状态并且提交任务
        public static void FinishSubmitMission(Mission mission,object obj = null)
        {
            int missionID=mission.id;
            bool doublePointTips=MissionHelper.IsMainMissionType(mission);
            if(obj == null)
            {
                GameUtil.GeneralReq<GeneralResponse>(Services.Mission_Finish(mission.id),delegate (GeneralResponse e)
                {
                    WorldManager.Instance.GetNpcViewManager().RefinishMissionFlag();
                    MissionNetMsg.SubmitMission(mission.id);
                },
                (e) =>
                {
                    //TipManager.AddTip(e.message);
                });
            }
            else
            {
                if(obj is List<BagItemDto>)
                {
                    List<BagItemDto> tBagItemDto = obj as List<BagItemDto>;
                    string submitItems = string.Empty;
                    for(int i = 0;i < tBagItemDto.Count;i++)
                    {
                        submitItems += tBagItemDto[i].uniqueId + ",";
                    }
                    submitItems = submitItems.Substring(0,submitItems.LastIndexOf(","));
                    GameUtil.GeneralReq<GeneralResponse>(Services.Mission_FinishWithItems(missionID,submitItems),delegate (GeneralResponse e)
                    {
                        WorldManager.Instance.GetNpcViewManager().RefinishMissionFlag();

                    },(e) =>
                    {
                        TipManager.AddTip(e.message);
                    });
                }
            }
        }
        #endregion

        #region 多人对话提交任务
        /// <summary>
        /// 这是个多人对话获得道具提交，暂时和对话任务一样，后期做物品提交得时候可能需要特殊处理
        /// </summary>
        /// <param name="npc">对话NPC</param>
        /// <param name="mission">任务</param>
        /// <param name="submitDto">任务提交项</param>
        public static void TalkFindItemMission(Npc npc,Mission mission,SubmitDto submitDto)
        {
            GameUtil.GeneralReq<GeneralResponse>(Services.Mission_Talk(npc.id,mission.id),delegate (GeneralResponse e)
            {
                if(submitDto.auto)
                {
                    ProxyNpcDialogueView.Close();
                }
                else
                {
                    
                }
                if(MissionHelper.IsMonsterNpc(npc))
                {
                    //WorldManager.Instance.GetNpcViewManager().RemoveNpc(npc.id);
                }
            },
            (e) =>
            {
                TipManager.AddTip(e.message);
            });
        }
        #endregion

        #region 任务日志领取奖励
        public static void ReqReceiveRewardByRecord(int evtId)
        {
            GameUtil.GeneralReq(Services.Mission_RecordExtract(evtId), e=> { RespReceiveRewardByRecord(evtId); });
        }

        private static void RespReceiveRewardByRecord(int evtId)
        {
            DataMgr._data.UpdateRecordList(evtId);
            FireData();
        }
        #endregion

        #region  请求明雷打怪任务
        public static void BattleShowMonster(Mission mission,Npc npc,int index,Action<bool,string> callback)
        {
            GameUtil.GeneralReq(Services.Mission_Battle(mission.id,index),delegate (GeneralResponse e)
            {
                if(callback != null)
                    callback(true,"");
            },delegate (ErrorResponse errorResponse)
            {
                if(callback != null)
                {
                    callback(false,errorResponse.message);
                }
                else
                {
                    //TipManager.AddTip(errorResponse.message);
                }
            });
        }
        #endregion

        #region 采集完采集物提交给服务器的方法
        /// <summary>
        /// 采集完采集物提交给服务器的方法
        /// </summary>
        /// <param name="PickID"></param>
        /// <param name="missionid"></param>
        public static void PickItemMission(long PickID,int missionID)
        {
            GameUtil.GeneralReq<GeneralResponse>(Services.Mission_Pick(PickID,missionID),delegate (GeneralResponse e)
            {
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(PickID);
            },
           (e) =>
           {
               TipManager.AddTip(e.message);
           });
        }
        #endregion


        #region 接受公共巡逻任务
        public static void AccepGhostMisson()
        {
            GameUtil.GeneralReq(Services.Mission_AcceptGhost(),delegate{});
        }
        #endregion

        #region 副本切换场景
        public static void ChangeCopyScene() {
            GameUtil.GeneralReq(Services.Copy_ChangeScene());
        }
        #endregion
    }
}
