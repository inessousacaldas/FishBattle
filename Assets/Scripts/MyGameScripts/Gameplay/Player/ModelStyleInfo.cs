using System.Collections.Generic;
using AppDto;
using GamePlot;
using SharpKit.JavaScript;
using UnityEngine;

public class ModelStyleInfo
{
    #region Default

    private float _modelScale = 0f;
    private string _specialModelResKey;
    //特殊模型资源Id，如宝箱，传送点

    public float shadowScale = 1f;
    public bool showShadow = false;

    //常规模式下,模型id
    public int defaultModelId = 0;

    public int mainTexture = 0;
    //主模型贴图Id
    public string mutateColorParam = "";
    //变色模型染色参数, mutateColorParams !="" 变色模型
    public int mutateTexture = 0;
    //变异模型贴图Id, mutateTexture !=0 变异模型

    public float ModelScale
    {
        get { return _modelScale; }
        set
        {
            if (value > 0f)
                _modelScale = value;
            else
                _modelScale = 1f;
        }
    }

    public string DefaultModelResKey
    {
        get
        {
            if (!string.IsNullOrEmpty(_specialModelResKey))
                return _specialModelResKey;

            if (_transformModelId != 0)
                return ModelHelper.GetCharacterPrefabPath(_transformModelId);

            GameDebuger.TODO(@"if (_fashionTransformModel != null)
                return _fashionTransformModel.modelId;");

            return ModelHelper.GetCharacterPrefabPath(defaultModelId);
        }
    }

    public int GetModelInfoId()
    {
        if (_transformModelId != 0)
            return _transformModelId;
        GameDebuger.TODO(@"if (_fashionTransformModel != null)
            return StringHelper.ToInt(_fashionTransformModel.modelId.Replace('pet_', ''));");

        return defaultModelId;
    }

    public void SetModelInfoScale(int modelId)
    {
        var model = DataCache.getDtoByCls<Model>(modelId);
        if (model != null)
        {
            ModelScale = model.scale;
        }
        else
        {
            GameDebuger.LogWarning("SetModelInfoScale NullError modelId=" + modelId);
        }
    }

    #endregion

    #region Pet

    public int ornamentId;
    //宠物配饰Id
    public string ornamentColorParam;
    //宠物配饰染色参数

    #endregion

    #region Ride

    public int rideId;
    //坐骑
    public string rideMutateColorParam;
    //坐骑模型染色参数, mutateColorParams !="" 变色模型
    public int rideOrnamentId;
    //坐骑配饰Id
    public string rideOrnamentColorParam;
    //坐骑配饰染色参数
    public bool rideMaxEffect = false;
    //坐骑的最大化特效
    public bool rideEffect = false;
    //坐骑的特效开关

    #endregion

    #region Charactor

    private int _transformModelId;
    //变身模型Id

    public bool useFashionDefaultModel;
    //使用时装默认模型
    //    private FashionDress _fashionHead; //时装模式下,上半身模型Id
    //    private FashionDress _fashionBody; //时装模式下,下半身模型Id
    //    private FashionDress _fashionFootprint; //足迹特效Id
    //    private FashionDress _fashionSurroundEffect; //环绕特效Id
    //    private FashionDress _fashionWeapon; //时装手持道具
    //    private FashionDress _fashionTransformModel; //时装变身道具

    //武器
    public int weaponId;
    //武器模型id
    public int weaponEffLv = 1;
    //武器特效级别
    public int weaponEffId = 0;
    //武器特效ID

    private int mHallowSpriteId;
    //灵器
    public int hallowSpriteId//跟服务端的名字尽量统一，所以成了这个名字
    {
        set
        {
            if (mHallowSpriteId != value)//器灵改变的时候也要改变模型
            {
                mHallowSpriteId = value;
                UpdateSoulModel();
            }
        }
        get
        {
            return mHallowSpriteId;
        }
    }

    public string SoulModelId { get; private set; }

    private void UpdateSoulModel()
    {
        GameDebuger.TODO(@"SoulModelId = HallowUtil.GetSoulModel(hallowSpriteId);");
    }

    //是否是变身模型
    public bool IsTransformModel
    {
        get
        { 
            GameDebuger.TODO(@"return _transformModelId != 0 || _fashionTransformModel != null;"); 
            return _transformModelId != 0/** || _fashionTransformModel != null*/;
        }
    }

    //是否是时装模型
    public bool IsFashionModel
    {
        get
        {
            GameDebuger.TODO(@"return _fashionHead != null
                   || _fashionBody != null
                   || _fashionWeapon != null;");
            return false;
        }
    }

    //是否是特殊模型
    public bool IsSpecialModel
    {
        get
        {
            return !string.IsNullOrEmpty(_specialModelResKey);
        }
    }

    //是否有形象模型(包括变身，时装，普通)
    public bool HasStyleModel
    {
        get
        {
            return IsTransformModel
            || IsFashionModel
            || IsSpecialModel
            || defaultModelId != 0;
        }
    }

    //是否有坐骑
    public bool HasRide
    {
        get { return rideId != 0; }
    }

    public int TransformModelId
    {
        get { return _transformModelId; }
        set
        {
            _transformModelId = value;
            if (value != 0)
            {
                weaponId = 0;
                defaultModelId = 0;
                hallowSpriteId = 0;
                mainTexture = value;
                mutateTexture = 0;
                mutateColorParam = "";
            }
        }
    }

    public string FashionHeadResKey
    {
        get
        {
            GameDebuger.TODO(@"if (_fashionHead != null)
            {
                return _fashionHead.modelId;
            }
            if (_fashionWeapon != null || useFashionDefaultModel)
            {
                return 'pet_' + defaultModelId + '_01_head';
            }");
            return "";
        }
    }

    public string FashionBodyResKey
    {
        get
        {
            GameDebuger.TODO(@"if (_fashionBody != null)
            {
                return _fashionBody.modelId;
            }
            if (_fashionWeapon != null || useFashionDefaultModel)
            {
                return 'pet_' + defaultModelId + '_01_body';
            }");
            return "";
        }
    }

    public string RideResKey
    {
        get
        {
            if (rideId > 0)
            {
                return "ride_pet_" + rideId;
            }
            return "";
        }
    }

    //    public FashionDress FashionFootprint
    //    {
    //        get { return _fashionFootprint; }
    //    }

    //    public FashionDress FashionSurroundEffect
    //    {
    //        get { return _fashionSurroundEffect; }
    //    }

    //    public FashionDress FashionWeapon
    //    {
    //        get { return _fashionWeapon; }
    //    }

    /// <summary>
    ///     设置时装id列表
    /// </summary>
    /// <param name="fashionIds"></param>
    public void SetupFashionIds(List<int> fashionIds)
    {
        GameDebuger.TODO(@"//重新设置时装列表时,先清空时装信息
        _fashionHead = null;
        _fashionBody = null;
        _fashionWeapon = null;
        _fashionFootprint = null;
        _fashionSurroundEffect = null;
        _fashionTransformModel = null;

        if (fashionIds != null)
        {
            for (int i = 0; i < fashionIds.Count; i++)
            {
                UpdateFashionPart(fashionIds[i]);
            }
        }
        //现在改为变身时装和其他时装是共存的（足迹和氛围）
        //if (_fashionTransformModel != null) //如果有变身时装 其他时装都不显示
        //{
        //    _fashionHead = null;
        //    _fashionBody = null;
        //    _fashionWeapon = null;
        //    _fashionFootprint = null;
        //    _fashionSurroundEffect = null;
        //}
        SetModelInfoScale(GetModelInfoId());");
    }

    //移除足迹和氛围
    public void RemoveRoundAndFootEff()
    {
        GameDebuger.TODO(@"_fashionFootprint = null;
        _fashionSurroundEffect = null;");
    }

    //    public void SetupFashionDresses(List<FashionDress> fashionDresses)
    //    {
    //        //重新设置时装列表时,先清空时装信息
    //        _fashionHead = null;
    //        _fashionBody = null;
    //        _fashionWeapon = null;
    //        _fashionFootprint = null;
    //        _fashionSurroundEffect = null;
    //        _fashionTransformModel = null;
    //
    //        if (fashionDresses != null)
    //        {
    //            for (int i = 0; i < fashionDresses.Count; i++)
    //            {
    //                UpdateFashionPart(fashionDresses[i]);
    //            }
    //        }
    //    }

    /// <summary>
    ///     更新某一部位的时装信息
    /// </summary>
    /// <param name="fashionDress"></param>
    /// <param name="remove"></param>
    //    public void UpdateFashionPart(FashionDress fashionDress, bool remove = false)
    //    {
    //        if (fashionDress == null)
    //            return;
    //
    //        FashionDress target = remove ? null : fashionDress;
    //        if (fashionDress.partType == FashionDress.PartTypeEnum_Head)
    //            _fashionHead = target;
    //        else if (fashionDress.partType == FashionDress.PartTypeEnum_Body)
    //            _fashionBody = target;
    //        else if (fashionDress.partType == FashionDress.PartTypeEnum_Hand)
    //            _fashionWeapon = target;
    //		else if (fashionDress.partType == FashionDress.PartTypeEnum_Foot)
    //            _fashionFootprint = target;
    //        else if (fashionDress.partType == FashionDress.PartTypeEnum_Special)
    //            _fashionSurroundEffect = target;
    //        else if (fashionDress.partType == FashionDress.PartTypeEnum_ChangeBodyDress)
    //            _fashionTransformModel = target;
    //    }

    /// <summary>
    ///     更新某一部位的时装信息
    /// </summary>
    /// <param name="fashionId"></param>
    /// <param name="remove"></param>
    //    public void UpdateFashionPart(int fashionId, bool remove = false)
    //    {
    //        FashionDress fashionDress = DataCache.getDtoByCls<FashionDress>(fashionId);
    //        UpdateFashionPart(fashionDress, remove);
    //    }
    #endregion

    #region Convert

    public static ModelStyleInfo ToInfo(CharacterEntity characterEntity)
    {
        var info = new ModelStyleInfo();
        info.defaultModelId = characterEntity.modelId;
        info.mainTexture = characterEntity.modelId;
        info.mutateTexture = characterEntity.mutateTexture;
        info.mutateColorParam = characterEntity.mutateColor;
        info.weaponId = characterEntity.wpModel;
        info.hallowSpriteId = characterEntity.hallowSpriteId;
        info.ModelScale = characterEntity.scale;
        info.showShadow = true;

        return info;
    }

    public static ModelStyleInfo ToInfo(ScenePlayerDto playerDto)
    {
        GeneralCharactor charactorInfo = playerDto.charactor;
        var info = new ModelStyleInfo();
        info.defaultModelId = charactorInfo.modelId;
        info.mainTexture = charactorInfo.texture;
        info.mutateTexture = charactorInfo.mutateTexture;
        info.mutateColorParam = charactorInfo.mutateColor;
        bool isSelf = playerDto.id == ModelManager.Player.GetPlayerId();
        if (isSelf)
        {
            info.weaponEffLv = 3;
            info.rideEffect = true;
        }
        else
        {
            GameDebuger.TODO(@"if (playerDto.dressInfoDto != null)
                info.weaponEffLv = ModelManager.Equipment.IsDefaultWeaponEffect(playerDto.dressInfoDto.weaponEffect) ? 1 : 3;
            else    //针对GM指令 机器人的情况
                info.weaponEffLv = 3;");
            info.rideEffect = false;
        }

        //设置武器模型
        if (isSelf)
        {
            info.weaponEffId = 0;//ModelManager.Player.GetPlayer().dressInfo.weaponEffect;  文思周说填0就可以了
            GameDebuger.TODO(@"info.hallowSpriteId = ModelManager.Backpack.GetCurrentHallowSpriteId();");
        }
        else if (playerDto.dressInfoDto != null)
        {
            info.weaponEffId = playerDto.dressInfoDto.weaponEffect;
//            info.hallowSpriteId = playerDto.dressInfoDto.hallowSpriteId;
        }
        info.weaponId = playerDto.dressInfoDto.wpmodel;
        //设置玩家染色、时装参数和变身参数
        GameDebuger.TODO(@"if (playerDto.transformModelId != 0)
        {
            info.TransformModelId = playerDto.transformModelId;
        }
        else if (playerDto.dressInfoDto != null)
        {
            if (playerDto.dressInfoDto.showDress
                && playerDto.dressInfoDto.fashionDressIds != null
                && playerDto.dressInfoDto.fashionDressIds.Count > 0)
            {
                info.SetupFashionIds(playerDto.dressInfoDto.fashionDressIds);
            }

        info.mutateColorParam = PlayerModel.GetDyeColorParams(playerDto.dressInfoDto);
        }");

        info.SetModelInfoScale(info.GetModelInfoId());
        info.showShadow = true;

        //坐骑
        GameDebuger.TODO(@"if (playerDto.playerShortRide != null 
        && playerDto.playerShortRide.status == PlayerRideDto.RideStatus_Riding
        && !info.IsTransformModel
        && playerDto.playerShortRide.rideMount != null)
        {
            RideMountNotify rideMount = playerDto.playerShortRide.rideMount;
            SetRideMount(info, rideMount, playerDto.playerShortRide.level);
        }
        else");
        {
            info.rideId = 0;
        }

        return info;
    }

    //    private static void SetRideMount(modelStyleInfo info, RideMountNotify rideMount, int rideLevel)
    //    {
    //        info.rideId = rideMount.mountId;
    //        info.rideMutateColorParam = ModelManager.Mount.GetDyeColorStr(rideMount.dyeCase);
    //        info.rideOrnamentId = rideMount.ornamentId;
    //        info.rideOrnamentColorParam = ModelManager.Mount.GetOrnamentDyeColorStr(rideMount.ornamentDyeCase);
    //        info.rideMaxEffect = rideLevel >= ModelManager.Mount.GetRideShowEffectGradLimit();
    //        info.RemoveRoundAndFootEff();
    //        if (rideMount != null && rideMount.mount.mountType == Mount.MountType_Fly)
    //        {
    //            info.showShadow = false;
    //        }
    //    }

    public static ModelStyleInfo ToInfo(GeneralCharactor charactor)
    {
        var info = new ModelStyleInfo();
        if (charactor != null)
        {
            info.defaultModelId = charactor.modelId;
            info.mainTexture = charactor.texture;
            info.mutateTexture = charactor.mutateTexture;
            info.mutateColorParam = charactor.mutateColor;
        }
        return info;
    }

    public static ModelStyleInfo ToInfo(VideoSoldier soldier, bool showFashion = true)
    {
        var info = new ModelStyleInfo();

        if (soldier.playerDressInfo != null && soldier.playerDressInfo.charactorId != 0)
        {
            if (soldier.playerDressInfo.transformModelId > 0)
            {
                info.TransformModelId = soldier.playerDressInfo.transformModelId;
            }
            else
            {
                //如果是宠物，则采用宠物流程
                GameDebuger.TODO(@"if (soldier.character != null && (soldier.character is Pet || soldier.character is Crew))
                {
                    info = ToInfo(soldier.character);
                    if (soldier.character is Pet)
                    {
                        info.ornamentId = soldier.playerDressInfo.ornamentId;
                        info.ornamentColorParam =
                            PetModel.GetOrnmentDyeColorStr(soldier.playerDressInfo.petOrnamentDyeCaseId);

                        var petDyeCase = DataCache.getDtoByCls<PetDyeCase>(soldier.playerDressInfo.dyeCaseId);

                        if (petDyeCase != null)
                        {
                            info.mutateColorParam = PetModel.GetDyeColorStr(soldier.playerDressInfo.dyeCaseId);
                            if (!petDyeCase.changeColor)
                            {
                                info.mutateTexture = 0;
                            }
                        }
                        else
                        {
                            if (!soldier.mutate)
                            {
                                info.mutateTexture = 0;
                            }
                        }
                    }
                }
                else");
                {
                    var charactor = DataCache.getDtoByCls<GeneralCharactor>(soldier.playerDressInfo.charactorId);
                    info.defaultModelId = charactor.modelId;
                    info.mainTexture = charactor.texture;
                    info.weaponId = soldier.playerDressInfo.wpmodel;
                    info.weaponEffId = soldier.playerDressInfo.weaponEffect;
                    GameDebuger.TODO(@"info.hallowSpriteId = soldier.playerDressInfo.hallowSpriteId;");

                    var isSelf = soldier.id == ModelManager.Player.GetPlayerId();
                    if (isSelf)
                    {
                        info.weaponEffLv = 3;
                    }
                    else
                    {
                        GameDebuger.TODO(@"info.weaponEffLv = ModelManager.Equipment.IsDefaultWeaponEffect(soldier.playerDressInfo.weaponEffect) ? 1 : 3;");
                    }

                    if (soldier.playerDressInfo.showDress && soldier.playerDressInfo.fashionDressIds.Count > 0)
                    {
                        info.SetupFashionIds(soldier.playerDressInfo.fashionDressIds);
                    }

                    info.mutateColorParam = PlayerModel.GetDyeColorParams(soldier.playerDressInfo);
                }
            }
        }
        else
        {
            GameLog.Log_Battle("charactorId ===" + soldier.charactorId);
            if (soldier.charactor != null)
            {
                info = ToInfo(soldier.charactor);
                GameDebuger.TODO(@"if (soldier.mutate == false)
                {
                    info.mutateTexture = 0;
                    info.mutateColorParam = "";
                }");
            }
            else
            {
                if (soldier.monster.modelId > 0)
                {
                    info.defaultModelId = soldier.monster.modelId;
                }
                // todo fish: 以下被屏蔽代码需要被处理
//                else if (soldier.monster.pet != null)
//                {
//                    info.defaultModelId = soldier.monster.pet.modelId;
//                }

                if (soldier.monster.texture > 0)
                {
                    info.mainTexture = soldier.monster.texture;
                }
//                else if (soldier.monster.pet != null)
//                {
//                    info.mainTexture = soldier.monster.pet.texture;
//                }
//
//                if (soldier.monster.mutateTexture > 0)
//                {
//                    info.mutateTexture = soldier.monster.mutateTexture;
//                }
//                else if (soldier.monster.pet != null)
//                {
//                    info.mutateTexture = soldier.monster.pet.mutateTexture;
//                }
//
//                if (soldier.monster.mutateColor != "")
//                {
//                    info.mutateColorParam = soldier.monster.mutateColor;
//                }
//                else if (soldier.monster.pet != null)
//                {
//                    info.mutateColorParam = soldier.monster.pet.mutateColor;
//                }
//
//                info.weaponId = soldier.monster.wpmodel;
//
//                if (soldier.monster.pet != null && soldier.monsterType != Monster.MonsterType_Mutate)
//                {
//                    info.mutateTexture = 0;
//                    info.mutateColorParam = "";
//                }
//
//            这里判断下， 如果>0才设置，==0表示这个设置不起作用， 让后续的设置
            if (soldier.monster.scale > 0)
            {
                info.ModelScale = soldier.monster.scale;
            }
            }
        }

        if (!showFashion)
        {
            info.SetupFashionIds(null);
        }

        //如果之前的设置不上效果，就采用默认的模型值设置
        if (info.ModelScale == 0)
        {
            info.SetModelInfoScale(info.GetModelInfoId());
        }
        
        info.RemoveRoundAndFootEff();
        return info;
    }
    //NPC Style
    public static ModelStyleInfo ToInfo(BaseNpcInfo baseNpcInfo)
    {
        var info = new ModelStyleInfo();
        info.defaultModelId = baseNpcInfo.modelId;
        info.mainTexture = baseNpcInfo.texture == 0 ? baseNpcInfo.modelId : baseNpcInfo.texture;
        info.mutateColorParam = baseNpcInfo.mutateColor;
        info.mutateTexture = baseNpcInfo.mutateTexture;
        info.weaponId = baseNpcInfo.wpmodel;
        PlayerDressInfo playerDressInfo = baseNpcInfo.playerDressInfo;
        GameDebuger.TODO(@"RideMountNotify rideMountNotify = baseNpcInfo.rideMountNotify;
        if (playerDressInfo != null)
            BaseNpcInfoToInfoSetPlayer(info, playerDressInfo, rideMountNotify, baseNpcInfo.rideLevel);");
        info.ModelScale = baseNpcInfo.scale;
        info.showShadow = true;
        info.shadowScale = 1f;
        info.ornamentId = baseNpcInfo.ornamentId;

        BaseNpcInfoToInfoSetNpc(baseNpcInfo, info);
        return info;
    }

    private static void BaseNpcInfoToInfoSetNpc(BaseNpcInfo baseNpcInfo, ModelStyleInfo info)
    {
        if (baseNpcInfo.npcStateDto.npc is NpcDoubleTeleport)
        {
            info._specialModelResKey = PathHelper.Portal_PREFAB_PATH;
        }
        GameDebuger.TODO(@"if (baseNpcInfo.npcStateDto.npc is NpcScenePreciousBox)
        {
            info._specialModelResKey = PathHelper.Chest_PREFAB_PATH;
            info.shadowScale = 2f;
        }
        else if (baseNpcInfo.npcStateDto.npc is NpcSceneBridalSedanBox)
        {
            info._specialModelResKey = PathHelper.SedanBox_PREFAB_PATH;
            info.shadowScale = 2f;
        }
        else if (baseNpcInfo.npcStateDto.npc is NpcSceneBridalSedanSweet)
        {
            info._specialModelResKey = PathHelper.MarrigeSweetBox_PREFAB_PATH;
            info.shadowScale = 2f;
        }
        else if (baseNpcInfo.npcStateDto.npc is NpcSceneBridalSedanPetBox)
        {
            info._specialModelResKey = PathHelper.SedanPetBox_PREFAB_PATH;
            info.shadowScale = 2f;
        }
        else if (baseNpcInfo.npcStateDto.npc is NpcSceneGoldBox)
        {
            info._specialModelResKey = PathHelper.WorldBoss_Chest_PREFAB_PATH;
            info.ModelScale = 1.5f;
            info.shadowScale = 2f;
        }
        else if (baseNpcInfo.npcStateDto.npc is NpcSceneMarriageSweetBox)
        {
            info._specialModelResKey = PathHelper.MarrigeSweetBox_PREFAB_PATH;
            info.shadowScale = 2f;
        }
        else if (baseNpcInfo.npcStateDto.npc is NpcSceneWorldBossPreciousBox)
        {
            info._specialModelResKey = PathHelper.WorldBoss_Chest_PREFAB_PATH;
            info.ModelScale = 1.5f;
            info.shadowScale = 2f;
        }
        else if (baseNpcInfo.npcStateDto.npc is NpcSceneGuildCompBox)
        {
            info._specialModelResKey = PathHelper.WorldBoss_Chest_PREFAB_PATH;
            info.ModelScale = 1.5f;
            info.shadowScale = 2f;
        }
        else if (baseNpcInfo.npcStateDto.npc is NpcSceneMazeBox)
        {
            NpcSceneMazeBox boxNpc = baseNpcInfo.npcStateDto.npc as NpcSceneMazeBox;
            if (boxNpc.boxType == NpcSceneMazeBox.MazeBoxType_Box)
            {
                info._specialModelResKey = PathHelper.Chest_PREFAB_PATH;
                info.shadowScale = 2f;
            }
            else
            {
                info._specialModelResKey = PathHelper.WorldBoss_Chest_PREFAB_PATH;
                info.ModelScale = 1.5f;
                info.shadowScale = 2f;
            }
        }
        else if (baseNpcInfo.npcStateDto.npc is NpcSceneGrass)
        {
            //NpcSceneGrass grassNpc = baseNpcInfo.npcStateDto.npc as NpcSceneGrass;
            info._specialModelResKey = PathHelper.Grass_PREFAB_PATH;
            info.ModelScale = 1.5f;
            info.shadowScale = 2f;
        }
        else if (baseNpcInfo.npcStateDto.npc is NpcSceneTeleport)
        {
            info._specialModelResKey = PathHelper.Portal_PREFAB_PATH;
        }
        else if (baseNpcInfo.npcStateDto.npc is NpcSceneMythLandBox)
        {
            var tNpcSceneMythLandBox = baseNpcInfo.npcStateDto.npc as NpcSceneMythLandBox;
            info._specialModelResKey = string.Format(PathHelper.Dreamland_Chest_PREFAB_PATH,
                tNpcSceneMythLandBox.boxType);
        }
        else if (baseNpcInfo.npcStateDto.npc is NpcSceneWorldGhostBox)
        {
            //世界Boss箱子
            info._specialModelResKey = PathHelper.WorldBoss_Chest_PREFAB_PATH;
            info.ModelScale = 1.5f;
            info.shadowScale = 2f;
        }");
    }

    //    private static void BaseNpcInfoToInfoSetPlayer(modelStyleInfo info, PlayerDressInfo playerDressInfo, RideMountNotify rideMountNotify, int rideLevel)
    //    {
    //        info.weaponEffId = playerDressInfo.weaponEffect;
    //        info.weaponEffLv = ModelManager.Equipment.IsDefaultWeaponEffect(playerDressInfo.weaponEffect) ? 1 : 3;
    //        info.hallowSpriteId = playerDressInfo.hallowSpriteId;
    //        info.TransformModelId = 0;
    //        if (playerDressInfo.transformModelId > 0)
    //        {
    //            info.TransformModelId = playerDressInfo.transformModelId;
    //            info.SetupFashionIds(null);
    //        }
    //        else if (playerDressInfo.showDress && playerDressInfo.fashionDressIds != null && playerDressInfo.fashionDressIds.Count > 0)
    //        {
    //            info.SetupFashionIds(playerDressInfo.fashionDressIds);
    //            info.mutateColorParam = PlayerModel.GetDyeColorParams(playerDressInfo);
    //        }
    //        //坐骑
    //        if (rideMountNotify != null && !info.IsTransformModel && rideMountNotify.mountId > 0)
    //        {
    //            SetRideMount(info, rideMountNotify, rideLevel);
    //            info.rideEffect = true;
    //        }
    //        else
    //        {
    //            info.rideId = 0;
    //        }
    //    }

    #endregion
}