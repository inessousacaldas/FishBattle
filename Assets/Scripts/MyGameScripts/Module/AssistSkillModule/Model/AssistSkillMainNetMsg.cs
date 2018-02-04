// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 9/20/2017 7:57:58 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class AssistSkillMainDataMgr
{
    public static class AssistSkillMainNetMsg
    {
        #region 生活技能
        //请求当前生活技能信息
        public static void ReqAssistInfo(Action completeAction=null)
        {
            GameUtil.GeneralReq(Services.AssistSkill_Info(), resp =>
            {
                var dto = resp as AssistSkillInfoDto;
                DataMgr._data.UpdateInitData(dto.asmds);
                DataMgr._data.FirstForget = dto.forget == 0 ? true : false;

                GameUtil.SafeRun(completeAction);
                FireData();
            });
        }

        //学习技能（包括第一次选择）
        public static void ReqLearnSkill(int id, int times)
        {
            GameUtil.GeneralReq(Services.AssistSkill_Study(id, times), resp =>
            {
                var dto = resp as AssistSkillDto;
                DataMgr._data.UpgradeData(dto);

                FireData();
            });
        }

        //生产
        public static void ReqProduct(int id, int times)
        {
            GameUtil.GeneralReq(Services.AssistSkill_Make(id, times), resp =>
            {
                var dto = resp as AssistSkillMakeDto;
                var str = string.Empty;
                dto.items.ForEachI((item,index) =>
                {
                    //if (index > 0)
                    //    str += "\n";
                    var name = string.Empty;
                    switch (item.type)
                    {
                        case (int)AssistSkillMakeItemDto.ItemType.Succeed:
                            name = ItemHelper.GetGeneralItemByItemId(item.pid)==null?"道具":ItemHelper.GetGeneralItemByItemId(item.pid).name;
                            str += string.Format("制作成功，你制作了1个{0}", name);
                            TipManager.AddTip(string.Format("制作成功，你制作了1个{0}", name));
                            break;
                        case (int)AssistSkillMakeItemDto.ItemType.Defeated:
                            name = ItemHelper.GetGeneralItemByItemId(item.pid) == null ? "道具" : ItemHelper.GetGeneralItemByItemId(item.pid).name;
                            str += string.Format("制作失败，你制作了1个{0}", name);
                            TipManager.AddTip(string.Format("制作失败，你制作了1个{0}", name));
                            break;
                        case (int)AssistSkillMakeItemDto.ItemType.Succeed_Crit:
                            name = ItemHelper.GetGeneralItemByItemId(item.pid) == null ? "道具" : ItemHelper.GetGeneralItemByItemId(item.pid).name;
                            str += string.Format("制作成功，你制作了2个{0}", name);
                            TipManager.AddTip(string.Format("制作成功，你制作了2个{0}", name));
                            break;
                        case (int)AssistSkillMakeItemDto.ItemType.Defeated_Crit:
                            name = ItemHelper.GetGeneralItemByItemId(item.pid) == null ? "道具" : ItemHelper.GetGeneralItemByItemId(item.pid).name;
                            str += string.Format("制作失败，你制作了2个{0}", name);
                            TipManager.AddTip(string.Format("制作失败，你制作了2个{0}", name));
                            break;
                    }
                });

                FireData();
            });
        }

        //遗忘
        public static void ReqForgetSkill(int id)
        {
            GameUtil.GeneralReq(Services.AssistSkill_Forget(id), resp =>
            {
                var dto = resp as AssistSkillForgetConsumeDto;
                string str = "";
                dto.payBackItem.ForEachI((item, index) =>
                {
                    if (index == 0)
                        str = "返还：";

                    str += ItemHelper.GetGeneralItemByItemId(item.itemId).name + item.count;
                    str = index == dto.payBackItem.Count - 1 ? str : str + "\n";
                });

                TipManager.AddTip(str);

                DataMgr._data.UpdateForgetData();

                FireData();
            });
        }
        #endregion

        #region 委托任务
        //任务信息
        public static void ReqDelegateMissionMsg()
        {
            GameUtil.GeneralReq(Services.Delegate_MissionList(), resp =>
            {
                var dto = resp as DelegateMissionHoleDto;
                DataMgr._data.UpdateMissionData(dto);
                FireData();
            });
        }

        //任务状态
        public static void ReqMissionState(int delegateMissionId)
        {
            GameUtil.GeneralReq(Services.Delegate_MissionState(delegateMissionId), resp =>
            {
                var dto = resp as DelegateMissionStateDto;
                //DataMgr._data.UpdateOneMissionData(delegateMissionId, dto);
                FireData();
            });
        }

        //刷新
        public static void ReqRefresh(long propUid)
        {
            GameUtil.GeneralReq(Services.Delegate_PropRefresh(propUid), resp =>
            {
                var dto = resp as DelegateMissionHoleDto;
                DataMgr._data.UpdateMissionData(dto);
                FireData();
            });
        }

        //接受任务
        public static void ReqAcceptMission(int delegateMissionId, long friendId, string crewIds)
        {
            GameUtil.GeneralReq(Services.Delegate_Accept(delegateMissionId, friendId, crewIds), resp =>
            {
                var dto = resp as DelegateMissionDto;
                DataMgr._data.UpdateCurMissionData(dto);
                DataMgr._data.AcceptNum += 1;
                FireData();
            });
        }

        //放弃任务
        public static void ReqAbandonMission(int delegateMissionId)
        {
            GameUtil.GeneralReq(Services.Delegate_Drop(delegateMissionId), resp =>
            {
                var dto1 = resp as DelegateMissionDto;
                var dto2 = resp as DelegateMissionHoleDto;
                if (dto1 != null)
                    DataMgr._data.ReplaceCurMissionDataWithNew(delegateMissionId, dto1);
                else if (dto2 != null)
                    DataMgr._data.UpdateMissionData(dto2);
                FireData();
            });
        }

        //领取奖励
        public static void ReqGetReward(int delegateMissionId)
        {
            GameUtil.GeneralReq(Services.Delegate_Harvest(delegateMissionId), resp =>
            {
                var dto1 = resp as DelegateMissionDto;
                var dto2 = resp as DelegateMissionHoleDto;
                if (dto1 != null)
                    DataMgr._data.ReplaceCurMissionDataWithNew(delegateMissionId, dto1);
                else if (dto2 != null)
                    DataMgr._data.UpdateMissionData(dto2);
                FireData();
            });
        }

        //快速完成
        public static void ReqFastComplete(int delegateMissionId, long propUid)
        {
            GameUtil.GeneralReq(Services.Delegate_Finish(delegateMissionId, propUid), resp =>
            {
                var dto1 = resp as DelegateMissionDto;
                var dto2 = resp as DelegateMissionHoleDto;
                if (dto1 != null)
                    DataMgr._data.ReplaceCurMissionDataWithNew(delegateMissionId, dto1);
                else if(dto2 != null)
                    DataMgr._data.UpdateMissionData(dto2);
                FireData();
            });
        }

        //请求伙伴信息
        public static void ReqCrewShortInfo()
        {
            GameUtil.GeneralReq<CrewShortListDto>(Services.Chat_CrewList(), resp =>
            {
                var dto = resp as CrewShortListDto;
                DataMgr._data.UpdateCrewInfo(dto);
            });
        }
        #endregion
    }
}
 