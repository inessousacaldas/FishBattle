using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QueueCommandRunner : ICommand {

	Queue <ICommand> commandQueue = new Queue<ICommand> ();
	bool isFinish = false;
	public System.Action<ICommand> onFinish = null;

	public bool IsFinish ()
	{
		return isFinish;
	}

	public void SetFinishCallback (System.Action <ICommand> onFinish)
	{
		this.onFinish = onFinish;
	}

	public void Execute (ICommand command)
	{
		this.Add (command);
		this.Execute ();
	}

	public void Execute ()
	{
		ICommand nextCommand = this.commandQueue.Peek ();
		if (nextCommand == null || nextCommand.IsFinish ())
			return;
		nextCommand.Execute ();
	}

	public void Add (ICommand command)
	{
		if (command == null)
			return;
		this.isFinish = false;
		command.SetFinishCallback (this.OnCommandFinish);
		this.commandQueue.Enqueue (command);
	}

	void OnCommandFinish (ICommand command)
	{
		if (this.commandQueue.Count == 0 || this.commandQueue.Peek () != command)
			return;
		this.commandQueue.Dequeue ();
		if (this.commandQueue.Count == 0)
		{
			if (this.onFinish != null)
			{
				this.isFinish = true;
				this.onFinish (this);
			}
			return;	
		}

		ICommand nextCommand = this.commandQueue.Peek ();
		nextCommand.Execute ();
	}
}
