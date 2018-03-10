using UnityEngine;

/// <summary>
/// 动作控制器相关扩展工具方法
/// @MarsZ 2017年12月19日15:14:09、
/// </summary>
public static class AnimatorExt
{
    /**获取控制器的任意动作的持续时间*/
    public static float GetClipLength(this Animator animator, string clip)
    {
        if (null == animator || string.IsNullOrEmpty(clip))
            return 0F;
        var rac = animator.runtimeAnimatorController;
        if (null == rac || null == rac.animationClips)
            return 0F;
        var tAnimation = rac.animationClips.Find((animation) => animation.name == clip);
        return null == tAnimation ? 0f : tAnimation.length;
    }
}