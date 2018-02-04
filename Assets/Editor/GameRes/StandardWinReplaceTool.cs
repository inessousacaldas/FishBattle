using System.Collections.Generic;
using System.Security.Policy;
using UnityEditor;
using UnityEngine;

public class StandardWinReplaceTool : EditorWindow
{
    public static StandardWinReplaceTool instance;

    // 窗口处理
    private GameObject _baseWindow;

    // 文字处理
    private List<UILabel> _lbls;
    private Dictionary<string, Color> _lblcolorMap;
    private Dictionary<string, Color> _lblcolorReplaceMap;

    #region 按钮处理

    /// <summary>
    /// 按钮处理根据 BaseButtonReplace.prefab and BaseSmallButtonReplace.prefab
    /// OldGroup(_oldBaseBnts) <-> NewGroup(_newBaseBtns) 的对应关系为:
    /// 1 对 N : 根据 1个 旧按钮的 Sprite 大小,替换为 N个 不同 Sprite 的新按钮
    /// N 对 1 : 把 N 个不同 Sprite 的旧按钮,替换为 1个 Sprite 的新按钮
    /// N 对 N : 新旧按钮一一对应替换
    /// </summary>
    private List<UIBtnContainer> _oldBaseBnts;
    private List<UIBtnContainer> _newBaseBtns;

    private UIButton _baseSmallBtn;
    private UILabel _baseSmallBtnLbl;
    #endregion

    private Transform _dragItem;
    private Transform _replaceWindow;

    private List<string> _childNameList = new List<string> { "WinBg", "CloseBtn", "TitleBg" , "TitleNameSprite", "WinTabGroup" };
    private Dictionary<string, string> _replaceDic = new Dictionary<string, string>();

    private string _titleNameSprite;
    private string _winTabGroup;

    private string _winBgName = "WinBg";
    private string _winBgSprite = "window_bg";
    private string _winTabGroupName = "WinTabGroup";

    private string _titleBgSprite = "window_title_under";
    private string _titleBgName = "TitleBg";
    private string _titleName = "TitleNameSprite";

    [MenuItem("GameResource/StandardWinReplaceTool #&R")]
    public static void ShowWindow()
    {
        if (StandardWinReplaceTool.instance == null)
        {
            StandardWinReplaceTool window = EditorWindow.GetWindow<StandardWinReplaceTool>(false,"StandardWinReplaceTool", true);
            window.minSize = new Vector2(400f, 500f);
            window.Init();
            window.Show();
        }
        else
        {
            EditorUtility.UnloadUnusedAssetsImmediate();
            System.GC.Collect();
            StandardWinReplaceTool.instance.Close();
        }
    }

    private void OnEnable()
    {
        StandardWinReplaceTool.instance = this;
    }

    private void OnDisable()
    {
        StandardWinReplaceTool.instance = null;
    }

    private void Init()
    {
        _baseWindow = AssetDatabase.LoadMainAssetAtPath("Assets/UI/Prefabs/BaseUI/BaseWindow.prefab") as GameObject;
        GameObject baseBtnObj = AssetDatabase.LoadMainAssetAtPath("Assets/UI/Prefabs/BaseUI/BaseButtonReplace.prefab") as GameObject;
        if (baseBtnObj != null)
        {
            _oldBaseBnts = new List<UIBtnContainer>();
            _newBaseBtns = new List<UIBtnContainer>();
            GameObject OldGroup = baseBtnObj.transform.Find("OldGroup").gameObject;
            GameObject NewGroup = baseBtnObj.transform.Find("NewGroup").gameObject;

            UIButton[] originals = OldGroup.GetComponentsInChildren<UIButton>();
            UIButton[] replaces = NewGroup.GetComponentsInChildren<UIButton>();

            if (originals.Length != replaces.Length)
            {
                if (originals.Length != 1 && replaces.Length != 1)
                {
                    Debug.LogError("BaseButtonReplace.prefab 原和替换 UIButton 数量不符合规范");
                    return;
                }
            }

            foreach (UIButton original in originals)
            {
                UIBtnContainer btnContainer = new UIBtnContainer();
                btnContainer.btn = original;
                btnContainer.lbl = original.GetComponentInChildren<UILabel>();
                btnContainer.sp = original.GetComponent<UISprite>();
                _oldBaseBnts.Add(btnContainer);
            }

            foreach (UIButton replace in replaces)
            {
                UIBtnContainer btnContainer = new UIBtnContainer();
                btnContainer.btn = replace;
                btnContainer.lbl = replace.GetComponentInChildren<UILabel>();
                btnContainer.sp = replace.GetComponent<UISprite>();
                _newBaseBtns.Add(btnContainer);
            }
        }

        GameObject baseSmallBtnObj = AssetDatabase.LoadMainAssetAtPath("Assets/UI/Prefabs/BaseUI/BaseSmallButton.prefab") as GameObject;
        if (baseSmallBtnObj != null)
        {
            _baseSmallBtn = baseSmallBtnObj.GetComponent<UIButton>();
            _baseSmallBtnLbl = _baseSmallBtn.transform.Find("Label").GetComponent<UILabel>();
        }

        _lbls = new List<UILabel>();
        _lblcolorMap = new Dictionary<string, Color>();
        _lblcolorReplaceMap = new Dictionary<string, Color>();
    }

    private bool ReplaceBaseBtn(UIButton btn)
    {
        bool result = false;
        if (_oldBaseBnts.Count != _newBaseBtns.Count)
        {
            if (_newBaseBtns.Count == 1)
            {
                foreach (UIBtnContainer oldBtn in _oldBaseBnts)
                {
                    if (oldBtn.btn.normalSprite == btn.normalSprite)
                    {
                        UIBtnContainer newBtn = _newBaseBtns[0];
                        ReplaceBaseBtnHandle(btn, newBtn);
                        result = true;
                        break;
                    }
                }
            }
            else if (_oldBaseBnts.Count == 1)
            {
                UIBtnContainer oldBtn = _oldBaseBnts[0];
                if (oldBtn.btn.normalSprite == btn.normalSprite)
                {
                    // 新btn list 选择1个,图片宽度匹配
                    int newIndex = 0;
                    int sw = btn.sprite.width;
                    int d = 1000000;
                    for (int index = 0; index < _newBaseBtns.Count; index++)
                    {
                        UIBtnContainer nb = _newBaseBtns[index];
                        int w = Mathf.Abs(sw - nb.sp.width);
                        if (w < d)
                        {
                            newIndex = index;
                            d = w;
                        }
                    }

                    UIBtnContainer newBtn = _newBaseBtns[newIndex];
                    ReplaceBaseBtnHandle(btn, newBtn);
                    result = true;
                }
            }
        }
        else
        {
            for (int index = 0; index < _oldBaseBnts.Count; index++)
            {
                UIBtnContainer oldBtn = _oldBaseBnts[index];
                if (oldBtn.btn.normalSprite == btn.normalSprite)
                {
                    UIBtnContainer newBtn = _newBaseBtns[index];
                    ReplaceBaseBtnHandle(btn, newBtn);
                    result = true;
                    break;
                }
            }
        }

        return result;
    }

    private void ReplaceBaseBtnHandle(UIButton btn, UIBtnContainer newBtn)
    {
        btn.normalSprite = newBtn.btn.normalSprite;
        btn.sprite.width = newBtn.sp.width;
        btn.sprite.height = newBtn.sp.height;

        foreach (Transform child in btn.transform)
        {
            UILabel lbl = child.gameObject.GetComponent<UILabel>();
            if (lbl != null)
            {
                lbl.applyGradient = false;
                lbl.fontSize = newBtn.lbl.fontSize;
                lbl.color = newBtn.lbl.color;
                lbl.effectStyle = newBtn.lbl.effectStyle;
                lbl.effectColor = newBtn.lbl.effectColor;
            }
        }
    }


    private bool _isStdWin;

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("BaseWindow", GUILayout.Height(50f)))
        {
            _baseWindow = AssetDatabase.LoadMainAssetAtPath("Assets/UI/Prefabs/BaseUI/BaseWindow.prefab") as GameObject;
            _replaceDic.Clear();
            _titleNameSprite = "";
            _isStdWin = true;
        }

        if (GUILayout.Button("BaseTabWindow", GUILayout.Height(50f)))
        {
            _baseWindow = AssetDatabase.LoadMainAssetAtPath("Assets/UI/Prefabs/BaseUI/BaseTabWindow.prefab") as GameObject;
            _replaceDic.Clear();
            _titleNameSprite = "";
            _isStdWin = true;
        }

        if (GUILayout.Button("非标准窗口替换", GUILayout.Height(50f)))
        {
            _baseWindow = AssetDatabase.LoadMainAssetAtPath("Assets/UI/Prefabs/BaseUI/BaseTabWindow.prefab") as GameObject;
            _replaceDic.Clear();
            _titleNameSprite = "";
            _isStdWin = false;
        }
        EditorGUILayout.EndHorizontal();

        if (_isStdWin)
        {
            EditorGUILayout.BeginVertical("HelpBox", GUILayout.Width(550f));
            {
                GUILayout.Label("标准窗口替换", "BoldLabel");
                EditorGUILayout.Space();
                _baseWindow =
                    (GameObject) EditorGUILayout.ObjectField("BaseWindow:", _baseWindow, typeof (GameObject), false);

                //_replaceWindow = (GameObject)EditorGUILayout.ObjectField("ReplaceWindow:", _replaceWindow, typeof(GameObject), false);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(@"替换窗口", GUILayout.Width(60f));
                //_dragItem = EditorGUILayout.ObjectField("ReplaceWindow", _dragItem, typeof(Transform), true, GUILayout.ExpandWidth(false), GUILayout.Width(140f)) as Transform;
                _dragItem =
                    EditorGUILayout.ObjectField(_dragItem, typeof (Transform), true, GUILayout.ExpandWidth(false),
                        GUILayout.Width(180f)) as Transform;
                if (_dragItem != null)
                {
                    if (_dragItem != _replaceWindow)
                    {
                        if (PrefabInstanceCheck(_dragItem))
                        {
                            _dragItem = PrefabUtility.FindPrefabRoot(_dragItem.gameObject).transform;
                            if (_replaceWindow != _dragItem)
                            {
                                _replaceWindow = _dragItem;
                                this.RemoveNotification();
                                LoadReplaceWindow();
                            }
                        }
                        else
                        {
                            this.ShowNotification(new GUIContent("这不是一个PrefabInstance"));
                            CleanUp();
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                GUILayout.Label("对应组件", "BoldLabel");
                for (int index = 0; index < _baseWindow.transform.childCount; index++)
                {
                    Transform t = _baseWindow.transform.GetChild(index);

                    if (_childNameList.IndexOf(t.name) == -1)
                    {
                        Debug.LogError("标准控件: " + _baseWindow.name + " 不应该包含 " + t.name);
                        break;
                    }
                    else if (_replaceWindow != null)
                    {
                        string replaceName = string.Empty;
                        if (_replaceDic.ContainsKey(t.name))
                            replaceName = _replaceDic[t.name];

                        replaceName = EditorGUILayout.TextField(t.name + ": ", replaceName, GUILayout.Width(500f));

                        if (_replaceDic.ContainsKey(t.name))
                            _replaceDic[t.name] = replaceName;
                        else
                            _replaceDic.Add(t.name, replaceName);

                        if (string.IsNullOrEmpty(replaceName))
                        {
                            AutoReplaceNameSetting(t);
                        }
                    }
                }
                if (_replaceDic.ContainsKey(_titleName) && string.IsNullOrEmpty(_replaceDic[_titleName]))
                {
                    AutoReplaceTitleSprite();
                }

                if (_replaceDic.ContainsKey(_winTabGroupName) && string.IsNullOrEmpty(_replaceDic[_winTabGroupName]))
                {
                    AutoReplaceTabGrid();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            _titleNameSprite = EditorGUILayout.TextField("替换 TitleName:", _titleNameSprite);
            if (GUILayout.Button("根据名字纠正 TitleNameSprite", GUILayout.Height(50f)))
            {
                if (!string.IsNullOrEmpty(_titleNameSprite))
                {
                    GetReplaceSpriteName(_titleNameSprite, _replaceWindow.gameObject);
                    if (_replaceDic.ContainsKey("TitleNameSprite"))
                    {
                        _replaceDic["TitleNameSprite"] = _replaceName;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if (_baseWindow.name == "BaseTabWindow")
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                _winTabGroup = EditorGUILayout.TextField("替换 WinTabGroup:", _winTabGroup);
                if (GUILayout.Button("根据名字纠正 WinTabGroup", GUILayout.Height(50f)))
                {
                    if (!string.IsNullOrEmpty(_winTabGroup))
                    {
                        GetReplaceGridName(_winTabGroup, _replaceWindow.gameObject);
                        if (_replaceDic.ContainsKey("WinTabGroup"))
                        {
                            _replaceDic["WinTabGroup"] = _replaceName;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }


            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.BeginVertical("HelpBox", GUILayout.Width(550f));
            {
                GUILayout.Label("非标准窗口替换", "BoldLabel");
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(@"替换窗口", GUILayout.Width(60f));
                //_dragItem = EditorGUILayout.ObjectField("ReplaceWindow", _dragItem, typeof(Transform), true, GUILayout.ExpandWidth(false), GUILayout.Width(140f)) as Transform;
                _dragItem =
                    EditorGUILayout.ObjectField(_dragItem, typeof (Transform), true, GUILayout.ExpandWidth(false),
                        GUILayout.Width(180f)) as Transform;
                if (_dragItem != null)
                {
                    if (_dragItem != _replaceWindow)
                    {
                        if (PrefabInstanceCheck(_dragItem))
                        {
                            _dragItem = PrefabUtility.FindPrefabRoot(_dragItem.gameObject).transform;
                            if (_replaceWindow != _dragItem)
                            {
                                _replaceWindow = _dragItem;
                                this.RemoveNotification();
                                LoadReplaceWindow();
                            }
                        }
                        else
                        {
                            this.ShowNotification(new GUIContent("这不是一个PrefabInstance"));
                            CleanUp();
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();
        }

        // 字体颜色处理
        EditorGUILayout.BeginVertical("HelpBox", GUILayout.Width(550f));
        {
            GUILayout.Label("字体颜色替换", "BoldLabel");
            if (_lblcolorMap != null)
            {
                foreach (var col in _lblcolorMap.Keys)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ColorField(string.Format("原: {0}", ColorToHex(_lblcolorMap[col])), _lblcolorMap[col]);
                    Color replaColor = _lblcolorReplaceMap[col];
                    replaColor = EditorGUILayout.ColorField(string.Format("替换: {0}", ColorToHex(replaColor)), replaColor);
                    _lblcolorReplaceMap[col] = replaColor;
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("注意:工具是根据 BaseWindow,BaseWindow,BaseButton,BaseSmallButton");
        EditorGUILayout.LabelField("四个 Prefab 进行处理的,请务必确认这四个 Prefab 标准");
        EditorGUILayout.LabelField("同时会对 prefab 内部节点进行 localPosition 取整");

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUI.color = Color.green;
        if (GUILayout.Button("替换", "LargeButton", GUILayout.Height(50f)))
        {
            HandleReplace();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUI.color = Color.red;
        EditorGUILayout.BeginVertical("HelpBox", GUILayout.Width(550f));
        {
            GUI.color = Color.yellow;
            GUILayout.Label("超级功能,对所有Prefab进行操作,前提条件:", "BoldLabel");
            GUILayout.Label("1.窗口Prefab结构必须是以BaseWindow,BaseTabWindow为基础的标准结构(参看H5).", "BoldLabel");
            GUILayout.Label("2.美术提供新的标准 BaseWindow,BaseTabWindow.", "BoldLabel");
            GUILayout.Label("3.美术提供新的标准 BaseButton,BaseSmallButton.", "BoldLabel");
            GUILayout.Label("4.美术提供新的标准 BaseLabel 配对 (OldGroup Label_1 -> NewGroup Label_1)", "BoldLabel");
            EditorGUILayout.Space();

            GUILayout.Label("1.替换所有标准窗口样式,BaseWindow or BaseTabWindow", "BoldLabel");
            if (GUILayout.Button("一键替换所有标准化窗口", "LargeButton", GUILayout.Height(50f)))
            {
                if (EditorUtility.DisplayDialog("替换所有标准化窗口", "确认替换所有标准化窗口?", "确认", "取消"))
                {
                    HandleAllStdWinReplace();
                    return;
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.Label("2.替换所有标准按钮样式,并且保留原来字体大小", "BoldLabel");
            if (GUILayout.Button("一键替换所有标准化按钮", "LargeButton", GUILayout.Height(50f)))
            {
                if (EditorUtility.DisplayDialog("替换所有标准化按钮", "确认替换所有标准化按钮?", "确认", "取消"))
                {
                    HandleAllStdBtnReplace();
                    return;
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.Label("3.根据 BaseLabel.prefab Label 的对应关系,替换Label样式,忽略按钮文字", "BoldLabel");
            if (GUILayout.Button("一键替换所有Label颜色值", "LargeButton", GUILayout.Height(50f)))
            {
                if (EditorUtility.DisplayDialog("替换所有Label颜色值", "确认替换所有Label颜色值?", "确认", "取消"))
                {
                    HandleAllLblColorReplace();
                    return;
                }
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    private string ColorToHex(Color matColor)
    {
        int r0 = (int)Mathf.Round(255 * matColor.r);
        int g0 = (int)Mathf.Round(255 * matColor.g);
        int b0 = (int)Mathf.Round(255 * matColor.b);


        string r = r0.ToString("X");
        if (r.Length < 2)
        {
            r = "0" + r;
        }
        string g = g0.ToString("X");
        if (g.Length < 2)
        {
            g = "0" + g;
        }
        string b = b0.ToString("X");
        if (b.Length < 2)
        {
            b = "0" + b;
        }
        return "#" + r + g + b;
    }

    private void LoadReplaceWindow()
    {
        _lbls.Clear();
        GetAllLabel(_replaceWindow.gameObject);

        _lblcolorMap.Clear();
        _lblcolorReplaceMap.Clear();
        foreach (var lbl in _lbls)
        {
            Debug.Log("lbl.color.ToString() = " + lbl.color.ToString());
            if(_lblcolorMap.ContainsKey(lbl.color.ToString()) == false)
                _lblcolorMap.Add(lbl.color.ToString(),lbl.color);

            if (_lblcolorReplaceMap.ContainsKey(lbl.color.ToString()) == false)
                _lblcolorReplaceMap.Add(lbl.color.ToString(), lbl.color);
        }
    }

    private void GetAllLabel(GameObject go)
    {
        UILabel[] uiLabels = go.GetComponents<UILabel>();
        if (uiLabels != null && uiLabels.Length > 0)
        {
            foreach (var lbl in uiLabels)
            {
                _lbls.Add(lbl);
            }
        }

        foreach (Transform child in go.transform)
        {
            GetAllLabel(child.gameObject);
        }
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

    private void CleanUp()
    {
        //清空操作
        _dragItem = null;
        _replaceWindow = null;
        _replaceDic.Clear();
        _titleNameSprite = "";
    }

    private void AutoReplaceNameSetting(Transform tf)
    {
        Transform wintf = _replaceWindow.transform.Find("BaseWindow");
        if (wintf != null)
        {
            // 标准模式
            Transform rtf = wintf.Find(tf.name);
            if (rtf != null)
            {
                if (_replaceDic.ContainsKey(tf.name))
                    _replaceDic[tf.name] = "BaseWindow/" + tf.name;
                return;
            }
        }

        wintf = _replaceWindow.transform.Find("BaseTabWindow");
        if (wintf != null)
        {
            // 标准模式
            Transform rtf = wintf.Find(tf.name);
            if (rtf != null)
            {
                if (_replaceDic.ContainsKey(tf.name))
                    _replaceDic[tf.name] = "BaseTabWindow/" + tf.name;
                return;
            }
        }

        // 非标准窗口,智能适配,规则适配 UISprite.spriteName
        // "WinBg", "CloseBtn", "TitleBg" , "TitleNameSprite", "WinTabGroup"
        UISprite sprite = tf.gameObject.GetComponent<UISprite>();
        _replaceName = "";
        GetReplaceSpriteName(sprite, _replaceWindow.gameObject);
        if (_replaceDic.ContainsKey(tf.name))
            _replaceDic[tf.name] = _replaceName;
    }

    private GameObject _uiRoot = null;
    private void AutoReplaceTitleSprite()
    {
        Transform titleTransform = null;
        if (_replaceDic.ContainsKey(_titleBgName))
        {
            titleTransform = GetSpriteTransform(_replaceWindow.gameObject, _replaceDic[_titleBgName], _titleBgSprite);
        }
        
        if (titleTransform == null) return;
        
        UISprite sprite = titleTransform.GetComponent<UISprite>();
        if (sprite == null) return;

        _uiRoot = GetUIRoot(_replaceWindow.gameObject);
        if (_uiRoot == null) return;

        string spriteName = GetTitleSpriteNameWithBg(_replaceWindow.gameObject, titleTransform.position, sprite);
        if(!string.IsNullOrEmpty(spriteName)) _replaceDic[_titleName] = spriteName;
    }

    private string GetTitleSpriteNameWithBg(GameObject root, Vector3 center, UISprite bgSprite)
    {
        float width = bgSprite.width * _uiRoot.transform.localScale.x;
        float height = bgSprite.height * _uiRoot.transform.localScale.y;
        UISprite sp = root.GetComponent<UISprite>();
        if (sp != null && sp.spriteName != bgSprite.spriteName)
        {
            var pos = sp.transform.position;
            if (sp.width < bgSprite.width && (sp.height-10) < bgSprite.height &&
                pos.x > (center.x-width/2) && pos.x < (center.x+width/2) &&
                pos.y > (center.y-height/2) && pos.y < (center.y+height/2))
            { 
                return GameObjectExt.GetHierarchyWithRoot(sp.transform, _replaceWindow);
            }
        }

        string name = string.Empty;
        foreach (Transform child in root.transform)
        {
            name = GetTitleSpriteNameWithBg(child.gameObject, center, bgSprite);
            if (!string.IsNullOrEmpty(name)) return name;
        }
        return string.Empty;
    }

    private void AutoReplaceTabGrid()
    {
        Transform trans = null;
        if (_replaceDic.ContainsKey(_winTabGroupName))
        {
            trans = GetSpriteTransform(_replaceWindow.gameObject, _replaceDic[_winBgName], _winBgSprite);
        }

        if (trans == null) return;

        UISprite sprite = trans.GetComponent<UISprite>();
        if (sprite == null) return;

        _uiRoot = GetUIRoot(_replaceWindow.gameObject);
        if (_uiRoot == null) return;

        string name = GetTabGridName(_replaceWindow.gameObject, trans.position, sprite);
        if (!string.IsNullOrEmpty(name)) _replaceDic[_winTabGroupName] = name;
    }

    private string GetTabGridName(GameObject root, Vector3 center, UISprite bgSprite)
    {
        float scaleX = _uiRoot.transform.localScale.x;
        float scaleY = _uiRoot.transform.localScale.y;
        float x = center.x / scaleX + bgSprite.width / 2;
        float y = center.y/scaleY + bgSprite.height / 2;
        UIGrid com = root.GetComponent<UIGrid>();
        if(com != null)
        {
            float comX = com.transform.position.x / scaleX;
            float comY = com.transform.position.y / scaleY;
            if(x<comX && comX<x+50 && comY > y-160 && comY < y - 70)
            {
                return GameObjectExt.GetHierarchyWithRoot(com.transform, _replaceWindow);
            }
        }

        string name = string.Empty;
        foreach (Transform child in root.transform)
        {
            name = GetTabGridName(child.gameObject, center, bgSprite);
            if (!string.IsNullOrEmpty(name)) return name;
        }

        return string.Empty;
    }

    //遍历处理同名的情况
    private Transform GetSpriteTransform(GameObject root, string objectName, string spriteName)
    {
        UISprite sp = root.transform.GetComponent<UISprite>();
        if (objectName.EndsWith(root.name) && sp != null && sp.spriteName == spriteName)
        {
            return root.transform;
        }

        foreach (Transform child in root.transform)
        {
            var trans = GetSpriteTransform(child.gameObject, objectName, spriteName);
            if (trans != null) return trans;
        }
        return null;
    }

    private GameObject GetUIRoot(GameObject go)
    {
        Transform t = go.transform;

        for (;;)
        {
            if (t.gameObject.name == "UIRoot") return t.gameObject;
            t = t.parent;
            if (t == null) break;
        }
        return null;
    }

    private string _replaceName = "";

    private void GetReplaceSpriteName(UISprite sprite, GameObject go)
    {
        //_replaceName = _replaceName + "/" + go.name;
        UISprite sp = go.GetComponent<UISprite>();
        if (sp != null && sprite != null)
        {
            if (sp.atlas == sprite.atlas && sp.spriteName == sprite.spriteName)
            {
                _replaceName = GameObjectExt.GetHierarchyWithRoot(sp.transform, _replaceWindow);
                return;
            }
        }

        foreach (Transform child in go.transform)
        {
            GetReplaceSpriteName(sprite,child.gameObject);
        }
    }

    private void GetReplaceSpriteName(string titleNameSprite, GameObject go)
    {
        //_replaceName = _replaceName + "/" + go.name;
        UISprite sp = go.GetComponent<UISprite>();
        if (sp != null)
        {
            if (sp.name == titleNameSprite)
            {
                _replaceName = GameObjectExt.GetHierarchyWithRoot(sp.transform, _replaceWindow);
                return;
            }
        }

        foreach (Transform child in go.transform)
        {
            GetReplaceSpriteName(titleNameSprite, child.gameObject);
        }
    }

    private void GetReplaceGridName(string gridNameSprite, GameObject go)
    {
        //_replaceName = _replaceName + "/" + go.name;
        UIGrid grid = go.GetComponent<UIGrid>();
        if (grid != null)
        {
            if (grid.name == gridNameSprite)
            {
                _replaceName = GameObjectExt.GetHierarchyWithRoot(grid.transform, _replaceWindow);
                return;
            }
        }

        foreach (Transform child in go.transform)
        {
            GetReplaceGridName(gridNameSprite, child.gameObject);
        }
    }

    private void HandleReplace()
    {
        if (_isStdWin)
        {
            HandleWinReplace();
        }

        // 控件位置取整
        CheckUIPosFractionsInGo(_replaceWindow.gameObject);

        handleLabelColorReplace(_replaceWindow.gameObject);

        if ((_oldBaseBnts == null || _oldBaseBnts.Count == 0) && (_newBaseBtns == null || _newBaseBtns.Count == 0))
            Debug.LogError("没有标准按钮替换数据: _oldBaseBnts and _newBaseBtns");
        else if ( _baseSmallBtn == null)
            Debug.LogError("没有标准按钮: BaseSmallBtn");
        else if(_baseSmallBtnLbl == null)
            Debug.LogError("BaseSmallBtn 下没有 Label");
        else
            HandleBtnLblReplace(_replaceWindow.gameObject);

        AssetDatabase.SaveAssets();
    }

    private void HandleWinReplace()
    {
        Transform wintf = _replaceWindow.transform.Find("BaseWindow");
        if (wintf != null)
        {
            // 标准模式,直接替换
            GameObject replaceWindow = NGUITools.AddChild(_replaceWindow.gameObject, _baseWindow);
            replaceWindow.name = "BaseWindow";

            Transform titletf = wintf.Find("TitleNameSprite");
            if (titletf != null)
            {
                // 保存标题图集
                string spriteName = titletf.gameObject.GetComponent<UISprite>().spriteName;
                UIAtlas atlas = titletf.gameObject.GetComponent<UISprite>().atlas;

                Transform titleReplacetf = replaceWindow.transform.Find("TitleNameSprite");
                if (titleReplacetf != null)
                {
                    titleReplacetf.gameObject.GetComponent<UISprite>().atlas = atlas;
                    titleReplacetf.gameObject.GetComponent<UISprite>().spriteName = spriteName;
                    titleReplacetf.gameObject.GetComponent<UISprite>().MakePixelPerfect();
                }
            }

            NGUITools.Destroy(wintf);
            return;
        }

        wintf = _replaceWindow.transform.Find("BaseTabWindow");
        if (wintf != null)
        {
            // 标准模式,直接替换
            GameObject replaceWindow = NGUITools.AddChild(_replaceWindow.gameObject, _baseWindow);
            replaceWindow.name = "BaseTabWindow";

            Transform titletf = wintf.Find("TitleNameSprite");
            if (titletf != null)
            {
                // 保存标题图集
                string spriteName = titletf.gameObject.GetComponent<UISprite>().spriteName;
                UIAtlas atlas = titletf.gameObject.GetComponent<UISprite>().atlas;

                Transform titleReplacetf = replaceWindow.transform.Find("TitleNameSprite");
                if (titleReplacetf != null)
                {
                    titleReplacetf.gameObject.GetComponent<UISprite>().atlas = atlas;
                    titleReplacetf.gameObject.GetComponent<UISprite>().spriteName = spriteName;
                    titleReplacetf.gameObject.GetComponent<UISprite>().MakePixelPerfect();
                }
            }

            NGUITools.Destroy(wintf);
            return;
        }

        // 非标准窗口,根据 _replaceDic 删除旧的相关Object
        {
            GameObject replaceWindow = NGUITools.AddChild(_replaceWindow.gameObject, _baseWindow);
            replaceWindow.name = _baseWindow.name;

            if (_replaceDic.ContainsKey("TitleNameSprite"))
            {
                string path = _replaceDic["TitleNameSprite"];
                if (!string.IsNullOrEmpty(path))
                {
                    Transform titletf = _replaceWindow.Find(path);
                    if (titletf != null)
                    {
                        // 保存标题图集
                        string spriteName = titletf.gameObject.GetComponent<UISprite>().spriteName;
                        UIAtlas atlas = titletf.gameObject.GetComponent<UISprite>().atlas;

                        Transform titleReplacetf = replaceWindow.transform.Find("TitleNameSprite");
                        if (titleReplacetf != null)
                        {
                            titleReplacetf.gameObject.GetComponent<UISprite>().atlas = atlas;
                            titleReplacetf.gameObject.GetComponent<UISprite>().spriteName = spriteName;
                            titleReplacetf.gameObject.GetComponent<UISprite>().MakePixelPerfect();
                        }
                    }
                }
            }

            List<string> replaceList = new List<string>();
            //替换代码路径接口
            Dictionary<string, string> rpScriptDct = new Dictionary<string, string>();

            foreach (string key in _replaceDic.Keys)
            {
                string value = _replaceDic[key];
                replaceList.Add(value);
                rpScriptDct[string.Format(@"(""{0}"")", value)] = string.Format(@"(""{0}/{1}"")", _baseWindow.name, key);
            }

            // 路径长的一定是在最里面的
            replaceList.Sort(delegate (string a, string b) { return b.Length.CompareTo(a.Length); });

            foreach (var value in replaceList)
            {
                Transform tf = _replaceWindow.Find(value);
                if (tf.childCount > 0)
                {
                    Component[] components = tf.GetComponents<Component>();
                    foreach (var component in components)
                    {
                        if (component is Transform)
                            continue;
                        DestroyImmediate(component);
                    }
                }
                else
                {
                    NGUITools.Destroy(tf);
                }
            }
            
            string sModifyPath = UIViewGenerator.ReplaceComponentPath(_replaceWindow.name, rpScriptDct);
            if (!string.IsNullOrEmpty(sModifyPath))
            {
                EditorUtility.DisplayDialog("留意", "已自动修改"+ sModifyPath, "确认");
            }
                
        }
    }

    // 按钮字体替换
    private void HandleBtnLblReplace(GameObject go)
    {
        UIButton[] uiButtons = go.GetComponents<UIButton>();
        if (uiButtons != null && uiButtons.Length > 0)
        {
            string path = GetHierarchyWithRoot(go);
            if (uiButtons.Length > 1)
            {
                Debug.LogError(string.Format("{0} 存在多于一个UIButton组件，请检查", path));
            }

            UIButton btn = uiButtons[0];
            UIButtonColor uIButtonColor = new UIButtonColor();

            btn.hover = uIButtonColor.hover;
            btn.pressed = uIButtonColor.pressed;
            btn.disabledColor = uIButtonColor.disabledColor;

            if (ReplaceBaseBtn(btn))
            {
                // ReplaceBaseBtn 函数处理
            }
            else if (btn.normalSprite == _baseSmallBtn.normalSprite)
            {
                foreach (Transform child in btn.transform)
                {
                    UILabel lbl = child.gameObject.GetComponent<UILabel>();
                    if (lbl != null)
                    {
                        lbl.applyGradient = false;
                        lbl.fontSize = _baseSmallBtnLbl.fontSize;
                        lbl.color = _baseSmallBtnLbl.color;
                        lbl.effectStyle = _baseSmallBtnLbl.effectStyle;
                        lbl.effectColor = _baseSmallBtnLbl.effectColor;
                    }
                }
            }
        }

        foreach (Transform child in go.transform)
        {
            HandleBtnLblReplace(child.gameObject);
        }
    }

    private string GetHierarchyWithRoot(GameObject go)
    {
        string path = go.name;
        Transform t = go.transform;
        while (t.parent != null)
        {
            path = t.parent.name + "/" + path;
            t = t.parent;
        }
        return path;
    }

    private void handleLabelColorReplace(GameObject go)
    {
        UILabel[] uiLabels = go.GetComponents<UILabel>();
        if (uiLabels != null && uiLabels.Length > 0)
        {
            foreach (var lbl in uiLabels)
            {
                lbl.applyGradient = false;
                if (lbl.effectStyle == UILabel.Effect.None)
                {
                    string key = lbl.color.ToString();
                    if (_lblcolorReplaceMap.ContainsKey(key))
                        lbl.color = _lblcolorReplaceMap[key];
                }
            }
        }

        foreach (Transform child in go.transform)
        {
            handleLabelColorReplace(child.gameObject);
        }
    }

    private void CheckUIPosFractionsInGo(GameObject go)
    {
        Vector3 newPos = new Vector3(Mathf.Round(go.transform.localPosition.x), Mathf.Round(go.transform.localPosition.y), Mathf.Round(go.transform.localPosition.z));
        if (go.transform.localPosition.x - newPos.x != 0
            || go.transform.localPosition.y - newPos.y != 0
            || go.transform.localPosition.z - newPos.z != 0)
        {
            go.transform.localPosition = newPos;
        }

        //递归遍历子节点
        foreach (Transform child in go.transform)
        {
            CheckUIPosFractionsInGo(child.gameObject);
        }
    }

    #region 超级功能,一键替换 XXX
    private int _goCount = 0, _hitCount = 0;
    private List<Object> _searchList;
    private GameObject _stdWin;
    private GameObject _strTabWin;
    private Dictionary<string, UILabel> _replaceLblDic;
     
    private void InitPrefabList()
    {
        if (_searchList == null)
        {
            string folderPath = "Assets/UI/Prefabs/Module";
            string[] GUIDs = AssetDatabase.FindAssets("t:prefab", new string[] { folderPath });
            List<UnityEngine.Object> objList = new List<Object>(GUIDs.Length);
            for (int i = 0; i < GUIDs.Length; ++i)
            {
                objList.Add(AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(GUIDs[i])));
            }
            _searchList = objList;

            _stdWin = AssetDatabase.LoadMainAssetAtPath("Assets/UI/Prefabs/BaseUI/BaseWindow.prefab") as GameObject;
            _strTabWin = AssetDatabase.LoadMainAssetAtPath("Assets/UI/Prefabs/BaseUI/BaseTabWindow.prefab") as GameObject;


        }
    }

    private bool InitLblDic()
    {
        if (_replaceLblDic == null)
        {
            GameObject lblsObj = AssetDatabase.LoadMainAssetAtPath("Assets/UI/Prefabs/BaseUI/BaseLabel.prefab") as GameObject;
            if (lblsObj != null)
            {
                GameObject OldGroup = lblsObj.transform.Find("OldGroup").gameObject;
                GameObject NewGroup = lblsObj.transform.Find("NewGroup").gameObject;

                UILabel[] originals = OldGroup.GetComponentsInChildren<UILabel>();
                UILabel[] replaces = NewGroup.GetComponentsInChildren<UILabel>();

                if (originals.Length != replaces.Length)
                {
                    Debug.LogError("BaseLabel.prefab 原和替换 Label 数量不相等");
                    return false;
                }

                _replaceLblDic = new Dictionary<string, UILabel>();
                foreach (UILabel original in originals)
                {
                    bool hasIt = false;
                    foreach (UILabel replace in replaces)
                    {
                        if (replace.name == original.name)
                        {
                            hasIt = true;
                            string key = original.color.ToString();
                            if (original.effectStyle != UILabel.Effect.None)
                            {
                                key += "_" + original.effectStyle + "_" + original.effectColor;
                            }

                            if (_replaceLblDic.ContainsKey(key))
                            {
                                Debug.LogError(string.Format("BaseLabel.prefab 原Label中 {0} 为重复 Label",original.name));
                                _replaceLblDic = null;
                                return false;
                            }

                            _replaceLblDic.Add(key, replace);
                        }
                    }

                    if (!hasIt)
                    {
                        Debug.LogError("BaseLabel.prefab 替换 Label 没有: " + original.name);
                        _replaceLblDic = null;
                        return false;
                    }
                }
                return true;
            }
        }

        return false;
    }

    private void ResetCount()
    {
        _goCount = 0;
        _hitCount = 0;
    }

    private void HandleAllStdWinReplace()
    {
        InitPrefabList();
        ResetCount();
        foreach (var obj in _searchList)
        {
            GameObject go = obj as GameObject;
            HandleStdWinReplace(go);
            EditorUtility.SetDirty(go);
        }

        Debug.Log(string.Format("Searched {0} GameObjects, Replace {1} Std Win", _goCount, _hitCount));
        AssetDatabase.SaveAssets();
    }

    private void HandleStdWinReplace(GameObject go)
    {
        _goCount++;

        Transform wintf = go.transform.Find("BaseWindow");
        if (wintf != null)
        {
            Debug.Log("BaseWindow Replace: " + go.name);
            _hitCount++;

            Transform base_bgtf = _stdWin.transform.Find("WinBg");
            Transform base_closetf = _stdWin.transform.Find("CloseBtn");
            Transform base_titleBgtf = _stdWin.transform.Find("TitleBg");
            Transform base_titleNametf = _stdWin.transform.Find("TitleNameSprite");

            Transform bgtf = wintf.Find("WinBg");
            Transform closetf = wintf.Find("CloseBtn");
            Transform titleBgtf = wintf.Find("TitleBg");
            Transform titleNametf = wintf.Find("TitleNameSprite");

            if (base_bgtf != null && bgtf != null && bgtf.gameObject.GetComponent<UISprite>() != null)
            {
                bgtf.gameObject.GetComponent<UISprite>().atlas = base_bgtf.gameObject.GetComponent<UISprite>().atlas;
                bgtf.gameObject.GetComponent<UISprite>().spriteName = base_bgtf.gameObject.GetComponent<UISprite>().spriteName;
                bgtf.gameObject.GetComponent<UISprite>().pivot = base_bgtf.gameObject.GetComponent<UISprite>().pivot;

                bgtf.localPosition = base_bgtf.localPosition;
                bgtf.localRotation = base_bgtf.localRotation;
                bgtf.localScale = base_bgtf.localScale;
            }

            if (base_closetf != null && closetf != null 
                && closetf.gameObject.GetComponent<UISprite>() != null && closetf.gameObject.GetComponent<UIButton>() != null)
            {
                closetf.gameObject.GetComponent<UISprite>().atlas = base_closetf.gameObject.GetComponent<UISprite>().atlas;
                closetf.gameObject.GetComponent<UISprite>().spriteName = base_closetf.gameObject.GetComponent<UISprite>().spriteName;
                closetf.gameObject.GetComponent<UISprite>().pivot = base_closetf.gameObject.GetComponent<UISprite>().pivot;

                closetf.localPosition = base_closetf.localPosition;
                closetf.localRotation = base_closetf.localRotation;
                closetf.localScale = base_closetf.localScale;

                closetf.gameObject.GetComponent<UIButton>().hover = base_closetf.gameObject.GetComponent<UIButton>().hover;
                closetf.gameObject.GetComponent<UIButton>().pressed = base_closetf.gameObject.GetComponent<UIButton>().pressed;
            }

            if (base_titleBgtf != null && titleBgtf != null && titleBgtf.gameObject.GetComponent<UISprite>() != null)
            {
                titleBgtf.gameObject.GetComponent<UISprite>().atlas = base_titleBgtf.gameObject.GetComponent<UISprite>().atlas;
                titleBgtf.gameObject.GetComponent<UISprite>().spriteName = base_titleBgtf.gameObject.GetComponent<UISprite>().spriteName;
                titleBgtf.gameObject.GetComponent<UISprite>().pivot = base_titleBgtf.gameObject.GetComponent<UISprite>().pivot;

                titleBgtf.localPosition = base_titleBgtf.localPosition;
                titleBgtf.localRotation = base_titleBgtf.localRotation;
                titleBgtf.localScale = base_titleBgtf.localScale;
            }

            if (base_titleNametf != null && titleNametf != null)
            {
                if (titleNametf.gameObject.GetComponent<UISprite>() != null)
                    titleNametf.gameObject.GetComponent<UISprite>().MakePixelPerfect();

                titleNametf.localPosition = base_titleNametf.localPosition;
                titleNametf.localRotation = base_titleNametf.localRotation;
                titleNametf.localScale = base_titleNametf.localScale;
            }

            return;
        }

        wintf = go.transform.Find("BaseTabWindow");
        if (wintf != null)
        {
            Debug.Log("BaseTabWindow Replace: " + go.name);
            _hitCount++;

            Transform base_bgtf = _strTabWin.transform.Find("WinBg");
            Transform base_closetf = _strTabWin.transform.Find("CloseBtn");
            Transform base_titleBgtf = _strTabWin.transform.Find("TitleBg");
            Transform base_titleNametf = _strTabWin.transform.Find("TitleNameSprite");
            Transform base_winTabGrouptf = _strTabWin.transform.Find("WinTabGroup");

            Transform bgtf = wintf.Find("WinBg");
            Transform closetf = wintf.Find("CloseBtn");
            Transform titleBgtf = wintf.Find("TitleBg");
            Transform titleNametf = wintf.Find("TitleNameSprite");
            Transform winTabGrouptf = wintf.transform.Find("WinTabGroup");

            if (base_bgtf != null && bgtf != null && bgtf.gameObject.GetComponent<UISprite>() != null)
            {
                bgtf.gameObject.GetComponent<UISprite>().atlas = base_bgtf.gameObject.GetComponent<UISprite>().atlas;
                bgtf.gameObject.GetComponent<UISprite>().spriteName = base_bgtf.gameObject.GetComponent<UISprite>().spriteName;
                bgtf.gameObject.GetComponent<UISprite>().pivot = base_bgtf.gameObject.GetComponent<UISprite>().pivot;

                bgtf.localPosition = base_bgtf.localPosition;
                bgtf.localRotation = base_bgtf.localRotation;
                bgtf.localScale = base_bgtf.localScale;
            }

            if (base_closetf != null && closetf != null 
                && closetf.gameObject.GetComponent<UISprite>() != null && closetf.gameObject.GetComponent<UIButton>() != null)
            {
                closetf.gameObject.GetComponent<UISprite>().atlas = base_closetf.gameObject.GetComponent<UISprite>().atlas;
                closetf.gameObject.GetComponent<UISprite>().spriteName = base_closetf.gameObject.GetComponent<UISprite>().spriteName;
                closetf.gameObject.GetComponent<UISprite>().pivot = base_closetf.gameObject.GetComponent<UISprite>().pivot;

                closetf.localPosition = base_closetf.localPosition;
                closetf.localRotation = base_closetf.localRotation;
                closetf.localScale = base_closetf.localScale;

                closetf.gameObject.GetComponent<UIButton>().hover = base_closetf.gameObject.GetComponent<UIButton>().hover;
                closetf.gameObject.GetComponent<UIButton>().pressed = base_closetf.gameObject.GetComponent<UIButton>().pressed;
            }

            if (base_titleBgtf != null && titleBgtf != null && titleBgtf.gameObject.GetComponent<UISprite>() != null)
            {
                titleBgtf.gameObject.GetComponent<UISprite>().atlas = base_titleBgtf.gameObject.GetComponent<UISprite>().atlas;
                titleBgtf.gameObject.GetComponent<UISprite>().spriteName = base_titleBgtf.gameObject.GetComponent<UISprite>().spriteName;
                titleBgtf.gameObject.GetComponent<UISprite>().pivot = base_titleBgtf.gameObject.GetComponent<UISprite>().pivot;

                titleBgtf.localPosition = base_titleBgtf.localPosition;
                titleBgtf.localRotation = base_titleBgtf.localRotation;
                titleBgtf.localScale = base_titleBgtf.localScale;
            }

            if (base_titleNametf != null && titleNametf != null)
            {
                if(titleNametf.gameObject.GetComponent<UISprite>() != null)
                    titleNametf.gameObject.GetComponent<UISprite>().MakePixelPerfect();

                titleNametf.localPosition = base_titleNametf.localPosition;
                titleNametf.localRotation = base_titleNametf.localRotation;
                titleNametf.localScale = base_titleNametf.localScale;
            }

            if (base_winTabGrouptf != null && winTabGrouptf != null)
            {
                winTabGrouptf.localPosition = base_winTabGrouptf.localPosition;
                winTabGrouptf.localRotation = base_winTabGrouptf.localRotation;
                winTabGrouptf.localScale = base_winTabGrouptf.localScale;
            }
            return;
        }
    }

    private void HandleAllStdBtnReplace()
    {
        InitPrefabList();
        ResetCount();

        foreach (var obj in _searchList)
        {
            GameObject go = obj as GameObject;
            HandleStdBtnReplace(go);
            EditorUtility.SetDirty(go);
        }

        Debug.Log(string.Format("Searched {0} GameObjects, Replace {1} Std Win", _goCount, _hitCount));
        AssetDatabase.SaveAssets();
    }

    private void HandleStdBtnReplace(GameObject go)
    {
        _goCount++;
        UIButton[] uiButtons = go.GetComponents<UIButton>();
        if (uiButtons != null && uiButtons.Length > 0)
        {
            _hitCount++;
            string path = GetHierarchyWithRoot(go);
            if (uiButtons.Length > 1)
            {
                Debug.LogError(string.Format("{0} 存在多于一个UIButton组件，请检查", path));
            }

            UIButton btn = uiButtons[0];
            UIButtonColor uIButtonColor = new UIButtonColor();

            btn.hover = uIButtonColor.hover;
            btn.pressed = uIButtonColor.pressed;
            btn.disabledColor = uIButtonColor.disabledColor;

            if (ReplaceBaseBtn(btn))
            {
                // ReplaceBaseBtn 函数处理
            }
            else if (btn.normalSprite == _baseSmallBtn.normalSprite)
            {
                foreach (Transform child in btn.transform)
                {
                    UILabel lbl = child.gameObject.GetComponent<UILabel>();
                    if (lbl != null)
                    {
                        lbl.applyGradient = false;
                        lbl.fontSize = _baseSmallBtnLbl.fontSize;
                        lbl.color = _baseSmallBtnLbl.color;
                        lbl.effectStyle = _baseSmallBtnLbl.effectStyle;
                        lbl.effectColor = _baseSmallBtnLbl.effectColor;
                    }
                }
            }
        }

        //递归遍历子节点
        foreach (Transform child in go.transform)
        {
            HandleStdBtnReplace(child.gameObject);
        }
    }

    private void HandleAllLblColorReplace()
    {
        InitPrefabList();
        ResetCount();

        if (!InitLblDic())
            return;

        if (_replaceLblDic == null || _replaceLblDic.Count == 0)
        {
            Debug.LogError("BaseLabel.prefab 出错: 没有替换 Label 数据");
            return;
        }

        foreach (var obj in _searchList)
        {
            GameObject go = obj as GameObject;
            HandleLblColorReplace(go);
            EditorUtility.SetDirty(go);
        }

        Debug.Log(string.Format("Searched {0} GameObjects, Replace {1} Std Win", _goCount, _hitCount));
        AssetDatabase.SaveAssets();
    }

    private void HandleLblColorReplace(GameObject go)
    {
        _goCount++;
        UILabel[] uiLabels = go.GetComponents<UILabel>();
        if (uiLabels != null && uiLabels.Length > 0)
        {
            foreach (UILabel lbl in uiLabels)
            {
                if (lbl.transform.parent != null && lbl.transform.parent.GetComponent<UIButton>() != null)
                {
                    // 忽略按钮字体
                    continue;
                }

                string key = lbl.color.ToString();
                if (lbl.effectStyle != UILabel.Effect.None)
                {
                    key += "_" + lbl.effectStyle + "_" + lbl.effectColor;
                }

                if (_replaceLblDic.ContainsKey(key))
                {
                    UILabel replace = _replaceLblDic[key];

                    lbl.applyGradient = false;
                    lbl.effectStyle = replace.effectStyle;
                    if (lbl.effectStyle != UILabel.Effect.None)
                    {
                        lbl.effectColor = replace.effectColor;
                    }
                    lbl.color = replace.color;
                }
            }
        }

        //递归遍历子节点
        foreach (Transform child in go.transform)
        {
            HandleLblColorReplace(child.gameObject);
        }
    }


    #endregion
}

class UIBtnContainer
{
    public UIButton btn;
    public UILabel lbl;
    public UISprite sp;
}