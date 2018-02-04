using UnityEngine;

public static class BattleConst
{
    public const int MaxTeamMemberCnt = 16;
    public const float PLAYER_Y_Rotation = 150f;
    public const float ENEMY_Y_Rotation = -30f;

    public static readonly Vector3 PlayerRetreatPoint = new Vector3(-20f, 0f, 0f);
    public static readonly Vector3 EnemyRetreatPoint = new Vector3(20f, 0f, 0f);
}