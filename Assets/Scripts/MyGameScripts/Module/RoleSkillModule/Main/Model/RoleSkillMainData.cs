// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 7/21/2017 4:06:00 PM
// **********************************************************************

using AppDto;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule;
using System;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// 技能系统的子选项卡（战技，魔法）
/// 导力器：Orbment
/// 战技：Crafts
/// S战技：S-Crafts
/// 魔法：Orbal Arts
/// </summary>


/// <summary>
/// S技的状态：普通，选中状态
/// </summary>
public enum RoleSkillMainSCraftsState
{
    Normal
   , Select
}

/// <summary>
/// 技能范围：友方，敌方
/// </summary>
public enum RoleSkillMainRangeState
{
    Friend
   , Enemy
}

public class RoleSkillCraftsVO
{
    public int id;
    public CraftsGradeDto gradeDto;
    public Crafts cfgVO;

    public int SkillMapId
    {
        get { return cfgVO != null ? cfgVO.skillMapId : 0; }
    }

    public string Name
    {
        get { return cfgVO != null ? cfgVO.name : ""; }
    }

    public int Grade
    {
        get { return gradeDto != null ? gradeDto.grade : 0; }
    }

    public bool IsOpen
    {
        get { return ModelManager.Player.GetPlayerLevel() >= cfgVO.playerGradeLimit; }
    }

    public bool IsSCrafts
    {
        get { return cfgVO != null ? cfgVO.superCrafts : false; }
    }

    public int NextGrade
    {
       get { return Math.Min(Grade + 1,cfgVO.maxGrade); }
    }

    //升级限制等级
    public int LimitLevel
    {
        get
        {
            var formula = DataCache.GetStaticConfigValues(AppStaticConfigs.CRAFTS_UPGRADE_FORMULA);
            return ExpressionManager.UpgradeSkillCraftsRoleLevel(formula, Grade, cfgVO.playerGradeLimit);
        }
    }

    //解锁限制等级
    public int LimitOpenLevel
    {
        get
        {
            return cfgVO.playerGradeLimit;
        }
    }
    public int ID
    {
        get { return id; }
        private set { id = value; }
    }
}

public class RoleSkillMagicVO
{
    public int id;
    public bool isEquip;//已装备上该技能
    public Magic cfgVO;

    public string Name
    {
        get { return cfgVO != null ? cfgVO.name : ""; }
    }

    public Skill.ElementPropertyType ElementId
    {
        get { return (Skill.ElementPropertyType) cfgVO.elementId; }
    }

    public bool IsOpen
    {
        get;
        set;
    }

    public int ID
    {
        get{return id;}
        private set{ id = value; }
    }
}

public sealed partial class RoleSkillMainData:IRoleSkillMainData
{
    public Skill.SkillEnum curTab;
    public RoleSkillMainSCraftsState sCraftsState;
    public RoleSkillCraftsItemController curSelCtrl;//当前选中的技能

    private Dictionary<int,RoleSkillCraftsVO> craftsDict = new Dictionary<int, RoleSkillCraftsVO>();
    private Dictionary<int,RoleSkillMagicVO> magicDict = new Dictionary<int, RoleSkillMagicVO>();
    private List<RoleSkillMagicVO> magicDtoDic = new List<RoleSkillMagicVO>();
    private Dictionary<int, int> magicOpenDic = new Dictionary<int, int>();

    public RoleSkillCraftsItemController curSelMagicCtrl;//当前选中的魔法
    
    private SkillsDto allSkillsDto;
    public RoleSkillMainData()
    {

    }

    public void InitData()
    {
        var craftsCfgList = DataCache.getDicByCls<Skill>();

        var skillIDList = ModelManager.Player.GetPlayer().faction.crafts;
        for(var i = 0;i < skillIDList.Count;i++)
        {
            var id = skillIDList[i];
            var cfgVO = craftsCfgList[id] as Crafts;
            if(cfgVO != null)
            {
                craftsDict[id] = new RoleSkillCraftsVO() { id = id,cfgVO = cfgVO };
            }
        }
        
        InitMagicData();
    }
    private void InitMagicData()
    {
        var magicCfgList = DataCache.getDicByCls<Magic>();
        foreach (var id in magicCfgList.Keys)
        {
            var cfgVO = magicCfgList[id] as Magic;
            if (cfgVO != null)
            {
                magicDict[id] = new RoleSkillMagicVO() { id = id, cfgVO = cfgVO };
            }
        }
        var content = DataCache.GetStaticConfigValues(AppStaticConfigs.MAGIC_CELL_OPEN_GRADE);
        magicOpenDic = RoleSkillUtils.ParseAttr(content, ';', '|');
    }

    public void Dispose()
    {
        curSelMagicCtrl = null;
    }

    #region 更新数据

    public void UpdateSkillsDto(SkillsDto skillsDto)
    {
        allSkillsDto = skillsDto;
        UpdateCraftsList(allSkillsDto.craftsInfos);
        UpdateMagicList(skillsDto.magic);
    }

    private void UpdateCraftsList(List<CraftsGradeDto> list)
    {
        for(var i = 0;i < list.Count;i++)
        {
            UpdateCrafts(list[i]);
        }
    }

    public void UpdateCrafts(CraftsGradeDto dto)
    {
        var craftsVo = GetSkillCraftsVO(dto.id);
        if(craftsVo != null)
        {
            craftsVo.gradeDto = dto;
        }
    }

    public void UpdateDefaultSCrafts(DefaultSCraftsDto dto)
    {
        if(allSkillsDto != null)
        {
            allSkillsDto.defaultSCrafts = dto.id;
        }
        else
        {
            GameLog.ShowError("DefaultSCraftsDto来了，但是skillsDto还没到");
        }
        sCraftsState = RoleSkillMainSCraftsState.Normal;
    }

    public void UpdateMagicList(List<int> list)
    {
        ResetMagicEquip();
        for (var i = 0;i < list.Count;i++)
        {
            int idx = list[i];
            var magicVo = GetSkillMagicVO(idx);
            if (magicVo != null)
            {
                magicVo.isEquip = true;
                magicDtoDic.Add(magicVo);
            }
        }
    }

    #endregion

    public RoleSkillCraftsVO GetSkillCraftsVO(int id)
    {
        return craftsDict.ContainsKey(id) ? craftsDict[id] : null;
    }

    #region 对外访问接口 IRoleSkillMainData
    public Skill.SkillEnum CurTab { get { return curTab; } }
    
    public RoleSkillCraftsItemController CurSelCtrl { get { return curSelCtrl; } set { curSelCtrl = value; } }
    

    public Dictionary<int,RoleSkillCraftsVO> CraftsDict { get { return craftsDict; } }

    public int CurSCrafts { get { return allSkillsDto != null ? allSkillsDto.defaultSCrafts : 0; } }

    public RoleSkillMainSCraftsState SCraftsState{ get { return sCraftsState; } }

    public string GetCraftsDesc(int id)
    {
        var vo = GetSkillCraftsVO(id);
        string shortDes = vo.cfgVO.shortDescription;
        if (shortDes.Length > 16)
            shortDes.Insert(16, "               ");
        var str = string.Format("类型：{0}\n属性：{1}\n硬直：{2}\n吟唱：{3}\n消耗：{4} CP\n范围：{5}\n效果：{6}",
            RoleSkillUtils.GetSkillEnumName(Skill.SkillEnum.Crafts),
            RoleSkillUtils.GetElementPropertyTypeName((Skill.ElementPropertyType)vo.cfgVO.elementId),
            RoleSkillUtils.GetSTDescEnumName(vo.cfgVO.skillTimeAfter),
            RoleSkillUtils.GetSTDescEnumName(vo.cfgVO.skillTimeBefore),
            vo.cfgVO.consume,DataCache.getDtoByCls<SkillScope>(vo.cfgVO.scopeId).desc, shortDes);
        return str;
    }

    //自封装一下，方便后续统一入口修改，算的是下一级
    public CharacterCraftsGrade GetCostByGradeDto(RoleSkillCraftsVO vo)
    {
        return DataCache.getDtoByCls<CharacterCraftsGrade>(vo.NextGrade);
    }

    public RoleSkillMainRangeState GetScopeTarType(int id)
    {
        var vo = DataCache.getDtoByCls<SkillScope>(id);
        return vo.targetType ? RoleSkillMainRangeState.Enemy : RoleSkillMainRangeState.Friend;
    }

    #region 魔法相关

    public Dictionary<int, int> MagicOpenDic
    {
        get { return magicOpenDic; }
    }
    public List<RoleSkillMagicVO> MagicDtoDic { get { return magicDtoDic; } }
    public RoleSkillCraftsItemController CurSelMagicCtrl { get { return curSelMagicCtrl; } set { curSelMagicCtrl = value; } }

    public string GetMagicDesc(int id)
    {
        var vo = GetSkillMagicVO(id);
        var str = string.Format("类型：{0}\n属性：{1}\n硬直：{2}\n吟唱：{3}\n消耗：{4} EP\n范围：{5}\n\n效果：{6}",
            RoleSkillUtils.GetSkillEnumName(Skill.SkillEnum.Magic),
            RoleSkillUtils.GetElementPropertyTypeName((Skill.ElementPropertyType)vo.cfgVO.elementId),
            RoleSkillUtils.GetSTDescEnumName(vo.cfgVO.skillTimeAfter),
            RoleSkillUtils.GetSTDescEnumName(vo.cfgVO.skillTimeBefore),
            vo.cfgVO.consume,DataCache.getDtoByCls<SkillScope>(vo.cfgVO.scopeId).desc,vo.cfgVO.shortDescription);
        return str;
    }

    #endregion

    #endregion

    public int GetUpGradeNeedRoleLevel(RoleSkillCraftsVO vo)
    {
        var formula = DataCache.GetStaticConfigValues(AppStaticConfigs.CRAFTS_UPGRADE_FORMULA);
        return ExpressionManager.UpgradeSkillCraftsRoleLevel(formula,vo.NextGrade,vo.cfgVO.playerGradeLimit);
    }

    public RoleSkillMagicVO GetSkillMagicVO(int id)
    {
        return magicDict.ContainsKey(id) ? magicDict[id] : null;
    }
    

    private void ResetMagicEquip()
    {
        magicDtoDic.Clear();
        foreach(var vo in magicDict.Values)
        {
            vo.isEquip = false;
        }
    }

}
