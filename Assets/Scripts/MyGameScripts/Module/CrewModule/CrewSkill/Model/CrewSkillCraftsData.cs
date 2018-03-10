using AppDto;
using System.Collections.Generic;
using System;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule;

public class CrewSkillCraftsVO: ICrewSkillVO
{
    /// <summary>
    /// 战技ID
    /// </summary>
    public int id;
    /// <summary>
    /// 所属伙伴id，此数据有点不合理，因为是为了反索引获得伙伴数据，后期有待优化
    /// </summary>
    public int belongCrew;
    public CraftsGradeDto gradeDto;
    public Crafts cfgVO;

    public string Name
    {
        get { return cfgVO != null ? cfgVO.name : ""; }
    }

    //技能属性(风火雷电等)
    public string SkillType
    {
        get { return RoleSkillUtils.GetElementPropertyTypeName((Skill.ElementPropertyType)cfgVO.elementId); }
    }
    //技能效果
    public string SkillDes
    {
        get { return cfgVO.shortDescription; }
    }
    //等级
    public int Grade
    {
        get { return gradeDto != null ? gradeDto.grade : 0; }
    }
    //图标名称
    public string Icon
    {
        get { return cfgVO.icon; }
    }
    //是否S战技
    public bool IsSuperCrafts
    {
        get { return cfgVO.superCrafts; }
    }
    //硬直（释放技能后ST值）
    public string SkillTimeAfter
    {
        get { return RoleSkillUtils.GetSTDescEnumName(cfgVO.skillTimeAfter); }
    }
    //吟唱（吟唱阶段ST值(如果有)）
    public string SkillTimeBefore
    {
        get { return RoleSkillUtils.GetSTDescEnumName(cfgVO.skillTimeBefore); }
    }
    //范围
    public string Scope
    {
        get { return CrewSkillHelper.GetCraftsData().GetScopeByID(cfgVO.scopeId).desc; }
    }

    public int NextGrade
    {
        get { return Math.Min(Grade + 1, cfgVO.maxGrade); }
    }
    //public bool IsOpen
    //{
        //get { return CrewSkillHelper.GetCrewInfo(belongCrew).crewCurGrade >= cfgVO.playerGradeLimit; }
    //}
}


public sealed class CrewSkillCraftsData 
{
    public CrewSkillCraftsVO curSelCrafVO;      //当前选中的技能

    private Dictionary<int, List<CrewSkillCraftsVO>> craftsDic = new Dictionary<int, List<CrewSkillCraftsVO>>();
    private Dictionary<int, Skill> craftsCfgList;
    public void InitData(List<CrewInfoDto> crewList)
    {
        //craftsCfgList = DataCache.getDicByCls<Skill>();
        //for (int i = 0, max = crewList.Count; i < max; i++)
        //{
        //    var crew = crewList[i];
        //    var curCraftsDataList = crew.crewSkillsDto.craftsGradeDtos;
        //    UpdateCraftsData(curCraftsDataList, craftsCfgList, crew);
        //}
    }

    public void UpdateCraftsData(List<CrewInfoDto> crewList)
    {
        craftsCfgList = DataCache.getDicByCls<Skill>();
        for (int i = 0, max = crewList.Count; i < max; i++)
        {
            var crew = crewList[i];
            var curCraftsDataList = crew.crewSkillsDto.craftsGradeDtos;
            UpdateCraftsData(curCraftsDataList, craftsCfgList, crew);
        }
    }

    private void UpdateCraftsData(List<CraftsGradeDto> list, Dictionary<int, Skill> dic, CrewInfoDto crew)
    {
        List<CrewSkillCraftsVO> tmpDic = new List<CrewSkillCraftsVO>();
        for (int j = 0, craListCount = list.Count; j < craListCount; j++)
        {
            var craftsData = list[j];
            int id = craftsData.id;
            if (dic.ContainsKey(id))
            {
                var cfgVO = dic[id] as Crafts;
                if (cfgVO != null)
                {
                    tmpDic.Add(new CrewSkillCraftsVO()
                    {
                        id = id,
                        cfgVO = cfgVO,
                        gradeDto = craftsData,
                        belongCrew = crew.crewId
                    });
                    craftsDic[crew.crewId] = tmpDic;
                }
            }
        }
    }

    public void UpdateCraftsDataByDto(int id, CraftsGradeDto dto)
    {
        if (craftsDic.ContainsKey(id))
        {
            for(int i = 0, max = craftsDic[id].Count; i < max; i++)
            {
                if(craftsDic[id][i].gradeDto.id == dto.id)
                {
                    craftsDic[id][i].gradeDto = dto;
                }
            }
        }
    }
    public void UpdateCraftsDataByTrain(int id, CrewSkillsDto dto)
    {
        List<CrewSkillCraftsVO> tmpDic = new List<CrewSkillCraftsVO>();
        if (!craftsDic.ContainsKey(id))
        {
            craftsDic.Add(id, tmpDic);
        }
        var list = dto.craftsGradeDtos;
        for (int j = 0, craListCount = list.Count; j < craListCount; j++)
        {
            var craftsData = list[j];
            int iid = craftsData.id;
            if (craftsCfgList != null && craftsCfgList.ContainsKey(iid))
            {
                var cfgVO = craftsCfgList[iid] as Crafts;
                if (cfgVO != null)
                {
                    tmpDic.Add(new CrewSkillCraftsVO()
                    {
                        id = id,
                        cfgVO = cfgVO,
                        gradeDto = craftsData,
                        belongCrew = id
                    });
                    craftsDic[id] = tmpDic;
                }
            }
        }
    }

    //获取战技信息，是后期改过来的，为了将数据分开
    public List<CrewSkillCraftsVO> GetSkillCrafts(int id)
    {
        return craftsDic.ContainsKey(id) ? craftsDic[id] : null;
    }


    public CrewCraftsGrade GetCostByGradeDto(CrewSkillCraftsVO vo)
    {
        return DataCache.getDtoByCls<CrewCraftsGrade>(vo.NextGrade);
    }
    public SkillScope GetScopeByID(int id)
    {
        return DataCache.getDtoByCls<SkillScope>(id);
    }

    public RoleSkillMainRangeState GetScopeTarType(int id)
    {
        var vo = GetScopeByID(id);
        return vo.targetType ? RoleSkillMainRangeState.Enemy : RoleSkillMainRangeState.Friend;
    }
    
}
