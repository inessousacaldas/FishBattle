using System.Collections.Generic;
using AppDto;


public class CrewSkillPassiveData  
{
    //服务器传过来的技能数据
    private Dictionary<int, List<PassiveSkillDto>> psvDtoDic = new Dictionary<int, List<PassiveSkillDto>>();
    //客户端已学习了得技巧读表数据
    private Dictionary<int, CrewPassiveSkill> psvVODic = new Dictionary<int, CrewPassiveSkill>();
    //客户端所有技能读表数据
    private Dictionary<int, Skill> passiveCfgList;
    private List<CrewInfoDto> crewList;
    //所有技能书
    private List<PassiveSkillBook> allPsvBook = new List<PassiveSkillBook>();

    private Dictionary<int, CrewPassiveGrade> crewPsvGradeDic;
    private List<CrewPassiveGrade> crewPsvGradeList;

    private PsvItemData curPsvItemData;

    //学习成功时跳入属性面板，遗忘成功时跳入背包面板
    private PsvWindowType nextType = PsvWindowType.None;

    public void UpdatePassiveData(List<CrewInfoDto> crewList)
    {
        crewPsvGradeDic = DataCache.getDicByCls<CrewPassiveGrade>();
        crewPsvGradeList = DataCache.getArrayByCls<CrewPassiveGrade>();
        crewList = CrewViewDataMgr.DataMgr.GetCrewListInfo().ToList();
        passiveCfgList = DataCache.getDicByCls<Skill>();
        for (int i = 0, max = crewList.Count; i < max; i++)
        {
            var tmp = crewList[i].crewSkillsDto.passiveSkillDtos;
            psvDtoDic[crewList[i].crewId] = tmp;
            for (int j = 0, pMax = tmp.Count; j < pMax; j++)
            {
                if (passiveCfgList.ContainsKey(tmp[j].id))
                {
                    var VO = passiveCfgList[tmp[j].id] as CrewPassiveSkill;
                    if (VO != null)
                    {
                        psvVODic[tmp[j].id] = VO;
                    }
                }
            }
        }
    }
    public void InitData()
    {
        //crewPsvGradeDic = DataCache.getDicByCls<CrewPassiveGrade>();
        //crewPsvGradeList = DataCache.getArrayByCls<CrewPassiveGrade>();
        //crewList = CrewViewDataMgr.DataMgr.GetCrewListInfo().ToList();
        //passiveCfgList = DataCache.getDicByCls<Skill>();
        //for(int i = 0,max = crewList.Count; i < max; i++)
        //{
        //    var tmp = crewList[i].crewSkillsDto.passiveSkillDtos;
        //    psvDtoDic[crewList[i].crewId] = tmp;
        //    for (int j =0,pMax = tmp.Count; j < pMax; j++)
        //    {
        //        if (passiveCfgList.ContainsKey(tmp[j].id))
        //        {
        //            var VO = passiveCfgList[tmp[j].id] as CrewPassiveSkill;
        //            if (VO != null)
        //            {
        //                psvVODic[tmp[j].id] = VO;
        //            }
        //        }
        //    }
        //}
        InitAllPsvSkillBook();
    }
    public void UpdateDataByAddCrew(int id, List<PassiveSkillDto> dtoList)
    {
        if (!psvDtoDic.ContainsKey(id))
        {
            psvDtoDic.Add(id, dtoList);
        }
    }

    //初始化获得所有技巧的技能书
    public List<PassiveSkillBook> InitAllPsvSkillBook()
    {
        var tmp = DataCache.getDicByCls<GeneralItem>();
        tmp.ForEach((data) =>
        {
            var psv = data.Value as PassiveSkillBook;
            if (psv != null)
            {
                allPsvBook.Add(psv);
            }
        });
        return allPsvBook;
    }


    public void UpdateDataByLearn(CrewInfoDto dto,int bookId)
    {
        curPsvItemData = new PsvItemData();
        var psvSkillList = dto.crewSkillsDto.passiveSkillDtos; 
        psvDtoDic[CrewSkillHelper.CrewID] = psvSkillList;
        var psvSkill = passiveCfgList.Find(e => e.Value is CrewPassiveSkill && (e.Value as CrewPassiveSkill).itemId == bookId).Value;
        if(psvSkill is CrewPassiveSkill)
        {
            var psv = psvSkill as CrewPassiveSkill;
            var curDto = psvSkillList.Find(e => e.id == psv.id);
            curPsvItemData.psvDto = curDto;

            if (passiveCfgList.ContainsKey(curDto.id))
            {
                var VO = passiveCfgList[curDto.id] as CrewPassiveSkill;
                if (VO != null)
                {
                    psvVODic[curDto.id] = VO;
                }
            }
            if (psvVODic.ContainsKey(curDto.id))
                curPsvItemData.psvVO = psvVODic[curDto.id];
            curPsvItemData.state = PassiveState.HaveItem;
        }
        
    }

    //升级
    public void UpdateDataByUp(CrewInfoDto dto, int skillMapId)
    {
        if (psvDtoDic.ContainsKey(CrewSkillHelper.CrewID))
        {
            curPsvItemData = new PsvItemData();
            var psvSkillList = dto.crewSkillsDto.passiveSkillDtos;
            psvDtoDic[CrewSkillHelper.CrewID] = psvSkillList;
            var psvSkill = passiveCfgList.Find(e => e.Value.skillMapId  == skillMapId).Value;
            if (psvSkill is CrewPassiveSkill)
            {
                var psv = psvSkill as CrewPassiveSkill;
                var curDto = psvSkillList.Find(e => e.id == psv.id);
                curPsvItemData.psvDto = curDto;
                var VO = passiveCfgList[curDto.id] as CrewPassiveSkill;
                if (VO != null)
                {
                    psvVODic[curDto.id] = VO;
                    curPsvItemData.psvVO = VO;
                }
                curPsvItemData.state = PassiveState.HaveItem;
            }
        }
    }

    //使用
    public void UpdateDataByUp(PassiveSkillDto dto)
    {
        if (psvDtoDic.ContainsKey(CrewSkillHelper.CrewID))
        {
            curPsvItemData = new PsvItemData();
            var list = psvDtoDic[CrewSkillHelper.CrewID];
            for (int i = 0, max = list.Count; i < max; i++)
            {
                if (list[i].id == dto.id)
                {
                    psvDtoDic[CrewSkillHelper.CrewID][i] = dto;
                    curPsvItemData.psvDto = dto;
                    if (passiveCfgList.ContainsKey(list[i].id))
                    {
                        var VO = passiveCfgList[list[i].id] as CrewPassiveSkill;
                        if (VO != null)
                        {
                            psvVODic[list[i].id] = VO;
                            curPsvItemData.psvVO = VO;
                        }
                    }
                }
                curPsvItemData.state = PassiveState.HaveItem;
            }
        }
    }

    public void UpdatePsvDtoDic(CrewSkillsDto dto)
    {
        if (psvDtoDic.ContainsKey(CrewSkillHelper.CrewID))
        {
            psvDtoDic[CrewSkillHelper.CrewID] = dto.passiveSkillDtos;
        }
    }

    //通过伙伴ID获取伙伴技能信息
    public List<PassiveSkillDto> GetPsvDtoList(int id)
    {
        return psvDtoDic.ContainsKey(id) ? psvDtoDic[id] : null;
    }

    //通过技能ID获取技能读表信息
    public CrewPassiveSkill GetPsvVO(int id)
    {
        return psvVODic.ContainsKey(id) ? psvVODic[id] : null;
    }

    public List<PassiveSkillBook> AllPsvBooks
    {
        get { return allPsvBook; }
    }

    public int GetConsumeExp(int grade)
    {
        return crewPsvGradeDic.ContainsKey(grade) ? crewPsvGradeDic[grade].consume : 0;
    }

    public List<CrewPassiveGrade> GetPsvGradeList
    {
        get { return crewPsvGradeList; }
    }

    public PsvItemData GetPsvItemData
    {
        get { return curPsvItemData; }
    }
    public void SetPsvItemData(PsvItemData data)
    {
        curPsvItemData = new PsvItemData();
        curPsvItemData = data;
    }
    
    public void SetNextType(PsvWindowType type)
    {
        nextType = type;
    }

    public PsvWindowType GetNextType
    {
        get { return nextType; }
    }
}
