// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 7/27/2017 7:53:13 PM
// **********************************************************************

using System.Collections.Generic;
using AppDto;
using UniRx;

public sealed partial class CrewViewDataMgr
{
    // 初始化
    private void LateInit()
    {
        stream.OnNext(_data);
        _disposable.Add(TeamFormationDataMgr.Stream.SubscribeAndFire(data =>
        {
            stream.OnNext(_data);
        }));

        _disposable.Add(NotifyListenerRegister.RegistListener<CrewInfoDto>(UpdateCrewInfoDto));
        _disposable.Add(NotifyListenerRegister.RegistListener<CrewChipNotify>(UpdateCrewChip));
    }
    
    public void OnDispose(){
            
    }

    private CDTaskManager mCDTaskManager;
    private CDTaskManager CDTaskManager
    {
        get
        {
            if (null == mCDTaskManager)
                mCDTaskManager = new CDTaskManager();
            return mCDTaskManager;
        }
    }

    public void UpdateCrewInfoDto(CrewInfoDto dto)
    {
        DataMgr._data.UpdateCrewDto(dto);
        FireData();
    }

    private void UpdateCrewChip(CrewChipNotify dto)
    {
        DataMgr._data.UpdateChipList(dto.crewChipItems);
        FireData();
    }

    //伙伴技能那边需要伙伴的信息
    public IEnumerable<CrewInfoDto> GetCrewListInfo()
    {
        return _data.GetSelfCrew();
    }

    public int GetCurCrewID
    {
        get { return _data.GetCurCrewId; }
    }

    public int GetCurCrewGrade
    {
        get
        {
            var dto = _data.IsHadCurPantner(_data.GetCurCrewId);
            if (dto == null) return 0;
            else return dto.grade;
        }
    }

    public int GetCurCrewQuality
    {
        get
        {
            var dto = _data.IsHadCurPantner(_data.GetCurCrewId);
            if (dto == null) return -1;
            else return dto.quality;
        }
    }
}
