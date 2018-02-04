using UnityEngine;
using System.Collections;

public class CamFollowPlayerPosCommand : BaseCommand {
    public CamFollowPlayerPosCommand (Vector3 playerPos)
    {
        this.playerPos = playerPos;
    }

    Vector3 playerPos;
    static GameObject go = null;
    public override void Execute()
    {
        base.Execute();
        playerPos = SceneHelper.GetSceneStandPosition(this.playerPos, playerPos);
        GamePlayer.CameraManager.Instance.SceneCamera.ResetTargetPos(playerPos);
        this.OnFinish();
    }
}
