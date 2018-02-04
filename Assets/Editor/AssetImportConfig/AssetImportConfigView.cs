using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using LITJson;
using UnityEngine;
using UnityEditor;

namespace AssetImport
{
    public class AssetImportConfigView : EditorWindow
    {
        public static AssetImportConfigView Instance;

        [MenuItem("美术/资源导入设置", false, 120)]
        public static void ShowWindow()
        {
            if (Instance == null)
            {
                Instance = GetWindow<AssetImportConfigView>(false, "AssetImportSetting", true);
                Instance.minSize = new Vector2(1000f, 680f);
                Instance.Show();
                Instance.Setup();
            }
            else
            {
                Instance.Close();
                Instance = null;
            }
        }

        private AssetImportConfig assetImportConfig;
        private string searchFilter;
        private AssetType selectAssetType;
        private AssetImportHelperBase helper;
        private List<string> assets;
        private List<string> matchList;
        private void Setup()
        {
            assetImportConfig = AssetImportConfig.LoadConfig();
            changeConfigAssets = new HashSet<string>();
        }
        private Vector2 assetScroll;
        private Vector2 assetConfigScroll;
        private string selectAssetPath;
        private bool selectAssetIsDefault;
        private bool selectAssetChangeConfig;
        private HashSet<string> changeConfigAssets;
        private PageHelper pageHelper;
        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            {
                selectAssetType = (AssetType)EditorGUILayout.EnumPopup(selectAssetType);
            }
            GUILayout.EndHorizontal();
            // Search field
            GUILayout.BeginHorizontal();
            {
                var after = EditorGUILayout.TextField("", searchFilter, "SearchTextField");

                if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
                {
                    after = string.Empty;
                    GUIUtility.keyboardControl = 0;
                }
                if (searchFilter != after)
                {
                    searchFilter = after;
                    matchList = assets.Where(path =>
                    {
                        string fileName = Path.GetFileNameWithoutExtension(path);
                        if (helper != null)
                        {
                            bool isDefault;
                            helper.GetAssetConfig(assetImportConfig, path, out isDefault);
                            if (!isDefault)
                                fileName = fileName.Insert(0, "!");
                        }
                        return string.IsNullOrEmpty(searchFilter) || MatchFilter(fileName);
                    }).ToList();
                    pageHelper.itemCount = matchList.Count;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("查找所有资源", GUILayout.Height(50f)))
                {
                    GetAllAssets();
                }
                if (GUILayout.Button("重新设置所有资源配置", GUILayout.Height(50f)))
                {
                    SetAllConfig();
                }
                if (GUILayout.Button("保存配置", GUILayout.Height(50f)))
                {
                    SaveConfig();
                }
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("HelpBox", GUILayout.Height(350f));
            {
                assetScroll = EditorGUILayout.BeginScrollView(assetScroll);
                if (assets != null)
                {
                    int i = 0;
                    foreach (var path in matchList)
                    {
                        i++;
                        if (pageHelper.CheckOutPage(i))
                            continue;
                        string fileName = Path.GetFileNameWithoutExtension(path);
                        GUI.backgroundColor = selectAssetPath == path ? Color.white : new Color(0.8f, 0.8f, 0.8f);
                        GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
                        {
                            GUILayout.Label(i.ToString(), GUILayout.Width(40f));
                            GUILayout.BeginHorizontal();
                            if ((GUILayout.Button(fileName, "OL TextField", GUILayout.Height(20f), GUILayout.Width(250f)) || GUILayout.Button(" : " + path, "OL TextField", GUILayout.Height(20f))) && path != selectAssetPath)
                            {
                                selectAssetPath = path;
                                selectAssetChangeConfig = false;
                            }
                            GUILayout.EndHorizontal();
                            if (selectAssetPath == path)
                            {
                                if (selectAssetIsDefault)
                                {
                                    GUI.backgroundColor = Color.green;
                                    if (GUILayout.Button("Add", GUILayout.Width(50f)))
                                        AddConfig();
                                }
                                else
                                {
                                    GUI.backgroundColor = Color.red;
                                    if (GUILayout.Button("X", GUILayout.Width(22f)))
                                        RemoveConfig();
                                }
                            }
                            GUI.backgroundColor = Color.white;
                        }
                        GUILayout.EndHorizontal();
                        GUI.backgroundColor = Color.white;

                    }
                }

                EditorGUILayout.EndScrollView();
                if (pageHelper != null)
                {
                    pageHelper.DrawGUI(ref assetScroll);
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical("HelpBox");
            selectAssetIsDefault = true;
            if (selectAssetPath != null && helper != null)
            {
                assetConfigScroll = EditorGUILayout.BeginScrollView(assetConfigScroll);
                var itemConfig = helper.GetAssetConfig(assetImportConfig, selectAssetPath, out selectAssetIsDefault);
                EditorGUI.BeginChangeCheck();
                EditorGUI.BeginDisabledGroup(selectAssetIsDefault);
                helper.DrawAssetConfigGUI(itemConfig);
                EditorGUI.EndDisabledGroup();
                selectAssetChangeConfig |= EditorGUI.EndChangeCheck();
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(selectAssetPath);
                if (selectAssetChangeConfig)
                    changeConfigAssets.Add(selectAssetPath);
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private void GetAllAssets()
        {
            if (selectAssetType == AssetType.None)
            {
                ShowNotification(new GUIContent("请选择有效的资源类型"));
                return;
            }
            helper = (AssetImportHelperBase)Assembly.GetExecutingAssembly().CreateInstance(selectAssetType.GetType().Namespace + "." + selectAssetType + AssetImportHelperBase.suffix);
            assets = helper.GetAllAsset()
                        .OrderBy(item => Path.GetFileNameWithoutExtension(item))
                        .ToList();
            matchList = assets;
            pageHelper = new PageHelper(assets.Count, 700);
        }

        private void AddConfig()
        {
            if (selectAssetPath != null && helper != null)
            {
                assetImportConfig.GetAtlasConfig(selectAssetType).Add(Path.GetFileNameWithoutExtension(selectAssetPath), helper.GetDefaultConfig(selectAssetPath));
                changeConfigAssets.Add(selectAssetPath);
            }
        }

        private void RemoveConfig()
        {
            if (selectAssetPath != null && helper != null)
            {
                assetImportConfig.GetAtlasConfig(selectAssetType).Remove(Path.GetFileNameWithoutExtension(selectAssetPath));
                changeConfigAssets.Add(selectAssetPath);
            }

        }
        private void SaveConfig()
        {
            assetImportConfig.SaveConfig();
            if (changeConfigAssets.Count > 0)
            {
                AssetImportPost.OnPostprocessAllAssets(changeConfigAssets.ToArray(), new string[0], new string[0], new string[0]);
                changeConfigAssets.Clear();
            }
            if (EditorUtility.DisplayDialog("提示", "保存成功,是否提交配置", "提交", "取消"))
            {
                Process.Start(Application.dataPath +  "/Editor/AssetImportConfig/ConfigFile");
            }
        }

        private void SetAllConfig()
        {
            if (assets == null)
            {
                ShowNotification(new GUIContent("请先“查找所有资源”"));
                return;
            }
            assetImportConfig.SaveConfig();
            AssetImportPost.OnPostprocessAllAssets(assets.ToArray(), new string[0], new string[0], new string[0]);
            changeConfigAssets.Clear();
            ShowNotification(new GUIContent("设置成功"));
        }

        private bool MatchFilter(string fileName)
        {
            //键入全是小写时 搜索不区分大小写。
            string fileNameLower = searchFilter == searchFilter.ToLower() ? fileName.ToLower() : fileName;
            int index = 0;
            foreach (char c in searchFilter)
            {
                index = fileNameLower.IndexOf(c, index);
                if (index == -1)
                    return false;
                else
                    index++;
            }
            return true;
        }

        private class PageHelper
        {
            public PageHelper(int itemCount, int pageMaxItem)
            {
                this._itemCount = itemCount;
                this.pageMaxItem = pageMaxItem;
            }

            private int _itemCount;
            public int itemCount
            {
                get { return _itemCount; }
                set
                {
                    _itemCount = value;
                    curPage = Mathf.Clamp(curPage, 0, maxPage);
                }
            }

            private int _pageMaxItem;

            public int pageMaxItem
            {
                get { return _pageMaxItem; }
                set
                {
                    _pageMaxItem = value;
                    curPage = Mathf.Clamp(curPage, 0, maxPage);
                }
            }
            public int curPage { get; private set; }

            public int maxPage
            {
                get
                {
                    return (itemCount == 0 ? 1 :
                            itemCount / pageMaxItem + (itemCount % pageMaxItem == 0 ? 0 : 1)) - 1;
                }
            }
            public bool CheckOutPage(int index)
            {
                return (index < curPage * pageMaxItem || index >= (curPage + 1) * pageMaxItem);
            }

            public string GetCurPageLabel()
            {
                return string.Format("{0}/{1}", curPage + 1, maxPage + 1);
            }

            public void AddPage()
            {
                curPage = Mathf.Min(curPage + 1, maxPage);
            }

            public void SubPage()
            {
                curPage = Mathf.Max(curPage - 1, 0);
            }

            public void DrawGUI(ref Vector2 assetScroll)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("上一页"))
                {
                    SubPage();
                    assetScroll.y = 0f;
                }
                var style = GUI.skin.label;
                style.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.LabelField(GetCurPageLabel(), style);
                if (GUILayout.Button("下一页"))
                {
                    AddPage();
                    assetScroll.y = 0f;
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}

