using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AssetPipeline;
using UnityEditor;

//[InitializeOnLoad]
public class IconConfigTool : AssetPostprocessor
{
    public const string IconRootPath = "Assets/UI/Atlas/Icons";
    public const string AtlasConfigPath = "Assets/GameResources/ConfigFiles/IconConfig/IconAtlasConfig.txt";

    private static ResConfig _resConfig;
    private static DateTime _lastReadConfigDataTime;
    private static List<ResInfo> _prefabList;

    static IconConfigTool()
    {
        UIAtlasMaker.UpdateAtlasCallBack += GenearteConfig;
    }

    public static void GenearteConfig(UIAtlas atlas)
    {
        var filePath = AssetDatabase.GetAssetPath(atlas.gameObject);
        if (!filePath.Contains(IconRootPath))
            return;

        //在过滤列表中的图集不处理
        if (IconConfigManager.IgnoreAtlasPrefixs.Any(filter => atlas.name.Contains(filter)))
            return;

        //初始化IconAtlasMap信息
        IconAtlasMap iconAtlasMap = null;
        if (File.Exists(AtlasConfigPath))
        {
            iconAtlasMap = FileHelper.ReadJsonFile<IconAtlasMap>(AtlasConfigPath);
        }
        else
        {
            iconAtlasMap = new IconAtlasMap();
        }

        //构建新版本图集配置信息
        var newIconAtlasConfig = new IconAtlasConfig();
        newIconAtlasConfig.Prefix = Regex.Replace(atlas.name, @"_\d+", "");
        newIconAtlasConfig.Name = atlas.name;
        foreach (var spriteData in atlas.spriteList)
        {
            newIconAtlasConfig.IconList.Add(spriteData.name);
        }

        //填充回IconAtlasMap中,保存json文件
        iconAtlasMap.map[newIconAtlasConfig.Name] = newIconAtlasConfig;
        FileHelper.SaveJsonObj(iconAtlasMap, AtlasConfigPath);
        AssetDatabase.Refresh();
    }
}

