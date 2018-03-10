using System.Collections.Generic;
using AssetPipeline;
using UnityEngine;

public class ModelArtistic
{
    #region PropertyToID
    private static int _ColorAlphaID;
    private static int _PlaneNormalID;
    private static int _TerrainID;
    private static int ColorAlphaId
    {
        get
        {
            if (_ColorAlphaID == 0)
                _ColorAlphaID = Shader.PropertyToID("_ColorAlpha");
            return _ColorAlphaID;
        }
    }
    private static int TerrainID
    {
        get
        {
            if (_TerrainID == 0)
                _TerrainID = Shader.PropertyToID("_Terrain");
            return _TerrainID;
        }
    }

    #endregion
    private readonly GameObject root;
    private List<Renderer> rendererList;
    private List<Material> shadowMaterials;
    private Transform shadowPoin;
    private bool shadowActive;
    private float matParam;
    private bool isInstanceMaterials; //用于标记客户端是否实例化过材质
    private List<Material> occlusionMaterials;
    private bool occlusionActive;

    public ModelArtistic(GameObject root)
    {
        this.root = root;
        rendererList = new List<Renderer>();
        shadowMaterials = new List<Material>();
        occlusionMaterials = new List<Material>();
    }
    public void LateUpdate()
    {
        if (shadowActive)
        {
            CaculateShadowParam();
            ResetShadowParam();
        }
    }

    public void RefreshRenderList()
    {
        isInstanceMaterials = false;
        rendererList.Clear();
        var renderers = root.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (r.sharedMaterial == null) continue;
            if (r.name.StartsWith("Shadow")) continue;
            if (r is SkinnedMeshRenderer || r is MeshRenderer)
            {
                rendererList.Add(r);
            }
        }
        RefreshMaterials();
        ResetShadowParam();
        ResetOcclusionState(this.occlusionActive);
    }
    private void RefreshMaterials()
    {
        shadowMaterials.Clear();
        occlusionMaterials.Clear();
        for (int i = 0; i < rendererList.Count; i++)
        {
            Renderer renderer = rendererList[i];
            Material[] materials = isInstanceMaterials ? renderer.sharedMaterials : renderer.materials;
            for (int j = 0; j < materials.Length; j++)
            {
                Material mat = materials[j];
                string shaderName = mat.shader.name;
                if (shaderName == "Baoyu/Unlit/Shadow")
                {
                    shadowMaterials.Add(mat);
                }
                else if (shaderName == "Baoyu/Unlit/ToonOcclusion" || shaderName == "Baoyu/Unlit/Toon")
                {
                    occlusionMaterials.Add(mat);
                }
            }
        }
        isInstanceMaterials = true;
    }
    #region Shadow

    private void CaculateShadowParam()
    {
        if (shadowPoin == null)
            return;
        matParam = shadowPoin.position.y;
    }

    private void ResetShadowParam()
    {
        for (int i = 0; i < shadowMaterials.Count; i++)
        {
            Material mat = shadowMaterials[i];
            ResetShadowMatParam(mat);
        }
    }
    private void ResetShadowMatParam(Material mat)
    {
        mat.SetColor(ColorAlphaId, new Color(1, 1, 1, shadowActive ? 1 : 0));
        mat.SetFloat(TerrainID, matParam);
    }
    public void SetShadowActive(bool active, GameObject shadowParentGo)
    {
        shadowActive = active;
        if(active == true)
            shadowPoin = shadowParentGo.transform.Find(ModelHelper.Mount_shadow);
        ResetShadowParam();
    }

    public bool CheckShadowMat()
    {
        return shadowMaterials.Count > 0;
    }
    #endregion
    #region 场景遮挡shader

    public void SetOcclusion(bool occlusionActive)
    {
        if (this.occlusionActive != occlusionActive)
        {
            this.occlusionActive = occlusionActive;
            ResetOcclusionState(this.occlusionActive);
        }
    }

    private void ResetOcclusionState(bool occlusionActive)
    {
        for (int i = 0; i < occlusionMaterials.Count; i++)
        {
            Material mat = occlusionMaterials[i];
            SetMaterialOcclusion(mat, occlusionActive);
        }
    }
    private void SetMaterialOcclusion(Material material, bool state)
    {
        if (state)
        {
            if (material.shader.name == "Baoyu/Unlit/Toon")
                material.shader = AssetManager.Instance.FindShader("Baoyu/Unlit/ToonOcclusion");
        }
        else
        {
            if (material.shader.name == "Baoyu/Unlit/ToonOcclusion")
                material.shader = AssetManager.Instance.FindShader("Baoyu/Unlit/Toon");
        }
    }
    #endregion
    public void Clear()
    {
        rendererList.Clear();
        shadowMaterials.Clear();
        //先还原到没有遮挡的状态
        ResetOcclusionState(false);
        occlusionMaterials.Clear();
    }
}
