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
        base.Update();
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
                WorldManager.Instance.VerifyWalk(_lastX, _lastZ);
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
                    WorldManager.Instance.PlanWalk(_lastX, _lastZ);
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
            Vector3 curPos = heroTrans.position;
            heroTrans.position = SceneHelper.GetSceneStandPosition(curPos, curPos);
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

    public void TextPlayAnimation(ModelHelper.AnimType action)
    {
        _modelDisplayer.PlayAnimation(action);
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
        WorldManager.Instance.PlanWalk(walkPoint.x, walkPoint.z);
        _mAgent.enabled = true;
        _mAgent.SetDestination(walkPoint);
    }

    #endregion

    #region 玩家位置检验

    private const float PlanWalkInterval = 3.0f;
    private const float VerifyWalkInterval = 1.0f;

    private float _lastX;
    private float _lastZ;
    private float _planWalkTimer;
    private float _verifyWalkTimer;

    /// <summary>
    ///     验证玩家位置是否有改变
    /// </summary>
    /// <returns></returns>
    private bool ValidateHeroPos()
    {
        float newX = _mTrans.position.x;
        float newZ = _mTrans.position.z;
        if (Mathf.Abs(_lastX - newX) > 0.1f || Mathf.Abs(_lastZ - newZ) > 0.1f)
        {
            _lastX = newX;
            _lastZ = newZ;
            return true;
        }
        return false;
    }

    /// <summary>
    ///     立即同步验证玩家位置
    /// </summary>
    public void SyncWithServer()
    {
        if (ValidateHeroPos())
        {
            WorldManager.Instance.VerifyWalk(_lastX, _lastZ);
        }
    }

    #endregion

    public string GetPosStr()
    {
        return string.Format("x={0}&z={1}",_mTrans.position.x, _mTrans.position.z);
    }
}