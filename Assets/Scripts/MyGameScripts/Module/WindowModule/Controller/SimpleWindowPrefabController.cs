using UnityEngine;
using System;

public class SimpleWindowPrefabController : MonoViewController<SimpleWindowPrefab>
{


	
	#region

    
	
	protected override void RegistCustomEvent ()
	{
		base.RegistCustomEvent ();		EventDelegate.Set (View.simple_OKButton.onClick, OnClickOkButton);
		EventDelegate.Set (View.simple_CancelButton.onClick, OnClickCancelButton);
	}
	
	
	#endregion
	
	public event Action OnOkHandler;
	public event Action OnCancelHandler;
	private bool _topWin = false;
	
	public void OpenConfirmWindow (string msg, 
	                               Action onHandler = null,
	                               Action cancelHandler = null,
	                               UIWidget.Pivot pivot = UIWidget.Pivot.Left,
									string okLabelStr = "确定",
									string cancelLabelStr = "取消",
									int time = 0,
									bool closeWinTimeForOk = false)
	{
	
		

		char[] strArr = msg.ToCharArray ();
		if(strArr.Length < 19)
		{
			View.simple_InfoLabel.pivot = UIWidget.Pivot.Center;
		}else
		{
			View.simple_InfoLabel.pivot = UIWidget.Pivot.Left;
		}
		View.simple_InfoLabel.text = msg;

		View.simple_OKLabel.text = okLabelStr;
		View.simple_OKLabel.spacingX = GetLabelSpacingX (okLabelStr);
		View.simple_OKButton.transform.localPosition = new Vector3 (103, -64, 0);

		View.simple_CancelLabel.text = cancelLabelStr;
		View.simple_CancelLabel.spacingX = GetLabelSpacingX(cancelLabelStr);

		if(time > 0)
		{
			if (closeWinTimeForOk)
			{
				View.simple_OKLabel.text = okLabelStr + "(" + time + ")";
				View.simple_OKLabel.spacingX = GetLabelSpacingX(View.simple_OKLabel.text);

                AddOrResetCDTask("WindowPrefabTime", time,
					(currTime) =>{
						int t = (int)Math.Ceiling(currTime);
						if (t > 0)
						{
							View.simple_OKLabel.text = okLabelStr + "(" + t + ")";
						}
						else
						{
							View.simple_OKLabel.text = okLabelStr;
							View.simple_OKLabel.spacingX = GetLabelSpacingX(okLabelStr);
						}
					}, 
					() =>{
						View.simple_OKLabel.text = okLabelStr;
						View.simple_OKLabel.spacingX = GetLabelSpacingX(okLabelStr);
						OnClickOkButton();
					}, 1f);					
			}
			else
			{
				View.simple_CancelLabel.text = cancelLabelStr + "(" + time + ")";
				View.simple_CancelLabel.spacingX = GetLabelSpacingX(View.simple_CancelLabel.text);

                AddOrResetCDTask("WindowPrefabTime", time,
					(currTime) =>{
						int t = (int)Math.Ceiling(currTime);
						if (t > 0)
						{
							View.simple_CancelLabel.text = cancelLabelStr + "(" + t + ")";
						}
						else
						{
							View.simple_CancelLabel.text = cancelLabelStr;
							View.simple_CancelLabel.spacingX = GetLabelSpacingX(cancelLabelStr);
						}
					}, 
					() =>{
						View.simple_CancelLabel.text = cancelLabelStr;
						View.simple_CancelLabel.spacingX = GetLabelSpacingX(cancelLabelStr);
						OnClickCancelButton();
					}, 1f);				
			}
		}
		
		View.simple_OKButton.gameObject.SetActive(true);
		View.simple_CancelButton.gameObject.SetActive(true);
		
		OnOkHandler = onHandler;
		OnCancelHandler = cancelHandler;
	}
	
	public void OpenMessageWindow (string msg, 
	                               Action onHandler = null,
	                               UIWidget.Pivot pivot = UIWidget.Pivot.Center,
	                               string okLabelStr = "确定", bool topWin = false)
	{
	
		

		_topWin = topWin;

		char[] strArr = msg.ToCharArray ();
		if(strArr.Length < 19)
		{
			View.simple_InfoLabel.pivot = UIWidget.Pivot.Center;
		}else
		{
			View.simple_InfoLabel.pivot = UIWidget.Pivot.Left;
		}
		View.simple_InfoLabel.text = msg;

		View.simple_OKLabel.text = okLabelStr;
		View.simple_OKLabel.spacingX = GetLabelSpacingX (okLabelStr);
		View.simple_OKButton.transform.localPosition = new Vector3 (0, -48, 0);
		View.simple_OKButton.gameObject.SetActive (true);
		View.simple_CancelButton.gameObject.SetActive (false);
		
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

	private void CloseWin()
	{
		if (_topWin)
		{
			ProxyWindowModule.closeSimpleWinForTop();
		}
		else
		{
			ProxyWindowModule.closeSimpleWin();
		}
	}
}

