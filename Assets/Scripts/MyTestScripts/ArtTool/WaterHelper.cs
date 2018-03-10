#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
[ExecuteInEditMode]
public class WaterHelper : MonoBehaviour
{
    public Light light;

    void Update()
    {
        if(light == null)
            return;
        var meshRender = gameObject.GetComponent<MeshRenderer>();
        if(meshRender == null)
            return;
        var mat = meshRender.sharedMaterial;
        if(mat == null)
            return;
        mat.SetVector("_LightDir", -light.transform.forward);
        mat.SetVector("_LightColor", light.color * light.intensity);
        EditorUtility.SetDirty(mat);
    }
}

#endif
