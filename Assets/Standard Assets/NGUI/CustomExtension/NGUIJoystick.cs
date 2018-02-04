using System;
using UnityEngine;

public class NGUIJoystick : MonoBehaviour
{
    private UIWidget _root;
    public UISprite bg;
    public UISprite thumb;

    private void Awake()
    {
        _root = GetComponent<UIWidget>();
        if (bg != null)
            bg.cachedTransform.localPosition = Vector3.zero;

        if (thumb != null)
            thumb.cachedTransform.localPosition = Vector3.zero;

        SetJoystickSize(_radius);
        Lighting(minAlpha);
    }

    private void OnDestory()
    {
        OnMoveStart = null;
        OnHolding = null;
        OnMoveEnd = null;
    }

    // Update is called once per frame   
    private void Update()
    {
        if (!IsDisable && IsHolding && IsPressed)
        {
            if (OnHolding != null)
            {
                OnHolding();
            }
        }
    }

    #region 激活、禁用的控制

    public void SetActive(bool active)
    {
        IsDisable = !active;
    }

    #endregion

    #region Delegate & Event

    /// <summary>
    ///     开如
    /// </summary>
    public Action OnMoveStart;

    /// <summary>
    ///     thumb偏离中心位置，并牌按住时，每帧的回调
    /// </summary>
    public Action OnHolding;

    /// <summary>
    ///     Occurs when the joystick stops move
    /// </summary>
    public Action OnMoveEnd;

    #endregion

    #region   Property

    [SerializeField] private bool isLimitRange = true;

    public bool IsLimitRange
    {
        get { return isLimitRange; }
    }

    private int _radius = 100;

    public int Radius
    {
        get { return _radius; }
        set { SetJoystickSize(value); }
    }

    [SerializeField] private float minAlpha = 0.3f;
    private Vector2 _lastJoystickAxis = Vector2.zero;
    private Vector2 _joystickAxis = Vector2.zero;

	//点击屏幕时候的定位，后面的遥感位移导致的方向改变根据这个做判断
	private Vector2 _checkJoystickAxis = Vector2.zero;

    public float MinAlpha
    {
        get { return minAlpha; }
    }

    /// <summary>
    ///     Gets the joystick axis value between -1 & 1...
    /// </summary>
    /// <value>
    ///     The joystick axis.
    /// </value>
    public Vector2 JoystickAxis
    {
        get { return _joystickAxis; }
    }

    public Vector3 Forward
    {
		get {
			Vector2 offVec =  (_joystickAxis-_checkJoystickAxis);
			return new Vector3(offVec.x, 0f, offVec.y).normalized; 
		}
    }

    public Vector2 LastJoystickAxis
    {
        get { return _lastJoystickAxis; }
    }

    /// <summary>
    ///     判断joystick是否被禁用
    /// </summary>
    public bool IsDisable { get; private set; }

    public bool IsHolding { get; private set; }

    public bool IsPressed { get; private set; }
    #endregion

    #region NGUI Event

    ///// <summary>
    ///// test
    ///// </summary>
    private void OnPress(bool isPressed)
    {
        IsPressed = isPressed;
        if (IsDisable)
        {
            IsHolding = false;
            return;
        }

        IsHolding = false;
        var offset = ScreenPos_to_NGUIPos(UICamera.currentTouch.pos);

		if (thumb != null)
			thumb.cachedTransform.localPosition = offset;

		_checkJoystickAxis = new Vector2(offset.x, offset.y);
    }

    private void OnDragStart()
    {
        if (IsDisable)
        {
            IsHolding = false;
            return;
        }

        Lighting(1f);
        CalculateJoystickAxis();
        IsHolding = true;

        if (OnMoveStart != null)
        {
            OnMoveStart();
        }
    }

    private void OnDragEnd()
    {
        if (IsDisable)
        {
            IsHolding = false;
            return;
        }

        CalculateJoystickAxis();
        FadeOut(1f, minAlpha);
        IsHolding = false;
        //if (thumb != null)
          //  thumb.cachedTransform.localPosition = Vector3.zero;

        if (OnMoveEnd != null)
        {
            OnMoveEnd();
        }
    }

    private void OnDrag(Vector2 delta)
    {
        if (IsDisable)
        {
            IsHolding = false;
            return;
        }

        CalculateJoystickAxis();
    }

    void OnApplicationPause(bool paused)
    {
        IsHolding = false;
    }

    #endregion

    #region Helper

    /// <summary>
    ///     计算JoystickAxis
    /// </summary>
    /// <returns></returns>
    private void CalculateJoystickAxis()
    {
        var offset = ScreenPos_to_NGUIPos(UICamera.currentTouch.pos);
        offset -= _root.cachedTransform.localPosition;
        if (isLimitRange)
        {
            if (offset.sqrMagnitude > _radius*_radius)
            {
                offset = offset.normalized*_radius;
            }
        }

        if (thumb != null)
            thumb.cachedTransform.localPosition = offset;

        _lastJoystickAxis = _joystickAxis;
        _joystickAxis = new Vector2(offset.x, offset.y);
    }

    /// <summary>
    ///     屏幕坐标-->ui坐标
    /// </summary>
    /// <param name="screenPos"></param>
    /// <returns></returns>
    private Vector3 ScreenPos_to_NGUIPos(Vector3 screenPos)
    {
        var uiPos = UICamera.currentCamera.ScreenToWorldPoint(screenPos);
        uiPos = UICamera.currentCamera.transform.InverseTransformPoint(uiPos);
        return uiPos;
    }

    /// <summary>
    ///     设置摇杆的大小
    /// </summary>
    /// <param name="radius"></param>
    private void SetJoystickSize(int radius)
    {
        _radius = radius;
        if (bg != null && thumb != null)
        {
            bg.width = 2*radius;
            bg.height = 2*radius;

            thumb.width = (int) (0.4f*bg.width);
            thumb.height = (int) (0.4f*bg.height);
        }
    }

    /// <summary>
    ///     点亮摇杆
    /// </summary>
    private void Lighting(float alpha)
    {
        _root.alpha = alpha;
    }

    /// <summary>
    ///     渐变摇杆的透明度
    /// </summary>
    private void FadeOut(float fromAlpha, float toAlpha)
    {
        if (fromAlpha == toAlpha) return;
        ;
        TweenAlpha.Begin(_root.cachedGameObject, 0.5f, toAlpha);
    }

    #endregion
}