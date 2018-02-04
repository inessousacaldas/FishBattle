namespace EditorNS
{
    public class BattleSceneChecker: ArtSceneChecker
    {
        public BattleSceneChecker(string scenePath) : base(scenePath)
        {
        }

        protected override string GetRootName()
        {
            return "BattleStage";
        }
    }
}