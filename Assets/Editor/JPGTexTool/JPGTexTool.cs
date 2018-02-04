using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AssetPipeline;
using ICSharpCode.SharpZipLib.Checksums;
using LITJson;
using UnityEditor;
using UnityEngine;


public class JPGTexTool
{
    const string SceneRawDataPath = "Assets/GameResources/ArtResources/Scenes2d/SceneRawData";
    [MenuItem("Tools/JPGTexTool/CurBuildTarget")]

    public static void BuildTextureCur()
    {
        BuildTexture(AssetBundleBuilder.GetExportBundlePath());
        EditorUtility.DisplayDialog("提示", "转换完成", "确定");

    }

    public static uint BuildTexture(string exportDir)
    {
        JPGTexTool instance = new JPGTexTool()
        {
            exportDir = exportDir,
        };
        instance.StartBuild();
        return instance.allTexCRC32;
    }
    private string exportDir;
    private Queue<TexConverInfo> allTexConverInfo;
    private HashSet<string> allTexSet = new HashSet<string>();
    private uint allTexCRC32;
    private bool _showProgressBar = true;
    private void StartBuild()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            _showProgressBar = false;
        }
        FindAllTex();
        ConverAllJPG();
        stopwatch.Stop();
    }

    private void FindAllTex()
    {
        try
        {
            allTexConverInfo = new Queue<TexConverInfo>();
            List<string> GUIDs =
                   new List<string>(AssetDatabase.FindAssets("t: Texture", new string[] { SceneRawDataPath }));
            GUIDs.Sort();
            Crc32 crc32 = new Crc32();
            for (int i = 0; i < GUIDs.Count; i++)
            {
                if (_showProgressBar)
                    EditorUtility.DisplayProgressBar("提示", string.Format("正在检测需要更新的图片{0}/{1}", i, GUIDs.Count),
                        (float)i / GUIDs.Count);
                string guid = GUIDs[i];
                string resPath = AssetDatabase.GUIDToAssetPath(guid);

                if (resPath.Contains("tilemap_") == false) continue;
                string fileName = Path.GetFileNameWithoutExtension(resPath);
                allTexSet.Add(fileName);

                uint sourceFileCRC = 0;
                if (CheckTexNeedUpdate(resPath, crc32, out sourceFileCRC))
                    allTexConverInfo.Enqueue(new TexConverInfo()
                    {
                        filePath = resPath,
                        sourceFileCRC = sourceFileCRC,
                        bundleName = string.Concat(ResGroup.TileMap.ToString().ToLower(), '/', fileName),
                    });
            }
            allTexCRC32 = (uint)crc32.Value;
        }
        finally
        {
            if (_showProgressBar)
                EditorUtility.ClearProgressBar();
        }
    }

    private bool CheckTexNeedUpdate(string filePath, Crc32 crc32, out uint crc)
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);
        crc = CRC32Hashing.HashBytes(fileBytes);
        crc32.Update(fileBytes);
        string jsonFilePath = Path.Combine(GetExportPath(), Path.GetFileNameWithoutExtension(filePath) + ".json");
        if (File.Exists(jsonFilePath))
        {
            TexConverInfo texConverInfo = JsonMapper.ToObject<TexConverInfo>(File.ReadAllText(jsonFilePath));
            if (crc == texConverInfo.sourceFileCRC)
            {
                return false;
            }
        }
        return true;
    }

    public void ConverAllJPG()
    {
        Directory.CreateDirectory(GetExportPath());
        int allCount = allTexConverInfo.Count;
        while (allTexConverInfo.Count != 0)
        {
            TexConverInfo texConverInfo = allTexConverInfo.Dequeue();
            try
            {
                int finishCount = allCount - allTexConverInfo.Count;
                if (_showProgressBar)
                    EditorUtility.DisplayProgressBar("正在转换", string.Format("转换{0}/{1}", finishCount, allCount), (float)finishCount / allCount);
                ConverToJPG(texConverInfo);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(texConverInfo.ToString() + "\n" + ex.ToString());
            }

        }
        if (_showProgressBar)
            EditorUtility.ClearProgressBar();
    }
    public void ConverToJPG(TexConverInfo texConverInfo)
    {
        string filePath = texConverInfo.filePath;
        byte[] pngBytes = File.ReadAllBytes(filePath);
        if (!CheckIsPNG(pngBytes))
            return;
        int width = BitConverter.ToUInt16(pngBytes, 16);
        int hegith = BitConverter.ToUInt16(pngBytes, 20);
        Texture2D pngTexture2D = new Texture2D(width, hegith);
        pngTexture2D.LoadImage(pngBytes);

        byte[] jpgBytes = pngTexture2D.EncodeToJPG(75);

        string jpgFilePath = Path.Combine(exportDir + "/custom", texConverInfo.bundleName);
        File.WriteAllBytes(jpgFilePath, jpgBytes);

        string json = JsonMapper.ToJson(texConverInfo);
        string jsonFileName = Path.GetFileName(jpgFilePath + ".json");
        File.WriteAllText(Path.Combine(GetExportPath(), jsonFileName), json);

    }
    static byte[] pngHead = new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a };

    static bool CheckIsPNG(byte[] pngBytes)
    {
        for (int i = 0; i < pngHead.Length; i++)
        {
            if (pngBytes[i] != pngHead[i])
                return false;
        }
        return true;
    }

    public class TexConverInfo
    {
        public string filePath;
        public string bundleName;
        public uint sourceFileCRC;

        public override string ToString()
        {
            return filePath;
        }
    }
    private string GetExportPath()
    {
        return string.Concat(exportDir, "/custom/tilemap");
    }
}
