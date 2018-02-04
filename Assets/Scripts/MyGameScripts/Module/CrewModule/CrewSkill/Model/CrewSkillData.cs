// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 8/3/2017 5:07:05 PM
// **********************************************************************

using AppDto;
using System.Collections.Generic;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule;
using System;

public partial interface ICrewSkillData
{

    Dictionary<int, int> MagicOpenDic { get; }
    List<CrewSkillMagicVO> GetSkillMagic(int id);
    ICrewSkillTrainData GetCrewSkillTrainData();

    #region 战技
    List<CrewSkillCraftsVO> GetSkillCrafts(int id);
    void UpdateCraftsDataByDto(int id, CraftsGradeDto dto);
    #endregion

    #region 技巧
    //传伙伴ID
    List<PassiveSkillDto> GetPsvDtoList(int id);
    //传技能ID
    CrewPassiveSkill GetPsvVO(int id);
    List<PassiveSkillBook> AllPsvBooks { get; }
    int GetConsumeExp(int grade);
    List<CrewPassiveGrade> GetPsvGradeList { get; }
    PsvItemData GetPsvItemData { get; }
    void SetPsvItemData(PsvItemData data);
    void SetNextType(PsvWindowType type);
    PsvWindowType GetNextType { get; }
    #endregion
}
public interface ICrewSkillVO
{
    string Name { get; }
    string SkillDes { get; }
    string SkillType { get; }
    int Grade { get; }
    string Icon { get; }

}


/// <summary>
/// 研修接口
/// </summary>
public interface ICrewSkillTrainData
{
    CrewSkillTrainingVO GetTrainList(int id);
    CrewTraining GetCrewTraining(int rare);
    void UpdateTrainListByDto(int id, CraftsTrainingDto dto);
    void UpdateTrainListByDto(int id, CrewInfoDto dto);
}

public class CrewSkillMagicVO:ICrewSkillVO
{

    public int id;
    public int crewCurGrade;//此伙伴
    public Magic magicVO;

    public bool IsOpen
    {
        get { return true; }
    }

    public string Name
    {
        get
        {
            return magicVO != null ? magicVO.name : "";
        }
    }

    public string SkillDes
    {
        get
        {
            return magicVO.shortDescription;
        }
    }

    public string SkillType
    {
        get
        {
            return RoleSkillUtils.GetElementPropertyTypeName((Skill.ElementPropertyType)magicVO.elementId);
        }
    }

    public int Grade
    {
        get
        {
            return magicVO != null ? magicVO.grade : 0;
        }
    }

    public string Icon
    {
        get
        {
            return magicVO.icon;
        }
    }

    //硬直（释放技能后ST值）
    public string SkillTimeAfter
    {
        get { return RoleSkillUtils.GetSTDescEnumName(magicVO.skillTimeAfter); }
    }
    //吟唱（吟唱阶段ST值(如果有)）
    public string SkillTimeBefore
    {
        get { return RoleSkillUtils.GetSTDescEnumName(magicVO.skillTimeBefore); }
    }

    //范围
    public string Scope
    {
        get { return CrewSkillHelper.GetCraftsData().GetScopeByID(magicVO.scopeId).desc; }
    }
}


/// <summary>
/// 检出一个出门存放伙伴信息的类
/// 因为某些数据需要伙伴id来索引，然后整个伙伴技能系统需要比较品质等
/// </summary>
public class CrewTmpInfo
{
    /// <summary>
    /// 伙伴唯一ID
    /// </summary>
    public long Id;
    //伙伴当前等级
    public int crewCurGrade;
    //伙伴品质
    public int crewQuality;
}

/// <summary>
/// 伙伴技能选项（战技，魔法，技巧）
/// </summary>
public enum CrewSkillTab
{
    Crafts,
    Magic,
    Passive
}


public sealed partial class CrewSkillDataMgr
{
    public sealed partial class CrewSkillData:ICrewSkillData
    {
        private CrewSkillTab mainTab = CrewSkillTab.Crafts;
        public ICrewSkillWindowController windowView;
        private Dictionary<int, List<CrewSkillMagicVO>> magicDic = new Dictionary<int, List<CrewSkillMagicVO>>();
        private Dictionary<int, int> magicOpenDic = new Dictionary<int, int>();

        //存储我们需要的伙伴信息，为了快速查找伙伴的一些基本信息
        private Dictionary<int, CrewTmpInfo> crewTmpDic = new Dictionary<int, CrewTmpInfo>();
        //伙伴信息列表，存储着所有的伙伴信息
        private IEnumerable<CrewInfoDto> crewList = new List<CrewInfoDto>();
        //战技
        public CrewSkillCraftsData craftsData ;
        //研修
        private CrewSkillTrainData trainData;
        //技巧
        public CrewSkillPassiveData passiveData;
        

        public void InitData()
        {
            InitDatas();
            UpdateCrewTmpData();
            InitMagicData();
        }

        public void Dispose()
        {

        }
        #region 初始化战技，魔法，技巧数据
        public void InitDatas()
        {
            craftsData = new CrewSkillCraftsData();
            craftsData.InitData(crewList.ToList());
            //trainData = new CrewSkillTrainData();
            //trainData.InitData(crewList.ToList());
            passiveData = new CrewSkillPassiveData();
            passiveData.InitData();
        }
        
        public void UpdateCrewTmpData()
        {
            crewList = CrewViewDataMgr.DataMgr.GetCrewListInfo();
            for (int i = 0, max = crewList.ToList().Count; i < max; i++)
            {
                var crew = crewList.ToList()[i];
                crewTmpDic[crew.crewId] = new CrewTmpInfo() { Id = crew.id, crewCurGrade = crew.grade, crewQuality = crew.quality };
            }
        }
        private Dictionary<int, Magic> magicCfgList = null;
        private Dictionary<int, Magic> MagicCfgList
        {
            get
            {
                if (magicCfgList == null)
                {
                    magicCfgList = DataCache.getDicByCls<Magic>();
                }
                return magicCfgList;
            }
        }
        private void InitMagicData()
        {
            
            var content = DataCache.GetStaticConfigValues(AppStaticConfigs.MAGIC_CELL_OPEN_GRADE);
            magicOpenDic = RoleSkillUtils.ParseAttr(content, ';','|');
        }

        public void UpdateMagicData(List<CrewInfoDto> list)
        {
            for (int i = 0, max = list.Count; i < max; i++)
            {
                var crew = list[i];
                var curMagicDataList = crew.crewSkillsDto.magic;
                UpdateMagicData(crew.crewId, curMagicDataList);
            }
        }

        
        public void UpdateMagicData(int id,List<int> list)
        {
            List<CrewSkillMagicVO> tmpDic = new List<CrewSkillMagicVO>();
            for (int j = 0, max = list.Count; j < max; j++)
            {
                //目前只有id
                var magicId = list[j];
                if (MagicCfgList.ContainsKey(magicId))
                {
                    var vo = MagicCfgList[magicId];
                    if (vo != null)
                        tmpDic.Add(new CrewSkillMagicVO() { id = magicId, magicVO = MagicCfgList[magicId] });
                }
            }
            magicDic[id] = tmpDic;
        }
        private void InitTechnicData()
        {
            //初始化技巧数据
        }
        #endregion

        #region 获取数据

        public List<CrewSkillMagicVO> GetSkillMagic(int id)
        {
            return magicDic.ContainsKey(id) ? magicDic[id] : null;
        }
        public void UpdateCrewTmpDic(int id,CrewInfoDto dto)
        {
            if (!crewTmpDic.ContainsKey(id))
            {
                crewTmpDic.Add(id,new CrewTmpInfo() { Id = dto.id, crewCurGrade = dto.grade, crewQuality = dto.quality });
            }
        }
        public Dictionary<int, CrewTmpInfo> GetCrewInfo()
        {
            return crewTmpDic;
        }

        public ICrewSkillTrainData GetCrewSkillTrainData()
        {
            return trainData;
        }
        

        #endregion

        #region 修改数据

        //当前点击界面是战技或是魔法或是技巧
        public CrewSkillTab MainTab
        {
            get { return mainTab; }
            set { mainTab = value; }
        }

        #endregion

        #region 战技
        public List<CrewSkillCraftsVO> GetSkillCrafts(int id)
        {
            return craftsData.GetSkillCrafts(id);
        }

        public void UpdateCraftsDataByDto(int id, CraftsGradeDto dto)
        {
            craftsData.UpdateCraftsDataByDto(id, dto);
        }

        #endregion

        #region 魔法

        public Dictionary<int,int> MagicOpenDic
        {
            get { return magicOpenDic; }
        }
        #endregion

        #region 技巧
        public void UpdataPsvByAddCrew(int id,List<PassiveSkillDto> dtoList)
        {
            passiveData.UpdateDataByAddCrew(id, dtoList);
        }
        public void UpdatePsvDataByLearn(CrewInfoDto dto,int bookId)
        {
            passiveData.UpdateDataByLearn(dto,bookId);
        }

        public void UpdatePsvDataByUp(CrewInfoDto dto, int skillMapId)
        {
            passiveData.UpdateDataByUp(dto, skillMapId);
        }

        public void UpdatePsvDataByUse(PassiveSkillDto dto)
        {
            passiveData.UpdateDataByUp(dto);
        }

        public void UpdatePsvDtoDic(CrewSkillsDto dto)
        {
            passiveData.UpdatePsvDtoDic(dto);
        }
        //穿伙伴ID
        public List<PassiveSkillDto> GetPsvDtoList(int id)
        {
            return passiveData.GetPsvDtoList(id);
        }
        //传技能ID
        public CrewPassiveSkill GetPsvVO(int id)
        {
            return passiveData.GetPsvVO(id);
        }
        //所有技巧的技能书
        public List<PassiveSkillBook> AllPsvBooks
        {
            get { return passiveData.AllPsvBooks; }
        }
        //消耗的总经验
        public int GetConsumeExp(int grade)
        {
            return passiveData.GetConsumeExp(grade);
        }
        //消耗材料列表
        public List<CrewPassiveGrade> GetPsvGradeList
        {
            get { return passiveData.GetPsvGradeList; }
        }
        //当前选择的技巧
        public PsvItemData GetPsvItemData
        {
            get { return passiveData.GetPsvItemData; }
        }
        //设置当前选择的技巧
        public void SetPsvItemData(PsvItemData data)
        {
            passiveData.SetPsvItemData(data);
        }
        //主要是用于学习成功和遗忘成功的界面转换
        public void SetNextType(PsvWindowType type)
        {
            passiveData.SetNextType(type);
        }
        //主要是用于学习成功和遗忘成功的界面转换
        public PsvWindowType GetNextType
        {
            get { return passiveData.GetNextType; }
        }
        #endregion


        public void SetWindowCtrl(ICrewSkillWindowController ctrl)
        {
            windowView = ctrl;
        }
    }
}

/// <summary>
/// 快速索引某个数据
/// </summary>
public static class CrewSkillHelper
{
    /// <summary>
    /// 战技信息
    /// </summary>
    /// <returns></returns>
    public static CrewSkillCraftsData GetCraftsData()
    {
        return CrewSkillDataMgr.DataMgr.CraftsData;
    }
    
    public static int CrewID
    {
        get{ return CrewViewDataMgr.DataMgr.GetCurCrewID; }
    }
}