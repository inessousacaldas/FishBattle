using UnityEngine;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public static class SkillEditorHelper
    {
        public static GameObject LoadPreab(string name)
        {
            return LoadPreab(SkillEditorConst.UIPrefabPath, name);
        }
        public static GameObject LoadPreab(string path, string name)
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("{0}/{1}.prefab", path, name));
        }

        public static C CreateController<C>(this GameObject go) where C : IViewController, new()
        {
            var controller = new C();
            controller.SetupView(go);

            return controller;
        }

        public static C CreateAndSetItemFast<C>(string name, GameObject parent = null) where C : IViewController, new()
        {
            var prefab = LoadPreab(name);
            GameObject go;
            if (parent != null)
            {
                go = GameObjectExt.AddChild(parent, prefab);
            }
            else
            {
                go = Object.Instantiate(prefab);
            }
            return go.CreateController<C>();
        }
    }
}
#endif