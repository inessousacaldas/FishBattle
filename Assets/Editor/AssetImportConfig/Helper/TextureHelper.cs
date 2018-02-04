

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetImport
{
    class TextureHelper : AssetImportHelperBase
    {
        static readonly Dictionary<string, BuildTarget> buildTargets = new Dictionary<string, BuildTarget>()
        {
            {"Standalone", BuildTarget.StandaloneWindows},
            {"iPhone", BuildTarget.iOS},
            {"Android", BuildTarget.Android},
        };
        const string pet = "Assets/GameResources/ArtResources/Characters/Pet";
        const string Scenes = "Assets/GameResources/ArtResources/Scenes";
        const string UITexture = "Assets/UI/Images";
        const string Soul = "Assets/GameResources/ArtResources/Characters/Soul";
        const string Weapon = "Assets/GameResources/ArtResources/Characters/Weapon";
        const string Fashion = "Assets/GameResources/ArtResources/Characters/Fashion";
        private static readonly string[] paths =
        {
            Soul,
            Weapon,
            Fashion,
            pet,
            Scenes,
            UITexture,
        };

        public override IEnumerable<string> GetAllAsset()
        {
            return AssetDatabase.FindAssets("t: texture", paths)
                .Select(item => AssetDatabase.GUIDToAssetPath(item));
        }

        public override AssetItemConfigBase GetAssetConfig(AssetImportConfig assetImportConfig, string assetPath, out bool isDefault)
        {
            TextureConfig config;
            isDefault = false;
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            if (!assetImportConfig.GetAtlasConfig<TextureConfig>().TryGetValue(fileName, out config))
            {
                config = (TextureConfig)GetDefaultConfig(assetPath);
                isDefault = true;
            }
            return config;
        }

        public override AssetItemConfigBase GetDefaultConfig(string assetPath)
        {
            if (assetPath.Contains(pet) || assetPath.Contains(Soul) || assetPath.Contains(Weapon) || assetPath.Contains(Fashion))
                return GetPetConfig();
            if (assetPath.Contains(Scenes))
            {
                if (assetPath.Contains("Lightmap-"))
                    return GetLightmapConfig();
                if (assetPath.Contains("/Terrain/"))
                    return GetTerrainConfig();
            }
            return GetDefaultConfig();
        }

        public override bool IsMatch(AssetImporter assetImporter)
        {
            return assetImporter is TextureImporter && paths.Any(assetImporter.assetPath.Contains);
        }

        public override void SetImporterByConfig(AssetImporter assetImporter, AssetItemConfigBase config)
        {
            TextureImporter importer = (TextureImporter)assetImporter;
            TextureConfig textureConfig = (TextureConfig)config;
            List<string> setFiledInfoList = new List<string>() { "maxTextureSize", "isReadable", "mipmapEnabled" };
            bool dirty = false;
            foreach (var item in setFiledInfoList)
                dirty |= TrySetField(item, config, importer);


            if (textureConfig.eachPlatform)
            {
                foreach (KeyValuePair<string, BuildTarget> keyValuePair in buildTargets)
                {
                    var format = textureConfig.GetFormatByTarget(keyValuePair.Value);
                    int maxTextureSize;
                    TextureImporterFormat oldFormat;
                    if (importer.GetPlatformTextureSettings(keyValuePair.Key, out maxTextureSize, out oldFormat) == false
                        || oldFormat != format
                        || maxTextureSize != textureConfig.maxTextureSize)
                    {
                        importer.SetPlatformTextureSettings(keyValuePair.Key, textureConfig.maxTextureSize, format);
                        dirty = true;
                    }
                }
            }
            else
            {
                if (textureConfig.textureFormat != importer.textureFormat)
                {
                    dirty = true;
                    importer.textureFormat = textureConfig.textureFormat;
                }
            }


            if (importer.textureType != TextureImporterType.Advanced)
            {
                importer.textureType = TextureImporterType.Advanced;
                dirty = true;
            }
            if (dirty)
                assetImporter.SaveAndReimport();
        }

        private static readonly int[] textureSize = new[] {32, 64, 128, 256, 512, 1024, 2048};
        private static readonly string[] textureSizeText = new[] {"32", "64", "128", "256", "512", "1024", "2048"};
        public override void DrawAssetConfigGUI(AssetItemConfigBase assetItemConfigBase)
        {
            var config = (TextureConfig)assetItemConfigBase;
            config.maxTextureSize = EditorGUILayout.IntPopup("maxTextureSize", config.maxTextureSize, textureSizeText, textureSize);
            config.isReadable = EditorGUILayout.Toggle("isReadable", config.isReadable);
            config.mipmapEnabled = EditorGUILayout.Toggle("mipmapEnabled", config.mipmapEnabled);
            config.eachPlatform = EditorGUILayout.Toggle("每个平台单独设置", config.eachPlatform);
            if (config.eachPlatform)
            {
                config.standalone = (TextureImporterFormat)EditorGUILayout.EnumPopup("standalone", config.standalone);
                config.iOS = (TextureImporterFormat)EditorGUILayout.EnumPopup("iOS", config.iOS);
                config.Android = (TextureImporterFormat)EditorGUILayout.EnumPopup("Android", config.Android);
            }
            else
            {
                config.textureFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup("TextureFormat", config.textureFormat);
            }
        }

        private TextureConfig GetDefaultConfig()
        {
            var config = new TextureConfig();
            config.textureFormat = TextureImporterFormat.AutomaticCompressed;
            config.eachPlatform = false;
            config.maxTextureSize = 1024;
            config.isReadable = false;
            config.mipmapEnabled = false;
            return config;
        }

        private TextureConfig GetPetConfig()
        {
            var config = new TextureConfig();
            config.textureFormat = TextureImporterFormat.AutomaticCompressed;
            config.eachPlatform = false;
            config.maxTextureSize = 512;
            config.isReadable = false;
            config.mipmapEnabled = false;
            return config;
        }
        private TextureConfig GetLightmapConfig()
        {
            var config = new TextureConfig();
            config.textureFormat = TextureImporterFormat.AutomaticTruecolor;
            config.eachPlatform = false;
            config.maxTextureSize = 512;
            config.isReadable = false;
            config.mipmapEnabled = false;
            return config;
        }
        private TextureConfig GetTerrainConfig()
        {
            var config = new TextureConfig();
            config.textureFormat = TextureImporterFormat.AutomaticTruecolor;
            config.eachPlatform = false;
            config.maxTextureSize = 512;
            config.isReadable = false;
            config.mipmapEnabled = false;
            return config;
        }
    }
}
