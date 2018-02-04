using System;

public static class GameEvents
{

	#region 测试用例

	public static readonly GameEvents.Event<string> OnTestEvent = new GameEvents.Event<string>("OnTestEvent");
	public static readonly GameEvents.Event<TestClass1> OnTestEvent2 = new GameEvents.Event<TestClass1>("OnTestEvent2");

	#endregion

    public class Event
    {
        public readonly string eventName;
        public Event(string _eventName)
        {
            eventName = _eventName;
        }
    }
    public class Event<T>
    {
        public readonly string eventName;
        public Event(string _eventName)
        {
            eventName = _eventName;
        }
    }
    public class Event<T1, T2>
    {
        public readonly string eventName;
        public Event(string _eventName)
        {
            eventName = _eventName;
        }
    }
    public class Event<T1, T2, T3>
    {
        public readonly string eventName;
        public Event(string _eventName)
        {
            eventName = _eventName;
        }
    }
    public class Event<T1, T2, T3, T4>
    {
        public readonly string eventName;
        public Event(string _eventName)
        {
            eventName = _eventName;
        }
    }
}
