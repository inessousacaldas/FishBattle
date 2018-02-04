using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using CodeGeneration;
using System.Text.RegularExpressions;

public class UIViewGenerator : EditorWindow
{
    private const string VIEWCODE_GENERATED_PATH = "Assets/Scripts/MyGameScripts/Module/";

    #region const for relate file generator

    private const string KEY_WORD_MODULE_NAME = "_ModuleName_";
    private const string KEY_WORD_AUTHOR = "_Author_";
    private const string KEY_WORD_CREATED_TIME = "_CreatedTime_";
    private const string KEY_WORD_EVENT_REGISTER = "//_BlockToReplaceForEventRegister_";
    private const string KEY_WORD_EVENT_HANDLER = "//_BlockToReplaceForEventHandler_";
    private const string KEY_WORD_EVENT_COMPONENT = "_ComponentName_";
    private const string VIEWCODE_GENERATED_PATH_CONTROLLER = "Controller/" + KEY_WORD_MODULE_NAME + "Controller.cs";
    private const string VIEWCODE_GENERATED_PATH_VIEW = "View/" + KEY_WORD_MODULE_NAME + "View.cs";
    private const string VIEWCODE_GENERATED_PATH_MODEL = "Model/" + KEY_WORD_MODULE_NAME + "Model.cs";

    private const string VIEWCODE_GENERATED_PATH_PROXY = "Proxy" + KEY_WORD_MODULE_NAME + "Module.cs";

    #endregion

    private const string VIEW_CACHE_PATH = "Assets/Editor/UIViewGenerator/Cache/";
    private const string STATIONARY_GENERATEDLUA_PATH = "Assets/_luaTest/Lua/GameScripts/UIModule/UIView/";
    private const string STATIONARY_GENERATEDLUA_PATH_DEFAULT = "/_luaTest/Lua/GameScripts/UIModule/UIView/";

    private const string VIEWCODE_PATH_MODEL_MANAGER = "Assets/Scripts/MyGameScripts/Manager/ModelManager.cs";
    private const string VIEWCODE_PATH_PROXY_MANAGER = "Assets/Scripts/MyGameScripts/Manager/ProxyManager.cs";
    private const string VIEWCODE_PREFIX_IN_CODE = @"自动生成专用代码段，请勿修改，添加请添加到本段之前。";

    private const string VIEWCODE_EVENT_REGISTER = "\t\tEventDelegate.Set (_view." + KEY_WORD_EVENT_COMPONENT + ".onClick, OnClick" + KEY_WORD_EVENT_COMPONENT + "Handler);\n";
    private const string VIEWCODE_EVENT_HANDLER = "\tprivate void OnClick" + KEY_WORD_EVENT_COMPONENT + "Handler (){\n\t}\n\n";

//    [MenuItem("Window/UIViewGenerator  #&u")]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow<UIViewGenerator>(false, "UIViewGenerator", true);
        window.Show();
    }

    #region Title

    private Transform dragItem;
    private Transform UIPrefabRoot;

    #endregion

    #region SelectionComponentView

    private Vector2 selectionViewPos;

    #endregion

    #region ComponentInfoView

    private UIComponentInfo _curComInfo = null;

    #endregion

    #region ExportListView

    private string _codeGeneratePath = "";
    private string _codeLuaGeneratePath = "";
    private bool exportDicChanged = true;

    private Vector2 exportViewPos;
    private Dictionary<string, UIComponentInfo> _exportInfoDic = new Dictionary<string, UIComponentInfo>();
    private Dictionary<string, bool> _validatedInfoDic = new Dictionary<string, bool>();
    private List<string> _deleteUIDList = new List<string>();

    #endregion

    #region Field and property for relate file generator

    private bool mGenerateRelatedFile = false;
    private bool mGenerateProxy = true;
    private bool mGenerateController = true;
    private bool mGenerateModel = true;
    private bool mGenerateInfo = true;
    //是否生成相关事件
    private bool mGenerateEventHandler = true;
    #pragma warning disable 
    private string mProxyFilePath = string.Empty;
    private string mModelFilePath = string.Empty;
    private string mControllerFilePath = string.Empty;
    private string mViewFilePath = string.Empty;
    private string mModuleName = string.Empty;
    //Current module to create , sometime it's not the module dir
    private string mAuthor = System.Environment.UserName;
    private string mCreateTime = string.Empty;
    private string mModuleFileDir = VIEWCODE_GENERATED_PATH;
   

    private const string _scriptRoot = "Assets/Scripts/MyGameScripts";
    private const string _savePathFile = "Assets/Editor/UIViewGenerator/Cache/PathList.config";

    private static Dictionary<string, string> _view2ScriptPath;

    private static Dictionary<string, string> view2ScriptPath
    {
        get
        {
            if (_view2ScriptPath == null)
            {
                _view2ScriptPath = new Dictionary<string, string>();
                LoadPathList();
            }
            return _view2ScriptPath;
        }
    }

    #endregion

    void OnSelectionChange()
    {
        EditorGUIUtility.editingTextField = false;
        //      _curComInfo = null;
        Repaint();
    }

    void OnProjectChange()
    {
        exportDicChanged = true;
    }

    void OnDestroy()
    {
        //if (UIPrefabRoot && exportDicChanged) {
        //    if (EditorUtility.DisplayDialog("Save UIViewCache", "当前UIViewCache尚未保存，请问是否保存", "Save", "Canel")) {
        //        SaveUIViewCache();
        //    }
        //}

        AssetDatabase.Refresh();
    }

    void OnGUI()
    {
        if (GUILayout.Button("更新文件列表配置文件(当找不到已有的view时使用)", "LargeButton", GUILayout.Height(50f)))
        {
            FixDirectory();
            CleanUp();
        }

        //按需开放此接口
        //if (GUILayout.Button("转换旧的ViewBase to ViewBaseNew", "LargeButton", GUILayout.Height(50f)))
        //{
        //    ChangeOldBaseView();
        //}


        DrawTitleGroup();

        if (!UIPrefabRoot)
            return;

        EditorGUILayout.BeginHorizontal();

        //SelectionComponentView
        DrawSelectionComponentView();

        EditorGUILayout.BeginVertical(); //Right Part Vertical Begin
        //ComponentInfoView
        DrawComponentInfoView();

        //ExportListView
        DrawExportListView();

        DrawRelateFileView();



        EditorGUILayout.EndVertical(); //Right Part Vertical End

        EditorGUILayout.EndHorizontal();
    }

    //读取缓存UIViewCache数据
    void LoadUIViewCache()
    {
        OnChangeUIPrefab();
        LoadUIViewByScript();
    }

    /// <summary>
    /// 修复文件列表
    /// </summary>
    private void FixDirectory()
    {
        view2ScriptPath.Clear();
        bool bSave = false;
        string[] strs = Directory.GetFiles(_scriptRoot, "*.cs", SearchOption.AllDirectories);
        foreach (string filename in strs)
        {
            string sFileContent = File.ReadAllText(filename);
            string sPattern = @"public\s+partial\s+class\s+(\S+)\s*\:\s*BaseView[\n\s]+";
            Match mat = Regex.Match(sFileContent, sPattern);
            if (!mat.Success)
            {
                mat = Regex.Match(sFileContent, @"public\s+class\s+(\S+)\s*\:\s*BaseView[\n\s]+");
            }
            if (mat != null && mat.Success)
            {
                string name = mat.Groups[1].Value;
                string path = filename.Replace("Assets", "");
                if (view2ScriptPath.ContainsKey(name) && view2ScriptPath[name] == path)
                {
                    continue;
                }
                view2ScriptPath[name] = path;
                bSave = true;
            }
        }

        if (bSave)
        {
            SavePathList();
        }
    }

    public static void LoadPathList()
    {
        string sFileContent = File.ReadAllText(_savePathFile);
        string[] strs = sFileContent.Split('\n');
        foreach (string str in strs)
        {
            string[] list = str.Split(' ');
            if (list.Length != 2)
            {
                continue;
            }
            view2ScriptPath[list[0]] = list[1];
        }
    }

    public void SavePathList()
    {
        string sSave = "";
        var keys = view2ScriptPath.Keys.ToList();
        
        keys.Sort();
        
        foreach (var name in keys)
        {
            sSave += name + " " + view2ScriptPath[name] + "\n";
        }
        File.WriteAllText(_savePathFile, sSave);
    }

    public void UpdatePateList(string name, string path)
    {
        if (view2ScriptPath.ContainsKey(name) && view2ScriptPath[name] == path)
        {
            return;
        }
        view2ScriptPath[name] = path;
        SavePathList();
    }

    public void LoadUIViewByScript()
    { 
        if (UIPrefabRoot == null)
        {
            return;
        }

        view2ScriptPath.TryGetValue(UIPrefabRoot.name, out _codeGeneratePath);
        if (string.IsNullOrEmpty(_codeGeneratePath))
            return;

        string sFileContent = File.ReadAllText(Application.dataPath + _codeGeneratePath);
        if (string.IsNullOrEmpty(sFileContent))
            return;

        Match contentMat = Regex.Match(sFileContent, @"protected override void InitElementBinding\s*\(\)[\n\s]*\{[\n\s]*([\n\s\S]+?)[\n\s]*\}");
        if (!contentMat.Success)
        {
            Debug.LogError("匹配出错！");
            return;
        }

        string sContent = contentMat.Groups[1].Value;
        string[] sLines = sContent.Split('\n');
        string pat1 = @"(\S+) = root\.GetComponent<(\S+)>\(\)";
        string pat2 = @"(\S+) = root\.Find\(""(\S+)""\)(\S*);";
        string comPattern = @"\.GetComponent<(\S+)>\(\)";
        Match mat;
        foreach (string sLine in sLines)
        {
            mat = Regex.Match(sLine, pat1);
            string name = string.Empty;
            string type = string.Empty;
            string path = string.Empty;
            if (mat.Success)
            {
                name = mat.Groups[1].Value;
                type = mat.Groups[2].Value;
                path = "";
            }
            else
            {
                mat = Regex.Match(sLine, pat2);
                name = mat.Groups[1].Value;
                path = mat.Groups[2].Value;


                if (path.Contains(" "))
                {
                    GameDebuger.LogError(string.Format("路径中有空格！path：{0}", path));
                    path = path.Replace(" ", string.Empty);
                    continue;
                }

                type = mat.Groups[3].Value;
                if (string.IsNullOrEmpty(type))
                {
                    type = "Transform";
                }
                else if (type.EndsWith("gameObject"))
                {
                    type = "GameObject";
                }
                else if (type.Contains("GetComponent"))
                {
                    Match comMat = Regex.Match(sLine, comPattern);
                    if (comMat == null || !comMat.Success)
                    {
                        Debug.LogError("未匹配的类型");
                    }
                    type = comMat.Groups[1].Value;
                }
                else
                {
                    Debug.LogError("未匹配的类型:" + sLine);
                }
            }
            if (!string.IsNullOrEmpty(name))
            {
                UIComponentInfo item = new UIComponentInfo(UIPrefabRoot, name, path, type);
                if (!_exportInfoDic.ContainsKey(item.uid))
                {
                    _exportInfoDic.Add(item.uid, item);
                    Transform itemTrans = UIPrefabRoot.Find(item.path);
                    _validatedInfoDic.Add(item.uid, (itemTrans != null));
                }
            }
        }
    }

    public static string ReplaceComponentPath(string name, Dictionary<string, string> replaceDct)
    {
        string scriptPath;
        view2ScriptPath.TryGetValue(name, out scriptPath);
        if (scriptPath == null)
            return string.Empty;

        string sFileContent = File.ReadAllText(Application.dataPath + scriptPath);
        if (string.IsNullOrEmpty(sFileContent))
            return string.Empty;

        string sSave = sFileContent;
        foreach (string src in replaceDct.Keys)
        {
            sSave = sSave.Replace(src, replaceDct[src]);
        }

        if (!sFileContent.Equals(sSave))
        {
            File.WriteAllText(Application.dataPath + scriptPath, sSave);
            return scriptPath;
        }
        return string.Empty;
    }

    private void AdjustCodePath(UIViewCache record)
    {
        string codePath = record.codePath;
        string flag = "/Assets";
        int index = codePath.IndexOf(flag);
        if (index != -1)
        {
            record.codePath = codePath.Substring(index + flag.Length);
        }
    }

    private void AdjustLuaCodePath(UIViewCache record)
    {
        //      string codeLuaPath = record.codeLuaPath;
        //      string flag = "/Assets";
        //      int index = codeLuaPath.IndexOf (flag);
        //      if (index != -1)
        //      {
        //          record.codeLuaPath = codeLuaPath.Substring(index+flag.Length);
        //      }
    }

    //保存UIViewCache信息
    UIViewCache SaveUIViewCache()
    {
        UIViewCache record = ScriptableObject.CreateInstance<UIViewCache>();
        record.codePath = _codeGeneratePath;
        //      record.codeLuaPath = _codeLuaGeneratePath;

        record.componentInfoList = new List<UIComponentInfo>(_exportInfoDic.Values);

        //EditorHelper.CreateScriptableObjectAsset (record, VIEW_CACHE_PATH, UIPrefabRoot.name);

        return record;
    }

    void DrawTitleGroup()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        GUILayout.Space(15f);
        EditorGUILayout.LabelField(@"UIPrefab", GUILayout.Width(60f));
        GUILayout.Space(5f);
        dragItem = EditorGUILayout.ObjectField(dragItem, typeof(Transform), true, GUILayout.ExpandWidth(false), GUILayout.Width(140f)) as Transform;
        if (dragItem != null)
        {
            if (dragItem != UIPrefabRoot)
            {
                if (PrefabInstanceCheck(dragItem))
                {
                    dragItem = PrefabUtility.FindPrefabRoot(dragItem.gameObject).transform;
                    if (UIPrefabRoot != dragItem)
                    {
                        UIPrefabRoot = dragItem;
                        this.RemoveNotification();
                        LoadUIViewCache();
                    }
                    else
                    {
                        this.ShowNotification(new GUIContent("这是同一个UIPrefab"));
                    }
                }
                else
                {
                    this.ShowNotification(new GUIContent("这不是一个PrefabInstance"));
                    CleanUp();
                }
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorHelper.DrawBoxLabel("C#\nGenertePath:", _codeGeneratePath == "" ? "NONE" : _codeGeneratePath, false,true,300);
        if (GUILayout.Button("Clean\nC#Path", GUILayout.Width(60f), GUILayout.Height(45f)))
        {
            _codeGeneratePath = null;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorHelper.DrawBoxLabel("Lua\nGenertePath:", _codeLuaGeneratePath == "" ? "NONE" : _codeLuaGeneratePath, false,true,300);
        if (GUILayout.Button("Clean\nLuaPath", GUILayout.Width(60f), GUILayout.Height(45f)))
        {
            _codeLuaGeneratePath = null;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private bool PrefabInstanceCheck(Object target)
    { 
        PrefabType type = PrefabUtility.GetPrefabType(target);

        if (type == PrefabType.PrefabInstance)
        {
            return true;
        }
        return false;
    }

    void DrawSelectionComponentView()
    {
        EditorGUILayout.BeginVertical(GUILayout.MinWidth(150f));
        if (EditorHelper.DrawHeader("Node Components"))
        {
            selectionViewPos = EditorGUILayout.BeginScrollView(selectionViewPos, "TextField");

            //根节点只显示除GameObject、Transform以外的组件
            int validateCode = ValidateSelection(Selection.activeTransform);
            if (validateCode != ERROR_NODE)
            {

                if (validateCode != ROOT_NODE && GUILayout.Button("---GameObject---"))
                {
                    UIComponentInfo newInfo = new UIComponentInfo(Selection.activeGameObject, UIPrefabRoot);
                    if (_exportInfoDic.ContainsKey(newInfo.uid))
                        _curComInfo = _exportInfoDic[newInfo.uid];
                    else
                        _curComInfo = newInfo;

                    EditorGUIUtility.editingTextField = false;
                }
                GUILayout.Space(3f);

                GUILayout.Label("ComponentList");
                Component[] comList = Selection.activeTransform.GetComponents<Component>();
                foreach (Component com in comList)
                {

                    if (validateCode == ROOT_NODE && com.GetType().Name == "Transform")
                        continue;

                    if (GUILayout.Button(com.GetType().Name))
                    {
                        UIComponentInfo newInfo = new UIComponentInfo(com, UIPrefabRoot);
                        if (_exportInfoDic.ContainsKey(newInfo.uid))
                            _curComInfo = _exportInfoDic[newInfo.uid];
                        else
                            _curComInfo = newInfo;

                        EditorGUIUtility.editingTextField = false;
                    }
                    GUILayout.Space(3f);
                }
            }
            else
                EditorGUILayout.HelpBox("This is not a node of UIPrefab", MessageType.Info);
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.EndVertical();
    }

    void DrawComponentInfoView()
    {
        EditorHelper.DrawHeader("ComponentInfo");
        if (_curComInfo != null)
        {
            EditorGUIUtility.labelWidth = 100f;
            EditorHelper.DrawBoxLabel("UID: ", _curComInfo.uid, true);
            EditorHelper.DrawBoxLabel("Type: ", _curComInfo.comType, false);

            string memberName = EditorGUILayout.TextField("MemberName: ", _curComInfo.memberName, GUILayout.Height(20));
            if (memberName != _curComInfo.memberName)
            {
                _curComInfo.memberName = memberName;
                exportDicChanged = true;
            }
            EditorHelper.DrawBoxLabel("Path: ", (_curComInfo.path == "" ? "This is a root node" : _curComInfo.path), true);

            GUILayout.BeginHorizontal(GUILayout.MinHeight(20f));
            if (!_exportInfoDic.ContainsKey(_curComInfo.uid))
            {
                if (GUILayout.Button("Add", GUILayout.Width(60f)))
                {
                    _exportInfoDic.Add(_curComInfo.uid, _curComInfo);
                    EditorGUIUtility.editingTextField = false;
                    exportDicChanged = true;
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    void DrawExportListView()
    {
        EditorHelper.DrawHeader("ExportList");
        GUILayout.BeginHorizontal();
        GUILayout.Space(3f);
        GUILayout.BeginVertical();
        exportViewPos = EditorGUILayout.BeginScrollView(exportViewPos);
        bool delete = false;
        int index = 0;
        foreach (UIComponentInfo item in _exportInfoDic.Values)
        {
            ++index;
            GUILayout.Space(-1f);
            bool highlight = (_curComInfo != null) && (item.uid == _curComInfo.uid);
            if (_validatedInfoDic.ContainsKey(item.uid) && !_validatedInfoDic[item.uid])
            {
                GUI.backgroundColor = Color.red;
            }
            else
                GUI.backgroundColor = highlight ? Color.white : new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));

            GUI.backgroundColor = Color.white;
            GUILayout.Label(index.ToString(), GUILayout.Width(20f));

            if (GUILayout.Button(item.GetName(), "OL TextField", GUILayout.Height(20f)))
            {
                _curComInfo = item;
                Selection.activeTransform = UIPrefabRoot.Find(_curComInfo.path);
            }

            if (_deleteUIDList.Contains(item.uid))
            {
                GUI.backgroundColor = Color.red;

                if (GUILayout.Button("Delete", GUILayout.Width(60f)))
                {
                    delete = true;
                }
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("X", GUILayout.Width(22f)))
                {
                    _deleteUIDList.Remove(item.uid);
                    delete = false;
                }
                GUI.backgroundColor = Color.white;
            }
            else
            {
                // If we have not yet selected a sprite for deletion, show a small "X" button
                if (GUILayout.Button("X", GUILayout.Width(22f)))
                    _deleteUIDList.Add(item.uid);
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical(); 
        GUILayout.Space(3f);
        GUILayout.EndHorizontal();

        if (delete)
        {
            foreach (string uid in _deleteUIDList)
            {
                _exportInfoDic.Remove(uid);
                _validatedInfoDic.Remove(uid);
            }
            _deleteUIDList.Clear();
            exportDicChanged = true;
        }

        EditorGUILayout.BeginHorizontal();

        //  C# 代码生成 ###############################
        if (GUILayout.Button("GenerateC#Code", GUILayout.Width(120f), GUILayout.Height(40f)))
        {
            if (exportDicChanged)
            {
                List<UIComponentInfo> comList = new List<UIComponentInfo>(_exportInfoDic.Values);
                if (ProduceCSharpCode(UIPrefabRoot.name, comList))
                {
                    exportDicChanged = false;
                    UpdatePateList(UIPrefabRoot.name, _codeGeneratePath);
                }
            }
            else
                this.ShowNotification(new GUIContent("Failure 保存为C# 但是没有任何变更操作"));
        }

        //  Lua 代码生成    ###############################
        GUI.color = Color.green;
        //GUI.backgroundColor = Color.green;
        //      if (GUILayout.Button ("GenerateLuaCode", GUILayout.Width (120f), GUILayout.Height (40f))) {
        //          if (exportDicChanged) {
        //              UIViewCache record = SaveUIViewCache ();
        //              GenerateLuaCode (record);
        //              exportDicChanged = false;
        //          }
        //          else
        //              this.ShowNotification(new GUIContent("Failure 保存为Lua 但是没有任何变更操作"));
        //      }
        GUI.color = Color.white;

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10f);
    }

    /// <summary>
    /// Validates the selection.
    /// -1:不是当前Prefab的节点
    /// 0：是当前Prefab的根节点
    /// 1：是当前Prefab的子节点
    /// </summary>
    private const int ERROR_NODE = -1;
    private const int ROOT_NODE = 0;
    private const int CHILD_NODE = 1;

    private int ValidateSelection(Transform selection)
    {
        if (selection == UIPrefabRoot)
            return 0;

        Transform trans = selection;
        while (trans != null)
        {
            if (trans == UIPrefabRoot)
                return 1;
            trans = trans.parent;
        }
        return -1;
    }

    private void CleanUp()
    {
        //清空操作
        dragItem = null;
        UIPrefabRoot = null;
        OnChangeUIPrefab();
    }

    private void OnChangeUIPrefab()
    {
        _codeGeneratePath = "";
        _codeLuaGeneratePath = "";
        _curComInfo = null;
        exportDicChanged = true;
        _deleteUIDList.Clear();
        _exportInfoDic.Clear();
        _validatedInfoDic.Clear();
        ClearRelateInfo();
    }

    public bool ProduceCSharpCode(string className, List<UIComponentInfo> componentList)
    {
        if (string.IsNullOrEmpty(_codeGeneratePath) && !Directory.Exists(_codeGeneratePath))
        {
            var outputPath = EditorUtility.SaveFolderPanel("Please Select Module Directory To Save", VIEWCODE_GENERATED_PATH, "");

            var viewPath = "/View/";

            if (!outputPath.Contains(viewPath))
            {
                outputPath = outputPath + viewPath;

                if (!File.Exists(outputPath))
                {
                    Refactor.RefactorUtils.CreateFileDirectory(outputPath, false);
                }
            }

            outputPath = outputPath + className + "AutoGen.cs";

            _codeGeneratePath = outputPath.Replace(Application.dataPath, "");
        }

        if (string.IsNullOrEmpty(_codeGeneratePath))
        {
            return false;
        }
         
        try
        {
            // Build the generator with the class name and data source.
            UIViewCodeTemplate generator = new UIViewCodeTemplate(className, componentList);

            // Generate output (class definition).
            var classDefintion = generator.TransformText();
            // Save new class to assets folder.
            File.WriteAllText(Application.dataPath + _codeGeneratePath, classDefintion);

            this.ShowNotification(new GUIContent("Success 生成C#代码完毕"));
            // Refresh assets.
            //          AssetDatabase.Refresh ();
        }
        catch (System.Exception e)
        {
            Debug.Log("An error occurred while saving file: " + e);
        }
        return true;
    }

    public void GenerateCSharpCode(UIViewCache source)
    {
        // Build the generator with the class name and data source.
        UIViewCodeTemplate generator = new UIViewCodeTemplate(source.name, source.componentInfoList);

        // Generate output (class definition).
        var classDefintion = generator.TransformText();

        if (string.IsNullOrEmpty(_codeGeneratePath) && !Directory.Exists(_codeGeneratePath))
        {
            var outputPath = EditorUtility.SaveFilePanel("Save C# UIView Code",
                                 VIEWCODE_GENERATED_PATH,
                                 source.name,
                                 "cs");

            outputPath = outputPath.Replace(Application.dataPath, "");

            _codeGeneratePath = outputPath;
            source.codePath = _codeGeneratePath;
        }

        if (string.IsNullOrEmpty(_codeGeneratePath))
            return;

        try
        {
            // Save new class to assets folder.
            File.WriteAllText(Application.dataPath + _codeGeneratePath, classDefintion);

            this.ShowNotification(new GUIContent("Success 生成C#代码完毕"));
            // Refresh assets.
            //          AssetDatabase.Refresh ();
        }
        catch (System.Exception e)
        {
            Debug.Log("An error occurred while saving file: " + e);
        }
    }

    #region View 保存方法 Begin

    public void GenerateLuaCode(UIViewCache source)
    {
        UIViewCodeTemplateToLua generatorToLua = new UIViewCodeTemplateToLua(source.name, source.componentInfoList);

        var classDefintionToLua = generatorToLua.TransformText();
        if (string.IsNullOrEmpty(_codeLuaGeneratePath) && !Directory.Exists(_codeLuaGeneratePath))
        {
            //var outputPath = EditorUtility.SaveFilePanel("Save Lua UIView Code",
            //                                             STATIONARY_GENERATEDLUA_PATH,
            //                                             source.name,
            //                                             "lua");
            //GameDebuger.OrangeDebugLog(outputPath.ToString());

            //outputPath = outputPath.Replace(Application.dataPath, "");
            var outputPath = string.Format("{0}{1}.lua", STATIONARY_GENERATEDLUA_PATH_DEFAULT, source.name);

            _codeLuaGeneratePath = outputPath;
            //          source.codeLuaPath = _codeLuaGeneratePath;
        }

        if (string.IsNullOrEmpty(_codeLuaGeneratePath))
            return;

        try
        {
            // Save new class to assets folder.
            File.WriteAllText(Application.dataPath + _codeLuaGeneratePath, classDefintionToLua);
            this.ShowNotification(new GUIContent("Success 生成Lua代码完毕"));
        }
        catch (System.Exception e)
        {
            Debug.Log("An error occurred while saving file: " + e);
        }
    }

    #endregion

    #region Relate file generator ,such as proxy,controller and model,etc.

    private void DrawRelateFileView()
    {
        mGenerateRelatedFile = GUILayout.Toggle(mGenerateRelatedFile, new GUIContent("Generate Relate File (Proxy/Controller/Model)"));

        if (mGenerateRelatedFile)
        {
            InitModuleName();

            GUILayout.BeginHorizontal();
            GUILayout.Label("ModuleName:", GUILayout.Width(150));
            mModuleName = GUILayout.TextField(mModuleName);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Author:", GUILayout.Width(150));
            mAuthor = GUILayout.TextField(mAuthor);
            GUILayout.EndHorizontal();
            mCreateTime = System.DateTime.Now.ToString();
            GUILayout.Label("Select File To Generate:");
            mGenerateProxy = GUILayout.Toggle(mGenerateProxy, new GUIContent("Generate Proxy : " + VIEWCODE_GENERATED_PATH_PROXY.Replace(KEY_WORD_MODULE_NAME, mModuleName)));
            mGenerateController = GUILayout.Toggle(mGenerateController, new GUIContent("Generate Controller : " + VIEWCODE_GENERATED_PATH_CONTROLLER.Replace(KEY_WORD_MODULE_NAME, mModuleName)));
            if (mGenerateController)
                mGenerateEventHandler = GUILayout.Toggle(mGenerateEventHandler, new GUIContent("\tGenerate Event Handler for Controller: " + VIEWCODE_GENERATED_PATH_CONTROLLER.Replace(KEY_WORD_MODULE_NAME, mModuleName)));
            mGenerateModel = GUILayout.Toggle(mGenerateModel, new GUIContent("Generate Model : " + VIEWCODE_GENERATED_PATH_MODEL.Replace(KEY_WORD_MODULE_NAME, mModuleName)));

            if (GUILayout.Button("Generate Related File "))
            {
                if (relateFileCreateEnable)
                {
                    UpdateModuleFileDir();
                    GenerateFile(); 
                }
            }
        }
    }

    private bool relateFileCreateEnable
    {
        get
        {
            string tNotification = string.Empty;
            if (string.IsNullOrEmpty(mModuleName))
            {
                tNotification = "Set the module name first!";
            }
            else if (string.IsNullOrEmpty(mAuthor))
            {
                tNotification = "Set the author name first!";
            }

            if (string.IsNullOrEmpty(tNotification))
                return true;
            else
            {
                ShowNotification(new GUIContent(tNotification));
                return false;
            }
        }
    }

    private void InitModuleName()
    {
        if (string.IsNullOrEmpty(mModuleName))
        {
            if (null != UIPrefabRoot)
            {
                string tViewFileName = UIPrefabRoot.name;//Path.GetFileNameWithoutExtension(_codeGeneratePath);
                if (!string.IsNullOrEmpty(tViewFileName))
                {
                    if (tViewFileName.IndexOf("View") != -1)
                    {
                        tViewFileName = tViewFileName.Replace("View", string.Empty);
                        mModuleName = tViewFileName;
                    }
                }
            }
        }
    }

    private void UpdateModuleFileDir()
    {
        string tTempFilePath = "Assets" + _codeGeneratePath;
        if (!string.IsNullOrEmpty(tTempFilePath) && File.Exists(tTempFilePath))
            mModuleFileDir = Directory.GetParent(tTempFilePath).FullName;//dir of the module dir , sometime it's not same with the view code...
        mModuleFileDir = EditorUtility.SaveFolderPanel("Please Select Module Directory To Save", mModuleFileDir, mModuleFileDir);
        if (string.IsNullOrEmpty(mModuleFileDir))
            mModuleFileDir = VIEWCODE_GENERATED_PATH;
    }

    private void GenerateFile()
    {
        if (mGenerateProxy)
        {
            //          GenerateFile(VIEWCODE_GENERATED_PATH_PROXY,VIEWCODE_GENERATED_PATH_PROXY.Replace(KEY_WORD_MODULE_NAME,mModuleName));
            mProxyFilePath = GenerateFileByCopyFromTempDirToModuleDir(VIEWCODE_GENERATED_PATH_PROXY);
            UpdateProxyManager(mModuleName, mAuthor, mCreateTime);
            GameDebuger.Log("Proxy 生成完毕");
        }
        if (mGenerateController)
        {
            mControllerFilePath = GenerateFileByCopyFromTempDirToModuleDir(VIEWCODE_GENERATED_PATH_CONTROLLER);
            GameDebuger.Log("Controller 生成完毕");
            if (mGenerateEventHandler)
            {
                GenerateRelatedEventHandler(mModuleFileDir + _codeGeneratePath, mControllerFilePath);
                GameDebuger.Log("EventHandler 生成完毕");
            }

        }

        if (mGenerateModel)
        {
            mModelFilePath = GenerateFileByCopyFromTempDirToModuleDir(VIEWCODE_GENERATED_PATH_MODEL);
            UpdateModelManager(mModuleName, mAuthor, mCreateTime);
            GameDebuger.Log("Model 生成完毕");
        }
        EditorUtility.DisplayDialog("代码生成", "代码生成完毕", "好的");
    }

    private string GenerateFileByCopyFromTempDirToModuleDir(string pTemplateFileName)
    {
        string tFilePath = string.Empty;
        if (string.IsNullOrEmpty(mModuleFileDir))
        {
            GameDebuger.LogWarning(string.Format("GenerateFileByCopyFromTempDirToModuleDir failed for mModuleFileDir is null, pTemplateFileName:{0}", pTemplateFileName));
        }
        else
        {
            string tTemplateFilePath = string.Format("{0}{1}Module/{2}", Refactor.RefactorConst.PROJECT_PATH + VIEWCODE_GENERATED_PATH, KEY_WORD_MODULE_NAME, pTemplateFileName);
            tFilePath = string.Format("{0}{1}/{2}", mModuleFileDir, string.Empty, pTemplateFileName.Replace(KEY_WORD_MODULE_NAME, mModuleName));
            GenerateFile(tTemplateFilePath, tFilePath);    
        }
        return tFilePath;
    }

    private void GenerateFile(string pTemplatePath, string pCreatedFilePath)
    {
        if (File.Exists(pTemplatePath))
        {
            if (!File.Exists(pCreatedFilePath))
            {
                Refactor.RefactorUtils.CreateFileDirectory(pCreatedFilePath, false);
                FileUtil.CopyFileOrDirectory(pTemplatePath, pCreatedFilePath);
                UpdateFileContent(pCreatedFilePath);
            }
            else
                GameDebuger.Log(string.Format("Generate File canceled for file is already exists. File Path:{0}", pCreatedFilePath));
        }
    }

    private void UpdateFileContent(string pFilePath)
    {
        if (!string.IsNullOrEmpty(pFilePath) && File.Exists(pFilePath))
        {
            string tFileContent = File.ReadAllText(pFilePath);
            if (!string.IsNullOrEmpty(tFileContent))
            {
                tFileContent = tFileContent.Replace(KEY_WORD_MODULE_NAME, mModuleName).Replace(KEY_WORD_AUTHOR, mAuthor).Replace(KEY_WORD_CREATED_TIME, mCreateTime);
                File.WriteAllText(pFilePath, tFileContent);
                GameDebuger.Log(string.Format("Generate File {0} success !", pFilePath));
            }

        }
    }

    private void ClearRelateInfo()
    {
        mGenerateRelatedFile = false;
        mGenerateProxy = true;
        mGenerateController = true;
        mGenerateModel = true;
        mGenerateEventHandler = true;
        mModuleName = string.Empty;//Current module to create , sometime it's not the module dir
        mAuthor = System.Environment.UserName;
        mCreateTime = string.Empty;
        mModuleFileDir = VIEWCODE_GENERATED_PATH;
    }

    #endregion

    #region 界面事件监听的自动生成

    private void GenerateRelatedEventHandler(string pViewFilePath, string pControllerFilePath)
    {
        GameDebuger.Log(string.Format("GenerateRelatedEventHandler pViewFilePath:{0},pControllerFilePath:{1}", pViewFilePath, pControllerFilePath));
        List<string > tBtnComponentList = GetBtnComponentList();
        if (null != tBtnComponentList && tBtnComponentList.Count > 0)
        {
            string tControllerFileContent = GetControllerFileContent(pControllerFilePath);
            if (!string.IsNullOrEmpty(tControllerFileContent))
            {
                tControllerFileContent = ReplaceEventRelatedCode(tControllerFileContent, tBtnComponentList);
                if (!string.IsNullOrEmpty(tControllerFileContent))
                {
                    File.WriteAllText(pControllerFilePath, tControllerFileContent);
                    GameDebuger.Log("GenerateRelatedEventHandler success !");
                }
                else
                {
                    GameDebuger.LogError("GenerateRelatedEventHandler failed ,tControllerFileContent is null now!");
                }
            }
        }
    }

    private string GetControllerFileContent(string pControllerFilePath)
    {
        string tControllerFileContent = File.ReadAllText(pControllerFilePath);
        if (string.IsNullOrEmpty(tControllerFileContent))
        {
            string tTip = "生成事件相关代码失败，找不到 Controller 类";
            GameDebuger.LogError(tTip);
            ShowNotification(new GUIContent(tTip));
            return null;
        }
        return tControllerFileContent;
    }

    private List<string> GetBtnComponentList(bool pNeedTip = true)
    {
        ;
        List<string> tBtnComponentList = null;
        if (null != _exportInfoDic && _exportInfoDic.Count > 0)
        {
            Dictionary<string,UIComponentInfo>.Enumerator tEnum = _exportInfoDic.GetEnumerator();
            UIComponentInfo tUIComponentInfo = null;
            while (tEnum.MoveNext())
            {
                tUIComponentInfo = tEnum.Current.Value;
                if (null != tUIComponentInfo && tUIComponentInfo.comType == "Button")
                {
                    if (null == tBtnComponentList)
                        tBtnComponentList = new List<string>();
                    tBtnComponentList.Add(tUIComponentInfo.memberName);
                }
            }
        }
        if ((null == tBtnComponentList || tBtnComponentList.Count <= 0) && pNeedTip)
            GameDebuger.Log("界面中没有按钮组件，不设置按钮相关事件监听和回掉。");
        return tBtnComponentList;
    }

    private string ReplaceEventRelatedCode(string pControllerFileContent, List<string > pBtnComponentList)
    {
        string tEventRegisterStr = string.Empty;
        string tEventHandlerStr = string.Empty;
        CreateEventRelatedCode(pBtnComponentList, ref tEventRegisterStr, ref tEventHandlerStr);
        pControllerFileContent = ReplateContent(pControllerFileContent, KEY_WORD_EVENT_REGISTER, tEventRegisterStr);
        pControllerFileContent = ReplateContent(pControllerFileContent, KEY_WORD_EVENT_HANDLER, tEventHandlerStr);
        return pControllerFileContent;
    }

    private string ReplateContent(string pAllContent, string pOrignBlockContent, string pNewBlockContent)
    {
        if (string.IsNullOrEmpty(pAllContent) || string.IsNullOrEmpty(pOrignBlockContent))
        {
            GameDebuger.LogError(string.Format("ReplateContent failed for pAllContent:{0} or pOrignBlockContent:{1} IsNullOrEmpty", pAllContent, pOrignBlockContent));
            return null;
        }
        else
        {
            return pAllContent.Replace(pOrignBlockContent, pNewBlockContent); 
        }
    }

    private void CreateEventRelatedCode(List<string > pComponentNameList, ref string tEventRegisterStr, ref string tEventHandlerStr)
    {
        tEventRegisterStr = string.Empty;
        tEventHandlerStr = string.Empty;
        if (null != pComponentNameList && pComponentNameList.Count > 0)
        {
            string tComponentName = string.Empty;
            for (int tCounter = 0; tCounter < pComponentNameList.Count; tCounter++)
            {
                tComponentName = pComponentNameList[tCounter];
                if (string.IsNullOrEmpty(tComponentName))
                    continue;
                tEventRegisterStr += VIEWCODE_EVENT_REGISTER.Replace(KEY_WORD_EVENT_COMPONENT, tComponentName);
                tEventHandlerStr += VIEWCODE_EVENT_HANDLER.Replace(KEY_WORD_EVENT_COMPONENT, tComponentName);
            }
        }
        if (!string.IsNullOrEmpty(tEventRegisterStr))//干掉多余的一行
            tEventRegisterStr = tEventRegisterStr.Substring(0, tEventRegisterStr.Length - 1);
        if (!string.IsNullOrEmpty(tEventHandlerStr))//干掉多余的一行
            tEventHandlerStr = tEventHandlerStr.Substring(0, tEventHandlerStr.Length - 1);
    }

    #endregion

    #region 转换旧版BaseView

    private const string CHECK_VIEW_LOG = "D:/Check_View_Log.txt";
    private const string PAT_VIEW = @"public\s+class\s+(\S+)\s*\:\s*BaseView[\n\s]+";

    //把GetMissingComponent|AddMissingComponent|AddComponent|GetComponent<XXXView>();
    //都替换成 new XXXView();
    //GameObjectExt.GetMissingComponent(gameObject)情况也已处理
    private const string PAT_REF = @"\S+\.(GetMissingComponent|AddMissingComponent|AddComponent|GetComponent)<\s*(\S+)\s*>\s*\(\s*\S*\s*\);";

    //找出继承MonoViewController的类,这些类会引用view
    private const string PAT_VIEWCONTAINER = @"class\s+(\S+)\s*<\s*T\s*>:\s*(\S+)\s*<\s*T\s*>";

    //处理引用View的情况,列如 class ArenaMainViewController : MonoViewController<ArenaMainView>
    private const string PAT_VIEWCONTAINER_REF = @"class\s+\S+\s*:\s*{0}\s*<\s*(\S+)\s*>";

    //转换旧版BaseView to BaseView
    private void ChangeOldBaseView()
    {
        //缓存文件内容
        Dictionary<string, string> path2Content = new Dictionary<string, string>();

        //重新文件遍历，确保列表完整
        Dictionary<string, string> tView2Path = new Dictionary<string, string>();

        //获取此类型（PAT_VIEWCONTAINER）继承的映射
        Dictionary<string, string> tContainerDct = new Dictionary<string, string>();

        //测试
        string[] fileNameList = Directory.GetFiles(_scriptRoot, "*.cs", SearchOption.AllDirectories);
        foreach (string filename in fileNameList)
        {
            string sFileContent = File.ReadAllText(filename);
            //View检测
            Match mat = Regex.Match(sFileContent, PAT_VIEW);
            if (mat != null && mat.Success)
            {
                string name = mat.Groups[1].Value;
                if (tView2Path.ContainsKey(name) && tView2Path[name] == filename)
                {
                    Debug.LogError("警告，发现同名view" + name);
                    continue;
                }
                tView2Path[name] = filename;
                path2Content.Add(filename, sFileContent);
            }
            else
            {
                //View Container 检测预处理
                mat = Regex.Match(sFileContent, PAT_VIEWCONTAINER);
                if (mat != null && mat.Success)
                {
                    string viewContainer = mat.Groups[1].Value;
                    string class1 = mat.Groups[1].Value;
                    string class2 = mat.Groups[2].Value;
                    tContainerDct.Add(class1, class2);
                }
            }
        }

        //筛选出所有View容器类，基类只发现MonoViewController;
        List<string> tViewContainers = new List<string>();
        List<string> baseList = new List<string>();
        baseList.Add("MonoViewController");
        while (baseList.Count > 0)
        {
            foreach (var baseCls in baseList)
            {
                tViewContainers.Add(baseCls);
            }
            baseList = GetSuperClass(tContainerDct, baseList);
        }


        Dictionary<string, List<string>> hitPath2Views = new Dictionary<string, List<string>>();
        List<string> hitViewPath = new List<string>();

        foreach (string filename in fileNameList)
        {
            //view文件不能忽略了
            //if (tView2Path.ContainsValue(filename)) continue;

            string sFileContent = File.ReadAllText(filename);
            var mat = Regex.Match(sFileContent, PAT_REF);
            while (mat != null && mat.Success)
            {
                string name = mat.Groups[2].Value;
                if (tView2Path.ContainsKey(name))
                {
                    //缓存文件
                    if (!path2Content.ContainsKey(filename))
                    {
                        path2Content.Add(filename, sFileContent);
                    }

                    //替换 gameObject.GetMissingComponent<XXView> to new XXView();
                    //可能会替换多个，不影响
                    path2Content[filename] = path2Content[filename].Replace(
                        mat.Groups[0].Value, string.Format(@"new {0}();", name));
                    string viewPath = tView2Path[name];
                    if (!hitViewPath.Contains(viewPath))//替换一次就够了
                    {
                        //替换BaseView to BaseView
                        hitViewPath.Add(viewPath);
                        path2Content[viewPath] = ReplaceView(name, path2Content[viewPath]);
                    }

                    if (!hitPath2Views.ContainsKey(filename))
                    {
                        hitPath2Views.Add(filename, new List<string>());
                    }
                    if (!hitPath2Views[filename].Contains(name))
                    {
                        hitPath2Views[filename].Add(name);
                    }
                }
                mat = mat.NextMatch();
            }

            foreach (string viewName in tViewContainers)
            {
                string tPattern = string.Format(PAT_VIEWCONTAINER_REF, viewName);
                //处理下多继承，多个的情况
                mat = Regex.Match(sFileContent, tPattern);
                if (mat != null && mat.Success)
                {
                    string name = mat.Groups[1].Value;
                    if (tView2Path.ContainsKey(name))
                    {
                        string viewPath = tView2Path[name];
                        if (!hitViewPath.Contains(viewPath))//替换一次就够了
                        {
                            //替换BaseView to BaseView
                            hitViewPath.Add(viewPath);
                            path2Content[viewPath] = ReplaceView(name, path2Content[viewPath]);
                        }
                    }
                    //检测到一个继承MonoViewController就可跳出
                    break;
                }
            }
        }

        string sLog = "";
        sLog += @"必须确保所有的view都被转换完毕，然后修改BaseView，去掉继承MonoBehaviour
把GetMissingComponent|AddMissingComponent|AddComponent|GetComponent<XXXView>()及GameObjectExt.GetMissingComponent(gameObject)
都替换成 new XXXView();
即使同一个view多个GetComponent<XXXView>(),一样转换为new XXXView()，无报错影响。
转换完cs并保存后需要手动处理的：
    BaseView改为不再继承MonoBehaviour,类部分替换为以下代码：
public abstract class BaseView
{
    public GameObject gameObject;
    #region 兼容旧版本
    public Transform transform
    {
        get { return gameObject.transform; }
    }

    public T GetComponent<T>()
    {
        return gameObject.GetComponent<T>();
    }
    #endregion

    public virtual void Setup(Transform root) { gameObject = root.gameObject; }
}

    MonoViewController及其子类(此类看项目)中诸如AddMissingComponent<T>()的操作改为new T();
    其它：查看编辑器报错
    查看以下未处理的警告
";

        sLog += "=============警告-未替换的View-请检查确认是否被引用================begin" + System.Environment.NewLine;
        foreach (string viewName in tView2Path.Keys)
        {
            if (!hitViewPath.Contains(tView2Path[viewName]))
            {
                sLog += viewName + System.Environment.NewLine;
            }
        }
        sLog += "===============警告-未替换的View==================end" + System.Environment.NewLine + System.Environment.NewLine;
        Debug.Log("修改controller的个数为:" + hitPath2Views.Keys.Count);


        //已下是写log
        string sResult1 = "";
        string sResult2 = "";
        foreach (string viewPath in hitViewPath)
        {
            string changeView = string.Empty;
            foreach (string viewName in tView2Path.Keys)
            {
                if (tView2Path[viewName].Equals(viewPath))
                {
                    changeView = viewName;
                }
            }

            string sTemp = "";
            int count = 0;
            foreach (var refName in hitPath2Views.Keys)
            {
                if (hitPath2Views[refName].Contains(changeView))
                {
                    sTemp += string.Format("    {0}{1}", refName, System.Environment.NewLine);
                    ++count;
                }
            }
            if (count == 0)
            {
                sResult1 += changeView + " 替换结果:" + System.Environment.NewLine;
                sResult1 += "    未替换对应的AddMissingComponent|.|.|.,被MonoViewController或其子类引用了,转换后请更改MonoViewController" + System.Environment.NewLine;
            }
            else
            {
                sResult2 += changeView + " 替换结果:" + System.Environment.NewLine + sTemp;
            }
        }
        sLog += sResult1 + sResult2;

        File.WriteAllText(CHECK_VIEW_LOG, sLog);

        if (EditorUtility.DisplayDialog("检测完成", string.Format("Log记录在{0}文件中（请务必查看，log已保持，可以先查看log再确定是否保存cs）。确认是否把转换的cs保存到文件", CHECK_VIEW_LOG), "确认", "取消"))
        {
            foreach (string path in path2Content.Keys)
            {
                if (hitViewPath.Contains(path) ||
                    hitPath2Views.ContainsKey(path))
                {
                    File.WriteAllText(path, path2Content[path]);
                }
            }
            EditorUtility.DisplayDialog("确认", "保存完毕，请修改BaseView及onoViewController文件", "确认");
        }
    }

    private string ReplaceView(string viewName, string sContent)
    {

        return Regex.Replace(sContent,
            @"public\s+void\s+Setup\s*\(Transform root\)[\n\s]*\{",
            @"protected override void InitElementBinding ()
    {
        var root = this.gameObject.transform;"
        );
        //不替换，手动修改BaseView代码
        /*
        return Regex.Replace(sContent,
                        @"public\s+class\s+(\S+)\s+\:\s+BaseView",
                        string.Format("public class {0} : BaseView", viewName)
                        );
                        */
    }

    //一次性替换所有BaseView，临时代码,已废弃
    //已经在处理controller时替换
    private void ReplaceAllView()
    {
        Dictionary<string, string> tView2Path = new Dictionary<string, string>();
        string[] fileNameList = Directory.GetFiles(_scriptRoot, "*.cs", SearchOption.AllDirectories);
        foreach (string filename in fileNameList)
        {
            string sFileContent = File.ReadAllText(filename);
            //View检测
            Match mat = Regex.Match(sFileContent, PAT_VIEW);
            if (mat != null && mat.Success)
            {
                if (sFileContent.Contains("base.Setup(root)"))
                {
                    continue;
                }

                string name = mat.Groups[1].Value;
                if (tView2Path.ContainsKey(name) && tView2Path[name] == filename)
                {
                    Debug.LogError("警告，发现同名view" + name);
                    continue;
                }
                tView2Path[name] = filename;
                sFileContent = ReplaceView(name, sFileContent);
                File.WriteAllText(filename, sFileContent);
            }
        }
    }

    private List<string> GetSuperClass(Dictionary<string, string> clsDct, List<string> baseList)
    {
        List<string> resultList = new List<string>();
        foreach (string superCls in clsDct.Keys)
        {
            foreach (string baseCls in baseList)
            {
                if (clsDct[superCls].Equals(baseCls))
                {
                    resultList.Add(superCls);
                    break;
                }
            }
        }
        return resultList;
    }

    #endregion

    public static void UpdateProxyManager(string pModuleName = KEY_WORD_MODULE_NAME, string pAuthor = KEY_WORD_AUTHOR, string pCreateTime = KEY_WORD_CREATED_TIME)
    {
        UpdateFileCopyCodeBetweenKeyWord(VIEWCODE_PATH_PROXY_MANAGER, VIEWCODE_PREFIX_IN_CODE, string.Format(" Proxy{0}Module ", pModuleName), pModuleName, pAuthor, pCreateTime);
    }

    public static void UpdateModelManager(string pModuleName = KEY_WORD_MODULE_NAME, string pAuthor = KEY_WORD_AUTHOR, string pCreateTime = KEY_WORD_CREATED_TIME)
    {
        UpdateFileCopyCodeBetweenKeyWord(VIEWCODE_PATH_MODEL_MANAGER, VIEWCODE_PREFIX_IN_CODE, string.Format(" {0}Model ", pModuleName), pModuleName, pAuthor, pCreateTime);
    }

    //检索文档中模板代码段，复制之并替换模块名字为目标名字
    private static void UpdateFileCopyCodeBetweenKeyWord(string pFilePath, string pPrefixKeyWord, string pInValidKeyWord = "",
                                                         string pModuleName = KEY_WORD_MODULE_NAME, string pAuthor = KEY_WORD_AUTHOR, string pCreateTime = KEY_WORD_CREATED_TIME)
    {
        if (string.IsNullOrEmpty(pModuleName) || pModuleName == KEY_WORD_MODULE_NAME)
            return;
        if (!string.IsNullOrEmpty(pFilePath) && File.Exists(pFilePath))
        {
            string tFileContent = File.ReadAllText(pFilePath);
            if (tFileContent.IndexOf(pInValidKeyWord) != -1)
            {
                GameDebuger.Log(string.Format("文件更新取消，存在禁止关键词，pModuleName：{0}，pInValidKeyWord：{1}", pModuleName, pInValidKeyWord));
                return;
            }
            UpdateFileContentCopyCodeBetweenKeyWord(ref tFileContent, pPrefixKeyWord, 0, pModuleName, pAuthor, pCreateTime);
            File.WriteAllText(pFilePath, tFileContent);
        }
    }

    private static void UpdateFileContentCopyCodeBetweenKeyWord(ref string pFileContent, string pPrefixKeyWord, int pStartMatchIndex = 0,
                                                                string pModuleName = KEY_WORD_MODULE_NAME, string pAuthor = KEY_WORD_AUTHOR, string pCreateTime = KEY_WORD_CREATED_TIME)
    {
        if (!string.IsNullOrEmpty(pFileContent))
        {
            MatchCollection tMatchCollection = Regex.Matches(pFileContent, "(#region " + pPrefixKeyWord + ")[^#]+?(#endregion)\n\n\t");
            if (null != tMatchCollection)
            {
                int tLeftMatchCount = tMatchCollection.Count;
                if (tLeftMatchCount > 0)
                {
                    int tCurMatchIndex = 0;
                    string tContentToCopy = string.Empty;
                    foreach (Match tMatch in tMatchCollection)
                    {
                        tCurMatchIndex += 1;
                        if (tCurMatchIndex > pStartMatchIndex)
                        {
                            pStartMatchIndex += 1;
                            tContentToCopy = tMatch.Value;
                            if (pModuleName != KEY_WORD_MODULE_NAME)
                                tContentToCopy = tContentToCopy.Replace(KEY_WORD_MODULE_NAME, pModuleName);
                            if (pAuthor != KEY_WORD_AUTHOR)
                                tContentToCopy = tContentToCopy.Replace(KEY_WORD_AUTHOR, pAuthor);
                            if (pCreateTime != KEY_WORD_CREATED_TIME)
                                tContentToCopy = tContentToCopy.Replace(KEY_WORD_CREATED_TIME, pCreateTime);
                            tContentToCopy = tContentToCopy.Replace(pPrefixKeyWord, pModuleName + "Module");
                            CopyAndInsert(ref pFileContent, tContentToCopy, tMatch.Index);
                            UpdateFileContentCopyCodeBetweenKeyWord(ref pFileContent, pPrefixKeyWord, pStartMatchIndex, pModuleName, pAuthor, pCreateTime);
                            break;
                        }
                    }
                }
            }
        }
    }

    private static void CopyAndInsert(ref string pFileContent, string pContentToCopy, int pIndexToInsert)
    {
        if (!string.IsNullOrEmpty(pFileContent) && pFileContent.Length > pIndexToInsert && !string.IsNullOrEmpty(pContentToCopy))
            pFileContent = pFileContent.Insert(pIndexToInsert, pContentToCopy);
    }
}
