// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  BattleMonsterBuff.cs
// Author   : SK
// Created  : 2013/3/5
// Purpose  : 战斗宠物头上的Buff显示
// **********************************************************************
using UnityEngine;
using System.Collections.Generic;
using AppDto;

public class BattleMonsterBuff : MonoBehaviour
{
    List<VideoBuffAddTargetState> buffList;

    private Dictionary<int, GameObject> effectMaps;

    private MonsterController _monsterController;

    private const string MCDTASKNAME_FORMAT = "BuffCoolDown_PlayerID_{0}_BuffID_{1}";

    public void SetMonster(MonsterController monsterController)
    {
        _monsterController = monsterController;

        buffList = new List<VideoBuffAddTargetState>();
        effectMaps = new Dictionary<int, GameObject>();	
    }

    public void AddOrResetBuff(VideoBuffAddTargetState buffState, bool tip = true)
    {
        if (buffState == null)
        {
            GameDebuger.LogError("[错误]AddBuff faield , buffState == null");
            return;
        }

        if (null == buffState.battleBuff)
        {
            GameDebuger.LogError(string.Format("[错误]AddBuff faield , battleBuff is null, buffId :{0}", buffState.battleBuffId));
            return;
        }

        GameDebuger.LogBattleInfo(string.Format("mcid:{0},mcname:{1} AddBuff:{2},id:{3} buffStateid:{4}", _monsterController.GetId(), _monsterController.videoSoldier.name, buffState.battleBuff.name, buffState.battleBuffId, buffState.id));

        buffList.ReplaceOrAdd(b => b.id == buffState.id, buffState);
        AddBuffEffect(buffState.battleBuff);
    }

    private Color GetBuffTipColor(SkillBuff buffer)
    {
        string colorStr = "fec742";
//		if (buffer.skillAiType == 2){
//			colorStr = "fec742";
//		}else{
//			colorStr = "b054ea";
//		}
        return ColorExt.Parse(colorStr);
    }

    public void RemoveBuff(VideoBuffAddTargetState buffState)
    {
        if (buffState == null)
        {
            GameDebuger.Log("buffState == null");
            return;
        }
		
        GameDebuger.LogBattleInfo(_monsterController.videoSoldier.name + " RemoveBuff " + buffState.battleBuff.name + " id=" + buffState.battleBuffId );
        buffList.Remove(buffState);
        RemoveBuffEffect(buffState.battleBuff);

        //string info = "RemoveBuff "+buff.buffer.name;
        //mText.Add(info, Color.red, 1f);
		
//        VideoSoldier monsterData = _monsterController.videoSoldier;
    }

    private void AddBuffEffect(SkillBuff buffer)
    {
        if (effectMaps == null || effectMaps.ContainsKey(buffer.id))
        {
            return;
        }

        if (buffer.animationMount == null)
        {
            buffer.animationMount = ModelHelper.Mount_shadow;
        }

        if (buffer.animation != 0 && !string.IsNullOrEmpty(buffer.animationMount))
        {
            Transform mountTransform = _monsterController.transform.GetChildTransform(buffer.animationMount);
            if (mountTransform == null)
            {
                return;
            }

            var buffName = "buff_eff_" + buffer.animation;

            effectMaps[buffer.id] = null;
            AssetPipeline.ResourcePoolManager.Instance.SpawnEffectAsync(buffName,
                (go) =>
                {
                    if (go == null)
                    {
                        return;
                    }

                    //可能在加载时已经因失效等原因而被移除
                    if (!effectMaps.ContainsKey(buffer.id))
                    {
                        AssetPipeline.ResourcePoolManager.Instance.DespawnEffect(go);
                        return;
                    }
                    else
                        effectMaps[buffer.id] = go;

                    if (buffer.animationMount == ModelHelper.Mount_shadow)
                    {
                        GameObjectExt.AddPoolChild(_monsterController.GetBattleGroundMount().gameObject, go);

//						if (!IsNeedRotationGroundEffect(buffName))
//						{
//							go.transform.localPosition = new Vector3(0f, 0.01f, 0f);
//							BattleShadow shadow = effectGO.GetMissingComponent<BattleShadow>();
//							shadow.Setup(_monsterController.GetBattleGroundMount());
//						}
                    }
                    else
                    {
                        //Utility.ResetPetMountRotation(_monsterController.gameObject, mountTransform);
                        GameObjectExt.AddPoolChild(mountTransform.gameObject, go);
                        NoRotation noRotation = go.GetMissingComponent<NoRotation>();
                        noRotation.fixYToZero = false;						

//						if (!IsNeedRotationGroundEffect(buffName))
//						{
//							NoRotation noRotation = GameObjectExt.GetMissingComponent<NoRotation>(effectGO);
//							noRotation.fixYToZero = false;
//						}
//						ParticleScaler scaler = GameObjectExt.GetMissingComponent<ParticleScaler>(effectGO);
//						float scaleValue = 1;
//						scaleValue /= (_monsterController.monsterData.hero.scale / 10000f);
//						scaler.SetscaleMultiple( scaleValue );
                    }
				});
        }
    }

    private bool IsNeedRotationGroundEffect(string buffName)
    {
        return buffName.Contains("buff_eff_120") || buffName.Contains("buff_eff_137");
    }

    private void RemoveBuffEffect(SkillBuff buffer)
    {
        if (effectMaps.ContainsKey(buffer.id))
        {
            GameObject effect = effectMaps[buffer.id];
            if (null != effect)
                AssetPipeline.ResourcePoolManager.Instance.DespawnEffect(effect);
            effectMaps.Remove(buffer.id);
        }
    }

    public void RemoveAllBuff()
    {
        foreach (GameObject effect in effectMaps.Values)
        {
            AssetPipeline.ResourcePoolManager.Instance.DespawnEffect(effect);
        }
        if (effectMaps != null)
        {
            effectMaps.Clear();
        }
        if (buffList != null)
        {
            buffList.Clear();
        }
    }

    public void SetActive(bool b)
    {
//        gameObject.SetActive(b);
        SetVisible(b);
    }

    public void SetVisible(bool pVisible)
    {
        if (null == effectMaps || effectMaps.Count <= 0)
            return;
        Dictionary<int, GameObject>.Enumerator tEnum = effectMaps.GetEnumerator();
        while (tEnum.MoveNext())
        {
            tEnum.Current.Value.SetActive(pVisible);
        }
    }


    void OnDestroy()
    {
        buffList = null;
        _monsterController = null;
    }
}

