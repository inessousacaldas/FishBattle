using System;
using UnityEngine;
using SceneTrigger;
public class MySceneManager
{

    private static MySceneManager instance;

    public static MySceneManager Instance
    {
        get
        {
            if(instance  == null)
                instance = new MySceneManager();
            return instance;
        }
    }

    private MySceneManager()
    {

    }

    public string currentSceneName ;

    public SceneTriggerMgr sceneTriggerMgr { get; private set; }
    public SceneGoManager sceneGoManager { get; private set; }

	System.Action onChangeFinish;
	QueueCommandRunner commandRunner;
    
	public void ChangeToScene (string sceneName, System.Action onChangeFinish, Vector3 playerPos){
        if (currentSceneName == sceneName)
        {
            onChangeFinish();
            return;
        }
	    if (commandRunner != null)
	        return;
		//ProfileHelper.SystimeBegin("Change To Scene " + sceneName);
		this.onChangeFinish = onChangeFinish;
		commandRunner = new QueueCommandRunner ();
        SceneTriggerMgr oldTriggerMgr = sceneTriggerMgr;
        sceneTriggerMgr = new SceneTriggerMgr();
	    SceneGoManager oldSceneGoMgr = sceneGoManager;
        sceneGoManager = new SceneGoManager(this);
		MySceneLoader sceneLoader = new MySceneLoader (sceneName, playerPos, sceneTriggerMgr, sceneGoManager);
		MySceneReleaser sceneReleaser = new MySceneReleaser (currentSceneName, oldTriggerMgr, oldSceneGoMgr);
        commandRunner.Add(sceneReleaser);
		commandRunner.Add (sceneLoader);
		commandRunner.SetFinishCallback (this.OnChangeFinish); 
		commandRunner.Execute ();
		this.currentSceneName = sceneName;
	}

    System.Action<float> onProgress = null;
    public void ChangeToScene (string sceneName, Vector3 playerPos, System.Action<float> onProgress, System.Action onChangeFinish)
    {
        this.onProgress = onProgress;
        this.onChangeFinish = onChangeFinish;
        this.ChangeToScene(sceneName, onChangeFinish, playerPos);
    }


	void OnChangeFinish (ICommand command)
	{
        commandRunner = null;
        //ProfileHelper.SystimeEnd("Change To Scene " + currentSceneName);
        if (this.onProgress != null)
            this.onProgress(1f);
        if (this.onChangeFinish != null)
            this.onChangeFinish();
        this.onProgress = null;
        this.onChangeFinish = null;

	}

    public void Releaser(bool ignoreChangeFinish = false, Action onFinish = null)
    {
        MySceneReleaser sceneReleaser = new MySceneReleaser(this.currentSceneName, sceneTriggerMgr, sceneGoManager);
        currentSceneName = null;
        if (commandRunner != null)
        {
            if (!ignoreChangeFinish && onChangeFinish != null)
                onChangeFinish += onFinish;
            commandRunner.Add(sceneReleaser);
        }
        else
        {
            onChangeFinish = onFinish;
            commandRunner = new QueueCommandRunner();
            commandRunner.SetFinishCallback(OnChangeFinish);
            commandRunner.Add(sceneReleaser);
            commandRunner.Execute();
        }
    }
}
