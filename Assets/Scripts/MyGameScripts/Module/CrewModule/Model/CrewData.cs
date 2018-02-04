// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 7/27/2017 7:53:13 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;

public interface ICrewViewData
{
    ICrewInfoData PartnerInfoData { get; }
    ICrewFetterData CrewFetterData { get; }
    ICrewFavorableData CrewFavorableData { get; }
    ICrewUpGradeData CrewUpGradeData { get; }
    PartnerTab GetCurCrewTab { get; }
    Dictionary<int, Crew> GetAllCrewDict();
    List<Crew> GetAllCrewList();

    IEnumerable<CrewInfoDto> GetSelfCrew();
    IEnumerable<ICrewBookData> GetCrewListByType(PropertyType type = PropertyType.All);
    ICrewBookData GetCrewByType(PropertyType type = PropertyType.All);
    int GetCurCrewId { get; }
    int GetNextRaise { get; }

    IEnumerable<CrewChipDto> GetChipList { get; }
    int GetNextPhase { get; }
    long GetFollowCrewId { get; }
    CrewInfoDto IsHadCurPantner(int id);
    IEnumerable<CrewInfoDto> GetCrewInfoByType(PropertyType type);
    ICrewSkillTrainData GetCrewSkillTrainData();
    Crew GetCrewDataById(int id);
    IEnumerable<ICrewBookData> GetCrewBookList { get; }
    long GetCurCrewUId { get; }
    BuyCrew GetBuyCrew { get; set; }
}

public interface ICrewInfoData
{
    int GetCurCrewId { get; }

    CrewInfoDto IsHadCurPantner(int id);
    CrewInfoTab GetCurInfoTab { get; }
}
public interface ICrewFetterData
{
    /// <summary>
    /// 当前选中的羁绊Id
    /// </summary>
    int GetCurCrewFetterId { get; }

    ICrewFetterItemVo GetCurCrewFetterVo { get; }
    //CrewFetter 
    List<ICrewFetterItemVo> CrewFetterVoList { get; }
}
public interface ICrewFetterItemVo
{
    long CrewUId { get; }
    int CrweId { get; }
    int CrewFetterId { get; }
    bool Acitve { get; } //是否激活
    Dictionary<Crew, bool> CrewDic { get; }
    CrewFetter CrewFetter { get; }
    CrewFetterDto crewFetterDto { get; }
}

public interface ICrewFavorableData
{
    ICrewBookData GetBookDataById(int crewId);
    int GetCurFavorIdx { get; }
    bool FavorableShowType { get; }
}

public interface ICrewUpGradeData
{
    ICrewBookData GetBookDataById(int crewId);
}

public class CrewFetterItemVo : ICrewFetterItemVo
{
    CrewFetterDto dto;

    long crewUid;
    int crewId;
    private Dictionary<Crew, bool> crewDic;
    CrewFetter crewFetter;
    public bool Acitve
    {
        get
        {
            return dto.active;
        }
    }

    public int CrewFetterId
    {
        get
        {
            return dto.crewFetterId;
        }
    }

    public long CrewUId
    {
        get
        {
            return crewUid;
        }
    }

    public int CrweId
    {
        get
        {
            return crewId;
        }
    }

    public Dictionary<Crew, bool> CrewDic
    {
        get
        {
            return crewDic;
        }
    }

    public CrewFetter CrewFetter
    {
        get
        {
            return crewFetter;
        }
    }

    public CrewFetterDto crewFetterDto
    {
        get
        {
            return dto;
        }
    }

    public CrewFetterItemVo(CrewFetterDto dto, int crewId, long crewUid, Dictionary<Crew, bool> crewDic, CrewFetter crewfetter)
    {
        this.dto = dto;
        this.crewId = crewId;
        this.crewUid = crewUid;
        this.crewDic = crewDic;
        this.crewFetter = crewfetter;
    }
}

public enum CrewInfoTab
{
    InfoTab = 1,
    FetterTab = 2    
}

public enum PartnerTab{
    Unkonwn = -1,        
    Info = 0,          //伙伴属性
    Skill = 1,         //伙伴技能
    Cultivate = 2,     //伙伴培养
    Favorable = 3      //好感度
}

public enum CrewQuality
{
    Blue = 3,
    Purple = 4,
    Orange = 5,
    Red = 6
}

public enum StrengthEffect
{
    EffectOne = 5,     //强化第一种特效
    EffectTwo = 8,     //强化第二种特效
    EffectThree = 10   //强化第三种特效
}
public enum PropertyType
{
    /** 0全部 */
    All = 0,
    /** 1力量 */
    Power = 1,
    /** 2魔法 */
    Magic = 2,
    /** 3控制 */
    Control = 3,
    /** 4辅助 */
    Treat = 4,
    
}

public enum CurListType
{
    None = 0,
    List = 1,
    Tiled = 2
}


public enum BuyCrew {
    None,
    Init,
    BuyIng
}

public enum CrewStatueType
{
    None = 0,
    Main = 1,
    Auxiliary = 2
}


public sealed partial class CrewViewDataMgr
{
    public sealed partial class CrewViewData:
        ICrewViewData
        ,ICrewInfoData
        ,ICrewFetterData
        , ICrewFavorableData
        , ICrewUpGradeData
    {
        #region inplememt ICrewViewData

        public ICrewInfoData PartnerInfoData { get { return this; }}
        public ICrewFavorableData CrewFavorableData { get { return this; } }
        public ICrewUpGradeData CrewUpGradeData { get { return this; } }

        public ICrewFetterData CrewFetterData
        {
            get
            {
                return this;
            }
        }

        #endregion

        public static Comparison<ICrewBookData> _comparison = null;
        public CrewViewData()
        {

        }

        public void InitData()
        {
            InitCrewListData();
        }

        public void Dispose()
        {
            
        }


        public BuyCrew GetBuyCrew { get { return buyCrew; } set { buyCrew = value; } }
        public BuyCrew buyCrew=BuyCrew.None;

        public int CdTimeThirty = 30;   //30秒cd
        
        #region 更新数据
        /// <param name="dataList"></param>
        /// <param name="isClear">是否清除原来的数据，如果是只是刷新部分数据的话isClear为False</param>
        public void UpdateCrewList(List<CrewInfoDto> dataList,bool isClear = true)
        {
            if (isClear)
                _selfCrewList.Clear();
            foreach(var c in dataList)
            {
                _selfCrewList.ReplaceOrAdd(n => n.id == c.id, c);
            }
            UpdateCrewFetterVoList();
            InitTrainData();
            UpdateCrewBookList();
        }

        public void UpdateCrewDto(CrewInfoDto dto)
        {
            _selfCrewList.ReplaceOrAdd(n => n.id == dto.id, dto);
            UpdateCrewFetterVoList();
            UpdateCrewBookList();
        }

        public void SetCurCrewId(int id = 0,long uid = 0)
        {
            if (id == 0 && uid == 0)
            {
                _allCrewList.ForEachI((d,idx)=>
                {
                    if (idx == 0)
                        _curCrewId = d.Value.id;
                });
                _curCrewUId = 0;
                return;
            }
            _curCrewId = id;
            _curCrewUId = uid;
            UpdateCrewFetterVoList();
        }

        public void SetNextPhaseAndRaise(int curPhase, int curRaise)
        {
            _nextRaise = curPhase + 1;
            _nextPhase = curPhase + 1;
        }

        public void SetCurCrewTab(PartnerTab tab)
        {
            _curCrewTab = tab;
        }

        public void UpdateChipList(List<CrewChipDto> list)
        {
            list.ForEach(dto =>
            {
                _allChipList.ReplaceOrAdd(d=>d.chipId == dto.chipId, dto);
            });
        }

        public void SetFollowCrewId(long id)
        {
            _followCrewId = id;
        }

        public void UpdateCrewBookList()
        {
            _allCrewList.ForEach(crew =>
            {
                var dto = _selfCrewList.Find(d => d.crewId == crew.Value.id);
                var chip = _allChipList.Find(d => d.chipId == crew.Value.id);
                var bookData = CrewBookData.Create(crew.Value.id, crew.Value, dto, 
                    dto == null ? CrewStatueType.None :(CrewStatueType)dto.battleCrewType,
                    chip == null ? 0 : chip.chipAmount);
                _crewBookList.ReplaceOrAdd(d=>d.GetCrewId == crew.Value.id, bookData);
            });

            CrewBookListSort();
        }

        private void CrewBookListSort()
        {
            if (_comparison == null)
            {
                _comparison = (a, b) =>
                {
                    var aState = GetCrewChipState(a);
                    var bState = GetCrewChipState(b);
 
                    if (aState == 2 && bState == 2)
                    {
                        var aType = a.GetInfoDto.battleCrewType;
                        var bType = b.GetInfoDto.battleCrewType;

                        if (aType == bType)
                        {
                            var aPower = a.GetInfoDto.power;
                            var bPower = b.GetInfoDto.power;
                            if (aPower == bPower)
                            {
                                var aRaise = a.GetInfoDto.raise;
                                var bRaise = b.GetInfoDto.raise;
                                if (aRaise == bRaise)
                                {
                                    var aQuality = a.GetInfoDto.quality;
                                    var bQuality = b.GetInfoDto.quality;
                                    if (aQuality == bQuality)
                                    {
                                        var aLv = a.GetInfoDto.grade;
                                        var bLv = b.GetInfoDto.grade;
                                        if (aLv == bLv)
                                            return a.GetCrewId - b.GetCrewId;
                                        if (aLv > bLv)
                                            return -1;
                                        return 1;
                                    }
                                    if (aQuality > bQuality)
                                        return -1;
                                    return 1;
                                }
                                if (aRaise > bRaise)
                                    return -1;
                                return 1;
                            }
                            if (aPower > bPower)
                                return -1;
                            return 1;
                        }
                        if (aType > 0 && bType > 0)
                            return aType - bType;
                        if (aType > 0 && bType == 0)
                            return -1;
                        if (aType == 0 && bType > 0)
                            return 1;
                        return a.GetCrewId - b.GetCrewId;
                    }
                    if (aState == bState)
                        return a.GetCrewId - b.GetCrewId;
                    return aState - bState;
                };
            }
            _crewBookList.Sort(_comparison);
        }

        //1代表可招募,2代表已招募,3代表未招募
        private int GetCrewChipState(ICrewBookData data)
        {   
            var had = _selfCrewList.Find(d => d.crewId == data.GetCrewId);  //是否拥有
            var state = data.GetChips > data.GetCrew.chipAmount;            //是否够招募碎片
            if (had == null && state)
                return 1;
            if (had != null)
                return 2;
            return 3;
        }

        public void UpdateInfoTab(CrewInfoTab tab)
        {
            _cureInfoTab = tab;
        }

        public void UpdateCurFavorableIdx(int idx)
        {
            _curFavorableIdx = idx;
        }
        #endregion

        #region CrewMain
        private PartnerTab _curCrewTab = PartnerTab.Info;    //选中标签页
        public PartnerTab GetCurCrewTab {get { return _curCrewTab; }}
        private long _curCrewUId;    //服务端唯一id
        public long GetCurCrewUId{get { return _curCrewUId; }}

        private int _curCrewId;        //导表id
        public int GetCurCrewId{get { return _curCrewId; }}

        private int _nextRaise;   //下一次强化的次数
        public int GetNextRaise { get { return _nextRaise; } }

        private int _nextPhase; //下一次进阶的次数
        public int GetNextPhase { get { return _nextPhase; } }

        //跟随伙伴id
        private long _followCrewId;
        public long GetFollowCrewId { get { return _followCrewId; } }

        private List<CrewChipDto> _allChipList = new List<CrewChipDto>();
        public IEnumerable<CrewChipDto> GetChipList{ get { return _allChipList; }  }
        #endregion

        #region CrewListData
        private Dictionary<int, Crew> _allCrewList = new Dictionary<int, Crew>();    //读表数据
        //不要设置为public    --xush
        private List<CrewInfoDto> _selfCrewList = new List<CrewInfoDto>();            //当前拥有的所有伙伴
        public IEnumerable<CrewInfoDto> GetSelfCrew(){ return _selfCrewList; }

        private void InitCrewListData()
        {
            _allCrewList.Clear();
            var list = DataCache.getArrayByCls<GeneralCharactor>();
            list.ForEach(d =>
            {
                if (d is Crew)
                    _allCrewList.Add((d as Crew).id, d as Crew);
            });
        }

        public Dictionary<int, Crew> GetAllCrewDict()
        { 
            return _allCrewList;
        }

        public List<Crew> GetAllCrewList()
        {
            List<Crew> list = new List<Crew>();
            _allCrewList.ForEach(data =>
            {
                list.Add(data.Value);
            });
            return list;
        } 

        public Crew GetCrewDataById(int id)
        {
            return _allCrewList.Find(d => d.Value.id == id).Value;
        }

        #region 图鉴
        private List<ICrewBookData> _crewBookList = new List<ICrewBookData>();
        public IEnumerable<ICrewBookData> GetCrewBookList { get { return _crewBookList; } }  
        //选择不同类型伙伴
        public IEnumerable<ICrewBookData> GetCrewListByType(PropertyType type = PropertyType.All)
        {
            List<ICrewBookData> list = new List<ICrewBookData>();
            if (type == PropertyType.All)
            {
                _crewBookList.ForEach(data => {list.Add(data); });
                return list; 
            }

            _crewBookList.ForEach(data =>
            {
                if(data.GetCrew.property == (int)type)
                    list.Add((data));
            });
            
            return list;
        }

        public ICrewBookData GetCrewByType(PropertyType type = PropertyType.All)
        {
            return GetCrewListByType(type).ToList()[0];
        }

        public void ReplaceOrAddData(ICrewBookData bookData)
        {
            _crewBookList.ReplaceOrAdd(d=>d.GetInfoDto.id == bookData.GetInfoDto.id, bookData);
        }
        #endregion

        #region 已拥有
        //判断是否拥有选中的伙伴
        public CrewInfoDto IsHadCurPantner(int id)
        {
            CrewInfoDto dto = null;
            _selfCrewList.ForEach(data =>
            {
                if (data.crewId == id)
                    dto = data;
            });
            return dto;
        }
        
        //获取已拥有的某种类型伙伴
        public IEnumerable<CrewInfoDto> GetCrewInfoByType(PropertyType type)
        {
            List<CrewInfoDto> list = new List<CrewInfoDto>();
            _selfCrewList.ForEach(data =>
            {
                var crew = _allCrewList[data.crewId];

                if(type == PropertyType.All)
                    list.Add(data);
                else if (crew != null && crew.property == (int) type)
                {
                    list.Add(data);
                }
            });
            return list;
        }
        #endregion

        #endregion

        #region CrewInfo
        private CrewInfoTab _cureInfoTab = CrewInfoTab.InfoTab;
        public CrewInfoTab GetCurInfoTab { get { return _cureInfoTab; } }
        #endregion

        #region CrewFetter
        private int curCrewFetterId;
        public int GetCurCrewFetterId
        {
            get
            {
                return curCrewFetterId;
            }
        }

        public void SetCurCrewFetterId(int id)
        {
            curCrewFetterId = id;
        }

        private List<ICrewFetterItemVo> crewFettervoList= new List<ICrewFetterItemVo>();
        /// <summary>
        /// 在点击新的Crew的时候更新CrewFetterVoList
        /// </summary>
        public void UpdateCrewFetterVoList()
        {
            crewFettervoList.Clear();
            var curCrewDto = GetSelfCrew().Find(x => x.crewId == GetCurCrewId);
            if (curCrewDto == null) return;

            var crewFetterDic = DataCache.getDicByCls<CrewFetter>();
            foreach (var cfdto in curCrewDto.fetterDto)
            {
                CrewFetter cf;
                if (crewFetterDic.TryGetValue(cfdto.crewFetterId, out cf))
                {
                    Dictionary<Crew, bool> crewDic = new Dictionary<Crew, bool>();
                    foreach (var otherCrew in cf.crewids)
                    {
                        //除去自身
                        if (otherCrew == GetCurCrewId)
                            continue;

                        var crew = _allCrewList[otherCrew];
                        if (crew != null)
                        {
                            bool active = IsHadCurPantner(otherCrew) != null;
                            crewDic.Add(crew, active);
                        }
                    }
                    var vo = new CrewFetterItemVo(cfdto, GetCurCrewId, GetCurCrewUId, crewDic, cf);
                    crewFettervoList.Add(vo);
                }
            }
        }

        public List<ICrewFetterItemVo> CrewFetterVoList
        {
            get
            {
                return crewFettervoList;
            }
        }

        public ICrewFetterItemVo GetCurCrewFetterVo
        {
            get {return CrewFetterVoList.Find(x => curCrewFetterId == x.CrewFetterId);}
        }
        #endregion

        #region 好感度

        private int _curFavorableIdx;   //好感度界面选中的伙伴idx
        private bool _isShowModel = true;      //显示状态
        public int GetCurFavorIdx { get { return _curFavorableIdx; } }

        public bool FavorableShowType
        {
            get { return _isShowModel; }
            set { _isShowModel = value; }
        }

        public ICrewBookData GetBookDataById(int crewId)
        {
            return _crewBookList.Find(d => d.GetCrew.id == crewId);
        }

        #endregion

        #region 研修
        private CrewSkillTrainData trainData;

        private void InitTrainData()
        {
            trainData = new CrewSkillTrainData();
            trainData.InitData(_selfCrewList);
        }
        public ICrewSkillTrainData GetCrewSkillTrainData()
        {
            return trainData;
        }
        #endregion
    }
}

public interface ICrewBookData
{
    int GetCrewId { get; }
    Crew GetCrew { get; }
    CrewInfoDto GetInfoDto { get; }
    CrewStatueType GetStatue { get; }
    int GetChips { get; }
}

//伙伴图鉴数据类型
public class CrewBookData:ICrewBookData
{
    private int _crewId;
    private Crew _crew;
    private CrewInfoDto _infoDto;
    private CrewStatueType _statue;
    private int _chips;

    public int GetCrewId { get { return _crewId; } }
    public Crew GetCrew { get { return _crew; } }
    public CrewInfoDto GetInfoDto { get { return _infoDto; } }
    public CrewStatueType GetStatue { get { return _statue; } }
    public int GetChips { get { return _chips; } }

    public static CrewBookData Create(
        int crewId, 
        Crew crew,
        CrewInfoDto infoDto,
        CrewStatueType statue,
        int chips)
    {
        CrewBookData bookData = new CrewBookData();
        bookData._crewId = crewId;
        bookData._crew = crew;
        bookData._infoDto = infoDto;
        bookData._statue = statue;
        bookData._chips = chips;
        return bookData;
    }
}
