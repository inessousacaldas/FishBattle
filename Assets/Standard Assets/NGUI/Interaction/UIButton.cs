//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using Assets.Standard_Assets.NGUI.CustomExtension.Manager;

/// <summary>
/// Similar to UIButtonColor, but adds a 'disabled' state based on whether the collider is enabled or not.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button")]
public class UIButton : UIButtonColor
{
	/// <summary>
	/// Current button that sent out the onClick event.
	/// </summary>

	static public UIButton current;

	/// <summary>
	/// Whether the button will highlight when you drag something over it.
	/// </summary>

	public bool dragHighlight = false;

	/// <summary>
	/// Name of the hover state sprite.
	/// </summary>

	public string hoverSprite;

	/// <summary>
	/// Name of the pressed sprite.
	/// </summary>

	public string pressedSprite;

	/// <summary>
	/// Name of the disabled sprite.
	/// </summary>

	public string disabledSprite;

	/// <summary>
	/// Name of the hover state sprite.
	/// </summary>

	public UnityEngine.Sprite hoverSprite2D;

	/// <summary>
	/// Name of the pressed sprite.
	/// </summary>

	public UnityEngine.Sprite pressedSprite2D;

	/// <summary>
	/// Name of the disabled sprite.
	/// </summary>

	public UnityEngine.Sprite disabledSprite2D;

	/// <summary>
	/// Whether the sprite changes will elicit a call to MakePixelPerfect() or not.
	/// </summary>

	public bool pixelSnap = false;

	/// <summary>
	/// Click event listener.
	/// </summary>

	public List<EventDelegate> onClick = new List<EventDelegate>();

	// Cached value
	[System.NonSerialized] UISprite mSprite;
	[System.NonSerialized] UI2DSprite mSprite2D;
	[System.NonSerialized] string mNormalSprite;
	[System.NonSerialized] UnityEngine.Sprite mNormalSprite2D;

    #region 新增一个访问按钮的文字接口
    public UILabel Label;

    public string Text
    {
        set { if(Label != null) Label.text = value; }
        get { return Label != null ? Label.text : "";}
    }
    #endregion

    #region 冯明达新增按钮一些事件：快速点击间隔、双击、按下、放开
    public const float CLICK_GAP = 0.18f;
    public float clickGap = CLICK_GAP;
    private bool clickTiming = false;

    private UIEventListener.VoidDelegate _clickFun;
    private UIEventListener.VoidDelegate _doubleClickFun;
    private UIEventListener.VoidDelegate _mouseDownFun;
    private UIEventListener.VoidDelegate _mouseUpFun;
    #endregion
    /// <summary>
    /// Whether the button should be enabled.
    /// </summary>

    public override bool isEnabled
	{
		get
		{
			if (!enabled) return false;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			Collider col = collider;
#else
			Collider col = gameObject.GetComponent<Collider>();
#endif
			if (col && col.enabled) return true;
			Collider2D c2d = GetComponent<Collider2D>();
			return (c2d && c2d.enabled);
		}
		set
		{
			if (isEnabled != value)
			{
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
				Collider col = collider;
#else
				Collider col = gameObject.GetComponent<Collider>();
#endif
				if (col != null)
				{
					col.enabled = value;
					UIButton[] buttons = GetComponents<UIButton>();
					foreach (UIButton btn in buttons) btn.SetState(value ? State.Normal : State.Disabled, false);
				}
				else
				{
					Collider2D c2d = GetComponent<Collider2D>();

					if (c2d != null)
					{
						c2d.enabled = value;
						UIButton[] buttons = GetComponents<UIButton>();
						foreach (UIButton btn in buttons) btn.SetState(value ? State.Normal : State.Disabled, false);
					}
					else enabled = value;
				}
			}
		}
	}

	/// <summary>
	/// Convenience function that changes the normal sprite.
	/// </summary>

	public string normalSprite
	{
		get
		{
			if (!mInitDone) OnInit();
			return mNormalSprite;
		}
		set
		{
			if (!mInitDone) OnInit();
			if (mSprite != null && !string.IsNullOrEmpty(mNormalSprite) && mNormalSprite == mSprite.spriteName)
			{
				mNormalSprite = value;
				SetSprite(value);
				NGUITools.SetDirty(mSprite);
			}
			else
			{
				mNormalSprite = value;
				if (mState == State.Normal) SetSprite(value);
			}
		}
	}

	/// <summary>
	/// Convenience function that changes the normal sprite.
	/// </summary>

	public UISprite sprite
	{
		get
		{
			if (!mInitDone) OnInit();
			return mSprite;
		}
	}

	/// <summary>
	/// Convenience function that changes the normal sprite.
	/// </summary>

	public UnityEngine.Sprite normalSprite2D
	{
		get
		{
			if (!mInitDone) OnInit();
			return mNormalSprite2D;
		}
		set
		{
			if (!mInitDone) OnInit();
			if (mSprite2D != null && mNormalSprite2D == mSprite2D.sprite2D)
			{
				mNormalSprite2D = value;
				SetSprite(value);
				NGUITools.SetDirty(mSprite);
			}
			else
			{
				mNormalSprite2D = value;
				if (mState == State.Normal) SetSprite(value);
			}
		}
	}
	/// <summary>
	/// Cache the sprite we'll be working with.
	/// </summary>

	protected override void OnInit ()
	{
		base.OnInit();
		mSprite = (mWidget as UISprite);
		mSprite2D = (mWidget as UI2DSprite);
		if (mSprite != null) mNormalSprite = mSprite.spriteName;
		if (mSprite2D != null) mNormalSprite2D = mSprite2D.sprite2D;

		//modify star by senkay at 2015-08-17
		UIButtonScale uiButtonScale = this.GetComponent<UIButtonScale>();
		if (uiButtonScale != null)
		{
			uiButtonScale.hover = new Vector3(1f,1f,1f);
			uiButtonScale.pressed = new Vector3(0.9f,0.9f,0.9f);
			uiButtonScale.duration = 0.1f;
	}
	}

	/// <summary>
	/// Set the initial state.
	/// </summary>

	protected override void OnEnable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			mInitDone = false;
			return;
		}
#endif
		if (isEnabled)
		{
			if (mInitDone) OnHover(UICamera.hoveredObject == gameObject);
		}
		else SetState(State.Disabled, true);
	}

	/// <summary>
	/// Drag over state logic is a bit different for the button.
	/// </summary>

	protected override void OnDragOver ()
	{
		if (isEnabled && (dragHighlight || UICamera.currentTouch.pressed == gameObject))
			base.OnDragOver();
	}

	/// <summary>
	/// Drag out state logic is a bit different for the button.
	/// </summary>

	protected override void OnDragOut ()
	{
		if (isEnabled && (dragHighlight || UICamera.currentTouch.pressed == gameObject))
			base.OnDragOut();
	}

	/// <summary>
	/// Call the listener function.
	/// </summary>

	protected virtual void OnBaseClick ()
	{
		if (current == null && isEnabled && UICamera.currentTouchID != -2 && UICamera.currentTouchID != -3)
		{
			current = this;
			EventDelegate.Execute(onClick);
			current = null;
		}
	}

	/// <summary>
	/// Change the visual state.
	/// </summary>

	public override void SetState (State state, bool immediate)
	{
		base.SetState(state, immediate);

		if (mSprite != null)
		{
			switch (state)
			{
				case State.Normal: SetSprite(mNormalSprite); break;
				case State.Hover: SetSprite(string.IsNullOrEmpty(hoverSprite) ? mNormalSprite : hoverSprite); break;
				case State.Pressed: SetSprite(pressedSprite); break;
				case State.Disabled: SetSprite(disabledSprite); break;
			}
		}
		else if (mSprite2D != null)
		{
			switch (state)
			{
				case State.Normal: SetSprite(mNormalSprite2D); break;
				case State.Hover: SetSprite(hoverSprite2D == null ? mNormalSprite2D : hoverSprite2D); break;
				case State.Pressed: SetSprite(pressedSprite2D); break;
				case State.Disabled: SetSprite(disabledSprite2D); break;
			}
		}
	}

	/// <summary>
	/// Convenience function that changes the sprite.
	/// </summary>

	protected void SetSprite (string sp)
	{
		if (mSprite != null && !string.IsNullOrEmpty(sp) && mSprite.spriteName != sp)
		{
			mSprite.spriteName = sp;
			if (pixelSnap) mSprite.MakePixelPerfect();
		}
	}

	/// <summary>
	/// Convenience function that changes the sprite.
	/// </summary>

	protected void SetSprite (UnityEngine.Sprite sp)
	{
		if (sp != null && mSprite2D != null && mSprite2D.sprite2D != sp)
		{
			mSprite2D.sprite2D = sp;
			if (pixelSnap) mSprite2D.MakePixelPerfect();
		}
	}

    #region 冯明达新增按钮一些事件：快速点击间隔、双击、按下、放开

    protected override void OnPress(bool isPressed)
    {
        if(isEnabled == false)
        {
            return;
        }
        base.OnPress(isPressed);
        if(isPressed == true)
        {
            if(_mouseDownFun != null)
            {
                _mouseDownFun(gameObject);
            }
        }
        else
        {
            if(_mouseUpFun != null)
            {
                _mouseUpFun(gameObject);
            }
        }
    }

    protected virtual void OnClick()
    {
        if(clickTiming == false)
        {
            clickTiming = true;
            UILoopManager.SetTimeout(OnClickTimeOut,clickGap);
            if(isEnabled == false)
            {
                return;
            }
            OnBaseClick();
            if(_clickFun != null)
            {
                _clickFun(gameObject);
            }
        }
    }

    private void OnClickTimeOut()
    {
        clickTiming = false;
    }

    protected void OnDoubleClick()
    {
        if(isEnabled == false)
        {
            return;
        }
        if(_doubleClickFun != null)
        {
            _doubleClickFun(gameObject);
        }
    }

    public void AddClick(UIEventListener.VoidDelegate fun)
    {
        if(_clickFun == null)
        {
            _clickFun = fun;
        }
        else
        {
            Debug.LogError("CButton only support one function");
        }
    }

    public void RemoveClick()
    {
        _clickFun = null;
    }

    public void AddDoubleClick(UIEventListener.VoidDelegate fun)
    {
        if(_doubleClickFun == null)
        {
            _doubleClickFun = fun;
        }
        else
        {
            Debug.LogError("CButton only support one function");
        }
    }

    public void RemoveDoubleClick()
    {
        _doubleClickFun = null;
    }

    public void AddMouseDown(UIEventListener.VoidDelegate fun)
    {
        if(_mouseDownFun == null)
        {
            _mouseDownFun = fun;
        }
        else
        {
            Debug.LogError("CButton only support one function");
        }
    }

    public void RemoveMosueDown()
    {
        _mouseDownFun = null;
    }

    public void AddMouseUp(UIEventListener.VoidDelegate fun)
    {
        if(_mouseUpFun == null)
        {
            _mouseUpFun = fun;
        }
        else
        {
            Debug.LogError("CButton only support one function");
        }
    }

    public void RemoveMosueUp()
    {
        _mouseUpFun = null;
    }

    #endregion

    void OnDestroy()
	{
		onClick.Clear();
	}
}
