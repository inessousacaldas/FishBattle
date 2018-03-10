using System;
using System.Collections.Generic;
using UnityEngine;
using AppDto;

public interface ITeamFormationData
{
    ICrewFormationData CrewFormationData { get; }
    int ActiveFormationId { get; }
    FormationState GetFormationState(int id);
    Formation GetFormationByIdx(int i);
    Tuple<string, int> GetActivatedFormationNameAndLev();
    IEnumerable<Formation> GetAllFormationInfo();
    int GetFormationLevel(int idx);
    List<FormationInfoDto> acquiredFormation { get; }
    FormationInfoDto GetFormationDtoById(int idx);
    TeamDto GetTeamDto { get; }
    int GetCurCaseIdx { get; }
    AllCaseInfoDto AllCaseInfoDto { get; }
    Formation CurUpFradeFormation { get; }
    TeamFormationDataMgr.TeamFormationData.FormationType CurFormationType { get; }
}

public partial interface ICrewFormationData
{
    int GetUseCaseIdx { get; }
    int GetArenaCaseId { get; }
    long GetMainCrewId { get; }
    Crew GetCrewDataById(int id);
    IEnumerable<CrewInfoDto> GetSelfCrew();
    IEnumerable<CrewInfoDto> GetCrewInfoByType(PropertyType type);
    List<FormationCaseInfoDto> GetCrewFormationList { get; }
    bool CrewIsFigthing(int caseId, long id);
}

public sealed partial class TeamFormationDataMgr
{
    public sealed partial class TeamFormationData : 
        ITeamFormationData
        ,ICrewFormationData
    {
        public ICrewFormationData CrewFormationData { get { return this; } }
        //掌握的阵法
        private List<FormationInfoDto> _acquiredFormation;

        public List<FormationInfoDto> acquiredFormation
        {
            get { return _acquiredFormation; }
            set { _acquiredFormation = value; }
        }

        public static Comparison<Formation> _comparison = null;
        public TeamFormationData()
        {

        }

        public void Dispose()
        {
            acquiredFormation.Clear();
            _acquiredFormation.Clear();
        }

        #region Client Data

        private FormationType _formationType;

        public FormationType CurFormationType
        {
            get { return _formationType; }
            set { _formationType = value; }
        }

        private int _curFormationID = (int)Formation.FormationType.Regular; //使用中的阵法
        public int ActiveFormationId{get { return _curFormationID; }}

        public FormationState GetFormationState(int id)
        {
            if (id == (int)Formation.FormationType.Regular)
                return FormationState.None;
            else
            {
                var info = acquiredFormation.Find<FormationInfoDto>(s => s.formationId == id);
                return info == null ? FormationState.UnEnable : FormationState.Learned;
            }
        }

        public Formation GetFormationByIdx(int i)
        {
            return allFormationList.TryGetValue(i);
        }

        public Formation GetFormationById(int id)
        {
            return allFormationList.Find(d => d.id == id);
        }

        public Tuple<string, int> GetActivatedFormationNameAndLev(){
            var f = allFormationList.Find<Formation>(s=>s.id ==ActiveFormationId);
            if (f == null)
                return null;
            
            if (ActiveFormationId == (int)Formation.FormationType.Regular)
            {
                return Tuple.Create<string, int>(f.name, 1);
            }
            else
            {
                if(acquiredFormation == null || acquiredFormation.Count == 0)
                    return Tuple.Create<string, int>(f.name, 0);

                var formaiton = acquiredFormation.Find<FormationInfoDto>(s => s.formationId == ActiveFormationId);
                return Tuple.Create<string, int>(f.name, formaiton == null ? 0: formaiton.level);
            }
        } 

        #endregion

        #region serverData

        public TeamDto teamDto;
        public List<Formation> allFormationList;
        private AllCaseInfoDto _allCaseInfoDto;
        #endregion

        #region 刷新数据

        public void UpdateSelfCrewData(List<CrewInfoDto> crewList)
        {
            _selfCrewList = crewList;
        }

        public void SetCurFormationID(int formationId)
        {
           _curFormationID = formationId;
        }

        public void UpdateSelfFormationList(DataList list)
        {
            _acquiredFormation.Clear();
            list.items.ForEach(data =>
            {
                _acquiredFormation.Add(data as FormationInfoDto);
            });
        }

        public void UpdateFormation(int formationId, int level)
        {
            FormationInfoDto formation = new FormationInfoDto();
            formation.formationId = formationId;
            formation.level = level;
            _acquiredFormation.ReplaceOrAdd(d => d.formationId == formationId, formation);
        }

        public void SetCurCaseIdx(int idx)
        {
            _curCaseIdx = idx;
        }

        public AllCaseInfoDto AllCaseInfoDto
        {
            get { return _allCaseInfoDto; }
            set { _allCaseInfoDto = value; }
        }
        #endregion
        private int _curCaseIdx;    //选中第几套方案
        public int GetCurCaseIdx { get { return _curCaseIdx; } }

        public void InitData()
        {
            InitFormatinList();
            FormationListSort();
            InitCrewList();
        }

        private void InitFormatinList()
        {
            acquiredFormation = new List<FormationInfoDto>();
            allFormationList = DataCache.getArrayByCls<Formation>();
            if (allFormationList[0].id == (int) (int) Formation.FormationType.Regular)
                allFormationList.RemoveAt(0);   //删掉默认阵法

            if (_comparison == null)
            {
                _comparison = (a, b) =>
                {
                    var hasA = acquiredFormation.Find<FormationInfoDto>(s => s.formationId == a.id) != null;
                    var hasB = acquiredFormation.Find<FormationInfoDto>(s => s.formationId == b.id) != null;
                    if (hasA && !hasB)
                        return -1;
                    else if (!hasA && hasB)
                        return 1;
                    else
                    {
                        return a.id - b.id;
                    }
                };
            }
        }

        public void FormationListSort()
        {
            allFormationList.Sort(_comparison);
        }

        private void InitCrewList()
        {
            var list = DataCache.getArrayByCls<GeneralCharactor>();
            list.ForEach(d =>
            {
                if (d is Crew)
                    _allCrewList.Add(d.id, d as Crew);
            });
        }

        public FormationInfoDto GetFormationDtoById(int id)
        {
            return _acquiredFormation.Find(d => d.formationId == id);
        }

        public int GetFormationLevel(int formationId)
        {
            if (FormationHelper.IsRegular(formationId))
                return 1;
            var dto = acquiredFormation.Find<FormationInfoDto>(s => s.formationId == formationId);
            return dto == null ? 0 : dto.level;
        }

        //获取所有阵型Id列表，优先已掌握的
        public IEnumerable<Formation> GetAllFormationInfo()
        {
            return allFormationList;
        }

        public TeamDto GetTeamDto{get { return teamDto; }}

        #region 伙伴布阵

        private Dictionary<int, Crew> _allCrewList = new Dictionary<int, Crew>();//读表数据
        private List<CrewInfoDto> _selfCrewList = new List<CrewInfoDto>();            //当前拥有的所有伙伴
        public IEnumerable<CrewInfoDto> GetSelfCrew() { return _selfCrewList; }

        private List<FormationCaseInfoDto> _crewFormationList = new List<FormationCaseInfoDto>();
        public List<FormationCaseInfoDto> GetCrewFormationList { get { return _crewFormationList; } }
        //主战伙伴
        private long _mainCrewId;
        public long GetMainCrewId { get { return _mainCrewId; } }

        public int ChangeCaseCdTime = 30;
        private int _useCaseId = 0;
        public int GetUseCaseIdx { get { return _useCaseId; } }     //进攻阵使用中的方案    

        public void SetMainCrewId(long id)
        {
            _mainCrewId = id;
        }

        public void SetUseCaseId(int caseId)
        {
            _useCaseId = caseId;
        }       

        public void UpdateCrewFormationList(List<FormationCaseInfoDto> list)
        {
            _crewFormationList = list;
        }

        //伙伴上阵
        //List<CasePositionDto>
        public void AddCasePositionDto(DataList datalist, int caseId = -1)
        {
            if (caseId < 0)
                caseId = GetUseCaseIdx;

            if (datalist.items.Count == 1)
            {
                var dto = datalist.items[0] as CasePositionDto;
                if (dto != null)
                    _crewFormationList.Find(d => d.caseId == caseId).casePositions.Add(dto);
                else
                    GameDebuger.LogError("------伙伴数据有误-----");
            }
            else
            {
                //上阵的时候如果datalist>1证明是上阵了主战伙伴,所以全部阵发方案都要更新
                //然而_crewFormationList[0]写死是竞技场防御阵,所以需要进行如下潜规则操作
                SetMainCrewId((datalist.items[0] as CasePositionDto).crewId);
                //_crewFormationList.cout == 1代表是防御阵
                if (_crewFormationList.Count == 1)
                {
                    var dto = datalist.items[0] as CasePositionDto;
                    _crewFormationList[0].casePositions.Add(dto);
                }
                else
                {
                    //_crewFormationList.cout == 3代表是进攻阵
                    //伙伴阵法界面只有3个方案,所以idx必须进行-1处理,因为0代表防御阵
                    datalist.items.ForEachI((data, idx) =>
                    {
                        if (idx > 0)
                        {
                            var dto = data as CasePositionDto;
                            if (dto != null)
                                _crewFormationList[idx - 1].casePositions.Add(dto);
                        }
                    });
                }
            }
        }

        public bool RemoveCasePositionDto(int caseId, List<CasePositionDto> dtoList)
        {
            if (dtoList == null || dtoList.Count == 0)
            {
                GameDebuger.LogError("------伙伴数据有误======");
                return false;
            }

            int index = -1;
            if (caseId < 0)
                caseId = GetUseCaseIdx;

            //当dtolist.count > 1 时,表示主战伙伴下阵
            if (dtoList.Count == 1)
            {
                _crewFormationList.Find(d=>d.caseId == caseId).casePositions.ForEachI((data, idx) =>
                {
                    if (data.crewId == dtoList[0].crewId)
                        index = idx;
                });

                if (index != -1)
                {
                    _crewFormationList.Find(d => d.caseId == caseId).casePositions.RemoveAt(index);
                    index = -1;
                }
            }
            else
            {
                _crewFormationList.ForEach(data =>
                {
                    data.casePositions.ForEachI((d, idx) =>
                    {
                        if (d.crewId == _mainCrewId)
                            index = idx;
                    });
                    if (index != -1)
                    {
                        data.casePositions.RemoveAt(index);
                        index = -1;
                    }
                });
                SetMainCrewId(-1);  //主战伙伴下阵,当前没有主战伙伴
            }
            return true;
        }

        public void CasePostitionDtoReplaceOrAdd(int caseId, CasePositionDto dto)
        {
            var caseInfo = _crewFormationList.Find(d => d.caseId == caseId);
            if (_crewFormationList == null ||
                _crewFormationList.Count == 0 ||
                caseInfo == null)
                return;

            caseInfo.casePositions.ReplaceOrAdd(d => d.crewId == dto.crewId, dto);
        }

        public void SetCaseFormationId(int caseId, int formationId)
        {
            var caseInfo = _crewFormationList.Find(d => d.caseId == caseId);
            if (_crewFormationList == null ||
                _crewFormationList.Count == 0
                || caseInfo == null) return;

            caseInfo.formationId = formationId;
        }

        public IEnumerable<CrewInfoDto> GetCrewInfoByType(PropertyType type)
        {
            List<CrewInfoDto> list = new List<CrewInfoDto>();
            _selfCrewList.ForEach(data =>
            {
                var crew = _allCrewList[data.crewId];

                if (type == PropertyType.All)
                    list.Add(data);
                else if (crew != null && crew.property == (int)type)
                {
                    list.Add(data);
                }
            });
            return list;
        }

        public Crew GetCrewDataById(int id)
        {
            return _allCrewList.Find(d => d.Value.id == id).Value; ;
        }

        public bool CrewIsFigthing(int caseId, long id)
        {
            bool b = false;
            var caseInfo = _crewFormationList.Find(d => d.caseId == caseId);
            if (caseInfo == null)
                return false;
            caseInfo.casePositions.ForEach(data =>
            {
                if (data.crewId == id)
                    b = true;
            });
            return b;
        }
        #endregion

        #region 竞技场防御阵
        private int _arenaCaseId = 0;
        public int GetArenaCaseId { get { return _arenaCaseId; } }  //竞技场使用中的方案
        public void SetArenaCaseId(int caseId)
        {
            _arenaCaseId = caseId;
        }
        #endregion

        private Formation _curUpGradeFormation;

        public Formation CurUpFradeFormation
        {
            get { return _curUpGradeFormation; }
            set { _curUpGradeFormation = value; }
        }

        //阵型种类
        public enum FormationType
        {
            CrewFormation = 0,  //伙伴布阵(进攻阵)
            ArenaFormation = 1  //竞技场布阵(防御阵)
        }
    }
}
