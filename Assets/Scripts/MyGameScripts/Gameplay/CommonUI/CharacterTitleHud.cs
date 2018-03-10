using System;
using AssetPipeline;
using UnityEngine;

public class CharacterTitleHud
{
    private ModelTitleHUDView _titleHUDView;

    private Transform titleFollowerTrans;
    private Vector3 titleFollowerOffset;
    private string titleName;


    public ModelTitleHUDView titleHUDView
    {
        get
        {
            return _titleHUDView;
        }
    }

    public CharacterTitleHud(Transform _TitleFollowerTrans, Vector3 _titleFollowerOffset, string _titleName)
    {
        ResetHudFollower(_TitleFollowerTrans, _titleFollowerOffset, _titleName);
        LoadPrefab();
    }
    public void ResetHudFollower(Transform _TitleFollowerTrans, Vector3 _titleFollowerOffset, string _titleName)
    {
        titleFollowerTrans = _TitleFollowerTrans;
        titleFollowerOffset = _titleFollowerOffset;
        titleName = _titleName;

        if (_titleHUDView != null && _titleHUDView.gameObject != null)
        {
            SetTitleFollower();
        }
    }

    private void LoadPrefab()
    {
        TryLoadTitlePrefab();
        ResetTitlePrefab();
    }
    private void TryLoadTitlePrefab()
    {
        if (_titleHUDView == null || _titleHUDView.gameObject == null)
        {
            GameObject titleHud = null;
            bool loadFinish = false;
            int i = 5;
            //这里缓存有个bug 先打个补丁 TODO ChaoJian
            while (loadFinish == false && i > 0)
            {
                i--;
                loadFinish = true;
                try
                {
                    titleHud = ResourcePoolManager.Instance.SpawnUIGo("ModelTitleHUDView", LayerManager.Root.SceneHudTextPanel.gameObject);
                    titleHud.name = titleName;
                    _titleHUDView = BaseView.Create<ModelTitleHUDView>(titleHud);
                    SetTitleFollower();
                }
                catch (MissingReferenceException ex)
                {
                    loadFinish = false;
                    GameObject.Destroy(titleHud);
                    GameDebuger.LogException(ex);
                }
            }
        }
    }



    private void SetTitleFollower()
    {
        _titleHUDView.follower.gameCamera = LayerManager.Root.SceneCamera;
        _titleHUDView.follower.uiCamera = LayerManager.Root.SceneHUDCamera;
        _titleHUDView.follower.target = titleFollowerTrans;
        _titleHUDView.follower.offset = titleFollowerOffset;
        _titleHUDView.follower.enabled = true;
    }


    private void ResetTitlePrefab()
    {
        if (_titleHUDView != null && _titleHUDView.gameObject != null)
        {
            Transform[] transForms = _titleHUDView.gameObject.GetComponentsInChildren<Transform>();
            for (int i = 0; i < transForms.Length; i++)
            {
                transForms[i].gameObject.SetActive(true);
            }
            MonoBehaviour[] monoBehaviours = _titleHUDView.gameObject.GetComponentsInChildren<MonoBehaviour>();
            for (int i = 0; i < monoBehaviours.Length; i++)
            {
                monoBehaviours[i].enabled = true;
            }
        }
    }

    public void Despawn()
    {
        if (_titleHUDView != null)
        {
            ResourcePoolManager.Instance.DespawnUI(_titleHUDView.gameObject);
            _titleHUDView = null;
        }
    }


    public void SetTitleHudActive(bool active)
    {
        if (_titleHUDView != null)
        {
            _titleHUDView.gameObject.SetActive(active);
        }
    }
}
