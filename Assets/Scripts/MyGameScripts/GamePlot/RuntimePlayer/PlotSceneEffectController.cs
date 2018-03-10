using AssetPipeline;
using UnityEngine;
using DG.Tweening;

namespace GamePlot
{
    public class PlotSceneEffectController : MonoBehaviour
    {
        private SceneEffectEntity _effectInfo;

        private GameObject _effGo;
        private Sequence _sequence;

        public void Setup(SceneEffectEntity effectInfo)
        {
            _effectInfo = effectInfo;

            ResourcePoolManager.Instance.SpawnEffectAsync(effectInfo.effPath, OnLoadFinish);

            _sequence = DOTween.Sequence();
            _sequence.AppendInterval(effectInfo.endTime - effectInfo.startTime).OnComplete(Dispose);
        }

        private void OnLoadFinish(UnityEngine.Object instance)
        {
            GameObject _effGo = instance as GameObject;
            if (_effGo == null)
                return;

            if (_effectInfo.loop)
            {
                ParticleSystem[] particleSystems = _effGo.GetComponentsInChildren<ParticleSystem>();
                if (particleSystems.Length > 0)
                {
                    for (int i = 0; i < particleSystems.Length; i++)
                    {
                        particleSystems[i].loop = true;
                    }
                }
            }

            GameObjectExt.AddPoolChild(this.gameObject, _effGo);
            _effGo.transform.position = _effectInfo.originPos;

            if (_effectInfo.rotate)
            {
                _effGo.transform.DOLocalRotate(_effectInfo.rotateValue, 0f);
            }
        }

        public void Dispose()
        {
            _sequence.Kill();
            _sequence = null;

            if (_effGo != null)
            {
                ResourcePoolManager.Instance.DespawnEffect(_effGo);
                _effGo = null;
            }

            GameObject.Destroy(this.gameObject);
        }
    }
}