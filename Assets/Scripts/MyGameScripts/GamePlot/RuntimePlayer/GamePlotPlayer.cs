using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace GamePlot
{
	public class GamePlotPlayer : MonoBehaviour
	{
		private GamePlotInfo _plotInfo;
		private Sequence _sequence;
		private List<PlotCharacterController> _characterList;
		private List<PlotSceneEffectController> _effectList;
		private List<PlotCameraController> _cameraList;
	    private GameObject _mGo;
		void Awake()
		{
		    _mGo = this.gameObject;
            DontDestroyOnLoad(_mGo);
        }

		public void Setup(GamePlotInfo plotInfo){
			_plotInfo = plotInfo;
			_characterList = new List<PlotCharacterController>(plotInfo.characterList.Count);
			_effectList = new List<PlotSceneEffectController>(plotInfo.sceneEffectList.Count);
			_cameraList = new List<PlotCameraController>(plotInfo.cameraList.Count);

			_sequence = DOTween.Sequence();
			_sequence.AppendInterval(plotInfo.plotTime).OnComplete(GamePlotManager.Instance.FinishPlot);

			//角色
		    Action<CharacterEntity> initCharacter = character =>
		    {
                _sequence.InsertCallback(character.startTime, () => {
                    GenerateCharacter(character);
                });
            };
			for(int i=0;i<_plotInfo.characterList.Count;++i){
				CharacterEntity character = _plotInfo.characterList[i];
				if(character.active)
				{
				    initCharacter(character);
				}
			}

            //特效
            Action<SceneEffectEntity> initSceneEffect = effect =>
            {
                _sequence.InsertCallback(effect.startTime, () => {
                    GenerateSceneEffect(effect);
                });
            };
            for (int i=0;i<_plotInfo.sceneEffectList.Count;++i){
				SceneEffectEntity effect = _plotInfo.sceneEffectList[i];
				if(effect.active)
				{
				    initSceneEffect(effect);
				}
			}

            //镜头
            Action<CameraEntity> initCamera = camera =>
            {
                _sequence.InsertCallback(camera.startTime, () => {
                    GenerateCamera(camera);
                });
            };
            for (int i=0;i<_plotInfo.cameraList.Count;++i){
				CameraEntity camera = _plotInfo.cameraList[i];
				if(camera.active)
				{
				    initCamera(camera);
				}
			}

            //音频指令
            Action<PlayAudioAction> initAudio = audioAction =>
            {
                _sequence.InsertCallback(audioAction.startTime, () => {
                    PlayAudio(audioAction);
                });
            };
            for (int i=0;i<_plotInfo.audioActionList.Count;++i){
				PlayAudioAction audioAction = _plotInfo.audioActionList[i];
				if(audioAction.active)
				{
				    initAudio(audioAction);
				}
			}

            //屏幕蒙版
            Action<ScreenMaskAction> initScreenMask = maskAction =>
            {
                _sequence.InsertCallback(maskAction.startTime, () => {
                    ShowScreenMask(maskAction);
                });
            };
            for (int i=0;i<_plotInfo.screenMaskActionList.Count;++i){
				ScreenMaskAction maskAction = _plotInfo.screenMaskActionList[i];
				if(maskAction.active){
                    initScreenMask(maskAction);
				}
			}

            //压屏效果
            Action<ScreenPresureAction> initScreenPresure = presureAction =>
            {
                _sequence.InsertCallback(presureAction.startTime, () => {
                    ShowScreenPresure(presureAction);
                });
            };
            for (int i=0;i<_plotInfo.screenPresureActionList.Count;++i){
				ScreenPresureAction presureAction = _plotInfo.screenPresureActionList[i];
				if(presureAction.active)
				{
				    initScreenPresure(presureAction);
				}
			}
		}

		private void GenerateCharacter(CharacterEntity character){
			if(GamePlotManager.PrintLog)
				Debug.LogError(string.Format("CharacterEntity_{0}",character.startTime));

			GameObject entityGo = new GameObject(string.Format("CharacterEntity_{0}",character.name));
			GameObjectExt.AddPoolChild(_mGo,entityGo);

			CharacterController characterController = entityGo.GetMissingComponent<CharacterController> ();
			characterController.center = new Vector3 (0f, 0.75f, 0f);
			characterController.radius = 0.4f;
			characterController.height = 2f;

			//初始化位置和朝向
			entityGo.transform.position = SceneHelper.GetPositionInScene(character.originPos);
			entityGo.transform.rotation = Quaternion.Euler(0f,character.rotateY,0f);

			PlotCharacterController com = entityGo.GetMissingComponent<PlotCharacterController>();
			com.Setup(character, IsNewBiePlot());
			_characterList.Add(com);
		}

		private bool IsNewBiePlot()
		{
			return _plotInfo.plotId == 1 || _plotInfo.plotId == 2;
		}

		private void GenerateSceneEffect(SceneEffectEntity effect){
			if(GamePlotManager.PrintLog)
				Debug.LogError(string.Format("SceneEffectEntity_{0}",effect.startTime));

			GameObject entityGo = new GameObject(string.Format("SceneEffectEntity_{0}",effect.effPath));
			GameObjectExt.AddPoolChild(_mGo,entityGo);

			PlotSceneEffectController com = entityGo.GetMissingComponent<PlotSceneEffectController>();
			com.Setup(effect);
			_effectList.Add(com);
		}

		private void GenerateCamera(CameraEntity camera){
			GameObject entityGo = new GameObject("CameraEntity");
			GameObjectExt.AddPoolChild(_mGo,entityGo);

			PlotCameraController com = entityGo.GetMissingComponent<PlotCameraController>();
			com.Setup(camera);
			_cameraList.Add(com);
		}

		private void PlayAudio(PlayAudioAction audioAction){
			if(GamePlotManager.PrintLog)
				Debug.LogError(string.Format("PlayAudioAction_{0}",audioAction.startTime));

			//这里做一下兼容处理，以前的音频路径是带目录的
			string audioName = System.IO.Path.GetFileName (audioAction.audioPath);
			if(audioAction.audioType == PlayAudioAction.AudioType.Sound){
				AudioManager.Instance.PlaySound(audioName);
			}else{
				AudioManager.Instance.PlayMusic(audioName);
				JSTimer.Instance.SetupCoolDown("PlotBgMusic",audioAction.duration,null,AudioManager.Instance.StopMusic);
			}
		}

		private void ShowScreenMask(ScreenMaskAction maskAction){
			if(GamePlotManager.PrintLog)
				Debug.LogError(string.Format("ScreenMaskAction_{0}",maskAction.startTime));

			ScreenMaskManager.OpenMaskView(maskAction);
		}

		private void ShowScreenPresure(ScreenPresureAction presureAction){
			if(GamePlotManager.PrintLog)
				Debug.LogError(string.Format("ScreenPresure_{0}",presureAction.startTime));

			GamePlotManager.OpenPresureView(presureAction,OnPresureSkipPlotCallback);
		}

		private void OnPresureSkipPlotCallback()
		{
			if (_plotInfo.plotId == 1)
			{
				TalkingDataHelper.OnEventSetp ("StartPlot1", "Skip");
			}
			else if (_plotInfo.plotId == 2)
			{
				TalkingDataHelper.OnEventSetp ("StartPlot2", "Skip");
			}
			GamePlotManager.Instance.FinishPlot();
		}

		//剧情播放完毕，清空操作
		public void Finish(){
			_sequence.Kill();
			_sequence = null;

			JSTimer.Instance.CancelCd("PlotBgMusic");

			//剧情实体清空操作
			for(int i=0;i<_characterList.Count;++i){
				if(_characterList[i] != null)
					_characterList[i].Dispose();
			}

			for(int i=0;i<_effectList.Count;++i){
				if(_effectList[i] != null)
					_effectList[i].Dispose();
			}

			for(int i=0;i<_cameraList.Count;++i){
				if(_cameraList[i] != null)
					_cameraList[i].Dispose();
			}

			Destroy(_mGo);
		    _mGo = null;
		}
	}
}