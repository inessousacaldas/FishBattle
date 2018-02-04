using UnityEngine;
using System.Collections;
using AppDto;

public class TeamApplyController : MonolessViewController<CommonTipWin>
{
    private TeamRequestNotify _requestNotify;
    private int time = 30;    //30s倒计时

    protected override void AfterInitView()
    {
        base.AfterInitView();
        _view.CancelLabel_UILabel.text = "拒绝";
        JSTimer.Instance.SetupCoolDown("TeamApplyController", 30f, (e) =>
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
        JSTimer.Instance.CancelCd("TeamApplyController");
        base.OnDispose();
    }

    private void OnCancelClick()
    {
        TeamDataMgr.TeamNetMsg.RefuseTeamRequest(_requestNotify.playerId);
        ProxyBaseWinModule.Close();
    }

    private void OnEnterClick()
    {
        TeamDataMgr.TeamNetMsg.ApproveJoinTeam(_requestNotify.playerId);
        ProxyBaseWinModule.Close();
    }

    public void SetMainContent(TeamRequestNotify notify)
    {
        _requestNotify = notify;

        _view.TitleLb_UILabel.text = "申请入队";
        _view.OKLabel_UILabel.text = "同意";

        _view.ContentLb_UILabel.text = string.Format("{0}{1}申请加入你的队伍", notify.playerNickname, notify.playerGrade);
    }
}
