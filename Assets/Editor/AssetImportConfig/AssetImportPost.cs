using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace AssetImport
{
    public class AssetImportPost : AssetPostprocessor
    {
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            var helpers = AssetImportHelperBase.GetAllAssetImportHelper();
            var allConfig = AssetImportConfig.LoadConfig();
            foreach (string path in importedAssets)
            {
                var assetType = Match(helpers, path);
                if (assetType != AssetType.None)
                {
                    object config;
                    bool idDefault;
                    config = helpers[assetType].GetAssetConfig(allConfig, path, out idDefault);
                    helpers[assetType].SetImporterByConfig(AssetImporter.GetAtPath(path), (AssetItemConfigBase)config);
                }
            }
            if (deletedAssets.Length == 0 && movedAssets.Length == 0)
                return;
            bool dirty = false;
            //删除资源后 删除配置表对应项目
            foreach (string path in deletedAssets)
            {
                var assetType = Match(helpers, path);
                if (assetType != AssetType.None)
                    dirty = allConfig.GetAtlasConfig(assetType).Remove(Path.GetFileNameWithoutExtension(path));
            }
            //移动资源后 修改配置表对应路径
            for (int i = 0; i < movedAssets.Length; i++)
            {
                var movePath = movedAssets[i];
                var moveFromPath = movedFromAssetPaths[i];
                var assetType = Match(helpers, moveFromPath);
                if (assetType != AssetType.None)
                {
                    object config;
                    Dictionary<string, object> configDic = allConfig.GetAtlasConfig(assetType);
                    if (configDic.TryGetValue(moveFromPath, out config))
                    {
                        dirty = true;
                        configDic.Remove(Path.GetFileNameWithoutExtension(moveFromPath));
                        configDic.Add(movePath, config);
                    }
                }
            }
            if(dirty)
                allConfig.SaveConfig();
        }

        public static AssetType Match(Dictionary<AssetType, AssetImportHelperBase> helpers, string path)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            if(assetImporter == null)
                return AssetType.None;
            foreach (KeyValuePair<AssetType, AssetImportHelperBase> item in helpers)
            {
                if (item.Value.IsMatch(assetImporter))
                    return item.Key;
            }
            return AssetType.None;
        }
    }
}
