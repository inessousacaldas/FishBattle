// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  Copy from RequestLoadingTip.cs

using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Request loading tip.
/// </summary>
public class SdkLoadingTipController : MonoBehaviour
{
    private int _loadingCount;
    private GameObject _mGo;

    private List<string> _tipList = new List<string>();

    private SdkLoadingTip  _view;
    // Use this for initialization
    public void InitView()
    {
        _mGo = gameObject;
        _view = BaseView.Create<SdkLoadingTip>(_mGo);
        _mGo.SetActive(false);
    }

    public void _Show(string tip, bool showCircle = false, bool boxCollider = false, float autoCloseTime = 0f)
    {
        _loadingCount++;

        _view.LoadingGroup_Transform.gameObject.SetActive(showCircle);
        _view.BlackSprite_BoxCollider.enabled = boxCollider;

        _tipList.Add(tip);

        if (showCircle)
        {
            UpdateTip();
        }
        _mGo.SetActive(true);

        if (autoCloseTime > 0f)
        {
            CancelInvoke("_Reset");
            Invoke("_Reset", autoCloseTime);
        }
    }

    private void UpdateTip()
    {
#if UNITY_EDITOR
        string tips = "";
        tips = string.Join("\n\n", _tipList.ToArray());
        _view.TipLabel_UILabel.text = tips;
#endif
    }

    public void _Stop(string tip)
    {
        _loadingCount--;

        if (_tipList.Contains(tip))
        {
            _tipList.Remove(tip);
        }

        if (_loadingCount > 0)
        {
            UpdateTip();
            return;
        }

        _Reset();
    }

    public void _Reset()
    {
        _tipList.Clear();
        _loadingCount = 0;
        _mGo.SetActive(false);
        CancelInvoke("_Reset");
    }

    #region Static Func

    private static SdkLoadingTipController _instance;

    public static void Setup(GameObject parent)
    {
        GameObject prefab = AssetPipeline.ResourcePoolManager.Instance.LoadUI(SdkLoadingTip.NAME); 
        if (prefab != null)
        {
            if (parent == null)
            {
                Debug.LogError("SdkLoadingTipController Setup not find parent");
                return;
            }
            GameObject loadingTip = NGUITools.AddChild(parent, prefab);
            loadingTip.name = SdkLoadingTip.NAME;
            _instance = loadingTip.GetMissingComponent<SdkLoadingTipController>();
            _instance.InitView();
            NGUITools.AdjustDepth(loadingTip, SdkModuleMgr.SdkTopLayer);
        }
    }

    public static void Show(string tip, bool showCircle = false, bool boxCollider = false, float autoCloseTime = 0f)
    {
        if (_instance != null)
        {
            if (showCircle || boxCollider)
            {
                _instance._Show(tip, showCircle, boxCollider, autoCloseTime);
            }
        }
    }

    public static void Stop(string tip)
    {
        if (_instance != null)
        {
            _instance._Stop(tip);
        }
    }

    public static void Reset()
    {
        if (_instance != null)
        {
            _instance._Reset();
        }
    }

    #endregion
}