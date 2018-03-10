// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  DoubleTeleportUnit.cs
// Author   : willson
// Created  : 2014/12/24 
// Porpuse  : 
// **********************************************************************

using AppDto;
using UnityEngine;

public class DoubleTeleportUnit : TriggerNpcUnit
{
    private bool waitingForTrigger;

    protected override bool NeedTrigger()
    {
        return true;
    }

    protected override void SetupBoxCollider()
    {
        _boxCollider = _unitGo.GetMissingComponent<BoxCollider>();
        _boxCollider.isTrigger = true;
        _boxCollider.center = new Vector3(0f, 0.5f, 0f);
        _boxCollider.size = new Vector3(2, 1, 2);
        _unitGo.tag = GameTag.Tag_Teleport;
    }

    public override bool NeedClose()
    {
        return true;
    }

    protected override float CheckDistance()
    {
        return 3f;
    }

    public override void Reset()
    {
        base.Reset();
        waitingForTrigger = false;
    }

    public override void DoTrigger()
    {
        base.DoTrigger();
        enabled = false;

        GameDebuger.Log("DoTrigger DoubleTeleportUnit");

        if (_heroView != null)
        {
            _heroView.StopAndIdle();
        }

        NpcViewManager.EnableTrigger = false;
        enterTeleport();
    }


    public void CheckTrigger()
    {
        if (waitingForTrigger)
        {
            enterTeleport();
        }
    }

    private void enterTeleport()
    {
        GameDebuger.Log("enterTeleport!!");

        waitingForTrigger = false;

        if (_npcInfo.npcStateDto.npc is NpcDoubleTeleport)
        {
            enterDoubleTeleport();
        }
    }

    private void enterDoubleTeleport()
    {
        // 挂机中忽略传送点
        if (ModelManager.Player.IsAutoFram)
            return;

        var npcDoubleTeleport = _npcInfo.npcStateDto.npc as NpcDoubleTeleport;
        if (npcDoubleTeleport != null)
            WorldManager.Instance.Enter(
                npcDoubleTeleport.toSceneId
                , true
                , flyPos:string.Format("x={0}&y={1}&z={2}",npcDoubleTeleport.x, npcDoubleTeleport.y, npcDoubleTeleport.z));
    }

    public void StopTrigger()
    {
        //Reset();
        //		waitingForTrigger = false;
    }
}