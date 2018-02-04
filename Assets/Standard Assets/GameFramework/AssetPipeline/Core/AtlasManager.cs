using System.Collections.Generic;
using AssetPipeline;
using UnityEngine;

public class AtlasManager
{
    private readonly AssetManager master;
    public ReadOnlyDictionary<int, AtlasInfo> readOnlyAtlasInfos;
    private Dictionary<int, AtlasInfo> atlasInfoDic;
    private List<AtlasRemoveInfo> removeList;
    //private const string ClearDestroyRefTimer = "ClearDestroyRefTimer";
    private const string UnloadAtlasUpdateTimer = "UnloadAtlasUpdateTimer";
    public AtlasManager(AssetManager master)
    {
        this.master = master;
        atlasInfoDic = new Dictionary<int, AtlasInfo>();
        readOnlyAtlasInfos = new ReadOnlyDictionary<int, AtlasInfo>(atlasInfoDic);
        removeList = new List<AtlasRemoveInfo>();
        //CSTimer.Instance.SetupTimer(ClearDestroyRefTimer, ClearDestroyRef, 5f);
        CSTimer.Instance.SetupTimer(UnloadAtlasUpdateTimer, UnloadAtlasUpdate, 0.5f);

    }

    public void AddAtlas(AssetBundleInfo assetBundleInfo)
    {
        bool preLoad = false;
        ResInfo resInfo = master.CurResConfig.GetResInfo(assetBundleInfo.bundleName);
        if (resInfo != null)
            preLoad = resInfo.preload;
        GameObject[] atlases = assetBundleInfo.assetBundle.LoadAllAssets<GameObject>();
        //打包机制保证不会同一个AB包出现2张图集，这里为了不用Load(assetName)如此处理
        for (int i = 0; i < atlases.Length; i++)
        {
            UIAtlas item = atlases[i].GetComponent<UIAtlas>();
            if (item != null)
            {
                atlasInfoDic.Add(item.GetInstanceID(), new AtlasInfo(item, assetBundleInfo, this, preLoad));
                //Debug.LogError(string.Format("LoadAtlas: {0}", item.name));

            }
        }
    }

    public void AddAtlasRef(UIAtlas uiAtlas, MonoBehaviour monoBehaviour)
    {
        if (uiAtlas == null)
            return;
        AtlasInfo atlasInfo;
        if (atlasInfoDic.TryGetValue(uiAtlas.GetInstanceID(), out atlasInfo))
        {
            atlasInfo.AddRef(monoBehaviour);
        }
    }

    public void RemoveAtlasRef(UIAtlas uiAtlas, MonoBehaviour monoBehaviour)
    {
        if (uiAtlas == null)
            return;
        AtlasInfo atlasInfo;
        if (atlasInfoDic.TryGetValue(uiAtlas.GetInstanceID(), out atlasInfo))
        {
            atlasInfo.RemoveRef(monoBehaviour);
            if (atlasInfo.CheckCanUnload())
            {
                foreach (AtlasRemoveInfo item in removeList)
                {
                    if (item.atlasInfo == atlasInfo)
                        return;
                }
                removeList.Add(new AtlasRemoveInfo(atlasInfo, Time.realtimeSinceStartup));
            }
        }
    }

    private void UnloadAtlasUpdate()
    {
        const float removeDelay = 1f;
        float curTime = Time.realtimeSinceStartup;
        while (removeList.Count != 0)
        {
            AtlasRemoveInfo info = removeList[0];
            if (curTime - info.time > removeDelay)
            {
                removeList.RemoveAt(0);
                if (info.atlasInfo.CheckCanUnload())
                {
                    //Debug.LogError(string.Format("RemoveAtlas: {0}", info.atlasInfo.uiAtlas.name));
                    info.atlasInfo.Unload();
                }
            }
            else
            {
                break;
            }
        }
    }

    private void ClearDestroyRef()
    {
        foreach (var item in atlasInfoDic)
        {
            AtlasInfo atlasInfo = item.Value;
            atlasInfo.ClearDestroyRef();
            if(atlasInfo.CheckCanUnload())
                removeList.Add(new AtlasRemoveInfo(atlasInfo, Time.realtimeSinceStartup));
        }
    }

    private void UnloadAtlasInfo(AtlasInfo info)
    {
        atlasInfoDic.Remove(info.uiAtlas.GetInstanceID());
        master.UnloadBundle(info.abInfo.bundleName, true);
    }

    public void Dispose()
    {
        //CSTimer.Instance.CancelTimer(ClearDestroyRefTimer);
        CSTimer.Instance.CancelTimer(UnloadAtlasUpdateTimer);
    }
    public class AtlasInfo
    {
        public readonly bool preLoad;
        public readonly UIAtlas uiAtlas;
        public readonly AssetBundleInfo abInfo;
        private readonly AtlasManager master;
        private HashSet<MonoBehaviour> refList;
        public AtlasInfo(UIAtlas uiAtlas, AssetBundleInfo abInfo, AtlasManager atlasManager, bool preLoad)
        {
            this.uiAtlas = uiAtlas;
            this.abInfo = abInfo;
            this.master = atlasManager;
            this.preLoad = preLoad;
            refList = new HashSet<MonoBehaviour>();
        }

        public void AddRef(MonoBehaviour monoBehaviour)
        {
            refList.Add(monoBehaviour);
        }

        public void RemoveRef(MonoBehaviour monoBehaviour)
        {
            refList.Remove(monoBehaviour);
        }

        public bool CheckCanUnload()
        {
            return !preLoad && refList.Count == 0 && abInfo.GetRefCount() == 0;
        }

        public void Unload()
        {
            master.UnloadAtlasInfo(this);
        }

        public void ClearDestroyRef()
        {
            if(refList.Count == 0)
                return;
            List<MonoBehaviour> list = new List<MonoBehaviour>();
            foreach (var item in refList)
            {
                if (item == null)
                {
                    list.Add(item);
                }
            }
            foreach (var item in list)
            {
                refList.Remove(item);
            }
        }
    }

    public class AtlasRemoveInfo
    {
        public readonly AtlasInfo atlasInfo;
        public float time;

        public AtlasRemoveInfo(AtlasInfo atlasInfo, float time)
        {
            this.atlasInfo = atlasInfo;
            this.time = time;
        }
    }
}

