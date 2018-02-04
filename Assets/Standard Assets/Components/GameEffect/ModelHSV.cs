using UnityEngine;
using System.Collections;

public class ModelHSV : MonoBehaviour
{
    //色相,饱和度,明度,混合值
    public Vector4 _RHueShift = new Vector4(0f, 1f, 1f, 0f);
    public Vector4 _GHueShift = new Vector4(0f, 1f, 1f, 0f);
    public Vector4 _BHueShift = new Vector4(0f, 1f, 1f, 0f);

    private Renderer _renderer;
    // Use this for initialization
    void Awake()
    {
        _renderer = this.GetComponent<Renderer>();
        UpdateModelHueMatrix();
    }

    void OnDestroy()
    {
        if (_renderer != null && _renderer.material != null)
            DestroyImmediate(_renderer.material);
    }

    #region EditorOnly

    void OnValidate()
    {
        _RHueShift = ValidateHueShift(_RHueShift);
        _GHueShift = ValidateHueShift(_GHueShift);
        _BHueShift = ValidateHueShift(_BHueShift);
        UpdateModelHueMatrix();
    }

    public void ChangeDebugMode(bool active)
    {
        if (active)
        {
            _renderer.material.shader = Shader.Find("Baoyu/Unlit/Hue-Debug");
        }
        else
            _renderer.material.shader = Shader.Find("Baoyu/Unlit/Xray-Hue");
    }

    #endregion

    //1,1,1,0;2,2,2,0;3,3,3,0
    public void SetupColorParams(string colorParams)
    {
        if (string.IsNullOrEmpty(colorParams))
        {
            Vector4 defaultVal = new Vector4(0f, 1f, 1f, 0f);
            SetupHueShift(defaultVal, defaultVal, defaultVal);
            return;
        }

        string[] splits = colorParams.Split(';');
        if (splits.Length != 3)
        {
            Debug.LogError("colorParams is error");
            return;
        }

        SetupHueShift(VectorHelper.ParseToVector4(splits[0], new Vector4(0f, 1f, 1f, 0f)),
                       VectorHelper.ParseToVector4(splits[1], new Vector4(0f, 1f, 1f, 0f)),
                       VectorHelper.ParseToVector4(splits[2], new Vector4(0f, 1f, 1f, 0f)));
    }

    public void SetupHueShift(Vector4 rHueShift, Vector4 gHueShift, Vector4 bHueShift)
    {
        _RHueShift = ValidateHueShift(rHueShift);
        _GHueShift = ValidateHueShift(gHueShift);
        _BHueShift = ValidateHueShift(bHueShift);
        UpdateModelHueMatrix();
    }

    private Vector4 ValidateHueShift(Vector4 hueShift)
    {
        hueShift.x = Mathf.Clamp(hueShift.x, 0f, 360f);
        hueShift.y = Mathf.Clamp(hueShift.y, 0f, 2f);
        hueShift.z = Mathf.Clamp(hueShift.z, 0f, 2f);
        hueShift.w = Mathf.Clamp(hueShift.w, 0f, 1f);
        return hueShift;
    }

	private void UpdateModelHueMatrix(Vector4 pRHueShift,Vector4 pGHueShift,Vector4 pBHueShift)
	{
		if (_renderer != null)
		{
            Material material = _renderer.material;
            Matrix4x4 RHueMatrix = GenerateHSVMatrix(pRHueShift.x, pRHueShift.y, pRHueShift.z);
            material.SetVector("_RHueShift1", RHueMatrix.GetRow(0));
			material.SetVector("_RHueShift2", RHueMatrix.GetRow(1));
            material.SetVector("_RHueShift3", RHueMatrix.GetRow(2));

            Matrix4x4 GHueMatrix = GenerateHSVMatrix(pGHueShift.x, pGHueShift.y, pGHueShift.z);
            material.SetVector("_GHueShift1", GHueMatrix.GetRow(0));
            material.SetVector("_GHueShift2", GHueMatrix.GetRow(1));
            material.SetVector("_GHueShift3", GHueMatrix.GetRow(2));

            Matrix4x4 BHueMatrix = GenerateHSVMatrix(pBHueShift.x, pBHueShift.y, pBHueShift.z);
            material.SetVector("_BHueShift1", BHueMatrix.GetRow(0));
            material.SetVector("_BHueShift2", BHueMatrix.GetRow(1));
            material.SetVector("_BHueShift3", BHueMatrix.GetRow(2));

			material.SetFloat("_blendFactorR", pRHueShift.w);
			material.SetFloat("_blendFactorG", pGHueShift.w);
			material.SetFloat("_blendFactorB", pBHueShift.w);
        }
    }

    private void UpdateModelHueMatrix()
    {
		UpdateModelHueMatrix (_RHueShift,_GHueShift,_BHueShift);
    }

    private Matrix4x4 GenerateHSVMatrix(float H, float S, float V)
    {
        float VSU = V * S * Mathf.Cos(Mathf.Deg2Rad * H);
        float VSW = V * S * Mathf.Sin(Mathf.Deg2Rad * H);

        Matrix4x4 T_HSV = Matrix4x4.zero;
        T_HSV.SetRow(0, new Vector4(.299f * V + .701f * VSU + .168f * VSW, .587f * V - .587f * VSU + .330f * VSW, .114f * V - .114f * VSU - .497f * VSW, 0));
        T_HSV.SetRow(1, new Vector4(.299f * V - .299f * VSU - .328f * VSW, .587f * V + .413f * VSU + .035f * VSW, .114f * V - .114f * VSU + .292f * VSW, 0));
        T_HSV.SetRow(2, new Vector4(.299f * V - .3f * VSU + 1.25f * VSW, .587f * V - .588f * VSU - 1.05f * VSW, .114f * V + .886f * VSU - .203f * VSW, 0));

        return T_HSV;
    }
    //暂时保留强制刷新接口，以后出问题可以在业务层热更
	/// <summary>
	/// Refreshs the model hue matrix.
	/// <see cref="http://oa.cilugame.com/redmine/issues/10915"/>
	/// </summary>
	public void RefreshModelHueMatrix()
	{
		if(null != gameObject && gameObject.activeInHierarchy)
			StartCoroutine (RefreshModelHueMatrixDelay());
	}

	private IEnumerator RefreshModelHueMatrixDelay()
	{
		UpdateModelHueMatrix (Vector4.zero,Vector4.zero,Vector4.zero);
		yield return null;
		UpdateModelHueMatrix ();
	}
}
