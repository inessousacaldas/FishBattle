using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using CodeGeneration;
using System.Text.RegularExpressions;
using Object = UnityEngine.Object;

public struct GenParams
{
    public string author;
    public string uiViewName;
    public string moduleName;
    public string generatePath;
    public string dataClassName
    {
        get { return moduleName + "Data"; }
    }
}

public class FRPCodeGenerator : EditorWindow
{
    private enum ControllerType
    {
        IsFRPBaseViewController = 1
        , IsMonoLessViewController = 2
        , IsMonoViewController = 3
    }

    private const string VIEWCODE_GENERATED_PATH = "Assets/Scripts/MyGameScripts/Module/";

    #region const for relate file generator

    private const string DefaultPath = "AutoGen";
    private const string KEY_WORD_MODULE_NAME = "_ModuleName_";
    private const string KEY_WORD_AUTHOR = "_Author_";
    private const string KEY_WORD_CREATED_TIME = "_CreatedTime_";
    private const string KEY_WORD_EVENT_COMPONENT = "_ComponentName_";
    private const string VIEWCODE_GENERATED_PATH_CONTROLLER = "Controller/" + KEY_WORD_MODULE_NAME + "Controller.cs";
    private const string VIEWCODE_GENERATED_PATH_VIEW = "View/" + KEY_WORD_MODULE_NAME + "View.cs";
    private const string VIEWCODE_GENERATED_PATH_MODEL = "Model/" + KEY_WORD_MODULE_NAME + "Model.cs";
    private const string VIEWCODE_GENERATED_PATH_NetMsg = "Model/" + KEY_WORD_MODULE_NAME + "NetMsg.cs";
    private const string VIEWCODE_GENERATED_PATH_HELPER = "Model/" + KEY_WORD_MODULE_NAME + "Helper.cs";

    private const string VIEWCODE_GENERATED_PATH_VIEW_FRP = "View/" + KEY_WORD_MODULE_NAME + "ViewAutoGen.txt";

    #endregion

    private const string VIEW_CACHE_PATH = "Assets/Editor/UIViewGenerator/Cache/";

    private const string VIEWCODE_PATH_MODEL_MANAGER =
        "Assets/Scripts/MyGameScripts/Manager/BusinessRelated/ModelManager.cs";

    private const string VIEWCODE_PATH_PROXY_MANAGER =
        "Assets/Scripts/MyGameScripts/Manager/BusinessRelated/ProxyManager.cs";

    private const string VIEWCODE_EVENT_REGISTER = "\t\tEventDelegate.Set (_view." + KEY_WORD_EVENT_COMPONENT +
                                                   ".onClick, OnClick" + KEY_WORD_EVENT_COMPONENT + "Handler);\n";

    private const string VIEWCODE_EVENT_HANDLER =
        "\tprivate void OnClick" + KEY_WORD_EVENT_COMPONENT + "Handler (){\n\t}\n\n";

    [MenuItem("Window/FRPCodeGenerator")]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow<FRPCodeGenerator>(false, "FRPCodeGenerator", true);
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

    private string _modulePath = "";
    private bool exportDicChanged = true;

    private Vector2 exportViewPos;
    private Dictionary<string, UIComponentInfo> _exportInfoDic = new Dictionary<string, UIComponentInfo>();
    private Dictionary<string, bool> _validatedInfoDic = new Dictionary<string, bool>();
    private List<string> _deleteUIDList = new List<string>();

    #endregion

    #region Field and property for relate file generator

    private int selectControllerType = (int)ControllerType.IsFRPBaseViewController;
    private readonly int[] types =
    {
        (int)ControllerType.IsFRPBaseViewController
        ,(int)ControllerType.IsMonoLessViewController
        , (int)ControllerType.IsMonoViewController
    };
    private readonly string[] controllerNames = { "IsFRPBaseViewController", "IsMonoLessViewController", "IsMonoViewController" };

    private bool mGenerateController = false;
    private bool mgenerateView = false;

    private bool mGenerateModel = false;
    private bool mGenerateNetMsg = false;
    private bool mgenerateHelper = false;

    private bool mGenerateInfo = true;

#pragma warning disable
    private string mModuleName = string.Empty;


    //Current module to create , sometime it's not the module dir
    private string mAuthor = Environment.UserName;
    private string mCreateTime = string.Empty;
    private string mModuleFileDir = VIEWCODE_GENERATED_PATH;


    private static string _scriptRoot = "Assets/Scripts/MyGameScripts";

    public static string ScriptRoot
    {
        set
        {
            // 只允许往上一层设置，减少冲突
            if (_scriptRoot.Contains(value))
            {
                _scriptRoot = value;
            }
        }
    }

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

        DrawTitleGroup();

        DrawRelateFileView();

        EditorGUILayout.BeginHorizontal();

        //SelectionComponentView
        if (UIPrefabRoot)
            DrawSelectionComponentView();

        EditorGUILayout.BeginVertical(); //Right Part Vertical Begin

        if (UIPrefabRoot)
        {
            //ComponentInfoView
            DrawComponentInfoView();

        }
        //ExportListView
        DrawExportListView();

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
        var bSave = false;
        var strs = Directory.GetFiles(_scriptRoot, "*.cs", SearchOption.AllDirectories);
        foreach (var filename in strs)
        {
            var sFileContent = File.ReadAllText(filename);
            const string sPattern = @"\s*public\s+\S*\s*\S*\s*class\s+(\S+)\s*\:\s*BaseView[\n\s]+";
            var mat = Regex.Match(sFileContent, sPattern);

            if (!mat.Success)
            {
                mat = Regex.Match(sFileContent, @"\s*public\s+\S*\s*\S*\s*class\s+(\S+)\s*\:\s*FRPBaseView");
            }
            if (mat != null && mat.Success)
            {
                var name = mat.Groups[1].Value;
                var path = filename.Replace("Assets", "").Replace("\\", "/");
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
        GameDebuger.Log(sFileContent);
        foreach (var str in strs)
        {
            string[] list = str.Split(' ');
            GameDebuger.Log(str + "\n");
            if (list.Length != 2)
            {
                continue;
            }

            view2ScriptPath[list[0]] = list[1].Replace("\\", "/");
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
        path = path.Replace("\\", "/");

        if (view2ScriptPath.ContainsKey(name) && view2ScriptPath[name] == path)
        {
            return;
        }
        var dataPath = Application.dataPath;
        if (path.Contains(dataPath))
        {
            path = path.Substring(dataPath.Length);
        }
        view2ScriptPath[name] = path;
        SavePathList();
    }

    private string GetPathNameFromConfig()
    {
        if (UIPrefabRoot == null)
        {
            return string.Empty;
        }

        var cfgpath = string.Empty;
        view2ScriptPath.TryGetValue(UIPrefabRoot.name, out cfgpath);
        if (string.IsNullOrEmpty(cfgpath))
            return cfgpath;

        var _path = Application.dataPath;

        if (cfgpath.Contains("AutoGen/"))
        {
            var index = _path.IndexOf("Assets");
            if (index > -1)
                _path = _path.Substring(0, _path.Length - 6);
        }

        _path = _path + cfgpath;
        return _path;
    }

    public void LoadUIViewByScript()
    {
        var _path = GetPathNameFromConfig();

        if (string.IsNullOrEmpty(_path) || !File.Exists(_path))
            return;

        var sFileContent = File.ReadAllText(_path);
        if (string.IsNullOrEmpty(sFileContent))
            return;

        var contentMat = Regex.Match(sFileContent, @"protected override void InitElementBinding\s*\(\)[\n\s]*\{[\n\s]*([\n\s\S]+?)[\n\s]*\}");
        if (!contentMat.Success)
        {
            Debug.LogError("匹配出错！");
            return;
        }

        var sContent = contentMat.Groups[1].Value;
        var sLines = sContent.Split('\n');
        var pat1 = @"(\S+) = root\.FindScript<(\S+)>\(""(\S+)?""\)";
        var pat2 = @"(\S+) = root\.FindTrans\(""(\S+)?""\);";
        var pat3 = @"(\S+) = root\.FindGameObject\(""(\S+)?""\);";

        var pat4 = @"(\S+) = root\.GetComponent<(\S+)>\(\)";
        var pat5 = @"(\S+) = root\.Find\(""(\S+)""\)(\S*);";
        var comPattern = @"\.GetComponent<(\S+)>\(\)";

        Match mat;
        foreach (string sLine in sLines)
        {
            if (string.IsNullOrEmpty(sLine)
                || sLine.Contains("var root = this.gameObject"))
                continue;

            var name = string.Empty;
            var type = string.Empty;
            var path = string.Empty;

            mat = Regex.Match(sLine, pat1);
            if (mat.Success)
            {
                name = mat.Groups[1].Value;
                type = mat.Groups[2].Value;
                path = mat.Groups[3].Value;
            }
            else if ((mat = Regex.Match(sLine, pat2)).Success)
            {
                name = mat.Groups[1].Value;
                path = mat.Groups[2].Value;
                type = "Transform";
            }

            else if ((mat = Regex.Match(sLine, pat3)).Success)
            {

                name = mat.Groups[1].Value;
                path = mat.Groups[2].Value;
                type = "GameObject";
            }
            else if ((mat = Regex.Match(sLine, pat4)).Success)
            {
                name = mat.Groups[1].Value;
                type = mat.Groups[2].Value;
                path = "";
            }
            else if ((mat = Regex.Match(sLine, pat5)).Success)
            {
                name = mat.Groups[1].Value;
                path = mat.Groups[2].Value;

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
                    var comMat = Regex.Match(sLine, comPattern);
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
            else
            {
                GameDebuger.LogError(" 匹配出错：" + sLine);
                continue;
            }

            if (path.Contains(" "))
            {
                GameDebuger.LogError(string.Format("路径中有空格！path：{0}", path));
                path = path.Replace(" ", string.Empty);
            }
            if (string.IsNullOrEmpty(name)) continue;

            var item = new UIComponentInfo(UIPrefabRoot, name, path, type);
            if (!_exportInfoDic.ContainsKey(item.uid))
            {
                _exportInfoDic.Add(item.uid, item);
            }
            var itemTrans = UIPrefabRoot.Find(item.path);
            _validatedInfoDic.Add(item.uid, (itemTrans != null));
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

    //保存UIViewCache信息
    UIViewCache SaveUIViewCache()
    {
        UIViewCache record = ScriptableObject.CreateInstance<UIViewCache>();
        record.codePath = _codeGeneratePath;

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
        EditorHelper.DrawBoxLabel("C#\nGenertePath:", _modulePath == "" ? "NONE" : _modulePath, false, true, 300);
        if (GUILayout.Button("Change\nModule Path", GUILayout.Width(60f), GUILayout.Height(45f)))
        {
            _modulePath = EditorUtility.SaveFolderPanel("Please Select Module Directory To Save", VIEWCODE_GENERATED_PATH, "");
            if (string.IsNullOrEmpty(_modulePath))
            {
                _modulePath = DefaultPath;
            }
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
                var comList = Selection.activeTransform.GetComponents<Component>();
                foreach (Component com in comList)
                {

                    if (validateCode == ROOT_NODE && com.GetType().Name == "Transform")
                        continue;

                    if (GUILayout.Button(com.GetType().Name))
                    {
                        var newInfo = new UIComponentInfo(com, UIPrefabRoot);
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
            if (relateFileCreateEnable)
            {
                var comList = new List<UIComponentInfo>(_exportInfoDic.Values);

                if (!ProduceCSharpCode(comList))
                {
                    ShowNotification(new GUIContent("Code generate Failure"));

                }
            }

        }

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
        _curComInfo = null;
        exportDicChanged = true;
        _deleteUIDList.Clear();
        _exportInfoDic.Clear();
        _validatedInfoDic.Clear();
        ClearRelateInfo();
    }

    public bool ProduceCSharpCode(List<UIComponentInfo> componentList)
    {

        if (string.IsNullOrEmpty(_modulePath))
        {
            _modulePath = EditorUtility.SaveFolderPanel("Please Select Module Directory To Save", VIEWCODE_GENERATED_PATH, "");
        }
        if (string.IsNullOrEmpty(_modulePath))
        {
            _modulePath = DefaultPath;
        }

        try
        {
            var name = UIPrefabRoot == null ? string.Empty : UIPrefabRoot.name;
            var genParams = new GenParams()
            {
                author = mAuthor,
                uiViewName = name,
                moduleName = mModuleName,
                generatePath = _modulePath,
            };

            if (mgenerateView)
            {
                if (UIPrefabRoot == null)
                {
                    ShowNotification(new GUIContent("请放入一个PrefabInstance"));
                    return false;
                }

                FRPCodeGeneratorHelper.GenerateUICodeAutoGen(componentList, genParams);

                var viewPath = _modulePath + "/View/" + genParams.uiViewName + "AutoGen.cs";

                UpdatePateList(genParams.uiViewName, viewPath);
            }

            if (mGenerateController)
            {
                if (UIPrefabRoot == null)
                {
                    ShowNotification(new GUIContent("请放入一个PrefabInstance"));
                    return false;
                }
                switch ((ControllerType)selectControllerType)
                {
                    case ControllerType.IsFRPBaseViewController:
                        FRPCodeGeneratorHelper.GenerateFRPControllerAutoGen(componentList, genParams);
                        FRPCodeGeneratorHelper.GenerateFRPController(componentList, genParams);
                        FRPCodeGeneratorHelper.GenerateFRPLogic(componentList, genParams);
                        break;
                    case ControllerType.IsMonoLessViewController:
                        FRPCodeGeneratorHelper.GenerateMonolessController(genParams);
                        FRPCodeGeneratorHelper.GenerateMonolessControllerAutoGen(componentList, genParams);
                        break;
                    case ControllerType.IsMonoViewController:
                        FRPCodeGeneratorHelper.GenerateMonoViewControllerAutoGen(componentList, genParams);
                        FRPCodeGeneratorHelper.GenerateMonoViewController(componentList, genParams);
                        FRPCodeGeneratorHelper.GenerateLogic(componentList, genParams);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (mGenerateModel)
            {
                FRPCodeGeneratorHelper.GenerateModule(genParams);
                FRPCodeGeneratorHelper.GenerateModuleData(genParams);
                FRPCodeGeneratorHelper.GenerateProxy(genParams);
            }
            if (mGenerateNetMsg)
            {
                FRPCodeGeneratorHelper.GenerateNetMsg(genParams);
            }
            if (mgenerateHelper)
            {
                FRPCodeGeneratorHelper.GenerateHelper(genParams);
            }

            ShowNotification(new GUIContent("Success 生成C#代码完毕"));
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

            ShowNotification(new GUIContent("Success 生成C#代码完毕"));
            // Refresh assets.
            //          AssetDatabase.Refresh ();
        }
        catch (System.Exception e)
        {
            Debug.Log("An error occurred while saving file: " + e);
        }
    }

    #region View 保存方法 Begin


    #endregion

    #region Relate file generator ,such as proxy,controller and model,etc.

    private void DrawRelateFileView()
    {
        selectControllerType = EditorGUILayout.IntPopup(
            "Controller Class Type:"
            , selectControllerType
            , controllerNames
            , types);
        GUILayout.BeginHorizontal();
        GUILayout.Label("DataName:", GUILayout.Width(150));
        GUILayout.EndHorizontal();

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

        mgenerateView = GUILayout.Toggle(mgenerateView, new GUIContent("Generate View : " + VIEWCODE_GENERATED_PATH_VIEW.Replace(KEY_WORD_MODULE_NAME, mModuleName)));
        mGenerateController = GUILayout.Toggle(mGenerateController, new GUIContent("Generate Controller : " + VIEWCODE_GENERATED_PATH_CONTROLLER.Replace(KEY_WORD_MODULE_NAME, mModuleName)));
        mGenerateModel = GUILayout.Toggle(mGenerateModel, new GUIContent("Generate Model : " + VIEWCODE_GENERATED_PATH_MODEL.Replace(KEY_WORD_MODULE_NAME, mModuleName)));
        mGenerateNetMsg = GUILayout.Toggle(mGenerateNetMsg, new GUIContent("Generate NetMsg : " + VIEWCODE_GENERATED_PATH_NetMsg.Replace(KEY_WORD_MODULE_NAME, mModuleName)));
        mgenerateHelper = GUILayout.Toggle(mgenerateHelper, new GUIContent("Generate ModuleHelper : " + VIEWCODE_GENERATED_PATH_HELPER.Replace(KEY_WORD_MODULE_NAME, mModuleName)));

    }

    private bool relateFileCreateEnable
    {
        get
        {
            var tNotification = string.Empty;
            if (string.IsNullOrEmpty(mModuleName)
                && (((ControllerType)selectControllerType == ControllerType.IsFRPBaseViewController
                     && mGenerateController)
                || mGenerateNetMsg
                || mGenerateModel
                || mgenerateHelper))
            {
                tNotification = "Set the module name first!";
            }

            if (string.IsNullOrEmpty(tNotification))
                return true;
            ShowNotification(new GUIContent(tNotification));
            return false;
        }
    }

    private void UpdateModuleFileDir()
    {
        var tTempFilePath = "Assets" + _codeGeneratePath;
        if (!string.IsNullOrEmpty(tTempFilePath) && File.Exists(tTempFilePath))
            mModuleFileDir = Directory.GetParent(tTempFilePath).FullName;//dir of the module dir , sometime it's not same with the view code...
        mModuleFileDir = EditorUtility.SaveFolderPanel("Please Select Module Directory To Save", mModuleFileDir, mModuleFileDir);
        if (string.IsNullOrEmpty(mModuleFileDir))
            mModuleFileDir = VIEWCODE_GENERATED_PATH;
    }

    private string GenerateFileByCopyFromTempDirToModuleDir(string pTemplateFileName)
    {
        var tFilePath = string.Empty;
        if (string.IsNullOrEmpty(mModuleFileDir))
        {
            GameDebuger.LogWarning(string.Format("GenerateFileByCopyFromTempDirToModuleDir failed for mModuleFileDir is null, pTemplateFileName:{0}", pTemplateFileName));
        }
        else
        {
            var tTemplateFilePath = string.Format("{0}{1}Module/{2}", Refactor.RefactorConst.PROJECT_PATH + VIEWCODE_GENERATED_PATH, KEY_WORD_MODULE_NAME, pTemplateFileName);
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
        mgenerateView = false;
        mGenerateController = false;
        mGenerateModel = false;
        mGenerateNetMsg = false;
        selectControllerType = (int)ControllerType.IsFRPBaseViewController;
        mModuleName = string.Empty;//Current module to create , sometime it's not the module dir
        mAuthor = Environment.UserName;
        mCreateTime = string.Empty;
        mModuleFileDir = VIEWCODE_GENERATED_PATH;
    }

    #endregion

    #region 界面事件监听的自动生成


    private List<string> GetBtnComponentList(bool pNeedTip = true)
    {
        ;
        List<string> tBtnComponentList = null;
        if (null != _exportInfoDic && _exportInfoDic.Count > 0)
        {
            var tEnum = _exportInfoDic.GetEnumerator();
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

    private string ReplaceView(string viewName, string sContent)
    {

        return Regex.Replace(sContent,
            @"public\s+void\s+Setup\s*\(Transform root\)[\n\s]*\{",
            @"protected override void InitElementBinding ()
    {
        var root = this.gameObject.transform;"
        );

    }

    #endregion

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
