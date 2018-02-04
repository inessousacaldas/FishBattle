using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace AssetImport
{
    public class AnimationHelper : AssetImportHelperBase
    {
        private const string animationPath = "Assets/GameResources/ArtResources/Characters/Animation";

        public override IEnumerable<string> GetAllAsset()
        {
            return AssetDatabase.FindAssets("t: model", new[] { animationPath })
                .Select(item => AssetDatabase.GUIDToAssetPath(item));
        }
        public override void DrawAssetConfigGUI(AssetItemConfigBase assetItemConfigBase)
        {
            AnimationConfig config = (AnimationConfig)assetItemConfigBase;
            config.animationCompression = (ModelImporterAnimationCompression)EditorGUILayout.EnumPopup("动画压缩模式:", config.animationCompression);
        }
        public override AssetItemConfigBase GetAssetConfig(AssetImportConfig assetImportConfig, string assetPath, out bool isDefault)
        {
            AnimationConfig config;
            isDefault = false;
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            if (!assetImportConfig.GetAtlasConfig<AnimationConfig>().TryGetValue(fileName, out config))
            {
                config = GetDefault();
                isDefault = true;
            }
            return config;
        }

        private AnimationConfig GetDefault()
        {
            return new AnimationConfig();

        }

        public override AssetItemConfigBase GetDefaultConfig(string selectAssetPath)
        {
            return GetDefault();
        }

        public override bool IsMatch(AssetImporter assetImporter)
        {
            return assetImporter is ModelImporter && assetImporter.assetPath.Contains(animationPath);
        }

        public override void SetImporterByConfig(AssetImporter assetImporter, AssetItemConfigBase config)
        {
            ModelImporter importer = (ModelImporter)assetImporter;
            List<string> setFiledInfoList = new List<string>() { "animationCompression", "importMaterials" };
            bool dirty = false;
            foreach (var item in setFiledInfoList)
                dirty |= TrySetField(item, config, importer);
            if(dirty)
                assetImporter.SaveAndReimport();
        }
    }
}
