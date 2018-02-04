
using UnityEditor;

namespace AssetImport
{
    class MeshImprotConfig: AssetItemConfigBase
    {
        public bool isReadable;
        public bool optimizeMesh;
        public bool importBlendShapes;
        public ModelImporterNormals importNormals;
        public ModelImporterTangents importTangents;
        public bool importMaterials;
    }
}
