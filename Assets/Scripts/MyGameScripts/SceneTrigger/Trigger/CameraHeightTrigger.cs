using GamePlayer;

namespace SceneTrigger
{
    public class CameraHeightTrigger : TriggerBase
    {
        public class Param
        {
            [Rename("高度"), Slider(-15, 15)]
            public float addHeight;
            [Rename("半径"), Slider(-15, 15)]
            public float addRadius;
        }

        private Param param;
        private CameraHightDecorator cameraHightDecorator;

        public CameraHeightTrigger(string paramJson)
            : base(paramJson)
        {
            param = JsHelper.ToObject<Param>(paramJson);
        }

        public override void OnActive()
        {
            cameraHightDecorator = new CameraHightDecorator(param.addHeight, param.addRadius);
            CameraManager.Instance.SceneCamera.SceneCameraController.AddDecorator(cameraHightDecorator);
        }

        public override void OnDeactive()
        {
            CameraManager.Instance.SceneCamera.SceneCameraController.RemoveDecorator(cameraHightDecorator);
            cameraHightDecorator = null;
        }

        public override void Dispose()
        {
            if(cameraHightDecorator != null)
                CameraManager.Instance.SceneCamera.SceneCameraController.RemoveDecorator(cameraHightDecorator);
            cameraHightDecorator = null;
        }
    }

}