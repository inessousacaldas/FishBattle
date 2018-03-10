using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 战斗 挂点组件
/// @MarsZ 2017年11月10日11:49:34
/// </summary>
public class BattleMountCMPT :IDisposable
{
    private Dictionary<int,BattleMount> mBattleMountDic;

    public BattleMountCMPT()
    {
        mBattleMountDic = new Dictionary<int, BattleMount>();
    }


    /**挂点跟随动作移动*/
    public void SetMountMoveWithAnimation(Transform mountTrans, bool moveWithAnimation, float duration)
    {
        BattleMount tBattleMount = GetMount(mountTrans);
        if (moveWithAnimation)
            tBattleMount.MoveWithTrans(duration);
        else
            tBattleMount.ResetLocalPosition();
    }

    public void Dispose()
    {
        if (null != mBattleMountDic)
        {
            mBattleMountDic.Values.ForEach((battleMount) =>
                {
                    battleMount.Dispose();
                    battleMount = null;
                });
            mBattleMountDic.Clear();
        }
    }

    private BattleMount GetMount(Transform mountTrans)
    {
        BattleMount tBattleMount = null;
        int tMountInstanceID = mountTrans.GetInstanceID();
        if (!mBattleMountDic.TryGetValue(tMountInstanceID, out tBattleMount))
        {
            tBattleMount = new BattleMount(mountTrans);
            mBattleMountDic.Add(tMountInstanceID, tBattleMount);
        }
        return tBattleMount;
    }

    public class BattleMount :IDisposable
    {
        private const string NAME_POSITION_UPDATER = "NAME_POSITION_UPDATER_{0}";

        /**原锚点，必要时要跟随一个参考部件移动*/
        private Transform mMountTrans;

        private string mTimerName;
        private JSTimer.CdTask mJSTimer;
        /**以之为目标跟随移动的参考部件*/
        private Transform mReferTrams;
        /**挂点的初始本地坐标*/
        private Vector3 mOriginLocalPosition;
        /**初始时挂点与参考点的距离*/
        private Vector3 mOffsetWorldPosition;

        public BattleMount(Transform mountTrans)
        {
            mMountTrans = mountTrans;
            Initialize();
        }

        private void Initialize()
        {
            mReferTrams = mMountTrans.parent.Find(ModelHelper.ANCHOR_TOP_BONE);
            mTimerName = string.Format(NAME_POSITION_UPDATER, mMountTrans.GetInstanceID());
            mOriginLocalPosition = mMountTrans.localPosition;
            mOffsetWorldPosition = null != mReferTrams ? (mReferTrams.position - mMountTrans.position) : Vector3.zero;
        }

        /**还原挂点本地坐标*/
        public void ResetLocalPosition()
        {
            if (null == mMountTrans)
                return;
            mMountTrans.localPosition = mOriginLocalPosition;
        }

        public void MoveWithTrans(float duration)
        {
            duration = duration <= 0F ? float.MaxValue : duration;
            mJSTimer = JSTimer.Instance.SetupCoolDown(mTimerName, duration, UpdatePositionToTarget, () =>
                {
                    Dispose();
                },0.01F);
        }

        private void UpdatePositionToTarget(float remainTime)
        {
            if (null == mReferTrams || null == mMountTrans)
            {
                Dispose();
                return;
            }
            mMountTrans.position = mReferTrams.position - mOffsetWorldPosition;
        }

        public void Dispose()
        {
            ResetLocalPosition();

            JSTimer.Instance.CancelTimer(mTimerName);
            if (null != mJSTimer)
            {
                mJSTimer.Dispose();
                mJSTimer = null;
            }
        }
    }
}