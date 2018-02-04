using UnityEngine;
using System.Collections;

public class SceneDataLoader : BaseCommand, ICommand {
	public SceneDataLoader (string sceneName)
	{
		this.sceneName = sceneName;
	}

	string sceneName;
	public override void Execute ()
	{
		base.Execute ();
	}
}
