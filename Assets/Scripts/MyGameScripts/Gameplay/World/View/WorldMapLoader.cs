// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  WorldMapLoader.cs
// Author   : SK
// Created  : 2013/2/27
// Purpose  : 
// **********************************************************************

using System;
using AppDto;
using AssetPipeline;
using UnityEngine;

public class WorldMapLoader : MonoBehaviour
{
    private static WorldMapLoader _instance;
    private bool _isBattleScene;

    private Action _loadMapFinish;
    private int _mapId;

    // 战斗
    private GameObject BattleLayer;
    // 场景
    private GameObject SceneLayer;

    public static WorldMapLoader Instance
    {
        get
        {
            CreateInstance();
            return _instance;
        }
    }

    private static void CreateInstance()
    {
        if (_instance == null)
        {
            var go = new GameObject("_WorldMapLoader");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<WorldMapLoader>();
            _instance.SceneLayer = LayerManager.Root.SceneLayer;
            _instance.BattleLayer = LayerManager.Root.BattleLayer;
            LayerManager.Root.AstarPath.GetMissingComponent<AstarPath>();
        }
    }

    public void UnLoadMapResource(int pMapID)
    {
        UnLoadMapResource(Get3DMapName(pMapID));
    }
    private bool LoadBeforeHandle(int mapId)
    {
        if (_isBattleScene)
        {
            // 战斗
            if (BattleLayer.transform.Find("Battle_" + mapId) == null)
            {
                BattleLayer.RemoveChildren();
                return false;
            }
            return true;
        }
        // 场景
        if (SceneLayer.transform.Find("World_" + mapId) == null)
        {
            SceneLayer.RemoveChildren();

            SceneLayer.SetActive(true);
            BattleLayer.RemoveChildren();
            BattleLayer.SetActive(false);
            return false;
        }
        SceneLayer.SetActive(true);
        BattleLayer.SetActive(false);
        return true;
    }

    public void LoadBattleMap(int mapId, Action onFinish, Action<float> onProgress = null)
    {
        LoadMap(mapId, true, onFinish, onProgress);
    }

    public void LoadWorldMap(int mapId, Vector3 playerPos, Action onFinish, Action<float> onProgress = null, bool isBattleScene = false)
    {
        UpdateSceneShadow(mapId);
        string sceneName = this.Get3DMapName(mapId);
        //SceneLocalGoLoader sceneLocalGoLoader = new SceneLocalGoLoader(sceneName);
        //sceneLocalGoLoader.SetFinishCallback((ICommand cmd) => { onFinish(); });
        _loadMapFinish = onFinish;
        _mapId = mapId;
        _isBattleScene = isBattleScene;
        MySceneManager.Instance.ChangeToScene(sceneName, playerPos, onProgress,()=> {
            this.OnAllSceneLoaded();
        });

    }
    public void LoadWorldMap(int resId, Action onFinish, Action<float> onProgress = null)
    {
        LoadMap(resId, false, onFinish, onProgress);
    }

    private void LoadMap(int mapId, bool isBattleScene, Action onFinish, Action<float> onProgress)
    {
        UpdateSceneShadow(mapId);
        GameDebuger.Log("Load3DMap = " + mapId + ", isBattleScene = " + isBattleScene);
        //        _world2dMapLoader.Close2DMode();
        _loadMapFinish = onFinish;
        _mapId = mapId;
        _isBattleScene = isBattleScene;
        if (LoadBeforeHandle(mapId))
        {
            OnAllSceneLoaded();
        }
        else
        {
            if (_isBattleScene)//changed from Battle_ to Scene_ in 2017-04-14 16:05:34
            {
                AssetPipeline.AssetManager.Instance.LoadLevelAsync(Get3DMapName(mapId), true, OnAllSceneLoaded, onProgress);
            }
            else
            {
                AssetManager.Instance.LoadLevelAsync("EmptyScene", false, () =>
                    {
                        AssetManager.Instance.LoadLevelAsync(Get3DMapName(_mapId), false, OnAllSceneLoaded, onProgress);
                    });
            }
        }
    }

    private void UpdateSceneShadow(int mapID)
    {
        var dto = DataCache.getDtoByCls<SceneMap>(mapID);
        var pShadowDirection = dto == null ? string.Empty : dto.shadowDir;
        if(!string.IsNullOrEmpty(pShadowDirection))
            SceneArtistic.SetShadowDir(pShadowDirection);    
        else
            SceneArtistic.SetShadowDir(SceneArtistic.shadowDir);
    }
    
    private void OnAllSceneLoaded()
    {
        WorldMapLayerHandle();

        CheckEditorObj();
        CheckTerrain();
        CheckEffects();

        if (_loadMapFinish != null)
        {
            _loadMapFinish();
        }

        _loadMapFinish = null;
    }

    //检查编辑器对象，如果有则删除，例如临时的摄像机， 灯光等
    private void CheckEditorObj()
    {
        var objs = GameObject.FindGameObjectsWithTag("EditorOnly");
        for (int i = 0, len = objs.Length; i < len; i++)
        {
            var obj = objs[i];
            Destroy(obj);
        }
    }

    private void CheckTerrain()
    {
        var sceneLayer = LayerManager.Root.SceneLayer;

        if (sceneLayer == null)
        {
            return;
        }

        //批量修改场景的碰撞为Terrain
        //S1用 BoxCollider 代替 MeshCollider ，2017-03-31 20:15:26
        var colliderList = sceneLayer.GetComponentsInChildren<Collider>(false);
        int layer = LayerMask.NameToLayer(GameTag.Tag_Terrain);
        for (int i = 0, len = colliderList.Length; i < len; i++)
        {
            var collider = colliderList[i];
            collider.gameObject.layer = layer;
            collider.gameObject.tag = GameTag.Tag_Terrain;
        }
    }

    private void CheckEffects()
    {
        var sceneEffect = GameObject.FindGameObjectWithTag(GameTag.Tag_SceneEffect);
        LayerManager.Instance.SceneEffect = sceneEffect;

        GameDebuger.TODO(@"SceneHelper.ToggleSceneEffect(ModelManager.SystemData.sceneEffectToggle);");
    }

    private void WorldMapLayerHandle()
    {
        if (_isBattleScene)
        {
            var state = GameObject.Find("BattleStage");
            if (state != null)
            {
                state.name = "Battle_" + _mapId;
                GameObjectExt.AddPoolChild(BattleLayer, state);
            }
        }
        else
        {
            var state = GameObject.Find("WorldStage");

            if (state != null)
            {
                state.name = "World_" + _mapId;
                GameObjectExt.AddPoolChild(SceneLayer, state);
            }
        }
    }

    private void UnLoadMapResource(string resourceName)
    {
        AssetManager.Instance.UnLoadSceneResource(resourceName);
    }
    private string Get3DMapName(int pMapID)
    {
        return string.Format("Scene_{0}", pMapID);
    }

    public void Destroy()
    {
        SceneLayer.RemoveChildren();
        BattleLayer.RemoveChildren();

        SceneLayer.SetActive(false);
        BattleLayer.SetActive(false);
    }
}