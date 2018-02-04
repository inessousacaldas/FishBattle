using System;
using System.Collections.Generic;
using System.Threading;

public class ThreadManager
{
	public const string ThreadManagerTimerTask = "ThreadManagerTimerTask";
	private bool _hasSetTimerTask;

	private static ThreadManager _instance;

	public static ThreadManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ThreadManager();
			}

			return _instance;
		}
	}

	private List<ThreadTask> _threadList = new List<ThreadTask>();


	public void AddThread(ThreadTask task)
	{
		CheckTimerTask();

		_threadList.Add(task);
	}

	private void CheckTimerTask()
	{
		if (!_hasSetTimerTask)
		{
			_hasSetTimerTask = true;
			CSTimer.Instance.SetupTimer(ThreadManagerTimerTask, CheckSaveFileList);
		}
	}

	private void CheckSaveFileList()
	{
		for (int i = _threadList.Count - 1; i >= 0; i--)
		{
			var task = _threadList[i];
			switch (task.State)
			{
				case ThreadTask.ThreadState.None:
					{
						task.Start();
						break;
					}
				case ThreadTask.ThreadState.Starting:
					{
						break;
					}
				case ThreadTask.ThreadState.Finished:
					{
						task.ActiveFinished();
						break;
					}
				case ThreadTask.ThreadState.Dead:
					{
						_threadList.RemoveAt(i);
						break;
					}
			}
		}
	}
}


/// <summary>
/// Thread任务，通过继承来扩展
/// </summary>
public class ThreadTask
{
	public enum ThreadState
	{
		None,
		Starting,
		Finished,
		Dead,
	}
	protected ThreadState _state = ThreadState.None;

	public ThreadState State
	{
		get { return _state; }
	}

	protected Action<ThreadTask> _targetAction;
	protected Action _finishedAction;

	public ThreadTask(Action<ThreadTask> targetAction = null, Action finishedAction = null)
	{
		_targetAction = targetAction ?? (task => task.SetFinished()) ;
		_finishedAction = finishedAction;
	}

	public void Start()
	{
		if (_state != ThreadState.None)
		{
			return;
		}
		_state = ThreadState.Starting;

		var thread = new Thread(Starting);
		thread.Start();
	}


	private void Starting()
	{
		if (_targetAction != null)
		{
			_targetAction(this);
		}
	}

	public void SetFinished()
	{
		_state = ThreadState.Finished;
	}

	public void ActiveFinished()
	{
		if (_state == ThreadState.Finished && _finishedAction != null)
		{
			_state = ThreadState.Dead;

			_finishedAction();
		}
	}
}