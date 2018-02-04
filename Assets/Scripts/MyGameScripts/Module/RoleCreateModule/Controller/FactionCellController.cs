using System;
using AppDto;
using UnityEngine;

public class FactionCellController : MonoBehaviour
{

    private Faction _faction;
    private Action<FactionCellController> _onSelect;
    private FactionCell _view;

    public Faction Faction
    {
        get { return _faction; }
    }

    public void InitItem(Vector3 pos, Action<FactionCellController> onSelect)
    {
        Transform trans = transform;
        if (_view == null)
        {
            _view = BaseView.Create<FactionCell>(trans);
            RegisterEvent();
        }

        trans.localPosition = Vector3.zero;
        _onSelect = onSelect;
        //_view.FactionIconSprite_UISprite.spriteName = "";
    }

    public void UpdateView(int factionId,Action<FactionCellController> onSelect,GameObject centerdObject)
    {
        if (_view == null)
        {
            _view = BaseView.Create<FactionCell>(transform);
            RegisterEvent();
        }
        _onSelect = onSelect;
        _faction = DataCache.getDtoByCls<Faction>(factionId);
        if(_faction == null)
        {
            GameDebuger.Log("请策划同学查看一下Faction表是否有此id: " + factionId);
            return;
        }
        _view.FactionNameSprite.spriteName = "faction_" + factionId + "name";
        _view.FactionIconBig.spriteName = "faction_" + factionId;
        _view.FactionIconSmall.spriteName = "faction_" + factionId + "_1"; 
        _view.pointLabel1.text = _faction.shortDescription;
        _view.pointLabel2.text = "武器：" + _faction.weaponTypeName;
        _view.pointLabel3.text = "定位：" + _faction.function;
        _view.FactionPointTable.Reposition();
        SetSel(false);
    }

    private void RegisterEvent()
    {
        EventDelegate.Set(_view.FactionBtn_UIButton.onClick, OnSelectItem);
    }
    
    public void SetSel(bool sel)
    {
        _view.Show_Go.SetActive(sel);
        _view.NoShow_Go.SetActive(!sel);
    }

    private void OnSelectItem()
    {
        //if (_onSelect != null)
           // _onSelect(this);
    }

    public void Dispose()
    {
    }
}