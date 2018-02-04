using UnityEngine;
using System.Collections;
using AssetPipeline;

public class LightMapLoader : BaseCommand{
	string sceneName;
    string assetName;
	public override void Execute ()
	{
		base.Execute ();
        this.assetName = "lightmap_" + sceneName;
        ProfileHelper.SystimeBegin(assetName);
        ResourcePoolManager.Instance.LoadStreamSceneConfig(this.assetName, this.OnLightMapAssetLoaded, this.OnLightMapAssetLoadedError, AssetLoadPriority.StreamScene);
	}

	public LightMapLoader (string sceneName)
	{
		this.sceneName = sceneName;
	}

    
    void OnLightMapAssetLoadedError()
    {
        GameDebuger.LogError("OnLightMapAssetLoadError: lightmap_"+ sceneName);
        OnFinish();

    }

    void OnLightMapAssetLoaded(Object asset)
    {

        LightMapAsset lightMapAsset = asset as LightMapAsset;

        AmbientSetting ambientSetting = lightMapAsset.ambientSetting;

        if (ambientSetting != null)
        {
            SetFog(ambientSetting);
        }
        int Count = lightMapAsset.lightmapFar.Length;
        LightmapData[] lightmapDatas = new LightmapData[Count];
        for (int i = 0; i < Count; ++i)
        {
            LightmapData Lightmap = new LightmapData();
            Lightmap.lightmapColor = lightMapAsset.lightmapFar[i];
            Lightmap.lightmapDir = lightMapAsset.lightmapNear[i];
            lightmapDatas[i] = Lightmap;
        }

        LightmapSettings.lightmaps = lightmapDatas;

        LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
        Resources.UnloadAsset(asset);
        //MySceneManager.Instance.lastLightMapAsset = asset;
        ProfileHelper.SystimeEnd(assetName);
        this.OnFinish ();
    }

	void SetFog(AmbientSetting ambientSetting)
	{
		RenderSettings.fog = ambientSetting.useFog;
		if (ambientSetting.useFog) {
			RenderSettings.fogMode = ambientSetting.fogMode;
			RenderSettings.fogColor = ambientSetting.fogColor;
			RenderSettings.fogEndDistance = ambientSetting.fogEndDistance;
			RenderSettings.fogStartDistance = ambientSetting.fogStartDistance;
		}
	}
}
