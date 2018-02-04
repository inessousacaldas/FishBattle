using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif


public static class EditorRuntimeHelper
{
    public static T LoadAssetAtPath<T>(string assetPath) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        return (T)AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
#else
        return Resources.Load<T>(assetPath);
#endif
    }
}
