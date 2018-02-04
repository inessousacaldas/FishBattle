using UnityEditor;
using UnityEngine;

public class ShadowToolView : EditorWindow
{
    private static ShadowToolView instance;
    [MenuItem("美术/光照调整")]
    public static void OnClickMenuitem()
    {
        if (instance == null)
        {
            instance = EditorWindow.CreateInstance<ShadowToolView>();
            instance.Show();
        }
        else
        {
            instance.Close();
            instance = null;
        }
    }

    void Awake()
    {
        EditorApplication.update += Update;
    }

    void OnDestroy()
    {
        EditorApplication.update -= Update;
    }

    private Light light;
    private Vector3 shadowDir;
    //private ShadowTest[] shadowTests;
    void OnGUI()
    {
        light = EditorGUILayout.ObjectField("平行光", light, typeof(Light), true) as Light;
        Update();
        if (GUILayout.Button("拷贝光照方向"))
        {
            EditorGUIUtility.systemCopyBuffer = string.Format("{0}:{1}:{2}", shadowDir.x, shadowDir.y, shadowDir.z);
        }
    }

    private void Update()
    {
        if (light != null)
        {
            var newShadowDir = light.transform.forward;
            newShadowDir.Normalize();
            if (newShadowDir != shadowDir)
            {
                shadowDir = newShadowDir;
                Shader.SetGlobalVector("_WorldShadowDir", shadowDir);
                //ShadowTest.shadowDir = shadowDir;
                SceneArtistic.shadowDir = shadowDir;
            }
        }
    }
}
