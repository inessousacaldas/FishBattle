namespace SkillEditor
{
    public class SBMonsterController : MonsterController
    {
        //重置状态，用于技能演示的重置等。
        public override void ResetMonsterStatus()
        {
            currentHP = MaxHP;
            dead = false;
        }

        public override bool IsPlayerMainCharactor()
        {
            return false;
        }
    }
}