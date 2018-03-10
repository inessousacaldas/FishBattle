using System;

public interface IBaseTipContentController
{
    void SetToggle(string txt, Action<bool> b);
}

public class BaseTipContentController : 
	MonolessViewController<CommonTipWin>, 
    IBaseTipContentController
{
	private int _time = 30;    //30s倒计时
	private Action _enterCallback;
	private Action _cancelCallback;

    private Action<bool> _toggleAction;

	protected override void RegistCustomEvent()
	{
		EventDelegate.Add(_view.CancelButton_UIButton.onClick, OnCancelClick);
		EventDelegate.Add(_view.OKButton_UIButton.onClick, OnEnterClick);
	}

	protected override void OnDispose()
	{
		JSTimer.Instance.CancelCd("BaseTipController");
		base.OnDispose();
	}

	private void OnCancelClick()
	{
        if (_cancelCallback != null)
	        _cancelCallback();
        ProxyBaseWinModule.Close();
    }

	private void OnEnterClick()
	{
	    if (_toggleAction != null)
	        _toggleAction(_view.ToggleBtn_UIToggle.value);
        if (_enterCallback != null)
			_enterCallback();
        ProxyBaseWinModule.Close();
    }
	
	public void SetMainContent(string txt,
        string content,
		int time,
		Action enterCallback, 
		Action cancelCallback,
        string leftBtnLb = "取消",
        string rightBtnLb = "确定")
	{
	    _view.TitleLb_UILabel.text = txt;
		_view.ContentLb_UILabel.text = content;
		_time = time;
		
		_enterCallback = enterCallback;
		_cancelCallback = cancelCallback;
	    _view.OKLabel_UILabel.text = rightBtnLb;
	    _view.CancelLabel_UILabel.text = leftBtnLb;

        if (time > 0)
	    {
            JSTimer.Instance.SetupCoolDown("BaseTipController", time, (e) =>
            {
                _time -= 1;
                _view.CancelLabel_UILabel.text = string.Format("{0}({1}s)", leftBtnLb, _time);
            }, () =>
            {
                OnCancelClick();
            }, 1f);
        }
    }

    public void SetToggle(string txt, Action<bool> action)
    {
        _view.ToggleGroup.gameObject.SetActive(true);
        _view.ToggleLb_UILabel.text = txt;
        _toggleAction = action;
    }
}
