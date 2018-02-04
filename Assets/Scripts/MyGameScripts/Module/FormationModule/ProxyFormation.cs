using AppDto;

public static class ProxyFormation
{
    public static void OpenFormationView(FormationPosController.FormationType type)
    {
        TeamFormationDataMgr.TeamFormationNetMsg.Formation_Info(() =>
        {
            if (type == FormationPosController.FormationType.Team)
            {
                TeamFormationDataMgr.TeamFormationNetMsg.Formation_CaseInfo(() =>
                {
                    TeamFormationDataMgr.TeamFormationTabContentViewLogic.Open(type);
                });
            }
            else
                TeamFormationDataMgr.TeamFormationTabContentViewLogic.Open(type);
        });
    }

    public static void OpenUpdateView()
    {
        TeamFormationDataMgr.FormationUpdateViewLogic.Open();
    }

    public static void OpenCrewFormation()
    {
        TeamFormationDataMgr.TeamFormationNetMsg.Formation_CaseInfo(() =>
        {
            TeamFormationDataMgr.CrewFormationLogic.Open();
        });
    }

    public static void OpenConstrainView()
    {
//        FormationDataMgr.FormationConstrainViewController.Open();
    }
}

