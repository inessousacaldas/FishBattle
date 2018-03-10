
using System;
using System.Collections.Generic;
using AppDto;
using UnityEngine;

/// <summary>
///     静态数据缓存
/// </summary>
public class DataCache
{
    /// <summary>
    ///     静态数据二进制数据,等待需要读取时才转换,转换完从这个字典中移除
    /// </summary>
    private static Dictionary<Type, ByteArray> _rawBytesDic;

    /// <summary>
    ///     从ByteArray转换为<id, Collection>的结构,方便业务代码读取
    /// </summary>
    private static Dictionary<Type, object> _staticDataMap;

    public static void Setup(Dictionary<Type, ByteArray> staticDataBytes)
    {
        Debug.Log("Start DataCache Setup");

        _rawBytesDic = staticDataBytes;

        if (_staticDataMap == null)
        {
            _staticDataMap = new Dictionary<Type, object>();
        }
    }

    public static T getDtoByCls<T>(int key)
    {
        if (_staticDataMap == null)
            return default(T);

        var type = typeof(T);
        var result = CachedToMap<T, DataCollection<T>>(type);
        if (result == null)
        {
            Debug.LogError(string.Format("Data not find!!  type = {0}, key = {1}" , type.ToString(), key));
        }
        else if (result.map.ContainsKey(key))
        {
            return result.map[key];
        }
        return default(T);
    }

    public static Dictionary<int, T> getDicByCls<T>()
    {
        if (_staticDataMap == null)
            return null;

        var type = typeof(T);
        var result = CachedToMap<T, DataCollection<T>>(type);
        return result != null ? result.map : null;
    }

    public static List<T> getArrayByCls<T>()
    {
        if (_staticDataMap == null)
        {
            GameDebuger.LogError("DataCache.getArrayByCls<T>() failed !, _staticDataMap is null");
            return null;
        }

        var type = typeof(T);
        var result = CachedToMap<T, DataCollection<T>>(type);
        if (result == null)
        {
            GameDebuger.LogError(string.Format("DataCache.getArrayByCls<{0}>() failed !", typeof(T).ToString()));
        }
        return result != null ? result.list : null;
    }

    public static List<T> getSimpleList<T>(Type type)
    {
        if (_staticDataMap == null)
            return null;

        var result = CachedToMap<T, List<T>>(type, true);
        return result;
    }

    private static TResult CachedToMap<T, TResult>(Type type, bool simpleDataType = false)
    {
        ByteArray ba;
        TResult resultData;
        if (_rawBytesDic.TryGetValue(type, out ba))
        {
            if (ba != null && ba.Length > 0)
            {
#if USE_JSZ
                var dataList = JsHelper.ParseJsz<DataList>(ba, true);
#else
                var dataList = JsHelper.ParseProtoObj(ba, true) as DataList;
#endif
                if (dataList != null)
                {

                    if (simpleDataType)
                    {
                        var list = new List<T>(dataList.items.Count);
                        for (int i = 0, count = dataList.items.Count; i < count; i++)
                        {
                            T val = (T)dataList.items[i];
                            list.Add(val);
                        }
                        _staticDataMap[type] = list;
                    }
                    else
                    {
                        var collection = new DataCollection<T>(dataList.items.Count);
                        for (int i = 0, count = dataList.items.Count; i < count; i++)
                        {
                            //通过反射的方式获取静态数据的key
                            T obj = (T)dataList.items[i];
                            int key = JsHelper.GetDataObjectKey(obj);
                            collection.AddMap(key, obj);
                            collection.AddList(obj);
                        }
                        _staticDataMap[type] = collection;
                    }
                }
            }
            _rawBytesDic.Remove(type);
        }
        resultData = (TResult)GetStaticDataMap(type);
        if(resultData == null)
        {
            CheckChildTypeCache(type);
        }
        return resultData;
    }

    private static object GetStaticDataMap(Type type)
    {
        return _staticDataMap.ContainsKey(type) ? _staticDataMap[type] : null;
    }

    private static bool HasMap(Type type)
    {
        return _rawBytesDic.ContainsKey(type) ? true : GetStaticDataMap(type) != null;
    }

    public static void Dispose()
    {
        _rawBytesDic = null;
        _staticDataMap = null;
    }

    #region Nested type: DataCollection

    public class DataCollection<T>
    {
        public List<T> list;
        public Dictionary<int, T> map;

        public DataCollection(int count)
        {
            map = new Dictionary<int, T>(count);
            list = new List<T>(count);
        }

        public void AddMap(int id, T obj)
        {
            if (map.ContainsKey(id))
            {
                Debug.LogError("ths same key:" + id + " to add <" + obj.GetType().Name + ">");
            }
            map[id] = obj;
        }

        public void AddList(T obj)
        {
            list.Add(obj);
        }

        public bool ContainsKey(int key)
        {
            return map.ContainsKey(key);
        }
    }

    #endregion

    #region 获取静态数据配置

    public static int GetStaticConfigValue(int key, int defaultValue = 0)
    {
        var data = getDtoByCls<StaticConfig>(key);
        return data != null && !string.IsNullOrEmpty(data.value) ? StringHelper.ToInt(data.value) : defaultValue;
    }

    public static string GetStaticConfigValues(int key, string defaultValue = "")
    {
        var data = getDtoByCls<StaticConfig>(key);
        return data != null ? data.value : defaultValue;
    }

    public static float GetStaticConfigValuef(int key, float defaultValue = 0.0f)
    {
        var data = getDtoByCls<StaticConfig>(key);
        return data != null ? float.Parse(data.value) : defaultValue;
    }

    public static string GetStaticStringValue(int key, string defaultValue = "")
    {
        var data = getDtoByCls<StaticString>(key);
        return data != null ? data.message : defaultValue;
    }

    #endregion

    private static void CheckChildTypeCache(Type type)
    {
        var i = 0;
        var typeName = type.ToString();
        type = type.BaseType;
        while(type != typeof(object))
        {
            if(i == 10) break;
            if(HasMap(type))
            {
                GameLog.ShowError("{0}读取导表数据失败，应该读取对应的父类{1}",typeName + "," + type.ToString());
                break;
            }else
            {
                type = type.BaseType;
            }
            i++;
        }
        GameLog.ShowError("{0}没有对应的导表数据，请查看是否存在对应的导表文件,或是否提前获取数据了",typeName);
    }
}