using AppDto;

public class TeamBeInviteController :
    MonolessViewController<CommonTipWin>
{
    private int time = 30;    //30s倒计时
    private TeamInvitationNotify _invitationNotify;
    

    public void SetMainContent(TeamInvitationNotify notify)
    {
        _invitationNotify = notify;
        var leaderInfo = notify.inviteTeamMembers.Find(m => m.id == notify.inviterPlayerId);
        string lv = string.Format("({0}级)", leaderInfo.grade);
        
        _view.TitleLb_UILabel.text = "邀请入队";
        _view.OKLabel_UILabel.text = "同意";

        _view.ContentLb_UILabel.text = string.Format("{0}{1}邀请你加入他的队伍", leaderInfo.nickname, lv);
    }

    protected override void AfterInitView()
    {
        base.AfterInitView();
        _view.CancelLabel_UILabel.text = "拒绝";
        JSTimer.Instance.SetupCoolDown("TeamBeInviteController", 30f, (e) =>
        {
            time -= 1;
            _view.CancelLabel_UILabel.text = string.Format("拒绝({0}s)", time);
        }, () =>
        {
            OnCancelClick();
        }, 1f);
    }

    protected override void RegistCustomEvent()
    {
        EventDelegate.Add(_view.CancelButton_UIButton.onClick, OnCancelClick);
        EventDelegate.Add(_view.OKButton_UIButton.onClick, OnEnterClick);
    }

    protected override void OnDispose()
    {
        JSTimer.Instance.CancelCd("TeamBeInviteController");
        base.OnDispose();
    }

    private void OnCancelClick()
    {
        TeamDataMgr.TeamNetMsg.RefuseTeamInvitation(_invitationNotify.inviterPlayerId);
        ProxyBaseWinModule.Close();
    }

    private void OnEnterClick()
    {
        TeamDataMgr.TeamNetMsg.ApproveInviteMember(_invitationNotify);
        ProxyNpcDialogueView.Close();
        ProxyBaseWinModule.Close();
    }
}
