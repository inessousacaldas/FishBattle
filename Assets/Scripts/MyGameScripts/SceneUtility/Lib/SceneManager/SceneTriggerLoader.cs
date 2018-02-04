using AssetPipeline;
using SceneTrigger;
using UnityEngine;

public class SceneTriggerLoader : BaseCommand
{
    private SceneTriggerMgr sceneTriggerMgr;
    private string sceneName;

    public SceneTriggerLoader(SceneTriggerMgr sceneTriggerMgr, string sceneName)
    {
        this.sceneTriggerMgr = sceneTriggerMgr;
        this.sceneName = sceneName;
    }

    public override void Execute()
    {
        base.Execute();
        string sceneTriggerName = "sceneTrigger_" + sceneName;
        ResourcePoolManager.Instance.LoadStreamSceneConfig(
           sceneTriggerName,
           OnGetConfig,
           OnLoadError,
           AssetLoadPriority.SceneConfig
           );
    }

    private void OnGetConfig(Object asset)
    {
        TextAsset textAsset = asset as TextAsset;
        if (textAsset == null)
        {
            //TODO CCJ 暂时屏蔽，免得被策划叼
            //GameDebuger.LogError("GetTriggerConfigError:sceneTrigger_" + sceneName);
        }
        else
        {
            sceneTriggerMgr.Init(textAsset.text, sceneName);
        }

        OnFinish();
    }

    private void OnLoadError()
    {
        //TODO CCJ 暂时屏蔽，免得被策划叼
        //GameDebuger.LogError("GetTriggerConfigError:sceneTrigger_" + sceneName);
        OnFinish();
    }
}

