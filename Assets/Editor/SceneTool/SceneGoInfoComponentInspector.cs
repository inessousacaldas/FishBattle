using System.Linq;
using UnityEditor;
using UnityEngine;


[CanEditMultipleObjects, CustomEditor(typeof(SceneGoInfoComponent))]
public class SceneGoInfoComponentInspector : Editor
{
    void Awake()
    {
        targets.Select(item => item as SceneGoInfoComponent)
            .ForEach(item =>
            {
                item.select = true;
                EditorUtility.SetDirty(item);
            });
    }

    void OnDestroy()
    {
        targets.Select(item => item as SceneGoInfoComponent)
            .ForEach(item =>
            {
                if (item != null)   //忽略脚本被Destroy的情况
                {
                    item.select = false;
                    EditorUtility.SetDirty(item);
                }
            });
    }
    public override void OnInspectorGUI()
    {
        SceneGoInfoComponent.showAll = GUILayout.Toggle(SceneGoInfoComponent.showAll, "ShowAllBounds");
        base.OnInspectorGUI();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

}
