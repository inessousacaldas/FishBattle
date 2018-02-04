using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class RoleSkillDataMgr
{
    public static class RoleSkillNetMsg
    {
        public static void InitListener()
        {
            NotifyListenerRegister.RegistListener<SpecialityExpGradeNotify>(HandleSpecialityExpGradeNotify);
        }

        #region 技能系统，战技，魔法
        public static void ReqSkillInfo()
        {
            GameUtil.GeneralReq<SkillsDto>(Services.Skill_Info(),RespSkillInfo);
        }

        private static void RespSkillInfo(SkillsDto dto)
        {
            DataMgr.MainData.UpdateSkillsDto(dto);
            FireData();
        }

        public static void ReqSkillDefaultSCrafts(int id)
        {
            GameUtil.GeneralReq<DefaultSCraftsDto>(Services.Skill_DefaultSCrafts(id),RespSkillDefaultSCrafts);
        }

        private static void RespSkillDefaultSCrafts(DefaultSCraftsDto dto)
        {
            DataMgr.MainData.UpdateDefaultSCrafts(dto);
            FireData();
            var vo = DataMgr.MainData.GetSkillCraftsVO(dto.id);
            TipManager.AddTip(string.Format("成功将S技—{0}切换为快捷S技",vo.Name));
        }

        public static void ReqSkillUpgrade(int skillMapId,int costId,int consumNum)
        {
            GameUtil.GeneralReq<CraftsGradeDto>(Services.Skill_Upgrade(skillMapId),e => { RespSkillUpgrade(e, costId,consumNum); });
        }

        private static void RespSkillUpgrade(CraftsGradeDto dto,int costId,int consumNum)
        {
            DataMgr.MainData.UpdateCrafts(dto);
            FireData();
            var vo = DataMgr.MainData.GetSkillCraftsVO(dto.id);

            var item = ItemHelper.GetGeneralItemByItemId(costId);
            if(item == null)
                TipManager.AddTip(string.Format("将{0}战技升到{1}级",vo.Name,vo.Grade));
            else
            {
                if(costId == (int)AppVirtualItem.VirtualItemEnum.SILVER)
                {
                    TipManager.AddTip(string.Format("消耗[7EE830]{0}{1}[-]将[7EE830]{2}战技[-]升到{3}级", consumNum, item.name,
                        vo.Name, vo.Grade.WrapColor(ColorConstantV3.Color_Green_Str)));
                }
                else
                {
                    TipManager.AddTip(string.Format("消耗[7EE830]{0}个{1}道具[-]将[7EE830]{2}战技[-]升到{3}级", consumNum, item.name,
                        vo.Name, vo.Grade.WrapColor(ColorConstantV3.Color_Green_Str)));
                }
            }
        }
        #endregion

        #region 潜能系统
        public static void ReqPotentialInfo()
        {
            GameUtil.GeneralReq(Services.Potential_GetInfo(),RespPotentialInfo);
        }

        private static void RespPotentialInfo(GeneralResponse resp)
        {
            var list = resp as DataList;
            DataMgr._data.potentialData.UpdateDto(list);
            FireData();
        }

        public static void ReqPotentialUpgrade(int potentialId,int time)
        {
            GameUtil.GeneralReq(Services.Potential_Upgrade(potentialId,time) ,e=> { RespPotentialUpgrade(e); } ,null);
        }

        private static void RespPotentialUpgrade(GeneralResponse resp)
        {
            var dto = resp as PotentialDto;
            if(dto != null)
            {
                DataMgr.PotentialData.UpdateDtoByUpgrade(dto);
                var vo = DataMgr.PotentialData.GetVOByID(dto.potentialInfoDto.id);
                string msg = string.Format("消耗{0}将{1}升级{2}次",(dto.silver + ItemHelper.GetGeneralItemByItemId((int)AppVirtualItem.VirtualItemEnum.SILVER).name).WrapColor(ColorConstantV3.Color_Green),
                    (vo.cfgVO.name+"潜能").WrapColor(ColorConstantV3.Color_Green),dto.time.WrapColor(ColorConstantV3.Color_Green_Str));
                TipManager.AddTip(msg);
                FireData();
            }
        }

        #endregion

        #region 天赋系统
        public static void ReqTalentInfo()
        {
            GameUtil.GeneralReq(Services.Talent_Info(),RespTalentInfo);
        }

        private static void RespTalentInfo(GeneralResponse resp)
        {
            var dto = resp as TalentDto;
            DataMgr.TalentData.UpdateDto(dto);
            FireData();
        }

        public static void ReqTalentReset()
        {
            GameUtil.GeneralReq(Services.Talent_Reset(),null,OnSuccTalentReset);
        }

        private static void OnSuccTalentReset()
        {
            TipManager.AddTip("重置天赋点成功");
            DataMgr.TalentData.UpdateDataReset();
            FireData();
        }

        public static void ReqTalentAddPoint(int talentId)
        {
            GameUtil.GeneralReq(Services.Talent_AddPoint(talentId),RespTalentAddPoint);
        }

        private static void RespTalentAddPoint(GeneralResponse resp)
        {
            var gradeDto = resp as TalentGradeDto;
            DataMgr.TalentData.UpdateDtoSingle(gradeDto);
            FireData();
            TipManager.AddTip("升级成功");
        }
        #endregion

        #region 专精系统
        public static void ReqSpecialityInfo()
        {
            GameUtil.GeneralReq(Services.Speciality_Info(),RespSpecialityInfo);
        }

        private static void RespSpecialityInfo(GeneralResponse resp)
        {
            var dto = resp as SpecialityDto;
            DataMgr.SpecData.UpdateInfo(dto);
            FireData();
        }

        public static void ReqSpecialityReset()
        {
            GameUtil.GeneralReq(Services.Speciality_Reset(),RespSpecialityReset,OnSuccSpecialityReset);
        }

        private static void RespSpecialityReset(GeneralResponse resp)
        {
            var dto = resp as SpecialityAddPointDto;
            DataMgr.SpecData.UpdateGradeListByReset(dto.specialityGradeDtos);
            FireData();
        }

        private static void OnSuccSpecialityReset()
        {
            TipManager.AddTip("重置专精点成功");
        }


        public static void ReqSpecialityAddPoint(string str)
        {
            GameUtil.GeneralReq(Services.Speciality_AddPoint(str),RespSpecialityAddPoint,OnSuccSpecialityAddPoint);
        }

        private static void RespSpecialityAddPoint(GeneralResponse resp)
        {
            var dto = resp as SpecialityAddPointDto;
            DataMgr.SpecData.UpdateGradeList(dto.specialityGradeDtos);
            DataMgr.SpecData.ResetTempList();
            FireData();
        }

        private static void OnSuccSpecialityAddPoint()
        {
            TipManager.AddTip("专精点加点成功");
        }

        public static void ReqSpecialityAddExp(SpecialityExpGrade.AddExpType addExpType,int count)
        {
            GameUtil.GeneralReq(Services.Speciality_AddExp((int)addExpType,count),null,RespSpecialityAddExp);
        }

        private static void RespSpecialityAddExp()
        {
            TipManager.AddTip("训练成功");
        }

        private static void HandleSpecialityExpGradeNotify(SpecialityExpGradeNotify expGradeNotify)
        {
            DataMgr.SpecData.UpdateInfoDtoByNotify(expGradeNotify);
            FireData();
        }
        #endregion
    }
}
