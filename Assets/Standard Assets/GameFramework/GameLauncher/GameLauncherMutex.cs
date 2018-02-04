#if UNITY_STANDALONE || UNITY_EDITOR

using System;


public static class GameLauncherMutex
{
	private static System.Threading.Mutex _mutex;
	private static int _curMutexTag = 1;

	public static int GetMutexTag(string mutexName)
	{
		if (_mutex != null)
		{
			return _curMutexTag;
		}

		for (int i = 0; i < Int32.MaxValue; i++)
		{
			var createNew = false;
			var mutex = new System.Threading.Mutex(true, mutexName + (i + 1), out createNew);
			if (createNew)
			{
				_mutex = mutex;
				_curMutexTag = i + 1;
				return _curMutexTag;
			}
		}

		return _curMutexTag;
	}
}

#endif