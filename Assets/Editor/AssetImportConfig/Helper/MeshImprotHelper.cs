using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetImport
{
    class MeshImprotHelper : AssetImportHelperBase
    {
        const string pet = "Assets/GameResources/ArtResources/Characters/Pet";
        const string Scenes = "Assets/GameResources/ArtResources/Scenes";
        const string Weapon = "Assets/GameResources/ArtResources/Characters/Weapon";
        private const string Soul = "Assets/GameResources/ArtResources/Characters/Soul";
        private const string Fashion = "Assets/GameResources/ArtResources/Characters/Fashion";
        private static readonly string[] path =
        {
            pet,
            Weapon,
            Scenes,
            Soul,
            Fashion,
        };

        public override void DrawAssetConfigGUI(AssetItemConfigBase assetItemConfigBase)
        {
            MeshImprotConfig config = (MeshImprotConfig)assetItemConfigBase;
            config.isReadable = EditorGUILayout.Toggle("readWriteEnabled", config.isReadable);
            config.optimizeMesh = EditorGUILayout.Toggle("optimiseMesh", config.optimizeMesh);
            config.importBlendShapes = EditorGUILayout.Toggle("importBlendShapes", config.importBlendShapes);
            config.importNormals = (ModelImporterNormals)EditorGUILayout.EnumPopup("importNormals", config.importNormals);
            config.importTangents = (ModelImporterTangents)EditorGUILayout.EnumPopup("importTangents", config.importTangents);

        }

        public override IEnumerable<string> GetAllAsset()
        {
            return AssetDatabase.FindAssets("t: model", path)
              .Select(item => AssetDatabase.GUIDToAssetPath(item));
        }

        public override AssetItemConfigBase GetAssetConfig(AssetImportConfig assetImportConfig, string assetPath, out bool isDefault)
        {
            MeshImprotConfig config;
            isDefault = false;
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            if (!assetImportConfig.GetAtlasConfig<MeshImprotConfig>().TryGetValue(fileName, out config))
            {
                config = (MeshImprotConfig)GetDefaultConfig(assetPath);
                isDefault = true;
            }
            return config;
        }

        public override AssetItemConfigBase GetDefaultConfig(string assetPath)
        {
            if(assetPath.Contains(pet) || assetPath.Contains(Weapon) || assetPath.Contains(Fashion) || assetPath.Contains(Soul))
                return GetPetConfig();
            if (assetPath.Contains(Scenes))
                return GetSceneConfig();
            return GetModelConfig();
        }

        public override bool IsMatch(AssetImporter assetImporter)
        {
            return assetImporter is ModelImporter && path.Any(assetImporter.assetPath.Contains);
        }

        public override void SetImporterByConfig(AssetImporter assetImporter, AssetItemConfigBase config)
        {
            ModelImporter importer = (ModelImporter)assetImporter;
            List<string> setFiledInfoList = new List<string>() { "isReadable", "optimizeMesh", "importBlendShapes", "importNormals", "importTangents", "importMaterials"};
            bool dirty = false;
            foreach (var item in setFiledInfoList)
                dirty |= TrySetField(item, config, importer);

            if (dirty)
                assetImporter.SaveAndReimport();
        }

        private MeshImprotConfig GetPetConfig()
        {
            var config = new MeshImprotConfig();
            config.isReadable = false;
            config.optimizeMesh = true;
            config.importBlendShapes = false;
            config.importNormals = ModelImporterNormals.Import;
            config.importTangents = ModelImporterTangents.None;
            config.importMaterials = false;
            return config;
        }
        private MeshImprotConfig GetSceneConfig()
        {
            var config = new MeshImprotConfig();
            config.isReadable = false;
            config.optimizeMesh = true;
            config.importBlendShapes = false;
            config.importNormals = ModelImporterNormals.Import;
            config.importTangents = ModelImporterTangents.None;
            config.importMaterials = false;
            return config;
        }
        private MeshImprotConfig GetModelConfig()
        {
            var config = new MeshImprotConfig();
            config.isReadable = false;
            config.optimizeMesh = true;
            config.importBlendShapes = false;
            config.importNormals = ModelImporterNormals.None;
            config.importTangents = ModelImporterTangents.None;
            config.importMaterials = false;
            return config;
        }
    }
}
