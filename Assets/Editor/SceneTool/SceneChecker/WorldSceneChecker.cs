namespace EditorNS
{
    public class WorldSceneChecker: ArtSceneChecker
    {
        public WorldSceneChecker(string scenePath) : base(scenePath)
        {
        }

        protected override string GetRootName()
        {
            return "WorldStage";
        }
    }
}