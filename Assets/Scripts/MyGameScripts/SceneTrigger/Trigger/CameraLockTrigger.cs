using AppDto;
using GamePlayer;
    
namespace SceneTrigger
{
    public class CameraLockTrigger : TriggerBase
    {
        public class Param
        {
            [Rename("旋转角度")]
            public float rotation;
        }

        private Param param;

        public CameraLockTrigger(string paramJson)
            : base(paramJson)
        {
            param = JsHelper.ToObject<Param>(paramJson);
        }

        public override void OnActive()
        {
            GamePlayer.CameraManager.Instance.SceneCamera.SceneCameraController.ChangeAction(CameraActionKey.Normal_Lock);
        }

        public override void OnDeactive()
        {
            GamePlayer.CameraManager.Instance.SceneCamera.SceneCameraController.ChangeAction(CameraActionKey.Normal);
        }

        public override void Dispose()
        {
            GamePlayer.CameraManager.Instance.SceneCamera.SceneCameraController.ChangeAction(CameraActionKey.Normal);
        }
    }

}