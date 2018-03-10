using UnityEngine;
using SceneTrigger;
public class MySceneReleaser : BaseCommand {
    private string sceneName;
    private SceneGoManager lastSceneGoMgr;
    private SceneTriggerMgr lastTriggerMgr;

    public MySceneReleaser(string sceneName, SceneTriggerMgr lastTriggerMgr, SceneGoManager sceneGoManager)
    {
        this.sceneName = sceneName;
        this.lastTriggerMgr = lastTriggerMgr;
        this.lastSceneGoMgr = sceneGoManager;
    }

    public override void Execute ()
	{
		base.Execute ();
		if (string.IsNullOrEmpty(sceneName)) {
			this.OnFinish ();
			return;
		}
        //ProfileHelper.SystimeBegin("sceneName Releaser");
        lastTriggerMgr.Dispose();

        lastSceneGoMgr.Clear (this.OnClearFinish);
	}

	void OnClearFinish ()
	{
        LightmapSettings.lightmaps = null;
        RenderSettings.skybox = null;
        //调用UnloadUnuse释放
        GameEventCenter.SendEvent(GameEvent.OnSceneChangeEnd);
        //ProfileHelper.SystimeEnd("sceneName Releaser");
        this.OnFinish ();
	}
}