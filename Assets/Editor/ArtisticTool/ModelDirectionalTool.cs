using System;
using UnityEngine;
using UnityEditor;

public class ModelDirectionalTool : EditorWindow
{
    private static ModelDirectionalTool instance;

    [MenuItem("美术/卡通材质光照")]
    static void Open()
    {
        if (instance == null)
        {
            instance = GetWindow<ModelDirectionalTool>();
            instance.Show();
        }
        else
        {
            instance.Close();
            instance = null;
        }
    }

    private Light light;
    private Material[] mats = new Material[1];
    private Color ambientColor = Color.black;
    private float edgeThickness = float.MaxValue;
    private Color edgeColor = Color.black;
    private float difFactor = float.MaxValue;

    void Awake()
    {
        EditorApplication.update += Update;
    }

    void OnDestroy()
    {
        EditorApplication.update -= Update;
    }

    void Update()
    {
        if (light != null)
        {
            foreach (var mat in mats)
            {
                if (mat != null)
                {
                    mat.SetVector("_LightDir", -light.transform.forward);
                    mat.SetFloat("_diffFactor", light.intensity);
                    if(ambientColor != Color.black)
                        mat.SetColor("_AmbientColor", ambientColor);
                    if(edgeThickness != float.MaxValue)
                        mat.SetFloat("_EdgeThickness", edgeThickness);
                    if(edgeColor != Color.black)
                        mat.SetColor("_OutlineColor", edgeColor);
                    if(difFactor != float.MaxValue)
                        mat.SetFloat("_DirFactor", difFactor);

                }
            }
        }
    }

    void OnGUI()
    {
        light = EditorGUILayout.ObjectField("灯光", light, typeof(Light), true) as Light;
        ambientColor = EditorGUILayout.ColorField("环境光", ambientColor);
        difFactor = EditorGUILayout.Slider("平行光强弱", difFactor, 0f, 1f);
        edgeThickness = EditorGUILayout.FloatField("描边粗细", edgeThickness);
        edgeColor = EditorGUILayout.ColorField("描边色", edgeColor);

        EditorGUI.BeginChangeCheck();
        int lenght = EditorGUILayout.IntField("材质数量", mats.Length);
        if (EditorGUI.EndChangeCheck())
        {
            Material[] materials = new Material[lenght];
            Array.Copy(mats, materials, Mathf.Min(materials.Length, mats.Length));
            mats = materials;
        }
        for (int index = 0; index < mats.Length; index++)
        {
            mats[index] = EditorGUILayout.ObjectField("卡通材质" + (index + 1), mats[index], typeof (Material), true) as Material;
            if (mats[index] != null && mats[index].shader.name != "Baoyu/Unlit/Toon" && mats[index].shader.name != "Baoyu/Unlit/ModelOutline")
            {
                mats[index] = null;
            }
        }
    }
}
