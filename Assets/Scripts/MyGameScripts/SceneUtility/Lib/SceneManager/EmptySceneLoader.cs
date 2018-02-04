using UnityEngine;
using System.Collections;

public class EmptySceneLoader : BaseCommand {
    string sceneName;
    public EmptySceneLoader (string sceneName)
    {
        this.sceneName = sceneName;
    }

    public override void Execute()
    {
        base.Execute();
        AssetPipeline.AssetManager.Instance.LoadLevelAsync(this.sceneName, false, () =>
        {
            this.OnFinish();
        }, null, ()=> { Debug.LogError("EmptySceneLoader LoadFail:" + this.sceneName); });
    }
}
