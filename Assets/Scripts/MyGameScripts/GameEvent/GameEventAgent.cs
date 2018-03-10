using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEventAgent : IGameEventAgent
{
    private static GameEventAgent _instance;
    public static GameEventAgent Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameEventAgent();
                GameEventCenter.Instance.AddGameEventAgent(_instance);
            }
            return _instance;
        }
    }
    private class GameEventDelegate
    {
        System.Action _action;
        bool needUpdate;
        private Delegate[] delegateList;

        public void Add(System.Action action)
        {
            _action -= action;   //防止重复监听
            _action += action;
            needUpdate = true;
        }
        public void Remove(System.Action action)
        {
            _action -= action;
            needUpdate = true;
        }
        public void Remove(object listener)
        {
            CheckUpdate();
            if (delegateList == null)
                return;
            for (int i = 0; i < delegateList.Length; i++)
            {
                Delegate _delegate = delegateList[i];
                if (_delegate.Target == listener)
                {
                    Remove(_delegate as System.Action);
                }
            }
        }
        public void Invoke()
        {
            if (_action == null)
                return;
            CheckUpdate();
            if (delegateList == null)
                return;
            for (int i = 0; i < delegateList.Length; i++)
            {
                try
                {
                    Action _delegate = delegateList[i] as System.Action;
                    _delegate();
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        public Delegate[] GetInvokeList()
        {
            CheckUpdate();
            return delegateList;
        }
        private void CheckUpdate()
        {
            if (needUpdate)
            {
                needUpdate = false;
                if (_action != null)
                    delegateList = _action.GetInvocationList();
                else
                    delegateList = null;
            }
        }
    }

    Dictionary<string, GameEventDelegate> eventList = new Dictionary<string, GameEventDelegate>();
    private GameEventAgent()
    {

    }
    public void AddListener(string gameEvent, System.Action action)
    {
        GameEventDelegate gameEventDelegate;
        if (eventList.TryGetValue(gameEvent, out gameEventDelegate) == false)
        {
            gameEventDelegate = new GameEventDelegate();
            eventList.Add(gameEvent, gameEventDelegate);
        }
        gameEventDelegate.Add(action);
    }
    public void Invoke(string gameEvent)
    {
        GameEventDelegate gameEventDelegate;
        if (eventList.TryGetValue(gameEvent, out gameEventDelegate))
        {
            gameEventDelegate.Invoke();
        }
    }
    public void RemoveListener(string gameEvent, System.Action action)
    {
        GameEventDelegate gameEventDelegate;
        if (eventList.TryGetValue(gameEvent, out gameEventDelegate))
        {
            gameEventDelegate.Remove(action);
        }
    }
    
    public bool CheckHaveListen(string gameEvent)
    {
        GameEventDelegate eventDelegate;
        if(eventList.TryGetValue(gameEvent, out eventDelegate))
        {
            Delegate[] delegateList = eventDelegate.GetInvokeList();
            if (delegateList != null && delegateList.Length > 0)
                return true;
        }
        return false;
    }
    public void RemoveListener(object listener)
    {
        Dictionary<string, GameEventDelegate>.Enumerator tor = eventList.GetEnumerator();
        while (tor.MoveNext())
        {
            tor.Current.Value.Remove(listener);
        }
    }
}

public class GameEventAgent<T> : IGameEventAgent
{
    private static GameEventAgent<T> _instance;
    public static GameEventAgent<T> Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameEventAgent<T>();
                GameEventCenter.Instance.AddGameEventAgent(_instance);
            }
            return _instance;
        }
    }
    private class GameEventDelegate
    {
        System.Action<T> _action;
        bool needUpdate;
        private Delegate[] delegateList;

        public void Add(System.Action<T> action)
        {
            _action -= action;   //防止重复监听
            _action += action;
            needUpdate = true;
        }
        public void Remove(System.Action<T> action)
        {
            _action -= action;
            needUpdate = true;
        }
        public void Remove(object listener)
        {
            CheckUpdate();
            if (delegateList == null)
                return;
            for (int i = 0; i < delegateList.Length; i++)
            {
                Delegate _delegate = delegateList[i];
                if(_delegate.Target == listener)
                {
                    Remove(_delegate as System.Action<T>);
                }
            }
        }
        public void Invoke(T param)
        {
            if (_action == null)
                return;
            CheckUpdate();
            if (delegateList == null)
                return;
            for (int i = 0; i < delegateList.Length; i++)
            {
                try
                {
                    Action<T> _delegate = delegateList[i] as System.Action<T>;
                    _delegate(param);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        public Delegate[] GetInvokeList()
        {
            CheckUpdate();
            return delegateList;
        }
        private void CheckUpdate()
        {
            if (needUpdate)
            {
                needUpdate = false;
                if (_action != null)
                    delegateList = _action.GetInvocationList();
                else
                    delegateList = null;
            }
        }
    }

    Dictionary<string, GameEventDelegate> eventList = new Dictionary<string, GameEventDelegate>();
    private GameEventAgent()
    {

    }
    public void AddListener(string gameEvent, System.Action<T> action)
    {
        GameEventDelegate gameEventDelegate;
        if (eventList.TryGetValue(gameEvent, out gameEventDelegate) == false)
        {
            gameEventDelegate = new GameEventDelegate();
            eventList.Add(gameEvent, gameEventDelegate);
        }
        gameEventDelegate.Add(action);
    }
    public void Invoke(string gameEvent, T param)
    {
        GameEventDelegate gameEventDelegate;
        if (eventList.TryGetValue(gameEvent, out gameEventDelegate))
        {
            gameEventDelegate.Invoke(param);
        }
    }
    public void RemoveListener(string gameEvent, System.Action<T> action)
    {
        GameEventDelegate gameEventDelegate;
        if (eventList.TryGetValue(gameEvent, out gameEventDelegate))
        {
            gameEventDelegate.Remove(action);
        }
    }
    public void RemoveListener(string gameEvent)
    {
        if (CheckHaveListen(gameEvent))
        {
            eventList.Remove(gameEvent);
        }
    }
    public bool CheckHaveListen(string gameEvent)
    {
        GameEventDelegate eventDelegate;
        if (eventList.TryGetValue(gameEvent, out eventDelegate))
        {
            Delegate[] delegateList = eventDelegate.GetInvokeList();
            if (delegateList != null && delegateList.Length > 0)
                return true;
        }
        return false;
    }
    public void RemoveListener(object listener)
    {
        Dictionary<string, GameEventDelegate>.Enumerator tor = eventList.GetEnumerator();
        while(tor.MoveNext())
        {
            tor.Current.Value.Remove(listener);
        }
    }
}

public class GameEventAgent<T1, T2> : IGameEventAgent
{
    private static GameEventAgent<T1, T2> _instance;
    public static GameEventAgent<T1, T2> Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameEventAgent<T1, T2>();
                GameEventCenter.Instance.AddGameEventAgent(_instance);
            }
            return _instance;
        }
    }
    private class GameEventDelegate
    {
        System.Action<T1, T2> _action;
        bool needUpdate;
        private Delegate[] delegateList;

        public void Add(System.Action<T1, T2> action)
        {
            _action -= action;   //防止重复监听
            _action += action;
            needUpdate = true;
        }
        public void Remove(System.Action<T1, T2> action)
        {
            _action -= action;
            needUpdate = true;
        }
        public void Remove(object listener)
        {
            CheckUpdate();
            if (delegateList == null)
                return;
            for (int i = 0; i < delegateList.Length; i++)
            {
                Delegate _delegate = delegateList[i];
                if (_delegate.Target == listener)
                {
                    Remove(_delegate as System.Action<T1, T2>);
                }
            }
        }
        public void Invoke(T1 param1, T2 param2)
        {
            if (_action == null)
                return;
            CheckUpdate();
            if (delegateList == null)
                return;
            for (int i = 0; i < delegateList.Length; i++)
            {
                try
                {
                    Action<T1, T2> _delegate = delegateList[i] as System.Action<T1, T2>;
                    _delegate(param1, param2);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        public Delegate[] GetInvokeList()
        {
            CheckUpdate();
            return delegateList;
        }
        private void CheckUpdate()
        {
            if (needUpdate)
            {
                needUpdate = false;
                if (_action != null)
                    delegateList = _action.GetInvocationList();
                else
                    delegateList = null;
            }
        }
    }

    Dictionary<string, GameEventDelegate> eventList = new Dictionary<string, GameEventDelegate>();
    private GameEventAgent()
    {

    }
    public void AddListener(string gameEvent, System.Action<T1, T2> action)
    {
        GameEventDelegate gameEventDelegate;
        if (eventList.TryGetValue(gameEvent, out gameEventDelegate) == false)
        {
            gameEventDelegate = new GameEventDelegate();
            eventList.Add(gameEvent, gameEventDelegate);
        }
        gameEventDelegate.Add(action);
    }
    public void Invoke(string gameEvent, T1 param1, T2 param2)
    {
        GameEventDelegate gameEventDelegate;
        if (eventList.TryGetValue(gameEvent, out gameEventDelegate))
        {
            gameEventDelegate.Invoke(param1, param2);
        }
    }
    public void RemoveListener(string gameEvent, System.Action<T1, T2> action)
    {
        GameEventDelegate gameEventDelegate;
        if (eventList.TryGetValue(gameEvent, out gameEventDelegate))
        {
            gameEventDelegate.Remove(action);
        }
    }
    public void RemoveListener(string gameEvent)
    {
        if (CheckHaveListen(gameEvent))
        {
            eventList.Remove(gameEvent);
        }
    }
    public bool CheckHaveListen(string gameEvent)
    {
        GameEventDelegate eventDelegate;
        if (eventList.TryGetValue(gameEvent, out eventDelegate))
        {
            Delegate[] delegateList = eventDelegate.GetInvokeList();
            if (delegateList != null && delegateList.Length > 0)
                return true;
        }
        return false;
    }
    public void RemoveListener(object listener)
    {
        Dictionary<string, GameEventDelegate>.Enumerator tor = eventList.GetEnumerator();
        while (tor.MoveNext())
        {
            tor.Current.Value.Remove(listener);
        }
    }
}

public class GameEventAgent<T1, T2, T3> : IGameEventAgent
{
    private static GameEventAgent<T1, T2, T3> _instance;
    public static GameEventAgent<T1, T2, T3> Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameEventAgent<T1, T2, T3>();
                GameEventCenter.Instance.AddGameEventAgent(_instance);
            }
            return _instance;
        }
    }
    private class GameEventDelegate
    {
        System.Action<T1, T2, T3> _action;
        bool needUpdate;
        private Delegate[] delegateList;

        public void Add(System.Action<T1, T2, T3> action)
        {
            _action -= action;   //防止重复监听
            _action += action;
            needUpdate = true;
        }
        public void Remove(System.Action<T1, T2, T3> action)
        {
            _action -= action;
            needUpdate = true;
        }
        public void Remove(object listener)
        {
            CheckUpdate();
            if (delegateList == null)
                return;
            for (int i = 0; i < delegateList.Length; i++)
            {
                Delegate _delegate = delegateList[i];
                if (_delegate.Target == listener)
                {
                    Remove(_delegate as System.Action<T1, T2, T3>);
                }
            }
        }
        public void Invoke(T1 param1, T2 param2, T3 param3)
        {
            if (_action == null)
                return;
            CheckUpdate();
            if (delegateList == null)
                return;
            for (int i = 0; i < delegateList.Length; i++)
            {
                try
                {
                    Action<T1, T2, T3> _delegate = delegateList[i] as System.Action<T1, T2, T3>;
                    _delegate(param1, param2, param3);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        public Delegate[] GetInvokeList()
        {
            CheckUpdate();
            return delegateList;
        }
        private void CheckUpdate()
        {
            if (needUpdate)
            {
                needUpdate = false;
                if (_action != null)
                    delegateList = _action.GetInvocationList();
                else
                    delegateList = null;
            }
        }
    }

    Dictionary<string, GameEventDelegate> eventList = new Dictionary<string, GameEventDelegate>();
    private GameEventAgent()
    {

    }
    public void AddListener(string gameEvent, System.Action<T1, T2, T3> action)
    {
        GameEventDelegate gameEventDelegate;
        if (eventList.TryGetValue(gameEvent, out gameEventDelegate) == false)
        {
            gameEventDelegate = new GameEventDelegate();
            eventList.Add(gameEvent, gameEventDelegate);
        }
        gameEventDelegate.Add(action);
    }
    public void Invoke(string gameEvent, T1 param1, T2 param2, T3 param3)
    {
        GameEventDelegate gameEventDelegate;
        if (eventList.TryGetValue(gameEvent, out gameEventDelegate))
        {
            gameEventDelegate.Invoke(param1, param2, param3);
        }
    }
    public void RemoveListener(string gameEvent, System.Action<T1, T2, T3> action)
    {
        GameEventDelegate gameEventDelegate;
        if (eventList.TryGetValue(gameEvent, out gameEventDelegate))
        {
            gameEventDelegate.Remove(action);
        }
    }
    public void RemoveListener(string gameEvent)
    {
        if (CheckHaveListen(gameEvent))
        {
            eventList.Remove(gameEvent);
        }
    }
    public bool CheckHaveListen(string gameEvent)
    {
        GameEventDelegate eventDelegate;
        if (eventList.TryGetValue(gameEvent, out eventDelegate))
        {
            Delegate[] delegateList = eventDelegate.GetInvokeList();
            if (delegateList != null && delegateList.Length > 0)
                return true;
        }
        return false;
    }
    public void RemoveListener(object listener)
    {
        Dictionary<string, GameEventDelegate>.Enumerator tor = eventList.GetEnumerator();
        while (tor.MoveNext())
        {
            tor.Current.Value.Remove(listener);
        }
    }
}

public class GameEventAgent<T1, T2, T3, T4> : IGameEventAgent
{
    private static GameEventAgent<T1, T2, T3, T4> _instance;
    public static GameEventAgent<T1, T2, T3, T4> Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameEventAgent<T1, T2, T3, T4>();
                GameEventCenter.Instance.AddGameEventAgent(_instance);
            }
            return _instance;
        }
    }
    private class GameEventDelegate
    {
        System.Action<T1, T2, T3, T4> _action;
        bool needUpdate;
        private Delegate[] delegateList;

        public void Add(System.Action<T1, T2, T3, T4> action)
        {
            _action -= action;   //防止重复监听
            _action += action;
            needUpdate = true;
        }
        public void Remove(System.Action<T1, T2, T3, T4> action)
        {
            _action -= action;
            needUpdate = true;
        }
        public void Remove(object listener)
        {
            CheckUpdate();
            if (delegateList == null)
                return;
            for (int i = 0; i < delegateList.Length; i++)
            {
                Delegate _delegate = delegateList[i];
                if (_delegate.Target == listener)
                {
                    Remove(_delegate as System.Action<T1, T2, T3, T4>);
                }
            }
        }
        public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            if (_action == null)
                return;
            CheckUpdate();
            if (delegateList == null)
                return;
            for (int i = 0; i < delegateList.Length; i++)
            {
                try
                {
                    Action<T1, T2, T3, T4> _delegate = delegateList[i] as System.Action<T1, T2, T3, T4>;
                    _delegate(param1, param2, param3, param4);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        public Delegate[] GetInvokeList()
        {
            CheckUpdate();
            return delegateList;
        }
        private void CheckUpdate()
        {
            if (needUpdate)
            {
                needUpdate = false;
                if (_action != null)
                    delegateList = _action.GetInvocationList();
                else
                    delegateList = null;
            }
        }
    }

    Dictionary<string, GameEventDelegate> eventList = new Dictionary<string, GameEventDelegate>();
    private GameEventAgent()
    {

    }
    public void AddListener(string gameEvent, System.Action<T1, T2, T3, T4> action)
    {
        GameEventDelegate gameEventDelegate;
        if (eventList.TryGetValue(gameEvent, out gameEventDelegate) == false)
        {
            gameEventDelegate = new GameEventDelegate();
            eventList.Add(gameEvent, gameEventDelegate);
        }
        gameEventDelegate.Add(action);
    }
    public void Invoke(string gameEvent, T1 param1, T2 param2, T3 param3, T4 param4)
    {
        GameEventDelegate gameEventDelegate;
        if (eventList.TryGetValue(gameEvent, out gameEventDelegate))
        {
            gameEventDelegate.Invoke(param1, param2, param3, param4);
        }
    }
    public void RemoveListener(string gameEvent, System.Action<T1, T2, T3, T4> action)
    {
        GameEventDelegate gameEventDelegate;
        if (eventList.TryGetValue(gameEvent, out gameEventDelegate))
        {
            gameEventDelegate.Remove(action);
        }
    }
    public void RemoveListener(string gameEvent)
    {
        if (CheckHaveListen(gameEvent))
        {
            eventList.Remove(gameEvent);
        }
    }
    public bool CheckHaveListen(string gameEvent)
    {
        GameEventDelegate eventDelegate;
        if (eventList.TryGetValue(gameEvent, out eventDelegate))
        {
            Delegate[] delegateList = eventDelegate.GetInvokeList();
            if (delegateList != null && delegateList.Length > 0)
                return true;
        }
        return false;
    }
    public void RemoveListener(object listener)
    {
        Dictionary<string, GameEventDelegate>.Enumerator tor = eventList.GetEnumerator();
        while (tor.MoveNext())
        {
            tor.Current.Value.Remove(listener);
        }
    }
}
