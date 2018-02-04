using System.Collections.Generic;
using System;
using AppDto;
using AppServices;
using UnityEngine;
using UniRx;
/// <summary>
/// Monster 数据管理
/// </summary>

public sealed partial class BattleDataManager
{
    public class MonsterManager
    {
        private List<MonsterController> _monsterList;
        private CompositeDisposable _diposible;

        public static void Dispose()
        {
            if (_ins == null) return;
            _ins.ResetData();
            MonsterController.selectController -= _ins.SelMonsterController;
            _ins._diposible = _ins._diposible.CloseOnceNull();
            _ins = null;
        }

        private void InitData()
        {
            _monsterList = new List<MonsterController>();
            MonsterController.selectController += SelMonsterController;
            _diposible = new CompositeDisposable();
            _diposible.Add(Stream.Subscribe(data=>UpdateDataAndMonster(data)));
        }

        private void UpdateDataAndMonster(IBattleDemoModel iData)
        {
            if (iData == null)
            {
                GameDebuger.LogError("Error:BattleDemoModel is null");
                return;
            }
            var data = DataMgr._data;

            var mc = data.CurActMonsterController;

            // Update SelEffect
            var show = (
                       data.IsCurActMonsterCanbeOpt)
                       && mc.GetCurSelectSkill() != null
                       && (
                           mc.GetCurSelectSkill().type != (int) Skill.SkillEnum.Magic
                           || !data.IsCurActMonsterDriving()
                    );
            if (show)
                ShowSelectEffect(data.CurActMonsterController);
            else
            {
                HideSelectEffect();
            }
        }

        public void RemoveMonsterController(long mID)
        {
            if (!DataMgr.IsInBattle)
                return;
            var mc = _monsterList.Find<MonsterController>(m => m.GetId() == mID);
            if (mc == null) return;
            mc.Dispose();
            RemoveMonsterController(mc);
            mc.DestroyMe();
        }

        private void SelMonsterController(MonsterController mc)
        {
            var isValid = DataMgr.ValidateChooseTarget(mc);
            if (!isValid) return;
            GameDebuger.Log("Set Target Success");

            DataMgr._data.CurActMonsterController.SetSkillTarget(mc.GetId());
            GameLog.Log_Battle("_data.choosePet.battleTargetSelector.getSelectParam()---" + DataMgr._data.CurActMonsterController.TargetSelector.getSelectParam());

            ChooseTargetPet(mc);
        }

        //选择目标宠物  
        // todo fish :仔细查一下这段逻辑 只需要选攻击对象，为什么要自己的actionstate  
        private void ChooseTargetPet(MonsterController target)
        {
            var targetSelector = DataMgr._data.CurActMonsterController.TargetSelector;
            if (targetSelector == null)
                return;

            if (targetSelector.IsCommandSkill())
            {
                GameDebuger.LogError(string.Format("[TEMP]对目标使用指令，ID：{0}，Name：{1}", targetSelector.GetSelectedSkillId(),
                    targetSelector.GetCurSkill().name));
                var tBattleOrderInfo = targetSelector.SkillParam as BattleOrderInfo;
                if (null == tBattleOrderInfo)
                    return;
                var battleId = DataMgr.GetCurrentGameVideoId();
                var tTargetId = targetSelector.GetTargetSoldierId();
                if (tBattleOrderInfo.isClearButton)
                    ServiceRequestAction.requestServer(Services.Battle_Order(battleId, tTargetId,
                        tBattleOrderInfo.orderName));
                else if (tBattleOrderInfo.isAllClearButton)
                    ServiceRequestAction.requestServer(Services.Battle_ClearOrder(battleId));
                else
                {
                    if (!string.IsNullOrEmpty(tBattleOrderInfo.orderName))
                    {
                        ServiceRequestAction.requestServer(Services.Battle_Order(battleId, tTargetId,
                            tBattleOrderInfo.orderName));
                    }
                }

                target.PlayTargetClickEffect();

//                BtnCommand_UIButtonClickHandler();
            }

            GeneralRequest requestInfo = null;
            if (targetSelector.IsItemSkill())
            {
                if (targetSelector.SkillParam == null || !(targetSelector.SkillParam is BagItemDto))
                {
                    GameLog.Log_Battle("param Error: SkillParam should be select BackItemDto");
                }
                var itemDto = targetSelector.SkillParam as BagItemDto;  
                requestInfo = Services.Battle_UseItem(
                    DataMgr._data.GameVideo.id
                    , targetSelector.GetSourceSoldierId()
                    , itemDto.index
                    , itemDto.itemId
                    , targetSelector.GetTargetSoldierId());
            }
            else
            {
                var mc = GetMonsterFromSoldierID(targetSelector.GetTargetSoldierId());

                requestInfo = Services.Battle_Attack(
                    DataMgr._data.GameVideo.id
                    , targetSelector.GetSourceSoldierId()
                    , targetSelector.GetTargetSoldierId()
                    , targetSelector.GetSelectedSkillId());
            }

            DataMgr._data.LockUI = true;
            DataMgr._data.UpdateReqActCnt(); 

            BattleNetworkManager.ReqServerWithSimulate(requestInfo, "", e =>
                {
                    DataMgr._data.LockUI = false;
                    DemoSimulateHelper.SimulateDefaultSkill(targetSelector);
                    OnRequestSkillTargetSuccessCallBack(targetSelector.GetSourceSoldierId(), target);
                    targetSelector.ClearCurSkill();
                    FireData();
                }
                , e =>
                {
                    DataMgr._data.UpdateReqActCnt(false);
                    //TipManager.AddTip(e.message);

                    //战斗已结束，直接结束游戏
                    DataMgr.CheckBattleOver(e);
                });
        }

        public void SetPlayTargetClickEffect(long sourceID, long mcID)
        {
            var mc = GetMonsterFromSoldierID(mcID);
            OnRequestSkillTargetSuccessCallBack(sourceID, mc);
        }

        private void OnRequestSkillTargetSuccessCallBack(long sourceID, MonsterController pTarget)
        {
            if (null == pTarget)
                return;

            var mc = GetMonsterFromSoldierID(sourceID);
            if (mc == null)
                mc.NeedReady = false;

            GameDebuger.TODO(@"DataMgr._data.choosePet.UpdateSkillCD();");
            pTarget.PlayTargetClickEffect();
        }

        public void AddMonsterController(MonsterController pMonsterController)
        {
            _monsterList.AddIfNotExist(pMonsterController);
        }

        public void RemoveMonsterController(MonsterController pMonsterController)
        {
            _monsterList.RemoveItem(pMonsterController);
        }

        public void ResetData()
        {
            _monsterList.ForEach<MonsterController>(mc =>
            {
                mc.Dispose();
                mc.DestroyMe();
            });
            _monsterList.Clear();
            _diposible.Clear();
            _diposible.Add(Stream.Subscribe(data=>UpdateDataAndMonster(data)));
        }

        public void UpdateMonsterNeedReady(bool pNeedReady, Func<MonsterController,bool> pConditionToUpdateNeedReady = null)
        {
            _monsterList.ForEach<MonsterController>(mc=>UpdateMonsterNeedReady(mc, pNeedReady, pConditionToUpdateNeedReady));
        }

        public void UpdateMonsterNeedReady(
            MonsterController pMonsterController
            , bool pNeedReady
            , Func<MonsterController,bool> pConditionToUpdateNeedReady = null)
        {
            if (null == pMonsterController)
                return;
            if (null == pConditionToUpdateNeedReady
                || (null != pConditionToUpdateNeedReady
                    && pConditionToUpdateNeedReady(pMonsterController)))
                pMonsterController.NeedReady = pNeedReady;
            GameDebuger.TODO(@"pMonsterController.UpdateSkillCD();");
        }

        public void UpdateMonsterNeedReadyLogic(bool pGuideBattle)
        {
            _monsterList.ForEach(mc =>
            {
                if (mc.IsPet() || mc.IsMainCharactor())
                {
                    mc.NeedReady = !pGuideBattle || mc.IsPlayerCtrlCharactor();
                }
                else
                {
                    mc.NeedReady = false;
                }
            });
        }

        public IEnumerable<MonsterController> GetMonsterList(
            BattlePosition.MonsterSide side = BattlePosition.MonsterSide.Player
            , bool includeDead = true
            , Predicate<MonsterController> p = null)
        {
            return _monsterList.Filter(mc => 
                mc != null
                && (mc.side == side)
                && (!mc.IsDead() || includeDead)
                && (p == null || p(mc)));
        }

        private void ShowSelectEffect(MonsterController actingPet)
        {
            for (int i = 0, len = _monsterList.Count; i < len; i++)
            {
                var mc = _monsterList[i];

                if (actingPet != null && actingPet.TargetSelector.CanSetTarget(actingPet, mc))
                {
                    mc.ShowSelectEffect(actingPet.TargetSelector.IsCoupleSKill());
                }
                else
                {
                    mc.HideSelectEffect();
                }
            }
        }

        private void HideSelectEffect()
        {
            _monsterList.ForEach<MonsterController>(mc=>mc.HideSelectEffect());
        }

        public UIWidget GetFirstSelectMonsterUIWidget()
        {
            return _monsterList.Find(mc => mc.IsShowSelectEffect(), mc => mc.GetSelectEffectUIWidget());
        }

        public MonsterController GetRandomSelectMonster()
        {
            var tempList = _monsterList.Filter(mc => mc.IsShowSelectEffect());
            if (tempList == null )
                return null;
            var t = tempList.ToList();
            return t[UnityEngine.Random.Range(0, t.Count)];
        }
        //获得我的英雄
        public MonsterController GetMyHero()
        {
            return _monsterList.Find<MonsterController>(mc => mc.IsPlayerMainCharactor());
        }

        public long GetMyHeroId()
        {
            var tMonsterController = GetMyHero();
            return null != tMonsterController ? tMonsterController.GetId() : 0L;
        }

        // 获得我的宠物
        public MonsterController GetMyPet()
        {
            return _monsterList.Find<MonsterController>(mc => mc != null && mc.IsPlayerPet());
        }

        public long GetMyPetId()
        {
            var tMonsterController = GetMyPet();
            return null != tMonsterController ? tMonsterController.GetId() : 0L;
        }

        public MonsterController GetPlayerPet(long playerId)
        {
            return _monsterList.Find<MonsterController>(mc =>
                mc.IsPet() && mc.GetPlayerId() == playerId || mc.GetPlayerId() == playerId);
        }

        // 主角
        public VideoSoldier GetMainCharactorSoldier()
        {
            var _mc = GetMyHero();
            return _mc == null ? null : _mc.videoSoldier;
        }

        public MonsterController GetMonsterFromSoldierID(long id)
        {
            return _monsterList.Find<MonsterController>(s=>s.GetId() == id);
        }

        public MonsterOptionStateManager GetMonsterOptionStateByUID(long id)
        {
            var tMonsterController = GetMonsterFromSoldierID(id);
            return null != tMonsterController ? tMonsterController.MonsterOptionStateManager : null;
        }

        public void UpdateBuffState()
        {
            for (int i = 0, len = _monsterList.Count; i < len; i++)
            {
                var mc = _monsterList[i];
                GameDebuger.TODO(@"if (mc && mc.side == BattlePosition.MonsterSide.Player)
                mc.battleTargetSelector = null;");

                mc.UpdateBuffState();
            }
        }

        public void ResetPetMessageState()
        {
            _monsterList.ForEach<MonsterController>(mc=>mc.ClearMessageEffect(true));
        }

        public void ShowMonsterName(bool pShow)
        {
            _monsterList.ForEach<MonsterController>(mc=>ShowMonsterName(pShow));
        }

        public void showOrder(CommandNotify order)
        {
            _monsterList.Filter(mc=>(mc.GetId() == order.targetSoldierId && !order.clearAll) || order.clearAll)
                .ForEach(mc=>mc.showOrder(order.command));
        }

        public void ShowMonsterPosition(BattlePosition.MonsterSide side, Func<BattlePosition.MonsterSide,Formation> pGetFormationFunc)
        {
            var formation = pGetFormationFunc(side);

            var monsterList = GetMonsterList(side, true);

            monsterList.ForEach(mc=>mc.ShowPosition(formation));
        }

        public void HideMonsterPosition()
        {
            _monsterList.ForEach(mc=>mc.HidePosition());
        }

        public bool IsInCD(long pPlayerUID)
        {
            var tMonsterController = GetMonsterFromSoldierID(pPlayerUID);
            return null != tMonsterController && tMonsterController.IsInCD;
        }

        public bool IsEnemy(long pPlayerUID)
        {
            var tMonsterController = GetMonsterFromSoldierID(pPlayerUID);
            return null != tMonsterController && tMonsterController.side == BattlePosition.MonsterSide.Enemy;
        }

        public void UpdateOptionState(long pMonsterUID, MonsterOptionStateManager.MonsterOptionState pMonsterOptionState)
        {
            var tMonsterOptionStateManager = GetMonsterOptionStateByUID(pMonsterUID);
            if (null != tMonsterOptionStateManager)
                tMonsterOptionStateManager.OptionState = pMonsterOptionState;
        }

        public void ResetMonsterStatus()
        {
            _monsterList.ForEach<MonsterController>(mc=>mc.ResetMonsterStatus());
        }
        #region 单例

        private static MonsterManager _ins;

        public static MonsterManager Create()
        {
            var _i = new MonsterManager();
            _i.InitData();
            return _i;
        }

        public static MonsterManager Instance
        {
            get
            {
                if (_ins == null)
                    _ins = Create();
                return _ins;
            }
        }

        #endregion

        private int monsterCnt = 0;
        private Action _loadFinish;
        private void CheckFinish(long id)
        {
            if (monsterCnt <= 0)
                return;
            monsterCnt--;
            if (monsterCnt > 0) return;
            if (_loadFinish != null)
                _loadFinish();

            _loadFinish = null;
        }

        public void ShowMonsters(Video video, Action loadFinish = null, Vector3 pos = default (Vector3))
        {
            var b = loadFinish != null;

            if (b)
            {
                _loadFinish = loadFinish;
                monsterCnt = video.ateam.teamSoldiers.Count + video.bteam.teamSoldiers.Count;
            }

            ResetData();
            CreateMonsters(video.ateam.teamSoldiers, BattlePosition.MonsterSide.Player, b, pos);
            CreateMonsters(video.bteam.teamSoldiers, BattlePosition.MonsterSide.Enemy, b, pos);
        }

        private bool CreateMonsters(
            List<VideoSoldier> strikers
            , BattlePosition.MonsterSide side
            , bool needCheckFinish = false
            , Vector3 offset = default(Vector3))
        {
            if (strikers.IsNullOrEmpty())
                return false;

            var yRotation = BattleConst.PLAYER_Y_Rotation;
            if (side == BattlePosition.MonsterSide.Enemy)
            {
                yRotation = BattleConst.ENEMY_Y_Rotation;
            }

            var mcIndex = 1;
            for (int i = 0, len = strikers.Count; i < len; i++)
            {
                var soldier = strikers[i];
                if (Instance.GetMonsterFromSoldierID(soldier.id) != null) continue;

                Action<long> cb = null;
                if (needCheckFinish)
                {
                    cb = (long id) =>
                    {
                        CheckFinish(id);
                    };
                }
                var pos = BattlePositionCalculator.GetMonsterPosition(soldier, side);
                var mc = CreateMonster(soldier, yRotation,
                    pos, side
                    , cb, offset);

                if (mc == null)
                {
                    continue;
                }

                mc.gameObject.name = mc.gameObject.name + "_" + mcIndex;

                mcIndex++;
            }

            return true;
        }

        private MonsterController CreateMonster(VideoSoldier monsterData, float yRotation, Vector3 position,
            BattlePosition.MonsterSide side, Action<long> callback = null, Vector3 offset = default (Vector3))
        {
            var go = new GameObject();
            var mc = go.AddComponent<MonsterController>();
            if (mc == null)
            {
                GameDebuger.Log("Add MonsterController component failed!!!!");
                return null;
            }

            mc.afterLoadModel = callback;
            GameObjectExt.AddPoolChild(LayerManager.Root.BattleActors, go);

            //float scale = monsterData.pet.scale / 10000.0f;

            mc.transform.localEulerAngles = new Vector3(0, yRotation, 0);
            //mc.transform.localScale = new Vector3( scale, scale, scale );
            mc.transform.localScale = Vector3.one;

            mc.transform.position = position + offset;

            GameDebuger.TODO(@"NpcAppearanceDto npcAppearanceDto = null;

        if (DataMgr._data.GameVideo is NpcMonsterVideo)
        {
            npcAppearanceDto = (DataMgr._data.GameVideo as NpcMonsterVideo).npcAppearanceDto;
            if (npcAppearanceDto != null && npcAppearanceDto.soldierId != monsterData.id)
            {
                npcAppearanceDto = null;
            }
        }");

            var showFashion = true;
            GameDebuger.TODO(@"var pvpVideo = DataMgr._data.GameVideo as PvpVideo;
        if (pvpVideo != null && pvpVideo.type == PvpVideo.PvpTypeEnum_Challenge)
        {
            showFashion = false;    
        }");
            GameDebuger.TODO(@"mc.InitMonster(monsterData, side, npcAppearanceDto, showFashion);");

            // 时序问题 保证异步回调的时候 monterlist的结果是对的 

            AddMonsterController(mc);
            mc.InitMonster(monsterData, side, showFashion, position);

            GameLog.Log_Battle(mc.GetDebugInfo());

            return mc;
        }

        public float RetreatOtherMonster(List<long> retreatSoldiers)
        {
            var count = 0;
            retreatSoldiers.Map(retreatSoldier => Instance.GetMonsterFromSoldierID(retreatSoldier))
                .Filter(mc => mc != null)
                .ForEach(mc =>
                {
                    if (mc.IsCrew() || mc.IsMonster() || (mc.IsPet() && mc.IsDead()))
                    {
                        mc.RetreatFromBattle(MonsterController.RetreatMode.Flash, 0.5f * count);
                        count++;
                    }
                    else
                    {
                        mc.RetreatFromBattle();
                    }
                });
            return 0.5f * count;
        }

        public void UpdateMonsterAttr(long mcId, VideoActionTimeReward acReward)
        {
            var mc = GetMonsterFromSoldierID(mcId);
            if (mc == null) return;

            if (acReward == null)
                return;

            mc.UpdateAttyByACReward(acReward);

            FireData();
        }

        public IEnumerable<MonsterController> GetMonsterFromPlayerID(long playerId)
        {
            return _monsterList.Filter(mc => mc.GetPlayerId() == playerId);
        }

        public float MonsterEnterScene()
        {
            var time = 0f;
            _monsterList.ForEach(mc => time = mc.EnterBattleScene());
            return time;
        }

        public void MonsterLeaveBattle(long pPlayerUID, int pPosition)
        {
            MonsterController tMonsterController = null;
            var mc = _monsterList.Find<MonsterController>(_mc =>
                _mc.IsCrew() && _mc.GetPlayerId() == pPlayerUID && _mc.videoSoldier.position == pPosition);
            
            if (mc != null)
            {
                mc.LeaveBattle();
            }
        }

        // 召唤宠物
        public void AddSolider(VideoSoldier soldier, bool tip = true)
        {
            var side = DataMgr.GetMonsterSide(soldier.playerId);
            var yRotation = BattleConst.PLAYER_Y_Rotation;
            if (side == BattlePosition.MonsterSide.Enemy)
            {
                yRotation = BattleConst.ENEMY_Y_Rotation;
            }
            
            var mc = CreateMonster(soldier, yRotation,
                BattlePositionCalculator.GetMonsterPosition(soldier, side), side);
            mc.PlaySummonEffect();
            
            mc.gameObject.name = mc.gameObject.name + "_" + soldier.id;
            
                    //触发宠物召唤喊话
//                    if (tip)
//                        BattleInstController.Instance.TriggerMonsterShount(mc.GetId(), ShoutConfig.BattleShoutTypeEnum_Summon, true);
            
                    if (soldier.playerId == ModelManager.Player.GetPlayerId())
                    {
                        if (tip)
                        {
//                            var count = BattleDataManager.CanCallBattlePetCount - _CallPetCount - 1;
//                            if (count > 0)
//                                TipManager.AddTip(string.Format("本场战斗还能召唤{0}只宠物", count));
                        }
//                        GameEventCenter.SendEvent(GameEvent.BATTLE_PET_REPLACE, mc);
            
//                        ModelManager.Pet.ChangeBattleTempPetByUID(soldier.id);
            
                        var tMonsterController = GetMyMainPet();
                        if (null != tMonsterController)
                        {
//                            UpdatePetSkillUI(tMonsterController);
            
//                            SetMonsterLastSkill(tMonsterController, 0);
                        }
                    }
        }

        /**获取主战伙伴*/
        private MonsterController GetMyMainPet()
        {
            return _monsterList.Find<MonsterController>(mc =>mc.IsPlayerPet());
        }

        public void SwitchSolider(VideoSoldier soldier)
        {
            //				if (null != soldier && soldier.playerId == ModelManager.Player.GetPlayerId())
            //					BattleDataManager.DataMgr.AddCallPetCount();
            MonsterLeaveBattle(soldier.playerId, soldier.position);
            AddSolider(soldier);                            
        }

        public void ForEachMonster(Action<MonsterController> p, bool includeDead = true){
            if (p == null) return;
            _monsterList.Filter(mc => 
                mc != null
                && (!mc.IsDead() || includeDead))
                .ForEach(mc=>p(mc));
        }
    }
}