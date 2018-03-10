// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 12/13/2017 11:28:04 AM
// **********************************************************************

using AppDto;
using System.Collections.Generic;
using UniRx;

public sealed partial class TowerDataMgr
{
    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<TowerBattleWinNotify>(UpdateTowerDataByDisappear));
        _disposable.Add(BattleDataManager.Stream.Select((data, state) => data.battleState).Subscribe(stateTuple =>
        {
            _data.CheckAutoNextTower();
        }));
    }
    
    public void OnDispose(){
            
    }

    //初始化
    public void SetUp(AfterLoginDto dto)
    {
        _data.UpdateData(dto.currentTowerId, dto.monsterInfo,dto.towerResetTimes,dto.sceneId);
    }
    
    //当前塔层名字
    public string TowerName
    {
        get
        {
            string name = "";
            var val = _data.TowerCfg.Find<TowerCfg>(e =>
                e.id == _data.CurTowerId
            );
            if (val != null)
                name = val.name;
            else
                GameDebuger.Log("四轮之塔id: " + _data.CurTowerId + "不存在于TowerCfg配表中");

            return name;
        }
    }

    public int ResetCount
    {
        get { return _data.ResetCount; }
    }

    public void UpdateResetCount()
    {
        TipManager.AddTip("重置成功");
        _data.UpdateResetCount();
    }

    //当前塔层
    public int CurTowerId
    {
        get { return _data.CurTowerId; }
    }

    //四轮之塔怪物数据
    public IEnumerable<TowerMonsterData> TowerMonsterList
    {
        get { return _data.TowerMonsterList; }
    }

    //塔层NPC
    public Npc ShowTowerNpc
    {
        get { return _data.ShowTowerNpc; }
    }

    //打完一层怪物后的notify
    public void UpdateTowerDataByDisappear(TowerBattleWinNotify notify)
    {
        if (notify == null) return;
        int sceneId = WorldManager.Instance.GetModel().GetSceneId();
        if (sceneId == notify.sceneId)
        {
            WorldManager.Instance.GetNpcViewManager().RemoveDynamicCommonNpc(notify.npcMonsterId);
        }
        _data.UpdateTowerDataByDisappear(notify);
        FireData();
    }

    //进入下一层成功后数据更新
    public void UpdateData()
    {
        //重置是否已完成全部关卡
        _data.ResetCurTowerClear();
    }

    //该层关卡是否打完
    public bool IsTowerClear
    {
        get { return _data.IsTowerClear; }
    }

    //主场景NPC
    public Npc MainNpc
    {
        get { return _data.MainNpc; }
    }

    //是否通关全部塔层
    public bool IsAllTowerClear
    {
        get { return _data.IsAllTowerClear(); }
    }

    //离开四轮之塔场景的判断
    public bool IsInTowerAndCheckLeave(System.Action callback)
    {
        if (WorldManager.Instance != null && WorldManager.Instance.GetModel() != null && WorldManager.Instance.GetModel().GetSceneDto() != null)
        {
            if( WorldManager.Instance.GetModel().GetSceneDto().sceneMap.sceneFunctionType == (int)SceneMap.SceneFunctionType.Tower)
            {
                string des = "离开四轮之塔不会清空进度，再次进入可以继续挑战。确定要离开？";
                string title = "";
                var ctrl = ProxyBaseWinModule.Open();
                BaseTipData data = BaseTipData.Create(title, des, 0, delegate 
                {
                    SetCanFireData();
                    callback();
                } , null);
                ctrl.InitView(data);
                return true;
            }
        }
        return false;
    }

    public bool IsInTower()
    {
        if (WorldManager.Instance != null && WorldManager.Instance.GetModel() != null && WorldManager.Instance.GetModel().GetSceneDto() != null)
        {
            if (WorldManager.Instance.GetModel().GetSceneDto().sceneMap.sceneFunctionType == (int)SceneMap.SceneFunctionType.Tower)
            {
                TipManager.AddTip("在四轮之塔中，不能创建队伍");
                return true;
            }
        }
        return false;
    }
    
    public void SetCanFireData()
    {
        JSTimer.Instance.CancelCd(AutoNextTower);
    }

    public void FindToNpc()
    {
        _data.FindToNpc();
    }

    public ITowerData ITowerData()
    {
        return _data;
    }
}
