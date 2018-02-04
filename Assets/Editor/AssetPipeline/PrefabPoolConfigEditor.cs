using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;

namespace AssetPipeline
{
    public class PrefabPoolConfigEditor : EditorWindow
    {
        public static PrefabPoolConfigEditor Instance;

        [MenuItem("GameResource/PrefabPoolConfigEditor")]
        public static void ShowWindow()
        {
            if (Instance == null)
            {
                var window = GetWindow<PrefabPoolConfigEditor>(false, "PrefabPoolConfigEditor", true);
                window.minSize = new Vector2(872f, 680f);
                window.Show();
                window.Setup();
            }
            else
            {
                Instance.Close();
            }
        }

        private PrefabPoolConfig _poolConfig;
        private Dictionary<ResGroup, bool> _foldoutDic;
        private Dictionary<ResGroup, List<string>> _bundleNameGroups;

        private void Setup()
        {
            Instance = this;

            _foldoutDic = new Dictionary<ResGroup, bool>();
            var resGroups = Enum.GetValues(typeof(ResGroup));
            foreach (ResGroup flag in resGroups)
            {
                _foldoutDic.Add(flag, EditorPrefs.GetBool("ABFoldOut_" + flag, false));
            }

            _bundleNameGroups = new Dictionary<ResGroup, List<string>>();
            var resGroupEnums = Enum.GetValues(typeof(ResGroup));
            foreach (ResGroup resGroup in resGroupEnums)
            {
                _bundleNameGroups.Add(resGroup, new List<string>());
            }

            string configPath = GetPoolConfigPath();
            if (FileHelper.IsExist(configPath))
            {
                _poolConfig = FileHelper.ReadJsonFile<PrefabPoolConfig>(GetPoolConfigPath());
            }

            if (_poolConfig == null)
                _poolConfig = new PrefabPoolConfig();

            var allBundleNames = AssetDatabase.GetAllAssetBundleNames();
            //筛选出符合的BundleName加入列表中
            foreach (string bundleName in allBundleNames)
            {
                var resGroup = ResConfig.GetResGroupFromBundleName(bundleName);
                if (resGroup == ResGroup.Audio || resGroup == ResGroup.Effect || resGroup == ResGroup.UIPrefab)
                {
                    _bundleNameGroups[resGroup].Add(bundleName);
                }
                else if (resGroup == ResGroup.Model)
                {
                    if (!bundleName.StartsWith("anim_") && !bundleName.EndsWith("_mat"))
                    {
                        _bundleNameGroups[resGroup].Add(bundleName);
                    }
                }
            }
            _bundleNameGroups[ResGroup.None].Add("Default");
        }

        private void OnDestroy()
        {
            Instance = null;
            SavePoolConfig();
        }

        private string _searchFilter = "";
        private string _selectedBundleName = "";
        private Vector2 _bundleNameScroll;
        private Vector2 _poolOptionPanelScroll;

        void OnGUI()
        {
            // Search field
            GUILayout.BeginHorizontal();
            {
                var after = EditorGUILayout.TextField("", _searchFilter, "SearchTextField");

                if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
                {
                    after = "";
                    GUIUtility.keyboardControl = 0;
                }

                if (_searchFilter != null && _searchFilter != after)
                {
                    _searchFilter = after;
                }
            }
            GUILayout.EndHorizontal();

            //BundleName列表
            if (_bundleNameGroups != null && _bundleNameGroups.Count > 0)
            {
                EditorGUILayout.BeginVertical(GUILayout.Height(300f));
                {
                    _bundleNameScroll = EditorGUILayout.BeginScrollView(_bundleNameScroll);
                    foreach (var pair in _bundleNameGroups)
                    {
                        var resGroup = pair.Key;
                        var buildResList = pair.Value;
                        _foldoutDic[resGroup] = EditorGUILayout.Foldout(_foldoutDic[resGroup], resGroup + " Count: " + buildResList.Count);
                        if (_foldoutDic[resGroup])
                        {
                            for (int i = 0; i < buildResList.Count; i++)
                            {
                                string bundleName = buildResList[i];
                                if (!string.IsNullOrEmpty(_searchFilter) &&
                                    bundleName.IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) < 0)
                                    continue;
                                GUILayout.Space(-1f);
                                GUI.backgroundColor = _selectedBundleName == bundleName
                                    ? Color.white
                                    : new Color(0.8f, 0.8f, 0.8f);
                                GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
                                GUI.backgroundColor = Color.white;

                                //编号
                                GUILayout.Label(i.ToString(), GUILayout.Width(40f));

                                if (GUILayout.Button(bundleName, "OL TextField", GUILayout.Height(20f)))
                                {
                                    _selectedBundleName = bundleName;
                                }

                                if (_poolConfig != null)
                                {
                                    if (_poolConfig.config.ContainsKey(bundleName))
                                    {
                                        GUI.backgroundColor = Color.red;
                                        if (GUILayout.Button("X", GUILayout.Width(22f)))
                                        {
                                            if (EditorUtility.DisplayDialog("提示", "删除该资源缓冲池配置,请确认?", "确定",
                                                "取消"))
                                            {
                                                _poolConfig.config.Remove(bundleName);
                                            }
                                        }
                                        GUI.backgroundColor = Color.white;
                                    }
                                    else
                                    {
                                        if (GUILayout.Button("Add", GUILayout.Width(50f)))
                                        {
                                            _poolConfig.config.Add(bundleName, new PrefabPoolOption(1, true, 10, 30, 5, false, 100, true));
                                            _selectedBundleName = bundleName;
                                        }
                                    }
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                GUILayout.Box("ResInfoList is null");
            }

            EditorGUILayout.Space();
            _poolOptionPanelScroll = DrawResInfoDetailPanel(_selectedBundleName, _poolOptionPanelScroll);

            if (GUILayout.Button("保存PrefabPoolConfig", GUILayout.Height(50f)))
            {
                SavePoolConfig();
            }
        }

        private Vector2 DrawResInfoDetailPanel(string bundleName, Vector2 scrollPos)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUILayout.Label(bundleName);
            var poolOption = _poolConfig.GetPoolOption(bundleName);
            if (poolOption != null)
            {
                poolOption.preloadAmount = EditorGUILayout.DelayedIntField("preloadAmount:",
                    poolOption.preloadAmount);
                GUILayout.Label("创建池时预加载数量");
                EditorGUILayout.Space();

                poolOption.preloadTime = EditorGUILayout.Toggle("preloadTime:", poolOption.preloadTime);
                GUILayout.Label("是否分时预加载池对象");
                EditorGUI.BeginDisabledGroup(!poolOption.preloadTime);
                poolOption.preloadFrames = EditorGUILayout.DelayedIntField("preloadFrames:",
                    poolOption.preloadFrames);
                GUILayout.Label("用多少帧来实例化所有的预加载对象");
                poolOption.preloadDelay = EditorGUILayout.DelayedFloatField("preloadDelay:", poolOption.preloadDelay);
                GUILayout.Label("分时预加载启动延迟");
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Space();

                poolOption.limitInstances = EditorGUILayout.Toggle("limitInstances:", poolOption.limitInstances);
                GUILayout.Label("是否限制池对象总数量,开启之后超过限制数量时,将会返回空对象");
                EditorGUI.BeginDisabledGroup(!poolOption.limitInstances);
                poolOption.limitAmount = EditorGUILayout.DelayedIntField("limitAmount:", poolOption.limitAmount);
                GUILayout.Label("只要limitInstances开启时有效,限制该池对象的最大数量");
                poolOption.limitFIFO = EditorGUILayout.Toggle("limitFIFO:", poolOption.limitFIFO);
                GUILayout.Label("开启这个选项后,超过限制总数时,将会把第一个生成的池对象先Despawn,然后再进行Spawn操作,保持总数量不变");
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Space();

                poolOption.cullDespawned = EditorGUILayout.Toggle("cullDespawned:", poolOption.cullDespawned);
                GUILayout.Label("是否自动剔除池对象");
                EditorGUI.BeginDisabledGroup(!poolOption.cullDespawned);
                poolOption.cullAbove = EditorGUILayout.DelayedIntField("cullAbove:", poolOption.cullAbove);
                GUILayout.Label("当池对象总数大于这个阈值时,将会开始剔除冗余的Despawned状态下的对象");
                poolOption.cullDelay = EditorGUILayout.DelayedIntField("cullDelay:", poolOption.cullDelay);
                GUILayout.Label("当池对象总数大于cullAbove时,将会触发剔除计时器,进行剔除操作,计时器会一直重复触发直到池对象总数降回合理的数值");
                poolOption.cullMaxPerPass = EditorGUILayout.DelayedIntField("cullMaxPerPass:",
                    poolOption.cullMaxPerPass);
                GUILayout.Label("每次触发剔除操作时,销毁池对象的最大值");
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();
                poolOption.unloadChangeScene = EditorGUILayout.Toggle("unloadChangeScene:",
                    poolOption.unloadChangeScene);
                GUILayout.Label("在跳转场景时检查是否移除该PrefabPool");
            }
            EditorGUILayout.EndScrollView();

            return scrollPos;
        }

        private void SavePoolConfig()
        {
            if (_poolConfig == null) return;
            FileHelper.SaveJsonObj(_poolConfig, GetPoolConfigPath());
            AssetDatabase.Refresh();
        }

        private string GetPoolConfigPath()
        {
            return "Assets/GameResources/ConfigFiles/" + PrefabPoolConfig.POOLCONFIG_FILE + ".json";
        }
    }
}