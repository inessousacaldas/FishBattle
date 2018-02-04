#if ENABLE_SKILLEDITOR

using UnityEngine;
using UnityEngine.SceneManagement;

namespace SkillEditor
{
    public class SkillEditorLauncher: MonoBehaviour
    {
        private void Start()
        {
            AppGameManager.InitHook += Lanuchering;

            SceneManager.LoadScene(0);
        }

        private void Lanuchering()
        {
            AppGameManager.InitHook -= Lanuchering;

            ProxySkillEditor.EnterSkillEditor();
        }
    }
}

#endif