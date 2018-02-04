// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  RequestLoadingTip.cs
// Author   : SK
// Created  : 2013/8/29
// Purpose  : 
// **********************************************************************

using System.Collections.Generic;
using AssetPipeline;
using UnityEngine;

/// <summary>
///     Request loading tip.
/// </summary>
public class RequestLoadingTip : MonoBehaviour
{
    private int _loadingCount;
    private GameObject _mGo;

    private List<string> _tipList = new List<string>();

    private RequestLoadingTipPrefab _view;
    // Use this for initialization
    public void InitView()
    {
        _mGo = gameObject;
		_view = BaseView.Create<RequestLoadingTipPrefab>(_mGo);

        _mGo.SetActive(false);
    }

    private void _Show(string tip, bool showCircle = false, bool boxCollider = false, float autoCloseTime = 0f)
    {
        _loadingCount++;

//		GameDebuger.Log("RequestLoadingTipShow "+tip +" "+_loadingCount);

        _view.LoadingGroup_Transform.gameObject.SetActive(showCircle);
        _view.BlackSprite_BoxCollider.enabled = boxCollider;

        _tipList.Add(tip);

        if (showCircle)
        {
            UpdateTip();
        }
        _mGo.SetActive(true);

        if (autoCloseTime <= 0f)
            autoCloseTime = 3f;

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

    private void _Stop(string tip)
    {
        _loadingCount--;

        if (_tipList.Contains(tip))
        {
            _tipList.Remove(tip);
        }

        GameDebuger.Log("RequestLoadingTipStop" + " " + _loadingCount);

        if (_loadingCount > 0)
        {
            UpdateTip();
            return;
        }

        _Reset();
    }

    private void _Reset()
    {
        _tipList.Clear();
        _loadingCount = 0;
        _mGo.SetActive(false);
        CancelInvoke("_Reset");
    }

    #region Static Func

    private static RequestLoadingTip _instance;

    public static void Setup()
    {
        GameObject prefab = ResourcePoolManager.Instance.LoadUI("RequestLoadingTipPrefab") as GameObject;
        if (prefab != null)
        {
            GameObject parent = LayerManager.Root.LockScreenPanel.cachedGameObject;
            if (parent == null)
            {
                Debug.LogError("RequestLoadingTip Setup not find parent");
                return;
            }
            GameObject loadingTip = NGUITools.AddChild(parent, prefab);
            loadingTip.name = "RequestLoadingTip";
            _instance = loadingTip.GetMissingComponent<RequestLoadingTip>();
            _instance.InitView();
        }
    }

    public static void Show(string tip, bool showCircle = false, bool boxCollider = false, float autoCloseTime = 0f)
    {
        if (_instance == null) return;
        if (showCircle || boxCollider)
        {
            _instance._Show(tip, showCircle, boxCollider, autoCloseTime);
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