using System;
using UnityEngine;
using DG.Tweening;

namespace GamePlot
{
   public class PlotCameraController : MonoBehaviour
    {
		private CameraEntity _cameraInfo;
		private Transform _cameraTrans;
       // private Transform _cameraRotationTra;
//		private List<CameraPathAnimator> _cameraAnimatorList;
        private CameraPathAnimator _curPathAnimator;
		private Sequence _sequence;
        //用来记录结束剧情后还原角度
        private Vector3 mOriginRotation;


        public void Setup (CameraEntity cameraInfo)
        {
            _cameraInfo = cameraInfo;
            //现在统一用同一个镜头对象做移动效果（跟随，剧情等）
            _cameraTrans = LayerManager.Root.SceneCamera.transform;
            //_cameraTrans = LayerManager.Root.SceneDefaultCameraTrans;
            //_cameraRotationTra = LayerManager.Root.SceneRotationCntr_Transform;

                //--------------因为主场景相机还在修改移动方法，暂先处理，等主场景相机功能完善再次修改
                //LayerManager.Root.SceneCamera.transform.localPosition = CameraConst.WorldCameraLocalPosition;
                //LayerManager.Root.SceneCamera.transform.localEulerAngles = CameraConst.WorldCameraLocalEulerAngles;
                //--------------End -----------
            _cameraTrans.localEulerAngles = _cameraInfo.originRotation;
            mOriginRotation = _cameraInfo.originRotation;
            _cameraTrans.localPosition = _cameraInfo.originPos;
            if(!LayerManager.Root.SceneCamera.gameObject.activeSelf)
                LayerManager.Root.SceneCamera.gameObject.SetActive(true);
            //			GameObject cameraGo = NGUITools.AddChild(this.gameObject);
            //			cameraGo.name = "PlotCamera";
            //			Camera camera = cameraGo.AddComponent<Camera>();
            //			_cameraTrans = cameraGo.transform;
            //预加载所有CameraPath数据
            //			_cameraAnimatorList = new List<CameraPathAnimator> (cameraInfo.camPathActionList.Count);
            _sequence = DOTween.Sequence ();
			_sequence.AppendInterval (_cameraInfo.endTime - _cameraInfo.startTime).OnComplete (Dispose);

            Action<CameraPathAction, GameObject> doAnimationAction = (action, go) =>
            {
                CameraPathAnimator animator = go.GetComponent<CameraPathAnimator>();
                animator.playOnStart = false;
                animator.AnimationObject = _cameraTrans;
                //					_cameraAnimatorList.Add (animator);

                _sequence.InsertCallback(action.startTime, () => {
                    if (_curPathAnimator != null)
                    {
                        _curPathAnimator.Stop();
                    }
                    _curPathAnimator = animator;
                    _curPathAnimator.Play();
                });
            };
            for (int i=0; i<_cameraInfo.camPathActionList.Count; ++i) {
				CameraPathAction action = cameraInfo.camPathActionList [i];
				if(action.active){
					string path = string.Format ("PlotCameraPath/{0}", action.prefabName);
					GameObject prefab = Resources.Load (path) as GameObject;
					if(prefab == null) continue;
					GameObject cameraPathGo = GameObject.Instantiate(prefab) as GameObject;
					cameraPathGo.transform.parent = this.transform;

				    doAnimationAction(action, cameraPathGo);
				}
			}

            Action<CameraShakeAction> doShakeAction = action =>
            {
                _sequence.InsertCallback(action.startTime, () => {
                    PlayShakeAction(action);
                });
            };
            for (int i=0; i<_cameraInfo.shakeActionList.Count; ++i) {
				CameraShakeAction action = _cameraInfo.shakeActionList [i];
				if(action.active)
				{
				    doShakeAction(action);
				}
			}

            Action<PlotTransformAction> doTransAction = action =>
            {
                _sequence.InsertCallback(action.startTime, () => {
                    PlayTransformAction(action);
                });
            };
            for (int i=0; i<_cameraInfo.tweenActionList.Count; ++i) {
				PlotTransformAction action = _cameraInfo.tweenActionList [i];
				if(action.active)
				{
				    doTransAction(action);
				}
			}
		}

		private void PlayShakeAction (CameraShakeAction shakeAction)
		{
			_cameraTrans.parent.DOShakePosition (shakeAction.duration, shakeAction.strength, shakeAction.vibrato, shakeAction.randomness)
				.OnComplete (() => {
					_cameraTrans.parent.localPosition = Vector3.zero;
				});
		}

		private void PlayTransformAction (PlotTransformAction action)
		{
			if (action.tweenType == PlotTransformAction.TweenType.PosMove) {
				_cameraTrans.DOLocalMove (action.endValue, action.duration);
			} else if (action.tweenType == PlotTransformAction.TweenType.Rotate) {
                _cameraTrans.DOLocalRotate (action.endValue, action.duration);
			}
		}

		public  void Dispose ()
		{
			_cameraTrans.parent.localPosition = Vector3.zero;
            _cameraTrans.localEulerAngles = Vector3.zero;
            _sequence.Kill ();
			_sequence = null;
			GameObject.Destroy (this.gameObject);
        }
	}
}
