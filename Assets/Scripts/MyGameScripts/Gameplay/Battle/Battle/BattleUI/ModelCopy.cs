using System;
using UnityEngine;

/// <summary>
/// 模型拷贝，虚影之类。
/// @MarsZ 2017-03-20 18:45:23
/// </summary>
public class ModelCopy:IDisposable
{
    private GameObject mRealityModel;

    public ModelCopy(GameObject pRealityModel)
    {
        mRealityModel = pRealityModel;
    }

    private void InitializeUI(Transform pPrefab)
    {
        mCopy.parent = pPrefab.parent;
        mCopy.localPosition = pPrefab.localPosition;
        mCopy.localRotation = pPrefab.localRotation;
        mCopy.localScale = pPrefab.localScale;
        PlayBattleAnimator();
    }

    private void PlayBattleAnimator()
    {
        var animator = mCopy.GetComponentInChildren<Animator>();
        if (animator != null)
            ModelHelper.PlayAnimation(animator, ModelHelper.AnimType.battle, false, null, false, ModelHelper.Animator_Layer_BattleLayer);
    }

    public void ShowCopy()
    {
        Copy.gameObject.SetActive(true);
    }

    public void HideCopy()
    {
        Copy.gameObject.SetActive(false);
    }

    public void Dispose()
    {
        NGUITools.Destroy(mCopy);
        mCopy = null;
        mRealityModel = null;
    }


    private Transform mCopy;

    public Transform Copy
    {
        get
        {
            if (null == mCopy)
            {
                mCopy = NGUITools.AddChild(mRealityModel.transform.parent.gameObject, mRealityModel).transform;
                InitializeUI(mRealityModel.transform);
            }
            return mCopy;
        }
    }
}