// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 8/3/2017 5:07:05 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class CrewSkillDataMgr
{
    public static class CrewSkillNetMsg
    {

        #region 战技升级
        public static void ReqSkillUpgrade(long id,int skillMapId)
        {
            GameUtil.GeneralReq<CraftsGradeDto>(Services.Crew_CraftsUpgrade(id, skillMapId),RespSkillUpgrade);
        }

        private static void RespSkillUpgrade(CraftsGradeDto dto)
        {
            DataMgr._data.UpdateCraftsDataByDto(CrewSkillHelper.CrewID, dto);
            CrewViewDataMgr.CrewMainViewLogic.FireDatas();
        }
        #endregion


        #region 技巧

        /// <summary>
        /// 技巧学习
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bookId"></param>
        public static void ReqPassiveLearn(long id,int bookId)
        {
            GameUtil.GeneralReq<CrewInfoDto>(Services.Crew_PassiveLearn(id, bookId), e=> { RespPassiveLearn(e, bookId); });
        }
        private static void RespPassiveLearn(CrewInfoDto dto,int bookId)
        {
            var book = DataCache.getDtoByCls<GeneralItem>(bookId) as PassiveSkillBook;
            if (book != null)
                TipManager.AddTip(string.Format("成功学习技巧【{0}】", book.name));
            DataMgr._data.UpdatePsvDataByLearn(dto, bookId);
            DataMgr._data.SetNextType(PsvWindowType.Property);
            CrewViewDataMgr.DataMgr.UpdateCrewInfoDto(dto);
        }

        /// <summary>
        /// 技巧升级
        /// </summary>
        /// <param name="id"></param>
        /// <param name="skillMapId"></param>
        public static void ReqPassiveUp(long id,int skillMapId)
        {
            GameUtil.GeneralReq<CrewInfoDto>(Services.Crew_PassiveUpgrade(id, skillMapId),e=> { RespPassiveUp(e, skillMapId); } );
        }
        private static void RespPassiveUp(CrewInfoDto dto,int skillMapId)
        {
            DataMgr._data.UpdatePsvDataByUp(dto, skillMapId);
            CrewViewDataMgr.DataMgr.UpdateCrewInfoDto(dto);
        }

        /// <summary>
        /// 技巧使用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="skillMapId"></param>
        /// <param name="count"></param>
        public static void ReqPassiveUse(long id,int skillMapId,int count)
        {
            GameUtil.GeneralReq<GeneralResponse>(Services.Crew_PassiveAddExp(id, skillMapId, count),e=> { RespPasiveUse(e, skillMapId); } );
        }
        private static void RespPasiveUse(GeneralResponse dto,int skillMapId)
        {
            TipManager.AddTip("使用成功");
            if(dto is PassiveSkillDto)
            {
                var val = dto as PassiveSkillDto;
                DataMgr._data.UpdatePsvDataByUse(val);
                CrewViewDataMgr.CrewMainViewLogic.FireDatas();
            }
            else if(dto is CrewInfoDto)
            {
                var val = dto as CrewInfoDto;
                DataMgr._data.UpdatePsvDataByUp(val, skillMapId);
                CrewViewDataMgr.DataMgr.UpdateCrewInfoDto(val);
            }
        }

        /// <summary>
        /// 技巧遗忘
        /// </summary>
        public static void ReqPassiveForget(long id, int skillMapId)
        {
            GameUtil.GeneralReq<CrewInfoDto>(Services.Crew_PassiveForget(id, skillMapId),
                e => { RespPassiveForget(e, skillMapId); });
        }
        private static void RespPassiveForget(CrewInfoDto dto, int skillMapId)
        {
            var skill =
                DataCache.getArrayByCls<Skill>()
                    .Filter(d => d is CrewPassiveSkill)
                    .Find(d => d.skillMapId == skillMapId);
            if (skill != null)
                TipManager.AddTip(string.Format("已遗忘技巧【{0}】", skill.name));
            DataMgr._data.UpdatePsvDtoDic(dto.crewSkillsDto);
            DataMgr._data.SetNextType(PsvWindowType.Backpack);
            CrewViewDataMgr.DataMgr.UpdateCrewInfoDto(dto);
        }
        #endregion
    }
}
