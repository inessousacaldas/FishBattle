using System.Collections.Generic;
using AppDto;
using DG.Tweening;
using Fish;
using UnityEngine;

public partial class BaseEffectInfo
{
    public virtual IBattlePlayCtl Interprete(MoveActionInfo actInfo, SkillConfigInfo skillCfg, Skill skill,
        VideoSkillAction vsAct)
    {
        return null;
    }
	
    public virtual IBattlePlayCtl Interprete(Skill skill,VideoSkillAction vsAct)
    {
        return null;
    }

    public virtual void Play(MonsterController mc, NormalActionInfo actInfo, VideoSkillAction vsAct, VideoTargetStateGroup stateGroup)
    {
    }
}

public partial class ShowInjureEffectInfo
{
    public override void Play(MonsterController mc, NormalActionInfo actInfo, VideoSkillAction vsAct, VideoTargetStateGroup stateGroup)
    {
        BattleStateHandler.HandleBattleState(mc.GetId(), stateGroup.targetStates, BattleDataManager.DataMgr.IsInBattle);
    }

    public override IBattlePlayCtl Interprete(Skill skill,VideoSkillAction vsAct)
    {
        var victimList = vsAct.GetVictimStateGroups();
        var ctlList = new List<IBattlePlayCtl>(vsAct.GetVictimStateGroupCount());
        foreach (var tuple in victimList)
        {
            ctlList.Add(InjuredEffectPlayCtl.Create(tuple.p1,tuple.p2));
        }
        return ctlList.ToParallel();
    }
}

public partial class NormalEffectInfo
{
    public override void Play(MonsterController mc, NormalActionInfo actInfo, VideoSkillAction vsAct, VideoTargetStateGroup stateGroup)
    {
        if (mc == null)
        {
            //todo fish: collect error
            return;
        }
        var skillName = BattleHelper.GetSkillEffectName(vsAct.skill, name);
        if (string.IsNullOrEmpty(skillName))
        {
            //TODO collect error
            return;
        }
        var tSkillNames = skillName.Split(',').ToList();
        if (tSkillNames.IsNullOrEmpty())
        {
            //TODO collect error
            return;
        }

        const int clientSkillScale = 10000;
        if (actInfo.initiator == ActionInitiator.Attacker)
        {
            if (fly && flyTarget == 0)
            {
                var victimIDs = vsAct.GetVictims();
                foreach (var victim in victimIDs)
                {
                    var injuredMc = BattleDataManager.MonsterManager.Instance.GetMonsterFromSoldierID(victim);
                    PlaySpecialEffect(skillName, mc, injuredMc, clientSkillScale,tSkillNames,actInfo);
                }
                return;
            }
        }
        
        var skillTarget = BattleDataManager.MonsterManager.Instance.GetMonsterFromSoldierID(vsAct.GetVictim(0));
        PlaySpecialEffect(skillName, mc, skillTarget, clientSkillScale,tSkillNames,actInfo);
    }

    private void PlaySpecialEffect(string skillName, MonsterController initiator,
        MonsterController skillTarget, int clientSkillScale, List<string> tSkillNames, NormalActionInfo actInfo)
    {
        if (initiator ==null || skillTarget == null) return;
        var root = new GameObject(skillName);
        tSkillNames.SpawnAllEffectsAsync(initiator,root,this,
            ()=>
            {
                OnEffectLoadFinish(root,skillName,initiator,actInfo,clientSkillScale,skillTarget);
            },OnEffectLoadError);
    }

    private void OnEffectLoadError(string errMsg)
    {
        GameLog.Log_BattleError(errMsg);
        //TODO collect errors
    }

    private void OnEffectLoadFinish(GameObject root, string skillName, MonsterController initiator,
        NormalActionInfo actInfo, int clientSkillScale, MonsterController skillTarget)
    {
        if (root.transform.childCount <= 0)
        {
            GameLog.Log_BattleError(string.Format("[Error]特效({0})播放失败，没有该资源" ,skillName));
            NGUITools.Destroy(root);
            return;
        }
        var effectStartPosition = this.GetEffStartPos(root, initiator, new Vector3(offX, offY, offZ));
        //特效时间
        var effectTime = initiator.CreateEffectTime(
            root
            , initiator.GetMountShadow().gameObject
            , (effectGameObject) =>
            {
                LayerManager.Instance.ResetBattleCameraParent();
            });

        var trans = root.transform;
        trans.position = effectStartPosition;
        trans.localRotation = Quaternion.identity;
        
        //todo fish: 链式攻击应该删除这个地方修改为新的方式
        if (actInfo.initiator == ActionInitiator.Attacker)
        {
            BattleSpecialFlowManager.Instance.ChainEffectTrans = root;
        }
        
        if (delayTime > 0)
        {
            effectTime.time = delayTime;
        }
        if (loop)
        {
            effectTime.loopCount = loopCount;
        }

        var scaler = effectTime.GetMissingComponent<ParticleScaler>();
        scaler.SetScale(scale / 100f *clientSkillScale / 10000f);

        /*
        if (faceToPrevious)
        {
            var tMonsterController = GetPreviousMonsterController(mTargetIndex, true);
            if (null != tMonsterController && null != tMonsterController.transform && mTargetIndex >= 0)
            {
                //朝向下一个，且要避免受击者在一条线时朝向老一样，2017-05-25 15:04:05
                root.transform.LookAt(root.transform.position + (initiator.transform.position - tMonsterController.transform.position));
                Vector3 tEulerAngles = root.transform.eulerAngles;
                float tRandomAngle = UnityEngine.Random.Range(10f, 30f);
                tRandomAngle = mTargetIndex % 2 == 0 ? tRandomAngle : -tRandomAngle;
                tEulerAngles = new Vector3(tEulerAngles.x, tEulerAngles.y + tRandomAngle, tEulerAngles.z);
                root.transform.localRotation = Quaternion.Euler(tEulerAngles);
            }
        }
        */
        
        if (skillTarget != null) //朝向目标
        {
            root.transform.LookAt(skillTarget.transform.position);
        }

        //跟随
        if (follow)
        {
            trans.parent = initiator.gameObject.transform;
        }//飞行
        else if (fly)
        {
            var targetPoint = this.GetEffStartPos(root,skillTarget,new Vector3(flyOffX, flyOffY, flyOffZ));
            effectTime.time = delayTime + flyTime;
            var delay = delayTime;
            if (delay < float.Epsilon)
            {
                delay = 1f;
            }
            effectTime.transform.LookAt(targetPoint);
            effectTime.transform.DOMove(targetPoint, delay);
        }
        
        if (IsEffectHasCamera && BattleDataManager.NeedBattleMap)
        {
            var tCameraParent = effectTime.transform.GetChildTransform("WarCameraRotate");
            if (tCameraParent == null) {
                UnityEngine.Assertions.Assert.IsNotNull(tCameraParent, "[Error]播放镜头特写失败，特效上没有指定名字的摄像机挂点(WarCameraRotate)");    
            }
            else
                LayerManager.Instance.UpdateBattleCameraParent(tCameraParent);
        }
    }

/*
                    "name": "skill_eff_1329_att",
                    "mount": "Mount_Shadow",
                    "faceToTarget": true,
                    
                    "name": "skill_eff_1329_hit",
                    "mount": "Mount_Hit",
                    "hitEff": true,
                    "playTime": 0.8
 */
    public override IBattlePlayCtl Interprete(MoveActionInfo actInfo, SkillConfigInfo skillCfg, Skill skill,
        VideoSkillAction vsAct)
    {
        return null;
    }
	
    public override IBattlePlayCtl Interprete(Skill skill,VideoSkillAction vsAct)
    {
        //var initiator = vsAct.actionSoldierId;
        //vsAct.targetStateGroups
        return null;
    }
}

public partial class HideEffectInfo
{
    public override IBattlePlayCtl Interprete(Skill skill,VideoSkillAction vsAct)
    {
        return null;
    }
    
    public override void Play(MonsterController mc, NormalActionInfo actInfo, VideoSkillAction vsAct, VideoTargetStateGroup stateGroup)
    {
        if (mc == null)
        {
            //todo fish: collect error
            return;
        }
        mc.PlayHideEffect(delayTime);
    }
}

public partial class SoundEffectInfo
{   
    public override void Play(MonsterController mc, NormalActionInfo actInfo, VideoSkillAction vsAct, VideoTargetStateGroup stateGroup)
    {
        if (mc == null)
        {
            //todo fish: collect error
            return;
        }
        
        AudioManager.Instance.PlaySound(name);
    }
}

public partial class ShakeEffectInfo
{
    public override void Play(MonsterController mc, NormalActionInfo actInfo, VideoSkillAction vsAct, VideoTargetStateGroup stateGroup)
    {
        if (mc == null)
        {
            //todo fish: collect error
            return;
        }

        LayerManager.Instance.BattleShakeEffectHelper.Launch(duaration, intensity);
    }
}
