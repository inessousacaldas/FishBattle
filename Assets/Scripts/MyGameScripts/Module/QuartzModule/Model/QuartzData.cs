// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 8/25/2017 6:05:18 PM
// **********************************************************************

using System.Collections.Generic;
using AppDto;

public interface IQuartzData
{
    IQuartzInfoData QuartzInfoData { get; }
    IQuartzStrengthData QuartzStrengthData { get; }
    IQuartzForgeData QuartzForgeData { get; }
    
    QuartzDataMgr.TabEnum CurTabPage { get; set; }
    OrbmentDto GetOrbmentDto { get; }
    IEnumerable<OrbmentInfoDto> GetOrbmentByType(int type);
    SelectOrbmentData SelectOrbment { get; }
}

public interface IQuartzInfoData
{
    OrbmentInfoDto GetCurOrbmentInfoDto { get; }
    int CurQuartzPos { get; }
}

public interface IQuartzStrengthData
{
    OrbmentInfoDto GetCurOrbmentInfoDto { get; }
    int CurQuartzPos { get; }
    int CurBagPos { get; }
    bool IsBagGroup { get; }
    SelectOrbmentData SelectOrbment { get; }
}

public interface IQuartzForgeData
{
    
}

public sealed partial class QuartzDataMgr
{
    public sealed partial class QuartzData:
        IQuartzData,
        IQuartzInfoData,
        IQuartzStrengthData,
        IQuartzForgeData
    {
        public IQuartzInfoData QuartzInfoData {get { return this; }} 
        public IQuartzStrengthData QuartzStrengthData{get { return this; }}
        public IQuartzForgeData QuartzForgeData{get { return this; }}
        
        private List<Crew> _allCrewList = new List<Crew>(); 
        #region clientData

        private int _curOrbmentIdx = 0;     //默认选中伙伴的序号
        private OrbmentInfoDto _curOrbmentData;    //选中的伙伴
        private TabEnum _curTabEnum;

        private SelectOrbmentData _selectOrbmentData;   //属性界面选中的结晶回路
        public SelectOrbmentData SelectOrbment
        {
            get { return _selectOrbmentData; }
            set { _selectOrbmentData = value; }
        }   

        public TabEnum CurTabPage
        {
            get { return _curTabEnum; }
            set { _curTabEnum = value; }
        }

        #endregion

        #region serviceData

        private OrbmentDto _orbmentDto;

        #endregion

        public QuartzData()
        {

        }

        public void InitData()
        {
            DataCache.getArrayByCls<GeneralCharactor>().ForEach(data =>
            {
                if(data is Crew)
                    _allCrewList.Add(data as Crew);
            });
        }

        public void Dispose()
        {

        }
        
        #region 刷新数据

        public void SetCurOrbmentData(int idx)
        {
            _curOrbmentIdx = idx;
            _curOrbmentData = _orbmentDto.orbmentInfoDtos[idx];
        }

        public void SetCurOrbmentData(OrbmentInfoDto dto)
        {
            _curOrbmentIdx = _orbmentDto.orbmentInfoDtos.FindIndex(d => d.ownId == dto.ownId);
            _curOrbmentData = _orbmentDto.orbmentInfoDtos.TryGetValue(_curOrbmentIdx);
        }

        public void RefreshOrbmentDto(OrbmentDto dto)
        {
            _orbmentDto = dto;
        }

        public void UpdateOrbmentDto(OrbmentInfoDto dto)
        {
            _orbmentDto.orbmentInfoDtos.ReplaceOrAdd(d => d.ownId == dto.ownId, dto);
        }
        #endregion

        #region 读取数据 
        public OrbmentDto GetOrbmentDto { get { return _orbmentDto; } }
        public OrbmentInfoDto GetCurOrbmentInfoDto { get { return _curOrbmentData; } }
        public int GetCurOrbmentIdx { get { return _curOrbmentIdx;;} }

        public OrbmentInfoDto GetOrbmentById(long id)
        {
            return _orbmentDto.orbmentInfoDtos.Find(d => d.ownId == id);
        }

        public IEnumerable<OrbmentInfoDto> GetOrbmentByType(int type)
        {
            if (type == (int)PropertyType.All)
                return _orbmentDto.orbmentInfoDtos;

            return _orbmentDto.orbmentInfoDtos.Filter(d => d.ownId == ModelManager.Player.GetPlayerId() || 
                                                    _allCrewList.Find(c=>c.id ==d.crewId).property == type);
        }
        #endregion

        #region strength

        private int _curBagPos = 0;
        public int CurBagPos
        {
            get { return _curBagPos; }
            set { _curBagPos = value; }
        }

        private int _curQuartzPos = 1;  //默认选中第一个

        public int CurQuartzPos
        {
            get { return _curQuartzPos; }
            set { _curQuartzPos = value; }
        }

        private bool _isBagGroup = false;

        public bool IsBagGroup
        {
            get { return _isBagGroup; }
            set { _isBagGroup = value; }
        }

        #endregion
    }
    public enum TabEnum
    {
        Info = 0,
        Strength = 1,
        Forge = 2,
        InfoMagic = 3   //魔法界面
    }
}
