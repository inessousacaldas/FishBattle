using System;
using AssetPipeline;
using UnityEngine;


public class CharacterHeadHud
{
    private ModelHeadHUDView _headHUDView;

    private Vector3 headFollowerOffset;
    private Transform headFollowerTrans;
    private string headName;

    public ModelHeadHUDView headHUDView
    {
        get
        {
            return _headHUDView;
        }
    }
    public CharacterHeadHud(Transform _headFollowerTrans, Vector3 _headFollowerOffset, string _headName)
    {
        ResetHudFollower(_headFollowerTrans, _headFollowerOffset, _headName);
        LoadPrefab();
    }
    public void ResetHudFollower(Transform _headFollowerTrans, Vector3 _headFollowerOffset, string _headName)
    {
        headFollowerTrans = _headFollowerTrans;
        headFollowerOffset = _headFollowerOffset;
        headName = _headName;

        if (_headHUDView != null && _headHUDView.gameObject != null)
        {
            SetHeadFollower();
        }
    }

    private void LoadPrefab()
    {
        TryLoadHeadPrefab();
        ResetHeadPrefab();
    }
    private void TryLoadHeadPrefab()
    {
        if (_headHUDView == null || _headHUDView.gameObject == null)
        {
            GameObject hudPrefab = null;
            hudPrefab = ResourcePoolManager.Instance.SpawnUIGo("ModelHeadHUDView", LayerManager.Root.SceneUIHUDPanel.gameObject);
            hudPrefab.name = headName;
            _headHUDView = BaseView.Create<ModelHeadHUDView>(hudPrefab);
            SetHeadFollower();
        }
    }

    private void SetHeadFollower()
    {
        _headHUDView.follower.gameCamera = LayerManager.Root.SceneCamera;
        _headHUDView.follower.uiCamera = LayerManager.Root.UICamera.cachedCamera;
        _headHUDView.follower.target = headFollowerTrans;
        _headHUDView.follower.offset = headFollowerOffset;
        _headHUDView.follower.enabled = true;
        _headHUDView.follower.AlwaysVisible = true;
    }

    private void ResetHeadPrefab()
    {
        if (_headHUDView != null && _headHUDView.gameObject != null)
        {
            Transform[] transForms = _headHUDView.gameObject.GetComponentsInChildren<Transform>();
            for (int i = 0; i < transForms.Length; i++)
            {
                transForms[i].gameObject.SetActive(true);
            }
            MonoBehaviour[] monoBehaviours = _headHUDView.gameObject.GetComponentsInChildren<MonoBehaviour>();
            for (int i = 0; i < monoBehaviours.Length; i++)
            {
                monoBehaviours[i].enabled = true;
            }
        }
    }

    public void Despawn()
    {
        if (_headHUDView != null)
        {
            _headHUDView.follower.target = null;
            headFollowerTrans = null;
            ResourcePoolManager.Instance.DespawnUI(_headHUDView.gameObject);
            _headHUDView = null;
        }
    }

    public void SetHeadHudActive(bool active)
    {
        if (_headHUDView != null)
        {
            _headHUDView.gameObject.SetActive(active);
        }
    }
}
