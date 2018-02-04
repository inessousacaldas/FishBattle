using AppDto;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum RoleSkillSpecItemSingleType
{
    Normal,
    Edit

}

public class RoleSkillSpecialityTempVO
{
    public int id;
    public int addGrade;
    public int gradeUnit = 1;//一次加多少点（默认1，后续有键盘组件可输入具体数字）
    public SpecialityGradeDto gradeDto;
    public Speciality cfgVO;

    public string Name
    {
        get { return cfgVO.name; }
    }
    public int ShowGrade
    {
        get { return gradeDto.grade + addGrade; }
    }

    public float ShowEff
    {
        get { return cfgVO.effect * ShowGrade; }
    }

    public string ShowEffPer
    {
        get { return (System.Math.Round(ShowEff,2) * 100).ToString() + "%"; }
    }

    public void Clear()
    {
        addGrade = 0;
    }
}


public sealed partial class RoleSkillSpecialityData:IRoleSkillSpecialityData
{
    private Dictionary<int,SpecialityGradeDto> gradeDtoList = new Dictionary<int, SpecialityGradeDto>();
    private SpecialityDto infoDto;

    private Dictionary<int,RoleSkillSpecialityTempVO> tempList = new Dictionary<int, RoleSkillSpecialityTempVO>();

    private Dictionary<int,int> expLevelLimitDict = new Dictionary<int, int>();//人物等级限制可升级的专精等级
    private int maxGrade = 0;//最大可升级
    public RoleSkillSpecialityData()
    {

    }

    public void InitData()
    {
        if(tempList.Count > 0) return;
        var cfgDict = DataCache.getDicByCls<Speciality>();
        foreach(var key in cfgDict.Keys)
        {
            var tempItem = new RoleSkillSpecialityTempVO() {id = key,cfgVO = cfgDict[key] };
            tempList[tempItem.id] = tempItem;
        }
        var expCfgByDataCache = DataCache.getArrayByCls<SpecialityExpGrade>();

        for(var i = 0;i < expCfgByDataCache.Count;i++)
        {
            if(expLevelLimitDict.ContainsKey(expCfgByDataCache[i].playerGradeLimit) == false)
            {
                expLevelLimitDict[expCfgByDataCache[i].playerGradeLimit] = 0;
            }
            expLevelLimitDict[expCfgByDataCache[i].playerGradeLimit] = Math.Max(expLevelLimitDict[expCfgByDataCache[i].playerGradeLimit],expCfgByDataCache[i].id);
            maxGrade = Math.Max(maxGrade,expCfgByDataCache[i].id);
        }
    }

    public void Dispose()
    {

    }

    public Dictionary<int,RoleSkillSpecialityTempVO> TempList { get { return tempList; } }

    #region 更新数据 SpecialityDto SpecialityExpGradeNotify

    public void UpdateInfo(SpecialityDto _infoDto)
    {
        infoDto = _infoDto;
        UpdateGradeList(_infoDto.specialityGradeDtos);
        ResetTempList();
    }

    public void UpdateGradeListByReset(List<SpecialityGradeDto> gradeList)
    {
        for(var i = 0;i < gradeList.Count;i++)
        {
            var tempItem = tempList[gradeList[i].specialityId];
            tempItem.addGrade = gradeDtoList[gradeList[i].specialityId].grade - gradeList[i].grade;
        }
        infoDto.specialityGradeDtos = gradeList;
        UpdateGradeList(gradeList);
    }

    public void UpdateGradeList(List<SpecialityGradeDto> gradeList)
    {
        for(var i = 0;i < gradeList.Count;i++)
        {
            UpdateGradeItem(gradeList[i]);
        }
    }

    private void UpdateGradeItem(SpecialityGradeDto gradeDto)
    {
        gradeDtoList[gradeDto.specialityId] = gradeDto;
        UpdateTempItem(gradeDto);
    }

    private void UpdateTempItem(SpecialityGradeDto gradeDto)
    {
        if(tempList.ContainsKey(gradeDto.specialityId))
        {
            tempList[gradeDto.specialityId].gradeDto = gradeDto;
        }
    }

    public void ResetTempList(int layer = 0)
    {
        foreach(var item in tempList.Values)
        {
            if(layer == 0 || item.cfgVO.layer == layer)
            {
                item.Clear();
            }
        }
    }

    //grade是最终的值
    public void AddGradeTempData(int id,int grade)
    {
        var vo = GetTempVO(id);
        if(vo != null)
        {
            vo.addGrade = grade;
        }
    }

    public void UpdateInfoDtoByNotify(SpecialityExpGradeNotify notify)
    {
        if(infoDto != null)
        {
            infoDto.exp = notify.exp;
            infoDto.grade = notify.grade; 
            infoDto.hasAddExpTime = notify.hasAddExpTime; 
        }
    }

    #endregion

    public RoleSkillSpecialityTempVO GetTempVO(int id)
    {
        return tempList.ContainsKey(id) ? tempList[id] : null;
    }
    public bool CanReset()
    {
        foreach(var itm in tempList.Values)
        {
            if (itm.gradeDto.grade != 0)
                return true;
        }
        return false;
    }
    public int GetShowPoint()
    {
        var specPoint = (int)ModelManager.Player.GetPlayerWealth(AppDto.AppVirtualItem.VirtualItemEnum.SPECIALITYPOINT);
        return specPoint - GetTempPoint();
    }

    private int GetTempPoint()
    {
        var count = 0;
        foreach(var item in tempList.Values)
        {
            count += item.addGrade;
        }
        return count;
    }

    public int GetHasAddExpTime()
    {
        return infoDto != null ? infoDto.hasAddExpTime : 0;
    }

    public int GetCurGrade()
    {
        return infoDto != null ? infoDto.grade : 0;
    }

    public int GetCurExp()
    {
        return infoDto != null ? infoDto.exp : 0;
    }

    public int GetLevelLimit()
    {
        int lv = 0;
        var playLv = ModelManager.Player.GetPlayerLevel();
        var serverGrade = ModelManager.Player.ServerGrade;
        lv = Math.Min(playLv, serverGrade);
        var curLimit = 0;
        foreach(var key in expLevelLimitDict.Keys)
        {
            if(lv >= key)
            {
                curLimit = expLevelLimitDict[key];
            }else
            {
                break;
            }
        }
        return curLimit;
    }

    public int MaxGrade
    {
        get { return maxGrade; }
    }

    public SpecialityExpGrade GetExpGradeVO(int grade)
    {
        return DataCache.getDtoByCls<SpecialityExpGrade>(grade);
    }

    public RoleSkillSpecItemSingleType GetCurType()
    {
        var specPoint = (int)ModelManager.Player.GetPlayerWealth(AppDto.AppVirtualItem.VirtualItemEnum.SPECIALITYPOINT);
        return specPoint == 0 ? RoleSkillSpecItemSingleType.Normal : RoleSkillSpecItemSingleType.Edit;
    }

    public string GetAddPointStr(char firstSep = ',',char secondSep = ':')
    {
        var msg = "";
        foreach(var item in tempList.Values)
        {
            if(item.addGrade != 0)
            {
                msg += item.id.ToString() + secondSep + item.addGrade.ToString() + firstSep;
            }
        }
        if(msg.Length > 0) msg = msg.Substring(0,msg.Length - 1);
        return msg;
    }

    public void CheckClearAddPoint(Speciality.SpecialityLayerEnum layer)
    {
        if(CheckCanAdd(layer) == false)
        {
            ResetTempList((int)layer);
        }
    }

    public bool CheckCanAdd(Speciality.SpecialityLayerEnum layer)
    {
        var needPoint = GetNeedPoint(layer);
        if(needPoint == 0) return true;
        var count = 0;
        foreach(var item in tempList.Values)
        {
            if(item.cfgVO.layer <= (int)layer - 1 )
            {
                count += item.ShowGrade;
            }
        }
        return count >= needPoint;
    }

    public int GetNeedPoint(Speciality.SpecialityLayerEnum layer)
    {
        if(layer == Speciality.SpecialityLayerEnum.First)
        {
            return 0;
        }else if(layer == Speciality.SpecialityLayerEnum.Second)
        {
            return DataCache.GetStaticConfigValue(AppStaticConfigs.SPECIALITY_SECOND_NEED_ADDPOINT);
        }
        else if(layer == Speciality.SpecialityLayerEnum.Third)
        {
            return DataCache.GetStaticConfigValue(AppStaticConfigs.SPECIALITY_THIRD_NEED_ADDPOINT);
        }else
        {
            return 0;
        }
    }

    public int GetTrainItemID()
    {
        return DataCache.GetStaticConfigValue(AppStaticConfigs.SPECIALITY_ADDEXP_CONSUME_ITEM_ID); 
    }
}
