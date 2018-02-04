namespace AssetPipeline
{
    /// <summary>
    /// 小的优先级高
    /// </summary>
    public static class AssetLoadPriority
    {
        public const float SceneConfig = 86f;
        public const float StreamScene = 87f;
        public const float SceneGoBig = 87.1f;
        public const float SceneGoNormal = 87.2f;
        public const float SceneGoSmall = 87.3f;
        public const float Scene2DMiniMap = 88f;
        public const float Model = 89f;
        public const float Scene2DTitleMap = 90f;
        public const float Default = 100f;
        public const float Cache = 101f;
    }
}


