using System;
using GamePlayer;
using UnityEngine;

namespace SceneTrigger
{
    public class SceneLayerTrigger : TriggerBase
    {
        public class Param
        {
//#if UNITY_EDITOR
//            [EnumMask(typeof(SceneLayer_Framework))]
//#endif
            public int layerMask;
        }

        private Param param;
        private int oldLayerMask;
        public SceneLayerTrigger(string paramJson) : base(paramJson)
        {
            param = JsHelper.ToObject<Param>(paramJson);
        }

        public override void OnActive()
        {
            oldLayerMask = MySceneManager.Instance.sceneGoManager.GetLayer();
            MySceneManager.Instance.sceneGoManager.SetLayer(param.layerMask);
        }

        public override void OnDeactive()
        {
            MySceneManager.Instance.sceneGoManager.SetLayer(oldLayerMask);
        }

        public override void Dispose()
        {
            MySceneManager.Instance.sceneGoManager.SetLayer(oldLayerMask);
        }
    }
}
