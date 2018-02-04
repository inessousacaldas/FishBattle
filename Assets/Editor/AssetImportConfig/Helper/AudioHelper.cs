using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetImport
{
    public class AudioHelper : AssetImportHelperBase
    {
        const string audioPath = "Assets/GameResources/ArtResources/Audios";
        public override void DrawAssetConfigGUI(AssetItemConfigBase assetItemConfigBase)
        {
            AudioConfig audioConfig = (AudioConfig)assetItemConfigBase;

            audioConfig.forceToMono = EditorGUILayout.Toggle("强制单声道", audioConfig.forceToMono);
            audioConfig.quality = EditorGUILayout.Slider("音频质量", audioConfig.quality*100f, 0, 100f)/100f;
            audioConfig.loadType = (AudioClipLoadType)EditorGUILayout.EnumPopup("加载方式", audioConfig.loadType);


        }

        public override IEnumerable<string> GetAllAsset()
        {
            return AssetDatabase.FindAssets("t: AudioClip", new[] { audioPath })
               .Select(item => AssetDatabase.GUIDToAssetPath(item));
        }

        public override AssetItemConfigBase GetAssetConfig(AssetImportConfig assetImportConfig, string assetPath, out bool isDefault)
        {
            AudioConfig config;
            isDefault = false;
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            if (!assetImportConfig.GetAtlasConfig<AudioConfig>().TryGetValue(fileName, out config))
            {
                config = GetDefault();
                isDefault = true;
            }
            return config;
        }

        public override AssetItemConfigBase GetDefaultConfig(string selectAssetPath)
        {
            return GetDefault();
        }

        private AudioConfig GetDefault()
        {
            var config = new AudioConfig();
            config.forceToMono = true;
            config.loadType = AudioClipLoadType.CompressedInMemory;
            config.quality = 0f;
            return config;
        }
        public override bool IsMatch(AssetImporter assetImporter)
        {
            return assetImporter is AudioImporter && assetImporter.assetPath.Contains(audioPath);
        }

        public override void SetImporterByConfig(AssetImporter assetImporter, AssetItemConfigBase config)
        {
            AudioImporter importer = (AudioImporter)assetImporter;
            List<string> setFiledInfoList = new List<string>() { "forceToMono", };
            bool dirty = false;
            foreach (var item in setFiledInfoList)
                dirty |= TrySetField(item, config, importer);

            setFiledInfoList.Clear();
            setFiledInfoList.Add("quality");
            setFiledInfoList.Add("loadType");
            //装箱后操作
            object defaultSample = importer.defaultSampleSettings;
            foreach (var item in setFiledInfoList)
                dirty |= TrySetField(item, config, defaultSample);
            importer.defaultSampleSettings = (AudioImporterSampleSettings)defaultSample;

            if (dirty)
                assetImporter.SaveAndReimport();
        }
    }
}
