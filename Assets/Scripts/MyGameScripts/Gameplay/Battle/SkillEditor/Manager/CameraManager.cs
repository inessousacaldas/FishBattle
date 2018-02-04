using System;
using UnityEngine;

namespace SkillEditor
{
    public class CameraManager
    {
        private static CameraManager mInstance = new CameraManager ();

        public static CameraManager Instance { get { return mInstance; } }

        private CameraManager ()
        {
        }


		private UICamera mUICamera;
        private int mCameraCullingMask;



        #region 摄像机操作

        public void ShowGameUI ()
        {
            LayerManager.Instance.SwitchLayerMode (UIMode.GAME);
			mUICamera.eventReceiverMask = mCameraCullingMask;
        }

        public void ShowBattleUI ()
        {
            if (LayerManager.Instance.CurUIMode != UIMode.BATTLE) {
                BattleDataManager.NeedBattleMap = false;
                LayerManager.Instance.SwitchLayerMode (UIMode.BATTLE);
				//不得隐藏UI，因为掉血信息HUD是UI摄像机的；不得调用ignoreAllEvents，因为U3D5.0以下不支持。
				mUICamera = UICamera.FindCameraForLayer (LayerMask.NameToLayer (GameTag.Tag_UI));
				mCameraCullingMask = (int) mUICamera.eventReceiverMask;
				mUICamera.eventReceiverMask = 0;
            }
        }
        #endregion
    }
}

