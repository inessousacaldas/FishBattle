using UnityEngine;
using System.Collections.Generic;
using AppDto;


public class CrewSkillTrainingVO
{
    #region 研修前
    //研修前战技等级信息
    public List<CrewSkillCraftsVO> befCraDto;
    //研修前成长率
    public int befGrow;
    //研修前是否最高
    public bool befMaxLevel;
    #endregion

    #region 研修后
    //研修后战技等级信息
    public List<CrewSkillCraftsVO> aftCraDto;
    //研修后成长率
    public int aftGrow;
    //研修后是否最高
    public bool aftMaxLevel;
    #endregion
}

public partial class CrewSkillTrainData: ICrewSkillTrainData
{

    private Dictionary<int, CrewSkillTrainingVO> trainDic = new Dictionary<int, CrewSkillTrainingVO>();
    private Dictionary<int, Skill> craftsCfgList;
    public void InitData(List<CrewInfoDto> crewList)
    {
        if (craftsCfgList != null) return;
        craftsCfgList = DataCache.getDicByCls<Skill>();
        for (int i = 0, max = crewList.Count; i < max; i++)
        {
            CrewSkillTrainingVO tmpVO = new CrewSkillTrainingVO();
            //研修前
            var befSkillDto = crewList[i].crewSkillsDto;
            var craftsBefListVO = new List<CrewSkillCraftsVO>();
            for (int j = 0; j < befSkillDto.craftsGradeDtos.Count; j++)
            {
                craftsBefListVO.Add(new CrewSkillCraftsVO());
                craftsBefListVO[j].gradeDto = befSkillDto.craftsGradeDtos[j];
                craftsBefListVO[j].belongCrew = crewList[i].crewId;
                if (craftsCfgList.ContainsKey(befSkillDto.craftsGradeDtos[j].id))
                {
                    var cfgVO = craftsCfgList[befSkillDto.craftsGradeDtos[j].id] as Crafts;
                    if (cfgVO != null)
                    {
                        craftsBefListVO[j].cfgVO = cfgVO;
                    }
                }
            }
                tmpVO.befCraDto = craftsBefListVO;
            tmpVO.befGrow = befSkillDto.extraGrow;
            tmpVO.befMaxLevel = befSkillDto.maxLevel;

            //研修后
            var aftSkillDto = crewList[i].crewSkillsDto.craftsTrainingDto;
            var craftsAftListVO = new List<CrewSkillCraftsVO>();
            for (int j = 0; j < aftSkillDto.craftsGradeDtos.Count; j++)
            {
                craftsAftListVO.Add(new CrewSkillCraftsVO());
                craftsAftListVO[j].gradeDto = aftSkillDto.craftsGradeDtos[j];
                craftsAftListVO[j].belongCrew = crewList[i].crewId;
                if (craftsCfgList.ContainsKey(aftSkillDto.craftsGradeDtos[j].id))
                {
                    var cfgVO = craftsCfgList[aftSkillDto.craftsGradeDtos[j].id] as Crafts;
                    if (cfgVO != null)
                    {
                        craftsAftListVO[j].cfgVO = cfgVO;
                    }
                }
            }
                tmpVO.aftCraDto = craftsAftListVO;
            tmpVO.aftGrow = aftSkillDto.extraGrow;
            tmpVO.aftMaxLevel = aftSkillDto.maxLevel;
            trainDic[crewList[i].crewId] = tmpVO;
        }
    }

    public void UpdateTrainListByDto(int id, CrewInfoDto dto)
    {
        if (!trainDic.ContainsKey(id))
        {
            trainDic.Add(id, new CrewSkillTrainingVO());
        }
        CrewSkillTrainingVO tmpVO = new CrewSkillTrainingVO();
        //研修前
        var befSkillDto = dto.crewSkillsDto;
        var craftsBefListVO = new List<CrewSkillCraftsVO>();
        for (int j = 0; j < befSkillDto.craftsGradeDtos.Count; j++)
        {
            craftsBefListVO.Add(new CrewSkillCraftsVO());
            craftsBefListVO[j].gradeDto = befSkillDto.craftsGradeDtos[j];
            craftsBefListVO[j].belongCrew = dto.crewId;
            if (craftsCfgList.ContainsKey(befSkillDto.craftsGradeDtos[j].id))
            {
                var cfgVO = craftsCfgList[befSkillDto.craftsGradeDtos[j].id] as Crafts;
                if (cfgVO != null)
                {
                    craftsBefListVO[j].cfgVO = cfgVO;
                }
            }
        }
        tmpVO.befCraDto = craftsBefListVO;
        tmpVO.befGrow = befSkillDto.extraGrow;
        tmpVO.befMaxLevel = befSkillDto.maxLevel;

        //研修后
        var aftSkillDto = dto.crewSkillsDto.craftsTrainingDto;
        var craftsAftListVO = new List<CrewSkillCraftsVO>();
        for (int j = 0; j < aftSkillDto.craftsGradeDtos.Count; j++)
        {
            craftsAftListVO.Add(new CrewSkillCraftsVO());
            craftsAftListVO[j].gradeDto = aftSkillDto.craftsGradeDtos[j];
            craftsAftListVO[j].belongCrew = dto.crewId;
            if (craftsCfgList.ContainsKey(aftSkillDto.craftsGradeDtos[j].id))
            {
                var cfgVO = craftsCfgList[aftSkillDto.craftsGradeDtos[j].id] as Crafts;
                if (cfgVO != null)
                {
                    craftsAftListVO[j].cfgVO = cfgVO;
                }
            }
        }
        tmpVO.aftCraDto = craftsAftListVO;
        tmpVO.aftGrow = aftSkillDto.extraGrow;
        tmpVO.aftMaxLevel = aftSkillDto.maxLevel;
        trainDic[id] = tmpVO;
    }
    
    /// <summary>
    /// 战技研修时的dto
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    public void UpdateTrainListByDto(int id, CraftsTrainingDto dto)
    {
        if (!trainDic.ContainsKey(id))
        {
            trainDic.Add(id, new CrewSkillTrainingVO());
        }
        List<CrewSkillCraftsVO> craftsAftListVO = new List<CrewSkillCraftsVO>();
        trainDic[id].aftGrow = dto.extraGrow;
        trainDic[id].aftMaxLevel = dto.maxLevel;
        for (int i = 0, max = dto.craftsGradeDtos.Count; i < max; i++)
        {
            craftsAftListVO.Add(new CrewSkillCraftsVO());
            craftsAftListVO[i].gradeDto = dto.craftsGradeDtos[i];
            craftsAftListVO[i].belongCrew = id;
            if (craftsCfgList.ContainsKey(dto.craftsGradeDtos[i].id))
            {
                var cfgVO = craftsCfgList[dto.craftsGradeDtos[i].id] as Crafts;
                if (cfgVO != null)
                {
                    craftsAftListVO[i].cfgVO = cfgVO;
                }
            }
        }
        trainDic[id].aftCraDto = craftsAftListVO;
    }

    /// <summary>
    /// 战技保存时的dto
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    public void UpdateTrainListByDto(int id,CraftsTrainingSaveDto dto)
    {
        if (trainDic.ContainsKey(id))
        {
            trainDic[id].befGrow = dto.extraGrow;
            trainDic[id].befMaxLevel = dto.maxLevel;
            List<CrewSkillCraftsVO> craftsBefListVO = new List<CrewSkillCraftsVO>();
            for (int i = 0,max = dto.craftsGradeDtos.Count; i < max; i++)
            {
                craftsBefListVO.Add(new CrewSkillCraftsVO());
                craftsBefListVO[i].gradeDto = dto.craftsGradeDtos[i];
                craftsBefListVO[i].belongCrew = id;
                if (craftsCfgList.ContainsKey(dto.craftsGradeDtos[i].id))
                {
                    var cfgVO = craftsCfgList[dto.craftsGradeDtos[i].id] as Crafts;
                    if (cfgVO != null)
                    {
                        craftsBefListVO[i].cfgVO = cfgVO;
                    }
                }
            }
            trainDic[id].befCraDto = craftsBefListVO;

            trainDic[id].aftGrow = dto.craftsTrainingDto.extraGrow;
            trainDic[id].aftMaxLevel = dto.craftsTrainingDto.maxLevel;
            List<CrewSkillCraftsVO> craftsAftListVO = new List<CrewSkillCraftsVO>();
            for (int i = 0, max = dto.craftsTrainingDto.craftsGradeDtos.Count; i < max; i++)
            {
                craftsAftListVO.Add(new CrewSkillCraftsVO());
                craftsAftListVO[i].gradeDto = dto.craftsTrainingDto.craftsGradeDtos[i];
                craftsAftListVO[i].belongCrew = id;
                if (craftsCfgList.ContainsKey(dto.craftsTrainingDto.craftsGradeDtos[i].id))
                {
                    var cfgVO = craftsCfgList[dto.craftsTrainingDto.craftsGradeDtos[i].id] as Crafts;
                    if (cfgVO != null)
                    {
                        craftsAftListVO[i].cfgVO = cfgVO;
                    }
                }
            }
            trainDic[id].aftCraDto = craftsAftListVO;
        }
    }



    public CrewSkillTrainingVO GetTrainList(int id)
    {
        return trainDic.ContainsKey(id) ? trainDic[id] : null;
    }

    public CrewTraining GetCrewTraining(int rare)
    {
        return DataCache.getDtoByCls<CrewTraining>(rare);
    }
}
