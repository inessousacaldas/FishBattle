using UnityEngine;
using DG.Tweening;
using GamePlot;

public class ScreenMaskManager{
    public const string MASK_VIEW = "ScreenMask";
    private static ScreenMaskViewController _instance;
    public static ScreenMaskViewController Instance {
        get {
            if(_instance == null){
                GameObject modulePrefab = AssetPipeline.ResourcePoolManager.Instance.LoadUI( MASK_VIEW ) as GameObject;
                GameObject moduleGo = NGUITools.AddChild(LayerManager.Root.UIModuleRoot,modulePrefab);
                moduleGo.GetMissingComponent<UIPanel>();
                var depth = UIModuleManager.Instance.GetCurDepthByLayerType(UILayerType.FadeInOut);
                NGUITools.AdjustDepth(moduleGo,depth);
                _instance = moduleGo.GetMissingComponent<ScreenMaskViewController>();
            }
            return _instance;
        }
    }

    public static void OpenMaskView(ScreenMaskAction maskAction)
    {
        Instance.Open(maskAction);
    }

    /// <summary>
    /// Fade Color --> Clear
    /// </summary>
    /// <param name="onFinish">On finish.</param>
    /// <param name="duration">Duration.</param>
    /// <param name="fadeTime">Fade time.</param>
    public static void FadeIn(TweenCallback onFinish=null,float duration =0f,float fadeTime=0.3f)
    {
        Color color = Color.black;
        Instance.FadeIn(onFinish,duration,fadeTime,color);
    }

    /// <summary>
    /// Fade Clear --> Color
    /// </summary>
    /// <param name="onFinish">On finish.</param>
    /// <param name="duration">Duration.</param>
    /// <param name="fadeTime">Fade time.</param>
    public static void FadeOut(TweenCallback onFinish=null,float duration =0f,float fadeTime=0.3f)
    {
        Color color = Color.black;
        Instance.FadeOut(onFinish,duration,fadeTime,color);
    }

    /// Fade Clear --> Color --> Clear
    /// </summary>
    /// <param name="onFinish">On finish.</param>
    /// <param name="duration">Duration.</param>
    /// <param name="fadeTime">Fade time.</param>
    public static void FadeInOut(TweenCallback onFinish=null,float duration =0.5f,float fadeTime=0.4f)
    {
        Color color = Color.black;
        Instance.FadeInOut(onFinish,duration,fadeTime,color);
    }
}

public class ScreenMaskViewController : MonoViewController<ScreenMask>
{
    private ScreenMaskAction _info;


    #region IViewController implementation
    public void Open(ScreenMaskAction info){
        _info = info;

        //          Debug.LogError("Start"+Time.time);
        Sequence sequence = DOTween.Sequence().SetId("ScreenMask");
        sequence.AppendInterval(info.duration);

        //蒙版文字内容设置
        View.ContentLbl.text = info.content;
        View.ContentLbl.fontSize = info.fontSize;
        View.ContentLbl.cachedGameObject.SetActive(false);
        sequence.InsertCallback(info.msgStartTime,()=>{
            View.ContentLbl.cachedGameObject.SetActive(true);
        });

        sequence.InsertCallback(info.msgEndTime,()=>{
            View.ContentLbl.cachedGameObject.SetActive(false);
        });

        View.MaskSprite.color = info.startColor;
        if(_info.fade){
            //淡入动画
            Tween fadeIn = DOTween.To(()=>View.MaskSprite.color,
                (x)=> View.MaskSprite.color = x,
                info.endColor,
                info.fadeTweenTime);
            fadeIn.SetEase(Ease.Linear);
            sequence.Insert(info.fadeInTime,fadeIn);

            //淡出动画
            Tween fadeOut = DOTween.To(()=>View.MaskSprite.color,
                (x)=> View.MaskSprite.color = x,
                info.startColor,
                info.fadeTweenTime);
            fadeOut.SetEase(Ease.Linear);
            sequence.Insert(info.fadeOutTime,fadeOut);
        }else{
            sequence.InsertCallback(info.fadeInTime,()=>{
               View.MaskSprite.color = info.endColor;
            });

            sequence.InsertCallback(info.fadeOutTime,()=>{
                View.MaskSprite.color = info.startColor;
            });
        }
    }

    //color --> clear
    //duration + fadeTime
    public void FadeIn(TweenCallback onFinish,float duration,float fadeTime,Color color){
        View.MaskSprite.color = color;
        View.ContentLbl.cachedGameObject.SetActive(false);
        //淡出动画
        Tween fadeIn = DOTween.To(()=>View.MaskSprite.color,
            (x)=> View.MaskSprite.color = x,
            Color.clear,
            fadeTime);
        fadeIn.SetId("ScreenMask");
        fadeIn.SetEase(Ease.Linear).SetDelay(duration);
        fadeIn.OnComplete(onFinish);
    }

    //clear --> color --> clear
    //fadeTime + duration + fadeTime
    public void FadeInOut(TweenCallback onFinish,float duration,float fadeTime,Color color){
        Sequence sequence = DOTween.Sequence().SetId("ScreenMask");
        //淡入动画
        Tween fadeIn = DOTween.To(()=>View.MaskSprite.color,
            (x)=> View.MaskSprite.color = x,
            color,
            fadeTime);
        fadeIn.SetEase(Ease.Linear).OnComplete(onFinish);
        sequence.Append(fadeIn);

        sequence.AppendInterval(duration);
        //淡出动画
        Tween fadeOut = DOTween.To(()=>View.MaskSprite.color,
            (x)=> View.MaskSprite.color = x,
            Color.clear,
            fadeTime);
        fadeOut.SetEase(Ease.Linear);
        sequence.Append(fadeOut);
    }

    //clear --> color
    //duration + fadeTime
    public void FadeOut(TweenCallback onFinish,float duration,float fadeTime,Color color){
        //淡入动画
        Tween fadeIn = DOTween.To(()=>View.MaskSprite.color,
            (x)=> View.MaskSprite.color = x,
            color,
            fadeTime);
        fadeIn.SetId("ScreenMask");
        fadeIn.SetEase(Ease.Linear).SetDelay(duration);
        fadeIn.OnComplete(onFinish);
    }


    protected override void AfterInitView ()
    {
        DOTween.Kill("ScreenMask",true);
    }

    #endregion
}
