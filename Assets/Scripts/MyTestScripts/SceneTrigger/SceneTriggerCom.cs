using System;
using UnityEngine;
using SceneTrigger;
namespace SceneTriggerEditor
{
    [ExecuteInEditMode]
    public class SceneTriggerCom : MonoBehaviour
    {
        public TriggerConfigItem configItem;
        [NonSerialized]
        public bool isDirty;
        void Awake()
        {
            if(configItem == null)
                configItem = new TriggerConfigItem();
        }
    }
}