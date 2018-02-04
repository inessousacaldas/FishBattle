using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// 支持每个场景差异化设置全局雾效
/// @MarsZ 2017-07-27 15:35:45
/// </summary>
public class RawSceneRenderSettingsConfig : MonoBehaviour
{
    /// <summary>
    /// 当前场景的雾效设置
    /// </summary>
    public RenderSettingsConfigInfo RenderSettingsConfigInfo;

    /// <summary>
    /// 进入本场景前的雾效设置，记录之，以便退出本场景后还原
    /// </summary>
    private RenderSettingsConfigInfo _mCachedPreRenderSettingsConfigInfo;

    #if UNITY_EDITOR
    [MenuItem("美术/战斗/添加雾效脚本")]
    public static void AddFogConfigScript()
    {
        GameObject tGameObject =  GameObject.Find("SceneEffect");
        if(null == tGameObject)
        {
            Debug.LogError("请先添加特效挂点，名字为：SceneEffect");
            return;
        }
        RawSceneRenderSettingsConfig tRawSceneRenderSettingsConfig =  tGameObject.GetComponent<RawSceneRenderSettingsConfig>();
        if(null == tRawSceneRenderSettingsConfig)
            tRawSceneRenderSettingsConfig = tGameObject.AddComponent<RawSceneRenderSettingsConfig>();
        tRawSceneRenderSettingsConfig.ApplyLightFogConfig();
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        Debug.Log("添加雾效脚本并保存成功！");
    }
    #endif

    [ContextMenu("ApplyLightFogConfig")]
    public void ApplyLightFogConfig()
    {
        RenderSettingsConfigInfo = RenderSettingsConfigInfo.CreateByUnitySettings();
#if UNITY_EDITOR
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
#endif
    }

    private void OnEnable()
    {
        _mCachedPreRenderSettingsConfigInfo = CachePreStatus();
        ApplyFogConfigInGame(RenderSettingsConfigInfo);
    }

    private void OnDisable()
    {
        RevertPreStatus();
    }

    /// <summary>
    /// 缓存非本场景的全局雾效设置
    /// </summary>
    /// <returns>The pre status.</returns>
    private RenderSettingsConfigInfo CachePreStatus()
    {
        RenderSettingsConfigInfo tRenderSettingsConfigInfo = RenderSettingsConfigInfo.CreateByUnitySettings();
        return tRenderSettingsConfigInfo;
    }

    /// <summary>
    /// 还原到非本场景的雾效设置
    /// </summary>
    private void RevertPreStatus()
    {
        ApplyFogConfigInGame(_mCachedPreRenderSettingsConfigInfo);
    }

    private void ApplyFogConfigInGame(RenderSettingsConfigInfo pRenderSettingsConfigInfo)
    {
        if (null == pRenderSettingsConfigInfo)
        {
            Debug.LogError("ApplyFogConfigInGame failed , pFogConfigInfo ==  null ! ");   
            return;
        }
        pRenderSettingsConfigInfo.CopyToUnitySettings();
    }
}

[System.Serializable]
public class RenderSettingsConfigInfo
{
    public bool fog;

    public FogMode fogMode = FogMode.Linear;

    public Color fogColor = Color.white;

    public float fogDensity;

    public float fogEndDistance;

    public float fogStartDistance;

    public Color ambientLight;

    public Color ambientEquatorColor;

    public Color ambientGroundColor;

    public AmbientMode ambientMode;

    public float ambientIntensity;

    public static RenderSettingsConfigInfo CreateByUnitySettings()
    {
        RenderSettingsConfigInfo tRenderSettingsConfigInfo = new RenderSettingsConfigInfo();
        tRenderSettingsConfigInfo.fog                  = RenderSettings.fog;
        tRenderSettingsConfigInfo.fogMode              = RenderSettings.fogMode;
        tRenderSettingsConfigInfo.fogColor             = RenderSettings.fogColor;
        tRenderSettingsConfigInfo.fogDensity           = RenderSettings.fogDensity;
        tRenderSettingsConfigInfo.fogEndDistance       = RenderSettings.fogEndDistance;
        tRenderSettingsConfigInfo.fogStartDistance     = RenderSettings.fogStartDistance;
        tRenderSettingsConfigInfo.ambientLight         = RenderSettings.ambientLight;
        tRenderSettingsConfigInfo.ambientEquatorColor  = RenderSettings.ambientEquatorColor;
        tRenderSettingsConfigInfo.ambientGroundColor   = RenderSettings.ambientGroundColor;
        tRenderSettingsConfigInfo.ambientMode          = RenderSettings.ambientMode;
        tRenderSettingsConfigInfo.ambientIntensity     = RenderSettings.ambientIntensity;
        return tRenderSettingsConfigInfo;
    }

    public void CopyToUnitySettings()
    {
        RenderSettings.fog                      = this.fog;
        RenderSettings.fogMode                  = this.fogMode;
        RenderSettings.fogColor                 = this.fogColor;
        RenderSettings.fogDensity               = this.fogDensity;
        RenderSettings.fogEndDistance           = this.fogEndDistance;
        RenderSettings.fogStartDistance         = this.fogStartDistance;
        RenderSettings.ambientLight             = this.ambientLight;
        RenderSettings.ambientEquatorColor      = this.ambientEquatorColor;
        RenderSettings.ambientGroundColor       = this.ambientGroundColor;
        RenderSettings.ambientMode              = this.ambientMode;
        RenderSettings.ambientIntensity         = this.ambientIntensity;
    }
}