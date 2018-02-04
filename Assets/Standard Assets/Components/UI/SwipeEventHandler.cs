using UnityEngine;
using System.Collections;

public class SwipeEventHandler : MonoBehaviour 

{
	//Swipe方向.
	public enum SwipeDirection
	{
		/// <summary>
        /// Moved to the right
        /// </summary>
        Right = 1 << 0,

        /// <summary>
        /// Moved to the left
        /// </summary>
        Left = 1 << 1,

        /// <summary>
        /// Moved up
        /// </summary>
        Up = 1 << 2,

        /// <summary>
        /// Moved down
        /// </summary>
        Down = 1 << 3,

        //--------------------

        None = 0,
        All = Right | Left | Up | Down,
        Vertical = Up | Down,
        Horizontal = Right | Left,
	}

	public GameObject _target;
	public string	  _methodName = "OnSwipe";
	public SwipeDirection _validDirections; //需要检测的方向.//
	
	/// <summary>
    /// Minimum swipe distance
    /// </summary>
    public float _minDistance = 1.0f;

    /// <summary>
    /// Minimum swipe velocity
    /// </summary>
    public float _minVelocity = 1.0f;

	/// <summary>
    /// Amount of tolerance when determining if the finger motion was performed along one of the supported swipe directions.
    /// This amount should be kept between 0 and 0.5f, where 0 means no tolerance and 0.5f means you can move within 45 degrees away from the allowed direction
    /// </summary>
    public float _directionTolerance = 0.2f; //DOT
	
	
	public delegate void OnSwipe(SwipeDirection direction_);
	public OnSwipe onSwipe;

	//当前事件的信息.
    //Vector2 _startPosition;   //GET WARNING
    //Vector2 _position;        //GET WARNING
    //Vector2 _delta;           //GET WARNING
    SwipeDirection _direction = SwipeDirection.None;
    float _velocity = 0;
    float _startTime = 0;
	bool  _isFailed = false;
	
	
	void OnPress( bool pressed )
	{
		if ( !pressed ) //弹起时应该检测是否成功才对.
		{
			
			if ( !_isFailed )
			{
				
				if( _direction != SwipeDirection.None )
                {
                    if ( _target != null && !string.IsNullOrEmpty(_methodName) )
					{
						_target.SendMessage(_methodName,_direction);
					}
					
					if (onSwipe != null)
					{
						onSwipe(_direction);
					}
					
                }
				
			}
			
			
			
			_isFailed = false;
			return;
		}
		
		//_position = UICamera.currentTouch.pos;
        //_startPosition = _position;
       	_direction = SwipeDirection.None;
		_startTime = Time.realtimeSinceStartup;
		_isFailed = false;
	}
	
	
	void OnDrag(Vector2 delta)
	{
		//如果已经失败,则不计算.
		if ( _isFailed )
			return;
		
		//_position = UICamera.currentTouch.pos;
		
        //_delta = _position - _startPosition;  //GET WARNING
		
		float distance = delta.magnitude;
		
		if ( distance < _minDistance )
		{
			return;
		}
		
		float elapsedTime = Time.realtimeSinceStartup - _startTime;
		
		if ( elapsedTime > 0 )
			_velocity = distance / elapsedTime;
		else
			_velocity = 0;
		
		// we're going too slow
		if( _velocity < _minVelocity )
		{
			_isFailed = true;
			return;
		}
		
		SwipeDirection newDirection = GetSwipeDirection( delta.normalized, _directionTolerance );
		
		// we went in a bad direction
        if( !IsValidDirection( newDirection ) || ( _direction != SwipeDirection.None && newDirection != _direction ) )
		{
			_isFailed = true;
            return;
		}
		
		
		_direction = newDirection;
	}
	
	
	public SwipeDirection GetSwipeDirection( Vector3 dir, float tolerance )
    {
        float minSwipeDot = Mathf.Clamp01( 1.0f - tolerance );

        if( Vector2.Dot( dir, Vector2.right ) >= minSwipeDot )
            return SwipeDirection.Right;

        if( Vector2.Dot( dir, -Vector2.right ) >= minSwipeDot )
            return SwipeDirection.Left;

        if( Vector2.Dot( dir, Vector2.up ) >= minSwipeDot )
            return SwipeDirection.Up;

        if( Vector2.Dot( dir, -Vector2.up ) >= minSwipeDot )
            return SwipeDirection.Down;

        // not a valid direction
        return SwipeDirection.None;
    }
	
	public bool IsValidDirection( SwipeDirection dir )
    {
        if( dir == SwipeDirection.None )
            return false;

        return ( ( _validDirections & dir ) == dir );
    }
	
}
