using System;
using UnityEngine;

public class ItemGainTipsCellController : MonolessViewController<ItemGainTipsCell>
{
    public Action OnShowFinished;
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Set(_view.ItemTweenPosition.onFinished, () =>
        {
            GameUtil.SafeRun(OnShowFinished);
            OnShowFinished = null;
        });
    }

    public void ShowTips(
        string icon
        , float delay
        , Vector3 from
        , Vector3 to
        , Action _OnShowFinished)
    {
        OnShowFinished = _OnShowFinished;
        this.gameObject.transform.localPosition = new Vector3(from.x, from.y, from.z);
        UIHelper.SetItemIcon(_view.IconSprite, icon);
        _view.ItemTweenPosition.ResetToBeginning();
        _view.ItemTweenPosition.from = from;
        _view.ItemTweenPosition.to = to;

        _view.ItemTweenPosition.delay = delay;
        _view.ItemTweenPosition.duration = 0.5f;

        _view.ItemTweenPosition.enabled = true;

    }

}