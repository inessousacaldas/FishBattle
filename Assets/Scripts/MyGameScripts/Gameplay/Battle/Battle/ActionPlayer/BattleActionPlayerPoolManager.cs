// **********************************************************************
// Copyright (c) 2016 cilugame. All rights reserved.
// File     : BattleActionPlayerPoolManager.cs
// Author   : senkay <senkay@126.com>
// Created  : 8/4/2016 
// Porpuse  : 
// **********************************************************************

using UnityEngine;
using System.Collections.Generic;

public class BattleActionPlayerPoolManager
{
    private GameObject _rootGO;
    private Queue<BattleActionPlayer> _playerQueue;
    //public BattleActionPlayer[] DebugQueue { get { return _rootGO.GetComponents< BattleActionPlayer>(); } }
    public void Setup(GameObject rootGO)
    {
        _rootGO = rootGO;
        _playerQueue = new Queue<BattleActionPlayer>();
    }

    public BattleActionPlayer Spawn()
    {
        BattleActionPlayer player = null;
        if (_playerQueue != null && _playerQueue.Count > 0)
        {
            player = _playerQueue.Dequeue();
        }
        else
        {
            player = _rootGO.AddComponent<BattleActionPlayer>();
        }

        player.retreatEvt = retreatHandler;
        player.enabled = true;
        return player;
    }

    private void retreatHandler(long playerID, List<long> soldidList)
    {
        BattleDataManager.DataMgr.RetreatBattle(playerID, soldidList);
    }

    public void Despawn(BattleActionPlayer player)
    {
        player.enabled = false;
        player.retreatEvt = null;
        _playerQueue.Enqueue(player);
    }

    public void Dispose()
    {
        if (_playerQueue != null)
        {
            _playerQueue.ForEach(p=>p.retreatEvt = null);
            _playerQueue.Clear();
        }
        
        _playerQueue = null;
        _rootGO = null;
    }

    #region 单例

    private static BattleActionPlayerPoolManager mInstance = new BattleActionPlayerPoolManager();

    public static BattleActionPlayerPoolManager Instance
    {
        get
        {
            return mInstance;
        }
    }

    #endregion
}