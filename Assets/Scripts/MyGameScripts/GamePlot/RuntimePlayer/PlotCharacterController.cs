using System;
using UnityEngine;
using System.Collections.Generic;
using AppDto;
using AssetPipeline;
using DG.Tweening;

namespace GamePlot
{
	public class PlotCharacterController : MonoBehaviour
	{
		private CharacterEntity _characterInfo;
		private int _mId;
		private GameObject _mGo;
		private Transform _mTrans;
		private ModelTitleHUDView _titleHUDView;
		private bool _isRunning = false;
		private Sequence _sequence;
		private List<OneShotSceneEffect> _effectList;
		private ModelDisplayer _modelDisplayer;
        private GridMapAgent _gridMapAgent;

        public void Setup (CharacterEntity info, bool isNewBiePlot)
		{
			_characterInfo = info;
			_effectList = new List<OneShotSceneEffect> (info.followEffectList.Count);

			_mGo = this.gameObject;
			_mId = _mGo.GetInstanceID();
			_mTrans = this.transform;
            _gridMapAgent = _mGo.GetMissingComponent<GridMapAgent>();
            _gridMapAgent.canSearch = true;
            _gridMapAgent.canMove = true;
            _gridMapAgent.speed = ModelHelper.DefaultModelSpeed;
            _gridMapAgent.rotationSpeed = 10;
            _gridMapAgent.interpolatePathSwitches = false;
            //_agent = _mGo.GetMissingComponent<NavMeshAgent> ();
            //_agent.radius = 0.4f;
            //_agent.speed = ModelHelper.DefaultModelSpeed;
            //_agent.acceleration = 1000;
            //_agent.angularSpeed = 1000;
            //_agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            //加载角色模型
            if (info.isHero) {
				info.name = ModelManager.Player.GetPlayerName ();

				PlayerDto playerDto = ModelManager.Player.GetPlayer ();
                _characterInfo.modelId = playerDto.charactor.modelId;
                GameDebuger.TODO(@"_characterInfo.mutateColor = PlayerModel.GetDyeColorParams (playerDto.dressInfoDto);
                if (isNewBiePlot) {
                    _characterInfo.wpModel = NewBieGuideManager.GetNewBieWeapon (playerDto.charactorId);
                } else {
                    _characterInfo.wpModel = ModelManager.Backpack.GetCurrentWeaponModel ();
                    _characterInfo.hallowSpriteId = ModelManager.Backpack.GetCurrentHallowSpriteId();
                }");
			} 

			ModelStyleInfo modelStyleInfo = null;
			if (info.isHero) {
				ScenePlayerDto ScenePlayerDto = WorldManager.Instance.GetModel().GetPlayerDto(ModelManager.Player.GetPlayerId());
				if (ScenePlayerDto == null)
				{
					modelStyleInfo = ModelStyleInfo.ToInfo(_characterInfo);
				}
				else
				{
					modelStyleInfo = ModelStyleInfo.ToInfo(ScenePlayerDto);
				}
			}
			else
			{
				modelStyleInfo = ModelStyleInfo.ToInfo(_characterInfo);
			}

			modelStyleInfo.SetupFashionIds(null);

			_modelDisplayer = new ModelDisplayer(_mGo, OnLoadModelFinish);
			_modelDisplayer.SetLookInfo(modelStyleInfo);
            //设置动作指令播放序列
            _sequence = DOTween.Sequence ();
			_sequence.AppendInterval (_characterInfo.endTime - _characterInfo.startTime).OnComplete (Dispose);

            Action<PlotTransformAction> doTransAction = action =>
            {
                _sequence.InsertCallback(action.startTime, () => {
                    PlayTransformAction(action);
                });
            };
            for (int i = 0; i < _characterInfo.tweenActionList.Count; ++i) {
				PlotTransformAction action = _characterInfo.tweenActionList [i];
				if (action.active)
				{
				    doTransAction(action);
				}
			}

            Action<PlotTalkAction> doTalkAction = action =>
            {
                _sequence.InsertCallback(action.startTime, () => {
                    PlayTalkAction(action);
                });
            };
            for (int i = 0; i < _characterInfo.talkActionList.Count; ++i) {
				PlotTalkAction action = _characterInfo.talkActionList [i];
				if (action.active)
				{
				    doTalkAction(action);
				}
			}

            Action<PlotFollowEffectAction> doEffectAction = action =>
            {
                _sequence.InsertCallback(action.startTime, () => {
                    PlayFollowEffectAction(action);
                });
            };
            for (int i = 0; i < _characterInfo.followEffectList.Count; ++i) {
				PlotFollowEffectAction action = _characterInfo.followEffectList [i];
				if (action.active && !string.IsNullOrEmpty (action.effPath))
				{
				    doEffectAction(action);
				}
			}

            Action<PlotAnimationAction> doAnimationAction = action =>
            {
                _sequence.InsertCallback(action.startTime, () => {
                    PlayAnimationAction(action);
                });
            };
            for (int i = 0; i < _characterInfo.animationActionList.Count; ++i) {
				PlotAnimationAction action = _characterInfo.animationActionList [i];
				if (action.active && !string.IsNullOrEmpty (action.clip))
				{
				    doAnimationAction(action);
				}
			}
		}

		private void OnLoadModelFinish ()
		{
			if (!string.IsNullOrEmpty (_characterInfo.defaultAnim))
			{
				_modelDisplayer.PlayAnimation(EnumParserHelper.TryParse(
					_characterInfo.defaultAnim
					, ModelHelper.AnimType.invalid));
			}
            var mountShadow = _modelDisplayer.GetMountingPoint(ModelHelper.Mount_shadow);
			if (mountShadow == null) return;
			//初始化人物名称
			if (_titleHUDView == null)
			{
				var hudPrefab = ResourcePoolManager.Instance.LoadUI("ModelTitleHUDView") as GameObject;
				var hudGo = NGUITools.AddChild(LayerManager.Root.PlotUIHUDPanel.cachedGameObject, hudPrefab);
				hudGo.name = "PlotHUDView_" + _characterInfo.name;
				_titleHUDView = BaseView.Create<ModelTitleHUDView>(hudGo.transform);

				_titleHUDView.nameLbl.text = _characterInfo.name.WrapColor(ColorConstant.Color_Battle_Enemy_Name);
                //手动将他设置为UI层，因为这个Object和场景NPC名字公用
                _titleHUDView.nameLbl.gameObject.layer = LayerMask.NameToLayer("UI"); 

            }

			_titleHUDView.follower.gameCamera = LayerManager.Root.SceneCamera;
			_titleHUDView.follower.uiCamera = LayerManager.Root.UICamera.cachedCamera;
			_titleHUDView.follower.target = mountShadow;
			_titleHUDView.follower.offset = new Vector3(0f, -0.5f, 0f);
			_titleHUDView.follower.disableIfInvisible = false;
		}

		void Update ()
		{
			if (_gridMapAgent.HasPath) {
				//PlayRunAnimation
				if (!_isRunning) {
					_modelDisplayer.PlayAnimation(ModelHelper.AnimType.run, false);
					_isRunning = true;
				}
			} else {
				//PlayIdleAnimation
				if (_isRunning) {
					_modelDisplayer.PlayAnimation(ModelHelper.AnimType.idle, true);
					_isRunning = false;
				}
			}
		}

		private void PlayTransformAction (PlotTransformAction action)
		{
			if (action.tweenType == PlotTransformAction.TweenType.NavMove) {
				Vector3 dest = action.endValue;
				dest.y = _mTrans.position.y;
                _gridMapAgent.speed = _gridMapAgent.speed != 0 ? action.speed : ModelHelper.DefaultModelSpeed;
                _gridMapAgent.SearchPath(dest);
            } else if (action.tweenType == PlotTransformAction.TweenType.Rotate) {
				_mTrans.DOLocalRotate (action.endValue, action.duration);
			} else if (action.tweenType == PlotTransformAction.TweenType.Scale) {
				_mTrans.DOScale (action.endValue, action.duration);
			} else if (action.tweenType == PlotTransformAction.TweenType.PosMove) {
				_modelDisplayer.DOLocalMove(action.endValue, action.duration);
                _modelDisplayer.PlayAnimation(ModelHelper.AnimType.run,false);
            }
		}

		private void PlayTalkAction (PlotTalkAction talkAction)
		{
            if(talkAction.duration > 0f)
            {
                ProxyActorPopoModule.Open(_mId,_modelDisplayer.GetMountingPoint(ModelHelper.Mount_shadow),talkAction.content,LayerManager.Root.SceneCamera,talkAction.offsetY,talkAction.duration);
            }
            else
            {
                ProxyActorPopoModule.Open(_mId,_modelDisplayer.GetMountingPoint(ModelHelper.Mount_shadow),talkAction.content,LayerManager.Root.SceneCamera,talkAction.offsetY);
            }
            GameDebuger.TODO(@"if (talkAction.duration > 0f) {
                ProxyManager.ActorPopo.Open(_mId, _mTrans, talkAction.content, LayerManager.Root.SceneCamera, talkAction.offsetY, talkAction.duration);
            } else {
                ProxyManager.ActorPopo.Open(_mId, _mTrans, talkAction.content, LayerManager.Root.SceneCamera, talkAction.offsetY);
            }            
");
		}

		private void PlayFollowEffectAction (PlotFollowEffectAction effectAction)
		{
			OneShotSceneEffect effect = OneShotSceneEffect.BeginFollowEffect (effectAction.effPath, _mTrans, effectAction.duration);
			_effectList.Add (effect);
		}

		private void PlayAnimationAction (PlotAnimationAction animationAction)
		{
			_modelDisplayer.PlayAnimation(EnumParserHelper.TryParse(animationAction.clip, ModelHelper.AnimType.invalid));
		}

		public void Dispose ()
		{
            if(_gridMapAgent != null)
            {
                _gridMapAgent.Clear();
                _gridMapAgent = null;
            }
            _sequence.Kill ();
			_sequence = null;

			if (_effectList.Count > 0) {
				for (int i = 0; i < _effectList.Count; ++i) {
					if (_effectList [i] != null)
						_effectList [i].Dispose ();
				}
			}

            GameDebuger.TODO("ProxyManager.ActorPopo.Close(_mId);");

		    if (_titleHUDView != null)
		    {
				GameObject.Destroy (_titleHUDView.gameObject);
		        _titleHUDView = null;
		    }

			_modelDisplayer.Destory();
			_modelDisplayer = null;
			
			GameObject.Destroy (_mGo);
		    _mGo = null;
		}
	}
}