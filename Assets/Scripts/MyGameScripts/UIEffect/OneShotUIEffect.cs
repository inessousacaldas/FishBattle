using UnityEngine;

public class OneShotUIEffect : MonoBehaviour
{
    private GameObject _effGo;
    private UIEffectRenderQueueSync _renderQSync;
    private bool _disposed;         //标记当前特效已经丢弃,用于加载完特效时判断

    private void PlayerOneShot(string effName, Vector3 worldPos, float effTime = 1f, float scaleFactor = 1f, System.Action onLoadFinish = null)
    {
        AssetPipeline.ResourcePoolManager.Instance.SpawnEffectAsync(effName,
            (inst) =>
            {
                if (_disposed)
                {
                    AssetPipeline.ResourcePoolManager.Instance.DespawnEffect(inst);
                    return;
                }

                GameObjectExt.AddPoolChild(this.gameObject, inst);
                _effGo = inst;
                _effGo.transform.position = new Vector3(worldPos.x, worldPos.y, _effGo.transform.position.z);

                ParticleScaler scaler = _effGo.GetComponent<ParticleScaler>();
                if (scaler != null)
                {
                    scaler.SetScale(scaleFactor);
                }

                _renderQSync = _effGo.GetMissingComponent<UIEffectRenderQueueSync>();
                _renderQSync.Init();

                if (onLoadFinish != null)
                    onLoadFinish();

                JSTimer.Instance.SetupCoolDown(string.Format("OneShotUIEffect_{0}", _effGo.GetInstanceID()), effTime, null, Dispose);
            });
    }

    public static OneShotUIEffect Begin(string effName, Vector3 worldPos, float effTime = 1f, float scaleFactor = 1f, System.Action onLoadFinish = null)
    {
        OneShotUIEffect controller = LayerManager.Root.EffectsAnchor.AddComponent<OneShotUIEffect>();
        controller.PlayerOneShot(effName, worldPos, effTime, scaleFactor, onLoadFinish);

        return controller;
    }

    #region PlayFollowEffect

    private void PlayFollowEffect(string effName, UIWidget widget, Vector2 offsetVector,
                                   int additiveQueue = 1, bool needClip = false, float scaleFactor = 1.0f, System.Action onLoadFinish = null)
    {
        AssetPipeline.ResourcePoolManager.Instance.SpawnEffectAsync(effName, (inst) =>
        {
            if (_disposed || widget == null)
            {
                AssetPipeline.ResourcePoolManager.Instance.DespawnEffect(inst);
                return;
            }

            GameObjectExt.AddPoolChild(widget.cachedGameObject, inst);

            _effGo = inst;
            _effGo.transform.localPosition += new Vector3(offsetVector.x, offsetVector.y, 0);

            ParticleScaler scaler = _effGo.GetComponent<ParticleScaler>();
            if (scaler != null)
            {
                scaler.SetScale(scaleFactor);
            }

            _renderQSync = _effGo.GetMissingComponent<UIEffectRenderQueueSync>();
            _renderQSync.additiveQueue = additiveQueue;
            _renderQSync.needClip = needClip;
            _renderQSync.Init();

            if (onLoadFinish != null)
                onLoadFinish();
        });
    }

    public void Play()
    {
        if (_effGo != null)
        {
            ParticleSystem pt = _effGo.GetComponent<ParticleSystem>();
            if (pt != null)
                pt.Play(true);
        }
    }

    public void ReclipEffectRegion()
    {
        if (_renderQSync != null)
            _renderQSync.RecalculateEffectRegion();
    }

    public void SetActive(bool active)
    {
        if (_effGo != null)
            _effGo.SetActive(active);
    }

    public static OneShotUIEffect BeginFollowEffect(string effName, UIWidget widget, Vector2 offsetVector, int additiveQueue = 1, bool needClip = false, float scaleFactor = 1.0f, System.Action onLoadFinish = null)
    {
        if (widget == null)
            return null;
        OneShotUIEffect controller = LayerManager.Root.EffectsAnchor.AddComponent<OneShotUIEffect>();
        controller.PlayFollowEffect(effName, widget, offsetVector, additiveQueue, needClip, scaleFactor, onLoadFinish);

        return controller;
    }

    #endregion

    public void Dispose()
    {
        if (_effGo != null)
            AssetPipeline.ResourcePoolManager.Instance.DespawnEffect(_effGo);

        _disposed = true;
        Destroy(this);
    }
}
