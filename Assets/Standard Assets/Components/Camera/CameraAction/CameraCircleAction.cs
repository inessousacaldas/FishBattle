using UnityEngine;

public class CameraCircleAction : ICameraAction
{
    //偏移调整基数
    private const float MOVE_SPEED_PER_FRAME = 0.125f;


    private readonly SceneCameraController master;
    //旋转偏移(手指拖动屏幕的偏移值)
    private Vector2 delta = Vector2.zero;


    public LerpType LerpType { get { return LerpType.None; } }

    public CameraCircleAction(SceneCameraController master)
    {
        this.master = master;
    }
    public void Active(Transform mTarget, Transform mFollower)
    {
        mFollower.position = mTarget.position + master.followDirector;
        mFollower.eulerAngles = master.worldCameraLocalEulerAngles;
        UICamera.onDrag += OnDragHandler;
    }

    public void DeActive(Transform mTarget, Transform mFollower)
    {
        UICamera.onDrag -= OnDragHandler;

    }

    public void ResultCameraLocation(Transform mTarget, Transform mFollower, out Vector3 pos, out Vector3 lookTarget, out Quaternion quaternion)
    {
        Vector2 ft = master.GetTargetOffserPos - mFollower.position;
        Vector3 fo = Vector3.Project(ft, -mTarget.up);
        Vector3 o = fo + mFollower.position;
        //获得follower新的水平坐标法向量
        Vector3 nto = (o - master.GetTargetOffserPos).normalized;
        //拖动旋转
        nto = GetFinalPosition(nto);
        //follower新的水平坐标
        Vector3 newo = nto * master.cameraRadius + master.GetTargetOffserPos;

        //fo的单位向量
        Vector3 nfo = -mTarget.up;
        Vector3 newf = newo - nfo * master.cameraHeight;

        pos = newf;
        lookTarget = mTarget.transform.position;
        quaternion = Quaternion.identity;
    }

    /// <summary>
    /// 根据delta偏移获取最终坐标
    /// </summary>
    /// <param name="vector">计算delta偏移前的坐标</param>
    /// <returns></returns>
    private Vector3 GetFinalPosition(Vector3 vector)
    {
        //水平偏移值是否等于0
        Vector3 position = delta.x != 0 ? Quaternion.Euler(0, delta.x * MOVE_SPEED_PER_FRAME, 0) * vector : vector;
        delta = Vector2.zero;
        return position;
    }
    private void OnDragHandler(GameObject go, Vector2 delta)
    {
        //Trigger实际上为 JoystickModule 下的
        if (go.name != "Trigger")
        {
            return;
        }
        this.delta = delta;
    }
}
