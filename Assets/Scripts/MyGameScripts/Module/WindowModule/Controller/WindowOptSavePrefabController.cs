using UnityEngine;
using System;

public class WindowOptSavePrefabController : MonoViewController<WindowOptSavePrefab>
{
   

	private bool _isComfirmWin = false;
    private bool _isCloseCallCancelHandler = true;

	#region

    protected override void AfterInitView ()
	{
        View.SelectToggle.value = false;
	}

	protected override void RegistCustomEvent ()
	{
		base.RegistCustomEvent ();		EventDelegate.Set (View.OKButton.onClick, OnClickOkButton);
		EventDelegate.Set (View.CancelButton.onClick, OnClickCancelButton);
		EventDelegate.Set (View.CloseBtn.onClick, OnClickCloseButton);
	}

	
	#endregion

	public event Action<bool> OnOkHandler;
    public event Action<bool> OnCancelHandler;

	public void OpenOptSaveWindow (string msg,
	                               string title="",
	                               Action<bool> onHandler = null,
	                               Action<bool> cancelHandler = null,
	                               UIWidget.Pivot pivot = UIWidget.Pivot.Left,
	                               string okLabelStr = "确定", string cancelLabelStr = "取消",
	                               string toggleStr = "不再提示",
	                               int time = 0, bool isCloseCallCancelHandler = true)
	{
	
		
		_isComfirmWin = true;
        _isCloseCallCancelHandler = isCloseCallCancelHandler;

		char[] strArr = msg.ToCharArray ();
		if(strArr.Length < 19)
		{
			View.InfoLabel.pivot = UIWidget.Pivot.Center;
		}else
		{
			View.InfoLabel.pivot = pivot;
		}
		View.InfoLabel.text = msg;


		View.TitleLabel.text = title;
		View.OKLabel.text = okLabelStr;
		View.OKLabel.spacingX = GetLabelSpacingX (okLabelStr);
		View.OKButton.transform.localPosition = new Vector3 (103, -91, 0);

		View.ToggleLabel_UILabel.text = toggleStr;

        if(time > 0)
        {
            View.CancelLabel.text = cancelLabelStr + "(" + time + ")";
			View.CancelLabel.spacingX = GetLabelSpacingX(View.CancelLabel.text);

            AddOrResetCDTask("WindowPrefabTime", time,
                (currTime) =>{
                    int t = (int)Math.Ceiling(currTime);
                    if (t > 0)
                    {
                        View.CancelLabel.text = cancelLabelStr + "(" + t + ")";
                    }
                    else
                    {
                        View.CancelLabel.text = cancelLabelStr;
                        View.CancelLabel.spacingX = GetLabelSpacingX(cancelLabelStr);
                    }
                }, 
                () =>{
                    View.CancelLabel.text = cancelLabelStr;
                    View.CancelLabel.spacingX = GetLabelSpacingX(cancelLabelStr);
                    OnClickCancelButton();
                }, 1f);
        }
        else
        {
            View.CancelLabel.text = cancelLabelStr;
            View.CancelLabel.spacingX = GetLabelSpacingX(cancelLabelStr);
        }

        View.OKButton.gameObject.SetActive(true);
        View.CancelButton.gameObject.SetActive(true);

        OnOkHandler = onHandler;
        OnCancelHandler = cancelHandler;
	}
	
	private int GetLabelSpacingX (string text)
	{
		if (text.Length <= 2) {
			return 12;
		} else if (text.Length <= 3) {
			return 6;
		} else {
			return 1;
		}
	}

	private void OnClickOkButton ()
	{
        RemoveCDTask("WindowPrefabTime");
        ProxyWindowModule.closeOptWin();		

		if (OnOkHandler != null) {
			OnOkHandler(View.SelectToggle.value);
		}
	}

	private void OnClickCancelButton ()
	{
        RemoveCDTask("WindowPrefabTime");
        ProxyWindowModule.closeOptWin();

		if (OnCancelHandler != null) {
            OnCancelHandler(View.SelectToggle.value);
		}
	}

	private void OnClickCloseButton()
	{
        if (_isCloseCallCancelHandler)
        {
            OnClickCancelButton();
        }
        else if (_isComfirmWin == false)
        {
            OnClickOkButton();
        }
        else
        {
            RemoveCDTask("WindowPrefabTime");
            ProxyWindowModule.closeOptWin();
        }

	}
}

