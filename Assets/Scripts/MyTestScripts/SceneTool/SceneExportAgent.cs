#if UNITY_EDITOR
using UnityEngine;
namespace SceneTool
{
    [AddComponentMenu("美术/SceneExportAgent")]
    public class SceneExportAgent:MonoBehaviour
    {
        public LayerConfig[] layerConfigs;
    }
    [System.Serializable]
    public class LayerConfig
    {
//        public SceneLayer_Framework layer = SceneLayer_Framework.Default;
        public GameObject[] prefabs;
    }
    
}

#endif