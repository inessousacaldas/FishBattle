using System;
using System.Collections.Generic;
using AppDto;
using AssetPipeline;
using UnityEngine;

public static class BattlePlayHelper
{
    public static ModelHelper.AnimType GetAnimType(this string name,
        ModelHelper.AnimType defaultValue = ModelHelper.AnimType.hit)
    {
        return EnumParserHelper.TryParse(name, defaultValue);
    }

    public static void NotNullInvoke(this Action act)
    {
        if (act == null) return;
        act.Invoke();
    }

    public static void NotNullInvoke<T>(this Action<T> act,T arg)
    {
        if (act == null) return;
        act.Invoke(arg);
    }

    public static void SpawnAllEffectsAsync(
        this List<string> tSkillNames
        , MonsterController monster
        , GameObject pParent
        , NormalEffectInfo node
        , Action onFinishCallBack, Action<string> onError)
    {
        if (monster == null || null == tSkillNames || tSkillNames.Count <= 0)
        {
            //GameDebuger.LogError("SpawnAllEffectsAsync failed ,tSkillNames' count is invalid !");
            onError.NotNullInvoke("SpawnAllEffectsAsync failed ,tSkillNames' count is invalid !");
            onFinishCallBack.NotNullInvoke();
            return;
        }

        var leftCountToSpawn = tSkillNames.Count;
        Action checkFinish = () =>
        {
            leftCountToSpawn--;
            if (leftCountToSpawn <= 0)
            {
                if (null != onFinishCallBack)
                {
                    onFinishCallBack();
                    onFinishCallBack = null;
                }
            }
        };
        for (var tCounter = 0; tCounter < tSkillNames.Count; tCounter++)
        {
            var tSkillName = tSkillNames[tCounter];
            ResourcePoolManager.Instance.SpawnEffectAsync(tSkillName, (tChainEffectTrans) =>
            {
                string errMsg = null;
                if (null == tChainEffectTrans)
                {
                    //GameDebuger.LogWarning(string.Format("特效资源加载失败，名字：{0}", tSkillName));
                    errMsg = string.Format("特效资源加载失败，名字：{0}", tSkillName);
                }else if (monster == null)//异步回调过程中Gameobject可能被删除导致MC为空，不可以删除这个检查
                {
                    ResourcePoolManager.Instance.DespawnEffect(tChainEffectTrans.gameObject);
                    errMsg = "monster controller 被删除了";
                }

                if (errMsg != null)
                {
                    onError.NotNullInvoke(errMsg);
                }
                else
                {
                    GameObjectExt.AddPoolChild(pParent, tChainEffectTrans);
                }

                checkFinish();
            }, () =>
            {
                //GameDebuger.LogWarning(string.Format("特效资源加载失败，名字：{0}", tSkillName));
                onError.NotNullInvoke(string.Format("特效资源加载失败，名字：{0}", tSkillName));
                checkFinish();
            });
        }
    }

    public static Vector3 GetEffStartPos(this NormalEffectInfo eff, GameObject root, MonsterController monster, Vector3 offset)
    {
        //位移
        /**加一段偏移位移的世界坐标版*/
        var startPos = root.transform.TransformVector(offset);

        switch (eff.target)
        {
            case EffectTargetType.defaultVal: //默认
                Transform mountTransform = null;
                if (!string.IsNullOrEmpty(eff.mount))
                {
                    mountTransform = monster.transform.GetChildTransform(eff.mount);
                }
                if (mountTransform == null)
                {
                    mountTransform = monster.gameObject.transform;
                }
                if (eff.mount == ModelHelper.Mount_shadow)
                {
                    startPos += new Vector3(mountTransform.position.x, mountTransform.position.y, mountTransform.position.z);
                }
                else
                {
                    startPos += mountTransform.position;
                }
                break;
            case EffectTargetType.scene: //场景中心
                startPos += Vector3.zero;
                break;
            case EffectTargetType.player: //我方中心
                startPos += BattlePositionCalculator.GetZonePosition(monster.side);
                break;
            case EffectTargetType.enemy: //敌方中心
                startPos =
                    BattlePositionCalculator.GetZonePosition(
                        monster.side == BattlePosition.MonsterSide.Player
                            ? BattlePosition.MonsterSide.Enemy
                            : BattlePosition.MonsterSide.Player);
                break;
        }

        return startPos;
    }
}