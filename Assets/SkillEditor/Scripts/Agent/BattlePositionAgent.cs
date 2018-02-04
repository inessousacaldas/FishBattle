using System;
using AppDto;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public class BattlePositionAgent
    {
        public enum BattleSide
        {
            TeamA,
            TeamB,
            None,
        }


        public static BattlePosition.MonsterSide ConvertBattleSide(BattleSide side)
        {
            return (BattlePosition.MonsterSide) side;
        }

        public static BattleSide ConvertBattleSide(BattlePosition.MonsterSide side)
        {
            return (BattleSide) side;
        }
    }
}
#endif