using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LITJson;
using SceneTrigger;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneTriggerEditor
{
    public class SceneTriggerView : EditorWindow
    {
        public static SceneTriggerView Instance;
        [MenuItem("美术/场景触发器")]
        public static void ShowWindow()
        {
            if (Instance == null)
            {
                Instance = GetWindow<SceneTriggerView>(false, "SceneTriggerView", true);
                Instance.minSize = new Vector2(200, 400);
                Instance.Show();
                Instance.Setup();
            }
        }


        private void Setup()
        {

        }
        void AddEmptyTrigger()
        {
            var root = GetTriggerRoot();
            var go = new GameObject("Trigger");
            go.transform.parent = root.transform;
            IconManager.SetIcon(go, IconManager.LabelIcon.Gray);
            go.AddComponent<SceneTriggerCom>();
            Selection.activeObject = go;
        }

        void SaveSceneConfig()
        {
            SaveJsonConfig();
            SavePrefab();
            AssetDatabase.Refresh();
            ShowNotification(new GUIContent("保存成功"));
        }

        private void SaveJsonConfig()
        {
            var configs = GetTriggerRoot().GetComponentsInChildren<SceneTriggerCom>()
                .Select(item => item.configItem)
                .OrderBy(item => GetConfigHash(item))
                .ToList();
            string configsJson = JsonMapper.ToJson(configs);
            string filePath = EditorConfig.SceneConfigPath + "scenetrigger_" + GetCurrentSceneName() + ".bytes";
            File.WriteAllText(filePath, configsJson);
        }
        private void SavePrefab()
        {
            string prefabName = GetRootName() + ".prefab";
            GameObject triggerRootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EditorConfig.TriggerPrefabPath + prefabName);
            if (triggerRootPrefab == null)
            {
                PrefabUtility.CreatePrefab(EditorConfig.TriggerPrefabPath + prefabName, GetTriggerRoot());
            }
            else
            {
                PrefabUtility.ReplacePrefab(GetTriggerRoot(), triggerRootPrefab);
            }
            if(!EditorUtility.DisplayDialog("提示", "是否需要继续编辑？", "是", "否"))
                GameObject.DestroyImmediate(GetTriggerRoot());
        }

        private void CreateEmptyConfig()
        {
            if (GameObject.Find(GetRootName()) != null)
            {
                ShowNotification(new GUIContent("已经加载配置，不需要创建加载"));
                return;
            }
            GameObject root = new GameObject(GetRootName());
            root.transform.position = Vector3.zero;
            root.transform.rotation = Quaternion.identity;
        }

        private void LoadPrefab()
        {
            string selectPath = EditorUtility.OpenFilePanel("加载配置Prefab", EditorConfig.TriggerPrefabPath, "prefab");
            if (string.IsNullOrEmpty(selectPath))
                return;
            string prefabPath = selectPath.Replace(GetProjectPath(), string.Empty);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            GameObject root = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            root.transform.position = Vector3.zero;
            root.transform.rotation = Quaternion.identity;

        }


        private int GetConfigHash(TriggerConfigItem configItem)
        {
            int hash = configItem.conditionType.GetHashCode();
            if (string.IsNullOrEmpty(configItem.conditionParamJson))
                hash ^= configItem.conditionParamJson.GetHashCode();
            hash ^= configItem.triggerType.GetHashCode();
            if (string.IsNullOrEmpty(configItem.triggerParamJson))
                hash ^= configItem.triggerParamJson.GetHashCode();

            return hash;
        }

        private GameObject GetTriggerRoot()
        {
            GameObject root = GameObject.Find(GetRootName());
            if (root == null)
            {
                ShowNotification(new GUIContent("没有加载场景配置，如果是新场景请点击“创建空配置”"));
                throw new Exception("没有加载场景配置，如果是新场景请点击“创建空配置”");
            }
            return root;
        }


        #region OnGUI
        void OnGUI()
        {
            GUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical();
                {
                    DrawLeftView();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }



        private void DrawLeftView()
        {
            if (GUILayout.Button("添加触发器", GUILayout.Height(50f)))
                EditorApplication.delayCall += AddEmptyTrigger;

            if (GUILayout.Button("创建空配置", GUILayout.Height(50f)))
                EditorApplication.delayCall += CreateEmptyConfig;

            if (GUILayout.Button("加载配置", GUILayout.Height(50f)))
                EditorApplication.delayCall += LoadPrefab;

            if (GUILayout.Button("保存配置", GUILayout.Height(50f)))
                EditorApplication.delayCall += SaveSceneConfig;
        }

        #endregion
        static string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name.ToLower();
        }

        private static string GetRootName()
        {
            return EditorConfig.SceneTriggerRoot + GetCurrentSceneName();
        }

        private static string GetProjectPath()
        {
            return Application.dataPath.Replace("Assets", string.Empty);
        }
    }


}
