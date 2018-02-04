using UnityEditor;

namespace AssetImport
{
    public class AnimationConfig : AssetItemConfigBase
    {
        public ModelImporterAnimationCompression animationCompression = ModelImporterAnimationCompression.Optimal;
        public bool importMaterials = false;
    }


}
