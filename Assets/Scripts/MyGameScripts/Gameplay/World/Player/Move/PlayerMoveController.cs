using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using AppDto;
using System;

public class PlayerMoveController
{
    private Transform _mTrans;
    private PlayerView _player;
    private bool _isHero;

    private GridMapAgent _gridMapAgent;
    private float _moveSpeed;

    public bool enabled
    {
        set
        {
            if (_gridMapAgent != null)
                _gridMapAgent.enabled = value;
        }

        get
        {
            if (_gridMapAgent != null)
                return _gridMapAgent.enabled;
            return false;
        }
    }

    public float radius
    {
        get
        {
            return 0f;
        }
    }

    public void Setup(PlayerView player, bool isHero, GameObject mGo, GameObject mModelGo, Transform mTrans)
    {
        _mTrans = mTrans;
        _player = player;
        _isHero = isHero;
        _moveSpeed = player.GetPlayerDto().moveSpeed;

        if (_gridMapAgent == null)
        {
            _gridMapAgent = mGo.GetMissingComponent<GridMapAgent>();
            //_gridMapAgent.SetOnPosChangeEvent(OnPosChange);   //由于涉及每一帧从框架到业务发送事件，性能消耗比较大。占时没有相关需求，先屏蔽。
            _gridMapAgent.canSearch = true;
            _gridMapAgent.canMove = true;
            _gridMapAgent.speed = _moveSpeed;
            _gridMapAgent.rotationSpeed = 10;
            _gridMapAgent.interpolatePathSwitches = false;
        }
        else
        {
            _gridMapAgent.enabled = true;
        } 
    }

    public Vector3[] GetWalkPathList()
    {
        return _gridMapAgent.GetVectorPath().ToArray();
    }

    public Vector3 GetNavDestination()
    {
        return Vector3.zero;
    }

    public void ResetPath()
    {
        if (_gridMapAgent != null)
        {
            _gridMapAgent.ReleasePath();
        }
    }

    public void SetDestination(Vector3 targetPoint)
    {
        _gridMapAgent.SearchPath(targetPoint);
    }

    public Vector3 GetGridPos()
    {
        return _gridMapAgent != null ? _gridMapAgent.gridPos : Vector3.zero;
    }
    public bool IsActiveAndEnabled()
    {
        return _gridMapAgent != null && _gridMapAgent.isActiveAndEnabled;
    }

    public bool HasPath()
    {
        return _gridMapAgent.HasPath;
    }

    public bool InMoving()
    {
        return _gridMapAgent != null &&  _gridMapAgent.isMoving;
    }

    public void ResetPos(Vector3 dest)
    {

        Vector3 tOriginPos = _mTrans.position;
        _mTrans.position = dest;
        //OnPosChange(dest, tOriginPos, true);  //由于涉及每一帧从框架到业务发送事件，性能消耗比较大。占时没有相关需求，先屏蔽。

    }

    public void UpdatePlayerMoveSpeed(float moveSpeed)
    {
        _moveSpeed = moveSpeed;
 
        if (_gridMapAgent != null)
        {
            _gridMapAgent.speed = _moveSpeed;
        }
    }
#region 组队处理

#endregion

    public float GetSpeed()
    {
        return _moveSpeed;
    }
    //由于涉及每一帧从框架到业务发送事件，性能消耗比较大。占时没有相关需求，先屏蔽。
    //public event Action<Vector3, Vector3, bool> mPosChangeAction;     
    //public void OnPosChange(Vector3 pos, Vector3 pPrePos, bool pTargetReached)
    //{

    //    if(mPosChangeAction != null)
    //        mPosChangeAction(pos, pPrePos, pTargetReached);
    //}


    private readonly Collider2D[] _gridMapBuild = new Collider2D[1];


    /// <summary>
    /// 场景遮罩初始化时需要设置玩家所在遮罩区域的碰撞体
    /// </summary>
    /// <param name="trigger"></param>
    public void SetupBuildTrigger(Collider2D trigger)
    {
        _gridMapBuild[0] = trigger;
    }

    public void Destroy()
    {
        if(_gridMapAgent != null)
            _gridMapAgent.Clear();
        _gridMapAgent = null;
        _mTrans = null;
        _player = null;
    }
}