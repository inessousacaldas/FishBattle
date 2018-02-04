using UnityEngine;
using System;

public class WindowInputPrefabController : MonoViewController<WindowInputPrefab>
{


    private int _type = 0;
    public const int TYPE_CHANGE_ROLE_NAME = 1;
    public const int CHANGE_WARE_NAME = 2;

	private int _minChar = 0;//最小输入
	private int _maxChar = 0;//最大输入
    private bool _illegalStrCheck;//非法字符检测

    #region

    
	
	protected override void RegistCustomEvent ()
	{
		base.RegistCustomEvent ();		EventDelegate.Set (View.OKButton.onClick, OnClickOkButton);
		EventDelegate.Set (View.CancelButton.onClick, OnClickCancelButton);
		EventDelegate.Set (View.CloseBtn.onClick, OnClickCloseButton);
	}
	
	
	#endregion
	
	public event Action <string>OnOkHandler;
	public event Action OnCancelHandler;
	
	public void OpenInputWindow (int minChar = 0,
	                             int maxChars = 0,
		                           string title="",
	                               string desContent = "",
	                               string inputTips = "",
	                               string inputVlaue = "",
	                               Action <string>onHandler = null,
	                               Action cancelHandler = null,
	                               UIWidget.Pivot pivot = UIWidget.Pivot.Left,
	                               string okLabelStr = "确定", string cancelLabelStr = "取消", int time = 0,int type = 0)
	{
	
		

		_minChar = minChar;
		_maxChar = maxChars;
        _type = type;

        View.NameInput.characterLimit = 20;
		View.NameInput.label.pivot = pivot;

		View.NameInput.label.text = inputTips;
		View.NameInput.value =   inputVlaue;
        View.NameInput.isSelected = string.IsNullOrEmpty(inputVlaue) ? false : true;

        View.TitleLabel.text = title;
		View.desLabel.text = desContent;
		View.OKLabel.text = okLabelStr;
		View.OKLabel.spacingX = GetLabelSpacingX (okLabelStr);
		View.OKButton.transform.localPosition = new Vector3 (118, -80, 0);

		if(time > 0)
		{
			View.CancelLabel.text = cancelLabelStr + "(" + time + ")";
			View.CancelLabel.spacingX = GetLabelSpacingX(View.CancelLabel.text);
			
            AddOrResetCDTask("InputWindowPrefabTime", time,
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

		View.DefaultButton.gameObject.SetActive(false);
		View.defaultLabel.gameObject.SetActive(false);
		
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
		if (string.IsNullOrEmpty(View.NameInput.value) && _minChar > 0)
		{
			TipManager.AddTip("输入不能为空");
			return;
		}
        
		string error = AppStringHelper.ValidateStrLength (View.NameInput.value, _minChar, _maxChar);
		if (!string.IsNullOrEmpty(error))
		{
			TipManager.AddTip (error);
			return;
		}

        string tValue = View.NameInput.value;
		RemoveCDTask("InputWindowPrefabTime");

	    switch (_type)
	    {
            case CHANGE_WARE_NAME:
                //不关闭界面
	            break;
            case TYPE_CHANGE_ROLE_NAME:
                if (ModelManager.Player.hasEnoughMoneyChangeName())
                {
                    ProxyWindowModule.closeInputWin();
                }
                break;
            default:
                ProxyWindowModule.closeInputWin();
                break;

	    }
		if (OnOkHandler != null) {
            OnOkHandler (tValue);
		}
	}
	
	private void OnClickCancelButton ()
	{
		RemoveCDTask("InputWindowPrefabTime");
		ProxyWindowModule.closeInputWin();
		
		if (OnCancelHandler != null) {
			OnCancelHandler ();
		}
	}

	private void OnClickCloseButton()
	{
		RemoveCDTask("InputWindowPrefabTime");
		ProxyWindowModule.closeInputWin();
		if (OnCancelHandler != null) {
			OnCancelHandler ();
		}
	}
}

