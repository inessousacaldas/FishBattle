// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  GameVideoGeneralActionPlayer.cs
// Author   : SK
// Created  : 2013/3/8
// Purpose  : 
// **********************************************************************

//using AppDto;
using AppDto;
using System.Collections.Generic;
using System;
using UniRx;
using UnityEngine;
using MonsterManager = BattleDataManager.MonsterManager;

public class GameVideoGeneralActionPlayer : BaseBattleInstPlayer
{
    public const float DEFAULT_EFFECT_MOVE_SPEED = 14f;
    
    private VideoSkillAction _gameAction;
    private SkillConfigInfo _skillConfig;
	
    private BattleActionPlayer _attackActionPlayer;
    private Dictionary<int, BattleActionPlayer> _injureActionPlayerDic;

    // Round 是主角行动多少次攻击行为
    private int _totalWaitingRound = 0;
    private int _currentFinishRound = 0;

    // Count 是主角每次行动， 有多少个行动对象(自己and敌人), Count 会包括自己的行动,所以+1
    private int _totalWaitingCount = 0;
    private int _currentFinishCount = 0;

    private List<long> _injureIds;

    /**key：受击者索引，value：对应的VideoTargetStateGroup*/
    private Dictionary<int,VideoTargetStateGroup> mInjureIndexGroupDic = null;
    
    private Skill _skill;

    public event Action OnActionFinish;

    private GameVideoGeneralActionPlayer _strikeBackActionPlayer;

    private bool mDamageOrHealInfoShowed = false;
    //这个标志设置的原因，是考虑到finish同一次行动中可能多次调用finish，如果测试一段时间后发现并未出现BUG，相关引用可删除。2016-07-15 17:13:43
    private int mTotalDamageOrHeal = 0;
    private BattlePosition.MonsterSide mActionSide = BattlePosition.MonsterSide.None;

    private float _totalPlayTime = 0f;

    public override void DoExcute(VideoAction inst)
    {
        _gameAction = inst as VideoSkillAction;

        _startCheckTime = DateTime.Now.Ticks;
        _totalPlayTime = SkillPlayTimeHelper.GetSkillVideoActionPlayTime(_gameAction);

        mDamageOrHealInfoShowed = false;
        UpdateActionSide();

        //如果作为反击的播放， 则不需要处理汇总血量
        if (OnActionFinish == null)
        {
            mTotalDamageOrHeal = CacheActionTotalDamageOrHeal();
        }

        DoSkillBeforeActions();
        DoSkillAction();
    }

    private void DoSkillBeforeActions()
    {
        GameDebuger.TODO(@"if (_gameAction.beforeActions != null) {
            for (int i = 0, len = _gameAction.beforeActions.Count; i < len; i++) {
                VideoAction action = _gameAction.beforeActions [i];
                BattleInfoOutput.ShowVideoAction (action);
                BattleStateHandler.HandleBattleStateGroup (0, action.targetStateGroups);
            }
        }");
    }

    private void DoSkillDoingActions()
    {
        BattleInfoOutput.ShowVideoAction(_gameAction);
        BattleStateHandler.HandleBattleStateGroup(0, _gameAction.targetStateGroups);
    }

    private void DoSkillEndActions()
    {
        GameDebuger.TODO(@"if (_gameAction.afterActions != null) {
            for (int i = 0, len = _gameAction.afterActions.Count; i < len; i++) {
                VideoAction action = _gameAction.afterActions [i];
                BattleInfoOutput.ShowVideoAction (action);
                BattleStateHandler.HandleBattleStateGroup (0, action.targetStateGroups);
            }
        }");
    }

    private void DoSkillAction()
    {
        var _gameAction = this._gameAction;	//http://oa.cilugame.com/redmine/issues/11354
        //防止递归后成员变量被修改
        if (BattleDataManager.DEBUG)
        {
            BattleInfoOutput.OutputTargetStateGroups(_gameAction.targetStateGroups);
            BattleInfoOutput.showVideoSkillAction(_gameAction);	
        }		

        //空的施放者
        if (_gameAction.actionSoldierId == 0)
        {
            GameDebuger.LogError("DoSkillAction attacker is null");
            BattleStateHandler.HandleBattleStateGroup(0, _gameAction.targetStateGroups);
            Finish();
            return;
        }

        var attacker = MonsterManager.Instance.GetMonsterFromSoldierID(_gameAction.actionSoldierId);

        if (attacker == null)
        {
            GameDebuger.LogError("attacker is null id = " + _gameAction.actionSoldierId);
            Finish();
            return;
        }
        attacker.driving = _gameAction.driving;
        attacker.CheckActionShout();

        if (_gameAction.skillId == BattleDataManager.GetDefenseSkillId())
        {
            DoSkillDoingActions();
            Finish();
            return;
        }

        if (_gameAction.skillId == BattleDataManager.GetProtectSkillId())
        {
            DoSkillDoingActions();
            Finish();
            return;
        }

        GameDebuger.TODO(@"if (_gameAction.skillStatusCode != Skill.SkillActionStatus_Ordinary) {
            DoSkillDoingActions ();
            GameDebuger.Log (attacker.GetDebugInfo () + ' ' + GetSkillStatusDesc (_gameAction.skillStatusCode));
            if (attacker.IsPlayerCtrlCharactor ()) {
                TipManager.AddTip (GetSkillStatusDesc (_gameAction.skillStatusCode));
                Finish ();
            } else {
                Finish ();
            }

            BattleStateHandler.PlayVideoSkillAction (attacker, _gameAction);

            return;
        }");
		
        _skill = DataCache.getDtoByCls<Skill>(_gameAction.skillId);

        if (_skill == null)
        {
            GameDebuger.LogError(string.Format("技能数据为空 skillId:{0}", _gameAction.skillId));
            Finish();
            return;
        }

        var skillConfigId = _skill.id;

        if (skillConfigId == BattleDataManager.GetUseItemSkillId())
        {
            var actionTargetState = GetActionTargetState();
            if (actionTargetState != null)
            {
                if (actionTargetState.hp != 0)
                {
                    skillConfigId = 102;
                }
                else if (actionTargetState.cp != 0)
                {
                    skillConfigId = 104;
                }
                else
                {
                    skillConfigId = 102;
                }
                GameDebuger.TODO(@"else if (actionTargetState.mp != 0) {
                    skillConfigId = 103;
                }"); 
            }

            if (attacker.IsPlayerCtrlCharactor())
            {
                GameDebuger.TODO(@"BattleController.Instance.AddItemUsedCount ();");
            }

            //触发物品使用喊招
            GameDebuger.TODO(@"BattleController.Instance.GetInstController().TriggerMonsterShount(attacker.GetId(), ShoutConfig.BattleShoutTypeEnum_UseItem);");
        }

        if (skillConfigId == 5 ||skillConfigId == 8) skillConfigId = 8;
        else if (skillConfigId < 10)
        {
            skillConfigId = _skill.clientSkillType;
        }

    
        _skillConfig = BattleConfigManager.Instance.getSkillConfigInfo(skillConfigId);

        if (_skillConfig == null)
        {
            skillConfigId = _skill.clientSkillType;
            _skillConfig = BattleConfigManager.Instance.getSkillConfigInfo(skillConfigId);
        }

        if (_skillConfig == null)
        {
            GameDebuger.LogError(string.Format("无技能配置{0}， 使用默认的配置{1}", skillConfigId, 1));
            _skillConfig = BattleConfigManager.Instance.getSkillConfigInfo(1);
        }
		
        if (_skillConfig == null)
        {
            GameDebuger.LogError("技能配置为空");
            Finish();
            return;
        }

        GameDebuger.LogBattleInfo(string.Format("战斗所用技能，skillId：{0}，skillConfigId：{1}", _gameAction.skillId, skillConfigId));

        if (_injureActionPlayerDic == null)
        {
            _injureActionPlayerDic = new Dictionary<int, BattleActionPlayer>();
        }

        _totalWaitingRound = 0;
        _currentFinishRound = 0;
        _totalWaitingCount = 0;
        _currentFinishCount = 0;

        _waintingAttackFinish = false;

        _callSoldierIds.Clear();

        _injureIds = GetInjureIds(_gameAction);

        // Count 会包括自己的行动,+1
        _totalWaitingCount += 1;
		
        if (_injureIds.Contains(_gameAction.actionSoldierId))
        {
            _waintingAttackFinish = true;
        }

        var hasSameInjurer = false;
        for (int i = 0, len = _injureIds.Count; i < len; i++)
        {
            var id = _injureIds[i];
            if (!_injureActionPlayerDic.ContainsKey(i))
            {
                var player = BattleActionPlayerPoolManager.Instance.Spawn();
                _injureActionPlayerDic.Add(i, player);
                player.Init(this, _gameAction, false, _skillConfig, id, _skill.atOnce);
            }
            else
            {
                hasSameInjurer = true;
            }
        }

        if (_attackActionPlayer == null)
        {
            _attackActionPlayer = BattleActionPlayerPoolManager.Instance.Spawn();
            _attackActionPlayer.Init(this, _gameAction, true, _skillConfig, _gameAction.actionSoldierId, _skill.atOnce);
            _attackActionPlayer.SetInjureInfo(_injureIds);
        }

        if (_injureIds.Count == 0)
        {
            _totalWaitingRound = 1;
        }
        else
        {
            if (_skill.atOnce)
            {
                _totalWaitingRound = 1; // Round 是主角行动多少次攻击行为
                _totalWaitingCount += _injureIds.Count; // Count 是主角每次行动，有多少个行动对象(自己and敌人)

                if (hasSameInjurer)
                {
                    var log = string.Format("配置或者服务器下发有异常， 有可能卡住 skillanme={0}", _skill.name);
                    TipManager.AddTip(log);
                    GameDebuger.LogError(log);
                }
            }
            else
            {
                _totalWaitingRound += _injureIds.Count; // Round 是主角行动多少次攻击行为
                _totalWaitingCount += 1; // Count 是主角每次行动，有多少个行动对象(自己and敌人)
            }
        }

        ShowSkillNameAndAction(_gameAction.skill);
    }

    private VideoActionTargetState GetActionTargetState()
    {
        if (_gameAction.targetStateGroups.Count > 0)
        {
            for (int i = 0, len = _gameAction.targetStateGroups[0].targetStates.Count; i < len; i++)
            {
                var state = _gameAction.targetStateGroups[0].targetStates[i];
                if (state is VideoActionTargetState)
                {
                    return state as VideoActionTargetState;
                }
            }

            return null;
        }
        else
        {
            return null;
        }
    }

    //把本次行动的总伤害或总加血缓存起来，因为里边的数据在行动过程中可能会变化。
    private int CacheActionTotalDamageOrHeal()
    {
        if (null == _gameAction || _gameAction.targetStateGroups.IsNullOrEmpty())
            return 0;
        
        var tTotalDamange = 0;
        var tTotalHPAdded = 0;

        MonsterController _mc;
        _gameAction.targetStateGroups.Filter(group => group != null)
            .ForEach(group =>
            {
                group.targetStates.Filter(targetState => (targetState is VideoActionTargetState))
                    .Map(targetState=> targetState as VideoActionTargetState)
                    .ForEach(tVideoActionTargetState =>
                    {
                        _mc = MonsterManager.Instance.GetMonsterFromSoldierID(tVideoActionTargetState.id);
                        if (_mc == null)
                            return;
                        if (tVideoActionTargetState.hp > 0 && _mc.side == mActionSide)
                        {
                            //加血的时候，要算实际加血量。// 自己阵营的加血才算，对面的加血或者吸血或者被加血不算。
                            tTotalHPAdded += Math.Min(_mc.MaxHP - _mc.currentHP,
                                tVideoActionTargetState.hp);
                        }
                        else if (tVideoActionTargetState.hp < 0 && _mc.side != mActionSide)
                        {
//伤害可能溢出，直接用服务端给的值。且 是敌方才需要计算减血
                            tTotalDamange += -tVideoActionTargetState.hp;
                        }
                    });
            });
    
        return tTotalDamange > 0 ? -tTotalDamange : tTotalHPAdded;
    }

    private List<long> GetInjureIds(VideoSkillAction gameAction)
    {
        return gameAction.targetStateGroups
            .Map<VideoTargetStateGroup, long>(GetInjureId)
            .Filter(id=>id > 0).ToList();
    }

    private bool IsIndexBelongGroup(int pIndex,VideoTargetStateGroup pVideoTargetStateGroup)
    {
        if (null == mInjureIndexGroupDic || mInjureIndexGroupDic.Count <= 0 || null == pVideoTargetStateGroup)
            return false;
        VideoTargetStateGroup tVideoTargetStateGroup = null;
        mInjureIndexGroupDic.TryGetValue(pIndex, out tVideoTargetStateGroup);
        return tVideoTargetStateGroup == pVideoTargetStateGroup;
    }
    
    private List<long> _callSoldierIds = new List<long>();

    private long GetInjureId(VideoTargetStateGroup group)
    {
        for (int i = 0, len = group.targetStates.Count; i < len; i++)
        {
            var state = group.targetStates[i];
            if (state is VideoRetreatState)
                continue;
            GameDebuger.TODO(@"if (state is VideoRageTargetState) {
                         continue;
                     }

                     if (state is VideoTargetShoutState) {
                         continue;
                     }");

            //          if (state is VideoBuffRemoveTargetState)
            //          {
            //              continue;
            //          }
            if (state is VideoBuffAddTargetState)
            {
                var videoBuffAddTargetState = state as VideoBuffAddTargetState;
                if (_callSoldierIds.Contains(videoBuffAddTargetState.id))
                {
                    continue;
                }

                //当只有buff添加state的时候， 才考虑加入受击者，如果不是， 就不需要加入受击者
                if (group.targetStates.Count > 1)
                {
                    continue;
                }
            }
            GameDebuger.TODO(@"if (state is VideoCallSoldierState) {
                VideoCallSoldierState videoCallSoldierState = state as VideoCallSoldierState;
                if (_callSoldierIds.Contains (videoCallSoldierState.soldier.id) == false) {
                    _callSoldierIds.Add (videoCallSoldierState.soldier.id);
                }
                continue;
            }");
            //			if (state is VideoActionTargetState && (state as VideoActionTargetState).hp == 0)
            //			{
            //				continue;
            //			}

            if (state.id > 0)
            {
                return state.id;
            }
        }

        return 0;
    }

    //private bool IsPlayerSideAction(MonsterController attacker)
    //{
    //    if (attacker == null)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return attacker.side == BattlePosition.MonsterSide.Player;
    //    }
    //}

    private void ShowSkillNameAndAction(Skill skill)
    {
        if (skill == null) return;
        //普通攻击不显示技能名
        if (skill.type != (int)Skill.SkillEnum.None)
        {
            var attacker = MonsterManager.Instance.GetMonsterFromSoldierID(_gameAction.actionSoldierId);
            attacker.PlaySkillName(skill, ShowSkillAction);
        }
        else
        {
            ShowSkillAction();
        }
    }

    private void ShowSkillAction()
    {
        var mc = MonsterManager.Instance.GetMonsterFromSoldierID(_gameAction.actionSoldierId);
        if (mc != null)
        {
            if (_gameAction.driving)
            {
                BattleActionPlayer.PlayDrivingEffect(mc);
                mc.PlayDrivingAnimation();
            }
            else
            {
                mc.StopDrivingEffect();
            }
        }
        
        if (_attackActionPlayer == null
            || _gameAction.targetStateGroups.Count <= _currentFinishRound)
        {
            FinishPlayer();
        }
        else
        {   
            if (_skill.atOnce)
            {
                _attackActionPlayer.Play(_gameAction.targetStateGroups[_currentFinishRound], _injureIds, IsLastAction());
            }
            else
            {
                _attackActionPlayer.Play(_gameAction.targetStateGroups[_currentFinishRound], GetNextInjureId(), IsLastAction());
            }
        }
    }

    #region 更新当前攻击者的总伤害或总治疗

    private void UpdateCurActionTotalDamageOrHeal()
    {
        if (mActionSide != BattlePosition.MonsterSide.None && mTotalDamageOrHeal != 0)
            GameEventCenter.SendEvent(GameEvent.BATTLE_UI_UPDATE_ACTION_TOTAL_DAMAGE_OR_HEAL, mActionSide, mTotalDamageOrHeal);
    }

    private void UpdateActionSide()
    {
        mActionSide = BattlePosition.MonsterSide.None;
        if (null == _gameAction) return;
        var tMonsterController = MonsterManager.Instance.GetMonsterFromSoldierID(_gameAction.actionSoldierId);
        if (null != tMonsterController)
        {
            mActionSide = tMonsterController.side;
        }
    }

    #endregion

    private long GetNextInjureId()
    {
        return _injureIds.Count > 0 ? _injureIds[_currentFinishRound] : 0;
    }

    private bool IsLastAction()
    {
        return _currentFinishRound + 1 >= _totalWaitingRound;
    }

    private void DoStrikeBackAction(VideoSkillAction action)
    {
        if (_strikeBackActionPlayer != null) return;
        _strikeBackActionPlayer = new GameVideoGeneralActionPlayer();
        _strikeBackActionPlayer.OnActionFinish += OnStrikeBackActionFinish;
        _strikeBackActionPlayer.DoExcute(action);
    }

    private void OnStrikeBackActionFinish()
    {
        _strikeBackActionPlayer.OnActionFinish -= OnStrikeBackActionFinish;
        _strikeBackActionPlayer = null;
        _attackActionPlayer.Continue();
    }

    public override void CheckFinish()
    {
        _currentFinishCount++;

        if (_currentFinishCount >= _totalWaitingCount)
        {
            VideoTargetStateGroup stateGroup = _gameAction.targetStateGroups[_currentFinishRound];
            if (stateGroup.strikeBackAction != null)
            {
                _currentFinishCount--;
                VideoSkillAction strikeBackAction = stateGroup.strikeBackAction;
                stateGroup.strikeBackAction = null;
                DoStrikeBackAction(strikeBackAction);
            }
            else
            {
                _currentFinishCount = 0;
                _currentFinishRound++;
                if (_currentFinishRound >= _totalWaitingRound)
                {
                    FinishPlayer();
                }
                else
                {
                    //执行下一个攻击动作
                    //重置剩余的受击者
                    VideoTargetStateGroup tVideoTargetStateGroup = _gameAction.targetStateGroups[_currentFinishRound];
                    _injureActionPlayerDic.ForEach((pKeyValuePair) =>
                        {
                            if (IsIndexBelongGroup(pKeyValuePair.Key, tVideoTargetStateGroup))
                                pKeyValuePair.Value.Reset();
                            #if UNITY_EDITOR
                            else
                            {
                                GameDebuger.Log(string.Format("这货不是我这个行动的受击者，跳过不重置！pKeyValuePair.Key:{0},pKeyValuePair.Value:{1},pKeyValuePair.Value.GetTargetId():{2},pKeyValuePair.Value.GetInstanceID():{3}"
                                    , pKeyValuePair.Key, pKeyValuePair.Value, pKeyValuePair.Value.GetTargetId(), pKeyValuePair.Value.GetInstanceID()));
                            }
                            #endif
                        });
                    //此处旧代码会把已经行动完毕的也重置，导致已行动的也可能再行动一次。http://oa.cilugame.com/redmine/issues/19621
                    /**foreach (BattleActionPlayer player in _injureActionPlayerDic.Values)
                    {
                        player.Reset();
                    }*/
                    _attackActionPlayer.Play(_currentFinishRound,tVideoTargetStateGroup, GetNextInjureId(), IsLastAction());
                }
            }
        }
        else
        {
            if (_waintingAttackFinish)
            {
                _waintingAttackFinish = false;

                _injureActionPlayerDic.Values.ForEachI((tBattleActionPlayer, pIndex) =>
                    {
                        tBattleActionPlayer.Play(GetTargetStateGroup(tBattleActionPlayer.GetTargetId(),pIndex));
                    });
            }
        }
    }

    private void FinishPlayer()
    {
        if (_attackActionPlayer != null)
        {
            _attackActionPlayer.Destroy();
            DespawnBattleActionPlayer(_attackActionPlayer);
            _attackActionPlayer = null;
        }
		
        foreach (var player in _injureActionPlayerDic.Values)
        {
            player.Destroy();
            DespawnBattleActionPlayer(player);
        }
        _injureActionPlayerDic.Clear();
		
        var attacker = MonsterManager.Instance.GetMonsterFromSoldierID(_gameAction.actionSoldierId);
        attacker.PlayVideoSkillAction(_gameAction);

        if (_gameAction.hpSpent != 0)
        {
            //DelayFinish(0.2f);
            Finish();
        }
        else
        {
            Finish();
        }
    }

    private bool _waintingAttackFinish = false;
    
    private List<long> mIds;
    private MonsterController mAttacker;
    private int mIndex = 0;
    
    public override void PlayInjureAction(List<long> ids, float randomTime)
    {
        mIds = ids;
        mIndex = 0;
        mAttacker = MonsterManager.Instance.GetMonsterFromSoldierID(_gameAction.actionSoldierId);
       
        if (_waintingAttackFinish == false && ids.Count > 0)
        {
            if (ids.Count == 1)
            {
                PlayInjureHandle(ids[0], _currentFinishRound, mAttacker, _gameAction.targetStateGroups[_currentFinishRound]);
            }
            else
            {
                if (BattleSpecialFlowManager.Instance.hitInOrder)
                {
                    ExecuteNextPlayer();
                }
                else if (randomTime <= 0)
                {
                    for (int i = 0, len = ids.Count; i < len; i++)
                    {
                        PlayInjureHandle(ids[i], i, mAttacker);
                    }
                }
                else
                {
                    Action<long,float, MonsterController,int> invokeInjureAction = (id, delay, target, pIndex) =>
                    {
                        if (_injureActionPlayerDic.ContainsKey(pIndex))
                        {
                            BattleActionPlayer player = _injureActionPlayerDic[pIndex];
                            player.SetSkillTarget(target);
                            JSTimer.Instance.SetupCoolDown("InjureActionPlayerTimer_" + id, delay, null, () =>
                                {
                                    player.Play(GetTargetStateGroup(id, pIndex));
//                                    if (_skill.skillActionType == 1 || _skill.skillActionType == 2)
//                                    {
//                                        BattleDataManager.BattleInstController.Instance.TriggerMonsterShount(id, ShoutConfig.BattleShoutTypeEnum_SufferBeating);
//                                    }
                                });
                        }
                    };

                    int i = 0, len = ids.Count;
                    float time = 0;
                    do
                    {
                        long id = ids[i];
                        if (time <= 0)
                        {
                            PlayInjureHandle(id, i, mAttacker);
                        }
                        else
                        {
                            invokeInjureAction(id, time, mAttacker, i);
                        }

                        time = UnityEngine.Random.Range(randomTime / 10f, randomTime);
                        i++;
                    } while (i < len);
                }
            }
        }
    }
    
    private float GetTimeBySpeed(float pSpeed, MonsterController pCurBattleActionPlayer, MonsterController pNextBattleActionPlayer)
    {
        if (null == pCurBattleActionPlayer || null == pNextBattleActionPlayer)
            return 0;
        float tDistance = Vector3.Distance(pNextBattleActionPlayer.transform.position, pCurBattleActionPlayer.transform.position);
        float tTime = tDistance / pSpeed;
        return tTime;
    }
    
    private void ExecuteNextPlayer()
    {
        if (null == mIds || mIds.Count <= mIndex)
            return;
        float tDelayTime = Mathf.Max(GetDelayTimeToPlayNextAction(mIndex),0.25f);
//        GameDebuger.LogError(string.Format("ExecuteNextPlayer tDelayTime:{0},speed:{1}",tDelayTime,mBattleController.EffectMoveSpeed));
        if (BattleSpecialFlowManager.Instance.hitInOrderParam <= 0)
        {
            //GameDebuger.LogError("速度获取失败，设置为默认值。" + DEFAULT_EFFECT_MOVE_SPEED.ToString());
            BattleSpecialFlowManager.Instance.hitInOrderParam = DEFAULT_EFFECT_MOVE_SPEED;
        }

        PlayInjureHandle(mIds[mIndex], mIndex, mAttacker);
        mIndex++;
        if (tDelayTime <= 0)//受击者间没有时间顺序的话，直接播放下一个受击动作即可。
            ExecuteNextPlayer();
        else//闪电链类型的，受击者间有执行顺序，延迟一定时间后播放下一个受击者
            JSTimer.Instance.SetupCoolDown("ExecuteNextPlayer", tDelayTime, null, ExecuteNextPlayer);
    }
    
    private float GetDelayTimeToPlayNextAction(int pIndex)
    {
        MonsterController tCurMonsterController = GetMonsterController(pIndex);
        if (null == tCurMonsterController)
            return 0;

        MonsterController tNextMonsterController = GetMonsterController(pIndex + 1);
        float tDelayTime = BattleSpecialFlowManager.Instance.hitByTime ? BattleSpecialFlowManager.Instance.hitInOrderParam : 
            (GetTimeBySpeed(BattleSpecialFlowManager.Instance.hitInOrderParam, tCurMonsterController, tNextMonsterController));
        return tDelayTime;
    }
    
    private MonsterController GetMonsterController(int pIndex)
    {
        if (null == mIds || mIds.Count <= 0)
            return null;
        if (mIds.Count > pIndex)
            return MonsterManager.Instance.GetMonsterFromSoldierID(mIds[pIndex]);
        else
            return null;
    }
    private void PlayInjureHandle(
        long id
        , int pIndex
        , MonsterController attacker
        , VideoTargetStateGroup stateGroup = null)
    {
        if (_injureActionPlayerDic.ContainsKey(pIndex))
        {
//            if (id == ModelManager.Pet.GetBattlePetUID())
//                GameDebuger.LogError(string.Format("(>>>>>>>>>>>>>>>>>>PlayInjureHandle id:{0},pIndex:{1} ,mIds.Count - 1 :{2}", id, pIndex, (mIds.Count)));
            var player = _injureActionPlayerDic[pIndex];
            player.SetSkillTarget(attacker);

            if (stateGroup == null)
                player.Play(GetTargetStateGroup(id,pIndex),pIndex == mIds.Count - 1);
            else
                player.Play(stateGroup);

            //触发挨打喊话
            //攻击类型的法术才触发
//            if (_skill.skillActionType == 1 || _skill.skillActionType == 2)
//            {
//                //触发受击者喊招
//                    BattleDataManager.BattleInstController.Instance
//                        .TriggerMonsterShount(id, ShoutConfig.BattleShoutTypeEnum_SufferBeating);
//            }
        }
    }
    
    private VideoTargetStateGroup GetTargetStateGroup(long id,int pIndex)
    {
        VideoTargetStateGroup group = null;
        _gameAction.targetStateGroups.TryGetValue(pIndex, out group);
        if (group == null || GetInjureId(group) != id)
            return null;
        return group;
    }

    private long _startCheckTime = 0;

    public override void Finish()
    {
        //如果作为反击的播放， 则不需要处理汇总血量
        if (OnActionFinish == null)
        {
            UpdateCurActionTotalDamageOrHeal();
            //GameDebuger.LogError (string.Format(">>>Finish IsLastAction:{0},mDamageOrHealInfoShowed:{1}",IsLastAction() ,mDamageOrHealInfoShowed));
        }

        DoSkillEndActions();

        if (BattleDataManager.DEBUG)
        {
            if (_startCheckTime > 0)
            {
                var passTime = DateTime.Now.Ticks - _startCheckTime;
                var elapsedSpan = new TimeSpan(passTime);

                var skillName = _gameAction.skillId.ToString();
                if (_gameAction.skill != null)
                {
                    skillName = _gameAction.skill.name;
                }

                if (Math.Abs(_totalPlayTime - elapsedSpan.TotalSeconds) > 0.3f)
                {
                    GameLog.Log_Battle(string.Format("预估播放时长={0}S 真实播放时长={1}S 技能={2}", _totalPlayTime, elapsedSpan.TotalSeconds, skillName), "orange");
                }
                else
                {
                    GameLog.Log_Battle(string.Format("预估播放时长={0}S 真实播放时长={1}S 技能={2}", _totalPlayTime, elapsedSpan.TotalSeconds, skillName), "orange");
                }
                _startCheckTime = 0;
            }
        }

        //如果作为反击的播放处理， 则不要调用基础的finish
        if (OnActionFinish != null)
        {
            OnActionFinish();
        }
        else
        {
            base.Finish();
        }
    }

    protected virtual void DespawnBattleActionPlayer(BattleActionPlayer pBattleActionPlayer)
    {
        BattleActionPlayerPoolManager.Instance.Despawn(pBattleActionPlayer);
    }

    public MonsterController GetPreviousMonsterController(int pCurrentIndex, bool pIgnoreAtOnce = false)
    {
        List<long> tIDList = pIgnoreAtOnce ? _injureIds : mIds;
        if (null == tIDList || tIDList.Count < pCurrentIndex)
            return null;
        if (pCurrentIndex == -1)
            return null;
        if (pCurrentIndex == 0)
            return mAttacker;
        return MonsterManager.Instance.GetMonsterFromSoldierID(tIDList[pCurrentIndex - 1]);
    }
}

