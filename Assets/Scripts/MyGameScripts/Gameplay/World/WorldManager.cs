// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  WorldManager.cs
// Author   : willson
// Created  : 2014/12/1 
// Porpuse  : 
// **********************************************************************
using UnityEngine;
using AppDto;
using AppServices;
using System;
using UniRx;

namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner disposeWorldMgr = new StaticDispose.StaticDelegateRunner(
            ()=> { 
            WorldManager.Create();
        } );
    }
}

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeWorldMgr = new StaticDelegateRunner(
            WorldManager.ExecuteDispose);
    }
}

public partial class WorldManager
{
    public static WorldManager Create(){
        if (_instance == null)
        {
            _instance = new WorldManager();
            _instance.Init();
        }
        
        return _instance;
    }

    private static WorldManager _instance;

    public static WorldManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private WorldModel _worldModel;
    private WorldView _worldView;

    private static Subject<IWorldModel> stream = new Subject<IWorldModel>();

    public static IObservableExpand<IWorldModel> WorkdModelStream { get { return stream; }}
    
    private static Subject<Vector2> heroPosStream;

    public static IObservableExpand<Vector2> OnHeroPosStreamChange {
        get { return heroPosStream; }
    }

    //private bool _isRemoverWorldActors = false;
    private CompositeDisposable _disposable = null;
    private int _oldSceneId = 0;

    public static bool FirstEnter = true;
    public static bool IsWaitingEnter = false;

    private WorldManager()
    {

    }
    
    private void Init()
    {
        if (_worldModel != null) return; // 避免重复创建
        _worldModel = WorldModel.Create();
        _worldView = new WorldView(_worldModel);
        heroPosStream = new Subject<Vector2>();
        heroPosStream.Hold(Vector2.zero);
        stream = new Subject<IWorldModel>();
        stream.Hold(_worldModel);
        _disposable= new CompositeDisposable();
        
        _disposable.Add(NotifyListenerRegister.RegistListener<SceneDto>(EnterWithSceneDto));
        
        _disposable.Add(TeamDataMgr.Stream.Subscribe(s=>{
                if (s == null)
                    return;
            
                if (_worldModel == null) return;
                var memberDto = s.GetTeamMemberDtoByPlayerID(ModelManager.IPlayer.GetPlayerId());
                if ( memberDto != null){
                    _worldModel.ChangePlayerTeamStatus(
                        memberDto.id
                        ,(TeamMemberDto.TeamMemberStatus)memberDto.memberStatus
                        ,s.TeamUniqueId);
                }
                else{
                    _worldModel.ChangePlayerTeamStatus(
                        ModelManager.IPlayer.GetPlayerId()
                        ,TeamMemberDto.TeamMemberStatus.NoTeam
                        , 0L);
                }
                stream.OnNext(_worldModel);
            }
        ));
        _disposable.Add(NotifyListenerRegister.RegistListener<SceneObjectWalkNotify>(noti =>
            {
                if (noti == null || noti.sceneId != Instance._worldModel.GetSceneId()) return;

                GameDebuger.Log("tSceneObjectWalkNotify " + noti.id);
                
                Instance._worldModel.HandleSceneObjectWalkNotify(noti);
                
            }));
            _disposable.Add(NotifyListenerRegister.RegistListener<SceneObjectTeleportNotify>(noti =>
            {
                if (noti == null) return;
                GameDebuger.Log("tSceneObjectWalkNotify " + noti.id.ToString());
                Instance._worldModel.HandleSceneObjectTeleportNotify(noti);
            }));
            _disposable.Add(NotifyListenerRegister.RegistListener<SceneObjectRemoveNotify>(noti =>
            {
                if (noti == null) return;

                GameDebuger.Log("tSceneObjectRemoveNotify" + noti.objId);
                if ((SceneObjectDto.SceneObjectType) noti.objType == SceneObjectDto.SceneObjectType.Player)
                    Instance._worldModel.HandleSceneObjectRemoveNotify(noti);
                else
                    Instance.GetNpcViewManager().RemoveDynamicCommonNpc(noti.objId);
            }));

            _disposable.Add(NotifyListenerRegister.RegistListener<SceneObjectNotify>(noti =>
            {
                if (noti == null) return;
                var tSceneObjectDto = noti.obj;
                if ((SceneObjectDto.SceneObjectType)tSceneObjectDto.objType == SceneObjectDto.SceneObjectType.Player)
                    Instance._worldModel.HandlePlayerSceneObjectNotify(tSceneObjectDto as ScenePlayerDto);
                else
                    Instance.GetNpcViewManager().AddDynamicCommonNpc(tSceneObjectDto as SceneNpcDto);
            }));

            _disposable.Add(NotifyListenerRegister.RegistListener<NpcBattleNotify>(noti => {
                if(noti == null) return;
                var tSceneObjectDto = noti;
                Instance.GetNpcViewManager().UpdateDynamicCommonNpcBattleState((tSceneObjectDto));
            }));

            _disposable.Add(NotifyListenerRegister.RegistListener<NpcDisappearNotify>(noti =>{
                if(noti == null) return;
                var tNpcDisappearNotify = noti;
                int sceneId = Instance.GetModel().GetSceneId();
                if(sceneId == tNpcDisappearNotify.sceneId) {
                    Instance.GetNpcViewManager().RemoveDynamicCommonNpc(tNpcDisappearNotify.id);
                }
            }));

            _disposable.Add(NotifyListenerRegister.RegistListener<PlayerSceneObjectChangeNotify>(noti =>
            {
                if (noti == null) return;

                noti.removed.ForEach<SceneObjectRemoveNotify>(moveItem =>
                {
                    GameDebuger.Log("PlayerSceneObjectChangeNotify@remove@" + moveItem.objId);
                    if ((SceneObjectDto.SceneObjectType)moveItem.objType == SceneObjectDto.SceneObjectType.Player)
                        Instance._worldModel.HandleSceneObjectRemoveNotify(moveItem);
                    else
                        Instance.GetNpcViewManager().RemoveDynamicCommonNpc(moveItem.objId);
                });

                noti.enters.ForEach<SceneObjectNotify>(enterItem =>
                {
                    GameDebuger.Log("PlayerSceneObjectChangeNotify@enter@" + enterItem.obj.name);
                    if ((SceneObjectDto.SceneObjectType)enterItem.obj.objType == SceneObjectDto.SceneObjectType.Player)
                        Instance._worldModel.HandlePlayerSceneObjectNotify(enterItem.obj as ScenePlayerDto);
                    else
                        Instance.GetNpcViewManager().AddDynamicCommonNpc(enterItem.obj as SceneNpcDto);
                });
            }));
            _disposable.Add(NotifyListenerRegister.RegistListener<PlayerNameNotify>(noti =>
            {
                if (noti == null) return;

                Instance._worldModel.UpdatePlayerName(noti);
            }));
        //_disposable = _disposable.CombineRelease(WorldModel.Stream.Select(x => x.WorldViewData.LatestPlayerChageTeamStatusSet).Subscribe(
        //     x => { _worldView.TryUpdatePlayerViewTeamStatus(x); }
        //    ));
        //_disposable = _disposable.CombineRelease(WorldModel.Stream.Subscribe(
        //    d =>
        //    {
        //        if (d != null)
        //        {
        //            _worldView.TryUpdatePlayerViewTeamStatus(d.WorldViewData.LatestPlayerChageTeamStatusSet);
        //            _worldView.TryUpdatePlayerViewTeamStatus(d.WorldViewData.GetNoTeamPlayers);
        //        }
        //    }
        //));

        //_isRemoverWorldActors = false;
    }
        
    public event Action<SceneDto> OnSceneChanged;
    

    private bool _needDelayLoadScene;
    //用于标记是否需要加载新场景

    public void Dispose()
    {
        Reset();
        _oldSceneId = 0;
        _worldModel.SetupSceneDto(null);

        _worldView.Destroy();
        _worldView = null;
        
        if (_disposable != null)
            _disposable.Dispose();
        stream = stream.CloseOnceNull();
        heroPosStream = heroPosStream.CloseOnceNull();
    }

    #region Getter

    //当前场景是否可以飞
    public bool CanFlyable()
    {
        SceneDto sceneDto = _worldModel.GetSceneDto();
        if (sceneDto != null)
        {
            return sceneDto.sceneMap.flyable;
        }

        return true;
    }

    public WorldModel GetModel()
    {
        return _worldModel;
    }

    public WorldView GetView()
    {
        return _worldView;
    }

    public NpcViewManager GetNpcViewManager()
    {
        return _worldView.GetNpcViewManager();
    }

    public HeroView GetHeroView()
    {
        return _worldView.GetHeroView();
    }

    public Vector3 GetHeroWorldPos()
    {
        var heroView = _worldView.GetHeroView();
        return heroView != null ? heroView.cachedTransform.position : Vector3.zero;
    }

    public bool IsInActivityScene()
    {
        return false;
    }

    #endregion



    #region 跳转场景

    //	跳转场景锁，防止重复调用,修过保存为当前id,处理点击传送点偶尔会出现的 正在进入其它地图 提示
    private int _isEntering = -1;

    public bool isEntering
    {
        get { return _isEntering != -1; }
    }

    public int OldSceneId {
        set { _oldSceneId = value; }
    }

    public void FirstEnterScene()
    {
        //Debug.LogError("FirstEnterScene:" + Time.realtimeSinceStartup + " " + (_waitingSceneDto == null));
        if (_waitingSceneDto != null)
        {
            EnterWithSceneDto(_waitingSceneDto);
        }
        else
        {
            var playView = Instance.GetHeroView();

            var posStr = playView == null ? string.Empty : playView.GetPosStr();
            Instance.Enter(ModelManager.Player.GetPlayer().sceneId, false, false, false, PlayerGameState.FollowTargetNpc, flyPos : posStr);
        }
    }

    public void Enter(
        int sceneId
        , bool isTeleport = false
        , bool isAutoFramEnter = false
        , bool checkSameScene = true
        , Npc targetNpc = null
        , string flyPos = "")
    {
        JSTimer.Instance.CancelCd("__waitTimeForNextMission");

        _isEntering = sceneId;

        _targetNpc = targetNpc;
        
        //**重登 数据没有reset， FirstEnter为false 无法进入loginScene  todo xjd
        if (FirstEnter)
        {
            RequestLoginScene(sceneId);
        }
        else
        {
            if (isTeleport)
            {
                RequestEnterScene(sceneId);
            }
            else
            {
                RequestFlyScene(sceneId, flyPos);
            }
        }
    }

    private void RequestLoginScene(int sceneId)
    {
        ServiceRequestAction.requestServer(Services.Scene_Login(string.Empty), "", null, RequestEnterFail);//sceneId and OnRequestTimeout is legacy 2017-02-22 15:05:23
        /** 测试用，实际会主动发过来的
         * SceneDto tSceneDto = new SceneDto();
        tSceneDto.id = WorldManager.DEFAULT_SCENE_ID;
        tSceneDto.name = "测试名字";
        WorldManager.DataMgr.EnterWithSceneDto(tSceneDto);*/
    }

    private void RequestEnterScene(int sceneId)
    {
        GameDebuger.TODO("RequestEnterScene sceneId:" + sceneId.ToString());//暂时用fly，有enter协议了再用 2017-02-22 15:17:51
        ServiceRequestAction.requestServer(Services.Scene_Fly(sceneId, string.Empty), "", null, RequestEnterFail/**, OnRequestTimeout*/);
    }
    private void RequestFlyScene(int sceneId,string flyPos)
    {
        ServiceRequestAction.requestServer(Services.Scene_Fly(sceneId, flyPos), "", null, RequestEnterFail/**, OnRequestTimeout*/);
    }

    public string MakeFlyPos(float x, float z,bool defaultPos = true)
    {
        string pos = "";
        if (defaultPos)
            pos = string.Empty;
        else
            pos = "x=" + x + "&z=" + z;
        return pos;
    }

   

    private void RequestEnterFail(ErrorResponse error)
    {
        TipManager.AddTip(error.message);
        _isEntering = -1;

        if (FirstEnter)
        {
            ExitGameScript.OpenReloginTipWindow(string.Format("首次场景进入请求失败，请重新登陆(原因={0})", error.message));
        }
    }

    private bool firstRequest = false;
    private Action<SceneDto> firstEnterRequestCallBack;
    //登陆提前请求进入场景的网络数据
    public void FirstEnterSceneRequest(Action<SceneDto> callBack)
    {
        firstEnterRequestCallBack = callBack;
        firstRequest = true;
        try
        {        
            Instance.Enter(ModelManager.Player.GetPlayer().sceneId, false, false, false,
                PlayerGameState.FollowTargetNpc);
        }
        catch (Exception ex)
        {

            //firstEnterRequestCallBack();
            //firstEnterRequestCallBack = null;
            throw;
        }
        finally
        {
            firstRequest = false;
        }
    }

    private void OnRequestTimeout()
    {
        _isEntering = -1;
    }

    private SceneDto _waitingSceneDto;
    //SceneDto改为下发通知，接口不直接返回
    public void EnterWithSceneDto(SceneDto sceneDto)
    {   
        _isEntering = -1;
        GameDebuger.Log("EnterWithSceneDto = " + sceneDto.id + " " + sceneDto.name);

        var dto = sceneDto.objects.Find(objDto => objDto.id == ModelManager.IPlayer.GetPlayerId());

        var battleID= dto != null && dto.battleId > 0 ?  dto.battleId : 0l;

        if (IsWaitingEnter)
        {
            _waitingSceneDto = sceneDto;
            if (firstEnterRequestCallBack != null)
            {
                firstEnterRequestCallBack(sceneDto);
                firstEnterRequestCallBack = null;
                if (battleID > 0l)
                    BattleDataManager.BattleNetworkManager.ReqBattleVideo(dto.battleId);
            }
            else
            //CleanTargetNpc();
            // 首次登陆进入场景 不打log
                GameDebuger.LogError("正在载入其它地图");
            return;
        }

        if (battleID > 0l)
            BattleDataManager.BattleNetworkManager.ReqBattleVideo(dto.battleId);
        
        _waitingSceneDto = null;

        GameDebuger.TODO(@"ProxyManager.Dialogue.CloseNpcDialogue();");
        ProxyWorldMapModule.CloseMiniMap();

        if (_worldView != null)
            _worldView.Destroy();

        ModelManager.Player.GetPlayer().sceneId = sceneDto.id;

        _worldModel.SetupSceneDto(sceneDto);

        //在战斗中，延迟加载场景
        if (BattleDataManager.DataMgr.IsInBattle)
        {
            Instance.HideScene();
            GameDebuger.Log("在战斗中，延迟加载场景");
            _needDelayLoadScene = true;
        }
        else
        {
            GameDebuger.Log(string.Format("EnterWithSceneDto sceneId={0} oldSceneId={1}", sceneDto.id, _oldSceneId));

            _worldView.InitView();
            if (FirstEnter)
            {
                FirstEnter = false;
                TipManager.AddTip(string.Format("欢迎进入{0}服务器", ServerManager.Instance.GetServerInfo().name.WrapColor(ColorConstant.Color_Name)));
                GameCheatManager.Instance.Setup();
            }

            IsWaitingEnter = true;
            JoystickModule.Instance.EnabledJoystick(false);
            if (sceneDto.id != _oldSceneId)
            {
                _oldSceneId = sceneDto.id;
                SceneFadeEffectController.Show(sceneDto, OnLoadMapFinish, OnFadeOutFinish);
            }
            else
            {
                OnLoadMapFinish(sceneDto.id);
                JSTimer.Instance.SetupCoolDown("DelayFadeOutFinish", 0.1f, null, OnFadeOutFinish);
            }
        }

        if (OnSceneChanged != null)
        {
            OnSceneChanged(sceneDto);
        }
        stream.OnNext(_worldModel);
    }

   
    #endregion


    public void PlayWorldMusic()
    {
        AudioManager.Instance.PlayMusic(_worldModel.GetWorldMusic());
    }

    public void Reset()
    {
        _isEntering = -1;
        IsWaitingEnter = false;
        _waitingSceneDto = null;
        //_isRemoverWorldActors = false;
    }

    public static void ExecuteDispose()
    {
        if (_instance == null)
            return;
        
        _instance.Dispose();
        _instance = null;
        GamePlayer.CameraManager.Instance.SceneCamera.ResetCamera();
    }

    public bool IsDestroy()
    {
        return _oldSceneId == 0;
    }

    private void OnLoadMapFinish(int sceneId)
    {
        IsWaitingEnter = false;

        if (_waitingSceneDto != null)
        {
            _oldSceneId = sceneId;
            return;
        }

        if (_oldSceneId != sceneId)
        {
            GamePlayer.CameraManager.Instance.SceneCamera.ResetCamera();
            _oldSceneId = sceneId;
        }

        PlayWorldMusic();

        _worldView.InitView();

        if (!BattleDataManager.BattleManager.Instance.CheckNextBattle())
        {
            LayerManager.Instance.SwitchLayerMode(UIMode.GAME);
        }
        
        GameDebuger.TODO(@"if (ModelManager.MissionData.GetPlayerMissionDtoByMissionID(2002) != null)
        {
            NewBieGuideManager.DataMgr.ActiveGuide(NewBieGuideManager.Key_GuideMainMissionMainView);
        }

        //如果玩家还没有选择是否是经验玩家， 则先打开选择，忽略后面的签到弹出
        if (!ProxyManager.NewbieGuide.OpenNewBieExperienced())
        {
            //这里除了签到弹出的检查， 还内嵌了检查战斗的逻辑， 很特殊的处理， 要注意
            ModelManager.CheckinReward.JudgePopupSignView();
        }");

        //如果是封停场景， 则主动关闭功能模块
        if (IsInBanScene() || ModelManager.Player.IsPlayerBandMode(false))
        {
            UIModuleManager.Instance.CloseOtherModuleWhenNpcDialogue();
        }
        //GameDebuger.GC();
    }

    public bool IsInBanScene()
    {
        return _worldModel.GetSceneId() == DataCache.GetStaticConfigValue(AppStaticConfigs.BAN_SCENE_ID);
    }

    public event System.Action OnFadeOutFinishEvt;

    private void OnFadeOutFinish()
    {
        if (_waitingSceneDto != null)
        {
            EnterWithSceneDto(_waitingSceneDto);
            return;
        }

        ResumeWalkToNpc();
        JoystickModule.Instance.EnabledJoystick(true);

        if (OnFadeOutFinishEvt != null)
            OnFadeOutFinishEvt();
    }

    public void HideScene()
    {
        /*
		 * 这里处理目前有问题， 先屏蔽
		if(SystemSetting.LowMemory && !FirstEnter)
        {
            if (LayerManager.Root.WorldActors.activeSelf == false)
            {
                GameDebuger.LogError("WorldActors 被隐藏,不能删除npc......");
            }

            // 删除npc,人物
            _isRemoverWorldActors = true;
            JoystickModule.DataMgr.SyncWithServer();
			if (GetHeroView() != null)
			{
				GetHeroView().GetPlayerDto().x = GetHeroView().gameObject.transform.position.x;
				GetHeroView().GetPlayerDto().z = GetHeroView().gameObject.transform.position.z;
			}
            _worldView.Destroy();
        }
        */

        LayerManager.Instance.SwitchLayerMode(UIMode.BATTLE);
    }

    public void ResumeScene()
    {
        if (_worldModel.GetSceneDto() != null)
        {
            PlayWorldMusic();

            //战斗结束,检查是否需要加载新场景
            if (_needDelayLoadScene)
            {
                IsWaitingEnter = true;
                _worldView.Destroy();
                SceneFadeEffectController.Show(_worldModel.GetSceneDto(), OnLoadMapFinish, OnFadeOutFinish);
                _needDelayLoadScene = false;
            }

            //if(_isRemoverWorldActors){
            //    _worldView.Setup();
            //    _isRemoverWorldActors = false;
            //}

            LayerManager.Instance.SwitchLayerMode(UIMode.GAME);

            if (FirstEnter)
            {
                Enter(ModelManager.Player.GetPlayer().sceneId);
            }
        }
    }

    public void PlayCameraAnimator(int sceneId, int cameraId)
    {
        if (cameraId > 0)
        {
            GamePlayer.CameraManager.Instance.PlayCameraAnimator(sceneId, cameraId);
        }
    }

    #region 玩家位置检验

    /// <summary>
    ///     立即同步验证玩家位置
    /// </summary>
    public void SyncWithServer()
    {
        var heroView = GetHeroView();
        if (heroView != null)
            heroView.SyncWithServer();
    }
    //没有队伍或者是队长时才发送验证信息
    public void PlanWalk(float targetX, float targetZ)
    {
        //Debug.LogError("PlanWalk " + targetX + ":" + targetZ);
        if (!BattleDataManager.DataMgr.IsInBattle)
        {
            if (!JoystickModule.DisableMove)
                ServiceRequestAction.requestServer(Services.Scene_PlanWalk(_worldModel.GetSceneId(), targetX, targetZ));
        }
    }

    //WorldClickCheck 每隔一段时间请求服务器验证玩家当前位置
    public void VerifyWalk(float toX, float toY)
    {
        //Debug.LogError("VerifyWalk " + toX + ":" + toY);
        if (!BattleDataManager.DataMgr.IsInBattle)
        {
            if (!JoystickModule.DisableMove)
            {
                ServiceRequestAction.requestServer(Services.Scene_VerifyWalk(_worldModel.GetSceneId(), toX, toY));
            }
            heroPosStream.OnNext(new Vector2(toX, toY));
        }
    }

    #endregion

    #region 寻路

    private Npc _targetNpc;

    public void CleanTargetNpc()
    {
        _targetNpc = null;
    }

    public Npc GetTargetNpc()
    {
        return _targetNpc;
    }

    public void ResumeWalkToNpc()
    {
        if (_targetNpc != null)
        {
            //	任务：查看是否寻路可穿越传送阵，设置CharacterController.enable ModelManager.MissionData.heroCharacterControllerEnable
            GameDebuger.TODO(@"if (ModelManager.MissionData.heroCharacterControllerEnable)
            {
                WalkToByNpc(_targetNpc, dialogFunctionID);
            }
            else");
            {
                WalkToByNpc(_targetNpc, dialogFunctionID, _toTargetCallback);
            }
        }
    }

    public void WalkToByNpcId(int npcId, int dialogFunctionID = 0)
    {
        Npc target = DataCache.getDtoByCls<Npc>(npcId);
        WalkToByNpc(target, dialogFunctionID);
    }

    public int dialogFunctionID = 0;
    private System.Action _toTargetCallback = null;

    //任务调用前去NPC接任务;
    public void FlyToByNpc(Npc npc,int functionID = 0, System.Action toTargetCallback = null)
    {
        if(IsWaitingEnter)
        {
            return;
        }
        if (npc == null) return;
        if(_worldModel.GetSceneId() != npc.sceneId)
        {
            _toTargetCallback = toTargetCallback;
            Enter(npc.sceneId, false, false, true, npc);
            return;
        }

        HeroView tHeroView = GetHeroView();

        if(tHeroView == null)
        {
            WalkToByNpc(npc, functionID, toTargetCallback);
            return;
        }

        BaseNpcUnit tNpcUnit = GetNpcViewManager().GetTangNpcUnit(npc.id);
        if(tNpcUnit != null)
        {
            Vector3 tNpcPos = tNpcUnit.GetPos();
            float tDistance = tHeroView.GetDistanceFromOther(tNpcPos);
            if(tDistance > 10.5f)
            {
                WalkToByNpc(npc, functionID, toTargetCallback);
                return;
            }
        }
        WalkToByNpc(npc, functionID, toTargetCallback);
    }

    public void WalkToByNpc(Npc npc, int functionID = 0, System.Action toTargetCallback = null)
    {
        if (npc == null)
        {
            GameDebuger.LogError("Npc is null,can not to find");
            return;
        }

        if (BattleDataManager.DataMgr.IsInBattle)
        {
            //TipManager.AddTip("战斗中，不能进行传送");
            return;
        }

        dialogFunctionID = functionID;
        _toTargetCallback = toTargetCallback;

        GameDebuger.TODO(@"ModelManager.MissionData.HeroCharacterControllerEnable(toTargetCallback == null, functionID);");

        if(_worldModel.GetSceneId() == npc.sceneId)
        {
            //	当前场景内自动寻路头顶标识
            ModelManager.Player.StartAutoNav();

            JoystickModule.Instance.EnabledJoystick(true);
            BaseNpcUnit npcUnit;
            GameDebuger.TODO(@"if(npc is NpcSceneDatangMatchlessMonster)
                npcUnit = _worldView.GetNpcViewManager().GetTangNpcUnit(npc.id);    
            else");
            if(npc.type == (int)Npc.NpcType.PickPoint)
            {
                BaseNpcUnit tNpcUnit = GetNpcViewManager().GetTangNpcUnit(npc.id);
                npcUnit = _worldView.GetNpcViewManager().GetNpcUnit(tNpcUnit.GetNpcUID());
            }
            else
            {
                npcUnit = _worldView.GetNpcViewManager().GetNpcUnit(npc.id);
            }
            if (npcUnit != null)
            {
                _targetNpc = npc;
                npcUnit.Trigger();
            }
            else
            {
                if (GetHeroView() == null || GetHeroView().cachedTransform == null)
                {
                    return;
                }

                Vector3 tPlayerPos = GetHeroView().cachedTransform.position;
                //	判断距离是否在单位步长内 Begin
                /*
				float tDis = Math.Abs(npc.x - tPlayerPos.x) + Math.Abs(npc.z - tPlayerPos.z);
				if (tDis < 1.0f) {
					if (toTargetCallback != null){npcUniqueId
						toTargetCallback();
						GetHeroView().SetNavFlag(false);
					}
					return;
				}
				*/
                //	判断距离是否在单位步长内 End

                Vector3 tLastNpcPos = new Vector3(npc.x, tPlayerPos.y, npc.z);
                tLastNpcPos = SceneHelper.GetSceneStandPosition(tLastNpcPos, Vector3.zero);

                //	判断距离是否在单位步长内 Begin
                float tDis = Vector3.Distance(tPlayerPos, tLastNpcPos);
                if (tDis < 1.0f)
                {
                    if (toTargetCallback != null)
                    {
                        toTargetCallback();
                        GetHeroView().SetNavFlag(false);
                    }
                    //	判断距离是否在单位步长内 End
                }
                else
                {
                    GetHeroView().WalkToPoint(tLastNpcPos, toTargetCallback, true);
                    _targetNpc = npc;
                }
            }
            //CleanTargetNpc();
        }
        else
        {
            Enter(npc.sceneId, false, false, true, npc);
        }
    }

    #endregion

    //判断玩家是否位于PK战斗范围
    public bool CheckPlayerAtBattleScope(long playerId)
    {
        if (_worldView != null)
        {
            if (_worldModel.GetSceneId() != 1001)
            {
                return false;
            }

            HeroView heroView = _worldView.GetHeroView();
            if (heroView == null)
            {
                return false;
            }

            PlayerView playerView = _worldView.GetPlayerView(playerId);
            if (playerView == null)
            {
                return false;
            }

            return SceneHelper.CheckAtBattleScope(heroView.transform.position) && SceneHelper.CheckAtBattleScope(playerView.transform.position);
        }
        else
        {
            return false;
        }
    }

    #region 获取同场景中指定玩家ID信息

    /// <summary>
    /// 获取同场景中指定玩家ID信息 Gets the client player dto.
    /// </summary>
    /// <param name="playerID">Player I.</param>
    /// <param name="callback">Callback.</param>
    public void GetClientPlayerDto(long playerID, Action<ScenePlayerDto> callback)
    {
        //	从服务器获取最新数据
        GameDebuger.TODO(@"ServiceRequestAction.requestServer(SceneService.playerInfo(playerID), "", (e) => {
            ScenePlayerDto serDto = e as ScenePlayerDto;
            
            if (callback != null) {
                callback(serDto);
            }
        });");
    }

    #endregion

    public bool IsFollowLeader()
    {
        return false;
    }

    public long GetPlayerBattleID(long getPlayerId)
    {
        var dto = _worldModel.GetPlayerDto(getPlayerId);
        return dto == null ? 0 : dto.battleId;
    }
}