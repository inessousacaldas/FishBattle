using UnityEngine;

public class SceneLocalGoLoader : BaseCommand {
	string sceneName;
    private readonly SceneGoManager sceneGoManager;
	public SceneLocalGoLoader (SceneGoManager sceneGoManager, string sceneName)
	{
	    this.sceneGoManager = sceneGoManager;
        this.sceneName = sceneName;
	}

	public override void Execute ()
	{
		base.Execute ();
        string configName = "sg_" + sceneName;
        ProfileHelper.SystimeBegin(configName);
        AssetPipeline.ResourcePoolManager.Instance.LoadStreamSceneConfig(configName, this.OnGetSceneData, this.OnGetSceneDataError);
	}

	AllSceneGoInfo allSceneInfo;
	void OnGetSceneData (Object asset)
	{
        string configName = "sg_" + sceneName;
        ProfileHelper.SystimeEnd(configName);
        allSceneInfo = asset as AllSceneGoInfo;
        this.StartLoadSceneGo ();
	}

    void OnGetSceneDataError()
    {
        Debug.LogError("GetSceneDataError: sg_" + this.sceneName);
        OnFinish();
    }
	void OnPreloadFinish ()
	{
        ProfileHelper.SystimeEnd("Preload");
        this.OnFinish ();
	}

	void StartLoadSceneGo ()
	{
        sceneGoManager.SetupConfig (allSceneInfo);
        string configName = "sg_" + sceneName;
        ProfileHelper.SystimeEnd(configName);
        ProfileHelper.SystimeBegin("Preload");
        sceneGoManager.Preload (this.OnPreloadFinish);
	}
}
