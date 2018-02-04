using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class PrefabSpriteManager
{
    public class SpriteData
    {
        //UI prefab路径
        public string uiPath;
        private string _prefabName;
        public string prefabName
        {
            set
            {
                _prefabName = value;
            }
            get
            {
                if (_prefabName != null)
                    return _prefabName;
                if (string.IsNullOrEmpty(uiPath))
                {
                    _prefabName = "";
                    return _prefabName;
                }
                int index1 = uiPath.LastIndexOf("/");
                int index2 = uiPath.LastIndexOf(".");
                _prefabName = uiPath.Substring(index1 + 1, index2 - index1 - 1);
                return _prefabName;
            }
        }
        //sprite 在UI prefab中的路径
        public string spriteUIPath;

        public string atlasPath;
        public string spriteName;
        private string _spriteFullName;
        public string spriteFullName
        {
            get
            {
                if (string.IsNullOrEmpty(atlasPath))
                {
                    return string.Empty;
                }

                if (_spriteFullName != null)
                    return _spriteFullName;
                if(atlasPath.LastIndexOf("/") <= 0)
                {
                    Debug.Log(prefabName + "    "+atlasPath);
                }
                string path = atlasPath.Substring(0, atlasPath.LastIndexOf("/"));
                _spriteFullName = path + "/imgs/" + spriteName + ".png";

                return _spriteFullName;
            }
        }

        private string _atlasName;
        public string atlasName
        {
            set
            {
                _atlasName = value;
            }
            get
            {
                if (_atlasName != null)
                    return _atlasName;
                if (string.IsNullOrEmpty(atlasPath))
                {
                    _atlasName = "";
                    return _atlasName;
                }
                int index1 = atlasPath.LastIndexOf("/");
                int index2 = atlasPath.LastIndexOf(".");
                _atlasName = atlasPath.Substring(index1 + 1, index2 - index1 - 1);
                return _atlasName;
            }
        }

        public SpriteData()
        {
        }
    }

    public class PrefabData
    {
        //public string prefabName;
        public string prefabPath;//UI的路径
        public List<SpriteData> spriteList;//prefab所有Sprite组件
        public string lastTime;//文件最后修改时间戳

        private string _prefabName;
        public string prefabName
        {
            get
            {
                if (_prefabName != null)
                    return _prefabName;
                int index1 = prefabPath.LastIndexOf("/");
                int index2 = prefabPath.LastIndexOf(".");
                _prefabName = prefabPath.Substring(index1 + 1, index2 - index1 - 1);
                return _prefabName;
            }
        }

        public PrefabData(string prefabPath)
        {
            lastTime = "0";
            this.prefabPath = prefabPath;
            spriteList = new List<SpriteData>();
        }

        public void AddItem(SpriteData data)
        {
            spriteList.Add(data);
        }

        public void ClearItem()
        {
            spriteList.Clear();
        }
    }

    //UI对应图集
    private static Dictionary<string, PrefabData> _prefabDct;

    //保持图片的所有引用,空间换时间
    private static Dictionary<string, List<SpriteData>> _pngName2Refs;
    private const string PREFAB_PATH = "Assets/UI/Prefabs";
    public static void InitAndCheck()
    {
        if (_prefabDct == null)
            _prefabDct = new Dictionary<string, PrefabData>();

        if (CheckUpdate(PREFAB_PATH))
            UpdatePngRefs();
    }
    public static IEnumerable<PrefabData> GetPrefabList()
    {
        return _prefabDct.Values;
    }

    /*
    private static string _cachePath;
    public static string CachePath
    {
        get
        {
            if (_cachePath != null)
                return _cachePath;

            string path = Application.dataPath;
            _cachePath = path.Substring(0, path.LastIndexOf("/")) + "/PrefabSpriteInfo.txt";
            return _cachePath;
        }
        
    }
    */

    public static bool CheckUpdate(string root)
    {
        bool bChange = false;
        string[] strs = Directory.GetFiles(root, "*.prefab", SearchOption.AllDirectories);

        _prefabDct.Clear();
        foreach (string name in strs)
        {
            string filename = name.Replace("\\", "/");
            PrefabData prefabData;
            if (_prefabDct.ContainsKey(filename))
                prefabData = _prefabDct[filename];
            else
            {
                prefabData = new PrefabData(filename);
                _prefabDct.Add(filename, prefabData);
            }

            FileInfo file = new FileInfo(filename);
            System.TimeSpan ts = file.LastWriteTime - new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            string lastTime = System.Convert.ToInt64(ts.TotalSeconds).ToString();
            
            //图集增加和删除可能会导致prefab.UISprite 的atlas属性变化
            //所以这里每次都刷新了
            //时间戳一样，忽略检测
            //if (prefabData.lastTime == lastTime)
            //    continue;

            prefabData.lastTime = lastTime;

            prefabData.ClearItem();
            GameObject go = AssetDatabase.LoadMainAssetAtPath(filename) as GameObject;
            if (go == null) continue;

            var spriteList = GetAllSprite(go);
            foreach (UISprite sprite in spriteList)
            {
                SpriteData data = new SpriteData();
                data.uiPath = filename;
                data.spriteName = sprite.spriteName;
                data.prefabName = go.name;
                if(sprite.atlas != null)
                {
                    data.atlasName = sprite.atlas.name;
                    data.atlasPath = AssetDatabase.GetAssetPath(sprite.atlas);
                    data.atlasPath = data.atlasPath.Replace("\\", "/");
                }
                data.spriteUIPath = GameObjectExt.GetHierarchyWithRoot(sprite.transform, go.transform);
                prefabData.AddItem(data);
            }

            bChange = true;
        }
        return bChange;
    }
    
    public static Dictionary<string, List<SpriteData>> pngName2Refs
    {
        get
        {
            return _pngName2Refs;
        }
    }

    public static void UpdatePngRefs()
    {

        if(_pngName2Refs == null)
            _pngName2Refs = new Dictionary<string, List<SpriteData>>();

        _pngName2Refs.Clear();

        foreach(PrefabData prefabData in _prefabDct.Values)
        {
            foreach(SpriteData data in prefabData.spriteList)
            {
                if (string.IsNullOrEmpty(data.spriteFullName))
                    continue;
                if (!_pngName2Refs.ContainsKey(data.spriteFullName))
                    _pngName2Refs.Add(data.spriteFullName, new List<SpriteData>());
                _pngName2Refs[data.spriteFullName].Add(data);
            }
        }
    }

    public static List<UISprite> GetAllSprite(GameObject go)
    {
        List<UISprite> spriteList = new List<UISprite>();
        UISprite[] sprites = go.GetComponents<UISprite>();
        spriteList.AddRange(sprites);
        sprites = go.GetComponentsInChildren<UISprite>(true);
        spriteList.AddRange(sprites);

        return spriteList;
    }
}
