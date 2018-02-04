using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AssetPipeline;
using UnityEditor;
using UnityEngine;

namespace AssetImportCop
{
    class AvatarImportTool 
    {
        //导入骨骼设置 忽略该路径
        private static readonly string[] ignorModelPath =
        {
            "Assets/GameResources/ArtResources/Characters/RoleCreate",
        };
        //角色配置
        private static readonly string[] petConfig =
        {
            "Bip001/Bip001 Prop1",
            "Bip001/Bip001 Prop2",
        };
        //坐骑配置
        private static readonly string[] rideConfig =
        {
            "Bip001/Bip001 Pelvis/Bip001 Spine",
        };
        //含有这个前缀的都导出
        private static readonly string particleView = "Particle View";

        //[MenuItem("Assets/资源导入/模型导入优化骨骼", false, 101)]
        private static void OptimizeGameObjectOnImport()
        {
            IEnumerable<string> modelGuids = GetSelectModel();
            foreach (string modelGuid in modelGuids)
            {
                try
                {
                    ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(modelGuid));
                    ModelOptimizeGmaeObjectOnImport(modelImporter);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("提示", "完成", "OK");
        }

        private static void ModelOptimizeGmaeObjectOnImport(ModelImporter modelImporter)
        {
            if (modelImporter.optimizeGameObjects)
            {
                return;
            }
            if (modelImporter.assetPath.IsGameModel() &&
                (ignorModelPath.Any(item => modelImporter.assetPath.StartsWith(item) == false)))
            {
                ImportType importType = GetImportType(GetPathName(modelImporter.assetPath));
                GameObject modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(modelImporter.assetPath);
                ModelThree modelThree = new ModelThree(modelAsset);
                List<string> configExtartList = ConfigExtart(importType, modelThree);
                List<string> extarPaths = new List<string>();
                extarPaths.AddRange(configExtartList);
                modelThree.nodeList.ForEach(item =>
                {
                    if (GetPathName(item.path).StartsWith(particleView))
                    {
                        extarPaths.Add(item.path);
                    }
                });

                modelImporter.optimizeGameObjects = true;
                modelImporter.extraExposedTransformPaths = extarPaths.Distinct().ToArray();

            }
        }

        [MenuItem("Assets/资源导入/一键优化模型骨骼", false, 100)]
        private static void ModelOptimizeGameObject()
        {
            IEnumerable<string> modelGuids = GetSelectModel();
            bool isAllSucceed = true;
            foreach (string guid in modelGuids)
            {
                try
                {
                    ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid));
                    ModelOptimizeGameObjectItem(modelImporter);
                }
                catch (Exception ex)
                {
                    Debug.LogError(string.Format("Path:{0}\n message:{1}", AssetDatabase.GUIDToAssetPath(guid), ex));
                    isAllSucceed = false;

                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.UnloadUnusedAssetsImmediate();
            EditorUtility.DisplayDialog("提示", isAllSucceed ? "优化成功" : "有优化失败的预设，请查看日志", "OK");
        }

        private static void ModelOptimizeGameObjectItem(ModelImporter modelImporter)
        {
            if (modelImporter.optimizeGameObjects)
                return;
            string modelAssetPath = modelImporter.assetPath;
            string prefabAssetPath = GetPrefabPath(modelAssetPath);

            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(prefabAssetPath)) as GameObject;
            prefabInstance.transform.position = Vector3.zero;
            GameObject modelInstance = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(modelAssetPath)) as GameObject;
            modelInstance.transform.position = Vector3.zero;

            ImportType importType = GetImportType(prefabInstance.name);
            PreProcessByImportType(importType, modelInstance, prefabInstance);

            ModelThree prefabThree = new ModelThree(prefabInstance);
            ModelThree modelThree = new ModelThree(modelInstance);

            List<ModelNode> except = GetExcept(prefabThree, modelThree);
            //计算要导出或移动的节点
            List<ModelNode> extarList = new List<ModelNode>();
            List<ModelNode> moveList = new List<ModelNode>();
            {
                for (int i = 0; i < except.Count; i++)
                {
                    ModelNode node = except[i];
                    string parentPath = GetParentPath(node.path);
                    ModelNode extarNode = null;
                    if (modelThree.path2Node.TryGetValue(parentPath, out extarNode))
                    {
                        if (extarNode.mainTransform != null)
                        {
                            extarList.Add(extarNode);
                            moveList.Add(node);
                        }
                        else
                        {
                            GameObject.DestroyImmediate(modelInstance);
                            GameObject.DestroyImmediate(prefabInstance);
                            throw new SystemException(string.Format("多个Transform同名，无法自动转换:{0}", modelAssetPath));
                        }
                    }
                    else
                    {
                        moveList.Add(node);
                    }

                }
                modelThree.nodeList.ForEach(item =>
                {
                    if (item.mainTransform != null && item.mainTransform.gameObject.name.StartsWith(particleView))
                        extarList.Add(item);
                });
                extarList = extarList.Distinct().ToList();
                moveList = moveList.Distinct().ToList();
            }

            //计算要导出的rootBone 
            SkinnedMeshRenderer[] skins = prefabInstance.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            List<string> extarPaths = new List<string>(extarList.Count + skins.Length + 5);
            for (int i = 0; i < skins.Length; i++)
            {
                Transform rootBone = skins[i].rootBone;
                if (rootBone != null)
                {
                    ModelNode modelNode = prefabThree.instanceID2Node[rootBone.GetInstanceID()];
                    extarPaths.Add(modelNode.path);
                }
            }
            //添加导出路径
            {
                extarList.ForEach(item => extarPaths.Add(item.path));

                List<string> configExtart = ConfigExtart(importType, modelThree);
                if (configExtart != null)
                    extarPaths.AddRange(configExtart);
            }

            //配置导出的骨骼
            extarPaths = extarPaths.Distinct().ToList();
            modelImporter.optimizeGameObjects = true;
            modelImporter.extraExposedTransformPaths = extarPaths.ToArray();
            modelImporter.SaveAndReimport();
            //此处用modelInstance 与Prefab有关联，在Reimport后modelInstance也会变更
            PreProcessByImportType(importType, modelInstance, prefabInstance);
            //移动特效层级
            foreach (ModelNode modelNode in moveList)
            {
                string parentPath = GetParentName(modelNode.path);
                Transform parentTransform = modelInstance.transform.Find(parentPath) ?? modelInstance.transform;
                modelNode.transList.ForEach(item => item.parent = parentTransform);
            }
            //设置tag
            foreach (ModelNode node in modelThree.nodeList)
            {
                Transform trans = modelInstance.transform.Find(GetPathName(node.path));
                if (trans != null)
                {
                    if (prefabThree.path2Node.ContainsKey(node.path))   //美术可能会删除FBX中的节点，Prefab中没有该节点
                        trans.gameObject.tag = prefabThree.path2Node[node.path].tag;
                }
            }
            //赋值材质 rootBone
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in skins)
            {
                ModelNode modelNode = prefabThree.instanceID2Node[skinnedMeshRenderer.transform.GetInstanceID()];
                Transform transform = modelInstance.transform.Find(GetPathName(modelNode.path));
                transform.GetComponent<SkinnedMeshRenderer>().sharedMaterials = skinnedMeshRenderer.sharedMaterials;

                Transform rootBone = skinnedMeshRenderer.rootBone;
                ModelNode rootBoneNode = rootBone == null ? null : prefabThree.instanceID2Node[rootBone.GetInstanceID()];
                transform.GetComponent<SkinnedMeshRenderer>().rootBone = rootBoneNode == null ? null : modelInstance.transform.Find(GetPathName(rootBoneNode.path));

                transform.gameObject.SetActive(skinnedMeshRenderer.gameObject.activeSelf);
            }
            SetRuntimeAnimator(prefabInstance, modelInstance);
            GameObject.DestroyImmediate(prefabInstance);
            PrefabUtility.ReplacePrefab(modelInstance, AssetDatabase.LoadAssetAtPath<GameObject>(prefabAssetPath));
            GameObject.DestroyImmediate(modelInstance);
        }

        [MenuItem("Assets/资源导入/一键还原骨骼优化", false, 101)]
        private static void RevertModelOptimizeGameObject()
        {
            IEnumerable<string> modelGuids = GetSelectModel();
            bool isAllSucceed = true;
            foreach (string guid in modelGuids)
            {
                try
                {
                    ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid));
                    RevertModelOptimizeGameObjectItem(modelImporter);
                }
                catch (Exception ex)
                {
                    Debug.LogError(string.Format("Path:{0}\n message:{1}", AssetDatabase.GUIDToAssetPath(guid), ex));
                    isAllSucceed = false;
                }
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            EditorUtility.UnloadUnusedAssetsImmediate();
            EditorUtility.DisplayDialog("提示", isAllSucceed ? "还原成功":"有还原失败的预设，请查看日志", "OK");
        }

        private static void RevertModelOptimizeGameObjectItem(ModelImporter modelImporter)
        {
            if (!modelImporter.optimizeGameObjects)
                return;
            string modelAssetPath = modelImporter.assetPath;
            string prefabAssetPath = GetPrefabPath(modelAssetPath);

            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(prefabAssetPath)) as GameObject;
            GameObject modelInstance = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(modelAssetPath)) as GameObject;
            prefabInstance.transform.position = Vector3.zero;
            modelInstance.transform.position = Vector3.zero;
            ImportType importType = GetImportType(prefabInstance.name);
            PreProcessByImportType(importType, modelInstance, prefabInstance);

            ModelThree prefabThree = new ModelThree(prefabInstance);
            ModelThree modelThree = new ModelThree(modelInstance);


            List<ModelNode> moveList = GetExcept(prefabThree, modelThree);
            //计算 优化路径 到 原路径
            Dictionary<string, string> optimize2RevertPath = new Dictionary<string, string>();
            {
                string[] extartPaths = modelImporter.extraExposedTransformPaths;
                foreach (string path in extartPaths)
                {
                    string pathName = GetPathName(path);
                    ModelNode modelNode;
                    if (modelThree.path2Node.TryGetValue(pathName, out modelNode))
                    {
                        optimize2RevertPath.Add(modelNode.path, path);
                    }
                }
            }

            modelImporter.optimizeGameObjects = false;
            modelImporter.SaveAndReimport();
            PreProcessByImportType(importType, modelInstance, prefabInstance);

            //移动这些逗逼
            foreach (ModelNode modelNode in moveList)
            {
                string revertParentPath;
                if (optimize2RevertPath.TryGetValue(GetParentPath(modelNode.path), out revertParentPath) == false)
                    revertParentPath = string.Empty;

                string parentPath = string.IsNullOrEmpty(revertParentPath) ? string.Empty : revertParentPath;
                Transform transform = string.IsNullOrEmpty(parentPath) ? modelInstance.transform : modelInstance.transform.Find(parentPath);
                if (transform)
                    modelNode.transList.ForEach(item => item.parent = transform);
                else
                    throw new SystemException(string.Format("Can Not Find ReverPath By model: {0}:{1}", modelNode.path, revertParentPath));
            }
            //tag还原
            {
                var tagList = prefabThree.nodeList.Intersect(modelThree.nodeList)
                            .Concat(prefabThree.nodeList
                            .Where(item =>
                            {
                                return optimize2RevertPath.ContainsValue(item.path);
                            }));
                foreach (ModelNode modelNode in tagList)
                {
                    string revertPath = modelNode.path;
                    Transform trans = modelInstance.transform.Find(revertPath);
                    if (!trans)
                    {
                        revertPath = optimize2RevertPath[modelNode.path];
                        trans = modelInstance.transform.Find(revertPath);
                    }
                    if (trans)
                        trans.tag = modelNode.tag;
                    else
                        throw new SystemException(string.Format("Can Not Find Transform{0}", modelNode.path));
                }
            }
            //赋值材质 rootBone
            {
                ModelThree newModelThree = new ModelThree(modelInstance);
                SkinnedMeshRenderer[] skins = modelInstance.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                foreach (SkinnedMeshRenderer skinnedMeshRenderer in skins)
                {
                    ModelNode modelNode = newModelThree.instanceID2Node[skinnedMeshRenderer.transform.GetInstanceID()];
                    Transform transform = prefabInstance.transform.Find(GetPathName(modelNode.path));
                    if (transform == null)
                    {
                        throw new SystemException(string.Format("无法找到对应的SkinnedMeshRenderer:{0} -> '空格'已替换成'#'", GetPathName(modelNode.path).Replace(' ','#')));
                    }
                    skinnedMeshRenderer.sharedMaterials = transform.GetComponent<SkinnedMeshRenderer>().sharedMaterials;

                    Transform rootBone = transform.GetComponent<SkinnedMeshRenderer>().rootBone;
                    ModelNode rootBoneNode = rootBone == null ? null : prefabThree.instanceID2Node[rootBone.GetInstanceID()];
                    skinnedMeshRenderer.rootBone = rootBoneNode == null ? null : modelInstance.transform.Find(optimize2RevertPath[rootBoneNode.path]);
                    skinnedMeshRenderer.gameObject.SetActive(transform.gameObject.activeSelf);
                }
            }
            SetRuntimeAnimator(prefabInstance, modelInstance);
            //生成新Prefab
            GameObject.DestroyImmediate(prefabInstance);
            PrefabUtility.ReplacePrefab(modelInstance, AssetDatabase.LoadAssetAtPath<GameObject>(prefabAssetPath));
            GameObject.DestroyImmediate(modelInstance);
        }
        private static void PreProcessByImportType(ImportType importType, GameObject modelInstance, GameObject prefabInstance)
        {
            //兼容美术的潜规则，针对 fbx文件 和 prefab文件节点名对应不上的情况
            if (importType == ImportType.Ride && modelInstance != null)
            {
                SkinnedMeshRenderer[] prefabSkin = prefabInstance.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                if (prefabSkin[0].name.StartsWith("ride_"))
                {
                    SkinnedMeshRenderer[] skins = modelInstance.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                    skins.ForEach(item =>
                    {
                        string itemName = item.gameObject.name;
                        if (itemName.StartsWith("ride_") == false)
                        {
                            item.gameObject.name = itemName.Insert(0, "ride_");
                        }
                    });
                }
            }
        }
        private static List<string> ConfigExtart(ImportType importType, ModelThree modelThree)
        {
            List<string> extartList = new List<string>();
            if (importType == ImportType.Pet)
            {
                petConfig.ForEach(item =>
                {
                    if (modelThree.nodeList.Find(modelNode => modelNode.path == item) != null)
                        extartList.Add(item);
                });
            }
            else if (importType == ImportType.Ride)
            {
                rideConfig.ForEach(item =>
                {
                    if (modelThree.nodeList.Find(modelNode => modelNode.path == item) != null)
                        extartList.Add(item);
                });
            }
            return extartList;
        }
        private static string GetPrefabPath(string modelAssetPath)
        {
            DirectoryInfo directoryInfo = Directory.GetParent(modelAssetPath).Parent;
            string modelPath = directoryInfo.FullName.Replace("\\", "/").Replace(Application.dataPath.Replace("Assets", string.Empty), string.Empty);
            string[] guids = AssetDatabase.FindAssets("t:Prefab ", new string[] { modelPath });
            if (guids.Length > 1)
            {
                throw new SystemException(string.Format("同一Model的Prefab数量大于1 path: {0}", modelAssetPath));
            }
            else if (guids.Length == 0)
            {
                throw new SystemException(string.Format("无法找到对应的Prefab path: {0}", modelAssetPath));
            }
            string prefabPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return prefabPath;
        }
        /// <summary>
        /// 计算Prefab多出的节点 删除多余的父节点
        /// </summary>
        private static List<ModelNode> GetExcept(ModelThree prefabThree, ModelThree modelThree)
        {
            var except = prefabThree.nodeList.Except(modelThree.nodeList).ToList();
            except.Sort((left, right) => string.Compare(left.path, right.path));
            for (int i = except.Count - 1; i >= 0; i--)
            {
                string parentPath = GetParentPath(except[i].path);
                bool haveParent = except.Find(item => item.path == parentPath) != null;
                if (haveParent)
                {
                    except.RemoveAt(i);
                }
            }

            return except;
        }
        /// <summary>
        /// 动画赋值
        /// </summary>
        private static void SetRuntimeAnimator(GameObject prefabInstance, GameObject modelInstance)
        {
            Animator animator = prefabInstance.GetComponent<Animator>();
            if (animator != null)
                modelInstance.GetComponent<Animator>().runtimeAnimatorController = animator.runtimeAnimatorController;
        }
        public class ModelThree
        {
            public readonly GameObject go;
            public List<ModelNode> nodeList = new List<ModelNode>();
            public Dictionary<int, ModelNode> instanceID2Node = new Dictionary<int, ModelNode>();
            public Dictionary<string, ModelNode> path2Node = new Dictionary<string, ModelNode>();
            public ModelThree(GameObject go)
            {
                this.go = go;
                GenerateNodeList();
            }
            private void GenerateNodeList()
            {
                foreach (Transform transform in go.transform)
                {
                    GenerateNode(transform, string.Empty);
                }
                nodeList.Sort((left, right) => string.Compare(left.path, right.path));
            }

            void GenerateNode(Transform trans, string parentPath)
            {
                string nodePath = string.IsNullOrEmpty(parentPath) ? trans.gameObject.name : string.Concat(parentPath, "/", trans.gameObject.name);
                ModelNode modelNode;
                if (path2Node.TryGetValue(nodePath, out modelNode))
                {
                    modelNode.transList.Add(trans);
                }
                else
                {
                    modelNode = new ModelNode(trans, parentPath);
                    nodeList.Add(modelNode);
                    instanceID2Node.Add(trans.GetInstanceID(), modelNode);
                    path2Node.Add(modelNode.path, modelNode);
                }

                foreach (Transform child in trans)
                {
                    GenerateNode(child, modelNode.path);
                }
            }

        }

        public class ModelNode
        {
            /// <summary>
            /// 不包含根节点的路径 Root->A->B path="A/B"
            /// </summary>
            public readonly string path;
            public readonly string transformName;
            public readonly List<Transform> transList;
            public readonly string tag;
            public Transform mainTransform
            {
                get
                {
                    return transList.Count == 1 ? transList[0] : null;
                }
            }
            public ModelNode(Transform trans, string parentPath)
            {
                path = string.IsNullOrEmpty(parentPath) ? trans.gameObject.name : string.Concat(parentPath, "/", trans.gameObject.name);
                transformName = trans.gameObject.name;
                transList = new List<Transform>();
                transList.Add(trans);
                tag = trans.tag;
            }

            public override int GetHashCode()
            {
                return path.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                ModelNode modelNode = obj as ModelNode;
                return modelNode != null && path.Equals(modelNode.path);
            }
        }

        enum ImportType
        {
            None,
            Pet,
            Ride,

        }

        #region Helper
        public static string GetParentPath(string path)
        {
            int index = path.LastIndexOf("/");
            return index >= 0 ? path.Remove(index) : string.Empty;
        }

        public static string GetPathName(string path)
        {
            int index = path.LastIndexOf("/");
            if (index >= 0)
                path = path.Remove(0, index + 1);
            return path;
        }

        public static string GetParentName(string path)
        {
            return GetPathName(GetParentPath(path));
        }

        private static ImportType GetImportType(string assetName)
        {
            if (assetName.StartsWith("pet_"))
            {
                return ImportType.Pet;
            }
            else if (assetName.StartsWith("ride_"))
            {
                return ImportType.Ride;
            }
            return ImportType.None;
        }

        private static IEnumerable<string> GetSelectModel()
        {
            string[] guids = Selection.assetGUIDs;
            List<string> modelGuidList = new List<string>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (File.Exists(path))
                {
                    ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
                    if (modelImporter != null)
                    {
                        modelGuidList.Add(guid);
                    }
                    else if(Path.GetExtension(path) == ".prefab")
                    {
                        DirectoryInfo directoryInfo = Directory.GetParent(path).Parent;
                        string modelPath = directoryInfo.FullName.Replace("\\", "/").Replace(Application.dataPath.Replace("Assets", string.Empty), string.Empty);
                        string[] modelGuids = AssetDatabase.FindAssets("t:Model ", new string[] { modelPath });
                        modelGuidList.AddRange(modelGuids);
                    }
                }
                else
                {
                    string[] modelGuids = AssetDatabase.FindAssets("t:Model", new string[] { path });
                    modelGuidList.AddRange(modelGuids);
                }
            }
            IEnumerable<string> modelGuidss = modelGuidList.Distinct();
            return modelGuidss;
        }

        #endregion
    }
}
