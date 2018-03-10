// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  WorldModel.cs
// Author   : willson
// Created  : 2014/12/2 
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using AppDto;
using UniRx;

public interface IWorldModel
{
    string SceneName { get; }
    PlayerGuildInfoDto PlayerGuildInfoDto { get; }
    int SceneId { get; }
}


public partial class WorldModel : IModuleModel
, IWorldModel
{
    private SceneDto _sceneDto;

    public Dictionary<long, SceneNpcDto> NpcsDic{ get; private set;}
    private WorldModelData _data = null;


    //玩家公会信息的改变（仅从SceneObjectNotify通知改变）
    private static Subject<Unit> playerGuildInfoStream = null;
    public static UniRx.IObservable<Unit> PlayerGuildInfoStream { get { return playerGuildInfoStream; } }

    public static UniRx.IObservableExpand<IWorldModelData> Stream
    {
        get
        {
            if (stream == null)
                stream = new Subject<IWorldModelData>();
            return stream;
        }
    }
    private static UniRx.Subject<IWorldModelData> stream = null;

    private CompositeDisposable _disposable = null;

    public static WorldModel Create()
    {
        var model = new WorldModel();
        model.Init();
        return model;
    }

    private WorldModel()
    {
    }

    private void Init(){
        _disposable = new CompositeDisposable();
        _data = new WorldModelData();
        NpcsDic = new Dictionary<long, SceneNpcDto>();
        if (playerGuildInfoStream == null)
            playerGuildInfoStream = new Subject<Unit>();
        if (stream == null)
            stream = new Subject<IWorldModelData>();
        _disposable.Add(NotifyListenerRegister.RegistListener<TeamSceneNotify>(noti =>
        {
            _data.UpdateWithSceneNotify(noti);
            stream.OnNext(_data);
        }));        
        _disposable.Add(NotifyListenerRegister.RegistListener<LeaveTeamNotify>(noti =>
        {
            _data.UpdateWithLeaveTeamNotify(noti);
            stream.OnNext(_data);
        }));
        _disposable.Add(NotifyListenerRegister.RegistListener<BattleDemoRequestNotify>(noti =>
        {
            BattleRequestNotify(noti);
        }));
        _disposable.Add(NotifyListenerRegister.RegistListener<RefuseBattleDemoNotify>(noti =>
        {
            RefuseBattleNotify(noti);
        }));

        _disposable.Add(NotifyListenerRegister.RegistListener<HearsayNotify>(noti =>
        {
            ChatNotify chatNotify = new ChatNotify(){ channelId = (int)AppDto.ChatChannel.ChatChannelEnum.System,content = noti.content,lableType = 1};
            ChatDataMgr.DataMgr.SendSystemMessage(chatNotify);
        }));
    }

    public string SceneName {
        get { 
            var dto = GetSceneDto();
            if (dto == null)
                return string.Empty;
            return dto.name;
        }
    }

    public int SceneId
    {
        get
        {
            var dto = GetSceneDto();
            if (dto == null)
                return -1;
            return dto.id;
        }
    }

    //公会信息
    public PlayerGuildInfoDto PlayerGuildInfoDto
    {
        get
        {
            var sceneDto = GetSceneDto();
            if (sceneDto == null) return null;
            var dto = sceneDto.objects.Find(e => e.id == ModelManager.Player.GetPlayerId());
            if (dto == null) return null;
            if(dto is ScenePlayerDto)
            {
                var scenePlayerDto = dto as ScenePlayerDto;
                return scenePlayerDto.guildInfoDto;
            }
            return null;
        }
    }

    #region Debug
    public string DumpPlayerDicInfo()
    {
        System.Text.StringBuilder debugInfo = new System.Text.StringBuilder();
        foreach (ScenePlayerDto playerDto in _data._playersDic.Values)
        {
            GameDebuger.TODO(@"debugInfo.AppendLine(string.Format('playId:{0} status:{1} index:{2} inBattle:{3} teamUID:{4} haseCode:{5}',
                                               playerDto.id,
                                               playerDto.teamStatus,
                                               playerDto.teamIndex,
                                               playerDto.inBattle, playerDto.teamUniqueId,
                                               playerDto.GetHashCode()));");
            debugInfo.AppendLine(string.Format("playId:{0} haseCode:{1}",
                playerDto.id,
                playerDto.GetHashCode()));
        }

        return debugInfo.ToString();
    }
    #endregion

    #region Getter
    public SceneDto GetSceneDto()
    {
        return _sceneDto;
    }

    public int GetSceneId()
    {
        if (_sceneDto == null) return -1;
        return _sceneDto.id;
    }

    public string GetBattleSceneName()
    {
        string battleSceneName = "Battle_" + _sceneDto.sceneMap.battleMapId;
        return battleSceneName;
    }

    public string GetWorldMusic()
    {
        if (_sceneDto == null || string.IsNullOrEmpty(_sceneDto.sceneMap.music))
        {
            if (ModelManager.Player.GetPlayer() == null)
            {
                return "";
            }
            else
            {
                int sceneId = ModelManager.Player.GetPlayer().sceneId;
                SceneMap sceneMap = DataCache.getDtoByCls<SceneMap>(sceneId);
                if (sceneMap == null || string.IsNullOrEmpty(sceneMap.music))
                {
                    return "music_world_1001";
                }
                else
                {
                    return sceneMap.music;
                }
            }
        }
        else
        {
            return _sceneDto.sceneMap.music;
        }
    }

    public Dictionary<long, ScenePlayerDto> GetPlayersDic()
    {
        return _data._playersDic;
    }

    public ScenePlayerDto GetPlayerDto(long playerId)
    {
        if (_sceneDto != null)
        {
            if (_data._playersDic.ContainsKey(playerId))
                return _data._playersDic[playerId];
        }

        return null;
    }

    public bool GetPlayerBattleStatus(long playerId)
    {
        GameDebuger.TODO(@"if (_sceneDto != null)
        {
            if (_playersDic.ContainsKey(playerId))
                return _playersDic[playerId].inBattle;
        }");

        return false;
    }

    public ScenePlayerDto GetTeamLeader(long teamUID)
    {
        foreach (ScenePlayerDto player in _data._playersDic.Values)
        {
            if (teamUID == player.teamId&& player.teamStatus == (int)TeamMemberDto.TeamMemberStatus.Leader)
                return player;
        }

        return null;
    }

//    public bool GetSceneNpcShowState(int npcId)
//    {
//        if (_sceneDto == null || _sceneDto.sceneNpcStates == null) return false;
//
//        for (int i = 0; i < _sceneDto.sceneNpcStates.Count; ++i)
//        {
//            if (_sceneDto.sceneNpcStates[i].npcId == npcId)
//                return _sceneDto.sceneNpcStates[i].show;
//        }
//
//        return false;
//    }

//    public SceneNpcStateDto GetSceneNpcState(int npcId)
//    {
//        if (_sceneDto == null || _sceneDto.sceneNpcStates == null) return null;
//
//        for (int i = 0; i < _sceneDto.sceneNpcStates.Count; ++i)
//        {
//            if (_sceneDto.sceneNpcStates[i].npcId == npcId)
//                return _sceneDto.sceneNpcStates[i];
//        }
//
//        return null;
//    }
    #endregion

    #region Game Logic
    public void SetupSceneDto(SceneDto sceneDto)
    {
        _sceneDto = sceneDto;
        _data._playersDic.Clear();
        if (_sceneDto != null)
        {
            long ownPlayerId = ModelManager.Player.GetPlayerId();
            var commonNpcStates = WorldManager.Instance.GetModel().NpcsDic;
            commonNpcStates.Clear();
            if (null == _sceneDto.objects)
                GameDebuger.Log("场景中没有玩家数据！");
            else
            {
                if (null != _sceneDto && null != _sceneDto.objects)
                {
                    for (int i = 0; i < _sceneDto.objects.Count; i++)
                    {
                        SceneNpcDto npcDto = _sceneDto.objects[i] as SceneNpcDto;
                        ScenePlayerDto playerDto = _sceneDto.objects[i] as ScenePlayerDto;
                        if(null != playerDto) {
                            //是玩家
                            _data._playersDic.Add(playerDto.id,playerDto);
                            if(playerDto.id == ownPlayerId)
                            {
                                GameDebuger.TODO(@"ModelManager.Player.UpdateTransformModelId(playerDto.transformModelId);");
                            }
                        }
                        if(npcDto != null) {
                            //如果是动态NPC就加进动态NPC列表
                            if(!WorldManager.Instance.GetModel().NpcsDic.ContainsKey(npcDto.id))
                            {
                                WorldManager.Instance.GetModel().NpcsDic.Add(npcDto.id,npcDto);
                            }
                        }
                        stream.OnNext(_data);
                    }               
                }   
            }
        }
    }

    public void AddPlayer(ScenePlayerDto playerDto)
    {
        //如果存在，直接更新已有玩家数据
        if (_data._playersDic.ContainsKey(playerDto.id))
        {
            _data._playersDic[playerDto.id] = playerDto;
            GameEventCenter.SendEvent(GameEvent.World_OnUpdatePlayer, playerDto); // todo fish 处理playerView的数据同步和创建
        }
        else
        {
            _data._playersDic.Add(playerDto.id, playerDto);
            GameEventCenter.SendEvent(GameEvent.World_OnAddPlayer, playerDto);  // todo fish  同上
        }
        SendGuildInfo(playerDto); //当前玩家公会信息仅从SceneObjectNotify通知改变,stream是由多个Notify触发，满足不了。
        stream.OnNext(_data);
    }

    //公会信息的改变，只能够从SceneObjectNotify获取
    public void SendGuildInfo(ScenePlayerDto dto)
    {
        if (dto != null && dto.id == ModelManager.Player.GetPlayerId())
        {
            playerGuildInfoStream.OnNext(new Unit());
        }
    }
    public void UpdatePlayerPos(long playerId, float x, float y, float z)
    {
        if (_data._playersDic.ContainsKey(playerId))
        {
            ScenePlayerDto playerDto = _data._playersDic[playerId];
            playerDto.x = x;
            playerDto.y = y;
            playerDto.z = z;
            GameEventCenter.SendEvent(GameEvent.World_OnUpdatePlayerPos, playerId, x, y, z);
        }
    }

    public void ChangePlayerTeamStatus(long playerId, TeamMemberDto.TeamMemberStatus teamStatus, long teamUID)
    {
        ScenePlayerDto p = null;
        _data._playersDic.TryGetValue(playerId, out p);
        if (p == null) return;
        p.teamStatus = (int)teamStatus;
        p.teamId = teamUID;
        _data.latestPlayerChageTeamStatusSet.Clear();
        _data.latestPlayerChageTeamStatusSet.Add(p.id);
        stream.OnNext(_data);
    }

    public void ChangePlayerBattleStatus(long playerId, bool inBattle)
    {
        if (_data._playersDic.ContainsKey(playerId))
        {
            GameDebuger.TODO(@"_playersDic[playerId].inBattle = inBattle;
            GameEventCenter.SendEvent(GameEvent.World_OnChangeBattleStatus, playerId, inBattle);");
            stream.OnNext(_data);
        }
    }
    #endregion

    #region Notify Handler
    public void HandlePlayerSceneObjectNotify(ScenePlayerDto pScenePlayerDto)
    {
        if (_sceneDto == null) return;
        if (pScenePlayerDto.sceneId == _sceneDto.id)
        {
            AddPlayer(pScenePlayerDto);
            GameDebuger.Log(string.Format("Player EnterScene: id:{0} name:{1}", pScenePlayerDto.id, pScenePlayerDto.name));
        }
    }

    public void HandleSceneObjectRemoveNotify(SceneObjectRemoveNotify notify)
    {
        if (_sceneDto == null) return;
        if (notify.sceneId != _sceneDto.id) return;
        var remove = _data._playersDic.Remove(notify.objId);
        if (remove)
        {
            stream.OnNext(_data);
            GameEventCenter.SendEvent(GameEvent.World_OnRemovePlayer, notify.objId);
        }
        else
        {
            GameDebuger.Log(string.Format("Player LeaveScene: id:{0} sceneId:{1}", notify.objId, notify.sceneId));
        }
    }

    public void HandleSceneObjectWalkNotify(SceneObjectWalkNotify notify)
    {
        if (_sceneDto == null 
            || notify.sceneId != _sceneDto.id)
            return;
        if ((SceneObjectDto.SceneObjectType) notify.objType == SceneObjectDto.SceneObjectType.Player)
        {
            UpdatePlayerPos(notify.id, notify.x, notify.y, notify.z);
        }
        else
        {
            UpdateNpcPos(notify.id, notify.x, notify.y, notify.z);
        }
        stream.OnNext(_data);
    }

    public void UpdatePlayerName(PlayerNameNotify notify)
    {
        ScenePlayerDto p;
        _data._playersDic.TryGetValue(notify.playerId, out p);
        if (p == null) return;
        p.name = notify.name;
        GameEventCenter.SendEvent(GameEvent.World_OnUpdatePlayer, p);
    }

    public void HandleSceneObjectTeleportNotify(SceneObjectTeleportNotify notify)
    {
        if (_data._playersDic.ContainsKey(notify.id))
        {
            ScenePlayerDto playerDto = _data._playersDic[notify.id];
            playerDto.x = notify.x;
            playerDto.z = notify.z;

            if (playerDto.teamStatus == (int)TeamMemberDto.TeamMemberStatus.Leader)
            {
                foreach (var player in _data._playersDic.Values)
                {
                    if (playerDto.teamId == player.teamId && player.teamStatus == (int)TeamMemberDto.TeamMemberStatus.Member)
                    {
                        player.x = notify.x;
                        player.z = notify.z;

                        GameEventCenter.SendEvent(GameEvent.World_OnChangePlayerPos, player.id, player.x, player.y, player.z);
                    }
                }
            }
            stream.OnNext(_data);
            GameEventCenter.SendEvent(GameEvent.World_OnChangePlayerPos, notify.id, notify.x, notify.y, notify.z);
        }
    }


    /**public void HandleKickoutSceneNotify(KickoutSceneNotify notify)
    {
        if (_sceneDto == null) return;
        if (notify.scene.id == _sceneDto.id)
        {
            if (RemovePlayer(notify.player.id))
            {
                GameDebuger.Log(string.Format("Player KickOutScene: id:{0} name:{1}", notify.player.id, notify.player.nickname));
            }
        }
    }*/

    /**public void HandlePlayersEnterScreenNotify(PlayersEnterScreenNotify notify)
    {
        if (_sceneDto == null) return;
        if (notify.scene.id == _sceneDto.id)
        {
            for (int i = 0, count = notify.players.Count; i < count; i++)
            {
                ScenePlayerDto playerDto = notify.players[i];
                AddPlayer(playerDto);
                GameDebuger.Log(string.Format("Player EnterScreen: id:{0} name:{1}", playerDto.id, playerDto.nickname));
            }
        }
    }*/

    //需要更新完WorldModel的所有组队状态信息再广播OnChangeTeamStatusList消息
    /**public void HandlePlayerTeamStatusChangeNotify(PlayerTeamStatusChangeNotify notify)
    {
        if (_sceneDto == null) return;

        HashSet<long> changedPlayerIdSet = new HashSet<long>();
        for (int i = 0; i < notify.playerIds.Count; ++i)
        {
            long playerId = notify.playerIds[i];
            int teamStatus = notify.teamStatuses[i];
            string teamUID = notify.teamUniqueIds[i];
            if (_playersDic.ContainsKey(playerId))
            {
    ScenePlayerDto tPlayerDto = _playersDic[playerId];
    tPlayerDto.teamStatus = teamStatus;
                //状态变更为NOTeam时，将teamUID清空
                GameDebuger.TODO(@"if (teamStatus == PlayerDto.PlayerTeamStatus_NoTeam)
                tPlayerDto.teamUniqueId = "";
                else
                tPlayerDto.teamUniqueId = teamUID;");

                changedPlayerIdSet.Add(playerId);
            }
        }

    for (int i = 0; i < notify.indexPlayerIds.Count; i++) {
    long playerId = notify.indexPlayerIds[i];
    if (_playersDic.ContainsKey(playerId)) {
    //状态变更为NOTeam时，将teamUID清空
    _playersDic[playerId].inTeamPlayerCount = notify.inTeamPlayerCount;

    if (!changedPlayerIdSet.Contains(playerId))
        changedPlayerIdSet.Add(playerId);
    }
    }

        //更新玩家队伍索引值
        if (notify.indexPlayerIds.Count > 0)
        {
            for (int i = 0; i < notify.indexPlayerIds.Count; ++i)
            {
                long playerId = notify.indexPlayerIds[i];
                if (_playersDic.ContainsKey(playerId))
                {
                    _playersDic[playerId].teamIndex = i;
                    changedPlayerIdSet.Add(playerId);
                }
            }
        }

        foreach (long playerId in changedPlayerIdSet)
        {
            GameDebuger.TODO(@"GameEventCenter.SendEvent(GameEvent.World_OnChangeTeamStatus, playerId);");
        }
    }*/

    /**public void HandleMainCharactorUpgradeSceneNotify(MainCharactorUpgradeSceneNotify notify)
    {
        if (_playersDic.ContainsKey(notify.playerId))
        {
            _playersDic[notify.playerId].grade = notify.level;
        }
    }*/

    /**public void HandleBattleSceneNotify(BattleSceneNotify notify)
    {
        for (int i = 0; i < notify.leaderPlayerIds.Count; ++i)
        {
            ChangePlayerBattleStatus(notify.leaderPlayerIds[i], notify.inBattle);

            ScenePlayerDto leaderDto = null;
            if (_playersDic.TryGetValue(notify.leaderPlayerIds[i], out leaderDto))
            {
                if (leaderDto.teamStatus == (int)TeamMemberDto.TeamMemberStatus.Leader )
                {
                    //更新属于该队伍的队员战斗状态
                    foreach (ScenePlayerDto teamMember in _playersDic.Values)
                    {
                        if (teamMember.teamStatus == (int)TeamMemberDto.TeamMemberStatus.Member && teamMember.teamUniqueId == leaderDto.teamUniqueId)
                        {
                            ChangePlayerBattleStatus(teamMember.id, notify.inBattle);
                        }
                    }
                }
            }
        }
    }*/

//    public void HandlePlayerWeaponChange(WeaponNotify notify)
//    {
//        if (TeamModel.ShowDebugInfo)
//        {
//            GameDebuger.Log(string.Format('WeaponNotify id:{0} wpmodel:{1}', notify.playerId, notify.wpmodel).WrapColorWithLog());
//        }
//        if (_playersDic.ContainsKey(notify.playerId))
//        {
//            _playersDic[notify.playerId].dressInfoDto.wpmodel = notify.wpmodel;
//
//            if (notify.playerId == ModelManager.Player.GetPlayerId())
//            {
//                ModelManager.Player.GetPlayer().dressInfoDto.wpmodel = notify.wpmodel;
//            }
//
//            GameEventCenter.SendEvent(GameEvent.World_OnChangeWeapon, notify.playerId, notify.wpmodel);
//        }
//
//        ModelManager.Arena.OpponentWeaponChange(notify.playerId, notify.wpmodel);
//    }

//    public void HandlePlayerWeaponEffectChange(WeaponEffectNotify notify)
//    {
//        if (TeamModel.ShowDebugInfo)
//        {
//            GameDebuger.Log(string.Format('WeaponEffectNotify id:{0} wpmodel:{1}', notify.playerId, notify.weaponEffectId).WrapColorWithLog());
//        }
//        if (_playersDic.ContainsKey(notify.playerId))
//        {
//            _playersDic[notify.playerId].dressInfoDto.weaponEffect = notify.weaponEffectId;
//
//            if (notify.playerId == ModelManager.Player.GetPlayerId())
//            {
//                ModelManager.Player.GetPlayer().dressInfoDto.weaponEffect = notify.weaponEffectId;
//            }
//
//            GameEventCenter.SendEvent(GameEvent.World_OnChangeWeaponEff, notify.playerId, notify.weaponEffectId);
//        }
//    }

//    public void HandleHallowSpriteChange(HallowSpriteNotify notify)
//    {
//        HandleHallowSpriteChange(notify.playerId, notify.spriteId);
//    }

//    public void HandleHallowSpriteChange(long  pPlayerId,int pHallowSpriteId)
//    {
//        if (TeamModel.ShowDebugInfo)
//        {
//            GameDebuger.Log(string.Format('HallowSpriteNotify id:{0} spriteId:{1}', pPlayerId, pHallowSpriteId).WrapColorWithLog());
//        }
//        if (_playersDic.ContainsKey(pPlayerId))
//        {
//            _playersDic[pPlayerId].dressInfoDto.hallowSpriteId = pHallowSpriteId;
//
//            if (pPlayerId == ModelManager.Player.GetPlayerId())
//            {
//                ModelManager.Player.GetPlayer().dressInfoDto.hallowSpriteId = pHallowSpriteId;
//            }
//
//            GameEventCenter.SendEvent(GameEvent.World_OnHallowSpriteNotify, pPlayerId, pHallowSpriteId);
//        }
//
//        ModelManager.Arena.OpponentHallowSpriteChange(pPlayerId, pHallowSpriteId);
//    }

//    public void HandlePlayerTitleChange(PlayerTitleNotify notify)
//    {
//        if (_playersDic.ContainsKey(notify.playerId))
//        {
//            _playersDic[notify.playerId].titleId = notify.titleId;
//            _playersDic[notify.playerId].titleName = notify.titleName;
//            GameEventCenter.SendEvent(GameEvent.World_OnChangePlayerTitle, notify.playerId);
//        }
//    }

//    public void HandleNicknameNotify(NicknameNotify notify)
//    {
//        if (_playersDic.ContainsKey(notify.playerId))
//        {
//            ScenePlayerDto playerDto = _playersDic[notify.playerId];
//            playerDto.nickname = notify.nickname;
//            GameEventCenter.SendEvent(GameEvent.World_OnChangePlayerTitle, notify.playerId);
//        }
//    }

//    public void HandlePlayerTitleChangeRefresh(PlayerFereRenameNotify notify)
//    {
//		HandlePlayerTitleChangeRefresh(notify.playerId,notify.fereBeforeName, notify.fereNowName);
//    }

	public void HandlePlayerTitleChangeRefresh(long pUserId,string pBeforeName,string pNewName)
	{
		GameDebuger.TODO(@"if (_playersDic.ContainsKey(pUserId))
        {
            _playersDic[pUserId].titleName = _playersDic[pUserId].titleName.Replace(pBeforeName, pNewName);

                  GameEventCenter.SendEvent(GameEvent.World_OnChangePlayerTitle, pUserId);
        }");
	}

//    public void HandleLocationNotify(LocationNotify notify)
//    {
//        if (_playersDic.ContainsKey(notify.playerId))
//        {
//            ScenePlayerDto playerDto = _playersDic[notify.playerId];
//            playerDto.closedLocation = notify.closedLocation;
//            playerDto.locationInfo = notify.locationInfo;
//        }
//    }

//	public void HandleIndentureFinishNotify(IndentureFinishNotify notify)
//	{
//		ScenePlayerDto tScenePlayerDto;
//		if (_playersDic.TryGetValue(notify.playerId,out tScenePlayerDto))
//		{
//			tScenePlayerDto.masterId = notify.masterId;
//			tScenePlayerDto.masterName = notify.masterName;
//
//          GameEventCenter.SendEvent(GameEvent.World_OnChangeMaster, notify.playerId);
//		}
//	}

    /**public void HandlePlayerDyeChange(PlayerDyeNotify notify)
    {
        if (_playersDic.ContainsKey(notify.playerId))
        {
            _playersDic[notify.playerId].dressInfoDto.hairDyeId = notify.hairDyeId;
            _playersDic[notify.playerId].dressInfoDto.dressDyeId = notify.dressDyeId;
            _playersDic[notify.playerId].dressInfoDto.accoutermentDyeId = notify.accoutermentDyeId;
            GameEventCenter.SendEvent(GameEvent.World_OnChangePlayerDye, notify.playerId);
        }
    }*/

    /**public void HandlePlayerScale(PlayerModelScaleUpdateNotify notify)
    {
        if (_playersDic.ContainsKey(notify.playerId))
        {
            float scale = 1;
            long expireAt = 0;
            if (notify.stateDto != null)
            {
                scale = notify.stateDto.scale;
                expireAt = notify.stateDto.expireAt;
            }
            _playersDic[notify.playerId].scale = scale;
            GameEventCenter.SendEvent(GameEvent.World_OnChangePlayerScale, notify.playerId, expireAt, scale);
        }
    }*/

    /**public void HandlePlayerModelChange(PlayerTransformNotify notify)
    {
        if (_playersDic.ContainsKey(notify.playerId))
        {
            _playersDic[notify.playerId].transformModelId = notify.modelId;
            GameEventCenter.SendEvent(GameEvent.World_OnChangePlayerModel, notify.playerId);
        }
    }*/

    /**public void HandleFashionDressChangeNotify(FashionDressChangeNotify notify)
    {
        if (_playersDic.ContainsKey(notify.playerId))
        {
    ScenePlayerDto ScenePlayerDto =  _playersDic[notify.playerId];
    ScenePlayerDto.dressInfoDto.showDress = notify.showDress;
            if (notify.showDress && notify.fashionDressIds.Count > 0)
            {
    ScenePlayerDto.dressInfoDto.fashionDressIds = notify.fashionDressIds;
            }
            else
            {
    ScenePlayerDto.dressInfoDto.fashionDressIds = null;
            }

            GameDebuger.TODO(@"if (notify.playerId == ModelManager.Player.GetPlayerId())
            {
                ModelManager.Player.GetPlayer().dressInfoDto.showDress = notify.showDress;
            ModelManager.Player.GetPlayer().dressInfoDto.fashionDressIds = ScenePlayerDto.dressInfoDto.fashionDressIds;
            }

            GameEventCenter.SendEvent(GameEvent.World_OnChangePlayerModel, notify.playerId);");
        }
    }*/

	/**public void HandleFactionCharactorChangeNotify(FactionCharactorChangeNotify notify)
    {
        if (_playersDic.ContainsKey(notify.playerId))
        {
            ScenePlayerDto ScenePlayerDto =  _playersDic[notify.playerId];
            GameDebuger.Log(string.Format("FactionCharactorChangeNotify old={0}/{1} new={2}/{3}", 
                ScenePlayerDto.factionId, ScenePlayerDto.charactorId, notify.factionId, notify.charactorId));

            ScenePlayerDto.factionId = notify.factionId;
            ScenePlayerDto.faction = null;
            if (ScenePlayerDto.charactorId != notify.charactorId)
            {
                ScenePlayerDto.charactorId = notify.charactorId;
                ScenePlayerDto.charactor = null;
                GameEventCenter.SendEvent(GameEvent.World_OnChangePlayerModel, notify.playerId);");
            }
        }
    }*/

//	public void HandlePlayerRideChangeNotify(PlayerRideChangeNotify notify)
//	{
//		if (_playersDic.ContainsKey(notify.playerId))
//		{
//			ScenePlayerDto ScenePlayerDto =  _playersDic[notify.playerId];
//
//			ScenePlayerDto.moveSpeed = notify.moveSpeed;
//          GameEventCenter.SendEvent(GameEvent.World_OnChangePlayerMoveSpeed, notify.playerId);
//
//			ScenePlayerDto.playerShortRide = notify.playerShortRide;
//			if (ScenePlayerDto.playerShortRide.rideMount == null)
//			{
//				Debug.LogError("PlayerRideChangeNotify rideMount=null");
//			}
//            GameDebuger.TODO(@"GameEventCenter.SendEvent(GameEvent.World_OnChangePlayerRide, notify.playerId);");	
//		}
//	}
	
//	public void HandlePlayerShipmentChangeNotify(PlayerShipmentChangeNotify notify) {
//		if (_playersDic.ContainsKey(notify.playerId)) {
//			ScenePlayerDto ScenePlayerDto =  _playersDic[notify.playerId];
//
//			int lastShipmentID = ScenePlayerDto.shipmentShort != null? ScenePlayerDto.shipmentShort.shipmentId : 0;
//			int curShipmentID = notify.shipmentShort != null? notify.shipmentShort.shipmentId : 0;
//
//			ScenePlayerDto.moveSpeed = notify.moveSpeed;
//			ScenePlayerDto.shipmentShort = notify.shipmentShort;
//
//			if (notify.playerId == ModelManager.Player.GetPlayerId()) {
//                GameDebuger.TODO(@"ModelManager.Escort.OnUpdatePlayerShipmentChangeNotify(notify);");
//			}
//
//			if (curShipmentID != lastShipmentID) {
//				PlayerView playerView = WorldManager.DataMgr.GetView().GetPlayerView(notify.playerId);
//				if (playerView) {
//					playerView.SetEscortFlag(true);
//				}
//			}
//
//          GameEventCenter.SendEvent(GameEvent.World_OnChangePlayerMoveSpeed, notify.playerId);
//		}
//	}

//    public void HandleSceneNpcStateDto(SceneNpcStateDto stateDto)
//    {
//        if (_sceneDto.sceneNpcStates != null)
//        {
//            for (int i = 0; i < _sceneDto.sceneNpcStates.Count; ++i)
//            {
//                if (_sceneDto.sceneNpcStates[i].npcId == stateDto.npcId)
//                {
//                    _sceneDto.sceneNpcStates[i].show = stateDto.show;
//                    return;
//                }
//            }
//
//            _sceneDto.sceneNpcStates.Add(stateDto);
//        }
//    }
    #endregion

    #region npc行走

    public void UpdateNpcPos(long npcUuid, float x, float y, float z)
    {
        if (NpcsDic != null && NpcsDic.Count > 0)
        {
            for (int index = 0; index < NpcsDic.Count; index++)
            {
                if (NpcsDic[index].id == npcUuid)
                {
                    NpcsDic[index].x = x;
                    NpcsDic[index].y = y;
                    NpcsDic[index].z = z;
                    GameEventCenter.SendEvent(GameEvent.World_OnUpdateNpcPos, npcUuid, x, y, z);
                }
            }
        }
    }

    #endregion

	public void Dispose()
	{
	    _data.Dispose();
	    _data = null;
	}
    
    //切磋邀请弹窗
        private static void BattleRequestNotify(BattleDemoRequestNotify notify)
        {
            var controller = ProxyBaseWinModule.Open();
            var title = "切磋";
            var txt = string.Format("{0}向你发起切磋,是否接受切磋(切磋不会有失败损失)", 
                notify.resquestName.WrapColor(ColorConstantV3.Color_Green_Strong_Str));
            if (TeamDataMgr.DataMgr.HasTeam())
            {
                var sb = new StringBuilder();
                sb.AppendLine(txt);
                var namelist = string.Empty;
                TeamDataMgr.DataMgr.GetAllAwayTeamMember().ForEachI((m, idx) =>
                {
                    if (idx > 0)
                        namelist += "," + m.nickname;
                    else
                        namelist += m.nickname;
                });
                
                if (!string.IsNullOrEmpty(namelist) && TeamDataMgr.DataMgr.IsLeader())
                     sb.Append(string.Format("当前队员{0}尚未归队", namelist.ToString().WrapColor(ColorConstantV3.Color_Green_Strong_Str)));
                
                BaseTipData data = BaseTipData.Create(title, sb.ToString(), 30, () =>
                {
                    BattleDataManager.BattleNetworkManager.BattleDemo(notify.resquestId);
                }, () =>
                {
                    BattleDataManager.BattleNetworkManager.BattleRefuse(notify.resquestId);
                }, "拒绝");
                controller.InitView(data);
            }
            else
            {
                BaseTipData data = BaseTipData.Create(title, txt, 30, () =>
                {
                    BattleDataManager.BattleNetworkManager.BattleDemo(notify.resquestId);
                }, () =>
                {
                    BattleDataManager.BattleNetworkManager.BattleRefuse(notify.resquestId);
                }, "拒绝");
                controller.InitView(data);
            }
        }

        //拒绝切磋邀请
    private static void RefuseBattleNotify(RefuseBattleDemoNotify notify)
        {
            TipManager.AddTip(string.Format("很遗憾,{0}拒绝了你的切磋邀请", 
                notify.resquestName.WrapColor(ColorConstantV3.Color_Green_Str)));
        }
}