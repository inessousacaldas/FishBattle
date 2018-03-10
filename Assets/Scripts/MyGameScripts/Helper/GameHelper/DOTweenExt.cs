using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public static class DOTweenExt
{
    /// <summary>
    /// 常规DOTween.DOMove更新时是在自身position基础上进行增减的。当自身有多个位移时，位移间会冲突。</p>
    /// 本方法是独立于自身position，根据起始到终点位置及时间，计算出每单位时间的位移，更新时对自身位置进行目标位移的增减的。可保证每个位移都能正确执行不冲突。</p>
    /// @MarsZ 2017-01-09 15:33:08
    /// </summary>
    /// <param name="pTransform">P transform.</param>
    /// <param name="pTotalPositionToChange">P total position to change.</param>
    /// <param name="pDuration">P duration.</param>
    /// <param name="pOnComplete">P on complete.</param>
    public static TweenerCore<Vector3,Vector3,VectorOptions> DOMove2(this Transform pTransform, Vector3 pTotalPositionToChange, float pDuration, TweenCallback pOnComplete, bool pIsLocalPosition = false,Ease pEase = Ease.Linear)
    {
        if (null == pTransform)
            return null;
        //        GameDebuger.LogError(string.Format("DOMoveIngoreCurPosition  pTransform:{0}, from:{1} pTotalPositionToChange:{2},pDuration:{3},pOnComplete:{4}",
        //            pTransform,pTransform.position, pTotalPositionToChange,pDuration,pOnComplete));
        Vector3 tBeginPosition = Vector3.zero;
        Vector3 tPositionNeedMove = pTotalPositionToChange;
        Vector3 tPositionOffSet = Vector3.zero;
        return DOTween.To(() => tBeginPosition, (x) =>
            {
                if (null != pTransform)
                {
                    tPositionOffSet = x - tBeginPosition;
                    if (pIsLocalPosition)
                        pTransform.localPosition += tPositionOffSet;
                    else
                        pTransform.position += tPositionOffSet;
                }
                tBeginPosition = x;
            }, tPositionNeedMove, pDuration).SetEase(pEase)
                .OnComplete(() =>
            {
                if (null == pTransform)
                    return;
                if (null != pOnComplete)
                    pOnComplete();
            });
    }
}