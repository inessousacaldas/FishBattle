//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Simple example script of how a button can be scaled visibly when the mouse hovers over it or it gets pressed.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button Scale")]
public class UIButtonScale : MonoBehaviour
{
	public Transform tweenTarget;
	public Vector3 hover = new Vector3(1.1f, 1.1f, 1.1f);
	public Vector3 pressed = new Vector3(1.05f, 1.05f, 1.05f);
	public float duration = 0.2f;

	Vector3 mScale;
	bool mStarted = false;
    BoxCollider _bc;
    Vector3 orginSize;
    private TweenScale _tweenScale;
    private bool _isPress;
    //private UIWidget _widget;

	void Start ()
	{
		if (!mStarted)
		{
			mStarted = true;
			if (tweenTarget == null) tweenTarget = transform;
            //_widget = tweenTarget.GetComponent<UIWidget>();
			mScale = tweenTarget.localScale;
            _bc = tweenTarget.GetComponent<BoxCollider>();
            if (_bc != null)
            {
                orginSize = _bc.size;
		}
	}
    }

    //void OnEnable() { if (mStarted) OnHover(UICamera.IsHighlighted(gameObject)); }

	void OnDisable ()
	{
		if (mStarted && tweenTarget != null)
		{
            if (_bc != null)
            {
                _bc.size = orginSize;
            }

            if (_tweenScale != null)
			{
                _tweenScale.value = mScale;
                _tweenScale.enabled = false;
			}
		}
	}

	void OnPress (bool isPressed)
	{
        _isPress = isPressed;
		if (enabled)
		{
			if (!mStarted) Start();

            //if (_widget != null)
            //{
            //    _widget.autoResizeBoxCollider = false;
            //}

            ////这里修改为按下时候，把碰撞盒放大，避免因为按钮缩小导致的释放时触发不到onClick事件
            _tweenScale = TweenScale.Begin(tweenTarget.gameObject, duration,
                    isPressed
                    ? Vector3.Scale(mScale, pressed)
                    : (UICamera.IsHighlighted(gameObject) ? Vector3.Scale(mScale, hover) : mScale));
            _tweenScale.method = UITweener.Method.EaseInOut;

            if (isPressed)
	{
                _tweenScale.SetOnFinished(() =>
		{
                    if (_bc != null)
                    {
                        _bc.size = _isPress
                            ? Vector3.Scale(orginSize, new Vector3(1f / pressed.x, 1f / pressed.y, 1f / pressed.z))
                            : orginSize;
		}

                    //if (_widget != null)
                    //{
                    //    _widget.autoResizeBoxCollider = true;
                    //}
                });
            }
            else
	{
                if (_bc != null) _bc.size = orginSize;
	}
}
    }

    //void OnHover(bool isOver)
    //{
    //    _isOver = isOver;
    //    if (enabled)
    //    {
    //        if (!mStarted) Start();
    //        UIWidget widget = tweenTarget.gameObject.GetComponent<UIWidget>();
    //        if (widget != null)
    //        {
    //            widget.autoResizeBoxCollider = true;
    //        }

    //        _tweenScale = TweenScale.Begin(tweenTarget.gameObject, duration,
    //            isOver ? Vector3.Scale(mScale, hover) : mScale);
    //        _tweenScale.method = UITweener.Method.EaseInOut;
    //        _tweenScale.SetOnFinished(() =>
    //        {
    //            if (_isOver)
    //            {
    //                if (bc != null)
    //                {
    //                    bc.size = Vector3.Scale(orginSize, new Vector3(1f / pressed.x, 1f / pressed.y, 1f / pressed.z));
    //                }
    //            }
    //        });
    //    }
    //    else
    //    {
    //        //鼠标移动上去，还原碰撞盒大小
    //        if (bc != null)
    //        {
    //            bc.size = orginSize;
    //        }
    //    }
    //}

    //void OnSelect(bool isSelected)
    //{
    //    if (enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
    //        OnHover(isSelected);
    //}
}
