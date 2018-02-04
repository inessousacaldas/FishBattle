using System;
using AppDto;
using AssetPipeline;
using GamePlayer;
using UniRx;

namespace StaticInit
{
    using StaticDispose;
    public partial class StaticInit 
    {
        private StaticDelegateRunner initBattleManager = new StaticDelegateRunner(
            () =>
            {
                var mgr = BattleDataManager.BattleManager.Instance;
                BattleNetworkHandler.InitNotifyListener();
            });
    }
}

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeBattleManager = new StaticDelegateRunner(
            BattleDataManager.BattleManager.ExecuteDispose);
    }
}

public sealed partial class BattleDataManager
{
    public class BattleManager
    {
        private BattleStateTrasition _trasition;
        private static BattleManager _ins;
        private const string BattleFinish_Timer = "BeforeExitBattle";
        
        public static BattleManager Instance{
            get{
                if (_ins == null)
                {
                    _ins = new BattleManager();
                    _ins._trasition = BattleStateTrasition.Create();
                }

                return _ins;
            }
        }

        public void PlayBattle(Video gv, int watchTeamId = 0, bool needRefreshBattle = false/**, BarrageVideo barrageVideo= null*/)
        {
            if (gv == null)
            {
                GameDebuger.Log("Can't get Video");
                return;
            }

            //PlayerDto playerDto = ModelManager.Player.GetPlayer();
            //if (playerDto == null || CheckVideoIsThisPlayer(playerDto, gv) == false) //服务器回调时，本地已经切换人物
            //{
            //    GameDebuger.Log("PlayBattleFailure");
            //    return;
            //}
            GameDebuger.TODO(@"_barrageVideo = barrageVideo;");

            GameDebuger.TODO(@"if (TeamModel.ShowDebugInfo) {
            GameDebuger.Log (string.Format (""PlayBattle gvId:{0} type:{1} watchTeamId:{2}"", gv.id, gv.ToString(), watchTeamId).WrapColorWithLog ());
        }
");

            GameDebuger.Log(string.Format("PlayBattle ateam.Count:{0},bteam.Count:{1}", gv.ateam.teamSoldiers.Count, gv.bteam.teamSoldiers.Count));

            if (WorldManager.IsWaitingEnter)
            {
                GameDebuger.Log(string.Format("当前正在进入场景， 先暂存战斗{0}", gv.id));

                DataMgr._nextGameVideo = gv;
                DataMgr._data._watchTeamId = watchTeamId;
                return;
            }

            if (WorldManager.Instance.GetHeroView() != null)
            {
                WorldManager.Instance.GetHeroView().StopAndIdle();
            }

            ProxyWorldMapModule.CloseAllModule();

            ModelManager.Player.StopAutoNav();

            //进出战斗需要把场景里的头顶气泡都关掉
            GameDebuger.TODO(@"ProxyManager.ActorPopo.CloseAll();
        if (MainUIViewController.DataMgr != null)
        {
            MainUIViewController.DataMgr.HideBtnInBattle(false);
        }
        ProxyManager.Tournament.Hide();
        ProxyManager.Tournament.HideV2();
        ProxyManager.GuildCompetition.OnClickExplandHorBtn(false);
        ProxyManager.CampWar.OnClickExplandHorBtn(false);
        ProxyManager.Escort.OnClickExplandHorBtn(false);
        ProxyManager.SnowWorldBossExpand.Hide();
        ProxyManager.DaTang.IsOpenTips(true);

        //  进入战斗，关闭副本评级界面
        ProxyManager.InstanceZones.CloseRank();        
");
            if (_trasition == null)
                _trasition = BattleStateTrasition.Create();
            BattleNetworkHandler.Setup();

            if (!DataMgr.IsInBattle)
            {
                BattleInstController.Instance.Setup(gv);
                DataMgr.PlayBattle(gv, watchTeamId, needRefreshBattle);
            }
            else
            {
                if (!DataMgr._data.IsSameBattle(gv.id))
                {
                    GameDebuger.Log(string.Format("当前有其它战斗{0}在进行，先暂存战斗{1} waitingExitBattle={2}", DataMgr._data._gameVideo.id, gv.id, DataMgr._nextGameVideo == null));
                    DataMgr._nextGameVideo = gv;
                }
                else
                {
                    DataMgr.UpdateCurVideo(gv);
                }
            }
        }
        
        public static void ExecuteDispose()
        {
            if (_ins != null)
                _ins.Dispose();
            _ins = null;
        }
        
        public void Dispose()
        {
            _trasition.Dispose();
            BattleModuleDispose();
        }
        
        public bool CheckNextBattle()
        {
            if (DataMgr._nextGameVideo == null) return false;
            var tempWatchId = DataMgr._data._watchTeamId;
            var tempGameVideo = DataMgr._nextGameVideo;

            PlayBattle(DataMgr._nextGameVideo);

            DataMgr._data._watchTeamId = 0;
            DataMgr._nextGameVideo = null;
            return true;
        }
        
        private static void BeforeExitBattle()
    {
        UIModuleManager.Instance.CloseModule(BattleSkillSelectView.NAME);
        
        switch (DataMgr.IsWin)
        {
            case BattleResult.LOSE:
                ProxyMainUI.CloseBattleBuffTipsView();
                ProxyBattleDemoModule.CloseBattleOrderListView();
                ExitBattle();
                break;
            case BattleResult.WIN:
                CameraManager.Instance.BattleCameraController.StopCameraEvt();
                var tCameraPath = LayerManager.Root.BattleDefaultRotationCntr_Transform.GetComponent<CameraPath>();
                var tCameraPathAnimator = LayerManager.Root.BattleDefaultRotationCntr_Transform.GetComponent<CameraPathAnimator>();
                tCameraPath.enabled = true;
                tCameraPathAnimator.enabled = true;
                tCameraPathAnimator.Stop();
                tCameraPathAnimator.Play();

                ProxyMainUI.CloseBattleBuffTipsView();
                ProxyBattleDemoModule.CloseBattleOrderListView();
                LayerManager.Root.BattleUIHUDPanel.cachedGameObject.SetActive(false);
                ProxyMainUI.Hide();
                //CameraManager.Instance.BattleCameraController.SetPosition();
                //CameraManager.Instance.BattleCameraController.SetRotation();
                //LayerManager.Root.BattlePositionCntr_Transform.transform.position = new UnityEngine.Vector3(1.75f,0,-1);
                // 启动定时器或者弹窗
                JSTimer.Instance.SetupCoolDown(
                    BattleFinish_Timer
                    , 1f  // 镜头移动，为避免出错这里不使用finishcallback
                    , null
                    , delegate {
                        // 播放胜利动作动画
                        MonsterManager.Instance.GetMonsterList(includeDead:false)
                            .ForEach(m=>m.PlayAnimation(ModelHelper.AnimType.victory));
                        JSTimer.Instance.SetupCoolDown(
                            "asdf"
                            , 2f  // 播战斗动画的时间 为避免出错这里不使用finishcallback
                            , null
                            , delegate {
                                ExitBattle();
                                //tCameraPathAnimator.Reverse();
                                //tCameraPathAnimator.IsInt
                                //LayerManager.Root.BattlePositionCntr_Transform.transform.position = new UnityEngine.Vector3(1.75f,0,-1);
                                tCameraPathAnimator.Stop();
                                tCameraPath.enabled = false;
                                tCameraPathAnimator.enabled = false;
                            });
                    
                    });
                break;
            }
        }

        private static void ExitBattle()
        {
            BattleModuleDispose();
            LayerManager.Instance.SwitchLayerMode(UIMode.GAME);
        
            ProxyMainUI.Show();
            ResourcePoolManager.UnloadAssetsAndGC();
        }
        
        private static void BattleModuleDispose()
        {
            if (!DataMgr.IsInBattle) return;
            GameDebuger.TODO(@"if (_guideBattle)
                {
                    TalkingDataHelper.OnEventSetp('StartBattle', 'End');
                }");
                
            MonsterManager.Instance.ResetData();
        
            BattleActionPlayerPoolManager.Instance.Dispose();

            DataMgr.LateExitBattle();
            BattleDataManager.ExecuteDispose();
            
            BattleStatusEffectManager.Instance.Dispose();
                
            GameDebuger.TODO(@"if (IsNeedSynSettingVideo())
                {
                    ModelManager.SystemData.SaveAutoFramToggle(DataMgr._data.isAIManagement);
                }");
        
            BattleNetworkHandler.StopNotifyListener();
        
            BattleInstController.DisposeOnExit();
        }
        
        
        public class BattleStateTrasition
{
    private IDisposable _disposable;
    
   
    public static BattleStateTrasition Create(){
        var mgr = new BattleStateTrasition();
        mgr._disposable = Stream.Select(
            (data, state) => 
            Tuple.Create(data.battleState, data.lastBattleState))
            .Subscribe(stateTuple=>mgr.UpdateByBattleState(stateTuple));
        return mgr;
    }

    private void UpdateByBattleState(Tuple<BattleSceneStat, BattleSceneStat> stateTuple)
    {
        if (stateTuple.p1 == stateTuple.p2) return; 
        switch (stateTuple.p1)
        {
            case BattleSceneStat.Invalid:
                break;
            case BattleSceneStat.Battle_OVER:
                _ins._trasition.Dispose();
                _ins._trasition = null;
                BeforeExitBattle();
                break;
            case BattleSceneStat.BATTLE_PRESTART:
                MonsterManager.Instance.UpdateMonsterNeedReadyLogic(false);
                break;
            case BattleSceneStat.ON_WAITING_ActTime_Update:
                break;
            case BattleSceneStat.BATTLE_PlayerOpt_Time:
                break;
            case BattleSceneStat.BATTLE_VIDEOROUND_PLAYING:
                break;
            default:break;
        }
    }

    

    public void Dispose()
    {
        _disposable.Dispose();
        _disposable = null;
    }
}
    }
}

