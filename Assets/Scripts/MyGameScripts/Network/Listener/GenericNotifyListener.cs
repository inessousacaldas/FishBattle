using System;
using System.Collections.Generic;
using UniRx;

public static class GenericNotifyListener
{
    private static Dictionary<Type, MessageProcessor> _dictionary = new Dictionary<Type, MessageProcessor>();

    public static GenericNotifyListener<T> Get<T>() where T : class
    {
        if (!_dictionary.ContainsKey(typeof(T)) || _dictionary[typeof(T)] == null)
        {
            var listener = new GenericNotifyListener<T>();
            listener.InitStream(typeof(T));
            _dictionary.Add(typeof(T), listener);
        }

        return _dictionary[typeof(T)] as GenericNotifyListener<T>;
    }

    public static void RemoveAllListener(Action<MessageProcessor> callback)
    {
        if (_dictionary == null || _dictionary.Keys == null || callback == null) return;
        var keys = _dictionary.Keys.ToArray();
        keys.ForEach<Type>(k =>
        {
            callback(_dictionary[k]);
        });
        
        _dictionary.Clear();
    }

    public static void Remove<T>() where T : class
    {
        _dictionary.Remove(typeof(T));
    }
}

public class GenericNotifyListener<T> : BaseDtoListener<T> where T : class
{
    protected Subject<T> _stream = null;

    public IObservableExpand<T> Stream
    {
        get {
            return _stream; 
        }
    }

    protected override void OnDispose()
    {
        GameLog.LOG_Notify(string.Format("OnDispose------------------{0}",_stream.GetType()));
        _stream = _stream.CloseOnceNull();
        
        GenericNotifyListener.Remove<T>();
    }

    protected override void HandleNotify(T notify)
    {
        if (notify == null)
        {
            GameDebuger.LogWarning("notify is null notify type :" + notify.GetType().ToString());
            return;
        }

        if (_stream == null)
            _stream = new Subject<T>();
        GameLog.LOG_Notify(string.Format("HandleNotify------------------{0}",_stream.GetType()));
        _stream.OnNext(notify);
    }

    public void InitStream(Type T)
    {
        if (_stream == null)
        {
            _stream = new Subject<T>();
            _stream.Hold(null);
        }
    }
}
