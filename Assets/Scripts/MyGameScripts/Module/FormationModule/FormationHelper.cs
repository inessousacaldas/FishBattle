using System.Collections.Generic;
using AppDto;
using UnityEngine;

public static class FormationHelper
{
    public static string GetFormationNameAndLevel(
        string name
        , int level
        , bool showLev = true)
    {
        return GetFormationNameAndLevel(name, level, FormationState.None, showLev);
    }

    public static string GetFormationNameAndLevel(
        string name
        , int level
        , FormationState state
        , bool showLev = true)
    {
        if (!showLev)
            return name;
        
        //        color  todo fish
        var col = state == FormationState.Enable ? Color.green : Color.gray;

        var str = GetLevelText(level);
        if (string.IsNullOrEmpty(str))
        {
            return (state == FormationState.UnEnable) ? name.WrapColor(col) : name;
        }
        else if (state == FormationState.None)
        {
            return string.Format("{0}.{1}", name, GetLevelText(level));
        }
        else
        {
            return (state == FormationState.UnEnable) ?
                string.Format("{0}.{1}", name, GetLevelText(level)).WrapColor(col)
                : string.Format("{0}.{1}", name, GetLevelText(level).WrapColor(col));
        }
    }

    public static string GetLevelText(int grade)
    {
        string[] gradeList = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };
        if (grade <= 0 || grade - 1 >= gradeList.Length)
            return "";
        return gradeList[grade - 1] + "级";
    }

    public static bool IsRegular(int id){
        return id == (int)Formation.FormationType.Regular;
    }

    public static IEnumerable<FormationUpdateMaterialsData> GetMaterial(int formationId, int lv)
    {
        var list = new List<FormationUpdateMaterialsData>();
        var formationGrade = DataCache.getDtoByCls<FormationGrade>(lv);
        var materials = DataCache.getDtoByCls<Formation>(formationId);
        formationGrade.materials.ForEachI((num, i)=>
        {
            var materialId = materials.materialIds[i];
            var hadcount = BackpackDataMgr.DataMgr.GetItemCountByItemID(materialId);
            list.Add(FormationUpdateMaterialsData.Create(materialId, hadcount, num));
        });
        return list;
    }

    // toto fish:根据阵法id和当前最高等级获取升下一级阵法的条件
    public static int GetPlayerLev(int lv)
    {
        var formationGrade = DataCache.getDtoByCls<FormationGrade>(lv);
        return formationGrade.gradeLimit;
    }

    public static string GetFormationIconName(int fid)
    {
        return string.Format("formation_{0}", fid);
    }
}
