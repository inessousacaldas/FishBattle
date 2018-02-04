using UnityEngine;
using System.Collections;

/// <summary>
/// 该接口负责提供Camera的行为的计算方法，根据SceneCameraControl来提供的状态数据实现。
/// Action原则上不能保存Camera的任何状态数据，任何状态数据的变化都应该保存在SceneCameraControl中。
/// 这样才能保证Action之间互相切换时状态数据正确。
/// </summary>
public interface ICameraAction
{
    /// <summary>
    /// 插值控制类型
    /// </summary>
    LerpType LerpType { get; }

    void Active(Transform mTarget, Transform mFollower);
    void DeActive(Transform mTarget, Transform mFollower);

    /// <summary>
    /// lookTarget 和 Quatternion 表示Camera的旋转，只有其中一个有效
    /// 当lookTarget == Vector.Zero 时候，Quatternion有效
    /// lookTarget表示摄像机Look的坐标
    /// Quatternion则表示摄像机旋转的四元数
    /// </summary>
    void ResultCameraLocation(Transform mTarget, Transform mFollower, out Vector3 pos, out Vector3 lookTarget, out Quaternion quaternion);


}
