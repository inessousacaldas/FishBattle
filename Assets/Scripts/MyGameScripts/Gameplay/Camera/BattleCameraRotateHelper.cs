using UnityEngine;
using System;
using DG.Tweening;
using UniRx;

/// <summary>
/// 战斗摄像机拖动旋转
/// 关于战斗镜头调整：
/// 美术直接调整WarCameara；
/// 程序把GameRoot拖到对应场景上，把WarCameara拖动到BattleCamera同层，把其属性拷贝到WarCamera即可，如果有调整摄像机参数，记得对BattleCamera_Terrain做对应调整
/// @MarsZ 2017年04月22日16:36:41
/// </summary>
public class BattleCameraRotateHelper : IDisposable
{

    //战斗摄像机移动速度，单位：秒每度（180°耗时3″）
    private const float BATTLE_CAMERA_ROTATE_SPEED_SEC_PER_DEGREE = 3f / 180f;
    private const float MOVE_SPEED_PER_FRAME = 8f;
    private const float BATTLE_CAMERA_ROTATE_Y_MAX = 7;
    //欧拉非负
    private const float BATTLE_CAMERA_ROTATE_Y_MIN = -7;

    /// <summary>
    /// 战斗摄像机允许拖拽旋转的方向
    /// </summary>
    /// <value>The rotation direction.</value>

    public CameraRotationDirection RotationDirection{ get; set; }

    private Transform trans;

    #region interface

    private BattleCameraRotateHelper()
    {

    }

    public void OnDragHandler(GameObject go, Vector2 delta)
    {
        if (Input.touchCount > 1)
            return;
        if (!IsDragValid(go))
            return;

        var tRotation = Vector3.zero;
        tRotation = new Vector3(0, GetMoveRotation(delta.x), 0);

        //瞬间拖动完毕
        RotateBattleRotationCntr(Quaternion.Euler(trans.localRotation.eulerAngles + tRotation), BATTLE_CAMERA_ROTATE_SPEED_SEC_PER_DEGREE);

        /**
        switch (RotationDirection)
        {
            case CameraRotationDirection.Horizontal:
                tRotation = new Vector3(0, GetMoveRotation(delta.x), 0);
                break;
            case CameraRotationDirection.Vertical:
                tRotation = new Vector3(GetMoveRotation(delta.y), 0, 0);
                break;
            case CameraRotationDirection.Both:
                {
                    //一次只转一个方向
                    if (Math.Abs(delta.x) > Math.Abs(delta.y))
                        tRotation = new Vector3(0, GetMoveRotation(delta.x), 0);
                    else
                        tRotation = new Vector3(GetMoveRotation(delta.y), 0, 0);
                }
                break;
        }
        //瞬间拖动完毕
        RotateBattleRotationCntr(Quaternion.Euler(LayerManager.Root.BattlePositionCntr_Transform.localRotation.eulerAngles + tRotation), BATTLE_CAMERA_ROTATE_SPEED_SEC_PER_DEGREE);
        **/
    }

    private float GetMoveRotation(float pDelta)
    {
        return pDelta / MOVE_SPEED_PER_FRAME;
    }

    private bool IsDragValid(GameObject go)
    {
        // todo fish
//        return JoystickModule.Instance.IsJoystickTrigger(go);
        return true;
    }

    public void OnBtnResetRotationClick()
    {
        Rotate(CameraConst.BATTLECAMERA_DEFAULT_ROTATION_INIT);
    }

    public void Rotate(Vector3 v3)
    {
        var tTotalDegree = trans.localRotation.eulerAngles.y;
        tTotalDegree = Math.Abs(tTotalDegree - Quaternion.Euler(v3).eulerAngles.y);
        //匀速回归
        var tDuration = Math.Abs(tTotalDegree * BATTLE_CAMERA_ROTATE_SPEED_SEC_PER_DEGREE);
        RotateBattleRotationCntr(Quaternion.Euler(v3), tDuration);
    }

    public void StopCameraEvt()
    {
        UICamera.onDrag -= OnDragHandler;
        ResetCamera();
    }

    public void Dispose()
    {
        StopCameraEvt();
    }

    #endregion

    #region utils

    public void Initialize()
    {
        UICamera.onDrag += OnDragHandler;
        ResetCamera();
    }

    private void ResetCamera()
    {
        ResetRotation();
        LayerManager.Root.BattleCamera_Animator.enabled = false;
    }

    private void RotateBattleRotationCntr(Quaternion pEndValue, float pDuration)
    {
        Vector3 tEndValueEuler = pEndValue.eulerAngles;
        //还是方向问题，编辑器中x为负数时，本处>180
        if (tEndValueEuler.x > 180)
        {
            tEndValueEuler.x = tEndValueEuler.x - 360;
            if (tEndValueEuler.x < BATTLE_CAMERA_ROTATE_Y_MIN)
                tEndValueEuler.x = BATTLE_CAMERA_ROTATE_Y_MIN;
        }
        else
        {
            if (tEndValueEuler.x > BATTLE_CAMERA_ROTATE_Y_MAX)
                tEndValueEuler.x = BATTLE_CAMERA_ROTATE_Y_MAX;
        }
        tEndValueEuler.z = 0;
        trans.DOKill();
        trans.DORotateQuaternion(Quaternion.Euler(tEndValueEuler), pDuration).SetEase(Ease.Linear);
    }

    private void ResetRotation()
    {
        trans.DOKill();
        trans.localRotation = Quaternion.Euler(CameraConst.BATTLECAMERA_DEFAULT_ROTATION_INIT);
    }

    #endregion

    public static BattleCameraRotateHelper Create(CameraRotationDirection dir)
    {
        var helper = new BattleCameraRotateHelper();
        helper.RotationDirection = dir;
        helper.trans = LayerManager.Root.BattleDefaultRotationCntr_Transform;
        
        return helper;
    }
}

/// <summary>
/// 战斗摄像机允许拖拽旋转的方向
/// </summary>
public enum CameraRotationDirection
{
    Undefined = 0,
    Horizontal = 1,
    Vertical = 2,
    Both = 3
}