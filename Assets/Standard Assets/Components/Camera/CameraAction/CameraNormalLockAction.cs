using UnityEngine;

/// <summary>
/// 锁定视角的场景SceneCamera的控制器,用于角色跟随， 相机行为
/// </summary>
public class CameraNormalLockAction : ICameraAction
{
    //偏移调整基数
    //旋转偏移(手指拖动屏幕的偏移值)
    private readonly SceneCameraController master;

    private Vector3 follow;
    public LerpType LerpType { get { return LerpType.None; } }
    public float ratation = 273.1f;
    private ICameraAction _cameraActionImplementation;

    public CameraNormalLockAction(SceneCameraController master)
    {
        this.master = master;
    }
    
    public void Active(Transform mTarget, Transform mFollower)
    {
        follow = master.followDirector;
    }

    public void DeActive(Transform mTarget, Transform mFollower)
    {
//        master.followDirector = follow ;
    }

    public void ResultCameraLocation(Transform mTarget, Transform mFollower, out Vector3 pos, out Vector3 lookTarget,
        out Quaternion quaternion)
    {
        Vector3 finalhorizontalvector = Quaternion.Euler(0, ratation, 0) * new Vector3(0, 0, 1);
        pos = finalhorizontalvector * master.cameraRadius + master.GetTargetOffserPos;
        pos.y += master.cameraHeight;
        lookTarget = master.GetTargetOffserPos;
        quaternion = Quaternion.identity;
    }
}

