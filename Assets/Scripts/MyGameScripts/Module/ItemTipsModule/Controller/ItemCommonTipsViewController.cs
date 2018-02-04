using System.Collections.Generic;
using UnityEngine;

//	DelayTip数据体
public struct DelayTipStruct
{
    public string tip;
    public bool addToSystemNotify;

    public DelayTipStruct(string info, bool addTo)
    {
        tip = info;
        addToSystemNotify = addTo;
    }
}

public sealed partial class ItemCommonTipsViewController : MonoViewController<ItemCommonTipsView>
{
    private static Queue<string> WaitingShowGainItemQueue = new Queue<string>();
    private bool GainItemShowing = false;

    public static void ShowGainItem(string icon)
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<ItemCommonTipsViewController>(
            ItemCommonTipsView.NAME
            , UILayerType.FloatTip
            , false
        );

        WaitingShowGainItemQueue.Enqueue(icon);
        ctrl.Show();

    }

    protected override void OnDispose()
    {
        WaitingShowGainItemQueue.Clear();
        JSTimer.Instance.CancelCd("ItemGainTipsCellController");
        GainItemShowing = false;
    }

    private void Show()
    {
        GameLog.Log_BAG("ShowNextGainItem---");
        if (GainItemShowing || WaitingShowGainItemQueue.Count <= 0)
            return;

        GainItemShowing = true;
        
        ShowNextGainItem();
    }

    private void ShowNextGainItem(){
        if (WaitingShowGainItemQueue.Count > 0)
        {
            var icon = WaitingShowGainItemQueue.Dequeue();
            var cell = AddCachedChild<ItemGainTipsCellController, ItemGainTipsCell>(
                View.ItemGainTipsCellAnchor
                , ItemGainTipsCell.NAME
            );

            var end =
                View.ItemGainTipsCellAnchor.transform.InverseTransformPoint(
                    ProxyMainUI.GetBagBtnPosition());

            var begin = new Vector3(0f, end.y, 0f);

            AudioManager.Instance.PlaySound("sound_UI_item_got");

            cell.ShowTips(icon, 0.5f, begin, end, () =>
                {
                    GameLog.Log_BAG("tween finish");
                    RemoveCachedChild<ItemGainTipsCellController, ItemGainTipsCell>(cell);
                    if (WaitingShowGainItemQueue.Count <= 0)
                    {
                        GainItemShowing = false;
                        JSTimer.Instance.CancelCd("ItemGainTipsCellController");
                        UIModuleManager.Instance.CloseModule(ItemCommonTipsView.NAME);
                    }
                    else
                    {
                        JSTimer.Instance.SetupCoolDown("ItemGainTipsCellController" + GetHashCode(), 0.2f, null, ShowNextGainItem);
                    }
                });

        }
    }
}