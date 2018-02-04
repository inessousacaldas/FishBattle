// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  TriggerNpcUnit.cs
// Author   : willson
// Created  : 2014/12/23 
// Porpuse  : 
// **********************************************************************

using AppDto;
using AssetPipeline;
using UnityEngine;

public class TriggerNpcUnit : BaseNpcUnit
{
    public static bool SOUND_LOCKED;

    protected HeroView _heroView;

    private bool _isRunning;

    protected UnityEngine.AI.NavMeshAgent _mAgent;
    public bool enabled;
    public bool touch;
    public bool waitingTrigger;
    public bool walk;

    public TriggerNpcUnit()
    {
        Reset();
    }

    public virtual void Reset()
    {
        enabled = true;
        waitingTrigger = false;
        touch = false;
    }

    public void SetupHeroView(HeroView heroView)
    {
        _heroView = heroView;
    }

    public virtual bool NeedClose()
    {
        return false;
    }

    protected override void AfterInit()
    {
        base.AfterInit();
        InitNpcAnimation();
    }

    protected void InitPlayerName()
    {
        var npc = _npcInfo.npcStateDto.npc;

        var mountShadow = _modelDisplayer.GetMountingPoint(ModelHelper.Mount_shadow);
        if (mountShadow != null)
        {
            if (titleHud == null)
                titleHud = new CharacterTitleHud(mountShadow, new Vector3(0f, -0.7f, 0f), "NpcTitleHUD_" + npc.id);
            else
                titleHud.ResetHudFollower(mountShadow, new Vector3(0f, -0.7f, 0f), "NpcTitleHUD_" + npc.id);
        }

        var mountHUD = _modelDisplayer.GetMountingPoint(ModelHelper.Mount_hud);
        if (mountHUD != null)
        {
            if(headHud == null)
                headHud = new CharacterHeadHud(mountHUD, Vector3.zero, "NpcHeadHUD_" + npc.id);
            else
                headHud.ResetHudFollower(mountHUD, Vector3.zero, "NpcHeadHUD_" + npc.id);

            headHud.headHUDView.runFlagSpriteAnimation.SetEnable(false);
            headHud.headHUDView.teamFlagSpriteAnimation.SetEnable(false);
            headHud.headHUDView.teamInfo_UISprite.enabled = false;
            headHud.headHUDView.escortFlag_UISprite.enabled = false;

            SetNPCMissionFlag(true);
            SetNPCFightFlag(_npcInfo.npcStateDto.battleId > 0);
        }
        UpdatePlayerName();
        //模型加载完毕,还原模型显示状态
        SetModelActive(_isModelActive);
    }

    private void UpdatePlayerName()
    {
        if (titleHud != null)
        {
            var npc = _npcInfo.npcStateDto.npc;

            string appellationStr = "";
            GameDebuger.TODO(@"if (npc is NpcGeneral)
            {
                if (!string.IsNullOrEmpty((npc as NpcGeneral).title))
                {
                    appellationStr = (npc as NpcGeneral).title.WrapColor(ColorConstant.Color_Title_Str) + '\n';
                }
            }");
            GameDebuger.TODO(@"else if (npc is NpcVariable)
            {
                if (!string.IsNullOrEmpty((npc as NpcVariable).title))
                {
                    appellationStr = (npc as NpcVariable).title.WrapColor(ColorConstant.Color_Title_Str) + '\n';
                }
            }");

            string npcName = _npcInfo.name;
            GameDebuger.TODO(@"if (_npcInfo.npcStateDto.npc is NpcSceneWorldBossMonster)
            {
                if (_npcInfo.npcStateDto.times == 0)
                {
                    npcName = '{0}{1}（{2}星）';
                }
                else
                {
                    npcName = '{0}{1}分身（{2}星）';
                }

                string prefix = '';
                var rankConfig = DataCache.getArrayByCls<WorldBossRankConfig>();
            if (rankConfig != null)
            {
            for (int index = 0; index < rankConfig.Count; index++)
            {
            if (rankConfig[index].minRank <= _npcInfo.npcStateDto.rank &&
            _npcInfo.npcStateDto.rank <= rankConfig[index].maxRank)
            {
            prefix = rankConfig[index].prefix;
            break;
            }
            }                   
            }

                npcName = string.Format(npcName, prefix, _npcInfo.npcStateDto.npc.name, _npcInfo.npcStateDto.rank);
            }");

            titleHud.titleHUDView.nameLbl.text = appellationStr + npcName.WrapColor(ColorConstant.Color_Battle_Enemy_Name);
        }
    }

    //	NPC进入战斗
    /// <summary>
    ///     NPC进入战斗 -- Sets the NPC fight flag.
    /// </summary>
    /// <param name="active">If set to <c>true</c> active.</param>
    public void SetNPCFightFlag(bool active)
    {
        if (headHud != null)
        {
           headHud.headHUDView.fightFlagSpriteAnimation.SetEnable(active);
        }
    }

    public override void Trigger()
    {
        var effpath = PathHelper.GetEffectPath(GameEffectConst.GameEffectConstEnum.Effect_CharactorClick);
        if(_unitTrans == null)
            OneShotSceneEffect.Begin(effpath, GetPos(), 2f, 1f);
        else
            OneShotSceneEffect.BeginFollowEffect(effpath, _unitTrans, 2f, 1f);
        if (enabled == false)
        {
            return;
        }
        if (touch)
        {
            DoTrigger();
            _heroView.StopAndIdle();
            return;
        }  
        //计算NPC和角色之间的距离
        float distance = Vector3.Distance(_heroView.cachedTransform.position, GetPos());
        if(distance < CheckDistance())
        {
            DoTrigger();
        }
        else
        {
            _heroView.WalkToPoint(GetPos(),null,true);
            //GameDebuger.Log(">>>>>>>>>>>>>>>>>>>>>>>>>> " + _npcInfo.name + " " + _unitGo.gameObject.name);
            waitingTrigger = true;
        }
    }

    protected virtual float CheckDistance()
    {
        return 2f;
    }

    public virtual void DoTrigger()
    {
        waitingTrigger = false;
        touch = false;

        PlayNpcSound();

        //npc朝向玩家
        FaceToHero();

        ModelManager.Player.StopAutoNav();

//		MissionGuidePathFinder.Instance.CheckNpcFinded(_npc);
    }

    private void PlayNpcSound()
    {
        if (!AudioManager.Instance.ToggleDubbing)
        {
            return;
        }

        GameDebuger.TODO(@"var npcGeneral = GetNpc() as NpcGeneral;
        if (npcGeneral == null) return;

        int soundId = npcGeneral.soundId;
        if (soundId == 0)
        {
            //soundId为0则表示此NPC无需配音
            return;
        }
        string objName = 'AudioSound:sound_npc_' + soundId;
        if (_unitTrans.FindChild(objName) == null && SOUND_LOCKED == false)
        {
            string soundName = 'sound_npc_' + soundId;
            ResourcePoolManager.Instance.LoadAudioClip(soundName, asset =>
            {
                if (asset != null)
                {
                    var audioClip = asset as AudioClip;
                    var go = new GameObject(objName);
                    go.transform.parent = _unitTrans;

                    // create the source
                    var source = go.AddComponent<AudioSource>();
                    source.clip = audioClip;
                    source.volume = AudioManager.Instance.DubbingVolume;
                    source.loop = false;
                    source.Play();
                    Object.Destroy(go, audioClip.length);
                    SOUND_LOCKED = true;

                    JSTimer.Instance.SetupCoolDown('soundUnlockTimer', audioClip.length, null,
                        delegate { SOUND_LOCKED = false; });
                }
                else
                {
                    GameDebuger.Log('Can not find the sound of ' + soundName);
                }
            });
        }");
    }

    public void FaceToHero()
    {
        var position = _heroView.cachedTransform.position;
        if (_unitTrans == null)
        {
            GameDebuger.LogError("当前npc为空，请不要清除打印");
            return;
        }
        _unitTrans.LookAt(position); //, targetOrientation);
        _unitTrans.eulerAngles = new Vector3(0, _unitTrans.eulerAngles.y, 0);
    }

//    public override void UpdateNpcState(SceneNpcStateDto npcState)
//    {
//        base.UpdateNpcState(npcState);
//        UpdatePlayerName();
//    }

    public void WalkToPoint(Vector3 targetPoint)
    {
        GameDebuger.TODO(@"if (_unitGo == null || touch || waitingTrigger || ProxyManager.Dialogue.GetDialogueNpcId() == GetNpcUID())
        {
            return;
        }");

        //targetPoint = new Vector3(-12f, targetPoint.y, 1.6f);

        if (_unitGo.activeInHierarchy)
        {
            if (_mAgent == null)
            {
                _mAgent = _unitGo.GetMissingComponent<UnityEngine.AI.NavMeshAgent>();
                _mAgent.radius = 0.4f;
                _mAgent.speed = ModelHelper.DefaultModelSpeed;
                _mAgent.acceleration = 1000;
                _mAgent.angularSpeed = 1000;
                _mAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
                _mAgent.autoTraverseOffMeshLink = false;
                _mAgent.autoRepath = false;
            }
            _mAgent.enabled = true;
            _mAgent.SetDestination(targetPoint);
            walk = true;
            _master.npcViewDataManager.AddToAlwaysShow(this);
        }
        else
        {
            SetPos(targetPoint);
        }
    }

    public void UpdateWalk()
    {
        if (_mAgent != null)
        {
            if (_mAgent.enabled)
            {
                if (_mAgent.hasPath)
                {
                    PlayRunAnimation();
                }
                else
                {
                    PlayIdleAnimation();
                }
            }
            else
            {
                PlayIdleAnimation();
            }
        }
    }

    private void InitNpcAnimation()
    {
        _isRunning = false;
        UpdateWalk();
    }

    private void PlayRunAnimation()
    {
        if (!_isRunning)
        {
            DoAction(ModelHelper.AnimType.run);
            _isRunning = true;
        }
    }

    private void PlayIdleAnimation()
    {
        if (_isRunning)
        {
            DoAction(ModelHelper.AnimType.idle);
            _isRunning = false;
        }
    }

    public void StopAndIdle()
    {
        _isRunning = false;
        walk = false;
//		DoAction(ModelHelper.AnimType.idle);
//		if (_mAgent != null)
//		{
//			if (_unitGo.activeInHierarchy)
//			{
//				_mAgent.ResetPath();
//			}
//		}
    }

    public override void SetUnitActive(bool active)
    {
        PlayIdleAnimation();
        if (_mAgent != null)
        {
            if (_unitGo.activeInHierarchy)
            {
                if (_mAgent != null && _mAgent.isActiveAndEnabled && _mAgent.hasPath)
                {
                    _mAgent.ResetPath();
                    _mAgent.enabled = false;
                }
            }
        }

        StopAndIdle();

        base.SetUnitActive(active);
    }

    public override void Destroy()
    {
        base.Destroy();
        StopAndIdle();
        SOUND_LOCKED = false;
        JSTimer.Instance.CancelCd("soundUnlockTimer");
        if (_mAgent != null)
        {
            GameObjectExt.DestroyLog(_mAgent);
            _mAgent = null;
        }
    }
   
}