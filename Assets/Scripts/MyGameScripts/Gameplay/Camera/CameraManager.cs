using System;
using UniRx;
using UnityEngine;

namespace GamePlayer
{
    interface IBattleInfo
    {
        BattleSceneStat CurBattleStat { get; }
        BattleSceneStat LastBattleStat { get; }
    }

    sealed class BattleInfo : IBattleInfo
    {
        public BattleSceneStat curBattleStat;

        public BattleSceneStat CurBattleStat
        {
            get { return curBattleStat; }
        }

        public BattleSceneStat LastBattleStat
        {
            get { return lastBattleStat; }
        }

        public BattleSceneStat lastBattleStat;
    }

    public sealed class BattleCameraController
    {
        public static BattleCameraController Create(Transform cameraPosTrans, GameObject cameraGO, Animator animator)
        {
            try
            {
                var ctrl = new BattleCameraController();
                ctrl.Init(cameraPosTrans, cameraGO, animator);

                return ctrl;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private BattleCameraMove _cameraMove;

        private CameraScaler _cameraScaler;

        public CameraScaler CameraScaler
        {
            get { return _cameraScaler; }
        }

        BattleCameraRotateHelper mBattleCameraRotateHelper;

        private Animator _camAnimator;
        private CompositeDisposable _dispose;

        public Animator CamAnimator
        {
            get { return _camAnimator; }
        }

        private void Init(Transform cameraPosionTrans, GameObject cameraGO, Animator animator)
        {
            _dispose = new CompositeDisposable();
            _cameraMove = cameraGO.GetMissingComponent<BattleCameraMove>();
            _cameraMove.PositionTrans = cameraPosionTrans;
            _cameraMove.CenterPosition = CameraConst.BattleSceneCenter;
            _cameraScaler = cameraGO.GetMissingComponent<CameraScaler>();
            _cameraScaler.MaxFieldOfView = CameraConst.BattleMaxFieldOfView;
            _cameraScaler.MinFieldOfView = CameraConst.BattleMinFieldOfView;
            _camAnimator = animator;

            mBattleCameraRotateHelper = BattleCameraRotateHelper.Create(CameraRotationDirection.Both);

            ResetCamera();

            _dispose.Add(BattleDataManager.Stream.Select(
                    (data, state) =>
                        new BattleInfo
                        {
                            curBattleStat = data.battleState,
                            lastBattleStat = data.lastBattleState
                            // 还有一个攻击对象选择 跃迁
                        } as IBattleInfo)
                .Subscribe(battleInfo => CameraChange(battleInfo)));

            _dispose.Add(LayerManager.Stream.SubscribeAndFire(mode => UpdateByUIMode(LayerManager.Stream.LastValue, mode)));
        }

        private void UpdateByUIMode(UIMode lastMode, UIMode mode)
        {
            if (lastMode == mode) return;
            if (mode == UIMode.BATTLE)
            {
                mBattleCameraRotateHelper.Initialize();
            }
            else
            {
                mBattleCameraRotateHelper.StopCameraEvt();
                ResetCamera();
            }
        }

        private BattleCameraController()
        {

        }

        /**
        订阅战斗数据
        中心点：初始值 场景中心 move reset
        战斗单位：follow
        播放动画：change parent

        切中心点之前，重置旋转
        播放期间禁止旋转

        弹菜单 状态切换 切战斗单位
        选操作目标  就是select的时候 切场景中心
        默认 场景中心
        开打，选技能

        **/

        private void CameraChange(IBattleInfo battleInfo)
        {
            MonsterController mc = null;
            if (battleInfo.CurBattleStat == BattleSceneStat.BATTLE_PlayerOpt_Time
                     && BattleDataManager.DataMgr.BattleDemo.IsCurActMonsterCanbeOpt)
            {
                mc = BattleDataManager.DataMgr.BattleDemo.CurActMonsterController;

                if (mc.GetCurSelectSkill() == null)
                {
                    _cameraMove.Follow(mc.GetMountHit());
                    _cameraScaler.ResetMinCamera();
                }
                else
                {
                    _cameraMove.ResetCoordinate();
                    _cameraScaler.ResetMaxCamera();
                }
            }
            else
            {
                _cameraMove.ResetCoordinate();
                _cameraScaler.ResetMaxCamera();
            }
        }

        public void ResetCamera()
        {
            _cameraMove.Reset();
            mBattleCameraRotateHelper.OnBtnResetRotationClick();

            _cameraScaler.Reset(_cameraScaler.MinFieldOfView);
        }

        public void Dispose()
        {
            mBattleCameraRotateHelper.Dispose();
            mBattleCameraRotateHelper = null;
            _dispose.Dispose();
            _dispose = null;
        }

        public void StopCameraEvt()
        {
            mBattleCameraRotateHelper.StopCameraEvt();
        }

        public void SetPreViewCamera()
        {
            _cameraMove.SetPreViewCamera();
        }
    }

    public sealed class SceneCamera
    {
        private CompositeDisposable _disposable;

        public static SceneCamera Create(GameObject cameraGO)
        {
            try
            {
                var ctrl = new SceneCamera(cameraGO);
                return ctrl;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private Camera _camera;

        public Camera Camera
        {
            get { return _camera; }
        }

        private SceneCameraController _sceneCameraController;
        private CameraScaler mCameraScaler;
        public SceneCameraController SceneCameraController
        {
            get { return _sceneCameraController; }
        }
        // Use this for initialization
        private SceneCamera(GameObject cameraGO)
        {
            _sceneCameraController = cameraGO.GetMissingComponent<SceneCameraController>();

            _camera = cameraGO.FindScript<Camera>("");
            mCameraScaler = cameraGO.GetMissingComponent<CameraScaler>();
            mCameraScaler.MinFieldOfView = CameraConst.WorldMinFieldOfView;
            mCameraScaler.MaxFieldOfView = CameraConst.WorldMaxFieldOfView;

            mCameraScaler.Follow(cameraGO.transform);

            _disposable = new CompositeDisposable();
            _disposable.Add(LayerManager.Stream.SubscribeAndFire(mode => UpdateCamereActive(mode)));
        }
        private void UpdateCamereActive(UIMode mode)
        {
            //根据游戏状态控制摄像机是否启动
            SceneCameraController.ActiveCameraController = (mode == UIMode.GAME);
        }
        public void ChangeMode(UIMode mode)
        {
            //重置摄像机缩放状态
            mCameraScaler.ResetPos();
            UpdateCamereActive(mode);

            mCameraScaler.canScale = mode == UIMode.GAME;
            mCameraScaler.followTarget = mode == UIMode.GAME;
        }

        /// <summary>
        /// Reset场景镜头参数
        /// </summary>
        public void ResetCamera()
        {
            LayerManager.Root.ScenePositionCameraTrans.position = Vector3.zero;
            LayerManager.Root.SceneRotationCntr_Transform.position = Vector3.zero;

            //游戏初始化时，设置MainCamera参数
            mCameraScaler.MinFieldOfView = CameraConst.WorldMinFieldOfView;
            mCameraScaler.MaxFieldOfView = CameraConst.WorldMaxFieldOfView;
            mCameraScaler.Reset(CameraConst.WorldCameraFieldOfView);
            mCameraScaler.Follow(LayerManager.Root.SceneCameraTrans.gameObject.transform);
            _camera.fieldOfView = CameraConst.WorldCameraFieldOfView;

            _sceneCameraController.SetCameraConst(CameraConst.WorldCameraLocalPosition, CameraConst.WorldCameraLocalEulerAngles);

            if (WorldManager.Instance != null && WorldManager.Instance.GetHeroView() != null)
                OnChangeTarget(WorldManager.Instance.GetHeroView().cachedTransform);
        }

        //游戏初始化时，设置MainCamera 默认控制器
        /// <summary>
        /// 设置SceneCamera跟随目标
        /// </summary>
        /// <param name="target"></param>        
        public void OnChangeTarget(Transform target)
        {
            _sceneCameraController.SetTartget(target);
        }
        public void ResetTargetPos(Vector3 playerPos)
        {
            _sceneCameraController.ResetCameraPos(playerPos);
        }

        //锁死缩放，不允许屏幕缩放
        public void LockScale(bool lockScale)
        {
            mCameraScaler.lockScale = lockScale;
        }

        public bool IsChangeCameraScaler()
        {
            return mCameraScaler.isMultiTouch;
        }
        public void Dispose()
        {
            _disposable = _disposable.CloseOnceNull();
        }
    }

    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;

        private SceneCamera _sceneCamera;

        public SceneCamera SceneCamera
        {
            get { return _sceneCamera; }
        }

        private BattleCameraController _battleCameraController;

        public BattleCameraController BattleCameraController
        {
            get { return _battleCameraController; }
        }

        // Use this for initialization
        private void Awake()
        {
            Instance = this;
            _sceneCamera = SceneCamera.Create(LayerManager.Root.SceneCameraTrans.gameObject);
            _sceneCamera.ResetCamera();

            _battleCameraController = BattleCameraController.Create(
                LayerManager.Root.BattleDefaultRotationCntr_Transform, LayerManager.Root.BattleCameraTrans.gameObject
                , LayerManager.Root.BattleCamera_Animator);
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        public void PlayCameraAnimator(int sceneId, int cameraId)
        {
            if (_battleCameraController.CamAnimator == null) return;
            var animatorName = string.Format("camera_{0}_{1}", sceneId, cameraId);
            PlayCameraAnimator(_battleCameraController.CamAnimator, animatorName);
        }

        private void PlayCameraAnimator(Animator animator, string animatorName)
        {
            if (animator == null) return;

            animator.enabled = true;
            animator.Play(animatorName, 0, 0f);
        }

        public void Dispose()
        {
            if (_battleCameraController != null)
                _battleCameraController.Dispose();
            _sceneCamera.Dispose();
        }

    }
}
