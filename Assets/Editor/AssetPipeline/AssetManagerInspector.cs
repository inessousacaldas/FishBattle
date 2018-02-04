using System;
using UnityEngine;
using System.Collections;
using AssetPipeline;
using UnityEditor;

[CustomEditor(typeof(AssetManager))]
public class AssetManagerInspector : Editor
{
    private AssetManager _mgr;
    private static string _searchFilter = "";
    private string _selectedBundleName = "";
    private Vector2 _bundleNameScroll;
    void OnEnable()
    {
        _mgr = target as AssetManager;
    }

    public override void OnInspectorGUI()
    {
        if (_mgr == null)
            return;

        _mgr.AutoUpgrade = (AssetManager.AutoUpgradeType)EditorGUILayout.EnumPopup(_mgr.AutoUpgrade);

        var abInfoDic = _mgr.AbInfoDic;
        if (abInfoDic == null) return;

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

        GUILayout.Label("abInfoDic Count: " + abInfoDic.Count);
        _bundleNameScroll = EditorGUILayout.BeginScrollView(_bundleNameScroll);
        foreach (var pair in abInfoDic)
        {
            string bundleName = pair.Key;
            var abInfo = pair.Value;
            if (!string.IsNullOrEmpty(_searchFilter) &&
                                    bundleName.IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) < 0)
                continue;

            GUILayout.Space(-1f);
            GUI.backgroundColor = _selectedBundleName == bundleName
                ? Color.white
                : new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
            GUI.backgroundColor = Color.white;

            if (GUILayout.Button(bundleName, "OL TextField", GUILayout.Height(20f)))
            {
                _selectedBundleName = bundleName;
            }

            if (abInfo.refBundleNames.Count > 0 && GUILayout.Button("Log", GUILayout.Width(50f)))
            {
                string content = "";
                foreach (string refBundleName in abInfo.refBundleNames)
                {
                    content += refBundleName + ",";
                }
                Debug.LogError("RefBundleNames: " + content);
            }

            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

}
