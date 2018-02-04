#if UNITY_EDITOR 
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ReplaceView)), CanEditMultipleObjects]
public class ABC_Editor : Editor
{
    private SerializedObject obj;
    public static SerializedProperty prefabList;
    private void OnEnable()
    {
        obj = new SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        ReplaceView abc = target as ReplaceView;
        obj.Update();
        EditorGUILayout.PropertyField(obj.FindProperty("prefab"), true);
        EditorGUILayout.PropertyField(obj.FindProperty("needCheck"), true);
        if (abc != null)
        {
            if (GUILayout.Button("Replace"))
            {
                abc.DrawBtn();
            }
        }
        obj.ApplyModifiedProperties();
    }

}
#endif