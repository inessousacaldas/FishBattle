using UnityEngine;
using DG.Tweening;

public class SurroundUIEffect : MonoBehaviour
{

    private GameObject _effGo;
    private UIWidget _widget;
    private float _tweenTime;
    private Tween _pathTweener;
    private PathType _pathType = PathType.Linear;
    private UIEffectRenderQueueSync _renderQSync;
    private bool _disposed;
    //标记当前特效已经丢弃,用于加载完特效时判断
    private string mEffectName;

    private void Play(string effName, UIWidget widget, float tweenTime = 1.0f, bool needClip = false, PathType pathType = PathType.Linear, float scaleFactor = 1.0f, System.Action onLoadFinish = null)
    {
        _widget = widget;
        _tweenTime = tweenTime;
        _pathType = pathType;
        mEffectName = effName;

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

                ResetPathTween();

                if (onLoadFinish != null)
                    onLoadFinish();
            });
    }

    public string GetEffectName()
    {
        return mEffectName;
    }

    private void ResetPathTween()
    {
        if (_effGo == null)
            return;

        if (_pathTweener != null)
        {
            _pathTweener.Kill();
            _pathTweener = null;
        }

        //初始化环绕动画
        Vector3[] wayPoints = null;
        if (_pathType == PathType.CatmullRom)
        {
            wayPoints = new Vector3[4];
            Transform wt = _widget.cachedTransform;
            float width = _widget.width * 0.8f;
            float height = _widget.height * 0.8f;

            Vector2 offset = _widget.pivotOffset;
            float x0 = -offset.x * width;
            float y0 = -offset.y * height;
            float x1 = x0 + width;
            float y1 = y0 + height;

            wayPoints[0] = new Vector3(x0, y0, -5f);
            wayPoints[1] = new Vector3(x0, y1, -5f);
            wayPoints[2] = new Vector3(x1, y1, -5f);
            wayPoints[3] = new Vector3(x1, y0, -5f);
        }
        else
        {
            wayPoints = _widget.localCorners;
        }

        _effGo.transform.localPosition = wayPoints[0];
        _pathTweener = _effGo.transform.DOLocalPath(wayPoints, _tweenTime, _pathType)
            .SetOptions(true)
            .SetEase(Ease.Linear)
            .SetLoops(-1);
    }

    public void Dispose()
    {
        if (_pathTweener != null)
        {
            _pathTweener.Kill();
            _pathTweener = null;
        }

        if (_effGo != null)
        {
            AssetPipeline.ResourcePoolManager.Instance.DespawnEffect(_effGo);
            _effGo = null;
        }

        mEffectName = string.Empty;
        _disposed = true;
        Destroy(this);
    }

    void OnDestroy()
    {
        if (_effGo != null)
            Dispose();
    }

    public void ReclipEffectRegion()
    {
        if (_renderQSync != null)
            _renderQSync.RecalculateEffectRegion();
    }

    public void SetActive(bool active)
    {
        if (_effGo != null)
        {
            _effGo.SetActive(active);
        }
    }

    public static SurroundUIEffect Begin(string effName, UIWidget widget, float tweenTime = 1.0f, bool needClip = false, PathType pathType = PathType.Linear, float scaleFactor = 1.0f, System.Action onLoadFinish = null)
    {
        if (widget == null || !widget.cachedGameObject.activeInHierarchy)
            return null;
        var controller = widget.cachedGameObject.AddComponent<SurroundUIEffect>();
        controller.Play(effName, widget, tweenTime, needClip, pathType, scaleFactor, onLoadFinish);

        return controller;
    }
}
