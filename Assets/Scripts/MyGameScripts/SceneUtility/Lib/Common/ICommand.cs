using UnityEngine;
using System.Collections;

public interface ICommand {
	bool IsFinish ();
	void SetFinishCallback (System.Action<ICommand> onFinish);
	void Execute ();
}
