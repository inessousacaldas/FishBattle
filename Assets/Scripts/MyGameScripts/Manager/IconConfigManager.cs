using System;
using System.Collections.Generic;
using UnityEngine;

public static class IconConfigManager
{
    /// <summary>
    ///     [AtlasPrefix_SpriteName,AtlasName],游戏中通过AtlasPrefix和SpriteName来获取得相应图集的prefabName
    /// </summary>
    private static Dictionary<string, string> _atlasDic;

    public static readonly List<string> IgnoreAtlasPrefixs = new List<string>
    {
        "OtherIconAtlas",
        "CommonUIAltas",
        "QuartzAtlas"
    };

    public static void Setup(Action onFinish)
    {
        _atlasDic = new Dictionary<string, string>();

        AssetPipeline.ResourcePoolManager.Instance.LoadConfig("IconAtlasConfig", asset =>
        {
            var textAsset = asset as TextAsset;
            if (textAsset == null)
            {
                Debug.LogError("Load AtlasConfig failed");
                if (onFinish != null)
                    onFinish();
                return;
            }

            var iconAtlasMap = JsHelper.ToObject<IconAtlasMap>(textAsset.text);
            if (iconAtlasMap == null || iconAtlasMap.map.Count == 0)
            {
                Debug.LogError("IconAtlasMap is null");
                if (onFinish != null)
                    onFinish();
                return;
            }

            foreach (var item in iconAtlasMap.map)
            {
                var atlasConfig = item.Value;
                for (int i = 0; i < atlasConfig.IconList.Count; i++)
                {
                    string key = atlasConfig.Prefix + "_" + atlasConfig.IconList[i];
                    try
                    {
                        _atlasDic.Add(key, atlasConfig.Name);
                    }
                    catch (Exception)
                    {
                        GameDebuger.LogError("IconAtlas 图片名字重复: " + key);
                    }
                }
            }
            if (onFinish != null)
                onFinish();
        });
    }

    /// <summary>
    ///     获取想要得到的图集名称
    /// </summary>
    /// <param name="spriteName"></param>
    /// <param name="atlasPrefix"></param>
    /// <returns></returns>
    public static string GetAltasName(string spriteName, string atlasPrefix)
    {
        if (String.IsNullOrEmpty(spriteName) || String.IsNullOrEmpty(atlasPrefix))
        {
            Debug.LogError(String.Format("图集前缀：{0}， 或者图片：{1}为空", atlasPrefix, spriteName));
            return atlasPrefix;
        }

        if (IgnoreAtlasPrefixs.Contains(atlasPrefix))
            return atlasPrefix;

        string key = atlasPrefix + "_" + spriteName;
        if (_atlasDic.ContainsKey(key))
        {
            return _atlasDic[key];
        }

        Debug.LogError(String.Format("存在图集前缀：{0}，但是不存在图片：{1}", atlasPrefix, spriteName));
        return atlasPrefix;
    }
}

public class IconAtlasMap
{
    public Dictionary<string, IconAtlasConfig> map = new Dictionary<string, IconAtlasConfig>();
}

public class IconAtlasConfig
{
    public List<string> IconList = new List<string>(); //图集Sprite列表
    public string Name; //图集prefab名
    public string Prefix; //图集前缀(未拆分前的prefab名)
}