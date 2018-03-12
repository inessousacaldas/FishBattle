using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AssetPipeline;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
///     Model helper.
///     <para>包含了模型帮助函数，大致分为三大类</para>
///     <para>1.获取模型资源路径</para>
///     <para>2.设置模型材质，或一些附加效果</para>
///     <para>3.模型动作相关方法</para>
/// </summary>

public static class ModelHelper
{
    public enum AnimType
    {
        // 无效动作
        invalid
        , randomAttack//随机工具动作
        //休闲待机
        , idle 
        //战斗待机
        , battle
        // 战斗跑步
        , forward
        //休闲跑步
        , run
//受击动作
        , hit
        //眩晕动作
        , dizzy
        //胜利结算
        , victory
        //死亡
        , death
        //普通攻击
        , attack
        //技能蓄力
        , sing
        //技能蓄力
        , sing2
        //魔法技能
        , magic
//战技动作1
        , spe1
//战技动作2
        , spe2
//战技动作3
        , spe3
//奥义动作
        , super
//表演
        , show

//坐骑待机
        , rideIdle
//坐骑走动
        , rideRun
//交互动作1
        , mood1
//交互动作2
        , mood2
//交互动作3
        , mood3
        //交互动作4
        , mood4
        , skill
        , showup //战斗进场
        , gather  // 采集
        , co1
        , co2
        , co3
        , co4
        , co5
        , co6
    }

    //模型基础动作名称
    public const int Animator_Layer_BaseLayer = 0;
    public const int Animator_Layer_BattleLayer = 1;

    public const int DefaultModelId = 101;
    //默认宠物ID

    public const float DefaultModelSpeed = 4f;
    //默认宠物speed
    public const float DefaultBattleModelSpeed = 25f;
    //默认战斗宠物speed
    public const float DefaultBattleCatchSpeed = 3f;
    //默认捕捉宠物speed
    
    //模型锚点
    public const string Mount_hit = "Mount_Hit";
    //受击锚点
    public const string Mount_hud = "Mount_HUD";
    //头顶锚点
    public const string Mount_shadow = "Mount_Shadow";
    //阴影锚点
    public const string Mount_face = "Mount_face";
    //面部锚点
    public const string Mount_ride = "Mount_ride";
    //坐骑锚点
    //器灵锚点
    public const string Mount_soul = "Mount_Soul";

    //器灵模型前缀
    public const string PREFIX_SOUL = "soul_";

    #region Model Path Func
    //有刀光的动作
    private static readonly List<AnimType> ANIMATION_HAS_DAO_GUANG = new List<AnimType>(4){ AnimType.attack, AnimType.skill};
    
    private const string STYLE_TYPE_PET = "pet";

    /**所有模型最顶层的骨骼名字*/
    public const string ANCHOR_TOP_BONE = "Bip001";
    public const string ANCHOR_WEAPON_1 = "Bip001 Prop1";
    public const string ANCHOR_WEAPON_2 = "Bip001 Prop2";
    /**武器挂点位置，路径必须对，否则只能一层层查*/
    public const string ANCHOR_WEAPON_PATH_1 = ANCHOR_TOP_BONE + "/" + ANCHOR_WEAPON_1;
    public const string ANCHOR_WEAPON_PATH_2 = ANCHOR_TOP_BONE + "/" + ANCHOR_WEAPON_2;
    /**手持挂点*/
    public const string ANCHOR_HANDLE = ANCHOR_WEAPON_PATH_2;

    private static List<string> WeaponEffNames = new List<string>
    {
        "weapon_{0}_eff",
        "weapon_{0}_eff_1",
        "weapon_{0}_eff_2",
        "weapon_{0}_eff_3",
        "weapon_{0}_eff_4",
        "weapon_{0}_eff_5",
    };

    private static Dictionary<string, WeaponBindConfig> weaponConfigMaps;

    public static void Setup()
    {
        if (weaponConfigMaps != null) return;
        weaponConfigMaps = new Dictionary<string, WeaponBindConfig>();

        ResourcePoolManager.Instance.LoadConfig("WeaponConfig", asset =>
        {
            if (asset == null)
                return;
            var textAsset = asset as TextAsset;
            if (textAsset == null)
                return;
            var weaponConfig = JsHelper.ToObject<WeaponConfig>(textAsset.text);
            if (weaponConfig == null) return;
            for (var i = 0; i < weaponConfig.list.Count; i++)
            {
                var config = weaponConfig.list[i];
                weaponConfigMaps.Add(config.key, config);
            }
        });
    }

    private static WeaponBindConfig GetWeaponBindConfig(string key)
    {
        WeaponBindConfig config = null;
        if (weaponConfigMaps != null)
        {
            weaponConfigMaps.TryGetValue(key, out config);
        }
        return config;
    }

    public static string AnimToString(AnimType animType)
    {
        return animType == ModelHelper.AnimType.invalid ? "" : animType.ToString();
    }

    public static string GetCharacterPrefabPath(int modelId)
    {
        return STYLE_TYPE_PET + "_" + modelId;
    }

    #endregion

    #region Material or Shader Func

    //设置宠物外观
    public static void SetPetLook(GameObject modelGo, int mainTexture, int mutateTexture, string colorParams,
                                  int ornamentId, string ornamentColorParam, Action onFinish = null)
    {
        if (modelGo == null)
        {
            if (onFinish != null)
            {
                onFinish();
            }
            GameDebuger.Log("SetPetLook Error petObject = null");
            return;
        }

        if (mainTexture == 0 && mutateTexture == 0)
        {
            if (onFinish != null)
            {
                onFinish();
            }
            GameDebuger.Log("SetPetLook Error mainTexture and mutateTexture = 0");
            return;
        }

        //遍历找到模型Renderer所在节点
        var petTrans = modelGo.transform;
        var childNodeName = Regex.Match(modelGo.name, "\\bpet_\\d+\\b").Value;

        var activeOrnament = ornamentId > 0;

        Renderer modelRenderer = null;
        for (int i = 0, imax = petTrans.childCount; i < imax; ++i)
        {
            var child = petTrans.GetChild(i);
            //fix ornament miss texture issue
            if (child.CompareTag(GameTag.Tag_NewOrnament))
            {
                child.gameObject.SetActive(activeOrnament);

                if (!activeOrnament) continue; //饰品染色
                Renderer ornamentRenderer = child.GetComponent<Renderer>();
                ChangeModelHSV(ornamentRenderer, ornamentColorParam);

                //变异大耳兔配饰特殊处理,选取Mutate贴图进行渲染
                if (mainTexture != 2010) continue;
                if (ornamentRenderer == null || ornamentRenderer.material == null) continue;
                var ornamentMat = ornamentRenderer.material;
                var val = mutateTexture > 0 ? 1f : 0f; 
                ornamentMat.SetFloat("_blendFactorR", val);
                ornamentMat.SetFloat("_blendFactorG", val);
                ornamentMat.SetFloat("_blendFactorB", val);
            }
            else if (child.CompareTag(GameTag.Tag_DefaultOrnament))
            {
                child.gameObject.SetActive(!activeOrnament);
            }
            else if (child.name.Trim().Equals(childNodeName))
            {
                modelRenderer = child.GetComponent<SkinnedMeshRenderer>();
            }
        }

        //针对宠物做宠物特效的特殊处理,受配件影响
        if (mainTexture > 1000)
        {
            HandlePetEffect(petTrans, activeOrnament);
        }

        if (modelRenderer != null)
        {
            modelRenderer.enabled = true;

            var curMatName = modelRenderer.material.name.Replace(" (Instance)", "");
            var newMatName = "";

            if (mutateTexture > 0)
            {
                newMatName = "pet_" + mutateTexture + "_mutate";
            }
            else if (!string.IsNullOrEmpty(colorParams))
            {
                newMatName = "pet_" + mainTexture + "_mask";
            }
            else
            {
                newMatName = "pet_" + mainTexture;
            }

            if (curMatName == newMatName)
            {
                ChangeModelHSV(modelRenderer, colorParams);

                if (onFinish != null)
                {
                    onFinish();
                }
            }
            else
            {
                ChangeMaterialAsync(modelRenderer, "model/" + modelRenderer.name + "_mat", newMatName, colorParams, onFinish);
            }
        }
        else
        {
            if (onFinish != null)
            {
                onFinish();
            }
        }
    }

    private static Dictionary<string, UnitEffectAndOrnamentConfig> _unitEffectAndOrnamentMaps = new Dictionary<string, UnitEffectAndOrnamentConfig>();

    //遍历处理宠物特效（因为有些特效受到配饰控制）
    private static void HandlePetEffect(this Transform root, bool showOrnament)
    {
        GameDebuger.TODO(@"bool unitEffectToggle = ModelManager.SystemData.unitEffectToggle;");
        var unitEffectToggle = true;

        if (root.childCount == 0) return;
        for (var i = 0; i < root.childCount; i++)
        {
            var childTransform = root.GetChild(i);
            if (childTransform == null) continue;
            HandlePetEffect(childTransform, showOrnament);

            if (childTransform.CompareTag(GameTag.Tag_UnitEffect))
            {
                childTransform.gameObject.SetActive(unitEffectToggle);
            }
            else if (childTransform.CompareTag(GameTag.Tag_NewOrnament))
            {
                childTransform.gameObject.SetActive(showOrnament);
            }
            else if (childTransform.CompareTag(GameTag.Tag_DefaultOrnament))
            {
                childTransform.gameObject.SetActive(!showOrnament);
            }
        }
    }

    private static UnitEffectAndOrnamentConfig GetUnitEffectAndOrnamentConfig(string key)
    {
        UnitEffectAndOrnamentConfig config = null;
        if (_unitEffectAndOrnamentMaps.ContainsKey(key))
        {
            config = _unitEffectAndOrnamentMaps[key];
        }
        else
        {
            config = new UnitEffectAndOrnamentConfig();
            _unitEffectAndOrnamentMaps.Add(key, config);
        }
        return config;
    }

    //设置坐骑外观
    public static void SetRideLook(GameObject modelGo, string colorParams,
                                   int ornamentId, string ornamentColorParam, bool rideMaxEffect, bool showEffect = true, Action onFinish = null)
    {
        if (modelGo == null)
        {
            if (onFinish != null)
            {
                onFinish();
            }
            GameDebuger.Log("SetRideLook Error rideObject = null");
            return;
        }

        //遍历找到模型Renderer所在节点
        var petTrans = modelGo.transform;
        var childNodeName = Regex.Match(modelGo.name, "\\bride_pet_\\d+\\b").Value;

        Renderer modelRenderer = null;
        List<Renderer> ornamentRendererList = null;

        var activeOrnament = ornamentId > 0;

        for (int i = 0, imax = petTrans.childCount; i < imax; ++i)
        {
            var child = petTrans.GetChild(i);
            //fix ornament miss texture issue
            if (child.CompareTag(GameTag.Tag_NewOrnament))
            {

                //特殊坐骑不需要饰品染色
                GameDebuger.TODO(@"if (!ModelManager.Mount.IsMutateOrnamentRide(childNodeName))");
                if (!activeOrnament) continue; //饰品染色
                if (ornamentRendererList == null)
                {
                    ornamentRendererList = new List<Renderer>();
                }

                var ornamentRenderer = child.GetComponent<Renderer>();
                if (ornamentRenderer != null)
                {
                    ornamentRendererList.Add(ornamentRenderer);
                }
            }
            else if (child.name.Trim().Equals(childNodeName))
            {
                modelRenderer = child.GetComponent<SkinnedMeshRenderer>();
            }
        }

        HandleRideEffect(petTrans, rideMaxEffect, showEffect, activeOrnament);

        if (ornamentRendererList != null)
        {
            for (var i = 0; i < ornamentRendererList.Count; i++)
            {
                ChangeModelHSV(ornamentRendererList[i], ornamentColorParam);
            }
        }

        if (modelRenderer != null)
        {
            modelRenderer.enabled = true;

            var curMatName = modelRenderer.material.name.Replace(" (Instance)", "");
            var newMatName = "";

            //特殊坐骑的加配饰方式改为换编译贴图
            GameDebuger.TODO(@"if (ModelManager.Mount.IsMutateOrnamentRide(childNodeName) && activeOrnament)
            {
                newMatName = childNodeName + '_mutate';
            }
            else");
            {
                if (!string.IsNullOrEmpty(colorParams))
                {
                    newMatName = childNodeName + "_mask";
                }
                else
                {
                    newMatName = childNodeName;
                }
            }

            if (curMatName == newMatName)
            {
                ChangeModelHSV(modelRenderer, colorParams);

                if (onFinish != null)
                {
                    onFinish();
                }
            }
            else
            {
                ChangeMaterialAsync(modelRenderer, "model/" + modelRenderer.name + "_mat", newMatName, colorParams, onFinish);
            }

            if (onFinish != null)
            {
                onFinish();
            }
        }
        else
        {
            if (onFinish != null)
            {
                onFinish();
            }
        }
    }

    private static void HandleRideEffect(this Transform root, bool showMaxEffect, bool showEffect, bool showOrnament)
    {
        GameDebuger.TODO(@"bool unitEffectToggle = ModelManager.SystemData.unitEffectToggle && showEffect;");
        var unitEffectToggle = true;

        if (root.childCount == 0) return;
        for (var i = 0; i < root.childCount; i++)
        {
            var childTransform = root.GetChild(i);
            if (childTransform == null) continue;
            HandleRideEffect(childTransform, showMaxEffect, showEffect, showOrnament);

            if (childTransform.CompareTag(GameTag.Tag_UnitEffect))
            {
                if (unitEffectToggle)
                {
                    if (childTransform.name.EndsWith("_LevelUp"))
                    {
                        childTransform.gameObject.SetActive(showMaxEffect);
                    }
                    else if (childTransform.name.EndsWith("_Default"))
                    {
                        childTransform.gameObject.SetActive(!showMaxEffect);
                    }
                    else
                    {
                        childTransform.gameObject.SetActive(true);
                    }
                }
                else
                {
                    childTransform.gameObject.SetActive(false);
                }
            }
            else if (childTransform.CompareTag(GameTag.Tag_NewOrnament))
            {
                childTransform.gameObject.SetActive(showOrnament);
            }
            else if (childTransform.CompareTag(GameTag.Tag_DefaultOrnament))
            {
                childTransform.gameObject.SetActive(!showOrnament);
            }
        }
    }

    public static void ChangeModelHSV(Renderer modelRenderer, string colorParams)
    {
        var hsv = modelRenderer.gameObject.GetMissingComponent<ModelHSV>();
        hsv.SetupColorParams(colorParams);
    }

    //异步加载材质球进行替换
    public static void ChangeMaterialAsync(Renderer modelRenderer, string bundleName, string assetName, string colorParams,
                                           Action onFinish = null)
    {
        if (modelRenderer == null)
        {
            Debug.LogError("the gameObject has not a Renderer component.");
            if (onFinish != null)
            {
                onFinish();
            }
            return;
        }

        modelRenderer.enabled = false;
        AssetManager.Instance.LoadAssetAsync(bundleName, assetName, asset =>
            {
                if (modelRenderer == null)
                {
                    return;
                }
                //实例化,材质//
                //读取材质球//
                Material newMaterial = asset as Material;
                if (newMaterial == null)
                {
                    GameDebuger.Log(string.Format("asset not found material!,path:{0}", assetName));
                    modelRenderer.enabled = true;
                    return;
                }

                Object.DestroyImmediate(modelRenderer.material);
                modelRenderer.material = newMaterial;
                ChangeModelHSV(modelRenderer, colorParams);
                modelRenderer.enabled = true;
                if (onFinish != null)
                {
                    onFinish();
                }
            }, () =>
            {
                if (modelRenderer != null)
                    modelRenderer.enabled = true;
                if (onFinish != null)
                {
                    onFinish();
                }
            });
    }

    public static void RemovePetShadow(GameObject model)
    {
        if (model == null)
        {
            return;
        }

        var shadowRoot = model.transform.Find(Mount_shadow);
        if (shadowRoot == null) return;
        var shadow = shadowRoot.Find("Shadow(Clone)");
        if (shadow != null)
        {
            ResourcePoolManager.Instance.DespawnModel(shadow);
        }
    }

    public static void SetPetShadow(GameObject modelGo, float scale = 1f)
    {
        if (modelGo == null)
        {
            return;
        }

        var shadowRoot = modelGo.transform.Find(Mount_shadow);
        if (shadowRoot == null) return;
        var shadow = shadowRoot.Find("Shadow(Clone)");
        if (shadow == null)
        {
            ResourcePoolManager.Instance.SpawnModelAsync(PathHelper.SHADOW_PREFAB_PATH, shadowGO =>
            {
                if (shadowGO == null)
                    return;

                if (shadowRoot == null)
                {
                    ResourcePoolManager.Instance.DespawnModel(shadowGO);
                    return;
                }

                var t = shadowGO.transform;
                t.parent = shadowRoot;
                shadowGO.layer = shadowRoot.gameObject.layer;
                t.localPosition = Vector3.zero;
                t.localEulerAngles = new Vector3(90f, 0f, 0f);
                t.localScale = new Vector3(scale, scale, scale);
                shadowGO.name = "Shadow(Clone)";
            });
        }
        else
        {
            shadow.gameObject.layer = shadowRoot.gameObject.layer;
        }
    }

    /// <summary>
    /// 获取一个子节点,节点名前缀为prefix的
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="prefix"></param>
    /// <returns></returns>
    public static Transform GetChildNodeStartwith(Transform parent, string prefix)
    {
        if (parent == null)
            return null;

        for (int i = 0, imax = parent.childCount; i < imax; ++i)
        {
            var child = parent.GetChild(i);
            if (child.name.StartsWith(prefix))
                return child;
        }

        return null;
    }

    public static void SetSurroundEffectActive(GameObject modelGo, string effName, Action<GameObject> onLoadFinish = null)
    {
        SetModelEffectActive(modelGo, effName, "roundeff", onLoadFinish);
    }

    public static void SetFootprintActive(GameObject modelGo, string effName, Action<GameObject> onLoadFinish = null)
    {
        SetModelEffectActive(modelGo, effName, "footmark", onLoadFinish);
    }

    public static void SetSoulEffectActive(GameObject modelGo, string effName, Action<GameObject> onLoadFinish = null)
    {
        SetModelEffectActive(modelGo, effName, PREFIX_SOUL, effName, Mount_soul, onLoadFinish);
    }

    private static void SetModelEffectActive(GameObject modelGo, string effName, string prefix, Action<GameObject> onLoadFinish = null)
    {
        SetModelEffectActive(modelGo, effName, prefix, effName, Mount_shadow, onLoadFinish);
    }

    private static void SetModelEffectActive(GameObject modelGo, string effName, string prefix, string pResPathName, string pMountName, Action<GameObject> onLoadFinish = null)
    {
        if (modelGo == null)
            return;

        var shadowRoot = modelGo.transform.Find(pMountName);
        if (shadowRoot == null) return;
        //先查找已经设置好的环绕特效节点
        var effectNode = GetChildNodeStartwith(shadowRoot, prefix);

        //清空环绕特效
        if (string.IsNullOrEmpty(effName))
        {
            if (effectNode != null)
                ResourcePoolManager.Instance.DespawnEffect(effectNode);
        }
        else
        {
            if (effectNode != null)
            {
                //如果已经生成当前特效,直接返回
                if (effectNode.name.Contains(effName))
                {
                    if (onLoadFinish != null)
                        onLoadFinish(effectNode.gameObject);
                    return;
                }

                //清空旧环绕特效
                ResourcePoolManager.Instance.DespawnEffect(effectNode);
            }

            ResourcePoolManager.Instance.SpawnEffectAsync(pResPathName, effGo =>
            {
                if (effGo == null)
                    return;

                if (shadowRoot == null)
                {
                    ResourcePoolManager.Instance.DespawnEffect(effGo);
                    return;
                }

                //添加到节点前再检测一次是否有添加到节点的预设
                effectNode = GetChildNodeStartwith(shadowRoot, prefix);
                if (null != effectNode)
                    ResourcePoolManager.Instance.DespawnEffect(effectNode);


                GameObjectExt.AddPoolChild(shadowRoot.gameObject, effGo);
                if (onLoadFinish != null)
                    onLoadFinish(effGo);
            });
        }
    }

    //移除足迹和氛围特效
    public static void RemoveSurroundAndFootEffect(GameObject modelGo)
    {
        RemoveEffectWithPrefix(modelGo, "roundeff");
        RemoveEffectWithPrefix(modelGo, "footmark");
    }

    private static void RemoveEffectWithPrefix(GameObject modelGo, string prefix)
    {
        if (modelGo == null)
            return;

        var shadowRoot = modelGo.transform.Find(Mount_shadow);
        if (shadowRoot == null) return;
        //先查找已经设置好的环绕特效节点
        var effectNode = GetChildNodeStartwith(shadowRoot, prefix);

        //清空环绕特效
        if (effectNode != null)
            ResourcePoolManager.Instance.DespawnEffect(effectNode);
    }

    #endregion

    #region ChangeModelWeapon

    public static void UpdateHandModel(GameObject go, string bodyModel, string handModel, bool withEff = true)
    {
        GameDebuger.TODO(@"if (!ModelManager.SystemData.unitEffectToggle)
        {
            withEff = false;
        }");

        RemoveBindModel(go, "Bip001/Bip001 Prop1");
        RemoveBindModel(go, "Bip001/Bip001 Prop2");

        DoUpdateHandModel(go, handModel,
            "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Mount_handle");
    }

    public static void RemoveAllBindModel(GameObject go)
    {
        RemoveBindModel(go, "Bip001/Bip001 Prop1");
        RemoveBindModel(go, "Bip001/Bip001 Prop2");

        RemoveBindModel(go,
            "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Mount_handle");
    }

    /// <summary>
    ///     删除绑定的模型， 如武器，时装手持物
    /// </summary>
    /// <param name="go">Go.</param>
    /// <param name="path">Path.</param>
    private static void RemoveBindModel(GameObject go, string path)
    {
        if (go == null)
        {
            Debug.LogError("RemoveBindModel go=null");
            return;
        }

        Transform t = go.transform.Find(path);
        if (t == null) return;
        for (int i = t.childCount - 1; i >= 0; i--)
        {
            var child = t.GetChild(i);
            if (child.name.Contains("weapon_"))
            {
                for (int j = child.childCount - 1; j >= 0; j--)
                {
                    var wpNode = child.GetChild(j);
                    if (!wpNode.name.Contains("weapon_")) continue;
                    for (int k = wpNode.childCount - 1; k >= 0; k--)
                    {
                        var effNode = wpNode.GetChild(k);
                        if (effNode.name.Contains("_eff"))
                        {
                            ResourcePoolManager.Instance.DespawnEffect(effNode);
                        }
                    }
                }

                ResourcePoolManager.Instance.DespawnModel(child);
            }
            else if (child.name.Contains("_handle"))
            {
                ResourcePoolManager.Instance.DespawnModel(child);
            }
        }
    }

    private static void DoUpdateHandModel(GameObject modelGo, string handModel, string bip001Name)
    {
        Transform t = modelGo.transform.Find(bip001Name);
        if (t == null)
        {
            GameDebuger.LogError(string.Format("DoUpdateHandModel cannot find {0} at {1}", bip001Name, modelGo));
            return;
        }

        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            var child = t.GetChild(i);
            if (child.name.Contains("_handle"))
            {
                ResourcePoolManager.Instance.DespawnModel(child);
            }
        }

        ResourcePoolManager.Instance.SpawnModelAsync(handModel, wpGO =>
            {
                if (wpGO == null) return;
                if (t == null)
                {
                    ResourcePoolManager.Instance.DespawnModel(wpGO);
                    return;
                }

                for (int i = 0, imax = t.childCount; i < imax; ++i)
                {
                    var child = t.GetChild(i);
                    if (child.name.Contains("_handle"))
                    {
                        ResourcePoolManager.Instance.DespawnModel(child);
                    }
                }

                if (wpGO == null) return;
                GameObjectExt.AddPoolChild(modelGo, wpGO);
                var wpTrans = wpGO.transform;
                wpTrans.parent = t;
                wpTrans.localPosition = Vector3.zero;
                wpTrans.localEulerAngles = Vector3.zero;
                wpTrans.localScale = Vector3.one;
            });
    }

    public static void UpdateModelWeapon(GameObject go, int actorModelId, int wpId, int withEffLv = 1, int weaponEffId = 0, bool uiMode = false, Action<GameObject> onLoadFinish = null)
    {
        RemoveBindModel(go,
            "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Mount_handle");

        DoUpdateModelWeapon(go, actorModelId, wpId, ANCHOR_WEAPON_PATH_1, withEffLv, weaponEffId, uiMode, onLoadFinish);
        DoUpdateModelWeapon(go, actorModelId, wpId, ANCHOR_WEAPON_PATH_2, withEffLv, weaponEffId, uiMode, onLoadFinish);
    }

    private static Transform FindChildContainsName(this Transform trans, string name)
    {
        if (trans == null)
            return null;
        for (var i = 0; i < trans.childCount; i++)
        {
            var childItem = trans.GetChild(i);
            if (childItem.name.Contains(name))
                return childItem;
        }
        return null;
    }

    private static void UnloadWeapon(Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--)
        {
            var child = t.GetChild(i);
            if (child.name.Contains("weapon_"))
            {
                for (int j = child.childCount - 1; j >= 0; j--)
                {
                    var wpNode = child.GetChild(j);
                    if (wpNode.name.Contains("weapon_"))
                    {
                        for (int k = wpNode.childCount - 1; k >= 0; k--)
                        {
                            var effNode = wpNode.GetChild(k);
                            if (effNode.name.Contains("_eff"))
                            {
                                ResourcePoolManager.Instance.DespawnEffect(effNode.gameObject);
                            }
                        }
                    }
                }
                
                ResourcePoolManager.Instance.DespawnModel(child.gameObject);
            }
            else if (child.name.Contains("_handle"))
            {
                ResourcePoolManager.Instance.DespawnModel(child.gameObject);
            }
        }
    }

    private static void DoUpdateModelWeapon(
        GameObject modelGo
        , int actorModelId
        , int wpId
        , string wpNodeName
        , int withEffLv
        , int weaponEffId
        , bool uiMode
        , Action<GameObject> onLoadFinish = null)
    {
        bool showWeaponEffect = withEffLv > 0;

        var t = modelGo.transform.Find(wpNodeName);
        if (null == t)
        {
            if (wpId != 0 && (wpNodeName.Contains(ANCHOR_WEAPON_1) || wpNodeName.Contains(ANCHOR_WEAPON_2)))
                GameDebuger.LogError(string.Format("[Error]模型({0})({1})加载武器({2})失败，模型上没有指定的武器节点{3}！", modelGo, actorModelId, wpId, wpNodeName));
            if (null != onLoadFinish)
                onLoadFinish(null);
            return;
        }

        if (wpId == 0)
        {
            UnloadWeapon(t);
        }
        else
        {
            var oldWpTrans = t.FindChildContainsName("weapon_" + wpId);
            var needLoadEffect = (wpId % 10) >= 3;
            if (oldWpTrans != null && needLoadEffect)
            {
                //需要显示特效
                LoadWeaponEffect(wpId, wpNodeName, withEffLv, weaponEffId, uiMode, showWeaponEffect, t, oldWpTrans);
                GameUtil.SafeRun(onLoadFinish, t.gameObject);
            }
            else
                LoadWeapon(modelGo, actorModelId, wpId, wpNodeName, withEffLv, weaponEffId, uiMode, showWeaponEffect, t,
                    needLoadEffect, onLoadFinish);
        }
    }

    private static void LoadWeapon(
        GameObject modelGo
        , int actorModelId
        , int wpId
        , string wpNodeName
        , int withEffLv
        , int weaponEffId
        , bool uiMode
        , bool showWeaponEffect
        , Transform t
        , bool needLoadEffect
        , Action<GameObject> onLoadFinish = null)
    {
        var wpPath = "weapon_" + wpId;
        ResourcePoolManager.Instance.SpawnModelAsync(wpPath, wpGO =>
            {
                if (wpGO == null)
                    return;
                
                for (int i = t.childCount - 1; i >= 0; i--)
                {
                    var child = t.GetChild(i);
                    if (child.name.Contains("weapon_") || child.name.Contains("_handle"))
                    {
                        ResourcePoolManager.Instance.DespawnModel(child);
                    }
                }

                GameObjectExt.AddPoolChild(modelGo, wpGO);
                var wpTrans = wpGO.transform;
                wpGO.SetParent(t.gameObject);

                //legacy D1直接挂到目标挂点下，不需要挂点配置，2017年09月04日16:37:42
                /**var config = GetWeaponBindConfig("pet_" + actorModelId + "/" + wpNodeName);
                if (config != null)*/
                {
//                    wpTrans.localPosition = config.localPosition;
//                    wpTrans.localEulerAngles = config.localEulerAngles;

                    if (wpNodeName == ANCHOR_WEAPON_PATH_1)
                    {
                        //如果是第1把武器，则判断匹配的2手武器需要翻转
                        var subTrans = wpTrans.Find("weapon_" + wpId + "_Prop2");
                        if (subTrans != null)
                        {
                            //把第2把武器名字修改，改为原名
                            subTrans.name = "weapon_" + wpId;

                            subTrans.localEulerAngles = new Vector3(subTrans.localEulerAngles.x,
                                subTrans.localEulerAngles.y - 180f, subTrans.localEulerAngles.z);
                            subTrans.localPosition = new Vector3(-1 * subTrans.localPosition.x,
                                subTrans.localPosition.y, subTrans.localPosition.z);
                        }
                    }
                    else if (wpNodeName == ANCHOR_WEAPON_PATH_2)
                    {
                        //如果是第2把武器，则判断匹配的1手武器需要翻转
                        var subTrans = wpTrans.Find("weapon_" + wpId);
                        if (subTrans != null)
                        {

                            //把第一把武器名字修改,加_Prop2
                            subTrans.name = "weapon_" + wpId + "_Prop2";

                            subTrans.localEulerAngles = new Vector3(subTrans.localEulerAngles.x,
                                subTrans.localEulerAngles.y + 180f, subTrans.localEulerAngles.z);
                            subTrans.localPosition = new Vector3(-1 * subTrans.localPosition.x,
                                subTrans.localPosition.y, subTrans.localPosition.z);
                        }
                    }
                }
//                if (needLoadEffect)
                {
                    LoadWeaponEffect(wpId, wpNodeName, withEffLv, weaponEffId, uiMode, showWeaponEffect, t, wpTrans);
                }
                GameUtil.SafeRun(onLoadFinish, wpGO);
            });
    }
    
    private static void LoadWeaponEffect(
        int wpId
        , string wpNodeName
        , int withEffLv
        , int weaponEffId
        , bool uiMode
        , bool showWeaponEffect
        , Transform t
        , Transform oldWpTrans)
    {
        if (t == null || oldWpTrans == null)
        {
            return;
        }
        var oldWpGO = oldWpTrans.gameObject;
        //var index = 1; //EquipmentMainDataMgr.DataMgr.GetWeaponEffectIndex(weaponEffId);
        
        var effPath = string.Format(WeaponEffNames[weaponEffId], wpId);

        var childNodeName = string.Format(wpNodeName == ANCHOR_WEAPON_PATH_2 ? "weapon_{0}_Prop2/" : "weapon_{0}/", wpId);

        var effTrans = FindOldEffect(oldWpTrans, effPath, childNodeName);

        if (effTrans != null)
        {
            ShowWeaponEff(oldWpGO, showWeaponEffect, wpId, wpNodeName, withEffLv, weaponEffId, uiMode);
        }
        else
        {
            ResourcePoolManager.Instance.SpawnEffectAsync(effPath, loadEffGO =>
                {
                    if (loadEffGO == null) return;
                    
                    for (int i = oldWpTrans.childCount - 1; i >= 0; i--)
                    {
                        var child = oldWpTrans.GetChild(i);
                        if (child.name.Contains(effPath) || child.name.Contains("_handle"))
                        {
                            ResourcePoolManager.Instance.DespawnEffect(child);
                        }
                    }

                    var subWpPath = string.Format("weapon_{0}", wpId);

                    var subWpTrans = oldWpTrans.FindChildContainsName(subWpPath);

                    if (subWpTrans == null) return;
                    GameObjectExt.AddPoolChild(subWpTrans.gameObject, loadEffGO);
                    loadEffGO.transform.parent = subWpTrans;
                    ShowWeaponEff(oldWpGO, showWeaponEffect, wpId, wpNodeName, withEffLv, weaponEffId, uiMode);
                });
        }
    }

    private static Transform FindOldEffect(Transform oldWpTrans, string effPath, string childNodeName)
    {
        Transform effTrans = null;
        for (int i = 0, imax = oldWpTrans.childCount; i < imax; i++)
        {
            var wpNode = oldWpTrans.GetChild(i);
            if (!wpNode.name.Contains(childNodeName)) continue;
            for (int j = 0, jmax = wpNode.childCount; j < jmax; j++)
            {
                var effNode = wpNode.GetChild(j);
                var effName = effNode.name;
                if (!effName.Contains(effPath) || (effName.Contains("eff_") != effPath.Contains("eff_"))) continue;
                effTrans = effNode;
                break;
            }
        }

        return effTrans;
    }


    private static void ShowWeaponEff(GameObject wpGO, bool showWeaponEffect, int wpId, string wpNodeName, int withEffLv, int weaponEffId, bool uiMode)
    {
        var startPath = wpNodeName == "Bip001/Bip001 Prop2" ? "weapon_{0}_Prop2/" : "weapon_{0}/";

        GameDebuger.TODO(@"int index = ModelManager.Equipment.GetWeaponEffectIndex(weaponEffId);");
        var index = 1;
        var targetEffName = string.Format(WeaponEffNames[index - 1], wpId);

        GameObject targetEffGO = null;
        var subWpTrans = wpGO.transform.Find(string.Format(startPath, wpId));
        if (subWpTrans != null)
        {
            for (var i = subWpTrans.childCount - 1; i >= 0; i--)
            {
                var child = subWpTrans.GetChild(i);
                var childName = child.name;
                if (targetEffGO == null && childName.Contains(targetEffName)
                    && ((!childName.Contains("eff_") && !targetEffName.Contains("eff_")) || (childName.Contains("eff_") && targetEffName.Contains("eff_"))))
                {
                    targetEffGO = child.gameObject;
                }
                else
                {
                    ResourcePoolManager.Instance.DespawnEffect(child);
                }
            }
        }

        if (targetEffGO == null
            || showWeaponEffect && targetEffGO.transform.childCount <= 0)
            return;
        
        targetEffGO.gameObject.SetActive(true);
        for (var i = 1; i <= 3; i++)
        {
            var active = (i <= withEffLv);
            var effLv3Path = string.Format("LV{0}", i);
            var effLv3GO = targetEffGO.transform.Find(effLv3Path);
            if (effLv3GO == null) continue;
            effLv3GO.gameObject.SetActive(active);
            if (!active) continue;
            if (!uiMode)
            {
                effLv3GO.tag = GameTag.Tag_UnitEffect;
                GameDebuger.TODO(@"ToggleWeaponEffect(effLv3GO.gameObject,
                                    ModelManager.SystemData.weaponEffectToggle);");
            }
            else
            {
                ToggleWeaponEffect(effLv3GO.gameObject, true);
            }
        }
    }

    #endregion

    #region Animation Func

    public static void PlayAnimation(
        Animator anim
        , AnimType action
        , bool crossFade
        , Action<AnimType, float> animClipCallBack = null
        , bool checkSameAnim = false
        , int layer = 0)
    {
        if (action == AnimType.invalid)
            return;
        
        if (anim == null)
        {
            if (animClipCallBack != null)
                animClipCallBack(action, 0);
            return;
        }

        if (checkSameAnim)
        {
            var animatorState = anim.GetCurrentAnimatorStateInfo(layer);
            if (animatorState.IsName(action.ToString()))
            {
                if (animClipCallBack != null)
                    animClipCallBack(action, 0);
                return;
            }
        }

        try
        {
            anim.SetLayerWeight(Animator_Layer_BattleLayer, layer == Animator_Layer_BaseLayer ? 0 : 1);

            if (crossFade)
            {
                anim.CrossFade(AnimToString(action), 0.2f, layer);
            }
            else
            {
                GameDebuger.LogError(AnimToString(action).ToString());
                anim.Play(AnimToString(action), layer, 0f);
            }
            if (animClipCallBack != null)
                animClipCallBack(action, anim.GetClipLength(action.ToString()));
        }
        #pragma warning disable 0168
        catch (Exception e)
        #pragma warning restore
        {
            TipManager.AddTip(" Can not find action : " + action);
            GameDebuger.Log(" Can not find action : " + action);
            if (animClipCallBack != null)
                animClipCallBack(action, 0);
        }
    }

    #endregion

    public static Transform GetMountingPoint(GameObject obj, string point)
    {
        if (obj == null)
        {
            return null;
        }

        var mountTF = obj.transform.Find(point);

        if (mountTF == null)
        {
            Debug.LogError("模型" + obj.name + " 没有配置锚点:" + point);
        }

        return mountTF;
    }

    public static void ToggleAllUnitEffect(bool toggle)
    {
        var goNodes = GameObject.FindGameObjectsWithTag(GameTag.Tag_UnitEffect);
        for (int i = 0; i < goNodes.Length; i++)
        {
            var node = goNodes[i];
            if (node == null) continue;
            var nodeName = node.name;
            //不包含武器特效和时装特效
            if (nodeName.Contains("LV") || nodeName.Contains("roundeff") || nodeName.Contains("footmark")) continue;
            var t = node.transform;
            for (int j = 0, imax = t.childCount; j < imax; ++j)
            {
                var child = t.GetChild(j);
                child.gameObject.SetActive(toggle);
            }
        }
    }

    public static void ToggleAllWeaponEffect(bool toggle)
    {
        var objs = GameObject.FindGameObjectsWithTag(GameTag.Tag_UnitEffect);
        for (var i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];
            if (obj == null) continue;
            if (!obj.name.Contains("LV")) continue;
            var t = obj.transform;
            for (int j = 0, imax = t.childCount; j < imax; ++j)
            {
                var child = t.GetChild(j);
                child.gameObject.SetActive(toggle);
            }
        }
    }

    public static void ToggleAllFashionEffect(bool toggle)
    {
        ToggleSpriteEffect(toggle, go=> go.name.Contains("roundeff") || go.name.Contains("footmark"));
    }

    public static void ToggleAllHallowSpriteEffect(bool toggle)
    {
        ToggleSpriteEffect(toggle, go=> go.name.Contains(ModelHelper.PREFIX_SOUL));
    }

    private static void ToggleSpriteEffect(bool toggle, Predicate<GameObject> predicate)
    {
        var goNodes = GameObject.FindGameObjectsWithTag(GameTag.Tag_UnitEffect);
        for (var i = 0; i < goNodes.Length; i++)
        {
            var node = goNodes[i];
            if (node == null) continue;

            if (!predicate(node)) continue;
            var t = node.transform;
            for (int j = 0, imax = t.childCount; j < imax; ++j)
            {
                var child = t.GetChild(j);
                child.gameObject.SetActive(toggle);
            }
        }
    }

    public static void ToggleUnitEffect(GameObject go, bool toggle)
    {
        var t = go.transform;
        if (go.CompareTag(GameTag.Tag_UnitEffect))
        {
            var nodeName = go.name;
            //不包含武器特效和时装特效
            if (nodeName.Contains("LV") || nodeName.Contains("roundeff") || nodeName.Contains("footmark")) return;
            for (int i = 0, imax = t.childCount; i < imax; ++i)
            {
                var child = t.GetChild(i);
                child.gameObject.SetActive(toggle);
            }
        }
        else
        {
            for (int i = 0, imax = t.childCount; i < imax; ++i)
            {
                Transform child = t.GetChild(i);
                ToggleUnitEffect(child.gameObject, toggle);
            }
        }
    }

    public static void ToggleWeaponEffect(GameObject go, bool toggle)
    {
        Transform t = go.transform;
        if (go.name.Contains("LV"))
        {
            for (int i = 0, imax = t.childCount; i < imax; ++i)
            {
                Transform child = t.GetChild(i);
                child.gameObject.SetActive(toggle);
            }
        }
        else
        {
            for (int i = 0, imax = t.childCount; i < imax; ++i)
            {
                Transform child = t.GetChild(i);
                ToggleWeaponEffect(child.gameObject, toggle);
            }
        }
    }

    public static void ToggleFashionEffect(GameObject go, bool toggle)
    {
        Transform t = go.transform;
        string nodeName = go.name;
        if (nodeName.Contains("roundeff") || nodeName.Contains("footmark"))
        {
            for (int i = 0, imax = t.childCount; i < imax; ++i)
            {
                Transform child = t.GetChild(i);
                child.gameObject.SetActive(toggle);
            }
        }
        else
        {
            for (int i = 0, imax = t.childCount; i < imax; ++i)
            {
                Transform child = t.GetChild(i);
                ToggleFashionEffect(child.gameObject, toggle);
            }
        }
    }

    public static void ToggleHallowSpriteEffect(GameObject go, bool toggle)
    {
        var t = go.transform;
        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            var child = t.GetChild(i);
            if (go.name.Contains(ModelHelper.PREFIX_SOUL))
                child.gameObject.SetActive(toggle);
            else
                ToggleHallowSpriteEffect(child.gameObject, toggle);
        }
    }

    /// <summary>
    ///     设置宠物饰品的可见性
    /// </summary>
    /// <param name="go"></param>
    /// <param name="visible"></param>
    public static void SetOrnamentVisible(GameObject go, bool visible)
    {
        var _mModelTrans = go.transform;
        for (int i = 0, imax = _mModelTrans.childCount; i < imax; ++i)
        {
            var child = _mModelTrans.GetChild(i);
            if (child.CompareTag(GameTag.Tag_NewOrnament))
            {
                child.gameObject.SetActive(visible);
            }
        }
    }

    /**技能释放动作太多，只能算包含了，比如skill1_0、skill2_3*/
        public static bool IsAnimationHasDG(AnimType action)
        {
            if (action.ToString().Contains("eff"))//skill_eff等特效的播放自己进行，不做刀光播放。
                return false;
            return ANIMATION_HAS_DAO_GUANG.FindIndex(a =>a == action) >= 0;
            //        return ANIMATION_HAS_DAO_GUANG.Contains(action);
        }
}

//单位特效和配饰配置
public class UnitEffectAndOrnamentConfig
{
    public List<string> newOrnamentList = new List<string>();
    public List<string> defaultOrnamentList = new List<string>();
    public List<string> effectList = new List<string>();
}