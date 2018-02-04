using UnityEngine;

namespace SceneTrigger
{

    public class CirleRegionCondition : ConditionBase
    {
        [HideInEditor]
        public class Param
        {
            public Vector3 center;
            public float radius;
        }
        private Param param;
        public CirleRegionCondition(string paramJson)
            : base(paramJson)
        {
            param = JsHelper.ToObject<Param>(paramJson);
        }

        public override bool Update()
        {
            Vector3 heroWorldPos = WorldManager.Instance.GetHeroWorldPos();
            return Vector3.Distance(heroWorldPos, param.center) <= param.radius;
        }

        public override void Dispose()
        {
            
        }
    }

}
