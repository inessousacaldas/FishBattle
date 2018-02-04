using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// CameraConrotller 负责提供Camera的状态数据给Action计算
/// 处理切换CameraAction的冲突
/// 内部数据的更新
/// 提供对外接口
/// </summary>
public class SceneCameraController : MonoBehaviour
{
    public bool ActiveCameraController
    {
        get { return mActiveCameraController; }
        set
        {
            if (mActiveCameraController == false && curAction != null)
            {
                if (target != null)
                {
                    curAction.Active(target, transform);
                    UpdateFollowDirector();
                }
            }
            mActiveCameraController = value;
        }
    }
    private bool mActiveCameraController = false;

    //插值模式，限制移动速度
    [SerializeField]
    private float speedLimit = 4;
    [SerializeField]
    private float distancelerpSpeed = 5;

    /// <summary>
    /// 当前控制器
    /// </summary>
    private ICameraAction curAction;
    private CameraActionKey curCameraActionKey;
    private Dictionary<CameraActionKey, ICameraAction> cameraActions;
    private List<CameraDecoratorBase> decoratorList;

    private Transform target;
    public Vector3 worldCameraLocalPosition { get; private set; }
    public Vector3 worldCameraLocalEulerAngles { get; private set; }
    [SerializeField]
    private float mRadius;
    [SerializeField]
    private float mHeight;
    [SerializeField]
    private float factor = 1;
    [SerializeField]
    private Vector3 offset = new Vector3(0, 1, 0);

    private Vector2 addDistance = Vector2.zero;

    public float cameraRadius { get { return mRadius * factor + addDistance.x; } }
    public float cameraHeight { get { return mHeight * factor + addDistance.y; } }
    public Vector3 GetTargetOffserPos { get { return target.position + offset; } }

    public Vector3 followDirector { get; private set; }
    void Awake()
    {
        cameraActions = new Dictionary<CameraActionKey, ICameraAction>(new CameraEqualityComparer());
        cameraActions.Add(CameraActionKey.Normal, new CameraNormalAction(this));
        cameraActions.Add(CameraActionKey.Circle, new CameraCircleAction(this));
        cameraActions.Add(CameraActionKey.Normal_Lock, new CameraNormalLockAction(this));
        curAction = cameraActions[CameraActionKey.Normal];
        curCameraActionKey = CameraActionKey.Normal;

        decoratorList = new List<CameraDecoratorBase>();
    }


    void LateUpdate()
    {
        if (mActiveCameraController && curAction != null)
        {
            UpdateDistanceSmoot();
            ApplyActionLocation();
            UpdateFollowDirector();
        }
    }

    private float smootVelocity;
    private float smootDistanceTime;
    private Vector2 smootDirector;
    private Vector2 smootStartDirector;
    private float smootCurrent;
    private void UpdateDistanceSmoot()
    {
        Vector2 targetAddDistance = CameraDecoratorBase.GetDisantaceAdd(decoratorList, new Vector2(mRadius, mHeight));
        if (targetAddDistance != smootDirector)
        {
            smootDistanceTime = Vector2.Distance(targetAddDistance, addDistance) / distancelerpSpeed;
            smootDirector = targetAddDistance;
            smootStartDirector = addDistance;
            smootCurrent = 0;
        }
        smootCurrent = Mathf.SmoothDamp(smootCurrent, 1, ref smootVelocity, smootDistanceTime);
        addDistance = Vector2.Lerp(smootStartDirector, targetAddDistance, smootCurrent);
        smootDistanceTime = Mathf.Max(smootDistanceTime - Time.deltaTime, 0);
    }

    void ApplyActionLocation()
    {
        Vector3 pos;
        Vector3 targetPos;
        Quaternion quaternion;
        curAction.ResultCameraLocation(target, transform, out pos, out targetPos, out quaternion);
        switch (curAction.LerpType)
        {
            case LerpType.None:
                transform.position = pos;
                transform.rotation = targetPos != Vector3.zero ? Quaternion.LookRotation(targetPos - pos, Vector3.up) : quaternion;
                break;
            case LerpType.LerpPos:
                float lenght = Vector3.Distance(pos, transform.position);
                float moveLimit = speedLimit * Time.deltaTime;
                if (lenght > moveLimit)
                    pos = transform.position + (pos - transform.position).normalized * speedLimit * Time.deltaTime;

                transform.position = pos;
                transform.rotation = targetPos != Vector3.zero ? Quaternion.LookRotation(targetPos - pos, Vector3.up) : quaternion;
                break;
        }
    }

    private void UpdateFollowDirector()
    {
        followDirector = transform.position - target.position;
    }

    public void SetTartget(Transform target)
    {
        if (this.target == target)
            return;
        this.target = target;
        ResetCameraPos(target.position);
        UpdateFollowDirector();
        curAction.Active(target, transform);
        UpdateFollowDirector();
    }
    /// <summary>
    /// 初始化Camera坐标
    /// </summary>
    public void ResetCameraPos(Vector3 position)
    {
        transform.position = position + worldCameraLocalPosition;
        transform.localEulerAngles = worldCameraLocalEulerAngles;
    }


    public void SetCameraConst(Vector3 worldCameraLocalPosition, Vector3 worldCameraLocalEulerAngles)
    {
        this.worldCameraLocalPosition = worldCameraLocalPosition;
        this.worldCameraLocalEulerAngles = worldCameraLocalEulerAngles;

        mHeight = Mathf.Abs(worldCameraLocalPosition.y);
        mRadius = new Vector3(worldCameraLocalPosition.x, 0, worldCameraLocalPosition.z).magnitude;

        if (followDirector == Vector3.zero)
            followDirector = worldCameraLocalPosition;
    }

    public void ChangeAction()
    {
        if (curCameraActionKey == CameraActionKey.Normal)
            ChangeAction(CameraActionKey.Circle);
        else
            ChangeAction(CameraActionKey.Normal);
    }

    public void ChangeAction(CameraActionKey cameraActionKey)
    {
        if (curCameraActionKey != cameraActionKey)
        {
            var nextAction = cameraActions[cameraActionKey];
            var lastAction = curAction;
            lastAction.DeActive(target, transform);
            nextAction.Active(target, transform);
            curCameraActionKey = cameraActionKey;
            curAction = nextAction;
        }
    }

    public void AddDecorator(CameraDecoratorBase cameraDecoratorBase)
    {
        decoratorList.Add(cameraDecoratorBase);
    }
    public void RemoveDecorator(CameraDecoratorBase cameraDecoratorBase)
    {
        decoratorList.Remove(cameraDecoratorBase);
    }
    #region Helper
    private class CameraEqualityComparer : IEqualityComparer<CameraActionKey>
    {
        public bool Equals(CameraActionKey x, CameraActionKey y)
        {
            return x == y;
        }

        public int GetHashCode(CameraActionKey obj)
        {
            return (int)obj;
        }
    }
    #endregion
}

public enum CameraActionKey
{
    Normal,
    Circle,
    Normal_Lock,
}