using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallelCommandRunner : ICommand{
	List<ICommand> commandList;
	List<ICommand> finishCommandList;

	public ParallelCommandRunner ()
	{
		this.commandList = new List<ICommand> ();
		this.finishCommandList = new List<ICommand> ();
	}

	System.Action <ICommand> onFinish;
	bool isFinish = false;
	public bool IsFinish ()
	{
		return isFinish;
	}

	public void SetFinishCallback (System.Action <ICommand> onFinish)
	{
		this.onFinish = onFinish;
	}

	public void Add (ICommand command)
	{
		if (this.commandList.Contains (command))
			return;
		this.commandList.Add (command);
	}

	public void Execute ()
	{
		for (int i = 0; i < commandList.Count; i++)
		{
			ICommand command = commandList[i];
			command.SetFinishCallback (this.OnCommandFinish);
			command.Execute ();
		}
	}

	void OnCommandFinish (ICommand command)
	{
		if (!this.commandList.Contains (command) || this.finishCommandList.Contains (command))
			return;
		this.finishCommandList.Add (command);

		if (this.commandList.Count == this.finishCommandList.Count)
		{
			if (this.onFinish != null)
				this.onFinish (this);
			return;
		}
	}
}
