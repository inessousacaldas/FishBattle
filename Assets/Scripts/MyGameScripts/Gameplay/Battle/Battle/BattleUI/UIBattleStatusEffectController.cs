using System;
using DG.Tweening;
using UnityEngine;


public sealed class EffectCtrlData{
    public enum TweenType
    {
        Damege
        , SkillName
    }
    
    public TweenType _tweenType;
    public Transform target;
    public string effectName;
    public string msg;
    public int fontIndex;
    public float duration;
    public string skillName;

}

public class UIBattleStatusEffectController : MonoViewController<BattleHUDText>
{
    
    public static readonly string[] fontType = { "ReduceHPFont", "AddHPFont", "CritFont", "AddHPCritFont", "ReduceVirtualHPFont" };
    
    #region IViewController

    protected override void AfterInitView()
    {
        View.followTarget.AlwaysVisible = true;
        View.followTarget.gameCamera = LayerManager.Root.BattleCamera_Camera;
        View.followTarget.uiCamera = LayerManager.Root.UICamera.cachedCamera;
    }


    protected override void OnDispose()
    {
        Clear();
    }

    private void Clear()
    {
        View.followTarget.offset = Vector3.zero;
        View.followTarget.target = null;
        View.statusSprite.transform.localPosition = Vector3.zero;
        View.msgLbl.transform.localPosition = Vector3.zero;
        View.damageInfo.transform.localPosition = Vector3.zero;
        View.skillInfo.transform.localPosition = Vector3.zero;

        AssetPipeline.ResourcePoolManager.Instance.DespawnUI(this.gameObject);    
    }

    #endregion

    public void ShowStatusEffect(Transform target, string effectName)
    {
        if (target == null)
            return;

        HideAll();
        View.statusSprite.cachedGameObject.SetActive(true);
        View.statusSprite.spriteName = effectName;
        View.statusSprite.MakePixelPerfect();

        View.followTarget.target = target;
        _view.tableGO_UITable.Reposition();
    }

    public void ShowEffect(EffectCtrlData data)
    {
        switch (data._tweenType)
        {
            case EffectCtrlData.TweenType.Damege:
                ShowDamage(data.target, data.msg, data.fontIndex, data.duration, data.effectName);
                break;
            case EffectCtrlData.TweenType.SkillName:
                ShowSkillName(data.skillName, data.target);
                break;
            default:
                break;
        }
    }

    public void ShowDamage(Transform target, string msg, int fontIndex, float duration, string effectName)
    {
        if (target == null)
            return;

        HideAll();
        View.damageLbl.cachedGameObject.SetActive(true);

        View.damageLbl.bitmapFont = FontManager.GetFont(fontType[fontIndex]);
        View.damageLbl.fontSize = 22;
        View.damageLbl.text = msg;

        if (fontIndex == 2)
        {
            View.critSprite.cachedGameObject.SetActive(true);
            View.critSprite.spriteName = "reduceHpCrit";
        }
        else if (fontIndex == 3)
        {
            View.critSprite.cachedGameObject.SetActive(true);
            View.critSprite.spriteName = "addHpCrit";
        }
        else
        {
            View.critSprite.cachedGameObject.SetActive(false);
        }

        if (!string.IsNullOrEmpty(effectName))
        {
            View.statusSprite.cachedGameObject.SetActive(true);
            View.statusSprite.spriteName = effectName;
            View.statusSprite.MakePixelPerfect();
        }
        _view.tableGO_UITable.Reposition();
        View.followTarget.target = target;
        if (fontIndex != 4)
        {
            View.damageInfo.transform.DOLocalMoveY(35f, duration).SetRelative().OnComplete(DestroyMe);
        }
        else
        {
            View.damageInfo.transform.DOLocalMove(new Vector3(-70f, 35f, 0), duration).SetRelative().OnComplete(DestroyMe);
        }
    }

    public void ShowSkillName(string name, Transform target)
    {
        if (target == null)
        {
            GameDebuger.LogError(string.Format("[错误]ShowSkillName failed , target == null , name:{0}", name));
            return;
        }


        HideAll();

        View.skillInfo.SetActive(true);
        View.skillSpriteAnimation.ResetToBeginning();
        View.skillNameLbl.cachedGameObject.SetActive(false);

        View.skillNameLbl.text = "[b]" + name;

        View.followTarget.target = target;

        CancelInvoke("HideSkillEff");
        CancelInvoke("DoShowSkillName");

        Invoke("HideSkillEff", 1f);
        Invoke("DoShowSkillName", 0.02f);
    }

    private void DoShowSkillName()
    {
        //View.skillNameLbl.alpha = 1;
        View.skillNameLbl.gameObject.SetActive(true);
        UIHelper.PlayAlphaTween(View.skillNameLbl, 1f, 0f, 0.5f, 0.45f);
    }

    private void HideSkillEff()
    {
        View.skillInfo.SetActive(false);
        DestroyMe();
    }

    private void HideAll()
    {
        View.damageLbl.cachedGameObject.SetActive(false);
        View.statusSprite.gameObject.SetActive(false);
        View.skillInfo.SetActive(false);
        View.msgLbl.cachedGameObject.SetActive(false);
        View.critSprite.cachedGameObject.SetActive(false);
    }

    private void DestroyMe()
    {
        Clear();
    }

}