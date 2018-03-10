using UnityEngine;

public static class BattleConst
{
    public const int MaxTeamMemberCnt = 16;
    public const float PLAYER_Y_Rotation = 150f;
    public const float ENEMY_Y_Rotation = -30f;

    public static readonly Vector3 PlayerRetreatPoint = new Vector3(-20f, 0f, 0f);
    public static readonly Vector3 EnemyRetreatPoint = new Vector3(20f, 0f, 0f);

    // 场景中心
    public static readonly Vector3 SceneCenterVector3 = new Vector3(0.14f, 0f, 0.07f);
    public static readonly Vector3 PlayerCenterVector3 = new Vector3(-2.165f, 0f, 2.7f);
    public static readonly Vector3 EnemyCenterVector3 = new Vector3(2.445f, 0f, -2.56f);
}