using UnityEngine;
using System;

public class WindowPrefabController : MonoViewController<WindowPrefab>
{

	

	private bool _isComfirmWin = false;
    private bool _isCloseCallCancelHandler = true;

	#region

    

	protected override void RegistCustomEvent ()
	{
		base.RegistCustomEvent ();		EventDelegate.Set (View.OKButton.onClick, OnClickOkButton);
		EventDelegate.Set (View.CancelButton.onClick, OnClickCancelButton);
		EventDelegate.Set (View.CloseBtn.onClick, OnClickCloseButton);
	}

	
	#endregion

	public event Action OnOkHandler;
	public event Action OnCancelHandler;

	public void OpenConfirmWindow (string msg, 
	                                     string title="",
	                            	 	 Action onHandler = null,
	                             		 Action cancelHandler = null,
		UIWidget.Pivot pivot = UIWidget.Pivot.Left,
                                         string okLabelStr = "确定", string cancelLabelStr = "取消", int time = 0, bool isCloseCallCancelHandler = true,bool isClearColor=false)
	{
	
		
		_isComfirmWin = true;
        _isCloseCallCancelHandler = isCloseCallCancelHandler;

		if (string.IsNullOrEmpty(msg))
		{
			msg = "";
		}

		char[] strArr = msg.ToCharArray ();
		if(strArr.Length < 19)
		{
			View.InfoLabel.pivot = UIWidget.Pivot.Center;
		}else
		{
			View.InfoLabel.pivot = pivot;
		}
        if(isClearColor)
            View.InfoLabel.color = Color.white;
        View.InfoLabel.text = msg;


		View.TitleLabel.text = title;
		View.OKLabel.text = okLabelStr;
		View.OKLabel.spacingX = GetLabelSpacingX (okLabelStr);
		View.OKButton.transform.localPosition = new Vector3 (103, View.OKButton.transform.localPosition.y, 0);
		
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

		UpdateBtnStatus ( View.OKButton.gameObject,true,false);
		UpdateBtnStatus ( View.CancelButton.gameObject,true);

        OnOkHandler = onHandler;
        OnCancelHandler = cancelHandler;
	}

	private bool _topWin = false;

	public void OpenMessageWindow (string msg,
	                              string title="",
	                              Action onHandler = null,
	                              UIWidget.Pivot pivot = UIWidget.Pivot.Center,
                                  string okLabelStr = "确定", bool justClose = false, bool topWin = false)
	{
	
		
		_isCloseCallCancelHandler = false;
        _isComfirmWin = justClose;
		_topWin = topWin;

		if (string.IsNullOrEmpty(msg))
		{
			msg = "";
		}

		char[] strArr = msg.ToCharArray ();
		if(strArr.Length < 19)
		{
			View.InfoLabel.pivot = UIWidget.Pivot.Center;
		}else
		{
			View.InfoLabel.pivot = UIWidget.Pivot.Left;
		}
		View.InfoLabel.text = msg;

		View.TitleLabel.text = title;
		View.OKLabel.text = okLabelStr;
		View.OKLabel.spacingX = GetLabelSpacingX (okLabelStr);
		View.OKButton.transform.localPosition = new Vector3 (0, View.OKButton.transform.localPosition.y, 0);
		UpdateBtnStatus ( View.OKButton.gameObject,true,false);
		UpdateBtnStatus ( View.CancelButton.gameObject,false);

		OnOkHandler = onHandler;
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
		CloseWin();	

		if (OnOkHandler != null) {
			OnOkHandler ();
		}
	}

	private void OnClickCancelButton ()
	{
        RemoveCDTask("WindowPrefabTime");
		CloseWin();

		if (OnCancelHandler != null) {
			OnCancelHandler ();
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
		    CloseWin();
        }
	}

	private void CloseWin()
	{
		if (_topWin)
		{
			ProxyWindowModule.CloseForTop();
		}
		else
		{
			ProxyWindowModule.Close();
		}
	}

	private void UpdateBtnStatus(GameObject pUIButton, bool pVisible,bool pUpdateGrid = true)
	{
	    // TODO FISH
		if (pVisible) {
			pUIButton.transform.parent = View.BtnGrid_UIGrid.transform;
			pUIButton.gameObject.SetActive (true);
		} else {
			pUIButton.transform.parent = View.transform;
			pUIButton.gameObject.SetActive (false);
		}
		if(pUpdateGrid)
			View.BtnGrid_UIGrid.Reposition ();
	}

    protected override void OnDispose()
    {
        RemoveCDTask("WindowPrefabTime");
    }
}

