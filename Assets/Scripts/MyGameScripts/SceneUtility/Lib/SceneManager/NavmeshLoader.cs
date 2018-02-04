using UnityEngine;
using AssetPipeline;

public class NavmeshLoader : BaseCommand {
	string sceneName;
	public NavmeshLoader (string sceneName)
	{
		this.sceneName = sceneName;
	}

	public override void Execute ()
	{
		base.Execute ();
        string navmeshName = "navmesh_" + this.sceneName;
        ResourcePoolManager.Instance.LoadStreamSceneConfig(
            navmeshName, 
            this.OnGetNavmeshAsset,
            OnLoadError,
            AssetLoadPriority.SceneConfig
            );
	}
	void OnGetNavmeshAsset (Object asset)
	{
        TextAsset textAsset = asset as TextAsset;
        if(textAsset == null)
            GameDebuger.LogError("GetNavConfigError:navmesh_" + sceneName);
        else
	        AstarPath.active.astarData.DeserializeGraphs(textAsset);
        this.OnFinish();
    }
    private void OnLoadError()
    {
        GameDebuger.LogError("GetNavConfigError:navmesh_" + sceneName);
        OnFinish();
    }
}
