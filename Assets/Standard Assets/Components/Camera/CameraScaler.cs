using UnityEngine;

/// <summary>
/// 摄像机（组）拉近拉远控制
/// 2017年07月05日11:19:13
/// </summary>
public class CameraScaler : MonoBehaviour
{
    private const string TIMER_NAME = "ResetScaleDelay";
    private const string TIMER_TO_RESET = "TIMER_TO_RESET";
    //释放拖动时衰减到停止的时间
    private const float TIME_TO_SLOW_DOWN = 0.5f;

    //最大（近）视角
    public float MaxFieldOfView = 38f;
    //最小（远）视角
    public float MinFieldOfView = 15f;

    public Transform target;
    public bool followTarget = true;
    //缩放手势变量
    public bool canScale = true;
    //锁死缩放
    public bool lockScale = false;
    public bool isMultiTouch = false;
    public float maxScaleFactor = 0.5f;
    public float minScaleFactor = -0.5f;
    public float damping = 5f;

    private Transform mTrans;
    private Camera[] mCameras;
    private float _scaleFactor;
    private Vector2 _lastTouchPos1 = Vector2.zero;
    private Vector2 _lastTouchPos2 = Vector2.zero;
    private float mDefaultFieldOfView = 30f;

    void Awake()
    {
        mTrans = this.transform;
        mCameras = GetComponentsInChildren<Camera>();
        InitProperty();
    }

    void Start()
    {
        SyncTargetPos();

        #if UNITY_STANDALONE
        UICamera.onScroll += OnUICameraScroll;
        #endif
    }

    private void OnDestroy()
    {
        #if UNITY_STANDALONE
        UICamera.onScroll -= OnUICameraScroll;
        #endif
        Reset();
    }

    #if UNITY_EDITOR || UNITY_STANDALONE
    private bool _isFocus = true;

    void OnApplicationFocus(bool isFocus)
    {
        _isFocus = isFocus;
    }
    #endif

    /// <summary>
    /// 视距更新变化核心逻辑
    /// </summary>
    void LateUpdate()
    {
        if (_scaleFactor != 0 && target && followTarget)
        {
            UpdateCameraFieldOfView(mCameras,_scaleFactor);
        }

        if (canScale && !lockScale)
        {
            float tTempScale = 0f;

            #if UNITY_EDITOR || UNITY_STANDALONE
            if (_isFocus && !CheckHasScrollOnce())
            {
                float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
                if (Mathf.Abs(scrollWheel) > 0f)
                {
                    tTempScale += scrollWheel;
                    tTempScale = Mathf.Clamp(tTempScale, minScaleFactor, maxScaleFactor);
                }
            }
            #endif

            //判断触摸数量为多点触摸
            if (Input.touchCount > 1)
            {
                isMultiTouch = true;

                //前两只手指触摸类型都为移动触摸
                if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
                {
                    //计算出当前两点触摸点的位置
                    var tempPosition1 = Input.GetTouch(0).position;
                    var tempPosition2 = Input.GetTouch(1).position;
                    //函数返回真为放大，返回假为缩小

                    if (_lastTouchPos1 != Vector2.zero && _lastTouchPos2 != Vector2.zero)
                    {
                        if (isEnlarge(_lastTouchPos1, _lastTouchPos2, tempPosition1, tempPosition2))
                        {
                            tTempScale += 0.5f;
                        }
                        else
                        {
                            tTempScale -= 0.5f;
                        }

                        tTempScale = Mathf.Clamp(tTempScale, minScaleFactor, maxScaleFactor);
                    }

                    //备份上一次触摸点的位置，用于对比
                    _lastTouchPos1 = tempPosition1;
                    _lastTouchPos2 = tempPosition2;
                }

            }
            else
            {
                if (Input.touchCount == 0)
                {
                    isMultiTouch = false;
                    _lastTouchPos1 = Vector2.zero;
                    _lastTouchPos2 = Vector2.zero;
                }
            }

            if (tTempScale == 0)
            {
                if(_scaleFactor != 0)
                    ResetScale();
                return;
            }
            CancelResetScaleDelay(TIMER_NAME);
            CancelResetScaleDelay(TIMER_TO_RESET);
            _scaleFactor = tTempScale;
        }
    }

    private void ResetScale()
    {
        if (CSTimer.Instance.IsCdExist(TIMER_NAME))
            return;
        CSTimer.Instance.SetupCoolDown(TIMER_NAME, TIME_TO_SLOW_DOWN, (pTime) =>
            {
                _scaleFactor *= pTime / TIME_TO_SLOW_DOWN;
            }, ResetScaleDelay,0.01f);
    }

    private bool CancelResetScaleDelay(string pTaskName)
    {
        if (!CSTimer.Instance.IsCdExist(pTaskName))
            return false;
        CSTimer.Instance.CancelCd(pTaskName);
        return true;
    }

    private void ResetScaleDelay()
    {
        _scaleFactor = 0;   
    }

    private void UpdateCameraFieldOfView(Camera[] pCameras,float pFieldOfView,bool pIsAdd =  true)
    {
        if (null == pCameras || pCameras.Length <= 0)
            return;
        for(int tCounter = 0 , tLen = pCameras.Length ; tCounter < tLen ; tCounter ++)
        {
            UpdateCameraFieldOfView(pCameras[tCounter],pFieldOfView,pIsAdd);
        }
    }

    private void UpdateCameraFieldOfView(Camera pCamera,float pFieldOfView,bool pIsAdd =  true)
    {
        float tFieldOfView = pFieldOfView;
        if (pIsAdd)
            tFieldOfView += pCamera.fieldOfView;
        tFieldOfView = Mathf.Clamp(tFieldOfView,MinFieldOfView,MaxFieldOfView);
        pCamera.fieldOfView = tFieldOfView;
    }

    public void SyncTargetPos()
    {
        if (target != null)
        {
            UpdatePos(target.position);
        }
    }

    public void UpdatePos(Vector3 pos)
    {
        mTrans.position = pos;
    }

    public void Follow(Transform t)
    {
        target = t;
        followTarget = true;
        SyncTargetPos();
    }

    public void ResetPos()
    {
        mTrans.localPosition = Vector3.zero;
        SyncTargetPos();
    }

    //函数返回真为放大，返回假为缩小
    private bool isEnlarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2)
    {
        //函数传入上一次触摸两点的位置与本次触摸两点的位置计算出用户的手势
        return (oP1 - oP2).sqrMagnitude >= (nP1 - nP2).sqrMagnitude;
    }

    #region 记录是否滑动过UIScrollView
    private GameObject _scrollGo;

    private void OnUICameraScroll(GameObject go, float delta)
    {
        _scrollGo = go;
    }

    /// <summary>
    /// 仅判断一次是否滑动过，方便及时释放掉
    /// </summary>
    private bool CheckHasScrollOnce()
    {
        //if (_scrollGo != null)
        //    CSGameDebuger.LogError("_scrollGo.name_________asdfasd:  " + _scrollGo.name);
        //return _scrollGo != null;

        var b = _scrollGo != null && _scrollGo.GetComponent<UIDragScrollView>() != null;
        _scrollGo = null;
        return b;
    }
    #endregion

    /// <summary>
    /// 初始化（绑定）缩放参数
    /// </summary>
    public void InitProperty()
    {
        mDefaultFieldOfView = 30f;
        followTarget = true;
        canScale = true;
        lockScale = false;
        isMultiTouch = false;
        maxScaleFactor = 0.5f;
        minScaleFactor = -0.5f;
        damping = 5f;
        _scaleFactor = 0f;
        _lastTouchPos1 = Vector2.zero;
        _lastTouchPos2 = Vector2.zero;
    }

    public void Reset(float fieldOfView, float pDuration = -1)
    {
        mDefaultFieldOfView = fieldOfView;
        ResetCameraScale(-pDuration);

        target = null;
        followTarget = false;
    }

    /// <summary>
    /// 还原摄像机视距设置，此操作不带有缓动
    /// </summary>
    
    public void Reset()
    {
        Reset(mDefaultFieldOfView, -1);
    }

    /// <summary>
    /// 重置摄像机视距为默认视距
    /// </summary>
    /// <param name="pDuration">P duration.参数为-1时无缓动</param>

    public void ResetMaxCamera(float pDuration = TIME_TO_SLOW_DOWN)
    {
        mDefaultFieldOfView = MaxFieldOfView;
        ResetCameraScale(pDuration);
    }

    public void ResetMinCamera(float pDuration = TIME_TO_SLOW_DOWN)
    {
        mDefaultFieldOfView = MinFieldOfView;
        ResetCameraScale(pDuration);
    }
    
    public void ResetCameraScale(float pDuration = TIME_TO_SLOW_DOWN)
    {
        if (null == mCameras || mCameras.Length <= 0 || CSTimer.Instance.IsCdExist(TIMER_TO_RESET))
            return;
        if (pDuration == 0)
            pDuration = TIME_TO_SLOW_DOWN;

        if (pDuration < 0)
            UpdateCameraFieldOfView(mCameras, mDefaultFieldOfView, false);
        else
        {
            Camera tCamera = mCameras[0];
            float tTotalOffset = mDefaultFieldOfView - tCamera.fieldOfView;
            float tTmpFieldOfView = 0f;
            CSTimer.Instance.SetupCoolDown(TIMER_TO_RESET, pDuration, (pTime) =>
                {
                    if (null == tCamera)
                        return;
                    tTmpFieldOfView = mDefaultFieldOfView - tTotalOffset * pTime / pDuration;
                    UpdateCameraFieldOfView(mCameras, tTmpFieldOfView, false);
                }, ResetScaleDelay, 0.01f);
        }
    }
}