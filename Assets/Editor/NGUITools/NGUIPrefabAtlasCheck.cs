using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


public class NGUIPrefabAtlasCheck : EditorWindow
{
    public static NGUIPrefabAtlasCheck instance = null;
    [MenuItem("Custom/NGUIPrefabAtlas/AtlasCheck")]
    public static void ShowWindow()
    {
        if (NGUIPrefabAtlasCheck.instance == null)
        {
            var win = EditorWindow.GetWindow<NGUIPrefabAtlasCheck>(false, "AltasCheck", true);
            win.minSize = new Vector2(400f, 700f);
            win.Show();
            NGUIPrefabAtlasCheck.instance = win;
        }
        else
        {
            NGUIPrefabAtlasCheck.instance.Close();
        }
    }

    private class HitData
    {
        public string prefabName;
        public string prefabPath;
        public List<string> hitPathList;

        public HitData(GameObject prefab)
        {
            prefabName = prefab.name;
            prefabPath = AssetDatabase.GetAssetPath(prefab);
            hitPathList = new List<string>();
        }

        public HitData(string prefabName, string prefabPath)
        {
            this.prefabName = prefabName;
            this.prefabPath = prefabPath;
            hitPathList = new List<string>();
        }

        public void AddHit(string path)
        {
            hitPathList.Add(path);
        }

        public int hitCount
        {
            get { return hitPathList.Count; }
        }
    }

    private class HitManager
    {
        private static Dictionary<string, HitManager> _managerDict = new Dictionary<string, HitManager>();
        public static HitManager GetManager(string spriteName)
        {
            if (!_managerDict.ContainsKey(spriteName))
            {
                _managerDict.Add(spriteName, new HitManager(spriteName));
            }
            return _managerDict[spriteName];
        }

        public static List<HitManager> GetAllManagers()
        {
            List<HitManager> list = new List<HitManager>();
            foreach (var obj in _managerDict.Values)
            {
                list.Add(obj as HitManager);
            }
            return list;
        }

        public static void Clear()
        {
            _managerDict.Clear();
        }

        public string spriteName;
        private List<HitData> _hitDataList;
        public List<HitData> hitDataList { get { return _hitDataList; } }
        private HitData _curHitData;
        public int hitTotal;
        public HitManager(string spriteName)
        {
            hitTotal = 0;
            this.spriteName = spriteName;
            _hitDataList = new List<HitData>();
        }

        public void SetEditPrefab(GameObject prefab)
        {
            if (_curHitData != null && _curHitData.prefabName == prefab.name) return;

            HitData hitData = null;
            foreach (HitData data in _hitDataList)
            {
                if (prefab.name == data.prefabName)
                {
                    hitData = data;
                    break;
                }
            }
            if (hitData == null)
            {
                hitData = new HitData(prefab);
                _hitDataList.Add(hitData);
            }
            _curHitData = hitData;
        }

        public void SetEditPrefab(string prefabName, string prefabPath)
        {
            if (_curHitData != null && _curHitData.prefabName == prefabName) return;

            HitData hitData = null;
            foreach (HitData data in _hitDataList)
            {
                if (prefabName == data.prefabName)
                {
                    hitData = data;
                    break;
                }
            }
            if (hitData == null)
            {
                hitData = new HitData(prefabName, prefabPath);
                _hitDataList.Add(hitData);
            }
            _curHitData = hitData;
        }

        public void AddHit(string path)
        {
            if (_curHitData == null) return;
            _curHitData.AddHit(path);

            ++hitTotal;
        }

        public override string ToString()
        {
            string sResult = "";
            int count = 0;
            foreach (HitData data in _hitDataList)
            {
                sResult += data.prefabName + "\n";
                foreach (string path in data.hitPathList)
                {
                    sResult += path + "\n";
                    ++count;
                }
            }
            return string.Format("{0} Hit {1}: \n{2}", this.spriteName, count, sResult);
        }
    }

    private UIAtlas _targetAtlas;
    private Vector2 _resultScrollPos;
    private Vector2 _notRefScrollPos;
    private Vector2 _prefabDetailPos;
    private string _prefabFolder = "Assets/UI/Prefabs";
    private Dictionary<string, bool> _spriteFolderExpand;
    private HitData _selectHit;
    private string _selectSpriteName;
    private string _searchSprite;
    private bool _alreadyCheck = false;

    private List<string> _ignoreList;
    private List<string> _ignoreSuffixs = new List<string>() { "_selected", "-selected", "_On", "_Off", "-On", "-Off" };


    public void OnEnable()
    {
        _spriteFolderExpand = new Dictionary<string, bool>();
        _ignoreList = new List<string>();
    }

    public void OnDisable()
    {
        Clear();
    }

    public void Clear()
    {
        _targetAtlas = null;
        _spriteFolderExpand.Clear();
        _ignoreList.Clear();

        _selectHit = null;
        _selectSpriteName = "";
        _alreadyCheck = false;
    }

    private int _editType;
    private void OnGUI()
    {

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("检查atlas在Prefabs引用", GUILayout.Height(50f)))
        {
            _editType = 0;
            Clear();
        }

        if (GUILayout.Button("检查重复的图片", GUILayout.Height(50f)))
        {
            _editType = 1;
            Clear();
        }
        EditorGUILayout.EndHorizontal();

        if (_editType == 0)
        {
            OnAtlasRefGUI();
        }
        else if (_editType == 1)
        {
            OnSpriteCheckGUI();
        }
    }

    /// <summary>
    /// 图集引用检测
    /// </summary>
    private void OnAtlasRefGUI()
    {
        //EditorGUILayout.Space();
        string curFolder = EditorGUILayout.TextField("prefab根目录:", _prefabFolder);
        var curAtlas = (UIAtlas)EditorGUILayout.ObjectField("UIAtlas:", _targetAtlas, typeof(UIAtlas), false);
        if (curAtlas == null || _targetAtlas != curAtlas || curFolder != _prefabFolder)
        {
            NGUIPrefabAtlasCheck.HitManager.Clear();
            _alreadyCheck = false;
        }

        _targetAtlas = curAtlas;
        _prefabFolder = curFolder;

        if (GUILayout.Button("Check", "LargeButton", GUILayout.Height(50f)))
        {
            OnCheckAtlas();
            OnCheckIgnore();
            _alreadyCheck = true;
        }

        List<HitManager> hitList = HitManager.GetAllManagers();
        List<HitManager> notHitList = new List<HitManager>();
        if (hitList.Count > 0)
        {
            GUILayout.BeginHorizontal();
            string after = EditorGUILayout.TextField("", _searchSprite, "SearchTextField");
            if (_searchSprite != after)
            {
                _searchSprite = after;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);

            _resultScrollPos = EditorGUILayout.BeginScrollView(_resultScrollPos, GUILayout.MinHeight(300f));
            foreach (var manager in hitList)
            {
                if (_searchSprite != null && (!manager.spriteName.Contains(_searchSprite))) continue;

                if (!_spriteFolderExpand.ContainsKey(manager.spriteName)) _spriteFolderExpand.Add(manager.spriteName, false);

                if (manager.hitTotal <= 0) GUI.color = Color.red;
                string sFolder = string.Format("{0} Count:{1}", manager.spriteName, manager.hitTotal);
                _spriteFolderExpand[manager.spriteName] = EditorGUILayout.Foldout(_spriteFolderExpand[manager.spriteName], sFolder);
                GUI.color = Color.white;

                if (manager.hitTotal <= 0)
                {
                    notHitList.Add(manager);
                    continue;
                }

                if (_spriteFolderExpand[manager.spriteName])
                {
                    for (int i = 0; i < manager.hitDataList.Count; i++)
                    {
                        HitData data = manager.hitDataList[i];
                        GUILayout.Space(-1f);
                        GUI.backgroundColor = data == _selectHit ? Color.white : new Color(0.8f, 0.8f, 0.8f);
                        GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
                        GUI.backgroundColor = Color.white;
                        GUILayout.Space(15f);
                        GUILayout.Label(i.ToString(), GUILayout.Width(40f));
                        string sText = string.Format("{0}.prefab count:{1}", data.prefabName, data.hitCount);
                        if (GUILayout.Button(sText, "OL TextField", GUILayout.Height(20f)))
                        {
                            _selectHit = data;
                            _selectSpriteName = manager.spriteName;
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndScrollView();
            string sResult = string.Format("Search Result: {0}/{1}(未引用/全部)", notHitList.Count, hitList.Count);
            if (EditorHelper.DrawHeader(sResult, "result", false, false))
            {
                _notRefScrollPos = EditorGUILayout.BeginScrollView(_notRefScrollPos, GUILayout.MinHeight(180f));
                foreach (var data in notHitList)
                {
                    GUI.backgroundColor = _ignoreList.Contains(data.spriteName) ? Color.red : Color.white;
                    GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
                    GUI.backgroundColor = Color.white;
                    GUILayout.Space(15f);
                    GUILayout.Label(data.spriteName);
                    if (!_ignoreList.Contains(data.spriteName))
                    {
                        if (GUILayout.Button("忽略", GUILayout.Width(60f)))
                        {
                            if (!_ignoreList.Contains(data.spriteName)) _ignoreList.Add(data.spriteName);
                        }
                    }
                    else
                    {
                        GUI.backgroundColor = Color.green;
                        if (GUILayout.Button("取消忽略", GUILayout.Width(85f)))
                        {
                            if (_ignoreList.Contains(data.spriteName)) _ignoreList.Remove(data.spriteName);
                        }
                    }
                    GUI.backgroundColor = Color.white;
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }


            if (GUILayout.Button("一键移除未被引用的sprite", "LargeButton", GUILayout.Height(50f)))
            {
                if (notHitList.Count > 0)
                {
                    if (EditorUtility.DisplayDialog("确认", "将会删除imgs文件夹下相应的png，确认后请等待数秒", "继续", "取消"))
                    {
                        string atlasPath = AssetDatabase.GetAssetPath(_targetAtlas);
                        string folderPath = atlasPath.Replace("/" + _targetAtlas.name + ".prefab", "");

                        int total = 0;
                        int remove = 0;
                        foreach (var manager in notHitList)
                        {
                            ++total;
                            if (_ignoreList.Contains(manager.spriteName)) continue;
                            string src = folderPath + "/imgs/" + manager.spriteName + ".png";
                            if (AssetDatabase.MoveAssetToTrash(src)) ++remove;
                        }
                        EditorUtility.DisplayDialog("移除完毕", string.Format("成功移除{0}/{1}。请自行生成新的图集", remove, total), "确定");
                        DoTexturePacker(folderPath, _targetAtlas.name);
                    }

                }
            }

            EditorGUILayout.LabelField("注意:工具只检测prefab是否引用资源，代码引用判断未处理，请人脑判断");
            EditorGUILayout.LabelField("一键移除，在图集目录新建了一个backups文件夹，并把未引用的png资源文件移动进去");
            EditorGUILayout.LabelField("一键移除后，自行使用TexturePacker 工具重新生成");

            if (_selectHit != null)
            {
                string detail = string.Format("{0}图片 {1}.prefab detail:", _selectSpriteName, _selectHit.prefabName);
                if (EditorHelper.DrawHeader(detail, "detail", false, false))
                {
                    _prefabDetailPos = EditorGUILayout.BeginScrollView(_prefabDetailPos, GUILayout.MinHeight(100f));
                    foreach (string path in _selectHit.hitPathList)
                    {
                        GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
                        GUILayout.Space(25f);
                        GUILayout.Label(path);
                        GUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
            else EditorHelper.DrawHeader("none select prefab", "detail", false, false);
        }
        else if (_alreadyCheck)//未生成过manager，表示atlas未被引用过
        {
            if (GUILayout.Button("图集未被引用，点击移除", "LargeButton", GUILayout.Height(50f)))
            {
                if (EditorUtility.DisplayDialog("确认", "此操作会移除图集目录，确认继续？", "确认", "取消"))
                {
                    string atlasPath = AssetDatabase.GetAssetPath(_targetAtlas);
                    string folderPath = atlasPath.Replace("/" + _targetAtlas.name + ".prefab", "");
                    if (AssetDatabase.MoveAssetToTrash(folderPath))
                    {
                        EditorUtility.DisplayDialog("", "移除图集目录成功", "确认");
                    }
                }
            }
        }
    }

    private void DoTexturePacker(string folder, string name)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        System.Diagnostics.Process.Start("TexturePacker.exe", folder + "/" + name + ".tps");

        /*
        if (EditorUtility.DisplayDialog("", string.Format("自动更新{0}.prefab", prefix), "确认", "取消"))
        {
            AssetDatabase.Refresh();
        }
        */
        //if(EditorUtility.DisplayDialog("", "将替你自动更新XXXAtlas.prefab", "确认", "取消"))
        //{
        //    TextAsset asset = AssetDatabase.LoadMainAssetAtPath(prefix + ".txt") as TextAsset;
        //    NGUIJson.LoadSpriteData(_targetAtlas, asset);
        //    EditorUtility.SetDirty(_targetAtlas);
        //    AssetDatabase.SaveAssets();
        //    Clear();
        //}
#endif
    }

    private void OnCheckAtlas()
    {
        _selectHit = null;
        _selectSpriteName = "";
        HitManager.Clear();

        List<Object> prefabList = GetPrefabs();
        foreach (Object obj in prefabList)
        {
            GameObject go = obj as GameObject;
            List<UISprite> sprites = PrefabSpriteManager.GetAllSprite(go);
            foreach (UISprite sprite in sprites)
            {
                if (sprite.atlas != _targetAtlas) continue;
                foreach (UISpriteData data in _targetAtlas.spriteList)
                {
                    HitManager managr = HitManager.GetManager(data.name);
                    if (sprite.spriteName != data.name)
                    {
                        continue;
                    }
                    string path = GameObjectExt.GetHierarchyWithRoot(sprite.transform, go.transform);
                    managr.SetEditPrefab(go);
                    managr.AddHit(path);
                }
            }
        }
    }

    private void OnCheckIgnore()
    {
        _ignoreList.Clear();
        if (HitManager.GetAllManagers().Count <= 0) return;

        foreach (UISpriteData data in _targetAtlas.spriteList)
        {
            HitManager mgr = HitManager.GetManager(data.name);
            if (mgr != null && mgr.hitTotal > 0) continue;

            foreach (string suffix in _ignoreSuffixs)
            {
                if (data.name.EndsWith(suffix))
                {
                    string prefix = data.name.Replace(suffix, "");
                    foreach (HitManager manager in HitManager.GetAllManagers())
                    {
                        if (manager.hitTotal > 0 &&
                            manager.spriteName != data.name && manager.spriteName.StartsWith(prefix))
                        {
                            _ignoreList.Add(data.name);
                        }
                    }
                }
            }

        }
    }

    private List<Object> GetPrefabs()
    {
        string[] GUIDs = AssetDatabase.FindAssets("t:prefab", new string[] { _prefabFolder });
        List<Object> objList = new List<Object>(GUIDs.Length);
        foreach (string guid in GUIDs)
        {
            objList.Add(AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid)));
        }
        return objList;
    }

    #region 重复图集检测
    /// <summary>
    /// 只处理图集目录下的imgs目录，以其它名字命名的不处理，比如img
    /// 含有忽略的目录名，见_ignoreFolderList列表。
    /// 需自行处理的：
    /// 1、确定代码中的引用
    /// 2、UI.prefab的替换为优化depth，可能是draw call增加
    /// </summary>
    private string _ImgRoot = "Assets/UI/Atlas";
    private UIAtlas _editPrefab;
    private Dictionary<long, List<string>> _size2Filenames;
    private Dictionary<string, List<string>> _hash2Filenames;
    private Dictionary<string, string> _filename2AtlasPath;
    private Dictionary<string, List<string>> _filename2UIPaths;//保持关联到的UI prefab
    private Vector2 _sameHashScroll;

    //忽略CommonTextures目录
    private List<string> _ignoreFolderList = new List<string>() { "CommonTextures/", };

    private Dictionary<string, bool> _sameFolderExpand;
    private Dictionary<string, bool> _selResultExpand;
    private string _preSelectFolder;
    private UIAtlas _replaceAtlas;
    private string _replaceName;
    private string _selFilename;
    private Vector2 _pngResultPos;
    private Vector2 _ResultDetailPos;
    private List<string> _ignoreResetList;
    private List<string> _areadyDealList;//已处理的hash

    /// <summary>
    /// 重复图集检测
    /// </summary>
    private void OnSpriteCheckGUI()
    {
        GUILayout.BeginHorizontal();
        _ImgRoot = EditorGUILayout.TextField("根目录:", _ImgRoot, GUILayout.Width(300));
        if (GUILayout.Button("检测全部", GUILayout.Height(50f)))
        {
            OnCheckSameFile();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        _editPrefab = EditorGUILayout.ObjectField("图集：", _editPrefab, typeof(UIAtlas), false, GUILayout.Width(300)) as UIAtlas;
        if (GUILayout.Button("根据图集检测", GUILayout.Height(50f)))
        {
            OnCheckSameFile(_editPrefab);
        }
        GUILayout.EndHorizontal();

        if (_sameFolderExpand == null)
            _sameFolderExpand = new Dictionary<string, bool>();
        if (_selResultExpand == null)
            _selResultExpand = new Dictionary<string, bool>();
        if (_ignoreResetList == null)
            _ignoreResetList = new List<string>();
        if (_areadyDealList == null)
            _areadyDealList = new List<string>();

        if(_sameCount > 0)
        {
            EditorGUILayout.BeginVertical("HelpBox", GUILayout.Width(700f));
            {
                GUILayout.Space(10f);
                GUILayout.Label(string.Format("Result: 重复总条目数：{0}  UI引用总数：{1} 已处理条目数:{2}", _sameCount, _refCount, _areadyDealList.Count));
                GUILayout.Space(5f);
                _sameHashScroll = EditorGUILayout.BeginScrollView(_sameHashScroll, GUILayout.MinHeight(350f));
                foreach (string hash in _hash2Filenames.Keys)
                {
                    List<string> filenames = _hash2Filenames[hash];
                    if (filenames.Count <= 1) continue;

                    if (!_sameFolderExpand.ContainsKey(hash)) _sameFolderExpand.Add(hash, false);

                    bool bDeal = _areadyDealList.Contains(hash);

                    string name = string.Format("png数：{0} UI引用总数：{1}",
                        filenames.Count, GetRefUICount(filenames));
                    if (bDeal)
                        name += "  已处理";

                    GUI.backgroundColor = bDeal ? Color.green : Color.white;
                    GUI.color = bDeal ? Color.green : Color.white;
                    if (EditorGUILayout.Foldout(_sameFolderExpand[hash], name))
                    {
                        //收起
                        if (_preSelectFolder != null && hash != _preSelectFolder
                            && _sameFolderExpand.ContainsKey(_preSelectFolder))
                        {
                            _sameFolderExpand[_preSelectFolder] = false;
                            _replaceAtlas = null;
                            _replaceName = null;
                            _selFilename = "";
                        }
                        _preSelectFolder = hash;
                        _sameFolderExpand[hash] = true;
                    }
                    else
                    {
                        _sameFolderExpand[hash] = false;
                    }
                    GUI.backgroundColor = Color.white;
                    GUI.color = Color.white;

                    if (_sameFolderExpand[hash])
                    {
                        foreach (var png in filenames)
                        {
                            GUILayout.Space(-1f);
                            GUI.backgroundColor = !png.Equals(_selFilename) ? Color.white : new Color(0.8f, 0.8f, 0.8f);
                            GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
                            GUI.backgroundColor = Color.white;
                            string sText = png.Replace(_ImgRoot + "/", "") + " "
                                + string.Format("UI引用数：{0}", GetRefUICount(png));
                            //GUILayout.Label(sText);
                            if (GUILayout.Button(sText, "OL TextField", GUILayout.Height(20f)))
                            {
                                _selFilename = png;
                            }

                            if (GUILayout.Button("复制sprite", GUILayout.Width(85f)))
                            {
                                string spriteName = png.Substring(png.LastIndexOf("/") + 1,
                                    png.LastIndexOf(".") - png.LastIndexOf("/") - 1);
                                NGUITools.clipboard = spriteName;
                            }

                            GUI.backgroundColor = _ignoreResetList.Contains(png) ? Color.green : Color.white;
                            sText = _ignoreResetList.Contains(png) ? "取消忽略" : "忽略";
                            if (GUILayout.Button(sText, GUILayout.Width(85f)))
                            {
                                if (_ignoreResetList.Contains(png))
                                    _ignoreResetList.Remove(png);
                                else
                                    _ignoreResetList.Add(png);
                            }
                            GUI.backgroundColor = Color.white;

                            if (GUILayout.Button("设置为目标", GUILayout.Width(100f)))
                            {
                                Debug.Log("图集目录:" + _filename2AtlasPath[png]);
                                _replaceAtlas = AssetDatabase.LoadAssetAtPath(_filename2AtlasPath[png], typeof(UIAtlas)) as UIAtlas;
                                _replaceName = GetSpriteName(png);
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(15f);
                        _replaceAtlas = (UIAtlas)EditorGUILayout.ObjectField("目标图集:", _replaceAtlas, typeof(UIAtlas), false);
                        _replaceName = EditorGUILayout.TextField("spriteName:", _replaceName);
                        if (GUILayout.Button("确认转移", GUILayout.Width(100f)))
                        {
                            CommitResetSame(hash);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();

            if (!string.IsNullOrEmpty(_selFilename))
            {
                var manager = HitManager.GetManager(_selFilename);
                string sResult = string.Format("UI引用{0} {1}", manager.hitTotal, _selFilename);
                GUILayout.BeginVertical();
                if (EditorHelper.DrawHeader(sResult, "pngResult", false, false))
                {
                    _pngResultPos = EditorGUILayout.BeginScrollView(_pngResultPos, GUILayout.MinHeight(120f));
                    foreach (HitData data in manager.hitDataList)
                    {
                        string keyName = data.prefabName + _selFilename;
                        if (!_selResultExpand.ContainsKey(keyName))
                            _selResultExpand.Add(keyName, false);

                        string sText = data.prefabName + string.Format("   引用数:{0}", data.hitCount);
                        if (EditorGUILayout.Foldout(_selResultExpand[keyName], sText))
                        {
                            GUILayout.BeginVertical();
                            foreach (string path in data.hitPathList)
                            {
                                GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
                                GUILayout.Space(30f);
                                GUILayout.Label(path);
                                GUILayout.EndHorizontal();
                            }
                            _selResultExpand[keyName] = true;
                            GUILayout.EndVertical();
                        }
                        else
                        {
                            _selResultExpand[keyName] = false;
                        }

                    }
                    EditorGUILayout.EndScrollView();
                }

                Texture img = AssetDatabase.LoadAssetAtPath(_selFilename, typeof(Texture)) as Texture;
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.Space(60);
                GUILayout.Box(img);
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
        }
        
        GUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("只处理图集目录下的imgs目录，以其它名字命名的不处理，比如img");
        EditorGUILayout.LabelField("含有忽略的目录名，见_ignoreFolderList列表。");
        EditorGUILayout.LabelField("需自行处理的：");
        EditorGUILayout.LabelField("1、确定代码中的引用");
        EditorGUILayout.LabelField("2、UI.prefab的替换为优化depth，可能是draw call增加");
        GUI.color = Color.red;
        EditorGUILayout.LabelField("3、九宫切图");
        GUI.color = Color.white;
        GUILayout.EndVertical();
    }

    /// <summary>
    /// 确认转移
    /// </summary>
    /// <param name="hash">要处理文件的hash值</param>
    private void CommitResetSame(string hash)
    {
        if (_areadyDealList.Contains(hash))
        {
            EditorUtility.DisplayDialog("确认", "此条目已处理", "确认");
            return;
        }
        if(_replaceAtlas == null || string.IsNullOrEmpty(_replaceName))
        {
            EditorUtility.DisplayDialog("错误", "请设置好目标图集", "确认");
            return;
        }

        //需要执行的图集
        List<string> compileList = new List<string>();
        //处理移入一个全新图集的情况
        string atlasPath = AssetDatabase.GetAssetPath(_replaceAtlas);
        bool inEditAtlas = false;
        foreach(string filename in _hash2Filenames[hash])
        {
            string path = _filename2AtlasPath[filename];
            if (path.Equals(atlasPath))
            {
                inEditAtlas = true;
                break;
            }
        }
        if (!inEditAtlas)
        {
            string srcPng = _hash2Filenames[hash][0];
            string srcPath = _filename2AtlasPath[srcPng];
            string srcFolder = srcPath.Substring(0, srcPath.LastIndexOf("/"));

            string destPath = atlasPath;
            string destFolder = atlasPath.Substring(0, atlasPath.LastIndexOf("/"));
            string destPng = destFolder + "/imgs/" + _replaceName + ".png";
            if (File.Exists(destPng))
            {
                EditorUtility.DisplayDialog("错误", "目标图集有同名sprite", "确认");
                return;
            }
            AssetDatabase.MoveAsset(srcPng, destPng);
            compileList.Add(destPath);
            Debug.Log(string.Format("移动{0}到{1}", srcPng, destPng));
        }

        List<string> filenames = _hash2Filenames[hash];
        foreach (string filename in filenames)
        {
            //忽略
            if (_ignoreResetList.Contains(filename))
            {
                Debug.Log("忽略检测：" + filename);
                continue;
            }

            HitManager manager = HitManager.GetManager(filename);
            foreach (HitData data in manager.hitDataList)
            {
                GameObject go = AssetDatabase.LoadMainAssetAtPath(data.prefabPath) as GameObject;
                if (go == null)
                {
                    Debug.LogError("找不到prefab：" + data.prefabName);
                    continue;
                }
                foreach (string path in data.hitPathList)
                {
                    var com = go.transform.Find(path);
                    if (com == null)
                    {
                        Debug.LogError(string.Format("{0}找不到对应的SpriteGameObject：{1}，请检查",
                            go.name, path));
                        continue;
                    }
                    var sprite = com.GetComponent<UISprite>();
                    if (sprite == null)
                    {
                        Debug.LogError(string.Format("{0}找不到对应的Sprite：{1}，请检查",
                            go.name, path));
                        continue;
                    }
                    sprite.atlas = _replaceAtlas;
                    sprite.spriteName = _replaceName;
                    EditorUtility.SetDirty(go);
                    //要添加替换log
                    Debug.Log(string.Format("替换了{0}.prefab的{1}", go.name, path));
                }
            }
        }

        //矫正UI.prefab
        string png = "/imgs/" + _replaceName + ".png";
        foreach(string filename in _hash2Filenames[hash])
        {
            if (_ignoreResetList.Contains(filename))
            {
                Debug.Log("忽略移除png：" + filename);
                continue;
            }

            string path = _filename2AtlasPath[filename];
            if (path.Equals(atlasPath))
            {
                Debug.Log("本身不做移除操作" + filename);
                continue;
            }

            AssetDatabase.MoveAssetToTrash(filename);
            Debug.Log("移除png："+filename);

            if (!compileList.Contains(path))
            {
                compileList.Add(path);
            }
        }

        //执行tps
        foreach(string path in compileList)
        {
            int index = path.LastIndexOf("/");
            int index2 = path.LastIndexOf(".");
            string folder = path.Substring(0, index);
            string atlasName = path.Substring(index + 1, index2 - index - 1);
            DoTexturePacker(folder, atlasName);
            Debug.Log("执行tps："+ atlasName);
        }

        //转移完毕
        //正式移除检测
        Debug.Log("转移完毕，hash："+ hash);
        
        _areadyDealList.Add(hash);
        _replaceAtlas = null;
        _replaceName = null;
        _selFilename = "";

        EditorUtility.DisplayDialog("转移结束", "转移结束，记得在编辑器Alt+S保存", "确认");
    }

    private string GetSpriteName(string filename)
    {
        Debug.Log(string.Format("设置sprite名为:{0}", filename));
        int index1 = filename.LastIndexOf("/");
        int index2 = filename.LastIndexOf(".");
        return filename.Substring(index1 + 1, index2 - index1 - 1);
    }

    private string GetAtlasName(string filename)
    {
        string atlasPath = _filename2AtlasPath[filename];
        Match mat = Regex.Match(atlasPath, @"/(\S+)\.prefab");
        if (mat.Success)
        {
            return mat.Groups[1].Value;
        }
        return "";
    }


    private int GetRefUICount(List<string> filenames)
    {
        int count = 0;
        foreach(string name in filenames)
        {
            count += GetRefUICount(name);
        }
        return count;
    }

    private int GetRefUICount(string filename)
    {
        HitManager manager = HitManager.GetManager(filename);
        return manager.hitTotal;
        /*
        if (!_filename2UIPaths.ContainsKey(filename)) return 0;
        List<string> list = _filename2UIPaths[filename];
        return list.Count;
        */
    }

    private List<UIAtlas> GetAtlasPrefabs()
    {
        string[] GUIDs = AssetDatabase.FindAssets("t:prefab", new string[] { _ImgRoot });
        List<UIAtlas> objList = new List<UIAtlas>(GUIDs.Length);
        foreach (string guid in GUIDs)
        {
            UIAtlas atlas = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(UIAtlas)) as UIAtlas;
            if (atlas == null) continue;
            objList.Add(atlas);
        }
        return objList;
    }

    private void ClearSameCheck()
    {
        _size2Filenames.Clear();
        _hash2Filenames.Clear();
        _sameHashScroll = new Vector2(0f, 0f);
        _sameFolderExpand.Clear();
        _selResultExpand.Clear();
        _ignoreResetList.Clear();
    }
    
    private void InitSame()
    {
        if (_size2Filenames == null)
        {
            _size2Filenames = new Dictionary<long, List<string>>();
        }
        else
        {
            _size2Filenames.Clear();
        }
        if (_hash2Filenames == null)
        {
            _hash2Filenames = new Dictionary<string, List<string>>();
        }
        else
        {
            _hash2Filenames.Clear();
        }
        if (_filename2AtlasPath == null) _filename2AtlasPath = new Dictionary<string, string>();
        
        /*
        if (_filename2UIPaths == null)
        {
            _filename2UIPaths = new Dictionary<string, List<string>>();
        }
        else
        {
            _filename2UIPaths.Clear();
        }
        */
    }

    private int _sameCount;
    private int _refCount;

    /// <summary>
    /// 检测与给定图集重复的图
    /// </summary>
    /// <param name="atlas"></param>
    private void OnCheckSameFile(UIAtlas atlas)
    {
        if(atlas == null)
        {
            EditorUtility.DisplayDialog("错误", "请设置图集", "确定");
            return;
        }
        List<string> validList = new List<string>();
        foreach(UISpriteData data in atlas.spriteList)
        {
            string path = GetAtlasFolder(atlas);
            path = path.Replace("\\", "/");
            string filename = path + "/imgs/" + data.name + ".png";
            validList.Add(filename);
        }
        OnCheckSameFile(validList);
    }

    /// <summary>
    /// 检查全部
    /// </summary>
    private void OnCheckSameFile(List<string> validList=null)
    {
        InitSame();

        UpdateSizeHitDct(validList);

        foreach (List<string> list in _size2Filenames.Values)
        {
            if(list != null && list.Count > 1)
            {
                UpdateHashHitFiles(list, validList);
            }
        }
        HitManager.Clear();
        
        //初始化
        PrefabSpriteManager.InitAndCheck();
        foreach (List<string> list in _hash2Filenames.Values)
        {
            if (list == null || list.Count <= 1) continue;

            foreach (string filename in list)
            {
                string filePath = filename;
                if (!PrefabSpriteManager.pngName2Refs.ContainsKey(filePath))
                    continue;
                var spriteDataList = PrefabSpriteManager.pngName2Refs[filePath];
                HitManager managr = HitManager.GetManager(filePath);
                foreach(var spriteData in spriteDataList)
                {
                    managr.SetEditPrefab(spriteData.prefabName, spriteData.uiPath);
                    managr.AddHit(spriteData.spriteUIPath);
                }
            }
        }    

        _sameCount = 0;
        foreach(List<string> list in _hash2Filenames.Values)
        {
            if (list != null && list.Count > 1)
            {
                _sameCount++;
            }
        }

        Debug.Log(string.Format("重复数为：{0}", _sameCount));

        _refCount = 0;
        foreach (var manager in HitManager.GetAllManagers())
        {
            _refCount += manager.hitTotal;
        }
        Debug.Log(string.Format("UI引用数为：{0}", _refCount));
    }

    private string GetAtlasFolder(UIAtlas atlas)
    {
        string atlasPath = AssetDatabase.GetAssetPath(atlas);
        atlasPath = atlasPath.Replace("\\", "/");
        return atlasPath.Substring(0, atlasPath.LastIndexOf("/"));
    }

    /// <summary>
    /// 先筛选出大小一样的
    /// </summary>
    private void UpdateSizeHitDct(List<string> validList)
    {
        foreach(UIAtlas atlas in GetAtlasPrefabs())
        {
            foreach(UISpriteData spriteData in atlas.spriteList)
            {
                string atlasPath = AssetDatabase.GetAssetPath(atlas);
                atlasPath = atlasPath.Replace("\\", "/");
                //atlasPath = Application.dataPath + "/" + atlasPath.Replace("Assets/", "");
                string folderPath = atlasPath.Replace("/" + atlas.name + ".prefab", "");
                string filename = folderPath + "/imgs/" + spriteData.name + ".png";

                //不是 imgs文件夹的图片先忽略
                FileInfo file = new FileInfo(filename);
                if (!file.Exists) continue;

                long size = file.Length;
                if (!_size2Filenames.ContainsKey(size))
                {
                    _size2Filenames.Add(size, new List<string>());
                }
                _size2Filenames[size].Add(filename);
                _filename2AtlasPath[filename] = atlasPath;
            }
        }

        //移除非给定validList的file
        if(validList != null)
        {
            List<long> removeList = new List<long>();
            foreach(long size in _size2Filenames.Keys)
            {
                List<string> list = _size2Filenames[size];
                bool valid = false;
                foreach(string filename in list)
                {
                    if (validList.Contains(filename))
                    {
                        valid = true;
                        break;
                    }
                }
                if (!valid)
                    removeList.Add(size);
            }
            foreach(long size in removeList)
            {
                _size2Filenames.Remove(size);
            }
        }

        /*
        string[] strs = Directory.GetFiles(_ImgRoot, "*.png", SearchOption.AllDirectories);
        foreach (string filename in strs)
        {
            if (IsInorgeFile(filename)) continue;

            FileInfo file = new FileInfo(filename);
            long size = file.Length;
            if (!_size2Filenames.ContainsKey(size))
            {
                _size2Filenames.Add(size, new List<string>());
            }
            _size2Filenames[size].Add(filename);
        }
        */
    }
    
    private bool IsInorgeFile(string filename)
    {
        foreach (string ignore in _ignoreFolderList)
        {
            if (filename.Contains(ignore)) return true;
        }
        return false;
    }

    /// <summary>
    /// 根据hash进一步匹配
    /// </summary>
    /// <param name="filenames"></param>
    private void UpdateHashHitFiles(List<string> filenames, List<string> validList)
    {
        FileStream file;
        StringBuilder sb;
        foreach (string filename in filenames)
        {
            file = new FileStream(filename, FileMode.Open);
            byte[] hash = GetFileHash(file);

            sb = new StringBuilder();
            foreach(byte by in hash)
            {
                sb.Append(by.ToString("x2"));
            }
            string md5 = sb.ToString();

            if (!_hash2Filenames.ContainsKey(md5))
            {
                _hash2Filenames[md5] = new List<string>();
            }
            _hash2Filenames[md5].Add(filename);
            file.Close();
        }

        List<string> removeList = new List<string>();
        foreach (string hash in _hash2Filenames.Keys)
        {
            var list = _hash2Filenames[hash];
            if(list == null || list.Count <= 1)
            {
                removeList.Add(hash);
            }
        }

        //移除非给定的validList
        if (validList != null)
        { 
            foreach (string hash in _hash2Filenames.Keys)
            {
                if (removeList.Contains(hash)) continue;

                List<string> list = _hash2Filenames[hash];
                bool valid = false;
                foreach (string filename in list)
                {
                    if (validList.Contains(filename))
                    {
                        valid = true;
                        break;
                    }
                }
                if (!valid)
                    removeList.Add(hash);
            }
        }

        foreach (string hash in removeList)
        {
            _hash2Filenames.Remove(hash);
        }
    }

    private bool IsSameFile(string filename1, string filename2)
    {
        FileStream file1 = new FileStream(filename1, FileMode.Open);
        FileStream file2 = new FileStream(filename2, FileMode.Open);
        if (file1.Length != file2.Length) return false;

        byte[] hash1 = GetFileHash(file1);
        byte[] hash2 = GetFileHash(file2);
        for(int i=0; i<hash1.Length; ++i)
        {
            if (hash1[i] != hash2[i]) return false;
        }

        return true;
    }

    private byte[] GetFileHash(FileStream file)
    {
        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        return md5.ComputeHash(file);
    }

    #endregion
}
