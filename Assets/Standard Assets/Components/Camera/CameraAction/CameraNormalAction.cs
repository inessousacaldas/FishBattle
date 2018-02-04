using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

/// <summary>
/// 场景SceneCamera的常驻控制器,用于角色跟随， 相机行为
/// </summary>
public class CameraNormalAction : ICameraAction
{
    private const float MOVE_SPEED_PER_FRAME = 0.125f;
    private readonly SceneCameraController master;

    //偏移调整基数
    //旋转偏移(手指拖动屏幕的偏移值)
    private Vector2 delta = Vector2.zero;

    public LerpType LerpType { get { return LerpType.None; } }

    public CameraNormalAction(SceneCameraController master)
    {
        this.master = master;
    }
    public void Active(Transform mTarget, Transform mFollower)
    {
        if (mTarget == null || mFollower == null)
            return;
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
        Vector3 lookDir = master.followDirector;
        lookDir.y = 0;
        //拖动旋转
        Vector3 finalhorizontalvector = GetFinalPosition(lookDir.normalized);
        pos = finalhorizontalvector * master.cameraRadius + master.GetTargetOffserPos;
        pos.y += master.cameraHeight;
        lookTarget = master.GetTargetOffserPos;
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
