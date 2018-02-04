using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class GameEventCenter
{
	private static GameEventCenter _instance;
	public static GameEventCenter Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GameEventCenter();
			}
			return _instance;
		}
	}
	public void AddGameEventAgent(IGameEventAgent agent)
	{
		gameEventAgentList.Add(agent);
	}
	public static void AddListener(string eventName, Action action)
	{
		GameEventAgent.Instance.AddListener(eventName, action);
	}
	public static void AddListener(GameEvents.Event gameEvent, Action action)
	{
		GameEventAgent.Instance.AddListener(gameEvent.eventName, action);
	}

	public static void SendEvent(GameEvents.Event gameEvent)
	{
		GameEventAgent.Instance.Invoke(gameEvent.eventName);
	}
	public static void RemoveListener(GameEvents.Event gameEvent, System.Action action)
	{
		GameEventAgent.Instance.RemoveListener(gameEvent.eventName, action);
	}
	public static void RemoveListener(string eventName, Action action)
	{
		GameEventAgent.Instance.RemoveListener(eventName, action);
	}
	
	public static void AddListener<T>(GameEvents.Event<T> gameEvent, Action<T> action)
	{
		GameEventAgent<T>.Instance.AddListener(gameEvent.eventName, action);
	}

	public static void SendEvent<T>(GameEvents.Event<T> gameEvent, T param)
	{
		GameEventAgent<T>.Instance.Invoke(gameEvent.eventName, param);
	}
	public static void RemoveListener<T>(GameEvents.Event<T> gameEvent, System.Action<T> action)
	{
		GameEventAgent<T>.Instance.RemoveListener(gameEvent.eventName, action);
	}
	public static void RemoveListener<T>(GameEvents.Event<T> gameEvent)
	{
		GameEventAgent<T>.Instance.RemoveListener(gameEvent.eventName);
	}
	public static bool CheckHaveListen<T>(GameEvents.Event<T> gameEvent)
	{
		return GameEventAgent<T>.Instance.CheckHaveListen(gameEvent.eventName);
	}
	public static void SendEvent<T1, T2>(GameEvents.Event<T1, T2> gameEvent, T1 param1, T2 param2)
	{
		GameEventAgent<T1, T2>.Instance.Invoke(gameEvent.eventName, param1, param2);
	}
	public static void AddListener<T1, T2>(GameEvents.Event<T1, T2> gameEvent, System.Action<T1, T2> action)
	{
		GameEventAgent<T1, T2>.Instance.AddListener(gameEvent.eventName, action);
	}
	public static void RemoveListener<T1, T2>(GameEvents.Event<T1, T2> gameEvent, System.Action<T1, T2> action)
	{
		GameEventAgent<T1, T2>.Instance.RemoveListener(gameEvent.eventName, action);
	}
	public static void RemoveListener<T1, T2>(GameEvents.Event<T1, T2> gameEvent)
	{
		GameEventAgent<T1, T2>.Instance.RemoveListener(gameEvent.eventName);
	}
	public static bool CheckHaveListen<T1, T2>(GameEvents.Event<T1, T2> gameEvent)
	{
		return GameEventAgent<T1, T2>.Instance.CheckHaveListen(gameEvent.eventName);
	}
	public static void SendEvent<T1, T2, T3>(GameEvents.Event<T1, T2, T3> gameEvent, T1 param1, T2 param2, T3 param3)
	{
		GameEventAgent<T1, T2, T3>.Instance.Invoke(gameEvent.eventName, param1, param2, param3);
	}
	public static void AddListener<T1, T2, T3>(GameEvents.Event<T1, T2, T3> gameEvent, System.Action<T1, T2, T3> action)
	{
		GameEventAgent<T1, T2, T3>.Instance.AddListener(gameEvent.eventName, action);
	}
	public static void RemoveListener<T1, T2, T3>(GameEvents.Event<T1, T2, T3> gameEvent, System.Action<T1, T2, T3> action)
	{
		GameEventAgent<T1, T2, T3>.Instance.RemoveListener(gameEvent.eventName, action);
	}
	public static void RemoveListener<T1, T2, T3>(GameEvents.Event<T1, T2, T3> gameEvent)
	{
		GameEventAgent<T1, T2, T3>.Instance.RemoveListener(gameEvent.eventName);
	}
	public static bool CheckHaveListen<T1, T2, T3>(GameEvents.Event<T1, T2, T3> gameEvent)
	{
		return GameEventAgent<T1, T2, T3>.Instance.CheckHaveListen(gameEvent.eventName);
	}
	public static void SendEvent<T1, T2, T3, T4>(GameEvents.Event<T1, T2, T3, T4> gameEvent, T1 param1, T2 param2, T3 param3, T4 param4)
	{
		GameEventAgent<T1, T2, T3, T4>.Instance.Invoke(gameEvent.eventName, param1, param2, param3, param4);
	}
	public static void AddListener<T1, T2, T3, T4>(GameEvents.Event<T1, T2, T3, T4> gameEvent, System.Action<T1, T2, T3, T4> action)
	{
		GameEventAgent<T1, T2, T3, T4>.Instance.AddListener(gameEvent.eventName, action);
	}
	public static void RemoveListener<T1, T2, T3, T4>(GameEvents.Event<T1, T2, T3, T4> gameEvent, System.Action<T1, T2, T3, T4> action)
	{
		GameEventAgent<T1, T2, T3, T4>.Instance.RemoveListener(gameEvent.eventName, action);
	}
	public static void RemoveListener<T1, T2, T3, T4>(GameEvents.Event<T1, T2, T3, T4> gameEvent)
	{
		GameEventAgent<T1, T2, T3, T4>.Instance.RemoveListener(gameEvent.eventName);
	}
	public static bool CheckHaveListen<T1, T2, T3, T4>(GameEvents.Event<T1, T2, T3, T4> gameEvent)
	{
		return GameEventAgent<T1, T2, T3, T4>.Instance.CheckHaveListen(gameEvent.eventName);
	}
	public static void RemoveListener(object listener)
	{
		if (_instance == null)
			return;
		List<IGameEventAgent>.Enumerator tor = _instance.gameEventAgentList.GetEnumerator();
		while (tor.MoveNext())
		{
			tor.Current.RemoveListener(listener);
		}
	}
	private GameEventCenter()
	{

	}
	private List<IGameEventAgent> gameEventAgentList = new List<IGameEventAgent>(30);   //这个数值要根据项目中的事件数量调整

}
