using UnityEngine;
using System.Collections.Generic;
using System.IO;
using AssetPipeline;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
public class ExportScene
{
    const string StreamScenesPath = @"Assets/GameResources/ArtResources/Scenes/StreamScenes/";
    static string GetScenePrefabABName(GameObject go)
    {
        PrefabType prefabType = PrefabUtility.GetPrefabType(go);
        string bundleName = null;
        switch (prefabType)
        {
            case PrefabType.PrefabInstance:
            case PrefabType.Prefab:
            case PrefabType.ModelPrefabInstance:
            case PrefabType.ModelPrefab:

                Object asset = PrefabUtility.GetPrefabObject(go);
                if (asset != null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(asset);
                    int changeCount = 0;
                    bundleName = AssetBundleBuilder.UpdateStreamSceneBySingle(assetPath, ref changeCount);
                }
                break;
        }
        return bundleName;
    }

    static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    static void ExportLightMap()
    {
        string sceneName = GetCurrentSceneName();
        string abName = "config/lightmap_" + sceneName;
        LightMapAsset lightmapAsset = ScriptableObject.CreateInstance<LightMapAsset>();

        int count = LightmapSettings.lightmaps.Length;
        lightmapAsset.lightmapFar = new Texture2D[count];
        lightmapAsset.lightmapNear = new Texture2D[count];

        AmbientSetting am = new AmbientSetting();
        am.useFog = RenderSettings.fog;
        am.fogMode = RenderSettings.fogMode;
        am.fogColor = RenderSettings.fogColor;
        am.fogEndDistance = RenderSettings.fogEndDistance;
        am.fogStartDistance = RenderSettings.fogStartDistance;
        am.fogMode = RenderSettings.fogMode;
        am.ambientColor = RenderSettings.ambientLight;
        lightmapAsset.ambientSetting = am;
        for (int i = 0; i < count; i++)
        {
            lightmapAsset.lightmapFar[i] = LightmapSettings.lightmaps[i].lightmapFar;
            lightmapAsset.lightmapNear[i] = LightmapSettings.lightmaps[i].lightmapNear;
        }
        string path = ("Assets/GameResources/ArtResources/SceneConfig/LightMap/lightmap_" + sceneName.ToLower() + ".asset");
        AssetDatabase.CreateAsset(lightmapAsset, path);
        AssetImporter assetImporter = AssetImporter.GetAtPath(path);
        assetImporter.assetBundleName = abName;
    }

    public static void CollectPrefabGo(Transform t, ref List<GameObject> prefabGoList)
    {
        if (!t.gameObject.activeInHierarchy)
        {
            return;
        }
        GameObject prefab = PrefabUtility.GetPrefabParent(t.gameObject) as GameObject;
        if (prefab == null)
        {
            foreach (Transform _t in t)
                CollectPrefabGo(_t, ref prefabGoList);
            return;
        }
        else
        {
            var notUseGoInfo = prefab.GetComponent<SceneGoInfoComponent>();
            if (notUseGoInfo != null)
            {
                var path = AssetDatabase.GetAssetPath(prefab);
                Debug.LogError(string.Format("删除多余的 GoInfo：{0}", path));
                Object.DestroyImmediate(notUseGoInfo, true);
            }
        }
        //{
        //    //转换代码
        //    BoxCollider[] boxColliders = t.GetComponentsInChildren<BoxCollider>();
        //    foreach (var boxCollider in boxColliders)
        //    {
        //        SceneGoInfoComponent sceneGoInfoComponent = boxCollider.gameObject.AddComponent<SceneGoInfoComponent>();
        //        sceneGoInfoComponent.bounds = boxCollider.bounds;
        //        sceneGoInfoComponent.treeLevel = GetLevel(boxCollider.tag);
        //        boxCollider.tag = "Untagged";
        //        Editor.DestroyImmediate(boxCollider);
        //    }
        //    Scene currentScene = EditorSceneManager.GetActiveScene();
        //    EditorSceneManager.MarkSceneDirty(currentScene);
        //}
        if (t.GetComponent<SceneGoInfoComponent>() == null)
        {
            SceneGoInfoComponent sceneGoInfoComponent = t.gameObject.AddComponent<SceneGoInfoComponent>();
            var meshFilters = t.gameObject.GetComponentsInChildren<MeshFilter>();
            if (meshFilters.Length > 0)
            {
                Bounds bounds = CaculateBoundsByMeshFilter(meshFilters[0]);
                foreach (MeshFilter meshFilter in meshFilters)
                {
                    bounds.Encapsulate(CaculateBoundsByMeshFilter(meshFilter));
                }
//                bounds.size += new Vector3(0.5f, 0f, 0.5f);
                sceneGoInfoComponent.bounds.center = bounds.center;
                sceneGoInfoComponent.bounds.size = bounds.size;
            }
            else
            {
                Debug.LogError("未能计算包围盒大小：" + t.gameObject.name);

                sceneGoInfoComponent.bounds.center = t.position;
            }
        }
        prefabGoList.Add(t.gameObject);
    }

    private static Bounds CaculateBoundsByMeshFilter(MeshFilter meshFilter)
    {
        Bounds meshBounds = meshFilter.sharedMesh.bounds;
        Matrix4x4 matrix4X4 = meshFilter.transform.localToWorldMatrix;
        Vector3[] vector3s = new Vector3[8];
        vector3s[0] = matrix4X4.MultiplyPoint3x4(CaculatePoinHeler(meshBounds.center, meshBounds.extents, -1, -1, -1));
        vector3s[1] = matrix4X4.MultiplyPoint3x4(CaculatePoinHeler(meshBounds.center, meshBounds.extents, -1, +1, -1));
        vector3s[2] = matrix4X4.MultiplyPoint3x4(CaculatePoinHeler(meshBounds.center, meshBounds.extents, +1, +1, -1));
        vector3s[3] = matrix4X4.MultiplyPoint3x4(CaculatePoinHeler(meshBounds.center, meshBounds.extents, +1, -1, -1));

        vector3s[4] = matrix4X4.MultiplyPoint3x4(CaculatePoinHeler(meshBounds.center, meshBounds.extents, -1, -1, +1));
        vector3s[5] = matrix4X4.MultiplyPoint3x4(CaculatePoinHeler(meshBounds.center, meshBounds.extents, -1, +1, +1));
        vector3s[6] = matrix4X4.MultiplyPoint3x4(CaculatePoinHeler(meshBounds.center, meshBounds.extents, +1, +1, +1));
        vector3s[7] = matrix4X4.MultiplyPoint3x4(CaculatePoinHeler(meshBounds.center, meshBounds.extents, +1, -1, +1));

        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        foreach (var poin in vector3s)
        {
            max.x = Mathf.Max(max.x, poin.x);
            max.y = Mathf.Max(max.y, poin.y);
            max.z = Mathf.Max(max.z, poin.z);

            min.x = Mathf.Min(min.x, poin.x);
            min.y = Mathf.Min(min.y, poin.y);
            min.z = Mathf.Min(min.z, poin.z);
        }
        Bounds worldSpaceBounds = new Bounds();
        worldSpaceBounds.SetMinMax(min, max);
        return worldSpaceBounds;
    }
    private static Vector3 CaculatePoinHeler(Vector3 vector3, Vector3 extents, float _x, float _y, float _z)
    {
        vector3.x += _x * extents.x;
        vector3.y += _y * extents.y;
        vector3.z += _z * extents.z;
        return vector3;
    }
    public static SceneGoInfo[] CreateSceneGoInfoArray(List<GameObject> prefabGoList, ref List<GameObject> errorGoList)
    {
        SceneGoInfo[] res = new SceneGoInfo[prefabGoList.Count];
        for (int i = 0; i < prefabGoList.Count; i++)
        {
            GameObject gameObject = prefabGoList[i];
            GameObject prefab = PrefabUtility.GetPrefabParent(gameObject) as GameObject;
            SceneGoInfo sceneGoInfo = new SceneGoInfo();
            res[i] = sceneGoInfo;
            sceneGoInfo.id = i;
            string abName = GetScenePrefabABName(prefab);
            if (abName == null)
            {
                Debug.LogError(gameObject.name + " Can Not Find abName");
            }
            sceneGoInfo.pos = gameObject.transform.position;
            sceneGoInfo.rotation = gameObject.transform.rotation;
            sceneGoInfo.scale = gameObject.transform.localScale;
            sceneGoInfo.prefabName = prefab.name;
            sceneGoInfo.gameobjectName = gameObject.transform.name;
            SceneGoInfoComponent sceneGoInfoComponent = gameObject.GetComponent<SceneGoInfoComponent>();

            if (sceneGoInfoComponent.bounds.size != Vector3.zero)
            {
                sceneGoInfo.treeLevel = sceneGoInfoComponent.treeLevel;
                MeshRenderer[] mrArray = gameObject.GetComponentsInChildren<MeshRenderer>();
                List<MeshRenderer> mrList = new List<MeshRenderer>();
                mrList.AddRange(mrArray);
                foreach (MeshRenderer _mr in mrList)
                {
                    LightMapDetail lightmapDetail = new LightMapDetail();
                    lightmapDetail.offset = _mr.lightmapScaleOffset;
                    lightmapDetail.index = _mr.lightmapIndex;
                    lightmapDetail.gameobjectName = _mr.gameObject.name;
                    sceneGoInfo.lightMapDetailList.Add(lightmapDetail);
                }
                Bounds bounds = GetGameObjectBounds(gameObject);
                sceneGoInfo.bounds = bounds;
            }
            else
            {
                errorGoList.Add(gameObject);
            }
        }
        return res;
    }

    private static Bounds GetGameObjectBounds(GameObject prefabGo)
    {
        SceneGoInfoComponent[] boxColliders = prefabGo.GetComponentsInChildren<SceneGoInfoComponent>();
        Bounds bounds = boxColliders[0].bounds;
        for (int i = 1; i < boxColliders.Length; i++)
        {
            bounds.Encapsulate(boxColliders[i].bounds);
        }
        return bounds;
    }

    [MenuItem("ExportTool/ExportScene")]
    static void Export()
    {
        //导出前先清理Scene目录下的BundleName，防止多余的Bundle被打包。
        CustomAssetTool.ClearBundleNameByDir(new string[] { StreamScenesPath + GetCurrentSceneName()});
        ExportLightMap();
        GameObject root = GameObject.Find("WorldStage");
        List<GameObject> prefabGoList = new List<GameObject>();
        CollectPrefabGo(root.transform, ref prefabGoList);
        AllSceneGoInfo allSceneGoInfo = ScriptableObject.CreateInstance<AllSceneGoInfo>();
        var errorGoInfoList = new List<GameObject>();
        SceneGoInfo[] sceneGoInfoArray = CreateSceneGoInfoArray(prefabGoList, ref errorGoInfoList);
        allSceneGoInfo.sceneGoInfoArray = sceneGoInfoArray;

        string path = "Assets/GameResources/ArtResources/SceneConfig/SceneGo/sg_" + GetCurrentSceneName().ToLower() + ".asset";
        AssetDatabase.CreateAsset(allSceneGoInfo, path);
        AssetImporter assetImporter = AssetImporter.GetAtPath(path);
        assetImporter.assetBundleName = "config/sg_" + GetCurrentSceneName();

        AssetDatabase.SaveAssets();

        SelectingEditorWindow.Show(errorGoInfoList, "未能计算包围盒大小，请修正");
    }
    [MenuItem("ExportTool/删除选中GameObject节点下导出")]
    static void RemoveSceneGoInfo()
    {
        GameObject root = Selection.activeGameObject;
        SceneGoInfoComponent[] sceneGoInfoComponents = root.GetComponentsInChildren<SceneGoInfoComponent>(true);
        foreach (var sceneGoInfoComponent in sceneGoInfoComponents)
        {
            GameObject.DestroyImmediate(sceneGoInfoComponent);
        }
    }

    [MenuItem("ExportTool/ExportServerNavMesh")]
    static void ExportServerNavMesh()
    {
        string path = Application.dataPath+ "/Docs/NavigationArea/";
        if (!string.IsNullOrEmpty(path))
        {
            if (Directory.Exists(path))
            {
                PlayerPrefs.SetString("dev_walkPath", path);
                NavigationArea area = new NavigationArea();
                area.CreateMoveNavigation();
                area.OutputFile(path);
            }
            else
            {
                TipManager.AddTip("目录不存在");
            }
        }
        else
        {
            TipManager.AddTip("目录不能为空");
        }
        AssetDatabase.Refresh();
    }
    [MenuItem("ExportTool/ExportUnityNavMesh")]

    static void ExportnavMesh()
    {
        var triangulation = NavMesh.CalculateTriangulation();
        Mesh mesh  = new Mesh();
        mesh.vertices = triangulation.vertices;
        mesh.triangles = triangulation.indices;
        mesh.uv = new Vector2[triangulation.vertices.Length];
        AssetDatabase.CreateAsset(mesh, "Assets/Navmesh.asset");
    }
}
