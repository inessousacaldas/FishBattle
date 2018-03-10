using UnityEngine;
using System.Collections;

public class BaseCommand : ICommand {
	protected System.Action <ICommand> onFinish;
	protected bool isFinish = false;
	public virtual bool IsFinish ()
	{
		return isFinish;
	}

	public virtual void SetFinishCallback (System.Action <ICommand> onFinish)
	{
		this.onFinish += onFinish;
	}

	public virtual void Execute ()
	{
		//Debug.LogError ("Start:" + this.GetPropertyType ().Name + "Time:" + Time.realtimeSinceStartup); 
		this.isFinish = false;
	}

	public virtual void OnFinish ()
	{
		//Debug.LogError ("OnFinish:" + this.GetPropertyType ().Name + "Time:" + Time.realtimeSinceStartup); 
		if (this.isFinish)
			return;
		this.isFinish = true;
		if (this.onFinish != null)
			this.onFinish (this);
	}
}
