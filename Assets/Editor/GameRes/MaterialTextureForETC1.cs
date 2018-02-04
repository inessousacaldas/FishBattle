using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;
using System.Text;
using LITJson;
using AssetImport;
using ICSharpCode.SharpZipLib.Checksums;

public class MaterialTextureForETC1
{
    private static string defaultWhiteTexPath_relative = "Assets/UI/Default_Alpha.png";
    private static Texture2D defaultWhiteTex = null;
    private static AssetImportConfig assetConfig;
    private static AssetImportHelperBase helper;
    private static void ReloadConfig()
    {
        assetConfig = AssetImportConfig.LoadConfig();
        helper = AssetImportHelperBase.GetAllAssetImportHelper()[AssetType.AtlasOrFont];
    }

    static private bool IsNormalMaterial(string path, BuildTarget buildTarget, out AtlasOrFontConfig config)
    {
        bool isDefault;
        config = (AtlasOrFontConfig)helper.GetAssetConfig(assetConfig, Path.ChangeExtension(path, ".prefab"), out isDefault);
        TextureImporterFormat format = config.GetFormatByTarget(buildTarget);
        return buildTarget == BuildTarget.iOS
            ? format != TextureImporterFormat.PVRTC_RGB4 || !config.stripAlpha
            : format != TextureImporterFormat.AutomaticCompressed || !config.stripAlpha;

    }
    [MenuItem("GameResource/Depart RGB and Alpha Channel")]
    static void SeperateAllTexturesRGBandAlphaChannel()
    {
        if (EditorUtility.DisplayDialog("处理确认", "是否确认分离图集贴图?", "确认", "取消"))
        {
            StartDepart();
        }
    }

    public static void StartDepart()
    {
        if (new List<BuildTarget> { BuildTarget.Android, BuildTarget.iOS }.Contains(EditorUserBuildSettings.activeBuildTarget) == false)
        {
            EditorUtility.DisplayDialog("平台错误", "当前不在 Android 或 iOS 平台，不做分离处理", "确定");
            throw new Exception("当前不在 Android 或 iOS 平台，不做分离处理");
        }

        DoSeperateAllTexturesRGBandAlphaChannel(EditorUserBuildSettings.activeBuildTarget);
    }
    static void DoSeperateAllTexturesRGBandAlphaChannel(BuildTarget buildTarget)
    {
        if (!GetDefaultWhiteTexture())
        {
            return;
        }
        ReloadConfig();

        string[] searchInFolders = { "Assets/UI" };
        string[] guids = AssetDatabase.FindAssets("t:Material", searchInFolders);

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            Material material = AssetDatabase.LoadMainAssetAtPath(path) as Material;
            if (material == null)
            {
                Debug.LogError("无效材质路径: " + path);
                continue;
            }
            AtlasOrFontConfig config;
            if (IsNormalMaterial(path, buildTarget, out config))
            {
                material.shader = Shader.Find("Unlit/Transparent Colored");
                if (material.HasProperty("_MainTex"))
                {
                    // 设置材质
                    string mainTexPath = Path.ChangeExtension(path, ".png");
                    Texture mainTex = AssetDatabase.LoadMainAssetAtPath(mainTexPath) as Texture;
                    if (mainTex != null)
                        material.SetTexture("_MainTex", mainTex);
                }
            }
            else
            {
                material.shader = Shader.Find("Unlit/Transparent RGB Alpha");
                if (material.HasProperty("_MainTex") && material.HasProperty("_AlphaTex"))
                {
                    var texturePath = Path.ChangeExtension(path, ".png");
                    if (string.IsNullOrEmpty(texturePath))
                    {
                        throw new Exception("严重错误,文件不存在: " + texturePath);
                    }

                    if (ValidateTexture(texturePath))
                    {
                        string mainTexPath = string.Empty;
                        string alphaTexPath = string.Empty;
                        // 分离图片
                        SeperateRGBAandAlphaChannel(texturePath, buildTarget, out mainTexPath, out alphaTexPath, config.alphaMip ? 1 : 0);

                        if (!string.IsNullOrEmpty(mainTexPath) && !string.IsNullOrEmpty(alphaTexPath))
                        {
                            // 设置材质
                            Texture mainTex = AssetDatabase.LoadMainAssetAtPath(mainTexPath) as Texture;
                            material.SetTexture("_MainTex", mainTex);
                            Texture alphaTex = AssetDatabase.LoadMainAssetAtPath(alphaTexPath) as Texture;
                            material.SetTexture("_AlphaTex", alphaTex);
                        }
                    }
                    else if (material.GetTexture("_AlphaTex") == null)
                    {
                        string alphaTexPath = path.Substring(0, path.LastIndexOf(".")) + "_Alpha.png";
                        if (File.Exists(alphaTexPath))
                        {
                            Texture alphaTex = AssetDatabase.LoadMainAssetAtPath(alphaTexPath) as Texture;
                            material.SetTexture("_AlphaTex", alphaTex);
                        }
                        else
                        {
                            throw new Exception(string.Format("{0} AlphaTex or rawTex is Null", path));
                        }
                    }
                }
            }
        }

        //Refresh to ensure new generated RBA and Alpha textures shown in Unity as well as the meta file
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    #region process texture
    private static Texture2D sourcetex = null;
    static void SeperateRGBAandAlphaChannel(string texPath, BuildTarget buildTarget, out string mainTexPath, out string alphaTexPath, int alphaTexLevel)
    {
        mainTexPath = string.Empty;
        alphaTexPath = string.Empty;
        sourcetex = null;
        string assetRelativePath = GetRelativeAssetPath(texPath);
        if (assetRelativePath.IndexOf("NGUI") != -1)
        {
            //Debug.Log("NGUI 图片不处理");
            return;
        }

        byte[] sourcebytes = File.ReadAllBytes(assetRelativePath);

        var check = CheckIsPNG(sourcebytes);
        if (!check)
        {
            throw new Exception("图片不是png格式：" + assetRelativePath);
        }

        sourcetex = new Texture2D(0, 0);
        sourcetex.LoadImage(sourcebytes, false);

        int width, rawWidth, height, rawHegiht;
        width = rawWidth = sourcetex.width;
        height = rawHegiht = sourcetex.height;

        if (buildTarget == BuildTarget.iOS && rawWidth != rawHegiht)
        {
            width = height = Mathf.Max(rawWidth, rawHegiht);
        }

        Texture2D rgbTex = new Texture2D(width, height, TextureFormat.RGB24, false);
        rgbTex.SetPixels(
            0,
            rawHegiht >= rawWidth ? 0 : height - rawHegiht,
            rawWidth,
            rawHegiht,
            sourcetex.GetPixels());

        byte[] bytes = rgbTex.EncodeToPNG();
        mainTexPath = GetRGBTexPath(texPath);

        File.WriteAllBytes(mainTexPath, bytes);
        if (rgbTex != null)
        {
            ReImportAsset(mainTexPath, rgbTex.width, rgbTex.height);
        }
        else
        {
            Debug.LogError("MainTex数据,莫名其妙丢失呐: " + mainTexPath);
        }

        //Alpha Channel needed here
        Texture2D mipMapTex = new Texture2D(rawWidth, rawHegiht, TextureFormat.RGBA32, true);
        mipMapTex.SetPixels(sourcetex.GetPixels());
        mipMapTex.Apply();

        //Second level of Mipmap
        Color[] colors2rdLevel = mipMapTex.GetPixels(alphaTexLevel);
        Color[] colorsAlpha = new Color[colors2rdLevel.Length];

        bool bAlphaExist = false;
        for (int i = 0; i < colors2rdLevel.Length; ++i)
        {
            float a = colors2rdLevel[i].a;
            colorsAlpha[i] = new Color(a, a, a);
            if (!Mathf.Approximately(colors2rdLevel[i].a, 1.0f))
            {
                bAlphaExist = true;
            }
        }
        Vector2 size = bAlphaExist ? new Vector2(width / (alphaTexLevel + 1), height / (alphaTexLevel + 1)) : new Vector2(defaultWhiteTex.width, defaultWhiteTex.height);

        var alphaTex = new Texture2D((int)size.x, (int)size.y, TextureFormat.RGB24, false);
        alphaTex.SetPixels(
            0,
            rawHegiht >= rawWidth ? 0 : (int)size.y - rawHegiht / (alphaTexLevel + 1),
            rawWidth / (alphaTexLevel + 1),
            rawHegiht / (alphaTexLevel + 1),
            colorsAlpha);

        alphaTex.Apply();
        byte[] alphabytes = alphaTex.EncodeToPNG();
        alphaTexPath = GetAlphaTexPath(texPath);
        File.WriteAllBytes(alphaTexPath, alphabytes);
        if (alphaTex != null)
        {
            ReImportAsset(alphaTexPath, alphaTex.width, alphaTex.height);
        }
        else
        {
            Debug.LogError("AlphaTex数据,莫名其妙丢失呐: " + alphaTexPath);
        }
    }

    static void ReImportAsset(string path, int width, int height)
    {
        try
        {
            AssetDatabase.ImportAsset(path);
        }
        catch
        {
            Debug.LogError("Import Texture failed: " + path);
            return;
        }

        TextureImporter importer = null;
        try
        {
            importer = (TextureImporter)TextureImporter.GetAtPath(path);
        }
        catch
        {
            Debug.LogError("Load Texture failed: " + path);
            return;
        }
        if (importer == null)
        {
            return;
        }

        importer.SaveAndReimport();
    }

    static bool GetDefaultWhiteTexture()
    {
        //not just the textures under Resources file  
        defaultWhiteTex = AssetDatabase.LoadAssetAtPath(defaultWhiteTexPath_relative, typeof(Texture2D)) as Texture2D;
        if (!defaultWhiteTex)
        {
            Debug.LogError("Load Texture Failed : " + defaultWhiteTexPath_relative);
            return false;
        }
        return true;
    }

    #endregion

    #region string or path helper

    static bool IsTextureFile(string _path)
    {
        string path = _path.ToLower();
        return path.EndsWith(".psd") || path.EndsWith(".tga") || path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".bmp") || path.EndsWith(".tif") || path.EndsWith(".gif");
    }

    static bool IsTextureConverted(string _path)
    {
        return _path.Contains("_RGB.") || _path.Contains("_Alpha.");
    }

    static string GetRGBTexPath(string _texPath)
    {
        return GetTexPath(_texPath, "_RGB.");
    }

    static string GetAlphaTexPath(string _texPath)
    {
        return GetTexPath(_texPath, "_Alpha.");
    }

    static string GetTexPath(string _texPath, string _texRole)
    {
        string dir = Path.GetDirectoryName(_texPath);
        string filename = Path.GetFileNameWithoutExtension(_texPath);
        string result = dir + "/" + filename + _texRole + "png";
        return result;
    }

    static string GetRelativeAssetPath(string _fullPath)
    {
        _fullPath = GetRightFormatPath(_fullPath);
        int idx = _fullPath.IndexOf("Assets");
        string assetRelativePath = _fullPath.Substring(idx);
        return assetRelativePath;
    }

    static string GetRightFormatPath(string _path)
    {
        return _path.Replace("\\", "/");
    }

    #endregion

    #region Helper

    private static bool ValidateTexture(string texturePath)
    {
        return !string.IsNullOrEmpty(texturePath)
        && IsTextureFile(texturePath)
        && !IsTextureConverted(texturePath)
        && Check_isChange(texturePath);
    }
    private static bool Check_isChange(string path)
    {
        var crcCodeKey = "crcCode";
        var importer = TextureImporter.GetAtPath(path);
        if (importer == null)
        {
            throw new ArgumentNullException(string.Format("{0} Can Not Find", path));
        }
        bool isDefault;
        JsonData deJd = JsonMapper.ToObject(importer.userData);
        var bytes = File.ReadAllBytes(path);
        Crc32 crc32 = new Crc32();
        crc32.Update(bytes);
        crc32.Update(Encoding.UTF8.GetBytes(JsonMapper.ToJson((AtlasOrFontConfig)helper.GetAssetConfig(assetConfig, Path.ChangeExtension(path, ".prefab"), out isDefault))));
        uint b = (uint)crc32.Value;

        var isChange = false;

        if (!((IDictionary)deJd).Contains("crcCode"))
        {
            isChange = true;
        }
        else
        {
            uint crcCode = 0;
            uint.TryParse(deJd[crcCodeKey].ToJson(), out crcCode);
            isChange = crcCode != b;
        }

        if (isChange)
        {
            deJd[crcCodeKey] = b;
            importer.userData = JsonMapper.ToJson(deJd);
            importer.SaveAndReimport();
        }
        return isChange;
    }

    private static byte[] pngHead = new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a };

    public static bool CheckIsPNG(byte[] pngBytes)
    {
        for (int i = 0; i < pngHead.Length; i++)
        {
            if (pngBytes[i] != pngHead[i])
                return false;
        }
        return true;
    }

    #endregion
}