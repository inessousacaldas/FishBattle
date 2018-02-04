// **********************************************************************
// Copyright (c) 2016 cilugame. All rights reserved.
// File     : ModelDisplayer.cs
// Author   : senkay <senkay@126.com>
// Created  : 1/16/2016 
// Porpuse  : 模型加载显示器
// **********************************************************************
//
using System;
using System.Collections.Generic;
using AppDto;
using AssetPipeline;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

public class ModelDisplayer
{
    //记录当前角色材质Alpha值
    private bool _activeFlag = true;
    private ModelHelper.AnimType _defaultAnimate = ModelHelper.AnimType.invalid;
    private Vector3 _defaultRotation = Vector3.zero;

    // 增加显示数量控制,让头和身体一起显示出来
    private int _loadingCount;
    private Animator _mAnimator;
    private GameObject _mModelGO;
    private Transform _mModelTrans;

    private int _loadingRideCount;
    //加载中的坐骑数量， >0 有坐骑在加载中， <=0 没有加载坐骑

    private readonly List<GameObject> _gameobjectList;
    private readonly List<Animator> _animatorList;
    private readonly List<Animator> _weaponAnimatorList;
    private readonly Dictionary<string, AssetManager.AssetHandler> _assetHandlerDic;
    private readonly List<Transform> _transformList;
    private readonly GameObject _root;
    private readonly Action _onLoadModelFinish;
    private readonly ModelArtistic modelArtistic;

    private bool _activeSurroundEffFlag = true;
    private GameObject _surroundEffBottom;
    private bool _activeFootPringEffFlag = false;
    private ParticleSystem[] _footprintEffects;
    private List<Renderer> _footpringRenderers;

    //神器器灵相关
    private GameObject mSoulEffect;
    private bool mActiveSoulEffFlag = true;
    private JSTimer.TimerTask mSoulAnimatorTimer;
    private Animator mSoulAnimator;

    private Animator SoulAnimator
    {
        get
        {
            if (null == mSoulAnimator)
            {
                if (null != mSoulEffect)
                {
                    mSoulAnimator = mSoulEffect.GetComponent<Animator>();
                }
            }
            return mSoulAnimator;
        }
    }

    //ui模式
    private bool _uiMode = false;

    //坐骑相关
    private Animator _rideAnimator;
    private GameObject _rideGO;
    //坐骑对象
    private Transform _rideRoleGO;
    //骑坐骑的角色的父对象
    private Transform _rideassengerGO;
    //共乘点
    public bool isRiding
    {
        get { return _rideGO != null; }
    }

    //是否正在加载模型
    public bool isLoading
    {
        get { return _loadingCount != 0 || _loadingRideCount != 0; }
    }

    //骑坐骑的角色的父对象
    public Transform RideRoleGo
    {
        get { return _rideRoleGO; }
    }

    //共乘点
    public Transform RideassengerGO
    {
        get { return _rideassengerGO; }
    }

    private bool occlusitionActive;
    public ModelDisplayer(GameObject root, Action onLoadModelFinish, bool uiMode = false, bool occlusitionActive = false)
    {
        _root = root;
        _onLoadModelFinish = onLoadModelFinish;
        _uiMode = uiMode;
        _animatorList = new List<Animator>();
        _weaponAnimatorList = new List<Animator>();
        _transformList = new List<Transform>();
        _gameobjectList = new List<GameObject>();
        _assetHandlerDic = new Dictionary<string, AssetManager.AssetHandler>();
        modelArtistic = new ModelArtistic(root);
        modelArtistic.SetOcclusion(occlusitionActive);
        this.occlusitionActive = occlusitionActive;
    }

    public ModelStyleInfo modelStyleInfo { get; private set; }

    private ModelStyleInfo _waitingInfo;
    public void SetLookInfo(ModelStyleInfo lookInfo)
    {
        if (_loadingCount > 0)
        {
            //还在加载中,则暂存新的，等旧的加载完毕后再启动
            _waitingInfo = lookInfo;
        }
        else
        {
            _waitingInfo = null;
            modelStyleInfo = lookInfo;

            LoadWholeModel();
        }
    }

    private void LoadWholeModel()
    {
        Clear();

        LoadRideModel(modelStyleInfo.rideId, "ride_pet_801_Model");
    }

    private void LoadStyleModel()
    {
        //如果没有含有形象模型， 则不加载
        if (!modelStyleInfo.HasStyleModel)
        {
        	GameUtil.SafeRun(_onLoadModelFinish);
            modelArtistic.RefreshRenderList();
            return;
        }

        if (modelStyleInfo.IsTransformModel)
        {
            _loadingCount = 1;
            LoadModel(modelStyleInfo.DefaultModelResKey, true,
                ModelHelper.GetCharacterPrefabPath(ModelHelper.DefaultModelId));
        }
        else if (modelStyleInfo.IsFashionModel || modelStyleInfo.useFashionDefaultModel)
        {
            string defaultModelBody = "pet_" + modelStyleInfo.defaultModelId + "_01_body";
            if (!AssetManager.Instance.ContainBundleName(defaultModelBody, ResGroup.Model))
            {
                defaultModelBody = "pet_1002_01_body";
            }

            string defaultModel_head = "pet_" + modelStyleInfo.defaultModelId + "_01_head";
            if (!AssetManager.Instance.ContainBundleName(defaultModel_head, ResGroup.Model))
            {
                defaultModel_head = "pet_1002_01_head";
            }

            // 增加显示数量控制,让头和身体一起显示出来
            _loadingCount = 2;
            LoadModel(modelStyleInfo.FashionBodyResKey, true, defaultModelBody);
            LoadModel(modelStyleInfo.FashionHeadResKey, false, defaultModel_head);

        }
//        D1的实现
//        else if (modelStyleInfo.modelOfHead > 0 && modelStyleInfo.modelOfCloth > 0)
//        {
//
//            string defaultModelBody = "pet_" + modelStyleInfo.defaultModelId + "_01_body";
//            if (!AssetManager.Instance.ContainBundleName(defaultModelBody, ResGroup.Model))
//            {
//                defaultModelBody = "pet_1002_01_body";
//            }
//
//
//            string defaultModel_head = "pet_" + modelStyleInfo.defaultModelId + "_01_head";
//            if (!AssetManager.Instance.ContainBundleName(defaultModel_head, ResGroup.Model))
//            {
//                defaultModel_head = "pet_1002_01_head";
//            }
//
//            FashionDress tFashionDress = DataCache.getDtoByCls<FashionDress>(ModelStyleInfo.modelOfHead);
//            string tModelOfHead = null != tFashionDress ? tFashionDress.modelId : string.Empty;
//            tFashionDress= DataCache.getDtoByCls<FashionDress>(ModelStyleInfo.modelOfCloth);
//            string tModelOfCloth =  null != tFashionDress ? tFashionDress.modelId : string.Empty;
//            if (string.IsNullOrEmpty(tModelOfHead))
//            {
//                GameDebuger.LogError(string.Format("[Error]配置有误， FashionDress 中不存在指定配置的头部模型({0})，将用代替资源（{1}）代替！", ModelStyleInfo.modelOfHead, defaultModel_head));
//                tModelOfHead = defaultModel_head;    
//            }
//            if (string.IsNullOrEmpty(tModelOfCloth))
//            {
//                GameDebuger.LogError(string.Format("[Error]配置有误， FashionDress 中不存在指定配置的身体模型({0})，将用代替资源（{1}）代替！", ModelStyleInfo.modelOfCloth, defaultModelBody));
//                tModelOfCloth = defaultModelBody;    
//            }
//
//            // 增加显示数量控制,让头和身体一起显示出来
//            _loadingCount = 2;
//            LoadModel(tModelOfCloth, true, defaultModelBody);
//            LoadModel(tModelOfHead, false, defaultModel_head);
//
//        }
        else
        {
            _loadingCount = 1;
            LoadModel(modelStyleInfo.DefaultModelResKey, true,
                ModelHelper.GetCharacterPrefabPath(ModelHelper.DefaultModelId));
        }
    }

    private void LoadModel(string modelResKey, bool isBody, string defaultModel)
    {
        if (string.IsNullOrEmpty(modelResKey))
        {
            modelResKey = defaultModel;
        }

        if (!AssetManager.Instance.ContainBundleName(modelResKey, ResGroup.Model))
        {
            GameDebuger.LogError("ModelDisplayer Use Default model " + modelResKey);
            modelResKey = defaultModel;
        }

        AssetManager.AssetHandler handler = null;
        handler = ResourcePoolManager.Instance.SpawnModelAsync(modelResKey, go =>
            {
                OnLoadModelFinish(modelResKey, go, isBody);
            },
            () =>
            {
                OnLoadError(modelResKey);
            });

        if (handler != null)
            _assetHandlerDic.Add(modelResKey, handler);
    }

    private void OnLoadError(string modelResKey)
    {
        _loadingCount--;
        _assetHandlerDic.Remove(modelResKey);

        if (_loadingCount <= 0)
        {
            if (_waitingInfo != null)
            {
                //加载完毕后， 如果有在等待的加载， 则进入等待加载
                SetLookInfo(_waitingInfo);
            }
        }
    }

    private void OnLoadModelFinish(string modelResKey, Object obj, bool isBody)
    {
        _loadingCount--;
        _assetHandlerDic.Remove(modelResKey);

        var modelGo = obj as GameObject;
        if (modelGo == null)
            return;

        if (_root == null)
        {
            Debug.LogError("OnLoadModelFinish _root == null");
            ResourcePoolManager.Instance.DespawnModel(modelGo);
            return;
        }

        if (modelStyleInfo == null)
        {
            Debug.LogError("OnLoadModelFinish _petLookInfo == null");
            ResourcePoolManager.Instance.DespawnModel(modelGo);
            return;
        }

        if (isBody)
        {
            if (_mModelGO != null)
            {
                ResourcePoolManager.Instance.DespawnModel(_mModelGO);
                _mModelGO = null;
            }
        }

        GameObjectExt.AddPoolChild(_root, modelGo);
        modelGo.SetActive(_activeFlag && _loadingCount <= 0);

        _gameobjectList.Add(modelGo);
        var modelTrans = modelGo.transform;
        _transformList.Add(modelTrans);
        //设置模型旋转
        UpdateRotation(_defaultRotation);
        //设置模型缩放
        UpdateScale(modelStyleInfo.ModelScale);

        if (_loadingCount <= 0)
        {
            SetActive(_activeFlag);
        }

        //设置播放的动作
        var playerAnimation = GetPlayerAnimation();
        var animator = modelGo.GetComponent<Animator>();
        if (animator != null)
        {
            _animatorList.Add(animator);
            ModelHelper.PlayAnimation(animator, playerAnimation, false);
        }

        if (isBody)
        {
            _mModelGO = modelGo;
            _mModelTrans = _mModelGO.transform;
            _mAnimator = animator;
            if (_mAnimator != null)
                _mAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            if (!modelStyleInfo.HasRide)
            {
                SetShadowActive(modelStyleInfo.showShadow, modelStyleInfo.shadowScale);
            }

            ModifyModelMount(_mModelGO, playerAnimation);

            //  环绕特效
            _surroundEffBottom = null;
            GameDebuger.TODO(@"if (modelStyleInfo.FashionSurroundEffect != null)
            {
                ModelHelper.SetSurroundEffectActive(_mModelGO, modelStyleInfo.FashionSurroundEffect.modelId, OnLoadSurroundEffect);
            }
            else");
            {
                ModelHelper.SetSurroundEffectActive(_mModelGO, null);
            }


            //  足迹特效
            _footprintEffects = null;
            _footpringRenderers = null;
            GameDebuger.TODO(@"if (modelStyleInfo.FashionFootprint != null)
            {
                ModelHelper.SetFootprintActive(_mModelGO, modelStyleInfo.FashionFootprint.modelId, OnLoadFootprintEffect);
            }
            else");
            {
                ModelHelper.SetFootprintActive(_mModelGO, null);
            }

            //  器灵特效
            UpdateHallowSprite();
        }

        GameDebuger.TODO(@"ModelHelper.ToggleUnitEffect(modelGo, ModelManager.SystemData.unitEffectToggle);");

        if (isBody)
        {
            if (modelStyleInfo.IsFashionModel)
            {
                //装备了时装手持道具
                GameDebuger.TODO(@"if (modelStyleInfo.FashionWeapon != null)
                {
                    UpdateModelWeapon(modelGo, modelStyleInfo.defaultModelId, 0, modelStyleInfo.weaponEffId, onLoadFinish: OnLoadWeaponFinish);
                    ModelHelper.UpdateHandModel(modelGo, modelStyleInfo.FashionBodyResKey, modelStyleInfo.FashionWeapon.modelId);
                }
                else");
                {
                    UpdateModelWeapon(modelGo, modelStyleInfo.defaultModelId, modelStyleInfo.weaponId,
                        modelStyleInfo.weaponEffLv, modelStyleInfo.weaponEffId, _uiMode, OnLoadWeaponFinish);
                }
            }
            else if (modelStyleInfo.IsTransformModel)
            {
                //变身卡模型下,隐藏配饰，恢复原贴图
                if (modelStyleInfo.TransformModelId > 0)
                {
                    ModelHelper.SetPetLook(modelGo, modelStyleInfo.mainTexture, modelStyleInfo.mutateTexture, modelStyleInfo.mutateColorParam, modelStyleInfo.ornamentId, modelStyleInfo.ornamentColorParam, OnMatChange);
                }
            }
            else
            {
                //普通模型下,设置其染色参数和变异贴图
                ModelHelper.SetPetLook(modelGo, modelStyleInfo.mainTexture, modelStyleInfo.mutateTexture,
                    modelStyleInfo.mutateColorParam, modelStyleInfo.ornamentId, modelStyleInfo.ornamentColorParam, OnMatChange);
                UpdateModelWeapon(modelGo, modelStyleInfo.defaultModelId, modelStyleInfo.weaponId, modelStyleInfo.weaponEffLv, modelStyleInfo.weaponEffId, _uiMode, OnLoadWeaponFinish);
            }
        }

        if (_loadingCount <= 0)
        {
            if (_waitingInfo != null)
            {
                //加载完毕后， 如果有在等待的加载， 则进入等待加载
                SetLookInfo(_waitingInfo);
            }
            else
            {
                if (_rideRoleGO != null)
                {
                    //把人物移到坐骑锚点
                    for (int i = 0, n = _gameobjectList.Count; i < n; i++)
                    {
                        GameObjectExt.AddPoolChild(_rideRoleGO.gameObject, _gameobjectList[i]);
                    }
                }
                LoadingFinish();
            }
        }
    }

    private void UpdateModelWeapon(
        GameObject modelGo
        , int actorModelId
        , int wpId
        , int withEffLv = 1
        , int weaponEffId = 0
        , bool uiMode = false
        , Action<GameObject> onLoadFinish = null)
    {
        _weaponAnimatorList.Clear();
        Action<GameObject> _onLoadFinish = delegate(GameObject go)
        {
            if (go == null) return;
            var animator = go.GetComponent<Animator>();
            if (animator != null)
            {
                //设置播放的动作
                var playerAnimation = GetPlayerAnimation();
                _weaponAnimatorList.Add(animator);
                ModelHelper.PlayAnimation(animator, playerAnimation, false); 
            }

            GameUtil.SafeRun(onLoadFinish, go);
        };
        ModelHelper.UpdateModelWeapon(modelGo, actorModelId, wpId, withEffLv, weaponEffId, uiMode, _onLoadFinish);
    }

    private void LoadingFinish()
    {
        OnMatChange();
        if (_onLoadModelFinish != null)
        {
            _onLoadModelFinish();
        }
    }

    private void OnMatChange()
    {
        modelArtistic.RefreshRenderList();

    }
    private void OnLoadWeaponFinish(GameObject wpGo)
    {
        OnMatChange();
    }

    public void UpdateHallowSprite()
    {
        //  器灵特效
        mSoulEffect = null;
        if (CanShowHallowSprite)
        {
            ModelHelper.SetSoulEffectActive(_mModelGO, modelStyleInfo.SoulModelId, OnLoadSoulEffect);
        }
        else
        {
            ModelHelper.SetSoulEffectActive(_mModelGO, null);
        }
    }

    private bool CanShowHallowSprite
    {
        get
        {
            if (null != modelStyleInfo)
            {
                GameDebuger.TODO(@"if (null == modelStyleInfo.FashionWeapon) //需求：有手持时装时不显示器灵。");
                {
                    return !string.IsNullOrEmpty(modelStyleInfo.SoulModelId);
                }
            }
            return false;
        }
    }

    #region Effect

    private void SetSurroundEffectBottomActive(bool active)
    {
        GameDebuger.TODO(@"if (modelStyleInfo == null || modelStyleInfo.FashionSurroundEffect == null)");
        if (modelStyleInfo == null /**|| modelStyleInfo.FashionSurroundEffect == null*/)
        {
            return;
        }

        GameDebuger.TODO(@"if (_uiMode == false && ModelManager.SystemData.fashionEffectToggle == false)");
        if (_uiMode == false /**&& ModelManager.SystemData.fashionEffectToggle == false*/)
        {
            return;
        }

        if (_surroundEffBottom == null)
        {
            _activeSurroundEffFlag = active;
        }
        else
        {
            //Debug.LogError("SurroundEffect: " + active);
            _surroundEffBottom.SetActive(active);
        }
    }

    public void SetSoulEffectActive(bool active)
    {
        if (modelStyleInfo == null || string.IsNullOrEmpty(modelStyleInfo.SoulModelId))
        {
            return;
        }

        GameDebuger.TODO(@"if (_uiMode == false && ModelManager.SystemData.hallowSpriteEffectToggle == false)");
        if (_uiMode == false /**&& ModelManager.SystemData.hallowSpriteEffectToggle == false*/)
        {
            return;
        }

        if (mSoulEffect == null)
        {
            mActiveSoulEffFlag = active;
        }
        else
        {
            //Debug.LogError("SurroundEffect: " + active);
            mSoulEffect.SetActive(active);
        }
    }

    private void SetFootprintActive(bool active)
    {
        GameDebuger.TODO(@"if (modelStyleInfo == null || modelStyleInfo.FashionFootprint == null)");
        if (modelStyleInfo == null /**|| modelStyleInfo.FashionFootprint == null*/)
        {
            return;
        }

        _activeFootPringEffFlag = active;
        if (_footprintEffects != null && _footprintEffects.Length > 0)
        {
            //Debug.LogError("Footprint: " + active);
            for (int i = 0; i < _footprintEffects.Length; i++)
            {
                if (_footprintEffects[i] != null)
                {
                    if (active)
                    {
                        _footprintEffects[i].Play();
                    }
                    else
                    {
                        _footprintEffects[i].Stop();
                    }
                }
            }
        }

        if (_footpringRenderers != null && _footpringRenderers.Count > 0)
        {
            for (int i = 0; i < _footpringRenderers.Count; i++)
            {
                if (_footpringRenderers[i] is TrailRenderer)
                {
                    _footpringRenderers[i].enabled = active;
                }
            }
        }
    }

    private void OnLoadSurroundEffect(GameObject effGo)
    {
        if (_root == null)
        {
            Debug.LogError("OnLoadSurroundEffect _root == null");
            ResourcePoolManager.Instance.DespawnEffect(effGo);
            return;
        }

        if (modelStyleInfo == null)
        {
            Debug.LogError("OnLoadSurroundEffect modelStyleInfo == null");
            ResourcePoolManager.Instance.DespawnEffect(effGo);
            return;
        }

        GameDebuger.TODO(@"if (modelStyleInfo.FashionSurroundEffect == null)
        {
            Debug.LogError('OnLoadSurroundEffect FashionSurroundEffect == null');
            ResourcePoolManager.Instance.DespawnEffect(effGo);
            return;
        }");

        if (_uiMode)
        {
            effGo.tag = GameTag.Tag_Untagged;
        }

        effGo.SetActive(true);

        var effTrans = effGo.transform;
        for (int i = 0, imax = effTrans.childCount; i < imax; ++i)
        {
            var child = effTrans.GetChild(i);
            GameDebuger.TODO(@"if (child.name.Contains(modelStyleInfo.FashionSurroundEffect.modelId) && !child.name.Contains('_2'))
            {
                if (_uiMode == false)
                {
                    child.tag = GameTag.Tag_UnitEffect;
                    ModelHelper.ToggleFashionEffect(child.gameObject, ModelManager.SystemData.fashionEffectToggle);
                }
            }");
        }

        GameDebuger.TODO(@"var bottom = effGo.transform.Find(modelStyleInfo.FashionSurroundEffect.modelId + '_2');
        if (bottom != null)
        {
            _surroundEffBottom = bottom.gameObject;
            bool activeSurroundEffFlag = _activeSurroundEffFlag;
            if (_uiMode == false) //非ＵＩ
            {
                _surroundEffBottom.SetActive(true);
                bottom.tag = GameTag.Tag_UnitEffect;
                if (_activeSurroundEffFlag)
                {
                    ModelHelper.ToggleFashionEffect(bottom.gameObject, ModelManager.SystemData.fashionEffectToggle);
                }
                else
                {
                    ModelHelper.ToggleFashionEffect(bottom.gameObject, false);
                }
            }
            else
            {
                _surroundEffBottom.SetActive(activeSurroundEffFlag);
            }
        }
        else
        {
            GameDebuger.Log('Can not find SurroundEffect bottom');
        }");
    }

    private void OnLoadSoulEffect(GameObject effGo)
    {
        if (_root == null)
        {
            Debug.LogError("OnLoadSoulEffect _root == null");
            ResourcePoolManager.Instance.DespawnEffect(effGo);
            return;
        }

        if (modelStyleInfo == null)
        {
            Debug.LogError("OnLoadSoulEffect modelStyleInfo == null");
            ResourcePoolManager.Instance.DespawnEffect(effGo);
            return;
        }

        mSoulEffect = effGo;

        if (_uiMode)
        {
            mSoulEffect.tag = GameTag.Tag_Untagged;
        }

        mSoulEffect.SetActive(true);

        foreach (Transform child in mSoulEffect.transform)
        {
            if (child.name.Contains(modelStyleInfo.SoulModelId))
            {
                if (_uiMode == false)
                {
                    child.tag = GameTag.Tag_UnitEffect;
                    GameDebuger.TODO(@"ModelHelper.ToggleHallowSpriteEffect(child.gameObject,
                        ModelManager.SystemData.hallowSpriteEffectToggle);");
                }
            }
        }

        Model tModel = DataCache.getDtoByCls<Model>(modelStyleInfo.hallowSpriteId);
        if (null != tModel)
        {
            mSoulEffect.transform.localScale = new Vector3(tModel.scale, tModel.scale, tModel.scale);
        }
        UpdateSoulAction();
    }

    private void UpdateSoulAction()
    {
        if (null != mSoulEffect)
        {
            if (null == mSoulAnimatorTimer)
            {
                mSoulAnimatorTimer = JSTimer.Instance.SetupTimer("SoulAnima_" + mSoulEffect.GetInstanceID(),
                    UpdateSoulAction, GeneralUnit.GetAnimateRandomTime());
            }
            else
            {
                mSoulAnimatorTimer.Reset(UpdateSoulAction, GeneralUnit.GetAnimateRandomTime(), false);
                ModelHelper.PlayAnimation(SoulAnimator, ModelHelper.AnimType.show, false, null, true);
            }
        }
    }

    private void ClearSoul()
    {
        if (null != mSoulEffect)
        {
            ResourcePoolManager.Instance.DespawnEffect(mSoulEffect);
            mSoulEffect = null;
        }
        mSoulAnimator = null;
        if (null != mSoulAnimatorTimer)
        {
            mSoulAnimatorTimer.Cancel();
            mSoulAnimatorTimer = null;
        }
    }

    private void OnLoadFootprintEffect(GameObject effGo)
    {
        if (_root == null)
        {
            Debug.LogError("OnLoadFootprintEffect _root == null");
            ResourcePoolManager.Instance.DespawnEffect(effGo);
            return;
        }

        if (modelStyleInfo == null)
        {
            Debug.LogError("OnLoadFootprintEffect _petLookInfo == null");
            ResourcePoolManager.Instance.DespawnEffect(effGo);
            return;
        }

        GameDebuger.TODO(@"if (modelStyleInfo.FashionFootprint == null)
        {
            Debug.LogError('OnLoadSurroundEffect FashionFootprint == null');
            ResourcePoolManager.Instance.DespawnEffect(effGo);
            return;
        }");

        if (_uiMode)
        {
            effGo.tag = GameTag.Tag_Untagged;
        }
        else
        {
            effGo.tag = GameTag.Tag_UnitEffect;
        }

        if (_uiMode == false)
        {
            GameDebuger.TODO(@"ModelHelper.ToggleFashionEffect(effGo, ModelManager.SystemData.fashionEffectToggle);");
        }

        _footprintEffects = effGo.GetComponentsInChildren<ParticleSystem>(true);
        if (_footprintEffects.Length == 0)
        {
            Debug.LogError("Has nothing find footprint ParticleSystem");
        }
        else
        {
            for (int i = 0; i < _footprintEffects.Length; i++)
            {
                _footprintEffects[i].playOnAwake = _activeFootPringEffFlag;
                if (_activeFootPringEffFlag)
                {
                    _footprintEffects[i].Play();
                }
                else
                {
                    _footprintEffects[i].Stop();
                }
            }
        }

        var renderers = effGo.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
        {
            Debug.LogError("Has nothing find footprint Renderer");
        }
        else
        {
            _footpringRenderers = new List<Renderer>(renderers.Length);
            for (int i = 0; i < renderers.Length; i++)
            {
                if (!(renderers[i] is ParticleSystemRenderer))
                {
                    renderers[i].enabled = _activeFootPringEffFlag;
                    _footpringRenderers.Add(renderers[i]);
                }
            }
        }

        GameDebuger.TODO(@"if (ItemHelper.IsFootDirection(modelStyleInfo.FashionFootprint))");
        {
            GameObject rotationRoot = _root.name.Contains("ModelRender") ? _mModelGO : _root;
            if (rotationRoot != null)
            {
                ParticleRotationSync prs = effGo.GetMissingComponent<ParticleRotationSync>();
                prs.target = rotationRoot.transform;
            }
        }
    }

    //加载坐骑模型
    private void LoadRideModel(int rideId, string defaultModel)
    {
        if (_root == null)
            return;

        //处理坐骑显示
        if (rideId > 0)
        {
            string rideName = "ride_pet_" + rideId.ToString();

            //如果当前坐骑是目标坐骑， 则跳过替换处理
            if (_rideGO != null && _rideGO.name.Contains(rideName))
            {
                LoadStyleModel();
                return;
            }

            string modelResKey = rideName;

            if (!AssetManager.Instance.ContainBundleName(modelResKey, ResGroup.Model))
            {
                GameDebuger.LogError("ModelDisplayer.LoadWholeModel Use Default model " + modelResKey);
                modelResKey = defaultModel;
            }

            _loadingRideCount = 1;
            AssetManager.AssetHandler handler = ResourcePoolManager.Instance.SpawnModelAsync(modelResKey, obj =>
                {
                    OnLoadRideModelFinish(modelResKey, obj);
                },
                                                                  () =>
                {
                    OnLoadError(modelResKey);
                });

            if (handler != null)
                _assetHandlerDic.Add(modelResKey, handler);
        }
        else //清空坐骑
        {
            CleanRidePet();
            //modelStyleInfo.ModelScale = 1f;
            LoadStyleModel();
        }
    }

    //移除坐骑， 如果坐骑上有人物， 则把人物下坐骑
    private void CleanRidePet()
    {
        if (_rideGO != null)
        {
            //把人物移回根节点
            for (int i = 0, n = _gameobjectList.Count; i < n; i++)
            {
                GameObjectExt.AddPoolChild(_root, _gameobjectList[i]);
            }

            //回收坐骑
            ResourcePoolManager.Instance.DespawnModel(_rideGO);
        }

        _rideGO = null;
        _rideRoleGO = null;
        _rideassengerGO = null;
    }

    //坐骑模型加载完成
    private void OnLoadRideModelFinish(string modelResKey, GameObject obj)
    {
        --_loadingRideCount;

        _assetHandlerDic.Remove(modelResKey);

        if (obj == null)
            return;

        if (_root == null)
        {
            Debug.LogError("LoadWholeModel _root == null");
            ResourcePoolManager.Instance.DespawnModel(obj);
            return;
        }

        if (modelStyleInfo == null)
        {
            Debug.LogError("LoadWholeModel modelStyleInfo == null");
            ResourcePoolManager.Instance.DespawnModel(obj);
            return;
        }

        GameObject rideGo = obj as GameObject;

        CleanRidePet();

        //显示新加载坐骑
        Transform mountTF = GameObjectExt.GetChildTransform(rideGo.transform, ModelHelper.Mount_ride);
        if (mountTF != null)
        {
            GameObjectExt.AddPoolChild(_root, rideGo);

            _rideRoleGO = mountTF;
        }
        else
        {
            Debug.LogError(string.Format("坐骑锚点不存在，坐骑模型={0}", modelResKey));
        }

        GameDebuger.TODO(@"Transform mountTF2 = GameObjectExt.GetChildTransform(rideGo.transform, ModelHelper.Mount_ride2);
        if (mountTF2 != null)
        {
            _rideassengerGO = mountTF2;
        }");

        _rideGO = rideGo;

        if (_rideGO != null)
        {
            GameDebuger.TODO(@"modelStyleInfo.ModelScale = IsOnSceneFlyMount() ? FlyRideAnimation.skyScale : 1f;");
            modelStyleInfo.ModelScale = /**IsOnSceneFlyMount() ? FlyRideAnimation.skyScale :*/ 1f;
            _rideAnimator = _rideGO.GetComponent<Animator>();
            PlayAnimation(_defaultAnimate);
            SetShadowActive(modelStyleInfo.showShadow, modelStyleInfo.shadowScale);
        }

        ModelHelper.SetRideLook(_rideGO,
            modelStyleInfo.rideMutateColorParam,
            modelStyleInfo.rideOrnamentId,
            modelStyleInfo.rideOrnamentColorParam,
            modelStyleInfo.rideMaxEffect,
            modelStyleInfo.rideEffect);

        UpdateRotation(_defaultRotation);
        UpdateScale(modelStyleInfo.ModelScale);

        SetActive(_activeFlag);

        LoadStyleModel();
    }

    #endregion

    public void UpdateRotation(Vector3 rotation)
    {
        _defaultRotation = rotation;

        if (_rideGO != null)
        {
            //如果有坐骑， 则直接旋转坐骑
            _rideGO.transform.localEulerAngles = rotation;
        }
        else
        {
            //如果没有坐骑， 则旋转人物
            if (_transformList.Count > 0)
            {
                for (int i = 0, n = _transformList.Count; i < n; i++)
                {
                    _transformList[i].localEulerAngles = rotation;
                }
            }
        }
    }

    public void Rotate(float angle)
    {
        if (_rideGO != null)
        {
            //如果有坐骑， 则直接旋转坐骑
            _rideGO.transform.Rotate(_rideGO.transform.up, angle);
        }
        else
        {
            //如果没有坐骑， 则旋转人物
            if (_transformList.Count > 0)
            {
                for (int i = 0, n = _transformList.Count; i < n; i++)
                {
                    _transformList[i].Rotate(_transformList[i].up, angle);
                }
            }
        }
    }

    public void UpdateScale(float scale)
    {
        if (modelStyleInfo == null)
            return;

        modelStyleInfo.ModelScale = scale;


        Vector3 newScale = new Vector3(modelStyleInfo.ModelScale, modelStyleInfo.ModelScale,
                               modelStyleInfo.ModelScale);

        if (_rideGO != null)
        {
            //如果有坐骑， 则直接缩放坐骑
            _rideGO.transform.localScale = newScale;
        }
        else
        {
            //如果没有坐骑， 则缩放人物
            if (_transformList.Count > 0)
            {
                for (int i = 0, n = _transformList.Count; i < n; i++)
                {
                    _transformList[i].localScale = newScale;
                }
            }
        }
    }

    public void SetShadowActive(bool active, float scale = 1f)
    {
        GameObject shadowParentGO = null;
        if (modelStyleInfo.HasRide)
        {
            shadowParentGO = _rideGO;
        }
        else
        {
            shadowParentGO = _mModelGO;
        }
        modelArtistic.RefreshRenderList();
        if (modelArtistic.CheckShadowMat())
        {
            modelArtistic.SetShadowActive(active, shadowParentGO);
            ModelHelper.RemovePetShadow(shadowParentGO);
        }
        else
        {
            if (active)
            {
                ModelHelper.SetPetShadow(shadowParentGO, scale);
            }
            else
            {
                ModelHelper.RemovePetShadow(shadowParentGO);
            }
        }
    }

    /// <summary>
    ///     返回模型锚点
    /// </summary>
    /// <returns>The mounting point.</returns>
    /// <param name="mount">Mount.</param>
    public Transform GetMountingPoint(string mount)
    {
        GameObject mountParentGO = _mModelGO;

        if (mount == ModelHelper.Mount_shadow)
        {
            if (modelStyleInfo.HasRide)
            {
                mountParentGO = _rideGO;
            }
            else
            {
                mountParentGO = _mModelGO;
            }
        }

        if (mountParentGO != null)
        {
            var mountTF = ModelHelper.GetMountingPoint(mountParentGO, mount);
            if (mountTF == null)
            {
                return mountParentGO.transform;
            }
            return mountTF;
        }
        return null;
    }

    // 强行清理某个锚点下的资源,处理战斗内有任务标记的问题
    public void ClearMountingPoint(string mount)
    {
        GameObject mountParentGO = _mModelGO;

        if (mountParentGO != null)
        {
            var mountTF = ModelHelper.GetMountingPoint(mountParentGO, mount);
            if (mountTF != null)
            {
                mountTF.RemoveChildren();
            }
        }
    }

    //更新坐骑
    public void UpdateRide(int rideId)
    {
        if (modelStyleInfo == null)
            return;

        modelStyleInfo.rideId = rideId;
        if (rideId == 0)
            modelStyleInfo.ModelScale = 1;
        LoadWholeModel();
    }

    public void UpdateWeapon(int weaponId)
    {
        if (modelStyleInfo == null)
            return;

        modelStyleInfo.weaponId = weaponId;
        GameDebuger.TODO(@"if (_mModelGO != null && !modelStyleInfo.IsTransformModel && modelStyleInfo.FashionWeapon == null)");
        {
            ModelHelper.UpdateModelWeapon(_mModelGO, modelStyleInfo.defaultModelId, modelStyleInfo.weaponId,
                modelStyleInfo.weaponEffLv, modelStyleInfo.weaponEffId, _uiMode, OnLoadWeaponFinish);
        }
    }

    public void UpdateWeaponEff(int weaponEffId)
    {
        if (modelStyleInfo == null)
            return;

        modelStyleInfo.weaponEffId = weaponEffId;
        GameDebuger.TODO(@"if (_mModelGO != null && !modelStyleInfo.IsTransformModel && modelStyleInfo.FashionWeapon == null)");
        {
            ModelHelper.UpdateModelWeapon(_mModelGO, modelStyleInfo.defaultModelId, modelStyleInfo.weaponId,
                modelStyleInfo.weaponEffLv, modelStyleInfo.weaponEffId, _uiMode, OnLoadWeaponFinish);
        }
    }

    public void UpdateHallowSprite(int pHallowSpriteId)
    {
        if (modelStyleInfo == null)
            return;

        modelStyleInfo.hallowSpriteId = pHallowSpriteId;
        UpdateHallowSprite();
    }

    public void UpdateModelHSV(string mutateColor, int mutateTexture)
    {
        if (modelStyleInfo == null)
            return;

        modelStyleInfo.mutateTexture = mutateTexture;
        modelStyleInfo.mutateColorParam = mutateColor;

        if (_mModelGO != null && !modelStyleInfo.IsTransformModel && !modelStyleInfo.IsFashionModel)
        {
            ModelHelper.SetPetLook(_mModelGO, modelStyleInfo.mainTexture, modelStyleInfo.mutateTexture,
                modelStyleInfo.mutateColorParam, modelStyleInfo.ornamentId, modelStyleInfo.ornamentColorParam, OnMatChange);
        }
    }

    public void UpdateTestModelHSV(string mutateColor, int mutateTexture)
    {
        if (modelStyleInfo == null)
            return;

        modelStyleInfo.mutateTexture = mutateTexture;
        modelStyleInfo.mutateColorParam = mutateColor;

        if (_mModelGO != null && !modelStyleInfo.IsFashionModel)
        {
            ModelHelper.SetPetLook(_mModelGO, modelStyleInfo.mainTexture, modelStyleInfo.mutateTexture,
                modelStyleInfo.mutateColorParam, modelStyleInfo.ornamentId, modelStyleInfo.ornamentColorParam, OnMatChange);
        }
    }

    public void UpdateRideModelHSV(string mutateColor)
    {
        if (modelStyleInfo == null)
            return;

        modelStyleInfo.rideMutateColorParam = mutateColor;

        if (_rideGO != null && !modelStyleInfo.IsTransformModel && !modelStyleInfo.IsFashionModel)
        {
            ModelHelper.SetRideLook(_rideGO,
                modelStyleInfo.rideMutateColorParam,
                modelStyleInfo.rideOrnamentId,
                modelStyleInfo.rideOrnamentColorParam,
                modelStyleInfo.rideMaxEffect,
                modelStyleInfo.rideEffect);
        }
    }

    public void UpdateOrnamentColorParam(string ornamentColorParam)
    {
        if (modelStyleInfo == null)
            return;

        modelStyleInfo.ornamentColorParam = ornamentColorParam;

        if (_mModelGO != null && !modelStyleInfo.IsTransformModel)
        {
            ModelHelper.SetPetLook(_mModelGO, modelStyleInfo.mainTexture, modelStyleInfo.mutateTexture,
                modelStyleInfo.mutateColorParam, modelStyleInfo.ornamentId, modelStyleInfo.ornamentColorParam, OnMatChange);
        }
    }

    public void UpdateRideOrnamentColorParam(string ornamentColorParam)
    {
        if (modelStyleInfo == null)
            return;

        modelStyleInfo.rideOrnamentColorParam = ornamentColorParam;

        if (_mModelGO != null && !modelStyleInfo.IsTransformModel)
        {
            ModelHelper.SetRideLook(_rideGO,
                modelStyleInfo.rideMutateColorParam,
                modelStyleInfo.rideOrnamentId,
                modelStyleInfo.rideOrnamentColorParam,
                modelStyleInfo.rideMaxEffect,
                modelStyleInfo.rideEffect);
        }
    }

    public void DOLocalMove(Vector3 endValue, float duration)
    {
        if (_mModelTrans != null)
        {
            _mModelTrans.DOLocalMove(endValue, duration).SetEase(Ease.Linear).OnComplete(()=> {
                PlayAnimation(ModelHelper.AnimType.idle,false);
            });
        }
    }

    public void SetActive(bool active)
    {
        _activeFlag = active;

        for (int i = 0, n = _gameobjectList.Count; i < n; i++)
        {
            if (_gameobjectList[i] != null)
            {
                _gameobjectList[i].SetActive(active);
            }
        }

        if (_rideGO != null)
        {
            _rideGO.SetActive(active);
        }
    }

    //设置人物的Active
    public void SetPersonActive(bool active)
    {
        if (_gameobjectList.Count > 0)
        {
            for (int i = 0, n = _gameobjectList.Count; i < n; i++)
            {
                _gameobjectList[i].SetActive(active);
            }
        }
    }

    public void LateUpdate()
    {
        modelArtistic.LateUpdate();
    }
    public void Clear()
    {
        foreach (var item in _assetHandlerDic)
        {
            var handler = item.Value;
            handler.Dispose();
        }
        _assetHandlerDic.Clear();
        ModelHelper.RemoveSurroundAndFootEffect(_mModelGO);

        if (_mModelGO != null)
        {
            Transform hudTF = GameObjectExt.GetChildTransform(_mModelGO.transform, ModelHelper.Mount_hud);
            Transform soulTF = GameObjectExt.GetChildTransform(_mModelGO.transform, ModelHelper.Mount_soul);

            if (_hudY != 0f && hudTF != null)
            {
                hudTF.localPosition = new Vector3(hudTF.localPosition.x, _hudY, hudTF.localPosition.z);
            }

            if (_soulY != 0 && soulTF != null)
            {
                soulTF.localPosition = new Vector3(soulTF.localPosition.x, _soulY, soulTF.localPosition.z);
            }
        }

        _hudY = 0f;
        _soulY = 0f;
        _changeMountPosition = false;

        CleanRidePet();
        ClearSoul();
        modelArtistic.Clear();

        for (int i = 0, n = _gameobjectList.Count; i < n; i++)
        {
            if (_gameobjectList[i] != null)
            {
                ModelHelper.RemoveAllBindModel(_gameobjectList[i]);
                ResourcePoolManager.Instance.DespawnModel(_gameobjectList[i]);
            }
        }

        _animatorList.Clear();
        _weaponAnimatorList.Clear();
        _gameobjectList.Clear();
        _transformList.Clear();

        //清理足迹和氛围
        _surroundEffBottom = null;
        _footprintEffects = null;
        _footpringRenderers = null;

        ResourcePoolManager.Instance.DespawnModel(_mModelGO);
        _mModelGO = null;
        _mModelTrans = null;
        _mAnimator = null;
        _rideAnimator = null;
        _defaultAnimate = ModelHelper.AnimType.invalid;
        _defaultRotation = Vector3.zero;

        _loadingCount = 0;
        _loadingRideCount = 0;
    }

    public void Destory()
    {
        //销毁模型时,重置模型 SkinnedMeshRenderer
        if (_mModelGO != null)
            AppearEffect();

        Clear();
        modelStyleInfo = null;
        _waitingInfo = null;
    }

    #region 动作相关处理

    public void PlayAnimateWithCallback(ModelHelper.AnimType animate, bool crossFade, Action<ModelHelper.AnimType, float> animClipCallBack = null,
                                        bool checkSameAnim = false, int layer = 0)
    {
        if (_mAnimator == null)
        {
            if (animClipCallBack != null)
            {
                animClipCallBack(animate, 0);
            }
            return;
        }

        for (int i = 0, n = _animatorList.Count; i < n; i++)
        {
            var animator = _animatorList[i];
            if (animator == _mAnimator)
            {
                ModelHelper.PlayAnimation(_mAnimator, animate, crossFade, animClipCallBack, checkSameAnim, layer);
            }
            else
            {
                ModelHelper.PlayAnimation(_animatorList[i], animate, crossFade);
            }
        }
        
        for (int i = 0, n = _weaponAnimatorList.Count; i < n; i++)
        {
            ModelHelper.PlayAnimation(_weaponAnimatorList[i], animate, crossFade);
        }
    }

    public bool IsAnimatorReady()
    {
        return _mAnimator != null;
    }

    public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layer = 0)
    {
        if (_mAnimator == null)
        {
            return new AnimatorStateInfo();
        }
        return _mAnimator.GetCurrentAnimatorStateInfo(layer);
    }

    /// <summary>
    ///     播放模型动作
    /// </summary>
    /// <param name="clip">Animate.</param>
    /// <param name="crossFade">If set to <c>true</c> cross fade.</param>
    /// <param name="checkSameAnim">是否检查相同动作</param>
    public void PlayAnimation(ModelHelper.AnimType clip, bool crossFade = false, bool checkSameAnim = false)
    {
        if (clip == ModelHelper.AnimType.invalid)
        {
            return;
        }

        _defaultAnimate = clip;

        var playerAnimation = GetPlayerAnimation();

        if (modelStyleInfo != null && modelStyleInfo.HasRide)
        {
            ModifyModelMount(_mModelGO, playerAnimation);
        }

        //坐骑动作
        if (_rideAnimator != null)
        {
            ModelHelper.PlayAnimation(_rideAnimator, _defaultAnimate, crossFade, null, checkSameAnim);
        }

        //人物动作
        if (_animatorList.Count > 0)
        {
            for (int i = 0, n = _animatorList.Count; i < n; i++)
            {
                ModelHelper.PlayAnimation(_animatorList[i], playerAnimation, crossFade, null, checkSameAnim);
            }
        }
        _weaponAnimatorList.ForEach(ani => ModelHelper.PlayAnimation(ani, playerAnimation, crossFade));

        if (playerAnimation == ModelHelper.AnimType.run)
        {
            SetSurroundEffectBottomActive(false);
            SetFootprintActive(true);
        }
        else
        {
            SetSurroundEffectBottomActive(true);
            SetFootprintActive(false);
        }
    }

    //获取玩家动作，这里需要判断是否有坐骑来调整
    private ModelHelper.AnimType GetPlayerAnimation()
    {
        ModelHelper.AnimType playerAnimation = _defaultAnimate;
        if (modelStyleInfo != null && modelStyleInfo.HasRide)
        {
            GameDebuger.TODO(@"int mountType = ModelManager.Mount.getMountType(modelStyleInfo.rideId);
            if (mountType == Mount.MountType_Land)
            {
                if (_defaultAnimate == ModelHelper.Anim_idle)
                {
                    playerAnimation = ModelHelper.Anim_ride_idle;
                }
                else if (_defaultAnimate == ModelHelper.Anim_run)
                {
                    playerAnimation = ModelHelper.Anim_ride_run;
                }
            }
            else");
            {
                playerAnimation = ModelHelper.AnimType.idle;
            }
        }

        return playerAnimation;
    }

    private bool _changeMountPosition = false;
    private float _hudY = 0f;
    private float _soulY = 0f;

    //根据动作，动态调整位置
    private void ModifyModelMount(GameObject go, ModelHelper.AnimType animate)
    {
        if (go == null)
        {
            return;
        }

        bool newStatus = false;
        if (animate == ModelHelper.AnimType.rideIdle|| animate == ModelHelper.AnimType.rideRun)
        {
            newStatus = true;
        }
        else
        {
            newStatus = false;
        }

        if (_changeMountPosition != newStatus)
        {
            _changeMountPosition = newStatus;

            Transform hudTF = GameObjectExt.GetChildTransform(go.transform, ModelHelper.Mount_hud);
            Transform soulTF = GameObjectExt.GetChildTransform(go.transform, ModelHelper.Mount_soul);

            if (_hudY == 0f && hudTF != null)
            {
                _hudY = hudTF.localPosition.y;
            }

            if (_soulY == 0f && soulTF != null)
            {
                _soulY = soulTF.localPosition.y;
            }

            if (_changeMountPosition)
            {
                if (hudTF != null)
                    hudTF.localPosition = new Vector3(hudTF.localPosition.x, hudTF.localPosition.y + 0.6f, hudTF.localPosition.z);
                if (soulTF != null)
                    soulTF.localPosition = new Vector3(soulTF.localPosition.x, soulTF.localPosition.y + 0.6f, soulTF.localPosition.z);
            }
            else
            {
                if (hudTF != null)
                    hudTF.localPosition = new Vector3(hudTF.localPosition.x, _hudY, hudTF.localPosition.z);
                if (soulTF != null)
                    soulTF.localPosition = new Vector3(soulTF.localPosition.x, _soulY, soulTF.localPosition.z);
            }
        }
    }

    public void OnEnable()
    {
        if (_defaultAnimate != ModelHelper.AnimType.invalid)
        {
            PlayAnimation(_defaultAnimate);
        }
    }

    public void OnDisable()
    {

    }

    #endregion

    public void HideEffect()
    {
        if (null == _mModelGO)
            return;
        Renderer[] list = _mModelGO.GetComponentsInChildren<Renderer>();
        for (int index = 0; index < list.Length; index++)
        {
            list[index].enabled = false;
        }
    }

    public void AppearEffect()
    {
        if (null == _mModelGO)
            return;
        Renderer[] list = _mModelGO.GetComponentsInChildren<Renderer>();
        for (int index = 0; index < list.Length; index++)
        {
            list[index].enabled = true;
        }
    }
}