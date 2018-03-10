using System;
using System.Collections.Generic;
using AppDto;
using AssetPipeline;
using UniRx;
using UnityEngine;

public static class TipManager
{

    public enum FloatTipsPreEnum
    {
        DialogueHUDTextEntry,
    }
    public static FloatTipText mDialogueText;
    public static FloatTipText mTopDialogueText;
    public static IDisposable _disposable = null;

    //  装载
    public static void Setup()
    {
        _disposable = BackpackDataMgr.GainItemStream.Subscribe(
            itemIDSet =>itemIDSet.ForEach(
                id =>
                {
                    var item = ItemHelper.GetGeneralItemByItemId(id);
                    if (item != null)
                        ShowGainItem(item.icon);
                }));
        GameObject floatTipPrefab = (GameObject)ResourcePoolManager.Instance.LoadUI("FloatTipText");

        GameObject dialogueHudText = NGUITools.AddChild(LayerManager.Root.FloatTipPanel.cachedGameObject, floatTipPrefab);
        mDialogueText = dialogueHudText.GetComponent<FloatTipText>();
        dialogueHudText.transform.localPosition = new Vector3(0f, 70f, 0f);

        GameObject topDialogueHudText = NGUITools.AddChild(LayerManager.Root.TopFloatTipPanel.cachedGameObject,
            floatTipPrefab);
        mTopDialogueText = topDialogueHudText.GetComponent<FloatTipText>();
        topDialogueHudText.transform.localPosition = new Vector3(0f, 70f, 0f);
    }

    #region delayShow

    //  DelayTip数据体
    private struct DelayTipStruct
    {
        public string tip;
        public bool addToSystemNotify;

        public DelayTipStruct(string info, bool addTo)
        {
            tip = info;
            addToSystemNotify = addTo;
        }
    }

    private static List<DelayTipStruct> delayShowList = new List<DelayTipStruct>();
    private static List<string> delayItemShowList = new List<string>();
    private static Dictionary<FloatTipsPreEnum, GameObject> entryPreDic = new Dictionary<FloatTipsPreEnum, GameObject>();

    public static void CheckDelayShow()
    {
        for (int i = 0; i < delayShowList.Count; i++)
        {
            DelayTipStruct delayTipStruct = delayShowList[i];
            AddTip(delayTipStruct.tip, delayTipStruct.addToSystemNotify, false);
        }

        delayShowList.Clear();


        for (int i = 0; i < delayItemShowList.Count; i++)
        {
            ItemCommonTipsViewController.ShowGainItem(delayItemShowList[i]);
        }

        delayItemShowList.Clear();
    }

    #endregion

    #region Text Tip

    //  队列数据体
    private struct TipStruct
    {
        public string tipInfo;
        public Color tipColor;
        public float delayTime;
        public bool topLayer;
        public string tipIcon;
        public FloatTipsPreEnum prefabName;

        public TipStruct(string info, string icon, Color color, float delay, bool top, FloatTipsPreEnum prefab)
        {
            tipInfo = info;
            tipColor = color;
            delayTime = delay;
            topLayer = top;
            tipIcon = icon;
            prefabName = prefab;
        }
    }

    //  队列用于存储获得道具提示
    private static Queue<TipStruct> tipQueue = new Queue<TipStruct>();
    private static bool _isWaiting = true;
    private static float _intervalDelayTime = 0.1f;
    private const string _coolDownName = "TipManagerTimer";

    /// <summary>
    /// 新增弹窗提示，扩充弹窗预制类型
    /// </summary>
    /// <param name="tip"></param>
    /// <param name="prefabName"> 枚举映射预制体名字，默认普通</param>
    public static void AddFloatTip(string tip, FloatTipsPreEnum prefabName = FloatTipsPreEnum.DialogueHUDTextEntry)
    {
        if (string.IsNullOrEmpty(tip))
        {
            return;
        }
        AddColorTip(tip, "", Color.white, 0.5f, false, prefabName);
    }

    public static void AddTip(string tip, bool addToSystemNotify = false, bool delayShow = false, string icon = "", FloatTipsPreEnum prefabName = FloatTipsPreEnum.DialogueHUDTextEntry)
    {
        if (string.IsNullOrEmpty(tip))
        {
            return;
        }

        if (delayShow)
        {
            delayShowList.Add(new DelayTipStruct(tip, addToSystemNotify));
        }
        else
        {
            if (addToSystemNotify)
            {
                GameDebuger.TODO(@"ModelManager.Chat.AddSystemNotifyFromTips(tip);");
            }
            AddColorTip(tip, icon, Color.white, 0.5f, false, prefabName);
        }
    }

    public static void AddTopTip(string tip, string icon = "")
    {
        //因为top层的提示控制不好跟普通层的飘动间隔， 所以先用普通层的
        AddColorTip(tip, icon, Color.white, 0.5f, false);
    }

    public static void AddColorTip(string tip, string icon, Color color, float stayDuration, bool topLayer = false, FloatTipsPreEnum preName = FloatTipsPreEnum.DialogueHUDTextEntry)
    {
        if (!string.IsNullOrEmpty(tip))
        {
            if (mDialogueText != null)
            {
                TipStruct tipStruct = new TipStruct(tip, icon, color, stayDuration, topLayer,preName);
                tipQueue.Enqueue(tipStruct);
                //OnFinishedTime(tipStruct, preName);
                if (_isWaiting)
                {
                    _isWaiting = false;
                    JSTimer.Instance.SetupCoolDown(_coolDownName, _intervalDelayTime, null, delegate
                    {
                        OnFinishedTime(tipStruct, preName);
                    });
                }
            }
        }
    }
    private static void OnFinishedTime(TipStruct tip,FloatTipsPreEnum preName = FloatTipsPreEnum.DialogueHUDTextEntry)
    {
        //  提示
        TipStruct tipInfo = tipQueue.Dequeue(); //tip;
        FloatTipText floatTipText = tipInfo.topLayer ? mTopDialogueText : mDialogueText;
        UIAtlas itemAtlas = UIHelper.GetIconAtlas(tipInfo.tipIcon, "ItemIconAtlas");
        GameObject floatTipEntryPrefab = null;
        if (!entryPreDic.ContainsKey(preName))
        {
            floatTipEntryPrefab = (GameObject)ResourcePoolManager.Instance.LoadUI(preName.ToString());
            entryPreDic.Add(preName, floatTipEntryPrefab);
        }
        else
        {
            floatTipEntryPrefab = entryPreDic[preName];
        }
        if (mDialogueText != null && floatTipEntryPrefab != null)
        {
            if (!mDialogueText.prefab.Contains(floatTipEntryPrefab))
                mDialogueText.prefab.Add(floatTipEntryPrefab);
        }
        floatTipText.Add(tipInfo.tipInfo, itemAtlas, tipInfo.tipIcon, tipInfo.tipColor, _intervalDelayTime, tipInfo.prefabName.ToString());
        //  重新开始
        if (tipQueue.Count > 0)
        {
            JSTimer.Instance.SetupCoolDown(_coolDownName, _intervalDelayTime, null, delegate
            {
                OnFinishedTime(tipInfo, tipInfo.prefabName);
            });
        }
        else
        {
            _isWaiting = true;
        }
    }

    public static void AddGainCurrencyTip(long changeValue, string currencyType, bool delayShow = false,
        string customTip = null)
    {
        if (string.IsNullOrEmpty(customTip))
        {
            customTip = "获得{0}{1}";
        }
        //      string tips = string.Format(customTip,Mathf.Abs(changeValue).ToString("#,#").WrapColor(ColorConstant.Color_Tip_GainCurrency),currencyType);
        string tips = string.Format(customTip,
            Math.Abs(changeValue).ToString().WrapColor(ColorConstant.Color_Tip_GainCurrency), currencyType);
        AddTip(tips, true, delayShow);
    }

    public static void AddLostCurrencyTip(long changeValue, string currencyType, bool delayShow = false,
        string customTip = null)
    {
        if (string.IsNullOrEmpty(customTip))
        {
            customTip = "消耗{0}{1}";
        }
        //      string tips = string.Format(customTip,Mathf.Abs(changeValue).ToString("#,#").WrapColor(ColorConstant.Color_Tip_LostCurrency),currencyType);
        string tips = string.Format(customTip,
            Math.Abs(changeValue).ToString().WrapColor(ColorConstant.Color_Tip_LostCurrency), currencyType);
        AddTip(tips, true, delayShow);
    }

    public static void AddGainCurrencyTip(long changeValue, int itemId, bool delayShow = false)
    {
        //      string tips = string.Format("获得{0}{1}",Mathf.Abs(changeValue).ToString("#,#").WrapColor(ColorConstant.Color_Tip_GainCurrency),ItemIconConst.GetIconConstByItemId(itemId));
        string tips = string.Format("获得{0}{1}",
            Math.Abs(changeValue).ToString().WrapColor(ColorConstant.Color_Tip_GainCurrency),
            ItemIconConst.GetIconConstByItemId((AppVirtualItem.VirtualItemEnum)itemId));
        AddTip(tips, true, delayShow);
    }

    public static void AddLostCurrencyTip(long changeValue, int itemId, bool delayShow = false)
    {
        //      string tips = string.Format("消耗{0}{1}",Mathf.Abs(changeValue).ToString("#,#").WrapColor(ColorConstant.Color_Tip_LostCurrency),ItemIconConst.GetIconConstByItemId(itemId));
        string tips = string.Format("消耗{0}{1}",
            Math.Abs(changeValue).ToString().WrapColor(ColorConstant.Color_Tip_LostCurrency),
            ItemIconConst.GetIconConstByItemId((AppVirtualItem.VirtualItemEnum)itemId));
        AddTip(tips, true, delayShow);
    }

    #endregion

    #region ShowGainItem 飘物品到背包动画

    public static void ShowGainItem(string icon, bool isDelayShow = false)
    {
        if (isDelayShow)
            delayItemShowList.Add(icon);
        else
        {
            ItemCommonTipsViewController.ShowGainItem(icon);
        }
    }
    #endregion
    /// <summary>
    /// 退出、重登清理Tip数据
    /// </summary>
    public static void Dispose()
    {
        if(delayShowList != null)
            delayShowList.Clear();
        if(delayItemShowList != null)
            delayItemShowList.Clear();
        if (_disposable != null)
        _disposable.Dispose();
        _disposable = null;
    }

}