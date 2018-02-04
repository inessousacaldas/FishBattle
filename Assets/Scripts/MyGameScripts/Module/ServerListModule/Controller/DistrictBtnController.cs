using UnityEngine;

public class DistrictBtnController : MonoViewController<DistrictBtn>
{

   
    private int _index;
    private System.Action<int,bool> _callBack;
    private string _btnName;
   

    

protected override void RegistCustomEvent ()
	{
		base.RegistCustomEvent ();        EventDelegate.Set(View.DistrictBtn_UIButton.onClick, OnBtnClick);
    }

    private void OnBtnClick()
    {
        if (_callBack != null)
            _callBack(_index,false);
    }

    public void SetData(int index,string nameStr,System.Action<int,bool> callBack)
    {
        
        _index = index;
        View.NameLbl_UILabel.text = nameStr;//.WrapColor(ColorConstantV3.Color_SealBrown);
        _btnName = nameStr;
        _callBack = callBack;
    }

    public void SetSelected(bool select)
    {
        View.Bg_UISprite.spriteName = select ? "Opt_1_On" : "Opt_1_Off";
        View.NameLbl_UILabel.color = select ? ColorExt.HexStrToColor("ffffff") : ColorExt.HexStrToColor("464646");
        View.NameLbl_UILabel.effectStyle = select ? UILabel.Effect.Outline : UILabel.Effect.None;
    }

    public void SetRedPointState(bool state)
    {
        View.RedPoint_UISprite.gameObject.SetActive(state);
    }
}
