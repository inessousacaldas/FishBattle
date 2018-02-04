using UnityEngine;
using System;

/**战斗流程控制*/
public class BattleSpecialFlowManager : IDisposable
{
    #region 顺序受击和闪电链

    //顺序受击
    public bool hitInOrder;
    //是否顺序受击的每次受击间间隔时间一定，否则速度一定。
    public bool hitByTime;
    //当前攻击是否闪电链。
    public bool ChainAttack;
    //顺序受击参数，可能为时间，可能是速度。
    public float hitInOrderParam = GameVideoGeneralActionPlayer.DEFAULT_EFFECT_MOVE_SPEED;
    public GameObject ChainEffectTrans;

    #endregion

    #region 连续多段快速打击

    /**连续多段快速打击*/
    private BattleMultiAnimaAttackerCMPT mBattleMultiAnimaAttackerCMPT;

    #endregion

    #region 闪电链

    public void DisposeChainEffect()
    {
        //        GameDebuger.LogError("DisposeChainEffect ChainEffectTrans : " + ChainEffectTrans);
        if (null != ChainEffectTrans)
        {
            EffectTime tEffectTime = ChainEffectTrans.GetComponent<EffectTime>();
            if (null != tEffectTime)
            {
                tEffectTime.Stop();
                tEffectTime = null;
            }
            NGUITools.Destroy(ChainEffectTrans);
            ChainEffectTrans = null;
        }
    }

    #endregion

    #region 连续多段快速打击

    /**连续多段快速打击*/
    public BattleMultiAnimaAttackerCMPT BattleMultiAnimaAttackerCMPT
    {
        get
        {
            if (null == mBattleMultiAnimaAttackerCMPT)
            {
                mBattleMultiAnimaAttackerCMPT = new BattleMultiAnimaAttackerCMPT();
            }
            return mBattleMultiAnimaAttackerCMPT;
        }
    }

    #endregion

    #region 共用

    public void Dispose()
    {
        DisposeChainEffect();

        if (null != mBattleMultiAnimaAttackerCMPT)
        {
            mBattleMultiAnimaAttackerCMPT.Dispose();
            mBattleMultiAnimaAttackerCMPT = null;
        }
    }

    private static BattleSpecialFlowManager mInstance = new BattleSpecialFlowManager();

    public static BattleSpecialFlowManager Instance { get { return mInstance; } }

    private BattleSpecialFlowManager()
    {
    }

    #endregion
}