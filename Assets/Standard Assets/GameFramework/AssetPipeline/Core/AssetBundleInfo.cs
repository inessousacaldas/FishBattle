using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetPipeline
{
    /// <summary>
    /// 对游戏运行时已加载的Bundle实体进行封装
    /// </summary>
    public class AssetBundleInfo
    {
        public readonly string bundleName;
        public AssetBundle assetBundle;
        public Object onlyAsset { get; private set; }
        public Object[] assetList { get; private set; }
        private HashSet<string> _refBundleNames;
        private HashSet<string> _poolRef;
        public HashSet<string> refBundleNames
        {
            get { return _refBundleNames ?? (_refBundleNames = new HashSet<string>()); }
        }

        public HashSet<string> poolRef
        {
            get { return _poolRef ?? (_poolRef = new HashSet<string>()); }
        }

        public AssetBundleInfo(string bundleName, AssetBundle ab)
        {
            this.bundleName = bundleName;
            assetBundle = ab;
        }

        public bool AddRef(string refBundleName)
        {
            return refBundleNames.Add(refBundleName);
        }

        public bool RemoveRef(string refBundleName)
        {
            return refBundleNames.Remove(refBundleName);
        }

        public bool AddPoolRef(string refPoolName)
        {
            return poolRef.Add(refPoolName);
        }

        public bool RemovePoolRef(string refPoolName)
        {
            return poolRef.Remove(refPoolName);
        }
        public bool Unload(bool unloadAll = false)
        {
            if (GetRefCount() > 0)
            {
#if GAMERES_LOG
                Debug.LogError("Can not unload RefBundleNames Count:" + refBundleNames.Count);
#endif
                return false;
            }

            //onlyAsset = null;
            //assetList = null;
            assetBundle.Unload(unloadAll);
            //assetBundle = null;
#if GAMERES_LOG
            Debug.LogError("AB Release:" + bundleName);
#endif
            return true;
        }

        public bool Contains(string assetName)
        {
            if (assetBundle == null)
                return false;

            return assetBundle.Contains(assetName);
        }

        public int GetRefCount()
        {
            return (_refBundleNames == null ? 0 : _refBundleNames.Count) 
                    + (_poolRef == null ? 0 : _poolRef.Count);
        }

        #region 加载Bundle资源接口
        public Object LoadAsset(string assetName, System.Type type)
        {
            if (assetBundle == null)
                return null;

            if (string.IsNullOrEmpty(assetName))
                return null;

            return assetBundle.LoadAsset(assetName, type);
        }

        public AssetBundleRequest LoadAssetAsync(string assetName, System.Type type)
        {
            if (assetBundle == null)
                return null;

            if (string.IsNullOrEmpty(assetName))
                return null;

            return assetBundle.LoadAssetAsync(assetName, type);
        }

        public Object[] LoadAllAsset()
        {
            if (assetBundle == null)
                return null;

            return assetBundle.LoadAllAssets();
        }

        /// <summary>
        /// 异步加载Bundle内的所有资源,并缓存下来
        /// </summary>
        /// <returns></returns>
        internal IEnumerator CacheAllAssetAsync()
        {
            if (assetBundle == null)
                yield break;

            var request = assetBundle.LoadAllAssetsAsync();
            if (request != null)
            {
                yield return request;
                var allAssets = request.allAssets;
                if (allAssets != null && allAssets.Length > 0)
                {
                    if (allAssets.Length > 1)
                    {
                        assetList = allAssets;
                    }
                    else
                    {
                        onlyAsset = allAssets[0];
                    }
                }
            }
        }

        public Object FindAsset(string assetName, System.Type type)
        {
            if (assetList == null) return null;

            for (int i = 0; i < assetList.Length; i++)
            {
                var asset = assetList[i];
                if (asset.name == assetName)
                {
                    //没有指定资源类型,只要名字相同就返回,否则需要判断资源类型
                    if (type == typeof(UnityEngine.Object))
                        return asset;

                    var assetType = asset.GetType();
                    if (assetType == type) return asset;
                }
            }

            return null;
        }
        #endregion
    }
}
