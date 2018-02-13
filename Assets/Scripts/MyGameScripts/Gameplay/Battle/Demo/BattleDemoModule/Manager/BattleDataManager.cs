using System;
using UnityEngine;
using AppDto;
using System.Collections.Generic;

public sealed partial class BattleDataManager
{
    #if UNITY_EDITOR
    public static bool DEBUG = true;
    #else
	public static bool DEBUG = false;
	#endif

    private const string BARRAGE_VIDEO_TIMER = "BARRAGE_VIDEO_TIMER";

    public static bool NeedBattleMap = true;

    // 初始化
    private void LateInit()
    {
    }
    
    public void OnDispose()
    {
        BattleInstController.DisposeOnExit();
        MonsterManager.Dispose();
        JSTimer.Instance.CancelTimer("AfterRetreat");
    }
    
    //    private BarrageVideo _barrageVideo;
    private long _barrageVideoBeginStamp;

    private Video _nextGameVideo
    {
        get { return nextGameVideo; }
        set
        {
            nextGameVideo = value;
        }
    }
    private Video nextGameVideo;
    
    private bool _needRefreshBattle;

    #region Battle CallBack

    public event Action<BattleResult,bool> OnBattleFinishCallback;

    #endregion


    public bool IsInBattle
    {
        get{
            return _data != null && _data._gameVideo != null;
        }
    }

    public IBattleDemoModel BattleDemo
    {
        get { return _data;}
    }

    public bool NeedPlayPlot;

    public void PlayBattle(Video gv, int watchTeamId = 0, bool needRefreshBattle = false/**, BarrageVideo barrageVideo= null*/)
    {   
        if (IsInBattle && !_data.IsSameBattle(gv.id))
        {
            GameDebuger.Log(string.Format("当前有其它战斗{0}在进行，先暂存战斗{1} waitingExitBattle={2}", _data._gameVideo.id, gv.id, _nextGameVideo == null));
            _nextGameVideo = gv;
//            _data._watchTeamId = watchTeamId;  // todo fish :只用一个字段处理观战ID 没有问题吗？有什么机制保障两场战斗同一个观战ID
            return;
        }

        if (_data._gameVideo == null)
            _data.ResetData();

        _data.GameVideo = gv;

        NeedPlayPlot = false;
        _data._watchTeamId = watchTeamId;
        
        if (_data._gameVideo == null)
            _data.ResetData();
        
        var isSameBattle = _data.IsSameBattle(gv.id);
        _data.GameVideo = gv; 
        
        _data.isAIManagement = _data._gameVideo.needPlayerAutoBattle;
        _needRefreshBattle = needRefreshBattle;
        //打印战斗具体的信息
        GameDebuger.Log(GetBattleInfo());

        AdjustGameContingent();
        
        if (!isSameBattle)
        {
            if (_data._gameVideo.mapId != 0)
            {
                NeedBattleMap = true;
                SceneFadeEffectController.Show(_data._gameVideo, OnBattleMapLoaded, OnFadeOutFinish);
            }
            else
            {
                NeedBattleMap = false;
                OnBattleMapLoaded(0);
                BattleDemoViewLogic.Open();
                CheckGameState();
            }
        }
        GameDebuger.TODO(@"if (ModelManager.SystemData.battleMapToggle && _currentGameVideo.mapId != 0)");
        if (_data._gameVideo.mapId <= 0 && !(_data._gameVideo is PreviewVideo))
        {
            TipManager.AddTip(string.Format("战斗地图没有设置，强制设置为{0}", BattleDemoConfigModel.DEFAULT_SCENE_ID));
            _data._gameVideo.mapId = BattleDemoConfigModel.DEFAULT_SCENE_ID;
        }
        if (_data._gameVideo.mapId != 0)
        {
            NeedBattleMap = true;
            SceneFadeEffectController.Show(_data._gameVideo, OnBattleMapLoaded, OnFadeOutFinish);
        }
        else
        {
            NeedBattleMap = false;
            OnBattleMapLoaded(0);
            BattleDemoViewLogic.Open();
            CheckGameState();
        }

        GameDebuger.TODO(@"if (_barrageVideo != null && _isVideotrap == true)
        {
            JSTimer.DataMgr.SetupTimer(BARRAGE_VIDEO_TIMER, onBarrageVideoTimerUpdate);
            _barrageVideoBeginStamp = SystemTimeManager.DataMgr.GetUTCTimeStamp();
        }");
        
        FireData();
    }

    private void UpdateCurVideo(Video gv)
    {
        if (gv != null && _data != null && _data.IsSameBattle(gv.id))
        {
            _data.GameVideo = gv;
            FireData();
        }
    }
    //private bool CheckVideoIsThisPlayer(PlayerDto playerDto, Video gv)
    //{
    //    Debug.Log("playerDto.id" + playerDto.id);
    //    for (int i = 0; i < gv.playerInfos.Count; i++)
    //    {
    //        Debug.Log(gv.playerInfos[i].playerId);
    //        if (playerDto.id == gv.playerInfos[i].playerId)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //根据时间偏移值，播放弹幕
    //    private void onBarrageVideoTimerUpdate()
    //    {
    //        if (_barrageVideo == null)
    //        {
    //            BarrageVideoDestory();
    //            return;
    //        }
    //
    //        int barrageLen = _barrageVideo.offsetList.Count;
    //        if (barrageLen <= 0)
    //        {
    //            BarrageVideoDestory();
    //            return;
    //        }
    //
    //        CheckAndAddBarrageMsg(0);
    //}

    //private bool CheckAndAddBarrageMsg(int pIndex)
    //{
    //    if (SystemTimeManager.DataMgr.GetUTCTimeStamp() - _barrageVideoBeginStamp >= _barrageVideo.offsetList[pIndex])
    //    {
    //        BarrageDto barrageDto = _barrageVideo.barrageList[pIndex];
    //        bool isSelf = barrageDto.playerId == ModelManager.Player.GetPlayerId();
    //        BarrageItemData barrageData = new BarrageItemData(barrageDto.content, barrageDto.type, isSelf);
    //        ProxyBarrageModule.addBarrageMsg(barrageData);
    //        _barrageVideo.offsetList.RemoveAt(pIndex);
    //        _barrageVideo.barrageList.RemoveAt(pIndex);
    //        return true;
    //    }
    //    return false;
    //}
    #pragma warning disable
    private bool _waitingRequestVideo = false;
    #pragma warning restore
    private void OnBattleMapLoaded(int sceneResId)
    {
        WorldManager.Instance.HideScene();
        if (_needRefreshBattle)
        {
            _waitingRequestVideo = true;
            GameDebuger.TODO(@"ServiceRequestAction.requestServer(CommandService.getVideo(_currentGameVideo.id), "", delegate(GeneralResponse e)
                {
                    Video video = e as Video;

                    _waitingRequestVideo = false;
                    GameDebuger.Log('CommandService.getVideo response ' + video);

                    if (video == null || _isInBattle == false)  //防止在服务器返回过程中，玩家进行切换人物等操作 强制关闭关卡
                    {
                        _isInBattle = false;
                        _currentGameVideo = null;
                        WorldManager.DataMgr.ResumeScene();
            //                  NGUIFadeInOut.FadeIn ();
                    }
                    else
                    {
                        _currentGameVideo = video;
                        StartBattleVideo();
                    }
                }, errorResponse =>
                {
                    _waitingRequestVideo = false;
                    _isInBattle = false;
                    _currentGameVideo = null;
                    WorldManager.DataMgr.ResumeScene();
            //              NGUIFadeInOut.FadeIn ();
                });");
        }
        else
        {
            StartBattleVideo();
        }
    }

    private void StartBattleVideo()
    {
        GameDebuger.TODO(@"ModelManager.Arena.CheckAndTipSupportCrew ();");

        SceneHelper.ToggleSceneEffect(false);

        GameDebuger.TODO(@"if (_currentGameVideo is GuideVideo)
        {
            UIModuleManager.Instance.HideModule(MainUIView.NAME);
        }");

        GameDebuger.TODO(@"if (ModelManager.Player.GetPlayer() != null)
        {
            ModelManager.Player.GetPlayer().curGameVideo = null;
        }");
        
        InitBattleMgrs(_data._gameVideo);
        
        GameDebuger.TODO(@"if (ModelManager.SystemData.battleMapToggle && _currentGameVideo.mapId != 0)");
        if (/**ModelManager.SystemData.battleMapToggle &&*/ _data._gameVideo.mapId != 0)
        {
            //场景编号:1\2\3  3D无镜头场景、3D普通镜头场景、3DBOSS镜头场景
            WorldManager.Instance.PlayCameraAnimator(_data._gameVideo.mapId, _data._gameVideo.cameraId - 1);
        } 
//		else
//		{
//			if (WorldManager.FirstEnter && ModelManager.Player.GetPlayer().curGameVideo != null)
//			{
//				GameObject locationGO = new GameObject("TempLocationGO");
//
//				Vector3 position = Vector3.zero;
//				locationGO.transform.position = position;
//
//				GamePlayer.CameraManager.DataMgr.SetupCamera (locationGO.transform, LayerManager.DataMgr.GameCameraGo);
//
//				GameObject.Destroy(locationGO);
//			}
//		}

        PlayBattleMusic();
    }

    // todo fish :流程不对 打开界面这些不应该放在battledatamanager
    private static void InitBattleMgrs(Video pVideo)
    {
        if (pVideo is PreviewVideo)
            ProxyBattleFightPreviewModule.Open(pVideo as PreviewVideo);
    }

    private void PlayBattleMusic()
    {
        GameDebuger.TODO(@"if (_currentGameVideo is PvpVideo)
        {
            AudioManager.DataMgr.PlayMusic('music_battle_pvp');
        }
        else if (_currentGameVideo is GuideVideo)
        {
            AudioManager.DataMgr.PlayMusic('music_battle_boss');
        }
        else
        {
            if (_currentGameVideo is TollgateVideo)
            {
                TollgateVideo tollgateVideo = _currentGameVideo as TollgateVideo;
                if (tollgateVideo.tollgate != null && !string.IsNullOrEmpty(tollgateVideo.tollgate.music))
                {
                    AudioManager.DataMgr.PlayMusic(tollgateVideo.tollgate.music);
                }
                else
                {
                    AudioManager.DataMgr.PlayMusic('music_battle_pve');
                }
            }
            else");
        {
            AudioManager.Instance.PlayMusic("music_battle_pve");
        }
        GameDebuger.TODO(@"}");
    }
    
    private void OnFadeOutFinish()
    {
        // 播放开场动作，或者 直接开始战斗
        if (!_data.AnimShown && 
            _data.CurRoundCnt <= 0)
        {
            MonsterManager.Instance.ShowMonsters(
                DataMgr._data._gameVideo
                , delegate
                {
                    //            跑动动作
                    var time = MonsterManager.Instance.MonsterEnterScene() ;
                    //                灯光特效 
                    //                        界面展示动作
                    //                            回调
                    JSTimer.Instance.SetupCoolDown(
                        "OnFadeOutFinish" + GetHashCode()
                        , time
                        , null
                        , delegate {
                        //                                灯光特效 
                        //                        界面展示动作
                        //                            回调
                        _data._isTimeCountFinish = true;
                        BattleDemoViewLogic.Open();
                        CheckGameState();
                    });

                }
                , new Vector3(0f, 20f, 0f));
        }
        else
        {
            MonsterManager.Instance.ShowMonsters(
                DataMgr._data._gameVideo
            , delegate {
                MonsterManager.Instance.ForEachMonster(mc=>mc.PlayIdleAnimation());
            });
            BattleDemoViewLogic.Open();
            CheckGameState();
        }
    }

    public bool CheckNextBattle()
    {
        if (_nextGameVideo != null)
        {
            var tempWatchId = _data._watchTeamId;
            var tempGameVideo = _nextGameVideo;

            _data._watchTeamId = 0;
            _nextGameVideo = null;
            PlayBattle(tempGameVideo, tempWatchId, true);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckResumeBattle()
    {
        //DOTO 这里还需要处理同一场战斗回合数已经过了很久的问题

//		if (_nextGameVideo != null)
//		{
//
//		}
    }
	
    //    private void BarrageVideoDestory()
    //    {
    //        JSTimer.DataMgr.CancelTimer(BARRAGE_VIDEO_TIMER);
    //        _barrageVideo = null;
    //    }

    public void BattleDestroy(BattleResult battleResult, bool youDead, Video gameVideo)
    {
        if (gameVideo == null)
            return;
        GameLog.Log_Battle("-------销毁战斗场景");
        GameDebuger.TODO(@"BarrageVideoDestory();");

        //退出战斗时把战斗场景销毁
        var go = GameObject.Find("BattleStage");
        GameObject.Destroy(go);
        
        WorldMapLoader.Instance.UnLoadMapResource(_data._gameVideo.mapId);
        //force  save
        PlayerPrefs.Save();

        if (_nextGameVideo == null)
        {
            GameDebuger.TODO(@"
            if (MainUIViewController.DataMgr != null)
            {
                MainUIViewController.DataMgr.HideBtnInBattle(true);
            }
            ProxyManager.Tournament.Show();
            ProxyManager.Tournament.ShowV2();
            ProxyManager.GuildCompetition.OnClickExplandHorBtn(true);
            ProxyManager.CampWar.OnClickExplandHorBtn(true);
            ProxyManager.Escort.OnClickExplandHorBtn(true);
            ProxyManager.SnowWorldBossExpand.Show();
            ProxyManager.DaTang.IsOpenTips(false);            
");

            if (_data._watchTeamId == 0)
            {
                GameDebuger.TODO("ModelManager.SiegeBattle.Resume();");
                ModelManager.Player.CleanPlayerSp();

                GameDebuger.TODO(@"if (_data._gameVideo is TollgateVideo && !IsTeamMemberBattle(_data._gameVideo) && battleResult == BattleDemoViewController.BattleResult.WIN)
                {
                    if (GamePlotManager.DataMgr.TriggerPlot(Plot.PlotTriggerType_TollgateWin, (_data._gameVideo as TollgateVideo).tollgateId))
                    {
                        NeedPlayPlot = true;
                    }
                }");

                GameDebuger.TODO(@"if (_data._gameVideo is GuideVideo)
                {
                    if (GamePlotManager.DataMgr.TriggerPlot(Plot.PlotTriggerType_PlotPlayOver, 1))
                    {
                        NeedPlayPlot = true;
                    }
                }");

                //PVP战斗结束处理
                GameDebuger.TODO(@"if (_data._gameVideo is PvpVideo)
                {
                    PvpVideo pvpVideo = _data._gameVideo as PvpVideo;
                    if (pvpVideo.type == PvpVideo.PvpTypeEnum_Challenge)
                    {
                        ProxyManager.Arena.Open();
                    }
                }
                else");
                {
                    if (battleResult == BattleResult.LOSE)
                    {
                        //TipManager.AddTip("胜败乃兵家常事，提升能力继续挑战！");
                        // TODO 触发失败提升指引
                        GameDebuger.TODO(@"if (_data._gameVideo is TollgateVideo)
                            ProxyManager.LoseGuide.Open(ModelManager.MissionData.IsCurrentMainMissionTollgate(_data._gameVideo as TollgateVideo));
                        else
                            ProxyManager.LoseGuide.Open();                        


					    if (_data._gameVideo is HundredGrassVideo)
					    {
					        string tips = '{0}失败，{1}进度不变';
                            HundredGrassVideo v = _data._gameVideo as HundredGrassVideo;
                            switch (v.type)
                            {
                                case HundredGrassInteractResultNotify.InteractTypeEnum_Water:
                                    tips = string.Format(tips, '浇水', '浇水');
                                    break;
                                case HundredGrassInteractResultNotify.InteractTypeEnum_Fertilize:
                                    tips = string.Format(tips, '施肥', '施肥');
                                    break;
                                case HundredGrassInteractResultNotify.InteractTypeEnum_Disinsect:
                                    tips = string.Format(tips, '除虫', '除虫');
                                    break;
                                default:;
                                    tips = string.Format(tips, '未知', '未知');
                                    break;
                            }
                            TipManager.AddTip(tips, false, true);
                        }");
                    }
                    else
                    {

                        //  win
                    }
                }
            }

            if (NeedPlayPlot == false && _nextGameVideo == null)
            {
                WorldManager.Instance.ResumeScene();
                //			NGUIFadeInOut.FadeIn ();
            }
            GameDebuger.TODO("SceneHelper.ToggleSceneEffect(ModelManager.SystemData.sceneEffectToggle);");

            GameDebuger.TODO("ModelManager.Team.Resume();");

            if (OnBattleFinishCallback != null)
            {
                GameDebuger.TODO(@"ModelManager.Team.IsLeaderAutoInBattle = false;");
                OnBattleFinishCallback(battleResult, _data._watchTeamId == 0);
            }
            //战斗后查看我是否有在战斗中被离婚

            //以下的次数限制必须是自身战斗退出的情况下  for：修复观战同样 5 或10场的情况下也会出现限制提示
            GameDebuger.TODO(@"if (_watchTeamId == 0)
            {
                if (_currentGameVideo is NpcMonsterVideo)
                {
                    NpcMonsterVideo tVideo = _currentGameVideo as NpcMonsterVideo;
                    switch (tVideo.npcSceneMonsterBronType)
                    {
                        case NpcMonsterVideo.NpcSceneMonsterBronType_WorldGhost:                    // 世界Boss鬼姬 10个限制 (不论输赢，计算一次)
                            ModelManager.SceneMonster.HandleOnWorldGhostBattleFinshCallback();
                            break;
                    }

                    if (battleResult == BattleDemoViewController.BattleResult.WIN)
                    {
                        switch (tVideo.npcSceneMonsterBronType)
                        {
                            case NpcMonsterVideo.NpcSceneMonsterBronType_Star:                      //  星宿10个限制
                                ModelManager.SceneMonster.HandleOnBattleFinishCallback();
                                break;
                            case NpcMonsterVideo.NpcSceneMonsterBronType_WorldBoss:                 // 上古神兽 5个限制
                                ModelManager.SceneMonster.HandleOnBossBattleFinishCallback();
                                break;
                            case NpcMonsterVideo.NpcSceneMonsterBronType_DatangMatchless:           //大唐无双
                                ModelManager.Tang.AfterBattle(tVideo);
                                break;
                        }
                    }
                }
            }");

            _data.GameVideo = null;

        }
        else
        {
            _data.GameVideo = null;

            if (!CheckNextBattle())
            {
                if (OnBattleFinishCallback != null)
                {
                    GameDebuger.TODO(@"ModelManager.Team.IsLeaderAutoInBattle = false;");
                    OnBattleFinishCallback(battleResult, _data._watchTeamId == 0);
                }
                //战斗后查看我是否有在战斗中被离婚
                GameDebuger.TODO(@"ModelManager.Marry.IsEmotionalBreakdown();");
            }
        }
        //进出战斗需要把场景里的头顶气泡都关掉
        GameDebuger.TODO("@ProxyManager.ActorPopo.CloseAll();");
        BattleInstController.DisposeOnExit();
    }

    //是否是队员战斗
    private bool IsTeamMemberBattle(Video video)
    {
        var soldier = GetTeamLeader(video.ateam);
        return soldier != null && soldier.playerId != ModelManager.Player.GetPlayerId();
    }

    private VideoSoldier GetTeamLeader(VideoTeam team)
    {
        GameDebuger.TODO(@"for (int i = 0; i < team.fighters.Count; i++)
        {
            VideoSoldier videoSoldier = team.fighters[i];
            if (videoSoldier.characterType == (int)GeneralCharactor.CharactorType.MainCharactor &&
            videoSoldier.playerId == videoSoldier.leaderPlayerId)
            {
                return videoSoldier;
            }
        }");
        return null;
    }

    public long GetCurrentGameVideoId()
    {
        return _data._gameVideo == null ? 0L : _data._gameVideo.id;
    }

    public string GetBattleInfo()
    {
        var info = "";

        if (_data._gameVideo != null)
        {
            info += " 当前战斗:" + _data._gameVideo.id;
        }

        return info;
    }

    public void Destroy()
    {
        GameDebuger.TODO(@"if (BattleController.DataMgr != null)
        {
            BattleController.DataMgr.Destory();
        }");
        GameEventCenter.SendEvent(GameEvent.BATTLE_FIGHT_DESTROY);

        GameDebuger.TODO(@"SceneFadeEffectController.Close();");

        //退出战斗时把战斗场景销毁
        var go = GameObject.Find("BattleStage");
        if (go != null)
        {
            GameObject.Destroy(go);
        }

        //force  save
        PlayerPrefs.Save();

        if (_data._gameVideo != null)
        {
            LayerManager.Instance.SwitchLayerMode(UIMode.GAME);
//			GamePlayer.CameraManager.DataMgr.cameraMove.enabled = true;
        }

        _data.GameVideo = null;
        _nextGameVideo = null;

        _waitingRequestVideo = false;
    }

    //	//是否在战斗模式
    //	public static bool IsBattlePlaying(){
    //		return true;
    //		//return UIManager.DataMgr.UIMode == UIManager.UI_MODES.BATTLE;
    //	}

    public static int GetUseItemSkillId()
    {
        return DataCache.GetStaticConfigValue(AppStaticConfigs.USE_ITEM_SKILL_ID, 2);
    }

    public static int GetRetreatSkillId()
    {
        return DataCache.GetStaticConfigValue(AppStaticConfigs.ESCAPE_SKILL_ID, 3);
    }

    public static int GetDefenseSkillId()
    {
        GameDebuger.TODO(@"return DataCache.GetStaticConfigValue (AppStaticConfigs.DEFAULT_DEFENSE_SKILL_ID, 99);");
        return 99;
    }

    public static int GetProtectSkillId()
    {
        GameDebuger.TODO(@"return DataCache.GetStaticConfigValue (AppStaticConfigs.DEFAULT_PROTECT_SKILL_ID, 98);");
        return 98;
    }

    public static int GetCaptureSkillId()
    {
        GameDebuger.TODO(@"return DataCache.GetStaticConfigValue (AppStaticConfigs.DEFAULT_CAPTURE_SKILL_ID, 97);");
        return 97;
    }

    public static int GetSummonSkillId()
    {
        GameDebuger.TODO(@"return DataCache.GetStaticConfigValue (AppStaticConfigs.DEFAULT_SUMMON_SKILL_ID, 96);");
        return 5;
    }

    public static int GetCommandSkillId()
    {
        GameDebuger.LogError("[TEMP]指令技能ID配表");
        return 88;
    }

    public static bool IsStuntSkill(Skill pSkill)
    {
        GameDebuger.LogError("[TEMP]检测是否装备特技");
        return null != pSkill && pSkill.id % 2 == 0;
    }

    //检查战斗中是否可以使用
    public bool CheckUseInBattle()
    {
        if (IsInBattle)
        {
            TipManager.AddTip("战斗中无法使用此功能");
            return false;
        }
        else
        {
            return true;
        }
    }

    #region GuideBattle

    public void PlayGuideBattle()
    {
        var video = new GuideVideo();
        video.id = 1;
        video.ateam = CreateVideoTeam(1, 2);
        GameDebuger.TODO(@"video.myTeam.playerIds.Add(ModelManager.Player.GetPlayerId());");
        video.needPlayerAutoBattle = false;

        AddSoilder(video.ateam, 1001, 1, "孙悟空", 1, 7766, 9999, 0, 0, 0, 0, 20019, GeneralCharactor.CharactorType.MainCharactor, true);
        AddSoilder(video.ateam, 1002, 7, "落日剑客", 1, 10468, 9999, 0, 0, 0, 0, 20001, GeneralCharactor.CharactorType.Crew);
        AddSoilder(video.ateam, 1003, 2, "明月", 1, 5000, 9999, 0, 0, 0, 0, 4, GeneralCharactor.CharactorType.MainCharactor);
        AddSoilder(video.ateam, 1004, 3, "太子", 1, 4000, 9999, 0, 0, 0, 0, 3, GeneralCharactor.CharactorType.MainCharactor);
        AddSoilder(video.ateam, 1005, 4, "剑侠", 1, 4000, 9999, 0, 0, 0, 0, 1, GeneralCharactor.CharactorType.MainCharactor);
        AddSoilder(video.ateam, 1006, 8, "灵狐", 1, 4000, 9999, 0, 0, 0, 0, 6, GeneralCharactor.CharactorType.MainCharactor);
        AddSoilder(video.ateam, 1007, 5, "熊猫", 1, 4000, 9999, 0, 0, 0, 0, 5, GeneralCharactor.CharactorType.MainCharactor);
        AddSoilder(video.ateam, 1008, 6, "飞燕", 1, 4000, 9999, 0, 0, 0, 0, 2, GeneralCharactor.CharactorType.MainCharactor);

        video.bteam = CreateVideoTeam(2, 7);
        AddSoilder(video.bteam, 2001, 7, "魔族侍从", 1, 968, 9999, 0, 0, 12130, 0, 0, GeneralCharactor.CharactorType.Monster);
        AddSoilder(video.bteam, 2002, 8, "魔族侍从", 1, 952, 9999, 0, 0, 12370, 0, 0, GeneralCharactor.CharactorType.Monster);
        AddSoilder(video.bteam, 2003, 4, "吸血鬼", 1, 12153, 9999, 0, 0, 12380, 0, 0, GeneralCharactor.CharactorType.Monster);
        AddSoilder(video.bteam, 2004, 5, "鬼姬", 1, 6749, 9999, 0, 0, 12240, 0, 0, GeneralCharactor.CharactorType.Monster);
        AddSoilder(video.bteam, 2005, 6, "地狱火魂", 1, 6219, 9999, 0, 0, 12310, 0, 0, GeneralCharactor.CharactorType.Monster);
        AddSoilder(video.bteam, 2006, 1, "魔族将军", 1, 22222, 9999, 0, 0, 10001, 0, 0, GeneralCharactor.CharactorType.Monster);

        PlayBattle(video);
    }

    private VideoTeam CreateVideoTeam(int teamId, int formationId)
    {
        var team = new VideoTeam();
        team.formationId = formationId;
        team.teamSoldiers = new List<VideoSoldier>();
        GameDebuger.TODO(@"team.playerIds = new List<long>();");
        GameDebuger.TODO(@"team.id = teamId;");
        return team;
    }

    private void AddSoilder(VideoTeam team, long id, int position, string name, int grade, int hp, int mp, int sp, int magicMana,
                            int monsterId, int monsterType, int charactorId, GeneralCharactor.CharactorType charactorType, 
                            bool isHero = false)
    {
        var soldier = new VideoSoldier();
        soldier.id = id;
        soldier.name = name;
        soldier.hp = soldier.maxHp = hp;
        soldier.ep = soldier.maxEp = mp;
        soldier.cp = soldier.maxCp = sp;
        soldier.monsterId = monsterId;
        soldier.monsterType = monsterType;
        soldier.grade = grade;
        soldier.charactorId = charactorId;
        soldier.charactorType = (int)charactorType;
        soldier.defaultSkillId = 1;

        var formation = team.formation;
        soldier.position = position;

        if (isHero)
        {
            soldier.id = ModelManager.Player.GetPlayerId();
            soldier.factionId = ModelManager.Player.GetPlayer().factionId;

            soldier.playerDressInfo = new PlayerDressInfo();
            soldier.playerDressInfo.charactorId = charactorId;
            //soldier.playerDressInfo.wpmodel = NewBieGuideManager.GetNewBieWeapon(charactorId);
        }
        else
        {
            if (charactorType == GeneralCharactor.CharactorType.MainCharactor)
            {
                soldier.playerDressInfo = new PlayerDressInfo();
                soldier.playerDressInfo.charactorId = charactorId;
                GameDebuger.TODO("soldier.playerDressInfo.wpmodel = NewBieGuideManager.GetNewBieWeapon(charactorId);");
            }
        }

        team.teamSoldiers.Add(soldier);
    }

    public void PlayGuideVideoRound(int index)
    {
        VideoRound vr = null;
        if (index == 1)
            vr = GetGuideRound1();
        else if (index == 2)
        {
            vr = GetGuideRound2();
        }
        else if (index == 3)
        {
            vr = GetGuideRound3();
        }

        if (vr == null) return;
        MonsterManager.Instance.UpdateOptionState(vr.id, MonsterOptionStateManager.MonsterOptionState.Disable);
        BattleInstController.Instance.AddVideoRound(vr);
        BattleInstController.Instance.CheckNextRound();
    }

    private int GetPlayerDefaultSkillId()
    {
        GameDebuger.TODO(@"
Faction faction = ModelManager.Player.GetPlayer().faction;
        return faction.defaultSkillId;        
");
        return 1;
    }

    public VideoRound GetGuideRound1()
    {
        VideoRound videoRound = new VideoRound();
        videoRound.battleId = 1;
        GameDebuger.TODO(@"videoRound.count = 1;");
        GameDebuger.TODO(@"videoRound.over = false;");
        videoRound.skillActions = new List<VideoSkillAction>();

        //2001	1	小鬼a	1003	明月	普通攻击	1	514	4486
        VideoSkillAction skillAction_2001 = NewVideoSkillAction(2001, 1, 10);
        AddVideoTargetState(skillAction_2001, 1, 1003, -514, 4486, false, false, false);

        //2002	2	小鬼b	1002	落日剑客	普通攻击	1	547	9921
        VideoSkillAction skillAction_2002 = NewVideoSkillAction(2002, 1, 10);
        AddVideoTargetState(skillAction_2002, 1, 1002, -547, 9921, false, false, false);

        //2003	3	小鬼c	1005	剑侠	普通攻击	1	539	3461
        VideoSkillAction skillAction_2003 = NewVideoSkillAction(2003, 1, 10);
        AddVideoTargetState(skillAction_2003, 1, 1005, -539, 3461, false, false, false);

        //2004	4	小鬼d	1006	灵狐	普通攻击	1	528	3472
        VideoSkillAction skillAction_2004 = NewVideoSkillAction(2004, 1, 10);
        AddVideoTargetState(skillAction_2004, 1, 1006, -528, 3472, false, false, false);

        //2005	5	小鬼e	1008	飞燕	深渊烈焰	5355	541	3459
        //2005	5	小鬼e	1004	太子	深渊烈焰	5355	541	3459
        //2005	5	小鬼e	1001	孙悟空	深渊烈焰	5355	541	7225
        VideoSkillAction skillAction_2005 = NewVideoSkillAction(2005, 5355, 10);
        AddVideoTargetState(skillAction_2005, 1, 1008, -541, 3459, false, false, false);
        AddVideoTargetState(skillAction_2005, 2, 1004, -541, 3459, false, false, false);
        AddVideoTargetState(skillAction_2005, 3, 1001, -541, 7225, false, false, false);

        //2006	6	BOSS	1003	明月	血雨腥风	5505	1035	3451
        //2006	6	BOSS	1002	落日剑客	血雨腥风	5505	1048	8873
        //2006	6	BOSS	1001	孙悟空	血雨腥风	5505	1067	6158
        VideoSkillAction skillAction_2006 = NewVideoSkillAction(2006, 5505, 10);
        AddVideoTargetState(skillAction_2006, 1, 1003, -1035, 3451, false, false, false);
        AddVideoTargetState(skillAction_2006, 2, 1002, -1048, 8873, false, false, false);
        AddVideoTargetState(skillAction_2006, 3, 1001, -1067, 6158, false, false, false);

        //1001	7	孙悟空	2006	BOSS	血战八方	1111	2166	20056
        //1001	7	孙悟空	2005	地狱火魂	血战八方	1111	2411	3808
        //1001	7	孙悟空	2003	吸血鬼	血战八方	1111	2344	9809
        VideoSkillAction skillAction_1001 = NewVideoSkillAction(1001, 1111, 10);
        AddVideoTargetState(skillAction_1001, 1, 2006, -2166, 20056, false, false, false);
        AddVideoTargetState(skillAction_1001, 2, 2005, -2411, 3808, false, false, false);
        AddVideoTargetState(skillAction_1001, 3, 2003, -2344, 9809, false, false, false);

        //1002	8	落日剑客	2006	BOSS	势如破竹	1112	801	19165
        //1002	8	落日剑客	2006	BOSS	势如破竹	1112	891	18274
        //1002	8	落日剑客	2006	BOSS	势如破竹	1112	981	17383
        VideoSkillAction skillAction_1002 = NewVideoSkillAction(1002, 1112, 10);
        AddVideoTargetState(skillAction_1002, 1, 2006, -801, 19255, false, false, false);
        AddVideoTargetState(skillAction_1002, 2, 2006, -891, 18364, false, false, false);
        AddVideoTargetState(skillAction_1002, 3, 2006, -981, 17383, false, false, false);

        //1003	9	明月	2001	小鬼a	翻云覆雨	1511	968	0
        //1003	9	明月	2002	小鬼b	翻云覆雨	1511	952	0
        //1003	9	明月	2006	BOSS	翻云覆雨	1511	616	16767
        VideoSkillAction skillAction_1003 = NewVideoSkillAction(1003, 1511, 10);
        AddVideoTargetState(skillAction_1003, 1, 2001, -968, 0, true, true, false);
        AddVideoTargetState(skillAction_1003, 2, 2002, -952, 0, true, true, false);
        AddVideoTargetState(skillAction_1003, 3, 2006, -616, 16767, false, false, false);

        //1004	10	太子	2003	吸血鬼	雷霆万钧	1411	954	8855
        //1004	10	太子	2004	鬼姬	雷霆万钧	1411	926	5823
        //1004	10	太子	2006	BOSS	雷霆万钧	1411	629	16138
        VideoSkillAction skillAction_1004 = NewVideoSkillAction(1004, 1411, 10);
        AddVideoTargetState(skillAction_1004, 1, 2003, -954, 8855, false, false, false);
        AddVideoTargetState(skillAction_1004, 2, 2004, -926, 5823, false, false, false);
        AddVideoTargetState(skillAction_1004, 3, 2006, -629, 16138, false, false, false);

        //1008	11	飞燕	2003	吸血鬼	五雷咒	1311	645	8210
        //1008	11	飞燕	2004	鬼姬	五雷咒	1311	621	5202
        //1008	11	飞燕	2006	BOSS	五雷咒	1311	443	15695
        VideoSkillAction skillAction_1008 = NewVideoSkillAction(1008, 1311, 10);
        AddVideoTargetState(skillAction_1008, 1, 2003, -645, 8210, false, false, false);
        AddVideoTargetState(skillAction_1008, 2, 2004, -621, 5202, false, false, false);
        AddVideoTargetState(skillAction_1008, 3, 2006, -443, 15695, false, false, false);

        //1005	12	剑侠	2003	吸血鬼	金刚咒	1211	675	7535
        //1005	12	剑侠	2004	鬼姬	金刚咒	1211	628	4574
        //1005	12	剑侠	2006	BOSS	金刚咒	1211	483	15212
        VideoSkillAction skillAction_1005 = NewVideoSkillAction(1005, 1211, 10);
        AddVideoTargetState(skillAction_1005, 1, 2003, -675, 7535, false, false, false);
        AddVideoTargetState(skillAction_1005, 2, 2004, -628, 4574, false, false, false);
        AddVideoTargetState(skillAction_1005, 3, 2006, -483, 15212, false, false, false);

        //1006	13	灵狐	2003	吸血鬼	毒蛛手	1911	657	6878
        //1006	13	灵狐	2004	鬼姬	毒蛛手	1911	635	3939
        //1006	13	灵狐	2006	BOSS	毒蛛手	1911	454	14758
        VideoSkillAction skillAction_1006 = NewVideoSkillAction(1006, 1911, 10);
        AddVideoTargetState(skillAction_1006, 1, 2003, -657, 6878, false, false, false);
        AddVideoTargetState(skillAction_1006, 2, 2004, -635, 3939, false, false, false);
        AddVideoTargetState(skillAction_1006, 3, 2006, -454, 14758, false, false, false);

        //1007	14	熊猫	2003	吸血鬼	飞沙走石	1711	978	5900
        //1007	14	熊猫	2004	鬼姬	飞沙走石	1711	997	2942
        //1007	14	熊猫	2006	BOSS	飞沙走石	1711	676	14082
        VideoSkillAction skillAction_1007 = NewVideoSkillAction(1007, 1711, 10);
        AddVideoTargetState(skillAction_1007, 1, 2003, -978, 5900, false, false, false);
        AddVideoTargetState(skillAction_1007, 2, 2004, -997, 2942, false, false, false);
        AddVideoTargetState(skillAction_1007, 3, 2006, -676, 14082, false, false, false);

        videoRound.skillActions.Add(skillAction_2001);
        videoRound.skillActions.Add(skillAction_2002);
        videoRound.skillActions.Add(skillAction_2003);
        videoRound.skillActions.Add(skillAction_2004);
        videoRound.skillActions.Add(skillAction_2005);
        videoRound.skillActions.Add(NewVideoShoutAction(2006, "真是不自量力，竟然妄想与战神大人作对！"));
        videoRound.skillActions.Add(skillAction_2006);

        videoRound.skillActions.Add(NewVideoShoutAction(1001, "我无法发挥全部力量，这场战斗就靠你们了！"));
        videoRound.skillActions.Add(skillAction_1001);
        videoRound.skillActions.Add(NewVideoShoutAction(1002, "我体内的武士之魂不允许我不战而败！"));
        videoRound.skillActions.Add(skillAction_1002);
        videoRound.skillActions.Add(skillAction_1003);
        videoRound.skillActions.Add(skillAction_1004);
        videoRound.skillActions.Add(skillAction_1008);
        videoRound.skillActions.Add(skillAction_1005);
        videoRound.skillActions.Add(skillAction_1006);
        videoRound.skillActions.Add(skillAction_1007);

        return videoRound;
    }

    public VideoRound GetGuideRound2()
    {
        VideoRound videoRound = new VideoRound();
        videoRound.battleId = 1;
        GameDebuger.TODO(@"videoRound.count = 2;");
        GameDebuger.TODO(@"videoRound.over = false;");
        videoRound.skillActions = new List<VideoSkillAction>();
		
        //2003	1	小鬼c	1005	剑侠	普通攻击	1	539	2922
        VideoSkillAction skillAction_2003 = NewVideoSkillAction(2003, 1, 10);
        AddVideoTargetState(skillAction_2003, 1, 1005, -539, 2922, false, false, false);
		
        //2004	2	小鬼d	1006	灵狐	普通攻击	1	471	3001
        VideoSkillAction skillAction_2004 = NewVideoSkillAction(2004, 1, 10);
        AddVideoTargetState(skillAction_2004, 1, 1006, -471, 3001, false, false, false);
		
        //2005	3	小鬼e	1007	熊猫	深渊烈焰	5355	584	3416
        //2005	3	小鬼e	1004	太子	深渊烈焰	5355	584	2875
        //2005	3	小鬼e	1001	孙悟空	深渊烈焰	5355	584	6641
        VideoSkillAction skillAction_2005 = NewVideoSkillAction(2005, 5355, 10);
        AddVideoTargetState(skillAction_2005, 1, 1007, -584, 3416, false, false, false);
        AddVideoTargetState(skillAction_2005, 2, 1004, -584, 2875, false, false, false);
        AddVideoTargetState(skillAction_2005, 3, 1001, -584, 6641, false, false, false);
		
        //2006	4	BOSS	1003	明月	炎狱	5507	1501	1950
        //2006	4	BOSS	1002	落日剑客	炎狱	5507	1516	7357
        //2006	4	BOSS	1004	太子	炎狱	5507	1527	1348
        //2006	4	BOSS	1005	剑侠	炎狱	5507	1564	1358
        //2006	4	BOSS	1006	灵狐	炎狱	5507	1576	1425
        //2006	4	BOSS	1007	熊猫	炎狱	5507	1589	1827
        //2006	4	BOSS	1008	飞燕	炎狱	5507	1535	1924
        //2006	4	BOSS	1001	孙悟空	炎狱	5507	1542	5099
        VideoSkillAction skillAction_2006 = NewVideoSkillAction(2006, 5507, 10);
        AddVideoTargetState(skillAction_2006, 1, 1003, -1501, 1950, false, false, false);
        AddVideoTargetState(skillAction_2006, 2, 1002, -1516, 7357, false, false, false);
        AddVideoTargetState(skillAction_2006, 3, 1004, -1527, 1348, false, false, false);
        AddVideoTargetState(skillAction_2006, 4, 1005, -1564, 1358, false, false, false);
        AddVideoTargetState(skillAction_2006, 5, 1006, -1576, 1425, false, false, false);
        AddVideoTargetState(skillAction_2006, 6, 1007, -1589, 1827, false, false, false);
        AddVideoTargetState(skillAction_2006, 7, 1008, -1535, 1924, false, false, false);
        AddVideoTargetState(skillAction_2006, 8, 1001, -1542, 5099, false, false, false);
		
        //1001	5	孙悟空	2006	BOSS	血战八方	1111	2178	11904
        //1001	5	孙悟空	2003	吸血鬼	血战八方	1111	2324	3576
        //1001	5	孙悟空	2005	地狱火魂	血战八方	1111	2784	1024
        VideoSkillAction skillAction_1001 = NewVideoSkillAction(1001, 1111, 10);
        AddVideoTargetState(skillAction_1001, 1, 2006, -2178, 11904, false, false, false);
        AddVideoTargetState(skillAction_1001, 2, 2003, -2324, 3576, false, false, false);
        AddVideoTargetState(skillAction_1001, 3, 2005, -2784, 1024, false, false, false);
		
        //1002	6	落日剑客	2006	BOSS	背水一战	1116	1024	10880
        //1002	6	落日剑客	2005	地狱火魂	背水一战	1116	1024	0
        //1002	6	落日剑客	2004	鬼姬	背水一战	1116	1024	1918
        VideoSkillAction skillAction_1002 = NewVideoSkillAction(1002, 1116, 10);
        AddVideoTargetState(skillAction_1002, 1, 2006, -1024, 10880, false, false, false);
        AddVideoTargetState(skillAction_1002, 2, 2005, -1024, 0, true, true, false);
        AddVideoTargetState(skillAction_1002, 3, 2004, -1024, 1918, false, false, false);
		
        //1003	7	明月	2003	吸血鬼	翻云覆雨	1511	838	2738
        //1003	7	明月	2004	鬼姬	翻云覆雨	1511	853	1065
        //1003	7	明月	2006	BOSS	翻云覆雨	1511	645	10235
        VideoSkillAction skillAction_1003 = NewVideoSkillAction(1003, 1511, 10);
        AddVideoTargetState(skillAction_1003, 1, 2003, -838, 2738, false, false, false);
        AddVideoTargetState(skillAction_1003, 2, 2004, -853, 1065, false, false, false);
        AddVideoTargetState(skillAction_1003, 3, 2006, -645, 10235, false, false, false);
		
        //1004	8	太子	2003	吸血鬼	飞剑穿心	1412	1369	1369
        //1004	8	太子	2003	吸血鬼	飞剑穿心	1412	1369	0
        VideoSkillAction skillAction_1004 = NewVideoSkillAction(1004, 1412, 10);
        AddVideoTargetState(skillAction_1004, 1, 2003, -1369, 1369, false, false, false);
        AddVideoTargetState(skillAction_1004, 2, 2003, -1369, 0, true, false, false);
		
        //1008	9	飞燕	2004	鬼姬	定身符	1315	0	1065 with buff 109
        VideoSkillAction skillAction_1008 = NewVideoSkillAction(1008, 1315, 10);
        AddVideoTargetState(skillAction_1008, 1, 2004, 0, 1065, false, false, false, 109);
		
        //1005	10	剑侠	1002	落日剑客	慈悲为怀	1212	1268	8625
        //1005	10	剑侠	1005	剑侠	慈悲为怀	1212	1268	1626
        //1005	10	剑侠	1006	灵狐	慈悲为怀	1212	1268	1693
        VideoSkillAction skillAction_1005 = NewVideoSkillAction(1005, 1212, 10);
        AddVideoTargetState(skillAction_1005, 1, 1002, 1268, 8625, false, false, false);
        AddVideoTargetState(skillAction_1005, 2, 1005, 1268, 2626, false, false, false);
        AddVideoTargetState(skillAction_1005, 3, 1006, 1268, 2693, false, false, false);
		
        //1006	11	灵狐	2006	BOSS	勾魂	1914	1500	8735
        //1006	11	灵狐	1006	灵狐	勾魂	1914	750	2175
        VideoSkillAction skillAction_1006 = NewVideoSkillAction(1006, 1914, 10);
        AddVideoTargetState(skillAction_1006, 1, 2006, -1500, 8735, false, false, false);
        AddVideoTargetState(skillAction_1006, 1, 1006, 750, 3443, false, false, false);
		
        //1007	12	熊猫	2004	鬼姬	三昧真火	1712	1065	0
        VideoSkillAction skillAction_1007 = NewVideoSkillAction(1007, 1712, 10);
        AddVideoTargetState(skillAction_1007, 1, 2004, -1065, 0, true, false, false);

        videoRound.skillActions.Add(NewVideoShoutAction(2003, "明明可以为我族效力！"));
        videoRound.skillActions.Add(skillAction_2003);
        videoRound.skillActions.Add(NewVideoShoutAction(2004, "真是不知好歹！"));
        videoRound.skillActions.Add(skillAction_2004);
        videoRound.skillActions.Add(skillAction_2005);
        videoRound.skillActions.Add(NewVideoShoutAction(2006, "战神大人的计划不允许任何人破坏！"));
        videoRound.skillActions.Add(skillAction_2006);
		
        videoRound.skillActions.Add(NewVideoShoutAction(1001, "那样的计划天理不容！"));
        videoRound.skillActions.Add(skillAction_1001);
        videoRound.skillActions.Add(NewVideoShoutAction(1002, "集结大家的力量，一定可以渡过难关！"));
        videoRound.skillActions.Add(skillAction_1002);
        videoRound.skillActions.Add(NewVideoShoutAction(1003, "就让这大雨冲刷掉世间的黑暗！"));
        videoRound.skillActions.Add(skillAction_1003);
        videoRound.skillActions.Add(NewVideoShoutAction(1004, "正义的飞剑将惩罚世间的罪恶！"));
        videoRound.skillActions.Add(skillAction_1004);
        videoRound.skillActions.Add(NewVideoShoutAction(1008, "有我在，休想行动！"));
        videoRound.skillActions.Add(skillAction_1008);
        videoRound.skillActions.Add(NewVideoShoutAction(1005, "我会在背后守护大家！"));
        videoRound.skillActions.Add(skillAction_1005);
        videoRound.skillActions.Add(NewVideoShoutAction(1006, "每个人都有存在的价值与意义！"));
        videoRound.skillActions.Add(skillAction_1006);
        videoRound.skillActions.Add(NewVideoShoutAction(1007, "伴随火焰消散吧！"));
        videoRound.skillActions.Add(skillAction_1007);
		
        return videoRound;
    }

    public VideoRound GetGuideRound3()
    {
        VideoRound videoRound = new VideoRound();
        videoRound.battleId = 1;
        GameDebuger.TODO(@"videoRound.count = 3;");
        videoRound.over = true;
        videoRound.winId = ModelManager.Player.GetPlayerId();
        videoRound.skillActions = new List<VideoSkillAction>();

        //2006	1	BOSS	1006	灵狐	斩魂	5506	1425	0
        //2006	1	BOSS	1001	孙悟空	斩魂	5506	4444	655
        //2006	1	BOSS	1003	明月	斩魂	5506	1950	0
        //2006	1	BOSS	1004	太子	斩魂	5506	1348	0
        //2006	1	BOSS	1005	剑侠	斩魂	5506	1358	0
        //2006	1	BOSS	1007	熊猫	斩魂	5506	1827	0
        //2006	1	BOSS	1008	飞燕	斩魂	5506	1924	0
        var skillAction_2006 = NewVideoSkillAction(2006, 5506, 10);
        AddVideoTargetState(skillAction_2006, 1, 1006, -3443, 0, true, false, false);
        AddVideoTargetState(skillAction_2006, 2, 1001, -4444, 655, false, false, false);
        AddVideoTargetState(skillAction_2006, 3, 1003, -1950, 0, true, false, false);
        AddVideoTargetState(skillAction_2006, 4, 1004, -1348, 0, true, false, false);
        AddVideoTargetState(skillAction_2006, 5, 1005, -2626, 0, true, false, false);
        AddVideoTargetState(skillAction_2006, 6, 1007, -1827, 0, true, false, false);
        AddVideoTargetState(skillAction_2006, 7, 1008, -1924, 0, true, false, false);
		
        //1002	2	落日剑客	2006	BOSS	长河落日	5508	8735	0
        var skillAction_1002 = NewVideoSkillAction(1002, 5508, 10);
        AddVideoTargetState(skillAction_1002, 1, 2006, -8735, 0, true, false, true);

        videoRound.skillActions.Add(NewVideoShoutAction(2006, "愚蠢的家伙们！接受吾辈的制裁！"));
        videoRound.skillActions.Add(skillAction_2006);

        videoRound.skillActions.Add(NewVideoShoutAction(1002, "该接受制裁的，是你才对！"));
        videoRound.skillActions.Add(skillAction_1002);
		
        return videoRound;
    }

    private VideoShoutAction NewVideoShoutAction(long actionSoldierId, string shoutContent)
    {
        GameDebuger.TODO(@"VideoShoutAction action = new VideoShoutAction();
        action.actionSoldierId = actionSoldierId;
        action.shoutContent = shoutContent;
        action.targetStateGroups = new List<VideoTargetStateGroup>();
        return action;");
        return null;
    }

    private VideoSkillAction NewVideoSkillAction(long actionSoldierId, int skillid, int mpSpent)
    {
        var action = new VideoSkillAction();
        action.actionSoldierId = actionSoldierId;
        action.skillId = skillid;
        GameDebuger.TODO(@"action.mpSpent = mpSpent;");
        return action;
    }

    private VideoTargetState NewVideoTargetState(long id, int hp, int currentHp, int currentCp, bool dead, bool leave, bool crit)
    {
        var state = new VideoActionTargetState();
        state.id = id;
        state.dead = dead;
        state.leave = leave;
        state.crit = crit;
        state.hp = hp;
        state.currentHp = currentHp;
        state.currentCp = currentCp;
        return state;
    }

    private VideoTargetState NewVideoBuffAddTargetState(long id, bool dead, bool leave, int buffId)
    {
        var state = new VideoBuffAddTargetState();
        state.id = id;
        state.dead = dead;
        state.leave = leave;
        GameDebuger.TODO(@"state.round = 2;");
        state.battleBuffId = buffId;
        return state;
    }

    private void AddVideoTargetState(VideoSkillAction skillAction, int groupIndex, long id, int hp, int currentHp, bool dead, bool leave, bool crit, int buffId = 0)
    {
        if (skillAction.targetStateGroups == null)
        {
            skillAction.targetStateGroups = new List<VideoTargetStateGroup>();
        }

        if (groupIndex > skillAction.targetStateGroups.Count)
        {
            var group = new VideoTargetStateGroup();
            skillAction.targetStateGroups.Add(group);
            group.targetStates = new List<VideoTargetState>();
        }

        skillAction.targetStateGroups[groupIndex - 1].targetStates.Add(NewVideoTargetState(id, hp, currentHp, 0, dead, leave, crit));
        if (buffId > 0)
        {
            skillAction.targetStateGroups[groupIndex - 1].targetStates.Add(NewVideoBuffAddTargetState(id, dead, leave, buffId));
        }
    }

    #endregion

    public bool IsCurrentPvpBattle()
    {
        GameDebuger.TODO(@"return _currentGameVideo is PvpVideo;");
        return false;
    }

    //是否竞技场
    public bool IsChallengePvpBattle()
    {
        if (!IsCurrentPvpBattle())
            return false;

        GameDebuger.TODO(@"PvpVideo video = _currentGameVideo as PvpVideo;
        if (video.type == PvpVideo.PvpTypeEnum_Challenge)
            return true;");

        return false;
    }

    //是否支持弹幕
    public bool IsSupportBarrage()
    {
        GameDebuger.TODO(@"if (_currentGameVideo is PvpVideo)
        {
            PvpVideo pvpVideo = _currentGameVideo as PvpVideo;
            if (pvpVideo.type != PvpVideo.PvpTypeEnum_Challenge
            && pvpVideo.type != PvpVideo.PvpTypeEnum_SiegeBattle)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (_currentGameVideo is NpcMonsterVideo)
        {
            NpcMonsterVideo npcMonsterVideo = _currentGameVideo as NpcMonsterVideo;
            if (npcMonsterVideo.npcSceneMonsterBronType == NpcMonsterVideo.NpcSceneMonsterBronType_WorldBoss)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {");
        return false;
        GameDebuger.TODO(@"}");
    }

    private void AdjustGameContingent()
    {
        //调整队伍，把主角的队伍放到A队, 也就是判断B队伍是否主角，如果是主角， 则换成AB队互换
        //      if (_data.GameVideo is NpcMonsterVideo)
            //      {
            //          NpcAppearanceDto npcAppearanceDto = (_data.GameVideo as NpcMonsterVideo).npcAppearanceDto;
            //          if (npcAppearanceDto != null)
            //          {
            //              for (int i=0,len=_data.GameVideo.bteam.teamSoldiers.Count; i<len; i++)
            //              {
            //                  VideoSoldier soldier = _data.GameVideo.bteam.teamSoldiers[i];
            //                  if (soldier.id == npcAppearanceDto.soldierId)
            //                  {
            //                      soldier.wpmodel = npcAppearanceDto.wpmodel;
            //                  }
            //              }
            //          }
            //      }

            GameDebuger.TODO(@"if (mIsWatchMode)
        {
            if (_data.GameVideo.bteam.id == _watchTeamId)
            {
                VideoTeam temp = _data.GameVideo.ateam;
                _data.GameVideo.ateam = _data.GameVideo.bteam;
                _data.GameVideo.bteam = temp;
            }
        }
        else");
            {
                var playerId = ModelManager.Player.GetPlayerId();
                if (_data.ContainPlayer(_data.GameVideo.bteam.teamSoldiers, playerId))
                {
                    var temp = _data.GameVideo.ateam;
                    _data.GameVideo.ateam = _data.GameVideo.bteam;
                    _data.GameVideo.bteam = temp;
                }
            }

            GameDebuger.TODO(@"if (_data.GameVideo.ateam.formation == null)
        {
            GameDebuger.LogError('ateam阵法设置为null， 请联系服务器端检查，此处强行设置阵型为1');
            _data.GameVideo.ateam.formationId = 1;
        }

        if (_data.GameVideo.bteam.formation == null)
        {
            GameDebuger.LogError('bteam阵法设置为null， 请联系服务器端检查，此处强行设置阵型为1');
            _data.GameVideo.bteam.formationId = 1;
        }");

            _data.GameVideo.ateam.teamSoldiers.Sort(OnVideoSoldierSort);
            _data.GameVideo.bteam.teamSoldiers.Sort(OnVideoSoldierSort);

            GameDebuger.TODO(
                @"if (_data.GameVideo is PvpVideo && ((PvpVideo)_data.GameVideo).type == PvpVideo.PvpTypeEnum_Arena /**&& !mIsWatchMode*/)
        {
            VideoSoldier leader = GetTeamLeader(_data.GameVideo.enemyTeam);
            if (leader != null)
            {
                string nameStr = string.Format('{0}（{1}级）', leader.name, leader.level);
                nameStr = nameStr.WrapColor(ColorConstantV3.Color_Green);
                TipManager.AddTip(string.Format('你进入了与{0}的切磋战斗', nameStr));
            }
        }"); 
    }
    
    private static int OnVideoSoldierSort(VideoSoldier x, VideoSoldier y)
    {
        return x.position - y.position;
    }

    private bool ValidateChooseTarget(MonsterController target)
    {
        if (!_data.CanUseCommand()
            || _data.LockUI
            || _data.battleState != BattleSceneStat.BATTLE_PlayerOpt_Time
            || _data.CurActMonsterController == null)
        {
            return false;
        }

        GameDebuger.TODO(@"if (_guideBattle)
        {
            _data.choosePet.NeedReady = false;
            _data.choosePet = null;
            target.PlayTargetClickEffect();
            OnActionRequestSuccess(null);

            return false;
        }");

        return _data.CurActMonsterController != null
            && _data.CurActMonsterController.TargetSelector.CanSetTarget(_data.CurActMonsterController, target)
            && _data.CurActMonsterController.TargetSelector.CanSetCaptureTarget(_data.CurActMonsterController, target);
    }

    /// <summary>
    ///     检查战斗是否已结束
    /// </summary>
    public void CheckBattleOver(ErrorResponse e)
    {
        //        //战斗已结束，直接结束游戏   
        if (e.id == AppErrorCodes.BATTLE_ID_NOT_FOUND
            || DataMgr._data.IsGameOver)
        {
             ExitBattle(); 
        }
    }

    public void ExitBattle()
    {
        GameLog.Log_Battle("ExitBattle--------------");
        //强制退出当前战斗
        if (!IsInBattle) return;

        GameDebuger.TODO(@"if (mIsWatchMode)
                {
                    ServiceRequestAction.requestServer(CommandService.exitWatchBattle(GameVideo.id));
                }
                else
        {
            ServiceRequestAction.requestServer(
                Services.Battle_Leave(DataMgr._data.GameVideo.id));
        }");

        BattleInstController.Instance.ClearUnPlayVideo();
        FireData();
        //ExitBattleWithoutReport();

//        CheckGameState();
//        BattleInstController.Instance.ShowBattleResult();
    }

    public void LateExitBattle()
    {
        GameEventCenter.SendEvent(GameEvent.BATTLE_FIGHT_EXITBATTLE);
        CheckResult();
        BattleDestroy(_data.Result, _data._isDead, _data.GameVideo);
    }

    public bool IsGameOver {
        set
        {
            if (_data.IsGameOver != value)
            {
                _data.IsGameOver = value;
                FireData();
            }
        }
    }

    private BattleSceneStat CaculateBattleState(){
        
        UpdateCurDto();

        var state = BattleSceneStat.Invalid;

        if (_data.IsGameOver && !BattleInstController.Instance.VideoPlaying)
            state = BattleSceneStat.Battle_OVER;
        else
        {
            var temp = _data._actionQueueDtoSet.IsNullOrEmpty()
                ? 0
                : _data._actionQueueDtoSet[0].round;
            temp = Math.Max(temp, _data.CurRoundCnt);

            if (_data.CurRoundCnt == 0
                && !_data._isTimeCountFinish)
            {
                state = BattleSceneStat.BATTLE_PRESTART;
            }
            // 每一次会把所有能播放的video播完，再进入等待
            else if ( BattleInstController.Instance.VideoPlaying)
            {
                state = BattleSceneStat.BATTLE_VIDEOROUND_PLAYING;
            }
            // 如果没有在播放，有三种情况 等actdto， 请求使用技能网络交互中， 等videoround， 数据表达做得不是非常好
            //等actdto，等videoround其实重叠左
            // 并不保证video下发之前必然下发curActDto
            else if (_data.curActDto == null 
                || (!_data._actionQueueDtoSet.IsNullOrEmpty() 
                    && _data._actionQueueDtoSet[0].round == _data.CurRoundCnt)) 
            {
                state = BattleSceneStat.ON_WAITING_ActTime_Update;
                DemoSimulateHelper.SimulateRoundStart();  // for test -- fish
            }
            else if (temp < _data.reqActCnt)
            {
                state = BattleSceneStat.BATTLE_WaitVideo;
            }
            else if (_data.curActDto != null
                     && !_data._actionQueueDtoSet.IsNullOrEmpty()
                    && _data._actionQueueDtoSet[0].round > _data.CurRoundCnt
                    && _data._actionQueueDtoSet[0].round >=_data.reqActCnt)
            {
                state = BattleSceneStat.BATTLE_PlayerOpt_Time;
                DemoSimulateHelper.SimulateRoundStart();  // for test -- fish
            }
        }

        return state;
    }
    // state 只是一个compurited data，所以不会fire data
    public void UpdateBattleState()
    {
        var state = CaculateBattleState();
        if (state == BattleSceneStat.Invalid)
            return;
        DataMgr._data.battleState = state;
        
        GameLog.Log_Battle(string.Format("UpdateByState state:{0}", state.ToString()));
    }

    // 这个函数最终要被删除 fish
    public void SetState(BattleSceneStat state)
    {
        GameLog.Log_Battle(string.Format("UpdateByState state:{0}", state.ToString()));
        UpdateBattleState();
        if (DataMgr._data.battleState != BattleSceneStat.Invalid)
        {
            DataMgr._data.battleState = state;
        }
        
        FireData();
    }

    public void CheckGameState()
    {
        if (_data.battleState == BattleSceneStat.Battle_OVER)
        {
            CheckResult();
            FireData();
        }
        else if (_data.battleState == BattleSceneStat.BATTLE_PRESTART)
        {
            FireData();
            return;
        }
        else
        {
            _data.LockUI = false;
            CheckNextRound();
        }
    }
    
    private void CheckResult()
    {
        if (_data.Result == BattleResult.UNKNOW)
        {
            _data.Result = _data.IsPlayerTeamLeaderId(_data.WinId) ? BattleResult.WIN : BattleResult.LOSE;
        }

        var myHero = MonsterManager.Instance.GetMyHero();
        _data._isDead = myHero == null || myHero.IsDead();
    }
    
    private void CheckNextRound()
    {
//        MonsterManager.Instance.UpdateBuffState();
        BattleInstController.Instance.CheckNextRound();
    }

    //从战斗中逃跑
    public void RetreatBattle(long playerId, List<long> retreatSoldiers)
    {
        var exitBattle = playerId == ModelManager.Player.GetPlayerId();
        if (exitBattle)
        {
            _data.Result = BattleResult.LOSE;
        }

        var delayTime = MonsterManager.Instance.RetreatOtherMonster(retreatSoldiers);
        RetreatOtherMonster(delayTime, exitBattle);
    }

    private void RetreatOtherMonster(float delayTime, bool exitBattle)
    {
        JSTimer.CdTask.OnCdFinish func = delegate
        {
            if (exitBattle)
            {
                _data.IsGameOver = true;
            
                FireData();
            }
            else
            {
                AfterRetreatOtherMonster();
            }
        };

        if (delayTime > 0)
        {
            JSTimer.Instance.SetupCoolDown("AfterRetreat", delayTime, null, func);
        }
        else
        {
            func();
        }
    }

    private void AfterRetreatOtherMonster()
    {
        TipManager.AddTip("由于你逃跑了，战斗失败");
    }

    //销毁战斗
    public void DestroyBattle()
    {
        BattleNetworkHandler.StopNotifyListener();

        JSTimer.Instance.SetupCoolDown("DelayDestroyBattle", 0.5f, null, DelayDestroyBattle);
    }

    private void DelayDestroyBattle()
    {
        GameDebuger.TODO(@"if (_guideBattle)
        {
            //TalkingDataHelper.OnEventSetp('StartBattle', 'End');
        }");

        DataMgr.BattleDestroy(DataMgr.BattleDemo.Result, DataMgr.BattleDemo.MainBattleView.IsDead, DataMgr._data.GameVideo);
    }

    private void UpdateCurDto()
    {
        if (BattleInstController.Instance.VideoPlaying)
        {
            _data.curActDto = BattleInstController.Instance._playRound.actionQueue;
        }
        else
        {
            _data.curActDto = _data._actionQueueDtoSet.Find(
                s => s.round == (_data.CurRoundCnt + 1)
                , notify => notify == null ? null : notify.actionQueue);
            if (_data.curActDto == null && BattleInstController.Instance._playRound != null)
                _data.curActDto = BattleInstController.Instance._playRound.actionQueue;
        }
    }

    public void HandelActionQueueDto(ActionReadyNotify dto)
    {
        
        if (dto == null
            ||!_data.IsSameBattle(dto.battleId)
            || dto.round <= _data.CurRoundCnt) return;
        
        if (dto != null)
        {
            var id = dto.actionQueue.soldierQueue[0];
            GameLog.Log_Battle(string.Format("HandelActionQueueDto----------round   {0} id = {1}", dto.round, id));
        }
        
        _data._actionQueueDtoSet.ReplaceOrAdd(s=>s.round == dto.round, dto);
        _data._actionQueueDtoSet.Sort(delegate(ActionReadyNotify x, ActionReadyNotify y) { return y.round - x.round; });

        FireData();
    }
    
    public void HandleBattleAutoNotify(BattleAutoNotify notify)
    {
        if (notify == null) return;
        _data.isAIManagement = notify.auto;
        FireData();
    }

    //被DEBUFF等打断后的处理，RM:http://oa.cilugame.com/redmine/issues/15011
    public void DealCDForInterrupttedByDebuff(long pPreActionUID)
    {
        if (!BattleInstController.Instance.IsInActionQueue(pPreActionUID) || !MonsterManager.Instance.IsInCD(pPreActionUID))
        {
            GameDebuger.Log(string.Format("DealCDForInterrupttedByDebuff canceled , IsInAllActionQueueDic({0}) == false || IsInCD({0}) == false ", pPreActionUID));
            return;
        }
        
        //重新吟唱，不论是谁。
        BattleNetworkManager.HandlerSoldierReadyNotify(_data.GameVideo.id, pPreActionUID, BattleDemoModel.CD_FOR_INTERRUPT, true);
    }

    private bool CanRetreat(out string tip)
    {
        tip = "超过15个行动才能逃跑！";
        return _data.CurRoundCnt > 15;
    }

    public void UpdateMonsterAttr(VideoRound gameRound)
    {
        if (gameRound == null)
            return;
        MonsterManager.Instance.UpdateMonsterAttr(gameRound.id, gameRound.acReward);
    }

    public BattleResult IsWin
    {
        get
        {
            if (BattleInstController.Instance._playRound == null
                || !BattleInstController.Instance._playRound.over)
                return BattleResult.UNKNOW;
            return _data.WinId > 0 && _data.IsPlayerTeamLeaderId(_data.WinId) ? BattleResult.WIN : BattleResult.LOSE;
        }
    }

    private BattlePosition.MonsterSide GetMonsterSide(long playerId)
    {
        BattlePosition.MonsterSide monsterSide;

        if ( _data._gameVideo.ateam.playerIds.Contains(playerId))
        {
            monsterSide = BattlePosition.MonsterSide.Player;
        }
        else
        {
            monsterSide = BattlePosition.MonsterSide.Enemy;
        }

        return monsterSide;
    }
    
    private BattleCrewData.CrewState GetCrewBattleState(long crewId)
    {
        var playerInfoDto = _data._gameVideo.playerInfos.Find(info=>info.playerId == ModelManager.IPlayer.GetPlayerId());
        var exit = playerInfoDto != null && !playerInfoDto.allCrewSoldierIds.IsNullOrEmpty() && playerInfoDto.allCrewSoldierIds.Exists(id=>id == crewId);
        return exit ? BattleCrewData.CrewState.None : BattleCrewData.CrewState.Fight;
    }
}