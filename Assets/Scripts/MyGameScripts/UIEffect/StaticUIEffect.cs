using UnityEngine;

public class StaticUIEffect : MonoBehaviour
{
    private GameObject _effGo;
    private UIWidget _widget;
    private UIEffectRenderQueueSync _renderQSync;

    private bool _disposed;         //标记当前特效已经丢弃,用于加载完特效时判断

    private void Play(string effName, UIWidget widget, bool needClip = false, float scaleFactor = 1.0f, System.Action onLoadFinish = null)
    {
        _widget = widget;

        AssetPipeline.ResourcePoolManager.Instance.SpawnEffectAsync(effName, (inst) =>
        {
            if (_disposed || _widget == null)
            {
                AssetPipeline.ResourcePoolManager.Instance.DespawnEffect(inst);
                return;
            }

            GameObjectExt.AddPoolChild(widget.cachedGameObject, inst);
            _effGo = inst;
            ParticleScaler scaler = _effGo.GetComponent<ParticleScaler>();
            if (scaler != null)
            {
                scaler.SetScale(scaleFactor);
            }

            _renderQSync = _effGo.GetMissingComponent<UIEffectRenderQueueSync>();
            _renderQSync.needClip = needClip;
            _renderQSync.Init();

            if (onLoadFinish != null)
                onLoadFinish();
        });
    }

    public void ReclipEffectRegion()
    {
        if (_renderQSync != null)
            _renderQSync.RecalculateEffectRegion();
    }

    public void Dispose()
    {
        if (_effGo != null)
        {
            AssetPipeline.ResourcePoolManager.Instance.DespawnEffect(_effGo);
            _effGo = null;
        }

        _disposed = true;
        Destroy(this);
    }

    void OnDestroy()
    {
        if (_effGo != null)
            Dispose();
    }

    public static StaticUIEffect Begin(string effName, UIWidget widget, bool needClip = false, float scaleFactor = 1.0f, System.Action onLoadFinish = null)
    {
        if (widget == null || !widget.cachedGameObject.activeInHierarchy)
            return null;
        var controller = widget.cachedGameObject.AddComponent<StaticUIEffect>();
        controller.Play(effName, widget, needClip, scaleFactor, onLoadFinish);

        return controller;
    }
}