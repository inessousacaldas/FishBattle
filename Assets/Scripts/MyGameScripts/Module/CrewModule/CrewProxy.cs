using UnityEngine;
using System.Collections;

public static class CrewProxy 
{
    public static void OpenCrewMainView()
    {
        CrewViewDataMgr.CrewViewNetMsg.ResCrewList(() =>
        {
            CrewViewDataMgr.CrewMainViewLogic.Open();
        });
    }

    public static void OpenCrewFavorableView()
    {
        CrewViewDataMgr.CrewFavorableViewLogic.Open();
    }

    public static void OpenCrewUpGradeView()
    {
        CrewViewDataMgr.CrewUpGradeViewLogic.Open();
    }

    public static void CloseCrewUpGradeView()
    {
        UIModuleManager.Instance.CloseModule(CrewUpGradeView.NAME);
    }
}
