// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  WorldView.cs
// Author   : willson
// Created  : 2014/12/2 
// Porpuse  : 
// **********************************************************************

using System;
using UnityEngine;
using System.Collections.Generic;
using AppDto;
using UniRx;

public class WorldView
{
    public static bool UsePool = true;
    private WorldModel _worldModel;
    private bool _isInitFinish;

    private NpcViewManager _npcViewManager;

    private HeroView _heroPlayerView;
    private Dictionary<long, PlayerView> _playerViewDic;
    private PlayerViewPool playerViewPool;

    //任务回调监听，主要用来生成主角之后，回调采集和使用物品的任务，刷新采集移动的事件
    public Func<List<Mission>> InitPlayerMission;

    public bool IsInitFinish
    {
        get { return _isInitFinish; }
    }
    public WorldView(WorldModel model)
    {
        _worldModel = model;
        _playerViewDic = new Dictionary<long, PlayerView>(10);
        _npcViewManager = new NpcViewManager();
        playerViewPool = new PlayerViewPool(this);
    }

    #region Debug
    public string DumpPlayerDicInfo()
    {
        System.Text.StringBuilder debugInfo = new System.Text.StringBuilder();
        foreach (PlayerView playerView in _playerViewDic.Values)
        {
            ScenePlayerDto playerDto = playerView.GetPlayerDto();
            GameDebuger.TODO(@"debugInfo.AppendLine(string.Format('playId:{0} status:{1} index:{2} inBattle:{3} teamUID:{4} hashCode:{5}',
                                               playerDto.id,
                                               playerDto.teamStatus,
                                               playerDto.teamIndex,
                                               playerDto.inBattle, playerDto.teamUniqueId,
                                               playerDto.GetHashCode()));");
            debugInfo.AppendLine(string.Format("playId:{0} hashCode:{1}",
                playerDto.id,
                playerDto.GetHashCode()));
        }

        return debugInfo.ToString();
    }
    #endregion

    #region Getter

    public Vector3 GetRandomNavPoint()
    {
        if (AstarPath.active != null)
        {
            return AstarPath.active.GetNavmeshRandomPoin();
        }
        return Vector3.zero;
    }

    public HeroView GetHeroView()
    {
        return _heroPlayerView;
    }

    public PlayerView GetPlayerView(long playerId)
    {
        PlayerView playerView = null;
        _playerViewDic.TryGetValue(playerId, out playerView);
        return playerView;
    }

    public NpcViewManager GetNpcViewManager()
    {
        return _npcViewManager;
    }
    #endregion

    private bool initListener = false;
    private void InitEvents(){
        if (initListener)
            return;
        initListener = true;
        GameEventCenter.AddListener(GameEvent.World_OnAddPlayer,OnNewPlayerEnterScene);
        GameEventCenter.AddListener(GameEvent.World_OnUpdatePlayer,UpdatePlayerViewInfo);
        GameEventCenter.AddListener(GameEvent.World_OnRemovePlayer,RemovePlayerView);
        GameEventCenter.AddListener(GameEvent.World_OnChangeBattleStatus,UpdatePlayerViewBattleStatus);
        GameEventCenter.AddListener(GameEvent.World_OnUpdatePlayerPos,UpdatePlayerViewPos);
        GameEventCenter.AddListener(GameEvent.World_OnChangePlayerPos,ChangePlayerViewPos);
        GameEventCenter.AddListener(GameEvent.World_OnChangeWeapon,UpdatePlayerWeapon);
        GameEventCenter.AddListener(GameEvent.World_OnChangeWeaponEff,UpdatePlayerWeaponEff);
        GameEventCenter.AddListener(GameEvent.World_OnHallowSpriteNotify,UpdateHallowSprite);
        GameEventCenter.AddListener(GameEvent.World_OnChangePlayerTitle,UpdatePlayerTitle);
        GameEventCenter.AddListener(GameEvent.World_OnChangePlayerDye,UpdatePlayerDye);
        GameEventCenter.AddListener(GameEvent.World_OnChangePlayerScale,UpdatePlayerScale);
        GameEventCenter.AddListener(GameEvent.World_OnChangePlayerModel,UpdatePlayerModel);
        GameEventCenter.AddListener(GameEvent.World_OnUpdateNpcPos,UpdateNpcViewPos);
        GameEventCenter.AddListener(GameEvent.World_OnChangePlayerMoveSpeed,UpdatePlayerMoveSpeed);
        GameEventCenter.AddListener(GameEvent.World_OnChangeMaster,OnChangeMasterHandler);

        GameEventCenter.AddListener(GameEvent.SCENE_TEAM_NOTIFY,OnSceneTeamNotify);
        //        GameEventCenter.AddListener(GameEvent.World_OnChangePlayerRide,UpdatePlayerRide);
    }
    public void InitView()
    {
        InitEvents();
        Clear();
        InitData();
    }


    public void InitData(){
        InitPlayers();
        InitNpc();
        _isInitFinish = true;
        //  重新刷新任务面板
        GameDebuger.TODO(@"ModelManager.MissionView.GetSubMissionMenuListInMainUIExpand();
        //  如果玩家使用宝图到达指定点后却不使用，跳转场景了需要把使用的关闭
        ProxyManager. Backpack.CloseItemQuickUsedExpandPanel();");
    }
    private void OnSceneTeamNotify(List<long> LatestPlayerChageTeamStatusSet)
    {
        TryUpdatePlayerViewTeamStatus(LatestPlayerChageTeamStatusSet);   
    }

    private void OnSceneNoTeamNotify(List<long> NoTeamList)
    {
        TryUpdatePlayerViewTeamStatus(NoTeamList);
    }

    private void InitPlayers()
    {
        var newList = SortPlayers();

        //添加玩家形象
        for (int i = 0, len = newList.Count; i < len; i++)
        {
            ScenePlayerDto playerDto = newList[i];
            AddPlayerView(playerDto);
        }

        if (null != _heroPlayerView)
        {
            GamePlayer.CameraManager.Instance.SceneCamera.OnChangeTarget(_heroPlayerView.cachedTransform);
        }
        //更新玩家队伍状态
        for (int i = 0, len = newList.Count; i < len; i++)
        {
            ScenePlayerDto playerDto = newList[i];
            TryUpdatePlayerViewTeamStatus(playerDto.id);
        }
    }

    private void InitNpc()
    {
        _npcViewManager.Setup(_heroPlayerView);
    }
    //对玩家列表进行排序， 优先主角队伍，其它队伍，主角帮派，主角好友
    private List<ScenePlayerDto> SortPlayers()
    {
        ScenePlayerDto myDto = _worldModel.GetPlayerDto(ModelManager.Player.GetPlayerId());
        List<ScenePlayerDto> myTeamList = new List<ScenePlayerDto>();
        List<ScenePlayerDto> myGuildList = new List<ScenePlayerDto>();
        List<ScenePlayerDto> myFriendList = new List<ScenePlayerDto>();
        List<ScenePlayerDto> otherTeamList = new List<ScenePlayerDto>();
        List<ScenePlayerDto> otherPlayerList = new List<ScenePlayerDto>();

        Dictionary<long, ScenePlayerDto> playersDic = _worldModel.GetPlayersDic();
        foreach (ScenePlayerDto playerDto in playersDic.Values)
        {
            if (playerDto != myDto)
            {
                if (myDto != null && playerDto.teamId == myDto.teamId && playerDto.teamId > 0)
                {
                    myTeamList.Add(playerDto);
                }
                //主角好友
                else if (FriendDataMgr.DataMgr.IsMyFriend(playerDto.id))
                {
                    myFriendList.Add(playerDto);
                }
                //主角队员
                else
                {
                    otherPlayerList.Add(playerDto);
                }
                //主角帮派成员
                GameDebuger.TODO(@"else if (myDto != null && playerDto.guildId == myDto.guildId && playerDto.guildId != 0)
                {
                    myGuildList.Add(playerDto);
                }
                else if (!string.IsNullOrEmpty(playerDto.teamUniqueId))
                {
                    otherTeamList.Add(playerDto);
                }");
                
            }
        }

        var newList = new List<ScenePlayerDto>();
        if (myDto != null)
        {
            newList.Add(myDto);
        }
        newList.AddRange(myTeamList);
        newList.AddRange(myGuildList);
        newList.AddRange(myFriendList);
        newList.AddRange(otherTeamList);
        newList.AddRange(otherPlayerList);

        return newList;
    }
    private void AddPlayerView(ScenePlayerDto playerDto)
    {
        if (playerDto.sceneId != _worldModel.GetSceneId())
        {
            GameDebuger.LogError(string.Format("AddPlayer failed, playerDto.id:{0}, playerDto.sceneId:{1}, _worldModel.GetSceneId:{2}", playerDto.id, playerDto.sceneId, _worldModel.GetSceneId()));
            return;
        }

        if (_playerViewDic.Count > GameDisplayManager.MaxPlayerDataCount)
        {
            GameDebuger.LogError(string.Format("AddPlayer failed for OverMaxPlayerCount ,playerDto.id:{0} maxCount= {1}", playerDto.id, GameDisplayManager.MaxPlayerDataCount));
            return;
        }

        if (_playerViewDic.ContainsKey(playerDto.id))
        {
            ResetPlayerViewPos(playerDto.id, playerDto.x, playerDto.z);
        }
        else
        {
            CreatePlayerView(playerDto);
        }
    }

    private void CreatePlayerView(ScenePlayerDto playerDto)
    {
        PlayerView newPlayerView;
        if (playerDto.id == ModelManager.Player.GetPlayerId())
        {
            newPlayerView = UpdateSelfPlayerView(playerDto);
        }
        else
        {
            newPlayerView = SpawnPlayerView();
            newPlayerView.cachedGameObject.name = "player_" + playerDto.id;
            newPlayerView.SetupPlayerDto(playerDto, false);
        }

        _playerViewDic.Add(playerDto.id, newPlayerView);

        GameDebuger.TODO(@"if (playerDto.walkPoint != null)
        {
            playerDto.x = playerDto.walkPoint.x;
            playerDto.z = playerDto.walkPoint.z;
            Vector3 walkPoint = SceneHelper.GetSceneStandPosition(new Vector3(playerDto.x, 0, playerDto.z), Vector3.zero);
            newPlayerView.WalkToPoint(walkPoint);
        }");
    }

    private PlayerView UpdateSelfPlayerView(ScenePlayerDto playerDto)
    {
        PlayerView newPlayerView;
        if (_heroPlayerView == null)
        {
            var heroViewGo = CreateSelfPlayerView();
            heroViewGo.name = "hero";
            _heroPlayerView = heroViewGo.GetMissingComponent<HeroView>();
        }
        else
        {
            _heroPlayerView.SetUnitActive(true);
        }

        newPlayerView = _heroPlayerView;
        newPlayerView.SetupPlayerDto(playerDto, true);

        // 加载完成如果是挂机状态,则执行挂机操纵
        if (ModelManager.Player.IsAutoFram)
        {
            ModelManager.Player.StartAutoFram();
        }
        else
        {
            _heroPlayerView.SetAutoFram(ModelManager.Player.IsAutoFram);
        }
        if(InitPlayerMission!=null)
            InitPlayerMission();
        return newPlayerView;
    }

    private void OnNewPlayerEnterScene(ScenePlayerDto playerDto)
    {
        AddPlayerView(playerDto);
        TryUpdatePlayerViewTeamStatus(playerDto.id);
    }

    private void UpdatePlayerViewInfo(ScenePlayerDto playerDto)
    {
        PlayerView playerView = null;
        if (_playerViewDic.TryGetValue(playerDto.id, out playerView))
        {
            playerView.UpdatePlayerDto(playerDto);
            playerView.UpdatePlayerName();
        }
    }

    private void OnChangeMasterHandler(long pPlayerId)
    {
        PlayerView playerView = null;
        if (_playerViewDic.TryGetValue(pPlayerId, out playerView))
        {
            playerView.UpdatePlayerName();
        }
    }

    private void RemovePlayerView(long playerId)
    {
        PlayerView playerView = null;
        if (_playerViewDic.TryGetValue(playerId, out playerView))
        {
            GameDebuger.Log("RemovePlayerView " + playerId);
            playerView.DestroyMe();
            _playerViewDic.Remove(playerId);
        }
    }

    private void ResetPlayerViewPos(long playerId, float x, float z)
    {
        PlayerView playerView = null;
        if (_playerViewDic.TryGetValue(playerId, out playerView))
        {
            Vector3 position = SceneHelper.GetSceneStandPosition(new Vector3(x, 0, z), Vector3.zero);
            playerView.ChangeToPoint(position);
        }
    }

    public void TryUpdatePlayerViewTeamStatus(IEnumerable<long> playerIdSet)
    {
        playerIdSet.ForEach(s=>TryUpdatePlayerViewTeamStatus(s));
    }

    private void TryUpdatePlayerViewTeamStatus(long playerId)
    {
        PlayerView playerView = null;
        if (_playerViewDic.TryGetValue(playerId, out playerView))
        {
            playerView.UpdateTeamStatus();
        }
    }

    private void UpdatePlayerViewBattleStatus(long playerId, bool inBattle)
    {
        PlayerView playerView = null;
        if (_playerViewDic.TryGetValue(playerId, out playerView))
        {
            playerView.SetFightFlag(inBattle);
            //进入战斗时手动停止玩家移动
            if (inBattle)
                playerView.StopAndIdle();
        }
    }

    private void UpdatePlayerViewPos(long playerId, float x, float z)
    {
        PlayerView playerView = null;
        if (_playerViewDic.TryGetValue(playerId, out playerView))
        {
            Vector3 position = SceneHelper.GetSceneStandPosition(new Vector3(x, 0, z), Vector3.zero);
            playerView.WalkToPoint(position);
        }
    }

    private void ChangePlayerViewPos(long playerId, float x, float z)
    {
        PlayerView playerView = null;
        if (_playerViewDic.TryGetValue(playerId, out playerView))
        {
            Vector3 position = SceneHelper.GetSceneStandPosition(new Vector3(x, 0, z), Vector3.zero);
            playerView.ChangeToPoint(position);
        }
    }

    private void UpdatePlayerWeapon(long playerId, int wpmodel)
    {
        PlayerView playerView = null;
        if (_playerViewDic.TryGetValue(playerId, out playerView))
        {
            playerView.UpdateWeapon(wpmodel);
        }
    }

    private void UpdateHallowSprite(long playerId, int wpmodel)
    {
        PlayerView playerView = null;
        if (_playerViewDic.TryGetValue(playerId, out playerView))
        {
            playerView.UpdateHallowSprite(wpmodel);
        }
    }

    private void UpdatePlayerWeaponEff(long playerId, int weaponEffId)
    {
        PlayerView playerView = null;
        if (_playerViewDic.TryGetValue(playerId, out playerView))
        {
            playerView.UpdateWeaponEff(weaponEffId);
        }
    }

    private void UpdatePlayerTitle(long playerId)
    {
        PlayerView playerView = null;
        if (_playerViewDic.TryGetValue(playerId, out playerView))
        {
            playerView.UpdatePlayerName();
        }
    }

    private void UpdatePlayerDye(long playerId)
    {
        PlayerView playerView = null;
        if (_playerViewDic.TryGetValue(playerId, out playerView))
        {
            playerView.UpdateModelHSV();
        }
    }

    private void UpdatePlayerScale(long playerId, long expireAt, float scale)
    {
        PlayerView playerView = null;
        if (_playerViewDic.TryGetValue(playerId, out playerView))
        {
            playerView.ChangeSize(scale, expireAt);
        }
    }

    private void UpdatePlayerModel(long playerId)
    {
        PlayerView playerView = null;
        if (_playerViewDic.TryGetValue(playerId, out playerView))
        {
            playerView.UpdateModel();
        }
    }

    private void UpdateNpcViewPos(long npcId, float x, float z)
    {
        BaseNpcUnit npcUnit = _npcViewManager.GetNpcUnit(npcId);
        if (npcUnit != null && (npcUnit is TriggerNpcUnit))
        {
            Vector3 position = SceneHelper.GetSceneStandPosition(new Vector3(x, 0, z), Vector3.zero);
            (npcUnit as TriggerNpcUnit).WalkToPoint(position);
        }
    }

    private void UpdatePlayerRide(long playerId)
    {
        PlayerView playerView = null;
        if (_playerViewDic.TryGetValue(playerId, out playerView))
        {
            playerView.UpdateModel();
        }
    }

    private void UpdatePlayerMoveSpeed(long playerId)
    {
        ScenePlayerDto playerDto = null;
        PlayerView playerView = null;
        if (_playerViewDic.TryGetValue(playerId, out playerView))
        {
            playerDto = playerView.GetPlayerDto();
            //不是队员状态， 就变更速度
            if (playerDto.teamStatus != (int)TeamMemberDto.TeamMemberStatus.Member)
            {
                playerView.UpdatePlayerMoveSpeed(playerDto.moveSpeed);
            }

            //如果是队长状态，需要找到队员把速度调整为队长速度
            if (playerDto.teamStatus == (int)TeamMemberDto.TeamMemberStatus.Leader)
            {
                //更新属于该队伍的队员速度
                foreach (PlayerView otherPlayerView in _playerViewDic.Values)
                {
                    var otherPlayerDto = otherPlayerView.GetPlayerDto();
                    if (otherPlayerDto.teamStatus == (int)TeamMemberDto.TeamMemberStatus.Member && otherPlayerDto.teamId == playerDto.teamId)
                    {
                        otherPlayerView.UpdatePlayerMoveSpeed(playerDto.moveSpeed);
                    }
                }
            }

        }
    }

    public void Clear(){
        foreach (PlayerView playerView in _playerViewDic.Values)
        {
            playerView.DestroyMe();
        }
        _isInitFinish = false;
        _playerViewDic.Clear();
        _npcViewManager.Dispose();

        GameDebuger.TODO(@"MarryPlotManager.Instance.Destroy();
        SedanVisitChangAnPlotManager.Instance.Dispose();");
    }

    public void Destroy()
    {
        initListener = false;
        Clear();
        GameEventCenter.RemoveListener(GameEvent.World_OnAddPlayer,OnNewPlayerEnterScene);
        GameEventCenter.RemoveListener(GameEvent.World_OnUpdatePlayer,UpdatePlayerViewInfo);
        GameEventCenter.RemoveListener(GameEvent.World_OnRemovePlayer,RemovePlayerView);
        GameEventCenter.RemoveListener(GameEvent.World_OnChangeBattleStatus,UpdatePlayerViewBattleStatus);
        GameEventCenter.RemoveListener(GameEvent.World_OnUpdatePlayerPos,UpdatePlayerViewPos);
        GameEventCenter.RemoveListener(GameEvent.World_OnChangePlayerPos,ChangePlayerViewPos);
        GameEventCenter.RemoveListener(GameEvent.World_OnChangeWeapon,UpdatePlayerWeapon);
        GameEventCenter.RemoveListener(GameEvent.World_OnChangeWeaponEff,UpdatePlayerWeaponEff);
        GameEventCenter.RemoveListener(GameEvent.World_OnHallowSpriteNotify,UpdateHallowSprite);
        GameEventCenter.RemoveListener(GameEvent.World_OnChangePlayerTitle,UpdatePlayerTitle);
        GameEventCenter.RemoveListener(GameEvent.World_OnChangePlayerDye,UpdatePlayerDye);
        GameEventCenter.RemoveListener(GameEvent.World_OnChangePlayerScale,UpdatePlayerScale);
        GameEventCenter.RemoveListener(GameEvent.World_OnChangePlayerModel,UpdatePlayerModel);
        GameEventCenter.RemoveListener(GameEvent.World_OnUpdateNpcPos,UpdateNpcViewPos);
        GameEventCenter.RemoveListener(GameEvent.World_OnChangePlayerMoveSpeed,UpdatePlayerMoveSpeed);
        GameEventCenter.RemoveListener(GameEvent.World_OnChangeMaster,OnChangeMasterHandler);
        GameEventCenter.RemoveListener(GameEvent.SCENE_TEAM_NOTIFY, OnSceneTeamNotify);
        GameDebuger.TODO(@"GameEventCenter.RemoveListener(GameEvent.World_OnChangePlayerRide,UpdatePlayerRide);");

        //GamePlayer.CameraManager.Instance.ResetCamera();

    }

    private List<PlayerView> _weddingHidePlayerList;
    public void SetInWeddingAreaPlayerState(bool b)
    {
        if (_weddingHidePlayerList == null)
        {
            _weddingHidePlayerList = new List<PlayerView>();
            foreach (var playerView in _playerViewDic.Values)
            {
                if (SceneHelper.CheckAtWeddingScope(playerView.transform.position))
                {
                    _weddingHidePlayerList.Add(playerView);
                }
            }
        }

        for (int i = 0; i < _weddingHidePlayerList.Count; i++)
        {
            _weddingHidePlayerList[i].SetUnitActive(b);
        }
    }

    public void ClearweddingHidePlayerList()
    {
        if (_weddingHidePlayerList == null) return;
        _weddingHidePlayerList.Clear();
        _weddingHidePlayerList = null;
    }

    private List<PlayerView> _bridalSedanHidePlayerList;
    public void SetInBridalSedanAreaPlayerState(bool b)
    {
        if (_bridalSedanHidePlayerList == null)
        {
            _bridalSedanHidePlayerList = new List<PlayerView>();
            foreach (var playerView in _playerViewDic.Values)
            {
                if (SceneHelper.CheckAtWeddingScope(playerView.transform.position))
                {
                    _bridalSedanHidePlayerList.Add(playerView);
                }
            }
        }

        for (int i = 0; i < _bridalSedanHidePlayerList.Count; i++)
        {
            _bridalSedanHidePlayerList[i].SetUnitActive(b);
        }
    }

    public void ClearBridalSedanHidePlayerList()
    {
        if (_bridalSedanHidePlayerList == null) return;
        _bridalSedanHidePlayerList.Clear();
        _bridalSedanHidePlayerList = null;
    }

    /// <summary>
    /// 隐藏玩家
    /// </summary>
    public void SetHidePlayerView()
    {
        foreach (var playerView in _playerViewDic.Values)
            playerView.SetUnitActive(false);
    }

    public void SetHideOtherView()
    {
        foreach(long key in _playerViewDic.Keys) {
            if(key != ModelManager.Player.GetPlayerId()) {
                _playerViewDic[key].SetUnitActive(false);
            }
        }
    }

    /// <summary>
    /// 显示玩家
    /// </summary>
    public void SetShowPlayerView()
    {
        foreach (var playerView in _playerViewDic.Values)
            playerView.SetUnitActive(true);
    }

    public void UpdatePlayerTitle()
    {
        foreach (PlayerView playerView in _playerViewDic.Values)
        {
            playerView.UpdatePlayerName();
        }
    }

    public void UpdatePlayerName(long playerId)
    {
        UpdatePlayerTitle(playerId);
    }

    #region PlayerView Pool Operation

    private GameObject CreateSelfPlayerView()
    {
        return playerViewPool.CreatePlayerViewGo();
    }

    public void DespawnPlayerView(PlayerView playerView)
    {
        playerViewPool.DespawnPlayerView(playerView);
    }
    public PlayerView SpawnPlayerView()
    {
        return playerViewPool.SpawnPlayerView();
    }
    #endregion

}