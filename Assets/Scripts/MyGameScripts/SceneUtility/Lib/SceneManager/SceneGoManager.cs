using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssetPipeline;
using SceneUtility;


public class SceneGoManager
{
    public readonly MySceneManager master;

    public SceneGoManager(MySceneManager master)
    {
        this.master = master;
    }

    Vector3 unvisiblePos = new Vector3(99999, 99999, 99999);
    bool[] sceneGoLoadingArray;
    Transform[] sceneGoArray;
    SceneGoInfo[] sceneGoInfoArray;
    VisibleState[] sceneGoVisibleArray;
    SceneQuadTree<SceneGoInfo> quadTree;

    bool clearing = false;
    bool inPreload = true;
    Transform SceneGoRoot { get { return LayerManager.Root.SceneLayer.transform; } }
    System.Action onPreloadFinish;
    System.Action onLoadSceneGoFinish;
    System.Action onClearFinish;

    private const float SmallGoSize = 1.5f;
    private const float NormalGoSize = 5f;

    private Camera _sceneCamera;
    private Camera SceneCamera
    {
        get
        {
            if (_sceneCamera == null)
            {
                // 有可能不是这么获取
                _sceneCamera = LayerManager.Root.SceneCamera;
            }
            return _sceneCamera;
        }
    }

    public void SetupConfig(AllSceneGoInfo allSceneGoInfo)
    {
        this.sceneGoInfoArray = allSceneGoInfo.sceneGoInfoArray;
        UpdateSceneGoType();
        this.sceneGoVisibleArray = new VisibleState[sceneGoInfoArray.Length];
        this.sceneGoLoadingArray = new bool[sceneGoInfoArray.Length];
        this.quadTree = new SceneQuadTree<SceneGoInfo>(new Vector2(10, 10), 5);
        ProfileHelper.SystimeBegin("quadTree");
        quadTree.Insert(sceneGoInfoArray);
        ProfileHelper.SystimeEnd("quadTree");

        sceneGoArray = new Transform[sceneGoInfoArray.Length];
    }

    public void Preload(System.Action onPreloadFinish)
    {
        inPreload = true;
        this.onPreloadFinish = onPreloadFinish;
        this.StartUpdateVisible();
        this.UpdateVisible();
    }
    void UpdateVisible()
    {
        this.SetAllSceneGo2Unvisible();
        // fish: 只有在断线重连的时候WorldManager.Instance可以是null，可以return
        if (WorldManager.Instance == null)
            return;
        
        var heroPos = WorldManager.Instance.GetHeroWorldPos();
        SceneGoInfo[] visibleList;
        if (heroPos != Vector3.zero)
        {
            visibleList = quadTree.Query(SceneCamera, WorldManager.Instance.GetHeroWorldPos());
        }
        else
        {
            visibleList = quadTree.QueryByCamera(SceneCamera);
        }
        for (int i = visibleList.Length - 1; i >= 0; i--)
        {
            sceneGoVisibleArray[visibleList[i].id] = VisibleState.Visible;
        }

        //先Destroy在Load，利用缓存
        DestroyAllUnvisibleGo();
        LoadAllVisible();
    }

    void SetVisible(int sceneGoId)
    {
        bool needSetPos = sceneGoVisibleArray[sceneGoId] == VisibleState.Unvisible; //TODO 无用的代码
        sceneGoVisibleArray[sceneGoId] = VisibleState.Visible;
        Transform trans = this.sceneGoArray[sceneGoId];
        if (trans == null)
            LoadSceneGo(sceneGoId);
        else if (needSetPos)
            SetSceneGoPos(sceneGoId, trans);
    }

    void LoadAllVisible()
    {
        for (int i = 0; i < sceneGoInfoArray.Length; i++)
        {
            SceneGoInfo item = sceneGoInfoArray[i];
            int sceneGoId = item.id;
            if (IsSceneGoVisible(sceneGoId))
            {
                Transform trans = sceneGoArray[sceneGoId];
                if (trans == null)
                    LoadSceneGo(sceneGoId);
                //else 
                //SetSceneGoPos(sceneGoId, trans);
            }
        }
    }
    void LoadSceneGo(int sceneGoId)
    {
        if (sceneGoArray[sceneGoId] != null)
        {
            OnLoadSceneGo();
            return;
        }

        if (this.sceneGoLoadingArray[sceneGoId])
            return;

        SceneGoInfo sceneGoInfo = sceneGoInfoArray[sceneGoId];
        string prefabName = sceneGoInfo.prefabName;
        this.sceneGoLoadingArray[sceneGoId] = true;

        //ProfileHelper.SystimeBegin(prefabName);
        AssetPipeline.ResourcePoolManager.Instance.SpawnStreamSceneAsync(prefabName, inPreload, go =>
        {
            go.transform.parent = SceneGoRoot;
            sceneGoArray[sceneGoId] = go.transform;
            this.sceneGoLoadingArray[sceneGoId] = false;
            SetSceneGoPos(sceneGoInfo.id, go.transform);
            this.SetSceneGoLightMap(go, sceneGoInfo);
            OnLoadSceneGo();

            //ProfileHelper.SystimeEnd(prefabName);
        },
        () =>
        {
            Debug.LogError(string.Format("Can Not Load Assets; prefabName:{0} SceneGoId:{1} GameObjectName:{2}", sceneGoInfo.prefabName, sceneGoId, sceneGoInfo.gameobjectName));
            sceneGoLoadingArray[sceneGoId] = false;
            sceneGoVisibleArray[sceneGoId] = VisibleState.Unvisible;
            OnLoadSceneGo();
        }, GetSceneGoLoadPriority(sceneGoInfo));

    }
    void OnLoadSceneGo()
    {
        if ((inPreload || clearing) && IsAllLoadFinish())
        {
            if (inPreload)
            {
                this.inPreload = false;
                if (this.onPreloadFinish != null)
                    this.onPreloadFinish();
            }

            if (clearing)
            {
                this.onLoadSceneGoFinish();
            }
        }
    }

    private void SetSceneGoPos(int sceneGoId, Transform trans)
    {
        if (IsSceneGoVisible(sceneGoId))
        {
            SceneGoInfo sceneGoInfo = this.sceneGoInfoArray[sceneGoId];
            trans.name = sceneGoInfo.gameobjectName;
            trans.position = sceneGoInfo.pos;
            trans.rotation = sceneGoInfo.rotation;
            trans.localScale = sceneGoInfo.scale;
        }
        else
        {
            trans.position = unvisiblePos;
        }
    }

    void SetSceneGoLightMap(GameObject sceneGo, SceneGoInfo sceneGoInfo)
    {
        MeshRenderer[] mrList = sceneGo.GetComponentsInChildren<MeshRenderer>(sceneGo);
        foreach (MeshRenderer mr in mrList)
        {
            foreach (LightMapDetail lmDetail in sceneGoInfo.lightMapDetailList)
            {
                if (lmDetail.gameobjectName.Equals(mr.name))
                {
                    mr.lightmapIndex = lmDetail.index;
                    mr.lightmapScaleOffset = lmDetail.offset;
                    break;
                }
            }
        }
    }

    void DestroyAllUnvisibleGo()
    {
        for (int i = 0; i < this.sceneGoVisibleArray.Length; i++)
        {
            if (this.IsSceneGoVisible(i))
                continue;
            this.DestroySceneGo(i);
        }
    }
    void DestroySceneGo(int sceneGoId)
    {
        Transform trans = this.sceneGoArray[sceneGoId];
        if (trans == null)
            return;
        ResourcePoolManager.Instance.DespawnStreamScene(trans);
        this.sceneGoArray[sceneGoId] = null;
    }
    public void Clear(System.Action onClearFinish)
    {
        if (clearing)
            return;
        inPreload = true;
        clearing = true;
        this.onClearFinish = onClearFinish;
        QueueCommandRunner clearCommand = new QueueCommandRunner();
        clearCommand.Add(new WaitLoadingGoFinishCmd(this));
        clearCommand.Add(new ClearAllSceneGoCmd(this));
        clearCommand.SetFinishCallback(this.OnClearCmdFinish);
        clearCommand.Execute();
    }
    void OnClearCmdFinish(ICommand command)
    {
        this.Reset();
        clearing = false;
        if (this.onClearFinish != null)
            this.onClearFinish();
        this.onClearFinish = null;
    }

    #region UpdateVisible
    Coroutine coroutine;
    float lastUpdateVisibleTime;

    IEnumerator IProcess()
    {
        while (true)
        {
            this.OnProcess();
            yield return null;
        }
    }

    public void StartUpdateVisible()
    {
        if (this.coroutine != null)
            return;
        this.coroutine = JSTimer.Instance.StartCoroutine(IProcess());
    }

    public void StopUpdateVisible()
    {
        JSTimer.Instance.StopCoroutine(this.coroutine);
    }

    void OnProcess()
    {
        if (this.quadTree == null)
            return;
        if (Time.unscaledTime - lastUpdateVisibleTime >= 0.3f && !inPreload && !clearing)
        {
            this.UpdateVisible();
            lastUpdateVisibleTime = Time.unscaledTime;
#if UNITY_EDITOR
            //quadTree.DrawTree(0.3f);
#endif
        }
    }
    #endregion

    #region Helper
    bool IsAllLoadFinish()
    {
        for (int i = 0; i < this.sceneGoArray.Length; i++)
        {
            if (this.IsSceneGoVisible(i) && this.sceneGoArray[i] == null)
            {
                var info = sceneGoInfoArray[i];
                //Debug.LogError(string.Format("UnFinish PrefabName:{0}: GoName:{1} SceneInfoID:{2}", info.prefabName, info.gameobjectName, i));
                return false;
            }
        }
        return true;
    }

    bool IsSceneGoVisible(int sceneGoId)
    {
        return this.sceneGoVisibleArray[sceneGoId] == VisibleState.Visible;
    }


    void SetAllSceneGo2Unvisible()
    {
        for (int i = 0; i < this.sceneGoVisibleArray.Length; i++)
        {
            this.sceneGoVisibleArray[i] = VisibleState.Unvisible;
        }
    }

    void Reset()
    {
        quadTree = null;
        this.sceneGoVisibleArray = null;
        this.sceneGoLoadingArray = null;
        this.sceneGoInfoArray = null;
        this.sceneGoArray = null;
        inPreload = true;
        clearing = false;
        _sceneCamera = null;
    }

    private void UpdateSceneGoType()
    {
        if (sceneGoInfoArray != null)
        {
            for (int i = sceneGoInfoArray.Length - 1; i >= 0; i--)
            {
                var goInfo = sceneGoInfoArray[i];
                if (goInfo.bounds.width2D() <= SmallGoSize && goInfo.bounds.height2D() <= SmallGoSize)
                {
                    goInfo.GoType = SceneGoType.Small;
                }
                else if (goInfo.bounds.width2D() <= NormalGoSize && goInfo.bounds.height2D() <= NormalGoSize)
                {
                    goInfo.GoType = SceneGoType.Normal;
                }
                else
                {
                    goInfo.GoType = SceneGoType.Big;
                }
            }
        }
    }

    private float GetSceneGoLoadPriority(SceneGoInfo goInfo)
    {
        switch (goInfo.GoType)
        {
            case SceneGoType.Small:
                {
                    return AssetLoadPriority.SceneGoSmall;
                }
            case SceneGoType.Normal:
                {
                    return AssetLoadPriority.SceneGoNormal;
                }
            case SceneGoType.Big:
                {
                    return AssetLoadPriority.SceneGoBig;
                }
        }

        return AssetLoadPriority.StreamScene;
    }
    #endregion

    #region Command
    class WaitLoadingGoFinishCmd : BaseCommand
    {
        private readonly SceneGoManager sceneGoManager;
        public WaitLoadingGoFinishCmd(SceneGoManager sceneGoManager)
        {
            this.sceneGoManager = sceneGoManager;
        }
        public override void Execute()
        {
            base.Execute();
            if (sceneGoManager.IsAllLoadFinish())
            {
                this.OnFinish();
                return;
            }
            sceneGoManager.onLoadSceneGoFinish = this.OnLoadFinish;
        }

        void OnLoadFinish()
        {
            this.OnFinish();
            sceneGoManager.onLoadSceneGoFinish = null;
        }
    }

    class ClearAllSceneGoCmd : BaseCommand
    {
        private readonly SceneGoManager sceneGoManager;

        public ClearAllSceneGoCmd(SceneGoManager sceneGoManager)
        {
            this.sceneGoManager = sceneGoManager;
        }
        public override void Execute()
        {
            base.Execute();
            for (int i = 0; i < sceneGoManager.sceneGoArray.Length; i++)
            {
                Transform sceneGoTrans = sceneGoManager.sceneGoArray[i];
                if (sceneGoTrans != null)
                {
                    ResourcePoolManager.Instance.DespawnStreamScene(sceneGoTrans);
                    sceneGoManager.sceneGoArray[i] = null;
                }
            }
            if (sceneGoManager.coroutine != null)
            {
                JSTimer.Instance.StopCoroutine(sceneGoManager.coroutine);
            }
            this.OnFinish();
        }
    }
    #endregion

    enum VisibleState
    {
        Unvisible,
        Visible,
    }

    public int GetLayer()
    {
        throw new System.NotImplementedException();
    }

    public void SetLayer(int paramLayerMask)
    {
        throw new System.NotImplementedException();
    }
}
