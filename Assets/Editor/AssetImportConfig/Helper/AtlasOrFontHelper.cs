using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetImport
{
    public class AtlasOrFontHelper : AssetImportHelperBase
    {
        static readonly Dictionary<string, BuildTarget> buildTargets = new Dictionary<string, BuildTarget>()
                {
                    {"Standalone", BuildTarget.StandaloneWindows},
                    {"iPhone", BuildTarget.iOS},
                    {"Android", BuildTarget.Android},
                };
        public override IEnumerable<string> GetAllAsset()
        {
            return AssetDatabase.FindAssets("t: prefab", new[] { "Assets/UI/Atlas", "Assets/UI/Fonts" })
                .Select(item => AssetDatabase.GUIDToAssetPath(item))
                .Where(item =>
                {
                    var go = AssetDatabase.LoadAssetAtPath<GameObject>(item);
                    return go != null && (go.GetComponent<UIAtlas>() || go.GetComponent<UIFont>());
                });
        }

        public override AssetItemConfigBase GetAssetConfig(AssetImportConfig assetImportConfig, string assetPath, out bool isDefault)
        {
            AtlasOrFontConfig config;
            isDefault = false;
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            if (!assetImportConfig.GetAtlasConfig<AtlasOrFontConfig>().TryGetValue(fileName, out config))
            {
                //找不到则遍历匹配文件名
                foreach (var pair in assetImportConfig.GetAtlasConfig<AtlasOrFontConfig>())
                {
                    if (fileName.Contains(pair.Key))
                        return pair.Value;
                }
                config = (AtlasOrFontConfig)GetDefaultConfig(assetPath);
                isDefault = true;
            }
            return config;
        }

        public override AssetItemConfigBase GetDefaultConfig(string selectAssetPath)
        {
            if (!selectAssetPath.Contains("/Fonts/"))
                return GetAtlasDefault();
            else
                return GetFontsDefault();
        }
        private AtlasOrFontConfig GetAtlasDefault()
        {
            return new AtlasOrFontConfig();
        }
        private AtlasOrFontConfig GetFontsDefault()
        {
            var config = new AtlasOrFontConfig();
            config.iOS = TextureImporterFormat.AutomaticTruecolor;
            return config;
        }
        public override bool IsMatch(AssetImporter assetImporter)
        {
            var path = assetImporter.assetPath;
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go != null)
            {
                return go.GetComponent<UIAtlas>() || go.GetComponent<UIFont>();
            }
            else
            {
                if (AssetImporter.GetAtPath(path) is TextureImporter)
                {
                    return AssetDatabase.FindAssets("t:prefab", new[] {path.Remove(path.LastIndexOf("/"))})
                        .Select(item =>
                        {
                            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(item);
                            return prefab != null && (prefab.GetComponent<UIAtlas>() || prefab.GetComponent<UIFont>());
                        }).Any();
                }
            }
            return false;
        }

        public override void SetImporterByConfig(AssetImporter assetImporter, AssetItemConfigBase config)
        {
            IEnumerable<string> texPaths = null;
            if (assetImporter is TextureImporter)
            {
                texPaths = new[] {assetImporter.assetPath};
            }
            else
            {
                texPaths = AssetDatabase.GetDependencies(assetImporter.assetPath)
                    .Where(item => item.Contains(Path.GetFileNameWithoutExtension(assetImporter.assetPath)) && AssetImporter.GetAtPath(item) is TextureImporter);
            }
            
            foreach (string path in texPaths)
            {
                TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
                AtlasOrFontConfig atlasConfig = (AtlasOrFontConfig)config;
                List<string> setFiledInfoList = new List<string>() { "maxTextureSize", "mipmapEnabled", "isReadable" };
                bool dirty = false;
                foreach (var item in setFiledInfoList)
                    dirty |= TrySetField(item, config, importer);
                foreach (KeyValuePair<string, BuildTarget> keyValuePair in buildTargets)
                {
                    var format = atlasConfig.GetFormatByTarget(keyValuePair.Value);
                    int maxTextureSize;
                    TextureImporterFormat oldFormat;
                    if (importer.GetPlatformTextureSettings(keyValuePair.Key, out maxTextureSize, out oldFormat) == false
                        || oldFormat != format
                        || maxTextureSize != atlasConfig.maxTextureSize)
                    {
                        importer.SetPlatformTextureSettings(keyValuePair.Key, atlasConfig.maxTextureSize, format);
                        dirty = true;
                    }
                }
                if (importer.textureType != TextureImporterType.Advanced)
                {
                    importer.textureType = TextureImporterType.Advanced;
                    dirty = true;
                }
                if (dirty)
                    importer.SaveAndReimport();
            }
        }

        private int[] sizes = new[] { 32, 64, 128, 256, 512, 1024, 2048, 4096 };
        private string[] sizeStrings = new[] { "32", "64", "128", "256", "512", "1024", "2048", "4096" };
        public override void DrawAssetConfigGUI(AssetItemConfigBase assetItemConfigBase)
        {
            AtlasOrFontConfig config = (AtlasOrFontConfig)assetItemConfigBase;
            // per platform settings
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Max Texture Size");
            config.maxTextureSize =
            EditorGUILayout.IntPopup(config.maxTextureSize, sizeStrings, sizes);
            EditorGUILayout.EndHorizontal();

            // mip maps
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Generate Mip Maps");
            config.mipmapEnabled = EditorGUILayout.Toggle(config.mipmapEnabled);
            EditorGUILayout.EndHorizontal();

            //read write enabled
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Read/Write Enabled");
            config.isReadable = EditorGUILayout.Toggle(config.isReadable);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Alpha Mip");
            config.alphaMip = EditorGUILayout.Toggle(config.alphaMip);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Strip Alpha Channel");
            config.stripAlpha = EditorGUILayout.Toggle(config.stripAlpha);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Standalone TextureFormat");
                config.standalone = (TextureImporterFormat)EditorGUILayout.EnumPopup(config.standalone);
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5);
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Android TextureFormat");
                config.Android = (TextureImporterFormat)EditorGUILayout.EnumPopup(config.Android);
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5);
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("iOS TextureFormat");
                config.iOS = (TextureImporterFormat)EditorGUILayout.EnumPopup(config.iOS);
                EditorGUILayout.EndHorizontal();
            }
        }


    }
}
