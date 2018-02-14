// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  BattleActionPlayer.cs
// Author   : SK
// Created  : 2013/3/8
// Purpose  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using DG.Tweening;
using AppDto;
using AssetPipeline;
using UnityEngine;
using MonsterManager = BattleDataManager.MonsterManager;

public class BattleActionPlayer : MonoBehaviour
{
    private static bool HasPlayFulEff;
    private int _actionIndex;

    private List<BaseActionInfo> _actionList;
    private BaseActionInfo _actionNode;

    private bool _atOnce; //是否一次性播放
    private bool _completed;

    private bool _finishAction;
    private bool _finishTime;
    private VideoSkillAction _gameAction;

    private BaseBattleInstPlayer _gamePlayer;
    private long _injurerId;
    private List<long> _injurerIds;

    protected bool _isAttack;

    private bool _lastAction;
    private float _maxWaitTime;

    //private bool _playerSideAction = false;

    private MonsterController _mc;

    private bool _needWaitNormalAction;
    private List<long> _oriInjurerIds;

    private MonsterController _protectMonster;
    private bool _sameTarget; //是否多次打同一个人

    private SkillConfigInfo _skillConfigInfo;
    private float _startTime;

    protected VideoTargetStateGroup _stateGroup;
    private int _takeDamageActionIndex; //受击停留动作序号
    private TakeDamageEffectInfo mTakeDamageEffectInfo;
    
    private long _targetId;
    private int mTargetIndex = 0;
    private VideoInsideSkillAction _videoInsideSkillAction;

    private List<BaseEffectInfo> _waitingEffects;

    public Action<long, List<long>> retreatEvt;


    private int mCurrentFinishRound = 0;
    private bool mIsLastInjure = false;
    private GameVideoGeneralActionPlayer mGameVideoGeneralActionPlayer;

    public void SetInjureInfo(List<long> injureIds)
    {
        _oriInjurerIds = new List<long>();
        _oriInjurerIds.AddRange(injureIds.ToArray());
    }

    public void Init(BaseBattleInstPlayer instPlayer, VideoSkillAction gameAction, bool isAttack,
        SkillConfigInfo skillConfigInfo, long targetId, bool atOnce = false, int pIndex = 0)
    {
        _gamePlayer = instPlayer;

        _gameAction = gameAction;
        _isAttack = isAttack;
        _skillConfigInfo = skillConfigInfo;
        _targetId = targetId;

        _atOnce = atOnce;
        mTargetIndex = pIndex;
        if (_isAttack)
        {
            _actionList = _skillConfigInfo.attackerActions;
            _takeDamageActionIndex = GetTakeDamageActionIndex(_actionList);
        }
        else
        {
            _actionList = _skillConfigInfo.injurerActions;
        }
        if (_actionIndex == _actionList.Count)
            GameDebuger.LogError("susipicious init 102");
        //_playerSideAction = playerSideAction;

        //		Skill skill = _gameAction.skill;
        //		if (_isAttack && skill.selfNextRoundForceSkillId > 0)
        //		{
        //			_actionIndex = _actionList.Count;
        //			_completed = true;
        //			return;
        //		}
    }
    
    public void Reset()
    {
        _finishAction = false;
        _finishTime = false;
        _needWaitNormalAction = false;
        _actionIndex = 0;

        mIsLastInjure = false;
        mCurrentFinishRound = 0;
        
        if (_isAttack)
        {
            HasPlayFulEff = false;
        }

        //the same target and not atOnce play mode
        if (_isAttack && _completed && !_atOnce && _sameTarget)
        {
            _actionIndex = _takeDamageActionIndex;
        }

        if (_mc != null)
        {
            _mc.actionEnd = null;
        }

        if (_protectMonster != null)
        {
            _protectMonster.actionEnd = null;
        }

        _completed = false;
        _startTime = 0;
        _maxWaitTime = 0;
    }

    public void Play(int pCurrentFinishRound, VideoTargetStateGroup stateGroup, long injureId, bool lastAction)
    {
        //GameDebuger.LogError(string.Format("Play(pCurrentFinishRound:{0}, stateGroup:{1}, injureId:{2}, lastAction:{3}",
        //        pCurrentFinishRound, stateGroup, injureId, lastAction));
        mCurrentFinishRound = pCurrentFinishRound;   
        Play(stateGroup, injureId, lastAction);
    }

    public void Play(VideoTargetStateGroup stateGroup, long injureId, bool lastAction)
    {
        _stateGroup = stateGroup;
        _sameTarget = _injurerId == injureId;
        _injurerId = injureId;
        _lastAction = lastAction;

        Play(stateGroup);
    }

    public void Play(VideoTargetStateGroup stateGroup, List<long> injureIds, bool lastAction)
    {
        _stateGroup = stateGroup;
        _injurerIds = injureIds;
        if (_injurerIds.Count > 0)
        {
            _injurerId = injureIds[0];
        }
        _lastAction = lastAction;

        Play(stateGroup);
    }

    public void Play(VideoTargetStateGroup stateGroup, bool pIsLastInjure = false)
    {
        _stateGroup = stateGroup;
        mIsLastInjure = pIsLastInjure;
        _mc = GetMonsterFromSoldierID(_targetId);

        if (_mc == null)
        {
            GameDebuger.Log("战斗指令对应的怪物已经不存在 怪物id=" + _targetId);
            _actionIndex = _actionList.Count;
            _completed = true;
        }
        else
        {
            //修正非飞行特效的受击者目标是空的的问题，2017年08月08日09:37:21
            if (_isAttack)
            {
                var tSkillTarget = MonsterManager.Instance.GetMonsterFromSoldierID(getTargetId());
                if (null != tSkillTarget)
                    _mc.SetSkillTarget(tSkillTarget);
            }
        }

        if (_gameAction.ignoreAttackAction && _isAttack)
        {
            PlayTakeDamage(mTakeDamageEffectInfo);
            return;
        }
        Reset();
        PlayNextAction();
    }

    public void PlayNextAction()
    {
        if (_isAttack && _mc != null && _gameAction.skillId == BattleDataManager.GetRetreatSkillId())
        {
            var retreatState = GetRetreatState();
            if (retreatState != null)
            {
                TipManager.AddTip(string.Format("逃跑成功率{0}%", Mathf.RoundToInt(retreatState.rate * 100f)));
                DoRetreatAction(retreatState.success);
                CheckFinish();
            }
            else
            {
                CompleteActions();
            }
            return;
        }

        if (_isAttack && _mc != null && _gameAction.skillId == BattleDataManager.GetDefenseSkillId())
        {
            CompleteActions();
            return;
        }

        if (_isAttack && _mc != null && _gameAction.skillId == BattleDataManager.GetProtectSkillId())
        {
            CompleteActions();
            return;
        }

        LogPlayerInfo();

        if (_actionIndex < _actionList.Count)
        {
            if (_isAttack && !_atOnce && _sameTarget && _actionIndex == _takeDamageActionIndex + 1 &&
                _lastAction == false)
            {
                CompleteActions();
            }
            else
            {
                if (_isAttack && _stateGroup.strikeBackAction != null && _actionIndex == _takeDamageActionIndex + 1)
                {
                    CompleteActions();
                }
                else
                {
                    if (_mc != null)
                    {
                        if (_mc.IsDead() && _mc.leave)
                        {
                            CompleteActions();
                        }
                        else
                        {
                            var actionNode = _actionList[_actionIndex];
                            _actionIndex++;
                            PlayActionInfo(actionNode);
                        }
                    }
                    else
                    {
                        GameDebuger.LogError("战斗指令对应的怪物已经不存在 怪物id=" + _targetId);
                        CompleteActions();
                    }
                }
            }
        }
        else
        {
            CompleteActions();
        }
    }

    private int GetTakeDamageActionIndex(List<BaseActionInfo> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            BaseActionInfo actionInfo = list[i];
            if (actionInfo.effects != null)
            {
                for (int j = 0, len = actionInfo.effects.Count; j < len; j++)
                {
                    BaseEffectInfo info = actionInfo.effects[j];
                    if (info is TakeDamageEffectInfo)
                    {
                        return i;
                    }
                }
            }
        }
        return 0;
    }

    private void DoRetreatAction(bool success)
    {
        //触发逃跑喊话
        GameDebuger.TODO(@"BattleController.Instance.GetInstController().TriggerMonsterShount(_mc.GetId(), ShoutConfig.BattleShoutTypeEnum_RunAway);");
        _mc.RotationToBack();
        _mc.PlayAnimation(ModelHelper.AnimType.run);
        if (success)
        {
            Invoke("DelayRetreatActionSuccess", 1.3f);
        }
        else
        {
            Invoke("DelayRetreatActionFail", 1.3f);
        }
    }

    private void DelayRetreatActionSuccess()
    {
        //string effpath = PathHelper.GetEffectPath(GameEffectConst.Effect_Retreat);

        var addVec = Vector3.zero;
        addVec = _mc.side == BattlePosition.MonsterSide.Player ? BattleConst.PlayerRetreatPoint : BattleConst.EnemyRetreatPoint;

        if (_mc.IsMainCharactor())
        {
            var retreatState = GetRetreatState();
            if (retreatState.retreatSoldiers != null)
            {
                for (int i = 0, len = retreatState.retreatSoldiers.Count; i < len; i++)
                {
                    var mc =
                        GetMonsterFromSoldierID(retreatState.retreatSoldiers[i]);
                    if (mc == null || mc.IsDead() || !mc.IsPet()) continue;
                    //触发逃跑喊话
                    GameDebuger.TODO(@"BattleController.Instance.GetInstController().TriggerMonsterShount(_mc.GetId(), ShoutConfig.BattleShoutTypeEnum_RunAway);");
                    //OneShotSceneEffect.BeginFollowEffect(effpath, mc.transform, 0.7f, 1f);
                    mc.RotationToBack();
                    mc.Goto(mc.transform.position + addVec, ModelHelper.AnimType.run);
                }
            }

            //			MonsterController mc = BattleController.Instance.GetPlayerPet(_monster.GetPlayerId());
            //			if (mc != null && mc.IsDead() == false)
            //			{
            //				OneShotSceneEffect.BeginFollowEffect (effpath, mc.transform, 0.7f, 1f);
            //				mc.RotationToBack ();
            //				mc.Goto (mc.transform.position + addVec, 0.7f, ModelHelper.AnimType.run);
            //			}
        }

        AudioManager.Instance.PlaySound("sound_battle_escape");

        //OneShotSceneEffect.BeginFollowEffect(effpath, _mc.transform, 0.7f, 1f);
        _mc.Goto(_mc.transform.position + addVec, ModelHelper.AnimType.run);
        Invoke("DelayRetreatActionSuccess2", 0.7f);
    }

    private void DelayRetreatActionSuccess2()
    {
        _mc.RetreatFromBattle();
        CompleteActions();
    }

    private void DelayRetreatActionFail()
    {
        _mc.PlayDieAnimation();
        Invoke("DelayRetreatActionFail2", 0.5f);
    }

    private void DelayRetreatActionFail2()
    {
        _mc.ResetRotation();
        _mc.PlayAnimation(ModelHelper.AnimType.battle);
        CompleteActions();
    }

    private VideoRetreatState GetRetreatState()
    {
        var state = _stateGroup.targetStates.Find(s=> s is VideoRetreatState) ;
        return state != null ? state as VideoRetreatState : null;
    }

    private VideoSoldierSwtichState GetSwtichPetState()
    {
        for (int i = 0, len = _stateGroup.targetStates.Count; i < len; i++)
        {
            VideoTargetState state = _stateGroup.targetStates[i];
            if (state is VideoSoldierSwtichState)
            {
                return state as VideoSoldierSwtichState;
            }
        }
        return null;
    }

//    private VideoCaptureState GetCaptureState()
//    {
//        for (int i = 0, len = _stateGroup.targetStates.Count; i < len; i++)
//        {
//            VideoTargetState state = _stateGroup.targetStates[i];
//            if (state is VideoCaptureState)
//            {
//                return state as VideoCaptureState;
//            }
//        }
//        return null;
//    }

//    private List<VideoCallSoldierState> GetVideoCallSoldierState()
//    {
//        List<VideoCallSoldierState> callList = null;
//        for (int i = 0, len = _stateGroup.targetStates.Count; i < len; i++)
//        {
//            VideoTargetState state = _stateGroup.targetStates[i];
//            if (state is VideoCallSoldierState)
//            {
//                if (callList == null)
//                {
//                    callList = new List<VideoCallSoldierState>();
//                }
//                callList.Add(state as VideoCallSoldierState);
//            }
//        }
//
//        return callList;
//    }

    private void CompleteActions()
    {
        //GameDebuger.LogWithColor(GetAttackType()+"CompleteActions");

        _completed = true;

        if (_mc != null)
        {
            //GameDebuger.LogWithColor("CompleteActions" + _monster.GetDebugInfo());
            HandleMonsterAfterAction(_mc);
        }

        if (_isAttack && _mc != null && _gameAction.skillId == BattleDataManager.GetRetreatSkillId() &&
            _mc.IsMainCharactor())
        {
            var retreatState = GetRetreatState();
            if (retreatState.success)
            {
                if (retreatEvt != null)
                    retreatEvt(_mc.GetPlayerId(), retreatState.retreatSoldiers);
            }
            else
            {
                DelayCheckFinish();
            }
        }
        else if (_isAttack && _mc != null && _gameAction.skillId == BattleDataManager.GetSummonSkillId() &&
                 _mc.IsMainCharactor())
        {
            VideoSoldierSwtichState switchPetState = GetSwtichPetState();
              
            if (switchPetState != null && switchPetState.soldier != null)
            {
                MonsterManager.Instance.SwitchSolider(switchPetState.soldier);
                DelayCheckFinish(1f);
            }
            else
            {
                GameDebuger.LogError("VideoSoldierSwtichState is null");
                DelayCheckFinish();
            }
        }
        else
        {
            DelayCheckFinish();
        }
        //放在上边的if间的
        GameDebuger.TODO(@"else if (_mc != null && GetVideoCallSoldierState() != null)
        {
            bool needDelay = false;
            List<VideoCallSoldierState> callList = GetVideoCallSoldierState();
            for (int i = 0, len = callList.Count; i < len; i++)
            {
                VideoCallSoldierState callSoldierState = callList[i];
                if (callSoldierState.soldier != null)
                {
                    BattleController.Instance.CallPet(callSoldierState.soldier);
                    needDelay = true;
                }
            }
            //http://oa.cilugame.com/redmine/issues/12895
            //BattleStateHandler.HandleAllBattleState(_stateGroup.targetStates, false);

            if (needDelay)
            {
                DelayCheckFinish(1f);
            }
            else
            {
                DelayCheckFinish();
            }
        }");
    }

    private void DelayCheckFinish(float deleyTime = 0.01f)
    {
        CancelInvoke("CheckFinish");
        Invoke("CheckFinish", deleyTime);
    }

    private void CheckFinish()
    {
        _gamePlayer.CheckFinish();
    }

    private void PlayActionInfo(BaseActionInfo node)
    {
        _actionNode = node;

        _finishAction = false;
        _finishTime = false;

        _mc.actionEnd = OnActionEnd;

        _waitingEffects = new List<BaseEffectInfo>(_actionNode.effects.ToArray());

        _maxWaitTime = 0f;
        if (node is NormalActionInfo)
        {
            var normalActionInfo = node as NormalActionInfo;
            _maxWaitTime = normalActionInfo.startTime + normalActionInfo.delayTime;

            if (_isAttack == false)
            {
                _maxWaitTime += 0.4f;
            }

            if (_isAttack)
            {
                _protectMonster = GetNextProtectMonster(false);
                if (_protectMonster != null)
                {
                    var targetMonster = GetMonsterFromSoldierID(getTargetId());
                    if (targetMonster != null)
                    {
                        _protectMonster.MoveTo(targetMonster.transform, 0.05f, ModelHelper.AnimType.invalid, 0.05f, Vector3.zero);
                    }

                    AudioManager.Instance.PlaySound("sound_battle_protect");
                }
            }
        }

        for (int i = 0, len = _waitingEffects.Count; i < len; i++)
        {
            var effectNode = _waitingEffects[i];
            var playTime = effectNode.playTime;
            if (effectNode is TakeDamageEffectInfo && HasVideoDodgeTargetState(_stateGroup.targetStates))
            {
                playTime = 0.1f;
            }
            if (playTime > _maxWaitTime)
            {
                _maxWaitTime = playTime;
            }
        }

        if (_maxWaitTime == 0f)
        {
            _finishTime = true;
        }

        if (_actionNode is MoveActionInfo)
        {
            DoMoveAction((MoveActionInfo)node);
        }
        else if (_actionNode is NormalActionInfo)
        {
            _needWaitNormalAction = true;
        }
        else if (_actionNode is MoveBackActionInfo)
        {
            if (_atOnce == false && _lastAction == false)
            {
                OnActionEnd(null);
            }
            else
            {
                GameDebuger.TODO(@"VideoCaptureState captureState = GetCaptureState();
                _mc.PlayMoveBackNode((MoveBackActionInfo)node, captureState != null);");
                _mc.PlayMoveBackNode((MoveBackActionInfo)node, false);
            }
        }

        _startTime = Time.time;
        Update();
    }

    private void CaptureSuccess()
    {
        GameDebuger.TODO(@"VideoCaptureState captureState = GetCaptureState();
        if (captureState.wildToBaobao)
        {
            TipManager.AddTip(string.Format('喜从天降，这是一只{0}宝宝',
                captureState.pet.name.WrapColor(ColorConstant.Color_Tip_Item)));
        }");

        _mc.LeaveBattle();
        _finishAction = true;
        CheckPlayerFinish();
    }

    private void CaptureFail()
    {
        _mc.Goto(_mc.originPosition, ModelHelper.AnimType.run, 0, true, true);
    }

    private void DoMoveAction(MoveActionInfo node)
    {
        if (node.center == false)
        {
            MonsterController target = GetMonsterFromSoldierID(getTargetId());
            _mc.SetSkillTarget(target);
        }
        _mc.PlayMoveNode(node);
    }

    private void DoNormalAttackAction(NormalActionInfo node)
    {
        var tActionDuration = 0f;
        var tActionName = ModelHelper.AnimType.battle;
        BattleSpecialFlowManager.Instance.BattleMultiAnimaAttackerCMPT.UpdateAttackerAction(_mc, _injurerId, node, mCurrentFinishRound, _lastAction, out tActionName, out tActionDuration);

        _mc.PlayNormalActionInfo(tActionName, tActionDuration, null != node ? node.mountFollow : false);

        if (ModelHelper.IsAnimationHasDG(tActionName))
            PlayDaoGuangEffect(_mc, tActionName);
    }

    private void PlayDaoGuangEffect(MonsterController mc, ModelHelper.AnimType action)
    {
        return;// todo fish:刀光效果未提
        if (null == _mc || null == _mc.videoSoldier)
            return;
        NormalEffectInfo node = new NormalEffectInfo();
        node.fly = false;
        node.target = 0;
        node.mount = ModelHelper.Mount_shadow;
        var tDaoGuangEffectName = EffHelper.GetDaoGuangEffectName(_mc.GetModel(), action);
        PlaySpecialEffect(node, tDaoGuangEffectName, _mc, mc, 1);
    }

    private void DoNormalInjureAction(NormalActionInfo node)
    {
        if (_mc != null)
        {
            PlayInjureAction(_mc, node);
        }
    }

    private void PlayInjureState(MonsterController mc, NormalActionInfo node, VideoTargetState state)
    {
        BattleStateHandler.CheckDeadState(mc, state);

        var actionName = ModelHelper.AnimType.battle;

        bool needHit = false;

        if (state is VideoDodgeTargetState)
        {
            needHit = true;
            actionName = ModelHelper.AnimType.battle;
            mc.BodyShift(1.0f, 0.1f, 3, 0.2f);
        }
        else
        {
            var videoActionTargetStat = state as VideoActionTargetState;
            if (videoActionTargetStat != null)
            {
                //【战斗】加血，加魔时不要有受击动作
                GameDebuger.TODO(@"if (videoActionTargetStat.hp > 0 || videoActionTargetStat.mp > 0)");
                if (videoActionTargetStat.hp > 0 /**|| videoActionTargetStat.mp > 0*/)
                {
                    needHit = false;
                }
                else
                {
                    GameDebuger.TODO(@"if (videoActionTargetStat.hp == 0 && videoActionTargetStat.mp == 0)");
                    if (videoActionTargetStat.hp == 0 /**&& videoActionTargetStat.mp == 0*/)
                    {
                        needHit = false;
                    }
                    else
                    {
                        needHit = true;
                    }
                }
            }
            else
            {
                needHit = false;
            }

            if (needHit)
            {
                var skill = DataCache.getDtoByCls<Skill>(_gameAction.skillId);

                if ((VideoSoldier.SoldierStatus)videoActionTargetStat.soldierStatus == VideoSoldier.SoldierStatus.SelfDefense
                    && (Skill.SkillType)skill.skillAttackType != Skill.SkillType.Magic)
                {
                    //                  if (_monster.dead)
                    //                  {
                    //                      if (_monster.IsMonster())
                    //                      {
                    //                          actionName = ModelHelper.AnimType.def;
                    //                          mc.BodyShift( 0.2f,0.2f,3 ,-1f);
                    //                      }
                    //                      else
                    //                      {
                    //                          actionName = ModelHelper.AnimType.death;
                    //                      }
                    //                  }
                    //                  else
                    //                  {
                    //                      actionName = ModelHelper.AnimType.def;
                    //                      mc.BodyShift( 0.2f,0.2f,3 ,node.delayTime);
                    //                  }
                    actionName = ModelHelper.AnimType.hit;
                    mc.BodyShift(0.2f, 0.2f, 3, node.delayTime);
                    PlayDefenseEffect(mc);
                }
                else
                {
                    //					if (_monster.dead)
                    //					{
                    //						if (_monster.IsMonster())
                    //						{
                    //							actionName = ModelHelper.AnimType.hit;
                    //							mc.BodyShift( 0.4f,0.2f,3, -1f);
                    //						}
                    //						else
                    //						{
                    //							actionName = ModelHelper.AnimType.death;
                    //						}
                    //					}
                    //					else
                    //					{
                    //						actionName = ModelHelper.AnimType.hit;
                    //						mc.BodyShift( 0.4f,0.2f,3, node.delayTime);
                    //					}
                    actionName = ModelHelper.AnimType.hit;
                    mc.BodyShift(0.4f, 0.2f, 3, node.delayTime);
                }
            }

            //Fixed http://oa.cilugame.com/redmine/issues/11408
            _videoInsideSkillAction = null;
            _protectMonster = GetNextProtectMonster(true);
            if (_protectMonster != null)
            {
                _videoInsideSkillAction = GetNextVideoInsideSkillAction();
                _protectMonster.BodyShift(0.4f, 0.2f, 3, node.delayTime + 0.3f, true);
                _protectMonster.actionEnd = OnProtectActionEnd;
                DoPlayNormalAction(_protectMonster, ModelHelper.AnimType.hit, node.delayTime);
            }
        }
        //本该放到上边的if else中
        GameDebuger.TODO(@"else if (state is VideoCaptureState)
        {
            PlayCatchffect(mc);
        }
        else if (state is VideoAntiSkillTargetState)
        {
            PlayAntiSkillEffect(mc);
        }");

        float delayTime = node.delayTime;
        if (actionName == ModelHelper.AnimType.hit)
        {
            //这里要特殊处理，因为防御动作结束后不需要播放hit， 需要直接回到battle
            delayTime += 0.3f;
        }

        if (actionName == ModelHelper.AnimType.invalid)
        {
            delayTime = 0.05f;
        }

        DoPlayNormalAction(mc, actionName, delayTime);
    }

    private void PlayInjureAction(MonsterController mc, NormalActionInfo node)
    {
        var targetStateList = GetTargetState(_stateGroup.targetStates, mc.videoSoldier.id);
        if (targetStateList.Count == 0)
        {
            //由于某些效果需要加到放到攻击后才执行， 所以这里有可能为空，就直接结束掉， 不然会卡住
            OnActionEnd("");
        }
        else
        {
            for (int i = 0, len = targetStateList.Count; i < len; i++)
            {
                VideoTargetState state = targetStateList[i];
                PlayInjureState(mc, node, state);
            }
        }
    }

    private MonsterController GetNextProtectMonster(bool needRemove)
    {
        return GetMonsterFromSoldierID(_stateGroup.protectSoldierId);   
    }

    private VideoInsideSkillAction GetNextVideoInsideSkillAction()
    {
        return _stateGroup.protectAction;
    }

    private void PlayDefenseEffect(MonsterController mc)
    {
        var node = new NormalEffectInfo();
        node.fly = false;
        node.target = 0;
        node.mount = ModelHelper.Mount_hit;
        PlaySpecialEffect(node, GameEffectConst.GameEffectConstEnum.Effect_Defence, _mc, mc, 1);
    }

    private void PlayCatchffect(MonsterController mc)
    {
        NormalEffectInfo node = new NormalEffectInfo();
        node.fly = false;
        node.target = 0;
        node.mount = ModelHelper.Mount_shadow;
        PlaySpecialEffect(node, GameEffectConst.GameEffectConstEnum.Effect_Sing, _mc, mc, 1);

        AudioManager.Instance.PlaySound("sound_battle_capture");
    }

    private void DoPlayNormalAction(MonsterController mc, ModelHelper.AnimType action, float delayTime)
    {
        //		if (action == ModelHelper.AnimType.death || action == ModelHelper.AnimType.hit)
        //		{
        //			DoPlayCameraShake( 0.5f, 3 );
        //		}
        if (action == ModelHelper.AnimType.hit)
        {
            Model model = DataCache.getDtoByCls<Model>(mc.GetModel());
            string skillSound = "";

            if (model != null)
            {
                skillSound = model.hitSound;
            }

            if (!string.IsNullOrEmpty(skillSound))
            {
                AudioManager.Instance.PlaySound(skillSound);
            }
        }
        else if (action == ModelHelper.AnimType.hit)
        {
            AudioManager.Instance.PlaySound("sound_battle_defence");
        }

        if (action == ModelHelper.AnimType.death)
        {
            mc.PlayDieAnimation();
        }
        else
        {
            mc.PlayNormalActionInfo(action, delayTime);
        }
    }

    private List<VideoTargetState> GetTargetState(List<VideoTargetState> arr, long id)
    {
        List<VideoTargetState> targetStateList = new List<VideoTargetState>();

        for (int i = 0, len = arr.Count; i < len; i++)
        {
            VideoTargetState state = arr[i];
            if (state.id == id)
            {
                if (state is VideoBuffAddTargetState)
                {
                    //这里是因为添加buff的时机需要放到攻击后，所以过滤攻击时候的状态
                    continue;
                }
                if (state is VideoRetreatState)
                {
                    continue;
                }
                GameDebuger.TODO(@"if (state is VideoRageTargetState)
                {
                    continue;
                }
                
                //              if (state is VideoBuffRemoveTargetState)
                //              {
                //                  continue;
                //              }");
                if (state is VideoSoldierSwtichState)
                {
                    continue;
                }

                targetStateList.Add(state);
            }
        }

        return targetStateList;
    }

    private bool HasVideoDodgeTargetState(List<VideoTargetState> arr)
    {
        bool hasDodge = false;
        for (int i = 0, len = arr.Count; i < len; i++)
        {
            VideoTargetState state = arr[i];
            if (state is VideoDodgeTargetState)
            {
                hasDodge = true;
                break;
            }
        }
        return hasDodge;
    }

    private long getTargetId()
    {
        return GetNextInjureId();
    }

    /**
	 *是否最后一个节点 
	 * @return 
	 * 
	 */

    public bool IsLastActionInfo()
    {
        return _actionList.IndexOf(_actionNode) == _actionList.Count - 1;
    }

    private void OnActionEnd(string type)
    {
        _finishAction = true;

        LogPlayerInfo();

        if (!_finishTime)
        {
            HandleMonsterAfterAction(_mc);
        }
        else
        {
            if (IsLastActionInfo())
            {
                HandleMonsterAfterAction(_mc);
            }
            else
            {
                _mc.PlayIdleAnimation();
            }
            CheckPlayerFinish();
        }
    }

    private void HandleMonsterAfterAction(MonsterController monster)
    {
        if (monster.leave)
        {
            monster.RetreatFromBattle(MonsterController.RetreatMode.Fly);
        }
        else
        {
            //如果怪物死亡后需要复活， 则处理
            if (monster.lastHP > 0)
            {
                monster.currentHP = _mc.lastHP;
                monster.lastHP = 0;
                monster.dead = false;
            }

            //http://oa.cilugame.com/redmine/issues/12591
            if (monster.lastCP >= 0 && monster.IsDead())
            {
                monster.currentCp = _mc.lastCP;
            }

            if (monster.IsDead())
            {
                monster.PlayDieAnimation();
            }
            else if (monster.driving)
            {
                monster.PlayDrivingAnimation();
            }
            else
            {
                monster.PlayStateAnimation();
            }
        }
    }

    private void CheckPlayerFinish()
    {
        LogPlayerInfo();
        if (!_finishAction || !_finishTime || _completed)
            return;
        _mc.actionEnd = null;
        PlayNextAction();
    }

    private void LogPlayerInfo()
    {
        if (_isAttack) {
            GameLog.Log_Battle(GetAttackType() + " finishAction=" + _finishAction + " finishTime=" + _finishTime + " completed=" + _completed + " playActionIndex="+_actionIndex + "/" + _actionList.Count);
        }
    }

    private void OnProtectActionEnd(string type)
    {
        _protectMonster.actionEnd = null;
        if (_protectMonster.IsDead())
        {
            HandleMonsterAfterAction(_protectMonster);
        }
        else
        {
            //TODO 这里还要处理一下保护者跑回去的时机
            _protectMonster.GoBack(0.05f, Vector3.zero, ModelHelper.AnimType.run, false, 0.1f);
        }
        //_protectMonster = null;
    }

    public bool IsComplete()
    {
        //GameDebuger.LogWithColor(GetAttackType()+"IsComplete="+_completed);
        return _completed;
    }

    private string GetAttackType()
    {
        var str = "";
        str = _isAttack ? "攻击方 " : "受击方 ";

        if (_mc != null)
        {
            str += _mc.GetDebugInfo();
        }

        return str;
    }

    private void Update()
    {
        var passTime = Time.time - _startTime;

        if (_needWaitNormalAction  && passTime >= (_actionNode as NormalActionInfo).startTime)
        {
            if (_isAttack)
            {
                DoNormalAttackAction((NormalActionInfo)_actionNode);
            }
            else
            {
                DoNormalInjureAction((NormalActionInfo)_actionNode);
            }
            _needWaitNormalAction = false;
        }

        if (_waitingEffects != null)
        {
            var removeNodes = new List<BaseEffectInfo>(_waitingEffects.Count);
            for (int i = 0, len = _waitingEffects.Count; i < len; i++)
            {
                var node = _waitingEffects[i];
                var playTime = node.playTime;
                if (node is TakeDamageEffectInfo && HasVideoDodgeTargetState(_stateGroup.targetStates))
                {
                    playTime = 0.1f;
                }

                if (passTime < playTime) continue;
                PlayEffect(node);
                removeNodes.Add(node);
            }

            if (removeNodes != null)
            {
                removeNodes.ForEach(removeNode=>_waitingEffects.Remove(removeNode));

                if (_waitingEffects.Count == 0)
                {
                    _waitingEffects = null;
                }
            }
        }

        if (passTime < 20 && passTime > 5f)
        {
            _finishAction = true;
            _finishTime = true;
            _completed = false;
            if (_mc != null)
                _mc.actionEnd = null;
            if (_stateGroup != null && BattleDataManager.BattleInstController.Instance.CurRoundIdx > 0)
                PlayNextAction();
            return;
        }

        if (passTime >= _maxWaitTime && !_finishTime)/*this is buggy!*/
        {
            _finishTime = true;
            CheckPlayerFinish();
            return;
        }

        if (!_finishTime && passTime > _maxWaitTime && CheckSuspiciousState(passTime))
        {
            //GameLog.Log_Battle(GetAttackType() + " finishAction=" + _finishAction + " finishTime=" + _finishTime + " completed=" + _completed + " playActionIndex=" + _actionIndex + "/" + _actionList.Count);
            //LogBeforeNextMove(1128,true); //GameDebuger.LogError(string.Format("force next action in Suspicious State, passTime:{0},_needWaitNormalAction:{1},_waitingEffects:{2}", passTime, _needWaitNormalAction, _waitingEffects));
            //_logSusipiciousStartTimeChanges = false;
//            BattleActionPlayerPoolManager.Instance.DebugQueue.ForEachI<BattleActionPlayer>((p,i) =>
//            {
//                p.LogBeforeNextMove(1129, true);
//            });
            _finishAction = true;
            _finishTime = true;
            _completed = false;
            if (_mc != null)
                _mc.actionEnd = null;
            if (_stateGroup != null && BattleDataManager.BattleInstController.Instance.CurRoundIdx > 0)
                PlayNextAction();
        }
    }

    private bool CheckSuspiciousState(float passTime)
    {
        return !_needWaitNormalAction && _waitingEffects == null && _actionIndex == _actionList.Count && passTime > 2.5f;
    }

    private void LogBeforeNextMove(int index,bool error=false)
    {
        var curIdx = BattleDataManager.BattleInstController.Instance.CurRoundIdx;
        String debugStr = "index:"+index+ ",_maxWaitTime:" + _maxWaitTime+ ",CurRoundIdx:" + curIdx+"," + GetAttackType() + " finishAction=" + _finishAction + " finishTime=" + _finishTime + " completed=" + _completed + " playActionIndex=" + _actionIndex + "/" + _actionList.Count;
        if (error)
            GameDebuger.LogError("instance ID："+ this.GetInstanceID() + ","+ debugStr);
        else
            GameDebuger.LogWarning(debugStr);
    }

    public void PlayEffect(BaseEffectInfo node)
    {
        if (node is ShakeEffectInfo)
            PlayShakeEffect((ShakeEffectInfo)node);
        if (node is TakeDamageEffectInfo)
            PlayTakeDamage((TakeDamageEffectInfo)node);
        //		else if ( node.type == "bodyShift" )
        //			_monster.PlayBodyShift( (BodyShiftEffectInfo)(node) );
//        else if (node is NormalEffectInfo && ModelManager.SystemData.skillEffectToggle)
        else if (node is NormalEffectInfo /**&& ModelManager.SystemData.skillEffectToggle*/)
        {
            if (_isAttack == false)
            {
                bool hasDodge = HasVideoDodgeTargetState(_stateGroup.targetStates);

                if (hasDodge && (node as NormalEffectInfo).hitEff)
                {
                    return;
                }
            }

            PlayNormalEffect((NormalEffectInfo)node);
        }
        else if (node is HideEffectInfo)
            ShowHideEffect((HideEffectInfo)(node));
        //		else if ( node.type == "ghost" && SystemSetting.ShowEffect)
        //			_monster.PlayShadowMotionNode(  (GhostEffectInfo)(node));
        else if (node is ShowInjureEffectInfo)
            ShowInjureEffect((ShowInjureEffectInfo)node);
        else if (node is SoundEffectInfo)
            PlaySoundEffect((SoundEffectInfo)node);
        //		else if ( node.type == "trail" ) //暂时屏蔽掉刀光效果
        //			_monster.PlayTrailEffect( (TrailEffectInfo)(node));
        //		else if ( node.type == "camera")
        //			PlayCameraEffect( (CameraEffectInfo)(node));
        else if (node is ShowBGTextureEffectInfo)
            ShowBGTextureEffect((ShowBGTextureEffectInfo)(node));
    }

    private void PlayNormalEffect(NormalEffectInfo node)
    {
        int skillId = _gameAction.skillId;
        Skill skill = DataCache.getDtoByCls<Skill>(skillId);

        string skillName = BattleHelper.GetSkillEffectName(skill, node.name);

        if (node.name == "full")
        {
            if (HasPlayFulEff)
            {
                return;
            }

            HasPlayFulEff = true;
        }

        int clientSkillScale = 10000;

        if (_isAttack)
        {
            if (node.fly && node.flyTarget == 0)
            {
                if (_atOnce == false)
                {
                    long id = GetNextInjureId();
                    MonsterController mc = GetMonsterFromSoldierID(id);
                    PlaySpecialEffect(node, skillName, _mc, mc, clientSkillScale);
                }
                else
                {
                    //特效目标是敌方身上并且是飞行特效,同时飞多个特效到目标上
                    List<long> ids = GetInjurerIds();
                    for (int i = 0, len = ids.Count; i < len; i++)
                    {
                        long id = ids[i];
                        MonsterController mc = GetMonsterFromSoldierID(id);
                        if (mc != null)
                        {
                            PlaySpecialEffect(node, skillName, _mc, mc, clientSkillScale);
                        }
                    }
                }
            }
            else
            {
                PlaySpecialEffect(node, skillName, _mc, _mc.GetSkillTarget(), clientSkillScale);
            }
        }
        else
        {
            if (node.target == 0)
            {
                if (_mc != null)
                {
                    PlaySpecialEffect(node, skillName, _mc, _mc.GetSkillTarget(), clientSkillScale);
                }
            }
            else
            {
                PlaySpecialEffect(node, skillName, _mc, _mc.GetSkillTarget(), clientSkillScale);
            }
        }
    }

    public void PlaySpecialEffect(
        NormalEffectInfo node
        , GameEffectConst.GameEffectConstEnum eff
        , MonsterController monster
        , MonsterController target
        , int clientSkillScale)
    {
        var effname = PathHelper.GetEffectPath(eff);
        PlaySpecialEffect(node, effname, monster, target, clientSkillScale);
    }
    
    public void PlaySpecialEffect(
        NormalEffectInfo node
        , string skillName,
        MonsterController monster
        , MonsterController target
        , int clientSkillScale
        , Action<float> pGetPlayTimeCallBack = null)
    {
        //	  public float delayTime;
        //    public int target;//特效目标  0默认， 1，场景中心 2，我方中心   3， 敌军中心
        //    public bool loop;//是否循环
        //    public int loopCount;//循环次数
        //    public int scale;//缩放 单位100

        Action<float> tCallBack = (tTimeCost) =>
        {
            if (null != pGetPlayTimeCallBack)
                pGetPlayTimeCallBack(tTimeCost);
        };

        if (string.IsNullOrEmpty(skillName))
        {
            GameDebuger.LogError("PlaySpecialEffect failed , skillName IsNullOrEmpty ");
            tCallBack(0F);
            return;
        }
        
        if (monster == null)
        {
            return;
        }

        if (clientSkillScale == 0)
        {
            clientSkillScale = 10000;
        }

        int targetType = node.target;
        string mountName = node.mount;

        var tSkillNames = skillName.Split(',').ToList();
        if (tSkillNames.IsNullOrEmpty())
        {
            tCallBack(0F);
            return;
        }

        var tEffectRoot = new GameObject(skillName);
        SpawnAllEffectsAsync(
            tSkillNames
            , monster
            , tEffectRoot
            , node
            , () =>
        {
            if (tEffectRoot.transform.childCount <= 0)
            {
                GameDebuger.LogError(string.Format("[Error]特效({0})播放失败，没有该资源" ,skillName));
                NGUITools.Destroy(tEffectRoot);
                tCallBack(0F);
                return;
            }

            //位移
            Vector3 offVec = new Vector3(node.offX, node.offY, node.offZ);
            /**加一段偏移位移的世界坐标版*/
            Vector3 effectStartPosition = tEffectRoot.transform.TransformVector(offVec);

            switch (targetType)
            {
                case 0: //默认
                    Transform mountTransform = null;
                    if (string.IsNullOrEmpty(mountName) == false)
                    {
                        mountTransform = monster.transform.GetChildTransform(mountName);
                    }
                    if (mountTransform == null)
                    {
                        mountTransform = monster.gameObject.transform;
                    }
                    if (mountName == ModelHelper.Mount_shadow)
                    {
                        effectStartPosition += new Vector3(mountTransform.position.x, mountTransform.position.y, mountTransform.position.z);
                    }
                    else
                    {
                        effectStartPosition += mountTransform.position;
                    }
                    break;
                case 1: //场景中心
                    effectStartPosition += Vector3.zero;
                    break;
                case 2: //我方中心
                    effectStartPosition += BattlePositionCalculator.GetZonePosition(monster.side);
                    break;
                case 3: //敌方中心
                    effectStartPosition =
                        BattlePositionCalculator.GetZonePosition(monster.side ==
                                                                 BattlePosition.MonsterSide.Player
                            ? BattlePosition.MonsterSide.Enemy
                            : BattlePosition.MonsterSide.Player);
                    break;
            }

            //特效时间
            var effectTime = monster.CreateEffectTime(
                tEffectRoot
                , monster.GetMountShadow().gameObject
                , (effectGameObject) =>
                {
                    LayerManager.Instance.ResetBattleCameraParent();
                });

            var trans = tEffectRoot.transform;
            trans.position = effectStartPosition;
            //http://oa.cilugame.com/redmine/issues/18885 D1的3D场景战斗需要特效跟随人物移动和转动，2017年07月17日11:25:47
//                trans.rotation = Quaternion.identity;
            trans.localRotation = Quaternion.identity;


            if (_isAttack)
            {
                BattleSpecialFlowManager.Instance.ChainEffectTrans = tEffectRoot;
            }

            if (node.delayTime > 0)
            {
                effectTime.time = node.delayTime;
            }
            if (node.loop)
            {
                effectTime.loopCount = node.loopCount;
            }

            float scaleValue = node.scale / 100f;

            scaleValue *= clientSkillScale / 10000f;

            //Set Effect Scale
            ParticleScaler scaler = effectTime.GetComponent<ParticleScaler>();
            if (scaler == null)
            {
                scaler = effectTime.gameObject.AddComponent<ParticleScaler>();
            }
            scaler.SetScale(scaleValue);

            //旋转
            /**int rotOffY = 0;
                if (node.fixRotation != true)
                {
                    //如果是我方受击的时候， 旋转特效180度
                    if (_isAttack == false && monster.side == MonsterController.MonsterSide.Player)
                    {
                        rotOffY = 180;
                    }

                    if (_isAttack && monster.side == MonsterController.MonsterSide.Enemy)
                    {
                        rotOffY = 180;
                    }
                }*/

            //http://oa.cilugame.com/redmine/issues/18885 D1的3D场景战斗需要特效跟随人物移动和转动，2017年07月17日11:25:47
//                trans.eulerAngles = new Vector3(node.rotX, node.rotY + rotOffY, node.rotZ);

            if (node.faceToPrevious)
            {
                var tMonsterController = GetPreviousMonsterController(mTargetIndex, true);
                if (null != tMonsterController && null != tMonsterController.transform && mTargetIndex >= 0)
                {
                    //朝向下一个，且要避免受击者在一条线时朝向老一样，2017-05-25 15:04:05
                    tEffectRoot.transform.LookAt(tEffectRoot.transform.position + (monster.transform.position - tMonsterController.transform.position));
                    Vector3 tEulerAngles = tEffectRoot.transform.eulerAngles;
                    float tRandomAngle = UnityEngine.Random.Range(10f, 30f);
                    tRandomAngle = mTargetIndex % 2 == 0 ? tRandomAngle : -tRandomAngle;
                    tEulerAngles = new Vector3(tEulerAngles.x, tEulerAngles.y + tRandomAngle, tEulerAngles.z);
                    tEffectRoot.transform.localRotation = Quaternion.Euler(tEulerAngles);
                }
            }
            if (target != null /** && node.faceToTarget**/) //朝向目标
            {
                tEffectRoot.transform.LookAt(target.transform.position);
            }

            //跟随
            if (node.follow)
            {
                trans.parent = monster.gameObject.transform;
            }
            //飞行
            else if (node.fly)
            {
                Vector3 targetPoint = Vector3.zero;
                switch (node.flyTarget)
                {
                    case 0: //默认
                        Transform flyTargetTransform = null;
                        if (target == null)
                        {
                            targetPoint = Vector3.zero;
                            break;
                        }
                        if (string.IsNullOrEmpty(node.mount) == false)
                        {
                            flyTargetTransform = target.transform.GetChildTransform(node.mount);
                        }
                        if (flyTargetTransform == null)
                        {
                            flyTargetTransform = target.gameObject.transform;
                        }
                        if (mountName == ModelHelper.Mount_shadow)
                        {
                            targetPoint = new Vector3(flyTargetTransform.position.x, flyTargetTransform.position.y, flyTargetTransform.position.z);
                        }
                        else
                        {
                            targetPoint = flyTargetTransform.position;
                        }
                        break;
                    case 1: //场景中心
                        targetPoint = Vector3.zero;
                        break;
                    case 2: //我方中心
                        //effectStartPosition = BattlePositionCalculator.GetZonePosition(monster.side);
                        break;
                    case 3: //敌方中心
                        //effectStartPosition = BattlePositionCalculator.GetZonePosition(monster.side == MonsterController.MonsterSide.Player ? MonsterController.MonsterSide.Enemy : MonsterController.MonsterSide.Player);
                        break;
                }

                // 增加飞行停顿
//                    if (node.fly && BattleSpecialFlowManager.Instance.ChainAttack)//chain 闪电链的特效自行管理生命周期，因其需要在受击时重复利用资源
//                        effectTime.time = float.MaxValue;
//                    else
                effectTime.time = node.delayTime + node.flyTime;

                //飞行位移
                Vector3 flyOffVec = new Vector3(node.flyOffX, node.flyOffY, node.flyOffZ);
                targetPoint = targetPoint + flyOffVec;

                float flyTime = node.delayTime;
                if (flyTime == 0)
                {
                    flyTime = 1f;
                }

                float tDistance = Vector3.Distance(effectTime.transform.position, targetPoint);
                if (_isAttack)//飞行特效不按顺序受击约数，直接根据攻击者受击者间距按等速执行。
                {
//                        BattleSpecialFlowManager.Instance.hitInOrderParam = tDistance / flyTime;
                    //                GameDebuger.LogError(string.Format("tDistance:{0} , flyTime:{1}, mEffectMoveSpeed:{2} ", tDistance, flyTime, mBattleController.EffectMoveSpeed));
                }

                if (node.fly)
                    effectTime.transform.LookAt(targetPoint);
                effectTime.transform.DOMove(targetPoint, flyTime);

                //取消飞行特效的旋转处理
                // effectTime.transform.DOLookAt(targetPoint,1f);

            }

            if (node.IsEffectHasCamera && BattleDataManager.NeedBattleMap)
            {
                Transform tCameraParent = effectTime.transform.GetChildTransform("WarCameraRotate");
                if (tCameraParent == null) {
                    UnityEngine.Assertions.Assert.IsNotNull(tCameraParent, "[Error]播放镜头特写失败，特效上没有指定名字的摄像机挂点(WarCameraRotate)");    
                }
                else
                    LayerManager.Instance.UpdateBattleCameraParent(tCameraParent);
            }

            tCallBack(effectTime.time);

            return;
        });

    }

    private static void SpawnAllEffectsAsync(
        List<string> tSkillNames
        , MonsterController monster
        , GameObject pParent
        , NormalEffectInfo node
        , Action pAllEffectSpawnedFinishCallBack)
    {
        if (null == tSkillNames || tSkillNames.Count <= 0)
        {
            GameDebuger.LogError("SpawnAllEffectsAsync failed ,tSkillNames' count is invalid !");
            if (null != pAllEffectSpawnedFinishCallBack)
                pAllEffectSpawnedFinishCallBack();
            return;
        }

        int tLeftCounterToSpawn = tSkillNames.Count;
        Action<string,int> tAction = (tSkillName, tIndexCounter) =>
        {
            ResourcePoolManager.Instance.SpawnEffectAsync(tSkillName, (tChainEffectTrans) =>
                {
                    if (null == tChainEffectTrans)
                    {
                        GameDebuger.LogWarning(string.Format("特效资源加载失败，名字：{0}", tSkillName));
                        if (null != pAllEffectSpawnedFinishCallBack)
                            pAllEffectSpawnedFinishCallBack();
                        return;
                    }

                    if (monster == null)
                    {
                        ResourcePoolManager.Instance.DespawnEffect(tChainEffectTrans.gameObject);
                        if (null != pAllEffectSpawnedFinishCallBack)
                            pAllEffectSpawnedFinishCallBack();
                        return;
                    }

                    GameObjectExt.AddPoolChild(pParent, tChainEffectTrans);
                    //            if(mBattleController.IsAttackTypeChainFly)
                    //legacy，这是上头没挂点时的做法，现在原则上会在挂点上处理朝向问题。见H5相关SVN日志，2017年08月03日18:13:27
//                    if (node.fly || node.faceToPrevious)//飞行特效都加朝向，因为闪电链需朝向。2017-01-06 14:47:20
//                            tChainEffectTrans.transform.localRotation = Quaternion.Euler(new Vector3(0, -90f, 0));  

                    tLeftCounterToSpawn--;

                    if (tLeftCounterToSpawn <= 0)
                    {
                        if (null != pAllEffectSpawnedFinishCallBack)
                            pAllEffectSpawnedFinishCallBack();
                        return;
                    }
                            
                }, () =>
                {
                    GameDebuger.LogWarning(string.Format("特效资源加载失败，名字：{0}", tSkillName));
                    if (null != pAllEffectSpawnedFinishCallBack)
                        pAllEffectSpawnedFinishCallBack();
                });
        };
        for (int tCounter = 0; tCounter < tSkillNames.Count; tCounter++)
        {
            tAction(tSkillNames[tCounter], tCounter);
        }
    }

    private void ShowHideEffect(HideEffectInfo node)
    {
        _mc.PlayHideEffect(node.playTime);
    }

    protected virtual void ShowInjureEffect(ShowInjureEffectInfo node)
    {
        if (_isAttack == false)
        {
            if (_mc != null)
            {
                BattleStateHandler.HandleBattleState(_mc.GetId(), _stateGroup.targetStates, /**BattleController.Instance*/BattleDataManager.DataMgr.IsInBattle);
            }

            if (_protectMonster != null && _videoInsideSkillAction != null)
            {
                BattleStateHandler.HandleBattleState(_protectMonster.GetId(),
                    _videoInsideSkillAction.targetStateGroups[0].targetStates, /**BattleController.Instance*/BattleDataManager.DataMgr.IsInBattle);
            }

            GameDebuger.TODO(@"VideoCaptureState captureState = GetCaptureState();
            if (captureState != null)
            {
                float moveBackTime = 3.4f;

                MonsterController attackMonster = GetMonsterFromSoldierID(getAttackerId());
                moveBackTime = _mc.Goto(attackMonster.originPosition, moveBackTime + 0.2f, ModelHelper.AnimType.run, 0.05f,
                    false, true);

                if (captureState.success)
                {
                    Invoke('CaptureSuccess', moveBackTime);
                }
                else
                {
                    float ranMoveTime = Random.Range(moveBackTime * 0.5f, moveBackTime * 0.8f);
                    Invoke('CaptureFail', ranMoveTime);
                }
            }");
        }
    }

    private void PlaySoundEffect(SoundEffectInfo info)
    {
        var soundName = info.name;
        AudioManager.Instance.PlaySound(soundName);
    }

    private void PlayShakeEffect(ShakeEffectInfo node)
    {
        if (_isAttack)
        {
            if (node.isHit)
            {
                var hasIt = false;
                for (int i = 0; i < _gameAction.targetStateGroups.Count; i++)
                {
                    if (_gameAction.targetStateGroups[i] == null ||
                        _gameAction.targetStateGroups[i].targetStates == null) continue;
                    for (int j = 0; j < _gameAction.targetStateGroups[i].targetStates.Count; j++)
                    {
                        if (!(_gameAction.targetStateGroups[i].targetStates[j] is VideoActionTargetState)) continue;
                        hasIt = true;
                        break;
                    }
                }

                if (!hasIt)
                    return;
            }

            if (node.PlayIndex >= 0 && mCurrentFinishRound != node.PlayIndex)
                return;
            
            LayerManager.Instance.BattleShakeEffectHelper.Launch(node.delayTime, node.intensity);
        }
        else
        {
            //D1的闪避从程序实现改成直接用美术的动作，所以这里取消，2017年06月29日18:10:31
            /**bool hasDodge = HasVideoDodgeTargetState(_stateGroup.targetStates, _targetId);*/
            if (/**!hasDodge &&*/ node.isHit)
            {
                return;
            }

            if (node.PlayIndex >= 0 && mTargetIndex != node.PlayIndex)
                return;

            LayerManager.Instance.BattleShakeEffectHelper.Launch(node.delayTime, node.intensity);
        }
    }

    // todo fish ：从这里直接调数据层 不对啊
    private void ShowBGTextureEffect(ShowBGTextureEffectInfo pHideEffectInfo)
    {
        if (BattleDataManager.NeedBattleMap)
            return;
        BattleHelper.ShowBGTexture(pHideEffectInfo.playTime, pHideEffectInfo.delayTime, pHideEffectInfo.bgTexture);
    }

    private void PlayTakeDamage(TakeDamageEffectInfo node)
    {
        List<long> ids = null;

        if (_atOnce)
        {
            ids = GetInjurerIds();
        }
        else
        {
            ids = new List<long>();
            ids.Add(getTargetId());
        }

        _gamePlayer.PlayInjureAction(ids, node.randomTime);
    }

    public long getAttackerId()
    {
        return _gameAction.actionSoldierId;
    }

    public List<long> GetInjurerIds()
    {
        return _injurerIds;
    }

    public List<long> GetOriInjurerIds()
    {
        return _oriInjurerIds;
    }

    public long GetNextInjureId()
    {
        return _injurerId;
    }

    public void SetSkillTarget(MonsterController target)
    {
        if (_mc == null)
            _mc = MonsterManager.Instance.GetMonsterFromSoldierID(_targetId);
        if (_mc != null)
            _mc.SetSkillTarget(target);
    }

    public void DelayPlay()
    {
        Invoke("Play", 0.1f);
    }

    public void Continue()
    {
        LogBeforeNextMove(1754);
        _finishAction = false;
        _finishTime = false;
        _needWaitNormalAction = false;

        if (_mc != null)
        {
            _mc.actionEnd = null;
        }

        if (_protectMonster != null)
        {
            _protectMonster.actionEnd = null;
        }

        _completed = false;
        _startTime = 0;
        _maxWaitTime = 0;

        PlayNextAction();
    }

    public void Destroy()
    {
        CancelInvoke("DelayRetreatActionSuccess");
        CancelInvoke("DelayRetreatActionSuccess2");
        CancelInvoke("DelayRetreatActionFail");
        CancelInvoke("DelayRetreatActionFail2");

        CancelInvoke("CaptureSuccess");
        CancelInvoke("CaptureFail");

        CancelInvoke("Play");

        CancelInvoke("CheckFinish");

        _finishAction = false;
        _finishTime = false;
        _needWaitNormalAction = false;
        _actionIndex = 0;

        _completed = false;
        _startTime = 0;
        _maxWaitTime = 0;

        if (_mc != null)
        {
            _mc.actionEnd = null;
            _mc = null;
        }

        if (_protectMonster != null)
        {
            _protectMonster.actionEnd = null;
            _protectMonster = null;
        }
        _videoInsideSkillAction = null;
        _waitingEffects = null;
    }

    protected virtual MonsterController GetMonsterFromSoldierID(long pGUID)
    {
        return MonsterManager.Instance.GetMonsterFromSoldierID(pGUID);
    }

    private MonsterController GetPreviousMonsterController(int pIndex, bool pIgnoreAtOnce = false)
    {
        if (null == _gamePlayer)
            return null;
        if (null == mGameVideoGeneralActionPlayer)
            return null;
        var tNextMonsterController = mGameVideoGeneralActionPlayer.GetPreviousMonsterController(pIndex, pIgnoreAtOnce);
        return tNextMonsterController;
    }

    public long GetTargetId()
    {
        return _targetId;
    }

    public static void Play_SCraft_Effect(MonsterController mc)
    {
        PlayStaticSpecialEffect(mc, "buff_eff_9999");
    }

    private static void PlayStaticSpecialEffect(
        MonsterController mc
        , GameEffectConst.GameEffectConstEnum eff
        , string mount = ModelHelper.Mount_shadow)
    {
        var effname = PathHelper.GetEffectPath(eff);
        PlayStaticSpecialEffect(mc, effname, mount);
    }
    
    public static void PlayStaticSpecialEffect(
        MonsterController mc
        , string effname
        , string mount = ModelHelper.Mount_shadow)
    {
        var node = new NormalEffectInfo();
        node.fly = false;
        node.target = 0;
        node.mount = mount;
        node.loop = true;

        PlayStaticSpecialEffect(node, effname, mc, mc, 1);

//        AudioManager.Instance.PlaySound("sound_battle_capture");
    }
    
    public static void PlayDrivingEffect(MonsterController mc)
    {
        PlayStaticSpecialEffect(mc, GameEffectConst.GameEffectConstEnum.Effect_Sing);
    }

    // fish todo：跨几个行动点的特效 不能用实例播放 需要调整
    public static void PlayStaticSpecialEffect(NormalEffectInfo node, string effname,
        MonsterController monster, MonsterController target, int clientSkillScale)
    {
        //	  public float delayTime;
        //    public int target;//特效目标  0默认， 1，场景中心 2，我方中心   3， 敌军中心
        //    public bool loop;//是否循环
        //    public int loopCount;//循环次数
        //    public int scale;//缩放 单位100

        if (monster == null)
        {
            return;
        }

        if (clientSkillScale == 0)
        {
            clientSkillScale = 10000;
        }

        var effPath = effname.ToLower();
        var go = target.GetEffGameObject(effname);
        if (go != null)
        {
            LoadEffGoFinish(node, go, monster, target, clientSkillScale);
        }
        else
        {
            ResourcePoolManager.Instance.SpawnEffectAsync(
                effPath,
                (effectGO) =>
                {
                    LoadEffGoFinish(node, effectGO, monster, target, clientSkillScale);
                });
        }
    }

    private static void LoadEffGoFinish(NormalEffectInfo node, GameObject effectGO,
        MonsterController monster, MonsterController target, int clientSkillScale)
    {
        if (effectGO == null) return;
        Vector3 effectStartPosition = new Vector3();
        var mountName = node.mount;

        switch (node.target)
        {
           case 0: //默认
               Transform mountTransform = null;
               if (!string.IsNullOrEmpty(mountName))
               {
                   mountTransform = monster.transform.GetChildTransform(mountName);
               }
               if (mountTransform == null)
               {
                   mountTransform = monster.gameObject.transform;
               }
               if (mountName == ModelHelper.Mount_shadow)
               {
                   effectStartPosition = new Vector3(mountTransform.position.x, 0, mountTransform.position.z);
               }
               else
               {
                   effectStartPosition = mountTransform.position;
               }
               break;
            case 1: //场景中心
                effectStartPosition = Vector3.zero;
                break;
            case 2: //我方中心
                effectStartPosition = BattlePositionCalculator.GetZonePosition(monster.side);
                break;
            case 3: //敌方中心
                effectStartPosition =
                    BattlePositionCalculator.GetZonePosition(monster.side ==
                                                             BattlePosition.MonsterSide.Player
                        ? BattlePosition.MonsterSide.Enemy
                        : BattlePosition.MonsterSide.Player);
                break;
        }
//位移
        var offVec = new Vector3(node.offX, node.offY, node.offZ);
        effectStartPosition = effectStartPosition + offVec;

        var trans = effectGO.transform;
        trans.position = effectStartPosition;
        trans.localRotation  = Quaternion.identity;
        //trans.rotation = Quaternion.identity;                         
        //特效时间
        var effectTime = target.CreateEffectTime(
            effectGO
            , monster.GetBattleGroundMount().gameObject
            , createWhenNotExist:true);

        effectTime.time = 5;
        effectTime.OnFinish = delegate(EffectTime time)
        {
            OnEffectTimeFinish(time, monster.GetId());
        };
        if (node.delayTime > 0)
        {
            effectTime.time = node.delayTime;
        }
        if (node.loop)
        {
            effectTime.loopCount = node.loopCount;
        }
        var scaleValue = node.scale / 100f;
        scaleValue *= clientSkillScale / 10000f;

        //Set Effect Scale
        var scaler = effectGO.GetComponent<ParticleScaler>() ?? effectGO.AddComponent<ParticleScaler>();

        scaler.SetScale(scaleValue);

        //旋转
        var rotOffY = 0;
        if (node.fixRotation != true)
        {
            //如果是我方受击的时候， 旋转特效180度
            //                    if (_isAttack == false && monster.side == BattlePosition.MonsterSide.Player)
            //                    {
            //                        rotOffY = 180;
            //                    }
            //
            //                    if (_isAttack && monster.side == BattlePosition.MonsterSide.Enemy)
            //                    {
            //                        rotOffY = 180;
            //                    }
        }

        trans.localEulerAngles = new Vector3(node.rotX, node.rotY + rotOffY, node.rotZ);

        //跟随

        if (node.follow)
        {
            trans.parent = monster.gameObject.transform;
        }
//飞行
        else if (node.fly)
        {
            var targetPoint = Vector3.zero;
            switch (node.flyTarget)
            {
                case 0: //默认
                    Transform flyTargetTransform = null;
                    if (target == null)
                    {
                        targetPoint = Vector3.zero;
                        break;
                    }
                    if (string.IsNullOrEmpty(node.mount) == false)
                    {
                        flyTargetTransform = target.transform.GetChildTransform(node.mount);
                    }
                    if (flyTargetTransform == null)
                    {
                        flyTargetTransform = target.gameObject.transform;
                    }
                    if (mountName == ModelHelper.Mount_shadow)
                    {
                        targetPoint = new Vector3(flyTargetTransform.position.x, 0,
                            flyTargetTransform.position.z);
                    }
                    else
                    {
                        targetPoint = flyTargetTransform.position;
                    }
                    break;
                case 1: //场景中心
                    targetPoint = Vector3.zero;
                    break;
                case 2: //我方中心
                    //effectStartPosition = BattlePositionCalculator.GetZonePosition(monster.side);
                    break;
                case 3: //敌方中心
                    //effectStartPosition = BattlePositionCalculator.GetZonePosition(monster.side == BattlePosition.MonsterSide.Player ? BattlePosition.MonsterSide.Enemy : BattlePosition.MonsterSide.Player);
                    break;
            }

            //飞行位移
            var flyOffVec = new Vector3(node.flyOffX, node.flyOffY, node.flyOffZ);
            targetPoint = targetPoint + flyOffVec;

            var flyTime = node.delayTime;
            if (flyTime == 0)
            {
                flyTime = 1f;
            }

            var effectTrans = effectGO.transform;
            effectTrans.DOMove(targetPoint, flyTime);

            //取消飞行特效的旋转处理
            //effectTrans.DOLookAt(targetPoint,1f);

        }
    }

    private static void OnEffectTimeFinish(EffectTime effectTime, long soliderID)
    {
        if (effectTime == null)
            return;
        ResourcePoolManager.Instance.DespawnEffect(effectTime.gameObject);
    }
}