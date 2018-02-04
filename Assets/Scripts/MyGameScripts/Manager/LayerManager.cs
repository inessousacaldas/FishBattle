// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  LayerManager.cs
// Author   : SK
// Created  : 2013/1/30
// Purpose  : 
// **********************************************************************
using UnityEngine;
using AppDto;
using GamePlayer;
using UniRx;

public enum UILayerType
{
    #region 玩法功能模块层级

    Invalid = -100,
    JOYSTICK = -20,
    HudLayer = -10,

    BaseModule = 0,
    //底层模块

    //弹幕层
    BarrageLayer = 3,

    SceneChange = 10,
    //场景切换提示SceneChange

    ChatModule = 20,
    //聊天模块层

    //public const int BottomModule = 0; //模块

    DefaultModule = 30,
    //模块

    SubModule = 40,
    //2层子模块

    ThreeModule = 50,
    //3层子模块

    FourModule = 60,
    //4层子模块

    FiveModule = 70,
    // 5层子模块

    #endregion

    Guide = 100,
    //引导层

    Dialogue = 110,
    //对话框

    ItemTip = 120,

    FloatTip = 130,
    //飘窗提示

    FadeInOut = 140,
    //黑屏过渡

    LockScreen = 150,
    //锁屏

    TopDialogue = 160,
    //对话框

    TopLockScreen = 170,
    //锁屏层

    QRCodeScan = 180,
    // 二维码扫描层
}

public enum UIMode
{
    NULL = 0,
    LOGIN,
    GAME,
    BATTLE,
    STORY,
    MARRY
}

public class LayerManager:MonoBehaviour
{
    private static GameRoot _root = null;

    public static GameRoot Root
    {
        get
        {
            return _root;
        }
    }
// todo fish:从D1抄过来 找机会整合cameraManager ，其实就是camerashake
    public BattleShakeEffectHelper BattleShakeEffectHelper;
    
    private static Subject<UIMode> stream = new Subject<UIMode>();

    public static IObservableExpand<UIMode> Stream
    {
        get { return stream; }
    }

    //场景特效引用
    public GameObject SceneEffect = null;

    private const float BaseScreenScale = 1136 / 640;
    private const float AdjustScale = 0.97f;

    public event System.Action<UIMode> OnChangeUIMode;

    private static LayerManager _instance = null;

    public static LayerManager Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
        _root = BaseView.Create<GameRoot>(this.transform);

        _root.SceneUIHUDPanel.depth = GetOriginDepthByLayerType(UILayerType.HudLayer);
        _root.PlotUIHUDPanel.depth = GetOriginDepthByLayerType(UILayerType.HudLayer);
        _root.BattleUIHUDPanel.depth = GetOriginDepthByLayerType(UILayerType.HudLayer);

        _root.FloatTipPanel.depth = GetOriginDepthByLayerType(UILayerType.FloatTip);
        _root.TopFloatTipPanel.depth = GetOriginDepthByLayerType(UILayerType.TopDialogue);
        _root.LockScreenPanel.depth = GetOriginDepthByLayerType(UILayerType.LockScreen);
        
        BattleShakeEffectHelper = _root.BattleCamera_Camera.gameObject.GetMissingComponent<BattleShakeEffectHelper>();
        BattleShakeEffectHelper.Setup();
        
        _root.SceneCameraTrans.parent.gameObject.GetMissingComponent<GamePlayer.CameraManager>();
        CacheCameraInfo(_root.BattleCamera_Camera);
    }

    void Start()
    {
        //LayerManager.Root.SceneCamera.audio.volume = 0.2f;
        _root.SceneHudTextPanel.startingRenderQueue = 2455;
        _root.PlotHudTextPanel.startingRenderQueue = 2455;
        stream.Hold(UIMode.NULL);
    }

    public UIMode CurUIMode
    {
        get { return _uiMode; }
        set
        {
            _uiMode = value;
            stream.OnNext(_uiMode);
        }
    }

    private UIMode _uiMode = UIMode.NULL;

    public void SwitchLayerMode(UIMode mode)
    {
        if (CurUIMode == mode)
        {
            return;
        }

        GameDebuger.TODO(@"if (MainUIViewController.Instance != null) {
            MainUIViewController.Instance.ChangeMode (CurUIMode);
        }");
        CameraManager.Instance.SceneCamera.ChangeMode(UIMode.GAME);
        AdjustCameraPosition(mode == UIMode.BATTLE|| mode == UIMode.STORY);

        //JoystickModule.Instance.SetActive (mode == UIMode.GAME);

        //弹幕面板
        GameDebuger.TODO(@"if (mode == UIMode.BATTLE && FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_Barrage, false))
            ProxyManager.Barrage.OpenBarrageLayer();
        else
        {
            ProxyManager.Barrage.CloseBarraylayer();
            ProxyManager.Barrage.CloseChat();
        }");

        var show = mode == UIMode.BATTLE && BattleDataManager.NeedBattleMap;
        _root.BattleLayer.SetActive(show);
        if (!show)
            _root.BattleLayer.RemoveChildren();
        
        show = mode != UIMode.BATTLE || !BattleDataManager.NeedBattleMap;
        _root.SceneLayer.SetActive(show);

        _root.BattleActors.SetActive(mode == UIMode.BATTLE);
        _root.WorldActors.SetActive(mode == UIMode.GAME || mode == UIMode.MARRY);//结婚的时候其他玩家也要显示
        _root.StoryActors.SetActive(mode == UIMode.STORY);

        _root.BattleCamera_Camera.enabled = (mode == UIMode.BATTLE);
        if (_root.BattleBgTexture.mainTexture == null)
        {
            _root.BattleBgTexture.mainTexture = Resources.Load<Texture>("Textures/battleLogo");
        }
        _root.BattleBgTexture.cachedGameObject.SetActive(mode == UIMode.BATTLE && !BattleDataManager.NeedBattleMap);

        _root.BattleHudTextPanel.cachedGameObject.SetActive(mode == UIMode.BATTLE);
        _root.BattleUIHUDPanel.cachedGameObject.SetActive(mode == UIMode.BATTLE);

        if (mode == UIMode.GAME)
        {
            _root.SceneHudTextPanel.cachedGameObject.SetActive(true);
            _root.SceneUIHUDPanel.cachedGameObject.SetActive(true);
        }
        else if (mode == UIMode.MARRY)
        {
            _root.SceneHudTextPanel.cachedGameObject.SetActive(false);
            _root.SceneUIHUDPanel.cachedGameObject.SetActive(false);
        }
        else
        {
            _root.SceneHudTextPanel.cachedGameObject.SetActive(false);
            _root.SceneUIHUDPanel.cachedGameObject.SetActive(false);
        }
        _root.PlotHudTextPanel.cachedGameObject.SetActive(mode == UIMode.STORY || mode == UIMode.MARRY);
        _root.PlotUIHUDPanel.cachedGameObject.SetActive(mode == UIMode.STORY || mode == UIMode.MARRY);

        if (mode == UIMode.BATTLE && !BattleDataManager.NeedBattleMap)
        {
            _root.EffectsAnchor.layer = LayerMask.NameToLayer(GameTag.Tag_BattleActor);
        }
        else
        {
            _root.EffectsAnchor.layer = LayerMask.NameToLayer(GameTag.Tag_Default);
        }

        if (mode == UIMode.GAME)
        {
            TipManager.CheckDelayShow();
            ModelManager.Player.CheckDelayShow();
            GameDebuger.TODO(@"ScreenFixedTipManager.Instance.CheckDelayShow();
            ModelManager.Pet.CheckDelayShow ();
            ModelManager.MissionView.CheckRefreshMissionPanel();
            ModelManager.Achievement.CheckDelayShow();");
        }
        else if (mode == UIMode.STORY)
        {
            GameDebuger.TODO(@"ModelManager.MissionView.CheckRefreshMissionPanel(false);");
        }

        if (OnChangeUIMode != null)
            OnChangeUIMode(mode);

        CurUIMode = mode;
    }

    private void CacheCameraInfo(Camera pBattleCamera)
    {
        CameraConst.BattleCameraLocalPosition = pBattleCamera.transform.localPosition;
        CameraConst.BattleCameraLocalEulerAngles = pBattleCamera.transform.localEulerAngles;
        CameraConst.BattleCameraFieldOfView = pBattleCamera.fieldOfView;
        float scaleFactor = ((float)Screen.width / (float)Screen.height) / BaseScreenScale;
        CameraConst.BattleCameraOrthographicSize = pBattleCamera.orthographicSize * scaleFactor;
    }

    private void AdjustCameraPosition(bool battleMode)
    {
        if (battleMode)
        {
            var scaleFactor = ((float)Screen.width / (float)Screen.height) / BaseScreenScale;
            scaleFactor *= AdjustScale;

            if (BattleDataManager.NeedBattleMap)
            {
                _root.BattleCameraTrans.localPosition = CameraConst.BattleCameraLocalPosition;
                _root.BattleCameraTrans.localEulerAngles = CameraConst.BattleCameraLocalEulerAngles;
                _root.BattleCamera_Camera.fieldOfView = CameraConst.BattleCameraFieldOfView;
                _root.BattleCamera_Camera.orthographicSize = CameraConst.BattleCameraOrthographicSize / scaleFactor;
            }
            else
            {
                _root.SceneCameraTrans.localEulerAngles = CameraConst.BattleCameraLocalEulerAngles;
                _root.SceneCamera.fieldOfView = CameraConst.BattleCameraFieldOfView;
            }
            _root.BattleCamera_Camera.orthographic = !BattleDataManager.NeedBattleMap;
            UpdateBattleCameraForBattleMode();    
        }
        else
        {
            ResetSceneCamera();
            
        }
    }

    private void ResetSceneCamera()
    {
        CameraManager.Instance.SceneCamera.ResetCamera();
    }

    public void LockUICamera(bool isLock)
    {
        if (_root != null)
        {
            _root.UICamera.enabled = !isLock;
        }
    }

    public int GetOriginDepthByLayerType(UILayerType type)
    {
        return (int)type * 10;  //temporary solution, get from cfg is a better way  -- todo fish
    }

    public void SetUIModuleRootActive(bool b)
    {
        if (_root != null)
        {
            _root.UIModuleRoot.SetActive(b);
        }
    }

    public void UpdateBGTextureVisible(bool pVisible)
    {
        _root.BattleBgTexture.alpha = pVisible ? 1f : 0f;
    }

    private void UpdateBattleCameraForBattleMode()
    {
        var tDefaultLayer = LayerMask.NameToLayer(GameTag.Tag_Default);
        var tBatteActorLayer = LayerMask.NameToLayer(GameTag.Tag_BattleActor);
        if (BattleDataManager.NeedBattleMap)
        {
            _root.BattleCamera_Camera.cullingMask = (1 << tDefaultLayer) + (1 << tBatteActorLayer);
            _root.BattleCamera_Camera.clearFlags = CameraClearFlags.SolidColor;
        }
        else
        {
            _root.BattleCamera_Camera.cullingMask = 1 << tBatteActorLayer;
            _root.BattleCamera_Camera.clearFlags = CameraClearFlags.Depth;
        }
    }
    
    private void ResetBattleCameraTrans()
    {
        _root.BattleCamera_Camera.transform.localPosition = CameraConst.BattleCameraLocalPosition;
        _root.BattleCamera_Camera.transform.localEulerAngles = CameraConst.BattleCameraLocalEulerAngles;
        _root.BattleCamera_Camera.transform.localScale = Vector3.one;
    }
    
    public void ResetBattleCameraParent()
    {
        Transform tTransform = Root.BattleCamera_Camera.transform;
        tTransform.SetParent(Root.BattleDefaultRotationCntr_Transform);
        ResetBattleCameraTrans();
    }

    public void UpdateBattleCameraParent(Transform pParent)
    {
        Transform tTransform = Root.BattleCamera_Camera.transform;
        tTransform.SetParent(pParent);
        tTransform.localPosition = Vector3.zero;
        tTransform.localRotation = Quaternion.Euler(Vector3.zero);
        tTransform.localScale = Vector3.one;
    }
}

