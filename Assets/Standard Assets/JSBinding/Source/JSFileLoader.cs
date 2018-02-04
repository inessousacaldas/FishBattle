using UnityEngine;
using System.Collections;
using System.IO;
using AssetPipeline;
using System;

/*
 * JSFileLoader
 * All js files are loaded by this class.
 */
public static class JSFileLoader
{
    private static bool _loadFinished;

    public static byte[] LoadJSSync(string jsScriptName)
    {
        if (_loadFinished)
        {
            Debug.LogError("JS脚本应该全都load完了，请检查！");
        }
        string filePath = GetJsScriptPath(jsScriptName);
        if (AssetManager.ResLoadMode == AssetManager.LoadMode.EditorLocal)
        {
            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }
        byte[] bytes = null; 
        var textAsset = AssetManager.Instance.LoadAsset<TextAsset>(GameResPath.AllScriptBundleName, filePath + ".min.bytes"); 
        if (textAsset != null)
        {
            bytes = textAsset.bytes;
        }
        else
        {
            textAsset = AssetManager.Instance.LoadAsset<TextAsset>(GameResPath.AllScriptBundleName, filePath);
            if (textAsset != null)
                bytes = textAsset.bytes;
        }
        return bytes;
    }

    public static void StartLoading()
    {
        _loadFinished = false;
    }
    public static void EndLoading()
    {
        AssetManager.Instance.UnloadBundle(GameResPath.AllScriptBundleName);
        _loadFinished = true;
    }

    /// <summary>
    /// Gets the full name of the javascript file.
    /// </summary>
    /// <param name="jsScriptName">The short name.</param>
    /// <returns></returns>
    public static string GetJsScriptPath(string jsScriptName)
    {
        if (AssetManager.ResLoadMode == AssetManager.LoadMode.EditorLocal)
        {
            if (jsScriptName.EndsWith(".json"))
                return jsScriptName;

            string minPath = JSPathSettings.jsDir + "/" + jsScriptName + ".min" + JSPathSettings.jsExtension;
            if (File.Exists(minPath))
            {
                return minPath;
            }
            return JSPathSettings.jsDir + "/" + jsScriptName + JSPathSettings.jsExtension;
        }
        else
        {
            return Path.GetFileNameWithoutExtension(jsScriptName);
        }
    }
}
