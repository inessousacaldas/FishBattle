using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;

public enum DragAction
{
    None,
    Rotate,
    Move,
}

public partial class ModelDisplayUIComponent : BaseView
{
    public const string NAME ="ModelDisplayUIComponent";

    public UITexture modelUITexture;
    public BoxCollider boxCollider;

    protected override void LateElementBinding()
    {
        var root = this.gameObject;
        modelUITexture = root.FindScript<UITexture>("");
        boxCollider = root.FindScript<BoxCollider>("");
    }
}

public class ModelDisplayController : MonolessViewController<ModelDisplayUIComponent>
{
    private const string MODEL_RENDERER = "ModelDisplayRenderer";
    public static int ModelRendererCount;

    public static List<ModelHelper.AnimType> DefaultAnimationClipList = new List<ModelHelper.AnimType>
    {
        //ModelHelper.AnimType.attack,
        //ModelHelper.AnimType.attack3,
        //ModelHelper.AnimType.magic
        ModelHelper.AnimType.mood1,
        ModelHelper.AnimType.show,
    };

    public static List<ModelHelper.AnimType> AttackAnimationClipList = new List<ModelHelper.AnimType>
    {
        ModelHelper.AnimType.attack
    };

    public static List<ModelHelper.AnimType> FashionAnimationClipList = new List<ModelHelper.AnimType>
    {
        ModelHelper.AnimType.idle,
        ModelHelper.AnimType.run
    };

    private static Vector3 CAMERA_POS = new Vector3(0f, 1.35f, 5f);
    private int _customAnimateIndex;
    private ModelHelper.AnimType[] _customAnimateList;
    private ModelHelper.AnimType _defaultAnimate = ModelHelper.AnimType.invalid;
    private Vector3 _defaultRotation = Vector3.zero;
    private bool _isPlaying = false;
    private Camera _mCam;
    private Transform _mCamTrans;

    private Subject<Unit> clickStream;
    public Subject<Unit> ClickEvt {
        get { return clickStream; }
    }

    public ModelDisplayer ModelDisplayer { get; private set; }

    //RenderTexture Relative Member
    private GameObject _modelRenderer;
    private ModelStyleInfo _modelStyleInfo;

    private UITexture _mUITexture;
    private int _width;
    private int _height;
    private const float DEFAULT_FOV = 20f;
    private float _fovFactor = 1f;
    private float _orthographicSize;
    private bool _useOrthographic;
    private float _yOffset;
    private bool _showShadow = false;

    private readonly string taskName;

    public ModelDisplayController()
    {
        taskName = "playmodel_" + this.GetHashCode();
    }

    public UITexture mUITexture
    {
        get { return _mUITexture; }
    }

    public ModelStyleInfo ModelStyleInfo
    {
        get { return _modelStyleInfo; }
    }

    public bool showShadow
    {
        get { return _showShadow; }
        set { _showShadow = value; }
    }


    private void OnEnable()
    {
        if (_modelRenderer != null)
            _modelRenderer.SetActive(true);

        if (ModelDisplayer != null)
        {
            ModelDisplayer.OnEnable();
        }
    }

    private void OnDisable()
    {
        JSTimer.Instance.CancelTimer(taskName);
        if (_modelRenderer != null)
            _modelRenderer.SetActive(false);

        if (ModelDisplayer != null)
        {
            ModelDisplayer.OnDisable();
        }
    }

    protected override void AfterInitView()
    {
        _mUITexture = View.modelUITexture;

        //必须在Start后才可以设置Camera的targetTexture
        _modelRenderer = AssetPipeline.ResourcePoolManager.Instance.SpawnUIGo(MODEL_RENDERER);
        GameObject.DontDestroyOnLoad(_modelRenderer);

        int index = ModelRendererCount++;
        _modelRenderer.name = "ModelRender_" + index;
        _modelRenderer.transform.position = new Vector3(-100f * index - 1000f, 0, 0);
        _mCamTrans = _modelRenderer.transform.Find("ModelCamera");
        _mCam = _mCamTrans.GetComponent<Camera>();

    }

    protected override void OnDispose()
    {
        clickStream = clickStream.CloseOnceNull();
        if (_modelRenderer != null)
        {
            CleanUpModel();
            if (_mUITexture != null)
            {
                _mUITexture.enabled = false;
            }
            if (_mCam != null)
            {
                GameObject.Destroy(_mCam.targetTexture);
                _mCam.targetTexture = null;
            }
            GameObject.Destroy(_modelRenderer);

            _modelRenderer = null;
        }
    }

    public void Init(int width, int height, float defaultRotateY = 0f, float fovFactor = 1.5f, DragAction _act = DragAction.Rotate)
    {
        Init(width, height, new Vector3(0f, defaultRotateY, 0f), fovFactor, act:_act);
    }

    public void Init(
        int width
        , int height
        , Vector3 defaultRotation
        , float fovFactor
        , ModelHelper.AnimType defaultAnimate = ModelHelper.AnimType.idle
        , DragAction act = DragAction.Rotate)
    {
        //先设置好渲染模型的参数，等待Start的时候才生成Camera和加载模型
        //如果游戏对象一直处于非激活状态，直接Destroy是不会调用OnDestroy方法的
        _defaultRotation = defaultRotation;
        _defaultAnimate = defaultAnimate;
        _fovFactor = fovFactor;
        _width = width;
        _height = height;

        UIEventTrigger eventTrigger = _mUITexture.GetComponent<UIEventTrigger>();
        EventDelegate.Set(eventTrigger.onClick, OnClickModel);
        if (clickStream != null)
            clickStream.CloseOnceNull();
        clickStream = _mUITexture.gameObject.OnClickAsObservable();

        var box = View.boxCollider;
        switch (act)
        {
            case DragAction.Rotate:
                EventDelegate.Set(eventTrigger.onDrag, OnDragModel);
                box.enabled = true;
                box.size = new Vector3(width, height);
                break;
            case DragAction.Move:
                this.gameObject.GetMissingComponent<UIDragDropItemCallbackable>();box.enabled = true;
                box.enabled = true;
                box.size = new Vector3(width, height);
                break;
            case DragAction.None:
                box.enabled = false;
                break;
        }

        int newW = _width;
        int newH = _height;
        float pixelSizeAdjustment = UIRoot.GetPixelSizeAdjustment(gameObject);
        //透视模式下,根据fovFactor参数放大UITexture大小
        if (!_useOrthographic)
        {
            _mCam.fieldOfView = DEFAULT_FOV * _fovFactor;
            newW = Mathf.RoundToInt(_width * _fovFactor);
            newH = Mathf.RoundToInt(_height * _fovFactor);
        }
        var renderTexture = new RenderTexture(
            Mathf.RoundToInt(newW / pixelSizeAdjustment),
            Mathf.RoundToInt(newH / pixelSizeAdjustment), 
            16);
        renderTexture.name = _modelRenderer.name;
        renderTexture.autoGenerateMips = false;
        _mCam.targetTexture = renderTexture;
        _mCam.orthographic = _useOrthographic;
        _mCam.orthographicSize = _orthographicSize;
        _mCam.enabled = true;
        _mUITexture.mainTexture = _mCam.targetTexture;
        _mUITexture.SetDimensions(newW, newH);

        ModelDisplayer = new ModelDisplayer(_modelRenderer, OnLoadModelFinish, true);
        UpdateModel();
    }

    public void SetupModel(GeneralCharactor charactor, bool isMutate = false, int weaponModelId = 0)
    {
        _modelStyleInfo = ModelStyleInfo.ToInfo(charactor);
        _modelStyleInfo.weaponId = weaponModelId;

        if (!isMutate)
        {
            _modelStyleInfo.mutateTexture = 0;
        }

        UpdateModel();
    }


    public void SetOrnamentShow(int ornamentId)
    {
        _modelStyleInfo.ornamentId = ornamentId;
        UpdateModel();
    }

    public void SetupModel(ModelStyleInfo info)
    {
        _modelStyleInfo = info;
        UpdateModel();
    }

    private void SetupModelByDressInfo(GeneralCharactor charactor, PlayerDressInfoDto dressInfo)
    {
        _modelStyleInfo = ModelStyleInfoWrapper.CreateModelStyleInfo(charactor, dressInfo);
        UpdateModel();
    }

    public void UpdateFashionPart(int fashionId, bool remove = false)
    {
        GameDebuger.TODO(@"_modelStyleInfo.UpdateFashionPart(fashionId, remove);");
        UpdateModel();
    }

    public void SetupModelByBaseNpcInfo(BaseNpcInfo info)
    {
        _modelStyleInfo = ModelStyleInfo.ToInfo(info);
        UpdateModel();
    }

    public void SetupModelByFashionIds(GeneralCharactor charactor, List<int> fashionList, int weaponId = 0, int weaponEffId = 0, int pHallowSpriteId = 0)
    {
        _modelStyleInfo = ModelStyleInfo.ToInfo(charactor);
        _modelStyleInfo.weaponId = weaponId;
        _modelStyleInfo.weaponEffId = weaponEffId;
        _modelStyleInfo.hallowSpriteId = pHallowSpriteId;
        // 取消默认模型显示
        //_modelStyleInfo.useFashionDefaultModel = true;
        _modelStyleInfo.SetupFashionIds(fashionList);
        UpdateModel();
    }

//    public void SetupModelByFashionDresses(GeneralCharactor charactor, List<FashionDress> fashionDresses,
//                                           int weaponId = 0, bool useFashionDefaultModel = true)
//    {
//        _modelStyleInfo = modelStyleInfo.ToInfo(charactor);
//        _modelStyleInfo.weaponId = weaponId;
//        _modelStyleInfo.useFashionDefaultModel = useFashionDefaultModel;
//        _modelStyleInfo.SetupFashionDresses(fashionDresses);
//        UpdateModel();
//    }

    public void SetupModelBySoul(PlayerDto pPlayerDto, int pHallowSpriteId)
    {
        _modelStyleInfo = ModelStyleInfo.ToInfo(pPlayerDto.charactor);
        GameDebuger.TODO(@"_modelStyleInfo.weaponId = ModelManager.Backpack.GetCurrentWeaponModel();");
        _modelStyleInfo.weaponEffId = pPlayerDto.dressInfo.weaponEffect;
        _modelStyleInfo.hallowSpriteId = pHallowSpriteId;
        _modelStyleInfo.mutateTexture = 0;
        _modelStyleInfo.mutateColorParam = PlayerModel.GetDyeColorParams(pPlayerDto.dressInfo);

        UpdateModel();
    }

    public void SetupMainRoleModel(bool showFashion = false)
    {
        _modelStyleInfo = ModelStyleInfoWrapper.CreateModelStyleInfo(ModelManager.Player, showFashion);
        _modelStyleInfo.weaponId = ModelManager.Player.PlayerFaction.weaponModelId;
        UpdateModel();
    }
//    public void SetupModel(PetPropertyInfo petInfo)
//    {
//        _modelStyleInfo = modelStyleInfo.ToInfo(petInfo.pet);
//        _modelStyleInfo.ornamentId = petInfo.petDto.ornamentId;
//
//        //设置宠物染色参数
//        _modelStyleInfo.mutateColorParam = PetModel.GetDyeColorStr(petInfo.petDto.dyeCaseId);
//
//        //设置宠物装饰染色参数
//        _modelStyleInfo.ornamentColorParam = PetModel.GetOrnmentDyeColorStr(petInfo.petDto.ornamentDyeCaseId);
//
//        //根据dyeCase决定是否使用变异贴图
//        if (!petInfo.petDto.ifMutate)
//            _modelStyleInfo.mutateTexture = 0;
//        else
//        {
//            PetDyeCase petDyeCase = DataCache.getDtoByCls<PetDyeCase>(petInfo.petDto.dyeCaseId);
//            if (petDyeCase != null && !petDyeCase.changeColor)
//            {
//                _modelStyleInfo.mutateTexture = 0;
//            }
//        }
//
//        UpdateModel();
//    }

    public void SetupModel(PlayerDto playerDto, int transformModelId, bool showFashion, int weaponEffId)
    {
        GameDebuger.TODO(@"GameEventCenter.RemoveListener(GameEvent.Backpack_OnWeaponModelChange, UpdateWeapon);
        GameEventCenter.RemoveListener(GameEvent.Backpack_OnHallowSpriteChange, UpdateHallowSprite);");
        if (transformModelId != 0)
        {
            _modelStyleInfo = new ModelStyleInfo();
            _modelStyleInfo.TransformModelId = transformModelId;
        }
        else
        {
            _modelStyleInfo = ModelStyleInfo.ToInfo(playerDto.charactor);
            GameDebuger.TODO(@"_modelStyleInfo.weaponId = ModelManager.Backpack.GetCurrentWeaponModel();");
            _modelStyleInfo.weaponEffId = weaponEffId;
            GameDebuger.TODO(@"_modelStyleInfo.hallowSpriteId = ModelManager.Backpack.GetCurrentHallowSpriteId();");
            _modelStyleInfo.mutateTexture = 0;
            _modelStyleInfo.mutateColorParam = PlayerModel.GetDyeColorParams(playerDto.dressInfo);
            GameDebuger.TODO(@"GameEventCenter.AddListener(GameEvent.Backpack_OnWeaponModelChange, UpdateWeapon);
            GameEventCenter.AddListener(GameEvent.Backpack_OnHallowSpriteChange, UpdateHallowSprite);");

            if (playerDto.dressInfo != null)
            {
                //_modelStyleInfo.hallowSpriteId = playerDto.dressInfo.hallowSpriteId;
                if (showFashion)
                    _modelStyleInfo.SetupFashionIds(playerDto.dressInfo.fashionDressIds);
            }
        }
        UpdateModel();
    }

//    public void SetupModel(GuildPlayerDto guildPlayerDto)
//    {
//        SetupModelByDressInfo(guildPlayerDto.charactor, guildPlayerDto.dressInfo);
//    }

    public void SetupModel(SimplePlayerDto simplePlayerDto)
    {
        GameDebuger.TODO(@"SetupModelByDressInfo(simplePlayerDto.charactor, simplePlayerDto.dressInfo);");
    }

    public void SetupModel(PlayerDressInfo playerDressInfo)
    {
        if (playerDressInfo.transformModelId != 0)
        {
            _modelStyleInfo = new ModelStyleInfo();
            _modelStyleInfo.TransformModelId = playerDressInfo.transformModelId;
        }
        else
        {
            GeneralCharactor charactor = DataCache.getDtoByCls<GeneralCharactor>(playerDressInfo.charactorId);
            _modelStyleInfo = new ModelStyleInfo();
            _modelStyleInfo.weaponId = playerDressInfo.wpmodel;
            GameDebuger.TODO(@"_modelStyleInfo.hallowSpriteId = playerDressInfo.hallowSpriteId;");
            _modelStyleInfo.weaponEffId = playerDressInfo.weaponEffect;
            _modelStyleInfo.defaultModelId = charactor.modelId;
            _modelStyleInfo.mainTexture = charactor.texture;
            _modelStyleInfo.mutateColorParam = PlayerModel.GetDyeColorParams(playerDressInfo);
        }
        UpdateModel();
    }

    //没有时装
//    public void TalentNoShow(PlayerDto playerDto, GeneralCharactor general, RideMountNotify rideInfo, int rideLV = 0, int weaponId = 0)
//    {
//        _modelStyleInfo = modelStyleInfo.ToInfo(general);
//        if (weaponId != 0)
//            _modelStyleInfo.weaponId = weaponId;
//        else
//            _modelStyleInfo.weaponId = playerDto.dressInfo.wpmodel;
//
//        _modelStyleInfo.weaponEffId = playerDto.dressInfo.weaponEffect;
//        _modelStyleInfo.hallowSpriteId = playerDto.dressInfo.hallowSpriteId;
//        _modelStyleInfo.mutateColorParam = PlayerModel.GetDyeColorParams(playerDto.dressInfo);
//        _modelStyleInfo.mutateTexture = 0;
//        if (rideInfo != null)
//        {
//            string rideMutateColorParam = rideInfo.dyeCase == null ? "" : ModelManager.Mount.GetDyeColorStr(rideInfo.dyeCase);
//            int rideOrnamentId = rideInfo.ornamentId;
//            string rideOrnamentColorParam = rideInfo.ornamentDyeCase == null ? "" : ModelManager.Mount.GetOrnamentDyeColorStr(rideInfo.ornamentDyeCase);
//
//            _modelStyleInfo.rideId = rideInfo.mountId;
//            _modelStyleInfo.rideMutateColorParam = rideMutateColorParam;
//            _modelStyleInfo.rideOrnamentId = rideOrnamentId;
//            _modelStyleInfo.rideOrnamentColorParam = rideOrnamentColorParam;
//            _modelStyleInfo.rideMaxEffect = rideLV >= ModelManager.Mount.GetRideShowEffectGradLimit();
//            _modelStyleInfo.rideEffect = true;   //特效开关
//        }
//        UpdateModel();
//    }

    //显示时装
//    public void TalentSetupModel(GeneralCharactor charactor, List<int> fashionList = null, RideMountNotify rideInfo = null,
//                                 int rideLV = 0, PlayerDto playerDto = null, int weaponId = 0, int weaponEffId = 0, int pHallowSpriteId = 0)
//    {
//        _modelStyleInfo = modelStyleInfo.ToInfo(charactor);
//        _modelStyleInfo.weaponId = weaponId;
//        _modelStyleInfo.weaponEffId = weaponEffId;
//        _modelStyleInfo.hallowSpriteId = pHallowSpriteId;
//        _modelStyleInfo.useFashionDefaultModel = true;
//        _modelStyleInfo.SetupFashionIds(fashionList);
//
//        string rideMutateColorParam = rideInfo.dyeCase == null ? "" : ModelManager.Mount.GetDyeColorStr(rideInfo.dyeCase);
//        int rideOrnamentId = rideInfo.ornamentId;
//        string rideOrnamentColorParam = rideInfo.ornamentDyeCase == null ? "" : ModelManager.Mount.GetOrnamentDyeColorStr(rideInfo.ornamentDyeCase);
//
//        _modelStyleInfo.rideId = rideInfo.mountId;
//        _modelStyleInfo.rideMutateColorParam = rideMutateColorParam;
//        _modelStyleInfo.rideOrnamentId = rideOrnamentId;
//        _modelStyleInfo.rideOrnamentColorParam = rideOrnamentColorParam;
//        _modelStyleInfo.rideMaxEffect = rideLV >= ModelManager.Mount.GetRideShowEffectGradLimit();
//        _modelStyleInfo.rideEffect = true;   //特效开关
//        UpdateRotation(new Vector3(0, 0, 0));
//        UpdateModel();
//    }

    public void SetupModel(int characterId, PlayerDressInfoDto dressInfo)
    {
        GeneralCharactor charactor = DataCache.getDtoByCls<GeneralCharactor>(characterId);
        if (charactor != null && dressInfo != null)
            SetupModelByDressInfo(charactor, dressInfo);
        else
            CleanUpModel();
    }

    public void SetupModel(int modelId, int mutateId = 0, string mutateColor = "")
    {
        _modelStyleInfo = new ModelStyleInfo();
        _modelStyleInfo.defaultModelId = modelId;
        if (modelId == 719 && mutateId == 726)
        {
            //针对孙悟空模型的特殊处理
            _modelStyleInfo.mainTexture = mutateId;
            _modelStyleInfo.mutateTexture = 0;
        }
        else
        {
            _modelStyleInfo.mainTexture = modelId;
            _modelStyleInfo.mutateTexture = mutateId;
        }
        _modelStyleInfo.mutateColorParam = mutateColor;
        UpdateModel();
    }

//    public void SetupRideModel(RideMountNotify rideInfo, int rideLV = 0, PlayerDto playerDto = null)
//    {
//        string rideMutateColorParam = rideInfo.dyeCase == null ? "" : ModelManager.Mount.GetDyeColorStr(rideInfo.dyeCase);
//        int rideOrnamentId = rideInfo.ornamentId;
//        string rideOrnamentColorParam = rideInfo.ornamentDyeCase == null ? "" : ModelManager.Mount.GetOrnamentDyeColorStr(rideInfo.ornamentDyeCase);
//
//        SetupRideModel(rideInfo.mountId, rideLV, playerDto, rideMutateColorParam, rideOrnamentId, rideOrnamentColorParam);
//    }

    public void SetupRideModel(int rideModelId, int rideLV = 0, PlayerDto playerDto = null, string rideMutateColorParam = "", int rideOrnamentId = 0, string rideOrnamentColorParam = "")
    {
        if (playerDto != null)
        {
            _modelStyleInfo = ModelStyleInfo.ToInfo(playerDto.charactor);
            _modelStyleInfo.weaponEffId = playerDto.dressInfo.weaponEffect;
            GameDebuger.TODO(@"_modelStyleInfo.weaponId = ModelManager.Backpack.GetCurrentWeaponModel();
            _modelStyleInfo.hallowSpriteId = ModelManager.Backpack.GetCurrentHallowSpriteId();");
            _modelStyleInfo.mutateTexture = 0;
            _modelStyleInfo.mutateColorParam = PlayerModel.GetDyeColorParams(playerDto.dressInfo);
        }
        else
        {
            _modelStyleInfo = new ModelStyleInfo();
        }
        _modelStyleInfo.rideId = rideModelId;
        _modelStyleInfo.rideMutateColorParam = rideMutateColorParam;
        _modelStyleInfo.rideOrnamentId = rideOrnamentId;
        _modelStyleInfo.rideOrnamentColorParam = rideOrnamentColorParam;
        GameDebuger.TODO(@"_modelStyleInfo.rideMaxEffect = rideLV >= ModelManager.Mount.GetRideShowEffectGradLimit();");
        _modelStyleInfo.rideEffect = true;   //特效开关
        UpdateRotation(new Vector3(0, -45, 0));
        UpdateModel();
    }

    public void UpdateOrnamentColorParam(string colorParams)
    {
        if (ModelDisplayer != null)
            ModelDisplayer.UpdateOrnamentColorParam(colorParams);
    }

    public void UpdateModelHSV(string colorParams, int mutateTexture)
    {
        if (ModelDisplayer != null)
            ModelDisplayer.UpdateModelHSV(colorParams, mutateTexture);
    }

    public void UpdateRideOrnamentColorParam(string colorParams)
    {
        if (ModelDisplayer != null)
            ModelDisplayer.UpdateRideOrnamentColorParam(colorParams);
    }

    public void UpdateRideModelHSV(string colorParams)
    {
        if (ModelDisplayer != null)
            ModelDisplayer.UpdateRideModelHSV(colorParams);
    }

    private void UpdateModel()
    {
        if (_modelStyleInfo == null)
            return;
        Model modelInfo = null;
        if (_modelStyleInfo.rideId > 0)
        {
            modelInfo = DataCache.getDtoByCls<Model>(_modelStyleInfo.rideId);
        }
        else
        {
            modelInfo = DataCache.getDtoByCls<Model>(_modelStyleInfo.GetModelInfoId());
        }
        if (modelInfo != null)
        {
            _modelStyleInfo.ModelScale = modelInfo.uiScale;
            _yOffset = modelInfo.uiPos;
        }
        _modelStyleInfo.weaponEffLv = 3;
        _modelStyleInfo.showShadow = _showShadow;
        _modelStyleInfo.shadowScale = 2f;
        if (ModelDisplayer != null)
        {
            ModelDisplayer.SetLookInfo(_modelStyleInfo);
        }
    }

    private void OnLoadModelFinish()
    {
        ModelDisplayer.ClearMountingPoint(ModelHelper.Mount_hud);
        SetModelOffset(_yOffset);

        ModelDisplayer.UpdateRotation(_defaultRotation);

        PlayAnimation(_defaultAnimate);
    }

    public void UpdateRotation(Vector3 rotation)
    {
        _defaultRotation = rotation;
        if (ModelDisplayer != null)
        {
            ModelDisplayer.UpdateRotation(rotation);
        }
    }

    public void CleanUpModel()
    {
        CleanUpCustomAnimations();
        if (ModelDisplayer != null)
            ModelDisplayer.Clear();

        GameDebuger.TODO(@"GameEventCenter.RemoveListener(GameEvent.Backpack_OnWeaponModelChange, UpdateWeapon);
        GameEventCenter.RemoveListener(GameEvent.Backpack_OnHallowSpriteChange, UpdateHallowSprite);");
    }

    public void CleanUpCustomAnimations()
    {
        _customAnimateList = null;
        _customAnimateIndex = 0;
    }

    public void SetUpCreateRoleAnimationList(ModelHelper.AnimType[] list)
    {
        if(list.Length>0)
            PlayAnimation(list[0]);
        _customAnimateList = list;
        if (_customAnimateIndex == -1)
        {
            _customAnimateIndex = 0;
        }
    }

    public void SetupFasionAnimationList(ModelHelper.AnimType initAnimType)
    {
        PlayAnimation(initAnimType);
        _customAnimateList = FashionAnimationClipList.ToArray();
        _customAnimateIndex = FashionAnimationClipList.IndexOf(initAnimType);
        if (_customAnimateIndex == -1)
        {
            _customAnimateIndex = 0;
        }
    }

    public void UpdateWeapon(int weaponModelId)
    {
        if (ModelDisplayer != null)
            ModelDisplayer.UpdateWeapon(weaponModelId);
    }

    public void UpdateHallowSprite(int pHallowSprite)
    {
        if (ModelDisplayer != null)
            ModelDisplayer.UpdateHallowSprite(pHallowSprite);
    }

    public void PlayAnimation(ModelHelper.AnimType clip)
    {
        _defaultAnimate = clip;
        if (ModelDisplayer != null)
        {
            ModelDisplayer.PlayAnimation(clip);
        }
    }

    private void OnDragModel()
    {
        if (ModelDisplayer == null)
            return;

        Vector2 delta = UICamera.currentTouch.delta;
        ModelDisplayer.Rotate(-Time.deltaTime * delta.x * 30f);
    }

    private void OnClickModel()
    {
        if (ModelDisplayer.isRiding
            || ModelDisplayer == null
            || _isPlaying)  //骑乘时点击不播放攻击动画
            return;

        if (!_isPlaying)
        {
            _isPlaying = true;
            JSTimer.Instance.SetupCoolDown(taskName, 0.5f, null, ()=>_isPlaying = false);
        }

        if (_customAnimateList != null)
        {
            _customAnimateIndex++;
            if (_customAnimateIndex >= _customAnimateList.Length)
            {
                _customAnimateIndex = 0;
            }
            var clip = _customAnimateList[_customAnimateIndex];
            ModelDisplayer.PlayAnimation(clip);
        }
        else
        {
            ModelDisplayer.PlayAnimation(DefaultAnimationClipList.Random());
        }

    }

    public void SetBoxColliderEnabled(bool b)
    {
        if (_mUITexture != null)
        {
            BoxCollider box = _mUITexture.GetComponent<BoxCollider>();
            if (box != null)
                box.enabled = b;
        }
    }

    public void SetBoxCollider(float width, float height)
    {
        if (_mUITexture != null)
        {
            BoxCollider box = _mUITexture.GetComponent<BoxCollider>();
            if (box != null)
            {
                box.size = new Vector3(width, height, 0f);
            }
        }
    }


    public void SetUITextureDepth(int depth)
    {
        if(_mUITexture != null)
            _mUITexture.depth = depth;
    }


    public void SetUITextureShader(string shaderName)
    {
        Shader shader = AssetPipeline.AssetManager.Instance.FindShader(shaderName);
        if (shader != null)
            _mUITexture.shader = shader;
    }

    public void SetOrthographic(float size)
    {
        _orthographicSize = size;
        _useOrthographic = true;
        if (_mCam != null)
        {
            _mCam.orthographic = true;
            _mCam.orthographicSize = size;
        }
    }

    public void SetModelOffset(float offsetY)
    {
        _yOffset = offsetY;
        if (_mCamTrans != null)
            _mCamTrans.localPosition = CAMERA_POS + new Vector3(0f, -_yOffset, 0f);
    }

    public void SetModelScale(float scale)
    {
        _modelStyleInfo.ModelScale = scale;

        if (ModelDisplayer != null)
            ModelDisplayer.UpdateScale(scale);
    }

    public void SetPosition(Vector3 pos)
    {
        this.transform.localPosition = pos;
    }
}