using UnityEngine;
using System.Collections;
using AppDto;

public class CallMemberController : 
    MonolessViewController<CommonTipWin>
{
    private int time = 30;    //30s倒计时
    public void SetMainContent()
    {
        _view.TitleLb_UILabel.text = "召唤";
        _view.ContentLb_UILabel.text = "队长召唤你立即归队";
        _view.OKLabel_UILabel.text = "同意";
        _view.CancelLabel_UILabel.text = "拒绝";
    }

    protected override void AfterInitView()
    {
        base.AfterInitView();
        JSTimer.Instance.SetupCoolDown("CallMemberController", 30f,(e) =>
        {
            time -= 1;
            _view.CancelLabel_UILabel.text = string.Format("拒绝({0}s)", time);
        }, () =>
        {
            OnCancelClick();
        },1f);
    }

    protected override void RegistCustomEvent()
    {
        EventDelegate.Add(_view.CancelButton_UIButton.onClick, OnCancelClick);
        EventDelegate.Add(_view.OKButton_UIButton.onClick, OnEnterClick);
    }

    protected override void OnDispose()
    {
        JSTimer.Instance.CancelCd("CallMemberController");
        base.OnDispose();
    }

    private void OnCancelClick()
    {
        ProxyBaseWinModule.Close();
    }

    private void OnEnterClick()
    {
        TeamDataMgr.TeamNetMsg.BackTeam();
        OnCancelClick();
    }
}
