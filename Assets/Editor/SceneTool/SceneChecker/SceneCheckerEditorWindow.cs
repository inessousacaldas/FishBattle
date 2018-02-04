using System.IO;
using UnityEditor;
using BaseClassNS;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace EditorNS
{
    public class SceneCheckerEditorWindow : BaseEditorWindow
    {
        public static readonly string _sceneFolderPath =
            "Assets/GameResources/ArtResources/Scenes";

        [MenuItem("Custom/SceneChecker/Window")]
        private static void Open()
        {
            Open<SceneCheckerEditorWindow>();
        }

        protected override void CustomOnGUI()
        {
            Space();
            TitleField("路径：", _sceneFolderPath);

            Space();
            Button("检测当前场景", () => EditorHelper.Run(CheckCurrentScene, false, false));

            Space();
            Button("检测所有场景", () => EditorHelper.Run(CheckAllScene, true, false));
        }


        private void CheckCurrentScene()
        {
            if (IsAppropriateRun() &&
                IsCurrentSceneSave())
            {
                EditorHelper.ClearConsole();

                Check(EditorSceneManager.GetActiveScene().path);
            }
        }


        private void CheckAllScene()
        {
            if (IsAppropriateRun() &&
                IsCurrentSceneSave())
            {
                EditorHelper.ClearConsole();

                var tempScene = EditorSceneManager.GetActiveScene().path;

                var unityList = Directory.GetFiles(_sceneFolderPath, "*.unity", SearchOption.AllDirectories);
                for (int i = 0; i < unityList.Length; i++)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("检查场景中", unityList[i],
                        1f * i / unityList.Length))
                    {
                        break;
                    }
                    else
                    {
                        Check(unityList[i]);
                    }
                }
                EditorSceneManager.OpenScene(tempScene);
                EditorUtility.ClearProgressBar();
            }
        }


        public static void Check(string path)
        {
            var checker = ArtSceneChecker.CreateChecker(path);
            if (checker != null)
            {
                checker.Check();
            }
        }
    }
}