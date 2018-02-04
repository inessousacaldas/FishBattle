using UnityEngine;

namespace SceneTrigger
{

    public class EnterSceneCondition : ConditionBase
    {
        public EnterSceneCondition(string paramJson)
            : base(paramJson)
        {
        }

        public override bool Update()
        {
            return true;
        }

        public override void Dispose()
        {
            
        }
    }

}
