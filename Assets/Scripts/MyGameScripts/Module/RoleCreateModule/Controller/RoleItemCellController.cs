using System;
using AppDto;
using UnityEngine;
using System.Collections.Generic;

public class RoleItemCellController : MonolessViewController<RoleItemCell>
{
    private MainCharactor _mainCharactor;

    private Action<RoleItemCellController> _onSelect;

    private List<ModelHelper.AnimType> animString = new List<ModelHelper.AnimType>();

    private const string FactionCellEffectBack = "ui_eff_RoleCreate_Faction";
    private const string FactionCellEffectFront = "ui_eff_RoleCreate_Faction_Front";
    private OneShotUIEffect _selectEffectBack;
    private OneShotUIEffect _selectEffectFront;
    public List<ModelHelper.AnimType> AnimString
    {
        get { return animString; }
    }

    public MainCharactor Charactor
    {
        get { return _mainCharactor; }
    }

    protected override void RegistCustomEvent ()
    {
        base.RegistCustomEvent(); EventDelegate.Set(View.RoleItemCell_UIButton.onClick, OnSelectItem);
    }
    

    public void SetData(MainCharactor charactor, Action<RoleItemCellController> onSelect)
    {
        _mainCharactor = charactor;
        _onSelect = onSelect;
        View.Lock.SetActive(!charactor.usable);
        View.RoleItemCell_UISprite.isGrey = !charactor.usable;
        if (View.RoleItemCell_UISprite.mainTexture == null)
        {
            AssetPipeline.ResourcePoolManager.Instance.LoadImage("roll2d_hero_" + charactor.id, asset =>
            {
                UIHelper.DisposeUITexture(View.RoleItemCell_UISprite);
                Texture2D texture = asset as Texture2D;
                if (texture != null)
                {
                    View.RoleItemCell_UISprite.mainTexture = texture;
                }
            });
        }
        SetSelect(false);
        string anim = charactor.anim;
        var ary = anim.Split('|');
        ary.ForEach(e =>
        {
            try
            {
                var val = (ModelHelper.AnimType)Enum.Parse(typeof(ModelHelper.AnimType), e);
                animString.Add(val);
            }
            catch (Exception d)
            {
                GameDebuger.Log("检查MainCharacter字段anim配置是否有误");
            }

        });
    }

    public void SetSelect(bool select)
    {
        if (select)
        {
                if (_selectEffectBack == null)
                {
                    _selectEffectBack = OneShotUIEffect.BeginFollowEffect(FactionCellEffectBack, _view.RoleItemCell_Bg,Vector2.zero);
                }
                if (_selectEffectFront == null)
                {
                    _selectEffectFront = OneShotUIEffect.BeginFollowEffect(FactionCellEffectFront, _view.eff_Front, Vector2.zero);
                }
            View.RoleItemCell_Bg.spriteName = "bg_Role_2";
            View.tween.PlayForward();
            View.RoleItemCell_Bg.SetDimensions(151, 280);
        }
        else
        {
            View.tween.PlayReverse();
            View.RoleItemCell_Bg.spriteName = "bg_Role_1";
            View.RoleItemCell_Bg.SetDimensions(144, 278);
        }
        if (_selectEffectBack != null)
            _selectEffectBack.SetActive(select);
        // View.eff_Back.gameObject.SetActive(select);
        View.RoleItemCell_UISprite.isGrey = !select;
        View.eff_Front.gameObject.SetActive(select);
        View.eff_Go.SetActive(select);
    }

    private void OnSelectItem()
    {
        if (_onSelect != null)
            _onSelect(this);
    }

    
}