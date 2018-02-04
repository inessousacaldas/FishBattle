using AppDto;
using System.Collections.Generic;
using System;
using System.Text;
using BattleNetworkManager = BattleDataManager.BattleNetworkManager;
using MonsterManager = BattleDataManager.MonsterManager;
using BattleInstController = BattleDataManager.BattleInstController;

/// <summary>
/// S1 战斗DEMO 数据模拟类，用于没有与服务端通讯的数据的模拟。
/// @MarsZ
/// </summary>
public static class DemoSimulateHelper
{
    const int SKILL_COUNT_TO_SIMULATE_IF_FAILED = 8;
    //两次释放技能间的间隔时间，可以理解为技能播放时间（认为是同时受击）
    public static int SIMULATE_QUEUE_DELAY = 5000;
    //技能吟唱时间
    public static int SIMULATE_SKILL_CD = 1500;

    private static int SIMULATE_FIGHTER_READY_TIME = 3000;

    public static List<FighterConfigDto> SimulateFighterConfigDtoList(bool pIsFriend = false)
    {
        int tSimulateListLength = 3;
        List<FighterConfigDto> tFighterConfigDtoList = new List<FighterConfigDto>();
        List<int> tSkillIdList = GetSuitableSkillIdList();
        List<Monster> tMonsterList = SimulateMonsterList();
        int tStartSkillIndex, tStartMonsterIndex;
        Monster tMonster;
        for (int tCounter = 0; tCounter < tSimulateListLength; tCounter++)
        {
            tStartSkillIndex = pIsFriend ? (tSkillIdList.Count - 1) : 0;
            tStartMonsterIndex = pIsFriend ? (tMonsterList.Count - 1) : 0;
            tMonster = tMonsterList[tStartMonsterIndex];
            try
            {
                tFighterConfigDtoList.Add(SimulateFighterConfigDtoByMonster(tMonster));    
            }
            catch (Exception e)
            {
                GameDebuger.LogException(e);
            }

            tSkillIdList.RemoveAt(tStartSkillIndex);
            tMonsterList.RemoveAt(tStartMonsterIndex);
        }
        //添加宠物和玩家自己
        AddPlayerMainCharacter(tFighterConfigDtoList, pIsFriend);
        return tFighterConfigDtoList;
    }

    public static List<FighterConfigDto> SimulateFighterConfigDtoListByIDList(List<int> pIDList, bool pIsFriend)
    {
        if (null == pIDList || pIDList.Count <= 0)
        {
            GameDebuger.LogError("SimulateFighterConfigDtoListByIDList failed , pIDList is invalid !");
            return null;
        }
        List<FighterConfigDto> tFighterConfigDtoList = new List<FighterConfigDto>();
        int tID;
        for (int tCounter = 0; tCounter < pIDList.Count; tCounter++)
        {
            tID = pIDList[tCounter];
            tFighterConfigDtoList.Add(SimulateFighterConfigDtoByMonster(tID));
        }
        //添加宠物和玩家自己
        AddPlayerMainCharacter(tFighterConfigDtoList, pIsFriend);
        return tFighterConfigDtoList;
    }

    public static FightersConfigDto SimulateFightersConfigDtoByIDList(List<int> pEnemyIDList, List<int> pFriendIDList)
    {
        FightersConfigDto tFightersConfigDto = new FightersConfigDto();
        tFightersConfigDto.ateam = SimulateFighterConfigDtoListByIDList(pEnemyIDList, true);
        tFightersConfigDto.bteam = SimulateFighterConfigDtoListByIDList(pFriendIDList, false);
        return tFightersConfigDto;
    }

    public static void AddPlayerMainCharacter(List<FighterConfigDto> pFighterConfigDtoList, bool pIsFriend = false)
    {
        FighterConfigDto tFighterConfigDto = pFighterConfigDtoList[0];
        GameDebuger.TODO(@"tFighterConfigDto.characterType = (int)GeneralCharactor.CharactorType.Pet;");
        if (pIsFriend)//add player self
        {
            GameDebuger.TODO(@"tFighterConfigDto.playerId = ModelManager.Player.GetPlayerId();");
            pFighterConfigDtoList.Add(SimulateFighterConfigDtoByPlayerDto(ModelManager.Player.GetPlayer()));
        }
        else
            GameDebuger.TODO(@"tFighterConfigDto.playerId = pFighterConfigDtoList[1].id;");
    }

    public static FighterConfigDto SimulateFighterConfigDtoByPlayerDto(PlayerDto pPlayerDto)
    {
        int tCharacterId = 0;
        string tCharacterName = string.Empty;
        if (null != pPlayerDto)
        {
            tCharacterId = pPlayerDto.charactorId;
            tCharacterName = pPlayerDto.name;
        }
        return SimulateFighterConfigDtoByCharacterInfo(pPlayerDto.id, tCharacterId, tCharacterName);
    }

//    public static FighterConfigDto SimulateFighterConfigDtoByPlayerDto(PlayerDto pPlayerDto, string pSkillIds)
//    {
//        int tCharacterId = 0;
//        string tCharacterName = string.Empty;
//        if (null != pPlayerDto)
//        {
//            tCharacterId = pPlayerDto.charactorId;
//            tCharacterName = pPlayerDto.name;
//        }
//        return SimulateFighterConfigDtoByCharacterInfo(pPlayerDto.id, tCharacterId, tCharacterName, pSkillIds);
//    }

    public static FighterConfigDto SimulateFighterConfigDtoByScenePlayerDto(ScenePlayerDto pScenePlayerDto)
    {
        var tCharacterId = 0;
        var tCharacterName = string.Empty;
        long tPlayerUID = 0;
        if (null != pScenePlayerDto)
        {
            tCharacterId = pScenePlayerDto.charactorId;
            tCharacterName = pScenePlayerDto.name;
            tPlayerUID = pScenePlayerDto.id;
        }
        return SimulateFighterConfigDtoByCharacterInfo(tPlayerUID, tCharacterId, tCharacterName);
    }

    public static FighterConfigDto SimulateFighterConfigDtoByCharacterInfo(long pPlayerUID, int pCharacterId, string pName)
    {
        FighterConfigDto tFighterConfigDto = ModelManager.BattleDemoConfig.GetFighterConfigDtoByCharacterId(pCharacterId);
        if (null == tFighterConfigDto)
            return null;
        return SimulateFighterConfigDtoByCharacterInfo(pPlayerUID, pCharacterId, pName, tFighterConfigDto.activeSkillIds);
    }

    public static FighterConfigDto SimulateFighterConfigDtoByCharacterInfo(long pPlayerUID, int pCharacterId, string pName, string pSkillIds)
    {
        FighterConfigDto tFighterConfigDto = ModelManager.BattleDemoConfig.GetFighterConfigDtoByCharacterId(pCharacterId);
        if (null == tFighterConfigDto)
            return null;
        if (pCharacterId < 0)
            return SimulateFighterConfigDtoByPlayerDto((int)GeneralCharactor.CharactorType.MainCharactor, 1000001, 1000001, UnityEngine.Random.Range(1, 4), "你自己", 
                pSkillIds, 0, tFighterConfigDto.phyAttack, tFighterConfigDto.magicAttack, tFighterConfigDto.phyDefence, tFighterConfigDto.magicDefence, tFighterConfigDto.hp, tFighterConfigDto.speed, tFighterConfigDto.ep);
        else
            return SimulateFighterConfigDtoByPlayerDto((int)GeneralCharactor.CharactorType.MainCharactor, pPlayerUID, pPlayerUID, pCharacterId, pName, 
                pSkillIds, 0, tFighterConfigDto.phyAttack, tFighterConfigDto.magicAttack, tFighterConfigDto.phyDefence, tFighterConfigDto.magicDefence, tFighterConfigDto.hp, tFighterConfigDto.speed, tFighterConfigDto.ep);
    }

    public static FighterConfigDto SimulateFighterConfigDtoByPlayerDto(int pPlayerType, long pFighterId, long pPlayerId, int pCharacterId, string pName, string pActiveSkill, int pPassiveSkill,
                                                                       int pAttack, int pAttackMagic, int pDefence, int pDefenceMagic, int pHP, int pSpeed, int pEP = 0)
    {
        var tFighterConfigDto = new FighterConfigDto();
        tFighterConfigDto.characterType = pPlayerType;
        tFighterConfigDto.id = pFighterId;
        tFighterConfigDto.playerId = pPlayerId;
        tFighterConfigDto.characterId = pCharacterId;
        tFighterConfigDto.activeSkillIds = pActiveSkill;
        tFighterConfigDto.phyAttack = pAttack;
        tFighterConfigDto.magicAttack = pAttackMagic;
        tFighterConfigDto.phyDefence = pDefence;
        tFighterConfigDto.magicDefence = pDefenceMagic;
        tFighterConfigDto.hp = pHP;
        tFighterConfigDto.ep = pEP;
        tFighterConfigDto.speed = pSpeed;
        return tFighterConfigDto;
    }

    public static FighterConfigDto SimulateFighterConfigDtoByMonster(int pMonsterId, int pCharacterId = 1)
    {
        var tMonster = DataCache.getDtoByCls<Monster>(pMonsterId);
        if (null == tMonster)
        {
            GameDebuger.LogError(string.Format("SimulateFighterConfigDtoByMonster failed, null == tMonster , tMonsterId:{0}", pMonsterId));
            return null;
        }
        return SimulateFighterConfigDtoByMonster(tMonster, pCharacterId);
    }

    public static FighterConfigDto SimulateFighterConfigDtoByMonster(Monster pMonster, int pCharacterId = 1)
    {
        return SimulateFighterConfigDtoByPlayerDto((int)GeneralCharactor.CharactorType.Monster, pMonster.id, 0, 
            pCharacterId, pMonster.name, ListToString(pMonster.activeSkillIds), 0, 
            pMonster.attack.ToInt(), pMonster.magicAttack.ToInt(), pMonster.defense.ToInt(), pMonster.magicDefense.ToInt(), pMonster.hp.ToInt(), pMonster.speed.ToInt());
    }

    public static Video SimulateVideo(FightersConfigDto pFightersConfigDto, SkillPreviewInfo pSkillPreviewInfo = null)
    {
        var tDemoVideo = new Video();
        tDemoVideo.id = VideoRoundSimulater.SIMULATE_BATTLE_ID;
        tDemoVideo.retreatable = true;
        tDemoVideo.readyTime = SIMULATE_FIGHTER_READY_TIME;
        tDemoVideo.ateam = SimulateVideoTeam(pFightersConfigDto.ateam, null != pSkillPreviewInfo ? pSkillPreviewInfo.formationA : 1);
        tDemoVideo.bteam = SimulateVideoTeam(pFightersConfigDto.bteam, null != pSkillPreviewInfo ? pSkillPreviewInfo.formationB : 1);
        return tDemoVideo;
    }

    public static PreviewVideo SimulatePreviewVideo(FightersConfigDto pFightersConfigDto, Skill pSkill)
    {
        if (null == pSkill)
            return null;
        var tDemoVideo = SimulateVideo(pFightersConfigDto, pSkill.preview);
        var tPreviewVideo = new PreviewVideo();
        tPreviewVideo.id = tDemoVideo.id;
        tPreviewVideo.retreatable = tDemoVideo.retreatable;
        tPreviewVideo.readyTime = tDemoVideo.readyTime;
        tPreviewVideo.ateam = tDemoVideo.ateam;
        tPreviewVideo.bteam = tDemoVideo.bteam;

        tPreviewVideo.SkillId = pSkill.id;
        return tPreviewVideo;
    }

    public static Monster SimulateMonster(int pId, string pName, int pNameType = 1, string levelFormula = "",
                                          string spellLevelFormula = "")
    {
        Monster tMonster = DataCache.getDtoByCls<Monster>(pId);
        if (null != tMonster) return tMonster;
        return null;
        GameDebuger.LogError(string.Format("DataCache.getDtoByCls<Monster>({0}) failed , simulate this !", pId));
        tMonster = new Monster();
        tMonster.id = pId;
        tMonster.name = pName;
        return tMonster;
    }

    private static List<Monster> SimulateMonsterList()
    {
        var list = DataCache.getArrayByCls<Monster>().Filter(s => s.id <= 10).ToList();
        return list;
    }

    private static string ListToString(List<int> pList)
    {
        if (null == pList || pList.Count <= 0)
            return string.Empty;
        var tStringBuilder = new StringBuilder();
        for (var tCounter = 0; tCounter < pList.Count; tCounter++)
        {
            tStringBuilder = tCounter == 0 
                ? tStringBuilder.Append(pList[tCounter].ToString()) 
                : tStringBuilder.AppendFormat(",{0}", pList[tCounter].ToString());
        }
        return tStringBuilder.ToString();
    }

    private static List<int> StringToList(string pFormattedString)
    {
        if (string.IsNullOrEmpty(pFormattedString))
            return null;
        var tStrings = pFormattedString.Split(',');
        var tList = new List<int>();
        var tIntParsed = 0;
        for (int tCounter = 0, tLen = tStrings.Length; tCounter < tLen; tCounter++)
        {
            if (int.TryParse(tStrings[tCounter], out tIntParsed))
                tList.Add(tIntParsed);
        }
        return tList;
    }

    public static VideoTeam SimulateVideoTeam(List<FighterConfigDto> pEnemyFighterConfigDtoList, int pFormation = 1)
    {
        var tVideoTeam = new VideoTeam();
        var tPlayerIdList = new List<long>(); 
        var tTeamSoldiers = new List<VideoSoldier>();
        if (null != pEnemyFighterConfigDtoList && pEnemyFighterConfigDtoList.Count > 0)
        {
            FighterConfigDto tFighterConfigDto = null;
            VideoSoldier tVideoSoldier = null;
            for (int tCounter = 0; tCounter < pEnemyFighterConfigDtoList.Count; tCounter++)
            {
                tFighterConfigDto = pEnemyFighterConfigDtoList[tCounter];
                if (null == tFighterConfigDto)
                    continue;
                tVideoSoldier = SimulateVideoSoldier(tFighterConfigDto, tCounter + 1);
                if (tVideoSoldier.charactorType == (int)GeneralCharactor.CharactorType.Pet)
                    tVideoSoldier.position += 5;
                tPlayerIdList.Add(tVideoSoldier.id);
                tTeamSoldiers.Add(tVideoSoldier);
            }
        }
        tVideoTeam.playerIds = tPlayerIdList;
        tVideoTeam.teamSoldiers = tTeamSoldiers;
        tVideoTeam.formationId = pFormation;
        tVideoTeam.formation = SimulateFormation(tVideoTeam.formationId);
        return tVideoTeam;
    }

    public static Formation SimulateFormation(int pId)
    {
        return SimulateFormation(pId, "阵法名字" + pId.ToString(), "阵法描述" + pId.ToString(), string.Empty, "阵法信息" + pId.ToString(), "位置信息" + pId.ToString());
    }

    public static Formation SimulateFormation(int pId, string pName, string pDescription, string pDebuffTargetsStr, string pMessageBox, string pPosEffectStr)
    {
        GameDebuger.LogError(string.Format("[TEMP]缺少数据表Formation，暂时模拟之，pId：{0}", pId));
        var tFormation = new Formation();
        tFormation.id = pId;
        tFormation.name = pName;
        tFormation.description = pDescription;
        return tFormation;
    }

    public static VideoSoldier SimulateVideoSoldier(FighterConfigDto pFighterConfigDto, int pPosition)
    {
        VideoSoldier tVideoSoldier = new VideoSoldier();
        if ((GeneralCharactor.CharactorType)pFighterConfigDto.characterType == GeneralCharactor.CharactorType.MainCharactor)
            tVideoSoldier.id = pFighterConfigDto.id;
        else
            tVideoSoldier.id = System.DateTime.Now.Ticks;
        tVideoSoldier.name = (pFighterConfigDto.id == ModelManager.Player.GetPlayerId()) ? ModelManager.Player.GetPlayerName() : ("C" + pFighterConfigDto.id).ToString();
        tVideoSoldier.playerId = pFighterConfigDto.playerId;
        tVideoSoldier.charactorType = pFighterConfigDto.characterType;
        tVideoSoldier.defaultSkillId = GetSkillFromSkillIdsStr(pFighterConfigDto.activeSkillIds);
        tVideoSoldier.skillIds = StringToList(pFighterConfigDto.activeSkillIds);
        tVideoSoldier.hp = pFighterConfigDto.hp;
        tVideoSoldier.maxHp = pFighterConfigDto.hp;
        var tMonster = SimulateMonster((int)tVideoSoldier.id, tVideoSoldier.name);
        if (null != tMonster)
            tVideoSoldier.monsterId = tMonster.id;
        tVideoSoldier.monster = tMonster;
        tVideoSoldier.position = pPosition;
        tVideoSoldier.charactorId = pFighterConfigDto.characterId;
//        tVideoSoldier.charactor = SimulateGeneralCharactor(tVideoSoldier.charactorId);
        return tVideoSoldier;
    }

    //    private static GeneralCharactor SimulateGeneralCharactor(int pCharacterId)
    //    {
    //        GeneralCharactor tGeneralCharactor = new GeneralCharactor();
    //        tGeneralCharactor.id = pCharacterId;
    //        tGeneralCharactor.modelId = 12;
    //        tGeneralCharactor.texture = pCharacterId * 10 + UnityEngine.Random.Range(1, 3);
    //        return tGeneralCharactor;
    //    }

//    private static List<VideoSoldier> tVideoSoldierList = null;

//    public static ActionQueueAddNotify SimulateActionQueueAddNotifyDto(VideoSoldier pVideoSoldier, long pTimeToPlay)
//    {
//        ActionQueueAddNotify tActionQueueAddNotifyDto = new ActionQueueAddNotify();
//        tActionQueueAddNotifyDto.battleId = DemoSimulateHelper.SIMULATE_BATTLE_ID;
//        tActionQueueAddNotifyDto.id = pVideoSoldier.id;
//        tActionQueueAddNotifyDto.name = pVideoSoldier.name;
//        tActionQueueAddNotifyDto.time = pTimeToPlay;
//        tActionQueueAddNotifyDto.durationTime = SIMULATE_QUEUE_DELAY;
//        return tActionQueueAddNotifyDto;
//    }

//    public static ActionQueueRemoveNotify SimulateActionQueueRemoveNotifyDto(long pCharacterUID = 0)
//    {
//        MonsterController tMonsterController = MonsterManager.DataMgr.GetMonsterFromSoldierID(pCharacterUID); 
//        VideoSoldier tVideoSoldier = null;
//        if (null == tMonsterController)
//        {
//            if (null == tVideoSoldierList || tVideoSoldierList.Count <= 0)
//            {
//                TipManager.AddTip("当前没有在队列中的对象");
//                return null;
//            }
//            tVideoSoldier = tVideoSoldierList[0];
//            tVideoSoldierList.RemoveAt(0);
//        }
//        else
//            tVideoSoldier = tMonsterController.videoSoldier;
//
//        ActionQueueRemoveNotify tActionQueueRemoveNotifyDto = new ActionQueueRemoveNotify();
//        tActionQueueRemoveNotifyDto.battleId = DemoSimulateHelper.SIMULATE_BATTLE_ID;
//        tActionQueueRemoveNotifyDto.id = tVideoSoldier.id;
//        return tActionQueueRemoveNotifyDto;
//    }

    public static void TestFighterReadyNotifyDto()
    {
        GameLog.Log_Battle("TestFighterReadyNotifyDto  " );
        GameDebuger.LogError("[单机/非错误]模拟全部准备就绪，正式时删除。 当前本地服务器时间:" + SystemTimeManager.Instance.GetServerTime().ToString("o"));
        if (null == BattleDataManager.DataMgr.BattleDemo.AllVideoSoldierList)
        {
            TipManager.AddTip("当前没有战斗对象");
            return;
        }
        var tVideoSoldierList = new List<VideoSoldier>(BattleDataManager.DataMgr.BattleDemo.AllVideoSoldierList.Values);
        var tPreDateTime = SystemTimeManager.Instance.GetServerTime().AddMilliseconds(SIMULATE_SKILL_CD);
        GameDebuger.Log(string.Format("tPreDateTime GetServerTime :{0}, tPreDateTime:{1}", SystemTimeManager.Instance.GetServerTime().ToString("o"), tPreDateTime.ToString("o")));
        VideoSoldier tVideoSoldier;
//        ActionQueueAddNotifyDto tActionQueueAddNotifyDto;
        FighterReadyNotifyDto tFighterReadyNotifyDto;
        VideoRound tVideoRound = null;
        Action<float,VideoSoldier,long> tAddToQueueDelay = (pCDTime, pVideoSoldier, pDelay) =>
        {
            var tAddToActionQueueTimerName = "AddToActionQueue_" + pVideoSoldier.id.ToString();
            JSTimer.Instance.CancelCd(tAddToActionQueueTimerName);
            GameDebuger.Log(string.Format("tAddToQueueDelay :{0},pCDTime:{1}, pDelay:{2}", tPreDateTime.ToString("o"), pCDTime, DateUtil.UnixTimeStampToDateTime(pDelay).ToString("o")));
            JSTimer.Instance.SetupCoolDown(tAddToActionQueueTimerName, pCDTime, null, () =>
                {
//                    tActionQueueAddNotifyDto = SimulateActionQueueAddNotifyDto(pVideoSoldier, pDelay);
//                    BattleDataManager.DataMgr.BattleDemo.AddToActionQueue(tActionQueueAddNotifyDto);
                        var tVideo = BattleDataManager.DataMgr.BattleDemo.GameVideo;
                        var round = BattleInstController.Instance.VideoRoundsCnt + 1;
                        tVideoRound = VideoRoundSimulater.SimulateVideoRound(tVideo.ateam.teamSoldiers,tVideo.bteam.teamSoldiers,pVideoSoldier.id, round:round);
                        BattleNetworkManager.HanderVideoRound(tVideoRound);
                });
        };
        GameLog.Log_Battle("tVideoSoldierList.Count = " + tVideoSoldierList.Count);
        for (var tCounter = 0; tCounter < tVideoSoldierList.Count; tCounter++)
        {
            tVideoSoldier = tVideoSoldierList[tCounter];
            if (ServiceRequestAction.SimulateNet)
            {
                var tMonsterController = MonsterManager.Instance.GetMonsterFromSoldierID(tVideoSoldier.id);
                if (null != tMonsterController)
                {
                    if (tMonsterController.dead)
                        continue;
                    if (HasType1Buff(tMonsterController.GetBuffs()))
                    {
                        GameDebuger.LogError("[单机/非错误]单机时有封印BUFF的跳过行动回合");
                        continue;
                    }
                }
            }

            tFighterReadyNotifyDto = SimulateFighterReadyNotifyDto(tVideoSoldier);
            BattleNetworkManager.HandlerSoldierReadyNotify(tFighterReadyNotifyDto);
            tAddToQueueDelay((float)tFighterReadyNotifyDto.releaseTime / 1000f, tVideoSoldier, DateUtil.DateTimeToUnixTimestamp(tPreDateTime));
            GameDebuger.Log(string.Format("tPreDateTime :{0}", tPreDateTime.ToString("o")));
            tPreDateTime = tPreDateTime.AddMilliseconds(SIMULATE_QUEUE_DELAY);//技能播放延迟，技能添加到队列多久后播放
        }
    }

    private static bool HasType1Buff(List<VideoBuffAddTargetState> pVideoBuffAddTargetStateList)
    {
        if (null == pVideoBuffAddTargetStateList || pVideoBuffAddTargetStateList.Count <= 0)
            return false;
        VideoBuffAddTargetState tVideoBuffAddTargetState;
        for (int tCounter = 0; tCounter < pVideoBuffAddTargetStateList.Count; tCounter++)
        {
            tVideoBuffAddTargetState = pVideoBuffAddTargetStateList[tCounter];
            if (null != tVideoBuffAddTargetState && tVideoBuffAddTargetState.battleBuff.group == (int)SkillBuff.BuffGroup.SEAL)
                return true;
        }
        return false;
    }

    //    public static void TestRemoveFromShowingActionQueue()
    //    {
    //        List<ActionQueueAddNotifyDto> tVideoSoldierList = BattleDataManager.DataMgr.BattleDemo.CurShowingActionQueueDic.Values.ToList();
    //        if (null == tVideoSoldierList || tVideoSoldierList.Count <= 0)
    //        {
    //            TipManager.AddTip("没有战斗对象");
    //            return;
    //        }
    //        ActionQueueAddNotifyDto tVideoSoldier = tVideoSoldierList[1];
    //        if (null == tVideoSoldier)
    //        {
    //            TipManager.AddTip("获取战斗对象失败");
    //            return;
    //        }
    //        string tString = string.Format("尝试从正在播放的队列中移除id:{0},name:{1}", tVideoSoldier.id, tVideoSoldier.name);
    //        GameDebuger.Log(tString);
    //        TipManager.AddTip(tString);
    //        BattleDataManager.DataMgr.BattleDemo.RemoveFromActionQueueForcibly(tVideoSoldier.id);
    //    }

    //    public static void TestRemoveFromActionQueue()
    //    {
    //        List<ActionQueueAddNotifyDto> tVideoSoldierList = BattleDataManager.DataMgr.BattleDemo.AllActionQueueDic.Values.ToList();
    //        if (null == tVideoSoldierList || tVideoSoldierList.Count <= 0)
    //        {
    //            TipManager.AddTip("没有战斗对象");
    //            return;
    //        }
    //        ActionQueueAddNotifyDto tVideoSoldier = tVideoSoldierList[1];
    //        if (null == tVideoSoldier)
    //        {
    //            TipManager.AddTip("获取战斗对象失败");
    //            return;
    //        }
    //        string tString = string.Format("尝试从队列中移除id:{0},name:{1}", tVideoSoldier.id, tVideoSoldier.name);
    //        GameDebuger.Log(tString);
    //        TipManager.AddTip(tString);
    //        BattleDataManager.DataMgr.BattleDemo.RemoveFromActionQueueForcibly(tVideoSoldier.id);
    //    }

//    public static Skill SimulateSkill(int pId)
//    {
//        Skill tSkill = DataCache.getDtoByCls<Skill>(pId);
//        if (null == tSkill)
//        {
//            GameDebuger.LogError(string.Format("DataCache.getDtoByCls<Skill>({0}) failed , simulate this !", pId));
//            tSkill = new Skill();
//            tSkill.id = pId;
//            tSkill.name = "S" + tSkill.id.ToString();
//            tSkill.icon = ((pId < 1000) ? (tSkill.id + 1000) : pId).ToString();
//            tSkill.skillType = (int)Skill.SkillType.Phy;
//        }
//        return tSkill;
//    }

    private static int GetSkillFromSkillIdsStr(string pSkillIdsStr)
    {
        if (string.IsNullOrEmpty(pSkillIdsStr))
            return 0;
        return StringHelper.ToInt(pSkillIdsStr.Split(',')[0]);
    }

//    private static int GetSuitableSkillId()
//    {
//        List<int> tSuitableSkillIdList = GetSuitableSkillIdList();
//        if (null == tSuitableSkillIdList || tSuitableSkillIdList.Count <= 0)
//            return 0;
//        return tSuitableSkillIdList[(tSuitableSkillIdList.Count >> 1) - 1];
//    }

    private static List<int> GetSuitableSkillIdList()
    {
        var tSkillList = DataCache.getArrayByCls<Skill>();   
        if (null == tSkillList || tSkillList.Count <= 0)
            return null;
        var tSkillIdList = new List<int>();
        Skill tSkill;
        for (var tCounter = 0; tCounter < tSkillList.Count; tCounter++)
        {
            tSkill = tSkillList[tCounter];
            //            if(null != tSkill && tSkill.atOnce)
            tSkillIdList.Add(tSkill.id);
        }
        return tSkillIdList;
    }

    public static IEnumerable<int> GetMainCharacterSkillIDList(VideoSoldier pVideoSoldier)
    {
        if (null == pVideoSoldier || null == pVideoSoldier.skillIds || pVideoSoldier.skillIds.Count <= 0)
        {
            GameDebuger.LogError(string.Format("GetMainCharacterSkillIDList failed , null == pVideoSoldier  or pVideoSoldier.skillIds is invalid ,pVideoSoldier:{0} !", pVideoSoldier));
            return SimulateSkillList(SKILL_COUNT_TO_SIMULATE_IF_FAILED);
        }
        return pVideoSoldier.skillIds;
    }

    //    public static List<int> GetMainCharacterSkillIDList(int pCharacterId)
    //    {
    //        GameDebuger.LogError(string.Format("[DEMO/非错误]缺少门派信息，暂时根据角色类型(charactorId:{0})来设置技能", pCharacterId));
    //        //技能获取失败后，模拟的技能数目
    //        const int SKILL_COUNT_TO_SIMULATE_IF_FAILED = 8;
    //        int tIndex = pCharacterId % 5;
    //        if (tIndex < 0 || tIndex >= mMainRolePropertyList.Count)
    //        {
    //            GameDebuger.LogError(string.Format("GetMainCharacterSkillIDList by pCharacterId:{0} failed !", pCharacterId));
    //            return SimulateSkillList(SKILL_COUNT_TO_SIMULATE_IF_FAILED);
    //        }
    //        FighterConfigDto tFighterConfigDto = mMainRolePropertyList[tIndex];
    //        if (null == tFighterConfigDto || string.IsNullOrEmpty(tFighterConfigDto.activeSkillIds))
    //            return SimulateSkillList(SKILL_COUNT_TO_SIMULATE_IF_FAILED);
    //        string[] tActiveSkillIds = tFighterConfigDto.activeSkillIds.Split(',');
    //        List<int> tSkillIDList = new List<int>();
    //        if (null != tActiveSkillIds && tActiveSkillIds.Length > 0)
    //        {
    //            for (int tCounter = 0, tLen = tActiveSkillIds.Length; tCounter < tLen; tCounter++)
    //            {
    //                tSkillIDList.Add(StringHelper.ToInt(tActiveSkillIds[tCounter]));
    //            }
    //            return tSkillIDList;
    //        }
    //        return SimulateSkillList(SKILL_COUNT_TO_SIMULATE_IF_FAILED);
    //    }

    public static List<int> SimulateSkillList(int pSkillListLength = 100)
    {
        GameDebuger.LogError("[TEMP]SimulateSkillList");
        if (pSkillListLength <= 0)
            return null;
        else
        {
            var tSkillList = DataCache.getArrayByCls<Skill>();
            if (null == tSkillList)
            {
                GameDebuger.LogError("SimulateSkillList failed for DataCache.getArrayByCls<Skill>() is null ! ");
                return null;
            }
            var tSkillIdList = new List<int>(pSkillListLength);
            for (int tCounter = 0, tLen = (tSkillList.Count > pSkillListLength ? pSkillListLength : tSkillList.Count); tCounter < tLen; tCounter++)
            {
                tSkillIdList.Add(tSkillList[tCounter].id);
            }
            return tSkillIdList;
        }
    }

    public static void SimulateRoundStart()
    {
        #region simulate for test,delete this in formal
        if (ServiceRequestAction.SimulateNet)
            TestFighterReadyNotifyDto();
        #endregion
    }

    public static FighterReadyNotifyDto SimulateFighterReadyNotifyDto(VideoSoldier pVideoSoldier)
    {
        var tFighterReadyNotifyDto = new FighterReadyNotifyDto();
        tFighterReadyNotifyDto.battleId = VideoRoundSimulater.SIMULATE_BATTLE_ID;
        tFighterReadyNotifyDto.id = pVideoSoldier.id;
        tFighterReadyNotifyDto.releaseTime = SIMULATE_SKILL_CD;
        tFighterReadyNotifyDto.skillId = pVideoSoldier.defaultSkillId;
        return tFighterReadyNotifyDto;
    }


//    public static CharactorDto SimulateCharactorDto(long pId, string pName, int pCharacterId)
//    {
//        CharactorDto tCharactorDto = new CharactorDto();
//        tCharactorDto.id = tCharactorDto.playerId = pId;
//        tCharactorDto.charactorId = pCharacterId;
//        tCharactorDto.name = pName;
//        GameDebuger.TODO(@"tCharactorDto.properties = SimulateShortBattlePropertyDto();");
//        return tCharactorDto;
//    }

    //    public static ShortBattlePropertNpcStateDtoyDto SimulateShortBattlePropertyDto()
    //    {
    //        ShortBattlePropertyDto tShortBattlePropertyDto = new ShortBattlePropertyDto();
    //        tShortBattlePropertyDto.hp = tShortBattlePropertyDto.mp = tShortBattlePropertyDto.attack = tShortBattlePropertyDto.defense = tShortBattlePropertyDto.magic = 100;
    //        return tShortBattlePropertyDto;
    //    }

//    public static CharactorDto SimulateCharactorDto(PlayerDto pPlayerDto)
//    {
//        if (null == pPlayerDto)
//            return SimulateCharactorDto(0, string.Empty, 0);
//        else
//            return SimulateCharactorDto(pPlayerDto.id, pPlayerDto.name, pPlayerDto.charactorId);
//    }

//    public static SkillBuff SimulateBuff(int pBuffId)
//    {
//        GameDebuger.LogError("SimulateBuff pBuffId:" + pBuffId.ToString());
//        SkillBuff tBuff = new SkillBuff();
//        tBuff.id = pBuffId;
//        tBuff.animation = 101;
//        tBuff.icon = "101";
//        tBuff.animationMount = "Mount_Shadow";
//        tBuff.description = "<特殊状态>虚弱：防御和灵力降低，不能进行任何行动";
//        tBuff.name = "虚弱";
//        return tBuff;
//    }

    public static void SimulatePVP(ScenePlayerDto pScenePlayerDto, Action pFinishCallBack = null)
    {
        if (null == pScenePlayerDto || pScenePlayerDto.charactorId < 0)
        {
            GameDebuger.LogError(string.Format("目标错误！null == pScenePlayerDto:{0} || pScenePlayerDto.charactorId < 0", pScenePlayerDto));
            return;
        }
        var tFighterConfigDto = SimulateFighterConfigDtoByScenePlayerDto(pScenePlayerDto);
        var tMyFighterConfigDto = SimulateFighterConfigDtoByPlayerDto(ModelManager.Player.GetPlayer());

//        GameDebuger.TODO(@"ServiceRequestAction.requestServer(DemoService.updateEnemyDummy(enemyDummySetting.GetSettingInfo()));");//legacy 2017-03-09 11:38:06
        var tFightersConfigDto = new FightersConfigDto();
        tFightersConfigDto.ateam = new List<FighterConfigDto>(){ tFighterConfigDto };
        tFightersConfigDto.bteam = new List<FighterConfigDto>(){ tMyFighterConfigDto };
        BattleNetworkManager.EnterBattle(ModelManager.BattleDemoConfig.BattleSceneId, tFightersConfigDto, pFinishCallBack);
    }

//    public static void SimulatePreview(int pSkillId)
//    {
//        SimulatePreview(pSkillId, null);
//    }

    public static void SimulatePreview(int pSkillId, Action pFinishCallBack)
    {
        var tSkill = DataCache.getDtoByCls<Skill>(pSkillId);
        if (null == tSkill)
        {
            GameDebuger.LogError("技能ID错误！");
            return;
        }

        if (null == tSkill.preview)
        {
            var tTip = string.Format("技能预览设置错误，请检查技能表{0}的previewId字段！", pSkillId);
            GameDebuger.LogError(tTip);
            TipManager.AddTip(tTip);
            return;
        }

        var tFightersConfigDto = new FightersConfigDto();
        tFightersConfigDto.ateam = DemoSimulateHelper.SimulateFighterConfigDtoListByIDList(tSkill.preview.teamA, true);
        tFightersConfigDto.bteam = DemoSimulateHelper.SimulateFighterConfigDtoListByIDList(tSkill.preview.teamB, false);

        BattleNetworkManager.HandleDemoVideo(DemoSimulateHelper.SimulatePreviewVideo(tFightersConfigDto, tSkill));
    }

    public static void SimulateDefaultSkill(BattleTargetSelector pBattleTargetSelector)
    {
        if (!ServiceRequestAction.SimulateNet)
            return;
        if (pBattleTargetSelector == null) return;
        //单机模式下播放的是默认技能，强制修改其即可，2017-03-17 19:43:06
        SimulateDefaultSkill(BattleDataManager.DataMgr.BattleDemo.CurActMonsterController, pBattleTargetSelector.GetSelectedSkillId());
    }

    public static void SimulateDefaultSkill(MonsterController pMonsterController, int pSkillId)
    {
        if (!ServiceRequestAction.SimulateNet)
            return;
//        GameDebuger.LogError(string.Format("SimulateDefaultSkill pMonsterController:{0}, pSkillId:{1}",pMonsterController, pSkillId));
        if (null != pMonsterController)
            pMonsterController.videoSoldier.defaultSkillId = pSkillId;
    }

    public static void SimulateDefaultSkill(long pPlayerUID, int pPlayerSkillId = 0, int pPetSkillId = 0)
    {
        if (!ServiceRequestAction.SimulateNet)
            return;
        var tMonsterController = MonsterManager.Instance.GetMonsterFromSoldierID(pPlayerUID);
        SimulateDefaultSkill(tMonsterController, pPlayerSkillId <= 0 ? pPetSkillId : pPlayerSkillId);
    }

//    public static TeamCommand SimulateTeamCommand(int pId)
//    {
//        GameDebuger.LogError(string.Format("[TEMP]缺少数据表TeamCommand，前端暂时模拟之，pId：{0}", pId));
//        string tTempNameFormat = "C_{0}_{1}";
//        TeamCommand tTeamCommand = new TeamCommand();
//        tTeamCommand.id = pId;
//        tTeamCommand.command = new List<string>(){ string.Format(tTempNameFormat, pId, 1), string.Format(tTempNameFormat, pId, 2), string.Format(tTempNameFormat, pId, 3) };
//        return tTeamCommand;
//    }

//    public static Dictionary<int,TeamCommand> SimulateTeamCommandDic()
//    {
//        Dictionary<int,TeamCommand> tTeamCommandDic = new Dictionary<int, TeamCommand>();
//        for (int tCounter = 0; tCounter < 5; tCounter++)
//        {
//            tTeamCommandDic.Add(tCounter, SimulateTeamCommand(tCounter));
//        }
//        return tTeamCommandDic;
//    }

    public static List<BagItemDto> SimulateBagItemDtoList(int pItemLength = 5)
    {
        GameDebuger.LogError("[TEMP]模拟物品列表！");
        var tBagItemDtoList = new List<BagItemDto>();
        for (var tCounter = 0; tCounter < pItemLength; tCounter++)
        {
            tBagItemDtoList.Add(SimulateBagItemDto(tCounter));
        }
        return tBagItemDtoList;
    }

    private static BagItemDto SimulateBagItemDto(int pItemID)
    {
        var tBagItemDto = new BagItemDto();
        tBagItemDto.itemId = pItemID;
        tBagItemDto.item = SimulateAppItem(pItemID);
        return tBagItemDto;
    }

    private static AppItem SimulateAppItem(int pItemID)
    {
        var tAppItem = new AppItem();
        tAppItem.id = pItemID;
        tAppItem.name = "物品" + pItemID.ToString();
        return tAppItem;
    }
}