using System;
using System.Collections.Generic;
using DG.Tweening;
using AppDto;
using AssetPipeline;
using MyGameScripts.Gameplay.Battle.Demo.Helper;
using UnityEngine;
using MonsterManager = BattleDataManager.MonsterManager;
using BattleInstController = BattleDataManager.BattleInstController;

public class MonsterController : MonoBehaviour
{
    public void UpdateHpEpCp(VideoTargetState bas)
    {
        if (bas is VideoActionTargetState)
        {
            var action = (VideoActionTargetState)bas;

            modifyHP = action.hp;
            modifyEP = action.ep;
            modifyCP = action.cp;
            lastHP = action.currentHp;
            lastCP = action.currentCp;
            lastEP = action.currentEp;
        }
        else
        {
            modifyHP = 0;
            modifyEP = 0;
            modifyCP = 0;
            lastHP = videoSoldier.hp;
            lastCP = videoSoldier.cp;
            lastEP = videoSoldier.ep;
        }
    }

    #region Delegates

    public Action<long> afterLoadModel;
    public delegate void ActionEnd(string type);
    public static event Action<MonsterController>selectController;
    #endregion

    public enum RetreatMode
    {
        Normal,
        Fly,
        Flash,
        Run
    }

    // Temporary stat
    public enum ShowMessageEffect
    {
        DODGE = 1 << 0,
        CRITICAL = 1 << 1,
        IMMUNE = 1 << 2
    }

    //--------------------------------------------------------------

    //private static string TargetSelectEffectPath = GameEffectConst.GetGameEffectPath(GameEffectConst.Effect_TargetSelect);

    private static string TargetClickEffectPath =
        PathHelper.GetEffectPath(GameEffectConst.GameEffectConstEnum.Effect_CharactorClick);

    private static string SummonEffectPath = PathHelper.GetEffectPath(GameEffectConst.GameEffectConstEnum.Effect_Summon);
    //初始化所有可用技能，防止在战斗中改变了
    private List<int> _allSkillIds;
    private bool _backToOriginPosition;

    private int _currentHP;
    //当前HP

    private int _currentEP;
    //当前MP

    private int _currentCP;
    //当前SP

    private string _delayShoutContent;

    private bool _firstAnimation = true;

    //--------------------------------------------------------------------------
    private float _lastBodyShiftDistance;
    private ModelDisplayer _modelDisplayer;

    private ModelStyleInfo _modelStyleInfo;

    private Action _playSkillCallback;

    //初始化可用技能，防止在战斗中改变了
    private IEnumerable<int> _skillIds;

    public ActionEnd actionEnd;
    private BattleMonsterBuff battleMonsterBuff;

    private BattleTargetSelector battleTargetSelector;
    public BattleTargetSelector TargetSelector{
        get{ return battleTargetSelector;}
    }

    private Dictionary<int, VideoBuffAddTargetState> buffStateMaps;

    public bool dead
    {
        get { return _dead; }
        set
        {
            _dead = value;
        }
    }
    private bool _dead;
    public bool driving;
    private bool floatTextPlaying;

    private Queue<string> floatTextQueue = new Queue<string>();

    private bool HasDestroyEffect;

    private bool HasDestroyMe;

    protected BattleMonsterHPSlider hpSlider;

    //action执行后的HP
    public int _lastHP;

    public int lastHP
    {
        get { return _lastHP; }
        set
        {
//            if (_videoSoldier.id == ModelManager.IPlayer.GetPlayerId() && value <= 0)
//                GameLog.Log_Battle_PlayerAttr("Set last Hp == 0");
            _lastHP = value;
        }
    }

    //action执行后的CP
    public int lastCP;

    //action执行后的EP
    public int lastEP;

    public bool leave = false;

    public int modifyHP;
    public int modifyEP;
    public int modifyCP;

    protected BattleMonsterName monsterName;

    private bool monsterNameShow = true;
    protected BattleMonsterOrder monsterOrder;
    public BattleMonsterOrderArrow monsterOrderArrow;
    public BattleMonsterPosition monsterPosition;

    private Transform _mTrans;

    public bool NeedReady;

    private bool mIsInCD = false;
    //是否正在CD
    public bool IsInCD
    {
        get{ return mIsInCD; }
        set
        {
            if (mIsInCD != value)
            {
                mIsInCD = value;
                GameEventCenter.SendEvent(GameEvent.BATTLE_UI_CD_STATUS_UPDATE,this);
            }
        }
    }

    public Vector3 originPosition;
    public Vector3 originRotation;

    public int ShoutDialogueId;

    public ShowMessageEffect showMessageEffect;

    public BattlePosition.MonsterSide side;

    public MonsterController target;
    public int targetReference = 0;

    //private GameObject targetSelectEffect;
/**技能特效管理*/
    private BattleSkillEffectCMPT mBattleSkillEffectCMPT;

    private VideoSoldier _videoSoldier;

    public VideoSoldier videoSoldier {
        get
        {
            return _videoSoldier;
        }

    }

    public int currentHP
    {
        get { return _currentHP; }
        set { _currentHP = Mathf.Clamp(value, 0, MaxHP); }
    }

    public int currentEp
    {
        get { return _currentEP; }
        set { _currentEP = Mathf.Clamp(value, 0, MaxEP); }
    }

    public int currentCp
    {
        get { return _currentCP; }
        set
        {
            var pTargetValue = Mathf.Clamp(value, 0, MaxCP);
            if (_currentCP == pTargetValue) return;
            _currentCP = pTargetValue;
            var tUID = GetId();
            if (tUID == ModelManager.Player.GetPlayerId())
                GameEventCenter.SendEvent(GameEvent.BATTLE_UI_SP_UPDATED, tUID, _currentCP);
        }
    }

    /**public int magicMana;*/

    private void DisposeSkillEffCmpt()
    {
        if (null != mBattleSkillEffectCMPT)
        {
            mBattleSkillEffectCMPT.Dispose();
            mBattleSkillEffectCMPT = null;
        }
    }

    public void Dispose()
    {
        DisposeSkillEffCmpt();
        // todo fish:为什么角色死亡时 battleTargetSelector已经为空？清理流程 
        if (battleTargetSelector != null)
            battleTargetSelector.Dispose();
        battleTargetSelector = null;
    }

    /**控制挂点在播放动作时是否跟随移动*/
    private BattleMountCMPT mBattleMountCMPT;
    
    protected void Awake()
    {
        _mTrans = transform;
    }

    public void ClearMessageEffect(bool cleanAll)
    {
        modifyHP = 0;
        modifyEP = 0;
        modifyCP = 0;

        lastHP = 0;
        lastCP = -1;
        lastEP = -1;
        showMessageEffect = 0;
    }

    public void AddMessageEffect(ShowMessageEffect effect)
    {
        GameDebuger.Log(effect.ToString());

        showMessageEffect = showMessageEffect | effect;
    }

    public bool existMessageEffect(ShowMessageEffect effect)
    {
        return (int)(showMessageEffect & effect) != 0;
    }

    public void InitMonster(VideoSoldier data, BattlePosition.MonsterSide _side = BattlePosition.MonsterSide.Player,
        /**NpcAppearanceDto npcAppearanceDto = null,*/bool showFashion = false, Vector3 pos = default(Vector3))
    {

        mBattleSkillEffectCMPT = new BattleSkillEffectCMPT();
        side = _side;

        _videoSoldier = data;
        IsInCD = false;

        currentHP = data.hp;

        currentEp = data.ep;

        lastCP = data.cp;
        currentCp = lastCP;

        //初始化可用技能，防止在战斗中改变了
        if (videoSoldier.playerId == ModelManager.IPlayer.GetPlayerId())
        {
            battleTargetSelector = BattleTargetSelector.Create(
                this
                , videoSoldier.GetNormallAtkSkill());
            var skill = DataCache.getDtoByCls<Skill>(data.defaultSkillId);
            battleTargetSelector.MagicOrCraftSkill = skill;
            
            _skillIds = DemoSimulateHelper.GetMainCharacterSkillIDList(data);
//            _skillIds = DemoSimulateHelper.SimulateSkillList();

            GameDebuger.TODO(@"List<int> fskills = new List<int>();
            Faction faction = GetFaction();
            fskills.Add(faction.mainFactionSkillId);
            fskills.AddRange(faction.propertyFactionSkillIds);


            for (int i = 0, len = fskills.Count; i < len; i++)
            {
                FactionSkill factionSkill = DataCache.getDtoByCls<FactionSkill>(fskills[i]);

                for (int j = 0, len2 = factionSkill.skillInfos.Count; j < len2; j++)
                {
                    SkillInfo info = factionSkill.skillInfos[j];
                    if (info.skill.activeSkill &&
                        ModelManager.FactionSkill.GetFactionSkillLevel(factionSkill.id) >=
                        info.acquireFactionSkillLevel)
                    {
                        _skillIds.Add(info.skillId);
                    }
                }
            }");
        }

        InitBuffStateMaps();

        GameDebuger.TODO(@"InitPet(modelStyleInfo.ToInfo(data, npcAppearanceDto, showFashion));");
        var p = pos == default(Vector3) ? _mTrans.position : pos;
        InitPet(ModelStyleInfo.ToInfo(data, showFashion), p);
        gameObject.name = side + "_" + GetId() + "_" + GetModel();
    }

    private void DelayInitBattleMount()
    {
        GetMountBattleEffect();
        GetMountDamageffect();
        GetBattleGroundMount();

        if (_showSelectEffect)
        {
            ShowSelectEffect();
        }
    }

    public void InitPetModel()
    {
        _modelDisplayer = new ModelDisplayer(gameObject, OnLoadModelFinish);
        _modelDisplayer.SetLookInfo(_modelStyleInfo);
    }

    private void OnLoadModelFinish()
    {
        //-------------------- Set LayMask ------------------------
        gameObject.layer = LayerMask.NameToLayer(GameTag.Tag_BattleActor);
        gameObject.tag = GameTag.Tag_BattleActor;

        Initialize();

        //InitBodyFadeInOutEffect();

        Invoke("DelayInitBattleMount", 0.05f);
        GameUtil.SafeRun(afterLoadModel, GetId());
    }

    protected void InitPet(ModelStyleInfo modelStyleInfo, Vector3 pos)
    {
        _modelStyleInfo = modelStyleInfo;

        gameObject.tag = GameTag.Tag_BattleActor;

        originPosition = pos;
        originRotation = _mTrans.rotation.eulerAngles;

        InitPetModel();
    }

    public void MoveTo(Transform target, float targetOffset, ModelHelper.AnimType animation, float time, Vector3 rotation, bool needFinishTurn = false)
    {
        Vector3 targetPosition = target.position + target.TransformDirection(Vector3.forward) * targetOffset;

        _mTrans.LookAt(target);
        _mTrans.eulerAngles = new Vector3(rotation.x, _mTrans.eulerAngles.y, rotation.z);

        Goto(targetPosition, animation, 0, false, false, needFinishTurn,time);
    }

    public float Goto(Vector3 position, float delay = 0, bool turn = false,
        bool catchMode = false, bool needFinishTurn = false)
    {
        var totalDis = Vector3.Distance(_mTrans.position, position);
        var time = totalDis /
                   (catchMode ? ModelHelper.DefaultBattleCatchSpeed * (turn ? 2f : 1f) : ModelHelper.DefaultBattleModelSpeed);

//        ModelCopy.ShowCopy();
//        _selectView.UpdateFollowTarget(ModelCopy.Copy);
        //		iTween.MoveTo(gameObject, iTween.Hash("x", position.x, "y", position.y, "z", position.z, "time", time, "delay", delay, "easetype", "linear", "oncomplete", "GotoComplete"));
        _mTrans.DOKill(false);

        _mTrans.DOMove(position, time).SetDelay(delay).SetEase(Ease.Linear).OnComplete(() =>
        {
            PlayAnimation(
                ModelHelper.AnimType.showup
                , delegate(ModelHelper.AnimType type, float f) {
                InstantiateGameObject("game_eff_chuchang", GetMountShadow(), false, false, 5f);
                ShowSCraftEff();
             });
        });

        return time +  + 1.5f; // showup的播放时间
    }

    public float Goto(Vector3 position, ModelHelper.AnimType animationName, float delay = 0, bool turn = false,
                      bool catchMode = false, bool needFinishTurn = false, float time=-1)
    {
        PlayAnimation(animationName);
        
        if (turn)
        {
            _mTrans.eulerAngles = new Vector3(_mTrans.eulerAngles.x, _mTrans.eulerAngles.y + 180,
                _mTrans.eulerAngles.z);
        }

        var totalDis = Vector3.Distance(_mTrans.position, position);
        if (time<float.Epsilon)
        {
            time = totalDis /
                   (catchMode ? ModelHelper.DefaultBattleCatchSpeed * (turn ? 2f : 1f) : ModelHelper.DefaultBattleModelSpeed);
        }

//        ModelCopy.ShowCopy();
//        _selectView.UpdateFollowTarget(ModelCopy.Copy);
        //		iTween.MoveTo(gameObject, iTween.Hash("x", position.x, "y", position.y, "z", position.z, "time", time, "delay", delay, "easetype", "linear", "oncomplete", "GotoComplete"));
        _mTrans.DOKill(false);
        _mTrans.DOMove(position, time).SetDelay(delay).SetEase(Ease.Linear).OnComplete(() =>
            {
                GotoComplete(needFinishTurn);
            });

        return time;
    }

    public void DoMove(
        Vector3 position
        ,float time
        )
    {
        _mTrans.DOKill();
        _mTrans.DOMove(position, time).SetEase(Ease.Linear);
    }

    private void GotoComplete(bool needFinishTurn)
    {
        if (needFinishTurn)
        {
            _mTrans.eulerAngles = new Vector3(originRotation.x, originRotation.y + 180,
                originRotation.z);
        }
        else
        {
            _mTrans.eulerAngles = originRotation;
        }
        PlayAnimation(ModelHelper.AnimType.battle);
        if (actionEnd != null)
            actionEnd("move");
    }

    public void ReturnToOrigin()
    {
        if (_mTrans.position != originPosition)
            GoBack(0.1f, Vector3.zero, ModelHelper.AnimType.run, false);
    }

    public void GoBack(float time, Vector3 rotation, ModelHelper.AnimType animationName, bool callbackWhenFinished = true,
                       float delayTime = 0f, bool catchMode = false)
    {
        //		_mTrans.position = originPosition;
        //		_mTrans.eulerAngles = originRotation;
        //
        //		GotoBackComplete(callbackWhenFinished);

        if (animationName != ModelHelper.AnimType.invalid)
        {
            PlayAnimation(animationName);
        }

        var position = originPosition;

        _mTrans.eulerAngles = originRotation + rotation;

        var totalDis = Vector3.Distance(_mTrans.position, position);
        if (time < float.Epsilon)
        {
            time = totalDis / (catchMode ? ModelHelper.DefaultBattleCatchSpeed : ModelHelper.DefaultBattleModelSpeed);
        }

        //		iTween.MoveTo(gameObject, iTween.Hash("x", position.x, "y", position.y, "z", position.z, "time", time, "delay", delayTime,
        //		                                      "easetype", "linear", "oncomplete", "GotoBackComplete",
        //		                                      "oncompleteparams", callbackWhenFinished));
        _mTrans.DOMove(position, time)
            .SetDelay(delayTime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                GotoBackComplete(callbackWhenFinished);
            });
    }
    
    private void GotoBackComplete(bool callbackWhenFinished)
    {
        _selectView.UpdateFollowTarget(this);
//        ModelCopy.HideCopy();
        _mTrans.eulerAngles = originRotation;
        //RotateToOriginDirection( 1.0f );

        //PlayAnimation( ModelHelper.AnimType.idle );
        //petAnimation.wrapMode = WrapMode.Loop;

        if (!callbackWhenFinished)
            return;

        if (actionEnd != null)
            actionEnd("moveback");
    }

    //if hp is 20% left then play pant, if not play idle
    public void PlayStateAnimation()
    {
        CheckStateAnimation();

        if (!IsDead())
        {
            if (driving)
                PlayDrivingAnimation();
            else
            {
                PlayIdleAnimation();    
            }
        }
    }

    //检查是否需要复活
    protected void CheckStateAnimation()
    {
        if (dead) return;
        _modelDisplayer.SetActive(true);
        HideShadow();
    }

    public void PlayIdleAnimation()
    {
        PlayAnimation(ModelHelper.AnimType.battle);
        _modelDisplayer.SetSoulEffectActive(true);
    }

    public void PlayDieAnimation(bool needCallback = true)
    {
        //if (_battleController != null)
        //{
        //	_battleController.AddDieMonster(GetId());
        //}

        if (_modelDisplayer.IsAnimatorReady())
        {

            var animatorState =
                _modelDisplayer.GetCurrentAnimatorStateInfo(ModelHelper.Animator_Layer_BattleLayer);
            if (!animatorState.IsName(ModelHelper.AnimType.death.ToString()))
            {
                if (needCallback)
                {
                    PlayAnimation(ModelHelper.AnimType.death, OnDieFinishCallback);
                }
                else
                {
                    PlayAnimation(ModelHelper.AnimType.death, null);
                }
            }
        }

        _modelDisplayer.SetSoulEffectActive(false);

        HideShadow();
    }

    private void OnDieFinishCallback(ModelHelper.AnimType animType, float length)
    {
        Invoke("OnFadeEffectFinish", length);
    }

    private void OnFadeEffectFinish()
    {
        AnimatinCallback();
    }

    public void InitDieState()
    {
        HideShadow();
        _modelDisplayer.SetActive(false);
    }

    private void HideShadow()
    {
        //		if (petModelObject != null)
        //		{
        //			FS_ShadowSimple shadow = petModelObject.GetComponentInChildren<FS_ShadowSimple>();
        //			if (shadow != null)
        //			{
        //				shadow.enabled = false;
        //			}
        //		}
    }

    public void BodyShift(float distance, float time, int direction, float shiftBackDelay = 0,
                          bool backToOriginPosition = false)
    {
        CancelInvoke("BodShiftComplete");

        _lastBodyShiftDistance = distance;
        _backToOriginPosition = backToOriginPosition;

        Vector3 position = _mTrans.position;

        //位移方向(0/1/2/3 上/下/前/后)
        Vector3 moveDir = new Vector3();
        if (direction == 0)
            moveDir = Vector3.up;
        else if (direction == 1)
            moveDir = -Vector3.up;
        else if (direction == 2)
            moveDir = Vector3.forward;
        else if (direction == 3)
            moveDir = -Vector3.forward;

        position += _mTrans.TransformDirection(moveDir) * distance;

        if (time == 0)
        {
            _mTrans.position = position;
        }
        else
        {
            _mTrans.DOMove(position, time).SetEase(Ease.Linear)
                .OnComplete(() => BodShiftCompleteDelay(shiftBackDelay));
        }
    }

    private void BodShiftComplete()
    {
        GoBackFromShit(0.1f);
    }

    private void BodShiftCompleteDelay(float delay)
    {
        if (delay == 0)
        {
            BodShiftComplete();
        }
        else if (delay > 0)
        {
            Invoke("BodShiftComplete", delay);
        }
    }

    public void GoBackFromShit(float time)
    {
        if (_modelDisplayer.IsAnimatorReady())
        {
            var animatorState =
                _modelDisplayer.GetCurrentAnimatorStateInfo(ModelHelper.Animator_Layer_BattleLayer);
            if (!animatorState.IsName(ModelHelper.AnimType.death.ToString()))
            {
                if (!animatorState.IsName(ModelHelper.AnimType.hit.ToString()))
                {
                    if (GetId() == ModelManager.IPlayer.GetPlayerId())
                    {
                        IntEnumHelper.getEnums<ModelHelper.AnimType>()
                            .ForEach(e =>
                            {
                                if (animatorState.IsName(e.ToString()))
                                GameLog.Log_Battle_Anim("GoBackFromShit  name" + e);
                            });
                    }
                    PlayAnimation(ModelHelper.AnimType.show);// todo fish:hit需要被拆 成两个动作，暂时没有hit2
                }
                else
                {
                    PlayAnimation(ModelHelper.AnimType.battle );
                }
            }
        }

        var position = originPosition;
        if (_backToOriginPosition == false)
        {
            //位移方向(0/1/2/3 上/下/前/后)
            Vector3 moveDir = Vector3.forward;
            position = _mTrans.position;

            position += _mTrans.TransformDirection(moveDir) * _lastBodyShiftDistance;
        }

        //		iTween.MoveTo(gameObject, iTween.Hash("x", position.x, "y", position.y, "z", position.z, "time", time,
        //		                                      "easetype", "linear", "oncomplete", "BodShiftBackComplete"));
        _mTrans.DOMove(position, time).SetEase(Ease.Linear).OnComplete(BodShiftBackComplete);
    }

    private void BodShiftBackComplete()
    {
        AnimatinCallback();
    }

    //------------------------------------------------------------------------------

    public void UpdateMCPositionByDirection(MonsterController pTargetMonsterController, ModelDirection pDirection)
    {
        if (null == pTargetMonsterController)
            return;
        //位移方向(0/1/2/3 上/下/前/后)
        Vector3 tTargetPosition = Vector3.zero;
        switch (pDirection)
        {
            case ModelDirection.Right:
                tTargetPosition = new Vector3(-1.4f, 0, 0.431f);
                break;
            case ModelDirection.Top:
                tTargetPosition = new Vector3(-0.196f, 0, -1.29f);
                break;
            case ModelDirection.Left:
                tTargetPosition = new Vector3(1.421f, 0, -0.276f);
                break;
            case ModelDirection.Bottom:
                tTargetPosition = new Vector3(0.264f, 0, 1.316f);
                break;
            default:
                GameDebuger.LogError(string.Format("更新设置人物方向失败，配置了未知方向：pDirection:{0}（方向只能为0-3）", pDirection));
                break;
        }
        if (pTargetMonsterController.side == BattlePosition.MonsterSide.Player)
            tTargetPosition *= -1;

        Vector3 tPositon = pTargetMonsterController.transform.position;
        tPositon += tTargetPosition;
        bool tNeedHide = Vector3.Distance(tPositon, _mTrans.position) > 1f;
        TweenCallback tTweenCallback = () =>
        {
            _mTrans.position = tPositon;
            _mTrans.LookAt(pTargetMonsterController.transform);
        };
        if (tNeedHide)
        {
            PlayHideEffect();
            JSTimer.Instance.SetupCoolDown(string.Format("UpdateMCPositionByDirection_{0}_{1}", _mTrans.GetInstanceID(), pTargetMonsterController.GetInstanceID()), 0.2f, null, () =>
            {
                if (null == _mTrans || null == pTargetMonsterController)
                    return;
                DelayHideEffect();
                tTweenCallback();
            });
        }
        else
            tTweenCallback();
      
    }
    
    public void PlayNormalActionInfo(ModelHelper.AnimType name, float duration,bool mountFollow = false)
    {
        //GameDebuger.LogError(string.Format("PlayNormalActionInfo(GetId():{0},name:{1}, duration:{2},Time.realtimeSinceStartup:{3})", GetId(), name, duration, Time.realtimeSinceStartup));
        CancelInvoke("AnimatinCallback");
        CancelInvoke("PlayIdleAnimation");
        CancelInvoke("CheckAnimationCallback");
        CancelInvoke("CheckAnimationIdleCallback");

        if (duration == 0)
        {
            PlayAnimation(name, delegate (ModelHelper.AnimType arg1, float arg2)
            {
                if (arg2 == 0)
                {
                    Invoke("CheckAnimationCallback", 0.05f);
                }
                else
                {
                    Invoke("AnimatinCallback", arg2);
                }

                SetMountMoveWithAnimation(mountFollow,arg2);
            });
        }
        else
        {
            PlayAnimation(name, delegate
            {
                //				if (duration > arg2 && name != ModelHelper.AnimType.def)
                //				{
                //					if (arg2 == 0)
                //					{
                //						Invoke("CheckAnimationIdleCallback", 0.05f);
                //					}
                //					else
                //					{
                //						Invoke("PlayIdleAnimation", arg2);
                //					}
                //                }
            });
            Invoke("AnimatinCallback", duration);
            SetMountMoveWithAnimation(mountFollow,duration);
        }
    }

    private void SetMountMoveWithAnimation(bool moveWithAnimation,float duration)
    {
        BattleMountCMPT.SetMountMoveWithAnimation(GetMountShadow(), moveWithAnimation, duration);
        BattleMountCMPT.SetMountMoveWithAnimation(GetMountHUD(), moveWithAnimation, duration);
        BattleMountCMPT.SetMountMoveWithAnimation(GetMountHit(), moveWithAnimation, duration);
    }
    
    private void CheckAnimationCallback()
    {
        if (_modelDisplayer.IsAnimatorReady())
        {
            AnimatorStateInfo animatorState =
                _modelDisplayer.GetCurrentAnimatorStateInfo(ModelHelper.Animator_Layer_BattleLayer);
            Invoke("AnimatinCallback", animatorState.length - 0.05f);
        }
        else
        {
            Invoke("AnimatinCallback", 0.5f);
        }
    }

    private void CheckAnimationIdleCallback()
    {
        if (_modelDisplayer.IsAnimatorReady())
        {
            AnimatorStateInfo animatorState =
                _modelDisplayer.GetCurrentAnimatorStateInfo(ModelHelper.Animator_Layer_BattleLayer);
            Invoke("PlayIdleAnimation", animatorState.length - 0.05f);
        }
        else
        {
            Invoke("PlayIdleAnimation", 0.5f);
        }
    }

    private void AnimatinCallback()
    {
        if (actionEnd != null)
        {
            actionEnd("normal");
        }
        else
        {
            PlayStateAnimation();
        }
    }

    public float GetAnimationDuration(ModelHelper.AnimType animate)
    {
        return _modelDisplayer.GetAnimationDuration(animate);
    }
    
    public void PlayAnimation(ModelHelper.AnimType name)
    {
        PlayAnimation(name, false, null);
    }

    public void PlayAnimation(ModelHelper.AnimType name, Action<ModelHelper.AnimType, float> animClipCallBack = null)
    {
        PlayAnimation(name, false, animClipCallBack);
    }

    private void PlayAnimation(ModelHelper.AnimType name,bool forcePlay, Action<ModelHelper.AnimType, float> animClipCallBack = null )
    {
        //if (name == ModelHelper.AnimType.battle && ShowLogEnable)
        //   GameDebuger.LogError("PlayIdleAnimation");

        var checkSameAnim = false;

        if (name == ModelHelper.AnimType.randomAttack)
        {
            name = ModelDisplayController.AttackAnimationClipList.Random();
        }

        if (name == ModelHelper.AnimType.death
            || name == ModelHelper.AnimType.battle)
        {
            checkSameAnim = true;
        }

        if (_firstAnimation)
        {
            _firstAnimation = false;
            checkSameAnim = false;
        }

        if (!forcePlay && _modelDisplayer.IsAnimatorReady())
        {
            var animatorState =
                _modelDisplayer.GetCurrentAnimatorStateInfo(ModelHelper.Animator_Layer_BattleLayer);
            if (animatorState.IsName(ModelHelper.AnimType.hit.ToString())/** || animatorState.IsName(ModelHelper.AnimType.hit2*/
                && name == ModelHelper.AnimType.battle)
            {
                return;
            }
        }
        
        _modelDisplayer.PlayAnimateWithCallback(name, false, animClipCallBack, checkSameAnim,
            ModelHelper.Animator_Layer_BattleLayer);
    }
    

    protected void Initialize()
    {
        InitHudText();

        var buf = _videoSoldier.buffs.Find<VideoBuffAddTargetState>(buff => buff.dead);
        if (buf != null)
        {
            dead = true;
            PlayDieAnimation(false);
        }

        _videoSoldier.buffs.ForEach<VideoBuffAddTargetState>(buff =>
        {
            AddBuffState(buff, false);
            AddBuffHandler(buff);
        });

//        PlayStateAnimation();
        Invoke("ShowPetShoutContent", 0.5f);
    }

    //---------------------------------------------------------------
    //hide()

    /// <summary>
    ///     0 normal do nothing
    ///     1 monster fly
    ///     2 crew flash
    ///     3 run
    /// </summary>
    /// <param name="mode">Mode.</param>
    /// <param name="delayTime">Delay time.</param>
    public void RetreatFromBattle(RetreatMode mode = RetreatMode.Normal, float delayTime = 0f)
    {
        RemoveAllBuff();

        switch (mode)
        {
            case RetreatMode.Normal:
                //normal do nothing
                LeaveBattle();
                break;
            case RetreatMode.Fly:
                //monster fly
                var autoRotate = gameObject.AddMissingComponent<AutoRotation>();
                autoRotate.rotationSpeed = new Vector3(0f, 40f, 0f);

                var addVec = side == BattlePosition.MonsterSide.Enemy ? BattleConst.EnemyRetreatPoint : BattleConst.PlayerRetreatPoint;
                Goto(_mTrans.position + addVec, ModelHelper.AnimType.run);
                Invoke("LeaveBattle", 0.7f);
                break;
            case RetreatMode.Flash:
                //crew flash
                RendererFlicker rendererFlicker = gameObject.AddMissingComponent<RendererFlicker>();
                rendererFlicker.flashDelay = delayTime + 0.2f;
                rendererFlicker.flashSpeed = 0.2f;
                Invoke("LeaveBattle", delayTime + 1f);
                break;
            case RetreatMode.Run:
                //run
                Invoke("DoRetreatAction", delayTime);
                break;
        }
    }

    public float EnterBattleScene()
    {
        var pos = BattlePositionCalculator.GetMonsterPosition(_videoSoldier, side);
        return Goto(pos);
    }

    private void DoRetreatAction()
    {
        RotationToBack();

        var addVec= side == BattlePosition.MonsterSide.Player ? BattleConst.PlayerRetreatPoint : BattleConst.EnemyRetreatPoint;
        var effname = PathHelper.GetEffectPath(GameEffectConst.GameEffectConstEnum.Effect_Retreat);
        OneShotSceneEffect.BeginFollowEffect(effname, transform, 1.2f, 1f);
        
        Goto(_mTrans.position + addVec, ModelHelper.AnimType.run);
        Invoke("LeaveBattle", 1.2f);
    }

    public void LeaveBattle()
    {
        /**if (BattleController.Instance != null)*/
        MonsterManager.Instance.RemoveMonsterController(this.GetId());
    }

    private BattleMountCMPT BattleMountCMPT
    {
        get
        {
            if (null == mBattleMountCMPT)
                mBattleMountCMPT = new BattleMountCMPT();
            return mBattleMountCMPT;
        }
    }
    public void DestroyMe()
    {
        if (HasDestroyMe)
        {
            return;
        }

        IsInCD = false;
        HasDestroyMe = true;

        CancelInvoke("DoDelayShout");

        this.RemoveComponent<AutoRotation>();

        RendererFlicker rendererFlicker = GetComponent<RendererFlicker>();
        if (rendererFlicker != null)
        {
            rendererFlicker.Dispose();
            Destroy(rendererFlicker);
        }

//        if (null != mModelCopy)
//        {
//            mModelCopy.Dispose();
//            mModelCopy = null;
//        }

        if (null != mMonsterOptionStateManager)
        {
            mMonsterOptionStateManager.Dispose();
            mMonsterOptionStateManager = null;
        }

        DestroyPetModel();
        DestroyEffect();
        
        if (null != mBattleMountCMPT)
        {
            mBattleMountCMPT.Dispose();
            mBattleMountCMPT = null;
        }
        
        Destroy(gameObject);
    }

    public void DestroyPetModel()
    {
        DestroyBattleMount();

        _modelDisplayer.Destory();

        RemoveAllBuff();
        DestroyHPSlider();
        DestroyMonsterName();
        DestroyMonsterOrder();
        DestroyMonsterOrderArrow();
        DestroyMonsterPosition();
        DestroyMonsterSelectView();
    }

    public void DestroyEffect()
    {
        HasDestroyEffect = true;
    }

    public bool CheckLogAttrPre()
    {
        return videoSoldier.id == ModelManager.IPlayer.GetPlayerId();
    }

    public void PlayInjure()
    {
        if (existMessageEffect(ShowMessageEffect.DODGE)
            || existMessageEffect(ShowMessageEffect.IMMUNE))
        {
            var eff = GetEffName();
            AddStatusEffect(eff);
        }
        else
        {
            var effName = existMessageEffect(ShowMessageEffect.CRITICAL) ? ShowMessageEffect.CRITICAL.ToString() : string.Empty;
            AddHPMPValue(effName);
        }
    }

    private string GetEffName()
    {
        var effectName = string.Empty;
        if (existMessageEffect(ShowMessageEffect.CRITICAL))
        {
            effectName = ShowMessageEffect.CRITICAL.ToString();
        }
        else if (existMessageEffect(ShowMessageEffect.DODGE))
            effectName = ShowMessageEffect.DODGE.ToString();
        else if (existMessageEffect(ShowMessageEffect.IMMUNE))//免疫等飘字
        {
            //			ShowBattleStatusEffect("mianyi", hudTransform);//事实上只要普通飘字就好，不需要图片，所以免了吧
        }
        return effectName;
    }

    private void AddStatusEffect(string effectName)
    {
        var hudTransform = GetMountDamageffect();
        BattleStatusEffectManager.Instance.AddEffect(hudTransform, effectName, this.GetId());
    }

    private void AddHPMPValue(string effName = "")
    {
        if (currentHP + modifyHP > MaxHP)
        {
            modifyHP = MaxHP - currentHP;
        }
        
        videoSoldier.hp = lastHP;
        videoSoldier.ep = lastEP;
        videoSoldier.cp = lastCP;
        
        dead = videoSoldier.hp <= 0;

        if (IsDead())   //http://oa.cilugame.com/redmine/issues/12278
                        //http://oa.cilugame.com/redmine/issues/12591
        {
            if (lastCP >= 0)
            {
                currentCp = lastCP;
            }
        }

        if (modifyHP != 0)
        {
            if (string.IsNullOrEmpty(effName))
            {
                AddFloatText("HP" + "," + modifyHP);
            }
            else
            {
                AddFloatText("CHP" + "," + modifyHP + "," + effName);
            }
        }

        if (modifyEP != 0)
        {
            AddFloatText("EP"+","+modifyEP);
        }

        ShowSCraftEff();
        
        PlayFloatText();
    }

    // 有S技需要显示特效
    private void ShowSCraftEff()
    {
        var show = _videoSoldier.CheckCPEnoughForSCraft();
        if (show)
        {
            BattleActionPlayer.Play_SCraft_Effect(this);
        }
        else
        {
            mBattleSkillEffectCMPT.RemoveEffectsByName("buff_eff_9999");
        }
    }

    private void AddFloatText(string info)
    {
        floatTextQueue.Enqueue(info);
    }

    private void PlayFloatText()
    {
        if (floatTextQueue.Count == 0)
        {
            return;
        }

        if (floatTextPlaying)
        {
            return;
        }

        var info = floatTextQueue.Dequeue();

        var infos = info.Split(',');
        var type = infos[0];
        var modifyValue = StringHelper.ToInt(infos[1]);

        var hudTransform = GetMountDamageffect();

        var effName = infos.Length > 2 ? infos[2] : string.Empty;

        if (type == "CHP")
        {
            BattleStatusEffectManager.Instance.PlayDamage(
                GetNumValue(modifyValue)
                , hudTransform
                , 1f
                , modifyValue > 0 ? 3 : 2
                , GetId()
                , 5f
                , effName);
        }

        else if (type == "HP")
            BattleStatusEffectManager.Instance.PlayDamage(
                GetNumValue(modifyValue)
                , hudTransform
                , 1f
                , modifyValue > 0 ? 1 : 0
                , GetId());
        
        floatTextPlaying = true;
        Invoke("DelayPlayFloatText", 0.6f);
    }

    private void DelayPlayFloatText()
    {
        floatTextPlaying = false;
        PlayFloatText();
    }

    private string GetNumValue(int num)
    {
        if (num > 0)
        {
            return "+" + num;
        }
        return Mathf.Abs(num).ToString();
    }

    private string GetRageValue(int num)
    {
        GameDebuger.Log("GetRageValue : " + num);
        if (num > 0)
        {
            return "+" + num;
        }
        return "-" + Mathf.Abs(num);
    }

    public void PlaySkillName(Skill skill, Action playSkillCallback)
    {
        if (skill == null) return;
        BattleStatusEffectManager.Instance.PlaySkillName(this.GetMountBattleEffect(), skill.name);

        var model = DataCache.getDtoByCls<Model>(_modelStyleInfo.defaultModelId);
        var skillSound = "";

        if (model != null)
        {
            skillSound = (Skill.SkillType)skill.skillAttackType == Skill.SkillType.Phy ? model.phySkillSound : model.magicSkillSound;
        }

        if (!string.IsNullOrEmpty(skillSound))
        {
            AudioManager.Instance.PlaySound(skillSound);
        }

        _playSkillCallback = playSkillCallback;
        Invoke("DelayPlaySkillNameCallback", 0.5f);
    }

    public void PlaySkillName(string name)
    {
        BattleStatusEffectManager.Instance.PlaySkillName(GetMountBattleEffect(), name);
    }

    private void DelayPlaySkillNameCallback()
    {
        GameUtil.SafeRun(_playSkillCallback);
        _playSkillCallback = null;
    }

    public Transform GetMountingPoint(string point)
    {
        return _modelDisplayer.GetMountingPoint(point);
    }

    public Transform GetMountHUD()
    {
        return GetMountingPoint(ModelHelper.Mount_hud);
    }

    public Transform GetMountFace()
    {
        return GetMountingPoint(ModelHelper.Mount_face);
    }

    public Transform GetMountHit()
    {
        return GetMountingPoint(ModelHelper.Mount_hit);
    }

    public Transform GetMountShadow()
    {
        return GetMountingPoint(ModelHelper.Mount_shadow);
    }

    public Transform GetMountBattleEffect()
    {
        var hudTransform = _mTrans.Find("floatTextGO");

        if (hudTransform != null) return hudTransform;
        hudTransform = GetMountHUD();

        if (hudTransform == null)
        {
            hudTransform = transform;
        }
        else
        {
            var floatTextGO = gameObject.AddChild();
            floatTextGO.name = "floatTextGO";
            var floatTrans = floatTextGO.transform;
            floatTrans.position = hudTransform.position;
            floatTrans.localPosition = new Vector3(0f, floatTrans.localPosition.y + 0.4f, 0f);
            hudTransform = floatTextGO.transform;
        }

        return hudTransform;
    }

    public Transform GetMountDamageffect()
    {
        var hudTransform = _mTrans.Find("damageTextGO");

        if (hudTransform != null) return hudTransform;
        hudTransform = GetMountHUD();

        if (hudTransform == null)
        {
            hudTransform = _mTrans;
        }
        else
        {
            var floatTextGO = gameObject.AddChild();
            var floatTrans = floatTextGO.transform;
            floatTextGO.name = "damageTextGO";
            floatTrans.position = hudTransform.position;
            hudTransform = floatTrans;
        }

        return hudTransform;
    }

    //获得固定的地面锚点
    public Transform GetBattleGroundMount()
    {
        var hudTransform = _mTrans.Find("groundMountGO");

        if (hudTransform != null) return hudTransform;
        hudTransform = GetMountShadow();

        if (hudTransform == null)
        {
            hudTransform = transform;
        }
        else
        {
            var go = gameObject.AddChild();
            go.name = "groundMountGO";
            go.transform.position = hudTransform.position;
            go.transform.rotation = hudTransform.rotation;
            hudTransform = go.transform;
        }

        return hudTransform;
    }

    private void DestroyBattleMount()
    {
        var hudTransform = _mTrans.Find("floatTextGO");
        if (hudTransform != null)
        {
            Destroy(hudTransform.gameObject);
        }

        hudTransform = _mTrans.Find("groundMountGO");
        if (hudTransform != null)
        {
            Destroy(hudTransform.gameObject);
        }
    }

    //----------------------------------------------------------------
    public void CancelEffect()
    {
        PlayIdleAnimation();
        CancelInvoke("PlayIdleAnimation");
    }


    public void TransparentEffectShader(float alpha = 0.4f)
    {
        //		if (IsDead())
        //			return;
        //		
        //		Shader shader = Shader.Find("Transparent/Diffuse");
        //		
        //		if (shader != null && petMaterial != null){
        //			petMaterial.shader = shader;
        //			petMaterial.SetColor("_Color", new Color(1, 1, 1, alpha));			
        //		}
    }

    public void ResetRotation()
    {
        _mTrans.eulerAngles = originRotation;
    }

    public void RotationToBack()
    {
        _mTrans.eulerAngles = new Vector3(originRotation.x, originRotation.y - 180, originRotation.z);
    }

    public void InstantiateGameObject(
        string effectName
        , Transform target
        , bool isOnGround = false
        , bool follow = false
        , float delayTime = 0f)
    {
        if (string.IsNullOrEmpty(effectName))
            return;

        if (target == null)
        {
            target = gameObject.transform;
        }

        if (target == null)
            return;

        ResourcePoolManager.Instance.SpawnEffectAsync(effectName, effGo =>
            {
                if (effGo == null)
                {
                    GameDebuger.Log("Instantiate Failed");
                    return;
                }

                if (target == null || target.gameObject == null)
                {
                    ResourcePoolManager.Instance.DespawnEffect(effGo);
                    return;
                }

                var t = effGo.transform;

                //跟随
                if (follow)
                {
                    GameObjectExt.AddPoolChild(target.gameObject, effGo);
                    var noRotation = effGo.GetMissingComponent<NoRotation>();

                    if (isOnGround)
                    {
                        noRotation.fixYToZero = true;
                    }
                }
                else
                {
                    GameObjectExt.AddPoolChild(LayerManager.Root.EffectsAnchor, effGo);
                    t.position = target.position;
                }

                if (isOnGround)
                    t.position = new Vector3(t.position.x, 0.02f, t.position.z);

                if (!(delayTime > 0f)) return;
                var effectTime = effGo.GetMissingComponent<EffectTime>();
                effectTime.time = delayTime;
                effectTime.OnFinish = OnEffectTimeFinish;

                //if (effectName == TargetSelectEffectPath)
                //{
                //    targetSelectEffect = effGo;
                //}
            });
    }

    private void OnEffectTimeFinish(EffectTime effectTime)
    {
        if (effectTime != null)
        {
            ResourcePoolManager.Instance.DespawnEffect(effectTime.gameObject);
        }
    }

    public void InstantiateGameObjectToParent(
        string effectName
        , Transform target
        , float x = 0.0f
        , float y = 0.0f
        ,float z = 0.0f)
    {
        if (string.IsNullOrEmpty(effectName))
            return;

        if (target == null)
            return;

        ResourcePoolManager.Instance.SpawnEffectAsync(effectName, go =>
            {
                if (go == null || HasDestroyEffect)
                {
                    GameDebuger.Log("Instantiate GameObject Failed");
                    return;
                }

                GameObjectExt.AddPoolChild(target.gameObject, go, x, y, z);
                go.transform.localPosition = new Vector3(0f, 0.01f, 0f);
                var shadow = go.GetMissingComponent<BattleShadow>();
                shadow.Setup(target);
            });
    }

    private void OnJoinBattleEffectFinish()
    {
        InitPetModel();
    }

    public void PlayTargetSelectEffect()
    {
        //		if (targetSelectEffect == null)
        //		{
        //			InstantiateGameObject(TargetSelectEffectPath, GetMountHit(), false, true);
        //		}
    }

    public void StopTargetSelectEffect()
    {
        //		if (targetSelectEffect != null)
        //		{
        //			ResourcePoolManager.Instance.Despawn(targetSelectEffect, ResourcePoolManager.PoolType.DESTROY_NO_REFERENCE);
        //			targetSelectEffect = null;
        //		}
    }

    public void PlayTargetClickEffect()
    {
        InstantiateGameObject(TargetClickEffectPath, GetMountShadow(), false, true, 2f);
    }

    public void PlayTargetOrderArrowEffect()
    {
        if (monsterOrderArrow == null)
        {
            monsterOrderArrow = BattleMonsterOrderArrow.CreateNew(this, DestroyMonsterOrderArrow);
        }
    }

    public void PlaySummonEffect()
    {
        InstantiateGameObject(SummonEffectPath, GetMountShadow(), false, false, 5f);
        AudioManager.Instance.PlaySound("sound_battle_summon");
    }

    private void OnStatusBarClick()
    {
        GameDebuger.TODO(@"BattleController.Instance.ShowMonsterStatus(this);");
    }

    private void ShowPetShoutContent()
    {
        CheckActionShout();

        GameDebuger.TODO(@"if (videoSoldier.monsterType == Monster.MonsterType_Baobao ||
            videoSoldier.monsterType == Monster.MonsterType_Mutate)
        {
            Pet pet = videoSoldier.monster.pet;
            if (string.IsNullOrEmpty(pet.shoutContent) == false)
            {
                Shout(pet.shoutContent);
            }
        }");
    }

    public void CheckActionShout()
    {
        if (ShoutDialogueId <= 0) return;
        GameDebuger.TODO(@"NpcDialog npcDialog = DataCache.getDtoByCls<NpcDialog>(ShoutDialogueId);
            if (npcDialog != null)
            {
                Shout(npcDialog.dialogContent[0]);
            }");

        ShoutDialogueId = 0;
    }

    public void DelayShout(string content, float delayTime = 0.1f)
    {
        GameDebuger.Log("DelayShout " + content);
        _delayShoutContent = content;
        Invoke("DoDelayShout", delayTime);
    }

    private void DoDelayShout()
    {
        if (string.IsNullOrEmpty(_delayShoutContent)) return;
        Shout(_delayShoutContent);
        _delayShoutContent = null;
    }

    public void Shout(string content)
    {
        Transform mountHUD = GetMountHUD();

        GameDebuger.TODO(@"if (mountHUD == null)
        {
            mountHUD = this.gameObject.transform;
            ProxyManager.ActorPopo.Open(GetId(), mountHUD, content, LayerManager.Root.BattleCamera, 2f, 3f);
        }
        else
        {
            ProxyManager.ActorPopo.Open(GetId(), mountHUD, content, LayerManager.Root.BattleCamera, 0f, 3f);
        }");
    }

    protected virtual void InitHudText()
    {
//        if (side == MonsterSide.Player)//S1怪物和英雄都需要血条等。
        {
            //初始化HPSlider
            if (hpSlider == null)
            {
                hpSlider = BattleMonsterHPSlider.CreateNew(this);
            }
            else
            {
                hpSlider.UpdateFollowTarget(this);
            }
        }

        if (monsterName == null)
        {
            monsterName = BattleMonsterName.CreateNew(this);
            GameDebuger.TODO(@"ShowMonsterName(BattleController.Instance.monsterNameShow, true);");
        }
        else
        {
            monsterName.UpdateFollowTarget(this);
        }

        if (_selectView == null)
        {
            _selectView = MonsterSelectView.CreateNew(this, OnMonsterSelect);
            HideSelectEffect();
        }
        else
        {
            _selectView.UpdateFollowTarget(this);
        }

        if (monsterPosition != null)
        {
            monsterPosition.UpdateFollowTarget(this);
        }

        if (monsterOrder != null)
        {
            monsterOrder.UpdateFollowTarget(this);
        }
    }

    public virtual bool IsPlayerMainCharactor()
    {
        return IsPlayerCtrlCharactor()
               && GetCharactorType() == GeneralCharactor.CharactorType.MainCharactor;
    }

    public bool IsPlayerPet()
    {
        return IsPlayerCtrlCharactor()
               && (GetCharactorType() == GeneralCharactor.CharactorType.Pet 
                   || GetCharactorType() == GeneralCharactor.CharactorType.Child);
    }

    public long GetId()
    {
        return videoSoldier.id;
    }

    //武将形象
    public int GetStyle()
    {
        GeneralCharactor charactor = videoSoldier.charactor;

        GameDebuger.TODO(@"Monster monster = videoSoldier.monster;
        if (monster != null)
        {
            return monster.texture;
        }");

        return charactor.texture;
    }

    //武将模型
    public int GetModel()
    {
        return _modelStyleInfo.defaultModelId;
        //		Monster monster = videoSoldier.monster;
        //		GeneralCharactor charactor = videoSoldier.charactor;
        //
        //		if (monster != null)
        //		{
        //			return monster.modelId;
        //		}
        //		else
        //		{
        //			return charactor.modelId;
        //		}
    }

    public int GetGrade()
    {
        return videoSoldier.grade;
    }

    public Faction GetFaction()
    {
        return videoSoldier.faction;
    }

    public List<int> GetSkillIds()
    {
        if (_skillIds == null)
        {
            _skillIds = DemoSimulateHelper.GetMainCharacterSkillIDList(videoSoldier);
//            _skillIds = DemoSimulateHelper.SimulateSkillList();
        }
        return _skillIds.ToList();
    }

    //包含夫妻技能
    public List<int> GetAllSkillIds()
    {
        if (_allSkillIds == null)
        {
            _allSkillIds = new List<int>();
            _allSkillIds.AddRange(GetSkillIds());

            GameDebuger.TODO(@"if (videoSoldier.fereId > 0)
            {
                List<int> list = ModelManager.FactionSkill.GetCoupleSkillIds();
                List<int> list = null;
                if (list != null)
                {
                    _allSkillIds.AddRange(list);
                }
            }");
        }
        return _allSkillIds;
    }


    //配偶是否在战斗中
    public bool IsCoupleAtBattle()
    {
        GameDebuger.TODO(@"if (videoSoldier.fereId > 0)
        {
            MonsterController mc = MonsterManager.Instance.GetMonsterFromSoldierID(videoSoldier.fereId);
            return mc != null;
        }");
        return false;
    }

    //获取配偶好友度
    public int GetFriendDegree()
    {
        GameDebuger.TODO(@"return ModelManager.Friend.GetFriendDegree(videoSoldier.fereId);");
        return 1;
    }

    public string GetName()
    {
        return videoSoldier.name;
    }

    public bool IsPet()
    {
        return GetCharactorType() == GeneralCharactor.CharactorType.Pet;
    }

    public bool IsMonster()
    {
        return GetCharactorType() == GeneralCharactor.CharactorType.Monster;
    }

    public bool IsMainCharactor()
    {
        return GetCharactorType() == GeneralCharactor.CharactorType.MainCharactor;
    }

    public bool IsCrew()
    {
        return GetCharactorType() == GeneralCharactor.CharactorType.Crew;
    }

    public GeneralCharactor.CharactorType GetCharactorType()
    {
        return (GeneralCharactor.CharactorType)videoSoldier.charactorType;
    }

    //是否主角
    public bool IsPlayerCtrlCharactor()
    {
        return (GetPlayerId() == ModelManager.Player.GetPlayerId());
    }

    //是否夫妻
    public bool IsMyCouple()
    {
        var hero = MonsterManager.Instance.GetMyHero();
        if (hero != null)
        {
            GameDebuger.TODO(@"return IsMainCharactor() && GetPlayerId() == hero.videoSoldier.fereId;");
            return false;
        }
        return false;
    }

    //是否玩家（非宠物和伙伴）
    public bool IsPlayer()
    {
        return GetCharactorType() == GeneralCharactor.CharactorType.MainCharactor;
    }

    public long GetPlayerId()
    {
        return videoSoldier.playerId;
    }

    public GeneralCharactor GetCharacter()
    {
        return videoSoldier.charactor;
    }

    public bool IsDead()
    {
        return dead;
    }

    private void InitBuffStateMaps()
    {
        buffStateMaps = new Dictionary<int, VideoBuffAddTargetState>();
    
        battleMonsterBuff = gameObject.AddComponent<BattleMonsterBuff>();
        battleMonsterBuff.SetMonster(this);
    }

    public void DestroyHPSlider()
    {
        if (hpSlider != null)
        {
            hpSlider.Destroy();
            hpSlider = null;
        }
    }

    public void DestroyMonsterName()
    {
        if (monsterName != null)
        {
            monsterName.Destroy();
            monsterName = null;
        }
    }

    public void DestroyMonsterOrder()
    {
        if (monsterOrder != null)
        {
            monsterOrder.Destroy();
            monsterOrder = null;
        }
    }

    public void DestroyMonsterOrderArrow()
    {
        if (monsterOrderArrow != null)
        {
            monsterOrderArrow.Destroy();
            monsterOrderArrow = null;
        }
    }

    public void DestroyMonsterPosition()
    {
        if (monsterPosition != null)
        {
            monsterPosition.Destroy();
            monsterPosition = null;
        }
    }
// todo
//    public void ShowMonsterName(bool show, bool atonce = false)
//    {
//        if (monsterName != null)
//        {
//            if (monsterNameShow != show)
//            {
//                monsterNameShow = show;
//                if (show)
//                {
//                    if (atonce)
//                    {
//                        UIHelper.PlayAlphaTween(monsterName.label, 1, 1);
//                    }
//                    else
//                    {
//                        UIHelper.PlayAlphaTween(monsterName.label, 0, 1);
//                    }
//                }
//                else
//                {
//                    if (atonce)
//                    {
//                        UIHelper.PlayAlphaTween(monsterName.label, 0, 0);
//                    }
//                    else
//                    {
//                        UIHelper.PlayAlphaTween(monsterName.label, 1, 0);
//                    }
//                }
//            }
//        }
//    }

    public void showOrder(string order)
    {
        if (string.IsNullOrEmpty(order))
        {
            DestroyMonsterOrder();
        }
        else
        {
            if (monsterOrder == null)
            {
                monsterOrder = BattleMonsterOrder.CreateNew(this);
            }
            monsterOrder.showOrder(order);
        }
    }

    public void ShowPosition(Formation formation)
    {
        var formationPosition = GetFormationPosition();

        if (formationPosition > 0 && formationPosition <= BattleConst.MaxTeamMemberCnt)
        {
            if (monsterPosition == null)
            {
                monsterPosition = BattleMonsterPosition.CreateNew(this);
            }

            monsterPosition.showPosition(formationPosition);
            monsterPosition.gameObject.SetActive(true);
        }
        else
        {
            HidePosition();
        }
    }

    private int GetFormationPosition()
    {
        return videoSoldier.position;
    }

    public void HidePosition()
    {
        if (monsterPosition != null)
        {
            monsterPosition.gameObject.SetActive(false);
        }
    }

    public string GetDebugInfo()
    {
        return "Instance ID:"+this.GetInstanceID()+"[" + videoSoldier.id + " " + videoSoldier.name + " " + currentHP + "/" + MaxHP + " " + currentEp + "/" +
        MaxEP + " " + videoSoldier.position + " " + GetCharactorType() + " " + IsDead() + "]";
    }

    #region SELECTOR

    public void SetNormalSkill()
    {
        battleTargetSelector.SetNormalSkill();
    }

    public void SetItemSkill(Skill skill, BagItemDto itemDto)
    {
        battleTargetSelector.SetItemSkill(skill, itemDto);
    }

    public void SetMagicOrCraftSkill()
    {
        battleTargetSelector.SetSkill();
    }

    public void SetMagicOrCraftSkill(int skillID,object pSkillAdditionParam = null)
    {
        var skill = DataCache.getDtoByCls<Skill>(skillID);
        SetMagicOrCraftSkill(skill, pSkillAdditionParam);
    }

    public void SetMagicOrCraftSkill(Skill skill,object pSkillAdditionParam = null)
    {
        battleTargetSelector.SetSkill(skill);
    }

    public BattleTargetSelector.TargetType GetTargetType()
    {
        return battleTargetSelector.getTargetType();
    }

    public void SetSkillTarget(long targets)
    {
        battleTargetSelector.SetTargets( targets);

        NeedReady = false;
        GameDebuger.TODO(@"UpdateSkillCD();");
    }

    public BattleTargetSelector GetBattleTargetSelector()
    {
        return battleTargetSelector;
    }

    public Skill GetDefaultMagicOrCraftSkill()
    {
        return battleTargetSelector == null ? null : battleTargetSelector.MagicOrCraftSkill;
    }

    public void ClearSkill()
    {
        if (battleTargetSelector != null)
            battleTargetSelector.ClearCurSkill();
    }

    public bool Set_S_Skill()
    {
        if (battleTargetSelector == null)
            return false;
        return battleTargetSelector.Set_S_Skill();
    }

    public Skill GetCurSelectSkill()
    {
        return battleTargetSelector == null ? null : battleTargetSelector.GetCurSkill();
    }

    private bool _showSelectEffect;
    protected MonsterSelectView _selectView;

    public void ShowSelectEffect(bool isCouple = false)
    {

        if (_selectView == null)
        {
            _selectView = MonsterSelectView.CreateNew(this, OnMonsterSelect);
        }
        if (_selectView != null)
        {
            _selectView.Show(true, isCouple);
        }
        _showSelectEffect = true;
    }

    protected void OnMonsterSelect()
    {
        if (selectController != null)
            selectController(this);
    }

    public void HideSelectEffect()
    {
        if (_selectView != null)
        {
            _selectView.Show(false);
        }
        _showSelectEffect = false;
    }

    public bool IsShowSelectEffect()
    {
        return _showSelectEffect;
    }

    public UIWidget GetSelectEffectUIWidget()
    {
        if (_selectView != null)
        {
            return _selectView._selectButton.sprite;
        }
        return null;
    }

    public void DestroyMonsterSelectView()
    {
        if (_selectView != null)
        {
            _selectView.Destroy();
            _selectView = null;
        }
    }

    #endregion

    #region Buff State

    public void AddBuffState(VideoBuffAddTargetState buff, bool tip = true)
    {
        if (buff.battleBuff.group == (int)SkillBuff.BuffGroup.SEAL)
        {
            BattleDataManager.DataMgr.DealCDForInterrupttedByDebuff(buff.id);
//            if (ServiceRequestAction.SimulateNet)//legacy 改版了，不论是否单机，中BUFF后都自行移除，2017-03-11 11:21:20
//            {
//                GameDebuger.LogError("[单机/非错误]单机时中封印BUFF即从队列移除");
//                BattleNetworkManager.Instance.HandlerActionQueueRemoveNotifyDto(DemoSimulateHelper.SimulateActionQueueRemoveNotifyDto(buff.id));
//            }
            BattleInstController.Instance.RemoveRound(buff.id);
//            BattleNetworkManager.Instance.HandlerActionQueueRemoveNotifyDto(ModelManager.BattleDemo.GameVideo.id, buff.id);
        }

        //只有己方才能看到抗药buff
        if (buff.battleBuffId == DataCache.GetStaticConfigValue(AppStaticConfigs.DRUG_RESISTANT_BUFF_ID) && side == BattlePosition.MonsterSide.Enemy)
            return;
    
        if (IsHideBuff(buff))
        {
            TransparentEffectShader(0.5f);
        }
    
        if (buffStateMaps.ContainsKey(buff.battleBuffId))
        {
            buffStateMaps[buff.battleBuffId] = buff;
            battleMonsterBuff.AddOrResetBuff(buff, tip);//S1跟H1不同，S1可以在中了某BUFF后，再中该BUFF，重置时间即可。
        }
        else
        {
            buffStateMaps.Add(buff.battleBuffId, buff);
            battleMonsterBuff.AddOrResetBuff(buff, tip);
        }

        AddBuffHandler(buff);
    }

    private void AddBuffHandler(VideoBuffAddTargetState buff)
    {
        if (buff == null) return;
        var skillBuff = DataCache.getDtoByCls<SkillBuff>(buff.battleBuffId);
        if (skillBuff != null)
            AddStatusEffect(skillBuff.showTipsEffect);
        GameEventCenter.SendEvent(GameEvent.BATTLE_FIGHT_BUFF_STATUS_CHANGED, GetId(), buff.battleBuff, true);
    }

    private void RemoveBuffHandler(VideoBuffAddTargetState buff)
    {
        GameEventCenter.SendEvent(GameEvent.BATTLE_FIGHT_BUFF_STATUS_CHANGED, GetId(), buff.battleBuff, false);
    }
    
    public void UpdateBuffState()
    {
        foreach (KeyValuePair<int, VideoBuffAddTargetState> keyVal in buffStateMaps)
        {
            var buff = keyVal.Value;
            
            //抗药点数
            if (buff.battleBuffId == DataCache.GetStaticConfigValue(AppStaticConfigs.DRUG_RESISTANT_BUFF_ID))
            {
                int num = 2 + (buff.effectValue / 7);
                buff.effectValue -= num;
            }
        }
    }

    public void RemoveBuffs(VideoBuffRemoveTargetState removeState)
    {
        for (int i = 0, len = removeState.buffId.Count; i < len; i++)
        {
            RemoveBuff(removeState.buffId[i]);
        }
    }

    public void RemoveBuff(int buffId)
    {
        if (!buffStateMaps.ContainsKey(buffId)) return;
        var buff = buffStateMaps[buffId];
        if (buff == null) return;
        if (IsHideBuff(buff))
        {
            _modelDisplayer.SetShadowActive(true);
        }
    
        buffStateMaps.Remove(buffId);
        battleMonsterBuff.RemoveBuff(buff);

        RemoveBuffHandler(buff);
        
    }

    public void RemoveAllBuff()
    {
        buffStateMaps.Clear();
        battleMonsterBuff.RemoveAllBuff();
    }

    //    是否隐身buff
    private bool IsHideBuff(VideoBuffAddTargetState buff)
    {
        GameDebuger.TODO(@"return buff.battleBuff.buffType == Buff.BattleBuffType_Hidden;");
        return false;
    }

    public bool ContainsBuff(int buffId)
    {
        return buffStateMaps.ContainsKey(buffId);
    }

    public int FindBuffByGroup(SkillBuff.BuffGroup pBuffGroup)
    {
        var tEnum = buffStateMaps.GetEnumerator();
        VideoBuffAddTargetState tVideoBuffAddTargetState;
        while (tEnum.MoveNext())
        {
            tVideoBuffAddTargetState = tEnum.Current.Value;
            if (tVideoBuffAddTargetState.battleBuff.group == (int)pBuffGroup)
                return tVideoBuffAddTargetState.battleBuffId;
        }
        return 0;
    }

    public List<VideoBuffAddTargetState> GetBuffs()
    {
        return buffStateMaps.Values.ToList();
    }

    public SkillBuff GetNeedUIEffectBuff()
    {
        GameDebuger.LogError("[TEMP]获取需要UI环绕特效的BUFF");
        int tBuffId = FindBuffByGroup(SkillBuff.BuffGroup.SEAL);
        if (tBuffId > 0)
            return DataCache.getDtoByCls<SkillBuff>(tBuffId);
        return null;
    }

    //是否可以选择
    public bool CanChoose(bool pShowTip = true)
    {
        if (FindBuffByGroup(SkillBuff.BuffGroup.SEAL) > 0)
        {
            if (pShowTip)
                TipManager.AddTip("正在封印状态，无法使用技能");
            return false;
        }
        
        return true;
    }

    public void UpdateSkillCD(float pDuration = 0, bool pPlayReverse = false, JSTimer.CdTask.OnCdFinish pOnCdFinish = null)
    {
        hpSlider.ShowCD(pDuration, pPlayReverse, pOnCdFinish);
    }

    #endregion

    #region

    public void UpdateMagicManaInRoundStart()
    {
        GameDebuger.TODO(@"if (magicMana < ModelManager.MagicEquipmentUpGrade.MaxMagicValue)
            magicMana += ModelManager.MagicEquipmentUpGrade.AddMagicValueInRoundStart;");
    }

    #endregion

    #region Base Action Node Player

    //---------------------------------Base Action Node Player---------------------------------------------------
    private MonsterController skillTarget;
    private int _modifySP;

    public void SetSkillTarget(MonsterController enemyTarget)
    {
        skillTarget = enemyTarget;
        if (skillTarget == null)
        {
            GameDebuger.LogError("SetSkillTarget is null");
        }
    }

    public MonsterController GetSkillTarget()
    {
        return skillTarget;
    }

    public void PlayMoveNode(MoveActionInfo node)
    {
        var go = new GameObject();
        var targetTransform = go.transform;

        var distance = node.distance;

        var needFinishTurn = false;

        if (node.center)
        {
            targetTransform.position = Vector3.zero;
            distance = 0f;
        }
        else
        {
            if (skillTarget != null)
            {
                targetTransform.position = skillTarget._mTrans.position;
                targetTransform.rotation = skillTarget._mTrans.rotation;
                //判断行动者跟目标是否是队友

                if (this.side.CompareTo(skillTarget.side) == 0)
                {
                    needFinishTurn = true;
                }
            }
            else
            {
                targetTransform.position = Vector3.zero;
                distance = 0f;
            }
        }

        ;
        MoveTo(targetTransform, distance, node.AnimationType, node.time, new Vector3(node.rotateX, node.rotateY, node.rotateZ), needFinishTurn);
        Destroy(go);
    }

    //-------------------------- Play Effect----------------------------------
    //    public void PlayTrailEffect(TrailEffectNode node)
    //    {
    //        Color color = ColorExt.HexToColor(node.color);
    //        color.a = node.alpha;
    //
    //        string matName = node.matName; // node.texture;
    //
    //        AddTrailEffect(color, node.delayTime, node.trailTime, node.mountTrailStart, node.mountTrailEnd,matName);
    //    }
    //
    //    public void PlayBodyShift(BodyShiftEffectNode node)
    //    {
    //		BodyShift(node.distance, node.time, node.aspect);
    //    }
    //
    //    public void PlayBodyHide(HideEffectNode node)
    //    {
    //        BodyHide(node.delayTime);
    //    }
    //
    //    public void PlayShadowMotionNode(GhostEffectNode node)
    //    {
    //        PlayShadowMotion(node.trailingDelayTime, node.scale, node.color,
    //                            node.alpha, node.blendMode, node.gapFrame, node.delayTime);
    //    }
    //
    //    public void PlayBodyInjure(ShowInjureEffectNode node)
    //    {
    //        PlayInjure(node.batterIndex);
    //    }
    //-------------------------------------------------------------------------

    public void PlayNormalNode(NormalActionInfo node)
    {
        PlayNormalActionInfo(EnumParserHelper.TryParse(node.name, ModelHelper.AnimType.battle), node.delayTime,node.mountFollow);
    }

    public void PlayMoveBackNode(MoveBackActionInfo node)
    {
        GoBack(node.time, new Vector3(node.rotateX, 180, node.rotateZ), node.AnimationType, true, 0, false);
    }

    #endregion

    #region 模型克隆体，虚影
    // S3 不需要实现残影
//    private ModelCopy mModelCopy;
//
//    private ModelCopy ModelCopy
//    {
//        get
//        {
//            if (null == mModelCopy)
//                mModelCopy = new ModelCopy(gameObject);
//            return mModelCopy;
//        }
//    }

    #endregion
    private void PlayHideEffect()
    {
        HideHPBar();
        if (null != monsterName)
            monsterName.SetActive(false);
        if (null != battleMonsterBuff)
            battleMonsterBuff.SetActive(false);
        if (null != _modelDisplayer)
            _modelDisplayer.HideEffect();
    }
    
    public void HideHPBar()
    {
        if (null != hpSlider)
            hpSlider.SetActive(false);
    }
    
    public void PlayHideEffect(float playTime)
    {
        _modelDisplayer.HideEffect();
        Invoke("DelayHideEffect", playTime);
    }

    private void DelayHideEffect()
    {
        _modelDisplayer.AppearEffect();
    }

    //重置状态，用于技能演示的重置等。
    virtual public void ResetMonsterStatus()
    {
        IsInCD = false;
        currentHP = _videoSoldier.hp;
        lastCP = _videoSoldier.hp;
        currentCp = lastCP;
        dead = false;

    }
    #region 操作状态管理器

    private MonsterOptionStateManager mMonsterOptionStateManager;

    public MonsterOptionStateManager MonsterOptionStateManager
    {
        get
        {
            if (null == mMonsterOptionStateManager)
                mMonsterOptionStateManager = new MonsterOptionStateManager(this);
            return mMonsterOptionStateManager;
        }
    }

    public Skill DefaultMagicOrCraft {
        get { return battleTargetSelector.MagicOrCraftSkill; }
    }

    #endregion

    public bool IsNormalAttack(int skillId)
    {
        return videoSoldier.GetNormallAtkSkillID() == skillId;
    }

    public void UpdateAttyByACReward(VideoActionTimeReward acReward)
    {
        currentHP = acReward.currentHp;
        currentCp = acReward.currentCp;
        currentEp = acReward.currentEp;

        videoSoldier.hp = currentHP;
        videoSoldier.cp = currentCp;
        videoSoldier.ep = currentEp;
    }

    public void PlayDrivingAnimation()
    {
        PlayAnimation(ModelHelper.AnimType.sing);
    }

    public void StopDrivingEffect()
    {
        var effname = PathHelper.GetEffectPath(GameEffectConst.GameEffectConstEnum.Effect_Sing);
        mBattleSkillEffectCMPT.RemoveEffectsByName(effname);
    }

    public int CurHp {
        get { return Mathf.Clamp(videoSoldier.hp, 0, videoSoldier.maxHp); }
    }

    public int MaxHP {
        get { return Mathf.Max(videoSoldier.maxHp, 0); }
    }
    //最大EP
    public int MaxEP{
        get { return Mathf.Max(videoSoldier.maxEp, 0); }
    }
    //最大CP
    public int MaxCP{
        get { return Mathf.Max(videoSoldier.maxCp, 0); }
    }

    public GameObject GetEffGameObject(string effName)
    {
        return mBattleSkillEffectCMPT.GetEffGameObject(effName);
    }

    public EffectTime CreateEffectTime(
        GameObject tEffectRoot
        , GameObject pParentContainer
        , Action<GameObject> effectDisposeCallback = null
        , bool createWhenNotExist = false)
    {

        if (tEffectRoot == null) return null;
        var effNameArr = tEffectRoot.name.Split('(');
        var effName = effNameArr[0];

        if (createWhenNotExist)
        {
            var eff = mBattleSkillEffectCMPT.GetEffTimeByName(effName);
            if (eff != null)
                return eff;
        }
        var tBattleSkillEffectInfo = mBattleSkillEffectCMPT.CreateBattleSkillEffectInfo(
            tEffectRoot
            , pParentContainer
            , effName
            , effectDisposeCallback);

        return tBattleSkillEffectInfo.MainEffectTime;
    }

    public void Rotate(Vector3 turnOffset)
    {
        _mTrans.eulerAngles += turnOffset;
    }
}