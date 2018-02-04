using UnityEditor;

namespace AssetImport
{
    public class AtlasOrFontConfig : AssetItemConfigBase
    {
        public int maxTextureSize;
        public bool mipmapEnabled;
        public bool isReadable;
        public bool alphaMip;   //Alpha贴图是否缩小
        public bool stripAlpha;
        public TextureImporterFormat standalone;
        public TextureImporterFormat iOS;
        public TextureImporterFormat Android;

        public AtlasOrFontConfig()
        {
            maxTextureSize = 2048;
            mipmapEnabled = false;
            isReadable = false;
            alphaMip = false;
            stripAlpha = true;

            standalone = TextureImporterFormat.AutomaticCompressed;
            Android = TextureImporterFormat.AutomaticCompressed;
            iOS = TextureImporterFormat.PVRTC_RGB4;
        }
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

