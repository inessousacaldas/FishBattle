using BaseClassNS;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace SkillEditor
{
    public class SkillEditorEditorWindow: BaseEditorWindow
    {
        private static readonly string SkillEditorScenesPath = "Assets/SkillEditor/Scenes";
        private static readonly string SkillEditorScenePath = SkillEditorScenesPath + "/SkillEditorScene.unity";
        private static readonly string SkillEditorUIScenePath = SkillEditorScenesPath + "/SkillEditorUIScene.unity";
        private static readonly string ENABLE_SKILLEDITOR = "ENABLE_SKILLEDITOR";
        private static readonly string UIViewGeneratorScrptRoot = "Assets";

        [MenuItem("Window/SkillEditor")]
        private static void Open()
        {
            Open<SkillEditorEditorWindow>();
        }

#if ENABLE_SKILLEDITOR
        [InitializeOnLoadMethod]
        private static void OnProjectLoadedInEditor()
        {
            FRPCodeGenerator.ScriptRoot = UIViewGeneratorScrptRoot;
        }
#endif

        protected override void CustomOnGUI()
        {
            Toggle("技能编辑器：", IsEnableSkillEditor());

            Space();
            Button("切换状态", ChangeSkillEditorState);

            Space();
            Button("游戏场景", OpenGameScene);
            Button("编辑器", OpenSkillEditorScene);
            Button("UI编辑", OpenSkillEidtorUIScene);
        }

        private bool IsEnableSkillEditor()
        {
#if ENABLE_SKILLEDITOR
            return true;
#endif
            return false;
        }

        private void ChangeSkillEditorState()
        {
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (IsEnableSkillEditor())
            {
                symbols = symbols.Replace(";" + ENABLE_SKILLEDITOR, "");
            }
            else
            {
                symbols = symbols + ";" + ENABLE_SKILLEDITOR;
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
        }

        private void OpenGameScene()
        {
            OpenScene(EditorBuildSettings.scenes[0].path);
        }

        private void OpenSkillEditorScene()
        {
            OpenScene(SkillEditorScenePath);
        }

        private void OpenSkillEidtorUIScene()
        {
            OpenScene(SkillEditorUIScenePath);
        }

        private void OpenScene(string path)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(path);
            }
        }
    }
}
