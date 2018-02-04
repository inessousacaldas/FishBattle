using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetImportCop
{
    class AnimationsImportTool
    {
        //[MenuItem("Assets/资源导入/动画删除缩放曲线", false, 103)]
        private static void DeleteAnimationScaleCurve()
        {
            foreach (string modelGuid in GetSelectModelGUID())
            {
                try
                {
                    ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(modelGuid));
                    DeleteScaleCurve(modelImporter);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("提示", "完成", "OK");
        }

        private static void DeleteScaleCurve(ModelImporter modelImporter)
        {
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(modelImporter.assetPath);

            try
            {

                EditorCurveBinding[] editorCurveBindings = AnimationUtility.GetCurveBindings(clip);
                foreach (EditorCurveBinding curveBinding in editorCurveBindings)
                {
                    if (curveBinding.propertyName.ToLower().Contains("scale"))
                    {
                        AnimationUtility.SetEditorCurve(clip, curveBinding, null);
                    }
                }

            }
            finally
            {
            }

        }
        [MenuItem("Assets/资源导入/动画最优压缩", false, 105)]
        private static void AnimationSetOptimal()
        {
            foreach (string modelGUID in GetSelectModelGUID())
            {
                try
                {
                    ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(modelGUID));
                    modelImporter.animationCompression = ModelImporterAnimationCompression.Optimal;
                    modelImporter.SaveAndReimport();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("提示", "完成", "OK");
        }

        private static IEnumerable<string> GetSelectModelGUID()
        {
            string[] guids = Selection.assetGUIDs;
            List<string> modelGuidList = new List<string>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (File.Exists(path))
                {
                    modelGuidList.Add(guid);
                }
                else
                {
                    string[] modelGuids = AssetDatabase.FindAssets("t:Model", new string[] { path });
                    modelGuidList.AddRange(modelGuids);
                }
            }
            return modelGuidList.Distinct();
        } 
    }
}
