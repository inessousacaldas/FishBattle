// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  NpcView.cs
// Author   : willson
// Created  : 2014/12/23 
// Porpuse  : 
// **********************************************************************

#define UseQuadTree
using System.Collections.Generic;
using AppDto;
using UnityEngine;
using System.Collections.ObjectModel;
public partial class NpcViewManager
{
    public static bool EnableTrigger = true;

    private bool _isInit;
#pragma warning disable 0414
    private bool _npcIsReady;
#pragma warning restore
    private UnitWaitingTrigger _unitWaitingTrigger;
    private readonly Dictionary<string, SceneNpcDto> offsetDic = new Dictionary<string, SceneNpcDto>();
    public NpcViewPool npcViewPool { get; private set; }
    public NpcViewDataManager npcViewDataManager { get; private set; }
    public Dictionary<long, BaseNpcUnit> _npcs
    {
        get { return npcViewDataManager.npcs; }
    }
    public NpcViewManager()
    {
        npcViewPool = new NpcViewPool(this);
        npcViewDataManager = new NpcViewDataManager(this);

    }
    public void Setup(HeroView heroView)
    {
        npcViewDataManager.Setup();
        _unitWaitingTrigger = new UnitWaitingTrigger();
        _unitWaitingTrigger.SetHeroPlayer(heroView);
        //	重置NPC可点击
        EnableTrigger = true;

        _npcIsReady = false;
        //npcViewDataManager.Dispose();
        InitStaticNpc();
        InitOtherNpc();
        InitDynamicNpc();
#if UseQuadTree
        StartQuadCheck();
#else
        StartLoadNpcs();
#endif
        GameDebuger.TODO(@"if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_PreciousBox, false))
        {
            GameEventCenter.RemoveListener(GameEvent.Player_OnPlayerGradeUpdate, CanSeePreciousBox); //预防加了多次
            GameEventCenter.AddListener(GameEvent.Player_OnPlayerGradeUpdate, CanSeePreciousBox);
        }

        if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_GoldBox, false))
        {
            GameEventCenter.RemoveListener(GameEvent.Player_OnPlayerGradeUpdate, CanSeeGoldBox); //预防加了多次
            GameEventCenter.AddListener(GameEvent.Player_OnPlayerGradeUpdate, CanSeeGoldBox);
        }");
    }

    private void InitStaticNpc()
    {
        var npcs = DataCache.getArrayByCls<Npc>();
        int sceneId = WorldManager.Instance.GetModel().GetSceneId();

        if (null == npcs)
            GameDebuger.LogError("没有NPC数据");
        else
        {
            for (int i = 0, len = npcs.Count; i < len; i++)
            {
                var npc = npcs[i];
                if (npc.sceneId == sceneId)
                {
                    if(npc is NpcPeriod)    //活动npc不显示,暂时如此处理   --xush
                        continue;

                    if (npc is NpcDoubleTeleport || npc is NpcGeneral)
                    {
                        AddNpcUnit(npc);
                    }
                }
            }
        }
    }

    private void InitDynamicNpc()
    {
        IEnumerable<NpcInfoDto> tShowNpcInfoDtoList;
        //	是否有明雷任务Npc存在当前场景
        tShowNpcInfoDtoList = MissionDataMgr.DataMgr.GetShowMonsterNpcInfoDtoList();
        if(tShowNpcInfoDtoList != null)
        {
            tShowNpcInfoDtoList.ForEach(e =>
            {
                AddNpcUnit(e);
            });
        }

        //显示四轮之塔的怪物
        if(WorldManager.Instance!=null && WorldManager.Instance.GetModel()!=null && WorldManager.Instance.GetModel().GetSceneDto()!=null&&
            WorldManager.Instance.GetModel().GetSceneDto().sceneMap.sceneFunctionType == (int)SceneMap.SceneFunctionType.Tower) 
        {
            IEnumerable<TowerDataMgr.TowerMonsterData> tShowMonsterInfoList;
            var towerData = TowerDataMgr.DataMgr;
            tShowMonsterInfoList = towerData.TowerMonsterList;
            if (tShowMonsterInfoList != null)
            {
                tShowMonsterInfoList.ForEach(e =>
                {
                    AddNpcUnit(e.Monster);
                });
                AddNpcUnit(towerData.ShowTowerNpc);
            }
        }
    }

    private void InitOtherNpc()
    {
        if (WorldManager.Instance != null && WorldManager.Instance.GetModel() != null && WorldManager.Instance.GetModel().GetSceneDto() != null)
        {
            //将字典转为哈希，之前for会报错
            List<SceneNpcDto> list = WorldManager.Instance.GetModel().NpcsDic.Values.ToList<SceneNpcDto>();
            if(list != null && list.Count > 0)
            {
                for(int i = 0, len = list.Count;i < len;i++)
                {
                    var npcState = list[i];
                    AddNpcUnit(npcState);
                }
            }
        }
    }

    private void StartLoadNpcs()
    {
        _npcIsReady = true;

        foreach (var npcUnit in _npcs.Values)
        {
            SetupNpc(npcUnit);
        }
    }
    private Vector3 lastPos = Vector3.zero;
    private void StartQuadCheck()
    {
        JSTimer.Instance.SetupTimer("NpcViewManager_QuadTree_Check", () =>
        {
            Vector3 pos = WorldManager.Instance.GetHeroWorldPos();
#if UNITY_EDITOR
            //************************Debug****************************
            npcViewDataManager.quadTree.DrawTree(0.5f);
            Bounds bounds2 = MathHelper.Bounds2D(pos.x, pos.z, 30, 30f);
            QuadTree<BaseNpcUnit>.DrawBounds(bounds2, Color.red, 0.5f, 1f);
            //*********************************************************
#endif
            if (QuadCanUpdate(pos) == false)
                return;
            Bounds bounds = MathHelper.Bounds2D(pos.x, pos.z, 30f, 30f);
            List<BaseNpcUnit> visualList = npcViewDataManager.quadTree.Query(bounds);
            List<long> setupNpcList = npcViewDataManager.setupNpcList;
            visualList.AddRange(npcViewDataManager.alwaysShowList);
            QuadRemove(visualList, setupNpcList);

            QuadSetup(visualList, setupNpcList);
        }
        , 0.5f, false);
    }

    private bool QuadCanUpdate(Vector3 heroPos)
    {
        if (heroPos == Vector3.zero)
            return false;
        if (lastPos == Vector3.zero)
        {
            lastPos = heroPos;
        }
        else
        {
            if (Vector3.Distance(lastPos, heroPos) < 1f)
                return false;
            else
                lastPos = heroPos;
        }
        return true;
    }
    private void QuadRemove(List<BaseNpcUnit> visualList, List<long> setupNpcList)
    {
        for (int i = 0; i < setupNpcList.Count; i++)
        {
            BaseNpcUnit item;
            if (_npcs.TryGetValue(setupNpcList[i], out item))
            {
                if (visualList.Contains(item) == false)
                {
                    item.Destroy();
                    setupNpcList.RemoveAt(i);
                    i--;
                }
            }
        }
    }
    private void QuadSetup(List<BaseNpcUnit> visualList, List<long> setupNpcList)
    {
        for (int i = 0; i < visualList.Count; i++)
        {
            BaseNpcUnit item = visualList[i];
            if (setupNpcList.Contains(item.GetNpcUID()) == false)
            {
                setupNpcList.Add(item.GetNpcUID());
                SetupNpc(item);
            }
        }
    }
//    public void UpdateSceneNpcState(SceneNpcStateDto newState)
//    {
//        if (_npcs.ContainsKey(newState.npcId))
//        {
//            _npcs[newState.npcId].UpdateNpcState(newState);
//        }
//    }

    public void AddDynamicCommonNpc(SceneNpcDto npcState)
    {
        if(npcState.sceneId == WorldManager.Instance.GetModel().GetSceneId() && !WorldManager.Instance.GetModel().NpcsDic.ContainsKey(npcState.id)) {
            WorldManager.Instance.GetModel().NpcsDic.Add(npcState.id,npcState);
            AddNpcUnit(npcState);
        }
    }

    public void RemoveDynamicCommonNpc(long npcUID)
    {
        var commonNpcStates = WorldManager.Instance.GetModel().NpcsDic;
        commonNpcStates.Remove(npcUID);
        RemoveNpc(npcUID);
    }

#region 监听升级后是否能看到金箱子

    public void CanSeeGoldBox()
    {
        GameDebuger.TODO(@"if (FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_GoldBox, false))
        {
            GameEventCenter.RemoveListener(GameEvent.Player_OnPlayerGradeUpdate, CanSeeGoldBox);

            var commonNpcStates = WorldManager.DataMgr.GetModel().NpcsDic;
            for (int i = 0, len = commonNpcStates.Count; i < len; i++)
            {
                var npcState = commonNpcStates[i];
                if (npcState.npc is NpcSceneGoldBox)
                {
                    var box = (NpcSceneGoldBox)npcState.npc;
                    if (box != null)
                    {
                        int playerLv = ModelManager.Player.GetPlayerLevel();
                        if (playerLv >= box.minGrade && playerLv <= box.maxGrade)
                        {
                            AddNpcUnit(npcState);
                        }
                    }
                }
            }
        }");
    }

#endregion


    //监听升级后是否能看到箱子
    public void CanSeePreciousBox()
    {
        GameDebuger.TODO(@"if (FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_PreciousBox, false))
        {
            if (ModelManager.AssistSkill.HidePreciousBox) return;
            GameEventCenter.RemoveListener(GameEvent.Player_OnPlayerGradeUpdate, CanSeePreciousBox);
            var commonNpcStates = WorldManager.DataMgr.GetModel().NpcsDic;
            for (int i = 0, len = commonNpcStates.Count; i < len; i++)
            {
                var npcState = commonNpcStates[i];
                if (npcState.npc is NpcScenePreciousBox)
                {
                    var box = (NpcScenePreciousBox)npcState.npc;
                    if (string.IsNullOrEmpty(box.visibleGrade))
                    {
                        AddNpcUnit(npcState);
                    }
                    else
                    {
                        var lvSteps = box.visibleGrade.ParseToList<int>(',', StringHelper.ToInt);
                        if (lvSteps != null)
                        {
                            int playerLv = ModelManager.Player.GetPlayerLevel();
                            if (playerLv >= lvSteps[0] && playerLv <= lvSteps[1])
                            {
                                AddNpcUnit(npcState);
                            }
                        }
                    }
                }
            }
        }");
    }

    private bool _isInitGold;
    /// <summary>
    /// 金箱子的隐藏显示
    /// </summary>
    /// <param name="show"></param>
    public void ShowOrHideGoldBox(bool show)
    {
        GameDebuger.TODO(@"if (FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_GoldBox, false))");
        if(true)
        {
            if (!_isInitGold)
            {
                _isInitGold = true;
                CanSeeGoldBox();
                return;
            }
            var commonNpcStates = WorldManager.Instance.GetModel().NpcsDic;
            for (int i = 0, len = commonNpcStates.Count; i < len; i++)
            {
                var npcState = commonNpcStates[i];
                GameDebuger.TODO(@"if (npcState.npc is NpcSceneGoldBox)
                {
                    var tBoxUnit = GetNpcUnit(npcState.id);
                    if (tBoxUnit != null)
                    {
                        tBoxUnit.SetUnitActive(!show);
                    }
                }");
            }
        }
        else
        {
            if (!_isInitGold) return;
            var commonNpcStates = WorldManager.Instance.GetModel().NpcsDic;
            for (int i = 0, len = commonNpcStates.Count; i < len; i++)
            {
                var npcState = commonNpcStates[i];
                GameDebuger.TODO(@"if (npcState.npc is NpcSceneGoldBox)
                {
                    var tBoxUnit = GetNpcUnit(npcState.id);
                    if (tBoxUnit != null)
                    {
                        tBoxUnit.SetUnitActive(false);
                    }
                }");
            }
        }
    }

    public void ShowOrHidePreciousBox(bool show)
    {
        GameDebuger.TODO(@"if (FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_PreciousBox, false))");
        if(true)
        {
            if (!_isInit)
            {
                _isInit = true;
                CanSeePreciousBox();
                return;
            }

            var commonNpcStates = WorldManager.Instance.GetModel().NpcsDic;
            for (int i = 0, len = commonNpcStates.Count; i < len; i++)
            {
                var npcState = commonNpcStates[i];
                GameDebuger.TODO(@"if (npcState.npc is NpcScenePreciousBox)
                {
                    var tBoxUnit = GetNpcUnit(npcState.id);
                    if (tBoxUnit != null)
                    {
                        tBoxUnit.SetUnitActive(!show);
                    }
                }");
            }
        }
        else
        {
            if (!_isInit) return;
            var commonNpcStates = WorldManager.Instance.GetModel().NpcsDic;
            for (int i = 0, len = commonNpcStates.Count; i < len; i++)
            {
                var npcState = commonNpcStates[i];
                GameDebuger.TODO(@"if (npcState.npc is NpcScenePreciousBox)
                {
                    var tBoxUnit = GetNpcUnit(npcState.id);
                    if (tBoxUnit != null)
                    {
                        tBoxUnit.SetUnitActive(false);
                    }
                }");
            }
        }
    }

#region 是否隐藏掉红娘

    public void HideMatchmaker(bool hide)
    {
        GameDebuger.TODO(@"if (_npcs.ContainsKey(ProxyDialogueModule.Matchmaker_NPC_ID))
        {
            _npcs[ProxyDialogueModule.Matchmaker_NPC_ID].SetModelActive(hide);
        }
        else
        {
            GameDebuger.Log('没有找到红娘');
        }");
    }

#endregion


#region 给定NPC显示其余全隐藏
    public void SetNpcState(List<int> npcIdList)
    {
        if (null == npcIdList)
            return;
        foreach (var baseNpcUnit in _npcs)
        {
            baseNpcUnit.Value.SetUnitActive(false);
            for (int i = 0; i < npcIdList.Count; i++)
            {
                if (_npcs.ContainsKey(npcIdList[i]))
                    _npcs[npcIdList[i]].SetUnitActive(true);
            }
        }
    }
#endregion

#region 所有NPC全显示
    public void SetNPCActive()
    {
        if (null == _npcs)
            return;
        foreach (var baseNpcUnit in _npcs)
        {
            baseNpcUnit.Value.SetUnitActive(true);
        }
    }
#endregion

#region 剧情隐藏npc

    public void SetNpcState(int npcId, bool state)
    {
        if (_npcs.ContainsKey(npcId))
        {
            _npcs[npcId].SetModelActive(state);
        }
        else
        {
            GameDebuger.Log("没有找到npc:" + npcId);
        }
    }

    #endregion

    public void UpdateDynamicCommonNpcBattleState(NpcBattleNotify notify)
    {
        var commonNpcStates = WorldManager.Instance.GetModel().NpcsDic.Values.ToList();
        for (int i = 0, len = commonNpcStates.Count; i < len; i++)
        {
            var npcState = commonNpcStates[i];
            if (npcState.id == notify.npcUniqueId)
            {
                npcState.battleId = notify.battleId;
                if (_npcs.ContainsKey(notify.npcUniqueId))
                {
                    if (_npcs[notify.npcUniqueId] is TriggerNpcUnit)
                    {
                        (_npcs[notify.npcUniqueId] as TriggerNpcUnit).SetNPCFightFlag(notify.battleId > 0);
                    }
                }
                break;
            }
        }
    }

    //    public void UpdatePlayerJoinLeaveGuildState(PlayerJoinLeaveGuildNotify notify)
    //    {
    //        var tScenePlayerDto = WorldManager.DataMgr.GetModel().GetPlayerDto(notify.playerId);
    //
    //        if (tScenePlayerDto != null && tScenePlayerDto.guildId != notify.guildId)
    //        {
    //            tScenePlayerDto.guildId = notify.guildId;
    //        }
    //    }

    public void AddNpcUnit(NpcInfoDto missionNpcInfo)
    {
        if(missionNpcInfo == null)
            return;
        var npcStateDto = new SceneNpcDto();
        npcStateDto.x = missionNpcInfo.x;
        npcStateDto.z = missionNpcInfo.z;
        npcStateDto.npcId = missionNpcInfo.id;
        npcStateDto.id = missionNpcInfo.id;
        var npcInfo = new BaseNpcInfo();
        npcInfo.npcStateDto = npcStateDto;
        npcInfo.name = missionNpcInfo.name;

        npcInfo.playerDressInfo = missionNpcInfo.playerDressInfo;

        npcInfo.npcAppearance = missionNpcInfo.npcAppearance;

        npcInfo.submitIndex = missionNpcInfo.index;

        AddNpcUnit(npcInfo,false);
    }


    private Mission mCollectionMission;
    /// <summary>
    /// 为了采集物特许添加的增加NPC方法
    /// </summary>
    /// <param name="Uid">采集物的UID</param>
    /// <param name="missionNpcInfo">采集物的具体信息</param>
    public void AddNpcUnit(long Uid,NpcInfoDto missionNpcInfo,Mission _mission)
    {
        var npcStateDto = new SceneNpcDto();
        npcStateDto.x = missionNpcInfo.x;
        npcStateDto.z = missionNpcInfo.z;
        npcStateDto.npcId = missionNpcInfo.id;
        npcStateDto.id = Uid;
        var npcInfo = new BaseNpcInfo();
        npcInfo.npcStateDto = npcStateDto;
        npcInfo.name = missionNpcInfo.name;
        npcInfo.playerDressInfo = missionNpcInfo.playerDressInfo;
        npcInfo.submitIndex = missionNpcInfo.index;
        mCollectionMission = _mission;
        AddNpcUnit(npcInfo,false);
    }

    public void AddNpcUnit(Npc npc)
    {
        var npcStateDto = new SceneNpcDto();
        npcStateDto.x = npc.x;
        npcStateDto.z = npc.z;
        npcStateDto.npcId = npc.id;
        npcStateDto.id = npc.id;
        AddNpcUnit(npcStateDto);
    }

    public void AddNpcUnit(SceneNpcDto npcStateDto)
    {
        if (npcStateDto == null)
        {
            GameDebuger.LogError("NpcViewManager.AddNpcUnit npcStateDto = null");
        }
        else if (null == npcStateDto.npc)
            GameDebuger.LogError(string.Format("AddNpcUnit failed for npc == null ,npcId:{0}", npcStateDto.npcId));
        else
        {
            var npcInfo = new BaseNpcInfo();
            npcInfo.npcStateDto = npcStateDto;
            npcInfo.name = npcStateDto.npc.name;

            //	非任务过来的Npc，其submitIndex值设置为0（public void AddNpcUnit(NpcInfoDto missionNpcInfo)）
            npcInfo.submitIndex = 0;//missionNpcInfo.index;

            AddNpcUnit(npcInfo);
        }
    }

    public void AddNpcUnit(BaseNpcInfo npcInfo, bool needCheckOffset = true)
    {
        if (_unitWaitingTrigger == null)
        {
            return;
        }
        GameDebuger.TODO(@"var stateDto = WorldManager.DataMgr.GetModel().GetSceneNpcState(npcInfo.npcStateDto.npcId);
        if (stateDto != null)
        {
            //  是否任务指定NPCID
            ModelManager.MissionData.missionTypeFactionTrialDelegate.SetMissionSpecifyNpc(stateDto);

            npcInfo.playerDressInfo = stateDto.playerDressInfo;
            npcInfo.rideMountNotify = stateDto.rideMountNotify;
            npcInfo.rideLevel = stateDto.rideLevel;
            npcInfo.name = stateDto.nickname;
        }");
        SceneNpcDto npcState = null;
        if (needCheckOffset)
        {
            //判断有重叠偏移
            npcState = overlapOffset(npcInfo.npcStateDto);
        }
        else
        {
            npcState = npcInfo.npcStateDto;
        }

        var npc = npcState.npc;
        int sceneId = WorldManager.Instance.GetModel().GetSceneId();
        if (npc.sceneId != sceneId)
        {
            //GameDebuger.Log("!! "+npc.name + " at " + npc.sceneId);
            npc.sceneId = sceneId;
        }
        if (_npcs.ContainsKey(npcState.id))
        {
            _npcs[npcState.id].UpdateNpc(npcState);
            return;
        }
        npcInfo.AdjustAppearance();

        BaseNpcUnit npcUnit = CreateNpcUnit(npcInfo, npc);
        if (npcUnit != null)
        {
            npcUnit.Init(npcInfo);
            npcViewDataManager.AddNpc(npcUnit);
            if(!_npcs.ContainsKey(npcState.id))
            {
                _npcs.Add(npcState.id,npcUnit);
            }
            if(npcUnit is TriggerNpcUnit)
            {
                _unitWaitingTrigger.AddTriggerUnit(npcUnit as TriggerNpcUnit);
            }
            if(npcUnit.unitGo == null)
            {
                //SetupNpc(npcUnit);
                //npcUnit.Load();
                Vector3 pos = WorldManager.Instance.GetHeroWorldPos();
                Bounds bounds = MathHelper.Bounds2D(pos.x, pos.z, 30f, 30f);
                List<BaseNpcUnit> visualList = npcViewDataManager.quadTree.Query(bounds);
                List<long> setupNpcList = npcViewDataManager.setupNpcList;
                visualList.AddRange(npcViewDataManager.alwaysShowList);
                QuadRemove(visualList,setupNpcList);
                QuadSetup(visualList,setupNpcList);
            }
            GameDebuger.TODO(@"if (npc is NpcPeriod)
                npcUnit.SetUnitActive(WorldManager.DataMgr.GetModel().GetSceneNpcShowState(npcInfo.npcStateDto.npcId));");
        }
    }

    private void SetupNpc(BaseNpcUnit npcUnit)
    {
        Npc npc = npcUnit.GetNpc();
        npcUnit.Setup(this);
        GameDebuger.TODO(@"if (!(npc is NpcGeneral && (NpcGeneral.NpcGeneralKindEnum)((npc as NpcGeneral).kind) == NpcGeneral.NpcGeneralKindEnum.Area))");
        {
            npcUnit.Load();
        }
    }

    private BaseNpcUnit CreateNpcUnit(BaseNpcInfo npcInfo, Npc npc)
    {
        SceneNpcDto npcState = npcInfo.npcStateDto;
        BaseNpcUnit npcUnit = null;
        if(npc.type == (int)Npc.NpcType.PickPoint) {
            npcUnit = new NpcSceneCollectionUnit();
            (npcUnit as NpcSceneCollectionUnit)._mission = mCollectionMission;
            mCollectionMission = null;
        }
        else
        {
            if(npc is NpcDoubleTeleport)
            {
                npcUnit = new DoubleTeleportUnit();
            }
            else if(npc is NpcGeneral)
            {
                GameDebuger.TODO(@"if (npc is NpcPeriod)
            {
                if ((npc as NpcPeriod).open)
                    npcUnit = new NpcPeriodUnit();
            }
            else");
                npcUnit = new GeneralUnit();
            }
            else if(npc is NpcVariable)
            {
                npcUnit = new MonsterUnit();
            }
        }
        GameDebuger.TODO(@"else if (npc is NpcSceneTeleport)
        {
            npcUnit = new NpcSceneTeleportUnit();
        }
        //  随机生成的明雷怪 or  场景公共怪
        else if (npc is NpcVariable)
        {
            npcUnit = new MonsterUnit();
        }
        else if (npcState.npc is NpcScenePreciousBox)
        {

            if (FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_PreciousBox, false));
            {
                var box = npcState.npc as NpcScenePreciousBox;
                if (!ModelManager.AssistSkill.HidePreciousBox)
                {
                    _isInit = true;
                    if (string.IsNullOrEmpty(box.visibleGrade))
                    {
                        npcUnit = new PreciousBoxUnit();
                    }
                    else
                    {
                        var lvSteps = box.visibleGrade.ParseToList<int>(',', StringHelper.ToInt);
                        if (lvSteps != null)
                        {
                            int playerLv = ModelManager.Player.GetPlayerLevel();
                            if (playerLv >= lvSteps[0] && playerLv <= lvSteps[1])
                            {
                                npcUnit = new PreciousBoxUnit();
                            }
                        }
                    }
                }
            }
        }
        else if (npcState.npc is NpcSceneMarriageSweetBox)
        {
            npcUnit = new MarrySweetBoxUnit();
        }
        else if (npcState.npc is NpcSceneBridalSedanBox)
        {
            npcUnit = new BridalSedanBoxUnit();
        }
        else if (npcState.npc is NpcSceneBridalSedanPetBox)
        {
            npcUnit = new BridalSedanPetBoxUnit();
        }
        else if (npcState.npc is NpcSceneGoldBox)
        {
                if (FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_GoldBox, false));
            {
                var box = (NpcSceneGoldBox)npcState.npc;
                if (box != null)
                {
                    int playerLv = ModelManager.Player.GetPlayerLevel();
                    if (playerLv >= box.minGrade && playerLv <= box.maxGrade)
                    {
                        npcUnit = new NpcSceneGoldBoxUnit();
                    }
                }
            }
        }
        else if (npcState.npc is NpcSceneBridalSedanSweet)
        {
            npcUnit = new BridalSedanSweetBoxUnit();
        }
        else if (npcState.npc is NpcSceneWorldBossPreciousBox)
        {
            //var box = npcState.npc as NpcSceneWorldBossPreciousBox;
            npcUnit = new WorldBossPreciousBoxUnit();
        }
        else if (npcState.npc is NpcSceneGuildCompBox)
        {
            //var box = npcState.npc as NpcSceneGuildCompBox;
            npcUnit = new NpcSceneGuildCompBoxUnit();
        }
        else if (npcState.npc is NpcCampWarPeach)
        {
            //var box = npcState.npc as NpcCampWarPeach;
            npcUnit = new NpcSceneCampWarPeachBoxUnit();
        }
        else if (npcState.npc is NpcSceneMazeBox)
        {
            npcUnit = new NpcSceneMazeBoxUnit();
        }
        else if (npcState.npc is NpcSceneGrass)
        {
            npcUnit = new NpcSceneGrassUnit();
        }
        else if (npc is NpcSceneWorldGhostBox)
        {
            //世界Boss 宝箱做处理
            //var box = npcState.npc as NpcSceneWorldGhostBox;
            npcUnit = new NpcSceneWorldGhostBoxUnit();
        }
        else if (npcState.npc is NpcSceneBabyMonster)
        {
            var baby = npcState.npc as NpcSceneBabyMonster;

            if (string.IsNullOrEmpty(baby.visibleGrade))
            {
                npcUnit = new MonsterUnit();
            }
            else
            {
                var lvs = baby.visibleGrade.Split(',');
                if (lvs.Length == 2)
                {
                    int playerLv = ModelManager.Player.GetPlayerLevel();
                    if (playerLv >= StringHelper.ToInt(lvs[0]) && playerLv <= StringHelper.ToInt(lvs[1]))
                    {
                        npcUnit = new MonsterUnit();
                    }
                }
                else
                {
                    TipManager.AddTip(string.Format('NpcSceneBabyMonster.visibleGrade数据异常 ID={0}', baby.id));
                    npcUnit = new MonsterUnit();
                }
            }
        }");
        return npcUnit;
    }

    public void RemoveNpc(long npcUID)
    {
        if (_unitWaitingTrigger == null)
        {
            return;
        }

        if (_npcs.ContainsKey(npcUID))
        {
            if (_npcs[npcUID] is TriggerNpcUnit)
            {
                if (_unitWaitingTrigger != null)
                {
                    _unitWaitingTrigger.RemoveTriggerUnit(_npcs[npcUID] as TriggerNpcUnit);
                }

                var npcstate = (_npcs[npcUID] as TriggerNpcUnit).getNpcState();
                if (npcstate != null)
                {
                    string key = Mathf.RoundToInt(npcstate.x).ToString();
                    if (offsetDic != null)
                    {
                        if (offsetDic.ContainsKey(key))
                        {
                            offsetDic.Remove(key);
                        }
                    }
                }
            }
            _npcs[npcUID].Destroy();
            _npcs.Remove(npcUID);
            npcViewDataManager.RemoveNpc(npcUID);
        }
    }
    //the params format is "npcid:1,npcid:0"
    public void HandlerMissionNpcStatus(string npcStatus, bool needReverse = false)
    {
        //TODO 异步问题还要处理
        //return;

        if (string.IsNullOrEmpty(npcStatus))
        {
            return;
        }

        int sceneId = WorldManager.Instance.GetModel().GetSceneId();
        var status = npcStatus.Split(',');
        for (int i = 0, len = status.Length; i < len; i++)
        {
            string npcs = status[i];
            var npcSplits = npcs.Split(':');
            int npcId = StringHelper.ToInt(npcSplits[0]);
            bool show = npcSplits[1] == "1";

            if (show)
            {
                var npc = DataCache.getDtoByCls<Npc>(npcId);
                if (npc != null)
                {
                    if (npc.sceneId == sceneId)
                    {
                        AddNpcUnit(npc);
                    }
                }
            }
            else
            {
                RemoveNpc(npcId);
            }
        }
    }

    public void Tick()
    {
        if (EnableTrigger)
        {
            if (_unitWaitingTrigger != null)
            {
                _unitWaitingTrigger.Tick();
            }
        }
    }

    public void TriggerTeleport(GameObject go)
    {
        var npcUnit = GetNpcUnit(go);
        if (npcUnit != null)
        {
            npcUnit.Trigger();
        }
    }

    public BaseNpcUnit GetNpcUnit(long npcUID)
    {
        BaseNpcUnit unit = null;
        _npcs.TryGetValue(npcUID, out unit);
        return unit;
    }

    public BaseNpcUnit GetTangNpcUnit(int npcID)
    {
        BaseNpcUnit unit = null;
        foreach (BaseNpcUnit basenpc in _npcs.Values)
        {
            if (basenpc.getNpcState().npcId == npcID)
                return basenpc;
        }
        return unit;
    }

    public BaseNpcUnit GetNpcUnit(GameObject go)
    {
        foreach (var npcUnit in _npcs.Values)
        {
            if (npcUnit.GetUnitGO() == go)
            {
                return npcUnit;
            }
        }
        return null;
    }

    public Dictionary<long, BaseNpcUnit> GetNpcUnits()
    {
        return _npcs;
    }

    public void RefinishMissionFlag()
    {
        if (BattleDataManager.DataMgr.IsInBattle)
        {
            return;
        }

        if (_npcs == null || _npcs.Count <= 0)
        {
            return;
        }

        //Debug.LogError("RefinishMissionFlag");
        //GamePlayer.CameraManager.DataMgr.RefinishMissionFlag();
        JSTimer.Instance.CancelCd("RefinishMissionFlag");

        JSTimer.Instance.SetupCoolDown("RefinishMissionFlag", 0.3f, null, () => { DoRefinishMissionFlag(); });
    }

    public void DoRefinishMissionFlag()
    {
        //Debug.LogError("DoRefinishMissionFlag");

        foreach (var tBaseNpcUnit in _npcs.Values)
        {
            var npc = tBaseNpcUnit.GetNpc();
            GameDebuger.TODO(@"if (npc is NpcSceneMonster == false && npc is NpcDoubleTeleport == false)");
            if (npc is NpcSceneMonster == false && npc is NpcDoubleTeleport == false)
            {
                tBaseNpcUnit.SetMissionNpcMark(false);
            }
        }
    }

    public void DoReFinishSignleMissionFlag(Npc npc)
    {
        if (_npcs.ContainsKey(npc.id))
        {
            var tBaseNpcUnit = _npcs[npc.id];
            //Npc tNpc = npc;
            var tNpc = tBaseNpcUnit.GetNpc();
            GameDebuger.TODO(@"if (tNpc is NpcSceneMonster == false && tNpc is NpcDoubleTeleport == false)");
            if (tNpc is NpcSceneMonster == false && tNpc is NpcDoubleTeleport == false)
            {
                tBaseNpcUnit.SetMissionNpcMark(true);
            }
        }
    }

    //是否动态公共怪， 如果是， 则战斗时调用 SceneService.battle(long npcUniqueId){
    public bool IsDynamicCommonNpc(long id)
    {
        var commonNpcStates = WorldManager.Instance.GetModel().NpcsDic;
        return null != commonNpcStates && commonNpcStates.ContainsKey(id);

        return false;
    }

    public void ResetWaitingTrigger()
    {
        if (_unitWaitingTrigger != null)
        {
            _unitWaitingTrigger.Reset();
        }
    }

#region NPC重叠处理

    private SceneNpcDto overlapOffset(SceneNpcDto npcState)
    {
        string xStr = Mathf.RoundToInt(npcState.x).ToString();
        string key = xStr;
        if (offsetDic.ContainsKey(key))
        {
            float distance = Mathf.Abs(offsetDic[key].z - npcState.z);
            if (offsetDic[key].id != npcState.id && (Npc.NpcType)npcState.npc.type == Npc.NpcType.Monster && distance < 3)
            {
                bool isOffset = false;
                key = Mathf.RoundToInt(npcState.x + 1f).ToString();
                if (SceneHelper.IsCanWalkScope(new Vector3(npcState.x + 1f, 0f, npcState.z)) &&
                    (offsetDic.ContainsKey(key) == false))
                {
                    npcState.x += 1f;
                    isOffset = true;
                    offsetDic[key] = npcState;
                }

                key = Mathf.RoundToInt(npcState.x - 1f).ToString();
                if (SceneHelper.IsCanWalkScope(new Vector3(npcState.x - 1f, 0f, npcState.z)) &&
                    offsetDic.ContainsKey(key) == false && isOffset == false)
                {
                    npcState.x -= 1f;
                    offsetDic[key] = npcState;
                    isOffset = true;
                }

                key = Mathf.RoundToInt(npcState.x + 2f).ToString();
                if (SceneHelper.IsCanWalkScope(new Vector3(npcState.x + 2f, 0f, npcState.z)) &&
                    offsetDic.ContainsKey(key) == false && isOffset == false)
                {
                    npcState.x += 2f;
                    offsetDic[key] = npcState;
                    isOffset = true;
                }

                key = Mathf.RoundToInt(npcState.x - 2f).ToString();
                if (SceneHelper.IsCanWalkScope(new Vector3(npcState.x - 2f, 0f, npcState.z)) &&
                    offsetDic.ContainsKey(key) == false && isOffset == false)
                {
                    npcState.x -= 2f;
                    offsetDic[key] = npcState;
                    isOffset = true;
                }

                if (isOffset == false)
                {
                    if (SceneHelper.IsCanWalkScope(new Vector3(npcState.x + 1f, 0f, npcState.z)))
                    {
                        npcState.x += 1f;
                    }
                }
            }
        }
        else
        {
            offsetDic[key] = npcState;
        }
        return npcState;
    }

#endregion

    public void Dispose()
    {
        JSTimer.Instance.CancelCd("RefinishMissionFlag");
        JSTimer.Instance.CancelTimer("NpcViewManager_QuadTree_Check");
        if (_unitWaitingTrigger != null)
        {
            _unitWaitingTrigger.Destroy();
            _unitWaitingTrigger = null;
        }
        if (offsetDic != null)
        {
            offsetDic.Clear();
        }

        GameDebuger.TODO(@"GameEventCenter.RemoveListener(GameEvent.Player_OnPlayerGradeUpdate, CanSeePreciousBox);");
        _isInit = false;
        npcViewDataManager.Dispose();
        if(_npcs != null)
        {
            foreach(var npcUnit in _npcs.Values)
            {
                npcUnit.Destroy();
            }

            _npcs.Clear();
        }
    }
}