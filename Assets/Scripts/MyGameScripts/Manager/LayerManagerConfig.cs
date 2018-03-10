using UnityEngine;

public class GameTag
{
    public const string Tag_Untagged = "Untagged";

    public const string Tag_WorldActor = "WorldActor";

    public const string Tag_BattleActor = "BattleActor";

    public const string Tag_Terrain = "Terrain";

    public const string Tag_UI = "UI";

    public const string Tag_Npc = "Npc";

    public const string Tag_Teleport = "Teleport";

    public const string Tag_Player = "Player";

    public const string Tag_Default = "Default";

    public const string Tag_SceneEffect = "SceneEffect";

    public const string Tag_UnitEffect = "UnitEffect";

    public const string Tag_DefaultOrnament = "DefaultOrnament";

    public const string Tag_NewOrnament = "NewOrnament";

    public const string Tag_DreamlandNpc = "DreamlandNpc";

    public const string Tag_Building = "Building";

    public const string Tag_GridMapBuild = "GridMapBuild";

    public static int LayerId_GridMapBuild = LayerMask.NameToLayer(Tag_GridMapBuild);
}
