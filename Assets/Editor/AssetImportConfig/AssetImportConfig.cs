
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LITJson;
using UnityEditor;
using UnityEngine;

namespace AssetImport
{
    /// <summary>
    /// 与类名匹配
    /// </summary>
    public enum AssetType
    {
        None,
        AtlasOrFont,
        Animation,
        Audio,
        MeshImprot,
        Texture,
    }
    public class AssetImportConfig
    {
        public const string config = "Assets/Editor/AssetImportConfig/ConfigFile/assetImportConfig.txt";
        private Dictionary<AssetType, Dictionary<string, object>> data;

        public static AssetImportConfig LoadConfig()
        {
            if (File.Exists(config))
            {
                JsonData rawData = JsonMapper.ToObject(File.ReadAllText(config));
                return new AssetImportConfig(rawData);
            }
            else
            {
                return new AssetImportConfig(new JsonData());
            }
        }
        public AssetImportConfig(JsonData rawData)
        {
            data = new Dictionary<AssetType, Dictionary<string, object>>();
            AssetType[] enumArray = (AssetType[])Enum.GetValues(typeof(AssetType));
            foreach (AssetType assetType in enumArray)
            {
                if(assetType == AssetType.None)
                    continue;
                string enumName = assetType.ToString();
                Type type = Type.GetType(assetType.GetType().Namespace + "." + enumName + AssetItemConfigBase.suffix);
                if (type == null)
                {
                    Debug.LogError("Can Not Find Type : " + assetType.GetType().Namespace + "." + enumName + AssetItemConfigBase.suffix);
                    continue;
                }
                Dictionary<string, object> dic = null;

                if (((IDictionary) rawData).Contains(enumName))
                {
                    JsonData jsonData = rawData[enumName];
                    dic = new Dictionary<string, object>();
                    foreach (string key in jsonData.Keys)
                    {
                        dic[key] = JsonMapper.ToObject(jsonData[key].ToJson(), type);
                    }
                }
                data[assetType] = dic ?? new Dictionary<string, object>();
            }

        }
        public void SaveConfig()
        {
            File.WriteAllText(config, ToJson());
        }
        public string ToJson()
        {
            JsonData root = new JsonData();
            foreach (KeyValuePair<AssetType, Dictionary<string, object>> keyValuePair in data)
            {
                if(keyValuePair.Value.Count == 0)
                    continue;
                JsonData assetRoot = new JsonData();
                AssetType assetType = keyValuePair.Key;
                root[assetType.ToString()] = assetRoot;
                foreach (KeyValuePair<string, object> assetsPair in keyValuePair.Value)
                {
                    Type type = Type.GetType(assetType.GetType().Namespace + "." + assetType + "Config");
                    JsonData assetData = JsonMapper.ToObject(JsonMapper.ToJson(assetsPair.Value));
                    assetRoot[assetsPair.Key] = assetData;
                }
            }
            return JsonMapper.ToJson(root, true);
        }
        public AssetConfigMap<T> GetAtlasConfig<T>() where T:AssetItemConfigBase
        {
            Type type = typeof (T);
            AssetType[] enumArray = (AssetType[])Enum.GetValues(typeof(AssetType));
            AssetType assetType = enumArray.Find(item => type.Name.Contains(item.ToString()));

            return new AssetConfigMap<T>(data[assetType]);
        }

        public Dictionary<string, object> GetAtlasConfig(AssetType assetType)
        {
            return data[assetType];
        }   
        
    }

    public abstract class AssetItemConfigBase
    {
        public const string suffix = "Config";
    }

    public abstract class AssetImportHelperBase
    {
        public const string suffix = "Helper";

        public abstract IEnumerable<string> GetAllAsset();
        public abstract AssetItemConfigBase GetAssetConfig(AssetImportConfig assetImportConfig, string assetPath, out bool isDefault);
        public abstract AssetItemConfigBase GetDefaultConfig(string assetPath);
        public abstract void DrawAssetConfigGUI(AssetItemConfigBase assetItemConfigBase);
        public abstract bool IsMatch(AssetImporter assetImporter);
        public abstract void SetImporterByConfig(AssetImporter assetImporter, AssetItemConfigBase config);
        protected static bool TrySetField(string field, object source, object target)
        {

            RefInfo sourceInfo = new RefInfo(source.GetType(), field);
            RefInfo setObjInfo = new RefInfo(target.GetType(), field);
            object sourceValue = sourceInfo.GetValue(source);
            object setValue = setObjInfo.GetValue(target);
            bool compare = sourceValue.Equals(setValue);
            if (!compare)
            {
                setObjInfo.SetValue(target, sourceValue);
                return true;
            }
            return false;
        }
        public static Dictionary<AssetType, AssetImportHelperBase> GetAllAssetImportHelper()
        {
            Dictionary<AssetType, AssetImportHelperBase> helpers = new Dictionary<AssetType, AssetImportHelperBase>();
            AssetType[] enumArray = (AssetType[])Enum.GetValues(typeof(AssetType));
            foreach (AssetType assetType in enumArray)
            {
                if (assetType == AssetType.None)
                    continue;
                string enumName = assetType.ToString();
                Type type = Type.GetType(assetType.GetType().Namespace + "." + enumName + AssetItemConfigBase.suffix);
                if (type == null)
                {
                    Debug.LogError("Can Not Find Type : " + assetType.GetType().Namespace + "." + enumName + AssetItemConfigBase.suffix);
                    continue;
                }
                AssetImportHelperBase helper = (AssetImportHelperBase)Assembly.GetExecutingAssembly().CreateInstance(assetType.GetType().Namespace + "." + assetType + AssetImportHelperBase.suffix);
                helpers.Add(assetType, helper);
            }
            return helpers;
        }
    }

    internal class RefInfo
    {
        const BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty;

        private FieldInfo fieldInfo;
        private PropertyInfo propertyInfo;
        public RefInfo(Type type, string field)
        {
            fieldInfo = type.GetField(field);
            if (fieldInfo == null)
            {
                propertyInfo = type.GetProperty(field);
                if(propertyInfo == null)
                    throw new ArgumentException(string.Format("Field {0} Can Not Find By Type:{1}", field, type.FullName));
            }

        }

        public object GetValue(object obj)
        {
            if (fieldInfo != null)
                return fieldInfo.GetValue(obj);

            if (propertyInfo != null)
                return propertyInfo.GetValue(obj, null);
            return null;
        }

        public void SetValue(object obj, object value)
        {
            if (fieldInfo != null)
                fieldInfo.SetValue(obj, value);
            if (propertyInfo != null)
                propertyInfo.SetValue(obj, value, null);
        }
    }

    public class AssetConfigMap<T> : IDictionary<string, T>
        where T : AssetItemConfigBase
    {
        private Dictionary<string, object> rawDictionary;
        public AssetConfigMap(Dictionary<string, object> rawDictionary)
        {
            this.rawDictionary = rawDictionary;
        }

        public T this[string key]
        {
            get { return (T)rawDictionary[key]; }

            set { rawDictionary[key] = value; }
        }

        public int Count
        {
            get { return rawDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((IDictionary<string, object>)rawDictionary).IsReadOnly; }
        }

        public ICollection<string> Keys
        {
            get { return rawDictionary.Keys; }
        }

        public ICollection<T> Values
        {
            get
            {
                List<T> list = new List<T>();
                foreach (KeyValuePair<string, object> keyValuePair in rawDictionary)
                {
                    list.Add((T)keyValuePair.Value);
                }
                return list;
            }
        }

        public void Add(KeyValuePair<string, T> item)
        {
            rawDictionary.Add(item.Key, item.Value);
        }

        public void Add(string key, T value)
        {
            rawDictionary.Add(key, value);

        }

        public void Clear()
        {
            rawDictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            return rawDictionary.ContainsKey(item.Key) && rawDictionary.ContainsValue(item.Value);
        }

        public bool ContainsKey(string key)
        {
            return rawDictionary.ContainsKey(key);
        }

        void ICollection<KeyValuePair<string, T>>.CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            foreach (KeyValuePair<string, object> keyValuePair in rawDictionary)
            {
                yield return new KeyValuePair<string, T>(keyValuePair.Key, (T)keyValuePair.Value);
            }
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            if (Contains(item))
            {
                rawDictionary.Remove(item.Key);
                return true;
            }
            return false;
        }

        public bool Remove(string key)
        {
            if (ContainsKey(key))
            {
                rawDictionary.Remove(key);
                return true;
            }
            return false;
        }

        public bool TryGetValue(string key, out T value)
        {
            object rawValue = null;
            value = null;
            if (rawDictionary.TryGetValue(key, out rawValue))
            {
                value = (T)rawValue;
                return true;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return rawDictionary.GetEnumerator();
        }
    }
}

