

using UnityEditor;

namespace AssetImport
{
    class TextureConfig : AssetItemConfigBase
    {
        public int maxTextureSize;
        public bool isReadable;
        public bool mipmapEnabled;
        public bool eachPlatform;
        public TextureImporterFormat textureFormat;
        public TextureImporterFormat standalone;
        public TextureImporterFormat iOS;
        public TextureImporterFormat Android;
        public TextureImporterFormat GetFormatByTarget(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.iOS:
                    return iOS;
                case BuildTarget.Android:
                    return Android;
                default:
                    return standalone;
            }
        }
    }
}
