using AppDto;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule.Model;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule.View;
using System.Collections.Generic;

public interface IRoleSkillData
{
    RoleSkillTab MainTab { get; }
    IRoleSkillPotentialData PotentialData { get; }
    IRoleSkillTalentData TalentData { get; }
    IRoleSkillSpecialityData SpecData { get; }
    IRoleSkillMainData MainData { get; }
    IRoleSkillBattleData BattleData { get; }
}

public interface IRoleSkillMainData
{
    Dictionary<int, int> MagicOpenDic { get; }
    Skill.SkillEnum CurTab { get; }
    RoleSkillCraftsItemController CurSelCtrl { get; set; }
    RoleSkillMainSCraftsState SCraftsState { get; }
    Dictionary<int,RoleSkillCraftsVO> CraftsDict { get; }
    int CurSCrafts { get; }
    string GetCraftsDesc(int id);
    CharacterCraftsGrade GetCostByGradeDto(RoleSkillCraftsVO vo);
    
    RoleSkillMainRangeState GetScopeTarType(int id);
    List<RoleSkillMagicVO> MagicDtoDic { get; }
    RoleSkillCraftsItemController CurSelMagicCtrl { get; set; }
    string GetMagicDesc(int id);
    int GetUpGradeNeedRoleLevel(RoleSkillCraftsVO vo);
}

    public interface IRoleSkillTalentData
{
    Dictionary<int,RoleSkillTalentItemVO> ListItem { get; }
    List<int> RecommendList { get; }
    Talent GetCfgVOById(int id);
    RoleSkillTalentSingleItemController LastItem { get; }
}

public interface IRoleSkillPotentialData
{
    RoleSkillPotentialItem LastItem { get; set; }
    int MaxLevel { get; }
    int GetIDByIndex(int index);
    Potential GetPotentialByIndex(int index);
    int GetLimitByID(int id);
    long GetCostByLevel(int level);
    long GetCostByID(int id);
    int GetLevelByID(int id);
    RoleSkillPotentialVO GetVOByID(int id);
    string GetEffectByLevel(RoleSkillPotentialVO vo,int level);
    Dictionary<int, int> LimitList { get; }
}

public interface IRoleSkillSpecialityData
{
    int GetShowPoint();
    int GetCurGrade();
    int GetHasAddExpTime();
    int GetCurExp();
    int GetLevelLimit();
    int MaxGrade { get; }
    SpecialityExpGrade GetExpGradeVO(int grade);
    Dictionary<int,RoleSkillSpecialityTempVO> TempList { get; }
    RoleSkillSpecItemSingleType GetCurType();
    bool CheckCanAdd(Speciality.SpecialityLayerEnum layer);
    int GetNeedPoint(Speciality.SpecialityLayerEnum layer);
    void CheckClearAddPoint(Speciality.SpecialityLayerEnum layer);
    int GetTrainItemID();
}


public interface IRoleSkillBattleData
{
    IRoleSkillMainData RoleSkillData { get;  }
    ICrewInfoData CrewData { get;  }
}

/// <summary>
/// 技能系统的选项卡（技能，潜能，天赋，专精）
/// </summary>
public enum RoleSkillTab
{
    Skill
   , Potential
   , Talent
   , Sepciality
}

public sealed partial class RoleSkillDataMgr
{
    public sealed partial class RoleSkillData:IRoleSkillData
    {
        public RoleSkillTab mainTab = RoleSkillTab.Skill;
        public RoleSkillMainData mainData;
        public RoleSkillPotentialData potentialData;
        public RoleSkillTalentData talentData;
        public RoleSkillSpecialityData specData;
        public RoleSkillBattleData battleData;
        public RoleSkillData()
        {

        }

        public void InitData()
        {
            mainData = new RoleSkillMainData();
            mainData.InitData();
            potentialData = new RoleSkillPotentialData();
            talentData = new RoleSkillTalentData();
            talentData.InitData();
            specData = new RoleSkillSpecialityData();
            specData.InitData();
            battleData = new RoleSkillBattleData();
        }

        
        public IRoleSkillMainData MainData
        {
            get { return mainData; }
        }

        public IRoleSkillPotentialData PotentialData
        {
            get { return potentialData; }
        }

        public IRoleSkillTalentData TalentData
        {
            get { return talentData; }
        }

        public IRoleSkillSpecialityData SpecData
        {
            get { return specData; }
        }

        public RoleSkillTab MainTab
        {
            get { return mainTab; }
        }

        public IRoleSkillBattleData BattleData
        {
            get { return battleData; }
        }

        public void Dispose()
        {
            if(talentData != null)
            {
                talentData.Dispose();
            }
        }
    }
}
