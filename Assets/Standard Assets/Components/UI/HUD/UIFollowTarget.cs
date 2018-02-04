//--------------------------------------------
//            NGUI: HUD Text
// Copyright (c) 2012 Tasharen Entertainment
//--------------------------------------------

using System;
using UnityEngine;

/// <summary>
/// Attaching this script to an object will make it visibly follow another object, even if the two are using different cameras to draw them.
/// </summary>

[AddComponentMenu("NGUI/Examples/Follow Target")]
public class UIFollowTarget : MonoBehaviour
{
#if UNITY_EDITOR
    [Obsolete("用于避开JSB导出")]
    public static event Func<Camera, Vector3, Vector3> GameCameraRectFix;
#endif


    /// <summary>
    /// 3D target that this object will be positioned above.
    /// </summary>

    public Transform target;
	public Vector3 offset;
	
	/// <summary>
	/// Game camera to use.
	/// </summary>
	
	public Camera gameCamera;
	
	/// <summary>
	/// UI camera to use.
	/// </summary>
	
	public Camera uiCamera;
	
	/// <summary>
	/// Whether the children will be disabled when this object is no longer visible.
	/// </summary>
	
	public bool disableIfInvisible = true;

    /// <summary>
    /// 一直显示所有子节点
    /// </summary>
    private bool _alwaysVisible = false;
    public bool AlwaysVisible
    {
        get { return _alwaysVisible; }
        set { _alwaysVisible = value; }
    }

    /// <summary>
	/// Cache the transform;
	/// </summary>
	Transform mTrans;
    bool mIsVisible = false;

    void Awake () { mTrans = transform; }
	
	/// <summary>
	/// Find both the UI camera and the game camera so they can be used for the position calculations
	/// </summary>
	
	void Start()
	{
		if (target != null)
		{
			if (gameCamera == null) gameCamera = NGUITools.FindCameraForLayer(target.gameObject.layer);
			if (uiCamera == null) uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
			SetVisible(false);
		}
		else
		{
			Debug.LogError("Expected to have 'target' set to a valid transform", this);
			enabled = false;
		}
	}
	
	/// <summary>
	/// Enable or disable child objects.
	/// </summary>
	
	void SetVisible (bool val)
	{
		mIsVisible = val;
		
        if(_alwaysVisible)
            return;

		for (int i = 0, imax = mTrans.childCount; i < imax; ++i)
		{
			NGUITools.SetActive(mTrans.GetChild(i).gameObject, val);
		}
	}
	
	/// <summary>
	/// Update the position of the HUD object every frame such that is position correctly over top of its real world object.
	/// </summary>
	
	void LateUpdate ()
	{
		if( target == null )
		{
			return;
		}

		Vector3 pos = gameCamera.WorldToViewportPoint(target.position + offset);
		
		// Determine the visibility and the target alpha
	    bool isVisible = false;
	    if (_alwaysVisible)
	    {
	        isVisible = true;
	    }
	    else
	    {
	        isVisible = (gameCamera.orthographic || pos.z > 0f) &&
	                    (!disableIfInvisible || (pos.x > 0f && pos.x < 1f && pos.y > 0f && pos.y < 1f));
	    }

	    // Update the visibility flag
		if (mIsVisible != isVisible) SetVisible(isVisible);
		
		// If visible, update the position
		if (isVisible)
		{
#if UNITY_EDITOR
            if (GameCameraRectFix != null)
            {
                pos = GameCameraRectFix(gameCamera, pos);
            }
#endif

            mTrans.position = uiCamera.ViewportToWorldPoint(pos);
			pos = mTrans.localPosition;
//			pos.x = Mathf.FloorToInt(pos.x);
//			pos.y = Mathf.FloorToInt(pos.y);
			pos.z = 0f;
			mTrans.localPosition = pos;
		}
		OnUpdate(isVisible);
	}
	
	/// <summary>
	/// Custom update function.
	/// </summary>
	
	protected virtual void OnUpdate (bool isVisible) { }
}