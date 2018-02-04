using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PrefabCheckTool : EditorWindow
{
    public static PrefabCheckTool instance;

    private Vector2 _listScrollPos;
    private Object _selectedObj;
    private AudioClip _buttonAudioClip;
    private UIAtlas _targetAtlas;

    private string _params = "";
    private List<Object> _searchList;
    private List<Object> _resultList;

    private int _goCount = 0, _comCount = 0, _hitCount = 0;

    [MenuItem("GameResource/PrefabCheckTool #&y")]
    public static void ShowWindow()
    {
        if (PrefabCheckTool.instance == null)
        {
            PrefabCheckTool window = EditorWindow.GetWindow<PrefabCheckTool>(false, "PrefabCheckTool", true);
            window.minSize = new Vector2(400f, 500f);
            window.Init();
            window.Show();
        }
        else
        {
            EditorUtility.UnloadUnusedAssetsImmediate();
            System.GC.Collect();
            PrefabCheckTool.instance.Close();
        }
    }

    private void OnEnable()
    {
        PrefabCheckTool.instance = this;
    }

    private void OnDisable()
    {
        PrefabCheckTool.instance = null;
    }

    private void Init()
    {
        _resultList = new List<Object>(32);
        _buttonAudioClip = AssetDatabase.LoadMainAssetAtPath("Assets/GameResources/ArtResources/Audios/Sound/sound_UI/sound_UI_button_click.wav") as AudioClip;
    }

    private void OnGUI()
    {
        _buttonAudioClip = (AudioClip)EditorGUILayout.ObjectField("Button AudioClip:", _buttonAudioClip, typeof(AudioClip), false);
        _targetAtlas = (UIAtlas)EditorGUILayout.ObjectField("UIAtlas:", _targetAtlas, typeof(UIAtlas), false);
        _params = EditorGUILayout.TextField("Input Params:", _params);

        EditorGUILayout.LabelField(string.Format("SearchList: {0}", _searchList == null ? 0 : _searchList.Count));
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Use Current Selections", GUILayout.Height(50f)))
        {
            UseCurSelections();
        }

        if (GUILayout.Button("Select Folder Prefabs", GUILayout.Height(50f)))
        {
            SelectPrefabInFolder();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Check Reference Sprite", GUILayout.Height(50f)))
        {
            CheckReferenceSprite(_targetAtlas, _params);
        }

        if (GUILayout.Button("Check UIButton Sound", GUILayout.Height(50f)))
        {
            CheckUIButtonSound();
        }

        if (GUILayout.Button("Check UIButton Hover And Pressed Color", GUILayout.Height(50f)))
        {
            CheckUIButtonHoverAndPressedColor();
        }

        if (GUILayout.Button("Check UI Pos Fractions", GUILayout.Height(50f)))
        {
            CheckUIPosFractions();
        }

        if(GUILayout.Button("Check UILabel Gradient", GUILayout.Height(50f)))
        {
            CheckLabelGradient();
        }

        //if(GUILayout.Button("Check UIAnchor enable",  GUILayout.Height(50f)))
        //{
        //    CheckUIAnchor();
        //}

        if (GUILayout.Button("检查Prefab是否有丢失脚本,带参数时搜索指定脚本", GUILayout.Height(50f)))
        {
            FindMissingScriptsRecursively(_params);
        }

        if (GUILayout.Button("检查Prefab是否丢失材质球,带参数时搜索指定材质球", GUILayout.Height(50f)))
        {
            FindMissingMaterialsRecursively(_params);
        }

        if (GUILayout.Button("CheckModelNodeName", GUILayout.Height(50f)))
        {
            CheckModelNodeName();
        }

        EditorHelper.DrawHeader(string.Format("Result: {0}", _resultList == null ? 0 : _resultList.Count));
        GUILayout.BeginHorizontal();
        GUILayout.Space(3f);
        GUILayout.BeginVertical(GUILayout.MinHeight(300f));
        if (_resultList != null && _resultList.Count > 0)
        {
            _listScrollPos = EditorGUILayout.BeginScrollView(_listScrollPos);
            for (int i = 0; i < _resultList.Count; ++i)
            {
                Object obj = _resultList[i];
                if (obj != null)
                {
                    GUILayout.Space(-1f);
                    GUI.backgroundColor = _selectedObj == obj ? Color.white : new Color(0.8f, 0.8f, 0.8f);

                    GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));

                    GUI.backgroundColor = Color.white;
                    GUILayout.Label(i.ToString(), GUILayout.Width(40f));

                    if (GUILayout.Button(obj.name, "OL TextField", GUILayout.Height(20f)))
                    {
                        _selectedObj = obj;
                        Selection.activeObject = obj;
                    }
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
        }
        GUILayout.EndVertical();
        GUILayout.Space(3f);
        GUILayout.EndHorizontal();
    }

    #region Helper Func

    private void SelectPrefabInFolder()
    {
        if (Selection.activeObject)
        {
            string folderPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            string[] GUIDs = AssetDatabase.FindAssets("t:prefab", new string[] { folderPath });
            List<UnityEngine.Object> objList = new List<Object>(GUIDs.Length);
            for (int i = 0; i < GUIDs.Length; ++i)
            {
                objList.Add(AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(GUIDs[i])));
            }
            _searchList = objList;
        }
    }

    private void UseCurSelections()
    {
        _searchList = new List<Object>(Selection.objects);
    }

    private void CleanUp()
    {
        _resultList.Clear();
        _goCount = 0;
        _comCount = 0;
        _hitCount = 0;
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

    #endregion

    #region 检查UIButton音效

    private void CheckUIButtonSound()
    {
        CleanUp();
        foreach (var obj in _searchList)
        {
            GameObject go = obj as GameObject;
            CheckUIButtonSoundInGo(go);
            EditorUtility.SetDirty(go);
        }

        Debug.Log(string.Format("Searched {0} GameObjects, check {1} UIButton Sound", _goCount, _hitCount));
        AssetDatabase.SaveAssets();
    }

    private void CheckUIButtonSoundInGo(GameObject go)
    {
        _goCount++;
        UIButton[] uiButtons = go.GetComponents<UIButton>();
        if (uiButtons != null && uiButtons.Length > 0)
        {
            _hitCount++;
            _resultList.Add(go);
            string path = GetHierarchyWithRoot(go);
            if (uiButtons.Length > 1)
            {
                Debug.LogError(string.Format("{0} 存在多于一个UIButton组件，请检查", path));
            }

            UIPlaySound[] uiPlaySounds = go.GetComponents<UIPlaySound>();
            UIPlaySound playSound = null;
            if (uiPlaySounds != null && uiPlaySounds.Length > 0)
            {
                playSound = uiPlaySounds[0];

                //删除冗余的UIPlaySound组件
                if (uiPlaySounds.Length > 1)
                {
                    Debug.LogError(string.Format("{0} 存在多于一个UIPlaySound组件，已清除", path));
                    for (var i = 1; i < uiPlaySounds.Length; i++)
                        GameObject.DestroyImmediate(uiPlaySounds[i], true);
                }
            }

            if (playSound == null)
            {
                playSound = go.AddComponent<UIPlaySound>();
            }

            playSound.audioClip = _buttonAudioClip;
            playSound.trigger = UIPlaySound.Trigger.OnClick;
        }

        //递归遍历子节点
        foreach (Transform child in go.transform)
        {
            CheckUIButtonSoundInGo(child.gameObject);
        }
    }

    #endregion

    #region 检查按钮 hover pressed 颜色
    private void CheckUIButtonHoverAndPressedColor()
    {
        CleanUp();
        foreach (var obj in _searchList)
        {
            GameObject go = obj as GameObject;
            CheckUIButtonHoverAndPressedInGo(go);
            EditorUtility.SetDirty(go);
        }

        Debug.Log(string.Format("Searched {0} GameObjects, check {1} UIButton Hover And PressedColor", _goCount, _hitCount));
        AssetDatabase.SaveAssets();
    }

    private void CheckUIButtonHoverAndPressedInGo(GameObject go)
    {
        _goCount++;
        UIButton[] uiButtons = go.GetComponents<UIButton>();
        if (uiButtons != null && uiButtons.Length > 0)
        {
            _hitCount++;
            _resultList.Add(go);
            string path = GetHierarchyWithRoot(go);
            if (uiButtons.Length > 1)
            {
                Debug.LogError(string.Format("{0} 存在多于一个UIButton组件，请检查", path));
            }

            uiButtons[0].hover = new Color(212f / 255f, 220f / 255f, 235f / 255f, 1f);
            uiButtons[0].pressed = new Color(134f / 255f, 173f / 255f, 246f / 255f, 1f);
        }

        //递归遍历子节点
        foreach (Transform child in go.transform)
        {
            CheckUIButtonHoverAndPressedInGo(child.gameObject);
        }
    }
    #endregion

    #region 检查组件位置小数,修改为整数

    private void CheckUIPosFractions()
    {
        CleanUp();
        foreach (var obj in _searchList)
        {
            GameObject go = obj as GameObject;
            CheckUIPosFractionsInGo(go);
            EditorUtility.SetDirty(go);
        }

        Debug.Log(string.Format("Searched {0} GameObjects, check {1} UI Pos Fractions", _goCount, _hitCount));
        AssetDatabase.SaveAssets();
    }

    private void CheckUIPosFractionsInGo(GameObject go)
    {
        _goCount++;
        Vector3 newPos = new Vector3(Mathf.Round(go.transform.localPosition.x), Mathf.Round(go.transform.localPosition.y), Mathf.Round(go.transform.localPosition.z));
        if (go.transform.localPosition.x - newPos.x != 0 
            || go.transform.localPosition.y - newPos.y != 0
            || go.transform.localPosition.z - newPos.z != 0)
        {
            _hitCount++;
            go.transform.localPosition = newPos;
        }


        UILabel[] uiLabels = go.GetComponents<UILabel>();
        if (uiLabels != null && uiLabels.Length > 0)
        {
            foreach (var lbl in uiLabels)
            {
                if (lbl.gameObject.transform.localScale.x - 1f != 0
                    || lbl.gameObject.transform.localScale.y - 1f != 0
                    || lbl.gameObject.transform.localScale.z - 1f != 0)
                {
                    lbl.gameObject.transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }

        //递归遍历子节点
        foreach (Transform child in go.transform)
        {
            CheckUIPosFractionsInGo(child.gameObject);
        }
    }
    #endregion

    //检查UILabel Gradient属性，若设置，去掉
    void CheckLabelGradient()
    {
        CleanUp();
        foreach (var obj in _searchList)
        {
            GameObject go = obj as GameObject;
            CheckLabelGradientInfoGo(go);
            EditorUtility.SetDirty(go);
        }

        Debug.Log(string.Format("Searched {0} GameObjects, check {1} UILabel Gradient", _goCount, _hitCount));
        AssetDatabase.SaveAssets();
    }

    void CheckLabelGradientInfoGo(GameObject go)
    {
        _goCount++;
        UILabel label = go.GetComponent<UILabel>();
        if(label != null)
        {
            if (label.applyGradient)
            {
                label.applyGradient = false;
                _hitCount++;
            }
        }

        foreach (Transform child in go.transform)
        {
            CheckLabelGradientInfoGo(child.gameObject);
        }
    }

    /// <summary>
    /// NGUI更新导致的Anchor失效
    /// 前版本enable=false，UIAnchor依旧有效，新版本后无效
    /// UIAnchor处理成：当runOnlyOnce=true时，使enable=true
    /// </summary>
    void CheckUIAnchor()
    {
        CleanUp();
        foreach (var obj in _searchList)
        {
            GameObject go = obj as GameObject;
            int preHit = _hitCount;
            CheckUIAnchorInfoGo(go);
            if(preHit != _hitCount)
            {
                EditorUtility.SetDirty(go);
            }
        }

        AssetDatabase.SaveAssets();
    }

    void CheckUIAnchorInfoGo(GameObject go)
    {
        _goCount++;
        UIAnchor com = go.GetComponent<UIAnchor>();
        if (com != null)
        {
            if (com.runOnlyOnce && !com.enabled)
            {
                com.enabled = true;
                _hitCount++;
            }
        }

        foreach (Transform child in go.transform)
        {
            CheckUIAnchorInfoGo(child.gameObject);
        }
    }

    #region 检查无效脚本

    private void FindMissingScriptsRecursively(string targetType)
    {
        CleanUp();
        foreach (var obj in _searchList)
        {
            GameObject go = obj as GameObject;
            FindMissingScriptsInGO(go, targetType);
        }
        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", _goCount, _comCount, _hitCount));
    }

    private void FindMissingScriptsInGO(GameObject go, string targetType)
    {
        _goCount++;
        bool hit = false;
        var components = go.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            Component com = components[i];
            _comCount++;
            if (com == null)
            {
                _hitCount++;
                hit = true;
                Debug.LogError(GetHierarchyWithRoot(go) + " has an empty script attached in position: " + i, go);
            }
            else if (!string.IsNullOrEmpty(targetType) && com.GetType().Name.Contains(targetType))
            {
                _hitCount++;
                hit = true;
                Debug.LogError(GetHierarchyWithRoot(go) + " has an target script attached in position: " + i, go);
            }
        }

        if (hit)
            _resultList.Add(go);
        //递归遍历子节点
        foreach (Transform child in go.transform)
        {
            FindMissingScriptsInGO(child.gameObject, targetType);
        }
    }

    #endregion

    #region 检查无效材质
    private void FindMissingMaterialsRecursively(string targetMatName)
    {
        CleanUp();
        foreach (var obj in _searchList)
        {
            GameObject go = obj as GameObject;
            FindMissingMaterialsInGO(go, targetMatName, go);
        }
        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", _goCount, _comCount, _hitCount));
    }

    private void FindMissingMaterialsInGO(GameObject go, string targetMatName, GameObject prefab)
    {
        _goCount++;
        bool hit = false;
        var renderers = go.GetComponents<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            var renderer = renderers[i];
            _comCount++;
            if (renderer != null)
            {
                var materials = renderer.sharedMaterials;
                if (materials == null || materials.Length == 0)
                {
                    _hitCount++;
                    hit = true;
                    Debug.LogError(GetHierarchyWithRoot(go) + " 存在未设置材质球的Renderer", prefab);
                }
                else
                {
                    for (int index = 0; index < materials.Length; index++)
                    {
                        var mat = materials[index];
                        if (mat != null)
                        {
                            if (mat.name == targetMatName)
                            {
                                _hitCount++;
                                hit = true;
                                Debug.LogError(GetHierarchyWithRoot(go) + " 找到指定材质球", prefab);
                            }
                        }
                        else
                        {
                            _hitCount++;
                            hit = true;
                            Debug.LogError(GetHierarchyWithRoot(go) + " 存在未设置材质球", prefab);
                        }
                    }
                }
            }
        }

        if (hit)
            _resultList.Add(go);
        //递归遍历子节点
        foreach (Transform child in go.transform)
        {
            FindMissingMaterialsInGO(child.gameObject, targetMatName, prefab);
        }
    }
    #endregion

    #region 检查模型节点命名

    private void CheckModelNodeName()
    {
        CleanUp();
        foreach (var obj in _searchList)
        {
            GameObject go = obj as GameObject;
            if (!go.name.Contains("pet_"))
                continue;

            bool hit = false;
            string modelName = go.name.Trim();
            if (modelName != go.name || go.transform.Find(modelName) == null)
            {
                hit = true;
            }

            if (hit)
            {
                _hitCount++;
                go.name = modelName;
                SkinnedMeshRenderer[] modelRenderer = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                if (modelRenderer != null)
                {
                    modelRenderer[0].gameObject.name = modelName;
                }
                else
                    Debug.LogError("Model Renderer is null");

                _resultList.Add(go);
                EditorUtility.SetDirty(go);
            }
        }

        AssetDatabase.SaveAssets();
    }

    #endregion

    #region 检查UIPrefab中是否用到指定图集的指定Sprite

    void CheckReferenceSprite(UIAtlas atlas, string spriteName)
    {
        if (atlas == null || string.IsNullOrEmpty(spriteName))
        {
            Debug.Log("Nothing to check");
            return;
        }

        CleanUp();
        foreach (var obj in _searchList)
        {
            GameObject go = obj as GameObject;
            bool hit = false;
            UISprite[] sprites = go.GetComponentsInChildren<UISprite>(true);
            for (int i = 0; i < sprites.Length; ++i)
            {
                if (sprites[i].atlas == atlas && sprites[i].spriteName.Contains(spriteName))
                {
                    _hitCount++;
                    hit = true;
                    Debug.LogError(GetHierarchyWithRoot(sprites[i].gameObject) + "use target sprite", sprites[i].gameObject);
                }
            }

            if (hit)
            {
                _resultList.Add(go);
            }
        }

        Debug.Log(string.Format("Searched {0} GameObjects, hitCount: {1}", _searchList.Count, _hitCount));
    }

    #endregion
}
