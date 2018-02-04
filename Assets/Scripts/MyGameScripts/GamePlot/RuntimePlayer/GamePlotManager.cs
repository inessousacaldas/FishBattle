using UnityEngine;
using System.Collections.Generic;
using AppDto;
using AppServices;
using AssetPipeline;

namespace GamePlot
{
	public class GamePlotManager
	{
		public static bool PrintLog = false;
		public const string PLOT_PATH = "ConfigFiles/GamePlotConfig";
		private static readonly GamePlotManager _instance = new GamePlotManager ();

		public static GamePlotManager Instance {
			get {
				return _instance;
			}
		}

		private GamePlotManager ()
		{
		}

		#region Plot UIView

		public static void OpenPresureView (ScreenPresureAction presureAction, System.Action endCallback)
		{
			var com = UIModuleManager.Instance.OpenFunModule<ScreenPresureViewController> (ScreenPresure.NAME, UILayerType.Dialogue, false);
			com.Open (presureAction, endCallback);
		}

		public static void ClosePresureView ()
		{
			UIModuleManager.Instance.CloseModule (ScreenPresure.NAME);
		}

		#endregion

		private GamePlotPlayer _curPlotPlayer;
		private Dictionary<string,Plot> _triggerPlotDic;
		private Plot _curTriggerPlot;
        private SceneCameraController _mSceneCameraController;
        //当前触发的剧情信息

        public event System.Action<Plot> OnFinishPlot;

		public void Setup ()
		{
			InitTriggerPlotDic ();
		}

		private void InitTriggerPlotDic ()
		{
            if(_triggerPlotDic == null)
            {
                _triggerPlotDic = new Dictionary<string,Plot>();
                List<Plot> plotList = DataCache.getArrayByCls<Plot>();
                if(plotList != null)
                {
                    for(int i = 0;i < plotList.Count;++i)
                    {
                        Plot plot = plotList[i];
                        string key = plot.triggerType + "_" + plot.triggerParam;
                        if(!_triggerPlotDic.ContainsKey(key))
                            _triggerPlotDic.Add(key,plot);
                    }
                }
            }
		}

		#region Getter

		public bool IsPlaying ()
		{
            //GameDebuger.TODO(@"return _curPlotPlayer != null;");
            //return false;
            return _curPlotPlayer != null;
        }

		public bool ContainsPlot (int type, int param)
		{
            if(_triggerPlotDic == null)
                return false;

            return _triggerPlotDic.ContainsKey(type + "_" + param);
        }

//		private bool IsBattlePlotEndEvent (Plot plot)
//		{
//			if (plot.id == 1) {
//				return true;
//			}
//			
//			if (plot != null && plot.plotEndEvent != null && plot.plotEndEvent.id == 1) {
//				return true;
//			} else {
//				return false;
//			}
//		}

//		private bool IsChangeScenePlotEndEvent (Plot plot)
//		{
//			if (plot != null && plot.plotEndEvent != null && plot.plotEndEvent.id == 4) {
//				return true;
//			} else {
//				return false;
//			}
//		}

		#endregion

		//其它业务逻辑触发剧情接口，例如任务，战斗胜利
		public bool TriggerPlot (int triggerType, int param)
		{
			InitTriggerPlotDic ();
            Plot plot = null;
            if(_triggerPlotDic.TryGetValue(triggerType+"_"+param,out plot))
            {
                PlayPlot(plot);
                return true;
            }
            return false;
		}

        //正常播放剧情，剧情结束后会调用RequestServer接口
        public void PlayPlot(Plot plot)
        {
            _curTriggerPlot = plot;

            //不可播放的剧情直接跳过
            if(_curTriggerPlot.show)
                PlayPlot(plot.id);
            else
                FinishPlot();
        }

        //GM指令播放剧情
        public void PlayPlot (int plotId)
		{
			GameDebuger.Log ("PlayPlot " + plotId);

			if (IsPlaying ()) {
                TipManager.AddTip ("剧情播放中，无法重复播放");
				return;
			}

			string plotResKey = string.Format ("GamePlot_{0}", plotId);
            ResourcePoolManager.Instance.LoadConfig(plotResKey, OnLoadPlotDataFinish);
		}

		private void OnLoadPlotDataFinish (Object asset)
		{
			if (asset != null) {
				TextAsset textAsset = asset as TextAsset;
				if (textAsset != null) {
					GamePlotInfo loadedPlot = JsHelper.ToObject<GamePlotInfo> (textAsset.text);
					if (loadedPlot != null) {

						if (loadedPlot.plotId == 1) {
                            //TalkingDataHelper.OnEventSetp ("StartPlot1", "Play");
						} else if (loadedPlot.plotId == 2) {
							//TalkingDataHelper.OnEventSetp ("StartPlot2", "Play");
						}
                        _mSceneCameraController = LayerManager.Root.SceneCamera.gameObject.GetComponent<SceneCameraController>();
                        _mSceneCameraController.enabled = false;
                        LayerManager.Instance.SwitchLayerMode (UIMode.STORY);
                        //关闭其他界面
                        UIModuleManager.Instance.CloseOtherModuleWhenNpcDialogue();
                        //为了测试主窗口，只关闭主窗口界面
                        GameObject plotPlayerGo = new GameObject ("GamePlotPlayer");
						_curPlotPlayer = plotPlayerGo.GetMissingComponent<GamePlotPlayer> ();
                        _curPlotPlayer.Setup (loadedPlot);
                        ScreenMaskManager.FadeIn(null,0.4f,0.2f);
                        //
                    }
				}
			}
		}

		//跳过剧情（播放完剧情后的回调）
		public void FinishPlot ()
		{
            BattleDataManager.DataMgr.NeedPlayPlot = false;
            _mSceneCameraController.enabled = true;
            RequestServer ();
			ClosePresureView ();
            if(OnFinishPlot != null)
            {
                OnFinishPlot(_curTriggerPlot);
                OnFinishPlot = null;
            }
            if (_curPlotPlayer != null) {
				_curPlotPlayer.Finish ();
				_curPlotPlayer = null;
			}
		}

		private void RequestServer ()
		{
            ScreenMaskManager.FadeIn(null,0.4f,0.2f);
            ProxyMainUI.Open();
            LayerManager.Instance.SwitchLayerMode(UIMode.GAME);
            WorldManager.Instance.PlayWorldMusic();
            //GM命令触发剧情的时候不发送给服务器
            if(_curTriggerPlot != null)
            {
                GameUtil.GeneralReq(Services.Plot_End(_curTriggerPlot.id),null);
                //清空当前触发的剧情
                _curTriggerPlot = null;
            }
            //ServiceRequestAction.requestServer(PlotService.end(plotId),"""",
            //               (e) =>
            //               {
            //                   if(e is PlayerMissionDto)
            //                   {
            //                       ModelManager.MissionData.missionStoryPlotDelegate.StoryEndPlotCallback(e as PlayerMissionDto);
            //                   }
            //               });
            GameDebuger.TODO(@"if (_curTriggerPlot == null) {
            //GM指令播放剧情，直接恢复场景
            LayerManager.Instance.SwitchLayerMode (UIMode.GAME);
            WorldManager.Instance.PlayWorldMusic ();
            return;
        }

            if (!IsChangeScenePlotEndEvent (_curTriggerPlot) && !IsBattlePlotEndEvent (_curTriggerPlot)) {
                WorldManager.Instance.ResumeScene ();
            } else {
                LayerManager.Instance.SwitchLayerMode (UIMode.GAME);
            }

            int plotId = _curTriggerPlot.id;

            if (plotId == 1) {
                TalkingDataHelper.OnEventSetp ('StartPlot1', 'Finish');
                         BattleDataManager.Instance.PlayGuideBattle ();
            } else if (plotId == 2) {
                TalkingDataHelper.OnEventSetp ('StartPlot2', 'Finish');
                PlayerPrefsExt.SetBool ('PassRoleCreatePlot', true);
                WorldMapLoader.Instance.Destroy ();
                AppGameManager.Instance.InitSPSdk ();
            } else {
                         
            ServiceRequestAction.requestServer (PlotService.end (plotId), """",
                             (e) => {
                                 if (e is PlayerMissionDto) {
                                     ModelManager.MissionData.missionStoryPlotDelegate.StoryEndPlotCallback (e as PlayerMissionDto);
                                 }
                             });                


                WorldManager.Instance.PlayWorldMusic ();
            }

            //清空当前触发的剧情
            _curTriggerPlot = null;");
		}

		#region 登陆时候的剧情播放

		public int LastPlotId = 0;

        public void SetLastPlotId(int plotId)
        {
            Plot lastPlot = DataCache.getDtoByCls<Plot>(plotId);
            if(lastPlot != null)
            {
                LastPlotId = plotId;
            }
            else
            {
                LastPlotId = 0;
            }
        }

        public bool HasLastPlot ()
		{
            //if (LastPlotId <= 2)
            //	return false;

            //if (LastPlotId > 0) {
            //	GameDebuger.TODO(@"Plot lastPlot = DataCache.getDtoByCls<Plot> (LastPlotId);
            //             if (lastPlot.id == 2) {
            //             if (NewBieGuideManager.Instance.IsFinishGuide (NewBieGuideManager.Key_GuideHeroBattle)) {
            //                                 PlayPlot (lastPlot);
            //                             } else {
            //                                 BattleDataManager.Instance.PlayGuideBattle ();
            //                             };      
            //             } else {
            //                 PlayPlot (lastPlot);
            //             }");
            //	LastPlotId = 0;
            //}

            //return true;
            return LastPlotId > 0;
        }

        #endregion

        public void PlayLastPlot()
        {
            if(LastPlotId > 0)
            {
                Plot lastPlot = DataCache.getDtoByCls<Plot>(LastPlotId);
                if(lastPlot != null)
                {
                    LayerManager.Instance.SwitchLayerMode(UIMode.STORY);
                    PlayPlot(lastPlot);
                }
                LastPlotId = 0;
            }
        }

        public void Destroy ()
		{
			OnFinishPlot = null;

			BattleDataManager.DataMgr.NeedPlayPlot = false;
			ClosePresureView ();

			if (_curPlotPlayer != null) {
				LayerManager.Instance.SwitchLayerMode (UIMode.GAME);
				_curPlotPlayer.Finish ();
				_curPlotPlayer = null;
			}
		}
	}
}