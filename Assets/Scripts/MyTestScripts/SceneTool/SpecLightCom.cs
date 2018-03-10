using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("美术/SepcLight")]
public class SpecLightCom : MonoBehaviour
{
    private static int _SpecDir;
    private static int _SpecLightColor;

    public static int SpecDir
    {
        get
        {
            if (_SpecDir == 0)
                _SpecDir = Shader.PropertyToID("_SpecDir");
            return _SpecDir;
        }
    }
    public static int SpecLightColor
    {
        get
        {
            if (_SpecLightColor == 0)
                _SpecLightColor = Shader.PropertyToID("_SpecLightColor");
            return _SpecLightColor;
        }
    }


    public Light light;
    public Material[] mats;

    void LateUpdate()
    {
        if (light != null)
            light.cullingMask = 0;
            
        if (light != null && mats != null)
        {
            foreach (Material mat in mats)
            {
                if (mat != null)
                {
                    mat.SetVector(SpecDir, -light.transform.forward);
                    mat.SetVector(SpecLightColor, light.color * light.intensity);
                }
            }
        }
    }
}
