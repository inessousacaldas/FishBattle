using System;
using AppDto;
using AppServices;
using MyGameScripts.Gameplay.Battle.Demo.Helper;

public sealed partial class BattleDataManager
{
    public static class BattleNetworkManager
    {
        #region 正式协议

        public static void EnterBattle(int pSceneId, FightersConfigDto pFightersConfigDto, Action pFinishCallBack = null,Video pVideo = null)
        {
            GameLog.Log_Battle("BattleNetworkManager EnterBattle---");
            var tVideo = pVideo;
            
            if (null == tVideo)
                tVideo = DemoSimulateHelper.SimulateVideo(pFightersConfigDto);
            if (ServiceRequestAction.SimulateNet)
            {
                OnEnterBattleSuccess(tVideo);
                if (null != pFinishCallBack)
                    pFinishCallBack();
            }
            else
            {
                //服务端限制了包体大小，包体过大会被裁掉。所以这里先压缩一下。
                var compressString = LITJson.JsonMapper.ToJson(pFightersConfigDto);
                GameDebuger.Log(string.Format("EnterBattle pSceneId：{0},pFightersConfigDto：{1}", pSceneId.ToString(), compressString));
                GameLog.Log_Battle(string.Format("EnterBattle pSceneId：{0},pFightersConfigDto：{1}", pSceneId.ToString(), compressString));
                compressString = ZipLibUtils.CompressString(compressString);
                ServiceRequestAction.requestServer(Services.Battle_Enter(compressString), "Battle_Enter", (e) =>
                {
                    if (null != pFinishCallBack)
                        pFinishCallBack();
                });
            }
        }

        public static void OnEnterBattleSuccess(Video pVideo)
        {
            OnEnterBattleSuccess(pVideo, null);
        }

        private static void OnEnterBattleSuccess(Video pVideo, Action pFinishCallBack)
        {
            if (null == pVideo)
            {
                GameDebuger.LogError("OnEnterBattleSuccess failed , request EnterBattleCommand success but the response is null or not type of Video!");
                return;
            }
            /**BattleNetworkManager.DataMgr.HandleDemoVideoListener(DemoS1SimulateHelper.SimulateDemoVideo(pEnemyBattleDemoS1ConfigDtoList, pFriendBattleDemoS1ConfigDtoList));*/
//            pVideo.ateam.teamSoldiers.ForEach(s=>GameLog.Log_Battle("HandleDemoVideo----ateam skill cnt---" + s.skillIds.Count));
//            pVideo.bteam.teamSoldiers.ForEach(s=>GameLog.Log_Battle("HandleDemoVideo----bteam skill cnt---" + s.skillIds.Count));

            HandleDemoVideo(pVideo);
            GameUtil.SafeRun(pFinishCallBack);
        }

        public static void HandleDemoVideo(Video pVideo)
        {
            //场景编号:1\2\3  3D无镜头场景、3D普通镜头场景、3DBOSS镜头场景
            if (!(pVideo is PreviewVideo))
            {
                if (ModelManager.BattleDemoConfig.BattleCameraId == 0)
                {
                    pVideo.mapId = 0;
                }
                else
                {
                    pVideo.mapId = pVideo.mapId > 0 ? pVideo.mapId : ModelManager.BattleDemoConfig.BattleSceneId;
                    pVideo.cameraId = ModelManager.BattleDemoConfig.BattleCameraId;
                }
            }
            var watchTeamId = ModelManager.BattleDemoConfig.DemoBattleMode != BATTLE_DEMO_MODEL_S1.Battle 
                ? pVideo.bteam.id
                : 0;
            var id = ModelManager.IPlayer.GetPlayerId();
            var soldier = pVideo.ateam.teamSoldiers.Find(s => s.id == id) ?? pVideo.bteam.teamSoldiers.Find(s => s.id == id);
            if (soldier != null)
            {
                var str = string.Format("round = {0} soldier.hp = {1}",pVideo.currentRound, soldier.hp);
                str += string.Format(" soldier.cp = {0}",soldier.cp);
                str += string.Format(" soldier.ep = {0}",soldier.ep);
                GameLog.Log_Battle_PlayerAttr(str);
            }

            BattleManager.Instance.PlayBattle(pVideo, watchTeamId);   

        }

        public static void HandlerSoldierReadyNotify(FighterReadyNotifyDto notify)
        {
            if (notify == null)
                return;
            GameDebuger.LogBattleInfo(string.Format("收到战士准备就绪通知，玩家id:{0}，技能id：{1}，吟唱时间：{2}，当前时间：{3}", 
                notify.id, notify.skillId, notify.releaseTime, SystemTimeManager.Instance.GetServerTime().ToString("o")));
            HandlerSoldierReadyNotify(notify.battleId, notify.id, (float)notify.releaseTime / 1000f);
        }

        public static void HandlerSoldierReadyNotify(long pBattleId, long pId, float pReleaseTime, bool pPlayReverse = false)
        {
            if (!DataMgr.BattleDemo.IsSameBattle(pBattleId))
                return;
        
            var mc = MonsterManager.Instance.GetMonsterFromSoldierID(pId);
            if (mc == null) return;
            mc.NeedReady = false;
            mc.IsInCD = true;
            mc.UpdateSkillCD(pReleaseTime, pPlayReverse, () =>
            {
                mc.IsInCD = false;
            });
            FireData();
        }

//    public void HandlerActionQueueAddNotifyDto(ActionQueueAddNotifyDto pActionQueueAddNotifyDto)
//    {      
//        ModelManager.BattleDemo.AddToActionQueue(pActionQueueAddNotifyDto);
//    }

//    public void HandlerActionQueueAddNotifyDto(long pPlayerId, string pPlayerName, long pPlayTime, int pDurationTime)
//    {      
//        ActionQueueAddNotifyDto tActionQueueAddNotifyDto = new ActionQueueAddNotifyDto();
//        tActionQueueAddNotifyDto.battleId = ModelManager.BattleDemo.GameVideo.id;
//        tActionQueueAddNotifyDto.id = pPlayerId;
//        tActionQueueAddNotifyDto.name = pPlayerName;
//        tActionQueueAddNotifyDto.time = pPlayTime;
//        tActionQueueAddNotifyDto.durationTime = pDurationTime;
//        ModelManager.BattleDemo.AddToActionQueue(tActionQueueAddNotifyDto);
//    }

        //因被控制等无法执行之前的技能或无法行动时，服务端发送过来取消队列的通知。注意目标可能不在行动队列
//    public void HandlerActionQueueRemoveNotifyDto(ActionQueueRemoveNotifyDto pFighterReadyNotifyDto)
//    {
//        GameDebuger.LogBattleInfo(string.Format("HandlerActionQueueRemoveNotifyDto id:{0}", pFighterReadyNotifyDto.id));
//        if (null != pFighterReadyNotifyDto)
//        {
//            HandlerActionQueueRemoveNotifyDto(pFighterReadyNotifyDto.battleId, pFighterReadyNotifyDto.id);
//        }
//        else
//        {
//            GameDebuger.LogError(string.Format("[错误]HandlerActionQueueRemoveNotifyDto id:{0} failed , battleId:{1} is invalid !", pFighterReadyNotifyDto.id, pFighterReadyNotifyDto.battleId));
//        }
//    }

//    public void HandlerActionQueueRemoveNotifyDto(long pBattleId, long pPlayerUID)
//    {
//        GameDebuger.LogBattleInfo(string.Format("HandlerActionQueueRemoveNotifyDto id:{0}", pPlayerUID));
//        if (ModelManager.BattleDemo.IsSameBattle(pBattleId))
//        {
//            if (ModelManager.BattleDemo.ShowTip)
//            {
//                string tString = string.Format("收到从队列中移除的通知,玩家id：{0},当前时间：{1}", pPlayerUID.ToString(), SystemTimeManager.DataMgr.GetServerTime().ToString("o"));
//                GameDebuger.LogBattleInfo(tString);
//                //                TipManager.AddTip(tString);
//            }
//            ModelManager.BattleDemo.RemoveFromActionQueueForcibly(pPlayerUID);
//        }
//        else
//        {
//            GameDebuger.LogError(string.Format("[错误]HandlerActionQueueRemoveNotifyDto id:{0} failed , battleId:{1} is invalid !", pPlayerUID, pBattleId));
//        }
//    }

        //S1:本处处理一个攻防信息，比如A攻击了B，其中A用了什么技能、B的受击信息（包括但不限于受到的效果，比如掉血、加血、闪避、捕捉等）2017-02-18 09:23:36
        public static void HanderVideoRound(VideoRound pVideoRound)
        {
            //战斗回合下发， PVP中， 当双方都请求了开战， 服务器主动下发，PVE则不通过下发这个， 直接请求接口返回

            #region LOG
            if (!pVideoRound.IsBattleValid())
            {
                return;
            }

            if (!DataMgr.BattleDemo.IsSameBattle(pVideoRound.battleId))
            {
                return;
            }

            if (null == pVideoRound.skillActions || pVideoRound.skillActions.Count <= 0)
            {
                GameDebuger.LogError(string.Format("[错误]战斗回合数据 VideoRound 有误，VideoRound.skillActions 长度{0}问题！", (pVideoRound.skillActions != null) ? "有" : "无"));
                return; 
            }

            var tVideoSkillAction = pVideoRound.skillActions[0];

            if (DataMgr.BattleDemo.ShowTip)
            {
                if (tVideoSkillAction == null)
                    GameLog.Log_Battle("tVideoSkillAction == null");
                if (tVideoSkillAction.skill == null)
                    GameLog.Log_Battle("tVideoSkillAction.skill == null");
                else
                {
                    var tString = string.Format("收到回合数据通知 round = {0} ，battleId:{1}，是否结束：{2}，actionSoldierId:{3},首技能id：{4}，首技能名字：{5}，中了BUFF的目标的ID：{6}，当前时间：{7}"
                        , pVideoRound.round
                        , pVideoRound.battleId, pVideoRound.over, tVideoSkillAction.actionSoldierId, tVideoSkillAction.skillId, tVideoSkillAction.skill.name, 
                        DataMgr.BattleDemo.HasBuff(tVideoSkillAction), SystemTimeManager.Instance.GetServerTime().ToString("o"));
                    GameDebuger.LogBattleInfo(tString);
                    //            TipManager.AddTip(tString);
                }
            }

            var id = pVideoRound.actionQueue.soldierQueue[0];
            GameLog.Log_Battle_RESP(string.Format("HanderVideoRound ------round   {0} pVideoRound.actionQueue.soldierQueue[0].id = {1}" +
                "over = {2} BattlePlayerInfoDto = {3} pVideoRound.winId = {4}" , pVideoRound.round, id, pVideoRound.over.ToString(), pVideoRound.battleId
            , pVideoRound.winId));
            
            pVideoRound.skillActions.ForEach<VideoSkillAction>(s =>
                    s.targetStateGroups.ForEach<VideoTargetStateGroup>(t =>
                        t.targetStates.ForEach<VideoTargetState>(ts=>
                        GameLog.Log_Battle("ts type " + ts.GetType())
            )));
            
            GameLog.Log_Battle_PlayerAttr(string.Format("HanderVideoRound round = {0} ", pVideoRound.round));
            var pid = ModelManager.IPlayer.GetPlayerId();
            if (pid == pVideoRound.id && pVideoRound.acReward != null)
            {
                var str = string.Format("acReward currentHp = {0}", pVideoRound.acReward.currentHp);
                str += string.Format("acReward currentCp = {0}", pVideoRound.acReward.currentCp);
                str += string.Format("acReward currentEp = {0}", pVideoRound.acReward.currentEp);

                GameLog.Log_Battle_PlayerAttr( "pVideoRound.id ==  mainrole, acReward value :" + str);
            }

            var skillAction = pVideoRound.skillActions.Filter(ac => ac.actionSoldierId == pid);
            skillAction.ForEach(ac =>
            {
                var str = string.Format("playerID = {0} action spent Hp = {1}",ac.actionSoldierId, ac.hpSpent);
                str += string.Format("action spent Ep = {0}", ac.epSpent);
                str += string.Format("action spent Cp = {0}", ac.cpSpent);
                GameLog.Log_Battle_PlayerAttr(str);
            });

            pVideoRound.skillActions.ForEach(ac =>
            {
                ac.targetStateGroups.ForEach(target =>
                {
                    target.targetStates.Filter(a=>a is VideoActionTargetState)
                        .ForEach(va =>
                        {
                            if (va.id != pid) return;
                            var v = va as VideoActionTargetState;
                            var str = string.Format("cur hp {0}, offset {1}", v.currentHp, v.hp);
                            str += string.Format("cur cp {0}, offset {1}", v.currentCp, v.cp);
                            str += string.Format("cur ep {0}, offset {1}", v.currentEp, v.ep);
                            GameLog.Log_Battle_PlayerAttr(str);
                        });
                });
            });

            #endregion

            //如果收到的回合数是0,则不处理
            GameDebuger.TODO(@"if (pVideoRound.count == 0)
        {
            return;
        }");
            // todo fish : 清理的地方欠缺考虑 国庆后处理
            var mc = DataMgr._data.GetMainRoleMonster;
            if (mc != null)
                mc.ClearSkill();
            MonsterManager.Instance.UpdateOptionState(pVideoRound.id, MonsterOptionStateManager.MonsterOptionState.Disable);
            BattleInstController.Instance.AddVideoRound(pVideoRound);

            if (DataMgr.IsInBattle && DataMgr.BattleDemo.battleState != BattleSceneStat.BATTLE_PRESTART)
            {
                //EnterBattleMode();
                BattleInstController.Instance.CheckNextRound();
            }
            
            FireData();
            DataMgr.CheckGameState();
        }

        //马上出战斗结果
        public static void ShowBattleResult()
        {
            DataMgr._data.IsGameOver = true;
            DataMgr.UpdateBattleState();
            BattleInstController.Instance.ShowBattleResult();
            FireData();
        }
        #endregion

        #region 非正式协议

        /**public void UpdateSoldiers(VideoSoldierUpdateNotify notify)
        {
            for (int i = 0, len = notify.soldiers.Count; i < len; i++)
            {
                VideoSoldier soldier = notify.soldiers[i];
                MonsterController mc = MonsterManager.DataMgr.GetMonsterFromSoldierID(soldier.id);
                mc.currentHP = soldier.hp;
                mc.currentEp = soldier.mp;
                mc.currentSP = soldier.sp;
                mc.magicMana = soldier.magicMana;
                mc.skillPoint = soldier.skillPoint;
    
                if (mc.IsPlayerMainCharactor())
                {
                    ModelManager.Player.UpdateHpMpSp(mc.currentHP, mc.currentEp, mc.currentSP);
                }
                else if (mc.IsPlayerPet())
                {
                    ModelManager.Pet.UpdateBattlePetHpMp(mc.currentHP, mc.currentEp, soldier.id);
                }
            }
        }*/

        /**public void showOrder(CommandNotify order)
        {
            if (!mIsWatchMode)
            {
                MonsterManager.DataMgr.showOrder(order);
            }
        }*/

        #endregion

        public static void ReqServerWithSimulate(GeneralRequest requestInfo
            , string tip = ""
            , ServiceRequestAction.OnRequestSuccess onSuccess = null
            , ServiceRequestAction.OnRequestError onRequestError = null)
        {
            ServiceRequestAction.requestServerWithSimulate(
                requestInfo
                , tip
                , e =>
                {
                    if (onSuccess != null) onSuccess(e);
                    FireData();
                }
                , e =>
                {
                    if (e != null)
                    {
                        var state = DataMgr.BattleDemo.LockUI = e.id == AppErrorCodes.BATTLE_ID_NOT_FOUND;
                        if (!state)
                            TipManager.AddTip(e.message);
                    }

                    DataMgr.CheckBattleOver(e);

                    if (onRequestError != null)
                    {
                        onRequestError(e);
                    }
                    FireData();
                });
        }

        public static void Req_Use_S_Craft(long battleId,long actorId)
        {
            if (DataMgr.BattleDemo.LockUI)
                return;
            var requestInfo = Services.Battle_UseCrafts(battleId, actorId);

            ReqServerWithSimulate(requestInfo, "", e =>
                {
                    DataMgr._data.LockUI = false;
                    DataMgr._data.isAIManagement = false;
                    var mc = MonsterManager.Instance.GetMonsterFromSoldierID(actorId);
                    if (mc == null) return;
                    if (mc.driving)
                    {
                        mc.driving = false;
                    }

                    mc.Set_S_Skill();
                    FireData();
                }
                , e =>
                {

                });
        }

        public static void ReqChangeDefaultSkill(long actorId, int skillID)
        {
            if (DataMgr.BattleDemo.LockUI)
                return;

            DataMgr.BattleDemo.LockUI = true;
            
            var requestInfo = Services.Battle_DefaultSkill(DataMgr.BattleDemo.GameVideo.id, actorId, skillID);

            ReqServerWithSimulate(requestInfo, "", e =>
                {
                    DataMgr._data.LockUI = false;

                    var mc = MonsterManager.Instance.GetMonsterFromSoldierID(actorId);
                    if (mc == null) return;
                    mc.videoSoldier.defaultSkillId = skillID;
                    FireData();
                }
                , e =>
                {

                });
        }
        
        public static void ReqBattleDemo(long playerID, Action callback)
        {
            if (DataMgr.BattleDemo.LockUI)
            {
                GameUtil.SafeRun(callback);
                return;
            }

            GameUtil.GeneralReq(
                Services.Battle_Demo(playerID)
                , null
                , () =>
                {
                    DataMgr._data.LockUI = false;
                    GameUtil.SafeRun(callback);
                }
                , fail =>
                {
                    DataMgr._data.LockUI = false;
                    GameUtil.SafeRun(callback);
                }
            );
        }

        //邀请对方切磋
        public static void BattleBefore(long playerId)
        {
            GameUtil.GeneralReq(Services.Battle_BeforeDemo(playerId), e =>
            {
                TipManager.AddTip("已发送切磋邀请,等待对方同意");
            });
        }

        //拒绝切磋
        public static void BattleRefuse(long playerId)
        {
            GameUtil.GeneralReq(Services.Battle_RefuseDemo(playerId));
        }

        //接受切磋
        public static void BattleDemo(long playerId)
        {
            GameUtil.GeneralReq(Services.Battle_Demo(playerId));
        }

        public static void ReqBattleVideo(long battleId)
        {
            GameUtil.GeneralReq(Services.Battle_Video(battleId), e =>
            {
                if (e != null)
                    OnEnterBattleSuccess(e as Video);
            });
        }
    }
}