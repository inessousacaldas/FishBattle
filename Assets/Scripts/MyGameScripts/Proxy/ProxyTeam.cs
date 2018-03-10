using TeamMainViewController = TeamDataMgr.TeamMainViewController;

public class ProxyTeamModule
{
    public static void OpenMainView(TeamMainViewTab tab = TeamMainViewTab.Team)
    {
        TeamDataMgr.TeamNetMsg.GetFormationInfo(() =>
        {
            TeamDataMgr.TeamNetMsg.TeamMatchBtn();
            TeamMainViewController.Open(tab);
        });
    }

    public static void OpenApplicationView()
    {
        TeamDataMgr.TeamApplicationViewLogic.Open();
    }

    public static void OpenInvitationView()
    {
        TeamDataMgr.TeamInvitationViewLogic.Open();
    }

    public static void OpenBeInvitatingView()
    {
        TeamDataMgr.TeamBeInviteViewController.Open();
    }
}

