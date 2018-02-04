using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LITJson;
using UnityEditor;

public class CustomAssetTool
{
    private static string[] ClearPath = new[] { "Assets/GameResources/TempRes" };

    [MenuItem("Assets/资源导入/清理AssetBundleName", false, 110)]
    public static void ClearBundleNameBySelect()
    {
        string[] guids = Selection.assetGUIDs;
        string[] path = new string[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            path[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
        }
        ClearBundleNameByDir(path);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("提示", "清理完成", "确定");
    }

    public static void ClearBundleNameByConfig()
    {
        ClearBundleNameByDir(ClearPath);
    }
    public static void ClearBundleNameByDir(string[] dirPath)
    {
        string[] guids = AssetDatabase.FindAssets("", dirPath);
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            assetImporter.assetBundleName = String.Empty;
        }
    }
    [MenuItem("Assets/资源导入/处理材质黑色问题", false, 111)]
    public static void ReplaceAssetName()
    {
        foreach (string guid in GetSelectMat())
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

            //if (material.shader.name == "Baoyu/Unlit/ModelHue")
            //{
            //    material.shader = Shader.Find("Baoyu/Unlit/ModelHueCullOff");
            //}
            if (material.shader.name.Contains("Hue"))
            {
                material.SetVector("_RHueShift1", new Vector4(1, 0, 0, 0));
                material.SetVector("_RHueShift2", new Vector4(0, 1, 0, 0));
                material.SetVector("_RHueShift3", new Vector4(0, 0, 1, 0));

                material.SetVector("_GHueShift1", new Vector4(1, 0, 0, 0));
                material.SetVector("_GHueShift2", new Vector4(0, 1, 0, 0));
                material.SetVector("_GHueShift3", new Vector4(0, 0, 1, 0));

                material.SetVector("_BHueShift1", new Vector4(1, 0, 0, 0));
                material.SetVector("_BHueShift2", new Vector4(0, 1, 0, 0));
                material.SetVector("_BHueShift3", new Vector4(0, 0, 1, 0));
                EditorUtility.SetDirty(material);
            }
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }


    [MenuItem("Assets/资源导入/模型规范检测", false, 112)]
    public static void GetBonesCount()
    {
        StringBuilder sb = new StringBuilder();
        IEnumerable<string> enumerable = GetSelectModel();
        sb.AppendLine("prefabName,MeshSkinned,Bones,VertexCount,Triangles,EachData,MeshFilter,VectexCount,Triangles,EachData");
        foreach (string guid in enumerable)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
            modelImporter.optimizeGameObjects = false;
            modelImporter.SaveAndReimport();
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            SkinnedMeshRenderer[] skinnedMeshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>();
            sb.AppendFormat("{0},", go.name);
            sb.Append("#,");
            GenerateSkinnedMeshData(sb, skinnedMeshRenderers);
            sb.Append("#,");
            GenerateMeshFilterData(sb, meshFilters);
            sb.AppendLine();
            modelImporter.optimizeGameObjects = true;
            modelImporter.SaveAndReimport();
        }
        using (StreamWriter streamWriter = new StreamWriter(File.Create("Assets/ModelCheck.csv")))
        {
            streamWriter.Write(sb.ToString());
            streamWriter.Close();
        }
        AssetDatabase.Refresh();
        Selection.objects = new[] { AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/ModelCheck.csv") };
        EditorUtility.UnloadUnusedAssetsImmediate();
    }
  [MenuItem("Assets/资源导入/处理动画控制器", false, 130)]
	private static void Test()
	{
		foreach (var path in GetSelectPrefab().Select(item => AssetDatabase.GUIDToAssetPath(item)))
		{
			var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
			var anims = go.GetComponents<Animator>();
			foreach (var anim in anims)
				anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			EditorUtility.SetDirty(go);
		}
		AssetDatabase.SaveAssets();
		
	}
    private static void GenerateMeshFilterData(StringBuilder sb, MeshFilter[] meshFilters)
    {
        List<JsonData> listJsonDatas = new List<JsonData>();
        const string Vectex = "vectex";
        const string Triangles = "triangles";
        foreach (var meshFilter in meshFilters)
        {
            JsonData jsonData = new JsonData();
            jsonData[Vectex] = meshFilter.sharedMesh.vertexCount;
            jsonData[Triangles] = meshFilter.sharedMesh.triangles.Length / 3;
            listJsonDatas.Add(jsonData);
        }
        listJsonDatas.Sort((left, right) => (int)(left[Vectex].GetNatural() - right[Vectex].GetNatural()));
        long vectex = 0;
        long triangles = 0;
        JsonData meshFilterJsonData = new JsonData();
        foreach (JsonData jsonData in listJsonDatas)
        {
            vectex += jsonData[Vectex].GetNatural();
            triangles += jsonData[Triangles].GetNatural();
            meshFilterJsonData.Add(jsonData);
        }
        sb.AppendFormat("{0},{1},", vectex, triangles);
        sb.AppendFormat("\"{0}\",", meshFilterJsonData.ToJson().Replace("\"", "\"\""));
    }

    private static void GenerateSkinnedMeshData(StringBuilder sb, SkinnedMeshRenderer[] skinnedMeshRenderers)
    {
        const string Vectex = "vectex";
        const string Triangles = "triangles";
        const string Bones = "bones";
        List<JsonData> listJsonDatas = new List<JsonData>();
        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
        {
            JsonData jsonData = new JsonData();
            jsonData[Bones] = skinnedMeshRenderer.bones.Length;
            jsonData[Vectex] = skinnedMeshRenderer.sharedMesh.vertexCount;
            jsonData[Triangles] = skinnedMeshRenderer.sharedMesh.triangles.Length / 3;
            listJsonDatas.Add(jsonData);
        }
        listJsonDatas.Sort((left, right) => (int)(left[Bones].GetNatural() - right[Bones].GetNatural()));
        JsonData skinnedMeshJsonData = new JsonData();
        long bones = 0;
        long vectex = 0;
        long triangles = 0;
        foreach (JsonData jsonData in listJsonDatas)
        {
            bones += jsonData[Bones].GetNatural();
            vectex += jsonData[Vectex].GetNatural();
            triangles += jsonData[Triangles].GetNatural();
            skinnedMeshJsonData.Add(jsonData);
        }
        sb.AppendFormat("{0},{1},{2},", bones, vectex, triangles);
        sb.AppendFormat("\"{0}\",", skinnedMeshJsonData.ToJson().Replace("\"", "\"\""));
    }
    [MenuItem("Assets/资源导入/场景Prefab命名", false, 130)]
    private static void RenamePrefab()
    {
        const string ScenePattern = @"Scene_(?<ID>[0-9]+)";
        const string PrefabPattern = @"^(?<ID>[0-9]+)_[\w]+";
        var selectDirectory = Selection.assetGUIDs
            .Select(item => AssetDatabase.GUIDToAssetPath(item))
            .Where(item => Directory.Exists(item) && Regex.IsMatch(item, ScenePattern));
        if (selectDirectory.Count() != 1)
        {
            EditorUtility.DisplayDialog("错误", "未选中场景文件夹", "确定");
            return;
        }
        string sceneID = Regex.Match(selectDirectory.Single(), ScenePattern).Groups["ID"].Value;
        foreach (var path in GetSelectPrefab().Select(item => AssetDatabase.GUIDToAssetPath(item)))
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            Match match = Regex.Match(fileName, PrefabPattern);
            string newFilename = null;
            if (match.Success)
            {
                string value = match.Groups["ID"].Value;
                if (value != sceneID)
                {
                    newFilename = fileName.Replace(value, sceneID);
                }
            }
            else
            {
                newFilename = fileName.Insert(0, sceneID + "_");
            }
            if (!string.IsNullOrEmpty(newFilename))
            {
                AssetDatabase.RenameAsset(path, newFilename);
            }
        }
        AssetDatabase.Refresh();
    }

    private static IEnumerable<string> GetSelectMat()
    {
        return GetSelectAssets<Material>("Material");
    }

    private static IEnumerable<string> GetSelectModel()
    {
        return GetSelectAssets<ModelImporter>("Model");
    }

    private static IEnumerable<string> GetSelectPrefab()
    {
        return GetSelectAssets<GameObject>("Prefab");
    }
    private static IEnumerable<string> GetSelectAssets<T>(string typeName)
        where T : UnityEngine.Object
    {
        string[] guids = Selection.assetGUIDs;
        List<string> assetsGuids = new List<string>();
        string filter = string.Format("t:{0}", typeName);
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (File.Exists(path))
            {
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                {
                    assetsGuids.Add(guid);
                }
            }
            else
            {
                string[] modelGuids = AssetDatabase.FindAssets(filter, new string[] { path });
                assetsGuids.AddRange(modelGuids);
            }
        }
        IEnumerable<string> modelGuidss = assetsGuids.Distinct();
        return modelGuidss;
    }
}
