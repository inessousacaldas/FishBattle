using UnityEngine;

namespace SceneTrigger
{

    public class CirleRegionCondition : ConditionBase
    {
        public class Param
        {
            [HideInEditor]
            public Vector3 center;
            [HideInEditor]
            public float radius;
            public float height;
        }
        private Param param;
        public CirleRegionCondition(string paramJson)
            : base(paramJson)
        {
            param = JsHelper.ToObject<Param>(paramJson);
        }

        public override bool Update()
        {
            if (WorldManager.Instance == null)
                return false;
            Vector3 heroWorldPos = WorldManager.Instance.GetHeroWorldPos();
            return Vector3.Distance(heroWorldPos, param.center) <= param.radius && (Mathf.Abs(heroWorldPos.y - param.center.y) <= param.height);
        }

        public override void Dispose()
        {
            
        }
    }

}
