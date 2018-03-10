using UnityEngine;

public class BattleOrderEditorCellController : MonolessViewController<BattleOrderEditorCell>
{

    private System.Action<BattleOrderEditorCellController> _OnCellSelect;

    public const int ORDER = 1;

    public const int CLEAR = 2;

    public const int ADD = 3;

    //按钮状态
    public static int btnState = 0;

    private BattleOrderInfo _info;



    protected override void AfterInitView()
    {
        _view.bigAddSprite.gameObject.SetActive(false);
        _view.smallAddSprite.gameObject.SetActive(false);


    }

    protected override void RegistCustomEvent()
    {
        EventDelegate.Set(_view.OrderButton.onClick, OnClickOrderButton);
    }

    public void setData(BattleOrderInfo inf, System.Action<BattleOrderEditorCellController> OnCellSelect = null)
    {
        info = inf;
        _OnCellSelect = OnCellSelect;
        _view.btnLabel.text = info.orderName;
        _view.panSprite.gameObject.SetActive(info.canEdit);
        _view.bigAddSprite.gameObject.SetActive(info.isAddButton);
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


}

