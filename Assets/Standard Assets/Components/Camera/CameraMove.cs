using System;
using UnityEngine;

public sealed class BattleCameraMove : MonoBehaviour
{
    public Transform target;
    public bool followTarget;

    public Transform cameraTrans;
    // 场景中心位置

    private Vector3 position;
    private Vector3 centrePosition;
    private Vector3 aaaPosition;  // 可以移动的坐标系的位置
    private const string bbbName = "SyncCoordinatePos";

    public Vector3 CenterPosition
    {
        set { centrePosition = value; }
    }

    private void SyncCoordinatePos(Vector3 pos)
    {
        position = pos;
        FollowTarget(parentTrans, bbbName);
    }

    private void FollowTarget(Transform transform, string timerName)
    {
        CSTimer.Instance.SetupCoolDown(
            timerName
            , 0.08f  // 播战斗动画的时间 为避免出错这里不使用finishcallback
            , null
            , delegate
            {
                var tempPos = Vector3.Lerp(position, transform.position, _scaleFactor);
                transform.position = tempPos;
                var offset = 0.0001f;
                if (Math.Abs(position.x - tempPos.x) > offset
                    || Math.Abs(position.y - tempPos.y) > offset
                    || Math.Abs(position.z - tempPos.z) > offset)
                {
                    FollowTarget(transform, timerName);
                }

            });
    }

    private Transform positionTrans;
    private Transform parentTrans;
    private float _scaleFactor = 0.2f;

    public Transform PositionTrans
    {
        set
        {
            positionTrans = value;
            parentTrans = positionTrans.parent;
        }
    }

    public void Follow(Transform t)
    {
        target = t;
        followTarget = t != null;
        SyncCoordinatePos(followTarget ? t.transform.position : centrePosition);
    }

    public void ResetCoordinate()
    {
        SyncCoordinatePos(centrePosition);
    }

    public void Reset()
    {
        parentTrans.position = Vector3.zero;
        positionTrans.position = Vector3.zero;
    }

    public void SetPreViewCamera()
    {
        parentTrans.position = Vector3.zero;
        positionTrans.position = new Vector3(0.9f, 0.12f, -0.28f);
        positionTrans.rotation = Quaternion.Euler(new Vector3(13.08f, 29.51f, 6.90f));
    }
}