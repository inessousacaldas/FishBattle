
using AppDto;
using UnityEngine;
using VideoSoldier = AppDto.VideoSoldier;

public class BattlePositionCalculator
{
    public static Vector3 GetMonsterPosition(VideoSoldier soldier, BattlePosition.MonsterSide side)
    {
        var bp = DataCache.getArrayByCls<BattlePosition>().Find(s => s.monsterSideId == (int)side && s.index == soldier.position);
        
        var vec = bp.position.Split(':');
        var x = float.Parse(vec[0]);
        var z = float.Parse(vec[1]);

        return new Vector3(x, 0, z);
    }

    public static Vector3 GetZonePosition(BattlePosition.MonsterSide side)
    {
        if (side == BattlePosition.MonsterSide.Player)
        {
            return new Vector3(-3.41f, 0f, -2.43f);
        }
        return new Vector3(3.72f, 0f, -1.95f);
    }
}