// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  PlayerView.cs
// Author   : willson
// Created  : 2014/12/2 
// Porpuse  : 
// **********************************************************************

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using AppDto;
using UniRx;
using Random = UnityEngine.Random;

public class PlayerView : MonoBehaviour
{
    //玩家显示数量
    public static Dictionary<long, PlayerView> LoadingModelDic = new Dictionary<long, PlayerView>();
    //记录当前待加载模型PlayerView的数量
    public static int PlayerVisibleCount;
    //坐骑显示数量
    public static int PlayerRideVisibleCount;

    protected bool _isRunning;
    protected bool _isVisual;
    //模型未加载完毕时,用于标记模型显示状态
    protected bool _isModelActive = true;

    protected PlayerMoveController _mAgent;
    protected GameObject _mGo;
    protected Transform _mTrans;
    protected CharacterController _characterController;

    //Character Model Member
    protected ModelStyleInfo _loadingStyleInfo;
    protected ModelDisplayer _modelDisplayer;

    protected ScenePlayerDto _playerDto;

    //PlayerHUD Name Member
    protected CharacterHeadHud headHud;
    protected CharacterTitleHud titleHud;

    protected Action _toTargetCallback;
    private IDisposable _disposable;

    public Action toTargetCallback
    {
        set { _toTargetCallback = value; }
    }
    public System.Action checkMissioinCallback { get; set; }

    //	国宝头顶特效	Begin
    private readonly string[] _escortEffectNames = { "", "game_eff_2021_Effect", "game_eff_2022_Effect" };
    private readonly string[] _escortSpriteNames = { "", "nornal", "advance" };
    //	private int _lastEscortShipmentType = Shipment.ShipmentType_Unknown;
    private Dictionary<int, OneShotUIEffect> _escortEffectDic = new Dictionary<int, OneShotUIEffect>();
    //	国宝头顶特效	End

    public void SetupPlayerDto(ScenePlayerDto dto, bool isHero)
    {
        _playerDto = dto;
        IsHero = isHero;

        if (_mGo == null)
            _mGo = gameObject;
        if (_mTrans == null)
            _mTrans = transform;
        if (_characterController == null)
            _characterController = _mGo.GetComponent<CharacterController>();
        _characterController = null;
        GameDebuger.TODO(@"ChangeSize(dto.scale, dto.scaleExpireAt);");
        _isModelActive = true;

        if (_modelDisplayer == null)
            _modelDisplayer = new ModelDisplayer(_mGo, OnLoadModelFinish, occlusitionActive:isHero);

        //set NavMeshAgent
        if (_mAgent == null)
        {
            // 设置行走控制器
            if (_mAgent == null)
            {
                _mAgent = new PlayerMoveController();
            }
            _mAgent.Setup(this, isHero, _mGo, _mGo, _mTrans);
        }

        //设置PlayerView坐标
        var pos = SceneHelper.GetSceneStandPosition(new Vector3(_playerDto.x, 0f, _playerDto.z), Vector3.zero);
        
        ResetPos(pos);

        if (IsHero)
        {
            GameDebuger.TODO(@"GameEventCenter.AddListener(GameEvent.Player_OnPlayerNicknameUpdate, UpdatePlayerNickName);
            GameEventCenter.AddListener(GameEvent.Player_OnPlayerGradeUpdate, OnHeroGradeUpdate);
            GameEventCenter.AddListener(GameEvent.Backpack_OnWeaponModelChange, UpdateWeapon);
            GameEventCenter.AddListener(GameEvent.Backpack_OnHallowSpriteChange, UpdateHallowSprite);");
            //这里不需要如是更新，会走WorldView那里更新，那样更新才能让场景中所有的玩家看到效果。 see:http://oa.cilugame.com/redmine/issues/11069
            //          GameEventCenter.AddListener(GameEvent.Mount_OnPlayerRideDtoUpdate, UpdatePlayerRideDto);
            //          GameEventCenter.AddListener(GameEvent.Mount_OnRidingMountStatusChange, UpdatePlayerRideDto);
            GameEventCenter.AddListener(GameEvent.Team_OnTeamStateUpdate, HandleOnTeamStateUpdate);
            InitModel();
        }
        else
        {
            var checker = _mGo.GetMissingComponent<ModelVisibleChecker>();
            checker.Setup(OnVisible, OnInvisible, 2f);
        }
    }
    public void UpdatePlayerDto(ScenePlayerDto playerDto)
    {

        _playerDto = playerDto;
        //_playerDto.rideId = ModelManager.Player.tempRideId;
    }

    private void OnHeroGradeUpdate()
    {
        var effpath = PathHelper.GetEffectPath(GameEffectConst.GameEffectConstEnum.Effect_PlayerUpgrade);
        OneShotSceneEffect.BeginFollowEffect(effpath, transform, 3f, 1f);

        AudioManager.Instance.PlaySound("sound_UI_upgrade");
    }

    // 寻路间隔帧数
    private const int _followDeltaTime = 2;
    private int _teamMemberFollowTimer = 2;

    protected virtual void Update()
    {
        if (leaderView != null)
        {
            Vector3 leaderPos = leaderView.cachedTransform.position;
            Vector3 mPos = cachedTransform.position;
            float magnitude = (leaderPos - mPos).magnitude;
            float tempStopSpace = teamMemberSpace;
            Vector3 dest;
            
            if (Mathf.Abs(magnitude - tempStopSpace) <= 0.1f || CheckTargetDistance(tempStopSpace, 0.1f, out dest))
            {
                _teamMemberFollowTimer = _followDeltaTime;
                if ( _mAgent != null)
                {
                    _mAgent.ResetPath();
                }
                PlayIdleAnimation();
            }
            else
            {
                if (_mAgent.IsActiveAndEnabled())
                {
                    _teamMemberFollowTimer++;
                    if (_teamMemberFollowTimer > _followDeltaTime)
                    {
                        _mAgent.SetDestination(dest);
                        _teamMemberFollowTimer = 0;
                    }

                    PlayRunAnimation();
                }
                else
                {
                    ResetPos(dest);
                    PlayIdleAnimation();
                }
            }
        }
        else
        {
            if (_playerDto.teamStatus == (int)TeamMemberDto.TeamMemberStatus.Member)
            {
                if (_mAgent != null)
                {
//                    _mAgent.autoBraking = true;
//                    _mAgent.stoppingDistance = 0f;
                    if (_mAgent.IsActiveAndEnabled())
                    {
                        _mAgent.ResetPath();
                    }
                }
                PlayIdleAnimation();
            }
            else
            {
                if (!_walkWithJoystick)
                {
                    UpdatePlayerAnimation();
                }

                //  任务判断\......判断角色坐标回调
                if (checkMissioinCallback != null)
                {
                    checkMissioinCallback();
                }
            }
        }

//        if (_playerDto.scaleExpireAt > 0 && SystemTimeManager.Instance.GetUTCTimeStamp() > _playerDto.scaleExpireAt)
//        {
//            _playerDto.scaleExpireAt = 0;
//            ChangeSize(1);
//        }

    }

    void LateUpdate()
    {
        if(_modelDisplayer != null)
            _modelDisplayer.LateUpdate();
    }

    private bool CheckTargetDistance(float tempStopSpace, float fade, out Vector3 target)
    {
        if (leaderView._mAgent != null)
        {
            var pos = leaderView.cachedTransform.position;
            target = pos - _playerDto.teamIndex * leaderView.cachedTransform.forward * (_mAgent.radius + 1f);     //间距
            //target = SceneHelper.GetSceneStandPosition(target, pos);
            return Vector3.Distance(target, _mAgent.GetGridPos()) < fade;
        }
        target = Vector3.zero;
        return false;
    }
    
    private void OnVisible()
    {
        if (_isVisual)
            return;

        if (PlayerVisibleCount + 1 <= GameDisplayManager.MaxPlayerVisibleCount)
        {
            InitModel();
        }
    }

    private void OnInvisible()
    {
        if (!_isVisual)
            return;

        CleanUpModel();
        CleanUpHUDView();
    }

    private void OnEnable()
    {
        if (_modelDisplayer != null)
        {
            _modelDisplayer.OnEnable();
        }
    }

    private void OnDisable()
    {
        if (_modelDisplayer != null)
        {
            _modelDisplayer.OnDisable();
        }
    }

    public virtual void DestroyMe()
    {
        CancelInvoke("DelayStopNavFlag");
        ClearTeamLeader();
        if (IsHero)
        {
            GameDebuger.TODO(@"GameEventCenter.RemoveListener(GameEvent.Backpack_OnWeaponModelChange, UpdateWeapon);
            GameEventCenter.RemoveListener(GameEvent.Player_OnPlayerNicknameUpdate, UpdatePlayerNickName);
            GameEventCenter.RemoveListener(GameEvent.Player_OnPlayerGradeUpdate, OnHeroGradeUpdate);
            GameEventCenter.RemoveListener(GameEvent.Backpack_OnHallowSpriteChange, UpdateHallowSprite);
            //这里不需要如是更新，会走WorldView那里更新，那样更新才能让场景中所有的玩家看到效果。 see:http://oa.cilugame.com/redmine/issues/11069
            //          GameEventCenter.RemoveListener(GameEvent.Mount_OnPlayerRideDtoUpdate, UpdatePlayerRideDto);
            //          GameEventCenter.RemoveListener(GameEvent.Mount_OnRidingMountStatusChange, UpdatePlayerRideDto);

            GameEventCenter.RemoveListener(GameEvent.Team_OnTeamStateUpdate, HandleOnTeamStateUpdate);");
        }

        CleanUpHUDView();
        CleanUpModel();
        ResetPos(Vector3.zero);
        CleanEscortEff();

        _playerDto = null;
        WorldManager.Instance.GetView().DespawnPlayerView(this);
    }

    /// <summary>
    /// 需要在NavMeshAgent禁用状态下设置好位置,否则会导致异常
    /// </summary>
    /// <param name="dest"></param>
    public void ResetPos(Vector3 dest)
    {
        _mAgent.ResetPath();
        _mAgent.ResetPos(dest);
    }

    private void CleanUpHUDView()
    {
        if (titleHud != null)
        {
            titleHud.Despawn();
            titleHud = null;
        }

        if (headHud != null)
        {
            headHud.Despawn();
            headHud = null;
            //            _disposable.Dispose();
        }
    }

    private void CleanEscortEff()
    {
        GameDebuger.TODO(@"_lastEscortShipmentType = Shipment.ShipmentType_Unknown;");

        List<OneShotUIEffect> escortList = new List<OneShotUIEffect>(_escortEffectDic.Values);

        for (int i = escortList.Count - 1; i >= 0; i--)
        {
            escortList[i].Dispose();
            escortList[i] = null;
        }
        _escortEffectDic.Clear();
    }

    //float PathLength(NavMeshPath path)
    //{
    //    if (path.corners.Length < 2)
    //        return 0;

    //    Vector3 previousCorner = path.corners[0];
    //    float lengthSoFar = 0.0F;
    //    int i = 1;
    //    while (i < path.corners.Length)
    //    {
    //        Vector3 currentCorner = path.corners[i];
    //        lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
    //        previousCorner = currentCorner;
    //        i++;
    //    }
    //    return lengthSoFar;
    //}

    #region Navigation

    public void ChangeToPoint(Vector3 targetPoint)
    {
        if (_mAgent == null)
            return;

        ResetPos(targetPoint);
        _walkWithJoystick = false;

        //有跟随的队员，重置队员位置
        if (inTeamPlayerList.Count > 0)
        {
            for (int i = 0; i < inTeamPlayerList.Count; ++i)
            {
                var memberView = inTeamPlayerList[i];
                var dest = _mTrans.position -
                                       memberView._playerDto.teamIndex * _mTrans.forward * (_mAgent.radius + 1f);     //间距
                memberView.ChangeToPoint(dest);
            }
        }
    }

    public Vector3[] GetWalkPathList()
    {
        return _mAgent.GetWalkPathList();
    }

    public Vector3 GetNavDestination()
    {
        return _mAgent.GetNavDestination();
    }

    public void StopAndIdle()
    {
        if (IsHero)
        {
            if (_isRunning)
            {
                WorldManager.Instance.PlanWalk(_mTrans.position.x, _mTrans.position.z);
                WorldManager.Instance.GetNpcViewManager().ResetWaitingTrigger();
            }
        }

        _toTargetCallback = null;
        _walkWithJoystick = false;
        PlayIdleAnimation();
        if (_mGo != null && _mGo.activeInHierarchy)
        {
            if (_mAgent != null && _mAgent.IsActiveAndEnabled() && _mAgent.InMoving())
            {
                _mAgent.ResetPath();
            }
        }
    }

    private void DelayStopNavFlag()
    {
        if (_isRunning)
        {
            return;
        }

        if (!ModelManager.Player.IsAutoFram)
        {
            SetNavFlag(false);
        }
    }

    protected bool _walkWithJoystick;

    public void DragBeginToWalk()
    {
        StopAndIdle();
        // 停止挂机
        ModelManager.Player.StopAutoFram();
        SetNavFlag(false);
        _walkWithJoystick = true;
    }

    public void WalkToPoint(Vector3 targetPoint, Action toTargetCallback = null, bool randomRange = false)
    {
        _toTargetCallback = toTargetCallback;

        if (_mAgent == null)
            return;

        if (IsHero)
        {
            if (randomRange)
            {
                WorldManager.Instance.PlanWalk(Random.Range(targetPoint.x - 1f, targetPoint.x + 1f),
                    Random.Range(targetPoint.z - 1f, targetPoint.z + 1f));
            }
            else
            {
                WorldManager.Instance.PlanWalk(targetPoint.x, targetPoint.z);
            }
            // 停止挂机状态
            ModelManager.Player.StopAutoFram();
            WorldManager.Instance.GetNpcViewManager().ResetWaitingTrigger();
        }

        //玩家不可见或禁用状态下，直接设置其坐标
        if (!_mGo.activeInHierarchy || !_mAgent.IsActiveAndEnabled())
        {
            ResetPos(targetPoint);
            _isRunning = false;
        }
        else
        {
            _mAgent.SetDestination(targetPoint);
        }

        _walkWithJoystick = false;
    }

    #endregion

    #region Team Relative

    private PlayerView leaderView;
    public List<PlayerView> inTeamPlayerList = new List<PlayerView>(5);

    public void UpdateTeamStatus()
    {
        if (_playerDto.teamStatus == (int)TeamMemberDto.TeamMemberStatus.Leader)
        {
            SetTeamLeaderFlag(true);
            leaderView = null;
//            _mAgent.autoBraking = true;
            //  帮战竞赛场景中队伍信息
            GameDebuger.TODO(@"SetTeamInfoFlag(ModelManager.GuildCompetitionData.IsInActivityScene());");

        }
        else
        {
            SetTeamLeaderFlag(false);
            //变更队长状态时，清空队员列表
            inTeamPlayerList.Clear();
            SetNavFlag(false);
            StopAndIdle();
        }

        if (_playerDto.teamStatus == (int)TeamMemberDto.TeamMemberStatus.Member)
        {
            var leaderDto = WorldManager.Instance.GetModel().GetTeamLeader(_playerDto.teamId);
            if (leaderDto != null)
            {
                var leaderView = WorldManager.Instance.GetView().GetPlayerView(leaderDto.id);
                if (leaderView != null)
                {
                    SetupTeamLeader(leaderView);
                }
                else
                {
                    if (IsHero)
                    {
                        Debug.LogError("场景中找不到队长View");
                    }
                }
            }
            else
            {
                if (IsHero)
                {
                    Debug.LogError("场景中找不到队长Dto");
                }
            }
        }
        else
        {
            ClearTeamLeader();
        }
    }

    private void SetTeamLeaderFlag(bool active)
    {
        if (headHud != null)
        {
            headHud.headHUDView.teamFlagSpriteAnimation.SetEnable(active);
        }
    }

    private void SetupTeamLeader(PlayerView newLeader)
    {
        if (leaderView != newLeader)
        {
            leaderView = newLeader;
//            _mAgent.autoBraking = false;
//            _mAgent.stoppingDistance = 0.2f;
            leaderView.AddTeamMember(this);
        }

        if (leaderView != null)
        {
            _mAgent.UpdatePlayerMoveSpeed(newLeader.GetPlayerDto().moveSpeed);
            var dest = leaderView.cachedTransform.position - _playerDto.teamIndex * leaderView.cachedTransform.forward * (_mAgent.radius + 0.5f);
            ResetPos(dest);
            _mTrans.rotation = leaderView.cachedTransform.rotation;
            SetNavFlag(false);
        }

        StopAndIdle();

        //如果是队员，就调用停止巡逻的接口，防止队员巡逻同时又进队加入队长遇怪
        if (_playerDto.id == ModelManager.Player.GetPlayerId())
        {
            ModelManager.Player.StopAutoFram(false);
        }
    }

    private void ClearTeamLeader()
    {
        _mAgent.UpdatePlayerMoveSpeed(_playerDto.moveSpeed);
        if (leaderView != null)
        {
            leaderView.RemoveTeamMember(this);
            leaderView = null;

//            _mAgent.autoBraking = true;
//            _mAgent.stoppingDistance = 0f;
            StopAndIdle();
        }
    }

    private void AddTeamMember(PlayerView playerView)
    {
        if (!inTeamPlayerList.Contains(playerView))
        {
            inTeamPlayerList.Add(playerView);
        }
    }

    private void RemoveTeamMember(PlayerView playerView)
    {
        inTeamPlayerList.Remove(playerView);
    }

    // 获得队员和队长之间的固定间隔
    private int _teamIndex = -1;
    private float _space;
    private float _stopSpace;
    private DetailRideStatus _rideStatus = DetailRideStatus.NoneRide;
    private float teamMemberSpace
    {
        get
        {
            DetailRideStatus rideStatus = GetDetailRideStatus(true);
            if (_teamIndex != _playerDto.teamIndex || rideStatus != _rideStatus)
            {
                _rideStatus = rideStatus;
                _teamIndex = _playerDto.teamIndex;
                float b = _rideStatus == DetailRideStatus.FlyHighMount ? 2f : 0.5f;
                _space = 0.1f * (_teamIndex - 1) + b * _teamIndex;
                _stopSpace = 1.5f * _space;
            }
            return _space;
        }
    }
    
    private float stopSpace
    {
        get { return _stopSpace; }
    }
    #endregion

    #region Getter

    public GameObject cachedGameObject
    {
        get
        {
            if (_mGo == null)
                _mGo = gameObject;
            return _mGo;
        }
    }

    public Transform cachedTransform
    {
        get
        {
            if (_mTrans == null)
                _mTrans = transform;
            return _mTrans;
        }
    }

    public float Speed
    {
        get { return _mAgent.GetSpeed(); }
    }

    public CharacterController CharacterController
    {
        get { return _characterController; }
    }

    public ScenePlayerDto GetPlayerDto()
    {
        return _playerDto;
    }

    public bool IsHero { get; private set; }

    public bool IsVisual()
    {
        return _isVisual;
    }

    #endregion

    #region Model

    private void InitModel()
    {
        PlayerVisibleCount++;
        _isVisual = true;

        if (_modelDisplayer != null)
        {
            var lookInfo = ModelStyleInfo.ToInfo(_playerDto);

            if (!IsHero)
            {
                if (lookInfo.HasRide)
                {
                    if (PlayerRideVisibleCount + 1 > GameDisplayManager.MaxRideVisibleCount)
                    {
                        lookInfo.rideId = 0;
                    }
                    else
                    {
                        PlayerRideVisibleCount++;
                    }

                }
            }

            if (IsHero)
            {
                _modelDisplayer.SetLookInfo(lookInfo);
            }
            else
            {
                //遇到其他玩家时,延迟初始化模型,防止一帧内加载过多模型导致卡顿
                LoadingModelDic.Add(_playerDto.id, this);
                _loadingStyleInfo = lookInfo;
                StopAllCoroutines();
                StartCoroutine(DelayInitModel());
            }
        }
    }
    const int LoadCount = 1;    //每帧加载数量,如果要间隔几帧加载 则改成float用小数 再取整
    private IEnumerator DelayInitModel()
    {
        int waitFrame = Time.frameCount + (LoadingModelDic.Count - 1) / LoadCount;    //计算目标加载帧 
        while (waitFrame >= Time.frameCount)
        {
            yield return null;
        }
        //Debug.LogError("DelayInitModel:" + _playerDto.nickname + "|" + Time.time);
        if (_playerDto != null)
        {
            _modelDisplayer.SetLookInfo(_loadingStyleInfo);
            LoadingModelDic.Remove(_playerDto.id);
        }
        _loadingStyleInfo = null;
    }

    private void CleanUpModel()
    {
        if (_isVisual)
            PlayerVisibleCount--;

        _isVisual = false;
        _isRunning = false;

        if (_modelDisplayer != null)
        {
            if (!IsHero)
            {
                if (_modelDisplayer.modelStyleInfo != null)
                {
                    if (_modelDisplayer.modelStyleInfo.HasRide)
                    {
                        PlayerRideVisibleCount--;
                    }
                }
            }
            _modelDisplayer.Destory();
            if (LoadingModelDic.ContainsKey(_playerDto.id))
            {
                LoadingModelDic.Remove(_playerDto.id);
                _loadingStyleInfo = null;
                StopAllCoroutines();
                //Debug.LogError("CleanUpModel:" + _playerDto.nickname + "|" + Time.time);				
            }
        }
    }

    private bool loadFinish = false;

    private void OnLoadModelFinish()
    {
        InitPlayerName();
        InitPlayerAnimation();
        loadFinish = true;

        if (IsHero)
        {
            //if()
            //{

            //}
            // 设置自动挂机
            SetPatrolFlag(ModelManager.Player.IsAutoFram);
        }

        //游花轿和结婚剧情时,隐藏对应玩家模型
        GameDebuger.TODO(@"if (ModelManager.Marry.IsIMarry (_playerDto.id) || ModelManager.BridalSedan.IsSedanPlayer (_playerDto.id)) {
            SetUnitActive (false);
        } else");
        {
            //模型加载完毕,还原模型显示状态
            SetUnitActive(_isModelActive);
        }
    }

    public void UpdateWeapon(int model)
    {
        if (_modelDisplayer != null)
        {
            _modelDisplayer.UpdateWeapon(model);
        }
    }

    public void UpdateWeaponEff(int weaponEffId)
    {
        if (_modelDisplayer != null)
        {
            _modelDisplayer.UpdateWeaponEff(weaponEffId);
        }
    }

    public void UpdateHallowSprite(int model)
    {
        if (_modelDisplayer != null)
        {
            _modelDisplayer.UpdateHallowSprite(model);
        }
    }

    private void HandleOnTeamStateUpdate()
    {
        //	帮战场景中刷新
        GameDebuger.TODO(@"SetTeamInfoFlag(ModelManager.GuildCompetitionData.IsInActivityScene());");
    }

    /// <summary>
    /// 本意是更新本模型上的坐骑。
    /// 但是这里不需要如是更新，需要走WorldView那里的更新，那样的更新才能让场景中所有的玩家看到效果。
    /// <see cref="http://oa.cilugame.com/redmine/issues/11069"/>
    /// </summary>
    public void UpdatePlayerRideDto()
    {
        if (null != _modelDisplayer)
        {
            GameDebuger.TODO(@"PlayerRideDto tPlayerRideDto = ModelManager.Mount.GetPlayerRideDto ();
            if (null != tPlayerRideDto) {
                _modelDisplayer.UpdateRide (tPlayerRideDto.mountId);
            }");

        }
    }

    public void UpdateRide(int rideId)
    {
        //_playerDto.rideId = rideId;
        if (_modelDisplayer != null)
        {
            _modelDisplayer.UpdateRide(rideId);
        }
    }

    public void UpdateModelHSV()
    {
        if (_modelDisplayer != null)
        {
            GameDebuger.TODO(@"_modelDisplayer.UpdateModelHSV (PlayerModel.GetDyeColorParams (_playerDto.dressInfoDto), 0);");
            _modelDisplayer.UpdateModelHSV("", 0);
        }
    }

    public void ChangeSize(float scale, long expireAt = 0)
    {
        if (scale <= 0f)
        {
            scale = 1f;
        }
        GameDebuger.TODO(@"_playerDto.scale = scale;
        _playerDto.scaleExpireAt = expireAt;");
        _mTrans.localScale = new Vector3(scale, scale, scale);
    }

    public void ChangeView(int modelId)
    {
        _playerDto.charactor.modelId = modelId;
        UpdateModel();
    }

    public void ChangeWeaponView(int weaponId)
    {
        var lookInfo = ModelStyleInfo.ToInfo(_playerDto);
        lookInfo.weaponId = weaponId;
        UpdateModel(lookInfo);
    }

    public void UpdateModel()
    {
        var lookInfo = ModelStyleInfo.ToInfo(_playerDto);
        UpdateModel(lookInfo);
    }

    public void UpdateModel(ModelStyleInfo lookInfo)
    {
        if (_modelDisplayer == null) return;

        if (!IsHero)
        {
            if (_modelDisplayer.modelStyleInfo != null)
            {
                if (_modelDisplayer.modelStyleInfo.HasRide)
                {
                    PlayerRideVisibleCount--;
                }
            }

            if (lookInfo.HasRide)
            {
                if (PlayerRideVisibleCount + 1 > GameDisplayManager.MaxRideVisibleCount)
                {
                    lookInfo.rideId = 0;
                }
                else
                {
                    PlayerRideVisibleCount++;
                }
            }
        }

        //当前模型还处于延迟加载中时,直接更新ModelStyleInfo信息
        if (LoadingModelDic.ContainsKey(_playerDto.id))
        {
            _loadingStyleInfo = lookInfo;
        }
        else
        {
            _modelDisplayer.SetLookInfo(lookInfo);
        }
    }

    public void UpdatePlayerMoveSpeed(float moveSpeed)
    {
        if (_mAgent != null)
        {
            _mAgent.UpdatePlayerMoveSpeed(moveSpeed);
        }
    }

    /// <summary>
    /// 用这种方式控制顶层的gameobject的显隐，可以触发OnEnable等，刷新骑乘状态，避免人物掉下坐骑等BUG。
    /// @MarsZ 2016-07-05 15:07:37
    /// </summary>
    public void SetUnitActive(bool active)
    {
        SetHUDActive(active);
        if (cachedGameObject != null)
            cachedGameObject.SetActive(active);
    }

    public void SetModelActive(bool active)
    {
        _isModelActive = active;
        if (_modelDisplayer != null)
            _modelDisplayer.SetActive(active);

        SetHUDActive(active);
    }

    public void ShowKnot(string bridegroomName, string brideName)
    {
        var mountHUD = _modelDisplayer.GetMountingPoint(ModelHelper.Mount_hud);
        string path = "game_eff_tongxinsuo";
        GameDebuger.TODO(@"KnotEffController.BeginFollowEffect (path, bridegroomName, brideName, mountHUD, 9);");
    }

    #endregion

    public float GetDistanceFromOther(Vector3 pTrans)
    {
        if (_mTrans == null)
            return 0f;

        Vector2 tPos1 = new Vector2(_mTrans.localPosition.x, _mTrans.localPosition.y);
        Vector2 tPos2 = new Vector2(pTrans.x, pTrans.y);

        return Vector2.Distance(tPos1, tPos2);
    }

    #region PlayerHUD

    private void UpdatePlayerNickName(string nickname)
    {
        _playerDto.name = nickname;
        UpdatePlayerName();
    }

    public void UpdatePlayerName()
    {
        if (titleHud != null)
        {
            string colorStr = ColorConstant.Color_Name_Str;
            GameDebuger.TODO(@"if (ModelManager.GuildCompetitionData.IsInActivityScene())
            {
                colorStr = ModelManager.GuildCompetitionData.GetNameColorStr(_playerDto, colorStr);
            }
            else if (ModelManager.CampWarData.IsInActivityScene())
            {
                colorStr = ModelManager.CampWarData.GetNameColorStr(_playerDto, colorStr);
            }");
            string nickName = _playerDto.name.WrapColor(colorStr);
            GameDebuger.TODO(@"if (ModelManager.CampWarData.IsInActivityScene())
            {
                string title = ModelManager.CampWarData.GetCampWarTitleName(_playerDto);
                nickName = title.WrapColor(ColorConstant.Color_Title_Str) + '\n' + nickName;
            }else if (ModelManager.CSPK.IsInCSPKSceneAsWatcher(_playerDto))
            {
            string title = CSPKModel.WATCHER_TITLE;
            nickName = title.WrapColor(ColorConstant.Color_Title_Str) + '\n' + nickName;
            }
            else
            {
                if (_playerDto.titleId != 0)
                {
                    _playerDto.titleName = _playerDto.titleName
                        .Replace('{factionName}', _playerDto.faction.shortName);
                        .Replace('{fereName}', _playerDto.fereName)
                        .Replace('{masterName}', _playerDto.masterName);
                    nickName = _playerDto.titleName.WrapColor(ColorConstant.Color_Title_Str) + '\n' + nickName;
                }
            }");
            titleHud.titleHUDView.nameLbl.text = nickName;
        }
    }

    public void UpdatePlayerFereName(string fereName)
    {
        GameDebuger.TODO(@"_playerDto.fereName = fereName;
        UpdatePlayerName ();");
    }

    private void InitPlayerName()
    {
        var mountShadow = _modelDisplayer.GetMountingPoint(ModelHelper.Mount_shadow);
        if (mountShadow != null)
        {
            if (titleHud == null)
                titleHud = new CharacterTitleHud(mountShadow, new Vector3(0f, -0.5f, 0f), "PlayerTitleHUD_" + _playerDto.id);
            else
                titleHud.ResetHudFollower(mountShadow, new Vector3(0f, -0.5f, 0f), "PlayerTitleHUD_" + _playerDto.id);

            if (_playerDto.name == "")
            {
                _playerDto.name = _playerDto.id.ToString();
            }
            UpdatePlayerName();
        }

        var mountHUD = _modelDisplayer.GetMountingPoint(ModelHelper.Mount_hud);
        if (mountHUD != null)
        {
            if (headHud == null)
            {
                headHud = new CharacterHeadHud(mountHUD, Vector3.zero, "PlayerHeadHUD_" + _playerDto.id);
                headHud.headHUDView.teamInfo_UISprite.enabled = false;
                headHud.headHUDView.teamCount_UILabel.enabled = false;
                headHud.headHUDView.escortFlag_UISprite.enabled = false;
                //                _disposable = TeamDataMgr.Stream.SubscribeAndFire(SetTeamInfoFlag);
            }

            else
                headHud.ResetHudFollower(mountHUD, Vector3.zero, "PlayerHeadHUD_" + _playerDto.id);

            SetTeamLeaderFlag(_playerDto.teamStatus == (int)TeamMemberDto.TeamMemberStatus.Leader);
            SetFightFlag(WorldManager.Instance.GetModel().GetPlayerBattleStatus(_playerDto.id));

            ////    帮战场景中刷新
            GameDebuger.TODO(@"SetTeamInfoFlag(ModelManager.GuildCompetitionData.IsInActivityScene());
            ////    国宝场景中刷新
            SetEscortFlag(ModelManager.Escort.IsInActivityScene());");

            headHud.headHUDView.runFlagSpriteAnimation.SetEnable(false);
            headHud.headHUDView.missionTypeSprite.enabled = false;
        }
    }

    public void Shout(string content)
    {
        var mountHUD = _modelDisplayer.GetMountingPoint(ModelHelper.Mount_hud);
        GameDebuger.TODO(@"ProxyManager.ActorPopo.Open(_playerDto.id, mountHUD, content, LayerManager.Root.SceneCamera);");
    }

    public void SetHUDActive(bool active)
    {
        if (headHud != null)
        {
            headHud.SetHeadHudActive(active);
        }

        if (titleHud != null)
        {
            titleHud.SetTitleHudActive(active);
        }
    }

    public void SetPatrolFlag(bool active)
    {
        if (headHud != null)
        {
            headHud.headHUDView.runFlagSpriteAnimation.SetEnable(active);
            if (active)
            {
                headHud.headHUDView.runFlagSpriteAnimation.sprite.spriteName = "PatrolFlag_01";
                headHud.headHUDView.runFlagSpriteAnimation.namePrefix = "PatrolFlag_";
            }
        }
    }

    public void SetNavFlag(bool active)
    {
        if (headHud != null)
        {
            headHud.headHUDView.runFlagSpriteAnimation.SetEnable(active);
            if (active)
            {
                headHud.headHUDView.runFlagSpriteAnimation.sprite.spriteName = "NavFlag_01";
                headHud.headHUDView.runFlagSpriteAnimation.namePrefix = "NavFlag_";
            }
        }
    }

    public void SetFightFlag(bool active)
    {
        if (headHud != null)
        {
            headHud.headHUDView.fightFlagSpriteAnimation.SetEnable(active);
        }
    }

    private void SetTeamInfoFlag(ITeamData teamdata)
    {
        // todo fish: 只在帮战场景中显示主角队伍人数,或者考虑进入帮战场景再开始订阅组队数据流
        if (headHud != null)
        {
            bool finalActive = false;
            int tTeamCount = 0;

            GameDebuger.TODO(@"if (active && _playerDto.teamStatus == (int)TeamMemberDto.TeamMemberStatus.Leader) {
                tTeamCount = ModelManager.Team.isOwnTeam(_playerDto.teamId) ?
                    ModelManager.Team.GetFollowMemberCount() : _playerDto.inTeamPlayerCount;
                finalActive = tTeamCount > 0;
            }");

            headHud.headHUDView.teamInfo_UISprite.enabled = finalActive;
            headHud.headHUDView.teamCount_UILabel.enabled = finalActive;

            headHud.headHUDView.teamCount_UILabel.text = finalActive ? string.Format("{0}/5", tTeamCount).WrapColor(ColorConstantV3.Color_Green_Str) : "";
        }
    }

    //	public void SetEscortFlag (bool active)
    //	{
    //		if (headHud != null) {
    //			bool finalActive = false;
    //			OneShotUIEffect effect = null;
    //
    //			if (active/* && _playerDto.teamStatus == PlayerDto.PlayerTeamStatus_Leader*/) {
    //				int tEscortTaskType = ModelManager.Escort.GetPlayerEscortTaskType(_playerDto);
    //
    //                if (tEscortTaskType == Shipment.TaskType_Team) {
    //                    finalActive = true;
    //
    //                    int tEscortShipmentType = ModelManager.Escort.GetPlayerEscortShipmentType(_playerDto);
    //
    //                                headHud.headHUDView.escortFlag_UISprite.spriteName = _escortSpriteNames[tEscortShipmentType];
    //
    //                    if (_lastEscortShipmentType != tEscortShipmentType) {
    //                        _lastEscortShipmentType = tEscortShipmentType;
    //
    //                        if (_escortEffectDic.ContainsKey(tEscortShipmentType)) {
    //                            effect = _escortEffectDic[tEscortShipmentType];
    //                        } else {
    //                            string finalEffectName = _escortEffectNames[tEscortShipmentType];
    //                            effect = OneShotUIEffect.BeginFollowEffect(finalEffectName, headHud.headHUDView.escortFlag_UISprite, Vector2.zero, -1);
    //                            _escortEffectDic.Add(tEscortShipmentType, effect);
    //                        }
    //                    }
    //                }
    //			}
    //            headHud.headHUDView.escortFlag_UISprite.enabled = finalActive;
    //
    //			if (finalActive) {
    //				if (effect != null) {
    //					effect.SetActive(finalActive);
    //				}
    //			} else {
    //				if (_lastEscortShipmentType != Shipment.ShipmentType_Unknown && _escortEffectDic.ContainsKey(_lastEscortShipmentType)) {
    //                    _escortEffectDic[_lastEscortShipmentType].SetActive(false);
    //                }
    //                _lastEscortShipmentType = Shipment.ShipmentType_Unknown;
    //			}
    //		}
    //	}

    #endregion

    #region Animation

    private void InitPlayerAnimation()
    {
        if (_isRunning)
        {
            _modelDisplayer.PlayAnimation(ModelHelper.AnimType.run, false);
        }
        else
        {
            _modelDisplayer.PlayAnimation(ModelHelper.AnimType.idle, false);
        }
    }

    private void UpdatePlayerAnimation()
    {
        if (_mAgent.IsActiveAndEnabled())
        {
            if (_mAgent.InMoving())
            {
                PlayRunAnimation();
            }
            else
            {
                PlayIdleAnimation();
            }
        }
        else
        {
            PlayIdleAnimation();
        }
    }

    private void PlayIdleAnimation()
    {
        if (_isRunning)
        {
            _modelDisplayer.PlayAnimation(ModelHelper.AnimType.idle);
            _isRunning = false;

            CancelInvoke("DelayStopNavFlag");
            Invoke("DelayStopNavFlag", 0.1f);

            //	到达目标点回调
            if (_toTargetCallback == null) return;
            _toTargetCallback();
            _toTargetCallback = null;
        }
    }

    protected void PlayRunAnimation()
    {
        if (_isRunning) return;
        GameLog.LogFish("PlayRunAnimation----------" + ModelManager.Player.GetPlayerName());
        _modelDisplayer.PlayAnimation(ModelHelper.AnimType.run);
        _isRunning = true;
    }

    public bool IsRunning()
    {
        return _isRunning;
    }

    #endregion

    //void OnApplicationFocus(bool isFocus)
    //{
    //    if (isFocus)
    //    {
    //        if (null != _mGo)
    //        {
    //            //可能有多个染色，比如人物的、坐骑的等。
    //            ModelHSV[] tModelHSVs = _mGo.GetComponentsInChildren<ModelHSV>();
    //            if (null != tModelHSVs && tModelHSVs.Length > 0)
    //            {
    //                ModelHSV tModelHSV;
    //                for (int tCounter = 0, tLen = tModelHSVs.Length; tCounter < tLen; tCounter++)
    //                {
    //                    tModelHSV = tModelHSVs[tCounter];
    //                    if (null != tModelHSV)
    //                        tModelHSV.RefreshModelHueMatrix();	
    //                }
    //            }
    //        }
    //    }
    //}
    
    public enum DetailRideStatus
    {
        //RideFlyMount,         //参战坐骑为飞行坐骑， 且已骑上
        //RideDownFlyMount,     //参战坐骑为飞行坐骑， 还没骑乘

        //RideLandMount,        //参战坐骑为陆地坐骑， 且已骑上
        //RideDownLandMount,    //参战坐骑为陆地坐骑， 还没骑乘

        Unkonw,
        //未知情况
        NoneRide,
        //没有坐骑
        LandMount,
        //陆地坐骑  骑乘
        FlyRideMount,
        //飞行坐骑  低飞
        FlyHighMount,
        //飞行坐骑  高飞

    };
    
    /// <summary>
    /// 获取骑乘信息, 若考虑组队的情况而没有队长， 则以自身为准
    /// </summary>
    /// <param name="pConsiderLeader"> 是否考虑有队长的情况</param>
    /// <returns></returns>
    public DetailRideStatus GetDetailRideStatus(bool pConsiderLeader)
    {
        return DetailRideStatus.NoneRide;
        ScenePlayerDto tPlayerDto = _playerDto;
        if (pConsiderLeader)
        {
            ScenePlayerDto leaderPlayerDto = null;
            if (leaderView != null)
            {
                leaderPlayerDto = leaderView._playerDto;
            }
            else if (_playerDto.teamStatus == (int)TeamMemberDto.TeamMemberStatus.Member)
            {
                leaderPlayerDto = WorldManager.Instance.GetModel().GetTeamLeader(_playerDto.teamId);
            }

            if (leaderPlayerDto != null)
            {
                tPlayerDto = leaderPlayerDto;
            }
        }

//        if (tPlayerDto == null || tPlayerDto.playerShortRide == null || 0 == tPlayerDto.playerShortRide.modelId)
//        {
//            return DetailRideStatus.Unkonw;
//        }
//
//        int tStaus = tPlayerDto.playerShortRide.status;        
//        int tMountType = ModelManager.Mount.GetMountTypeByModelId(tPlayerDto.playerShortRide.modelId);        
//
//        if (tStaus == RideDto.RideStatus_Walking)
//        {
//            return DetailRideStatus.NoneRide;
//        }
//
//        if (tStaus == RideDto.RideStatus_Riding && tMountType == Mount.MountType_Land)
//        {
//            return DetailRideStatus.LandMount;
//        }
//
//        if (tStaus == RideDto.RideStatus_Riding && tMountType == Mount.MountType_Fly)
//        {
//            return DetailRideStatus.FlyRideMount;
//        }
//
//        if (tStaus == RideDto.RideStatus_HighFly && tMountType == Mount.MountType_Fly)
//        {
//            return DetailRideStatus.FlyHighMount;
//        }
//
//        return DetailRideStatus.Unkonw;
    }
    
    //设置模型的缩放
    public void SetModelScale(float scale)
    {
        if (_modelDisplayer != null)
            _modelDisplayer.UpdateScale(scale);
    }

    public bool GetModelScale(ref float scale)
    {
        if (_modelDisplayer != null && _modelDisplayer.modelStyleInfo != null)
        {
            scale = _modelDisplayer.modelStyleInfo.ModelScale;
            return true;
        }

        return false;
    }
}