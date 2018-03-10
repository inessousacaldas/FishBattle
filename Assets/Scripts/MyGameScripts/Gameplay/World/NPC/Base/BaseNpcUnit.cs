// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  BaseNpcUnit.cs
// Author   : willson
// Created  : 2014/12/23 
// Porpuse  : 
// **********************************************************************

using UnityEngine;
using System.Collections.Generic;
using AppDto;
using System;
using AssetPipeline;
using Object = UnityEngine.Object;
public class BaseNpcInfo
{
    public string name;

    public SceneNpcDto npcStateDto
    {
        get
        {
            return _npcStateDto;
        }
        set
        {
            _npcStateDto = value;

            GameDebuger.TODO(@"if (value is WorldBossNpcStateDto)
            {
                string npcName = "";

                WorldBossNpcStateDto bossNpc = _npcStateDto as WorldBossNpcStateDto;
                if (bossNpc.times == 0)
                {
                    npcName = '{0}{1}（{3}星）';
                }
                else
                {
                    npcName = '{0}{1}（{3}星）';
                }

                string prefix = '';
                List<WorldBossRankConfig> rankConfig = DataCache.getArrayByCls<WorldBossRankConfig>();
                for (int index = 0; index < rankConfig.Count; index++)
                {
                    if (rankConfig[index].minRank <= bossNpc.rank && bossNpc.rank <= rankConfig[index].maxRank)
                    {
                        prefix = rankConfig[index].prefix;
                        break;
                    }
                }
                name = string.Format(npcName, prefix, _npcStateDto.npc.name, bossNpc.rank);
            }");
        }
    }

    private SceneNpcDto _npcStateDto;

    /** 外观编号 */
    public NpcAppearance npcAppearance;
//    public RideMountNotify rideMountNotify;

    public PlayerDressInfo playerDressInfo;

    /** 模型编号 */
    public int modelId;

    /** 关联武器id */
    public int wpmodel;

    /** 变色颜色,示例: 0,0,0;1,1,1;2,2,2 */
    public string mutateColor;

    /** 贴图 */
    public int texture;

    /** 变色贴图 */
    public int mutateTexture;

    /** 装饰id */
    public int ornamentId;

    /** 缩放比例 */
    public float scale = 1f;

    /** */
    public int submitIndex = -1;
    internal int rideLevel;

    public void AdjustAppearance()
    {
        scale = 1f;

        if (string.IsNullOrEmpty(name))
        {
            name = _npcStateDto.npc.name;
        }

        if (playerDressInfo != null && playerDressInfo.charactorId != 0)
        {
            if (playerDressInfo.transformModelId > 0)
            {
                modelId = playerDressInfo.transformModelId;
                texture = modelId;
                wpmodel = 0;
                mutateTexture = 0;
                mutateColor = "";
                ornamentId = 0;
            }
            else
            {
                GeneralCharactor generalCharactor = DataCache.getDtoByCls<GeneralCharactor>(playerDressInfo.charactorId);
                modelId = generalCharactor.modelId;
                texture = generalCharactor.texture;
                wpmodel = playerDressInfo.wpmodel;
                mutateColor = PlayerModel.GetDyeColorParams(playerDressInfo);
            }
        }
        else
        {
            if (npcStateDto.npc != null)
            {
                scale = npcStateDto.npc.scale;
            }

            NpcSceneMonster sceneMonster = npcStateDto.npc as NpcSceneMonster;
            if (sceneMonster != null && sceneMonster.npcAppearance != null)
            {
                modelId = sceneMonster.npcAppearance.modelId;
                mutateColor = sceneMonster.npcAppearance.mutateColor;
                wpmodel = sceneMonster.npcAppearance.wpmodel;
                mutateTexture = sceneMonster.npcAppearance.mutateTexture;
                ornamentId = sceneMonster.npcAppearance.ornamentId;
                scale = sceneMonster.npcAppearance.scale;
                texture = sceneMonster.modelId;
            }
            else if(npcAppearance != null)
            {
                modelId = npcAppearance.modelId;
                mutateColor = npcAppearance.mutateColor;
                mutateTexture = npcAppearance.mutateTexture;
                ornamentId = npcAppearance.ornamentId;
                texture = modelId;
                npcAppearance = null;
            }
            else
            {
                modelId = npcStateDto.npc.modelId;
                mutateColor = npcStateDto.npc.mutateColor;
                mutateTexture = npcStateDto.npc.mutateTexture;
                ornamentId = npcStateDto.npc.ornamentId;
                texture = modelId;

                GameDebuger.TODO(@"NpcVariable npcVariable = npcStateDto.npc as NpcVariable;
                if (npcVariable != null)
                {
                    wpmodel = npcVariable.wpmodel;
                }");

                GameDebuger.TODO(@"NpcGeneral npcGeneral = npcStateDto.npc as NpcGeneral;
                if (npcGeneral != null)
                {
                    wpmodel = npcGeneral.wpmodel;
                }");
            }
        }

        //本身处于上边的if和else之间的 2017-02-18 15:28:09
        GameDebuger.TODO(@"else if (npcAppearance != null)
        {
            modelId = npcAppearance.modelId;
            mutateColor = npcAppearance.mutateColor;
            wpmodel = npcAppearance.wpmodel;
            mutateTexture = npcAppearance.mutateTexture;
            ornamentId = npcAppearance.ornamentId;
            texture = modelId;
            scale = npcAppearance.scale;
        }");
        
        Model model = DataCache.getDtoByCls<Model>(modelId);
        if (scale == 0)
        {
            scale = 1f;
        }
        if (model != null)
        {
            float modelScale = model.scale == 0 ? 1f : model.scale;
            scale = scale * modelScale;
        }
    }
}

public class BaseNpcUnit : IQuadObject<BaseNpcUnit>
{
    public BaseNpcInfo _npcInfo { get; protected set; }

    protected GameObject _unitGo;

    public GameObject unitGo {
        get { return _unitGo; }
    }
    /// <summary>
    /// 用 SetPos 设置坐标
    /// </summary>
    protected Transform _unitTrans;

    protected BoxCollider _boxCollider;
    protected CharacterTitleHud titleHud;
    protected CharacterHeadHud headHud;

    protected ModelDisplayer _modelDisplayer;
    //模型未加载完毕时,用于标记模型显示状态
    protected bool _isModelActive = true;
    protected NpcViewManager _master;
    protected GameObject _unitModelGo;
    /// <summary>
    /// 相当于 unitGo.activeInHierarchy
    /// </summary>
    public bool selfActive { get; protected set; }
    public void Init(BaseNpcInfo npcInfo)
    {
        _npcInfo = npcInfo;
        selfActive = true;
    }
    public void Setup(NpcViewManager master)
    {
        _master = master;
        _unitGo = _master.npcViewPool.SpawnPlayerView().gameObject;
        _unitGo.name = string.Format("{0}_{1}_{2}", _npcInfo.npcStateDto.npc.GetType().Name, _npcInfo.npcStateDto.npc.id, GetNpcModel());
        _unitTrans = _unitGo.transform;
        _isModelActive = true;
        _modelDisplayer = new ModelDisplayer(_unitGo, OnLoadModelFinish);
        SetUnitActive(selfActive);
    }

    public virtual void Load()
    {
        UpdateNpcPosition();

        var checker = _unitGo.GetMissingComponent<ModelVisibleChecker>();
        checker.Setup(OnVisible, OnInvisible, 2f);

        SetUnitActive(selfActive);
    }

    protected void OnVisible()
    {
        if (_modelDisplayer != null)
        {
            ModelStyleInfo lookInfo = ModelStyleInfo.ToInfo(_npcInfo);
            _modelDisplayer.SetLookInfo(lookInfo);
        }
    }

    private void OnInvisible()
    {
        if (_modelDisplayer != null)
            _modelDisplayer.Destory();
        //DestroyMissionMark();
        CleanUpHUDView();
    }

    private void OnLoadModelFinish()
    {
        if(!(this is DoubleTeleportUnit
            ||this is NpcSceneTeleportUnit))
        {
            //_modelDisplayer.ClearMountingPoint(ModelHelper.Mount_hud);
        }

        if(_acceptSignGO != null || _completeSignGO != null)
        {
            _tMissionSignRoot = _modelDisplayer.GetMountingPoint(ModelHelper.Mount_hud);
            if(_acceptSignGO != null)
                _acceptSignGO.transform.parent = _tMissionSignRoot;
            if(_completeSignGO != null)
                _completeSignGO.transform.parent = _tMissionSignRoot;
        }
        if (NeedTrigger())
        {
            SetupBoxCollider();
            if (!(this is DoubleTeleportUnit || this is NpcSceneTeleportUnit))
            {
                if (!(this is PreciousBoxUnit))
                {
                    //	Npc 头顶标示
                    SetMissionNpcMark(true);
                    _waitReMarkSta = false;
                }
            }
        }

        if (_waitReMarkSta)
        {
            SetMissionNpcMark();
        }

        AfterInit();

        CheckInWeddingMode();
        //TrySetRideAnimation();
    }

    #region Getter

    public Npc GetNpc()
    {
        return _npcInfo.npcStateDto.npc;
    }

    public long GetNpcUID()
    {
        return _npcInfo.npcStateDto.id;
    }

    public SceneNpcDto getNpcState()
    {
        return _npcInfo.npcStateDto;
    }
    /// <summary>
    /// 获取pos 用 GetPos()
    /// </summary>
    public GameObject GetUnitGO()
    {
        return _unitGo;
    }

    public BoxCollider GetNpcCollider()
    {
        return _boxCollider;
    }

    #endregion

    public void SetTransparent(float alpha)
    {
        //if(_modelDisplayer != null)
            //_modelDisplayer.SetTransparent(alpha);
    }

    protected virtual void SetupBoxCollider()
    {
        _boxCollider = _unitGo.GetMissingComponent<BoxCollider>();
        _boxCollider.isTrigger = true;
        _boxCollider.center = new Vector3(0f, 0.75f, 0f);
        _boxCollider.size = new Vector3(1f, 1f, 1f);
        _unitGo.tag = GameTag.Tag_Npc;
    }

    protected virtual int GetNpcModel()
    {
        return _npcInfo.modelId;
    }

    protected virtual float GetNpcScale()
    {
        if (_npcInfo.scale == 0)
        {
            return 1f;
        }
        else
        {
            return _npcInfo.scale;
        }
    }

    #region 设置任务Npc头顶标示

    private GameObject _acceptSignGO;
    private GameObject _completeSignGO;

    private void DestroyMissionMark()
    {
        if (_acceptSignGO != null)
        {
            ResourcePoolManager.Instance.DespawnModel(_acceptSignGO);
            _acceptSignGO = null;
        }

        if (_completeSignGO != null)
        {
            ResourcePoolManager.Instance.DespawnModel(_completeSignGO);
            _completeSignGO = null;
        }
        _tMissionSignRoot = null;
    }

    private bool _changeMissionMark = false;
    private NpcMissionMark _lastNpcMissionMark = NpcMissionMark.Nothing;
    private bool _waitReMarkSta = false;

    public void SetMissionNpcMark(bool resetChangeSta = false)
    {
        _waitReMarkSta = _modelDisplayer != null ? !_modelDisplayer.IsAnimatorReady() : true;
        if(_waitReMarkSta)
        {
            return;
        }

        //	任务标示
        Npc tNpc = _npcInfo.npcStateDto.npc;
        NpcMissionMark tNpcMissionMark = _master.GetNpcMissionMarkByNpcInternal(tNpc,_npcInfo.submitIndex);
        _changeMissionMark = resetChangeSta ? true : _lastNpcMissionMark != tNpcMissionMark;
        if(_changeMissionMark)
        {
            _lastNpcMissionMark = tNpcMissionMark;
            if(tNpcMissionMark == NpcMissionMark.Nothing)
            {
                SetAcceptMissionSign(false);
                SetSubmitMissionSign(false,false);
            }
            else if(tNpcMissionMark == NpcMissionMark.Accept)
            {
                SetAcceptMissionSign(true);
                SetSubmitMissionSign(false,false);
            }
            else if(tNpcMissionMark == NpcMissionMark.Process)
            {
                //	进行中任务 | 灰色
                SetAcceptMissionSign(false);
                SetSubmitMissionSign(true,true);
            }
            else if(tNpcMissionMark == NpcMissionMark.Submit)
            {
                // 可提交任务 | 金黄色
                SetAcceptMissionSign(false);
                SetSubmitMissionSign(true,false);
            }
            else
            {
                SetAcceptMissionSign(false);
                SetSubmitMissionSign(false,false);
            }

            SetNPCMissionFlag(false);
        }
    }

    //	NPC状态(是否主线 \ 战斗)
    /// <summary>
    /// NPC状态(是否主线 \ 战斗) | active为false时 mainMission值无效 -- Sets the NPC mission flag.
    /// </summary>
    /// <param name="active">If set to <c>true</c> active.</param>
    /// <param name="mainMission">If set to <c>true</c> main mission.</param>
    protected void SetNPCMissionFlag(bool refreshMark = false)
    {
        if (headHud == null)
        {
            return;
        }
        bool active = false;
        bool mainMission = false;
        ////	任务标示
        if(_changeMissionMark || refreshMark)
        {
            if(_lastNpcMissionMark == NpcMissionMark.MainAccept
                || _lastNpcMissionMark == NpcMissionMark.MainProcess
                || _lastNpcMissionMark == NpcMissionMark.MainSubmit
                )
            {
                active = true;
                mainMission = true;
            }
            else if(_lastNpcMissionMark==NpcMissionMark.Battle)
            {
                active = true;
            }
            headHud.headHUDView.missionTypeSprite.enabled = active;
            if(active)
            {
                headHud.headHUDView.missionTypeSprite.spriteName = mainMission ? "mission_ThreadOfNPC" : "mission_CellBattle";
            }
        }
        //以下是我注释的代码
        //        //	任务标示
        //        if (_changeMissionMark || refreshMark)
        //        {
        //            GameDebuger.TODO(@"
        //   if (_lastNpcMissionMark == MissionNpcModel.NpcMissionMark.MainAccept
        //                || _lastNpcMissionMark == MissionNpcModel.NpcMissionMark.MainProcess
        //                || _lastNpcMissionMark == MissionNpcModel.NpcMissionMark.MainSubmit)
        //            {
        //                active = true;
        //                mainMission = true;
        //            }
        //            else if (_lastNpcMissionMark == MissionNpcModel.NpcMissionMark.Battle)
        //            {
        //                active = true;
        //            }            
        //");
        //        }

        //        headHud.headHUDView.missionTypeSprite.enabled = active;

        //        if (active)
        //        {
        //            headHud.headHUDView.missionTypeSprite.spriteName = mainMission ? "mission_ThreadOfNPC" : "mission_BattleOfNPC";
        //        }
    }

    private Transform _tMissionSignRoot = null;

    public void SetAcceptMissionSign(bool active)
    {
        if (_acceptSignGO == null)
        {
            if (active && _modelDisplayer.IsAnimatorReady())
            {
                if (_tMissionSignRoot == null)
                {
                    _tMissionSignRoot = _modelDisplayer.GetMountingPoint(ModelHelper.Mount_hud);
                }

                if (_tMissionSignRoot != null)
                {
                    ResourcePoolManager.Instance.SpawnModelAsync(PathHelper.ACCEPTMISSION_PREFAB_PATH, (go)=>
                    {
                        if(go == null || _tMissionSignRoot == null || _acceptSignGO != null) {
                            if(go == null) {
                                GameDebuger.LogError("获取不到预制件");
                            }
                            if(_tMissionSignRoot == null) {
                                GameDebuger.LogError("返回模型锚点为空");
                            }
                            if(_acceptSignGO != null) {
                                GameDebuger.LogError("_acceptSignGO不为空");
                            }
                            if(go != null) {
                                GameObject.Destroy(go);
                            }
                            return;
                        }
                        GameObjectExt.AddPoolChild(_tMissionSignRoot.gameObject, go);
                        go.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);

                        go.GetMissingComponent<AutoRotation>();
                        _acceptSignGO = go;
                    });
                }
            }
        }
        else
        {
            _acceptSignGO.SetActive(active);
        }
    }

    public void SetSubmitMissionSign(bool active, bool isGrey)
    {
        if (_completeSignGO == null)
        {
            if (active && _modelDisplayer.IsAnimatorReady())
            {
                if (_tMissionSignRoot == null)
                {
                    _tMissionSignRoot = _modelDisplayer.GetMountingPoint(ModelHelper.Mount_hud);
                }

                if (_tMissionSignRoot != null)
                {
                    ResourcePoolManager.Instance.SpawnModelAsync(PathHelper.SUBMITMISSION_PREFAB_PATH, (go)=>
                    {
                        if(go == null || _tMissionSignRoot == null || _completeSignGO !=null) {
                            if(go == null) {
                                GameDebuger.LogError("获取不到预制件");
                            }
                            if(_tMissionSignRoot == null) {
                                GameDebuger.LogError("返回模型锚点为空");
                            }
                            if(go != null) {
                                GameObject.Destroy(go);
                            }
                            if(_completeSignGO != null)
                            {
                                GameDebuger.LogError("_completeSignGO不为空");
                            }
                            return;
                        }

                        GameObjectExt.AddPoolChild(_tMissionSignRoot.gameObject, go);
                        go.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                        go.GetMissingComponent<AutoRotation>();
                        _completeSignGO = go;

                    });
                }
            }
        }
        else
        {
            _completeSignGO.gameObject.SetActive(active);
        }
    }

    #endregion

    public void DoAction(ModelHelper.AnimType action, bool checkSameAnim = false)
    {
        _modelDisplayer.PlayAnimation(action, false, checkSameAnim);
    }

    public void UpdateNpc(SceneNpcDto npcState)
    {
        _npcInfo.npcStateDto = npcState;
        UpdateNpcPosition();
        
    }

    protected virtual void UpdateNpcPosition()
    {
        if (_unitGo != null)
        {
            Vector3 position = SceneHelper.GetPositionInScene(_npcInfo.npcStateDto.x, _npcInfo.npcStateDto.y, _npcInfo.npcStateDto.z);
            SetPos(position);
            _unitTrans.localEulerAngles = new Vector3(0, _npcInfo.npcStateDto.npc.rotateY, 0);
           
        }
    }
    public void SetPos(Vector3 pos)
    {
        if(_unitGo != null)
        {
            _unitTrans.position = pos;
            if (_boundsChange != null)
                _boundsChange(this);
        }
    }
    public Vector3 GetPos()
    {
        return _unitGo != null ? _unitGo.transform.position : SceneHelper.GetPositionInScene(_npcInfo.npcStateDto.x, _npcInfo.npcStateDto.y, _npcInfo.npcStateDto.z);
    }
    protected virtual bool NeedTrigger()
    {
        return false;
    }

    protected virtual void AfterInit()
    {

    }

    public virtual void Trigger()
    {

    }

    public virtual void Destroy()
    {
        DestroyMissionMark();
        _tMissionSignRoot = null;
        CleanUpHUDView();

        if(_modelDisplayer != null)
        {
            _modelDisplayer.Destory();
            _modelDisplayer = null;
        }

        _boxCollider = null;

        if (_unitGo != null)
        {
            _master.npcViewPool.DespawnPlayerView(_unitGo);
            _unitGo = null;
            _unitTrans = null;
            //自己添加的代码
        }
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
        }
    }

    public virtual void SetUnitActive(bool active)
    {
        selfActive = active;
        if(_unitGo != null)
            _unitGo.SetActive(active);
        SetHUDActive(active);
    }

    public void SetModelActive(bool active)
    {
        _isModelActive = active;
        if (_modelDisplayer != null)
            _modelDisplayer.SetActive(active);

        SetHUDActive(active);
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

//    public virtual void UpdateNpcState(SceneNpcStateDto npcState)
//    {
//        SetUnitActive(npcState.show);
//
//        //	是否任务指定NPCID
//        ModelManager.MissionData.missionTypeFactionTrialDelegate.SetMissionSpecifyNpc(npcState);
//
//        if (npcState.show)
//        {
//            _npcInfo.playerDressInfo = npcState.playerDressInfo;
//            _npcInfo.rideMountNotify = npcState.rideMountNotify;
//            _npcInfo.rideLevel = npcState.rideLevel;
//            _npcInfo.name = npcState.nickname;
//            _npcInfo.AdjustAppearance();
//
//            modelStyleInfo lookInfo = modelStyleInfo.ToInfo(_npcInfo);
//            _modelDisplayer.SetLookInfo(lookInfo);
//
//            _modelDisplayer.SetActive(true);
//        }
//    }

    public bool IsVisible()
    {
        return selfActive;
    }

    public void CheckInWeddingMode()
    {
        GameDebuger.TODO(@"if (ModelManager.Marry.WeddingInfo != null)
        {
            if (_npcInfo.npcStateDto.id == ProxyDialogueModule.Matchmaker_NPC_ID)
            {
                if (_modelDisplayer != null)
                    _modelDisplayer.SetActive(false);
                SetHUDActive(false);
            }
        }        
");
    }
    public bool CheckHaveRide()
    {
        GameDebuger.TODO(@"if (_npcInfo.rideMountNotify != null && _npcInfo.rideMountNotify.mountId > 0
            && _npcInfo.playerDressInfo != null && _npcInfo.playerDressInfo.transformModelId == 0)
        {
            return true;
        }");
        return false;
    }
    private void TrySetRideAnimation()
    {
        if (CheckHaveRide())
        {
            _modelDisplayer.PlayAnimation(ModelHelper.AnimType.idle);
            UpdateNpcPosition();
        }
    }

    event Action<BaseNpcUnit> IQuadObject<BaseNpcUnit>.BoundsChanged
    {
        add
        {
            _boundsChange += value;
        }

        remove
        {
            _boundsChange -= value;
        }
    }
    private event Action<BaseNpcUnit> _boundsChange;
    Bounds IQuadObject<BaseNpcUnit>.Bounds
    {
        get
        {
            return MathHelper.Bounds2D(new Vector2(_npcInfo.npcStateDto.x, _npcInfo.npcStateDto.z), Vector2.one);
        }
    }
    public override int GetHashCode()
    {
        return GetNpcUID().GetHashCode();
    }

    bool IEqualityComparer<BaseNpcUnit>.Equals(BaseNpcUnit left, BaseNpcUnit right)
    {
        return left == right;
    }

    int IEqualityComparer<BaseNpcUnit>.GetHashCode(BaseNpcUnit obj)
    {
        return obj.GetHashCode();
    }
}
