using UnityEngine;
using SceneTrigger;
//这个类做的事情是:
//在进入场景前,做好场景的加载工作
//
public class MySceneLoader : ICommand{
	LightMapLoader lightMapLoader;
	NavmeshLoader navmeshLoader;
	SceneLocalGoLoader sceneLocalGoLoader;
    SceneTriggerLoader sceneTriggerLoader;

    System.Action<float> onProgress;
	bool isFinish = false;
	System.Action <ICommand> onFinish;
	QueueCommandRunner sequenceTask = new QueueCommandRunner ();
	ParallelCommandRunner allTask = new ParallelCommandRunner ();
	string sceneName;

    public MySceneLoader (string sceneName, Vector3 playerPos, SceneTriggerMgr sceneTriggerMgr, SceneGoManager sceneGoManager)
    {
		this.sceneName = sceneName;
		this.lightMapLoader = new LightMapLoader (sceneName);
		this.navmeshLoader = new NavmeshLoader (sceneName);
		this.sceneLocalGoLoader = new SceneLocalGoLoader (sceneGoManager, sceneName);
	    this.sceneTriggerLoader = new SceneTriggerLoader(sceneTriggerMgr, sceneName);
		this.allTask.Add (lightMapLoader);
        CamFollowPlayerPosCommand camFollowCommand = new CamFollowPlayerPosCommand(playerPos);
		this.sequenceTask.Add (this.navmeshLoader);
        this.sequenceTask.Add(camFollowCommand);
		this.sequenceTask.Add (this.sceneLocalGoLoader);
		this.allTask.Add (this.sequenceTask);
        this.allTask.Add(sceneTriggerLoader);
	}

	void OnAllTaskFinish (ICommand allTaskCommand)
	{
		this.isFinish = true;
		if (this.onFinish != null)
		{
			//this.isFinish = true;
			this.onFinish (this);
		}
	}

	public void Execute ()
	{
		this.allTask.SetFinishCallback (this.OnAllTaskFinish);
		this.allTask.Execute ();
	}

	public bool IsFinish ()
	{
		return isFinish;
	}

	public void SetFinishCallback (System.Action<ICommand> onFinish)
	{
		this.onFinish = onFinish;
	}
}
	