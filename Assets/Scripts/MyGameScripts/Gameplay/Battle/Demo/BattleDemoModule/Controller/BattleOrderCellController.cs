using UnityEngine;

public class BattleOrderCellController : MonolessViewController<BattleOrderCell>
{

    private System.Action<BattleOrderCellController> _OnCellSelect;
    //按钮状态
    public  static int btnState = 0;

    private BattleOrderInfo _info;

    protected override void AfterInitView()
    {
        _view.smallAddSprite.gameObject.SetActive(false);
        Selected = false;
    }

    protected override void RegistCustomEvent()
    {
        EventDelegate.Set(_view.OrderButton.onClick, OnClickOrderButton);
    }

    public void setData(BattleOrderInfo inf, System.Action<BattleOrderCellController> OnCellSelect = null)
    {
        info = inf;
        _OnCellSelect = OnCellSelect;
        _view.smallAddSprite.gameObject.SetActive(info.isAddButton);
        if (_info.isClearButton)
        {
            _view.btnLabel.useFloatSpacing = true;
            _view.btnLabel.text = "清除指令".WrapColor(ColorConstantV3.Color_Red_Str);
        }
        else if (_info.isAllClearButton)
        {
            _view.btnLabel.useFloatSpacing = true;
            _view.btnLabel.text = "全部清除".WrapColor(ColorConstantV3.Color_Red_Str);
        }
        else
        {
            _view.btnLabel.useFloatSpacing = false;
            _view.btnLabel.text = _info.orderName;
        }
    }

    public BattleOrderInfo info
    {
        get
        {
            return _info;
        }
        set
        {
            _info = value;
        }
    }

    private void OnClickOrderButton()
    {
        if (_OnCellSelect != null)
        {
            _OnCellSelect(this);
        }
    }

    private bool mSelected = true;

    public bool Selected
    {
        get
        { 
            return mSelected;
        }
        set
        {
            if (mSelected != value)
            {
                mSelected = value;
                View.SpriteSelected.SetActive(mSelected);
            }
        }
    }
}

