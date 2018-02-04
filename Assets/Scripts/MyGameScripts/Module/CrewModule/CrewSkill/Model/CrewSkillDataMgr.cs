// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 8/3/2017 5:07:05 PM
// **********************************************************************

using AppDto;
using System.Collections.Generic;

public sealed partial class CrewSkillDataMgr
{

    public void UpdateCrewTmpDic(int id, CrewInfoDto dto)
    {
        _data.UpdateCrewTmpDic(id, dto);
    }
    public CrewSkillCraftsData CraftsData
    {
        get { return _data.craftsData; }
    }

    public void UpdateCrewPassiveData(int id,List<PassiveSkillDto> dtoList)
    {
        _data.UpdataPsvByAddCrew(id, dtoList);
    }

    public CrewSkillTab MainTab
    {
        get { return _data.MainTab; }
    }

    public  void UpdateMagicData(List<CrewInfoDto> list)
    {
        _data.UpdateMagicData(list);
        _data.UpdateCrewTmpData();
        _data.craftsData.UpdateCraftsData(list);
        _data.passiveData.UpdatePassiveData(list);
    }


    public void UpdateMagicData(int id, List<int> list)
    {
        _data.UpdateMagicData(id, list);
    }
    
    // 初始化
    private void LateInit()
    {
        
    }
    
    public void OnDispose(){
            
    }
}
