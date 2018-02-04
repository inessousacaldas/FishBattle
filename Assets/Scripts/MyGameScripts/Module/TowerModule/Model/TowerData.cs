// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 12/13/2017 11:28:04 AM
// **********************************************************************

using System.Collections.Generic;
using AppDto;

public interface ITowerData
{
    bool ShowTowerGuildUI();
    string CurTowerName { get; }
    string LeftMonster { get; }
}

public sealed partial class TowerDataMgr
{
    public const string AutoNextTower = "AutoNextTower";
    public const string DelayAutoNextTower = "DelayAutoNextTower";
    public const string CheckAutoNextTower = "CheckAutoNextTower";
    public struct TowerMonsterData
    {
        private Npc monster;//怪物Id
        //private int seriaId;//编号Id

        public Npc Monster
        {
            get { return monster; }
            set { monster = value; }
        }

        //public int SeriaId
        //{
        //    get { return seriaId; }
        //    set { seriaId = value; }
        //}
    }
    public sealed partial class TowerData : ITowerData
    {

        private int currentTowerId = -1;
        private int resetCount = -1;
        private int maxTowerCount = 0;
        private List<int> monsterInfo = new List<int>();

        private List<TowerCfg> towerCfg;
        private List<Npc> _npcList;
        //该显示的怪物数据
        private List<TowerMonsterData> _showMonterList;
        //主场景进入Npc
        private Npc mainNpc;
        //塔层npc
        private Npc _showTowerNpc;
        //当前塔层是否已打完
        private bool isCurTowerClear = false;
        //所有塔层通关
        private bool isAllTowerClear = false;

        private int curSceneId = 0;

        private bool canFireData = true;
        private const float seconds = 5;

        public void InitData()
        {
            List<NpcGeneral> list = new List<NpcGeneral>();
        }

        public void Dispose()
        {

        }

        //初始化数据
        public void UpdateData(int currentTowerId, List<int> monsterInfo, int resetCount, int curSceneId)
        {
            this.currentTowerId = currentTowerId;
            this.resetCount = resetCount;
            this.curSceneId = curSceneId;
            this.monsterInfo.Clear();
            monsterInfo.ForEach(e =>
            {
                this.monsterInfo.Add(e);
            });
            UpdateTowerMonsterData();
            JudgeIsAllTowerClear();
        }

        private void JudgeIsAllTowerClear()
        {
            if (currentTowerId == MaxTowerCount)
            {
                var val = TowerCfg.Find(e => e.id == currentTowerId);
                if (val != null)
                {
                    bool isAllClear = true;
                    for (int i = 0, max = val.npcMonsterIds.Count; i < max; i++)
                    {
                        var id = val.npcMonsterIds[i];
                        if (!monsterInfo.Contains(id))
                        {
                            isAllClear = false;
                            break;
                        }
                    }
                    isAllTowerClear = isAllClear;
                }
            }
        }

        public IEnumerable<TowerCfg> TowerCfg
        {
            get
            {
                if (towerCfg == null)
                    towerCfg = DataCache.getArrayByCls<TowerCfg>();
                return towerCfg;
            }
        }

        public int MaxTowerCount
        {
            get
            {
                if (towerCfg == null)
                {
                    towerCfg = DataCache.getArrayByCls<TowerCfg>();
                }
                return towerCfg.Count;
            }
        }


        //打完一个关卡后下发的怪物消失通知
        public void UpdateTowerDataByDisappear(TowerBattleWinNotify notify)
        {
            if (WorldManager.Instance.GetModel().GetSceneDto().sceneMap.sceneFunctionType == (int)SceneMap.SceneFunctionType.Tower)
            {
                monsterInfo.Add((int)notify.npcMonsterId);
                var monster = TowerCfg.Find(e => e.id == currentTowerId);
                if (monster != null)
                {
                    bool isAllContains = true;
                    var monsterIds = monster.npcMonsterIds;
                    for (int i = 0, max = monsterIds.Count; i < max; i++)
                    {
                        if (!monsterInfo.Contains(monsterIds[i]))
                        {
                            isAllContains = false;
                            isCurTowerClear = false;
                            break;
                        }
                    }
                    if (isAllContains)
                    {
                        if (currentTowerId < MaxTowerCount)
                        {
                            currentTowerId++;
                            if (currentTowerId != notify.towerId)
                                currentTowerId = notify.towerId;
                            monsterInfo.Clear();
                        }
                        else
                        {
                            JudgeIsAllTowerClear();
                        }
                        isCurTowerClear = true;
                    }
                }
                else
                {
                    GameDebuger.Log("导表TowerCfg不存在id: " + currentTowerId + "，请策划检查");
                }
            }
            UpdateTowerMonsterData();
            AutoNextTower();
        }

        private bool autoNextTower = false;
        //自动打下层
        private void AutoNextTower()
        {
            if (isCurTowerClear && !isAllTowerClear)
            {
                autoNextTower = true;
                //TipManager.AddTip("[7ee830]5[-]秒传送到下一层");
                //JSTimer.Instance.SetupCoolDown(TowerDataMgr.AutoNextTower, seconds, null, delegate { TowerNetMsg.EnterTower(); });
            }
            else
            {
                JSTimer.Instance.CancelTimer(TowerDataMgr.CheckAutoNextTower);
                isTimer = false;
            }
        }

        private bool isTimer = false;
        private bool isFirst = false;
        public void CheckAutoNextTower()
        {
            if (BattleDataManager.DataMgr.IsWin == BattleResult.WIN && !isTimer)
            {
                isTimer = true;
                JSTimer.Instance.SetupTimer(TowerDataMgr.CheckAutoNextTower, () =>
                 {
                     if (!BattleDataManager.DataMgr.IsInBattle && !isFirst && autoNextTower)
                     {
                         isFirst = true;
                         JSTimer.Instance.SetupCoolDown(TowerDataMgr.DelayAutoNextTower, 0.5f, null, () =>
                         {
                             TipManager.AddTip("[7ee830]" + seconds + "[-]秒传送到下一层");
                             JSTimer.Instance.SetupCoolDown(TowerDataMgr.AutoNextTower, seconds, null, delegate { TowerNetMsg.EnterTower(); });
                         });
                     }
                 });
            }
        }

        //更新可视怪物数据
        public void UpdateTowerMonsterData()
        {
            var monster = TowerCfg.Find(f => f.id == currentTowerId);
            if (monster != null)
            {
                var monsterIdStr = monster.npcMonsterIds;
                List<TowerMonsterData> _monsterList = new List<TowerMonsterData>();
                TowerMonsterData towerData = new TowerMonsterData();
                monsterIdStr.ForEach(e =>
                {
                    if (!monsterInfo.Contains(e))
                    {
                        //添加四轮之塔怪物
                        towerData.Monster = Npcs.Find(data => data.id == e);
                        _monsterList.Add(towerData);
                    }
                });
                //添加四轮之塔塔层npc
                _showTowerNpc = Npcs.Find(e => monster.npcGeneralId == e.id);
                _showMonterList = _monsterList;
            }
            else
            {
                GameDebuger.Log("导表TowerCfg不存在id: " + currentTowerId + "，请策划检查");
            }
        }

        //塔层怪物
        public IEnumerable<TowerMonsterData> TowerMonsterList
        {
            get { return _showMonterList; }
        }

        //主场景NPC
        public Npc MainNpc
        {
            get
            {
                mainNpc = Npcs.Find(e => e is NpcGeneral && e.type == (int)Npc.NpcType.Tower && e.sceneId != 0);
                return mainNpc;
            }
        }

        public IEnumerable<Npc> Npcs
        {
            get
            {
                if (_npcList == null)
                    _npcList = DataCache.getArrayByCls<Npc>();
                return _npcList;
            }
        }
        //塔层NPC
        public Npc ShowTowerNpc
        {
            get { return _showTowerNpc; }
        }

        //重置次数
        public int ResetCount
        {
            get { return resetCount; }
        }

        public void UpdateResetCount()
        {
            currentTowerId = towerCfg[0].id;
            monsterInfo.Clear();
            resetCount = 0;
            isCurTowerClear = false;
            isAllTowerClear = false;
            UpdateTowerMonsterData();
        }
        
        //当前塔层id
        public int CurTowerId
        {
            get { return currentTowerId; }
        }

        //当前塔层名字
        public string CurTowerName
        {
            get
            {
                string towerName = "四轮之塔";
                int curId = currentTowerId;
                if (isCurTowerClear && !isAllTowerClear)
                    curId--;
                var monster = TowerCfg.Find(f => f.id == curId);
                if (monster != null && !isCurTowerClear && !isAllTowerClear)
                    towerName = monster.name;
                else if(isCurTowerClear && !isAllTowerClear)
                    towerName = "前往下一层";
                else if (isAllTowerClear)
                    towerName = "挑战结束";

                return towerName;
            }
        }

        //剩余怪物
        public string LeftMonster
        {
            get
            {
                string des = "";
                int count = _showMonterList.Count;
                if (!isCurTowerClear && !isAllTowerClear)
                    des = "剩余怪物" + count + "个"; 
                return des;
            }
        }

        public bool ShowTowerGuildUI()
        {
            bool show = false;
            if (WorldManager.Instance != null && WorldManager.Instance.GetModel() != null && WorldManager.Instance.GetModel().GetSceneDto() != null)
            {
                if (WorldManager.Instance.GetModel().GetSceneDto().sceneMap.sceneFunctionType == (int)SceneMap.SceneFunctionType.Tower)
                    show = true;
                else
                    show = false;
            }
            else
                show = false;

            return show;
        }

        //当前塔层是否已打完
        public bool IsTowerClear
        {
            get { return isCurTowerClear; }
        }
        public void ResetCurTowerClear()
        {
            JSTimer.Instance.CancelCd(TowerDataMgr.AutoNextTower);
            JSTimer.Instance.CancelTimer(TowerDataMgr.CheckAutoNextTower);
            JSTimer.Instance.CancelCd(TowerDataMgr.DelayAutoNextTower);
            isTimer = false;
            isFirst = false;
            autoNextTower = false;
            isCurTowerClear = false;
        }

        //所有塔层是否打完
        public bool IsAllTowerClear()
        {
            return isAllTowerClear;
        }
        
        public void FindToNpc()
        {
            if (isCurTowerClear || isAllTowerClear)
                WorldManager.Instance.FlyToByNpc(_showTowerNpc, 0);
            else
                WorldManager.Instance.FlyToByNpc(_showMonterList[UnityEngine.Random.Range(0, _showMonterList.Count)].Monster, 0);
        }
    }
}
