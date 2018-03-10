// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 7/27/2017 7:53:13 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AppDto;
using AppServices;

public sealed partial class CrewViewDataMgr
{
    public static class CrewViewNetMsg
    {
        //已拥有的伙伴
        //CrewDto
        public static void ResCrewList(Action callback)
        {
            GameUtil.GeneralReq(Services.Crew_Info(), response =>
            {
                CrewDto _dto = response as CrewDto;
                DataMgr._data.UpdateCrewList(_dto.crewInfos);
                if (_dto.crewInfos.Count > 0)
                {
                    var tempDtoInfo = DataMgr._data.GetCrewBookList.TryGetValue(0).GetInfoDto;
                    if(_dto.mainCrewId != -1)
                    {
                        _dto.crewInfos.ForEach(e =>
                        {
                            if(e.id == _dto.mainCrewId) {
                                tempDtoInfo = e;
                            }
                        });
                    }
                    DataMgr._data.SetCurCrewId(tempDtoInfo.crewId,tempDtoInfo.id);
                    DataMgr._data.SetNextPhaseAndRaise(tempDtoInfo.phase, tempDtoInfo.raise);
                    var crewDto = DataMgr._data.IsHadCurPantner(tempDtoInfo.crewId);
                    if (crewDto.fetterDto.Count > 0)
                        DataMgr._data.SetCurCrewFetterId(crewDto.fetterDto[0].crewFetterId);
                }
                else
                    DataMgr._data.SetCurCrewId();
                DataMgr._data.buyCrew = BuyCrew.Init;
                CrewSkillDataMgr.DataMgr.UpdateMagicData(_dto.crewInfos);
                if(callback != null)
                    callback();
                FireData();
            });
        }

        //伙伴招募
        //CrewInfoDto
        public static void Crew_Add(int crewId)
        {
            GameUtil.GeneralReq(Services.Crew_Add(crewId), response =>
            {
                CrewDto dto = response as CrewDto;
                if (dto == null)
                {
                    GameDebuger.LogError("Crew_Add返回的数据有问题,请检查");
                    return;
                }
                var crewDto = dto.crewInfos.Find(d => d.crewId == crewId);
                if (crewDto == null)
                {
                    GameDebuger.LogError(string.Format("找不到{0}伙伴",crewDto.crewId));
                    return;
                }
                DataMgr._data.UpdateCrewList(dto.crewInfos);
                DataMgr._data.GetCrewSkillTrainData().UpdateTrainListByDto(crewId, crewDto);
                CrewSkillDataMgr.DataMgr.UpdateMagicData(crewId, crewDto.crewSkillsDto.magic);
                CrewSkillDataMgr.DataMgr.UpdateCrewTmpDic(crewId, crewDto);
                CrewSkillDataMgr.DataMgr.CraftsData.UpdateCraftsDataByTrain(crewId, crewDto.crewSkillsDto);
                CrewSkillDataMgr.DataMgr.UpdateCrewPassiveData(crewId, crewDto.crewSkillsDto.passiveSkillDtos);
                DataMgr._data.UpdateCrewDto(crewDto);
                DataMgr._data.SetCurCrewId(crewDto.crewId, crewDto.id);
                if (crewDto.fetterDto.Count > 0)
                    DataMgr._data.SetCurCrewFetterId(crewDto.fetterDto[0].crewFetterId);
                DataMgr._data.buyCrew = BuyCrew.BuyIng;
                FireData();
                
            });
        }

        public static void Crew_Phase(long uid,int id)
        {
            if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_16, true)) return;

            GameUtil.GeneralReq(Services.Crew_Phase(uid), response =>
             {
                 var _dto = response as CrewDto;
                 DataMgr._data.UpdateCrewList(_dto.crewInfos);
                 DataMgr._data.SetCurCrewId(id, uid);
                 FireData();
                 TipManager.AddTip("进阶成功!");
             });
        }

        public static void Crew_Raise(long uid,int id)
        {
            GameUtil.GeneralReq(Services.Crew_Raise(uid), response =>
            {
                var _dto = response as CrewDto;
                DataMgr._data.UpdateCrewList(_dto.crewInfos);
                DataMgr._data.SetCurCrewId(id, uid);
                FireData();
                TipManager.AddTip("强化成功!");
            });
        }

        public static void CrewFetterActive(long crewId,int crewFetterId,bool active)
        {
            GameUtil.GeneralReq(Services.Crew_ActivationFetter(crewId, crewFetterId,active),resp=> {

                CrewInfoDto _dto = resp as CrewInfoDto;
                DataMgr._data.UpdateCrewDto(_dto);
                //DataMgr._data.UpdateCrewFetterVoList();
                if(active)
                    TipManager.AddTip("激活羁绊成功");
                else
                    TipManager.AddTip("取消羁绊成功");
                FireData();
            });
        }
        public static void Crew_FollowChange(long id)
        {
            GameUtil.GeneralReq(Services.Crew_FollowChange(id), response =>
            {
                TipManager.AddTip("===跟随成功===");
                FireData();
            });
        }


        #region 研修请求
        public static void ReqSkillTrain(long id)
        {
            GameUtil.GeneralReq<CraftsTrainingDto>(Services.Crew_CraftsTraining(id), RespSkillTrain);
        }
        private static void RespSkillTrain(CraftsTrainingDto dto)
        {
            DataMgr._data.GetCrewSkillTrainData().UpdateTrainListByDto(CrewSkillHelper.CrewID, dto);
            TipManager.AddTip("研修成功");
            FireData();
        }
        #endregion

        #region 研修保存
        public static void ReqSkillTrainSave(long id)
        {
            GameUtil.GeneralReq<CrewInfoDto>(Services.Crew_CraftsSave(id), RespSkillTrainSave);
        }

        private static void RespSkillTrainSave(CrewInfoDto dto)
        {
            TipManager.AddTip("保存成功");
            DataMgr._data.GetCrewSkillTrainData().UpdateTrainListByDto(CrewSkillHelper.CrewID, dto);
            CrewSkillDataMgr.DataMgr.CraftsData.UpdateCraftsDataByTrain(CrewSkillHelper.CrewID, dto.crewSkillsDto);
            DataMgr._data.UpdateCrewDto(dto);
            FireData();
        }
        #endregion

        #region 好感度

        //伙伴送礼
        public static void CrewReward(long bCrewId, int itemId)
        {
            GameUtil.GeneralReq(Services.Crew_GiveGifts(bCrewId, itemId), response =>
            {
                OnIncreaseFavorDto dto = response as OnIncreaseFavorDto;
                if (dto == null)
                {
                    TipManager.AddTip("Crew_GiveGifts接口服务器给错数据了!!!");
                    return;
                }
                TipManager.AddTip("===送礼成功===");
                var crewDto = DataMgr._data.GetSelfCrew().Find(d => d.id == bCrewId);
                crewDto.favor = dto.favor;
                DataMgr._data.UpdateCrewDto(crewDto);
                FireData();
            });
        }

        public static void CrewClickModel(long bCrewId)
        {
            GameUtil.GeneralReq(Services.Crew_ClickModel(bCrewId), response =>
            {
                TipManager.AddTip("好感度+2");
                var crewDto = DataMgr._data.GetSelfCrew().Find(d => d.id == bCrewId);
                crewDto.favor += 2;
                DataMgr._data.UpdateCrewDto(crewDto);
                FireData();
            });
        }
        #endregion

        #region 伙伴道具升级

        public static void CrewUpGrade(long uid)
        {
            GameUtil.GeneralReq(Services.Crew_Upgrade(uid), response =>
            {
                CrewShortDto shortDto = response as CrewShortDto;   //没升级
                CrewInfoDto infoDto = response as CrewInfoDto;      //已经升级
                var dto = DataMgr._data.GetCrewBookList.Find(d => d.GetInfoDto.id == uid);
                if (shortDto != null)
                {
                    CrewInfoDto crewInfoDto = dto.GetInfoDto;
                    crewInfoDto.exp = shortDto.exp;
                    DataMgr._data.UpdateCrewDto(crewInfoDto);
                }
                else if (infoDto != null)
                    DataMgr._data.UpdateCrewDto(infoDto);
                else 
                    GameDebuger.LogError("服务端返回的数据有问题,请检查！！！");

                TipManager.AddTip("使用道具成功");
                FireData();
            });
        }

        public static void UseAllProps(long uid)
        {
            GameUtil.GeneralReq(Services.Crew_UpgradeAllProps(uid), response =>
            {
                CrewShortDto shortDto = response as CrewShortDto;   //没升级
                CrewInfoDto infoDto = response as CrewInfoDto;      //已经升级
                var dto = DataMgr._data.GetCrewBookList.Find(d => d.GetInfoDto.id == uid);
                if (shortDto != null)
                {
                    CrewInfoDto crewInfoDto = dto.GetInfoDto;
                    crewInfoDto.exp = shortDto.exp;
                    DataMgr._data.UpdateCrewDto(crewInfoDto);
                }
                else if (infoDto != null)
                    DataMgr._data.UpdateCrewDto(infoDto);
                else
                    GameDebuger.LogError("服务端返回的数据有问题,请检查！！！");

                TipManager.AddTip("使用道具成功");
                FireData();
            });
        }
        #endregion
    }
}
