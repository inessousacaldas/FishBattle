// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  HeroView.cs
// Author   : willson
// Created  : 2014/12/3 
// Porpuse  : 
// **********************************************************************

using UnityEngine;

public class HeroView : PlayerView
{
    #region TriggerEvent
    private GameObject _triggerGo;
    private void OnTriggerEnter(Collider other)
    {
        if (JoystickModule.DisableMove)
        {
            GameDebuger.TODO(@"if (ModelManager.BridalSedan.IsMe())
            {
                TipManager.AddTip('你正在乘坐花轿，不能到处乱跑哦！');
            }");
            return;
        }

        var triggerGo = other.gameObject;
        if (other.CompareTag(GameTag.Tag_Teleport))
        {
            if (_triggerGo != triggerGo)
            {
                //Debug.LogError(string.Format("OnTriggerEnter:{0}",Time.frameCount));
                WorldManager.Instance.GetNpcViewManager().TriggerTeleport(triggerGo);
            }
        }
        else if (other.CompareTag(GameTag.Tag_DreamlandNpc))
        {
            WorldManager.Instance.GetNpcViewManager().TriggerTeleport(triggerGo);
        }
        _triggerGo = triggerGo;
    }

    private void OnTriggerExit(Collider other)
    {
        if (JoystickModule.DisableMove)
            return;

        var triggerGo = other.gameObject;
        if (other.CompareTag(GameTag.Tag_Teleport))
        {
            //Debug.LogError(string.Format("OnTriggerExit:{0}",Time.frameCount));
            BaseNpcUnit npcUnit = WorldManager.Instance.GetNpcViewManager().GetNpcUnit(triggerGo);
            DoubleTeleportUnit teleportUnit = npcUnit as DoubleTeleportUnit;
            if (teleportUnit != null)
            {
                teleportUnit.StopTrigger();
            }
            NpcSceneTeleportUnit npcSceneTeleportUnit = npcUnit as NpcSceneTeleportUnit;
            if (npcSceneTeleportUnit != null)
            {
                npcSceneTeleportUnit.StopTrigger();
            }

            _triggerGo = null;
        }
    }
    #endregion
    protected override void Update()
    {
        UpdateTowerFly();
        base.Update();
        // todo fish: cp from D1, not sure this problem exist in S3
        //        var pos = GetFeetPosition();
        //        WorldManager.Instance.UpdateHeroPos(pos.x, pos.y, pos.z);
        //
        //        // 组队位置偏移问题
        //        if (_prePos != pos)
        //        {
        //            _prePos = pos;
        //            _mPlayerMoveController.OnPosChange(pos, _prePos, true);
        //
        //        }

        if (IsAutoFram)
        {
            _autoFramTimer += Time.deltaTime;
            if (_autoFramTimer > _coolDown)
            {
                AutoWalk();
                _autoFramTimer = 0f;
                _coolDown = Random.Range(2f, 10f);
                return;
            }

            if (!IsRunning())
            {
                AutoWalk();
            }
        }

        _verifyWalkTimer += Time.deltaTime;
        if (_verifyWalkTimer >= VerifyWalkInterval)
        {
            _verifyWalkTimer = 0f;
            if (ValidateHeroPos())
            {
                WorldManager.Instance.VerifyWalk(_lastX, _lastY, _lastZ);
            }
        }

        if (_walkWithJoystick)
        {
            _planWalkTimer += Time.deltaTime;
            if (_planWalkTimer >= PlanWalkInterval)
            {
                _planWalkTimer = 0f;
                if (ValidateHeroPos())
                {
                    WorldManager.Instance.PlanWalk(_lastX, _lastY, _lastZ);
                }
            }
        }
    }

    private void UpdateTowerFly()
    {
        if (MySceneManager.Instance.currentSceneName == "Scene_10001")
        {
            if (Input.GetKeyDown(KeyCode.O) && Input.GetKey(KeyCode.I))
            {
                Vector3 upPoin = new Vector3(0.046f, 8.465f, 0.112f);
                Vector3 downPoin = new Vector3(-0.1f, 0.25f, 2.88f);
                if (Vector3.Distance(upPoin, cachedTransform.position) < 2f) //判断是不在塔顶
                {
                    ResetPos(downPoin);
                }
                else
                {
                    ResetPos(upPoin);
                }
            }
        }
    }

    public void WalkWithJoystick(Vector3 forward)
    {
        if (_walkWithJoystick)
        {
            var heroTrans = cachedTransform;
            heroTrans.rotation = Quaternion.LookRotation(forward);
            heroTrans.Translate(forward * Speed * Time.deltaTime, Space.World);
            PlayRunAnimation();
        }
    }

    #region TestFunc

    public void MutateTest(string colorParams)
    {
        if (!string.IsNullOrEmpty(colorParams))
        {
            _modelDisplayer.UpdateModelHSV(colorParams, 0);
        }
        else
        {
            _modelDisplayer.UpdateModelHSV("", 0);
        }
    }

    /// <summary>
    ///添加一个播放主角动画的接口
    /// </summary>
    /// <param name="action">动画名称（枚举）</param>
    /// <param name="crossFade">是否淡出淡入</param>
    /// <param name="checkSameAnim">检查相同动画</param>
    public void PlayAnimation(ModelHelper.AnimType action,bool crossFade = false,bool checkSameAnim = false) {
        _modelDisplayer.PlayAnimation(action,crossFade,checkSameAnim);
        _isRunning = false;
    }

    #endregion

    #region 自动寻路

    private float _autoFramTimer;
    private float _coolDown;
    public bool IsAutoFram { get; private set; }

    public void SetAutoFram(bool b)
    {
        if (IsAutoFram != b)
        {
            IsAutoFram = b;
            if (IsAutoFram)
            {
                _coolDown = Random.Range(2f, 10f);
            }
        }
    }

    private void AutoWalk()
    {
        var walkPoint = WorldManager.Instance.GetView().GetRandomNavPoint();
        WorldManager.Instance.PlanWalk(walkPoint.x, walkPoint.y, walkPoint.z);
        _mAgent.enabled = true;
        _mAgent.SetDestination(walkPoint);
    }

    #endregion

    #region 玩家位置检验

    private const float PlanWalkInterval = 3.0f;
    private const float VerifyWalkInterval = 1.0f;

    private float _lastX;
    private float _lastY;
    private float _lastZ;
    private float _planWalkTimer;
    private float _verifyWalkTimer;

    /// <summary>
    ///     验证玩家位置是否有改变
    /// </summary>
    /// <returns></returns>
    private bool ValidateHeroPos()
    {
        var newX = _mTrans.position.x;
        var newY = _mTrans.position.y;
        var newZ = _mTrans.position.z;
        if (!(Mathf.Abs(_lastX - newX) > 0.1f) && !(Mathf.Abs(_lastZ - newZ) > 0.1f) &&
            !(Mathf.Abs(_lastY - newY) > 0.1f)) return false;
        _lastX = newX;
        _lastY = newY;
        _lastZ = newZ;
        return true;
    }

    /// <summary>
    ///     立即同步验证玩家位置
    /// </summary>
    public void SyncWithServer()
    {
        if (ValidateHeroPos())
        {
            WorldManager.Instance.VerifyWalk(_lastX, _lastY, _lastZ);
        }
    }

    #endregion

    public string GetPosStr()
    {
        return string.Format("x={0}&y={1}&z={2}",_mTrans.position.x, _mTrans.position.y, _mTrans.position.z);
    }
}