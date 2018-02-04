// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class TeamFormationDataMgr
{
    public static class TeamFormationNetMsg
    {
        //升级学习
        //FormationInfoDto
        public static void Formation_UpgradeOrLearn(int id)
        {
            GameUtil.GeneralReq(Services.Formation_Upgrade(id), response =>
            {
                var res = response as FormationInfoDto;
                if(res.level > 1)
                    TipManager.AddTip("升级队形成功");
                else
                    TipManager.AddTip("学习队形成功");
                DataMgr._data.acquiredFormation.ReplaceOrAdd(n => n.formationId == res.formationId, res);
                DataMgr._data.CurUpFradeFormation = DataMgr._data.GetFormationById(res.formationId);
                DataMgr._data.FormationListSort();
                FireData();
            });
        }

        //交换行,上下换(有队伍)
        public static void Team_ExchangeRow(int oldPosition, int newPosition)
        {
            GameUtil.GeneralReq(Services.Team_ExchangeRow(oldPosition, newPosition), response =>
            {
                FireData();
            });
        }

        //交换行,上下换(无队伍)
        //List<CaseCasePositionDto>
        public static void Team_ExchangeRow(int caseId, int oldPosition, int newPosition)
        {
            var id = ModelManager.Player.GetPlayerId();
            GameUtil.GeneralReq(Services.Formation_ChangeRow(caseId, oldPosition, newPosition, id), response =>
            {
                var list = response as DataList;
                CasePositionDto dto = null;
                list.items.ForEach(d =>
                {
                    DataMgr._data.CasePostitionDtoReplaceOrAdd(caseId, d as CasePositionDto);
                });
                FireData();
            });
        }


        //交换列,左右换(有队伍)
        public static void Team_ExchangeColumn(string orderStr)
        {
            GameUtil.GeneralReq(Services.Team_ExchangeColumn(orderStr), response =>
            {
                TeamDataMgr.TeamNetMsg.GetFormationInfo(null);
                FireData();
            });
        }

        //交换列,左右换(无队伍)
        public static void Team_ExchangeColumn(int caseId, int statrtIndex, int targetIndex)
        {
            GameUtil.GeneralReq(Services.Formation_ChangeCol(caseId, statrtIndex, targetIndex), response =>
            {
                var list = response as DataList;
                CasePositionDto dto = null;
                list.items.ForEach(d =>
                {
                    var caseDto = d as CasePositionDto;
                    if (caseDto != null)
                    {
                        DataMgr._data.AllCaseInfoDto.caseInfoDtos.Find(info=> info.caseId == caseId).casePositions.ReplaceOrAdd(
                            a => a.crewId == caseDto.crewId, caseDto);
                        DataMgr._data.CasePostitionDtoReplaceOrAdd(caseId, d as CasePositionDto);
                    }
                });
                FireData();
            });
        }


        //开启或关闭
        //caseId 方案id
        //formationId 队形id  关闭时为默认队形id
        public static void Formastion_ChangeCaseFormation(int caseId, int formationId)
        {
            GameUtil.GeneralReq(Services.Formation_ChangeCaseFormation(caseId, formationId), response =>
            {
                if(formationId == (int)Formation.FormationType.Regular)
                    TipManager.AddTip("关闭队形成功");
                else
                    TipManager.AddTip("开启队形成功");

                DataMgr._data.SetCurFormationID(formationId);
                DataMgr._data.SetCaseFormationId(caseId, formationId);
                DataMgr._data.FormationListSort();
                FireData();
            });
        }

        //队形界面,队形列表
        //List<FormationInfoDto>
        public static void Formation_Info(Action callback)
        {
            GameUtil.GeneralReq(Services.Formation_Info(), response =>
            {
                DataMgr._data.UpdateSelfFormationList(response as DataList);
                if (callback != null)
                    callback();
            });
        }

        #region crewFormation
        //上阵
        //List<CasePositionDto>     如果是list.cout>1则表示添加了主战伙伴
        public static void Formation_AddPosition(int caseId, long id)
        {
            GameUtil.GeneralReq(Services.Formation_AddPosition(caseId, id), response =>
            {
                var datalist = response as DataList;
                if (datalist != null)
                {
                    DataMgr._data.AddCasePositionDto(datalist, caseId);
                    FireData();
                    if (DataMgr._data.GetMainCrewId == id)
                        DataMgr._data.SetMainCrewId(id);
                    TipManager.AddTip("上阵成功");
                }
                else
                    GameDebuger.LogError("=====伙伴上阵数据出错======");
            });
        }

        //设为主战伙伴
        //List<FormationCaseInfoDto>
        public static void Formation_ChangeMainCrew(int caseId, long crewId)
        {
            GameUtil.GeneralReq(Services.Formation_ChangeMainCrew(crewId), response =>
            {
                var dto = response as DataList;
                if (dto != null)
                {
                    List<FormationCaseInfoDto> dtoList = new List<FormationCaseInfoDto>();
                    if (caseId == 0)
                        dtoList.Add(dto.items[0] as FormationCaseInfoDto);
                    else
                    {
                        dto.items.ForEachI((d,i) =>
                        {
                            if(i > 0)
                                dtoList.Add(d as FormationCaseInfoDto);
                        });
                    }
                        
                    DataMgr._data.UpdateCrewFormationList(dtoList);
                    DataMgr._data.SetMainCrewId(crewId);
                    FireData();
                    TipManager.AddTip("设为主战伙伴");
                }
                else
                    GameDebuger.LogError("=====设为主战伙伴数据出错======");
            });
        }

        //下阵
        //List<CasePositionDto>
        public static void Formation_RemovePosition(int caseId, long id)
        {
            var caseIdx = caseId < 0 ? DataMgr._data.GetUseCaseIdx : caseId;
            GameUtil.GeneralReq(Services.Formation_RemovePosition(caseIdx, id), response =>
            {
                var list = response as DataList;
                if (list != null)
                {
                    var dtoList = list.items.Map(s => s as CasePositionDto).ToList();
                    var sucess = DataMgr._data.RemoveCasePositionDto(caseId, dtoList);
                    if (sucess)
                        FireData();
                    TipManager.AddTip("下阵成功");
                }
                else
                    GameDebuger.LogError("=====下阵数据出错======");
            });
        }

        //切换伙伴布阵方案
        public static void Formation_ChangeCase(int caseId)
        {
            GameUtil.GeneralReq(Services.Formation_ChangeCase(caseId), response =>
            {
                DataMgr._data.SetUseCaseId(caseId);
                FireData();
            });
        }

        //伙伴布阵界面数据
        //AllCaseInfoDto
        public static void Formation_CaseInfo(Action callback)
        {
            GameUtil.GeneralReq(Services.Formation_CaseInfo(), response =>
            {
                AllCaseInfoDto Dto = response as AllCaseInfoDto;
                if (Dto != null)
                {
                    DataMgr._data.UpdateCrewFormationList(Dto.caseInfoDtos);
                    DataMgr._data.SetMainCrewId(Dto.mainCrewId);
                    DataMgr._data.SetUseCaseId(Dto.activeAttackFormationCaseIndex); //设置使用中的方案
                    DataMgr._data.SetCurCaseIdx(Dto.activeAttackFormationCaseIndex);//设置选中方案
                    DataMgr._data.UpdateSelfCrewData(Dto.crewInfoDtos);

                    DataMgr._data.AllCaseInfoDto = Dto;
                    DataMgr._data.FormationListSort();
                    var caseinfo = Dto.caseInfoDtos.Find(d => d.caseId == Dto.activeAttackFormationCaseIndex);
                    DataMgr._data.SetCurFormationID(caseinfo == null ? 0 : caseinfo.formationId);

                    if (callback != null)
                        callback();
                }
            });
        }
        #endregion

        #region 竞技场防御阵型
        public static void ReqGarandArenaFormation(Action callback)
        {
            GameUtil.GeneralReq(Services.Formation_DefenceCaseInfo(), response =>
            {
                var Dto = response as AllCaseInfoDto;
                if (Dto != null)
                {
                    DataMgr._data.UpdateCrewFormationList(Dto.caseInfoDtos);
                    DataMgr._data.SetMainCrewId(Dto.mainCrewId);
                    DataMgr._data.SetArenaCaseId(Dto.activeAttackFormationCaseIndex); //设置使用中的方案
                    DataMgr._data.SetCurCaseIdx(Dto.activeAttackFormationCaseIndex);//设置选中方案
                    DataMgr._data.UpdateSelfCrewData(Dto.crewInfoDtos);

                    DataMgr._data.AllCaseInfoDto = Dto;
                    DataMgr._data.FormationListSort();
                    var caseinfo = Dto.caseInfoDtos.Find(d => d.caseId == Dto.activeAttackFormationCaseIndex);
                    DataMgr._data.SetCurFormationID(caseinfo == null ? 0 : caseinfo.formationId);

                    callback();
                }
            });
        }
        #endregion

    }
}
