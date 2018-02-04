#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Object = UnityEngine.Object;

/// <summary>
/// 找到这几个类：MonolessViewController、MonolessViewControllerV1或者其它类似的
/// 在以上几个类中的构造方法或者Setup方法中加入以下代码
/// #if UNITY_EDITOR
/// gameObject.GetMissingComponent<OpenScript>().type = GetType();
/// #endif
/// /// </summary>
[CustomEditor(typeof(OpenScript))]
public class TestOpenScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.ObjectField("Script", TestOpenScriptData.GetObjectBName(((OpenScript)target).className), typeof(UnityEngine.Object), false);
    }
}

public class OpenScript : MonoBehaviour
{
    string mClassName = string.Empty;
    public string className { get { return mClassName; } }
    public Type type
    {
        set { if (value != null) mClassName = value.Name; }
    }
}

[InitializeOnLoad]
public class TestOpenScriptData : Editor
{
    static bool mCanSave;
    static Dictionary<string, string> mPathDics = new Dictionary<string, string>();
    static TestOpenScriptData()
    {
        mCanSave = true;
        EditorApplication.playmodeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playmodeStateChanged += OnPlayModeStateChanged;
    }
    static void OnPlayModeStateChanged()
    {
        if (mCanSave) SaveFile();
        mCanSave = false;
    }

    static public void SaveFile()
    {
        if (mPathDics != null) mPathDics.Clear();

        var tDirectory = new DirectoryInfo(Path.Combine(Application.dataPath, "Scripts/MyGameScripts"));
        var tPattern = string.Format(@"public\s+\S*\s*class\s+(?<ClassName>.*?)\s*:");
        foreach (var item in tDirectory.GetFiles("*.cs", SearchOption.AllDirectories))
        {
            using (StreamReader tReader = new StreamReader(item.FullName))
            {
                var tScriptContent = tReader.ReadToEnd();
                var tMatches = Regex.Matches(tScriptContent, tPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                foreach (Match m in tMatches) AddPathToCache(m.Groups["ClassName"].Value, item.FullName);
            }
        }
    }
    static public Object GetObjectBName(string pClassName)
    {
        if (string.IsNullOrEmpty(pClassName) || !mPathDics.ContainsKey(pClassName) || string.IsNullOrEmpty(mPathDics[pClassName])) return null;
        var tPath = mPathDics[pClassName].Replace("\\", "/").Substring(Application.dataPath.Length - "Assets".Length);
        var tObj = AssetDatabase.LoadAssetAtPath(tPath, typeof(Object));
        return tObj;
    }
    static void AddPathToCache(string pKey, string pVal)
    {
        if (mPathDics.ContainsKey(pKey)) mPathDics[pKey] = pVal;
        else mPathDics.Add(pKey, pVal);
    }
}
#endif